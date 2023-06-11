using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using PowWinForms;
using SysWinLib;
using SysWinLib.Structs;
using UserEvents.Utils;
using WinAPI.User32;

namespace EventsMonitor;

partial class MainWin : Form
{
	public MainWin()
	{
		InitializeComponent();

		this.InitRX(d =>
		{
			formEventDisplayer.SetTrackedControl(this);
			ctrlEventDisplayer.SetTrackedControl(basicControl);

			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.C)
				.Subscribe(_ =>
				{
					formEventDisplayer.Clear();
					ctrlEventDisplayer.Clear();
					dxFormEventDisplayer.Clear();
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.R)
				.Subscribe(_ =>
				{
					Controls.Remove(basicControl);
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.A)
				.Subscribe(_ =>
				{
					Controls.Add(basicControl);
				}).D(d);


			this.Events().KeyDown
				.Where(e => e.KeyCode == Keys.D)
				.Subscribe(_ =>
				{
					basicControl.Dispose();
				}).D(d);


			var win = MakeWindow().D(d);
			win.Init();
			var winEvt = UserEvtGenerator.MakeForWin(win);
			dxFormEventDisplayer.SetTrackedEvtSrc(winEvt);
		});
	}


	private static SysWin MakeWindow() => new(opt => {
		opt.CreateWindowParams = new CreateWindowParams {
			Name = "Main Win",
			Styles = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE,
			X = -300,
			Y = 20,
			Width = 256,
			Height = 256,
			ControlStyles = (uint)ControlStyles.OptimizedDoubleBuffer
		};
	});
}
