using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex.TreeLogic;
using PowBasics.Geom;
using PowTrees.Algorithms;
using Shouldly;

namespace LayoutSystem.Tests.TestSupport;

static class SolverChecker
{
	public static void Check(
		this TNod<FlexNodeFluent> treeFluentRaw,
		Sz totalSz,
		TNod<R> expRTree
	)
	{
		var treeRaw = treeFluentRaw.Map(e => e.Build());
		var freeSz = FreeSzMaker.FromSz(totalSz);
		var (tree, _) = treeRaw.RespectRules(freeSz);
		treeRaw.L("Raw");
		tree.L("Good");

		var layout = FlexSolver.Solve(treeRaw, freeSz);
		var actRTree = layout.Root.MapN(node => layout.RMap[node]);

		//tree.ZipTree(expRTree).L("Expected");
		//tree.ZipTree(actRTree).L("Actual");
		expRTree.L("Expected");
		actRTree.L("Actual");
		var isCorrect = actRTree.IsEqual(expRTree);
		if (isCorrect)
		{
			"-> OK".L();
		}
		else
		{
			var zipTree = tree.ZipTree(actRTree.ZipTree(expRTree));
			var diffCnt = zipTree.Count(t => t.V.Item2.Item1 != t.V.Item2.Item2);
			zipTree
				.Map(t =>
				{
					var (node, (actR, expR)) = t;
					return (actR == expR) switch
					{
						true => $" {node} ",
						false => $" {node}".PadRight(32) + "  (!)"
					};
				})
				.LogToString()
				.LWithTitle($"Differences {diffCnt} / {tree.Count()}");

		}
		isCorrect.ShouldBeTrue();
	}
}