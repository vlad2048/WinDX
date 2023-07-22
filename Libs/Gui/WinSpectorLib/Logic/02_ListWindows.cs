using ControlSystem;
using PowMaybe;
using PowRxVar;
using PowWinForms.ListBoxSourceListViewing;
using ControlSystem.Logic.Popup_.Structs;
using WinFormsTooling.Utils.Exts;
using WinSpectorLib.Controls;
using WinSpectorLib.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ListWindowsAndGetLayout(
		WinSpectorWin ui,
		out IRoMayVar<PartitionSet> selLayout,
		out IRoMayVar<Win> selWinRo,
		SpectorPrefs prefs
	)
	{
		var d = new Disp();
		ListBoxSourceListViewer.View(out var selWin, WinMan.MainWins.Items, ui.winList).D(d);
		selWinRo = selWin;

		ui.windowUnselectItem.Events().Click.Subscribe(_ => selWin.V = May.None<Win>()).D(d);

		selLayout = selWin.SwitchMayMayVar(e => e.PartSet);

		ui.windowResizeItem.EnableWhenSome(selWin).D(d);

		ui.windowResizeItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			var dlg = new ResizeDialog(win, prefs);
			dlg.ShowDialog();
		}).D(d);

		return d;
	}
}