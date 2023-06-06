using PowBasics.Geom;
using RenderLib.Renderers;
using RenderLib.Structs;

namespace RenderLib;

public static class GfxExts
{
	public static void FillDrawR(this IGfx gfx, R r, BrushDef brush, PenDef pen)
	{
		gfx.FillR(r, brush);
		gfx.DrawR(r, pen);
	}
}