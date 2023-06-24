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
		actTree.LogTree("Actual");
		expTree.LogTree("Expected");
		var areSame = actTree.IsEqual(expTree);
		areSame.ShouldBeTrue("Wrong tree");
		L("Trees match ✅");
	}
}

public static class TreeLogger
{
	public static void LogTree<T>(this TNod<T> root, string title)
	{
		LTitle(title);
		L(root.LogToString());
		L("");
	}
}