using PowBasics.Geom;

namespace UserEvents.Tests.MakeForWinTesting.TestSupport;

class GeomData
{
	public static readonly R rMain = new(0, 0, 200, 350);
	public static readonly R rPop0 = new(50, 100, 200, 150);
	public static readonly R rPop1 = new(50, 150, 250, 100);

	public static readonly Pt pMain = new(25, 25);
	public static readonly Pt pPop0 = new(75, 125);
	public static readonly Pt pPop1 = new(75, 200);
	public static readonly Pt pOut = new(500, 400);
}