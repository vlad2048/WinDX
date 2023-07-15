using ControlSystem.Logic.Rendering_;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowRxVar;
using PowTrees.Algorithms;
using RenderLib.Renderers;
using TreePusherLib;
using TreePusherLib.ConvertExts;
using TreePusherLib.ConvertExts.Structs;

namespace ControlSystem.Utils;

static class MixTreeUtils
{
	public static ReconstructedTree<IMixNode> BuildTree(this Ctrl ctrl, IRenderWinCtx renderer)
	{
		using var d = new Disp();
		using var gfx = renderer.GetGfx(true).D(d);
		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var r = new RenderArgs(gfx, pusher).D(d);

		r.WhenCtrlPushNext.Subscribe(ctrl => ctrl.SignalRender(r)).D(d);

		return
			treeEvtObs.ToTree(
				onPush: _ => { },
				onPop: _ => { },
				runAction: () =>
				{
					using (r[ctrl]) { }
				}
			);
	}

	public static MixLayout SolveTree(this ReconstructedTree<IMixNode> tree, FreeSz freeSz, Win win)
	{
		var mixRoot = tree.Root.ApplyTextMeasures();

		var stFlexRoot = mixRoot.OfTypeTree<IMixNode, StFlexNode>();
		var flexRoot = stFlexRoot.Map(e => e.Flex);

		var layout = FlexSolver.Solve(flexRoot, freeSz);

		var flex2st = flexRoot.Zip(stFlexRoot).ToDictionary(e => e.First, e => e.Second.V.State);
		var nodeMap = tree.Root
			.Where(e => e.V is StFlexNode)
			.ToDictionary(e => ((StFlexNode)e.V).State);
		return
			new MixLayout(
				win,
				freeSz,
				mixRoot,
				nodeMap,
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
	
	
	public static IReadOnlyDictionary<MixNode, Ctrl> BuildMixNode2CtrlMap(MixNode root)
	{
		var map = new Dictionary<MixNode, Ctrl>();
		void Rec(MixNode node, Ctrl ctrlCur)
		{
			if (node.V is CtrlNode { Ctrl: var ctrlNext })
				ctrlCur = ctrlNext;
			map[node] = ctrlCur;
			foreach (var kid in node.Children)
				Rec(kid, ctrlCur);
		}
		if (root.V is not CtrlNode { Ctrl: var ctrlRoot }) throw new ArgumentException("The root should always be a CtrlNode");
		Rec(root, ctrlRoot);
		return map;
	}


	private static Action<IMixNode> CtrlPushTriggersRender(RenderArgs renderArgs) => mixNode =>
	{
		if (mixNode is not CtrlNode { Ctrl: var nodeCtrl }) return;
		if (nodeCtrl.D.IsDisposed)
			throw new ObjectDisposedException(nodeCtrl.GetType().Name, "Cannot render a disposed Ctrl");
		nodeCtrl.SignalRender(renderArgs);
	};


	private static Ctrl FindCtrlContaining(this MixNode nod)
	{
		while (nod.V is not CtrlNode { Ctrl: not null })
			nod = nod.Parent ?? throw new ArgumentException("Cannot find containing Ctrl");
		return ((CtrlNode)nod.V).Ctrl;
	}
}