using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStrats;

public class ScrollStrat : IStrat
{
	public BoolVec CanScroll { get; }

	public ScrollStrat(BoolVec canScroll)
	{
		CanScroll = canScroll;
	}

	public override string ToString() => $"Scroll({CanScroll})";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		throw new NotImplementedException();
	}
}
