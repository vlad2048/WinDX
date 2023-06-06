using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStrats;

public class ScrollStrat : IStrat
{
	public VecBool CanScroll { get; }

	public ScrollStrat(VecBool canScroll)
	{
		CanScroll = canScroll;
	}

	public override string ToString() => "Scroll";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		DimVec[] kidDims
	)
	{

	}
}


static class Foo
{
	public static void Do()
	{
		var v = new DimVec(D.Fil, D.Fil);
		var c = v.Dir(Dir.Horz);
	}
}