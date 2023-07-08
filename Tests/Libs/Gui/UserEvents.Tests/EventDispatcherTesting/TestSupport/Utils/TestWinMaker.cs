using Moq;
using PowRxVar;
using UserEvents.Structs;

namespace UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;

class WinWrapper : IDisposable
{
	private readonly Disp d;
	public void Dispose() => d.Dispose();

	private readonly IRwTracker<NodeZ> nodes;
	private readonly List<NodeZ> list = new();

	public IMainWinUserEventsSupport Win { get; }

	public WinWrapper(IMainWinUserEventsSupport win, IRwTracker<NodeZ> nodes, Disp d)
	{
		this.d = d;
		Win = win;
		this.nodes = nodes;
	}

	public void AddNodes(NodeZ[] arr)
	{
		list.AddRange(arr);
		nodes.Update(list.ToArray());
	}

	public void DelNodes(NodeZ[] arr)
	{
		foreach (var elt in arr)
			list.Remove(elt);
		nodes.Update(list.ToArray());
	}
}

static class TestWinMaker
{
	public static WinWrapper Make(
		IObservable<IUserEvt> evt
	)
	{
		var d = new Disp();
		var winMock = new Mock<IMainWinUserEventsSupport>();

		var nodes = Tracker.Make<NodeZ>().D(d);

		winMock
			.Setup(e => e.Nodes)
			.Returns(nodes);

		winMock
			.Setup(e => e.Evt)
			.Returns(evt);

		//winMock
		//	.Setup(e => e.Invalidate());


		return new WinWrapper(winMock.Object, nodes, d);
	}
}
