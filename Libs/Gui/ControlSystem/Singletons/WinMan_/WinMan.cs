using System.Reactive.Disposables;
using DynamicData;
using PowRxVar;

namespace ControlSystem.Singletons.WinMan_;

sealed class WinMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceList<Win> wins;

	public IObservable<IChangeSet<Win>> Wins { get; }

	public WinMan()
	{
		wins = new SourceList<Win>().D(d);
		Wins = wins.Connect();
	}

	public void AddWin(Win win)
	{
		wins.Add(win);
		Disposable.Create(() =>
		{
			wins.Remove(win);
		}).D(win.D);
	}
}