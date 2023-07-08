﻿using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;

namespace ControlSystem.Logic.Popup_.Structs;

/// <summary>
/// Represents the portion of the partition added by the system (used for ScrollBars)
/// </summary>
sealed record SysPartition(
	IReadOnlyDictionary<NodeState, Ctrl[]> CtrlTriggers,
	IReadOnlyDictionary<NodeState, R> RMap,
	IReadOnlyDictionary<NodeState, NodeState[]> StateLinks
)
{
	public NodeState[] NodeStates => RMap.Keys.ToArray();

	public static readonly SysPartition Empty = new(
		new Dictionary<NodeState, Ctrl[]>(),
		new Dictionary<NodeState, R>(),
		new Dictionary<NodeState, NodeState[]>()
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
/// ● This is the Ctrl that gets renderered to trigger the tree rendering <br/>
/// ● Does not contain the Extra Ctrls and NodeStates (used to add ScrollBars)
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
/// <param name="SysPartition">
/// Represents the portion of the partition added by the system (used for ScrollBars)
/// </param>
sealed record Partition(
    NodeState? Id,
    MixNode Root,
	IReadOnlyDictionary<NodeState, MixNode> NodeMap,
    IReadOnlyDictionary<NodeState, R> RMap,
    IReadOnlySet<Ctrl> CtrlSet,
	SysPartition SysPartition,
	int ZOrderWin
)
{
	public override string ToString() => $"Id:{Id}  Nodes:{NodeStates.Length}";

    public Ctrl RootCtrl => ((CtrlNode)Root.V).Ctrl;

    public NodeState[] NodeStates => RMap.Keys.ToArray();

    public NodeZ[] AllNodeStates
    {
	    get
	    {
		    var zOrderNodeMapNormal = Root.GetZOrderNodeMapNormal(RMap);
		    var zOrderNodeMapSys = SysPartition.RMap.GetZOrderNodeMapSys();
		    return
			    RMap.Keys.Select(node => new NodeZ(node, new ZOrder(ZOrderWin, false, zOrderNodeMapNormal[node]))).Concat(
					    SysPartition.RMap.Keys.Select(node => new NodeZ(node, new ZOrder(ZOrderWin, true, zOrderNodeMapSys[node])))
				    )
				    .ToArray();
	    }
    }
	    
	    
	    //=> RMap.Keys.OfType<INode>().Concat(SysPartition.RMap.Keys).ToArray();

    
    private static readonly Ctrl emptyCtrl = new Ctrl().DisposeOnProgramExit();

    public static readonly Partition Empty = new(
        null,
        Nod.Make<IMixNode>(new CtrlNode(emptyCtrl)),
		new Dictionary<NodeState, MixNode>(),
        new Dictionary<NodeState, R>(),
        new HashSet<Ctrl> { emptyCtrl },
		SysPartition.Empty,
		0
    );
}


static class PartitionExt
{
	/*public static INodeStateUserEventsSupport[] FindNodesAtMouseCoordinates(this Partition partition, Pt pt) =>
		partition.NodeStates.Concat(partition.SysPartition.NodeStates)
			.Where(state => partition.GetNodeR(state).Ensure().Contains(pt))
			.Reverse()
			.OfType<INodeStateUserEventsSupport>()
			.ToArray();*/


	public static NodeState[] NodeStatesWithScrolling(this Partition partition) => partition.NodeStates
		.WhereToArray(e => partition.NodeMap.TryGetValue(e, out var node) switch
			{
				truer => ((StFlexNode)node.V).Flex.Flags.Scroll != BoolVec.False,
				false => false,
			}
		);


	public static Maybe<R> GetNodeR(this Partition partition, NodeState state)
	{
		if (partition.SysPartition.RMap.TryGetValue(state, out var sysR)) return May.Some(sysR);
		if (partition.RMap.TryGetValue(state, out var r)) return May.Some(r);
		return May.None<R>();
	}


	public static IReadOnlyDictionary<NodeState, int> GetZOrderNodeMapNormal(this MixNode root, IReadOnlyDictionary<NodeState, R> rMap) =>
		root
			.Where(e => e.V is StFlexNode)
			.Select(e => ((StFlexNode)e.V).State)
			.Where(rMap.ContainsKey)
			.Select((n, i) => (n, i))
			.ToDictionary(t => t.n, t => t.i);

	public static IReadOnlyDictionary<NodeState, int> GetZOrderNodeMapSys(this IReadOnlyDictionary<NodeState, R> rMap) =>
		rMap.Keys
			.Select((n, i) => (n, i))
			.ToDictionary(t => t.n, t => t.i);
}