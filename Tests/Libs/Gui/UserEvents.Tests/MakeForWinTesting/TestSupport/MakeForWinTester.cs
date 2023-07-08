using System.Reactive.Subjects;
using Moq;
using PowBasics.Geom;
using PowRxVar;
using TestBase;
using UserEvents.Generators;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Windows;
using static UserEvents.Tests.MakeForWinTesting.TestSupport.GeomData;

namespace UserEvents.Tests.MakeForWinTesting.TestSupport;

enum Dst
{
	Main,
	Pop0,
	Pop1
}

sealed class MakeForWinTester : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISubject<IPacket> evtMain;
	private readonly ISubject<IPacket> evtPop0;
	private readonly ISubject<IPacket> evtPop1;
	private readonly ObservableChecker<IUserEvt> checker;

	public MakeForWinTester()
	{
		evtMain = new Subject<IPacket>().D(d);
		evtPop0 = new Subject<IPacket>().D(d);
		evtPop1 = new Subject<IPacket>().D(d);
		var popups = new RxTracker<IWin>().D(d);
		popups.Update(new []
		{
			MakeWin(rMain, evtMain),
			MakeWin(rPop0, evtPop0),
			MakeWin(rPop1, evtPop1),
		});

		var userEvt = UserEventGenerator.MakeForWin(popups).D(d);
		checker = new ObservableChecker<IUserEvt>(userEvt, "UserEvt").D(d);
	}


	public void Check(IUserEvt[] evts) => checker.Check(evts);


	private static Pt mousePos = Pt.Empty;

	public void Send_ActivateApp(Dst dst) => Send(dst, WM.ACTIVATEAPP, n(1));
	public void Send_InactivateApp(Dst dst) => Send(dst, WM.ACTIVATEAPP, n(0));
	public void Send_Activate(Dst dst, bool withMouseClick) => Send(dst, WM.ACTIVATE, n((int)(withMouseClick ? WindowActivateFlag.WA_CLICKACTIVE : WindowActivateFlag.WA_ACTIVE)));
	public void Send_Inactivate(Dst dst) => Send(dst, WM.ACTIVATE, n((int)WindowActivateFlag.WA_INACTIVE));
	public void Send_GotFocus(Dst dst) => Send(dst, WM.SETFOCUS);
	public void Send_LostFocus(Dst dst) => Send(dst, WM.KILLFOCUS);

	public void Send_BtnDown(Dst dst) => Send(dst, WM.LBUTTONDOWN);
	public void Send_BtnUp(Dst dst) => Send(dst, WM.LBUTTONUP);
	public void Send_Move(Dst dst, Pt pt)
	{
		mousePos = pt;
		var ofs = dst switch
		{
			Dst.Main => Pt.Empty,
			Dst.Pop0 => rPop0.Pos,
			Dst.Pop1 => rPop1.Pos,
		};
		var ptOfs = pt + ofs;
		Send(dst, WM.MOUSEMOVE, null, nxy(ptOfs.X, ptOfs.Y));
	}

	public void Send_Leave(Dst dst) => Send(dst, WM.MOUSELEAVE);

	private static nint n(int v) => new(v);
	private static nint n(uint v) => new(v);
	private static nint nxy(int x, int y)
	{
		var xf = (uint)x & 0xFFFF;
		var yf = (uint)y & 0xFFFF;
		return n(xf + yf << 16);
	}


	private void Send(Dst dst, WM wm, nint? wParam = null, nint? lParam = null) => GetEvt(dst).OnNext(MkMsg(wm, wParam ?? nint.Zero, lParam ?? nint.Zero));

	private ISubject<IPacket> GetEvt(Dst dst) => dst switch
	{
		Dst.Main => evtMain,
		Dst.Pop0 => evtPop0,
		Dst.Pop1 => evtPop1,
	};

	private static IPacket MkMsg(WM wm, nint wParam, nint lParam)
	{
		var msg = new WindowMessage(IntPtr.Zero, wm, wParam, lParam);
		return Packetizer.MakePacket(ref msg);
	}


	private static IWin MakeWin(R r, IObservable<IPacket> sysEvt)
	{
		var popupMock = new Mock<IWin>();
		popupMock
			.Setup(e => e.SysEvt)
			.Returns(sysEvt);
		popupMock
			.Setup(e => e.PopupOffset)
			.Returns(Pt.Empty);
		popupMock
			.Setup(e => e.ScreenPt)
			.Returns(Var.MakeConst(r.Pos));
		popupMock
			.Setup(e => e.ScreenR)
			.Returns(Var.MakeConst(r));
		return popupMock.Object;
	}
}