using LayoutSystem.Flex.Details.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex.Structs;

public sealed record FlexLayout(
	Node Root,
	FreeSz WinSize,
	IReadOnlyDictionary<Node, R> RMap,
	IReadOnlyDictionary<Node, FlexWarning> WarningMap,

	Node RootFixed,
	IReadOnlyDictionary<Node, R> RMapFixed,
	IReadOnlyDictionary<Node, NodeDetails> DetailsMapFixed
)
{
	public Sz TotalSz => RMap[Root].Size;
}
