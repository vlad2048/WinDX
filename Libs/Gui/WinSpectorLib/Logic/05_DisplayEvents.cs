using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using PowRxVar;
using UserEvents.Structs;
using WinAPI.User32;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable DisplayEvents(
		WinSpectorWin ui,
		IRwVar<bool> showEvents
	)
	{
		var d = new Disp();

		var evt = ui.eventDisplayer;

		IObservable<Unit> WhenKey(Keys key) => Obs.Merge(
			ui.Events().KeyDown.Where(e => e.KeyCode == key).ToUnit(),
			WinMan.MainWins.Items.MergeMany(win => win.Evt.WhenKeyDown(key))
		);

		ui.eventsShowItem.Events().CheckedChanged.Subscribe(_ => showEvents.V = ui.eventsShowItem.Checked).D(d);
		WhenKey(Keys.S).Subscribe(_ => showEvents.V = ui.eventsShowItem.Checked = !showEvents.V).D(d);

		ui.eventsEnabledItem.Events().CheckedChanged.Subscribe(_ => evt.IsPaused.V = ui.eventsEnabledItem.Checked).D(d);
		WhenKey(Keys.E).Subscribe(_ => evt.IsPaused.V = ui.eventsEnabledItem.Checked = !evt.IsPaused.V).D(d);

		Obs.Merge(
			ui.eventsClearItem.Events().Click.ToUnit(),
			WhenKey(Keys.C)
		)
			.Subscribe(_ => evt.Clear()).D(d);

		return d;
	}
}