using PowBasics.Geom;

// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class FDimVecMaker
{
	public static FDimVec MkDir(Dir dir, FDim x, FDim y) => dir switch
	{
		Dir.Horz => new(x, y),
		Dir.Vert => new(y, x),
	};

	public static FDimVec DirFun(Func<Dir, FDim> fun) => new(fun(Dir.Horz), fun(Dir.Vert));
}