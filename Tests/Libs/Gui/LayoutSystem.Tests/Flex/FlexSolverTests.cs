using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Tests.TestSupport;
using PowBasics.Geom;

namespace LayoutSystem.Tests.Flex;

sealed class FlexSolverTests
{
	private static FlexNodeFluent F() => new();
	private static TNod<FlexNodeFluent> N(FlexNodeFluent f, params TNod<FlexNodeFluent>[] kids) => Nod.Make(f, kids);

	[Test]
	public void _01_Wrap() =>
		N(F(),
			N(F().DimFilFit().StratWrap(Dir.Horz),
				N(F().Dim(30, 20))
		))
			.Check(new Sz(50, 100),
				A(0, 0, 50, 100,
					A(0, 0, 50, 20,
						A(0, 0, 30, 20)
					)
				)
			);

	[Test]
	public void _02_Stack() =>
		N(F(),
			N(F().DimFilFit().StratStack(Dir.Horz),
				N(F().Dim(30, 20)),
				N(F().DimFilFix(60)),
				N(F().Dim(40, 50))
			)
		)
			.Check(new Sz(150, 70),
				A(0, 0, 150, 70,
					A(0, 0, 150, 60,
						A(0, 0, 30, 20),
						A(30, 0, 80, 60),
						A(110, 0, 40, 50)
					)
				)
			);

	[Test]
	public void _03_Stack2() =>
		N(F(),
			N(F().DimFilFit().StratStack(Dir.Horz),
				N(F().Dim(30, 20)),
				N(F().DimFit(),
					N(F().Dim(20, 30))
				),
				N(F().Dim(40, 50))
			)
		)
			.Check(new Sz(150, 50),
				A(0, 0, 150, 50,
					A(0, 0, 150, 50,
						A(0, 0, 30, 20),
						A(30, 0, 20, 30,
							A(30, 0, 20, 30)
						),
						A(50, 0, 40, 50)
					)
				)
			);

	[Test]
	public void _04_StackElseFit() =>
		N(F(),
			N(F().DimFitFil().StratStack(Dir.Vert, Align.Middle),
				N(F().Dim(110, 100)),
				N(F().Dim(50, 60))
			)
		)
			.Check(new Sz(150, 200),
				A(0, 0, 150, 200,
					A(0, 0, 110, 200,
						A(0, 0, 110, 100),
						A(30, 100, 50, 60)
					)
				)
			);

	[Test]
	public void _10_StackWrap() =>
		N(F().StratStack(Dir.Vert),
			N(F().DimFilFit().StratWrap(Dir.Horz),
				N(F().Dim(30, 20))
			),
			N(F().Dim(10, 40))
		)
			.Check(new Sz(50, 100),
				A(0, 0, 50, 100,
					A(0, 0, 50, 20,
						A(0, 0, 30, 20)
					),
					A(0, 20, 10, 40)
				)
			);

	[Test]
	public void _11_StackWrap2() =>
		N(F().StratStack(Dir.Vert),
			N(F().DimFilFit().StratWrap(Dir.Horz),
				N(F().Dim(30, 20)),
				N(F().Dim(10, 15)),
				N(F().Dim(20, 10))
			),
			N(F().Dim(10, 40))
		)
			.Check(new Sz(50, 100),
				A(0, 0, 50, 100,
					A(0, 0, 50, 30,
						A(0, 0, 30, 20),
						A(30, 0, 10, 15),
						A(0, 20, 20, 10)
					),
					A(0, 30, 10, 40)
				)
			);

	[Test]
	public void _20_NestedPop() =>
		N(F().StratStack(Dir.Horz),
			N(F().Dim(100, 60)),
			N(F().Dim(110, 190).Pop(),
				N(F().DimFilFit().StratStack(Dir.Vert),
					N(F().Dim(60, 50)),
					N(F().Dim(80, 40).Pop()),
					N(F().Dim(60, 60))
				)
			),
			N(F().Dim(40, 140))
		)
			.Check(new Sz(400, 200),
				A(0, 0, 400, 200,
					A(0, 0, 100, 60),
					A(100, 0, 110, 190,
						A(100, 0, 110, 110,
							A(100, 0, 60, 50),
							A(100, 50, 80, 40),
							A(100, 50, 60, 60)
						)
					),
					A(100, 0, 40, 140)
				)
			);



	private static TNod<R> A(int x, int y, int width, int height, params TNod<R>[] kids) =>
		Nod.Make(new R(x, y, width, height), kids);
}