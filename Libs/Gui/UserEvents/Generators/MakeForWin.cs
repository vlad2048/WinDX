using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;
using UserEvents.Utils;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;
using MouseButtonDownUserEvt = UserEvents.Structs.MouseButtonDownUserEvt;

namespace UserEvents.Generators;

public static class UserEventGenerator
{
	private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(50);

	private sealed record EvtWin(IUserEvt Evt, IWin Win);
	private sealed record MouseNfo(Pt Pos, Pt WinPos)
	{
		public Pt ScreenPos => Pos + WinPos;
	}

	private static int cnt;

	public static (IObservable<IUserEvt>, IDisposable) MakeForWin(
		IRoTracker<IWin> popups,
		bool log = false
	)
	{
		var d = new Disp();
		var prefix = "[" + ((char)('A' + cnt++)) + "] ";
		string GetWinName(IWin win)
		{
			if (win is IMainWin) return "Main";
			return $"Pop[{popups.ItemsArr.IndexOf(win) - 1}]";
		}

		var sysEvtsItems = popups.Items.MergeMany(win =>
			MakeForSysWin(win.SysEvt)
				.Translate(() => win.PopupOffset)
				.Select(e => new EvtWin(e, win))
		);

		TrackMouseNfo(out var mouseNfo, sysEvtsItems).D(d);
		MakeForWinInternal(out var evt, sysEvtsItems, popups, mouseNfo, GetWinName, log, prefix).D(d);
		EnableMouseCapture(evt, popups, GetWinName, prefix).D(d);

		return (evt, d);
	}

	private static IDisposable TrackMouseNfo(
		out IRoVar<MouseNfo> mouseNfo,
		IObservable<EvtWin> sysEvtsItems
	)
	{
		var d = new Disp();
		var mouseNfoRw = Var.Make(new MouseNfo(Pt.Empty, Pt.Empty)).D(d);
		mouseNfo = mouseNfoRw.ToReadOnly();

		sysEvtsItems.Where(e => e.Evt is MouseMoveUserEvt).Subscribe(e =>
		{
			mouseNfoRw.V = new MouseNfo(
				((MouseMoveUserEvt)e.Evt).Pos,
				e.Win.ScreenPt.V
			);
		}).D(d);
		

		return d;
	}


	private static IDisposable EnableMouseCapture(
		IObservable<IUserEvt> evt,
		IRoTracker<IWin> popups,
		Func<IWin, string> getWinName,
		string prefix
	)
	{
		var d = new Disp();
		var serD = new SerialDisp<Disp>().D(d);

		evt.OfType<MouseButtonDownUserEvt>()
			.Subscribe(_ =>
			{
				var hwnd = popups.ItemsArr.First().Handle;
				var prevHwnd = User32Methods.SetCapture(hwnd);

				serD.Value = null;
				serD.Value = new Disp();

				evt.OfType<MouseButtonUpUserEvt>().Subscribe(_ =>
				{
					var res = User32Methods.ReleaseCapture();
					//L($"{prefix} - ReleaseCapture   (res:{res})");
				}).D(serD.Value);


				var prevWin = popups.ItemsArr.FirstOrDefault(e => e.Handle == prevHwnd);
				var prevWinName = (prevHwnd == nint.Zero) switch
				{
					true => "(none)",
					false => prevWin switch
					{
						null => "(other)",
						not null => getWinName(prevWin)
					}
				};

				//L($"{prefix} - MouseCapture   (prev:{prevWinName})");

			}).D(d);

		return d;
	}



