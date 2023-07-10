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

    public IRwMayVar<MixNode> SelNode { get; }
    public IRwMayVar<MixNode> HovNode { get; }
    public IRwMayVar<INode> LockedNode { get; }
    public IObservable<Unit> WhenChanged { get; }

    public void SetRenderCountToLog(int cnt) => subMap.Items.ForEach(e => e.RenderCountToLog = cnt);

    public bool ShouldLogRender(IWin win)
    {
	    var sub = subMap.Lookup(win).Value;
	    return sub.RenderCountToLog-- > 0;
    }

    public SpectorWinDrawState(IRoTracker<IWin> wins)
    {
        SelNode = VarMay.Make<MixNode>().D(d);
        HovNode = VarMay.Make<MixNode>().D(d);
        LockedNode = VarMay.Make<INode>().D(d);
        WhenChanged = Obs.Merge(
	        SelNode.Skip(1).ToUnit(),
            HovNode.Skip(1).ToUnit(),
            LockedNode.Skip(1).ToUnit()
        );

        subMap = wins
	        .Items
	        .Transform(e => new SpectorSubWinDrawState(e))
	        .AddKey(e => e.Win)
	        .AsObservableCache().D(d);
    }
}
