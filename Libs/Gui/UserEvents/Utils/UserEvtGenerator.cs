using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using SysWinInterfaces;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

namespace UserEvents.Utils;

public static class UserEvtGenerator
{
// @formatter:off
	public static IUIEvt MakeForWin(ISysWinUserEventsSupport win)
	{
		var isOver = false;
		return new UIEvt(
			Var.MakeConst(May.Some(win.Handle)),

			Obs.Merge<IUserEvt>(
				win.WhenMsg.WhenLBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Left		)),
				win.WhenMsg.WhenLBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Left		)),
				win.WhenMsg.WhenRBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Right		)),
				win.WhenMsg.WhenRBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Right		)),
				win.WhenMsg.WhenMBUTTONDOWN	().Select(e => new MouseButtonDownUserEvt	(e.Point.ToPt(), MouseBtn.Middle	)),
				win.WhenMsg.WhenMBUTTONUP	().Select(e => new MouseButtonUpUserEvt		(e.Point.ToPt(), MouseBtn.Middle	)),

				win.WhenMsg.WhenMOUSEMOVE	()
					.Where(_ => !isOver)
					.Do(_ => isOver = true)
											  .Select(e => new MouseEnterUserEvt		(e.Point.ToPt()						)),

				win.WhenMsg.WhenMOUSEMOVE	().Select(e => new MouseMoveUserEvt			(e.Point.ToPt()						)),
				win.WhenMsg.WhenMOUSELEAVE	()
					.Do(_ => isOver = false)
												.Select(_ => new MouseLeaveUserEvt		(									)),

				win.WhenMsg.WhenKEYDOWN		().Select(e => new KeyDownUserEvt			(e.Key								)),
				win.WhenMsg.WhenKEYUP		().Select(e => new KeyUpUserEvt				(e.Key								)),
				win.WhenMsg.WhenCHAR		().Select(e => new KeyCharUserEvt			(e.Key								)),

				win.WhenMsg.WhenSETFOCUS	().Select(_ => new GotFocusUserEvt			(									)),
				win.WhenMsg.WhenKILLFOCUS	().Select(_ => new LostFocusUserEvt			(									)),

				win.WhenMsg.WhenACTIVATE	().Where(e => e.Flag.IsActive()		).Select(e => new ActivateUserEvt		(e.Flag.IsClickActive()	)),
				win.WhenMsg.WhenACTIVATE	().Where(e => e.Flag.IsInactive()	).Select(_ => new InactivateUserEvt		(						)),

				win.WhenMsg.WhenACTIVATEAPP	().Where(e => e.IsActive			).Select(_ => new ActivateAppUserEvt	(						)),
				win.WhenMsg.WhenACTIVATEAPP	().Where(e => !e.IsActive			).Select(_ => new InactivateAppUserEvt	(						))
			)
		);
	}
	// @formatter:on



	public static (IRwVar<IUIEvt>, IUIEvt, IDisposable) MakeWithSource()
	{
		var d = new Disp();
		var evtSrc = Var.Make(MakeEmpty()).D(d);
		var evt = new UIEvt(
			evtSrc.SwitchVar(e => e.WinHandle),
			evtSrc.Select(e => e.Evt).Switch()
		);
		return (evtSrc, evt, d);
	}

	
	public static IDisposable GenerateForNodes<N>(
		IUIEvt winEvt,
		IObservable<IChangeSet<N>> nodes,
		Func<Pt, IEnumerable<N>> hitFun
	)
		where N : INodeState
	{
		var d = new Disp();
		nodes
			.Transform(node =>
			{
				var nodeD = new Disp();

				var isOver = Var.Make(false).D(nodeD);
				ISubject<IUserEvt> whenLeave = new Subject<IUserEvt>().D(nodeD);
				

				// ***********************************
				// * Synthesize the MouseLeave event *
				// ***********************************
				isOver
					.DistinctUntilChanged()
					.Select(_isOver => _isOver switch
					{
						false => winEvt.WhenMouseMove().Where(pt => hitFun(pt).Contains(node)).Select(_ => true),
						true =>
							Observable.Merge(
									winEvt.WhenMouseLeave().ToUnit(),
									winEvt.WhenMouseMove().Where(pt => !hitFun(pt).Contains(node)).ToUnit()
								)
								.Select(_ => false)
					})
					.Switch()
					.DistinctUntilChanged()
					.Subscribe(isOverNext =>
					{
						if (!isOverNext)
							whenLeave.OnNext(new MouseLeaveUserEvt());
						isOver.V = isOverNext;
					}).D(nodeD);

				
				// ***********************
				// * Generate the events *
				// ***********************
				node.EvtSrc.V = winEvt
					.Map(evt =>
						{
							var evtNoLeaveAndNoMove = evt.Where(e => e is not MouseLeaveUserEvt && e is not MouseMoveUserEvt);
							var mouseMoveInside = evt.Where(e => e is MouseMoveUserEvt mv && hitFun(mv.Pos).Contains(node));

							return Observable.Merge(
								
									//evtNoLeaveAndNoMove
									//	.Where(e => e is IUserEvtKeyboard)
									//	.Where(_ => focusFun(area).Foc.V.HasFocus()),
									whenLeave
										.AsObservable(),
									evtNoLeaveAndNoMove
										.Where(e => e is IUserEvtMouse)
										.Where(_ => isOver.V),
									mouseMoveInside
								)
								;
						}
					)
					.Translate(() => -node.R.Pos)
					
					.MakeHot(nodeD);

				return nodeD;
			})
			.DisposeMany()
			.MakeHot(d);
		return d;
	}
	

	private static IUIEvt MakeEmpty() => new UIEvt(
		Var.MakeConst(May.None<nint>()),
		Obs.Never<IUserEvt>()
	);



// @formatter:off
	private static bool IsActive		(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_ACTIVE or WindowActivateFlag.WA_CLICKACTIVE;
	private static bool IsInactive		(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_INACTIVE;
	private static bool IsClickActive	(this WindowActivateFlag flag) => flag is WindowActivateFlag.WA_CLICKACTIVE;
// @formatter:on
}