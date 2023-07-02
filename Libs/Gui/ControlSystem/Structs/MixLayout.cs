using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace ControlSystem.Structs;

/// <summary>
/// Layout result
/// </summary>
/// <param name="Win"></param>
/// <param name="WinSize"></param>
/// <param name="MixRoot"></param>
/// <param name="RMap">Guaranteed to have exactly one entry for every node in MixRoot</param>
/// <param name="WarningMap"></param>
/// <param name="UnbalancedCtrls"></param>
/// <param name="Ctrl2NodMap"></param>
sealed record MixLayout(
	Win Win,
	FreeSz WinSize,
	MixNode MixRoot,
	IReadOnlyDictionary<NodeState, R> RMap,
	IReadOnlyDictionary<NodeState, FlexWarning> WarningMap,
	IReadOnlyDictionary<Ctrl, IMixNode> UnbalancedCtrls,

	IReadOnlyDictionary<Ctrl, MixNode> Ctrl2NodMap
);

static class MixLayoutExt
{
	public static MixLayout Translate(this MixLayout lay, Pt ofs) => lay with
	{
		RMap = lay.RMap.MapValues(e => e + ofs)
	};
}