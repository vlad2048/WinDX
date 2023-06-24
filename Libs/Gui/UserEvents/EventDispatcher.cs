using DynamicData;
using PowRxVar;
using UserEvents.Converters;
using UserEvents.Utils;
using IWin = UserEvents.IWinUserEventsSupport;
using INode = UserEvents.INodeStateUserEventsSupport;

namespace UserEvents;

public sealed class EventDispatcher : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public IWin Win { get; }
	private readonly ISourceList<INode> nodesSrc;
	private readonly IObservableList<INode> nodes;

	public EventDispatcher(IWin win)
	{
		Win = win;
		nodesSrc = new SourceList<INode>().D(d);
		nodes = nodesSrc.AsObservableList().D(d);
		var nodesChanges = nodesSrc.Connect();

		UserEventConverter.MakeForNodes(Win.Evt, nodesChanges, Win.HitFun).D(d);
	}

	public void Update(INode[] nodeStates)
	{
		var (adds, dels) = nodes.GetAddDels(nodeStates);
		nodesSrc.Edit(upd =>
		{
			upd.RemoveMany(dels);
			upd.AddRange(adds);
		});
	}
}