using System.Reactive.Linq;
using PowRxVar;

namespace UserEvents.Utils;

static class RxExts
{
	public static IObservable<T> MakeHot<T>(this IObservable<T> obs, IRoDispBase d)
	{
		var pub = obs.Publish();
		pub.Connect().D(d);
		return pub;
	}
}