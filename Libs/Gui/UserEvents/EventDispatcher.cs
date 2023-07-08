using DynamicData;
using PowRxVar;
using UserEvents.Converters;
using UserEvents.Structs;

namespace UserEvents;

public static class EventDispatcher
{
	public static IDisposable DispatchEvents(this IRoTracker<NodeZ> nodes, IMainWin mainWin)
	{
		var d = new Disp();

		UserEventConverter.MakeForNodes(
			nodes,
			mainWin.Evt
		).D(d);

		nodes.Items
			.MergeMany(e => e.Node.WhenInvalidateRequired)
			.Subscribe(_ => mainWin.Invalidate()).D(d);

		return d;
	}


	/*public static IDisposable DispatchEvents(this IMainWin mainWin, IWin win)
	{
		var d = new Disp();
		UserEventConverter.MakeForNodes(win.Nodes, mainWin.Evt).D(d);

		win.Nodes.Items
			.MergeMany(e => e.WhenInvalidateRequired)
			.Subscribe(_ => mainWin.Invalidate()).D(d);

		return d;
	}*/
}
