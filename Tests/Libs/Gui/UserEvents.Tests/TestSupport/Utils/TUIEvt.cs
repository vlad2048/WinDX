using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Tests.TestSupport.Utils;

sealed class TUIEvt : IUIEvt, IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    private readonly ISubject<IUserEvt> whenEvt;

    public IRoMayVar<nint> WinHandle => VarMay.MakeConst<nint>();
    public IObservable<IUserEvt> Evt => whenEvt.AsObservable();

    public TUIEvt()
    {
        whenEvt = new Subject<IUserEvt>().D(d);
    }

    public void Send(IUserEvt evt) => whenEvt.OnNext(evt);
}