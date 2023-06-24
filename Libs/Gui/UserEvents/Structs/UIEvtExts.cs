using System.Reactive;
using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using WinAPI.User32;

namespace UserEvents.Structs;

public static class IUIEvtExts
{
	// *********
	// * Mouse *
	// *********
	public static IObservable<Pt> WhenMouseDown(this IObservable<IUserEvt> evt, MouseBtn btn) =>
		evt
			.OfType<MouseButtonDownUserEvt>()
			.Where(e => e.Btn == btn)
			.Select(e => e.Pos);

	public static IObservable<Pt> WhenMouseUp(this IObservable<IUserEvt> evt, MouseBtn btn) =>
		evt
			.OfType<MouseButtonUpUserEvt>()
			.Where(e => e.Btn == btn)
			.Select(e => e.Pos);

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


	// ************
	// * Keyboard *
	// ************
	public static IObservable<Unit> WhenKeyDown(this IObservable<IUserEvt> evt, VirtualKey key) =>
		evt
			.OfType<KeyDownUserEvt>()
			.Where(e => e.Key == key)
			.ToUnit();

	public static IObservable<Unit> WhenKeyUp(this IObservable<IUserEvt> evt, VirtualKey key) =>
		evt
			.OfType<KeyUpUserEvt>()
			.Where(e => e.Key == key)
			.ToUnit();

	public static IObservable<Unit> WhenChar(this IObservable<IUserEvt> evt, char ch) =>
		evt
			.OfType<KeyCharUserEvt>()
			.Where(e => e.Char == ch)
			.ToUnit();



/*	// *********
	// * Mouse *
	// *********
	public static IObservable<Pt> WhenMouseDown(this IUIEvt evt, MouseBtn btn) =>
		evt.Evt
			.OfType<MouseButtonDownUserEvt>()
			.Where(e => e.Btn == btn)
			.Select(e => e.Pos);

	public static IObservable<Pt> WhenMouseUp(this IUIEvt evt, MouseBtn btn) =>
		evt.Evt
			.OfType<MouseButtonUpUserEvt>()
			.Where(e => e.Btn == btn)
			.Select(e => e.Pos);

	public static IObservable<Pt> WhenMouseMove(this IUIEvt evt) =>
		evt.Evt
			.OfType<MouseMoveUserEvt>()
			.Select(e => e.Pos);

	public static IObservable<Pt> WhenMouseEnter(this IUIEvt evt) =>
		evt.Evt
			.OfType<MouseEnterUserEvt>()
			.Select(e => e.Pos);

	public static IObservable<Unit> WhenMouseLeave(this IUIEvt evt) =>
		evt.Evt
			.OfType<MouseLeaveUserEvt>()
			.ToUnit();


	// ************
	// * Keyboard *
	// ************
	public static IObservable<Unit> WhenKeyDown(this IUIEvt evt, VirtualKey key) =>
		evt.Evt
			.OfType<KeyDownUserEvt>()
			.Where(e => e.Key == key)
			.ToUnit();

	public static IObservable<Unit> WhenKeyUp(this IUIEvt evt, VirtualKey key) =>
		evt.Evt
			.OfType<KeyUpUserEvt>()
			.Where(e => e.Key == key)
			.ToUnit();

	public static IObservable<Unit> WhenChar(this IUIEvt evt, char ch) =>
		evt.Evt
			.OfType<KeyCharUserEvt>()
			.Where(e => e.Char == ch)
			.ToUnit();*/
}