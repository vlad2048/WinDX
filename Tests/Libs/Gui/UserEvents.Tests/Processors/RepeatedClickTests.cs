using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using Shouldly;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.Processors.TestSupport;
using UserEvents.Utils;

namespace UserEvents.Tests.Processors;

public class RepeatedClickTests : RxTimeTest
{
	private static readonly R r0 = new(100, 50, 50, 50);
	private static readonly Pt p0 = r0.Center;
	private static readonly R r1 = new(200, 50, 50, 50);
	private static readonly Pt p1 = r1.Center;
	private static readonly TimeSpan Delay = InputConstants.RepeatClickDelay;
	private static readonly TimeSpan Speed = InputConstants.RepeatClickSpeed;
	private static TimeSpan DelayHalf0;
	private static TimeSpan DelayHalf1;
	private static TimeSpan SpeedHalf0;
	private static TimeSpan SpeedHalf1;
	private INode node = null!;
	private Action<R> setR = null!;
	private ISubject<IUserEvt> whenEvt = null!;
	private ObservableChecker<Unit> checker = null!;

	private void Send(IUserEvt e)
	{
		L($"<- {e}");
		whenEvt.OnNext(e);
	}

	private Pt mousePos = Pt.Empty;
	private void Move(Pt pt)
	{
		mousePos = pt;
		Send(new MouseMoveUserEvt(pt));
	}
	private void BtnDown() => Send(new MouseButtonDownUserEvt(mousePos, MouseBtn.Left));
	private void BtnUp() => Send(new MouseButtonUpUserEvt(mousePos, MouseBtn.Left));

	private bool dbg = false;
	private void Check(int cntExp)
	{
		var cntAct = checker.Clear().Length;
		L($"check Exp:{cntExp} Act:{cntAct}");
		if (!dbg) cntAct.ShouldBe(cntExp);
	}


	[Test]
	public void _00_Basics()
	{
		Move(p0);
		Check(0);

		BtnDown();
		Check(1);

		Advance(DelayHalf0);
		Check(0);

		Advance(DelayHalf1);
		Check(1);

		Advance(SpeedHalf0);
		Check(0);

		Advance(SpeedHalf1);
		Check(1);
	}



	[SetUp]
	public new void Setup()
	{
		(node, setR) = NodeMaker.Make().D(D);
		setR(r0);
		whenEvt = new Subject<IUserEvt>().D(D);
		checker = new ObservableChecker<Unit>(
			whenEvt.AsObservable()
				.WhenRepeatedClick(
					node,
					MouseBtn.Left,
					Scheduler
				).D(D),
			"Repeated"
		).D(D);
		Check(0);
		(DelayHalf0, DelayHalf1) = Delay.Split();
		(SpeedHalf0, SpeedHalf1) = Speed.Split();
	}
}


file static class RepeatedClickTestsExt
{
	public static (TimeSpan, TimeSpan) Split(this TimeSpan ts)
	{
		var t = ts.Ticks;
		var t0 = t / 2;
		var t1 = t - t0;
		return (TimeSpan.FromTicks(t0), TimeSpan.FromTicks(t1));
	}
}