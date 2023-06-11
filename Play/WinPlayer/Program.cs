using System.Drawing;
using PowBasics.Geom;
using PowRxVar;
using RenderLib;
using RenderLib.Structs;
using WinAPI.User32;
using WinAPI.Windows;
using SysWinLib;
using SysWinLib.Structs;

namespace WinPlayer;

static class Program
{
	//[STAThread]
	static void Main()
	{
		// To customize application configuration such as set high DPI settings or default font,
		// see https://aka.ms/applicationconfiguration.
		//ApplicationConfiguration.Initialize();

		//var smallestSz = new Sz(136, 39);


		var win = new SysWin(opt =>
		{
			opt.CreateWindowParams = new CreateWindowParams
			{
				Name = "Main Win",
				Styles = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE,
				X = -300,
				Y = 250,
				Width = 256,
				Height = 256,
			};
		});

		//win.WhenInit().Subscribe(_ => L("INIT"));
		//win.ClientR.Subscribe(r => L($"{r}"));

		var renderWinCtx = RendererGetter.Get(RendererType.Direct2D, win).D(win.D);

		win.WhenMsg.WhenPAINT().Subscribe(e => {
			using var gfx = renderWinCtx.GetGfx();
			for (var i = 0; i < 6; i++) {
				var p0 = new Pt(5, 3 + i * 10);
				var p1 = new Pt(15, 3 + i * 10);
				gfx.DrawLine(p0, p1, new PenDef(Color.White, i + 1));
			}

			for (var i = 0; i < 6; i++) {
				var p0 = new Pt(30 + 3 + i * 10, 5);
				var p1 = new Pt(30 + 3 + i * 10, 15);
				gfx.DrawLine(p0, p1, new PenDef(Color.White, i + 1));
			}
		});

		win.Init();
		App.Run();
	}

	private static readonly BrushDef brush = new SolidBrushDef(Color.Green);
	private static readonly PenDef pen = new PenDef(Color.White, 2.0f);
}