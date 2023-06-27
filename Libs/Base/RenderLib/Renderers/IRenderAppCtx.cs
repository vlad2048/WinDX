﻿using PowBasics.Geom;
using RenderLib.Structs;
using SysWinInterfaces;

namespace RenderLib.Renderers;

public interface IRenderAppCtx
{
	IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win);
}

public interface IRenderAppCtxWithDispose : IRenderAppCtx, IDisposable
{
}

public interface IRenderWinCtx : IDisposable
{
	void Resize(Sz sz);
	IGfx GetGfx(bool measureOnly);
}

public interface IGfx : IDisposable
{
	R R { get; set; }

	void FillR(R r, BrushDef brush);
	void DrawR(R r, PenDef pen);
	void DrawLine(Pt a, Pt b, PenDef penDef);

	Sz MeasureText_(string text, FontDef fontDef);
	void DrawText_(string text, FontDef fontDef, Color color);
}

