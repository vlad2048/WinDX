using PowBasics.Geom;
using RenderLib.Structs;

namespace RenderLib.Renderers.Dummy;

/*public class Dummy_AppCtx : IRenderAppCtxWithDispose
{
	private static readonly IRenderWinCtx winCtx = new Dummy_WinCtx();
	public void Dispose() {}
	public IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win) => winCtx;
}

public class Dummy_WinCtx : IRenderWinCtx
{
	private static readonly IGfx gfx = new Dummy_Gfx();
	public void Dispose() {}
	public void Resize(Sz sz) { }
	public IGfx GetGfx() => gfx;
}*/

public class Dummy_Gfx : IGfx
{
	public void Dispose() { }

	public R R { get; set; } = R.Empty;

	public void Dbg() { }
	public void FillR(R r, BrushDef brush) { }
	public void DrawR(R r, PenDef pen) { }
	public void DrawLine(Pt a, Pt b, PenDef penDef) { }
}