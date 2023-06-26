using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class PopNodeComplexDemo : Win
{
	public PopNodeComplexDemo() : base(opt => opt.R = new R(-300, 50, 200, 100))
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
			using (r.Flex(F(n0).StratStack(Dir.Horz)))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.BrushN0);
				using (r.Flex(F(n1).DimFixFil(70)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN1);
				}
				using (r.Flex(F(n2).StratStack(Dir.Vert)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN2);
					using (r.Ctrl(c1)) { }

					using (r.Flex(F(p1).Dim(250, 150).Pop()))
					{
						r.Gfx.FillR(r.Gfx.R, Consts.BrushP1);
						using (r.Flex(F(n4).StratStack(Dir.Horz)))
						{
							r.Gfx.FillR(r.Gfx.R, Consts.BrushN4);
							using (r.Flex(F(n5).DimFixFil(50)))
							{
								r.Gfx.FillR(r.Gfx.R, Consts.BrushN5);
								using (r.Flex(F(p2).Dim(45, 230).Pop()))
								{
									r.Gfx.FillR(r.Gfx.R, Consts.BrushP2);
									using (r.Flex(F(n6).DimFil()))
									{
										r.Gfx.FillR(r.Gfx.R, Consts.BrushN6);
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
				using (r.Flex(F(n3).DimFilFix(90)))
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
				using (r.Flex(F(p3).Dim(190, 340).Pop()))
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
				using (r.Flex(F(n7)))
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