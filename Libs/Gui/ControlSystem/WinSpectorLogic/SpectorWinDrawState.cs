using System.Reactive;
using System.Reactive.Linq;
using PowRxVar;

namespace ControlSystem.WinSpectorLogic;

sealed class SpectorWinDrawState : IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    public IRwMayVar<MixNode> SelNode { get; }
    public IRwMayVar<MixNode> HovNode { get; }
    public IRwMayVar<INode> LockedNode { get; }
    public IObservable<Unit> WhenChanged { get; }

    public SpectorWinDrawState()
    {
        SelNode = VarMay.Make<MixNode>().D(d);
        HovNode = VarMay.Make<MixNode>().D(d);
        LockedNode = VarMay.Make<INode>().D(d);
        WhenChanged = Obs.Merge(
	        SelNode.Skip(1).ToUnit(),
            HovNode.Skip(1).ToUnit(),
            LockedNode.Skip(1).ToUnit()
        );
    }
}
