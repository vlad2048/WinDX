using System.Reactive.Linq;
using ControlSystem;
using PowMaybe;
using PowRxVar;
using PowWinForms.ListBoxSourceListViewing;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using PowTrees.Algorithms;
using UserEvents;
using WinFormsTooling.Utils.Exts;
using ControlSystem.Utils;
using UserEvents.Utils;
using WinSpectorLib.Controls;
using WinSpectorLib.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ListWindowsAndGetLayout(
		WinSpectorWin ui,
		out IRoMayVar<PartitionSet> selLayout,
		SpectorPrefs prefs
	)
	{
		var d = new Disp();
		ListBoxSourceListViewer.View(out var selWin, WinMan.MainWins.Items, ui.winList).D(d);

		ui.windowUnselectItem.Events().Click.Subscribe(_ => selWin.V = May.None<Win>()).D(d);

		selLayout = selWin.SwitchMayMayVar(e => e.PartSet);

		ui.windowRedrawItem.EnableWhenSome(selWin).D(d);
		ui.windowLogRedrawItem.EnableWhenSome(selWin).D(d);
		ui.windowLogNextRedrawItem.EnableWhenSome(selWin).D(d);
		ui.windowLogNext2RedrawsItem.EnableWhenSome(selWin).D(d);
		ui.windowResizeItem.EnableWhenSome(selWin).D(d);

		ui.windowRedrawItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw);
		}).D(d);

		ui.windowLogRedrawItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.SpectorDrawState.SetRenderCountToLog(1);
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw);
		}).D(d);

		ui.windowLogNextRedrawItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.SpectorDrawState.SetRenderCountToLog(1);
		}).D(d);

		ui.windowLogNext2RedrawsItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.SpectorDrawState.SetRenderCountToLog(2);
		}).D(d);

		ui.windowResizeItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			var dlg = new ResizeDialog(win, prefs);
			dlg.ShowDialog();
		}).D(d);

		return d;
	}
}