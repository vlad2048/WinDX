using System.Reactive.Linq;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Utils;

public interface IPrettyUserEvt { DateTime Timestamp { get; } }

// @formatter:off
public sealed record NormalPrettyUserEvt			(DateTime Timestamp, IUserEvt Evt)					: IPrettyUserEvt { public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, $"{Evt}");				}
public sealed record DelayPrettyUserEvt			(DateTime Timestamp)								: IPrettyUserEvt { public override string ToString() => string.Empty;												}
public sealed record MouseMovePrettyUserEvt		(DateTime Timestamp, MouseMoveUserEvt Evt)			: IPrettyUserEvt { public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, $"Move {Evt.Pos}");	}
public sealed record MouseMoveUpdatePrettyUserEvt	(DateTime Timestamp, MouseMoveUserEvt Evt, int Idx)	: IPrettyUserEvt {
	public bool IsSubsequent => Idx > 1;
	public override string ToString() => PrettyPrintAggregator.Fmt(Timestamp, $"Move {Evt.Pos} (x{Idx})");
}
// @formatter:on

public static class PrettyPrintAggregator
{
	internal static string Fmt(DateTime t, string s) => $"[{t:HH:mm:ss.fff}] {s}";

	private static readonly TimeSpan Delay = TimeSpan.FromSeconds(1);

	public static IObservable<IPrettyUserEvt> Transform(IObservable<IUserEvt> uiEvt) => Obs.Create<IPrettyUserEvt>(obs =>
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

		var updateIdx = 0;
		void OnUpdate() => updateIdx++;
		void OnNonUpdate() => updateIdx = 0;

		uiEvt.Where(e => e is not MouseMoveUserEvt).Subscribe(e =>
		{
			isInMouseMove = false;
			CheckDelay();
			OnNonUpdate();
			obs.OnNext(new NormalPrettyUserEvt(T(), e));
		}).D(d);

		uiEvt.OfType<MouseMoveUserEvt>().Subscribe(e =>
		{
			CheckDelay();

			if (!isInMouseMove)
			{
				OnNonUpdate();
				obs.OnNext(new MouseMovePrettyUserEvt(T(), e));
				isInMouseMove = true;
			}
			else
			{
				OnUpdate();
				obs.OnNext(new MouseMoveUpdatePrettyUserEvt(T(), e, updateIdx));
			}
		}).D(d);

		return d;
	});




	/*public static IObservable<IPrettyUserEvt> Transform(IUIEvt uiEvt) => Obs.Create<IPrettyUserEvt>(obs =>
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

		var updateIdx = 0;
		void OnUpdate() => updateIdx++;
		void OnNonUpdate() => updateIdx = 0;

		uiEvt.Evt.Where(e => e is not MouseMoveUserEvt).Subscribe(e =>
		{
			isInMouseMove = false;
			CheckDelay();
			OnNonUpdate();
			obs.OnNext(new NormalPrettyUserEvt(T(), e));
		}).D(d);

		uiEvt.Evt.OfType<MouseMoveUserEvt>().Subscribe(e =>
		{
			CheckDelay();

			if (!isInMouseMove)
			{
				OnNonUpdate();
				obs.OnNext(new MouseMovePrettyUserEvt(T(), e));
				isInMouseMove = true;
			}
			else
			{
				OnUpdate();
				obs.OnNext(new MouseMoveUpdatePrettyUserEvt(T(), e, updateIdx));
			}
		}).D(d);

		return d;
	});*/
}