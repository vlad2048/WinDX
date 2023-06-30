using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Tests.Flex.TestSupport;

static class BuildUtils
{
	public static FreeSz Win(int? width, int? height) => new(width, height);

	public static TNod<FlexNodeFluent> N(FlexNodeFluent f, params TNod<FlexNodeFluent>[] kids) => Nod.Make(f, kids);

	public static FlexNodeFluent F => new();

	public static TNod<R> R(int x, int y, int width, int height, params TNod<R>[] kids) => Nod.Make(new R(x, y, width, height), kids);
}