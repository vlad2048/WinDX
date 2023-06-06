using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Utils;

public static class TreeBuilder
{
	public static Node M(DimOptVec dim, IStrat? strat = null, params Node[] kids) =>
		Nod.Make(new FlexNode(dim, Mg.Zero, strat ?? Strats.Fill), kids);

	public static TNod<R> R(int x, int y, int width, int height, params TNod<R>[] kids) =>
		Nod.Make(new R(x, y, width, height), kids);
}