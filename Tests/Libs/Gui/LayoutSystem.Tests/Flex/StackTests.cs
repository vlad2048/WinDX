global using static LayoutSystem.Tests.Flex.TestSupport.BuildUtils;
global using static LayoutSystem.Tests.Flex.TestSupport.CheckUtils;
using PowBasics.Geom;

namespace LayoutSystem.Tests.Flex;

sealed class StackTests
{
	[Test]
	public void _00_FixBigBig() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).Dim(110, 90),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 110, 90,
				R(0, 0, 80, 40),
				R(0, 40, 70, 30)
			)
		)
	);

	[Test]
	public void _01_FixBigSmall() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).Dim(110, 60),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 110, 60,
				R(0, 0, 80, 40),
				R(0, 40, 70, 20)
			)
		)
	);

	[Test]
	public void _02_FixSmallBig() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).Dim(50, 90),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 50, 90,
				R(0, 0, 50, 40),
				R(0, 40, 50, 30)
			)
		)
	);


	[Test]
	public void _03_FixSmallSmall() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).Dim(50, 60),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 50, 60,
				R(0, 0, 50, 40),
				R(0, 40, 50, 20)
			)
		)
	);





	[Test]
	public void _10_FixFil() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).DimFixFil(110),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 110, 100,
				R(0, 0, 80, 40),
				R(0, 40, 70, 30)
			)
		)
	);

	[Test]
	public void _11_FixFit() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).DimFixFit(50),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 50, 70,
				R(0, 0, 50, 40),
				R(0, 40, 50, 30)
			)
		)
	);








	[Test]
	public void _10_Fil() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 150, 100,
				R(0, 0, 80, 40),
				R(0, 40, 70, 30)
			)
		)
	);

	[Test]
	public void _12_FixSmallFil() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).DimFixFil(50),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 50, 100,
				R(0, 0, 50, 40),
				R(0, 40, 50, 30)
			)
		)
	);

	[Test]
	public void _13_FitFil() => Check(
		Win(150, 100),

		N(F,
			N(F.StratStack(Dir.Vert).DimFitFil(),
				N(F.Dim(80, 40)),
				N(F.Dim(70, 30))
			)
		),

		R(0, 0, 150, 100,
			R(0, 0, 80, 100,
				R(0, 0, 80, 40),
				R(0, 40, 70, 30)
			)
		)
	);
}