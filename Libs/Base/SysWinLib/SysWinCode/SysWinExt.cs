using PowBasics.Geom;
using WinAPI.DwmApi;
using WinAPI.NetCoreEx.Geometry;
using WinAPI.User32;
using WinAPI.Utils.Exts;

// ReSharper disable once CheckNamespace
namespace SysWinLib;

public enum RType
{
	Win,
	WinWithGripAreas,
	Client,
}

public static class SysWinExt
{
	public static R GetR(this SysWin win, RType type)
	{
		Rectangle r;
		switch (type)
		{
			case RType.Win:
				DwmApiHelpers.DwmGetWindowAttribute(win.Handle, DwmWindowAttributeType.DWMWA_EXTENDED_FRAME_BOUNDS, out r);
				break;
			case RType.WinWithGripAreas:
				User32Methods.GetWindowRect(win.Handle, out r);
				break;
			case RType.Client:
				User32Methods.GetClientRect(win.Handle, out r);
				break;
			default:
				throw new ArgumentException($"Invalid RType: {type}");
		}
		return r.ToR();
	}

	public static void SetR(this SysWin win, R r, WindowPositionFlags flags) =>
		User32Methods.SetWindowPos(win.Handle, IntPtr.Zero, r.X, r.Y, r.Width, r.Height, flags);
}