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
		using var d = new Disp();
		var win = new SimpleWin().D(d);
		App.Run();
	}
}

class SimpleWin : Win
{
	private static readonly BrushDef backBrush = new SolidBrushDef(Color.DarkOrange);

	public SimpleWin() : base(opt => opt.R = new R(-300, 64, 200, 300))
	{
		var ctrl = new SimpleCtrl().D(D);

		WhenRender.Subscribe(r =>
		{
			r.Gfx.FillR(r.Gfx.R, backBrush);
			using (r.Ctrl(ctrl))
			{

			}
		}).D(D);
	}
}

class SimpleCtrl : Ctrl
{
	private static readonly BrushDef backBrush = new SolidBrushDef(Color.DodgerBlue);

	public SimpleCtrl()
	{
		var nodeTop = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r.Flex(nodeTop, Vec.Fix(120, 180), Strats.Fill))
			{
				r.Gfx.FillR(r.Gfx.R, backBrush);
			}
		}).D(D);
	}
}