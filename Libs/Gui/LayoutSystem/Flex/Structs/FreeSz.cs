using System.Diagnostics;
using PowBasics.Geom;

namespace LayoutSystem.Flex.Structs;


[DebuggerStepThrough]
public readonly record struct FreeSz(int? X, int? Y)
{
	public override string ToString()
	{
		static string Fmt(int? v) => v.HasValue ? $"{v}" : "inf";
		return $"{Fmt(X)} x {Fmt(Y)}";
	}
}



public static class FreeSzExts
{
	internal static Sz CapWith(this Sz sz, FreeSz free) => GeomMaker.SzDirFun(dir => 
		free.Dir(dir).HasValue switch
		{
			true => Math.Min(free.Dir(dir)!.Value, sz.Dir(dir)),
			false => sz.Dir(dir)
		}
	);

	internal static FreeSz UnbridleScrolls(this FreeSz free, Node node) =>
		FreeSzMaker.DirFun(dir =>
			node.V.Flags.Scroll.Dir(dir) switch
			{
				truer => null,
				false => free.Dir(dir)
			}
		);

	internal static DimVec ToDim(this FreeSz free) => Vec.DirFun(dir => 
		free.Dir(dir).HasValue switch
		{
			truer => D.Fix(free.Dir(dir)!.Value),
			false => D.Fit
		}
	);

	public static FreeSz ChangeComponent(this FreeSz free, Dir dir, int? v) => dir switch
	{
		Dir.Horz => free with { X = v },
		Dir.Vert => free with { Y = v },
	};
}
