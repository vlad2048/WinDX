using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Utils;

// ReSharper disable once CheckNamespace
namespace UserEvents.Structs;

public static class Processor_RepeatedClick
{
	public static (IObservable<Unit>, IDisposable) WhenRepeatedClick(
		this IObservable<IUserEvt> evt,
		INode node,
		MouseBtn btn = MouseBtn.Left,
		IScheduler? scheduler = null,
		bool dbg = false
	)
	{
		var d = new Disp();

		var ptMin = new Pt(int.MinValue, int.MinValue);

		var mp = Var.Make(
			ptMin,
			Obs.Merge(
				evt.WhenMouseMove(),
				evt.WhenMouseUp(btn).Select(_ => ptMin),
				evt.WhenMouseDown()//.Do(e => LIf(dbg, $"MP(Down) <- {e}"))
			)
		).D(d);

		//mp.LogIf(dbg, "mp <- ").D(d);


		return
		(
			Obs.Merge(
					evt.WhenMouseDown(node, btn).Select(_ => true),
					evt.WhenMouseUp(btn).Select(_ => false)
				)
				//.Do(e => LIf(dbg, $"Switch({e})"))
				.Select(v => v switch
					{
						true =>
							Obs.Timer(InputConstants.RepeatClickDelay, InputConstants.RepeatClickSpeed, scheduler ?? DefaultScheduler.Instance)
								.ToUnit()
								.Prepend(Unit.Default)
								.Where(_ => node.ContainsMouse(mp.V)),
								//.Where(_ => node.ContainsMouse(MouseUtils.GetMousePos() - node.WinPos.V)),
						false =>
							Obs.Never<Unit>()
					}
				)
				.Switch()
				.ToUnit(),

			d
		);
	}

	private static bool ContainsMouse(this INode node, Pt mp) => node.R.V.Contains(mp);
}