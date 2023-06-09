using System.Drawing;
using D3DPlay.Rendering;
using PowRxVar;
using SysWinLib.Structs;
using SysWinLib;
using Vortice.Mathematics;
using WinAPI.User32;
using WinAPI.Windows;

namespace D3DPlay;

static class Program
{
	public static void Main()
	{
		using var win = MakeWindow();
		win.Init();

		var appCtx = new RenderAppCtx().D(win.D);
		var winCtx = appCtx.GetWinCtx(win).D(win.D);
		var brush = winCtx.D2DRenderTarget.CreateSolidColorBrush(new Color4(64, 32, 255, 255)).D(win.D);

		win.WhenMsg.WhenPAINT().Subscribe(_ => {
			using var gfx = winCtx.GetGfx();
			var r = new RectangleF(30, 20, 70, 50);

			gfx.T.BeginDraw();
			gfx.T.FillRectangle(r, brush);
			gfx.T.EndDraw();

			gfx.WinCtx.SwapChain.Present(0);

		}).D(win.D);

		App.Run();
	}

	private static SysWin MakeWindow() => new(opt => {
		opt.CreateWindowParams = new CreateWindowParams {
			Name = "Main Win",
			Styles = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE,
			X = -500,
			Y = 100,
			Width = 400,
			Height = 500,
		};
	});
}

