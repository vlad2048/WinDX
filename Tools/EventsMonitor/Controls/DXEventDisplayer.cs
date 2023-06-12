using System.Reactive.Linq;
using EventsMonitor.Structs;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using UserEvents.Structs;
using UserEvents.Utils;

namespace EventsMonitor.Controls;

sealed partial class DXEventDisplayer : UserControl
{
	private const int MAX_ITEMS = 256;

	private readonly IRwVar<bool> isEmpty;
	private readonly IRwVar<IUIEvt> evtSrc;

	public DXEventDisplayer()
	{
		InitializeComponent();

		var topD = new Disp();
		isEmpty = Var.Make(true).D(topD);
		(evtSrc, var evt) = UserEvtGenerator.MakeWithSource().D(topD);

		this.InitRX(d =>
		{
			topD.D(d);
			var isPaused = Var.Make(false).D(d);

			isPaused.Subscribe(paused => pauseBtn.Text = paused ? "Resume" : "Pause").D(d);
			pauseBtn.Events().Click.Subscribe(_ => isPaused.V = !isPaused.V).D(d);

			isEmpty.Subscribe(e => clearBtn.Enabled = !e).D(d);
			clearBtn.Events().Click.Subscribe(_ => Clear()).D(d);

			var obs = PrettyPrintAggregator.Transform(evt)
				.Where(_ => !isPaused.V)
				.Where(IsNotClearKey);


			obs.Subscribe(e => {
				if (e is MouseMoveUpdatePrettyUserEvt { IsSubsequent: true } && eventListBox.Items.Count > 0) {
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

	private static bool IsNotClearKey(IPrettyUserEvt e) => e switch
	{
		NormalPrettyUserEvt f => f.Evt switch
		{
			KeyDownUserEvt { Key: WinAPI.User32.VirtualKey.C } => false,
			KeyUpUserEvt { Key: WinAPI.User32.VirtualKey.C } => false,
			KeyCharUserEvt { Char: 'c' } => false,
			_ => true
		},
		_ => true
	};

	public void SetTrackedEvtSrc(IUIEvt evt) => evtSrc.V = evt;

	public void Clear()
	{
		eventListBox.Items.Clear();
		isEmpty.V = true;
	}
}
