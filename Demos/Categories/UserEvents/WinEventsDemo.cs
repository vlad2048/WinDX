using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.UserEvents;

sealed class WinEventsDemo : Win
{
	public WinEventsDemo() : base(opt => opt.R = new R(-350, 100, 200, 300))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeTop = new NodeState().D(D);
		var nodePop = new NodeState().D(D);
		var childCtrl = new ChildCtrl().D(D);
		var nodeBottom = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.Gfx.FillR(Consts.BrushRoot);

				using (r[nodeTop].DimFilFix(100).M)
					r.Gfx.FillR(Consts.BrushTop);

				using (r[nodePop].StratStack(Dir.Vert).Dim(250, 150).Pop().M)
				{
					r.Gfx.FillR(Consts.BrushPop);

					//using (r[childCtrl]) { }
				}

				using (r[nodeBottom].M)
					r.Gfx.FillR(Consts.BrushBottom);
			}
		}).D(D);
	}

	private sealed class ChildCtrl : Ctrl
	{
		public ChildCtrl()
		{
			var nodeTop2 = new NodeState().D(D);
			var nodePop2 = new NodeState().D(D);

			WhenRender.Subscribe(r =>
			{
				using (r[nodeTop2].DimFilFix(60).M)
					r.Gfx.FillR(Consts.BrushTop2);
				using (r[nodePop2].Dim(300, 50).Pop().M)
					r.Gfx.FillR(Consts.BrushPop2);
			}).D(D);
		}
	}

	private static class Consts
	{
		public static readonly BrushDef BrushRoot = new SolidBrushDef(Color.FromArgb(20, 57, 117));
		public static readonly BrushDef BrushTop = new SolidBrushDef(Color.FromArgb(51, 102, 130));
		public static readonly BrushDef BrushPop = new SolidBrushDef(Color.FromArgb(85, 151, 201));
		public static readonly BrushDef BrushBottom = new SolidBrushDef(Color.FromArgb(70, 135, 54));

		public static readonly BrushDef BrushTop2 = new SolidBrushDef(Color.FromArgb(112, 117, 55));
		public static readonly BrushDef BrushPop2 = new SolidBrushDef(Color.FromArgb(208, 219, 79));
	}
}


















/*using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.UserEvents;

sealed class WinEventsDemo : Win
{
	public WinEventsDemo() : base(opt => opt.R = new R(-350, 100, 200, 300))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeTop = new NodeState().D(D);
		var nodePop = new NodeState().D(D);
		var nodeBottom = new NodeState().D(D);
		var nodeTop2 = new NodeState().D(D);
		var nodePop2 = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.Gfx.FillR(Consts.BrushRoot);

				using (r[nodeTop].DimFilFix(100).M)
					r.Gfx.FillR(Consts.BrushTop);

				using (r[nodePop].StratStack(Dir.Vert).Dim(250, 150).Pop().M)
				{
					r.Gfx.FillR(Consts.BrushPop);

					using (r[nodeTop2].DimFilFix(60).M)
						r.Gfx.FillR(Consts.BrushTop2);
					using (r[nodePop2].Dim(300, 50).Pop().M)
						r.Gfx.FillR(Consts.BrushPop2);
				}

				using (r[nodeBottom].M)
					r.Gfx.FillR(Consts.BrushBottom);
			}
		}).D(D);
	}

	private static class Consts
	{
		public static readonly BrushDef BrushRoot = new SolidBrushDef(Color.FromArgb(20, 57, 117));
		public static readonly BrushDef BrushTop = new SolidBrushDef(Color.FromArgb(51, 102, 130));
		public static readonly BrushDef BrushPop = new SolidBrushDef(Color.FromArgb(85, 151, 201));
		public static readonly BrushDef BrushBottom = new SolidBrushDef(Color.FromArgb(70, 135, 54));

		public static readonly BrushDef BrushTop2 = new SolidBrushDef(Color.FromArgb(112, 117, 55));
		public static readonly BrushDef BrushPop2 = new SolidBrushDef(Color.FromArgb(208, 219, 79));
	}
}*/