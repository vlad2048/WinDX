using PowBasics.Geom;
using RenderLib.Structs;

namespace RenderLib.Renderers.Dummy;

public sealed class Dummy_Gfx : IGfx
{
	public void Dispose() { }

	public R R { get; set; } = R.Empty;

	public void Dbg() { }
	public void FillR(R r, BrushDef brush) { }
	public void DrawR(R r, PenDef pen) { }
	public void DrawLine(Pt a, Pt b, PenDef penDef) { }
}