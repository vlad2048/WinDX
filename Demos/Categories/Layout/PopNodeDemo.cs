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

sealed class PopNodeDemo : Win
{
	public PopNodeDemo() : base(opt => opt.R = new R(-300, 50, 150, 200))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFill1 = new NodeState().D(D);
		var nodeFill2 = new NodeState().D(D);
		var nodePop = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fil, Stack(Dir.Vert, Align.Start)))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.BrushRoot);
				using (r.Flex(nodeFill1, Vec.Fix(120, 75), Fill))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushFill1);
				}
				using (r.Flex(nodePop, Vec.Fix(250, 50), Pop))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushPop);
				}
				using (r.Flex(nodeFill2, Vec.Fix(80, 100), Fill))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushFill2);
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