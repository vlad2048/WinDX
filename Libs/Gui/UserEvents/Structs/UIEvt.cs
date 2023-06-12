using PowMaybe;
using PowRxVar;

namespace UserEvents.Structs;

public interface IUIEvt
{
	IRoVar<Maybe<nint>> WinHandle { get; }
	IObservable<IUserEvt> Evt { get; }
}

public sealed class UIEvt : IUIEvt
{
	public IRoVar<Maybe<nint>> WinHandle { get; }
	public IObservable<IUserEvt> Evt { get; }

	internal UIEvt(IRoVar<Maybe<nint>> winHandle, IObservable<IUserEvt> evt)
	{
		WinHandle = winHandle;
		Evt = evt;
	}
}
