using ControlSystem.Structs;

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
sealed record PartitionSet(
    Partition[] Partitions,
    IReadOnlyDictionary<NodeState, NodeState?> ParentMapping
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

    public static readonly PartitionSet Empty = new(
        new[] { Partition.Empty },
        new Dictionary<NodeState, NodeState?>()
    );
}