using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;
using C = ControlSystem.Logic.Scrolling_.ScrollBarConsts;

namespace ControlSystem.Logic.Scrolling_;

public sealed class ScrollBarCtrl : Ctrl
{
	public ScrollBarCtrl(Dir dir, ScrollDimState state)
	{
		var nodeRoot = new NodeState().D(D);
		var nodeStack = new NodeState().D(D);
		var nodeBtnDec = new NodeState().D(D);
		var nodeBtnInc = new NodeState().D(D);
		var nodeTrackDec = new NodeState().D(D);
		var nodeTrackInc = new NodeState().D(D);
		var nodeThumb = new NodeState().D(D);

		var thumbPos = 0;

		nodeRoot.Evt.Subscribe(e =>
		{
			var abc = 123;
		}).D(D);

		var btnDecState = nodeBtnDec.Evt.IsMouseOver().D(D).SelectVar(e => e ? ScrollBtnState.Hover : ScrollBtnState.Normal);

		Obs.Merge(
				btnDecState.ToUnit()
			)
			.Subscribe(_ => this.Invalidate()).D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].DimFilFix(dir, 17).M)
			{
				var (ptInnerA, ptInnerB) = GetInnerLine(r.Gfx.R, dir);
				var (ptOuterA, ptOuterB) = GetOuterLine(r.Gfx.R, dir);
				r.Gfx.DrawLine(ptInnerA, ptInnerB, C.EdgeInnerColor);
				r.Gfx.DrawLine(ptOuterA, ptOuterB, C.EdgeOuterColor);

				using (r[nodeStack].DimFil().StratStack(dir).Marg(dir, 0, 1).M)
				{
					r.Gfx.FillR(C.BackColor);

					using (r[nodeBtnDec].Dim(dir, 17, 15).M)
					{
						var (bmp, col) = C.GetBtnBmpCol(dir, ScrollBtnDecInc.Dec, btnDecState.V);
						r.Gfx.FillR(col);
						r.Gfx.DrawBmp(bmp);
					}


					using (r[nodeThumb].DimFil().M)
					{
						var thumbBrush = C.GetThumbBackColor(ScrollBtnState.Normal);
						r.Gfx.FillR(thumbBrush);
					}


					using (r[nodeBtnInc].Dim(dir, 17, 15).M)
					{
						var stateInc = ScrollBtnState.Normal;
						var (bmp, col) = C.GetBtnBmpCol(dir, ScrollBtnDecInc.Inc, stateInc);
						r.Gfx.FillR(col);
						r.Gfx.DrawBmp(bmp);
					}

				}
			}
		}).D(D);
	}

	private static (Pt, Pt) GetInnerLine(R r, Dir dir) => dir switch
	{
		Dir.Horz => (new Pt(r.X, r.Y), new Pt(r.X + r.Width, r.Y)),
		Dir.Vert => (new Pt(r.X, r.Y), new Pt(r.X, r.Y + r.Height)),
	};

	private static (Pt, Pt) GetOuterLine(R r, Dir dir) => dir switch
	{
		Dir.Horz => (new Pt(r.X, r.Y + r.Height - 1), new Pt(r.X + r.Width, r.Y + r.Height - 1)),
		Dir.Vert => (new Pt(r.X + r.Width - 1, r.Y), new Pt(r.X + r.Width - 1, r.Y + r.Height)),
	};
}