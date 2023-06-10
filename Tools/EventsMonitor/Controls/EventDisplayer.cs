using System.Reactive.Linq;
using EventsMonitor.Structs;
using PowMaybe;
using PowRxVar;
using PowWinForms;

namespace EventsMonitor.Controls;

partial class EventDisplayer : UserControl
{
	private const int MAX_ITEMS = 256;

	private readonly IRwVar<Maybe<Control>> trackedCtrl;

	public EventDisplayer()
	{
		InitializeComponent();

		trackedCtrl = Var.Make(May.None<Control>());

		this.InitRX(d => {
			trackedCtrl.D(d);
			var isPaused = Var.Make(false).D(d);
			var isEmpty = Var.Make(true).D(d);

			isPaused.Subscribe(paused => pauseBtn.Text = paused ? "Resume" : "Pause").D(d);
			trackedCtrl.Subscribe(e => pauseBtn.Enabled = e.IsSome()).D(d);
			pauseBtn.Events().Click.Subscribe(_ => isPaused.V = !isPaused.V).D(d);

			isEmpty.Subscribe(e => clearBtn.Enabled = !e).D(d);
			clearBtn.Events().Click.Subscribe(_ => {
				eventListBox.Items.Clear();
				isEmpty.V = true;
			}).D(d);

			var evt = trackedCtrl
				.SelectMaySwitch(ctrl => WinFormsEvtGenerator.MakeForControl(ctrl).PrettyPrint())
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

	public void SetTrackedControl(Control ctrl) => trackedCtrl.V = May.Some(ctrl);
}
