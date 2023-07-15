using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Rendering_;
using ControlSystem.Logic.Scrolling_;
using ControlSystem.Logic.Scrolling_.Utils;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers;
using TreePusherLib;
using TreePusherLib.ConvertExts;

namespace ControlSystem.Utils;

static class RenderUtils
{
	public static void RenderTree(
		Partition part,
		IGfx gfx,
		Pt ofs,
		bool log
	)
	{
		using var d = new Disp();
		var (treeEvtSig, treeEvtObs) = TreeEvents<IMixNode>.Make().D(d);
		var pusher = new TreePusher<IMixNode>(treeEvtSig);
		var r = new RenderArgs(gfx, pusher).D(d);

		var lgr = new RenderLogger(log, part);
		lgr.Start();


		r.WhenCtrlPushPrev.Subscribe(ctrl =>
		{
			lgr.PushCtrl(ctrl);
		}).D(d);

		r.WhenCtrlPushNext.Subscribe(ctrl =>
		{
			if (!part.CtrlsToRender.Contains(ctrl)) return;

			ctrl.SignalRender(r);
		}).D(d);

		r.WhenCtrlPopPrev.Subscribe(ctrl =>
		{
		}).D(d);

		r.WhenCtrlPopNext.Subscribe(ctrl =>
		{
			lgr.PopCtrl(ctrl);
		}).D(d);


		
		r.WhenFlexPushPrev.Subscribe(st =>
		{
			st.State.SetNameIFN($"{st.Flex}");
			lgr.PushFlex(st);
		}).D(d);

		r.WhenFlexPushNext.Subscribe(st =>
		{
			var (state, flex) = st;
			if (part.NodeStates.Contains(state))
			{
				var nfo = GetRenderNfo(part, state, flex);
				gfx.R = nfo.R + ofs;
				if (nfo.Clip)
				{
					r.PushClip(nfo.R + ofs);
				}
			}
			else
			{
				gfx.R = R.Empty;
			}
		}).D(d);

		r.WhenFlexPopPrev.Subscribe(st =>
		{
			var ctrls = part.Set.ExtraCtrlsToRenderOnPop.GetOrEmpty(st.State);
			if (ctrls.Any())
			{
				r.PopClip();
				foreach (var ctrl in ctrls)
					ctrl.SignalRender(r);
			}
		}).D(d);

		r.WhenFlexPopNext.Subscribe(st =>
		{
			lgr.PopFlex(st);
		}).D(d);


		r.WhenDraw.Subscribe(str =>
		{
			lgr.Draw(str);
		}).D(d);



		treeEvtObs.ToTree(
			onPush: _ => { },
			onPop: _ => { },
			runAction: () =>
			{
				using (r[part.RenderCtrl]) { }
			}
		);

		lgr.Finish();
	}




	private sealed record FlexRenderNfo(
		R R,
		bool Clip
	);

	private static FlexRenderNfo GetRenderNfo(Partition partition, NodeState state, FlexNode flex)
	{
		if (flex.Flags.Scroll == BoolVec.False)
		{
			var r = partition.Set.RMap[state];
			return new FlexRenderNfo(r, false);
		}
		else
		{
			var nfo = partition.Set.GetScrollInfos(state);
			return new FlexRenderNfo(nfo.ViewR, true);
		}
	}
}