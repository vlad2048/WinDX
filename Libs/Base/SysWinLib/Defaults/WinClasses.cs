using SysWinLib.Structs;
using SysWinLib.Utils;
using WinAPI.Gdi32;
using WinAPI.User32;

namespace SysWinLib.Defaults;

public static class WinClasses
{
	private static readonly Lazy<string> mainWindow = new(() => RegisterClassUtils.Register(
		"MainWindow",
		new RegisterClassParams
		{
			Styles =
				WindowClassStyles.CS_HREDRAW |
				WindowClassStyles.CS_VREDRAW |
				WindowClassStyles.CS_DBLCLKS |
				0
			,
			WinProc = SysWin.WndProc,
			BackgroundBrush = Gdi32Helpers.GetStockObject(StockObject.BLACK_BRUSH),
		}
	));
	public static string MainWindow => mainWindow.Value;
	
	/*private static readonly Lazy<string> slaveWindow = new(() => RegisterClassUtils.Register(
		"SlaveWindow",
		new RegisterClassParams
		{
			Styles =
				WindowClassStyles.CS_HREDRAW |
				WindowClassStyles.CS_VREDRAW |
				WindowClassStyles.CS_DBLCLKS |
				0
			,
			WinProc = SysWin.WndProc,
			BackgroundBrush = Gdi32Helpers.GetStockObject(StockObject.BLACK_BRUSH),
		}
	));
	public static string SlaveWindow => slaveWindow.Value;*/
}