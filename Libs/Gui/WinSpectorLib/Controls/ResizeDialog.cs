using System.Reactive.Linq;
using ControlSystem;
using PowBasics.Geom;
using PowRxVar;
using PowWinForms;

namespace WinSpectorLib.Controls;

sealed partial class ResizeDialog : Form
{
	public ResizeDialog(Win win, SpectorPrefs prefs)
	{
		InitializeComponent();

		widthUpDown.Value = prefs.ResizeSz.Item1;
		heightUpDown.Value = prefs.ResizeSz.Item2;

		this.InitRX(d => {
			win.ScreenR.Subscribe(r => {
				widthLabel.Text = $"{r.Width}";
				heightLabel.Text = $"{r.Height}";
			}).D(d);

			Sz GetAndSaveSz()
			{
				var res = new Sz((int)widthUpDown.Value, (int)heightUpDown.Value);
				prefs.ResizeSz = (res.Width, res.Height);
				prefs.Save();
				return res;
			}



			Obs.Merge(
					addBtn.Events().Click.ToUnit(),
					this.Events().KeyDown.Where(e => e.KeyCode == Keys.Add).Do(e => e.Handled = true).ToUnit()
				)
				.Subscribe(_ => win.SetSize(win.ScreenR.V.Size + GetAndSaveSz())).D(d);

			Obs.Merge(
					subBtn.Events().Click.ToUnit(),
					this.Events().KeyDown.Where(e => e.KeyCode == Keys.Subtract).Do(e => e.Handled = true).ToUnit()
				)
				.Subscribe(_ => win.SetSize(win.ScreenR.V.Size - GetAndSaveSz())).D(d);


			this.Events().KeyDown.Where(e => e.KeyCode == Keys.Escape)
				.Subscribe(_ => Close()).D(d);
		});
	}
}
