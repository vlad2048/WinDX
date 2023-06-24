using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Generators;

public static partial class UserEventGenerator
{
	public static IUIEvt MakeManual(IObservable<IUserEvt> obs) => new UIEvt(VarMay.MakeConst<nint>(), obs);
}