using SysWinLib.Structs;
using SysWinLib.Utils;

// ReSharper disable once CheckNamespace
namespace SysWinLib;


public interface INCStrat { }

public sealed class NoneNCStrat : INCStrat { }

public sealed class CustomNCStrat : INCStrat
{
	public NCHitTestDelegate HitTest { get; set; } = NcHitTestUtils.MakeCustom();
}

public sealed class PopupNCStrat : INCStrat
{
	public NCHitTestDelegate HitTest { get; set; } = NcHitTestUtils.MakePopup();
}
