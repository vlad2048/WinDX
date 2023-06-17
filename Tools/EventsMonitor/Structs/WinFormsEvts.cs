using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace EventsMonitor.Structs;

public interface IWinFormsEvt { }
public interface IWinFormsEvtMouse : IWinFormsEvt { }
public interface IWinFormsEvtMousePos : IWinFormsEvtMouse
{
	Pt Pos { get; }
}

public interface IWinFormsEvtKeyboard : IWinFormsEvt { }

public interface IWinFormsEvtWindow : IWinFormsEvt { }


// @formatter:off
public sealed record MouseButtonDownWinFormsEvt(Pt Pos, MouseBtn Btn)		: IWinFormsEvtMousePos			{ public override string ToString() => $"{Btn} down ({Pos})";							}
public sealed record   MouseButtonUpWinFormsEvt(Pt Pos, MouseBtn Btn)		: IWinFormsEvtMousePos			{ public override string ToString() => $"{Btn} up ({Pos})";								}
public sealed record       MouseMoveWinFormsEvt(Pt Pos)					: IWinFormsEvtMousePos			{ public override string ToString() => $"Move {Pos}";									}
public sealed record      MouseEnterWinFormsEvt							: IWinFormsEvtMouse				{ public override string ToString() => "Enter";											}
public sealed record      MouseLeaveWinFormsEvt							: IWinFormsEvtMouse				{ public override string ToString() => "Leave";											}

public sealed record         KeyDownWinFormsEvt(Keys Key)					: IWinFormsEvtKeyboard			{ public override string ToString() => $"'{Key}' down";									}
public sealed record           KeyUpWinFormsEvt(Keys Key)					: IWinFormsEvtKeyboard			{ public override string ToString() => $"'{Key}' up";									}
public sealed record         KeyCharWinFormsEvt(char Char)					: IWinFormsEvtKeyboard			{ public override string ToString() => $"'{Char}' char";								}

public sealed record        GotFocusWinFormsEvt							: IWinFormsEvtWindow			{ public override string ToString() => "Got focus";										}
public sealed record       LostFocusWinFormsEvt							: IWinFormsEvtWindow			{ public override string ToString() => "Lost focus";									}
public sealed record        ActivateWinFormsEvt(bool WithMouseClick)		: IWinFormsEvtWindow			{ public override string ToString() => "Activate" + (WithMouseClick ? " (mouse)" : "");	}
public sealed record      InactivateWinFormsEvt							: IWinFormsEvtWindow			{ public override string ToString() => "Inactivate";									}
public sealed record     ActivateAppWinFormsEvt							: IWinFormsEvtWindow			{ public override string ToString() => "ActivateApp";									}
public sealed record   InactivateAppWinFormsEvt							: IWinFormsEvtWindow			{ public override string ToString() => "InactivateApp";									}

public sealed record        EnterWinFormsEvt								: IWinFormsEvtWindow			{ public override string ToString() => "CtrlEnter";										}
public sealed record        LeaveWinFormsEvt								: IWinFormsEvtWindow			{ public override string ToString() => "CtrlLeave";										}
// @formatter:on


public static class WinFormsEvtGenerator
{
// @formatter:off
	public static IObservable<IWinFormsEvt> MakeForControl(Control ctrl) => Obs.Merge<IWinFormsEvt>(
		ctrl.Events().MouseDown		.Select(e => new MouseButtonDownWinFormsEvt		(e.GetPt(), e.GetBtn()	)),
		ctrl.Events().MouseUp		.Select(e => new MouseButtonUpWinFormsEvt		(e.GetPt(), e.GetBtn()	)),

		ctrl.Events().MouseMove		.Select(e => new MouseMoveWinFormsEvt			(e.GetPt()				)),
		ctrl.Events().MouseEnter	.Select(_ => new MouseEnterWinFormsEvt			(						)),
		ctrl.Events().MouseLeave	.Select(_ => new MouseLeaveWinFormsEvt			(						)),

		ctrl.Events().KeyDown		.Select(e => new KeyDownWinFormsEvt				(e.KeyCode				)),
		ctrl.Events().KeyUp			.Select(e => new KeyUpWinFormsEvt				(e.KeyCode				)),
		ctrl.Events().KeyPress		.Select(e => new KeyCharWinFormsEvt				(e.KeyChar				)),

		ctrl.Events().GotFocus		.Select(_ => new GotFocusWinFormsEvt			(						)),
		ctrl.Events().LostFocus		.Select(_ => new LostFocusWinFormsEvt			(						)),

		ctrl.Events().Enter			.Select(_ => new EnterWinFormsEvt				(						)),
		ctrl.Events().Leave			.Select(_ => new LeaveWinFormsEvt				(						))
	);
// @formatter:on

