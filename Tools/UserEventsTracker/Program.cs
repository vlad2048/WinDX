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

			evt.EnableMouseTracking().D(win.D);

			var updateIdx = 0;
			PrettyPrintAggregator.Transform(evt).Subscribe(e =>
			{
				updateIdx = e is MouseMoveUpdatePrettyUserEvt ? updateIdx + 1 : 0;
				var updateIdxStr = updateIdx == 0 ? "" : $" (x{updateIdx})            ";
				if (e is MouseMoveUpdatePrettyUserEvt && updateIdx > 1) Console.CursorTop--;
				Console.WriteLine($"{e}{updateIdxStr}");
			}).D(win.D);

			App.Run();
		}

		Var.CheckForUnDisposedDisps(true);
	}

	private static IDisposable EnableMouseTracking(this IUIEvt evt)
	{
		var d = new Disp();
		var isTracking = Var.Make(
			false,
			val => val
				.Select(v => v switch
				{
					false => evt.Evt.OfType<MouseMoveUserEvt>().Select(_ => true),
					true => evt.Evt.OfType<MouseLeaveUserEvt>().Select(_ => false)
				})
				.Switch()
		).D(d);

		isTracking.Where(e => e).Subscribe(_ =>
		{
			var opt = new TrackMouseEventOptions
			{
				Flags = TrackMouseEventFlags.TME_LEAVE,
				Size = (uint) Marshal.SizeOf<TrackMouseEventOptions>(),
				TrackedHwnd = evt.WinHandle
			};
			Console.WriteLine("TrackMouseEvent");
			User32Methods.TrackMouseEvent(ref opt);
		}).D(d);

		isTracking.Subscribe(e => Console.WriteLine($"tracking <- {e}")).D(d);

		return d;
	}
}