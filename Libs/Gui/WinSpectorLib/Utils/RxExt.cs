using System.Reactive.Linq;
using PowRxVar;

namespace WinSpectorLib.Utils;

static class RxExt
{
	public static IRoMayVar<U> SwitchMayMayVar<T, U>(this IRoMayVar<T> sel, Func<T, IRoMayVar<U>> fun) =>
		VarMay.Make(
			sel.Select(e => e.Select(fun)).Select(e =>
				from t in e
				from u in t.V
				select u
			)
		).D(sel);
}