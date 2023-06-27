global using D2D = Vortice.Direct2D1;
global using DWRITE = Vortice.DirectWrite;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using PowBasics.Geom;
using PowRxVar;
using RenderLib;
using RenderLib.Renderers.Direct2D;
using RenderLib.Structs;
using WinAPI.Windows;
using SysWinLib;
using SysWinLib.Structs;
using Vortice.Mathematics;
using WinAPI.Utils.Exts;
using Color = System.Drawing.Color;

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
				X = -500,
				Y = 250,
				Width = 480,
				Height = 256,
			};
		});



		var isInit = false;
		DWRITE.IDWriteTextFormat textFormat = null!;
		DWRITE.IDWriteTextLayout textLayout = null!;
		D2D.ID2D1Brush brush = null!;

		if (true)
		{
			var renderWinCtx = RendererGetter.Get(RendererType.Direct2D, win).D(win.D);
			win.WhenMsg.WhenPAINT().Subscribe(_ =>
			{
				using var gfx = (Direct2D_Gfx)renderWinCtx.GetGfx(false);
				var dw = gfx.DWRITEFactory;

				var text = "  Windowsj  ";

				if (!isInit)
				{
					isInit = true;
					textFormat = dw.CreateTextFormat("Segoe UI", DWRITE.FontWeight.Normal, DWRITE.FontStyle.Normal, FontSizePixels);
					textFormat.TextAlignment = DWRITE.TextAlignment.Leading;
					textFormat.ParagraphAlignment = DWRITE.ParagraphAlignment.Near;
					textFormat.WordWrapping = DWRITE.WordWrapping.NoWrap;
					textFormat.ReadingDirection = DWRITE.ReadingDirection.LeftToRight;
					textFormat.FlowDirection = DWRITE.FlowDirection.TopToBottom;

					textLayout = dw.CreateTextLayout(text, textFormat, 1000, 1000);

					var trimming = dw.CreateEllipsisTrimmingSign(textLayout);
					var trimmingOpt = new DWRITE.Trimming
					{
						Granularity = DWRITE.TrimmingGranularity.None
					};
					textFormat.SetTrimming(trimmingOpt, trimming);


					brush = gfx.T.CreateSolidColorBrush(Colors.Black);

					var sz = new Sz(
						(int)Math.Round(textLayout.Metrics.Width),
						(int)Math.Round(textLayout.Metrics.Height)
					);
					Console.WriteLine($"sz: {sz}");
				}

				gfx.FillR(win.ClientR.V, backBrushDef);

				var pos = new Vector2(0, 0);
				D2D.DrawTextOptions opts = D2D.DrawTextOptions.None;

				gfx.T.DrawTextLayout(pos, textLayout, brush, opts);
			});
		}
		else
		{
			var done = false;
			win.WhenMsg.WhenPAINT().Subscribe(_ =>
			{
				using var gfx = Graphics.FromHwnd(win.Handle);
				var r = win.ClientR.V.ToDrawRect();
				gfx.FillRectangle(backBrush, r);
				TextFormatFlags flags = 0; //TextFormatFlags.NoPadding;
				var text = "Windowsj";
				TextRenderer.DrawText(gfx, text, font, Point.Empty, Color.Black, flags);
				if (!done)
				{
					done = true;
					var proposedSz = r.Size;
					var sz = TextRenderer.MeasureText(text, font, proposedSz, flags);
					Console.WriteLine($"sz: {sz.Width}x{sz.Height}");
				}
			});
		}


		

		win.Init();
		App.Run();
	}

	private const int FontSizePoints = 9;
	private const int FontSizePixels = FontSizePoints * 96 / 72;

	private static readonly BrushDef backBrushDef = new SolidBrushDef(Color.FromArgb(240, 240, 240));

	private static readonly Brush backBrush = new SolidBrush(Color.FromArgb(240, 240, 240));

	/*
		points = pixels * 72 / 96
		pixels = points * 96 / 72

		- WinForms & Paint.NET use points
		- GDI can use both
		- DirectWrite uses pixels
	*/
	private static readonly Font font = new(new FontFamily("Segoe UI"), FontSizePoints, FontStyle.Regular, GraphicsUnit.Point);
}