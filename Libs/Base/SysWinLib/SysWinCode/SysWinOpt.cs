using PowBasics.Geom;
using SysWinLib.Structs;
using SysWinLib.Utils;
using WinAPI.User32;

// ReSharper disable once CheckNamespace
namespace SysWinLib;

public delegate HitTestResult NCHitTest(R winR, Pt pt);

public class SysWinOpt
{
	public bool NCAreaCustom { get; set; }
	public NCHitTestDelegate NCAreaHitTest { get; set; } = NcHitTestUtils.Make();
	public bool CouldBeMainWindow { get; set; } = true;

	private SysWinOpt()
	{
	}

	internal static SysWinOpt Make(Action<SysWinOpt>? optFun)
	{
		var opt = new SysWinOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}