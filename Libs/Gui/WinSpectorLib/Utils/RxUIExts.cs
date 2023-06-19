using PowMaybe;

namespace WinSpectorLib.Utils;

static class RxUIExts
{
	public static IDisposable EnableWhenSome<T>(this ToolStripItem ctrl, IObservable<Maybe<T>> obsMay) => obsMay.Subscribe(may => ctrl.Enabled = may.IsSome());
}