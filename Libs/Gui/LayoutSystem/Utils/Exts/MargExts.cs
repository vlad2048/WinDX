using PowBasics.Geom;

namespace LayoutSystem.Utils.Exts;

/*public static class MargUtils
{
	public static (Pt, FreeSz) ApplyMargin(FreeSz freeSz, Marg marg)
	{
		int MkDir(Dir dir) => freeSz.IsInfinite(dir) switch
		{
			truer => int.MaxValue,
			false => Math.Max(0, freeSz.Dir(dir) - marg.Dir(dir))
		};
		return (
			new Pt(marg.Left, marg.Top),
			new FreeSz(
				MkDir(Dir.Horz),
				MkDir(Dir.Vert)
			)
		);
	}


	public static DimVec AddMargin(this DimVec v, Marg m)
	{
		Dim Mk(Dir dir)
		{
			var d = v.Dir(dir);
			return d.Type switch
			{
				DType.Fix => D.Fix(d.Min + m.Dir(dir)),
				DType.Flt => D.Flt(d.Min + m.Dir(dir), d.Max + m.Dir(dir)),
				DType.Fil => D.Fil,
				_ => throw new ArgumentException("Shouldn't happen")
			};
		}
		return new DimVec(
			Mk(Dir.Horz),
			Mk(Dir.Vert)
		);
	}
}*/

public static class MargExts
{
	public static Marg Enlarge(this Marg m, int v)
	{
		if (v >= 0)
		{
			return new Marg(
				m.Top + v,
				m.Right + v,
				m.Bottom + v,
				m.Left + v
			);
		}
		else
		{
			v = -v;
			return new Marg(
				Math.Max(0, m.Top - v),
				Math.Max(0, m.Right - v),
				Math.Max(0, m.Bottom - v),
				Math.Max(0, m.Left - v)
			);
		}
	}
}