using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.UserEvents;

sealed class UserEventsDemo : Win
{
	public UserEventsDemo() : base(opt => opt.R = new R(-250, 100, 230, 120))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeFill1 = new NodeState().D(D);
		var nodeFill2 = new NodeState().D(D);

		//nodeRoot.Evt.Evt.Subscribe(e => L($"[Ctrl-R] - {e}")).D(D);
		nodeFill1.Evt.Subscribe(e => L($"[Ctrl-1] - {e}")).D(D);
		nodeFill2.Evt.Subscribe(e => L($"[Ctrl-2] - {e}")).D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Horz).M)
			{
				r.FillR(Consts.BrushRoot);
				using (r[nodeFill1].Marg(10).M)
				{
					r.FillR(Consts.BrushFill1);
				}
				using (r[nodeFill2].Marg(10).M)
				{
					r.FillR(Consts.BrushFill2);
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