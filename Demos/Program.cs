using Demos.Categories.Base;
using Demos.Categories.Layout;
using Demos.Logic;
using Demos.Structs;
using PowRxVar;
using PowWinForms;
using Structs;
using SysWinLib;
using WinSpectorLib;

namespace Demos;

class Program
{
	static void Main()
	{
		//VarDbg.BreakpointOnDispAlloc(21);

		using (var d = new Disp())
		{
			var userPrefs = new UserPrefs().Track();
			Setup.InitConsole(userPrefs).D(d);
			var serD = new SerialDisp<Disp>().D(d);

			Action Wrap(Func<IDisposable> fun) => () =>
			{
				serD.Value = null;
				serD.Value = new Disp();
				fun().D(serD.Value);
			};

			WinSpector.RunInternal(
				new DemoNfo("SysWin", Wrap(SysWinDemo.Run)),
				new DemoNfo("UnbalancedCtrlHandling", Wrap(() => new UnbalancedCtrlHandlingDemoWin())),
				new DemoNfo("BalancedCtrlHandling", Wrap(() => new BalancedCtrlHandlingDemoWin())),
				new DemoNfo("PopNode", Wrap(() => new PopNodeDemoWin()))
			);
		}

		
		//using (SysWinDemo.Run()) App.Run();


		VarDbg.CheckForUndisposedDisps(true);
	}
}
