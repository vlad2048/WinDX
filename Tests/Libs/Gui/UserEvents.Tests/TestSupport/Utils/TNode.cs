using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Tests.TestSupport.Utils;

sealed class TNode : INode, IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    private readonly string name;
    private readonly ISubject<IUserEvt> whenEvt;

    public int Depth { get; }
    public IRwVar<R> RSrc { get; }

    public IRoVar<R> R => RSrc.ToReadOnly();
    public IObservable<IUserEvt> Evt => whenEvt.AsObservable();
    public void DispatchEvt(IUserEvt evt) => whenEvt.OnNext(evt);

    public TNode(R r, int depth, string name)
    {
	    this.name = name;
        Depth = depth;
        RSrc = Var.Make(r).D(d);
        whenEvt = new Subject<IUserEvt>().D(d);
    }

    public override string ToString() => name;
}