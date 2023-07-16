using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using PowBasics.CollectionsExt;
using PowRxVar;
using UserEvents;

namespace ControlSystem.WinSpectorLogic;

sealed class SpectorSubWinDrawState
{
	public IWin Win { get; }
	public int RenderCountToLog { get; set; }
	public SpectorSubWinDrawState(IWinUserEventsSupport win)
	{
		Win = win;
	}
}

sealed class SpectorWinDrawState : IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    private readonly IObservableCache<SpectorSubWinDrawState, IWin> subMap;

    public IRwMayVar<MixNode> NodeTreeSel { get; }
    public IRwMayVar<MixNode> NodeTreeHov { get; }
    public IRwMayVar<INode> NodeLock { get; }
    public IRwMayVar<INode> NodeHovr { get; }
    public IObservable<Unit> WhenChanged { get; }

    public void SetRenderCountToLog(int cnt) => subMap.Items.ForEach(e => e.RenderCountToLog = cnt);

    public bool ShouldLogRender(IWin win)
    {
	    var sub = subMap.Lookup(win).Value;
	    return sub.RenderCountToLog-- > 0;
    }

    public SpectorWinDrawState(IRoTracker<IWin> wins)
    {
        NodeTreeSel = VarMay.Make<MixNode>().D(d);
        NodeTreeHov = VarMay.Make<MixNode>().D(d);
        NodeLock = VarMay.Make<INode>().D(d);
        NodeHovr = VarMay.Make<INode>().D(d);

        WhenChanged =
	        Obs.Merge(
		        NodeTreeSel.Skip(1).ToUnit(),
	            NodeTreeHov.Skip(1).ToUnit(),
	            NodeLock.Skip(1).ToUnit(),
		        NodeHovr.Skip(1).ToUnit()
	        )
		        .Where(_ => !Cfg.V.Tweaks.DisableWinSpectorDrawing);

        subMap = wins
	        .Items
	        .Transform(e => new SpectorSubWinDrawState(e))
	        .AddKey(e => e.Win)
	        .AsObservableCache().D(d);
    }
}
