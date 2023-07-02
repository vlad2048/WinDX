using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStratsUtils_;

static class LayStratsUtils
{
	public static Sz ComputeSz(
		DimVec dim,
		FreeSz freeSz,
		Func<Dir, int> naturalDimFun
	)
	{
		var sz = GeomMaker.SzDirFun(
			dir =>
			{
				var dimDir = dim.Dir(dir);
				var dimTyp = dimDir.Typ();
				return (dimTyp == DimType.Fit || !freeSz.Dir(dir).HasValue) switch
				{
					truer => naturalDimFun(dir).CapWith(freeSz.Dir(dir)),
					false => dimTyp switch
					{
						DimType.Fil => freeSz.Dir(dir)!.Value,
						_ => dimDir!.Value.Max.EnsureNotInf().CapWith(freeSz.Dir(dir)),
					}
				};
			});

		if (sz.Width == int.MaxValue || sz.Height == int.MaxValue)
			throw new ArgumentException("Impossible");

		return sz;
	}

	private static int CapWith(this int v, int? f) => f switch
	{
		null => v,
		not null => Math.Min(v, f.Value)
	};
}