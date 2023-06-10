using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStrats;

/// <summary>
/// All the children will be:
///   - at the same position
///   - fill the available space (under the kids.DimVec Min/Max constraint)
/// </summary>
public class FillStrat : IStrat
{
	public override string ToString() => "Fill";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
		=>
			FillUtilsShared.ComputeLay(node, freeSz, kidDims);
}


static class FillUtilsShared
{
	public static LayNfo ComputeLay(
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