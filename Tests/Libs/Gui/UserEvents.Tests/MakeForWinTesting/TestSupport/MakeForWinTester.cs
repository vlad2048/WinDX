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
		var popups = Tracker.Make<IWin>().D(d);
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

	public void Send_ActivateApp(Dst dst)
	{
		LogEvt(dst, "ActivateApp");
		Send(dst, WM.ACTIVATEAPP, n(1));
	}

	public void Send_InactivateApp(Dst dst)
	{
		LogEvt(dst, "InactivateApp");
		Send(dst, WM.ACTIVATEAPP, n(0));
	}

	public void Send_Activate(Dst dst, bool withMouseClick)
	{
		LogEvt(dst, "Activate" + (withMouseClick ? " (mouse)" : ""));
		Send(dst, WM.ACTIVATE, n((int)(withMouseClick ? WindowActivateFlag.WA_CLICKACTIVE : WindowActivateFlag.WA_ACTIVE)));
	}

	public void Send_Inactivate(Dst dst)
	{
		LogEvt(dst, "Inactivate");
		Send(dst, WM.ACTIVATE, n((int)WindowActivateFlag.WA_INACTIVE));
	}

	public void Send_GotFocus(Dst dst)
	{
		LogEvt(dst, "Got focus");
		Send(dst, WM.SETFOCUS);
	}

	public void Send_LostFocus(Dst dst)
	{
		LogEvt(dst, "Lost focus");
		Send(dst, WM.KILLFOCUS);
	}

	public void Send_BtnDown(Dst dst)
	{
		LogEvt(dst, $"Left down ({mousePos})");
		Send(dst, WM.LBUTTONDOWN);
	}

	public void Send_BtnUp(Dst dst)
	{
		LogEvt(dst, $"Left up ({mousePos})");
		Send(dst, WM.LBUTTONUP);
	}

	public void Send_Move(Dst dst, Pt pt)
	{
		mousePos = pt;
		LogEvt(dst, $"Move {pt}");
		Send(dst, WM.MOUSEMOVE, null, nxy(pt.X, pt.Y));
	}

	public void Send_Leave(Dst dst)
	{
		LogEvt(dst, "Leave");
		Send(dst, WM.MOUSELEAVE);
	}
	
	private void LogEvt(Dst dst, string s) => L($"<- [{dst}] {s}");

	private static nint n(int v) => new(v);
	private static nint n(uint v) => new(v);
	private static nint nxy(int x, int y)
	{
		var xf = (uint)x & 0xFFFF;
		var yf = (uint)y & 0xFFFF;
		return n(xf + (yf << 16));
	}


	private void Send(Dst dst, WM wm, nint? wParam = null, nint? lParam = null)
	{
		var winMsg = new WindowMessage(IntPtr.Zero, wm, wParam ?? nint.Zero, lParam ?? nint.Zero);
		var msg = Packetizer.MakePacket(ref winMsg);
		GetEvt(dst).OnNext(msg);
	}

	private ISubject<IPacket> GetEvt(Dst dst) => dst switch
	{
		Dst.Main => evtMain,
		Dst.Pop0 => evtPop0,
		Dst.Pop1 => evtPop1,
	};


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