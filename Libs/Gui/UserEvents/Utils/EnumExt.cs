/*
namespace UserEvents.Utils;

static class EnumExt
{
	public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> fun)
	{
		var i = 0;
		foreach (var elt in source)
		{
			if (fun(elt))
				return i;
			i++;
		}
		return -1;
	}
}
*/