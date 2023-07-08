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


public enum RedrawReason
{
	Resize,
	Ctrl,
	SpectorOverlay,
	SpectorRequestFullRedraw,
	UserCode,
}

public interface IInvalidator
{
	void Invalidate(RedrawReason reason);
}

public interface IMainWinUserEventsSupport : IWinUserEventsSupport
{
	IObservable<IUserEvt> Evt { get; }
	IInvalidator Invalidator { get; }
	IRoTracker<IWin> Wins { get; }
}