namespace LayoutSystem.Utils.Exts;

static class CapExts
{
	public static int Cap(this int v, int min, int max) => Math.Max(Math.Min(v, max), min);
}
