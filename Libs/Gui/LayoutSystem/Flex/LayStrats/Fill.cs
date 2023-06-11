using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStrats;

/// <summary>
/// • All the children will be overlapped at the same position (top left) <br/>
/// • Support scrolling <br/>
/// <br/>
/// If scrolling is enabled in a direction: <br/>
/// • We compute the kids layout as if this direction was Fit (to measure them) <br/>
/// • The FlexSolver will not clip the kids to this node in that direction <br/>
/// </summary>
public class FillStrat : IStrat
{
	public BoolVec ScrollEnabled { get; }

	public FillStrat(BoolVec scrollEnabled) => ScrollEnabled = scrollEnabled;

	public override string ToString() => "Fill" + ScrollEnabled switch
	{
		(false, false) => string.Empty,
		(truer, false) => "(scroll X)",
		(false, truer) => "(scroll Y)",
		(truer, truer) => "(scroll X/Y)",
	};

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		var sz = GeomMaker.SzDirFun(
			dir => freeSz.IsInfinite(dir) switch
			{
				false => freeSz.Dir(dir),
				truer => kidDims.Any() switch
				{
					truer => kidDims.Max(e => e.Dir(dir).Max.EnsureNotInf()),
					false => 0,
				}
			}
		);

		var kidRs =
			node.Children.Zip(kidDims)
				.Select(t => (kid: t.First, kidDim: t.Second))
				.Map(t => new R(
					Pt.Empty,
					GeomMaker.SzDirFun(dir => freeSz.IsInfinite(dir) switch
					{
						truer => t.kidDim.Dir(dir).Max.EnsureNotInf(),
						false => StackUtilsShared.LayoutMain(freeSz.Dir(dir), new[] { t.kidDim.Dir(dir) })[0]
					})
				));

		return new LayNfo(
			sz,
			kidRs
		);
	}
}
