/*
using ControlSystem.Logic.Popup_;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.State;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Structs.Enum;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;

namespace ControlSystem.Logic.Scrolling_.Utils;

static class ScrollUtils
{
	public static ScrollNfo GetScrollInfos(this Partition partition, NodeState state)
	{
		if (!partition.NodeMap.ContainsKey(state))
			return ScrollNfo.Empty;

		var node = partition.NodeMap[state];
		var stNode = (StFlexNode)node.V;
		var enabled = stNode.Flex.Flags.Scroll;
		var view = partition.RMap[state].Size;
		var cont = node
			.GetFirstChildrenWhere(e => e is StFlexNode st && partition.RMap.ContainsKey(st.State))
			.Select(e => partition.RMap[((StFlexNode)e.V).State] + ((StFlexNode)e.V).Flex.Marg)
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
			new R(partition.GetNodeR(state).Ensure().Pos, view - viewSub)
		);
	}


	public static PartitionSet ApplyScrollOffsets(this PartitionSet partitionSet)
	{
		var states = partitionSet.Partitions.SelectMany(e => e.NodeStates).ToArray();
		var ofsMap = states.ToDictionary(e => e, _ => Pt.Empty);

		var statesWithOfs = states.WhereToArray(e => e.ScrollState.ScrollOfs != Pt.Empty);

		var extraStateLinks = partitionSet.Partitions.Select(e => e.SysPartition.GetStateLinks()).Merge();
		var mixLayout = partitionSet.MixLayout;

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



	
	private static Partition ApplyOfsMap(this Partition partition, IReadOnlyDictionary<NodeState, Pt> ofsMap) =>
		partition with
		{
			RMap = partition.RMap.ToDictionary(
				kv => kv.Key,
				kv => kv.Value - ofsMap[kv.Key]
			)
		};





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


	private static BoolVec IsScrollNeeded(
		BoolVec scrollEnabled,
		Sz viewSz,
		Sz contSz
	) =>
		(scrollEnabled.X, scrollEnabled.Y) switch
		{
			(false, false) => BoolVec.False,
			(truer, false) => new BoolVec(
				contSz.Width > viewSz.Width && viewSz.Height >= FlexFlags.ScrollBarCrossDims.Height,
				false
			),
			(false, truer) => new BoolVec(
				false,
				contSz.Height > viewSz.Height && viewSz.Width >= FlexFlags.ScrollBarCrossDims.Width
			),
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
*/