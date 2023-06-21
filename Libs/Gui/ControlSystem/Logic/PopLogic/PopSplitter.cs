using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace ControlSystem.Logic.PopLogic;


/// <summary>
/// The Root will always be a Ctrl
/// The On/Off status of the nodes is encoded into the keys present in RMap
/// CtrlSet are all the Ctrls contained in Root (these will be the ones we call Render for)
/// </summary>
record Partition(
	MixNode Root,
	IReadOnlyDictionary<NodeState, R> RMap,
	HashSet<Ctrl> CtrlSet
)
{
	public Ctrl RootCtrl => ((CtrlNode)Root.V).Ctrl;
}

sealed record SubPartition(
	MixNode Root,
	IReadOnlyDictionary<NodeState, R> RMap,
	HashSet<Ctrl> CtrlSet,
	NodeState Id
) : Partition(Root, RMap, CtrlSet);


static class PopSplitter
{
	public static (Partition, SubPartition[]) Split(MixNode root, IReadOnlyDictionary<NodeState, R> rMap)
	{
		var backMap = root.ToDictionary(e => e.V);
		var parts = root
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
				return (
					partitionExtended,
					partitionRMap,
					ctrlSet,
					stateStart
				);
			});


		return (
			new Partition(
				parts[0].partitionExtended,
				parts[0].partitionRMap,
				parts[0].ctrlSet
			),
			parts.Skip(1).SelectToArray(part => new SubPartition(
				part.partitionExtended,
				part.partitionRMap,
				part.ctrlSet,
				part.stateStart ?? throw new ArgumentException("A subpartition should have an associated NodeState")
			))
		);
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
	


	private static bool IsPop(this IMixNode node) => node is StFlexNode { Flex.Strat: FillStrat { Spec: PopSpec } };
	//private static bool IsCtrl(this IMixNode node) => node is CtrlNode;
	private static bool IsCtrl(this MixNode node) => node.V is CtrlNode;


	private static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> source)
	{
		foreach (var elt in source)
			queue.Enqueue(elt);
	}
}