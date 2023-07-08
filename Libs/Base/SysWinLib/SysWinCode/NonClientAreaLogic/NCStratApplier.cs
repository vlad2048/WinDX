using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

// ReSharper disable once CheckNamespace
namespace SysWinLib;

static class NCStratApplier
{
	public static IDisposable ApplyNCStrat(this SysWin win, INCStrat strat)
	{
		var d = new Disp();
		if (strat.HitTest == null) return d;

		// Without this, the custom NC area will not be displayed until the user resizes the window
		win.WhenMsg.WhenCREATE().Subscribe(_ =>
		{
			win.SetR(R.Empty, WindowPositionFlags.SWP_FRAMECHANGED | WindowPositionFlags.SWP_NOMOVE | WindowPositionFlags.SWP_NOSIZE);
		});

		win.WhenMsg.WhenNCCALCSIZE()
			.Skip(1)
			.Where(p => p.ShouldCalcValidRects)
			.Subscribe(p =>
			{
				p.MarkAsHandled();
			});

		win.WhenMsg.WhenNCHITTEST().Subscribe(e =>
		{
			e.Result = strat.HitTest(win.GetR(RType.WinWithGripAreas), e.Point.ToPt());
			e.MarkAsHandled();
		});

		return d;
	}
}