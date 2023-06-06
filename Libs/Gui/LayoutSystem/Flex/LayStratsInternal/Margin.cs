using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStratsInternal;

public class MarginStrat : IStratInternal
{
	public override string ToString() => "Margin";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		DimVec[] kidDims
	)
	{
		if (kidDims.Length > 1) throw new ArgumentException("Margin nodes should have exactly 1 kid");
		if (kidDims.Length == 0) return new LayNfo(Sz.Empty, new[] { R.Empty });

		var kidDim = kidDims[0];
		var marg = node.V.Marg;

		int MkDir(Dir dir) => freeSz.IsInfinite(dir) switch
		{
			false => freeSz.Dir(dir),
			true => kidDim.Dir(dir).Max + marg.Dir(dir)
		};
	
		var sz = new Sz(MkDir(Dir.Horz), MkDir(Dir.Vert));
		var kidR = (sz.Width > marg.Dir(Dir.Horz) && sz.Height > marg.Dir(Dir.Vert)) switch
		{
			true => new R(
				marg.Left,
				marg.Top,
				sz.Width - marg.Dir(Dir.Horz),
				sz.Height - marg.Dir(Dir.Vert)
			),
			false => R.Empty
		};

		return new LayNfo(
			sz,
			new[] { kidR }
		);
	}
}