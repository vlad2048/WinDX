namespace ControlSystem.Utils;

static class CapExt
{
	public static int Cap(this int v, int? max = null) => max switch
	{
		not null => Math.Max(0, Math.Min(v, max.Value)),
		null => Math.Max(0, v)
	};

	public static int CapMin(this int v, int min) => Math.Max(min, v);
}