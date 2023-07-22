using ControlSystem.Structs;
using ControlSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using UserEvents.Structs;

namespace ControlSystem.Logic.Popup_.Structs;

/*

We split the layout along the Pop nodes to display each part in its own popup window
(the first part is displayed on the main window)

************
* Example: *
************

- Full tree:
  ----------
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤        ┌──n3──<p2>──n4
	           └──<p1>──┤
	                    └──C3──<p3>──C4──n5

C1, C2, C3, C4		are Ctrls
n1, n2, n3, n4, n5	are NonPopNodes
<p1>, <p2>, <p3>	are PopNodes


- [0] MainPartition: (the first one displayed on the main window)
  ------------------
	    ┌──n1
	C1──┤      ┌──C2
	    └──n2──┤
	           └──⬤

  NodeStateId		= null			NodeStates		= { n1, n2 }		ZOrderWin			= 0
  ParentNodeStateId	= null			Ctrls			= { C1, C2 }		RootNode			= C1
  RenderCtrl		= C1			CtrlsToRender	= { C1, C2 }
  
  
- [1] Partition: (displayed in its own popup window)
  --------------
	                    ┌──n3──⬤
	              <p1>──┤
	                    └──C3──⬤

  NodeStateId		= <p1>			NodeStates		= { <p1>, n3 }		ZOrderWin			= 1
  ParentNodeStateId	= null			Ctrls			= {     C3 }		RootNode			= <p1>
  RenderCtrl		= C1			CtrlsToRender	= { C1, C3 }

- [2] Partition: (displayed in its own popup window)
  --------------
	                           <p2>──n4

  NodeStateId		= <p2>			NodeStates		= { <p2>, n4 }		ZOrderWin			= 2
  ParentNodeStateId	= <p1>			Ctrls			= { }				RootNode			= <p2>
  RenderCtrl		= C1			CtrlsToRender	= { C1 }

- [3] Partition: (displayed in its own popup window)
  --------------
	                           <p3>──C4──n5

  NodeStateId		= <p3>			NodeStates		= { <p3>, n5 }		ZOrderWin			= 3
  ParentNodeStateId	= <p1>			Ctrls			= {     C4 }		RootNode			= <p3>
  RenderCtrl		= C3			CtrlsToRender	= { C3, C4 }


**************
* Invariants *
**************
  - NodeStates of the partitions form a partition of all the NodeStates
  - Ctrls      of the partitions form a partition of all the Ctrls
  - RootNode is
      - main  partition -> the root Ctrl
      - popup partition -> its associated popup node (corresponds to NodeStateId)
  - RenderCtrl is
      - main  partition -> the root Ctrl
      - popup partition -> the closest Ctrl above the RootNode
  - CtrlsToRender = { RenderCtrl } ∪ Ctrls   (for the main partition, RenderCtrl ∊ Ctrls)

*/
sealed class Partition : IEquatable<Partition>
{
	public NodeState? NodeStateId { get; }				// for MainWin->null, for PopupWins->NodeState of the Pop node (included in NodeStateSet)
	public NodeState? ParentNodeStateId { get; }
	public Ctrl RenderCtrl { get; }						// the Ctrl we need to render for this window
	public int ZOrderWin { get; }
	public PartitionSet Set { get; set; } = null!;


	// **********************
	// * Derived Properties *
	// **********************
	public Pt Offset => NodeStateId switch
	{
		null => Pt.Empty,
		not null => Set.RMap[NodeStateId].Pos
	};

	// Used for:
	//   - dispatching events
	//   - call invalidate when the node requires it
	public NodeState[] NodeStates => RootNode.GetAllNodeStatesUntil(e => e.IsPop());
	public NodeZ[] NodeStatesZ => NodeStates.Select((e, i) => (e, i)).SelectToArray(t => new NodeZ(t.e, new ZOrder(ZOrderWin, false, t.i)));

	// Used for:
	//   - call invalidate when the ctrl requires it
	public Ctrl[] Ctrls => RootNode.GetAllCtrlsUntil(e => e.IsPop());

	// Used for:
	//   - determine which ctrls to call render for while rendering
	public HashSet<Ctrl> CtrlsToRender => Ctrls.Prepend(RenderCtrl).Distinct().ToHashSet();

	public R R => NodeStateId switch
	{
		null => RootNode.GetCtrlR(Set),
		not null => Set.RMap[RootNode.GetNodeState()]
	};

	public MixNode RootNode => NodeStateId switch
	{
		null => Set.Root,
		not null => Set.Lookups.NodeState2Nod[NodeStateId]
	};


	public Partition(
		NodeState? nodeStateId,
		NodeState? parentNodeStateId,
		Ctrl renderCtrl,
		int zOrderWin
	)
	{
		ParentNodeStateId = parentNodeStateId;
		RenderCtrl = renderCtrl;
		ZOrderWin = zOrderWin;
		NodeStateId = nodeStateId;
	}


	public bool Equals(Partition? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Equals(NodeStateId, other.NodeStateId);
	}
	public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Partition other && Equals(other);
	public override int GetHashCode() => NodeStateId != null ? NodeStateId.GetHashCode() : 0;
	public static bool operator ==(Partition? left, Partition? right) => Equals(left, right);
	public static bool operator !=(Partition? left, Partition? right) => !Equals(left, right);
}








/*
/// <summary>
/// Represents the portion of the partition added by the system (used for ScrollBars)
/// </summary>
sealed record SysPartition(
	IReadOnlyDictionary<NodeState, MixNode[]> Forest,
	IReadOnlyDictionary<NodeState, Ctrl[]> CtrlTriggers,
	IReadOnlyDictionary<NodeState, R> RMap
)
{
	public static readonly SysPartition Empty = new(
		new Dictionary<NodeState, MixNode[]>(),
		new Dictionary<NodeState, Ctrl[]>(),
		new Dictionary<NodeState, R>()
	);
}


static class SysPartitionExt
{
	public static IReadOnlyDictionary<NodeState, NodeState[]> GetStateLinks(this SysPartition sysPartition) =>
		sysPartition.Forest.MapValues(e => e.SelectMany(f => f.GetAllNodeStatesInTree()).ToArray());

	public static ICtrl[] GetCtrls(this SysPartition sysPartition) =>
		sysPartition.CtrlTriggers.Values.SelectMany(e => e).OfType<ICtrl>().ToArray();
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

    public ICtrl[] AllCtrls =>
	    (
		    Id switch
		    {
			    null => Root.GetCtrls(),
			    not null => Root.GetCtrls().Skip(1).ToArray(),
		    }
	    )
	    .Concat(
		    SysPartition.GetCtrls()
	    )
	    .ToArray();


    public bool IsEmpty => RMap.Count == 0;

    
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
	public static void UpdateFromPartition(this (IRwTracker<NodeZ> nodes, IRwTracker<ICtrl> ctrls) trackers, Partition partition)
	{
		trackers.nodes.Update(partition.AllNodeStates);
		trackers.ctrls.Update(partition.AllCtrls);
	}


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


file static class PartitionExtLocal
{
	public static ICtrl[] GetCtrls(this MixNode root) =>
		root
			.Select(e => e.V)
			.OfType<CtrlNode>()
			.SelectToArray(e => e.Ctrl);
}
*/
