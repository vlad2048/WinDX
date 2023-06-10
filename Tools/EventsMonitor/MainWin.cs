using PowWinForms;

namespace EventsMonitor;

partial class MainWin : Form
{
	public MainWin()
	{
		InitializeComponent();

		this.InitRX(d => {
			formEventDisplayer.SetTrackedControl(this);
		});
	}
}
