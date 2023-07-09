using System.Reactive.Disposables;
using DynamicData;
using PowRxVar;
using UserEvents;

namespace ControlSystem.Singletons.WinMan_;

sealed class WinMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IRwTracker<Win> rwMainWins;

	public IRoTracker<Win> MainWins => rwMainWins;

	public WinMan()
	{
		rwMainWins = Tracker.Make<Win>().D(d);
	}

	public void AddWin(Win win)
	{
		rwMainWins.Src.Add(win);
		Disposable.Create(() =>
		{
			rwMainWins.Src.Remove(win);
		}).D(win.D);
	}
}