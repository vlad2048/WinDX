using Demos.Categories.Base;
using Demos.Categories.Layout;
using Demos.Categories.Ownership;
using Demos.Categories.UserEvents;
using Demos.Logic;
using Demos.Structs;
using PowRxVar;
using PowWinForms;
using SysWinLib;
using WinSpectorLib;
using WinSpectorLib.Structs;
// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162

namespace Demos;

class Program
{
	private const bool RunSingleDemo = false;

	static void Main()
	{
		//VarDbg.BreakpointOnDispAlloc(21);

		Thread.CurrentThread.Name = "Main-Thread";

		if (RunSingleDemo)
		{
			using (new UserEventsDemoWin()) App.Run();
		}
		else
		{
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

					new DemoNfo("UnbalancedCtrlHandling", Wrap(() => new UnbalancedCtrlHandlingDemo())),
					new DemoNfo("BalancedCtrlHandling", Wrap(() => new BalancedCtrlHandlingDemo())),
					new DemoNfo("PopNode", Wrap(() => new PopNodeDemo())),
					new DemoNfo("PopNodeComplex", Wrap(() => new PopNodeComplexDemo())),
					new DemoNfo("Text", Wrap(() => new TextDemo())),

					new DemoNfo("UserEvents", Wrap(() => new UserEventsDemoWin())),

					new DemoNfo("DetachCtrl", Wrap(() => new DetachCtrlDemo()))
				);
			}
		}


		VarDbg.CheckForUndisposedDisps(true);
	}
}
