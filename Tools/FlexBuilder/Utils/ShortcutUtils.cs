using System.Reactive;
using System.Reactive.Linq;
using PowRxVar;
using WinFormsTooling.Shortcuts;

namespace FlexBuilder.Utils;

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
	
	
	private static IObservable<Unit> WhenShortcut(this IObservable<ShortcutMsg> obs, Keys key, Func<bool> predicate) =>
		Obs.Merge(
			obs.Where(msg => predicate() && msg.Key == key).Do(msg => msg.Handled = true).ToUnit()
			//WinMan.MainWins.Items.MergeMany(win => win.Evt.WhenKeyDown(key))
		);
}