using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.PopupLogic;
using ControlSystem.Logic.UserEventsLogic;
using ControlSystem.Structs;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Structs;
using ControlSystem.WinSpectorLogic.Utils;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
using RenderLib;
using RenderLib.Renderers.Dummy;
using SysWinLib;
using SysWinLib.Structs;
using TreePusherLib;
using TreePusherLib.ConvertExts;
using UserEvents;
using UserEvents.Generators;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem;


public class Win : Ctrl, IWinUserEventsSupport
{
	private readonly SysWin sysWin;
	private readonly ISubject<Unit> whenInvalidate;
	private PartitionSet partitionSet = PartitionSet.Empty;
	private IObservable<Unit> WhenInvalidate => whenInvalidate.AsObservable();

	internal SpectorWinDrawState SpectorDrawState { get; }
	internal IRoVar<R> ClientR => sysWin.ClientR;
	internal IRoVar<Pt> ScreenPt => sysWin.ScreenPt;

	// IWinUserEventsSupport
	// =====================
	public IObservable<IUserEvt> Evt { get; }
	public Maybe<INodeStateUserEventsSupport> HitFun(Pt pt) => partitionSet.MainPartition.FindNodeAtMouseCoordinates(pt);

	public override string ToString() => GetType().Name;
	public void Invalidate() => whenInvalidate.OnNext(Unit.Default);
	public nint Handle => sysWin.Handle;


	public Win(Action<WinOpt>? optFun = null)
	{
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
			Invalidate();
		}).D(D);

		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);
		G.WinMan.AddWin(this);

		//Evt.Subscribe(e => L($"[Win] - {e}")).D(D);

		WhenInvalidate
			.Subscribe(_ =>
			{
				sysWin.Invalidate();
				popupMan.InvalidatePopups();
			}).D(D);

		
		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var gfx = renderer.GetGfx().D(d);

			// Layout
			// ======
			if (canSkipLayout.IsNotSet())
			{
				partitionSet = this
					.BuildTree()
					.SolveTree(this, out var mixLayout)
					.SplitPopups()
					.CreatePopups(popupMan)
					.DispatchNodeEvents(eventDispatcher, popupMan.GetWin)
					.Assign_CtrlWins_and_NodeRs(this);

				G.WinMan.SetWinLayout(mixLayout);
			}

			// Render
			// ======
			RenderUtils.RenderTree(partitionSet.MainPartition, gfx);
			SpectorWinRenderUtils.Render(SpectorDrawState, partitionSet.MainPartition, gfx);

		}).D(D);
	}
}


file static class WinUtils
{
	public static ReconstructedTree<IMixNode> BuildTree(this Ctrl rootCtrl)
	{
		using var d = new Disp();
		var gfx = new Dummy_Gfx().D(d);
		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var renderArgs = new RenderArgs(gfx, pusher).D(d);
		return
			treeEvtObs.ToTree(
				mixNode =>
				{
					if (mixNode is not CtrlNode { Ctrl: var ctrl }) return;
					if (ctrl.D.IsDisposed)
						throw new ObjectDisposedException(ctrl.GetType().Name, "Cannot render a disposed Ctrl");
					ctrl.SignalRender(renderArgs);
				},
				() =>
				{
					using (renderArgs.Ctrl(rootCtrl)) { }
				}
			);
	}


	public static MixLayout SolveTree(
		this ReconstructedTree<IMixNode> tree,
		Win win,
		out MixLayout mixLayout
	)
	{
		var mixRoot = tree.Root;

		var stFlexRoot = mixRoot.OfTypeTree<IMixNode, StFlexNode>();
		var flexRoot = stFlexRoot.Map(e => e.Flex);
		var freeSz = FreeSzMaker.FromSz(win.ClientR.V.Size);
		var layout = FlexSolver.Solve(flexRoot, freeSz);

		var flex2st = flexRoot.Zip(stFlexRoot).ToDictionary(e => e.First, e => e.Second.V.State);

		return mixLayout =
			new MixLayout(
				win,
				freeSz,
				mixRoot,
				layout.RMap.MapKeys(flex2st),
				layout.WarningMap.MapKeys(flex2st),

				tree.IncompleteNodes
					.GroupBy(e => e.ParentNod)
					.Select(e => e.First())
					.ToDictionary(
						e => e.ParentNod.FindCtrlContaining(),
						e => e.ChildNode
					),

				mixRoot
					.Where(e => e.V is CtrlNode)
					.ToDictionary(
						e => ((CtrlNode)e.V).Ctrl,
						e => e
					)
			);
	}

	public static PartitionSet SplitPopups(
		this MixLayout layout
	)
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

		return partitionSet;
	}







	private static Ctrl FindCtrlContaining(this MixNode nod)
	{
		while (nod.V is not CtrlNode { Ctrl: not null })
			nod = nod.Parent ?? throw new ArgumentException("Cannot find containing Ctrl");
		return ((CtrlNode)nod.V).Ctrl;
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