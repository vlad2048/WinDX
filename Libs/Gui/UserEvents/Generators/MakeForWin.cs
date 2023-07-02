using System.Reactive.Linq;
using SysWinInterfaces;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

namespace UserEvents.Generators;

public static partial class UserEventGenerator
{
// @formatter:off
	public static IObservable<IUserEvt> MakeForWin(ISysWinUserEventsSupport win)
	{
		var isOver = false;

		return Obs.Merge<IUserEvt>(
			win.WhenMsg.WhenLBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Left		     )),
			win.WhenMsg.WhenLBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Left		     )),
			win.WhenMsg.WhenRBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Right		     )),
			win.WhenMsg.WhenRBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Right		     )),
			win.WhenMsg.WhenMBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Middle	     )),
			win.WhenMsg.WhenMBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Middle	     )),

			win.WhenMsg.WhenMOUSEWHEEL  ().Select(e => new MouseWheelUserEvt        (e.Point.ToPt(), -Math.Sign(e.WheelDelta))),

			win.WhenMsg.WhenMOUSEMOVE	()
				.Where(_ => !isOver)
				.Do(_ => isOver = true)
										  .Select(e => new MouseEnterUserEvt		(e.Point.ToPt()						     )),

			win.WhenMsg.WhenMOUSEMOVE	().Select(e => new MouseMoveUserEvt			(e.Point.ToPt()						     )),
			win.WhenMsg.WhenMOUSELEAVE	()
				.Do(_ => isOver = false)
										  .Select(_ => new MouseLeaveUserEvt		(									     )),

			win.WhenMsg.WhenKEYDOWN		().Select(e => new KeyDownUserEvt			(e.Key								     )),
			win.WhenMsg.WhenKEYUP		().Select(e => new KeyUpUserEvt				(e.Key								     )),
			win.WhenMsg.WhenCHAR		().Select(e => new KeyCharUserEvt			(e.Key								     )),

			win.WhenMsg.WhenSETFOCUS	().Select(_ => new GotFocusUserEvt			(									     )),
			win.WhenMsg.WhenKILLFOCUS	().Select(_ => new LostFocusUserEvt			(									     )),

			win.WhenMsg.WhenACTIVATE	().Where(e => e.Flag.IsActive()		).Select(e => new ActivateUserEvt		(e.Flag.IsClickActive()	)),
			win.WhenMsg.WhenACTIVATE	().Where(e => e.Flag.IsInactive()	).Select(_ => new InactivateUserEvt		(						)),

			win.WhenMsg.WhenACTIVATEAPP	().Where(e => e.IsActive			).Select(_ => new ActivateAppUserEvt	(						)),
			win.WhenMsg.WhenACTIVATEAPP	().Where(e => !e.IsActive			).Select(_ => new InactivateAppUserEvt	(						))
		);
	}


	private static bool IsActive		(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_ACTIVE or WindowActivateFlag.WA_CLICKACTIVE;
	private static bool IsInactive		(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_INACTIVE;
	private static bool IsClickActive	(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_CLICKACTIVE;
// @formatter:on
}