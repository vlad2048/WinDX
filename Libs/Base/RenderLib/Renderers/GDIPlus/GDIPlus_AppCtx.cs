using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers.GDIPlus.Utils;
using RenderLib.Structs;
using SysWinInterfaces;
using WinAPI.Utils.Exts;

namespace RenderLib.Renderers.GDIPlus;

public class GDIPlus_AppCtx : IRenderAppCtxWithDispose
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

public class GDIPlus_WinCtx : IRenderWinCtx
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

	public IGfx GetGfx() => new GDIPlus_Gfx(win, pencils);
}

public class GDIPlus_Gfx : IGfx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly Pencils pencils;
	private readonly Graphics gfx;

	public R R { get; set; }

	internal GDIPlus_Gfx(ISysWinRenderingSupport win, Pencils pencils)
	{
		this.pencils = pencils;
		R = win.ClientR.V;
		gfx = Graphics.FromHwnd(win.Handle).D(d);
	}

	public void Dbg() {}
	public void FillR(R r, BrushDef brush) => gfx.FillRectangle(pencils.GetBrush(brush), r.ToDrawRect());
	public void DrawR(R r, PenDef pen) => gfx.DrawRectangle(pencils.GetPen(pen), r.ReduceDimsByOne().ToDrawRect());
	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
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
}