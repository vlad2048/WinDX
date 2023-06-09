using SysWinLib.Structs;
using SysWinLib.Utils;

// ReSharper disable once CheckNamespace
namespace SysWinLib;


public interface INCStrat { }

public class NoneNCStrat : INCStrat { }

public class CustomNCStrat : INCStrat
{
	public NCHitTestDelegate HitTest { get; set; } = NcHitTestUtils.Make();
}
