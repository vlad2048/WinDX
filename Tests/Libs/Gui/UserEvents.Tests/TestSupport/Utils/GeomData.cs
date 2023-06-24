using PowBasics.Geom;

namespace UserEvents.Tests.TestSupport.Utils;

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

static class GeomData
{
    public static readonly R rR = new(10, 10, 110, 60);
    public static readonly R rA = new(30, 30, 30, 20);
    public static readonly R rB = new(70, 30, 30, 20);

    public static readonly Pt pR1 = new(40, 20);
    public static readonly Pt pR2 = new(50, 20);

    public static readonly Pt pA1 = new(40, 40);
    public static readonly Pt pA2 = new(50, 40);

    public static readonly Pt pB1 = new(80, 40);
    public static readonly Pt pB2 = new(90, 40);

    public static readonly Pt pOut1 = new(5, 30);
    public static readonly Pt pOut2 = new(5, 50);
}