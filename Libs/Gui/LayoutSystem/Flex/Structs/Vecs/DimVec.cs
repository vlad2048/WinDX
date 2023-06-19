// ReSharper disable once CheckNamespace

using System.Text.Json.Serialization;

namespace LayoutSystem.Flex.Structs;


public readonly record struct DimVec(Dim X, Dim Y)
{
	[JsonIgnore]
	public bool IsResolvable => X.HasValue && Y.HasValue;

	public FDimVec ResolveEnsure() => IsResolvable switch
	{
		true  => new FDimVec(X!.Value, Y!.Value),
		false => throw new ArgumentException()
	};

	public override string ToString() => $"{X.Fmt()} x {Y.Fmt()}";
}