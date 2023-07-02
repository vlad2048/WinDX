using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
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
					case StFlexNode { State: var state }:
						gfx.R = partition.GetNodeR(state).FailWith(R.Empty);
						break;
				}
			},

			onPop: mixNode =>
			{
				if (mixNode is not StFlexNode st) return;
				var state = st.State;
				if (!partition.ScrollBars.ControlMap.TryGetValue(state, out var ctrls)) return;

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