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
	IRoTracker<NodeZ> Nodes { get; }
	IRoTracker<ICtrl> Ctrls { get; }
	void SysInvalidate();
}

public interface IInvalidator
{
	void InvalidateLayout();
	void InvalidateRender();
}

public interface IMainWinUserEventsSupport : IWinUserEventsSupport
{
	IObservable<IUserEvt> Evt { get; }
	IInvalidator Invalidator { get; }
	IRoTracker<IWin> Wins { get; }
}