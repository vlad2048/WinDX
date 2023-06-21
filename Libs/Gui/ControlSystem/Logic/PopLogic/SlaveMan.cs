using ControlSystem.Structs;
using ControlSystem.WinSpectorLogic;
using PowRxVar;
using SysWinLib;

namespace ControlSystem.Logic.PopLogic;

class SlaveMan : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly SysWin parentWin;
	private readonly Dictionary<NodeState, SlaveWin> map;

	public SlaveMan(SysWin parentWin)
	{
		this.parentWin = parentWin;
		map = new Dictionary<NodeState, SlaveWin>().D(d);
	}

	public void ShowSubPartitions(
		SubPartition[] layouts,
		IReadOnlyDictionary<int, int?> parentMapping,
		nint mainWinHandle,
		SpectorWinDrawState spectorDrawState
	)
	{
		var handles = new List<nint>();

		for (var layoutIdx = 0; layoutIdx < layouts.Length; layoutIdx++)
		{
			var layout = layouts[layoutIdx];
			if (!map.TryGetValue(layout.Id, out var slaveWin))
			{
				var parentIdx = parentMapping[layoutIdx];
				var winParentHandle = parentIdx switch
				{
					null => mainWinHandle,
					not null => handles[parentIdx.Value]
				};
				slaveWin = map[layout.Id] = new SlaveWin(
					layout,
					parentWin,
					winParentHandle,
					spectorDrawState
				);
			}
			else
			{
				slaveWin.Invalidate();
			}

			handles.Add(slaveWin.Handle);
		}
	}
}