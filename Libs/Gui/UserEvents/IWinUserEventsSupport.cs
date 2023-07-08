using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;
using WinAPI.Windows;

namespace UserEvents;

public interface IWinUserEventsSupport
{
	nint Handle { get; }
	IObservable<IPacket> SysEvt { get; }
	Pt PopupOffset { get; }
	IRoVar<Pt> ScreenPt { get; }
	IRoVar<R> ScreenR { get; }
	RxTracker<INode> Nodes { get; }
	//INodeStateUserEventsSupport[] HitFun(Pt pt);
	void Invalidate();
}

public interface IMainWinUserEventsSupport : IWinUserEventsSupport
{
	IObservable<IUserEvt> Evt { get; }
}