// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;


public readonly record struct BoolVec(bool X, bool Y)
{
	public override string ToString() => $"{X} x {Y}";

	public static readonly BoolVec False = new(false, false);
}


/*public readonly record struct ScrollVec(int X, int Y)
{
	public override string ToString() => $"{X} x {Y}";

	public static readonly ScrollVec Zero = new(0, 0);
}*/
