using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;
using RenderLib.Renderers;
using PowRxVar;
using TreePusherLib;

namespace ControlSystem.Structs;

public sealed class RenderArgs : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly MixNodeTreePusher pusher;

	public IGfx Gfx { get; }

	internal RenderArgs(IGfx gfx, ITreeEvtSig<IMixNode> treeEvtSig)
	{
		Gfx = gfx;
		pusher = new MixNodeTreePusher(treeEvtSig).D(d);
		pusher.RenderArgs = this;
	}

	public IDisposable Flex(
		NodeState state,
		DimVec dim,
		IStrat strat,
		Marg? marg = null
	) =>
		pusher.Push(new StFlexNode(
			state,
			new FlexNode(
				dim,
				strat,
				marg ?? Mg.Zero
			)
		));

	public IDisposable Ctrl(
		Ctrl ctrl
	) =>
		pusher.Push(new CtrlNode(
			ctrl
		));
}