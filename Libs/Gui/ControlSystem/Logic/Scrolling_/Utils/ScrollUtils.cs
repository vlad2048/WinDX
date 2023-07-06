using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;

namespace ControlSystem.Logic.Scrolling_.Utils;

sealed record ScrollNfo(
	ScrollState State,
	BoolVec Enabled,
	BoolVec Visible,
	Sz ViewSz,
	Sz ContSz,
	Sz TrakSz,
	R ViewR
)
{
	public static ScrollNfo MkEmpty(NodeState state) => new(
		state.ScrollState,
		BoolVec.False,
		BoolVec.False,
		Sz.Empty,
		Sz.Empty,
		Sz.Empty,
		R.Empty
	);
}

static class ScrollUtils
{
	public static ScrollNfo GetScrollInfos(this Partition partition, NodeState state)
	{
		if (!partition.NodeMap.ContainsKey(state))
			return ScrollNfo.MkEmpty(state);

		var node = partition.NodeMap[state];
		var nodeR = partition.RMap[state];
		var stNode = (StFlexNode)node.V;
		var scrollEnabled = stNode.Flex.Flags.Scroll;
		var scrollVisible = partition.AreScrollBarsVisible(state);
		var crossSz = GeomMaker.SzDirFun(dir => scrollVisible.Dir(dir.Neg()) ? FlexFlags.ScrollBarCrossDims.Dir(dir) : 0);
		var viewSz = partition.RMap[state].Size - crossSz;
		var contSz = node
			.GetFirstChildrenWhere(e => e is StFlexNode st && partition.RMap.ContainsKey(st.State))
			.Select(e => partition.RMap[((StFlexNode)e.V).State] + ((StFlexNode)e.V).Flex.Marg)
			.Union()
			.Size;
		var trakSz = GeomMaker.SzDirFun(dir => scrollVisible.Dir(dir) switch
		{
			false => 0,
			truer => Math.Max(0, GetScrollBarR(nodeR, dir, scrollVisible == BoolVec.True).Dir(dir) - 2 * 17)
		});
		return new ScrollNfo(
			stNode.State.ScrollState,
			scrollEnabled,
			scrollVisible,
			viewSz,
			contSz,
			trakSz,
			new R(partition.GetNodeR(state).Ensure().Pos, viewSz)
		);
	}


	public static PartitionSet ApplyScrollOffsets(this PartitionSet partitionSet, MixLayout mixLayout)
	{
		var states = partitionSet.Partitions.SelectMany(e => e.NodeStates).ToArray();
		var ofsMap = states.ToDictionary(e => e, _ => Pt.Empty);

		var statesWithOfs = states.WhereToArray(e => e.ScrollState.ScrollOfs != Pt.Empty);

		var extraStateLinks = partitionSet.Partitions.Select(e => e.SysPartition.StateLinks).Merge();

		foreach (var state in statesWithOfs)
		{
			var kidStates = mixLayout.NodeMap[state].Children
				.SelectMany(e => e)
				.Where(e => e.V is StFlexNode)
				.Select(e => ((StFlexNode)e.V).State)
				.SelectMany(e => extraStateLinks.TryGetValue(e, out var kids) switch
				{
					truer => kids.Prepend(e).ToArray(),
					false => new[] { e },
				})
				.ToArray();

			var kidStatesDistinct = kidStates.Distinct().ToArray();
			if (kidStates.Length != kidStatesDistinct.Length) throw new ArgumentException("Impossible, they should all be distinct");

			var scrollOfs = state.ScrollState.ScrollOfs;
			foreach (var kidState in kidStates)
				ofsMap[kidState] += scrollOfs;
		}

		return partitionSet with
		{
			Partitions = partitionSet.Partitions.SelectToArray(partition => partition.ApplyOfsMap(ofsMap))
		};
	}


	private static Partition ApplyOfsMap(this Partition partition, IReadOnlyDictionary<NodeState, Pt> ofsMap) =>
		partition with
		{
			RMap = partition.RMap.ToDictionary(
				kv => kv.Key,
				kv => kv.Value - ofsMap[kv.Key]
			)
		};


	private static IReadOnlyDictionary<K, V[]> Merge<K, V>(this IEnumerable<IReadOnlyDictionary<K, V[]>> source) where K : notnull
	{
		var res = new Dictionary<K, List<V>>();
		foreach (var dict in source)
		{
			foreach (var (key, vals) in dict)
				foreach (var val in vals)
					res.AddToDictionaryList(key, val);
		}
		return res.MapValues(e => e.ToArray());
	}





	private static BoolVec AreScrollBarsVisible(this Partition partition, NodeState state)
	{
		var node = partition.NodeMap[state];
		var scrollEnabled = ((StFlexNode)node.V).Flex.Flags.Scroll;
		var viewSz = partition.RMap[state].Size;
		var contSz = node
			.GetFirstChildrenWhere(e => e is StFlexNode st && partition.RMap.ContainsKey(st.State))
			.Select(e => partition.RMap[((StFlexNode)e.V).State] + ((StFlexNode)e.V).Flex.Marg)
			.Union()
			.Size;
		return IsScrollNeeded(scrollEnabled, viewSz, contSz);
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
		var isY = r.Width >= FlexFlags.ScrollBarCrossDims.Width;
		if (!isX || !isY) throw new ArgumentException("Impossible");
		return new R(
			r.X + r.Width - cross.Width,
			r.Y + r.Height - cross.Height,
			cross.Width,
			cross.Height
		);
	}


	private static BoolVec IsScrollNeeded(
		BoolVec scrollEnabled,
		Sz viewSz,
		Sz contSz
	) =>
		(scrollEnabled.X, scrollEnabled.Y) switch
		{
			(false, false) => BoolVec.False,
			(truer, false) => new BoolVec(contSz.Width > viewSz.Width && viewSz.Height >= FlexFlags.ScrollBarCrossDims.Height, false),
			(false, truer) => new BoolVec(false, contSz.Height > viewSz.Height && viewSz.Width >= FlexFlags.ScrollBarCrossDims.Width),
			(truer, truer) => IsScrollNeededWhenTotallyEnabled(viewSz, contSz)
		};







	private static BoolVec IsScrollNeededWhenTotallyEnabled(Sz viewSz, Sz contSz)
	{
		var (viewX, viewY) = (viewSz.Width, viewSz.Height);
		var (contX, contY) = (contSz.Width, contSz.Height);

		var isX = viewSz.Height >= FlexFlags.ScrollBarCrossDims.Height;
		var isY = viewSz.Width >= FlexFlags.ScrollBarCrossDims.Width;

		if (contX > viewX)
		{
			viewY = Math.Max(0, viewY - FlexFlags.ScrollBarCrossDims.Height);
			return (contY > viewY) switch
			{
				truer => new BoolVec(isX, isY),
				false => new BoolVec(isX, false)
			};
		}
		if (contY > viewY)
		{
			viewX = Math.Max(0, viewX - FlexFlags.ScrollBarCrossDims.Width);
			return (contX > viewX) switch
			{
				truer => new BoolVec(isX, isY),
				false => new BoolVec(false, isY)
			};
		}

		return BoolVec.False;
	}
}