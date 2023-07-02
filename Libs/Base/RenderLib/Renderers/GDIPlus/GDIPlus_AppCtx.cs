using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers.GDIPlus.Utils;
using RenderLib.Structs;
using SysWinInterfaces;
using WinAPI.Utils.Exts;

namespace RenderLib.Renderers.GDIPlus;

public sealed class GDIPlus_AppCtx : IRenderAppCtxWithDispose
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly Pencils pencils;

	internal GDIPlus_AppCtx()
	{
		pencils = new Pencils().D(d);
	}

	public IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win) => new GDIPlus_WinCtx(win, pencils);
}

public sealed class GDIPlus_WinCtx : IRenderWinCtx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISysWinRenderingSupport win;
	private readonly Pencils pencils;

	internal GDIPlus_WinCtx(ISysWinRenderingSupport win, Pencils pencils)
	{
		this.win = win;
		this.pencils = pencils;
	}

	public void Resize(Sz sz) {}

	public IGfx GetGfx(bool measureOnly) => new GDIPlus_Gfx(win, pencils, measureOnly);
}

public sealed class GDIPlus_Gfx : IGfx
{
	private const TextFormatFlags TextFormatFlags = 0;
	private static readonly Size TextProposedSize = new(int.MaxValue, int.MaxValue);

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly Pencils pencils;
	private readonly Graphics gfx;
	private readonly bool measureOnly;

	public R R { get; set; }

	internal GDIPlus_Gfx(ISysWinRenderingSupport win, Pencils pencils, bool measureOnly)
	{
		this.pencils = pencils;
		this.measureOnly = measureOnly;
		R = win.ClientR.V;
		gfx = Graphics.FromHwnd(win.Handle).D(d);
	}

	private bool DrawDisabled => measureOnly || R.IsDegenerate;

	public void FillR(R r, BrushDef brush)
	{
		if (DrawDisabled) return;
		gfx.FillRectangle(pencils.GetBrush(brush), r.ToDrawRect());
	}

	public void DrawR(R r, PenDef pen)
	{
		if (DrawDisabled) return;
		gfx.DrawRectangle(pencils.GetPen(pen), r.ReduceDimsByOne().ToDrawRect());
	}

	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
		if (DrawDisabled) return;
		var isHorz = a.Y == b.Y;
		var isVert = a.X == b.X;

		// ReSharper disable once CompareOfFloatsByEqualityOperator
		if (penDef.Width == 1)
		{
			if (isHorz)
			{
				if (a.X > b.X)
					(a, b) = (b, a);
				b = new Pt(Math.Max(a.X, b.X - 1), b.Y);
			}

			if (isVert)
			{
				if (a.Y > b.Y)
					(a, b) = (b, a);
				b = new Pt(b.X, Math.Max(a.Y, b.Y - 1));
			}
		}

		gfx.DrawLine(pencils.GetPen(penDef), a.ToDrawPt(), b.ToDrawPt());
	}

	public void DrawBmp(Bitmap bmp)
	{
		if (DrawDisabled) return;
		gfx.DrawImage(bmp, R.Pos.ToDrawPt());
	}




	public Sz MeasureText_(string text, FontDef fontDef) => TextRenderer.MeasureText(
		text,
		pencils.GetFont(fontDef),
		TextProposedSize,
		TextFormatFlags
	).ToSz();

	public void DrawText_(string text, FontDef fontDef, Color color)
	{
		if (DrawDisabled) return;
		TextRenderer.DrawText(
			gfx,
			text,
			pencils.GetFont(fontDef),
			R.Pos.ToDrawPt(),
			color,
			TextFormatFlags
		);
	}
}



file static class GfxUtils
{
	public static Sz ToSz(this Size sz) => new(sz.Width, sz.Height);
}
