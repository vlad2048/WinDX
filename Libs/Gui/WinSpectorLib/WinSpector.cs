using PowWinForms;

namespace WinSpectorLib;

public static class WinSpector
{
	public static void Run()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.SetHighDpiMode(HighDpiMode.SystemAware);

		using var winSpectorWin = new WinSpectorWin().Track();
		Application.Run(winSpectorWin);
	}
}