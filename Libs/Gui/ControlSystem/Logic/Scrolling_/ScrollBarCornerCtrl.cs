using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowRxVar;
using C = ControlSystem.Logic.Scrolling_.ScrollBarConsts;

namespace ControlSystem.Logic.Scrolling_;

public sealed class ScrollBarCornerCtrl : Ctrl
{
	public ScrollBarCornerCtrl()
	{
		var nodeRoot = new NodeState().D(D);

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].Dim(FlexFlags.ScrollBarCrossDims.Width, FlexFlags.ScrollBarCrossDims.Height).M)
			{
				r.Gfx.FillR(C.BackColor);
			}
		}).D(D);
	}
}