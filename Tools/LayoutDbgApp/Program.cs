using PowRxVar;
using PowWinForms;

namespace LayoutDbgApp;

static class Program
{
	[STAThread]
	static void Main()
	{
		//VarDbg.BreakpointOnDispAlloc(49);

		ApplicationConfiguration.Initialize();
		using var win = new MainWin().Track();
		Application.Run(win);

		VarDbg.CheckForUndisposedDisps(true);
	}
}