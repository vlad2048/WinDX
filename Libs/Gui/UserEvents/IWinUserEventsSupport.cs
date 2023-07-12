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
	IRoVar<Sz> ClientSz { get; }
	IRoTracker<NodeZ> Nodes { get; }
	IRoTracker<ICtrl> Ctrls { get; }
	void SysInvalidate();
}


public enum RedrawReason
{
	Resize,
	Ctrl,
	Node,
	SpectorOverlay,
	SpectorRequestFullRedraw,
	UserCode,
	RendererSwitched,
}

public static class RedrawReasonExt
{
	public static bool IsLayoutRequired(this RedrawReason e) => e switch
	{
		RedrawReason.Resize => true,
		RedrawReason.Ctrl => true,
		RedrawReason.Node => true,
		RedrawReason.SpectorOverlay => false,
		RedrawReason.SpectorRequestFullRedraw => true,
		RedrawReason.UserCode => true,
		RedrawReason.RendererSwitched => false,
	};
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
	void SetSize(Sz sz);
}