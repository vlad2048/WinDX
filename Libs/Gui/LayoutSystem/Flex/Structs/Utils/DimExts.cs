// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class DimExts
{
	public static DimType Typ(this Dim d) => d.HasValue switch
	{
		truer => d.Value.Type,
		false => DimType.Fit
	};
	public static bool IsFit(this Dim d) => d.Typ() == DimType.Fit;
	public static bool IsFil(this Dim d) => d.Typ() == DimType.Fil;

	public static string Fmt(this Dim v) => v.HasValue ? $"{v}" : "Fit";
}

