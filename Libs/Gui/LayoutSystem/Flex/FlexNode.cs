using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public record FlexNode(
	DimOptVec Dim,
	Marg Marg,
	IStrat Strat
)
{
	public override string ToString() => $"{Dim} - {Strat}";

	public DimOptVec DimWithMarg => new(DimWithMargDir(Dir.Horz), DimWithMargDir(Dir.Vert));

	public DimOpt DimWithMargDir(Dir dir)
	{
		var d = Dim.Dir(dir);
		return d.Typ() switch
		{
			DType.Fix => D.Fix(d!.Value.Min + Marg.Dir(dir)),
			DType.Flt => D.Flt(d!.Value.Min + Marg.Dir(dir), d.Value.Max + Marg.Dir(dir)),
			DType.Fil => D.Fil,
			DType.Fit => null,
		};
	}
}
