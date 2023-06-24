using System.Reactive.Linq;
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
			evtSrc.SwitchMayVar(e => e.WinHandle),
			evtSrc.Select(e => e.Evt).Switch()
		);
		return (evtSrc, evt, d);
	}

	private static IUIEvt MakeEmpty() => new UIEvt(
		VarMay.MakeConst<nint>(),
		Obs.Never<IUserEvt>()
	);
}