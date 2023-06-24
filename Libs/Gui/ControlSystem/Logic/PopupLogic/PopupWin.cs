using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using RenderLib;
using SysWinLib;
using SysWinLib.Structs;
using UserEvents;
using UserEvents.Generators;
using UserEvents.Structs;
using UserEvents.Utils;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem.Logic.PopupLogic;

sealed class PopupWin : Ctrl, IWinUserEventsSupport
{
	private readonly SysWin sysWin;
	private readonly IRwVar<R> layoutR;
	private Partition subPartition;
	private Partition subPartitionRebased;

	public nint Handle => sysWin.Handle;

	// IWinUserEventsSupport
	// =====================
	public IUIEvt Evt { get; }
	public Maybe<INodeStateUserEventsSupport> HitFun(Pt pt) => subPartition.FindNodeAtMouseCoordinates(pt);


	public PopupWin(
		Partition subPartition,
		SysWin parentWin,
		nint winParentHandle,
		SpectorWinDrawState spectorDrawState
	)
	{
		this.subPartition = subPartition;
		(subPartitionRebased, var layR) = subPartition.SplitOffset();
		layoutR = Var.Make(layR).D(D);
		sysWin = PopupWinUtils.MakeWin(layoutR.V, parentWin, winParentHandle).D(D);
		this.D(sysWin.D);
		Evt = UserEventGenerator.MakeForWin(sysWin).Translate(() => layoutR.V.Pos);

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

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var gfx = renderer.GetGfx().D(d);
			RenderUtils.RenderTree(subPartitionRebased, gfx);
			SpectorWinRenderUtils.Render(spectorDrawState, subPartitionRebased, gfx);
		}).D(D);
	}

	public void SetLayout(Partition subPartition_)
	{
		subPartition = subPartition_;
		(subPartitionRebased, layoutR.V) = subPartition_.SplitOffset();
	}

	public void Invalidate() => sysWin.Invalidate();
}


file static class PopupWinUtils
{
	public static SysWin MakeWin(R layoutR, SysWin parentWin, nint winParentHandle)
	{
		var win = new SysWin(e =>
		{
			e.CreateWindowParams = new CreateWindowParams
			{
				Name = "Popup",
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

				Parent = winParentHandle,
			};
		});
		win.Init();
		return win;
	}

	public static (Partition, R) SplitOffset(this Partition part)
	{
		var r = part.RMap[part.Id ?? throw new ArgumentException("Should not be null for a subpartition")];
		return (
			part with { RMap = part.RMap.MapValues(e => e - r.Pos) },
			r
		);
	}
}