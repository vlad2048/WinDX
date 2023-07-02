using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowBasics.MathCode;
using PowRxVar;

namespace ControlSystem.Logic.Scrolling_;


file static class ScrollConsts
{
	public const int ScrollUnit = 16;
	public const int WheelMult = 3;
}

public sealed class ScrollDimState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISubject<Unit> whenInvalidateRequired;
	private void TriggerInvalidate() => whenInvalidateRequired.OnNext(Unit.Default);

	public int ViewLng { get; private set; }
	public int ContLng { get; private set; }
	public int TrakLng { get; private set; }

	public int ScrollOfs { get; private set; }
	public bool IsTailing { get; private set; }

	public IObservable<Unit> WhenInvalidateRequired => whenInvalidateRequired.AsObservable();

	internal ScrollDimState()
	{
		whenInvalidateRequired = new Subject<Unit>().D(d);
	}

	internal void UpdateFromLayout(
		bool enabled,
		bool visible,
		int viewLng,
		int contLng,
		int trakLng
	)
	{
		if (!enabled)
		{
			ViewLng = ContLng = TrakLng = ScrollOfs = 0;
			IsTailing = false;
			return;
		}

		ViewLng = viewLng;
		ContLng = contLng;
		TrakLng = trakLng;
		CapScrollOfs();
	}

	internal bool CanRunScrollCmd(IScrollCmd cmd) => cmd.DecInc switch
	{
		ScrollBtnDecInc.Dec => ScrollOfs > 0,
		ScrollBtnDecInc.Inc => ScrollOfs < MaxScrollOfs
	};

	internal void RunScrollCmd(IScrollCmd cmd)
	{
		if (!CanRunScrollCmd(cmd)) return;

		var delta = cmd.GetSign() * cmd switch
		{
			UnitScrollCmd => ScrollConsts.ScrollUnit,
			WheelScrollCmd => ScrollConsts.ScrollUnit * ScrollConsts.WheelMult,
			PageScrollCmd => PageSize,
			_ => throw new ArgumentException("Impossible")
		};

		ScrollOfs += delta;
		CapScrollOfs();
		TriggerInvalidate();
	}






	private int MaxScrollOfs => Math.Max(0, ContLng - ViewLng);
	private int PageSize => MathUtils.FillingDiv(ViewLng, ScrollConsts.ScrollUnit);

	private void CapScrollOfs() => ScrollOfs = ScrollOfs.Cap(MaxScrollOfs);
}


public sealed class ScrollState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public ScrollDimState X { get; }
	public ScrollDimState Y { get; }

	public Pt ScrollOfs => new (X.ScrollOfs, Y.ScrollOfs);
	public IObservable<Unit> WhenInvalidateRequired => Obs.Merge(X.WhenInvalidateRequired, Y.WhenInvalidateRequired);

	public ScrollState()
	{
		X = new ScrollDimState().D(d);
		Y = new ScrollDimState().D(d);

		//WhenInvalidateRequired.Subscribe(_ => L($"ofs: {ScrollOfs}")).D(d);
	}

	internal void UpdateFromLayout(ScrollNfo nfo)
	{
		void Do(Dir dir)
		{
			var st = Get(dir);
			st.UpdateFromLayout(
				nfo.Enabled.Dir(dir),
				nfo.Visible.Dir(dir),
				nfo.ViewSz.Dir(dir),
				nfo.ContSz.Dir(dir),
				nfo.TrakSz.Dir(dir)
			);
		}

		Do(Dir.Horz);
		Do(Dir.Vert);
	}

	private ScrollDimState Get(Dir dir) => dir switch
	{
		Dir.Horz => X,
		Dir.Vert => Y
	};
}



file static class ScrollStateLocalExt
{
	public static int Cap(this int v, int max) => Math.Max(0, Math.Min(v, max));
}