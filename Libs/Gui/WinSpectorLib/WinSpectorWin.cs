using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using UserEvents;
using WinAPI.User32;
using WinSpectorLib.Logic;
using WinSpectorLib.Structs;
using WinSpectorLib.Utils;
using Message = System.Windows.Forms.Message;

namespace WinSpectorLib;

sealed partial class WinSpectorWin : Form
{
	private readonly ISubject<ShortcutMsg> whenShortcut = null!;
	private IObservable<ShortcutMsg> WhenShortcut => whenShortcut.AsObservable();

	public WinSpectorWin(params DemoNfo[] demos)
	{
		InitializeComponent();
		if (WinFormsUtils.IsDesignMode) return;

		var prefs = new SpectorPrefs().Track();
		var ui = this;

		whenShortcut = new Subject<ShortcutMsg>().D(this);
		var showEvents = prefs.VarMake(e => e.ShowEvents).D(this);
		var trackedState = VarMay.Make<NodeState>().D(this);
		//ui.layoutShowSysCtrlsItem.Checked = showSysCtrls.V;

		this.InitRX(d => {

			//ui.layoutShowSysCtrlsItem.Events().CheckedChanged.Subscribe(_ => showSysCtrls.V = ui.layoutShowSysCtrlsItem.Checked).D(d);

			WinUtils.HookShowEvents(ui, showEvents).D(d);
			Setup.ShowDemos(ui, demos).D(d);
			Setup.ListWindowsAndGetLayout(ui, out var selLayout, prefs).D(d);
			WinUtils.SetupShortcuts((WhenShortcut, selLayout), prefs, ui).D(d);

			selLayout.Subscribe(_ => trackedState.V = May.None<NodeState>()).D(d);

			Setup.ViewLayout(ui, selLayout, showEvents, trackedState).D(d);
			Setup.OpenInFlexBuilder(ui, selLayout).D(d);
			Setup.DisplayEvents(ui, showEvents).D(d);
			Setup.PrintTrackedNodeState(ui, trackedState).D(d);

			eventDisplayer.SetShowEvents(showEvents);

		});
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		var m = new ShortcutMsg(keyData);
		whenShortcut.OnNext(m);
		return m.Handled switch {
			true => true,
			false => base.ProcessCmdKey(ref msg, keyData)
		};

	}
}


file static class WinUtils
{
	public static IDisposable SetupShortcuts((IObservable<ShortcutMsg>, IRoMayVar<PartitionSet>) t, SpectorPrefs prefs, WinSpectorWin ui)
	{
		var d = new Disp();
		t.WhenShortcut(Keys.Control | Keys.Add).Subscribe(win => win.SetSize(win.ScreenR.V.Size + prefs.ResizeSz.ToSz())).D(d);
		t.WhenShortcut(Keys.Control | Keys.Subtract).Subscribe(win => win.SetSize(win.ScreenR.V.Size - prefs.ResizeSz.ToSz())).D(d);

		t.WhenShortcut(Keys.Control | Keys.R).Subscribe(win => {
			win.SpectorDrawState.SetRenderCountToLog(1);
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw);
		}).D(d);

		Obs.Merge(
				ui.windowClearConsoleItem.Events().Click.ToUnit(),
				t.WhenShortcut(Keys.Control | Keys.C).ToUnit()
		)
			.Subscribe(_ => Console.Clear()).D(d);


		return d;
	}


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