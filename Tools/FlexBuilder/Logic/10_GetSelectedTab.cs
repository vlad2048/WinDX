using FlexBuilder.Structs;
using PowRxVar;
using PowWinForms.Utils;

namespace FlexBuilder.Logic;



static partial class Setup
{
	public static IDisposable GetSelectedTab(
		MainWin ui,
		UserPrefs userPrefs,
		out IRoVar<TabName> selTab
	)
	{
		var d = new Disp();

		var selTabRw = Var.Make(userPrefs.SelectedTab).D(d);
		selTab = selTabRw.ToReadOnly();
		ui.tabControl.SelectedIndex = (int)selTab.V;

		ui.tabControl.Events().SelectedIndexChanged.Subscribe(_ =>
		{
			selTabRw.V = (TabName)ui.tabControl.SelectedIndex;
			userPrefs.SelectedTab = selTabRw.V;
			userPrefs.Save();
		}).D(d);

		var isInit = false;
		Obs.Timer(TimeSpan.FromSeconds(1)).ObserveOnWinFormsUIThread().Subscribe(_ =>
		{
			if (userPrefs.DetailsSplitterPos != 0)
				ui.splitContainer.SplitterDistance = userPrefs.DetailsSplitterPos;
			isInit = true;
		}).D(d);

		ui.splitContainer.Events().SplitterMoved.Subscribe(_ =>
		{
			if (!isInit) return;
			userPrefs.DetailsSplitterPos = ui.splitContainer.SplitterDistance;
			userPrefs.Save();
		}).D(d);

		return d;
	}
}