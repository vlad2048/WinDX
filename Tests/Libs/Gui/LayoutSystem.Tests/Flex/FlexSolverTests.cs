using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Tests.TestSupport;
using PowBasics.Geom;

namespace LayoutSystem.Tests.Flex;

sealed class FlexSolverTests
{
	[Test]
	public void _01_Wrap() =>
			M(Vec.Fil, null,
				M(Vec.FilFit, Wrap(Dir.Horz),
					M(Vec.Fix(30, 20))
				)
			)
			.Check(new Sz(50, 100),
				A(0, 0, 50, 100,
					A(0, 0, 50, 20,
						A(0, 0, 30, 20)
					)
				)
			);


	[Test]
	public void _02_Stack() =>
		M(Vec.Fil, null,
				M(Vec.FilFit, Stack(Dir.Horz, Align.Start),
					M(Vec.Fix(30, 20)),
					M(Vec.FilFix(60)),
					M(Vec.Fix(40, 50))
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
		M(Vec.Fil, null,
				M(Vec.FilFit, Stack(Dir.Horz, Align.Start),
					M(Vec.Fix(30, 20)),
					M(Vec.Fit, null,
						M(Vec.Fix(20, 30))
					),
					M(Vec.Fix(40, 50))
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
		M(Vec.Fil, null,
				M(Vec.FitFil, Stack(Dir.Vert, Align.Middle),
					M(Vec.Fix(110, 100)),
					M(Vec.Fix(50, 60))
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
		M(Vec.Fil, Stack(Dir.Vert, Align.Start),
			M(Vec.FilFit, Wrap(Dir.Horz),
				M(Vec.Fix(30, 20))
			),
			M(Vec.Fix(10, 40))
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
		M(Vec.Fil, Stack(Dir.Vert, Align.Start),
				M(Vec.FilFit, Wrap(Dir.Horz),
					M(Vec.Fix(30, 20)),
					M(Vec.Fix(10, 15)),
					M(Vec.Fix(20, 10))
				),
				M(Vec.Fix(10, 40))
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
		M(Vec.Fil, Stack(Dir.Horz, Align.Start),
				M(Vec.Fix(100, 60), Fill),
				M(Vec.Fix(110, 190), Pop,
					M(Vec.FilFit, Stack(Dir.Vert, Align.Start),
						M(Vec.Fix(60, 50), Fill),
						M(Vec.Fix(80, 40), Pop),
						M(Vec.Fix(60, 60), Fill)
					)
				),
				M(Vec.Fix(40, 140), Fill)
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
}