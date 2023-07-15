using System.Reactive.Disposables;
using System.Reactive.Linq;
using ControlSystem.WinSpectorLogic;
using PowRxVar;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using DynamicData;
using PowBasics.CollectionsExt;
using UserEvents;
using PowMaybe;

namespace ControlSystem.Logic.Popup_;



static class PopupMaker
{
	public static (Func<NodeState?, IWin>, IDisposable) MakePopups(
		this IRoMayVar<PartitionSet> partitionSet,
		IRwTracker<IWin> wins,
		IWin mainWin,
		SpectorWinDrawState spectorWinDrawState
	)
	{
		var d = new Disp();
		wins.Src.Add(mainWin);

		var partsSet = new HashSet<Partition>();
		var winMap = new Dictionary<NodeState, PopupWin>();

		PopupWin MakeWin(Partition partition)
		{
			//if (partition.NodeStateId == null) return mainWin;
			if (partition.NodeStateId == null) throw new ArgumentException("Impossible");
			var parentHandle = partition.ParentNodeStateId switch
			{
				null => mainWin.Handle,
				not null => winMap[partition.ParentNodeStateId].Handle
			};
			var win = new PopupWin(
				partition,
				mainWin,
				parentHandle,
				spectorWinDrawState
			);
			winMap[partition.NodeStateId!] = win;
			partsSet.Add(partition);
			Disposable.Create(() =>
			{
				partsSet.Remove(partition);
				winMap.Remove(partition.NodeStateId);
			}).D(win.D);
			return win;
		}


		var partsVar = Var.Make(
			Array.Empty<Partition>(),
			partitionSet
				.Select(mayPartSet => mayPartSet.IsSome(out var partSet) switch
				{
					truer => partSet.PopupPartitions,
					false => Array.Empty<Partition>()
				})
		).D(d);



		// var partsSet = new HashSet<Partition>();
		// var winMap = new Dictionary<NodeState, IWin>();

		partsVar.Subscribe(parts =>
		{
			var partsDel = partsSet.WhereNotToArray(parts.Contains);
			var partsAdd = parts.WhereNotToArray(partsSet.Contains);
			var partsRef = parts.WhereToArray(partsSet.Contains);

			foreach (var partDel in partsDel)
			{
				var win = winMap[partDel.NodeStateId!];
				wins.Src.Remove(win);
				win.Dispose();
			}

			foreach (var partAdd in partsAdd)
			{
				var win = MakeWin(partAdd);
				wins.Src.Add(win);
			}

			foreach (var partRef in partsRef)
			{
				var win = winMap[partRef.NodeStateId!];
				win.SetLayout(partRef);
			}

		}).D(d);


		return (
			nodeState => nodeState switch
			{
				not null => winMap[nodeState],
				null => mainWin,
			},
			d
		);




		/*var partsObs = partitionSet
			.SelectMany(mayPartSet => mayPartSet.IsSome(out var partSet) switch
			{
				truer => partSet.PopupPartitions,
				false => Array.Empty<Partition>()
			});

		partsObs.Where(e => !partsSet.Contains(e)).Subscribe(part =>
		{
			var win = MakeWin(part);
			wins.Src.Add(win);
		})*/





		/*var winMap = new Dictionary<NodeState, IWin>();

		// TODO: Refresh to call PopupWin.SetLayout
		partitionSet
			.Select(mayPartSet => mayPartSet.IsSome(out var partSet) switch
			{
				truer => partSet.PopupPartitions,
				false => Array.Empty<Partition>()
			})
			.ToTracker().D(d).Items
			.OnItemRemoved(part => winMap.Remove(part.NodeStateId ?? throw new ArgumentException("Impossible")))
			.Transform(partition =>
			{
				if (partition.NodeStateId == null) return mainWin;
				var parentHandle = partition.ParentNodeStateId switch
				{
					null => mainWin.Handle,
					not null => winMap[partition.ParentNodeStateId].Handle
				};
				var win = new PopupWin(
					partition,
					mainWin,
					parentHandle,
					spectorWinDrawState
				);
				winMap[partition.NodeStateId!] = win;
				return win;
			})
			.DisposeMany()
			.PopulateInto(wins.Src).D(d);

		return (
			nodeState => nodeState switch
			{
				not null => winMap[nodeState],
				null => mainWin,
			},
			d
		);*/
	}
}


/*
sealed class PopupMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly IWin mainWin;
	private readonly SpectorWinDrawState spectorDrawState;
	private readonly IRwTracker<NodeState> subPartitions;

	public IRoTracker<IWin> Wins { get; }

	public PopupMan(IWin mainWin, SpectorWinDrawState spectorDrawState)
	{
		this.mainWin = mainWin;
		this.spectorDrawState = spectorDrawState;
		subPartitions = Tracker.Make<NodeState>().D(d);

	}
}
*/




/*
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
*/