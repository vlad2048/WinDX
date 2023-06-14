using PowRxVar;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using PowBasics.Geom;
using SysWinInterfaces;
using SysWinLib.Utils;
using WinAPI.User32;
using WinAPI.Windows;

namespace SysWinLib;


/// <summary>
/// Low level Win32 window wrapper
/// </summary>
public sealed class SysWin : ISysWin
{
	// static
	private static bool isFirst = true;

	// IDisposable
	private bool isDisposed;
	public Disp D { get; } = new();
	public void Dispose()
	{
		if (!hasDestroyBeenSent)
		{
			Destroy();
			return;
		}
		if (isDisposed) return;
		D.Dispose();
		isDisposed = true;
	}
	private void Destroy()
	{
		if (hasDestroyBeenSent) return;
		User32Methods.DestroyWindow(Handle);
		hasDestroyBeenSent = true;
	}

	// private
	private readonly SysWinOpt opt;
	private readonly bool isMainWindow;
	private readonly ISubject<IPacket> whenMsg;
	private GCHandle gcHandle;
	private bool hasDestroyBeenSent;

	// public
	public IntPtr Handle { get; private set; }
	public IObservable<IPacket> WhenMsg => whenMsg.AsObservable();
	public IRoVar<bool> IsInit { get; }
	public IRoVar<R> ClientR { get; }
	public IRoVar<Pt> ScreenPt { get; }

	public SysWin(
		Action<SysWinOpt>? optFun = null
	)
	{
		opt = SysWinOpt.Make(optFun);
		isMainWindow = isFirst;
		isFirst = false;
		whenMsg = new Subject<IPacket>().D(D);
		ClientR = Var.Make(
			R.Empty,
			Obs.Merge(
				// This observation was made with our custom NonClientArea
				// =======================================================
				// this is just so other code doesn't get ClientR=Empty in WM_CREATE.
				// RType.Client				: not quite right 240x217 (instead of 256x256)
				// RType.Win				: correct 256x256
				// RType.WinWithGripAreas	: correct 256x256
				WhenMsg.WhenCREATE().Select(_ => this.GetR(RType.WinWithGripAreas).WithZeroPos()),
				WhenMsg.WhenWINDOWPOSCHANGED().Select(e => new R(Pt.Empty, new Sz(e.Position.Width, e.Position.Height)))
			)
		).D(D); // flagging it as handled (not calling the default windowproc) means we won't receive WM_SIZE & WM_MOVE messages
		ScreenPt = Var.Make(
			Pt.Empty,
			WhenMsg.WhenWINDOWPOSCHANGED().Select(e => new Pt(e.Position.X, e.Position.Y))
		).D(D);
		IsInit = Var.Make(false, WhenMsg.WhenCREATE().Select(_ => true)).D(D);
		gcHandle = GCHandle.Alloc(this);
		

		Disposable.Create(() =>
		{
			Destroy();
			gcHandle.Free();
		}).D(D);

		this.SetupCustomNCAreaIFN(opt);
		this.GenerateMouseLeaveMessagesIFN(opt);
	}

	public void Init()
	{
		try
		{
			User32Methods.CreateWindowEx(
				opt.CreateWindowParams.ExStyles,
				opt.WinClass,
				opt.CreateWindowParams.Name,
				opt.CreateWindowParams.Styles,
				opt.CreateWindowParams.X,
				opt.CreateWindowParams.Y,
				opt.CreateWindowParams.Width,
				opt.CreateWindowParams.Height,
				opt.CreateWindowParams.Parent,
				opt.CreateWindowParams.Menu,
				RegisterClassUtils.InstanceHandle,
				GCHandle.ToIntPtr(gcHandle)
			);
		}
		finally
		{
			Check(Handle, "CreateWindowEx");
		}

		//Disposable.Create(Destroy).D(D);
	}

	public static IntPtr WndProc(IntPtr hWnd, WM id, IntPtr wParam, IntPtr lParam)
	{
		//L($"  {id}");
	    var msg = new WindowMessage
	    {
		    Id = id,
		    WParam = wParam,
		    LParam = lParam,
		    Result = IntPtr.Zero,
		    Hwnd = hWnd
	    };
	    IntPtr DefProc() => User32Methods.DefWindowProc(msg.Hwnd, msg.Id, msg.WParam, msg.LParam);

		// 1. Extract the SysWin instance from WM_NCCREATE message and write it in the window extra params for subsequent messages
	    var win = ExtractWinFromMsg(msg);
	    if (win == null) return DefProc();

		// 2. Watch for WM_NCDESTROY. When it arrives, destroy the window and exit the message loop if this is the main window
		if (win.HandleDestroy(msg))
			return IntPtr.Zero;

		// 3. Broadcast the message to WhenMsg
		win.whenMsg.OnNext(Packetizer.MakePacket(ref msg));

		// 4. If no receiver to the broadcast flagged the message as handled, send it to the default window procedure
		return msg.Handled switch
		{
			true => msg.Result,
			false => DefProc()
		};
    }


	private bool HandleDestroy(WindowMessage msg)
	{
		if (msg.Id != WM.NCDESTROY) return false;
		hasDestroyBeenSent = true;
		if (isMainWindow && App.IsWinDXAppRunning)
		{
			User32Methods.PostQuitMessage(0);
		}

		Dispose();

		return true;
	}


	private static unsafe SysWin? ExtractWinFromMsg(WindowMessage msg)
	{
		SysWin win;
		if (msg.Id == WM.NCCREATE)
		{
			var createStruct = *(CreateStruct*) msg.LParam.ToPointer();
			var winPtr = createStruct.CreateParams;
			User32Helpers.SetWindowLongPtr(msg.Hwnd, WindowLongFlags.GWLP_USERDATA, winPtr);
			win = winPtr.GetWinFromPtr<SysWin>();
			win.Handle = msg.Hwnd;
		}
		else
		{
			var winPtr = User32Helpers.GetWindowLongPtr(msg.Hwnd, WindowLongFlags.GWLP_USERDATA);
			if (winPtr == IntPtr.Zero)
				return null;
			win = winPtr.GetWinFromPtr<SysWin>();
		}
		return win;
	}
}


file static class SysWinPrivateExt
{
	public static T GetWinFromPtr<T>(this IntPtr ptr) where T : class
	{
		var handle = GCHandle.FromIntPtr(ptr);
		var resObj = handle.Target ?? throw new NullReferenceException("handle shouldn't be null");
		var res = resObj as T ?? throw new NullReferenceException("handle is of the wrong type");
		return res;
	}
}