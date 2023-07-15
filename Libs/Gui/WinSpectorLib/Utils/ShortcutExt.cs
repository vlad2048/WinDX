using System.Reactive;
using System.Reactive.Linq;
using ControlSystem;
using ControlSystem.Logic.Popup_.Structs;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
using WinSpectorLib.Structs;

namespace WinSpectorLib.Utils;

static class ShortcutExt
{
	public static IObservable<Win> WhenShortcut(this (IObservable<ShortcutMsg>, IRoMayVar<PartitionSet>) t, Keys key) =>
		Obs.Merge(
				t.Item1
					.Where(_ => t.Item2.V.IsSome())
					.Where(msg => msg.Key == key)
					.Do(msg => msg.Handled = true)
					.ToUnit(),
				t.Item2
					.Select(maySet => maySet.IsSome(out var set) switch
					{
						true => set.Nfo.MainWin.Evt.WhenKeyDown(key),
						false => Obs.Never<Unit>(),
					})
					.Switch()
			)
			.Select(_ => t.Item2.V.Ensure().Nfo.MainWin);
}