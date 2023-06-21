using System.Drawing;
using ControlSystem.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using RenderLib;
using RenderLib.Structs;
using SysWinLib;
using SysWinLib.Structs;
using TreePusherLib;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem.Logic.PopLogic;

sealed class SlaveWin : Ctrl
{
	private static readonly BrushDef brush = new SolidBrushDef(Color.DodgerBlue);
	private readonly SysWin sysWin;
	private readonly IRwVar<R> layoutR;
	private SubPartition layout;

	public SlaveWin(SysWin parentWin, SubPartition layoutUnoffset)
	{
		(layout, var layR) = layoutUnoffset.SplitOffset();
		layoutR = Var.Make(layR).D(D);
		sysWin = SlaveWinUtils.MakeWin(parentWin, layoutR.V).D(D);
		this.D(sysWin.D);

		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);

		Obs.Merge(
			parentWin.ScreenPt.ToUnit(),
			layoutR.ToUnit()
		)
			.Subscribe(_ =>
			{
				var r = layoutR.V + parentWin.ScreenPt.V;
				sysWin.SetR(r, 0);
			}).D(D);

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ => RenderUtils.RenderTree(layout, renderer)).D(D);
	}

	public void SetLayout(SubPartition layout_) => (layout, layoutR.V) = layout_.SplitOffset();

	public void Invalidate() => sysWin.Invalidate();
}


file static class SlaveWinUtils
{
	private const int DEFAULT = (int)CreateWindowFlags.CW_USEDEFAULT;

	public static SysWin MakeWin(SysWin parentWin, R layoutR)
	{
		var win = new SysWin(e =>
		{
			e.CreateWindowParams = new CreateWindowParams
			{
				Name = "Slave",
				X = parentWin.ScreenPt.V.X + layoutR.X,
				Y = parentWin.ScreenPt.V.Y + layoutR.Y,
				Width = layoutR.Width,
				Height = layoutR.Height,

				Styles =
					WindowStyles.WS_VISIBLE |
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

				Parent = parentWin.Handle,
			};
		});
		win.Init();
		return win;
	}

	public static (SubPartition, R) SplitOffset(this SubPartition part)
	{
		var r = part.RMap[part.Id];
		return (
			part with { RMap = part.RMap.MapValues(e => e - r.Pos) },
			r
		);
	}
}