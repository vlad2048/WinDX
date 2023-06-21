using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Utils;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class PopNodeComplexDemoWin : Win
{
	public PopNodeComplexDemoWin() : base(opt => opt.R = new R(-300, 50, 200, 100))
	{
		var c1 = new C1().D(D);
		var c2 = new C2().D(D);
		var n0 = new NodeState().D(D);
		var n1 = new NodeState().D(D);
		var n2 = new NodeState().D(D);
		var n4 = new NodeState().D(D);
		var n5 = new NodeState().D(D);
		var n6 = new NodeState().D(D);
		var p1 = new NodeState().D(D);
		var p2 = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(n0, Vec.Fil, Strats.Stack(Dir.Horz, Align.Start)))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.BrushN0);
				using (r.Flex(n1, Vec.FixFil(70), Strats.Fill))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN1);
				}
				using (r.Flex(n2, Vec.Fil, Strats.Stack(Dir.Vert, Align.Start)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN2);
					using (r.Ctrl(c1)) { }

					using (r.Flex(p1, Vec.Fix(250, 150), Strats.Pop))
					{
						r.Gfx.FillR(r.Gfx.R, Consts.BrushP1);
						using (r.Flex(n4, Vec.Fil, Strats.Stack(Dir.Horz, Align.Start)))
						{
							r.Gfx.FillR(r.Gfx.R, Consts.BrushN4);
							using (r.Flex(n5, Vec.FixFil(50), Strats.Fill))
							{
								r.Gfx.FillR(r.Gfx.R, Consts.BrushN5);
								using (r.Flex(p2, Vec.Fix(45, 230), Strats.Pop))
								{
									r.Gfx.FillR(r.Gfx.R, Consts.BrushP2);
									using (r.Flex(n6, Vec.Fil, Strats.Fill))
									{
										r.Gfx.FillR(r.Gfx.R, Consts.BrushP2);
									}
								}
							}

							using (r.Ctrl(c2)) { }
						}
					}
				}
			}
		}).D(D);
	}

	private sealed class C1 : Ctrl
	{
		public C1()
		{
			var n3 = new NodeState().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r.Flex(n3, Vec.FilFix(90), Strats.Fill))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN3);
				}
			}).D(D);
		}
	}

	private sealed class C2 : Ctrl
	{
		public C2()
		{
			var c3 = new C3().D(D);
			var p3 = new NodeState().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r.Flex(p3, Vec.Fix(190, 340), Strats.Pop))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushP3);
					using (r.Ctrl(c3)) { }
				}
			}).D(D);
		}
	}

	private sealed class C3 : Ctrl
	{
		public C3()
		{
			var n7 = new NodeState().D(D);
			WhenRender.Subscribe(r =>
			{
				using (r.Flex(n7, Vec.Fil, Strats.Fill))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN7);
				}
			}).D(D);
		}
	}

	private static class Consts
	{
		public static readonly BrushDef BrushN0 = new SolidBrushDef(Color.FromArgb(72, 124, 247));
		public static readonly BrushDef BrushN1 = new SolidBrushDef(Color.FromArgb(156, 29, 20));
		public static readonly BrushDef BrushN2 = new SolidBrushDef(Color.FromArgb(207, 58, 48));
		public static readonly BrushDef BrushN3 = new SolidBrushDef(Color.FromArgb(229, 232, 65));
		public static readonly BrushDef BrushN4 = new SolidBrushDef(Color.FromArgb(173, 54, 209));
		public static readonly BrushDef BrushN5 = new SolidBrushDef(Color.FromArgb(130, 45, 156));
		public static readonly BrushDef BrushN6 = new SolidBrushDef(Color.FromArgb(95, 22, 102));
		public static readonly BrushDef BrushN7 = new SolidBrushDef(Color.FromArgb(47, 145, 135));
		public static readonly BrushDef BrushP1 = new SolidBrushDef(Color.FromArgb(245, 189, 34));
		public static readonly BrushDef BrushP2 = new SolidBrushDef(Color.FromArgb(122, 218, 235));
		public static readonly BrushDef BrushP3 = new SolidBrushDef(Color.FromArgb(97, 107, 237));
	}
}