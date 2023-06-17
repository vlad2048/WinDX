using PowRxVar;
using PowWinForms;
using WinAPI.Kernel32;

namespace FlexBuilder;

static class Program
{
	[STAThread]
	static void Main()
	{
		//VarDbg.BreakpointOnDispAlloc(1);

		// TODO: understand why it's needed
		Kernel32Methods.AllocConsole();

		ApplicationConfiguration.Initialize();
		using (var win = new MainWin().Track())
			Application.Run(win);

		VarDbg.CheckForUndisposedDisps(true);
	}
}