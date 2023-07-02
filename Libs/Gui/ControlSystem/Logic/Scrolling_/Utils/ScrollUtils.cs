using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace ControlSystem.Logic.Scrolling_.Utils;

static class ScrollUtils
{
	public static BoolVec IsScrollNeeded(
		BoolVec scrollEnabled,
		Sz viewSz,
		Sz contSz
	) =>
		(scrollEnabled.X, scrollEnabled.Y) switch
		{
			(false, false) => BoolVec.False,
			(truer, false) => new BoolVec(contSz.Width > viewSz.Width && viewSz.Height >= FlexFlags.ScrollBarCrossDims.Height, false),
			(false, truer) => new BoolVec(false, contSz.Height > viewSz.Height && viewSz.Width >= FlexFlags.ScrollBarCrossDims.Width),
			(truer, truer) => IsScrollNeededWhenTotallyEnabled(viewSz, contSz)
		};

	private static BoolVec IsScrollNeededWhenTotallyEnabled(Sz viewSz, Sz contSz)
	{
		var (viewX, viewY) = (viewSz.Width, viewSz.Height);
		var (contX, contY) = (contSz.Width, contSz.Height);

		var isX = viewSz.Height >= FlexFlags.ScrollBarCrossDims.Height;
		var isY = viewSz.Width >= FlexFlags.ScrollBarCrossDims.Width;

		if (contX > viewX)
		{
			viewY = Math.Max(0, viewY - FlexFlags.ScrollBarCrossDims.Height);
			return (contY > viewY) switch
			{
				truer => new BoolVec(isX, isY),
				false => new BoolVec(isX, false)
			};
		}
		if (contY > viewY)
		{
			viewX = Math.Max(0, viewX - FlexFlags.ScrollBarCrossDims.Width);
			return (contX > viewX) switch
			{
				truer => new BoolVec(isX, isY),
				false => new BoolVec(false, isY)
			};
		}

		return BoolVec.False;
	}
}