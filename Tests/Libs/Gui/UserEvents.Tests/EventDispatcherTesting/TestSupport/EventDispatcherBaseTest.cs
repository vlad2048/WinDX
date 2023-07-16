using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;

namespace UserEvents.Tests.EventDispatcherTesting.TestSupport;

class EventDispatcherBaseTest : RxTest
{
	protected NodeZ R => new(tester.NodeR, new ZOrder(0, false, 0));
	protected NodeZ A => new(tester.NodeA, new ZOrder(0, false, 1));
	protected NodeZ B => new(tester.NodeB, new ZOrder(0, false, 2));

	protected void Check(params NodeEvt[] evts) => tester.Check(evts);

	protected void User(params IUserEvt[] events)
	{
		Step();
		tester.User(events);
	}

	protected void AddNodes(params NodeZ[] arr)
	{
		Step();
		L($"[User] -> AddNode {arr.JoinText()}");
		tester.AddNodes(arr);
	}

	protected void DelNodes(params NodeZ[] arr)
	{
		Step();
		L($"[User] -> DelNodes {arr.JoinText()}");
		tester.DelNodes(arr);
	}

	protected void MoveNode(NodeZ node, R r)
	{
		Step();
		L($"[User] -> MoveNode {node} to {r}");
		((TNode)node.Node).RSrc.V = r;
	}

	private void Step() => LBigTitle($"Step {step++}");


	private EventTester tester = null!;
	private int step;

	[SetUp]
	public new void Setup()
	{
		step = 0;
		Win2NodesTestsUtils.ResetUtilsMousePos();
		tester = new EventTester().D(D);
		Check();
		AddNodes(R, A, B);
		Check();
	}
}