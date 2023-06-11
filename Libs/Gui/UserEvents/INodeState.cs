using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents;

public interface INodeState
{
	R R { get; }
	IRwVar<IUIEvt> EvtSrc { get; }
}