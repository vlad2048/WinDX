using System.Reactive.Linq;
using System.Runtime.InteropServices;
using PowRxVar;
using SysWinInterfaces;
using SysWinLib;
using SysWinLib.Structs;
using UserEvents.Structs;
using UserEvents.Utils;
using WinAPI.User32;

namespace UserEventsTracker;

static class Program
{
	static void Main()
	{
		using (var win = new SysWin(opt => opt.CreateWindowParams = new CreateWindowParams
		       {
			       X = -300,
			       Y = 20,
			       Width = 256,
			       Height = 256,
		       }))
		{
			win.Init();

			//User32Methods.SetCapture(win.Handle);

			var evt = UserEvtGenerator.MakeForWin(win);

			PrettyPrintAggregator.Transform(evt).Subscribe(e =>
			{
				if (e is MouseMoveUpdatePrettyUserEvt { IsSubsequent: true }) Console.CursorTop--;
				Console.WriteLine($"{e}");
			}).D(win.D);

			App.Run();
		}

		VarDbg.CheckForUndisposedDisps(true);
	}
}