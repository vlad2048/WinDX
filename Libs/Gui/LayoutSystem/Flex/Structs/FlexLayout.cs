using LayoutSystem.Flex.Details.Structs;
using PowBasics.CollectionsExt;
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


/*
public static class FlexLayoutExt
{
	public static FlexLayout Translate(this FlexLayout lay, Pt ofs) => lay with
	{
		RMap = lay.RMap.MapValues(e => e + ofs),
		RMapFixed = lay.RMapFixed.MapValues(e => e + ofs),
	};
}
*/