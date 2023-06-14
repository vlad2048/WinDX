using PowBasics.Geom;

namespace LayoutSystem.Flex.Structs;


public record Layout(
	Node Root,
	IReadOnlyDictionary<Node, R> RMap,
	IReadOnlyDictionary<Node, FlexWarning> WarningMap
)
{
	public Sz TotalSz => RMap[Root].Size;
}
