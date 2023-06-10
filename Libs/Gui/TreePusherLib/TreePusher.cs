using System.Reactive.Disposables;

namespace TreePusherLib;

public class TreePusher<T>
{
	private readonly ITreeEvtSig<T> evtSig;

	public TreePusher(ITreeEvtSig<T> evtSig) => this.evtSig = evtSig;

	public IDisposable Push(T args)
	{
		evtSig.SignalPush(args);
		return Disposable.Create(() => evtSig.SignalPop(args));
	}
}