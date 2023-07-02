using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowMaybe;
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


		var reconstructedTree = treeEvtObs.ToTree(
			onPush: mixNode =>
			{
				switch (mixNode)
				{
					case CtrlNode { Ctrl: var ctrl } when partition.CtrlSet.Contains(ctrl):
						ctrl.SignalRender(renderArgs);
						break;
					case StFlexNode { State: var state, Flex: var flex }:
						var r = partition.GetNodeR(state).FailWith(R.Empty);
						gfx.R = r;
						if (flex.Flags.Scroll != BoolVec.False)
						{
							gfx.PushClip(r);
						}
						break;
				}
			},

			onPop: mixNode =>
			{
				if (mixNode is not StFlexNode st) return;
				var state = st.State;
				if (!partition.ExtraCtrlPopTriggers.TryGetValue(state, out var ctrls)) return;

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
}