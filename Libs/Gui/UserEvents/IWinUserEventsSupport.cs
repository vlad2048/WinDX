using PowBasics.Geom;
using PowMaybe;
using UserEvents.Structs;

namespace UserEvents;

public interface IWinUserEventsSupport
{
	IUIEvt Evt { get; }
	Maybe<INodeStateUserEventsSupport> HitFun(Pt pt);
}