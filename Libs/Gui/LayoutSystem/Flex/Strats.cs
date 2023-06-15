using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public static class Strats
{
	public static IStrat Stack(Dir mainDir, Align align) => new StackStrat(mainDir, align);
	public static IStrat Wrap(Dir mainDir) => new WrapStrat(mainDir);
	public static readonly IStrat Fill = new FillStrat(new ScrollSpec(new BoolVec(false, false)));
	public static readonly IStrat ScrollX = new FillStrat(new ScrollSpec(new BoolVec(true, false)));
	public static readonly IStrat ScrollY = new FillStrat(new ScrollSpec(new BoolVec(false, true)));
	public static readonly IStrat ScrollXY = new FillStrat(new ScrollSpec(new BoolVec(true, true)));
	public static readonly IStrat Pop = new FillStrat(new PopSpec());
}