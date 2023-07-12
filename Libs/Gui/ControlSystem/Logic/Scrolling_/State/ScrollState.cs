using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Structs.Enum;
using ControlSystem.Utils;
using PowBasics.Geom;
using PowBasics.MathCode;
using PowRxVar;
using C = ControlSystem.Logic.Scrolling_.ScrollBarConsts;
using St = ControlSystem.Logic.Scrolling_.Structs.Enum.ScrollBarState;

namespace ControlSystem.Logic.Scrolling_.State;


public sealed class ScrollState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	// ReSharper disable NotAccessedPositionalProperty.Local
	private sealed record StateRec(
		St State,
		int ViewLng,
		int ContLng,
		int ScrollOfs
	);
	// ReSharper restore NotAccessedPositionalProperty.Local

	private readonly ISubject<Unit> whenChanged;
	private readonly ISubject<IScrollCmd> whenCmd;
	private readonly ISubject<Unit> whenInvalidateRequired;
	private void TriggerCmd(IScrollCmd cmd) => whenCmd.OnNext(cmd);
	private void TriggerChanged() => whenChanged.OnNext(Unit.Default);
	private void TriggerInvalidate() => whenInvalidateRequired.OnNext(Unit.Default);
	private StateRec GetStateRec() => new(
		State,
		ViewLng,
		ContLng,
		ScrollOfs
	);


	// Has anything changed as a result of Layout or a UserOperation (for WinSpector)
	internal IObservable<Unit> WhenChanged => whenChanged.AsObservable();
	internal IObservable<IScrollCmd> WhenCmd => whenCmd.AsObservable();

	// Derived quantities
	internal int MaxScrollOfs => Math.Max(0, ContLng - ViewLng);
	internal int TrakLng => (ViewLng - 2 * C.BtnLength).Cap();
	internal int PageSize => MathUtils.FillingDiv(ViewLng, C.ScrollUnit);

	internal int ThumbLng => State switch
	{
		St.Visible => TrakLng switch
		{
			<= C.MinThumbLength => TrakLng,
			_ => (ViewLng * TrakLng / ContLng).CapMin(C.MinThumbLength)
		},
		_ => 0
	};

	internal int MaxThumbPos => State switch
	{
		St.Visible => TrakLng - ThumbLng,
		_ => 0
	};

	internal int ThumbPos => State switch
	{
		St.Visible => ScrollStateUtils.Src2Dst(ScrollOfs, MaxScrollOfs + 1, MaxThumbPos + 1),
		_ => 0
	};

	internal int TrackDecLng => State switch
	{
		St.Visible => ThumbPos,
		_ => 0
	};

	internal int TrackIncLng => State switch
	{
		St.Visible => TrakLng - (ThumbPos + ThumbLng),
		_ => 0
	};


	public Dir Dir { get; }
	public St State { get; private set; }
	public int ViewLng { get; private set; }		// Size of the scroll node minus the thickness of the orthogonal scrollbar if visible
	public int ContLng { get; private set; }		// Size of the content
	public int ScrollOfs { get; private set; }
	public bool IsTailing => ScrollOfs > 0 && ScrollOfs == MaxScrollOfs;

	// Do we need a repaint as a result of a UserOperation
	public IObservable<Unit> WhenInvalidateRequired => whenInvalidateRequired.AsObservable();

	internal ScrollState(Dir dir)
	{
		Dir = dir;
		whenCmd = new Subject<IScrollCmd>().D(d);
		whenChanged = new Subject<Unit>().D(d);
		whenInvalidateRequired = new Subject<Unit>().D(d);
	}

	internal void UpdateFromLayout(
		St state,
		int viewLng,
		int contLng
	)
	{
		var st = GetStateRec();

		State = state;

		if (!State.IsEnabled())
		{
			ViewLng = ContLng = ScrollOfs = 0;
		}
		else
		{
			var wasTailing = IsTailing;
			ViewLng = viewLng;
			ContLng = contLng;
			if (wasTailing)
				ScrollOfs = MaxScrollOfs;
			CapScrollOfs();
		}

		if (GetStateRec() != st) TriggerChanged();
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

		var st = GetStateRec();

		var delta = GetCmdDelta(cmd);
		ScrollOfs += delta;
		CapScrollOfs();

		if (GetStateRec() != st)
		{
			TriggerChanged();
			TriggerInvalidate();
		}
	}

	internal int GetCmdDelta(IScrollCmd cmd) => cmd.GetSign() * cmd switch
	{
		UnitScrollCmd => C.ScrollUnit,						// 1 line
		WheelScrollCmd => C.WheelMult * C.ScrollUnit,		// 3 lines
		PageScrollCmd => (PageSize - 1) * C.ScrollUnit,		// 1 page - 1 line
		_ => throw new ArgumentException("Impossible")
	};


	private void CapScrollOfs() => ScrollOfs = ScrollOfs.Cap(MaxScrollOfs);
}



file static class ScrollStateUtils
{
	public static int Src2Dst(int src, int srcCount, int dstCount)
	{
		if (srcCount <= 0) throw new ArgumentException("Precondition 1 broken");
		if (dstCount <= 0) throw new ArgumentException("Precondition 2 broken");
		if (src < 0 || src >= srcCount) throw new ArgumentException("Precondition 3 broken");

		int dst;

		if (dstCount <= 1)
		{
			dst = 0;
		}
		else if (src == srcCount - 1)
		{
			dst = (dstCount - 1).Cap();
		}
		else if (srcCount <= dstCount)
		{
			var inc = MathUtils.FillingDiv(dstCount - 1, srcCount - 1);
			dst = src * inc;
		}
		else if (srcCount > dstCount)
		{
			var mod = MathUtils.FillingDiv(srcCount - 1, dstCount - 1);
			dst = src / mod;
		}
		else
			throw new ArgumentException("Impossible");

		if (src == 0 && dst != 0) throw new ArgumentException("Postcondition 1 broken");
		if (src != srcCount - 1 && dst == dstCount - 1) throw new ArgumentException("Postcondition 2 broken");
		if (src == srcCount - 1 && dst != dstCount - 1) throw new ArgumentException("Postcondition 3 broken");

		return dst;
	}
}