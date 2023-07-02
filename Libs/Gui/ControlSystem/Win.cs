using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Popup_;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Logic.UserEvents_;
using ControlSystem.Structs;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Structs;
using ControlSystem.WinSpectorLogic.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using RenderLib;
using SysWinLib;
using SysWinLib.Structs;
using TreePusherLib.ConvertExts.Structs;
using UserEvents;
using UserEvents.Generators;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem;


public class Win : Ctrl, IWinUserEventsSupport
{
	private readonly SysWin sysWin;
	private readonly ISubject<Unit> whenInvalidateAll;
	private readonly ISubject<Unit> whenInvalidate;
	private PartitionSet partitionSet = PartitionSet.Empty;
	private IObservable<Unit> WhenInvalidateAll => whenInvalidateAll.AsObservable();
	private IObservable<Unit> WhenInvalidate => whenInvalidate.AsObservable();

	internal SpectorWinDrawState SpectorDrawState { get; }
	internal IRoVar<R> ClientR => sysWin.ClientR;
	internal IRoVar<Pt> ScreenPt => sysWin.ScreenPt;

	// IWinUserEventsSupport
	// =====================
	public IObservable<IUserEvt> Evt { get; }
	public Maybe<INodeStateUserEventsSupport> HitFun(Pt pt) => partitionSet.MainPartition.FindNodeAtMouseCoordinates(pt);
	public void Invalidate() => whenInvalidate.OnNext(Unit.Default);

	public override string ToString() => GetType().Name;
	public void InvalidateAll() => whenInvalidateAll.OnNext(Unit.Default);
	public nint Handle => sysWin.Handle;


	public Win(Action<WinOpt>? optFun = null)
	{
		whenInvalidateAll = new Subject<Unit>().D(D);
		whenInvalidate = new Subject<Unit>().D(D);
		var opt = WinOpt.Build(optFun);
		sysWin = WinUtils.MakeWin(opt).D(D);
		this.D(sysWin.D);
		Evt = UserEventGenerator.MakeForWin(sysWin);
		SpectorDrawState = new SpectorWinDrawState().D(D);
		var popupMan = new PopupMan(this, sysWin, SpectorDrawState).D(D);
		var eventDispatcher = new WinEventDispatcher().D(D);

		var canSkipLayout = new TimedFlag();
		SpectorDrawState.WhenChanged.Subscribe(_ =>
		{
			canSkipLayout.Set();
			InvalidateAll();
		}).D(D);

		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);
		var scrollMan = new ScrollMan(renderer).D(D);
		G.WinMan.AddWin(this);

		WhenInvalidateAll
			.Subscribe(_ =>
			{
				sysWin.Invalidate();
				popupMan.InvalidatePopups();
			}).D(D);

		WhenInvalidate
			.Subscribe(_ =>
			{
				sysWin.Invalidate();
			}).D(D);

		
		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();

			// Layout
			// ======
			if (canSkipLayout.IsNotSet())
			{
				partitionSet = this
					.BuildCtrlTree(renderer)
					.SolveTree(this, out var mixLayout)
					.SplitPopups()
					.CreatePopups(popupMan)
					.AddScrollBars(scrollMan)
					.ApplyScrollOffsets(mixLayout)
					.DispatchNodeEvents(eventDispatcher, popupMan.GetWin)
					.Assign_CtrlWins_and_NodeRs(this);

				G.WinMan.SetWinLayout(mixLayout);
			}

			// Render
			// ======
			var gfx = renderer.GetGfx(false).D(d);
			RenderUtils.RenderTree(partitionSet.MainPartition, gfx);
			SpectorWinRenderUtils.Render(SpectorDrawState, partitionSet.MainPartition, gfx);

		}).D(D);
	}
}


file static class WinUtils
{
	public static MixLayout SolveTree(
		this ReconstructedTree<IMixNode> tree,
		Win win,
		out MixLayout mixLayout
	)
		=> mixLayout = tree.ResolveCtrlTree(FreeSzMaker.FromSz(win.ClientR.V.Size), win);

	public static PartitionSet AddScrollBars(this PartitionSet partitionSet, ScrollMan scrollMan)
		=> scrollMan.AddScrollBars(partitionSet);
	
	public static PartitionSet SplitPopups(this MixLayout layout)
		=> PopupSplitter.Split(layout.MixRoot, layout.RMap);

	

	public static PartitionSet Assign_CtrlWins_and_NodeRs(this PartitionSet partitionSet, Win win)
	{
		(
			from partition in partitionSet.Partitions
			from ctrl in partition.CtrlSet
			select ctrl
		)
			.ForEach(ctrl => ctrl.WinSrc.V = May.Some(win));

		(
			from partition in partitionSet.Partitions
			from nodeState in partition.AllNodeStates
			select (nodeState, r: partition.RMap[nodeState])
		)
			.ForEach(t => t.nodeState.RSrc.V = t.r);



		(
			from partition in partitionSet.Partitions
			from ctrl in partition.ScrollBars.ControlMap.Values.SelectMany(e => e)
			select ctrl
		)
			.ForEach(ctrl => ctrl.WinSrc.V = May.Some(win));

		(
			from partition in partitionSet.Partitions
			from nodeState in partition.ScrollBars.RMap.Keys
			select (nodeState, r: partition.ScrollBars.RMap[nodeState])
		)
			.ForEach(t => t.nodeState.RSrc.V = t.r);



		return partitionSet;
	}



	




	private const int DEFAULT = (int)CreateWindowFlags.CW_USEDEFAULT;

	public static SysWin MakeWin(WinOpt opt)
	{
		var win = new SysWin(e =>
		{
			e.CreateWindowParams = new CreateWindowParams
			{
				Name = opt.Title,
				X = opt.Pos?.X ?? DEFAULT,
				Y = opt.Pos?.Y ?? DEFAULT,
				Width = opt.Size?.Width ?? DEFAULT,
				Height = opt.Size?.Height ?? DEFAULT,
				Styles = opt.Styles,
				ExStyles = opt.ExStyles,
			};
		});
		win.Init();
		return win;
	}
}