using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace TestBase;

public class RxTimeTest : RxTest
{
	private TestScheduler scheduler = null!;

	protected IScheduler Scheduler => scheduler;

	protected void Advance(TimeSpan t)
	{
		L($"advance: {t}");
		scheduler.AdvanceBy(t.Ticks);
	}

	[SetUp]
	public new void Setup()
	{
		scheduler = new TestScheduler();
	}
}