using DynamicData;
using PowRxVar;
using UserEvents.Converters;

namespace UserEvents;

public sealed class EventDispatcher : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public EventDispatcher(
		IWin win,
		IMainWin mainWin
	)
	{
		UserEventConverter.MakeForNodes(win.Nodes, mainWin.Evt, mainWin.HitFun).D(d);

		win.Nodes
			.MergeMany(e => e.WhenInvalidateRequired)
			.Subscribe(_ => mainWin.Invalidate()).D(d);
	}

	//public void Update(INode[] nodeStates) => nodesSrc.EditDiff(nodeStates);
}