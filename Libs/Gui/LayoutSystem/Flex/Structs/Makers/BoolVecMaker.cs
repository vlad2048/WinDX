using PowBasics.Geom;

// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class BoolVecMaker
{
	public static BoolVec Make(Dir dir, bool x, bool y) => dir switch
	{
		Dir.Horz => new(x, y),
		Dir.Vert => new(y, x),
	};
}