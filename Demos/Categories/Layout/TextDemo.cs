using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class TextDemo : Win
{
	public TextDemo() : base(opt => opt.R = new R(-300, 50, 150, 200))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeText1 = new NodeState().D(D);
		var nodeText2 = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.FillR(Consts.BrushRoot);
				using (r[nodeText1].DimFit().M)
				{
					r.FillR(Consts.BrushText1);
					r.DrawText("First item", Consts.Font, Consts.TextColor);
				}
				using (r[nodeText2].DimFit().M)
				{
					r.FillR(Consts.BrushText2);
					r.DrawText("Second item", Consts.Font, Consts.TextColor);
				}
			}
		}).D(D);
	}

	private static class Consts
	{
		public static readonly BrushDef BrushRoot = new SolidBrushDef(Color.FromArgb(21, 12, 69));
		public static readonly BrushDef BrushText1 = new SolidBrushDef(Color.FromArgb(194, 242, 196));
		public static readonly BrushDef BrushText2 = new SolidBrushDef(Color.FromArgb(236, 192, 237));
		public static readonly FontDef Font = FontDef.Default;
		public static readonly Color TextColor = Color.Black;
	}
}