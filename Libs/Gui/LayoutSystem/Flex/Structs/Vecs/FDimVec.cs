// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;


public readonly record struct FDimVec(FDim X, FDim Y)
{
	public override string ToString() => $"{X} x {Y}";
}
