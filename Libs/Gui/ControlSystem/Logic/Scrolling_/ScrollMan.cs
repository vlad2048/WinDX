using System.Reactive.Disposables;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using PowTrees.Algorithms;
using RenderLib.Renderers;

namespace ControlSystem.Logic.Scrolling_;




sealed class ScrollMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

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
			var both = nfo.Visible == BoolVec.True;

			ScrollManLocalUtils.RenderIf(
				nfo.Visible.Dir(Dir.Horz),
				sys,
				nodeState,
				() => (
					stateMap[nodeState].Get(Dir.Horz),
					ScrollUtils.GetScrollBarR(nodeR, Dir.Horz, both)
				),
				renderer
			);

			ScrollManLocalUtils.RenderIf(
				nfo.Visible.Dir(Dir.Vert),
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

sealed class SysPartitionMut
{
	public Dictionary<NodeState, List<Ctrl>> CtrlTriggers { get; } = new();
	public Dictionary<NodeState, R> RMap { get; } = new();
	public Dictionary<NodeState, List<NodeState>> StateLinks { get; } = new();

	public SysPartition ToSysPartition() => new(
		CtrlTriggers.ToDictionary(e => e.Key, e => e.Value.ToArray()),
		RMap,
		StateLinks.ToDictionary(e => e.Key, e => e.Value.ToArray())
	);
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
		var (ctrlRoot, ctrlRMap) = RenderCtrl(ctrl, ctrlR, renderer);

		sys.CtrlTriggers.AddToDictionaryList(state, ctrl);
		foreach (var (key, val) in ctrlRMap)
		{
			sys.RMap[key] = val;
			sys.StateLinks.AddToDictionaryList(state, key);
		}
	}



	private static (MixNode, IReadOnlyDictionary<NodeState, R>) RenderCtrl(Ctrl ctrl, R nodeR, IRenderWinCtx renderer)
	{
		var tree = ctrl
			.BuildCtrlTree(renderer);

		var rMap = tree
			.ResolveCtrlTree(FreeSzMaker.FromSz(nodeR.Size), null!)
			.Translate(nodeR.Pos)
			.RMap;

		return (
			tree.Root,
			rMap
		);
	}
}




static class ScrollManDictionaryExt
{
	public static void AddToDictionaryList<K, V>(this IDictionary<K, List<V>> dict, K key, V val) where K : notnull
	{
		if (!dict.TryGetValue(key, out var list))
			list = dict[key] = new List<V>();
		list.Add(val);
	}
}


file static class ScrollManDictionaryExtLocal
{
	public static Dictionary<K1, Dictionary<K2, V>> D<K1, K2, V>(this Dictionary<K1, Dictionary<K2, V>> dicts, IRoDispBase d)
		where K1 : notnull
		where K2 : notnull
		where V : IDisposable
	{
		Disposable.Create(() =>
		{
			foreach (var dict in dicts.Values)
			{
				foreach (var val in dict.Values)
					val.Dispose();
				dict.Clear();
			}
			dicts.Clear();
		}).D(d);
		return dicts;
	}

	public static IReadOnlyDictionary<K, V> Merge<K, V>(this IReadOnlyDictionary<K, V> dict, IReadOnlyDictionary<K, V> mergeDict) where K : notnull
	{
		var dictRes = new Dictionary<K, V>();
		foreach (var (key, val) in dict)
			dictRes[key] = val;
		foreach (var (key, val) in mergeDict)
			dictRes[key] = val;
		return dictRes;
	}

	public static IReadOnlyDictionary<K, V[]> Merge<K, V>(this IReadOnlyDictionary<K, V[]> dict, IReadOnlyDictionary<K, List<V>> mergeDict) where K : notnull
	{
		var dictRes = new Dictionary<K, List<V>>();
		foreach (var (key, vals) in dict)
		foreach (var val in vals)
			dictRes.AddToDictionaryList(key, val);
		foreach (var (key, vals) in mergeDict)
		foreach (var val in vals)
			dictRes.AddToDictionaryList(key, val);
		return dictRes.MapValues(e => e.ToArray());
	}

	public static HashSet<T> Merge<T>(this HashSet<T> set, List<T> list)
	{
		var setRes = new HashSet<T>();
		foreach (var elt in set)
			setRes.Add(elt);
		foreach (var elt in list)
			setRes.Add(elt);
		return setRes;
	}

	/*public static Dictionary<K, V> Clone<K, V>(this IReadOnlyDictionary<K, V> dict) where K : notnull
	{
		var res = new Dictionary<K, V>();
		foreach (var (key, val) in dict)
			res[key] = val;
		return res;
	}*/
}
