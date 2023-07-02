namespace LayoutSystem.Utils.Exts;

static class IntExts
{
	public static int Cap(this int v, int min, int max) => Math.Max(Math.Min(v, max), min);

	public static int EnsureNotInf(this int v) => v switch
	{
		int.MaxValue => throw new ArgumentException("Cannot be int.MaxValue"),
		_ => v
	};

	public static int SilentlyIgnoreInf(this int v) => v == int.MaxValue ? 0 : v;
}
