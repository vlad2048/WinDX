// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class DimMaker
{
// @formatter:off
	public static          FDim Fix(int val         ) => new(val, val         );
	public static          FDim Flt(int min, int max) => new(min, max         );
	public static readonly FDim Fil                   =  new(0  , int.MaxValue);
	public static readonly  Dim Fit                   =  null;
// @formatter:on
}
