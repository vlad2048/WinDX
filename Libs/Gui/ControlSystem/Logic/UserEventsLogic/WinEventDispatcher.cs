﻿using ControlSystem.Logic.PopupLogic;
using ControlSystem.Structs;
using ControlSystem.Utils;
using DynamicData;
using PowBasics.CollectionsExt;
using PowRxVar;
using UserEvents;
using IWin = UserEvents.IWinUserEventsSupport;
using INode = UserEvents.INodeStateUserEventsSupport;

namespace ControlSystem.Logic.UserEventsLogic;


sealed class WinEventDispatcher : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISourceCache<EventDispatcher, IWin> winsSrc;
	private readonly IObservableCache<EventDispatcher, IWin> wins;

	public WinEventDispatcher()
	{
		winsSrc = new SourceCache<EventDispatcher, IWin>(e => e.Win).D(d);
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
				upd.AddOrUpdate(new EventDispatcher(winAdd));
		});

		foreach (var partition in partitionSet.Partitions)
		{
			var win = winFun(partition.Id);
			var nodeTracker = wins.Lookup(win).Value;
			nodeTracker.Update(partition.AllNodeStates.OfType<INode>().ToArray());
		}
	}
}


static class WinEventDispatcherExts
{
	public static PartitionSet DispatchNodeEvents(this PartitionSet partitionSet, WinEventDispatcher winEventDispatcher, Func<NodeState?, IWin> winFun)
	{
		winEventDispatcher.DispatchNodeEvents(partitionSet, winFun);
		return partitionSet;
	}
}