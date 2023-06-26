using ControlSystem.Structs;
using LayoutSystem.Flex;

namespace ControlSystem;

public sealed class StFlexNodeFluent : FlexNodeFluent
{
	internal NodeState NodeState { get; }

	public StFlexNodeFluent(NodeState nodeState)
	{
		NodeState = nodeState;
	}
}


public static class FlexMaker
{
	public static StFlexNodeFluent F(NodeState nodeState) => new(nodeState);

	public static StFlexNode StBuild(this FlexNodeFluent f) => new(((StFlexNodeFluent)f).NodeState, f.Build());
}