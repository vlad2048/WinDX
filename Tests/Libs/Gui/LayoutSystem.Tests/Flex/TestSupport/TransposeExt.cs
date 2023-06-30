using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.LayStratsInternal;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Tests.Flex.TestSupport;

static class TransposeExt
{
	public static Node Transpose(this Node root) => Nod.Make(root.V.Transpose(), root.Children.Select(e => e.Transpose()));

	public static FreeSz Transpose(this FreeSz v) => new(v.Y, v.X);

	public static TNod<R> Transpose(this TNod<R> root) => Nod.Make(root.V.Transpose(), root.Children.Select(e => e.Transpose()));


	private static R Transpose(this R r) => new(r.Y, r.X, r.Height, r.Width);

	private static FlexNode Transpose(this FlexNode v) => new(
		v.Dim.Transpose(),
		v.Flags.Transpose(),
		v.Strat.Transpose(),
		v.Marg.Transpose()
	);
	private static DimVec Transpose(this DimVec v) => new(v.Y, v.X);
	private static FlexFlags Transpose(this FlexFlags v) => v with { Scroll = v.Scroll.Transpose() };
	private static BoolVec Transpose(this BoolVec v) => new(v.Y, v.X);

	private static IStrat Transpose(this IStrat strat) => strat switch
	{
		FillStrat => strat,
		StackStrat s => new StackStrat(s.MainDir.Neg(), s.Align),
		WrapStrat s => new WrapStrat(s.MainDir.Neg()),
		MarginStrat => strat,
		_ => throw new ArgumentException()
	};
	private static Marg Transpose(this Marg v) => new(v.Left, v.Bottom, v.Right, v.Top);
}