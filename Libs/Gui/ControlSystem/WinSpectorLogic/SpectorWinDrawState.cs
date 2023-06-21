using System.Reactive;
using System.Reactive.Linq;
using ControlSystem.Structs;
using PowRxVar;

namespace ControlSystem.WinSpectorLogic;

sealed class SpectorWinDrawState : IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    public IRwMayVar<NodeState> SelNode { get; }
    public IRwMayVar<NodeState> HovNode { get; }
    public IObservable<Unit> WhenChanged { get; }

    public SpectorWinDrawState()
    {
        SelNode = VarMay.Make<NodeState>().D(d);
        HovNode = VarMay.Make<NodeState>().D(d);
        WhenChanged = SelNode.Skip(1).Merge(
            HovNode.Skip(1)
        ).ToUnit();
    }
}
