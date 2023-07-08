using DynamicData;
using Moq;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using UserEvents.Structs;
using WinAPI.Windows;

namespace UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;

class WinWrapper : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceList<INode> nodesSrc;

	public IWin Win { get; }

	public WinWrapper(IWin win, ISourceList<INode> nodesSrc)
	{
		Win = win;
		this.nodesSrc = nodesSrc;
	}

	public void AddNodes(TNode[] arr) => nodesSrc.AddRange(arr);
	public void DelNodes(TNode[] arr)
	{
		foreach (var elt in arr)
			nodesSrc.Remove(elt);
	}
}

static class TestWinMaker
{
	public static IMainWinUserEventsSupport MakeMainWin(
		IObservable<IUserEvt> evt
	)
	{
		var winMock = new Mock<IMainWinUserEventsSupport>();
		winMock
			.Setup(e => e.Evt)
			.Returns(evt);
		winMock
			.Setup(e => e.Invalidate());
		return winMock.Object;
	}

	public static WinWrapper Make(
	)
	{
		var d = new Disp();
		var winMock = new Mock<IWin>();

		var nodesSrc = new SourceList<INode>().D(d);
		var nodes = nodesSrc.Connect();

		winMock
			.Setup(e => e.Nodes)
			.Returns(nodes);

		return new WinWrapper(winMock.Object, nodesSrc);
	}
}

/*
sealed class TestWinMaker : IWin
{
    private readonly List<TNode> nodes = new();

    public TNode[] Nodes => nodes.OrderByDescending(e => e.Depth).ToArray();

    // IWin
    // ====
    public nint Handle => nint.Zero;
    public IObservable<IPacket> SysEvt => Obs.Never<IPacket>();
    public Pt PopupOffset => Pt.Empty;
    public IRoVar<Pt> ScreenPt => Var.MakeConst(Pt.Empty);
    public IRoVar<R> ScreenR => Var.MakeConst(R.Empty);
    public IObservable<IUserEvt> Evt { get; }
    public INode[] HitFun(Pt pt) => Nodes.Where(e => e.R.V.Contains(pt)).SelectToArray(e => (INode)e);
    public void Invalidate() { }

    public TestWinMaker(IObservable<IUserEvt> evt)
    {
        Evt = evt;
    }

    public void AddNodes(TNode[] arr) => nodes.AddRange(arr);

    public void DelNodes(TNode[] arr)
    {
        foreach (var elt in arr)
            nodes.Remove(elt);
    }
}
*/