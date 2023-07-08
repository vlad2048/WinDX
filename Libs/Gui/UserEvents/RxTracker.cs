using DynamicData;
using DynamicData.Alias;
using PowRxVar;

namespace UserEvents;


public interface IRoTracker<T>
{
	Disp D { get; }
	IObservable<IChangeSet<T>> Items { get; }
	T[] ItemsArr { get; }
}

public interface IRwTracker<T> : IRoTracker<T>, IDisposable
{
	void Update(T[] items);
}


sealed class RoTracker<T> : IRoTracker<T>, IDisposable
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();

	private readonly IObservableList<T> itemsList;
	public IObservable<IChangeSet<T>> Items { get; }
	public T[] ItemsArr => itemsList.Items.ToArray();

	public RoTracker(IObservable<IChangeSet<T>> items)
	{
		Items = items;
		itemsList = items.AsObservableList().D(D);
	}
}


sealed class RwTracker<T> : IRwTracker<T>
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();

	private readonly ISourceList<T> itemsSrc;
	private readonly IObservableList<T> itemsList;

	public IObservable<IChangeSet<T>> Items { get; }
	public T[] ItemsArr => itemsList.Items.ToArray();

	public RwTracker()
	{
		itemsSrc = new SourceList<T>().D(D);
		itemsList = itemsSrc.AsObservableList().D(D);
		Items = itemsSrc.Connect();
	}

	public void Update(T[] items) => itemsSrc.EditDiff(items);
}


public sealed record ItemWithSelector<T, U>(T Selector, U Item);


public static class Tracker
{
	public static (IRwTracker<T>, IDisposable) Make<T>() => new RwTracker<T>().WithDisp();


	public static IRoTracker<U> SelectTracker<T, U>(
		this IRoTracker<T> tracker,
		Func<T, U> selectorFun
	) =>
		new RoTracker<U>(tracker.Items.Transform(selectorFun)).D(tracker.D);


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