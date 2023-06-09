using System.Reactive.Linq;
using PowBasics.Geom;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

// ReSharper disable once CheckNamespace
namespace SysWinLib;

static class SysWinUtils
{
	public static void SetupCustomNCAreaIFN(this SysWin sysWin, SysWinOpt opt)
	{
		if (opt.NCStrat is not CustomNCStrat { HitTest: var hitTest }) return;

		// Without this, the custom NC area will not be displayed until the user resizes the window
		sysWin.WhenMsg.WhenCREATE().Subscribe(_ =>
		{
			sysWin.SetR(R.Empty, WindowPositionFlags.SWP_FRAMECHANGED | WindowPositionFlags.SWP_NOMOVE | WindowPositionFlags.SWP_NOSIZE);
		});

		sysWin.WhenMsg.WhenNCCALCSIZE()
			.Skip(1)
			.Where(p => p.ShouldCalcValidRects)
			.Subscribe(p =>
			{
				p.MarkAsHandled();
			});

		sysWin.WhenMsg.WhenNCHITTEST().Subscribe(e =>
		{
			e.Result = hitTest(sysWin.GetR(RType.WinWithGripAreas), e.Point.ToPt());
			e.MarkAsHandled();
		});
	}
}