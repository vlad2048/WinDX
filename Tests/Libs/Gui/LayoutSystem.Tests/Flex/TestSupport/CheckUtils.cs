using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex.TreeLogic;
using PowTrees.Algorithms;
using PowBasics.Geom;
using Shouldly;
using TestBase;

namespace LayoutSystem.Tests.Flex.TestSupport;

static class CheckUtils
{
	public static void Check(
		FreeSz winSz,
		TNod<FlexNodeFluent> treeRaw,
		TNod<R> expRTree
	)
	{
		var tree = treeRaw.Map(e => e.Build());
		CheckSingle(winSz, tree, expRTree, false);
		CheckSingle(winSz, tree, expRTree, true);
	}


	private static void CheckSingle(
		FreeSz winSz,
		Node treeRaw,
		TNod<R> expRTree,
		bool transpose
	)
	{
		LBigTitle(transpose ? "Transpose Test" : "Straight Test");
		winSz = winSz.Transpose(transpose);
		treeRaw = treeRaw.Transpose(transpose);
		expRTree = expRTree.Transpose(transpose);

		var (tree, _) = treeRaw.RespectRules(winSz);

		treeRaw.LTree("Flex tree (before fixes)");
		tree.LTree("Flex tree");

		var layout = FlexSolver.Solve(treeRaw, winSz);
		var actRTree = layout.Root.MapN(node => layout.RMap[node]);

		expRTree.LTree("Expected", e => e.fmt());
		actRTree.LTree("Actual", e => e.fmt());
		var isCorrect = actRTree.IsEqual(expRTree);
		if (isCorrect)
		{
			L("-> OK");
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



	private static Node Transpose(this Node root, bool transpose) => transpose switch
	{
		false => root,
		true => root.Transpose()
	};

	private static FreeSz Transpose(this FreeSz sz, bool transpose) => transpose switch
	{
		false => sz,
		true => sz.Transpose()
	};

	private static TNod<R> Transpose(this TNod<R> root, bool transpose) => transpose switch
	{
		false => root,
		true => root.Transpose()
	};
}