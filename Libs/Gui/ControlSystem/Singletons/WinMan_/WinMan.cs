using System.Reactive.Disposables;
using ControlSystem.Structs;
using DynamicData;
using PowRxVar;

namespace ControlSystem.Singletons.WinMan_;

sealed class WinMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceList<Win> wins;
	private readonly ISourceCache<MixLayout, Win> win2layout;

	public IObservable<IChangeSet<Win>> Wins { get; }
	public IObservable<IChangeSet<MixLayout, Win>> Win2Layout { get; }

	public WinMan()
	{
		wins = new SourceList<Win>().D(d);
		Wins = wins.Connect();

		win2layout = new SourceCache<MixLayout, Win>(e => e.Win).D(d);
		Win2Layout = win2layout.Connect();
	}

	public void AddWin(Win win)
	{
		wins.Add(win);
		Disposable.Create(() =>
		{
			wins.Remove(win);
		}).D(win.D);
	}

	public void SetWinLayout(MixLayout layout) => win2layout.AddOrUpdate(layout);
}