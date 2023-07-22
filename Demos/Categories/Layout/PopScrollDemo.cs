using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class PopScrollDemo : Win
{
	public PopScrollDemo() : base(opt =>
	{
		opt.R = new R(-400, 50, 200, 500);
		//opt.R = new R(-400, 50, 350, 500);
	})
	{
		var nodeRoot = new NodeState("nodeRoot").D(D);
		var nodeTop = new NodeState("nodeTop").D(D);
		var nodeBottom = new NodeState("nodeBottom").D(D);
		var nodeScroll = new NodeState("nodeScroll").D(D);
		var nodeScrollInner = new NodeState("nodeScrollInner").D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.FillR(Consts.Root);

				using (r[nodeTop].M)
					r.FillR(Consts.Top);

				using (r[nodeScroll].Pop().ScrollXY().Dim(250, 100).M)
				{
					r.FillR(Consts.Scroll);
					using (r[nodeScrollInner].Dim(320, 200).M)
					{
						r.FillR(Consts.ScrollInner);
						r.DrawBmp(Consts.Bmp);
					}
				}

				using (r[nodeBottom].M)
					r.FillR(Consts.Bottom);
			}
		}).D(D);
	}


	private static class Consts
	{
		public static readonly BrushDef Root = new SolidBrushDef(Color.FromArgb(33, 33, 33));
		public static readonly BrushDef Top = new SolidBrushDef(Color.FromArgb(66, 100, 158));
		public static readonly BrushDef Bottom = new SolidBrushDef(Color.FromArgb(96, 156, 67));
		public static readonly BrushDef Scroll = new SolidBrushDef(Color.FromArgb(168, 40, 141));
		public static readonly BrushDef ScrollInner = new SolidBrushDef(Color.FromArgb(173, 72, 42));
		public static readonly Bitmap Bmp = new(@"C:\Dev_Explore\WinDX\_infos\images\pixelart\monkey-island.png");
	}
}