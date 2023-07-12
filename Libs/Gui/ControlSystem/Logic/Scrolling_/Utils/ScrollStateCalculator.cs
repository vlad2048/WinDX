using ControlSystem.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using St = ControlSystem.Logic.Scrolling_.Structs.Enum.ScrollBarState;

namespace ControlSystem.Logic.Scrolling_.Utils;

static class ScrollStateCalculator
{
	private static Sz Cross => FlexFlags.ScrollBarCrossDims;

	/// <summary>
	/// Returns both ScrollBars states given the view and content size
	/// </summary>
	/// <param name="enabled">Is scrolling enabled per direction</param>
	/// <param name="view">Scroll node size</param>
	/// <param name="cont">Content of the scroll node size</param>
	/// <returns>ScrollBars states per direction</returns>
	public static (St, St) Get(BoolVec enabled, Sz view, Sz cont) =>
		(enabled.X, enabled.Y) switch
		{
			(false, false) => (
				St.Disabled,
				St.Disabled
			),

			(truer, false) => (
				(cont.Width > view.Width) switch
				{
					false => St.NotNeeded,
					truer => (view.Height > Cross.Height) switch
					{
						false => St.NeededButTooSmallToBeVisible,
						truer => St.Visible
					}
				},
				St.Disabled
			),

			(false, truer) => (
				St.Disabled,
				(cont.Height > view.Height) switch
				{
					false => St.NotNeeded,
					truer => (view.Height > Cross.Height) switch
					{
						false => St.NeededButTooSmallToBeVisible,
						truer => St.Visible
					}
				}
			),

			(truer, truer) => GetScrollBarStatesWhenBothAreEnabled(view, cont)
		};


	private static (St, St) GetScrollBarStatesWhenBothAreEnabled(Sz view, Sz cont)
	{
		var (viewX, viewY) = (view.Width, view.Height);
		var (contX, contY) = (cont.Width, cont.Height);
		var (crossX, crossY) = (Cross.Width, Cross.Height);

		var (smallX, smallY) = (viewY <= crossY, viewX <= crossX);

		var isX = false;
		bool NeedX() => contX > viewX;

		var isY = false;
		bool NeedY() => contY > viewY;

		for (var i = 0; i < 2; i++)
		{
			if (!isX && NeedX() && !smallX)
			{
				isX = true;
				viewY = (viewY - crossY).Cap();
			}

			if (!isY && NeedY() && !smallY)
			{
				isY = true;
				viewX = (viewX - crossX).Cap();
			}
		}

		return (
			(NeedX(), smallX) switch
			{
				(false, _) => St.NotNeeded,
				(truer, truer) => St.NeededButTooSmallToBeVisible,
				(truer, false) => St.Visible
			},
			(NeedY(), smallY) switch
			{
				(false, _) => St.NotNeeded,
				(truer, truer) => St.NeededButTooSmallToBeVisible,
				(truer, false) => St.Visible
			}
		);
	}
}