using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public record FlexNode(
	DimVec Dim,
	Marg Marg,
	IStrat Strat
)
{
	public override string ToString() => $"{Dim} - {Strat}";

	public DimVec DimWithMarg => new(DimWithMargDir(Dir.Horz), DimWithMargDir(Dir.Vert));

	public Dim DimWithMargDir(Dir dir)
	{
		var d = Dim.Dir(dir);
		return d.Typ() switch
		{
			DimType.Fix => D.Fix(d!.Value.Min + Marg.Dir(dir)),
			DimType.Flt => D.Flt(d!.Value.Min + Marg.Dir(dir), d.Value.Max + Marg.Dir(dir)),
			DimType.Fil => D.Fil,
			DimType.Fit => null,
		};
	}
}
