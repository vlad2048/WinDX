using System.Reactive.Linq;
using PowMaybe;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Generators;

public static partial class UserEventGenerator
{
	public static (IRwVar<IUIEvt>, IUIEvt, IDisposable) MakeWithSource()
	{
		var d = new Disp();
		var evtSrc = Var.Make(MakeEmpty()).D(d);
		var evt = new UIEvt(
			evtSrc.SwitchVar(e => e.WinHandle),
			evtSrc.Select(e => e.Evt).Switch()
		);
		return (evtSrc, evt, d);
	}

	private static IUIEvt MakeEmpty() => new UIEvt(
		Var.MakeConst(May.None<nint>()),
		Obs.Never<IUserEvt>()
	);
}