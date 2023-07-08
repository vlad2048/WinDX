using System.Reactive;
using PowRxVar;

// ReSharper disable once CheckNamespace
namespace UserEvents.Structs;

public static class Processor_RepeatedClick
{
	public static IObservable<Unit> WhenRepeatedClick(
		this IObservable<IUserEvt> evt,
		MouseBtn btn
	) =>
		evt.WhenMouseDown(btn)
			.ToUnit();
}