using PowBasics.Geom;

namespace FlexBuilder.Utils.Exts;

static class GeomExt
{
	public static Size ToSize(this Sz s) => new(s.Width, s.Height);
	public static Sz ToSz(this Size s) => new(s.Width, s.Height);

	public static Point ToPoint(this Pt p) => new(p.X, p.Y);
	public static Pt ToPt(this Point p) => new(p.X, p.Y);

	public static Rectangle ToRect(this R r) => new(r.X, r.Y, r.Width, r.Height);
	public static R ToR(this Rectangle r) => new(r.Left, r.Top, r.Width, r.Height);
}