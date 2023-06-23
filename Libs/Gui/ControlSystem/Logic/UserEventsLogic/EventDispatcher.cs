using ControlSystem.Logic.PopupLogic;
using ControlSystem.Structs;
using ControlSystem.Utils;
using DynamicData;
using PowBasics.CollectionsExt;
using PowRxVar;
using UserEvents.Converters;
using UserEvents.Utils;
using IWin = ControlSystem.IWinUserEventsSupport;

namespace ControlSystem.Logic.UserEventsLogic;


sealed class NodeTracker : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public IWin Win { get; }
	private readonly ISourceList<NodeState> nodesSrc;
	private readonly IObservableList<NodeState> nodes;

	public NodeTracker(IWin win)
	{
		Win = win;
		nodesSrc = new SourceList<NodeState>().D(d);
		nodes = nodesSrc.AsObservableList().D(d);
		var nodesChanges = nodesSrc.Connect();

		UserEventConverter.MakeForNodes(Win.Evt, nodesChanges, Win.HitFun).D(d);
	}

	public void Update(NodeState[] nodeStates)
	{
		var (adds, dels) = nodes.GetAddDels(nodeStates);
		nodesSrc.Edit(upd =>
		{
			upd.RemoveMany(dels);
			upd.AddRange(adds);
		});
	}
}


sealed class EventDispatcher : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceCache<NodeTracker, IWin> winsSrc;
	private readonly IObservableCache<NodeTracker, IWin> wins;

	public EventDispatcher()
	{
		winsSrc = new SourceCache<NodeTracker, IWin>(e => e.Win).D(d);
		wins = winsSrc.AsObservableCache().D(d);
		winsSrc.Connect().DisposeMany().MakeHot(d);
	}

	public void DispatchNodeEvents(PartitionSet partitionSet, Func<NodeState?, IWin> winFun)
	{
		var winsNext = partitionSet.Partitions.SelectToArray(e => winFun(e.Id));
		var (winsAdd, winsDel) = wins.GetAddDels(winsNext);

		winsSrc.Edit(upd =>
		{
			upd.RemoveKeys(winsDel);
			foreach (var winAdd in winsAdd)
				upd.AddOrUpdate(new NodeTracker(winAdd));
		});

		foreach (var partition in partitionSet.Partitions)
		{
			var win = winFun(partition.Id);
			var nodeTracker = wins.Lookup(win).Value;
			nodeTracker.Update(partition.AllNodeStates);
		}
	}

	/*public void Update(
		Partition mainPartition,
		BoundPartition[] boundSubPartitions
	)
	{
		var boundPartitions = boundSubPartitions.Prepend(new BoundPartition(mainPartition, mainWin)).ToArray();
		var winsNext = boundPartitions.SelectToArray(e => e.Win);
		var (winsAdd, winsDel) = wins.GetAddDels(winsNext);

		winsSrc.Edit(upd =>
		{
			upd.RemoveKeys(winsDel);
			foreach (var winAdd in winsAdd)
				upd.AddOrUpdate(new NodeTracker(winAdd));
		});

		foreach (var boundPartition in boundPartitions)
		{
			var nodeTracker = wins.Lookup(boundPartition.Win).Value;
			nodeTracker.Update(boundPartition.Partition.AllNodeStates);
		}
	}*/
}


static class WinNodeEventDispatcherExts
{
	public static PartitionSet DispatchNodeEvents(this PartitionSet partitionSet, EventDispatcher eventDispatcher, Func<NodeState?, IWin> winFun)
	{
		eventDispatcher.DispatchNodeEvents(partitionSet, winFun);
		return partitionSet;
	}
}