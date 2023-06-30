using PowBasics.Geom;

namespace TestBase;

public static class TestFmtExt
{
	public static string fmt(this R r) =>
		$"{r.X}".PadLeft(3) + "," +
		$"{r.Y}".PadLeft(3) + " " +
		$"{r.Width}".PadLeft(3) + "x" +
		$"{r.Height}".PadLeft(3);
}