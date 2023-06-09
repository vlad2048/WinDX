using System.Reactive.Linq;
using SysWinInterfaces;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

namespace UserEvents.Utils;

public static class UserEvtGenerator
{
// @formatter:off
	public static IUIEvt MakeForWin(ISysWinUserEventsSupport win) => new UIEvt(
		win.Handle,

		Observable.Merge<IUserEvt>(
			win.WhenMsg.WhenLBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Left		)),
			win.WhenMsg.WhenLBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Left		)),
			win.WhenMsg.WhenRBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Right		)),
			win.WhenMsg.WhenRBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Right		)),
			win.WhenMsg.WhenMBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Middle	)),
			win.WhenMsg.WhenMBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Middle	)),

			win.WhenMsg.WhenMOUSEMOVE	().Select(e => new MouseMoveUserEvt			(e.Point.ToPt()						)),
			win.WhenMsg.WhenMOUSELEAVE	().Select(e => new MouseLeaveUserEvt		(									)),

			win.WhenMsg.WhenKEYDOWN		().Select(e => new KeyDownUserEvt			(e.Key								)),
			win.WhenMsg.WhenKEYUP		().Select(e => new KeyUpUserEvt				(e.Key								)),
			win.WhenMsg.WhenCHAR		().Select(e => new KeyCharUserEvt			(e.Key								)),

			win.WhenMsg.WhenSETFOCUS	().Select(_ => new GotFocusUserEvt			(									)),
			win.WhenMsg.WhenKILLFOCUS	().Select(_ => new LostFocusUserEvt			(									)),

			win.WhenMsg.WhenACTIVATE	()
				.Where(e => e.Flag is WindowActivateFlag.WA_ACTIVE or WindowActivateFlag.WA_CLICKACTIVE)
				.Select(e => new ActivateUserEvt(e.Flag == WindowActivateFlag.WA_CLICKACTIVE)),
			win.WhenMsg.WhenACTIVATE	()
				.Where(e => e.Flag is WindowActivateFlag.WA_INACTIVE)
				.Select(_ => new InactivateUserEvt()),

			win.WhenMsg.WhenACTIVATEAPP	()
				.Where(e => e.IsActive)
				.Select(_ => new ActivateAppUserEvt()),
			win.WhenMsg.WhenACTIVATEAPP	()
				.Where(e => !e.IsActive)
				.Select(_ => new InactivateAppUserEvt())
		)
	);
// @formatter:on
}