using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using PowBasics.CollectionsExt;
using PowRxVar;
using ControlSystem.Logic.Popup_.Structs;
using UserEvents;

namespace ControlSystem.Logic.Popup_;


sealed record BoundPartition(
	Partition Partition,
	IWin Win
);


sealed class PopupMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IWin mainWin;
	private readonly SpectorWinDrawState spectorDrawState;
	private readonly Dictionary<INode, PopupWin> map;
	private readonly IRwTracker<IWin> rwWins;

	public PopupMan(IWin mainWin, IRwTracker<IWin> rwWins, SpectorWinDrawState spectorDrawState)
	{
		this.mainWin = mainWin;
		this.rwWins = rwWins;
		this.spectorDrawState = spectorDrawState;
		map = new Dictionary<INode, PopupWin>().D(d);
	}

	public IWin GetWin(INode? nodeState) => nodeState switch
	{
		null => mainWin,
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
				null => mainWin.Handle,
				not null => map[parentNodeState].Handle
			};
			map[partitionAdd.Id] = new PopupWin(
				partitionAdd,
				mainWin,
				popupParentHandle,
				spectorDrawState
			);
		}

		foreach (var partitionCom in partitionComs)
		{
			var win = map[partitionCom.Id!];
			win.SetLayout(partitionCom);
		}

		rwWins.Update(map.Values.Prepend(mainWin).ToArray());

		return partitionSet;
	}
}



static class PopupManExts
{
	public static PartitionSet CreatePopups(this PartitionSet partitionSet, PopupMan popupMan) => popupMan.CreatePopups(partitionSet);
}
