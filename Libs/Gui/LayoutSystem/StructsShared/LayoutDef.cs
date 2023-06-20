using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using static LayoutSystem.Utils.TreeBuilder;

namespace LayoutSystem.StructsShared;

sealed record LayoutDef(
	FreeSz WinSize,
	Node Root
)
{
	public static readonly LayoutDef Default = new(
		new FreeSz(150, 200),
		M(Vec.Fil, Strats.Stack(Dir.Horz, Align.Start))
	);
}
