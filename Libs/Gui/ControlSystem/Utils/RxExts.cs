using System.Reactive.Linq;

namespace ControlSystem.Utils;

static class RxExts
{
	public static IObservable<V> OfType<U, V>(this IObservable<object> obs, Func<U, V> mapFun) => obs.OfType<U>().Select(mapFun);
}