using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex;
using LayoutSystem.Utils;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.UserEvents;

public class UserEventsDemoWin : Win
{
	public UserEventsDemoWin() : base(opt => opt.R = new R(50, 70, 230, 120))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFill1 = new NodeState().D(D);
		var nodeFill2 = new NodeState().D(D);

		//nodeRoot.Evt.Evt.Subscribe(e => L($"[Ctrl-R] - {e}")).D(D);
		nodeFill1.Evt.Evt.Subscribe(e => L($"[Ctrl-1] - {e}")).D(D);
		nodeFill2.Evt.Evt.Subscribe(e => L($"[Ctrl-2] - {e}")).D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fil, Strats.Stack(Dir.Horz, Align.Start)))
			{
				r.Gfx.FillR(r.Gfx.R, Consts.BrushRoot);
				using (r.Flex(nodeFill1, Vec.Fil, Strats.Fill, Mg.Mk(10)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushFill1);
				}
				using (r.Flex(nodeFill2, Vec.Fil, Strats.Fill, Mg.Mk(10)))
				{
					r.Gfx.FillR(r.Gfx.R, Consts.BrushFill2);
				}
			}
		}).D(D);
	}

	private static class Consts
	{
		public static readonly BrushDef BrushRoot = new SolidBrushDef(Color.FromArgb(20, 57, 117));
		public static readonly BrushDef BrushFill1 = new SolidBrushDef(Color.FromArgb(22, 199, 43));
		public static readonly BrushDef BrushFill2 = new SolidBrushDef(Color.FromArgb(22, 158, 199));
	}
}