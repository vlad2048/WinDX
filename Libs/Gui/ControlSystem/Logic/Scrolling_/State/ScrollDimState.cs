using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Scrolling_.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowBasics.MathCode;
using PowRxVar;

namespace ControlSystem.Logic.Scrolling_.State;


file static class ScrollConsts
{
	public const int ScrollUnit = 16;
	public const int WheelMult = 3;
	public static int Cap(this int v, int max) => Math.Max(0, Math.Min(v, max));
}


public sealed class ScrollDimState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private sealed record St(
		bool Enabled,
		bool Visible,
		int ViewLng,
		int ContLng,
		int TrakLng,
		int ScrollOfs,
		bool IsTailing
	);

	private readonly ISubject<Unit> whenChanged;
	private readonly ISubject<IScrollCmd> whenCmd;
	private readonly ISubject<Unit> whenInvalidateRequired;
	private void TriggerCmd(IScrollCmd cmd) => whenCmd.OnNext(cmd);
	private void TriggerChanged() => whenChanged.OnNext(Unit.Default);
	private void TriggerInvalidate() => whenInvalidateRequired.OnNext(Unit.Default);
	private St GetSt() => new(
		Enabled,
		Visible,
		ViewLng,
		ContLng,
		TrakLng,
		ScrollOfs,
		IsTailing
	);


	// Has anything changed as a result of Layout or a UserOperation (for WinSpector)
	internal IObservable<Unit> WhenChanged => whenChanged.AsObservable();
	internal IObservable<IScrollCmd> WhenCmd => whenCmd.AsObservable();

	// Derived quantities
	internal int MaxScrollOfs => Math.Max(0, ContLng - ViewLng);
	internal int TrakLng => Math.Max(0, ViewLng - 2 * FlexFlags.ScrollBarCrossDims.Width); // TODO: HACK
	internal int PageSize => MathUtils.FillingDiv(ViewLng, ScrollConsts.ScrollUnit);


	public Dir Dir { get; }
	public bool Enabled { get; private set; }
	public bool Visible { get; private set; }
	// Size of the scroll node minus the thickness of the orthogonal scrollbar if visible
	public int ViewLng { get; private set; }
	// Size of the content
	public int ContLng { get; private set; }
	public int ScrollOfs { get; private set; }
	public bool IsTailing { get; private set; }

	// Do we need a repaint as a result of a UserOperation
	public IObservable<Unit> WhenInvalidateRequired => whenInvalidateRequired.AsObservable();

	internal ScrollDimState(Dir dir)
	{
		Dir = dir;
		whenCmd = new Subject<IScrollCmd>().D(d);
		whenChanged = new Subject<Unit>().D(d);
		whenInvalidateRequired = new Subject<Unit>().D(d);
	}

	internal void UpdateFromLayout(
		bool enabled,
		bool visible,
		int viewLng,
		int contLng
	)
	{
		var st = GetSt();

		Enabled = enabled;
		Visible = visible;

		if (!Enabled)
		{
			ViewLng = ContLng = ScrollOfs = 0;
			IsTailing = false;
		}
		else
		{
			ViewLng = viewLng;
			ContLng = contLng;
			CapScrollOfs();
		}

		if (GetSt() != st) TriggerChanged();
	}

	internal bool CanRunScrollCmd(IScrollCmd cmd) => cmd.DecInc switch
	{
		ScrollBtnDecInc.Dec => ScrollOfs > 0,
		ScrollBtnDecInc.Inc => ScrollOfs < MaxScrollOfs
	};

	internal void RunScrollCmd(IScrollCmd cmd)
	{
		if (!CanRunScrollCmd(cmd)) return;
		TriggerCmd(cmd);

		var st = GetSt();

		var delta = GetCmdDelta(cmd);
		ScrollOfs += delta;
		CapScrollOfs();

		if (GetSt() != st)
		{
			TriggerChanged();
			TriggerInvalidate();
		}
	}

	internal int GetCmdDelta(IScrollCmd cmd) => cmd.GetSign() * cmd switch
	{
		UnitScrollCmd => ScrollConsts.ScrollUnit,
		WheelScrollCmd => ScrollConsts.ScrollUnit * ScrollConsts.WheelMult,
		PageScrollCmd => PageSize,
		_ => throw new ArgumentException("Impossible")
	};


	private void CapScrollOfs() => ScrollOfs = ScrollOfs.Cap(MaxScrollOfs);
}
