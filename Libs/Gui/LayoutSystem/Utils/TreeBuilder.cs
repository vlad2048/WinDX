using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Utils;

public static class TreeBuilder
{
	public static Node M(DimVec dim, IStrat? strat = null, params Node[] kids) =>
		Nod.Make(new FlexNode(dim, strat ?? Strats.Fill, Mg.Zero), kids);

	public static TNod<R> A(int x, int y, int width, int height, params TNod<R>[] kids) =>
		Nod.Make(new R(x, y, width, height), kids);
}