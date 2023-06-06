using PowMaybe;
using System.Reactive.Linq;
using PowRxVar;

namespace LayoutDbgApp.Utils.Exts;

static class RxUIExts
{
	public static IObservable<T> ObserveOnUIThread<T>(this IObservable<T> obs) => obs
		.ObserveOn(SynchronizationContext.Current!);

	public static IDisposable SubscribeToSome<T>(this IObservable<Maybe<T>> obs, Action<T> action) =>
		obs.WhenSome().Subscribe(action);

	public static IDisposable SubscribeToNone<T>(this IObservable<Maybe<T>> obs, Action action) =>
		obs.WhenNone().Subscribe(_ => action());

	public static IDisposable EnableWhenSome<T>(this Control ctrl, IObservable<Maybe<T>> obsMay) => obsMay.Subscribe(may => ctrl.Enabled = may.IsSome());


	public static IRoVar<Maybe<U>> Map2<T, U>(this IRoVar<Maybe<T>> v, Func<T, U> fun) =>
		v.SelectVar(e => e.Select(fun));

	
	public static IDisposable EditInner<T>(
		this IFullRwBndVar<Maybe<T>> mayVar,
		Action<bool> enableUI,
		Action<T> setUI,
		IObservable<Func<T, T>> UI2Val
	)
	{
		var d = new Disp();

		mayVar.WhenOuter.Subscribe(may =>
		{
			enableUI(may.IsSome());
			if (may.IsSome(out var val))
				setUI(val);
		}).D(d);

		UI2Val.Subscribe(valFun =>
		{
			if (mayVar.V.IsNone(out var valPrev)) return;
			var valNext = valFun(valPrev);
			mayVar.SetInner(May.Some(valNext));
			setUI(valNext);
		}).D(d);

		return d;
	}


	public static IDisposable EditInner<T>(
		this IFullRwBndVar<Maybe<T>> mayVar,
		Action<bool> enableUI,
		Action<T> setUI,
		IObservable<T> UI2Val
	)
	{
		var d = new Disp();

		mayVar.WhenOuter.Subscribe(may =>
		{
			enableUI(may.IsSome());
			if (may.IsSome(out var val))
				setUI(val);
		}).D(d);

		UI2Val.Subscribe(val =>
		{
			mayVar.SetInner(May.Some(val));
			setUI(val);
		}).D(d);

		return d;
	}
}