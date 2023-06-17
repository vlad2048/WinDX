using LayoutSystem.Flex.LayStrats;
using PowBasics.Geom;

// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class FreeSzMaker
{
	public static FreeSz MkDir(Dir dir, int x, int y) => dir switch
	{
		Dir.Horz => new(x, y),
		Dir.Vert => new(y, x)
	};
	public static FreeSz DirFun(Func<Dir, int?> fun) => new(fun(Dir.Horz), fun(Dir.Vert));

	public static FreeSz FromSz(Sz sz) => new(sz.Width, sz.Height);

	public static FreeSz ForPop(Node nod)
	{
		var dim = nod.V.Dim;
		return nod.V.Strat switch
		{
			FillStrat { Spec: PopSpec } => DirFun(dir =>
				dim.Dir(dir).Typ() switch
				{
					DimType.Fit => null,
					DimType.Fix => dim.Dir(dir)!.Value.Max,
					DimType.Flt => dim.Dir(dir)!.Value.Max,
					DimType.Fil => throw new ArgumentException("Rule broken")
				}
			),
			_ => throw new ArgumentException("Not a Pop node")
		};
	}
}