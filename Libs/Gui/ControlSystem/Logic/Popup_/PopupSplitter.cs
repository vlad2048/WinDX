using System.Diagnostics.CodeAnalysis;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using ControlSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace ControlSystem.Logic.Popup_;

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
				var nodeMap = partitionExtended
					.Where(e => e.V is StFlexNode { State: var state } && enabledStates.Contains(state))
					.ToDictionary(e => ((StFlexNode)e.V).State);
				var partitionRMap = rMap
					.Where(t => enabledStates.Contains(t.Key))
					.ToDictionary(e => e.Key, e => e.Value);
				var ctrlSet = partitionExtended.Select(e => e.V).OfType<CtrlNode>().ToHashSet(e => e.Ctrl);
				return new Partition(
					stateStart,
					partitionExtended,
					nodeMap,
					partitionRMap,
					ctrlSet,
					PartitionScrollBars.Empty
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
		if (node.V is StFlexNode { Flex.Flags.Pop: true, State: var nodeState_ })
		{
			nodeState = nodeState_;
			return true;
		}
		else
		{
			nodeState = null;
			return false;
		}
	}

	private static bool IsCtrl(this MixNode node) => node.V is CtrlNode;
	private static bool IsPop(this IMixNode node) => node is StFlexNode { Flex.Flags.Pop: true };


	private static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> source)
	{
		foreach (var elt in source)
			queue.Enqueue(elt);
	}
}