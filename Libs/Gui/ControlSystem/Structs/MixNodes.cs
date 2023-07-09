using LayoutSystem.Flex;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace ControlSystem.Structs;


public interface IMixNode { }

public sealed record CtrlNode(Ctrl Ctrl) : IMixNode
{
	public override string ToString() => Ctrl.GetType().Name;
}

public sealed record StFlexNode(NodeState State, FlexNode Flex) : IMixNode
{
	public override string ToString() => $"{Flex}";
}

public sealed record TextMeasureNode(Sz Size) : IMixNode
{
	public override string ToString() => $"text:{Size}";
}


static class MixNodeExt
{
	public static NodeState[] GetAllNodeStatesInTree(this MixNode root) =>
		root
			.Where(e => e.V is StFlexNode)
			.SelectToArray(e => ((StFlexNode)e.V).State);

	public static Ctrl[] GetAllCtrlsInTree(this MixNode root) =>
		root
			.Where(e => e.V is CtrlNode)
			.SelectToArray(e => ((CtrlNode)e.V).Ctrl);

	public static IReadOnlyDictionary<NodeState, MixNode> Make_NodeState_2_MixNode_Map(this MixNode root) =>
		root
			.Where(e => e.V is StFlexNode)
			.ToDictionary(e => ((StFlexNode)e.V).State);

	public static IReadOnlyDictionary<Ctrl, MixNode> Make_Ctrl_2_MixNode_Map(this MixNode root) =>
		root
			.Where(e => e.V is CtrlNode)
			.ToDictionary(e => ((CtrlNode)e.V).Ctrl);
}