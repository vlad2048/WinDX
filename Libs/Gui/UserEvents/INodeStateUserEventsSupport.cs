using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents;

public interface INodeStateUserEventsSupport
{
	IRoVar<R> R { get; }
	// IRwVar<IUIEvt> EvtSrc { get; }
	void DispatchEvt(IUserEvt evt);
}