﻿using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class BalancedCtrlHandlingDemo : Win
{
	public BalancedCtrlHandlingDemo() : base(opt => opt.R = new R(-600, 64, 500, 400))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFit = new NodeState().D(D);
		var c1 = new C1Ctrl().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].Dim(400, 350).M)
			{
				r.FillR(r.R, Consts.WinBrush1);
				using (r[nodeFit].DimFit().Marg(30).M)
				{
					r.FillR(r.R, Consts.WinBrush2);
					using (r[c1])
					{
					}
				}
			}
		}).D(D);
	}


	private sealed class C1Ctrl : Ctrl
	{
		public C1Ctrl()
		{
			var nodeRoot = new NodeState().D(D);
			var nodeFil = new NodeState().D(D);
			var c2 = new C2Ctrl().D(D);

			WhenRender.Subscribe(r =>
			{
				using (r[nodeRoot].Dim(180, 250).M)
				{
					r.FillR(r.R, Consts.C1Brush1);
					using (r[nodeFil].DimFit().Marg(20).M)
					{
						r.FillR(r.R, Consts.C1Brush2);
						using (r[c2])
						{
						}
					}
				}
			}).D(D);
		}
	}

	private sealed class C2Ctrl : Ctrl
	{
		public C2Ctrl()
		{
			var nodeRoot = new NodeState().D(D);
			var nodeFil = new NodeState().D(D);
			var c3 = new C3Ctrl().D(D);

			WhenRender.Subscribe(r =>
			{
				using (r[nodeRoot].Dim(110, 170).M)
				{
					r.FillR(r.R, Consts.C2Brush1);
					using (r[nodeFil].DimFit().Marg(10).M)
					{
						r.FillR(r.R, Consts.C2Brush2);
						using (r[c3])
						{
						}
					}
				}
			}).D(D);
		}
	}

	private sealed class C3Ctrl : Ctrl
	{
		public C3Ctrl()
		{
			var nodeRoot = new NodeState().D(D);
			var nodeFil = new NodeState().D(D);

			WhenRender.Subscribe(r =>
			{
				using (r[nodeRoot].Dim(60, 80).M)
				{
					r.FillR(r.R, Consts.C3Brush1);
					using (r[nodeFil].Marg(5).M)
					{
						r.FillR(r.R, Consts.C3Brush2);
					}
				}
			}).D(D);
		}
	}

	private static class Consts
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
}
