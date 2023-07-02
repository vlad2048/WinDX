using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using PowBasics.CollectionsExt;
using PowRxVar;
using SysWinLib;
using IWin = UserEvents.IWinUserEventsSupport;
using INode = UserEvents.INodeStateUserEventsSupport;
using ControlSystem.Logic.Popup_.Structs;

namespace ControlSystem.Logic.Popup_;


sealed record BoundPartition(
	Partition Partition,
	IWin Win
);


sealed class PopupMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IWin parentWin;
	private readonly Action invalidateAllAction;
	private readonly SysWin parentSysWin;
	private readonly SpectorWinDrawState spectorDrawState;
	private readonly Dictionary<INode, PopupWin> map;

	public PopupMan(IWin parentWin, Action invalidateAllAction, SysWin parentSysWin, SpectorWinDrawState spectorDrawState)
	{
		this.parentWin = parentWin;
		this.invalidateAllAction = invalidateAllAction;
		this.parentSysWin = parentSysWin;
		this.spectorDrawState = spectorDrawState;
		map = new Dictionary<INode, PopupWin>().D(d);
	}

	public void InvalidatePopups() => map.Values.ForEach(e => e.CallSysWinInvalidate());

	public IWin GetWin(INode? nodeState) => nodeState switch
	{
		null => parentWin,
		not null => map[nodeState]
	};

	public PartitionSet CreatePopups(PartitionSet partitionSet)
	{
		var (partitionAdds, nodeStateDels, partitionComs) = map.GetAddDelsComs(partitionSet.SubPartitions, e => e.Id ?? throw new ArgumentException("Should not be null for a subpartition"));
		foreach (var nodeStateDel in nodeStateDels)
		{
			map[nodeStateDel].Dispose();
			map.Remove(nodeStateDel);
		}

		foreach (var partitionAdd in partitionAdds)
		{
			var partitionId = partitionAdd.Id ?? throw new ArgumentException("Should not be null for a subpartition");
			var parentNodeState = partitionSet.ParentMapping[partitionId];

			var popupParentHandle = parentNodeState switch
			{
				null => parentSysWin.Handle,
				not null => map[parentNodeState].Handle
			};
			map[partitionAdd.Id] = new PopupWin(
				partitionAdd,
				parentSysWin,
				invalidateAllAction,
				popupParentHandle,
				spectorDrawState
			);
		}

		foreach (var partitionCom in partitionComs)
		{
			var win = map[partitionCom.Id!];
			win.SetLayout(partitionCom);
		}

		return partitionSet;
	}
}



static class PopupManExts
{
	public static PartitionSet CreatePopups(this PartitionSet partitionSet, PopupMan popupMan) => popupMan.CreatePopups(partitionSet);
}
