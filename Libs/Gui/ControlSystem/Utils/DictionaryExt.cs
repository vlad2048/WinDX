using PowBasics.CollectionsExt;

namespace ControlSystem.Utils;

public static class DictionaryExt
{
	public static IReadOnlyDictionary<K, V> Merge<K, V>(this IEnumerable<IReadOnlyDictionary<K, V>> source) where K : notnull
	{
		var res = new Dictionary<K, V>();
		foreach (var dict in source)
		{
			foreach (var (key, val) in dict)
				res[key] = val;
		}
		return res;
	}

	internal static IReadOnlyDictionary<K, V[]> Merge<K, V>(this IEnumerable<IReadOnlyDictionary<K, V[]>> source) where K : notnull
	{
		var res = new Dictionary<K, List<V>>();
		foreach (var dict in source)
		{
			foreach (var (key, vals) in dict)
			foreach (var val in vals)
				res.AddToDictionaryList(key, val);
		}
		return res.MapValues(e => e.ToArray());
	}

	internal static void AddToDictionaryList<K, V>(this IDictionary<K, List<V>> dict, K key, V val) where K : notnull
	{
		if (!dict.TryGetValue(key, out var list))
			list = dict[key] = new List<V>();
		list.Add(val);
	}
}