﻿using PowBasics.Geom;

// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class DimVecMaker
{
	public static DimVec MkDir(Dir dir, Dim x, Dim y) => dir switch
	{
		Dir.Horz => new(x, y),
		Dir.Vert => new(y, x),
	};

	public static DimVec DirFun(Func<Dir, Dim> fun) => new(fun(Dir.Horz), fun(Dir.Vert));

	
// @formatter:off
	public static          DimVec Mk (FDim x, FDim y) => new(x       , y       );

	public static readonly DimVec Fit                 =  new(null    , null    );
	public static          DimVec Fix(int x, int y)   => new(D.Fix(x), D.Fix(y));
	public static readonly DimVec Fil                 =  new(D.Fil   , D.Fil   );

	public static readonly DimVec FitFil              =  new(null    , D.Fil   );
	public static readonly DimVec FilFit              =  new(D.Fil   , null    );
	
	public static          DimVec FixFil(int x)       => new(D.Fix(x), D.Fil   );
	public static          DimVec FilFix(int y)       => new(D.Fil   , D.Fix(y));

	public static          DimVec FixFit(int x)       => new(D.Fix(x), null    );
	public static          DimVec FitFix(int y)       => new(null    , D.Fix(y));


	public static          DimVec Fix(Dir dir, int x, int y) => Fix(x, y).SwapDir(dir);
	public static          DimVec FitFilD(Dir dir)           => FitFil.SwapDir(dir);
	public static          DimVec FilFitD(Dir dir)           => FilFit.SwapDir(dir);
	public static          DimVec FixFil(Dir dir, int x)     => FixFil(x).SwapDir(dir);
	public static          DimVec FilFix(Dir dir, int y)     => FilFix(y).SwapDir(dir);
	public static          DimVec FixFit(Dir dir, int x)     => FixFit(x).SwapDir(dir);
	public static          DimVec FitFix(Dir dir, int y)     => FitFix(y).SwapDir(dir);

// @formatter:on


	private static DimVec SwapDir(this DimVec v, Dir dir) => dir switch
	{
		Dir.Horz => v,
		Dir.Vert => new DimVec(v.Y, v.X)
	};
}
