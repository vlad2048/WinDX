using DynamicData;
using DynamicData.Alias;
using PowRxVar;

namespace UserEvents;


public interface IRoTracker<T>
{
	Disp D { get; }
	IObservable<IChangeSet<T>> Items { get; }
	IObservableList<T> ItemsList { get; }
	T[] ItemsArr { get; }
}

public interface IRwTracker<T> : IRoTracker<T>, IDisposable
{
	ISourceList<T> Src { get; }
}


sealed class RoTracker<T> : IRoTracker<T>, IDisposable
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();

	// IRoTracker
	// ==========
	public IObservable<IChangeSet<T>> Items { get; }
	public IObservableList<T> ItemsList { get; }
	public T[] ItemsArr => ItemsList.Items.ToArray();

	public RoTracker(IObservable<IChangeSet<T>> items)
	{
		Items = items;
		ItemsList = items.AsObservableList().D(D);
	}
}


sealed class RwTracker<T> : IRwTracker<T>
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();

	// IRoTracker
	// ==========
	public IObservable<IChangeSet<T>> Items { get; }
	public IObservableList<T> ItemsList { get; }
	public T[] ItemsArr => ItemsList.Items.ToArray();

	// IRwTracker
	// ==========
	public ISourceList<T> Src { get; }

	public RwTracker()
	{
		Src = new SourceList<T>().D(D);
		ItemsList = Src.AsObservableList().D(D);
		Items = Src.Connect();
	}
}


public sealed record ItemWithSelector<T, U>(T Selector, U Item);


public static class Tracker
{
	public static void Update<T>(this IRwTracker<T> tracker, T[] items) => tracker.Src.EditDiff(items);

	public static (IRoTracker<T>, IDisposable) ToTracker<T>(this IObservable<T[]> obs)
	{
		var d = new Disp();
		var tracker = Make<T>().D(d);
		obs.Subscribe(tracker.Update).D(d);
		return (tracker, d);
	}


	public static (IRwTracker<T>, IDisposable) Make<T>() => new RwTracker<T>().WithDisp();


	public static IRoTracker<U> SelectTracker<T, U>(
		this IRoTracker<T> tracker,
		Func<T, U> selectorFun
	) =>
		new RoTracker<U>(tracker.Items.Transform(selectorFun)).D(tracker.D);

	/*public static IRoTracker<U> SelectTrackerDisposeMany<T, U>(
		this IRoTracker<T> tracker,
		Func<T, U> selectorFun
	) =>
		new RoTracker<U>(tracker.Items.Transform(selectorFun).DisposeMany()).D(tracker.D);*/


	public static IRoTracker<ItemWithSelector<T, U>> MergeManyTrackers<T, U>(
		this IRoTracker<T> outer,
		Func<T, IRoTracker<U>> innerFun
	)
	{
		var resSrc = new SourceList<ItemWithSelector<T, U>>().D(outer.D);

		outer.Items
			.MergeMany(selector => innerFun(selector).Items.Select(item => new ItemWithSelector<T, U>(selector, item)))
			.PopulateInto(resSrc).D(outer.D);

		var resSrcChanges = resSrc.Connect();

		var result = new RoTracker<ItemWithSelector<T, U>>(resSrcChanges).D(outer.D);

		return result;
	}
}