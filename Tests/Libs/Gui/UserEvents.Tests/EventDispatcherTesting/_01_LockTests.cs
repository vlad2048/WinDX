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

sealed class _01_LockTests : EventDispatcherBaseTest
{
	[Test]
	public void _00_MoveToA_Down_MoveToB()
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

		User(Move(pB1));
		Check(
			Move(A, pB1)
		);
	}

	[Test]
	public void _01_MoveToA_Down_MoveToB__Up()
	{
		_00_MoveToA_Down_MoveToB();

		User(BtnUp(), Move(pB1));
		Check(
			BtnUp(A),
			Leave(A),
			Enter(B, pB1),
			Move(B, pB1)
		);
	}

	[Test]
	public void _02_MoveToA_Down_MoveASomewhereElse_Up()
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

        MoveNode(A, rB);
        Check();

		User(BtnUp());
		Check(
			BtnUp(A)
		);
	}

}