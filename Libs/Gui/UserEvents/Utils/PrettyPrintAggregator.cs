﻿using System.Reactive.Linq;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Utils;

public interface IPrettyUserEvt { DateTime Timestamp { get; } }

// @formatter:off
public record NormalPrettyUserEvt			(DateTime Timestamp, IUserEvt Evt)			: IPrettyUserEvt { public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, $"{Evt}");							}
public record DelayPrettyUserEvt			(DateTime Timestamp)						: IPrettyUserEvt { public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, "(delay)");						}
public record MouseMovePrettyUserEvt		(DateTime Timestamp, MouseMoveUserEvt Evt)	: IPrettyUserEvt { public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, $"Move Move {Evt.Pos}");			}
public record MouseMoveUpdatePrettyUserEvt	(DateTime Timestamp, MouseMoveUserEvt Evt)	: IPrettyUserEvt { public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, $"Move Move Update {Evt.Pos}");	}
// @formatter:on

public static class PrettyPrintAggregator
{
	internal static string Fmt(DateTime t, string s) => $"[{t:HH:mm:ss.fff}] {s}";

	private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

	public static IObservable<IPrettyUserEvt> Transform(IUIEvt uiEvt) => Observable.Create<IPrettyUserEvt>(obs =>
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
				obs.OnNext(new DelayPrettyUserEvt(T()));
				isInMouseMove = false;
			}
		}


		uiEvt.Evt.Where(e => e is not MouseMoveUserEvt).Subscribe(e =>
		{
			isInMouseMove = false;
			CheckDelay();
			obs.OnNext(new NormalPrettyUserEvt(T(), e));
		}).D(d);

		uiEvt.Evt.OfType<MouseMoveUserEvt>().Subscribe(e =>
		{
			CheckDelay();

			if (!isInMouseMove)
			{
				obs.OnNext(new MouseMovePrettyUserEvt(T(), e));
				isInMouseMove = true;
			}
			else
			{
				obs.OnNext(new MouseMoveUpdatePrettyUserEvt(T(), e));
			}
		}).D(d);

		return d;
	});
}