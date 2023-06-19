using System.Drawing;
using System.Reactive.Linq;
using System.Reflection.Metadata;
using PowBasics.Geom;
using SysWinLib;
using SysWinLib.Structs;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

namespace Demos.Categories.Base;

static class SysWinDemo
{
	private static readonly Brush brush1 = new SolidBrush(Color.DodgerBlue);
	private static readonly Brush brush2 = new SolidBrush(Color.PaleGreen);

	public static SysWin Run()
	{
		var win = new SysWin(opt =>
		{
			opt.CreateWindowParams = new CreateWindowParams
			{
				Name = "SysWin Demo",
				X = -300,
				Y = 250,
				Width = 256,
				Height = 256,
			};
		});

		win.WhenMsg.WhenKEYDOWN().Where(e => e.Key == VirtualKey.D1).Subscribe(_ =>
		{
			L("-> Sending DestroyWindow");
			User32Methods.DestroyWindow(win.Handle);
		});

		win.WhenMsg.WhenKEYDOWN().Where(e => e.Key == VirtualKey.D2).Subscribe(_ =>
		{
			L("-> sysWin.Dispose()");
			win.Dispose();
		});

		win.WhenMsg.WhenPAINT().Subscribe(e =>
		{
			using var gfx = Graphics.FromHwnd(e.Hwnd);
			var r1 = new R(0, 0, 128, 256);
			var r2 = new R(128, 0, 128, 256);
			gfx.FillRectangle(brush1, r1.ToDrawRect());
			gfx.FillRectangle(brush2, r2.ToDrawRect());
		});

		win.Init();

		return win;
	}
}