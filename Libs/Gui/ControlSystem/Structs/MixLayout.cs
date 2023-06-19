using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace Structs;


sealed record MixLayout(
	Win Win,
	FreeSz WinSize,
	MixNode MixRoot,
	IReadOnlyDictionary<NodeState, R> RMap,
	IReadOnlyDictionary<NodeState, FlexWarning> WarningMap,
	IReadOnlyDictionary<Ctrl, IMixNode> UnbalancedCtrls,

	IReadOnlyDictionary<Ctrl, MixNode> Ctrl2NodMap
);
