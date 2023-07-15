using DynamicData;
using PowBasics.CollectionsExt;

namespace ControlSystem.Utils;

static class AddDelExts
{
	public static void Update<K, V>(this Dictionary<K, V> dict, K[] keys, Func<K, V> genFun)
		where K : notnull
		where V : IDisposable
	{
		var keysAdd = keys.WhereNotToArray(dict.ContainsKey);
		var keysDel = dict.Keys.WhereNotToArray(keys.Contains);
		foreach (var keyDel in keysDel)
		{
			var val = dict[keyDel];
			val.Dispose();
			dict.Remove(keyDel);
		}

		foreach (var keyAdd in keysAdd)
			dict[keyAdd] = genFun(keyAdd);
	}

	/*
	public static (K[] adds, K[] dels) GetAddDels<K, V>(this IReadOnlyDictionary<K, V> map, K[] arr) where K : notnull => (
		arr.WhereNotToArray(map.ContainsKey),
		map.Keys.WhereNotToArray(arr.Contains)
	);

	public static (T[] adds, K[] dels) GetAddDels<T, K, V>(this IReadOnlyDictionary<K, V> map, T[] arr, Func<T, K> projFun) where K : notnull
	{
		var keys = arr.SelectToArray(projFun);
		return (
			arr.WhereNotToArray(e => map.ContainsKey(projFun(e))),
			map.Keys.WhereNotToArray(keys.Contains)
		);
	}

	public static (T[] adds, K[] dels, T[] coms) GetAddDelsComs<T, K, V>(this IReadOnlyDictionary<K, V> map, T[] arr, Func<T, K> projFun) where K : notnull
	{
		var keys = arr.SelectToArray(projFun);
		return (
			arr.WhereNotToArray(e => map.ContainsKey(projFun(e))),
			map.Keys.WhereNotToArray(keys.Contains),
			arr.WhereToArray(e => map.ContainsKey(projFun(e)))
		);
	}

	public static (K[] adds, K[] dels) GetAddDels<K, V>(this IObservableCache<V, K> cache, K[] arr) where K : notnull => (
		arr.WhereToArray(e => !cache.Lookup(e).HasValue),
		cache.Keys.WhereNotToArray(arr.Contains)
	);
	*/
}