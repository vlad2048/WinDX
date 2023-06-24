using System.Diagnostics.CodeAnalysis;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using UserEvents;

namespace ControlSystem.Logic.PopupLogic;



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
record Partition(
	NodeState? Id,
	MixNode Root,
	IReadOnlyDictionary<NodeState, R> RMap,
	HashSet<Ctrl> CtrlSet
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

	public Maybe<INodeStateUserEventsSupport> FindNodeAtMouseCoordinates(Pt pt) =>
		AllNodeStates
			.Where(state => RMap[state].Contains(pt))
			.Reverse()
			.OfType<INodeStateUserEventsSupport>()
			.FirstOrMaybe();


	private static readonly Ctrl emptyCtrl = new Ctrl().DisposeOnProgramExit();

	public static readonly Partition Empty = new(
		null,
		Nod.Make<IMixNode>(new CtrlNode(emptyCtrl)),
		new Dictionary<NodeState, R>(),
		new HashSet<Ctrl>{emptyCtrl}
	);
}



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

	public static readonly PartitionSet Empty = new(
		new [] { Partition.Empty },
		new Dictionary<NodeState, NodeState?>()
	);
}

static class PopupSplitter
{
	public static PartitionSet Split(MixNode root, IReadOnlyDictionary<NodeState, R> rMap)
	{
		var backMap = root.ToDictionary(e => e.V);
		var partitions = root
			.PartitionPopNodes()
			.SelectToArray((partition, partitionIdx) =>
			{
				NodeState? stateStart = partitionIdx switch
				{
					0 => null,
					_ => ((StFlexNode)partition.V).State
				};
				var partitionExtended = partition
					.ExtendToCtrlUp(backMap)
					.ExtendToCtrlDown(backMap);
				var enabledStates = partitionExtended.GetEnabledStates(stateStart);
				var partitionRMap = rMap
					.Where(t => enabledStates.Contains(t.Key))
					.ToDictionary(e => e.Key, e => e.Value);
				var ctrlSet = partitionExtended.Select(e => e.V).OfType<CtrlNode>().ToHashSet(e => e.Ctrl);
				return new Partition(
					stateStart,
					partitionExtended,
					partitionRMap,
					ctrlSet
				);
			});


		return new PartitionSet(
			partitions,
			root.GetParentMapping()
		);
	}


	private static IReadOnlyDictionary<NodeState, NodeState?> GetParentMapping(this MixNode root)
	{
		var map = new Dictionary<NodeState, NodeState?>();
		void Rec(MixNode node, NodeState? parentNodeState)
		{
			if (node.IsPop(out var nodeState))
			{
				map[nodeState] = parentNodeState;
				parentNodeState = nodeState;
			}
			foreach (var kid in node.Children)
				Rec(kid, parentNodeState);
		}
		Rec(root, null);
		return map;
	}


	private enum Status
	{
		NotStarted,
		Started,
		Finished
	}

	private static HashSet<NodeState> GetEnabledStates(this MixNode root, NodeState? stateStart)
	{
		var nodeSet = new HashSet<NodeState>();
		

		void Rec(MixNode node, Status status_)
		{
			status_ = node.V switch
			{
				StFlexNode { State: var state } =>
					status_ switch
					{
						Status.Started when node.V.IsPop() => Status.Finished,
						Status.NotStarted when state == stateStart => Status.Started,
						_ => status_
					},
				_ => status_
			};

			if (status_ == Status.Started && node.V is StFlexNode { State: var state_ })
				nodeSet.Add(state_);

			if (status_ == Status.Finished)
				return;

			foreach (var kid in node.Children)
				Rec(kid, status_);
		}

		var status = stateStart switch
		{
			null => Status.Started,
			not null => Status.NotStarted
		};
		Rec(root, status);

		return nodeSet;
	}

	internal static MixNode ExtendToCtrlUp(this MixNode rootPart, IReadOnlyDictionary<IMixNode, MixNode> backMap)
	{
		var rootOrig = backMap[rootPart.V];
		while (!rootOrig.IsCtrl())
		{
			var rootOrigUp = rootOrig.Parent ?? throw new ArgumentException("Cannot be null");
			rootPart = Nod.Make(rootOrigUp.V, rootPart);
			rootOrig = rootOrigUp;
		}
		return rootPart;
	}

	internal static MixNode ExtendToCtrlDown(this MixNode rootPartition, IReadOnlyDictionary<IMixNode, MixNode> backMap)
	{
		var rootOrig = backMap[rootPartition.V];

		void Rec(MixNode nodePart, MixNode nodeOrig)
		{
			var kidsPart = nodePart.Children;
			var kidsOrig = nodeOrig.Children;

			var kidsPartNext = kidsOrig
				.SelectToArray(kidOrig => kidsPart.Any(e => e.V == kidOrig.V) switch
				{
					true => kidsPart.First(e => e.V == kidOrig.V),
					false => kidOrig.IsCtrl() switch
					{
						true => null,
						false => Nod.Make(kidOrig.V)
					}
				})
				.WhereToArray(e => e != null)
				.SelectToArray(e => e!);
			nodePart.ClearChildren();
			nodePart.AddChildren(kidsPartNext);

			foreach (var kidPart in nodePart.Children)
			{
				var kidOrig = nodeOrig.Children.First(e => e.V == kidPart.V);
				Rec(kidPart, kidOrig);
			}
		}

		Rec(rootPartition, rootOrig);

		return rootPartition;
	}

	internal static MixNode[] PartitionPopNodes(this MixNode root)
	{
		var list = new List<MixNode>();

		var queue = new Queue<MixNode>();
		queue.Enqueue(root);

		void BuildPartition(MixNode node)
		{
			var (partitionRoot, boundary) = node.ExtendDownToAndExcluding(e => e.IsPop());
			queue.EnqueueRange(boundary);
			list.Add(partitionRoot);
		}

		while (queue.TryDequeue(out var partitionRoot))
			BuildPartition(partitionRoot);

		return list.ToArray();
	}



	private static bool IsPop(this MixNode node, [NotNullWhen(true)] out NodeState? nodeState)
	{
		if (node.V is StFlexNode { State: var nodeState_, Flex.Strat: FillStrat { Spec: PopSpec } })
		{
			nodeState = nodeState_;
			return true;
		}
		nodeState = null;
		return false;
	}

	private static bool IsCtrl(this MixNode node) => node.V is CtrlNode;

	private static bool IsPop(this IMixNode node) => node is StFlexNode { Flex.Strat: FillStrat { Spec: PopSpec } };
	//private static bool IsCtrl(this IMixNode node) => node is CtrlNode;


	private static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> source)
	{
		foreach (var elt in source)
			queue.Enqueue(elt);
	}
}