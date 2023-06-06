using PowRxVar;
using PowWinForms;
using WinAPI.Kernel32;

namespace LayoutDbgApp;

static class Program
{
	[STAThread]
	static void Main()
	{
		ApplicationConfiguration.Initialize();

		Kernel32Methods.AllocConsole();

		using var win = new MainWin().Track();

		Application.Run(win);

		Var.CheckForUnDisposedDisps(true);
	}
}