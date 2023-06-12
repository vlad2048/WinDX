using System.Reactive.Disposables;

namespace TreePusherLib;

public class TreePusher<T>
{
	protected ITreeEvtSig<T> EvtSig { get; }

	public TreePusher(ITreeEvtSig<T> evtSig) => EvtSig = evtSig;

	public virtual IDisposable Push(T args)
	{
		EvtSig.SignalPush(args);
		return Disposable.Create(() => EvtSig.SignalPop(args));
	}
}