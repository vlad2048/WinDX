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
		IScheduler? scheduler = null
	)
	{
		var d = new Disp();
		var mp = Pt.Empty;
		evt.WhenMouseMove().Subscribe(e => mp = e).D(d);

		return
		(
			Obs.Merge(
					evt.WhenMouseDown(btn).Select(_ => true),
					evt.WhenMouseUp(btn).Select(_ => false)
				)
				.Select(v => v switch
					{
						true =>
							Obs.Timer(InputConstants.RepeatClickDelay, InputConstants.RepeatClickSpeed, scheduler ?? DefaultScheduler.Instance)
								.ToUnit()
								.Prepend(Unit.Default)
								.Where(_ => node.ContainsMouse(mp)),
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