using PowRxVar;
using TreePusherLib;

namespace ControlSystem.Structs;

sealed class MixNodeTreePusher : TreePusher<IMixNode>, IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public RenderArgs RenderArgs { get; set; } = null!;

	public MixNodeTreePusher(ITreeEvtSig<IMixNode> evtSig) : base(evtSig) { }

	public override IDisposable Push(IMixNode args)
	{
		var res = base.Push(args);
		if (args is CtrlNode { Ctrl: var ctrl })
			ctrl.SignalRender(RenderArgs);
		return res;
	}
}