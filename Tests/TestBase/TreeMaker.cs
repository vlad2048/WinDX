using PowTrees.Algorithms;
using Shouldly;

namespace TestBase;

public static class TreeMaker
{
	public static TNod<int> N(int v, params TNod<int>[] children) => Nod.Make(v, children);
}

public static class TreeChecker
{
	public static void ShouldBeSameTree<T>(this TNod<T> actTree, TNod<T> expTree)
	{
		actTree.LTree("Actual");
		expTree.LTree("Expected");
		var areSame = actTree.IsEqual(expTree);
		areSame.ShouldBeTrue("Wrong tree");
		L("Trees match ✅");
	}
}
