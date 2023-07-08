using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Utils;
using ControlSystem.WinSpectorLogic;
using ControlSystem.WinSpectorLogic.Utils;
using DynamicData;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using RenderLib;
using SysWinInterfaces;
using SysWinLib;
using SysWinLib.Structs;
using UserEvents;
using UserEvents.Generators;
using UserEvents.Structs;
using WinAPI.User32;
using WinAPI.Windows;
using IWin = UserEvents.IWinUserEventsSupport;

namespace ControlSystem.Logic.Popup_;

sealed class PopupWin : Ctrl, IWin
{
	private readonly Action invalidateAllAction;
	private readonly SysWin sysWin;
	private readonly IRwVar<R> layoutR;
	private readonly RxTracker<INodeStateUserEventsSupport> nodeTracker;
	private Partition subPartition;
	private Partition subPartitionRebased;

	// IWinUserEventsSupport
	// =====================
	public nint Handle => sysWin.Handle;
	public IObservable<IPacket> SysEvt => sysWin.WhenMsg;
	public IObservable<IUserEvt> SysWinEvt { get; }
	public Pt PopupOffset => layoutR.V.Pos;
	public IRoVar<Pt> ScreenPt => sysWin.ScreenPt;
	public IRoVar<R> ScreenR => sysWin.ScreenR;
	public IObservable<IChangeSet<INodeStateUserEventsSupport>> Nodes { get; }
	public INodeStateUserEventsSupport[] HitFun(Pt pt) => subPartition.FindNodesAtMouseCoordinates(pt);
	public void Invalidate() => invalidateAllAction();

	public void CallSysWinInvalidate() => sysWin.Invalidate();
	public ISysWinUserEventsSupport SysWin => sysWin;

	private static int cnt;

	public PopupWin(
		Partition subPartition,
		IWin mainWin,
		Action invalidateAllAction,
		nint winParentHandle,
		SpectorWinDrawState spectorDrawState
	)
	{
		nodeTracker = new RxTracker<INodeStateUserEventsSupport>().D(D);
		Nodes = nodeTracker.Items;

		this.invalidateAllAction = invalidateAllAction;
		layoutR = Var.Make(R.Empty).D(D);
		(this.subPartition, subPartitionRebased) = SetLayout(subPartition);
		sysWin = PopupWinUtils.MakeWin(layoutR.V, mainWin, winParentHandle).D(D);
		this.D(sysWin.D);
		SysWinEvt = UserEventGenerator.MakeForSysWin(sysWin.WhenMsg);


		var renderer = RendererGetter.Get(RendererType.GDIPlus, sysWin).D(D);

		Obs.Merge(
			mainWin.ScreenPt.ToUnit(),
			layoutR.ToUnit()
		)
			.Subscribe(_ =>
			{
				var r = layoutR.V + mainWin.ScreenPt.V;
				sysWin.SetR(r, 0);
			}).D(D);

		//Evt.Log($"Popup[{cnt++}]").D(D);

		sysWin.WhenMsg.WhenPAINT().Subscribe(_ =>
		{
			using var d = new Disp();
			var gfx = renderer.GetGfx(false).D(d);
			RenderUtils.RenderTree(subPartitionRebased, gfx);
			SpectorWinRenderUtils.Render(spectorDrawState, subPartitionRebased, gfx);
		}).D(D);
	}

	public (Partition, Partition) SetLayout(Partition subPartition_)
	{
		subPartition = subPartition_;
		(subPartitionRebased, layoutR.V) = subPartition_.SplitOffset();
		nodeTracker.Update(subPartition.NodeStates.OfType<INodeStateUserEventsSupport>().ToArray());
		Invalidate();
		return (subPartition, subPartitionRebased);
	}
}


file static class PopupWinUtils
{
	public static SysWin MakeWin(R layoutR, IWin mainWin, nint winParentHandle)
	{
		var win = new SysWin(e =>
		{
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
					WindowStyles.WS_CLIPSIBLINGS |
					//WindowStyles.WS_ |
					//WindowStyles.WS_CLIPCHILDREN |
					0,

				ExStyles =
					WindowExStyles.WS_EX_LEFT |
					WindowExStyles.WS_EX_LTRREADING |
					WindowExStyles.WS_EX_RIGHTSCROLLBAR |
					//WindowExStyles.WS_EX_ |
					//WindowExStyles.WS_EX_NOACTIVATE |
					0,

				Parent = winParentHandle,
			};
			// ReSharper disable once AccessToModifiedClosure
			e.NCStrat = NCStrats.Always(HitTestResult.HTCLIENT);
			//.Popup(pt => mainWin.ScreenR.V.Contains(pt));
		});
		win.Init();
		return win;
	}

	public static (Partition, R) SplitOffset(this Partition part)
	{
		var r = part.RMap[part.Id ?? throw new ArgumentException("Should not be null for a subpartition")];
		return (
			part with
			{
				RMap = part.RMap.MapValues(e => e - r.Pos),
				SysPartition = part.SysPartition with
				{
					RMap = part.SysPartition.RMap.MapValues(e => e - r.Pos),
				}
			},
			r
		);
	}
}