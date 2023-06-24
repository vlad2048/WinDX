/*
using DynamicData;
using PowRxVar;

namespace ControlSystem.Utils;

sealed class SetWatcher<T> : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceList<T> setSrc;

	public IObservable<IChangeSet<T>> Set { get; }

	public SetWatcher()
	{
		setSrc = new SourceList<T>().D(d);
		Set = setSrc.Connect();
	}

	public void Update(T[] arr) => setSrc.EditDiff(arr);
}
*/