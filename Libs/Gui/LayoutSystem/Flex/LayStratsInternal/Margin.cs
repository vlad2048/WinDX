using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;

namespace LayoutSystem.Flex.LayStratsInternal;

sealed class MarginStrat : IStratInternal
{
	public override string ToString() => "Margin";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		if (kidDims.Length > 1) throw new ArgumentException("Margin nodes should have exactly 1 kid");
		if (kidDims.Length == 0) return new LayNfo(Sz.Empty, new[] { R.Empty });

		var kidDim = kidDims[0];
		var marg = node.V.Marg;

		var sz = GeomMaker.SzDirFun(
			dir => freeSz.Dir(dir).HasValue switch
			{
				truer => freeSz.Dir(dir)!.Value,
				false => kidDim.Dir(dir).Max.EnsureNotInf() + marg.Dir(dir)
			}
		);

		var kidR = (sz.Width > marg.Dir(Dir.Horz) && sz.Height > marg.Dir(Dir.Vert)) switch
		{
			truer => new R(
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
