using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;
using UserEvents.Utils;

namespace Demos.Categories.UserEvents;

sealed class WinEventsDemo : Win
{
	private static char sPref = 'A';
	public WinEventsDemo() : base(opt => opt.R = new R(-350, 100, 200, 300))
	{
		var pref = sPref++;
		var nodeRoot = new NodeState("root").D(D);
		var nodeTop = new NodeState("top").D(D);
		var nodePop = new NodeState("pop").D(D);
		var childCtrl = new ChildCtrl(pref).D(D);
		var nodeBottom = new NodeState("bottom").D(D);

		nodePop.Evt.Log($"nodeOuter({pref}): ").D(D);

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

					using (r[childCtrl]) { }
				}

				using (r[nodeBottom].M)
					r.Gfx.FillR(Consts.BrushBottom);
			}
		}).D(D);
	}

	private sealed class ChildCtrl : Ctrl
	{
		public ChildCtrl(char pref)
		{
			var nodeTopInner = new NodeState("topInner").D(D);
			var nodePopInner = new NodeState("popInner").D(D);
			nodePopInner.Evt.Log($"nodeInner({pref}): ").D(D);

			WhenRender.Subscribe(r =>
			{
				using (r[nodeTopInner].DimFilFix(60).Marg(50, 0, 0, 0).M)
					r.Gfx.FillR(Consts.BrushTopInner);
				using (r[nodePopInner].Dim(300, 50).Pop().M)
					r.Gfx.FillR(Consts.BrushPopInner);
			}).D(D);
		}
	}

	private static class Consts
	{
		public static readonly BrushDef BrushRoot = new SolidBrushDef(Color.FromArgb(20, 57, 117));
		public static readonly BrushDef BrushTop = new SolidBrushDef(Color.FromArgb(51, 102, 130));
		public static readonly BrushDef BrushPop = new SolidBrushDef(Color.FromArgb(194, 48, 189));
		public static readonly BrushDef BrushBottom = new SolidBrushDef(Color.FromArgb(232, 132, 232));

		public static readonly BrushDef BrushTopInner = new SolidBrushDef(Color.FromArgb(47, 156, 90));
		public static readonly BrushDef BrushPopInner = new SolidBrushDef(Color.FromArgb(208, 219, 79));
	}
}
