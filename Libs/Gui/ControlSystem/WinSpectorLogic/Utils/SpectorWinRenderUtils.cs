using System.Drawing;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using RenderLib.Renderers;
using RenderLib.Structs;

namespace ControlSystem.WinSpectorLogic.Utils;

static class SpectorWinRenderUtils
{
	public static void Render(
		SpectorWinDrawState state,
		Partition layout,
		IGfx gfx
	)
	{
		gfx.R = layout.RMap.Values.First().WithZeroPos();

		bool GetR(IRoMayVar<MixNode> nod, out R nodeR)
		{
			nodeR = R.Empty;
			if (nod.V.IsNone(out var node) || node.V is not StFlexNode { State: var nodeState }) return false;
			return layout.RMap.TryGetValue(nodeState, out nodeR);
		}

		var isSel = GetR(state.SelNode, out var selR);
		var isHov = GetR(state.HovNode, out var hovR);
		if (isSel && isHov)
		{
			gfx.DrawSelHov(selR);
		}
		else
		{
			if (isSel)
				gfx.DrawSel(selR);
			if (isHov)
				gfx.DrawHov(hovR);
		}
	}
}


file static class Consts
{
	public static void DrawSelHov(this IGfx gfx, R r)
	{
		gfx.DrawR(r.Enlarge(-2), SelPen);
		gfx.DrawR(r.Enlarge(-1), HoverPen);
		gfx.DrawR(r.Enlarge(-0), SelPen);
		gfx.DrawR(r.Enlarge(-1), HoverPenMiddle);
	}

	public static void DrawSel(this IGfx gfx, R r)
	{
		gfx.DrawR(r.Enlarge(-2), SelPen);
		gfx.DrawR(r.Enlarge(-1), HoverPen);
		gfx.DrawR(r.Enlarge(-0), SelPen);
	}

	public static void DrawHov(this IGfx gfx, R r)
	{
		gfx.DrawR(r.Enlarge(-2), HoverPen);
		gfx.DrawR(r.Enlarge(-1), HoverPen);
		gfx.DrawR(r.Enlarge(-0), HoverPen);
		gfx.DrawR(r.Enlarge(-1), HoverPenMiddle);
	}

	private static readonly PenDef SelPen = new(Color.DeepPink, 1);
	private static readonly PenDef HoverPen = new(Color.Black, 1);
	private static readonly PenDef HoverPenMiddle = new(Color.White, 1)
	{
		DashStyle = DashStyleDef.Dash,
	};
}