using DynamicData;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;
using WinAPI.Windows;

namespace UserEvents;

public interface IWinUserEventsSupport
{
	nint Handle { get; }
	IObservable<IPacket> SysEvt { get; }
	IObservable<IUserEvt> SysWinEvt { get; }
	Pt PopupOffset { get; }
	IRoVar<Pt> ScreenPt { get; }
	IRoVar<R> ScreenR { get; }
	IObservable<IChangeSet<INode>> Nodes { get; }
	INodeStateUserEventsSupport[] HitFun(Pt pt);
	void Invalidate();
}

public interface IMainWinUserEventsSupport : IWinUserEventsSupport
{
	IObservable<IUserEvt> Evt { get; }
}