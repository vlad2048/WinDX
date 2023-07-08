using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using UserEvents;

namespace ControlSystem.Logic.Invalidate_;

class Invalidator : IInvalidator, IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISubject<Unit> whenInvalidateLayout;
	private readonly ISubject<Unit> whenInvalidateRender;

	private IObservable<Unit> WhenInvalidateLayout => whenInvalidateLayout.AsObservable();
	private IObservable<Unit> WhenInvalidateRender => whenInvalidateRender.AsObservable();

	private bool layoutRequired = true;


	public void InvalidateLayout() => whenInvalidateLayout.OnNext(Unit.Default);
	public void InvalidateRender() => whenInvalidateRender.OnNext(Unit.Default);

	public IObservable<Unit> WhenChanged => Obs.Merge(WhenInvalidateLayout, WhenInvalidateRender);
	public void SetLayoutRequired() => layoutRequired = true;
	public bool IsLayoutRequired()
	{
		var res = layoutRequired;
		layoutRequired = false;
		return res;
	}

	public Invalidator()
	{
		whenInvalidateLayout = new Subject<Unit>().D(d);
		whenInvalidateRender = new Subject<Unit>().D(d);

		WhenInvalidateLayout.Subscribe(_ => layoutRequired = true).D(d);
	}
}