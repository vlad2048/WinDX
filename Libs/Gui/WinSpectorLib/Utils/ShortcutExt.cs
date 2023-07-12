using System.Reactive;
using System.Reactive.Linq;
using ControlSystem;
using ControlSystem.Structs;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
using WinAPI.User32;
using WinSpectorLib.Structs;

namespace WinSpectorLib.Utils;

static class ShortcutExt
{
	public static IObservable<Win> WhenShortcut(this (IObservable<ShortcutMsg>, IRoMayVar<MixLayout>) t, Keys key) =>
		Obs.Merge(
				t.Item1
					.Where(_ => t.Item2.V.IsSome())
					.Where(msg => msg.Key == key)
					.Do(msg => msg.Handled = true)
					.ToUnit(),
				t.Item2
					.Select(mayLay => mayLay.IsSome(out var lay) switch
					{
						true => lay.Win.Evt.WhenKeyDown(key),
						false => Obs.Never<Unit>(),
					})
					.Switch()
			)
			.Select(_ => t.Item2.V.Ensure().Win);
}