using LayoutSystem.Flex;

namespace ControlSystem.Structs;


public interface IMixNode { }

public sealed record CtrlNode(Ctrl Ctrl) : IMixNode
{
	public override string ToString() => Ctrl.GetType().Name;
}

public sealed record StFlexNode(NodeState State, FlexNode Flex) : IMixNode
{
	public override string ToString() => $"[R:{State.R}] - {Flex}";
}
