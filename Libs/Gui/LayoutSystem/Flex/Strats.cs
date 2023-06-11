using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public static class Strats
{
	public static IStrat Stack(Dir mainDir, Align align) => new StackStrat(mainDir, align);
	public static IStrat Wrap(Dir mainDir) => new WrapStrat(mainDir);
	public static readonly IStrat Fill = new FillStrat(BoolVec.False);
	public static readonly IStrat ScrollX = new FillStrat(new BoolVec(true, false));
	public static readonly IStrat ScrollY = new FillStrat(new BoolVec(false, true));
	public static readonly IStrat ScrollXY = new FillStrat(new BoolVec(true, true));
}