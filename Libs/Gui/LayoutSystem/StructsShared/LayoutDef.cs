using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;

namespace LayoutSystem.StructsShared;

sealed record LayoutDef(
	FreeSz WinSize,
	Node Root
)
{
	public static readonly LayoutDef Default = new(
		new FreeSz(150, 200),
		Nod.Make(new FlexNode(Vec.Fil, FlexFlags.None, Strats.Stack(Dir.Horz, Align.Start), Mg.Zero))
	);
}
