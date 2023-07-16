using PowBasics.Geom;
using PowRxVar;
using System.Reactive;
using UserEvents.Structs;

namespace UserEvents;

public interface INodeStateUserEventsSupport
{
	Disp D { get; }
	IRoVar<R> R { get; }
	IRoVar<Pt> WinPos { get; }
	IObservable<IUserEvt> Evt { get; }
	void DispatchEvt(IUserEvt evt);
	IObservable<Unit> WhenInvalidateRequired { get; }
}

public interface ICtrlUserEventsSupport
{
	IObservable<Unit> WhenChanged { get; }
}