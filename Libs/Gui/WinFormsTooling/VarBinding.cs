/*
using PowRxVar;

namespace WinFormsTooling;

public static class VarBinding
{
	public static IDisposable EditCheckBox(IRwVar<bool> v, CheckBox checkBox)
	{
		var d = new Disp();

		v.Subscribe(e => checkBox.Checked = e).D(d);
		checkBox.Events().CheckedChanged.Subscribe(_ => v.V = checkBox.Checked).D(d);

		return d;
	}
}
*/