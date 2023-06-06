using PowBasics.Geom;

namespace LayoutDbgApp.Utils.Exts;

static class GeomExt
{
	public static Size ToSize(this Sz s) => new(s.Width, s.Height);
	public static Sz ToSz(this Size s) => new(s.Width, s.Height);

	public static Point ToPoint(this Pt p) => new(p.X, p.Y);
	public static Pt ToPt(this Point p) => new(p.X, p.Y);

	public static Rectangle ToRect(this R r) => new(r.X, r.Y, r.Width, r.Height);
	public static R ToR(this Rectangle r) => new(r.Left, r.Top, r.Width, r.Height);


	public static R Enlarge(this R r, int v)
	{
		if (v >= 0)
		{
			return new R(r.X - v, r.Y - v, r.Width + v * 2, r.Height + v * 2);
		}
		else
		{
			v = -v;
			var left = r.X + v;
			var top = r.Y + v;
			var right = r.Right - v;
			var bottom = r.Bottom - v;
			if (left > right)
				left = right = (left + right) / 2;
			if (top > bottom)
				top = bottom = (top + bottom) / 2;
			return new R(left, top, right - left, bottom - top);
		}
	}
}