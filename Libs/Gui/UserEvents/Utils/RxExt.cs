using PowRxVar;
using System.Reactive.Linq;
using System.Reactive.Disposables;

namespace UserEvents.Utils;

public static class RxExt
{
	public static IDisposable LogIf<T>(this IObservable<T> obs, bool condition, string? title = null)
	{
		if (!condition) return Disposable.Empty;
		return obs.Log(title);
	}

	public static IDisposable Log<T>(this IObservable<T> obs, string? title = null) =>
		obs.Subscribe(e => L($"{title.FmtOrEmpty(f => f)}{e}"));


	internal static IDisposable RunActionAfterIfNot<T, TAfter, TPrevent>(
		this IObservable<T> obs,
		Action action,
		TimeSpan delay
	)
		where TAfter : T
		where TPrevent : T
	{
		var d = new Disp();

		obs
			.Where(e => e is TAfter)
			.Select(_ => Obs.Timer(delay))
			.Switch()
			.TakeUntil(obs.Where(e => e is TPrevent))
			.Repeat()
			.Subscribe(_ => action()).D(d);

		return d;
	}
}