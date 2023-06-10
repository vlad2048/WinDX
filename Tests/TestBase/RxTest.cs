using NUnit.Framework;
using PowRxVar;
using Shouldly;

namespace TestBase;

public class RxTest
{
	[SetUp]
	public void Setup() => VarDbg.ClearUndisposedCountersForTest();

	[TearDown]
	public void Teardown() => VarDbg.CheckForUndisposedDisps().ShouldBeFalse();
}