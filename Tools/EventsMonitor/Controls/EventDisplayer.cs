using System.Reactive.Linq;
using EventsMonitor.Structs;
using PowMaybe;
using PowRxVar;
using PowWinForms;

namespace EventsMonitor.Controls;

sealed partial class EventDisplayer : UserControl
{
	private const int MAX_ITEMS = 256;

	private readonly IRwVar<bool> isEmpty;
	private readonly IRwVar<bool> isPaused;
	private readonly IRwMayVar<Control> trackedCtrl;

	public EventDisplayer()
	{
		InitializeComponent();

		isEmpty = Var.Make(true).D(this);
		isPaused = Var.Make(false).D(this);
		trackedCtrl = VarMay.Make<Control>().D(this);

		this.InitRX(d =>
		{
			isPaused.Subscribe(paused => pauseBtn.Text = paused ? "Resume" : "Pause").D(d);
			trackedCtrl.Subscribe(e => pauseBtn.Enabled = e.IsSome()).D(d);
			pauseBtn.Events().Click.Subscribe(_ => isPaused.V = !isPaused.V).D(d);

			isEmpty.Subscribe(e => clearBtn.Enabled = !e).D(d);
			clearBtn.Events().Click.Subscribe(_ => Clear()).D(d);

			var evt = trackedCtrl
				.SelectMaySwitch(ctrl =>
					WinFormsEvtGenerator.MakeForControl(ctrl)
						.Where(IsNotClearKey)
						.PrettyPrint()
				)
				.Where(_ => !isPaused.V);

			evt.Subscribe(e => {
				if (e is MouseMoveUpdatePrettyWinFormsEvt { IsSubsequent: true } && eventListBox.Items.Count > 0) {
					eventListBox.Items[^1] = $"{e}";
				} else {
					eventListBox.Items.Add($"{e}");
				}
				if (eventListBox.Items.Count > MAX_ITEMS)
					eventListBox.Items.RemoveAt(0);

				eventListBox.TopIndex = eventListBox.Items.Count - 1;
				isEmpty.V = false;
			}).D(d);

		});
	}

	private static bool IsNotClearKey(IWinFormsEvt e) => e switch
	{
		KeyDownWinFormsEvt { Key: Keys.C } => false,
		KeyUpWinFormsEvt { Key: Keys.C } => false,
		KeyCharWinFormsEvt { Char: 'c' } => false,
		_ => true
	};

	public void SetTrackedControl(Control ctrl) => trackedCtrl.V = May.Some(ctrl);

	public void Clear()
	{
		eventListBox.Items.Clear();
		isEmpty.V = true;
	}

	public void Pause() => isPaused.V = !isPaused.V;
}
