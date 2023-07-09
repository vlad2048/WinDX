using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;

namespace ControlSystem.Structs;

/// <summary>
/// Layout result
/// </summary>
/// <param name="Win"></param>
/// <param name="WinSize"></param>
/// <param name="MixRoot"></param>
/// <param name="NodeMap"></param>
/// <param name="RMap">Guaranteed to have exactly one entry for every node in MixRoot</param>
/// <param name="WarningMap"></param>
/// <param name="UnbalancedCtrls"></param>
/// <param name="Ctrl2NodMap"></param>
sealed record MixLayout(
	Win Win,
	FreeSz WinSize,
	MixNode MixRoot,
	IReadOnlyDictionary<NodeState, MixNode> NodeMap,
	IReadOnlyDictionary<NodeState, R> RMap,
	IReadOnlyDictionary<NodeState, FlexWarning> WarningMap,
	IReadOnlyDictionary<Ctrl, IMixNode> UnbalancedCtrls,

	IReadOnlyDictionary<Ctrl, MixNode> Ctrl2NodMap
)
{
	private static readonly StFlexNode emptyStFlexNode = new StFlexNode(
		new NodeState().DisposeOnProgramExit(),
		new FlexNode(
			new DimVec(null, null),
			FlexFlags.None,
			Strats.Fill,
			Mg.Zero
		)
	);

	public static readonly MixLayout Empty = new(
		null!,
		new FreeSz(null, null),
		Nod.Make<IMixNode>(emptyStFlexNode),
		new Dictionary<NodeState, MixNode>(),
		new Dictionary<NodeState, R>(),
		new Dictionary<NodeState, FlexWarning>(),
		new Dictionary<Ctrl, IMixNode>(),
		new Dictionary<Ctrl, MixNode>()
	);
}

static class MixLayoutExt
{
	public static MixLayout Translate(this MixLayout lay, Pt ofs) => lay with
	{
		RMap = lay.RMap.MapValues(e => e + ofs)
	};
}