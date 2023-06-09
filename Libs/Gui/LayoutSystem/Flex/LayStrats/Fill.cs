﻿using LayoutSystem.Flex.LayStratsUtils_;
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
public sealed class FillStrat : IStrat
{
	public override string ToString() => "Fill";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		var sz = LayStratsUtils.ComputeSz(
			node.V.Dim,
			freeSz,
			dir => kidDims.MaxT(e => e.Dir(dir).Max)
		);


		var kidRs =
			node.Children.Zip(kidDims)
				.Select(t => (kid: t.First, kidDim: t.Second))
				.Map(t => new R(
					Pt.Empty,
					GeomMaker.SzDirFun(dir => freeSz.Dir(dir).HasValue switch
					{
						false => t.kidDim.Dir(dir).Type switch
						{
							DimType.Fil => 0,
							_ => t.kidDim.Dir(dir).Max.EnsureNotInf()
						},
						truer => StackUtilsShared.LayoutMain(freeSz.Dir(dir)!.Value, new[] { t.kidDim.Dir(dir) })[0]
					})
				));

		return new LayNfo(
			sz,
			kidRs
		);
	}
}
