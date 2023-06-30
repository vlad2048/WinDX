using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace ControlSystem.Logic.Scrolling_;

static class ScrollUtils
{
	public static BoolVec IsScrollNeeded(
		BoolVec scrollEnabled,
		Sz viewSz,
		Sz contentSz
	) =>
		(scrollEnabled.X, scrollEnabled.Y) switch
		{
			(false, false) => BoolVec.False,
			(true, false) => new BoolVec(contentSz.Width > viewSz.Width, false),
			(false, true) => new BoolVec(false, contentSz.Height > viewSz.Height),
			(true, true) => IsScrollNeededWhenTotallyEnabled(viewSz, contentSz)
		};

	private static BoolVec IsScrollNeededWhenTotallyEnabled(Sz viewSz, Sz contentSz)
	{
		var (viewX, viewY) = (viewSz.Width, viewSz.Height);
		var (contX, contY) = (contentSz.Width, contentSz.Height);

		if (contX > viewX)
		{
			viewY = Math.Max(0, viewY - FlexFlags.ScrollBarCrossDims.Height);
			return (contY > viewY) switch
			{
				true => new BoolVec(true, true),
				false => new BoolVec(true, false)
			};
		}
		if (contY > viewY)
		{
			viewX = Math.Max(0, viewX - FlexFlags.ScrollBarCrossDims.Width);
			return (contX > viewX) switch
			{
				true => new BoolVec(true, true),
				false => new BoolVec(false, true)
			};
		}

		return BoolVec.False;
	}
}