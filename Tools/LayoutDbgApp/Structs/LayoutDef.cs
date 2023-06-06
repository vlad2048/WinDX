using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutDbgApp.Structs;

record LayoutDef(
	FreeSz WinSize,
	Node Root
)
{
	public static readonly LayoutDef Default = new(
		new FreeSz(150, 200),
		M(Vec.Fil, Stack(Dir.Vert, Align.Start))
	);
}
