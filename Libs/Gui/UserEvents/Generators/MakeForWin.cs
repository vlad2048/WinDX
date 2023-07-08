using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;
using DynamicData;
using PowBasics.Geom;
using PowRxVar;
using SysWinInterfaces;
using UserEvents.Structs;
using UserEvents.Utils;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

namespace UserEvents.Generators;

public static class UserEventGenerator
{
	private static readonly TimeSpan delay = TimeSpan.FromMilliseconds(50);

	
	public static (IObservable<IUserEvt>, IDisposable) MakeForWin(RxTracker<IWin> popups, bool log = false)
	{
		var d = new Disp();

		var sysEvtsItems = popups.Items.MergeMany(win => MakeForSysWin(win.SysEvt).Translate(() => win.PopupOffset).Select(e => (win, e)));
		var sysEvts = sysEvtsItems.Select(e => e.e);
		var wins = popups.Items.AsObservableList().D(d);
		//var sysEvts = MakeForSysWin(main.SysEvt);


		if (log)
		{
			var pops = popups.Items.AsObservableList().D(d);

			string GetWinName(IWin win)
			{
				if (win is IMainWin) return "Main";
				return $"Pop[{pops.Items.IndexOf(win) - 1}]";
			}

			sysEvtsItems
				//.Where(e => e.e is MouseEnterUserEvt or MouseLeaveUserEvt)
				.Select(e => $"{GetWinName(e.Item1)} {e.e}")
				.Log().D(d);
		}



		var whenEvt = new Subject<IUserEvt>().D(d);



		void Send(IUserEvt evt) => whenEvt.OnNext(evt);

		
		// State
		// =====
		var isActivateApp = Var.Make(false).D(d);
		var isActivate = Var.Make(false).D(d);
		var hasFocus = Var.Make(false).D(d);
		var isMouseOver = Var.Make(false).D(d);


		// State changes
		// =============
		var isLastActivateMouse = false;
		var mousePos = Pt.Empty;
		//var mousePosWinPos = Pt.Empty;
		sysEvts.OfType<ActivateUserEvt>().Subscribe(e => isLastActivateMouse = e.WithMouseClick).D(d);
		//Obs.Merge(sysEvts.WhenMouseMove(), sysEvts.WhenMouseEnter()).Subscribe(e => mousePos = e).D(d);
		//sysEvts.WhenMouseMove().Subscribe(e => mousePos = e).D(d);
		sysEvtsItems.Where(t => t.e is MouseMoveUserEvt).Subscribe(t =>
		{
			mousePos = ((MouseMoveUserEvt)t.e).Pos;
			//mousePosWinPos = t.Item1.ScreenPt.V;
		}).D(d);
		/*bool IsMouseOverWin()
		{
			User32Methods.GetCursorPos(out var pt);
			var mp = pt.ToPt();
			//var mp = mousePosWinPos + mousePos;
			return wins.Items.Any(e => e.ScreenR.V.Contains(mp));
		}*/

		isActivateApp.Skip(1).Subscribe(e => Send(e ? new ActivateAppUserEvt() : new InactivateAppUserEvt())).D(d);
		isActivate.Skip(1).Subscribe(e => Send(e ? new ActivateUserEvt(isLastActivateMouse) : new InactivateUserEvt())).D(d);
		isMouseOver.Skip(1).Subscribe(e => Send(e ? new MouseEnterUserEvt(mousePos) : new MouseLeaveUserEvt())).D(d);
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
		//sysEvts.WhenMouseLeave().Where(_ => !IsMouseOverWin()).Subscribe(_ => isMouseOver.V = false).D(d);
		sysEvts.RunActionAfterIfNot<IUserEvt, MouseLeaveUserEvt, MouseMoveUserEvt>(() => isMouseOver.V = false, delay).D(d);


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


		var evt = whenEvt.AsObservable();


		if (log)
		{
			evt
				//.Where(e => e is MouseEnterUserEvt or MouseLeaveUserEvt)
				//.Select(e => $"{e}  (IsMouseOverWin: {IsMouseOverWin()})")
				.Log(Pad).D(d);
		}


		return (
			evt,
			d
		);
	}
	

	private static IObservable<T> DoIf<T>(this IObservable<T> obs, bool cond, Action<T> action) => cond switch
	{
		false => obs,
		true => obs.Do(action)
	};
	private static readonly string Pad = new(' ', 40);
	private static void L(string s) => Console.WriteLine(s);




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