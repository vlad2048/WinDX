using System.Reactive.Linq;
using PowMaybe;
using PowRxVar;

namespace WinSpectorLib.Utils;

static class RxExt
{
	public static IRoMayVar<U> SwitchMayMayVar<T, U>(this IRoMayVar<T> sel, Func<T, IRoMayVar<U>> fun) =>
		VarMay.Make(
			sel.Select(ma => ma.IsSome(out var a) switch
			{
				true => fun(a),
				false => Obs.Never<Maybe<U>>().Prepend(May.None<U>()),
			})
				.Switch()
		).D(sel);
}