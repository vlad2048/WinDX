using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents;

namespace ControlSystem.Logic.Popup_.Structs;

/// <summary>
/// ScrollBar controls info
/// </summary>
/// <param name="ControlMap">Map from the Dad NodeState to the scroll controls</param>
/// <param name="RMap">Map from the Kids NodeStates to Rs</param>
/// <param name="StateLinkMap">Map from the Kids NodesStates to the Dad NodeStates</param>
sealed record PartitionScrollBars(
	IReadOnlyDictionary<NodeState, Ctrl[]> ControlMap,
	IReadOnlyDictionary<NodeState, R> RMap,
	IReadOnlyDictionary<NodeState, NodeState> StateLinkMap
)
{
	public static readonly PartitionScrollBars Empty = new(
		new Dictionary<NodeState, Ctrl[]>(),
		new Dictionary<NodeState, R>(),
		new Dictionary<NodeState, NodeState>()
	);
}

/// <summary>
/// Represents a partition of the layout to be displayed on the main window or child popup window
/// </summary>
/// <param name="Id">
/// ● null for the main window partition <br/>
/// ● The NodeState of the Pop node for a popup window partition
/// </param>
/// <param name="Root">
/// Tree root <br/>
/// ● Always a Ctrl <br/>
/// ● This is the Ctrl that gets renderered to trigger the tree rendering
/// </param>
/// <param name="RMap">
/// ● The keys represents which nodes logically belong in this partition <br/>
/// ● The values are the rectangles from the layout
/// </param>
/// <param name="CtrlSet">
/// The set of all the Ctrls contained in Root (these will be the ones we call Render for)
/// </param>
sealed record Partition(
    NodeState? Id,
    MixNode Root,
	IReadOnlyDictionary<NodeState, MixNode> NodeMap,
    IReadOnlyDictionary<NodeState, R> RMap,
    HashSet<Ctrl> CtrlSet,
	PartitionScrollBars ScrollBars
)
{
    public Ctrl RootCtrl => ((CtrlNode)Root.V).Ctrl;

    public NodeState[] AllNodeStates =>
        (
            from node in Root
            where node.V is StFlexNode
            let state = ((StFlexNode)node.V).State
            where RMap.ContainsKey(state)
            select state
        )
        .ToArray();

    public NodeState[] AllNodeStatesIncludingScrollBars => AllNodeStates.Concat(ScrollBars.RMap.Keys).ToArray();

    
    private static readonly Ctrl emptyCtrl = new Ctrl().DisposeOnProgramExit();

    public static readonly Partition Empty = new(
        null,
        Nod.Make<IMixNode>(new CtrlNode(emptyCtrl)),
		new Dictionary<NodeState, MixNode>(),
        new Dictionary<NodeState, R>(),
        new HashSet<Ctrl> { emptyCtrl },
		PartitionScrollBars.Empty
    );
}


static class PartitionExt
{
	public static Maybe<INodeStateUserEventsSupport> FindNodeAtMouseCoordinates(this Partition partition, Pt pt) =>
		partition.AllNodeStatesIncludingScrollBars
			.Where(state => partition.GetNodeR(state).Ensure().Contains(pt))
			.Reverse()
			.OfType<INodeStateUserEventsSupport>()
			.FirstOrMaybe();

	public static NodeState[] NodeStatesWithScrolling(this Partition partition) => partition.AllNodeStates.WhereToArray(e => ((StFlexNode)partition.NodeMap[e].V).Flex.Flags.Scroll != BoolVec.False);

	public static Maybe<R> GetNodeR(this Partition partition, NodeState state) =>
		PartitionLocalUtils.GetFirst(
			() => partition.RMap.GetOrMaybe(state),
			() => partition.ScrollBars.RMap.GetOrMaybe(state)
		);
}


file static class PartitionLocalUtils
{
	public static Maybe<T> GetFirst<T>(params Func<Maybe<T>>[] funs)
	{
		foreach (var fun in funs)
		{
			var res = fun();
			if (res.IsSome())
				return res;
		}
		return May.None<T>();
	}
}