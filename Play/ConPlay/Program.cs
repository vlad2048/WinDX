global using static LayoutSystem.Flex.Strats;
global using static LayoutSystem.Utils.TreeBuilder;
global using Node = TNod<LayoutSystem.Flex.FlexNode>;
global using Vec = LayoutSystem.Flex.Structs.DimVecMaker;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.JsonUtils;
using PowBasics.Geom;
using PowTrees.Algorithms;

namespace ConPlay;
static class Program
{
	//public static TNod<Rec> R(Rec r, params TNod<Rec>[] kids) => Nod.Make(r, kids);

	static void Main()
	{
		var root =
			M(Vec.Fil, Stack(Dir.Vert, Align.Start),
				M(Vec.FilFit, Wrap(Dir.Horz),
					M(Vec.Fix(30, 20))
				),
				M(Vec.Fix(10, 40))
			);

		var str = Jsoner.Ser(root);
		var root2 = Jsoner.Deser<Node>(str);

		L(root.LogToString());
		//L(str);
		L(root2.LogToString());
	}

	private static void L(string s) => Console.WriteLine(s);
}

/*
var tree =
	M(DimVec.Flt, Stack(Dir.Vert, Align.Start),
		M(DimVec.FltFit, Wrap(Dir.Horz, DimFix.Flt()),
			M(DimVec.Fix(30, 20)),
			M(DimVec.Fix(10, 40)),
			M(DimVec.Fix(50, 60)),

			M(DimVec.Fix(30, 30)),
			M(DimVec.Fix(50, 35)),
			M(DimVec.Fix(20, 40)),

			M(DimVec.Fix(40, 25))
		),
		//M(DimVec.Fix),
		M(DimVec.Fix(30, 10)),
		M(DimVec.Fix(50, 20)),
		M(DimVec.FixFlt(60)),
		M(DimVec.Fix(40, 30))
	);

var rTree = FlexSolver.Solve(tree, new Sz(100, 250));

L(
	tree.ZipTree(rTree).LogToString(opt => opt.FormatFun = t => $"{t.Item1}".PadRight(32) + $" -> {t.Item2}")
);

//L(rTree.LogToString());
*/
