using System.Drawing;
using RenderLib.Renderers;
using PowRxVar;
using RenderLib.Structs;
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

	public RenderStFlexNodeFluent this[NodeState nodeState] => new(this, nodeState);
	public IDisposable this[Ctrl ctrl] => pusher.Push(new CtrlNode(ctrl));

	internal IDisposable Flex(StFlexNode f)
	{
		f.State.SetNameIFN($"{f.Flex}");
		return pusher.Push(f);
	}

	internal IDisposable Ctrl(Ctrl ctrl) => pusher.Push(new CtrlNode(ctrl));


	public void DrawText(string text, FontDef font, Color color)
	{
		var sz = Gfx.MeasureText_(text, font);
		pusher.Push(new TextMeasureNode(sz)).Dispose();
		Gfx.DrawText_(text, font, color);
	}
}
