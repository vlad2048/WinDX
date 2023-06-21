using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reactive.Linq;
using ControlSystem.Logic.PopLogic;
using ControlSystem.Structs;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Structs;
using ControlSystem.WinSpectorLogic.Utils;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using Logging;
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
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem;


public class Win : Ctrl
{
	private readonly SysWin sysWin;

	internal SpectorWinDrawState SpectorDrawState { get; }

	public Win(Action<WinOpt>? optFun = null)
	{
		var opt = WinOpt.Build(optFun);
		sysWin = WinUtils.MakeWin(opt).D(D);
		this.D(sysWin.D);
		var slaveMan = new SlaveMan(sysWin).D(D);

		var canSkipLayout = new TimedFlag();
		SpectorDrawState = new SpectorWinDrawState().D(D);
		SpectorDrawState.WhenChanged.Subscribe(_ =>
		{
			canSkipLayout.Set();
			Invalidate();
		}).D(D);

		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);
		G.WinMan.AddWin(this);

		PartitionSet? partitionSet = null;

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var gfx = renderer.GetGfx().D(d);

			if (canSkipLayout.IsNotSet())
			{
				WinUtils.BuildTree(out var reconstructedTree, this);
				WinUtils.AssignWinToCtrls(this, reconstructedTree);
				WinUtils.SolveTree(out var mixLayout, this, reconstructedTree, sysWin.ClientR.V.Size);
				G.WinMan.SetWinLayout(mixLayout);
				partitionSet = PopSplitter.Split(mixLayout.MixRoot, mixLayout.RMap);
			}
			if (partitionSet == null) return;

			var (partition, subPartitions, parentMapping) = partitionSet;
			RenderUtils.RenderTree(partition, gfx);
			slaveMan.ShowSubPartitions(subPartitions, parentMapping, sysWin.Handle, SpectorDrawState);
			SpectorWinRenderUtils.Render(SpectorDrawState, partition, gfx);

		}).D(D);
	}

	public override string ToString() => GetType().Name;

	public void Invalidate() => sysWin.Invalidate();
}

file static class WinUtils
{
	public static void BuildTree(
		out ReconstructedTree<IMixNode> reconstructedTree,
		Ctrl rootCtrl
	)
	{
		using var d = new Disp();
		var gfx = new Dummy_Gfx().D(d);
		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var renderArgs = new RenderArgs(gfx, pusher).D(d);
		reconstructedTree = treeEvtObs.ToTree(
			mixNode =>
			{
				if (mixNode is not CtrlNode { Ctrl: var ctrl }) return;
				ctrl.SignalRender(renderArgs);
			},
			() =>
			{
				using (renderArgs.Ctrl(rootCtrl)) { }
			}
		);
	}



	public static void AssignWinToCtrls(
		Win win,
		ReconstructedTree<IMixNode> reconstructedTree
	) =>
		reconstructedTree.Root
			.Select(e => e.V)
			.OfType<CtrlNode>()
			.SelectToArray(e => e.Ctrl)
			.ForEach(ctrl => ctrl.WinRW.V = May.Some(win));


	public static void SolveTree(
		out MixLayout mixLayout,
		Win win,
		ReconstructedTree<IMixNode> reconstructedTree,
		Sz winSz
	)
	{
		var mixRoot = reconstructedTree.Root;

		var stFlexRoot = mixRoot.OfTypeTree<IMixNode, StFlexNode>();
		var flexRoot = stFlexRoot.Map(e => e.Flex);
		var freeSz = FreeSzMaker.FromSz(winSz);
		var layout = FlexSolver.Solve(flexRoot, freeSz);

		var flex2st = flexRoot.Zip(stFlexRoot).ToDictionary(e => e.First, e => e.Second.V.State);

		mixLayout = new MixLayout(
			win,
			freeSz,
			mixRoot,
			layout.RMap.MapKeys(flex2st),
			layout.WarningMap.MapKeys(flex2st),

			reconstructedTree.IncompleteNodes
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
			};
		});
		win.Init();
		return win;
	}
}