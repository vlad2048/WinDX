﻿using LayoutSystem.Flex.Structs;
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
	public ISpec Spec { get; }

	public FillStrat(ISpec spec) => Spec = spec;

	/*public override string ToString() => "Fill" + Spec switch
	{
		ScrollSpec {Enabled: (false, false)} => string.Empty,
		ScrollSpec {Enabled: (truer, false)} => " (scroll X)",
		ScrollSpec {Enabled: (false, truer)} => " (scroll Y)",
		ScrollSpec {Enabled: (truer, truer)} => " (scroll X/Y)",
		PopSpec => " (pop)",
		_ => throw new ArgumentException()
	};*/

	public override string ToString()
	{
		static string b(bool v) => v switch
		{
			true => "☑",
			false => "☐"
		};
		return Spec switch
		{
			ScrollSpec { Enabled: (false, false) } => "Fill",
			ScrollSpec { Enabled: var (sx, sy) } => $"Scroll({b(sx)},{b(sy)})",
			PopSpec => "Pop",
			_ => throw new ArgumentException()
		};
	}

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		/*var sz = GeomMaker.SzDirFun(
			dir => freeSz.Dir(dir).HasValue switch
			{
				truer => freeSz.Dir(dir)!.Value,
				false => kidDims.Any() switch
				{
					truer => kidDims.Max(e => e.Dir(dir).Max.EnsureNotInf()),
					false => 0,
				}
			}
		);*/

		var sz = GeomMaker.SzDirFun(dir =>
			{
				var d = node.V.Dim.Dir(dir);
				return d.Typ() switch
				{
					DimType.Fix => d!.Value.Max,
					DimType.Flt => d!.Value.Max,
					DimType.Fit => kidDims.MaxT(e => e.Dir(dir).Max.EnsureNotInf()),
					DimType.Fil => freeSz.Dir(dir).HasValue switch
					{
						truer => freeSz.Dir(dir)!.Value,
						false => 0
					}
				};
			}
		).CapWith(freeSz);

		var kidRs =
			node.Children.Zip(kidDims)
				.Select(t => (kid: t.First, kidDim: t.Second))
				.Map(t => new R(
					Pt.Empty,
					GeomMaker.SzDirFun(dir => freeSz.Dir(dir).HasValue switch
					{
						//false => t.kidDim.Dir(dir).Max.EnsureNotInf(),
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
