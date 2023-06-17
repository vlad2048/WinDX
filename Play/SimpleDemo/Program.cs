﻿using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Utils;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;
using WinSpectorLib;

namespace SimpleDemo;

static class Program
{
	static void Main()
	{
		using (var d = new Disp())
		{
			new SimpleWin().D(d);
			//new SimpleWin().D(d);
			WinSpector.Run();
		}
		VarDbg.CheckForUndisposedDisps(true);
	}
}

sealed class SimpleWin : Win
{
	public SimpleWin() : base(opt => opt.R = new R(-600, 64, 500, 400))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFit = new NodeState().D(D);
		var c1 = new C1Ctrl().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fix(400, 350), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.WinBrush1);
				using (r.Flex(nodeFit, Vec.Fit, Strats.Fill, Mg.Mk(30)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.WinBrush2);
					using (r.Ctrl(c1))
					{
					}
				}
			}
		}).D(D);
	}
}

sealed class C1Ctrl : Ctrl
{
	public C1Ctrl()
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFil = new NodeState().D(D);
		var c2 = new C2Ctrl().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fix(180, 250), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.C1Brush1);
				using (r.Flex(nodeFil, Vec.Fit, Strats.Fill, Mg.Mk(20)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.C1Brush2);
					using (r.Ctrl(c2))
					{
					}
				}
			}
		}).D(D);
	}
}
sealed class C2Ctrl : Ctrl
{
	public C2Ctrl()
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFil = new NodeState().D(D);
		var c3 = new C3Ctrl().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fix(110, 170), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.C2Brush1);
				//using (r.Flex(nodeFil, Vec.Fit, Strats.Fill, Mg.Mk(10)))
				r.Flex(nodeFil, Vec.Fit, Strats.Fill, Mg.Mk(10));
				{
					r.Gfx.FillR(r.Gfx.R, Consts.C2Brush2);
					using (r.Ctrl(c3))
					{
					}
				}
			}
		}).D(D);
	}
}
sealed class C3Ctrl : Ctrl
{
	public C3Ctrl()
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFil = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fix(60, 80), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.C3Brush1);
				using (r.Flex(nodeFil, Vec.Fil, Strats.Fill, Mg.Mk(5)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.C3Brush2);
				}
			}
		}).D(D);
	}
}

static class Consts
{
	public static readonly BrushDef WinBrush1 = new SolidBrushDef(Color.FromArgb(252, 144, 3));
	public static readonly BrushDef WinBrush2 = new SolidBrushDef(Color.FromArgb(176, 121, 49));
	public static readonly BrushDef C1Brush1 = new SolidBrushDef(Color.FromArgb(232, 242, 34));
	public static readonly BrushDef C1Brush2 = new SolidBrushDef(Color.FromArgb(193, 199, 68));
	public static readonly BrushDef C2Brush1 = new SolidBrushDef(Color.FromArgb(58, 240, 38));
	public static readonly BrushDef C2Brush2 = new SolidBrushDef(Color.FromArgb(63, 158, 52));
	public static readonly BrushDef C3Brush1 = new SolidBrushDef(Color.FromArgb(72, 124, 247));
	public static readonly BrushDef C3Brush2 = new SolidBrushDef(Color.FromArgb(70, 90, 138));
}