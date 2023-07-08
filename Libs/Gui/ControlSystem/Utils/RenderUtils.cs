using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers;
using TreePusherLib;
using TreePusherLib.ConvertExts;

namespace ControlSystem.Utils;

static class RenderUtils
{
	public static void RenderTree(
		Partition partition,
		IGfx gfx
	)
	{
		using var d = new Disp();
		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var renderArgs = new RenderArgs(gfx, pusher).D(d);


		treeEvtObs.ToTree(
			onPush: mixNode =>
			{
				switch (mixNode)
				{
					case CtrlNode { Ctrl: var ctrl } when partition.CtrlSet.Contains(ctrl):
						ctrl.SignalRender(renderArgs);
						break;
					case StFlexNode { State: var state, Flex: var flex }:
						var nfo = GetRenderNfo(partition, state, flex);
						gfx.R = nfo.R;
						if (nfo.Clip)
							gfx.PushClip(nfo.R);
						break;
				}
			},

			onPop: mixNode =>
			{
				if (mixNode is not StFlexNode st) return;
				if (!partition.SysPartition.CtrlTriggers.TryGetValue(st.State, out var ctrls)) return;

				gfx.PopClip();

				foreach (var ctrl in ctrls)
				{
					ctrl.SignalRender(renderArgs);
				}
			},

			runAction: () =>
			{
				using (renderArgs[partition.RootCtrl]) { }
			}
		);
	}


	private sealed record FlexRenderNfo(
		R R,
		bool Clip
	);

	private static FlexRenderNfo GetRenderNfo(Partition partition, NodeState state, FlexNode flex)
	{
		var sys = partition.SysPartition;
		if (sys.RMap.TryGetValue(state, out var r))
			return new FlexRenderNfo(r, false);

		var scrollNfo = partition.GetScrollInfos(state);
		return new FlexRenderNfo(
			scrollNfo.ViewR,
			flex.Flags.Scroll != BoolVec.False
		);
	}
}