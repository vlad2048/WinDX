using LayoutSystem.Flex.Structs;
using System.Text.Json.Serialization;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.LayStratsInternal;

namespace LayoutSystem.Flex;

[JsonDerivedType(typeof(FillStrat), typeDiscriminator: "Fill")]
[JsonDerivedType(typeof(StackStrat), typeDiscriminator: "Stack")]
[JsonDerivedType(typeof(WrapStrat), typeDiscriminator: "Wrap")]
[JsonDerivedType(typeof(MarginStrat), typeDiscriminator: "Margin")]
public interface IStrat
{
	LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	);
}

public interface IStratInternal : IStrat { }
