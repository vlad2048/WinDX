using ControlSystem.Utils;
using PowBasics.CollectionsExt;
using Shouldly;
using TestBase;
using static ControlSystem.Tests.Utils.TestSupport;

namespace ControlSystem.Tests.Utils;

sealed class TreeExtsTests
{
	/*
	    ┌──n1
	    │
	10──┤
	    │      ┌──11
	    └──n2──┤      ┌──n4──n5──n6
	           └──n3──┤
	                  └──12──n7──13──n8
	*/
	private static readonly TNod<int> extendTree =
		N(10,
			N(1),
			N(2,
				N(11),
				N(3,
					N(4,
						N(5,
							N(6)
						)
					),
					N(12,
						N(7,
							N(13,
								N(8)
							)
						)
					)
				)
			)
		);

	[Test]
	public void _00_ExtendUpAndIncluding_1()
	{
		var (root, child) = extendTree.Single(e => e.V == 3).ExtendUpAndIncluding(e => e >= 10);
		TreeLogger.LogTree(root, "Root");
		TreeLogger.LogTree(child, "Child");
		root.V.ShouldBe(10);
		child.V.ShouldBe(3);
	}

	[Test]
	public void _01_ExtendUpAndIncluding_2()
	{
		var (root, child) = extendTree.Single(e => e.V == 12).ExtendUpAndIncluding(e => e >= 10);
		TreeLogger.LogTree(root, "Root");
		TreeLogger.LogTree(child, "Child");
		root.V.ShouldBe(12);
		child.V.ShouldBe(12);
	}


	[Test]
	public void _05_ExtendDownToAndExcluding()
	{
		var (root, boundary) = extendTree.Single(e => e.V == 2).ExtendDownToAndExcluding(e => e >= 10);
		extendTree.LogTree("ExtendTree");
		root.LogTree("Root");
		root.ShouldBeSameTree(
			N(2,
				N(3,
					N(4,
						N(5,
							N(6)
						)
					)
				)
			)
		);
		CollectionAssert.AreEqual(new[] { 11, 12 }, boundary.SelectToArray(e => e.V));
	}

	/*
    ┌──n1
    │
10──┤
    │      ┌──11
    └──n2──┤      ┌──n4──n5──n6
           └──n3──┤
                  └──12──n7──13──n8
*/



	[Test]
	public void _10_OfTypeTree() =>
		B("a",
			G(1,
				G(2,
					B("d",
						G(5),
						G(6)
					)
				),

				B("b",
					G(3)
				),

				B("c",
					B("e"),
					G(4,
						G(7),
						B("f")
					)
				)
			)
		)
			.OfTypeTree<IMix, Good>()
			.ShouldBeSameTree(
				GG(1,
					GG(2,
						GG(5),
						GG(6)
					),

					GG(3),

					GG(4,
						GG(7)
					)
				)
			);
}


interface IMix { }

sealed record Bad(string Str) : IMix
{
	public override string ToString() => $"Bad({Str})";
}

sealed record Good(int Num) : IMix
{
	public override string ToString() => $"Good({Num})";
}


file static class TestSupport
{
	public static TNod<int> N(int num, params TNod<int>[] children) => Nod.Make(num, children);

	public static TNod<IMix> B(string str, params TNod<IMix>[] children) => Nod.Make(new Bad(str), children);
	public static TNod<IMix> G(int num, params TNod<IMix>[] children) => Nod.Make(new Good(num), children);
	public static TNod<Good> GG(int num, params TNod<Good>[] children) => Nod.Make(new Good(num), children);
}