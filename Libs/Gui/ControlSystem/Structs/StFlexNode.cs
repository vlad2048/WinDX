using LayoutSystem.Flex;
using PowMaybe;

namespace ControlSystem.Structs;


public record StFlexNode
{
	public NodeState State { get; init; }
	public FlexNode Flex { get; init; }
	public Maybe<Ctrl> Ctrl { get; init; }

	public StFlexNode(NodeState state, FlexNode flex)
	{
		State = state;
		Flex = flex;
		Ctrl = May.None<Ctrl>();
	}

	public StFlexNode(Ctrl ctrl)
	{
		State = ctrl.RootState;
		Flex = ctrl.RootFlex;
		Ctrl = May.Some(ctrl);
	}
	
	public override string ToString() => $"[R:{State.R}] - {Flex}";
}
