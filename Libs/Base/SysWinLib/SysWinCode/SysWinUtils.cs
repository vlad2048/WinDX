using System.Reactive.Linq;
using PowRxVar;
using WinAPI.User32;
using WinAPI.Windows;

// ReSharper disable once CheckNamespace
namespace SysWinLib;

static class SysWinUtils
{
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