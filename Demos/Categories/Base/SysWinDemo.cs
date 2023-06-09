﻿using System.Drawing;
using PowBasics.Geom;
using SysWinLib;
using SysWinLib.Structs;
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
				Height = 128,
			};
			opt.NCStrat = NCStrats.Custom();
		});

		/*
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

		win.WhenMsg.WhenKEYDOWN().Where(e => e.Key == VirtualKey.D3).Subscribe(_ =>
		{
			L("-> sysWin.SetR()");
			var r = new R(-280, 200, 250, 120);
			win.SetR(r, 0);
		});
		*/

		win.WhenMsg.WhenPAINT().Subscribe(e =>
		{
			using var gfx = Graphics.FromHwnd(e.Hwnd);
			var r1 = new R(0, 0, 128, 128);
			var r2 = new R(128, 0, 128, 128);
			gfx.FillRectangle(brush1, r1.ToDrawRect());
			gfx.FillRectangle(brush2, r2.ToDrawRect());
		});

		win.Init();

		return win;
	}
}