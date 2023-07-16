using UserEvents.Tests.EventDispatcherTesting.TestSupport;
using static UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils.Win2NodesTestsUtils;
using static UserEvents.Tests.EventDispatcherTesting.TestSupport.Utils.GeomData;

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

sealed class _00_BasicTests : EventDispatcherBaseTest
{
    [Test]
    public void _00_MoveToA_MoveWithinA_MoveToR()
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
    public void _01_MoveToA_MoveToB_MoveToA()
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
    public void _10_MoveToA_Down_Up()
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
    public void _20_MoveToA_RemoveA()
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
    public void _21_RemoveA_MoveToWhereAWas_AddA()
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
    public void _22_MoveToA_MoveASomewhereElse()
    {
        User(Move(pA1));
        Check(
            Enter(A, pA1),
            Move(A, pA1)
        );

        MoveNode(A, rB);
        Check(
            Leave(A),
            Enter(R, pA1),
            Move(R, pA1)
        );
    }

    [Test]
    public void _23_MoveASomewhereElse_MoveToWhereAWas_MoveABack()
    {
	    MoveNode(A, rB);
        Check();

        User(Move(pA1));
        Check(
            Enter(R, pA1),
            Move(R, pA1)
        );

	    MoveNode(A, rA);
        Check(
            Leave(R),
            Enter(A, pA1),
            Move(A, pA1)
        );
    }

    [Test]
    public void _30_MoveToA_LeaveWin_MoveToB()
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
}
