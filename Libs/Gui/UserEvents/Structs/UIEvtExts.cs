using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;
using PowBasics.Geom;
using PowRxVar;

namespace UserEvents.Structs;

public static class IUIEvtExts
{
	// *********
	// * Mouse *
	// *********
	public static IObservable<Pt> WhenMouseDown(this IObservable<IUserEvt> evt, MouseBtn btn = MouseBtn.Left) =>
		evt
			.OfType<MouseButtonDownUserEvt>()
			.Where(e => e.Btn == btn)
			.Select(e => e.Pos);

	public static IObservable<Pt> WhenMouseUp(this IObservable<IUserEvt> evt, MouseBtn btn = MouseBtn.Left) =>
		evt
			.OfType<MouseButtonUpUserEvt>()
			.Where(e => e.Btn == btn)
			.Select(e => e.Pos);

	public static IObservable<int> WhenMouseWheel(this IObservable<IUserEvt> evt) =>
		evt
			.OfType<MouseWheelUserEvt>()
			.Select(e => e.Direction);

	public static IObservable<Pt> WhenMouseMove(this IObservable<IUserEvt> evt) =>
		evt
			.OfType<MouseMoveUserEvt>()
			.Select(e => e.Pos);

	public static IObservable<Pt> WhenMouseEnter(this IObservable<IUserEvt> evt) =>
		evt
			.OfType<MouseEnterUserEvt>()
			.Select(e => e.Pos);

	public static IObservable<Unit> WhenMouseLeave(this IObservable<IUserEvt> evt) =>
		evt
			.OfType<MouseLeaveUserEvt>()
			.ToUnit();

	public static (IRoVar<bool>, IDisposable) IsMouseOver(this IObservable<IUserEvt> evt) =>
		Var.Make(
			false,
			Obs.Merge(
				evt.WhenMouseEnter().Select(_ => true),
				evt.WhenMouseLeave().Select(_ => false)
			)
				.Where(_ => !Cfg.V.Tweaks.DisableHover)
		);


	// ************
	// * Keyboard *
	// ************
	public static IObservable<Unit> WhenKeyDown(this IObservable<IUserEvt> evt, Keys key) =>
		evt
			.OfType<KeyDownUserEvt>()
			.Where(e => e.Key == key)
			.ToUnit();

	public static IObservable<Unit> WhenKeyUp(this IObservable<IUserEvt> evt, Keys key) =>
		evt
			.OfType<KeyUpUserEvt>()
			.Where(e => e.Key == key)
			.ToUnit();

	public static IObservable<Unit> WhenChar(this IObservable<IUserEvt> evt, char ch) =>
		evt
			.OfType<KeyCharUserEvt>()
			.Where(e => e.Char == ch)
			.ToUnit();
}