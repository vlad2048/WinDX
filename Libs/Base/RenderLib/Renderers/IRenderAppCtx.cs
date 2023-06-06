using PowBasics.Geom;
using RenderLib.Structs;
using SysWinLib;

namespace RenderLib.Renderers;

public interface IRenderAppCtx : IDisposable
{
	IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win);
}

public interface IRenderWinCtx : IDisposable
{
	void Resize(Sz sz);
	IGfx GetGfx();
}

public interface IGfx : IDisposable
{
	R R { get; }

	void Dbg();
	void FillR(R r, BrushDef brush);
	void DrawR(R r, PenDef pen);
	void DrawLine(Pt a, Pt b, PenDef penDef);
}
