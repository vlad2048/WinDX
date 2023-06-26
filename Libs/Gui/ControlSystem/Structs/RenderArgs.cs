using LayoutSystem.Flex;
using RenderLib.Renderers;
using PowRxVar;
using TreePusherLib;

namespace ControlSystem.Structs;


public sealed class RenderArgs : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly TreePusher<IMixNode> pusher;

	public IGfx Gfx { get; }

	internal RenderArgs(IGfx gfx, TreePusher<IMixNode> pusher)
	{
		Gfx = gfx;
		this.pusher = pusher;
	}

	public IDisposable Flex(FlexNodeFluent f) => pusher.Push(f.StBuild());
	public IDisposable Ctrl(Ctrl ctrl) => pusher.Push(new CtrlNode(ctrl));
}
