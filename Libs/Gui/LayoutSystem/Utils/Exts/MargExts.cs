using PowBasics.Geom;

namespace LayoutSystem.Utils.Exts;

public static class MargExts
{
	public static Marg Enlarge(this Marg m, int v) => (v >= 0) switch
	{
		truer => new Marg(
			m.Top + v,
			m.Right + v,
			m.Bottom + v,
			m.Left + v
		),
		false => new Marg(
			Math.Max(0, m.Top + v),
			Math.Max(0, m.Right + v),
			Math.Max(0, m.Bottom + v),
			Math.Max(0, m.Left + v)
		)
	};
}