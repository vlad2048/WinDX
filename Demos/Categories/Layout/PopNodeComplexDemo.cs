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
			using (r[n0].StratStack(Dir.Horz).M)
			{
				r.Gfx.FillR(r.Gfx.R, Consts.BrushN0);
				using (r[n1].DimFixFil(70).M)
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN1);
				}
				using (r[n2].StratStack(Dir.Vert).M)
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushN2);
					using (r[c1]) { }

					using (r[p1].Dim(250, 150).Pop().M)
					{
						r.Gfx.FillR(r.Gfx.R, Consts.BrushP1);
						using (r[n4].StratStack(Dir.Horz).M)
						{
							r.Gfx.FillR(r.Gfx.R, Consts.BrushN4);
							using (r[n5].DimFixFil(50).M)
							{
								r.Gfx.FillR(r.Gfx.R, Consts.BrushN5);
								using (r[p2].Dim(45, 230).Pop().M)
								{
									r.Gfx.FillR(r.Gfx.R, Consts.BrushP2);
									using (r[n6].DimFil().M)
									{
										r.Gfx.FillR(r.Gfx.R, Consts.BrushN6);
									}
								}
							}

							using (r[c2]) { }
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
				using (r[n3].DimFilFix(90).M)
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
				using (r[p3].Dim(190, 340).Pop().M)
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushP3);
					using (r[c3]) { }
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
				using (r[n7].M)
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