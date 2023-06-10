using LayoutSystem.Flex.Structs;

namespace LayoutSystem.Flex.LayStrats;

/// <summary>
/// If scrolling is enabled in a direction, we compute the kids
/// layout as if this direction was Fit
/// </summary>
public class ScrollStrat : IStrat
{
	public BoolVec Enabled { get; }

	public ScrollStrat(BoolVec enabled)
	{
		Enabled = enabled;
	}

	public override string ToString() => $"Scroll(enabled:{Enabled})";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
		=>
			FillUtilsShared.ComputeLay(node, freeSz, kidDims);
}
