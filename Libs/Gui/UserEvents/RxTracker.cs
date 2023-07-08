using DynamicData;
using PowRxVar;

namespace UserEvents;

public sealed class RxTracker<T> : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceList<T> winsSrc;

	public IObservable<IChangeSet<T>> Items { get; }

	public RxTracker()
	{
		winsSrc = new SourceList<T>().D(d);
		Items = winsSrc.Connect();
	}

	public void Update(T[] wins) => winsSrc.EditDiff(wins);
}

