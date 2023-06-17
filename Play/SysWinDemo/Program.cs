using System.Drawing;
using PowBasics.Geom;
using PowRxVar;
using RenderLib;
using RenderLib.Structs;
using WinAPI.Windows;
using SysWinLib;
using SysWinLib.Structs;

namespace SysWinDemo;

static class Program
{
	private static void Main()
	{
		var win = new SysWin(opt =>
		{
			opt.CreateWindowParams = new CreateWindowParams
			{
				Name = "Main Win",
				X = -300,
				Y = 250,
				Width = 256,
				Height = 256,
			};
		});

		var renderWinCtx = RendererGetter.Get(RendererType.Direct2D, win).D(win.D);

		win.WhenMsg.WhenPAINT().Subscribe(_ => {
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
}