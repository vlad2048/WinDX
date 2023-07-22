using ControlSystem.Logic.Invalidate_;
using ControlSystem.Logic.Popup_;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Rendering_;
using ControlSystem.Logic.Scrolling_;
using ControlSystem.Structs;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Utils;
using DynamicData;
using LayoutSystem.Flex.Structs;
using PowBasics.ColorCode.Utils;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using SysWinLib;
using SysWinLib.Structs;
using UserEvents;
using UserEvents.Generators;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem;


public class Win : Ctrl, IMainWin
{
	private readonly SysWin sysWin;
	private readonly Invalidator fullInvalidator;
	private readonly IRwTracker<NodeZ> rwNodes;
	private readonly IRwTracker<ICtrl> rwCtrls;
	private readonly IRwTracker<IWin> rwWins;

	// WinSpector
	// ==========
	internal IRoMayVar<PartitionSet> PartSet { get; }
	internal SpectorWinDrawState SpectorDrawState { get; }

	// IWinUserEventsSupport
	// =====================
	public INode? Id => null;
	public nint Handle => sysWin.Handle;
	public IObservable<IPacket> SysEvt => sysWin.WhenMsg;
	public Pt PopupOffset => Pt.Empty;
	public IRoVar<Pt> ScreenPt => sysWin.ScreenPt;
	public IRoVar<R> ScreenR => sysWin.ScreenR;
	public IRoVar<Sz> ClientSz => sysWin.ClientSz;
	public IRoTracker<NodeZ> Nodes => rwNodes;
	public IRoTracker<ICtrl> Ctrls => rwCtrls;
	public void SysInvalidate() => sysWin.Invalidate();

	// IMainWinUserEventsSupport
	// =========================
	public IObservable<IUserEvt> Evt { get; }
	public IRoTracker<IWin> Wins => rwWins;
	public IInvalidator Invalidator => fullInvalidator;
	public void SetSize(Sz sz) => sysWin.SetR(new R(ScreenR.V.Pos, sz), WindowPositionFlags.SWP_NOMOVE | WindowPositionFlags.SWP_NOACTIVATE);


	public override string ToString() => GetType().Name;


	public Win(Action<WinOpt>? optFun = null)
	{
		rwNodes = Tracker.Make<NodeZ>().D(D);
		rwCtrls = Tracker.Make<ICtrl>().D(D);
		rwWins = Tracker.Make<IWin>().D(D);
		var scrollMan = new ScrollMan().D(D);
		fullInvalidator = new Invalidator(this).D(D);
		var partitionSet = VarMay.Make<PartitionSet>().D(D);
		PartSet = partitionSet.ToReadOnlyMay();
		SpectorDrawState = new SpectorWinDrawState(Wins).D(D);
		var getWin = PartSet.MakePopups(rwWins, this, SpectorDrawState).D(D);

		PartSet.WhenSome().Subscribe(set =>
		{
			set.Assign_CtrlWins_and_NodeRs(this, getWin);
		}).D(D);

		var opt = WinOpt.Build(optFun);
		sysWin = WinUtils.MakeWin(opt).D(D);
		this.D(sysWin.D);


		Evt = UserEventGenerator.MakeForWin(Wins).D(D);

		Wins.MergeManyTrackers(e => e.Nodes).SelectTracker(e => e.Item).DispatchEvents(out var nodeLock, out var nodeHovr, this).D(D);
		nodeLock.PipeTo(SpectorDrawState.NodeLock);
		nodeHovr.PipeTo(SpectorDrawState.NodeHovr);

		var rendererSwitcher = new RendererSwitcher(sysWin, Invalidator).D(D);

		var isAdded = false;
		void WinMan_AddWin() { if (isAdded) return; isAdded = true; WinMan.AddWin(this); }


		// Invalidate Triggers
		// ===================
		sysWin.ClientSz.Trigger(() => Invalidator.Invalidate(RedrawReason.Resize)).D(D);
		SpectorDrawState.WhenChanged.Trigger(() => Invalidator.Invalidate(RedrawReason.SpectorOverlay)).D(D);

		Wins.MergeManyTrackers(e => e.Ctrls)
			.SelectTracker(e => e.Item).Items
			.MergeMany(e => e.WhenChanged)
			.Trigger(() => Invalidator.Invalidate(RedrawReason.Ctrl)).D(D);

		Wins.MergeManyTrackers(e => e.Nodes)
			.SelectTracker(e => e.Item).Items
			.MergeMany(e => e.Node.WhenInvalidateRequired)
			.Trigger(() => Invalidator.Invalidate(RedrawReason.Node)).D(D);


		// Layout / Render
		// ===============
		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			var isLayoutRequired = fullInvalidator.IsLayoutRequired();
			if (isLayoutRequired)
			{
				partitionSet.V = May.Some(
					this
						.BuildTree(rendererSwitcher.Renderer)
						.SolveTree(FreeSzMaker.FromSz(ClientSz.V), this)
						.SplitIntoPartitions()
						.VerifyInvariants(0)
						.HandleScrolling(scrollMan, rendererSwitcher.Renderer)
						.VerifyInvariants(1)
						//.Assign_CtrlWins_and_NodeRs(this, getWin)
				);

				(rwNodes, rwCtrls).UpdateFromPartition(partitionSet.V.Ensure().MainPartition);

				WinMan_AddWin();
				//WinUtils.LogPartitionSet(partitionSet.V);
			}


			using var d = new Disp();
			var gfx = rendererSwitcher.Renderer.GetGfx(false).D(d);
			var set = partitionSet.V.Ensure();
			var shouldLog = SpectorDrawState.ShouldLogRender(this);
			if (shouldLog)
			{
				var txt = set.PrettyPrint();
				txt.PrintToConsole();
				//txt.RenderToHtml(@"C:\tmp\fmt\set.html", typeof(PartitionSetPrettyPrinter.C));
			}
			RenderUtils.RenderTree(
				set.MainPartition,
				gfx,
				Pt.Empty,
				shouldLog
			);
			SpectorWinRenderUtils.Render(SpectorDrawState, set.MainPartition, gfx, Pt.Empty);

			foreach (var popupWin in Wins.ItemsArr.Skip(1))
				popupWin.SysInvalidate();
		}).D(D);
	}
}



file static class WinUtils
{
	public static void Assign_CtrlWins_and_NodeRs(
		this PartitionSet set,
		Win mainWin,
		Func<NodeState?, IWin> getWin)
	{
		var nodeStates = set.Root.GetAllNodeStates();
		foreach (var nodeState in nodeStates)
		{
			nodeState.RSrc.V = set.RMap[nodeState];

			var part = set.Partitions.Single(part => part.NodeStates.Contains(nodeState));
			var win = getWin(part.NodeStateId);
			nodeState.WinPosSrc.V = win.ScreenPt.V;
		}

		foreach (var partition in set.Partitions)
		{
			var win = getWin(partition.NodeStateId);
			var ctrls = partition.Ctrls;
			foreach (var ctrl in ctrls)
			{
				ctrl.WinSrc.V = May.Some(mainWin);
				ctrl.PopupWinSrc.V = May.Some(win);
			}
		}
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






	/*private static class Cols
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
				_ =>
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
	}*/
}