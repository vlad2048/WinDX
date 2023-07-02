using DynamicData;
using PowRxVar;
using UserEvents.Converters;
using IWin = UserEvents.IWinUserEventsSupport;
using INode = UserEvents.INodeStateUserEventsSupport;

namespace UserEvents;

public sealed class EventDispatcher : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public IWin Win { get; }
	private readonly ISourceList<INode> nodesSrc;

	public EventDispatcher(IWin win)
	{
		Win = win;
		nodesSrc = new SourceList<INode>().D(d);
		var nodesChanges = nodesSrc.Connect().MakeHot(d);

		UserEventConverter.MakeForNodes(Win.Evt, nodesChanges, Win.HitFun).D(d);

		nodesChanges
			.MergeMany(e => e.WhenInvalidateRequired)
			.Subscribe(_ => Win.Invalidate()).D(d);
	}

	public void Update(INode[] nodeStates) => nodesSrc.EditDiff(nodeStates);
}