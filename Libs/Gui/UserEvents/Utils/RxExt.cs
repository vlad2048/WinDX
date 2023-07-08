using PowRxVar;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
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

	/*internal static IDisposable RunActionAfterIfNot<T, TAfter, TPrevent1, TPrevent2>(
		this IObservable<T> obs,
		Action action,
		TimeSpan delay
	)
		where TAfter : T
		where TPrevent1 : T
		where TPrevent2 : T
	{
		var d = new Disp();

		obs
			.Where(e => e is TAfter)
			.Select(_ => Obs.Timer(delay))
			.Switch()
			.TakeUntil(obs.Where(e => e is TPrevent1 or TPrevent2))
			.Repeat()
			.Subscribe(_ => action()).D(d);

		return d;
	}*/



	/*internal static (Action, IDisposable) DoIfEventDoesntHappenWithin<T>(this IObservable<T> obs, Func<T, bool> preventFun, Action action, TimeSpan delay)
	{
		var d = new Disp();
		var serD = new SerialDisp<Disp>().D(d);
		
		var whenDo = new Subject<Unit>().D(d);
		var doObs = whenDo.AsObservable();
		
		doObs.Subscribe(_ =>
		{
			serD.Value = null;
			serD.Value = new Disp();
			var hasReceived = false;
			obs.Where(preventFun).Subscribe(_ => hasReceived = true).D(serD.Value);
			Observable.Timer(delay).Where(_ => !hasReceived).Subscribe(_ => action()).D(serD.Value);
		}).D(d);
		
		return (
			() => whenDo.OnNext(Unit.Default),
			d
		);
	}*/
}