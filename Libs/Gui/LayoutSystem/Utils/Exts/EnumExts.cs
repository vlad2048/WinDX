using LayoutSystem.Flex.Structs;

namespace LayoutSystem.Utils.Exts;

static class EnumExt
{
	// Max(Total function)
	public static int MaxT(this IEnumerable<int> source)
	{
		var max = 0;
		foreach (var elt in source)
			if (elt > max)
				max = elt;
		return max;
	}

	public static int MaxT<T>(this IEnumerable<T> source, Func<T, int> fun) =>
		source
			.Select(fun)
			.MaxT();

	public static int[] GetIndicesWhere<T>(this T[] arr, Func<T, bool> fun) => arr.Select((e, i) => (e, i)).Where(t => fun(t.e)).Map(t => t.i);

	public static int FindIndex<T>(this IEnumerable<T> source, T obj) where T : class
	{
		var index = 0;
		foreach (var elt in source)
		{
			if (elt == obj)
				return index;
			index++;
		}
		throw new ArgumentException();
	}

	public static IEnumerable<T> SkipIndex<T>(this IEnumerable<T> source, int index) =>
		source
			.Select((e, i) => (e, i))
			.Where(t => t.i != index)
			.Select(t => t.e);

	public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
	{
		var i = 0;
		foreach (var elt in source)
			action(elt, i++);
	}

	public static U[] Map<T, U>(this IEnumerable<T> source, Func<T, U> fun) => source.Select(fun).ToArray();
	public static U[] Map<T, U>(this IEnumerable<T> source, Func<T, int, U> fun) => source.Select(fun).ToArray();
	public static T[] Filt<T>(this IEnumerable<T> source, Func<T, bool> fun) => source.Where(fun).ToArray();

	/// <summary>
	/// Equivalent to FoldL but returns all the intermediate accumulators
	/// but not the last one
	/// 
	/// example:
	/// new [] { 3, 9, 7, 10 }.ScanL((a, b) => a + b, 0)
	/// returns: [0, 3, 12, 19]
	/// </summary>
	public static U[] ScanL<T, U>(this IEnumerable<T> source, Func<U, T, U> fun, U seed)
	{
		var accumulation = new List<U>();

		var current = seed;
		foreach (var item in source)
		{
			accumulation.Add(current);
			current = fun(current, item);
		}

		return accumulation.ToArray();
	}

	//public static IEnumerable<U> Zip<T1, T2, T3, U>(this IEnumerable<T1> t1s, IEnumerable<T2> t2s, IEnumerable<T3> t3s, Func<T1, T2, T3, U> fun)
	//{
	//	using var e1 = t1s.GetEnumerator();
	//	using var e2 = t2s.GetEnumerator();
	//	using var e3 = t3s.GetEnumerator();
	//	while (e1.MoveNext() && e2.MoveNext() && e3.MoveNext())
	//		yield return fun(e1.Current, e2.Current, e3.Current);
	//}
}