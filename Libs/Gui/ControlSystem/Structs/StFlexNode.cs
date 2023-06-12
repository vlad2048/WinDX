using LayoutSystem.Flex;

namespace ControlSystem.Structs;


public interface IMixNode { }

public record CtrlNode(Ctrl Ctrl) : IMixNode
{
	public override string ToString() => Ctrl.GetType().Name;
}

public record StFlexNode(NodeState State, FlexNode Flex) : IMixNode
{
	public override string ToString() => $"[R:{State.R}] - {Flex}";
}
