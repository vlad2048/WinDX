using ControlSystem.Utils;
using TestBase;
using static ControlSystem.Tests.Utils.TestSupport;

namespace ControlSystem.Tests.Utils;

sealed class TreeExtsTests
{
	[Test]
	public void _00_Basic() =>
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

record Bad(string Str) : IMix
{
	public override string ToString() => $"Bad({Str})";
}

record Good(int Num) : IMix
{
	public override string ToString() => $"Good({Num})";
}


file static class TestSupport
{
	public static TNod<IMix> B(string str, params TNod<IMix>[] children) => Nod.Make(new Bad(str), children);
	public static TNod<IMix> G(int num, params TNod<IMix>[] children) => Nod.Make(new Good(num), children);
	public static TNod<Good> GG(int num, params TNod<Good>[] children) => Nod.Make(new Good(num), children);
}