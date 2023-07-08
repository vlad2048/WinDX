using PowBasics.Geom;
using PowRxVar;
using TestBase;
using UserEvents.Structs;
using UserEvents.Tests.EventDispatcherTesting.TestSupport;
using UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils;
using WinAPI.User32;
using static UserEvents.Tests.EventDispatcherTesting.Win2NodesTestsUtils;
using static UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils.GeomData;
#pragma warning disable CS8618

namespace UserEvents.Tests.EventDispatcherTesting;

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
    private NodeZ R => new(tester.NodeR, new ZOrder(0, false, 0));
    private NodeZ A => new(tester.NodeA, new ZOrder(0, false, 1));
    private NodeZ B => new(tester.NodeB, new ZOrder(0, false, 2));


    [Test]
    public void _00_MoveBasics()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        User(Move(pA2));
        Check(
            Move(A, pA2)
        );

        User(Move(pR1));
        Check(
            Leave(A),
            Enter(R, pR1),
            Move(R, pR1)
        );
    }

    [Test]
    public void _01_MoveBothDirections()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        User(Move(pB1));
        Check(
            Leave(A),
            Enter(B, pB1),
            Move(B, pB1)
        );

        User(Move(pA1));
        Check(
            Leave(B),
            Enter(A, pA1),
            Move(A, pA1)
        );
    }

    [Test]
    public void _10_Buttons()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        User(BtnDown());
        Check(
            BtnDown(A)
        );

        User(BtnUp());
        Check(
            BtnUp(A)
        );
    }

    [Test]
    public void _20_NodeRemoved()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        DelNodes(A);
        Check(
            Leave(A),
            Enter(R, pA1),
            Move(R, pA1)
        );
    }

    [Test]
    public void _21_NodeAdded()
    {
        DelNodes(A);
        Check();

        User(Move(pA1));
        Check(
            Enter(R, pA1),
            Move(R, pA1)
        );

        AddNodes(A);
        Check(
            Leave(R),
            Enter(A, pA1),
            Move(A, pA1)
        );
    }

    [Test]
    public void _22_NodeMovedOut()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        tester.NodeA.RSrc.V = new R(70, 30, 30, 20);
        Check(
            Leave(A),
            Enter(R, pA1),
            Move(R, pA1)
        );
    }

    [Test]
    public void _23_NodeMovedIn()
    {
		tester.NodeA.RSrc.V = new R(70, 30, 30, 20);
        Check();

        User(Move(pA1));
        Check(
            Enter(R, pA1),
            Move(R, pA1)
        );

        tester.NodeA.RSrc.V = rA;
        Check(
            Leave(R),
            Enter(A, pA1),
            Move(A, pA1)
        );
    }

    [Test]
    public void _30_Leave()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        User(Leave());
        Check(
            Leave(A)
        );

        User(Move(pB1));
        Check(
            Enter(B, pB1),
            Move(B, pB1)
        );
    }


    private EventTester tester;

    private void Check(params NodeEvt[] evts) => tester.Check(evts);

    private void User(params IUserEvt[] events) => tester.User(events);
    private void AddNodes(params NodeZ[] arr) => tester.AddNodes(arr);
    private void DelNodes(params NodeZ[] arr) => tester.DelNodes(arr);


    [SetUp]
    public new void Setup()
    {
        ResetUtilsMousePos();
        tester = new EventTester().D(D);
        Check();
        AddNodes(R, A, B);
        Check();
    }
}


file static class Win2NodesTestsUtils
{
    private static Pt mousePos = Pt.Empty;

    public static void ResetUtilsMousePos() => mousePos = Pt.Empty;




    public static IUserEvt Enter(Pt pt)
    {
        mousePos = pt;
        return new MouseEnterUserEvt(pt);
    }
    public static IUserEvt Leave() => new MouseLeaveUserEvt();
    public static IUserEvt Move(Pt pt)
    {
        mousePos = pt;
        return new MouseMoveUserEvt(pt);
    }
    public static IUserEvt BtnDown() => new MouseButtonDownUserEvt(mousePos, MouseBtn.Left);
    public static IUserEvt BtnUp() => new MouseButtonUpUserEvt(mousePos, MouseBtn.Left);
    public static IUserEvt KeyDown() => new KeyDownUserEvt(VirtualKey.K);
    public static IUserEvt KeyUp() => new KeyUpUserEvt(VirtualKey.K);




    public static NodeEvt Enter(NodeZ node, Pt pt)
    {
        mousePos = pt;
        return new NodeEvt(node.Node, new MouseEnterUserEvt(pt));
    }
    public static NodeEvt Leave(NodeZ node) => new(node.Node, new MouseLeaveUserEvt());
    public static NodeEvt Move(NodeZ node, Pt pt)
    {
        mousePos = pt;
        return new NodeEvt(node.Node, new MouseMoveUserEvt(pt));
    }
    public static NodeEvt BtnDown(NodeZ node) => new(node.Node, new MouseButtonDownUserEvt(mousePos, MouseBtn.Left));
    public static NodeEvt BtnUp(NodeZ node) => new(node.Node, new MouseButtonUpUserEvt(mousePos, MouseBtn.Left));
    public static NodeEvt KeyDown(NodeZ node) => new(node.Node, new KeyDownUserEvt(VirtualKey.K));
    public static NodeEvt KeyUp(NodeZ node) => new(node.Node, new KeyUpUserEvt(VirtualKey.K));
}