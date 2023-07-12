using System.Reactive.Linq;
using ControlSystem.Logic.Scrolling_.State;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Structs.Enum;
using ControlSystem.Structs;
using LoggingConfig.Threading_;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;
using C = ControlSystem.Logic.Scrolling_.ScrollBarConsts;

namespace ControlSystem.Logic.Scrolling_;

public sealed class ScrollBarCtrl : Ctrl
{
	public ScrollBarCtrl(Dir dir, ScrollState state, IObservable<IUserEvt> nodeEvt)
	{
		var nodeRoot = new NodeState().D(D);
		var nodeStack = new NodeState().D(D);
		var nodeBtnDec = new NodeState().D(D);
		var nodeBtnInc = new NodeState().D(D);
		var nodeTrackDec = new NodeState().D(D);
		var nodeTrackInc = new NodeState().D(D);
		var nodeThumb = new NodeState().D(D);

		ScrollBarCtrlUtils.HookBtn(out var btnDecState, state, ScrollBtnDecInc.Dec, nodeBtnDec).D(D);
		ScrollBarCtrlUtils.HookBtn(out var btnIncState, state, ScrollBtnDecInc.Inc, nodeBtnInc).D(D);
		ScrollBarCtrlUtils.HookTrack(state, ScrollBtnDecInc.Dec, nodeTrackDec).D(D);
		ScrollBarCtrlUtils.HookTrack(state, ScrollBtnDecInc.Inc, nodeTrackInc).D(D);


		if (dir == Dir.Vert)
		{
			nodeEvt.OfType<MouseWheelUserEvt>()
				.Select(e => (e, new WheelScrollCmd(Dir.Vert, Math.Sign(e.Direction) == -1 ? ScrollBtnDecInc.Dec : ScrollBtnDecInc.Inc)))
				.Where(t => state.CanRunScrollCmd(t.Item2))
				.Subscribe(t =>
				{
					t.e.Handled = true;
					state.RunScrollCmd(t.Item2);
				}).D(D);
		}

		Obs.Merge(
				btnDecState.ToUnit(),
				btnIncState.ToUnit()
			)
			.Subscribe(_ => SignalChanged()).D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].DimFilFix(dir, 17).M)
			{
				var (ptInnerA, ptInnerB) = ScrollBarCtrlUtils.GetInnerLine(r.R, dir);
				var (ptOuterA, ptOuterB) = ScrollBarCtrlUtils.GetOuterLine(r.R, dir);
				r.DrawLine(ptInnerA, ptInnerB, C.EdgeInnerColor);
				r.DrawLine(ptOuterA, ptOuterB, C.EdgeOuterColor);

				using (r[nodeStack].DimFil().StratStack(dir).Marg(dir, 0, 1).M)
				{
					r.FillR(C.BackColor);

					// Button Decrement
					// ----------------
					using (r[nodeBtnDec].Dim(dir, 17, 15).M)
					{
						var (bmp, col) = C.GetBtnBmpCol(dir, ScrollBtnDecInc.Dec, btnDecState.V);
						r.FillR(col);
						r.DrawBmp(bmp);
					}

					// Track Dec
					// ---------
					using (r[nodeTrackDec].Dim(dir, state.TrackDecLng, 15).M)
					{
						//r.FillR();
					}

					// Thumb
					// -----
					using (r[nodeThumb].Dim(dir, state.ThumbLng, 15).M)
					{
						var thumbBrush = C.GetThumbBackColor(ScrollBtnState.Normal);
						r.FillR(thumbBrush);
					}

					// Track Inc
					// ---------
					using (r[nodeTrackInc].Dim(dir, state.TrackIncLng, 15).M)
					{
						//r.FillR();
					}

					// Button Increment
					// ----------------
					using (r[nodeBtnInc].Dim(dir, 17, 15).M)
					{
						var (bmp, col) = C.GetBtnBmpCol(dir, ScrollBtnDecInc.Inc, btnIncState.V);
						r.FillR(col);
						r.DrawBmp(bmp);
					}

				}
			}
		}).D(D);
	}
}



file static class ScrollBarCtrlUtils
{
	public static IDisposable HookBtn(
		out IRoVar<ScrollBtnState> btnState,
		ScrollState state,
		ScrollBtnDecInc decInc,
		NodeState node
	)
	{
		var d = new Disp();
		btnState = node.Evt.IsMouseOver().D(d).SelectVar(e => e ? ScrollBtnState.Hover : ScrollBtnState.Normal);

		node.Evt.WhenRepeatedClick(node).D(d)
			.ObserveOnUIThread()
			.Select(_ => new UnitScrollCmd(state.Dir, decInc))
			.Where(state.CanRunScrollCmd)
			.Subscribe(state.RunScrollCmd).D(d);

		return d;
	}


	public static IDisposable HookTrack(
		ScrollState state,
		ScrollBtnDecInc decInc,
		NodeState node
	)
	{
		var d = new Disp();

		node.Evt.WhenRepeatedClick(node).D(d)
			.ObserveOnUIThread()
			.Select(_ => new PageScrollCmd(state.Dir, decInc))
			.Where(state.CanRunScrollCmd)
			.Subscribe(state.RunScrollCmd).D(d);

		return d;
	}


	public static (Pt, Pt) GetInnerLine(R r, Dir dir) => dir switch
	{
		Dir.Horz => (new Pt(r.X, r.Y), new Pt(r.X + r.Width, r.Y)),
		Dir.Vert => (new Pt(r.X, r.Y), new Pt(r.X, r.Y + r.Height)),
	};

	public static (Pt, Pt) GetOuterLine(R r, Dir dir) => dir switch
	{
		Dir.Horz => (new Pt(r.X, r.Y + r.Height - 1), new Pt(r.X + r.Width, r.Y + r.Height - 1)),
		Dir.Vert => (new Pt(r.X + r.Width - 1, r.Y), new Pt(r.X + r.Width - 1, r.Y + r.Height)),
	};
}