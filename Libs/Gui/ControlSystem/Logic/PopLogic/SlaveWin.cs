using System.Drawing;
using PowRxVar;
using RenderLib;
using RenderLib.Structs;
using SysWinLib;
using SysWinLib.Structs;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem.Logic.PopLogic;

sealed class SlaveWin : Ctrl
{
	private static readonly BrushDef brush = new SolidBrushDef(Color.DodgerBlue);
	private readonly SysWin sysWin;

	public IntPtr Handle => sysWin.Handle;

	public SlaveWin(IntPtr parent)
	{
		sysWin = SlaveWinUtils.MakeWin(parent).D(D);
		this.D(sysWin.D);

		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);

		sysWin.WhenMsg.WhenPAINT().Subscribe(e =>
		{
			using var gfx = renderer.GetGfx();

			var r = sysWin.ClientR.V;
			gfx.FillR(r, brush);
		}).D(D);
	}
}


file static class SlaveWinUtils
{
	private const int DEFAULT = (int)CreateWindowFlags.CW_USEDEFAULT;

	public static SysWin MakeWin(IntPtr parent)
	{
		var win = new SysWin(e =>
		{
			e.CreateWindowParams = new CreateWindowParams
			{
				Name = "Slave",
				X = 200,
				Y = 50,
				Width = 500,
				Height = 300,

				Styles =
					//WindowStyles.WS_VISIBLE |
					WindowStyles.WS_POPUP |
					WindowStyles.WS_CLIPSIBLINGS |
					WindowStyles.WS_CLIPCHILDREN |
					0,

				ExStyles =
					WindowExStyles.WS_EX_LEFT |
					WindowExStyles.WS_EX_LTRREADING |
					WindowExStyles.WS_EX_RIGHTSCROLLBAR |
					WindowExStyles.WS_EX_NOACTIVATE |
					0,

				Parent = parent,
			};
		});
		//win.Init();
		return win;
	}
}