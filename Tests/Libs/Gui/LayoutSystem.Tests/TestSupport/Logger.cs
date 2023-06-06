using LayoutSystem.Flex;
using PowBasics.Geom;
using PowTrees.Algorithms;

namespace LayoutSystem.Tests.TestSupport;

static class Logger
{
	public static void L(this string s) => Console.WriteLine(s);

	public static void LTitle(this string s)
	{
		L(s);
		L(new string('=', s.Length));
	}

	public static void LWithTitle(this string s, string title)
	{
		LTitle(title);
		L(s);
	}

	public static void L(this Node tree, string title)
	{
		var treeStr = tree.LogToString(opt => opt.FormatFun = t => $"{t}");
		LTitle(title);
		L(treeStr);
		L("");
	}
	
	public static void L(this TNod<R> tree, string title)
	{
		var treeStr = tree.LogToString();
		LTitle(title);
		L(treeStr);
		L("");
	}

	public static void L(this TNod<(FlexNode, R)> tree, string title)
	{
		var treeStr = tree.LogToString(opt => opt.FormatFun = t => $"{t.Item1}".PadRight(32) + $" -> {t.Item2}");
		LTitle(title);
		L(treeStr);
		L("");
	}
}