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
			Setup.ListWindowsAndGetLayout(ui, out var selLayout, out var selWin, prefs).D(d);
			WinUtils.SetupShortcuts(WhenShortcut, selWin, ui, prefs, showEvents).D(d);

			selLayout.Subscribe(mayLay =>
			{
				if (trackedState.V.IsSome(out var trk) && (mayLay.IsNone(out var lay) || !lay.ContainsNodeState(trk)))
					trackedState.V = May.None<NodeState>();
			}).D(d);

			Setup.ViewLayout(ui, selLayout, showEvents, trackedState).D(d);
			Setup.OpenInFlexBuilder(ui, selLayout).D(d);
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
	public static bool ContainsNodeState(this PartitionSet set, NodeState nodeState) => set.Partitions.Any(part => part.NodeStates.Contains(nodeState));

	public static IDisposable SetupShortcuts(
		IObservable<ShortcutMsg> whenShortcut,
		IRoMayVar<Win> selWin,
		WinSpectorWin ui,
		SpectorPrefs prefs,
		IRwVar<bool> showEvents
	)
	{
		var d = new Disp();


		// ***********
		// * Console *
		// ***********
		ShortcutUtils.SetAction(Keys.C, ui.eventsClearItem, whenShortcut,
			ui.eventDisplayer.Clear
		).D(d);


		// ***********
		// * Redraws *
		// ***********
		ShortcutUtils.SetWinAction(Keys.Control | Keys.R, ui.windowLogRedrawItem, whenShortcut, selWin, win =>
		{
			win.SpectorDrawState.SetRenderCountToLog(1);
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw);
		}).D(d);
		ShortcutUtils.SetWinAction(ui.windowRedrawItem, selWin, win =>
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw)
		).D(d);
		ShortcutUtils.SetWinAction(ui.windowLogNextRedrawItem, selWin, win =>
			win.SpectorDrawState.SetRenderCountToLog(1)
		).D(d);
		ShortcutUtils.SetWinAction(ui.windowLogNext2RedrawsItem, selWin, win =>
			win.SpectorDrawState.SetRenderCountToLog(2)
		).D(d);


		// *****************
		// * Window Resize *
		// *****************
		ShortcutUtils.SetWinAction(Keys.Control | Keys.Add, whenShortcut, selWin,
			win => win.SetSize(win.ScreenR.V.Size + prefs.ResizeSz.ToSz())
		).D(d);
		ShortcutUtils.SetWinAction(Keys.Control | Keys.Subtract, whenShortcut, selWin, win =>
			win.SetSize(win.ScreenR.V.Size - prefs.ResizeSz.ToSz())
		).D(d);


		// **********
		// * Events *
		// **********
		ShortcutUtils.SetToggle(Keys.S, ui.eventsShowItem, whenShortcut,
			showEvents
		).D(d);
		ShortcutUtils.SetToggle(Keys.E, ui.eventsShowItem, whenShortcut,
			ui.eventDisplayer.IsPaused
		).D(d);


		return d;
	}

	/*public static IDisposable SetupShortcuts((IObservable<ShortcutMsg>, IRoMayVar<PartitionSet>) t, SpectorPrefs prefs, WinSpectorWin ui)
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
	}*/


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