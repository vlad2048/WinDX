using ControlSystem.Logic.PopupLogic;
using ControlSystem.Structs;
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

        var reconstructedTree = treeEvtObs.ToTree(
            mixNode =>
            {
                switch (mixNode)
                {
                    case CtrlNode { Ctrl: var ctrl } when partition.CtrlSet.Contains(ctrl):
                        ctrl.SignalRender(renderArgs);
                        break;
                    case StFlexNode { State: var state }:
                        gfx.R = partition.RMap.GetValueOrDefault(state, R.Empty);
                        break;
                }
            },
            () =>
            {
                using (renderArgs[partition.RootCtrl]) { }
            }
        );
    }
}