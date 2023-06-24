using PowBasics.Geom;
using PowRxVar;
using UserEvents.Generators;
using UserEvents.Structs;

namespace UserEvents.Tests.TestSupport.Utils;

sealed class TNode : INode, IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    public int Depth { get; }
    public IRwVar<R> RSrc { get; }
    public IUIEvt Evt { get; }

    public IRoVar<R> R => RSrc.ToReadOnly();
    public IRwVar<IUIEvt> EvtSrc { get; }

    public TNode(R r, int depth)
    {
        Depth = depth;
        RSrc = Var.Make(r).D(d);
        (EvtSrc, Evt) = UserEventGenerator.MakeWithSource().D(d);
    }
}