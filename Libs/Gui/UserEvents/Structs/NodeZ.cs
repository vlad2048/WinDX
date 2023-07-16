namespace UserEvents.Structs;

public sealed record NodeZ(INode Node, ZOrder ZOrder)
{
	public override string ToString() => $"{Node}";
}