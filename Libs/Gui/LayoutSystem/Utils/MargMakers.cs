using PowBasics.Geom;

namespace LayoutSystem.Utils;

public static class Mg
{
	public static readonly Marg Zero = new();
	public static Marg Mk(int v) => new(v, v, v, v);
	public static Marg Dirs(int h, int v) => new(v, h, v, h);
}