	private static IDisposable MakeForWinInternal(
		out IObservable<IUserEvt> evt,
		IObservable<EvtWin> sysEvtsItems,
		IRoTracker<IWin> popups,
		IRoVar<MouseNfo> mouseNfo,
		Func<IWin, string> getWinName,
		bool log,
		string prefix
	)
	{
		var d = new Disp();

		if (log)
		{
			sysEvtsItems
				//.Where(e => e.e is MouseEnterUserEvt or MouseLeaveUserEvt)
				.Select(e => $"{prefix} {getWinName(e.Win)} {e.Evt}")
				.Log().D(d);
		}

		var sysEvts = sysEvtsItems.Select(e => e.Evt);
		var whenEvt = new Subject<IUserEvt>().D(d);
		void Send(IUserEvt evt) => whenEvt.OnNext(evt);
		bool IsMouseOver()
		{
			var mp = GetMousePos();
			/*L($"------------ Y = {mp.Y}");
			if (mp.Y == 100 + 100)
			{
				var abc = 123;
			}*/
			return popups.ItemsArr.Any(e => e.ScreenR.V.Contains(mp));
		}


		// State
		// =====
		var isActivateApp = Var.Make(false).D(d);
		var isActivate = Var.Make(false).D(d);
		var hasFocus = Var.Make(false).D(d);
		var isMouseOver = Var.Make(false).D(d);


		// State changes
		// =============
		var isLastActivateMouse = false;
		sysEvts.OfType<ActivateUserEvt>().Subscribe(e => isLastActivateMouse = e.WithMouseClick).D(d);

		isActivateApp.Skip(1).Subscribe(e => Send(e ? new ActivateAppUserEvt() : new InactivateAppUserEvt())).D(d);
		isActivate.Skip(1).Subscribe(e => Send(e ? new ActivateUserEvt(isLastActivateMouse) : new InactivateUserEvt())).D(d);
		isMouseOver.Skip(1).Subscribe(e => Send(e ? new MouseEnterUserEvt(mouseNfo.V.Pos) : new MouseLeaveUserEvt())).D(d);
		hasFocus.Skip(1).Subscribe(e => Send(e ? new GotFocusUserEvt() : new LostFocusUserEvt())).D(d);


		// Events (IUserEvtWindow)
		// ======
		sysEvts.OfType<ActivateAppUserEvt>().Subscribe(_ => isActivateApp.V = true).D(d);
		sysEvts.OfType<InactivateAppUserEvt>().Subscribe(_ => isActivateApp.V = false).D(d);

		sysEvts.OfType<ActivateUserEvt>().Subscribe(_ => isActivate.V = true).D(d);
		sysEvts.RunActionAfterIfNot<IUserEvt, InactivateUserEvt, ActivateUserEvt>(() => isActivate.V = false, delay).D(d);

		sysEvts.OfType<GotFocusUserEvt>().Subscribe(_ => hasFocus.V = true).D(d);
		sysEvts.RunActionAfterIfNot<IUserEvt, LostFocusUserEvt, GotFocusUserEvt>(() => hasFocus.V = false, delay).D(d);

		sysEvts.WhenMouseMove().Subscribe(_ => isMouseOver.V = true).D(d);
		sysEvts.WhenMouseLeave().Where(_ => !IsMouseOver()).Subscribe(_ => isMouseOver.V = false).D(d);
		//sysEvts.RunActionAfterIfNot<IUserEvt, MouseLeaveUserEvt, MouseMoveUserEvt>(() => isMouseOver.V = false, delay).D(d);


		// Events (IUserEvtMouse + IUserEvtKeyboard)
		// ======
		Obs.Merge<IUserEvt>(
				sysEvts.OfType<MouseButtonDownUserEvt>(),
				sysEvts.OfType<MouseButtonUpUserEvt>(),
				sysEvts.OfType<MouseMoveUserEvt>(),
				sysEvts.OfType<MouseWheelUserEvt>(),
				sysEvts.OfType<KeyDownUserEvt>(),
				sysEvts.OfType<KeyUpUserEvt>(),
				sysEvts.OfType<KeyCharUserEvt>()
			)
			.Subscribe(Send).D(d);


		evt = whenEvt.AsObservable();


		if (log)
		{
			evt
				.Log($"{Pad}{prefix}").D(d);
		}


		return d;
	}
	

	private static readonly string Pad = new(' ', 40);

	private static Pt GetMousePos()
	{
		User32Methods.GetCursorPos(out var pt);
		return pt.ToPt();
	}




	public static IObservable<IUserEvt> MakeForSysWin(IObservable<IPacket> msg) =>
		Obs.Merge<IUserEvt>(
// @formatter:off
			msg.WhenLBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Left		     )),
			msg.WhenLBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Left		     )),
			msg.WhenRBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Right		     )),
			msg.WhenRBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Right		     )),
			msg.WhenMBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Middle	     )),
			msg.WhenMBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Middle	     )),

			msg.WhenMOUSEWHEEL  ().Select(e => new MouseWheelUserEvt        (e.Point.ToPt(), -Math.Sign(e.WheelDelta))),

			msg.WhenMOUSEMOVE	().Select(e => new MouseMoveUserEvt			(e.Point.ToPt()						     )),
			msg.WhenMOUSELEAVE	().Select(_ => new MouseLeaveUserEvt		(									     )),

			msg.WhenKEYDOWN		().Select(e => new KeyDownUserEvt			(e.Key								     )),
			msg.WhenKEYUP		().Select(e => new KeyUpUserEvt				(e.Key								     )),
			msg.WhenCHAR		().Select(e => new KeyCharUserEvt			(e.Key								     )),

			msg.WhenSETFOCUS	().Select(_ => new GotFocusUserEvt			(									     )),
			msg.WhenKILLFOCUS	().Select(_ => new LostFocusUserEvt			(									     )),

			msg.WhenACTIVATE	().Where(e => e.Flag.IsActive()		).Select(e => new ActivateUserEvt		(e.Flag.IsClickActive()	)),
			msg.WhenACTIVATE	().Where(e => e.Flag.IsInactive()	).Select(_ => new InactivateUserEvt		(						)),

			msg.WhenACTIVATEAPP	().Where(e => e.IsActive			).Select(_ => new ActivateAppUserEvt	(						)),
			msg.WhenACTIVATEAPP	().Where(e => !e.IsActive			).Select(_ => new InactivateAppUserEvt	(						))
// @formatter:on
		);


	private static bool IsActive		(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_ACTIVE or WindowActivateFlag.WA_CLICKACTIVE;
	private static bool IsInactive		(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_INACTIVE;
	private static bool IsClickActive	(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_CLICKACTIVE;
// @formatter:on
}