using LayoutSystem.Flex;
using PowBasics.Geom;
using PowTrees.Algorithms;

namespace LayoutSystem.Tests.TestSupport;

static class Logger
{
	public static void LTreeFlexR(TNod<(FlexNode, R)> tree, string title)
	{
		var treeStr = tree.LogToString(opt => opt.FormatFun = t => $"{t.Item1}".PadRight(32) + $" -> {t.Item2}");
		LTitle(title);
		L(treeStr);
		L("");
	}
}