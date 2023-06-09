namespace UserEvents.Structs;

public interface IUIEvt
{
	IntPtr WinHandle { get; }
	IObservable<IUserEvt> Evt { get; }
}

public class UIEvt : IUIEvt
{
	public IntPtr WinHandle { get; }
	public IObservable<IUserEvt> Evt { get; }

	internal UIEvt(IntPtr winHandle, IObservable<IUserEvt> evt)
	{
		WinHandle = winHandle;
		Evt = evt;
	}
}