	private static Pt GetPt(this MouseEventArgs e) => new(e.X, e.Y);

	private static MouseBtn GetBtn(this MouseEventArgs e)
	{
		var isLeft = e.Button.HasFlag(MouseButtons.Left);
		var isRight = e.Button.HasFlag(MouseButtons.Right);
		var isMiddle = e.Button.HasFlag(MouseButtons.Middle);
		return (isLeft, isRight, isMiddle) switch
		{
			(true, false, false) => MouseBtn.Left,
			(false, true, false) => MouseBtn.Right,
			(false, false, true) => MouseBtn.Middle,
			_ => throw new ArgumentException(),
		};
	}
}







public interface IPrettyWinFormsEvt { DateTime Timestamp { get; } }

// @formatter:off
public sealed record NormalPrettyWinFormsEvt			(DateTime Timestamp, IWinFormsEvt Evt)					: IPrettyWinFormsEvt { public override string ToString() => PrettyPrintWinFormsAggregator.Fmt(Timestamp, $"{Evt}");				}
public sealed record DelayPrettyWinFormsEvt			(DateTime Timestamp)									: IPrettyWinFormsEvt { public override string ToString() => string.Empty;														}
public sealed record MouseMovePrettyWinFormsEvt		(DateTime Timestamp, MouseMoveWinFormsEvt Evt)			: IPrettyWinFormsEvt { public override string ToString() => PrettyPrintWinFormsAggregator.Fmt(Timestamp, $"Move {Evt.Pos}");	}
public sealed record MouseMoveUpdatePrettyWinFormsEvt	(DateTime Timestamp, MouseMoveWinFormsEvt Evt, int Idx)	: IPrettyWinFormsEvt {
	public bool IsSubsequent => Idx > 1;
	public override string ToString() => PrettyPrintWinFormsAggregator.Fmt(Timestamp, $"Move {Evt.Pos} (x{Idx})");
}
// @formatter:on

public static class PrettyPrintWinFormsAggregator
{
	internal static string Fmt(DateTime t, string s) => $"[{t:HH:mm:ss.fff}] {s}";

	private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

	public static IObservable<IPrettyWinFormsEvt> PrettyPrint(this IObservable<IWinFormsEvt> evt) => Obs.Create<IPrettyWinFormsEvt>(obs =>
	{
		var d = new Disp();

		var isInMouseMove = false;
		var lastMsgTime = DateTime.MaxValue;
		DateTime T() => DateTime.Now;

		void CheckDelay()
		{
			var time = DateTime.Now;
			var isDelay = time - lastMsgTime >= Delay;
			lastMsgTime = time;
			if (isDelay)
			{
				obs.OnNext(new DelayPrettyWinFormsEvt(T()));
				isInMouseMove = false;
			}
		}

		var updateIdx = 0;
		void OnUpdate() => updateIdx++;
		void OnNonUpdate() => updateIdx = 0;

		evt.Where(e => e is not MouseMoveWinFormsEvt).Subscribe(e =>
		{
			isInMouseMove = false;
			CheckDelay();
			OnNonUpdate();
			obs.OnNext(new NormalPrettyWinFormsEvt(T(), e));
		}).D(d);

		evt.OfType<MouseMoveWinFormsEvt>().Subscribe(e =>
		{
			CheckDelay();

			if (!isInMouseMove)
			{
				OnNonUpdate();
				obs.OnNext(new MouseMovePrettyWinFormsEvt(T(), e));
				isInMouseMove = true;
			}
			else
			{
				OnUpdate();
				obs.OnNext(new MouseMoveUpdatePrettyWinFormsEvt(T(), e, updateIdx));
			}
		}).D(d);

		return d;
	});
}