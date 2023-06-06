using LayoutSystem.Flex.LayStrats;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public static class Strats
{
	public static IStrat Stack(Dir mainDir, Align align) => new StackStrat(mainDir, align);
	public static IStrat Wrap(Dir mainDir) => new WrapStrat(mainDir);
	public static readonly IStrat Fill = new FillStrat();
}