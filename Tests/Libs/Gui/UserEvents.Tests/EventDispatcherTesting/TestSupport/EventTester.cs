using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;
using static UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils.GeomData;

namespace UserEvents.Tests.EventDispatcherTesting.TestSupport;

sealed record NodeEvt(INode N, IUserEvt Evt)
{
    public override string ToString() => $"[{N}] - {Evt}";
}

sealed class EventTester : IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    private readonly ISubject<IUserEvt> whenEvt;
    private readonly WinWrapper winWrapper;
    private readonly ObservableChecker<NodeEvt> checker;

    public TNode NodeR { get; }
    public TNode NodeA { get; }
    public TNode NodeB { get; }

    public EventTester()
    {
        whenEvt = new Subject<IUserEvt>().D(d);
        NodeR = new TNode(rR, 0, "R").D(d);
        NodeA = new TNode(rA, 1, "A").D(d);
        NodeB = new TNode(rB, 1, "B").D(d);
        checker = new ObservableChecker<NodeEvt>(
            Obs.Merge(
                NodeR.Evt.Select(e => new NodeEvt(NodeR, e)),
                NodeA.Evt.Select(e => new NodeEvt(NodeA, e)),
                NodeB.Evt.Select(e => new NodeEvt(NodeB, e))
            ),
            "user",
            true,
            e => e with { Evt = e.Evt.TranslateMouse(e.N.R.V.Pos) }
        ).D(d);
        winWrapper = TestWinMaker.Make(whenEvt.AsObservable()).D(d);

        var nodes = new[] { NodeB, NodeA, NodeR };
        INode[] HitFun(Pt pt) => nodes.Where(e => e.R.V.Contains(pt)).OfType<INode>().ToArray();
        //var mainWin = TestWinMaker.MakeMainWin(whenEvt.AsObservable(), HitFun);
        var eventDispatcher = new EventDispatcher(winWrapper.Win, winWrapper.Win).D(d);
    }


    public void User(IUserEvt[] events)
    {
        foreach (var evt in events)
        {
            L($"[User] -> {evt}");
            whenEvt.OnNext(evt);
        }
    }

    public void Check(NodeEvt[] evts) => checker.Check(evts.SelectToArray(e => e with { Evt = e.Evt.TranslateMouse(-e.N.R.V.Pos) }));

    public void AddNodes(TNode[] arr) => winWrapper.AddNodes(arr);

    public void DelNodes(TNode[] arr) => winWrapper.DelNodes(arr);


    //private void UpdateNodes() => eventDispatcher.Update(win.Nodes.OfType<INode>().ToArray());
}