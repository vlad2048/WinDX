using ControlSystem.Structs;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using PowBasics.CollectionsExt;
using PowRxVar;
using SysWinLib;
using IWin = ControlSystem.IWinUserEventsSupport;

namespace ControlSystem.Logic.PopupLogic;


sealed record BoundPartition(
	Partition Partition,
	IWinUserEventsSupport Win
);


sealed class PopupMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IWin parentWin;
	private readonly SysWin parentSysWin;
	private readonly SpectorWinDrawState spectorDrawState;
	private readonly Dictionary<NodeState, PopupWin> map;

	public PopupMan(IWin parentWin, SysWin parentSysWin, SpectorWinDrawState spectorDrawState)
	{
		this.parentWin = parentWin;
		this.parentSysWin = parentSysWin;
		this.spectorDrawState = spectorDrawState;
		map = new Dictionary<NodeState, PopupWin>().D(d);
	}

	public void InvalidatePopups() => map.Values.ForEach(e => e.Invalidate());

	public IWin GetWin(NodeState? nodeState) => nodeState switch
	{
		null => parentWin,
		not null => map[nodeState]
	};

	public PartitionSet CreatePopups(PartitionSet partitionSet)
	{
		var (partitionAdds, nodeStateDels) = map.GetAddDels(partitionSet.SubPartitions, e => e.Id ?? throw new ArgumentException("Should not be null for a subpartition"));
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
				popupParentHandle,
				spectorDrawState
			);
		}

		return partitionSet;
	}

	/*public BoundPartition[] ShowSubPartitions(
		SubPartition[] subPartitions,
		IReadOnlyDictionary<int, int?> parentMapping
	)
	{
		var boundSubPartitions = new List<BoundPartition>();

		var handles = new List<nint>();
		for (var layoutIdx = 0; layoutIdx < subPartitions.Length; layoutIdx++)
		{
			var subPartition = subPartitions[layoutIdx];
			if (!map.TryGetValue(subPartition.Id, out var popupWin))
			{
				var parentIdx = parentMapping[layoutIdx];
				var winParentHandle = parentIdx switch
				{
					null => parentWin.Handle,
					not null => handles[parentIdx.Value]
				};
				popupWin = map[subPartition.Id] = new PopupWin(
					subPartition,
					parentWin,
					winParentHandle,
					spectorDrawState
				);
			}
			else
			{
				popupWin.Invalidate();
			}

			handles.Add(popupWin.Handle);
			boundSubPartitions.Add(new BoundPartition(subPartition, popupWin));
		}

		return boundSubPartitions.ToArray();
	}*/
}



static class PopupManExts
{
	public static PartitionSet CreatePopups(this PartitionSet partitionSet, PopupMan popupMan) => popupMan.CreatePopups(partitionSet);
}
