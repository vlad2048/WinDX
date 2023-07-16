using PowBasics.Geom;
using WinAPI.User32;
using WinAPI.Utils.Exts;

namespace UserEvents.Utils;

public static class MouseUtils
{
	public static Pt GetMousePos()
	{
		User32Methods.GetCursorPos(out var pt);
		return pt.ToPt();
	}
}