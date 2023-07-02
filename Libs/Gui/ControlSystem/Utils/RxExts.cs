using System.Reactive.Linq;
using DynamicData;

namespace ControlSystem.Utils;

static class RxExts
{
	public static IObservable<V> OfType<U, V>(this IObservable<object> obs, Func<U, V> mapFun) => obs.OfType<U>().Select(mapFun);

	public static void EditDiffKeys<K, V>(
		this ISourceCache<V, K> itemsSrc,
		IObservableCache<V, K> items,
		K[] keys,
		Func<K, V> makeFun
	)
		where K : notnull
	{
		var (keysAdd, keysDel) = items.GetAddDels(keys);
		itemsSrc.Edit(upd =>
		{
			upd.RemoveKeys(keysDel);
			foreach (var keyAdd in keysAdd)
				upd.AddOrUpdate(makeFun(keyAdd));
		});
	}
}