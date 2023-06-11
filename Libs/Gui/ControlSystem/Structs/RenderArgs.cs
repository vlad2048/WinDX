using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;
using RenderLib.Renderers;
using TreePusherLib;

namespace ControlSystem.Structs;

public class RenderArgs
{
	private readonly TreePusher<StFlexNode> pusher;
	public IGfx Gfx { get; }

	public RenderArgs(TreePusher<StFlexNode> pusher, IGfx gfx)
	{
		this.pusher = pusher;
		Gfx = gfx;
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
		pusher.Push(new StFlexNode(
			ctrl
		));
}