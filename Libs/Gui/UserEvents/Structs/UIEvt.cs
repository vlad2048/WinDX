using PowMaybe;
using PowRxVar;

namespace UserEvents.Structs;

public interface IUIEvt
{
	IObservable<IUserEvt> Evt { get; }
}

public sealed class UIEvt : IUIEvt
{
	public IObservable<IUserEvt> Evt { get; }

	internal UIEvt(IRoMayVar<nint> winHandle, IObservable<IUserEvt> evt)
	{
		WinHandle = winHandle;
		Evt = evt;
	}
}
