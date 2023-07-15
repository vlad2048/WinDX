using System.Reactive.Disposables;
using ControlSystem.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;

namespace ControlSystem.Utils;

public static class DictionaryExt
{
	public static void MergeInto<K, V>(this IReadOnlyDictionary<K, V> srcDict, Dictionary<K, V> dstDict) where K : notnull
	{
		foreach (var (key, val) in srcDict)
			dstDict[key] = val;
	}

	public static void ApplyOffsets<K>(this Dictionary<K, R> dict, IReadOnlyDictionary<K, Pt> ofsMap) where K : notnull
	{
		foreach (var (key, ofs) in ofsMap)
			dict[key] += ofs;
	}

	public static V[] GetOrEmpty<K, V>(this IReadOnlyDictionary<K, V[]> dict, K key) where K : notnull => dict.GetValueOrDefault(key, Array.Empty<V>());

	/*public static IReadOnlyDictionary<K, V> Merge<K, V>(this IEnumerable<IReadOnlyDictionary<K, V>> source) where K : notnull
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


	public static Dictionary<K1, Dictionary<K2, V>> D<K1, K2, V>(this Dictionary<K1, Dictionary<K2, V>> dicts, IRoDispBase d)
		where K1 : notnull
		where K2 : notnull
		where V : IDisposable
	{
		Disposable.Create(() =>
		{
			foreach (var dict in dicts.Values)
			{
				foreach (var val in dict.Values)
					val.Dispose();
				dict.Clear();
			}
			dicts.Clear();
		}).D(d);
		return dicts;
	}*/
}