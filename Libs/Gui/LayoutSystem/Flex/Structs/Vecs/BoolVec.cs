// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;


public readonly record struct BoolVec(bool X, bool Y)
{
	public override string ToString() => $"{X} x {Y}";
}