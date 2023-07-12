using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Structs.Enum;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers;

namespace ControlSystem.Logic.Scrolling_;


static class ScrollManExt
{
	public static PartitionSet AddScrollBars(this PartitionSet partitionSet, ScrollMan scrollMan) => scrollMan.AddScrollBars(partitionSet);
}

sealed class ScrollMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	// ReSharper disable once NotAccessedPositionalProperty.Local
	private sealed record NKey(NodeState? Id);

	private readonly IRenderWinCtx renderer;
	private readonly Dictionary<NKey, Dictionary<NodeState, Duo>> stateMaps;

	public ScrollMan(IRenderWinCtx renderer)
	{
		this.renderer = renderer;
		stateMaps = new Dictionary<NKey, Dictionary<NodeState, Duo>>().D(d);
	}


	public PartitionSet AddScrollBars(PartitionSet partitionSet) =>
		partitionSet with
		{
			Partitions = partitionSet.Partitions.SelectToArray(AddScrollBars)
		};


	private Partition AddScrollBars(Partition partition)
	{
		UpdateMap(partition);
		var sysPartition = Create(partition);
		var partitionRes = partition with { SysPartition = sysPartition };
		UpdateScrollStates(partitionRes);
		return partitionRes;
	}


	private void UpdateMap(Partition partition)
	{
		var treeMap = partition.Root.Where(e => e.V is StFlexNode).ToDictionary(e => ((StFlexNode)e.V).State, e => (StFlexNode)e.V);
		var states = partition.NodeStates.WhereToArray(e => treeMap[e].Flex.Flags.Scroll != BoolVec.False);
		var map = stateMaps.GetOrCreate(new NKey(partition.Id), () => new Dictionary<NodeState, Duo>());
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

	

	private SysPartition Create(Partition partition)
	{
		var sys = new SysPartitionMut();

		var stateMap = stateMaps.GetOrCreate(new NKey(partition.Id), () => new Dictionary<NodeState, Duo>());

		var nodeStates = partition.NodeStatesWithScrolling();
		foreach (var nodeState in nodeStates)
		{
			var nodeR = partition.RMap[nodeState];
			var nfo = partition.GetScrollInfos(nodeState);
			var both = nfo.State.X.IsVisible() && nfo.State.Y.IsVisible();

			ScrollManLocalUtils.RenderIf(
				nfo.State.Dir(Dir.Horz).IsVisible(),
				sys,
				nodeState,
				() => (
					stateMap[nodeState].Get(Dir.Horz),
					ScrollUtils.GetScrollBarR(nodeR, Dir.Horz, both)
				),
				renderer
			);

			ScrollManLocalUtils.RenderIf(
				nfo.State.Dir(Dir.Vert).IsVisible(),
				sys,
				nodeState,
				() => (
					stateMap[nodeState].Get(Dir.Vert),
					ScrollUtils.GetScrollBarR(nodeR, Dir.Vert, both)
				),
				renderer
			);

			ScrollManLocalUtils.RenderIf(
				both,
				sys,
				nodeState,
				() => (
					stateMap[nodeState].GetCorner(),
					ScrollUtils.GetScrollBarCornerR(nodeR)
				),
				renderer
			);

		}

		return sys.ToSysPartition();
	}

	private void UpdateScrollStates(Partition partition)
	{
		var nodeStates = partition.NodeStatesWithScrolling();
		foreach (var nodeState in nodeStates)
		{
			var nfo = partition.GetScrollInfos(nodeState);
			nodeState.ScrollState.UpdateFromLayout(nfo);
		}
	}
}

file static class ScrollManLocalUtils
{
	public static void RenderIf(
		bool condition,
		SysPartitionMut sys,
		NodeState state,
		Func<(Ctrl, R)> makeFun,
		IRenderWinCtx renderer
	)
	{
		if (!condition) return;

		var (ctrl, ctrlR) = makeFun();
		var (tree, ctrlRMap) = RenderCtrl(ctrl, ctrlR, renderer);

		sys.Forest.AddToDictionaryList(state, tree);
		sys.CtrlTriggers.AddToDictionaryList(state, ctrl);
		foreach (var (key, val) in ctrlRMap)
			sys.RMap[key] = val;
	}



	private static (MixNode, IReadOnlyDictionary<NodeState, R>) RenderCtrl(Ctrl ctrl, R nodeR, IRenderWinCtx renderer)
	{
		var tree = ctrl
			.BuildTree(renderer);

		var rMap = tree
			.SolveTree(FreeSzMaker.FromSz(nodeR.Size), null!)
			.Translate(nodeR.Pos)
			.RMap;

		return (tree.Root, rMap);
	}
}
