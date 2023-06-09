using PowBasics.Geom;

// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class GeomMaker
{
	public static Pt MkPtDir(Dir dir, int x, int y) => dir switch
	{
		Dir.Horz => new(x, y),
		Dir.Vert => new(y, x),
	};

	public static Sz MkSzDir(Dir dir, int x, int y) => dir switch
	{
		Dir.Horz => new(x, y),
		Dir.Vert => new(y, x),
	};

	public static Pt PtDirFun(Func<Dir, int> fun) => new(fun(Dir.Horz), fun(Dir.Vert));
	public static Sz SzDirFun(Func<Dir, int> fun) => new(fun(Dir.Horz), fun(Dir.Vert));
}