using System.Reactive.Linq;
using PowRxVar;
using PowWinForms;
using WinFormsTooling;
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

		var showEvents = prefs.VarMake(e => e.ShowEvents).D(this);
		var showSysCtrls = prefs.VarMake(e => e.ShowSysCtrls).D(this);

		this.InitRX(d => {
			var ui = this;

			VarBinding.EditCheckBox(showSysCtrls, showSysCtrlsCheckBox).D(d);
			WinUtils.HookShowEvents(ui, showEvents).D(d);

			Setup.ShowDemos(ui, demos).D(d);
			Setup.ListWindowsAndGetVirtualTree(ui, out var selLayout, showSysCtrls).D(d);
			Setup.ViewLayout(ui, selLayout, showEvents, prefs).D(d);
			Setup.OpenInFlexBuilder(ui, selLayout).D(d);

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
	}
}