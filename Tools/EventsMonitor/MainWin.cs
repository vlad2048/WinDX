using System.Reactive.Linq;
using PowRxVar;
using PowWinForms;

namespace EventsMonitor;

sealed partial class MainWin : Form
{
	public MainWin()
	{
		InitializeComponent();

		this.InitRX(d => {
			formEventDisplayer.SetTrackedControl(this);
			ctrlEventDisplayer1.SetTrackedControl(basicControl1);
			ctrlEventDisplayer2.SetTrackedControl(basicControl2);

			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.C)
				.Subscribe(_ => {
					formEventDisplayer.Clear();
					ctrlEventDisplayer1.Clear();
					ctrlEventDisplayer2.Clear();
				}).D(d);

			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.P)
				.Subscribe(_ => {
					formEventDisplayer.Pause();
					ctrlEventDisplayer1.Pause();
					ctrlEventDisplayer2.Pause();
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.R)
				.Subscribe(_ => {
					Controls.Remove(basicControl1);
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.A)
				.Subscribe(_ => {
					Controls.Add(basicControl1);
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.D)
				.Subscribe(_ => {
					basicControl1.Dispose();
				}).D(d);

			var y1 = basicControl2.Top;
			var y2 = y1 - 50;
			var y3 = y1 - 310;


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.T)
				.Subscribe(_ => {
					var yPrev = basicControl2.Top;
					var yNext = yPrev == y2 ? y1 : y2;
					basicControl2.Top = yNext;
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.Y)
				.Subscribe(_ => {
					var yPrev = basicControl2.Top;
					var yNext = yPrev == y3 ? y1 : y3;
					basicControl2.Top = yNext;
				}).D(d);
		});
	}
}
