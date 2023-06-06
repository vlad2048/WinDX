using PowRxVar;

namespace LayoutDbgApp.Utils.Exts;

static class StatusStripExts
{
	public static IDisposable AddVar<T>(this StatusStrip ctrl, string title, IObservable<T> rxVar)
	{
		var d = new Disp();
		var label = new ToolStripLabel();
		ctrl.Items.Add(label);

		rxVar.Subscribe(val =>
		{
			label.Text = $"{title}: {val}";
		}).D(d);

		return d;
	}
}