using PowBasics.CollectionsExt;
using PowRxVar;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.TestSupport.Utils;

namespace UserEvents.Tests.TestSupport;

sealed class EventTester : IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    private readonly TUIEvt winEvt;
    private readonly TWin win;
    private readonly EventDispatcher eventDispatcher;
    private readonly ObservableChecker<IUserEvt> checkNodeR, checkNodeA, checkNodeB;

    public TNode NodeR { get; }
    public TNode NodeA { get; }
    public TNode NodeB { get; }

    public EventTester()
    {
        NodeR = new TNode(rR, 0).D(d);
        NodeA = new TNode(rA, 1).D(d);
        NodeB = new TNode(rB, 1).D(d);
        winEvt = new TUIEvt().D(d);
        checkNodeR = new ObservableChecker<IUserEvt>(NodeR.Evt.Evt, "NodeR", true, e => e.TranslateMouse(NodeR.R.V.Pos)).D(d);
        checkNodeA = new ObservableChecker<IUserEvt>(NodeA.Evt.Evt, "NodeA", true, e => e.TranslateMouse(NodeA.R.V.Pos)).D(d);
        checkNodeB = new ObservableChecker<IUserEvt>(NodeB.Evt.Evt, "NodeB", true, e => e.TranslateMouse(NodeB.R.V.Pos)).D(d);
        win = new TWin(winEvt);
        eventDispatcher = new EventDispatcher(win).D(d);
        UpdateNodes();
    }


    public void User(IUserEvt[] events)
    {
        foreach (var evt in events)
            winEvt.Send(evt);
    }

    public void Check(IUserEvt[] nodeR, IUserEvt[] nodeA, IUserEvt[] nodeB)
    {
        checkNodeR.Check(nodeR.SelectToArray(e => e.TranslateMouse(-NodeR.R.V.Pos)));
        checkNodeA.Check(nodeA.SelectToArray(e => e.TranslateMouse(-NodeA.R.V.Pos)));
        checkNodeB.Check(nodeB.SelectToArray(e => e.TranslateMouse(-NodeB.R.V.Pos)));
    }

    public void AddNodes(TNode[] arr)
    {
	    win.AddNodes(arr);
        UpdateNodes();
    }

    public void DelNodes(TNode[] arr)
    {
	    win.DelNodes(arr);
        UpdateNodes();
    }


    private void UpdateNodes() => eventDispatcher.Update(win.Nodes.OfType<INode>().ToArray());
}