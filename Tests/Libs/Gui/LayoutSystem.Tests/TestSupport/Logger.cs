using LayoutSystem.Flex;
using PowBasics.Geom;
using PowBasics.StringsExt;
using PowTrees.Algorithms;

namespace LayoutSystem.Tests.TestSupport;

static class Logger
{
	public static void LTreeFlexR(TNod<(FlexNode, R)> tree, string title)
	{
		var treeStr = tree.Log(opt => opt.FmtFun = t => $"{t.Item1}".PadRight(32) + $" -> {t.Item2}").JoinLines();
		LTitle(title);
		L(treeStr);
		L("");
	}
}