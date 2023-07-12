using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using PowBasics.Geom;

namespace ControlSystem.Logic.Scrolling_.Structs;

sealed class SysPartitionMut
{
    public Dictionary<NodeState, List<MixNode>> Forest { get; } = new();
    public Dictionary<NodeState, List<Ctrl>> CtrlTriggers { get; } = new();
    public Dictionary<NodeState, R> RMap { get; } = new();

    public SysPartition ToSysPartition() => new(
        Forest.ToDictionary(e => e.Key, e => e.Value.ToArray()),
        CtrlTriggers.ToDictionary(e => e.Key, e => e.Value.ToArray()),
        RMap
    );
}