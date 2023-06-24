using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents;

public interface INodeStateUserEventsSupport
{
	Disp D { get; }
	IRoVar<R> R { get; }
	IObservable<IUserEvt> Evt { get; }
	void DispatchEvt(IUserEvt evt);
}