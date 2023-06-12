using PowRxVar;
using PowWinForms;

namespace EventsMonitor.Controls;

sealed partial class BasicControl : UserControl
{
	public BasicControl()
	{
		InitializeComponent();

		this.InitRX(d =>
		{
			this.Events().Paint.Subscribe(e =>
			{
				var gfx = e.Graphics;
				gfx.FillRectangle(Brushes.Black, ClientRectangle);
			}).D(d);
		});
	}
}
