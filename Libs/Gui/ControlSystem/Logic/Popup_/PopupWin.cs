using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Rendering_;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Utils;
using PowBasics.Geom;
using PowRxVar;
using SysWinInterfaces;
using SysWinLib;
using SysWinLib.Defaults;
using SysWinLib.Structs;
using UserEvents;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Windows;

namespace ControlSystem.Logic.Popup_;

sealed class PopupWin : Ctrl, IWin
{
	private readonly SysWin sysWin;
	private readonly IRwTracker<NodeZ> rwNodes;
	private readonly IRwTracker<ICtrl> rwCtrls;
	private Partition subPartition = null!;
	private readonly IRwVar<R> layoutR = null!;

	// IWinUserEventsSupport
	// =====================
	public INode Id => subPartition.NodeStateId ?? throw new ArgumentException("A SubPartition should always have a NodeStateId");
	public nint Handle => sysWin.Handle;
	public IObservable<IPacket> SysEvt => sysWin.WhenMsg;
	public Pt PopupOffset => layoutR.V.Pos;
	public IRoVar<Pt> ScreenPt => sysWin.ScreenPt;
	public IRoVar<R> ScreenR => sysWin.ScreenR;
	public IRoVar<Sz> ClientSz => sysWin.ClientSz;
	public IRoTracker<NodeZ> Nodes => rwNodes;
	public IRoTracker<ICtrl> Ctrls => rwCtrls;
	public void SysInvalidate() => sysWin.Invalidate();

	public ISysWinUserEventsSupport SysWin => sysWin;


	public PopupWin(
		Partition subPartition_,
		IWin mainWin,
		nint winParentHandle,
		SpectorWinDrawState spectorDrawState
	)
	{
		rwNodes = Tracker.Make<NodeZ>().D(D);
		rwCtrls = Tracker.Make<ICtrl>().D(D);

		layoutR = Var.Make(R.Empty).D(D);
		SetLayout(subPartition_);
		sysWin = PopupWinUtils.MakeWin(layoutR.V, mainWin, winParentHandle).D(D);
		this.D(sysWin.D);


		var rendererSwitcher = new RendererSwitcher(sysWin, null).D(D);


		Obs.Merge(
			mainWin.ScreenPt.ToUnit(),
			layoutR.ToUnit()
		)
			.Subscribe(_ =>
			{
				var r = layoutR.V + mainWin.ScreenPt.V;
				sysWin.SetR(r, 0);
			}).D(D);

		
		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var gfx = rendererSwitcher.Renderer.GetGfx(false).D(d);
			RenderUtils.RenderTree(
				subPartition,
				gfx,
				-layoutR.V.Pos,
				spectorDrawState.ShouldLogRender(this)
			);
			SpectorWinRenderUtils.Render(spectorDrawState, subPartition, gfx, -layoutR.V.Pos);
		}).D(D);
	}

	public void SetLayout(Partition subPartition_)
	{
		subPartition = subPartition_;
		layoutR.V = subPartition.Set.RMap[subPartition.NodeStateId ?? throw new ArgumentException("Should not be null for a subpartition")];
		(rwNodes, rwCtrls).UpdateFromPartition(subPartition);
	}
}


file static class PopupWinUtils
{
	public static SysWin MakeWin(R layoutR, IWin mainWin, nint winParentHandle)
	{
		var win = new SysWin(e =>
		{
			e.WinClass = WinClasses.PopupWindow;
			e.CreateWindowParams = new CreateWindowParams
			{
				Name = "Popup",
				X = mainWin.ScreenR.V.X + layoutR.X,
				Y = mainWin.ScreenR.V.Y + layoutR.Y,
				Width = layoutR.Width,
				Height = layoutR.Height,

				Styles =
					WindowStyles.WS_VISIBLE |
					WindowStyles.WS_POPUP |
					//WindowStyles.WS_CLIPSIBLINGS |
					//WindowStyles.WS_ |
					0,

				ExStyles =
					WindowExStyles.WS_EX_LEFT |
					WindowExStyles.WS_EX_LTRREADING |
					WindowExStyles.WS_EX_RIGHTSCROLLBAR |
					//WindowExStyles.WS_EX_ |
					0,

				Parent = winParentHandle,
			};
			e.NCStrat = NCStrats.Always(HitTestResult.HTCLIENT);
		});
		win.Init();
		return win;
	}
}