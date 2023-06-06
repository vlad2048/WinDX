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
		DimVec[] kidDims
	)
	{
		int MkDir(Dir dir) => freeSz.IsInfinite(dir) switch
		{
			false => freeSz.Dir(dir),
			true => kidDims.Any() switch
			{
				false => 0,
				true => kidDims.Max(e => e.Dir(dir).Max)
			}
		};
	
		var sz = new Sz(MkDir(Dir.Horz), MkDir(Dir.Vert));
		var kidR = new R(Pt.Empty, sz);
		return new LayNfo(
			sz,
			node.Children.Map(_ => kidR)
		);
	}
}
