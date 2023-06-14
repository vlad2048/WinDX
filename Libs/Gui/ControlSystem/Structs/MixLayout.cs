using ControlSystem;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace Structs;

record MixLayout(
	Win Win,
	MixNode MixRoot,
	IReadOnlyDictionary<NodeState, R> RMap,
	IReadOnlyDictionary<NodeState, FlexWarning> WarningMap
);
