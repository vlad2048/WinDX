using PowBasics.Geom;
using PowRxVar;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.TestSupport;
using UserEvents.Tests.TestSupport.Utils;
using WinAPI.User32;
using static UserEvents.Tests.Win2NodesTestsUtils;
#pragma warning disable CS8618

namespace UserEvents.Tests;

/* X╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶┐
   ╷                                     ╷
   ╷   ┌─nodeR───────────────────────┐   ╷
   ╷   │ pR1 X    X pR2              │   ╷
   ╷ X │   ┌─nodeA──┐   ┌─nodeB──┐	 │   ╷
   pOut1   │        │   │        │   │   ╷
   ╷   │   │ X    X │   │ X    X │   │   ╷
   ╷   │   │pA1  pA2│   │pB1  pB2│   │   ╷
   ╷ X │   └────────┘   └────────┘   │   ╷
   pOut2                             │   ╷
   ╷   └─────────────────────────────┘   ╷
   ╷                                     ╷
   └╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶╶┘ */
sealed class EventDispatcherTests : RxTest
{
    private EventTester tester;
    private TNode NodeR => tester.NodeR;
    private TNode NodeA => tester.NodeA;
    private TNode NodeB => tester.NodeB;


    [Test]
    public void _00_Basics()
    {
        Check(A(), A(), A());
        AddNodes(NodeR, NodeA, NodeB);

        User(Move(pA1));

        Check(
            nodeR: A(),
            nodeA: A(Enter(pA1), Move(pA1)),
            nodeB: A()
        );
    }



    private void User(params IUserEvt[] events) => tester.User(events);
    private void Check(IUserEvt[] nodeR, IUserEvt[] nodeA, IUserEvt[] nodeB) => tester.Check(nodeR, nodeA, nodeB);
    private void AddNodes(params TNode[] arr) => tester.AddNodes(arr);
    private void DelNodes(params TNode[] arr) => tester.DelNodes(arr);


    [SetUp]
    public new void Setup()
    {
        ResetUtilsMousePos();
        tester = new EventTester().D(D);
    }
}


file static class Win2NodesTestsUtils
{
    private static Pt mousePos = Pt.Empty;

    public static void ResetUtilsMousePos() => mousePos = Pt.Empty;

    public static IUserEvt[] A(params IUserEvt[] arr) => arr;

    public static IUserEvt Enter(Pt pt)
    {
        mousePos = pt;
        return new MouseEnterUserEvt(pt);
    }
    public static readonly IUserEvt Leave = new MouseLeaveUserEvt();
    public static IUserEvt Move(Pt pt)
    {
        mousePos = pt;
        return new MouseMoveUserEvt(pt);
    }
    public static IUserEvt BtnDown() => new MouseButtonDownUserEvt(mousePos, MouseBtn.Left);
    public static IUserEvt BtnUp() => new MouseButtonUpUserEvt(mousePos, MouseBtn.Left);
    public static readonly IUserEvt KeyDown = new KeyDownUserEvt(VirtualKey.K);
    public static readonly IUserEvt KeyUp = new KeyUpUserEvt(VirtualKey.K);
}