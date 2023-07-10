using System.Reactive.Linq;
using ControlSystem.Structs;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using WinSpectorLib.Logic;
using WinSpectorLib.Structs;

namespace WinSpectorLib;

sealed partial class WinSpectorWin : Form
{
	public WinSpectorWin(params DemoNfo[] demos)
	{
		InitializeComponent();
		if (WinFormsUtils.IsDesignMode) return;

		var prefs = new SpectorPrefs().Track();
		var ui = this;

		var showEvents = prefs.VarMake(e => e.ShowEvents).D(this);
		var showSysCtrls = prefs.VarMake(e => e.ShowSysCtrls).D(this);
		var trackedState = VarMay.Make<NodeState>().D(this);
		ui.layoutShowSysCtrlsItem.Checked = showSysCtrls.V;

		this.InitRX(d => {

			ui.layoutShowSysCtrlsItem.Events().CheckedChanged.Subscribe(_ => showSysCtrls.V = ui.layoutShowSysCtrlsItem.Checked).D(d);

			WinUtils.HookShowEvents(ui, showEvents).D(d);
			Setup.ShowDemos(ui, demos).D(d);
			Setup.ListWindowsAndGetVirtualTree(ui, out var selLayout, showSysCtrls).D(d);

			selLayout.Subscribe(_ => trackedState.V = May.None<NodeState>()).D(d);

			Setup.ViewLayout(ui, selLayout, showEvents, trackedState).D(d);
			Setup.OpenInFlexBuilder(ui, selLayout).D(d);
			Setup.DisplayEvents(ui, showEvents).D(d);
			Setup.PrintTrackedNodeState(ui, trackedState).D(d);

			eventDisplayer.SetShowEvents(showEvents);

		});
	}
}


file static class WinUtils
{
	public static IDisposable HookShowEvents(
		WinSpectorWin ui,
		IRoVar<bool> showEvents
	)
	{
		var d = new Disp();

		var delta = ui.eventsGroupBox.Width + 6;

		void StartLayout()
		{
			ui.SuspendLayout();
			ui.layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			ui.eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		}

		void FinishLayout()
		{
			ui.layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			ui.eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			ui.ResumeLayout();

		}

		if (!showEvents.V) {
			StartLayout();
			ui.layoutGroupBox.Width += delta;
			ui.eventsGroupBox.Visible = false;
			FinishLayout();
		}

		showEvents.Skip(1).Subscribe(show => {
			StartLayout();
			switch (show) {
				case false:
					ui.layoutGroupBox.Width += delta;
					break;
				case true:
					ui.layoutGroupBox.Width -= delta;
					ui.eventsGroupBox.Left = ui.layoutGroupBox.Right + 6;
					break;
			}
			ui.eventsGroupBox.Visible = show;
			FinishLayout();
		}).D(d);

		return d;
	}




	/*public static IDisposable HookShowEvents(
		WinSpectorWin ui,
		IRoVar<bool> showEvents
	)
	{
		var d = new Disp();

		const int delta = 1125 - 808;

		void StartLayout()
		{
			ui.SuspendLayout();
			ui.layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			ui.eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		}

		void FinishLayout()
		{
			ui.layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			ui.eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			ui.ResumeLayout();

		}

		if (showEvents.V) {
			StartLayout();
			ui.layoutGroupBox.Width -= delta;
			ui.eventsGroupBox.Left = ui.layoutGroupBox.Right + 6;
			FinishLayout();
		}

		showEvents.Skip(1).Subscribe(show => {
			StartLayout();
			switch (show) {
				case false:
					ui.Width -= delta;
					break;
				case true:
					ui.Width += delta;
					break;
			}
			ui.eventsGroupBox.Left = ui.layoutGroupBox.Right + 6;
			FinishLayout();
		}).D(d);

		return d;
	}*/
}