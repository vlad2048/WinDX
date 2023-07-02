using PowBasics.Geom;
using PowMaybe;
using UserEvents.Structs;

namespace UserEvents;

public interface IWinUserEventsSupport
{
	IObservable<IUserEvt> Evt { get; }
	Maybe<INodeStateUserEventsSupport> HitFun(Pt pt);
	void Invalidate();
}