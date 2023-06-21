using ControlSystem.Structs;
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

	public void ShowSubPartitions(SubPartition[] layouts)
	{
		foreach (var layout in layouts)
		{
			if (!map.TryGetValue(layout.Id, out var slaveWin))
			{
				slaveWin = map[layout.Id] = new SlaveWin(parentWin, layout);
			}
			else
			{
				slaveWin.Invalidate();
			}
		}
	}
}