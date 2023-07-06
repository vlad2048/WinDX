using System.Drawing;
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
using PowBasics.ColorCode;
using PowBasics.ColorCode.Structs;
using PowBasics.ColorCode.Utils;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
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
	private readonly ISubject<Unit> whenInvalidate;
	private PartitionSet partitionSet = PartitionSet.Empty;
	private IObservable<Unit> WhenInvalidate => whenInvalidate.AsObservable();

	internal SpectorWinDrawState SpectorDrawState { get; }
	internal IRoVar<R> ClientR => sysWin.ClientR;
	internal IRoVar<Pt> ScreenPt => sysWin.ScreenPt;

	// IWinUserEventsSupport
	// =====================
	public IObservable<IUserEvt> Evt { get; }
	public INodeStateUserEventsSupport[] HitFun(Pt pt) => partitionSet.MainPartition.FindNodesAtMouseCoordinates(pt);
	public void Invalidate() => whenInvalidate.OnNext(Unit.Default);

	public override string ToString() => GetType().Name;
	public nint Handle => sysWin.Handle;
	private int cnt;


	public Win(Action<WinOpt>? optFun = null)
	{
		whenInvalidate = new Subject<Unit>().D(D);
		var opt = WinOpt.Build(optFun);
		sysWin = WinUtils.MakeWin(opt).D(D);
		this.D(sysWin.D);
		// TODO: understand why MakeHot is needed, if removed:
		// - MouseEnter event appears in log here
		// - MouseEnter event does not appear in WinSpector
		Evt = UserEventGenerator.MakeForWin(sysWin).MakeHot(D);
		Evt.Subscribe(e => L($"{e}")).D(D);
		SpectorDrawState = new SpectorWinDrawState().D(D);
		var popupMan = new PopupMan(this, Invalidate, sysWin, SpectorDrawState).D(D);
		var eventDispatcher = new WinEventDispatcher().D(D);

		var canSkipLayout = new TimedFlag();
		SpectorDrawState.WhenChanged.Subscribe(_ =>
		{
			canSkipLayout.Set();
			Invalidate();
		}).D(D);

		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);
		var scrollMan = new ScrollMan(renderer).D(D);
		G.WinMan.AddWin(this);

		WhenInvalidate
			.Subscribe(_ =>
			{
				sysWin.Invalidate();
				popupMan.InvalidatePopups();
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
					.AddScrollBars(scrollMan)
					.ApplyScrollOffsets(mixLayout)
					.CreatePopups(popupMan)
					.DispatchNodeEvents(eventDispatcher, popupMan.GetWin)
					.Assign_CtrlWins_and_NodeRs(this);

				if (cnt++ == 0)
					WinUtils.LogPartitionSet(partitionSet);
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
			from nodeState in partition.NodeStates
			select (nodeState, r: partition.RMap[nodeState])
		)
			.ForEach(t => t.nodeState.RSrc.V = t.r);


		(
			from partition in partitionSet.Partitions
			from ctrls in partition.SysPartition.CtrlTriggers.Values
			from ctrl in ctrls
			select ctrl
		)
			.ForEach(ctrl => ctrl.WinSrc.V = May.Some(win));

		(
			from partition in partitionSet.Partitions
			from t in partition.SysPartition.RMap
			select t
		)
			.ForEach(t => t.Key.RSrc.V = t.Value);

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






	private static class Cols
	{
		public static readonly Color PartitionArrowColor = Color.White;
		public static readonly Color PartitionBoxColor = Color.FromArgb(94, 164, 230);

		public static readonly Color CtrlColorInSet = Color.FromArgb(255, 255, 255);
		public static readonly Color CtrlColorNotInSet = Color.FromArgb(77, 77, 77);
		public static readonly Color CtrlRootBoxColor = Color.FromArgb(196, 69, 190);
		public static readonly Color CtrlExtraBoxColor = Color.FromArgb(116, 168, 219);

		public static readonly Color FlexColorInSet = Color.FromArgb(0xc9, 0x43, 0xd7);
		public static readonly Color FlexColorNotInSet = Color.FromArgb(87, 87, 87);
		public static readonly Color FlexRColor = Color.FromArgb(0xcc, 0xe6, 0x51);

		public static readonly Color TxtMeasureColor = Color.FromArgb(125, 125, 125);
	}




	public static void LogPartitionSet(PartitionSet partitionSet)
	{
		var partitionNames = partitionSet.Partitions
			.Select((p, i) => (p, i switch
			{
				0 => "MainPartition",
				_ => $"SubPartition {i - 1}"
			}))
			.ToDictionary(e => e.p, e => e.Item2);


		void WriteNode(IMixNode node, ITxtWriter writer, Partition partition)
		{
			switch (node)
			{
				case CtrlNode { Ctrl: var ctrl }:
				{
					var isRoot = ctrl == partition.RootCtrl;
					var isExtra = partition.SysPartition.CtrlTriggers.Values.SelectMany(e => e).Any(e => e == ctrl);
					var isInSet = partition.CtrlSet.Contains(ctrl);
					var color = isInSet switch
					{
						truer => Cols.CtrlColorInSet,
						false => Cols.CtrlColorNotInSet,
					};
					var surround = isRoot || isExtra;
					var surroundBoxColor = (isRoot, isExtra) switch
					{
						(false, false) => Color.Black,
						(truer, false) => Cols.CtrlRootBoxColor,
						(false, truer) => Cols.CtrlExtraBoxColor,
						(truer, truer) => throw new ArgumentException("Ctrl cannot both be root and extra")
					};
					var name = ctrl.GetType().Name;

					writer.Write(name, color);
					if (surround)
						writer.SurroundWith(AsciiBoxes.Curved, surroundBoxColor);

					break;
				}

				case StFlexNode { State: var state }:
				{
					var isInSet = partition.RMap.ContainsKey(state);
					var color = isInSet switch
					{
						truer => Cols.FlexColorInSet,
						false => Cols.FlexColorNotInSet,
					};
					writer.WriteLine(state.Name, color);
					if (isInSet)
					{
						var r = partition.RMap[state];
						writer.Write($"{r}", Cols.FlexRColor);
					}
					break;
				}

				case TextMeasureNode n:
				{
					writer.Write($"txt:{n.Size}", Cols.TxtMeasureColor);
					break;
				}

				default:
					throw new ArgumentException("Impossible");
			}
		}


		void WritePartition(Partition partition, ITxtWriter writer)
		{
			var txt = partition.Root.LogColored(
				(n, w) => WriteNode(n, w, partition),
				opt =>
				{
				}
			);
			writer.Write(txt);
			writer.SurroundWith(AsciiBoxes.Double, Cols.PartitionBoxColor, partitionNames[partition]);
		}


		var txt = partitionSet.PartitionTree
			.LogColored(
				WritePartition,
				opt =>
				{
					opt.GutterSz = new Sz(6, 2);
					opt.ArrowColor = Cols.PartitionArrowColor;
				}
			);
		txt.PrintToConsole();

		//txt.RenderToHtml(@"C:\tmp\webtemplate\layout.html", typeof(Cols));
	}
}