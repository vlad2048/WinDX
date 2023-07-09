using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.CollectionsExt;
using PowRxVar;

namespace ControlSystem.Logic.Popup_.Structs;

/// <summary>
/// Contains all the layout partitions across the main window and the popup windows
/// </summary>
/// <param name="Partitions">
/// Partitions: <br/>
/// ● The first one is associated with the main window <br/>
/// ● The subsequent ones are associated with the popup windows
/// </param>
/// <param name="ParentMapping">
/// Mapping from the Partitions Ids (NodeState) to the Partitions Ids (NodeState) (or null to reference the main window)
/// </param>
/// <param name="MixLayout">
/// Identical for all partitions, the full MixLayout
/// </param>
sealed record PartitionSet(
    Partition[] Partitions,
    IReadOnlyDictionary<NodeState, NodeState?> ParentMapping,
    MixLayout MixLayout
)
{
    public Partition MainPartition => Partitions[0];

    public Partition[] SubPartitions => Partitions.Skip(1).ToArray();

    public Ctrl[] AllCtrls => (
            from partition in Partitions
            from ctrl in partition.CtrlSet
            select ctrl
        )
        .Distinct()
        .ToArray();

    public TNod<Partition> PartitionTree
    {
	    get
	    {
		    var root = Nod.Make(MainPartition);
		    var nodes = SubPartitions.SelectToArray(e => Nod.Make(e));
		    var nodeMap = nodes.ToDictionary(e => e.V.Id ?? throw new ArgumentException("Id cannot be null for a sub partition"));
		    foreach (var (ns, nsParent) in ParentMapping)
		    {
			    var nodeKid = nodeMap[ns];
			    var nodeDad = nsParent switch
			    {
				    null => root,
				    not null => nodeMap[nsParent]
			    };
				nodeDad.AddChild(nodeKid);
		    }
		    return root;
	    }
    }


    private static readonly StFlexNode emptyStFlexNode = new StFlexNode(
	    new NodeState().DisposeOnProgramExit(),
	    new FlexNode(
		    new DimVec(null, null),
		    FlexFlags.None,
		    Strats.Fill,
		    Mg.Zero
	    )
    );


    public static readonly PartitionSet Empty = new(
        new[] { Partition.Empty },
        new Dictionary<NodeState, NodeState?>(),
		MixLayout.Empty
    );
}