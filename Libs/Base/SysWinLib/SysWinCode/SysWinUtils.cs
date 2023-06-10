using System.Reactive.Linq;
using System.Reflection.Metadata;
using PowBasics.Geom;
using PowRxVar;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;

// ReSharper disable once CheckNamespace
namespace SysWinLib;

static class SysWinUtils
{
	public static void SetupCustomNCAreaIFN(this SysWin win, SysWinOpt opt)
	{
		if (opt.NCStrat is not CustomNCStrat { HitTest: var hitTest }) return;

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
			e.Result = hitTest(win.GetR(RType.WinWithGripAreas), e.Point.ToPt());
			e.MarkAsHandled();
		});
	}

	public static void GenerateMouseLeaveMessagesIFN(this SysWin win, SysWinOpt opt)
	{
		if (!opt.GenerateMouseLeaveMessages) return;

		var isTracking = Var.Make(
			false,
			val => val
				.Select(v => v switch
				{
					false => win.WhenMsg.WhenMOUSEMOVE().Select(_ => true),
					true => win.WhenMsg.WhenMOUSELEAVE().Select(_ => false),
				})
				.Switch()
		).D(win.D);

		isTracking
			.Where(e => e)
			.Where(_ => win.Handle != nint.Zero)
			.Subscribe(_ =>
			{
				User32Helpers.TrackMouseEventGenerateLeaveMessage(win.Handle);
			}).D(win.D);
	}
}