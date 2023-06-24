using NUnit.Framework;
using PowRxVar;
using Shouldly;

namespace TestBase;

public class RxTest
{
	protected Disp D { get; private set; } = null!;

	[SetUp]
	public void Setup()
	{
		VarDbg.ClearUndisposedCountersForTest();
		D = new Disp();
	}

	[TearDown]
	public void Teardown()
	{
		D.Dispose();
		VarDbg.CheckForUndisposedDisps().ShouldBeFalse();
	}
}