﻿using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Tests.TestSupport;
using PowBasics.Geom;

namespace LayoutSystem.Tests.Flex;

class FlexSolverTests
{
	[Test]
	public void _01_Wrap()
	{
			M(Vec.Fil, null,
				M(Vec.FilFit, Wrap(Dir.Horz),
					M(Vec.Fix(30, 20))
				)
			)
			.Check(new Sz(50, 100),
				R(0, 0, 50, 100,
					R(0, 0, 50, 20,
						R(0, 0, 30, 20)
					)
				)
			);
	}

	[Test]
	public void _02_Stack()
	{
		M(Vec.Fil, null,
				M(Vec.FilFit, Stack(Dir.Horz, Align.Start),
					M(Vec.Fix(30, 20)),
					M(Vec.FilFix(60)),
					M(Vec.Fix(40, 50))
				)
			)
			.Check(new Sz(150, 50),
				R(0, 0, 150, 50,
					R(0, 0, 150, 60,
						R(0, 0, 30, 20),
						R(30, 0, 80, 60),
						R(110, 0, 40, 50)
					)
				)
			);
	}

	[Test]
	public void _03_Stack2()
	{
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
				R(0, 0, 150, 50,
					R(0, 0, 150, 50,
						R(0, 0, 30, 20),
						R(30, 0, 20, 30,
							R(30, 0, 20, 30)
						),
						R(50, 0, 40, 50)
					)
				)
			);
	}


	[Test]
	public void _10_StackWrap()
	{
		M(Vec.Fil, Stack(Dir.Vert, Align.Start),
			M(Vec.FilFit, Wrap(Dir.Horz),
				M(Vec.Fix(30, 20))
			),
			M(Vec.Fix(10, 40))
		)
			.Check(new Sz(50, 100),
				R(0, 0, 50, 100,
					R(0, 0, 50, 20,
						R(0, 0, 30, 20)
					),
					R(0, 20, 10, 40)
				)
			);
	}

	[Test]
	public void _11_StackWrap2()
	{
		M(Vec.Fil, Stack(Dir.Vert, Align.Start),
				M(Vec.FilFit, Wrap(Dir.Horz),
					M(Vec.Fix(30, 20)),
					M(Vec.Fix(10, 15)),
					M(Vec.Fix(20, 10))
				),
				M(Vec.Fix(10, 40))
			)
			.Check(new Sz(50, 100),
				R(0, 0, 50, 100,
					R(0, 0, 50, 30,
						R(0, 0, 30, 20),
						R(30, 0, 10, 15),
						R(0, 20, 20, 10)
					),
					R(0, 30, 10, 40)
				)
			);
	}
}