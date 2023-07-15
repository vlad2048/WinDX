using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class PopNodeDemo : Win
{
	public PopNodeDemo() : base(opt => opt.R = new R(-300, 50, 150, 200))
	{
		var nodeRoot = new NodeState("root").D(D);
		var nodeFill1 = new NodeState("fill1").D(D);
		var nodeFill2 = new NodeState("fill2").D(D);
		var nodePop = new NodeState("pop").D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.FillR(r.R, Consts.BrushRoot);
				using (r[nodeFill1].Dim(120, 75).M)
				{
					r.FillR(r.R, Consts.BrushFill1);
				}
				using (r[nodePop].Dim(250, 50).Pop().M)
				{
					r.FillR(r.R, Consts.BrushPop);
				}
				using (r[nodeFill2].Dim(80, 100).M)
				{
					r.FillR(r.R, Consts.BrushFill2);
				}
			}
		}).D(D);
	}

	private static class Consts
	{
		public static readonly BrushDef BrushRoot = new SolidBrushDef(Color.FromArgb(72, 124, 247));
		public static readonly BrushDef BrushFill1 = new SolidBrushDef(Color.FromArgb(252, 144, 3));
		public static readonly BrushDef BrushFill2 = new SolidBrushDef(Color.FromArgb(199, 98, 58));
		public static readonly BrushDef BrushPop = new SolidBrushDef(Color.FromArgb(245, 239, 66));
	}
}