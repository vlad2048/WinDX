using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers.GDIPlus.Utils;
using RenderLib.Structs;
using SysWinInterfaces;
using WinAPI.User32;
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
	private const TextFormatFlags TextFormatFlags = System.Windows.Forms.TextFormatFlags.PreserveGraphicsClipping;
	private static readonly Size TextProposedSize = new(int.MaxValue, int.MaxValue);

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly Pencils pencils;
	public Graphics Gfx { get; }
	private readonly bool measureOnly;
	private readonly Stack<R> clipStack = new();

	public R R { get; set; }

	internal GDIPlus_Gfx(ISysWinRenderingSupport win, Pencils pencils, bool measureOnly)
	{
		this.pencils = pencils;
		this.measureOnly = measureOnly;
		R = win.ClientR.V;
		Gfx = Graphics.FromHwnd(win.Handle).D(d);

		//var hdc = User32Methods.GetWindowDC(win.Handle);
		//Gfx = Graphics.FromHdc(hdc).D(d);
	}

	private bool DrawDisabled => measureOnly || R.IsDegenerate;

	public void PushClip(R clipR)
	{
		if (DrawDisabled) return;
		clipStack.Push(clipR);
		Gfx.SetClipStack(clipStack);
	}

	public void PopClip()
	{
		if (DrawDisabled) return;
		clipStack.Pop();
		Gfx.SetClipStack(clipStack);
	}
	

	public void FillR(R r, BrushDef brush)
	{
		if (DrawDisabled) return;
		Gfx.FillRectangle(pencils.GetBrush(brush), r.ToDrawRect());
	}

	public void DrawR(R r, PenDef pen)
	{
		if (DrawDisabled) return;
		Gfx.DrawRectangle(pencils.GetPen(pen), r.ReduceDimsByOne().ToDrawRect());
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

		Gfx.DrawLine(pencils.GetPen(penDef), a.ToDrawPt(), b.ToDrawPt());
	}

	public void DrawBmp(Bitmap bmp)
	{
		if (DrawDisabled) return;
		Gfx.DrawImage(bmp, R.Pos.ToDrawPt());
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
			Gfx,
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

	public static void SetClipStack(this Graphics gfx, Stack<R> clipStack)
	{
		if (clipStack.Any())
			gfx.SetClip(clipStack.Intersection().ToDrawRect());
		else
			gfx.ResetClip();
	}
}
