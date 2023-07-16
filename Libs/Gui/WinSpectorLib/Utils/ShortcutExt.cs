using System.Reactive;
using System.Reactive.Linq;
using ControlSystem;
using ControlSystem.Logic.Popup_.Structs;
using DynamicData;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;
using WinFormsTooling.Utils.Exts;
using WinSpectorLib.Structs;

namespace WinSpectorLib.Utils;

static class ShortcutUtils
{
	public static IDisposable SetAction(
		Keys key,
		ToolStripMenuItem menuItem,
		IObservable<ShortcutMsg> whenShortcut,
		Action action
	)
	{
		var d = new Disp();

		Obs.Merge(
				menuItem.Events().Click.ToUnit(),
				whenShortcut.WhenShortcut(key, () => menuItem.Enabled)
			)
			.Subscribe(_ => action()).D(d);

		return d;
	}

	public static IDisposable SetToggle(
		Keys key,
		ToolStripMenuItem menuItem,
		IObservable<ShortcutMsg> whenShortcut,
		IRwVar<bool> toggleVar
	)
	{
		var d = new Disp();

		menuItem.Events().CheckedChanged.Subscribe(_ => toggleVar.V = menuItem.Checked).D(d);
		whenShortcut.WhenShortcut(key, () => true).Subscribe(_ => toggleVar.V = !toggleVar.V).D(d);

		return d;
	}



	public static IDisposable SetWinAction(
		Keys key,
		ToolStripMenuItem menuItem,
		IObservable<ShortcutMsg> whenShortcut,
		IRoMayVar<Win> selWin,
		Action<Win> action
	)
	{
		var d = new Disp();

		menuItem.EnableWhenSome(selWin).D(d);

		Obs.Merge(
				menuItem.Events().Click.ToUnit(),
				whenShortcut.WhenShortcut(key, () => menuItem.Enabled)
			)
			.Where(_ => selWin.V.IsSome())
			.Subscribe(_ => action(selWin.V.Ensure())).D(d);

		return d;
	}

	public static IDisposable SetWinAction(
		ToolStripMenuItem menuItem,
		IRoMayVar<Win> selWin,
		Action<Win> action
	)
	{
		var d = new Disp();

		menuItem.EnableWhenSome(selWin).D(d);

		menuItem.Events().Click
			.Where(_ => selWin.V.IsSome())
			.Subscribe(_ => action(selWin.V.Ensure())).D(d);

		return d;
	}

	public static IDisposable SetWinAction(
		Keys key,
		IObservable<ShortcutMsg> whenShortcut,
		IRoMayVar<Win> selWin,
		Action<Win> action
	)
	{
		var d = new Disp();

		whenShortcut.WhenShortcut(key, () => selWin.V.IsSome()).Subscribe(_ => action(selWin.V.Ensure())).D(d);

		return d;
	}



	private static IObservable<Unit> WhenShortcut(this IObservable<ShortcutMsg> obs, Keys key, Func<bool> predicate) =>
		Obs.Merge(
			obs.Where(msg => predicate() && msg.Key == key).Do(msg => msg.Handled = true).ToUnit(),
			WinMan.MainWins.Items.MergeMany(win => win.Evt.WhenKeyDown(key))
		);
}
