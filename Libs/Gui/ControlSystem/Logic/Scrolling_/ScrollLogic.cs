using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.Structs.Enum;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using ControlSystem.Utils;
using PowTrees.Algorithms;
using RenderLib.Renderers;

namespace ControlSystem.Logic.Scrolling_;

static class ScrollLogic
{
	public static PartitionSet HandleScrolling(this PartitionSet set, ScrollMan scrollMan, IRenderWinCtx renderer) =>
		set
			.UpdateScrollStates()
			.CreateScrollBars(scrollMan)
			.RenderScrollBars(scrollMan, renderer)
			.ApplyScrollOffsets();


	private static PartitionSet UpdateScrollStates(this PartitionSet set)
	{
		var nodeStates = set.Root.GetAllNodeStatesWithScrollingEnabled();
		foreach (var nodeState in nodeStates)
		{
			var nfo = set.GetScrollInfos(nodeState);
			nodeState.ScrollState.UpdateFromLayout(nfo);
		}
		return set;
	}

	private static PartitionSet CreateScrollBars(this PartitionSet set, ScrollMan scrollMan)
	{
		scrollMan.CreateScrollBars(set);
		return set;
	}

	private static PartitionSet RenderScrollBars(this PartitionSet set, ScrollMan scrollMan, IRenderWinCtx renderer)
	{
		var nodeStates = set.Root.GetAllNodeStatesWithScrollingEnabled();
		foreach (var nodeState in nodeStates)
		{
			var dadNod = set.Lookups.NodeState2Nod[nodeState];
			var duo = scrollMan.GetDuo(nodeState);
			var nfo = set.GetScrollInfos(nodeState);
			var nodeR = set.RMap[nodeState];
			var both = nfo.State.X.IsVisible() && nfo.State.Y.IsVisible();
			var extraCtrls = new List<Ctrl>();

			set.RenderIf(
				nfo.State.Dir(Dir.Horz).IsVisible(),
				() => duo.Get(Dir.Horz),
				() => GetScrollBarR(nodeR, Dir.Horz, both),
				dadNod,
				renderer,
				extraCtrls
			);

			set.RenderIf(
				nfo.State.Dir(Dir.Vert).IsVisible(),
				() => duo.Get(Dir.Vert),
				() => GetScrollBarR(nodeR, Dir.Vert, both),
				dadNod,
				renderer,
				extraCtrls
			);

			set.RenderIf(
				both,
				() => duo.GetCorner(),
				() => GetScrollBarCornerR(nodeR),
				dadNod,
				renderer,
				extraCtrls
			);

			if (extraCtrls.Any())
				set.ExtraCtrlsToRenderOnPop[nodeState] = extraCtrls.ToArray();
		}

		set.RecomputeLookupMaps();

		return set;
	}

	private static PartitionSet ApplyScrollOffsets(this PartitionSet set)
	{
		var trees = set.Root
			.Filter(e => e.IsNodeState())
			.SelectToArray(tree => tree.Map(e => e.GetNodeState()));

		foreach (var tree in trees)
		{
			var ofsMap = tree.FoldL_Parent_Dict(e => -e.ScrollState.ScrollOfs, (a, b) => a + b, Pt.Empty);
			set.RMap.ApplyOffsets(ofsMap);
		}

		foreach (var (nodeState, ctrls) in set.ExtraCtrlsToRenderOnPop)
		{
			var extraNodeStates = ctrls.Select(ctrl => set.Lookups.Ctrl2Nod[ctrl]).SelectMany(e => e.GetAllNodeStates());
			foreach (var extraNodeState in extraNodeStates)
				set.RMap[extraNodeState] += nodeState.ScrollState.ScrollOfs;
		}

		return set;
	}




	public static ScrollNfo GetScrollInfos(this PartitionSet set, NodeState state)
	{
		var nod = set.Lookups.NodeState2Nod[state];
		var nodR = set.RMap[state];
		var flex = ((StFlexNode)nod.V).Flex;
		if (flex.Flags.Scroll == BoolVec.False) throw new ArgumentException("Impossible");
		var enabled = flex.Flags.Scroll;
		var view = set.RMap[state].Size;
		var cont = nod
			.GetFirstChildrenWhere(e => e.IsNodeState())
			.Select(e => set.RMap[((StFlexNode)e.V).State] + ((StFlexNode)e.V).Flex.Marg)
			.Union()
			.Size;
		var (stateX, stateY) = ScrollStateCalculator.Get(enabled, view, cont);
		var viewSub = new Sz(
			stateY.IsVisible() ? FlexFlags.ScrollBarCrossDims.Width : 0,
			stateX.IsVisible() ? FlexFlags.ScrollBarCrossDims.Height : 0
		);
		return new ScrollNfo(
			(stateX, stateY),
			view - viewSub,
			cont,
			new R(nodR.Pos, view - viewSub)
		);
	}


	private static void RenderIf(
		this PartitionSet set,
		bool condition,
		Func<Ctrl> ctrlFun,
		Func<R> ctrlRFun,
		MixNode dadNod,
		IRenderWinCtx renderer,
		List<Ctrl> ctrlList
	)
	{
		if (!condition) return;

		var ctrl = ctrlFun();
		var ctrlR = ctrlRFun();
		ctrlList.Add(ctrl);

		var tree = ctrl.BuildTree(renderer);
		var mixLayout = tree
			.SolveTree(FreeSzMaker.FromSz(ctrlR.Size), null!)
			.Translate(ctrlR.Pos);

		dadNod.AddChild(mixLayout.MixRoot);
		mixLayout.RMap.MergeInto(set.RMap);
	}




	public static R GetScrollBarR(R r, Dir dir, bool both)
	{
		var cross = FlexFlags.ScrollBarCrossDims;
		var isX = r.Height >= cross.Height;
		var isY = r.Width >= FlexFlags.ScrollBarCrossDims.Width;

		if (both && (!isX || !isY)) throw new ArgumentException("Impossible");
		if (dir == Dir.Horz && !isX) throw new ArgumentException("Impossible");
		if (dir == Dir.Vert && !isY) throw new ArgumentException("Impossible");

		return (dir, both) switch
		{
			(Dir.Horz, false) => new R(
				r.X,
				r.Y + r.Height - cross.Height,
				r.Width,
				cross.Height
			),
			(Dir.Horz, true) => new R(
				r.X,
				r.Y + r.Height - cross.Height,
				r.Width - cross.Width,
				cross.Height
			),

			(Dir.Vert, false) => new R(
				r.X + r.Width - cross.Width,
				r.Y,
				cross.Width,
				r.Height
			),
			(Dir.Vert, true) => new R(
				r.X + r.Width - cross.Width,
				r.Y,
				cross.Width,
				r.Height - cross.Height
			)
		};
	}

	public static R GetScrollBarCornerR(R r)
	{
		var cross = FlexFlags.ScrollBarCrossDims;
		var isX = r.Height >= cross.Height;
		var isY = r.Width >= cross.Width;
		if (!isX || !isY) throw new ArgumentException("Impossible");
		return new R(
			r.X + r.Width - cross.Width,
			r.Y + r.Height - cross.Height,
			cross.Width,
			cross.Height
		);
	}
}