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
		IGfx gfx,
		Pt ofs
	)
	{
		if (Cfg.V.Tweaks.DisableWinSpectorDrawing) return;

		gfx.R = layout.R - ofs;

		bool GetR(IRoMayVar<MixNode> mayNodVar, out R nodeR)
		{
			nodeR = R.Empty;
			if (mayNodVar.V.IsNone(out var nod)) return false;
			if (nod.IsNodeState())
			{
				nodeR = layout.Set.RMap[nod.GetNodeState()];
				return true;
			}
			if (nod.IsCtrl())
			{
				nodeR = nod.GetCtrlR(layout.Set);
				return true;
			}
			return false;
		}

		bool GetRSt(IRoMayVar<INode> mayNodeVar, out R nodeR)
		{
			nodeR = R.Empty;
			if (mayNodeVar.V.IsSome(out var st))
			{
				nodeR = layout.Set.RMap[(NodeState)st];
				return true;
			}
			return false;
		}

		var isSel = GetR(state.SelNode, out var selR);
		var isHov = GetR(state.HovNode, out var hovR);
		var isLock = GetRSt(state.LockedNode, out var lockR);
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

		if (isLock)
		{
			gfx.DrawLock(lockR);
		}



		/*
		if (layout.IsEmpty || Cfg.V.Tweaks.DisableWinSpectorDrawing) return;

		gfx.R = layout.RMap.Values.First().WithZeroPos();

		bool GetRMix(IRoMayVar<MixNode> nod, out R nodeR)
		{
			nodeR = R.Empty;
			if (nod.V.IsNone(out var node) || node.V is not StFlexNode { State: var ns }) return false;
			if (layout.RMap.TryGetValue(ns, out nodeR))
				return true;
			if (layout.SysPartition.RMap.TryGetValue(ns, out nodeR))
				return true;
			return false;
		}

		bool GetR(IRoMayVar<INode> nod, out R nodeR)
		{
			nodeR = R.Empty;
			if (nod.V.IsNone(out var ns)) return false;
			if (layout.RMap.TryGetValue((NodeState)ns, out nodeR))
				return true;
			if (layout.SysPartition.RMap.TryGetValue((NodeState)ns, out nodeR))
				return true;
			return false;
		}

		var isSel = GetRMix(state.SelNode, out var selR);
		var isHov = GetRMix(state.HovNode, out var hovR);
		var isLock = GetR(state.LockedNode, out var lockR);
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

		if (isLock)
		{
			gfx.DrawLock(lockR);
		}
		*/
	}
}


file static class Consts
{
	public static void DrawLock(this IGfx gfx, R r)
	{
		gfx.FillR(r, LockBrush);
	}


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

	private static readonly BrushDef LockBrush = new SolidBrushDef(Color.FromArgb(127, 0, 0, 0));
	private static readonly PenDef SelPen = new(Color.DeepPink, 1);
	private static readonly PenDef HoverPen = new(Color.Black, 1);
	private static readonly PenDef HoverPenMiddle = new(Color.White, 1)
	{
		DashStyle = DashStyleDef.Dash,
	};
}