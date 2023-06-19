using PowWinForms;
using Structs;

namespace WinSpectorLib;

public static class WinSpector
{
	public static void Run() => RunInternal();

	internal static void RunInternal(params DemoNfo[] demos)
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Application.SetHighDpiMode(HighDpiMode.SystemAware);

		using var winSpectorWin = new WinSpectorWin(demos).Track();
		Application.Run(winSpectorWin);
	}
}