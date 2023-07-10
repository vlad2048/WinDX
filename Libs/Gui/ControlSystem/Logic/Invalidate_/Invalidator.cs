using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using PowBasics.CollectionsExt;
using PowRxVar;
using UserEvents;
using WinAPI.Windows;

namespace ControlSystem.Logic.Invalidate_;



sealed class Invalidator : IInvalidator, IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IRwTracker<RedrawReason> reasonTracker;

	// IInvalidator
	// ============
	public void Invalidate(RedrawReason reason) => reasonTracker.Src.Add(reason);

	public Invalidator(IWin mainWin)
	{
		reasonTracker = Tracker.Make<RedrawReason>().D(d);

		reasonTracker.ItemsList.CountChanged.Where(e => e > 0).Subscribe(_ =>
		{
			mainWin.SysInvalidate();
		}).D(d);
	}

	public bool IsLayoutRequired()
	{
		var reasons = reasonTracker.ItemsArr;
		var isLayoutRequired = reasons.Any(e => e.IsLayoutRequired());
		reasonTracker.Src.Clear();

		if (Cfg.V.Log.Redraws)
			L($"Redraw (layout:{isLayoutRequired}) <- {reasons.JoinText("; ")}");

		return isLayoutRequired;
	}
}



file static class InvalidatorExt
{
	public static bool IsLayoutRequired(this RedrawReason e) => e switch
	{
		RedrawReason.Resize => true,
		RedrawReason.Ctrl => true,
		RedrawReason.Node => true,
		RedrawReason.SpectorOverlay => false,
		RedrawReason.SpectorRequestFullRedraw => true,
		RedrawReason.UserCode => true,
	};
}



/*
sealed class Invalidator : IInvalidator, IDisposable
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
*/