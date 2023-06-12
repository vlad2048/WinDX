using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using TreePusherLib.Utils;

namespace TreePusherLib;


public interface ITreeEvtObs<out T>
{
	IObservable<T> WhenPush { get; }
	IObservable<T> WhenPop { get; }
}

public interface ITreeEvtSig<in T>
{
	void SignalPush(T args);
	void SignalPop(T args);
}

public sealed class TreeEvents<T> : ITreeEvtSig<T>, ITreeEvtObs<T>, IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISubject<T> whenPush;
	public IObservable<T> WhenPush => whenPush.AsObservable();
	public void SignalPush(T args) => whenPush.OnNext(args);

	private readonly ISubject<T> whenPop;
	public IObservable<T> WhenPop => whenPop.AsObservable();
	public void SignalPop(T args) => whenPop.OnNext(args);

	private TreeEvents()
	{
		whenPush = new Subject<T>().D(d);
		whenPop = new PopSubject<T>().D(d);
	}

	public static (ITreeEvtSig<T>, ITreeEvtObs<T>, IDisposable) Make()
	{
		var evt = new TreeEvents<T>();
		return (evt, evt, evt);
	}
}