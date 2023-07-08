using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;

sealed class TNode : INode, IDisposable
{
    public Disp D { get; } = new();
    public void Dispose() => D.Dispose();

    private readonly string name;
    private readonly ISubject<IUserEvt> whenEvt;

    public int Depth { get; }
    public IRwVar<R> RSrc { get; }

    public IRoVar<R> R => RSrc.ToReadOnly();
    public IObservable<IUserEvt> Evt => whenEvt.AsObservable();
    public void DispatchEvt(IUserEvt evt) => whenEvt.OnNext(evt);
    public IObservable<Unit> WhenInvalidateRequired => Obs.Never<Unit>();

    public TNode(R r, int depth, string name)
    {
        this.name = name;
        Depth = depth;
        RSrc = Var.Make(r).D(D);
        whenEvt = new Subject<IUserEvt>().D(D);
    }

    public override string ToString() => name;
}