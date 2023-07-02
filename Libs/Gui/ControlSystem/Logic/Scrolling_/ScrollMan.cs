using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers;

namespace ControlSystem.Logic.Scrolling_;


sealed class ScrollMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IRenderWinCtx renderer;
	private readonly Dictionary<NodeState, Duo> map;

	public ScrollMan(IRenderWinCtx renderer)
	{
		this.renderer = renderer;
		map = new Dictionary<NodeState, Duo>().D(d);
	}


	public PartitionSet AddScrollBars(PartitionSet partitionSet) =>
		partitionSet with
		{
			Partitions = partitionSet.Partitions.SelectToArray(AddScrollBars)
		};


	private Partition AddScrollBars(Partition partition)
	{
		UpdateMap(partition);
		var partitionScrollBars = Create(partition);
		UpdateScrollStates(partition);

		return partition with { ScrollBars = partitionScrollBars };
	}


	private void UpdateMap(Partition partition)
	{
		var treeMap = partition.Root.Where(e => e.V is StFlexNode).ToDictionary(e => ((StFlexNode)e.V).State, e => (StFlexNode)e.V);
		var states = partition.AllNodeStates.WhereToArray(e => treeMap[e].Flex.Flags.Scroll != BoolVec.False);
		var (statesAdd, statesDel) = map.GetAddDels(states);

		foreach (var stateDel in statesDel)
		{
			map.Remove(stateDel, out var duo);
			duo!.Dispose();
		}

		foreach (var stateAdd in statesAdd)
		{
			var duo = new Duo(treeMap[stateAdd]);
			map[stateAdd] = duo;
		}
	}

	private PartitionScrollBars Create(Partition partition)
	{
		(Dictionary<NodeState, List<Ctrl>> ctrlMap, Dictionary<NodeState, NodeState> linkMap, Dictionary<NodeState, R> rMap) maps = (new Dictionary<NodeState, List<Ctrl>>(), new Dictionary<NodeState, NodeState>(), new Dictionary<NodeState, R>());

		var nodeStates = partition.NodeStatesWithScrolling();
		foreach (var nodeState in nodeStates)
		{
			var nodeR = partition.RMap[nodeState];
			var nfo = partition.GetScrollInfos(nodeState);
			var both = nfo.Visible == BoolVec.True;
			maps
				.RenderIf(
					nfo.Visible.Dir(Dir.Horz),
					nodeState,
					() => (
						map[nodeState].Get(Dir.Horz),
						ScrollUtils.GetScrollBarR(nodeR, Dir.Horz, both)
					),
					renderer
				)
				.RenderIf(
					nfo.Visible.Dir(Dir.Vert),
					nodeState,
					() => (
						map[nodeState].Get(Dir.Vert),
						ScrollUtils.GetScrollBarR(nodeR, Dir.Vert, both)
					),
					renderer
				)
				.RenderIf(
					both,
					nodeState,
					() => (
						map[nodeState].GetCorner(),
						ScrollUtils.GetScrollBarCornerR(nodeR)
					),
					renderer
				);
		}

		return new PartitionScrollBars(
			maps.ctrlMap.MapValues(e => e.ToArray()),
			maps.rMap,
			maps.linkMap
		);
	}

	private void UpdateScrollStates(Partition partition)
	{
		var nodeStates = partition.NodeStatesWithScrolling();
		foreach (var nodeState in nodeStates)
		{
			var nfo = partition.GetScrollInfos(nodeState);
			nfo.State.UpdateFromLayout(nfo);
		}
	}
	


	private sealed class Duo : IDisposable
	{
		private readonly Disp d = new();
		public void Dispose() => d.Dispose();

		private readonly ScrollBarCtrl? scrollBarX;
		private readonly ScrollBarCtrl? scrollBarY;
		private readonly ScrollBarCornerCtrl? scrollBarCorner;

		public Duo(StFlexNode st)
		{
			var scroll = st.Flex.Flags.Scroll;
			if (scroll == BoolVec.False) throw new ArgumentException("Impossible");

			scrollBarX = scroll.X ? new ScrollBarCtrl(Dir.Horz, st.State.ScrollState.X, st.State.Evt).D(d) : null;
			scrollBarY = scroll.Y ? new ScrollBarCtrl(Dir.Vert, st.State.ScrollState.Y, st.State.Evt).D(d) : null;
			scrollBarCorner = (scroll == BoolVec.True) ? new ScrollBarCornerCtrl().D(d) : null;
		}

		public ScrollBarCtrl Get(Dir dir) => dir switch
		{
			Dir.Horz => scrollBarX ?? throw new ArgumentException("Impossible"),
			Dir.Vert => scrollBarY ?? throw new ArgumentException("Impossible"),
		};

		public ScrollBarCornerCtrl GetCorner() => scrollBarCorner ?? throw new ArgumentException("Impossible");
	}
}


file static class ScrollManLocalUtils
{
	public static (Dictionary<NodeState, List<Ctrl>>, Dictionary<NodeState, NodeState>, Dictionary<NodeState, R>) RenderIf(
		this (Dictionary<NodeState, List<Ctrl>> ctrlMap, Dictionary<NodeState, NodeState> linkMap, Dictionary<NodeState, R> rMap) maps,
		bool condition,
		NodeState state,
		Func<(Ctrl, R)> makeFun,
		IRenderWinCtx renderer
	)
	{
		if (!condition) return maps;

		var (ctrl, ctrlR) = makeFun();
		var ctrlRMap = RenderCtrl(ctrl, ctrlR, renderer);
		foreach (var (key, val) in ctrlRMap)
		{
			maps.rMap[key] = val;
			maps.linkMap[key] = state;
		}

		if (!maps.ctrlMap.TryGetValue(state, out var list))
			list = maps.ctrlMap[state] = new List<Ctrl>();
		list.Add(ctrl);

		return maps;
	}



	private static IReadOnlyDictionary<NodeState, R> RenderCtrl(Ctrl ctrl, R nodeR, IRenderWinCtx renderer) =>
		ctrl
			.BuildCtrlTree(renderer)
			.ResolveCtrlTree(FreeSzMaker.FromSz(nodeR.Size), null!)
			.Translate(nodeR.Pos)
			.RMap;
}