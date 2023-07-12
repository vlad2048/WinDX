using PowBasics.Geom;

namespace WinSpectorLib.Utils;

static class GeomExt
{
	public static Sz ToSz(this (int, int) t) => new(t.Item1, t.Item2);
}