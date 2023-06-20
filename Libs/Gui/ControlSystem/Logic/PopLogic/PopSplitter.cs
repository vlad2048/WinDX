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
/// And CtrlSet contains the controls for which we'll call Render()
/// </summary>
sealed record LayoutPartition(
	MixNode Root,
	IReadOnlyDictionary<NodeState, R> RMap,
	HashSet<Ctrl> CtrlSet,
	object Id
);


static class PopSplitter
{
	public static (LayoutPartition, LayoutPartition[]) Split(MixNode root, IReadOnlyDictionary<NodeState, R> rMap)
	{
		var backMap = root.ToDictionary(e => e.V);
		var partitions = root.PartitionPopNodes();
		var layoutPartitions = partitions.SelectToArray((partition, partitionIdx) =>
		{
			NodeState? stateStart = partitionIdx switch
			{
				0 => null,
				_ => ((StFlexNode)partition.V).State
			};
			var partitionExtended = partition
				.ExtendToCtrlUp(backMap)
				.ExtendToCtrlDown(backMap);
			var (enabledStates, enabledCtrls) = partitionExtended.GetEnabledStatesAndCtrls(stateStart);
			var partitionRMap = rMap
				.Where(t => enabledStates.Contains(t.Key))
				.ToDictionary(e => e.Key, e => e.Value);
			return new LayoutPartition(
				partitionExtended,
				partitionRMap,
				enabledCtrls,
				(object?)stateStart ?? ((CtrlNode)partitionExtended.V).Ctrl
			);
		});

		return (
			layoutPartitions[0],
			layoutPartitions.Skip(1).ToArray()
		);
	}

	private static (HashSet<NodeState>, HashSet<Ctrl>) GetEnabledStatesAndCtrls(this MixNode root, NodeState? stateStart)
	{
		var nodeSet = new HashSet<NodeState>();
		var ctrlSet = new HashSet<Ctrl>();
		var isEnabled = stateStart == null;

		void Rec(MixNode node)
		{
			switch (node.V)
			{
				case StFlexNode { State: var state }:
					switch (isEnabled)
					{
						case true:
							if (node.V.IsPop())
								isEnabled = false;
							break;

						case false:
							if (state == stateStart)
								isEnabled = true;
							break;
					}

					if (isEnabled)
					{
						nodeSet.Add(state);
					}

					break;

				case CtrlNode { Ctrl: var ctrl }:
					if (isEnabled)
						ctrlSet.Add(ctrl);
					break;
			}

			foreach (var kid in node.Children)
				Rec(kid);
		}

		Rec(root);

		return (nodeSet, ctrlSet);
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