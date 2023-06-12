using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;
using SysWinLib;

namespace SimpleDemo;

static class Program
{
	static void Main()
	{
		using (var d = new Disp())
		{
			var _ = new SimpleWin().D(d);
			App.Run();
		}
		VarDbg.CheckForUndisposedDisps(true);
	}
}

class SimpleWin : Win
{
	private static readonly BrushDef backBrush = new SolidBrushDef(Color.DarkOrange);

	public SimpleWin() : base(opt => opt.R = new R(-300, 64, 200, 300))
	{
		var nodeRoot = new NodeState().D(D);
		var ctrl = new SimpleCtrl().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fix(200, 300), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, backBrush);
				using (r.Ctrl(ctrl))
				{

				}
			}
		}).D(D);
	}
}

class SimpleCtrl : Ctrl
{
	private static readonly BrushDef backBrush = new SolidBrushDef(Color.DodgerBlue);

	public SimpleCtrl()
	{
		var nodeRoot = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeRoot, Vec.Fix(120, 180), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, backBrush);
			}
		}).D(D);
	}
}