using System.Reactive;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using System.Windows.Forms;
using PowBasics.ColorCode.Utils;
using PowMaybe;
using PowMaybeErr;
using UserEvents;
using UserEvents.Structs;

namespace ControlSystem.Logic.Popup_.Structs;


sealed record PartitionSetWinSpectorNfo(
	FreeSz WinSize,
	Win MainWin,
	IReadOnlyDictionary<NodeState, FlexWarning> WarningMap,
	IReadOnlyDictionary<Ctrl, IMixNode> UnbalancedCtrls
);


sealed class PartitionSet
{
	public MixNode Root { get; }
	public Dictionary<NodeState, R> RMap { get; }
	public TNod<Partition> PartitionRoot { get; }
	public Dictionary<NodeState, Ctrl[]> ExtraCtrlsToRenderOnPop { get; } = new();
	public PartitionSetWinSpectorNfo Nfo { get; }


	// **********************
	// * Derived Properties *
	// **********************
	public Partition MainPartition => PartitionRoot.V;
	public Partition[] Partitions => PartitionRoot.SelectToArray(e => e.V);
	public Partition[] PopupPartitions => Partitions.Skip(1).ToArray();
	public LookupMaps Lookups { get; private set; }


	public PartitionSet(MixNode root, Dictionary<NodeState, R> rMap, TNod<Partition> partitionRoot, PartitionSetWinSpectorNfo nfo)
	{
		Root = root;
		RMap = rMap;
		PartitionRoot = partitionRoot;
		Nfo = nfo;
		foreach (var partition in Partitions)
			partition.Set = this;
		Lookups = new LookupMaps(Root);
	}

	public void RecomputeLookupMaps() => Lookups = new LookupMaps(Root);

	public class LookupMaps
	{
		public IReadOnlyDictionary<NodeState, MixNode> NodeState2Nod { get; }
		public IReadOnlyDictionary<Ctrl, MixNode> Ctrl2Nod { get; }

		public LookupMaps(MixNode root)
		{
			NodeState2Nod = root.Where(e => e.IsNodeState()).ToDictionary(e => e.GetNodeState());
			Ctrl2Nod = root.Where(e => e.IsCtrl()).ToDictionary(e => e.GetCtrl());
		}
	}
}


static class PartitionExt
{
	public static void UpdateFromPartition(this (IRwTracker<NodeZ> nodes, IRwTracker<ICtrl> ctrls) trackers, Partition partition)
	{
		trackers.nodes.Update(partition.NodeStatesZ);
		trackers.ctrls.Update(partition.Ctrls.OfType<ICtrl>().ToArray());
	}


	public static PartitionSet VerifyInvariants(this PartitionSet set, int step)
	{
		var ok =
			from _1 in VerifyPartition(
				set.Root.GetAllNodeStates(),
				set.Partitions.SelectMany(e => e.NodeStates).ToArray()
			)
			from _2 in VerifyPartition(
				set.Root.GetAllCtrls(),
				set.Partitions.SelectMany(e => e.Ctrls).ToArray()
			)
			from _3 in VerifyAreTheSameNotOrdered(
				set.Root.GetAllNodeStates(),
				set.RMap.Keys.ToArray()
			)
			select _3;

		if (ok.IsNone(out _, out var err))
		{
			var txt = set.PrettyPrint();
			txt.PrintToConsole();
			//txt.RenderToHtml(@"C:\tmp\fmt\set.html", typeof(PartitionSetPrettyPrinter.C));
			throw new ArgumentException(err);
		}

		return set;
	}


	private static MaybeErr<Unit> VerifyPartition<T>(T[] allItems, T[] subItems)
	{
		var t = typeof(T).Name;

		var allItemsUniq = allItems.Distinct().ToArray();
		if (allItemsUniq.Length != allItems.Length) return MayErr.None<Unit>($"Invariant broken. {t} allItems are not distinct");

		var subItemsUniq = subItems.Distinct().ToArray();
		if (subItemsUniq.Length != subItems.Length) return MayErr.None<Unit>($"Invariant broken. {t} subItems are not distinct (there is overlap)");

		var subOnly = subItems.WhereToArray(e => !allItems.Contains(e));
		var allOnly = allItems.WhereToArray(e => !subItems.Contains(e));
		if (subOnly.Length > 0) return MayErr.None<Unit>($"Invariant broken. {t} do not form a partition (the whole is smaller than the parts)");
		if (allOnly.Length > 0) return MayErr.None<Unit>($"Invariant broken. {t} do not form a partition (the whole is bigger than the parts)");
		if (subItems.Length != allItems.Length) return MayErr.None<Unit>($"Invariant broken. {t} do not form a partition (length do not match)");
		return MayErr.Some(Unit.Default);
	}

	private static MaybeErr<Unit> VerifyAreTheSameNotOrdered<T>(T[] us, T[] vs)
	{
		var t = typeof(T).Name;
		if (!AreDistinct(us)) return MayErr.None<Unit>($"{t} are not distinct (1)");
		if (!AreDistinct(vs)) return MayErr.None<Unit>($"{t} are not distinct (2)");
		var usOnly = us.WhereNotToArray(vs.Contains);
		var vsOnly = vs.WhereNotToArray(us.Contains);
		if (usOnly.Length > 0) return MayErr.None<Unit>($"{t} more us than vs");
		if (vsOnly.Length > 0) return MayErr.None<Unit>($"{t} more vs than us");
		return MayErr.Some(Unit.Default);
	}

	private static bool AreDistinct<T>(T[] arr) => arr.Distinct().ToArray().Length == arr.Length;
}




/*
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
*/