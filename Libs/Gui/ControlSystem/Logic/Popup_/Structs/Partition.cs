using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents;

namespace ControlSystem.Logic.Popup_.Structs;


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
/// <param name="NodeMap">
/// NodeMap
/// </param>
/// <param name="RMap">
/// ● The keys represents which nodes logically belong in this partition <br/>
/// ● The values are the rectangles from the layout
/// </param>
/// <param name="CtrlSet">
/// The set of all the Ctrls contained in Root (these will be the ones we call Render for)
/// </param>
/// <param name="ExtraCtrlPopTriggers">
/// Extra Ctrls to render when a NodeState is popped (used to add ScrollBars)
/// </param>
/// <param name="ExtraStateLinks">
/// Indicates how the Extra States are parented (used to propagate ScrollOffsets)
/// </param>
sealed record Partition(
    NodeState? Id,
    MixNode Root,
	IReadOnlyDictionary<NodeState, MixNode> NodeMap,
    IReadOnlyDictionary<NodeState, R> RMap,
    HashSet<Ctrl> CtrlSet,
	IReadOnlyDictionary<NodeState, Ctrl[]> ExtraCtrlPopTriggers,
	IReadOnlyDictionary<NodeState, NodeState[]> ExtraStateLinks
)
{
	public override string ToString() => $"Id:{Id}  Nodes:{AllNodeStates.Length}  CtrlPopTriggers:{ExtraCtrlPopTriggers.Values.SelectMany(e => e).Count()}";

    public Ctrl RootCtrl => ((CtrlNode)Root.V).Ctrl;

    public NodeState[] AllNodeStates => RMap.Keys.ToArray();

    
    private static readonly Ctrl emptyCtrl = new Ctrl().DisposeOnProgramExit();

    public static readonly Partition Empty = new(
        null,
        Nod.Make<IMixNode>(new CtrlNode(emptyCtrl)),
		new Dictionary<NodeState, MixNode>(),
        new Dictionary<NodeState, R>(),
        new HashSet<Ctrl> { emptyCtrl },
		new Dictionary<NodeState, Ctrl[]>(),
		new Dictionary<NodeState, NodeState[]>()
    );
}


static class PartitionExt
{
	public static INodeStateUserEventsSupport[] FindNodesAtMouseCoordinates(this Partition partition, Pt pt) =>
		partition.AllNodeStates
			.Where(state => partition.GetNodeR(state).Ensure().Contains(pt))
			.Reverse()
			.OfType<INodeStateUserEventsSupport>()
			.ToArray();

	public static NodeState[] NodeStatesWithScrolling(this Partition partition) => partition.AllNodeStates
		.WhereToArray(e => partition.NodeMap.TryGetValue(e, out var node) switch
			{
				truer => ((StFlexNode)node.V).Flex.Flags.Scroll != BoolVec.False,
				false => false,
			}
		);

	public static Maybe<R> GetNodeR(this Partition partition, NodeState state) => partition.RMap.GetOrMaybe(state);
}