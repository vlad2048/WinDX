using PowBasics.Geom;
using PowTrees.Algorithms;

namespace TestBase;

public static class TestLogger
{
	// Basic log
	// =========
	public static void L(string s) => Console.WriteLine(s);


	// Titles
	// ======
	public static void LTitle(string s)
	{
		var pad = new string('=', s.Length);
		L(s);
		L(pad);
	}

	public static void LBigTitle(string s)
	{
		var pad = new string('*', s.Length + 4);
		L("");
		L(pad);
		L($"* {s} *");
		L(pad);
	}

	public static void LWithTitle(this string s, string title)
	{
		LTitle(title);
		L(s);
	}


	// Array
	// =====
	public static void LArr<T>(IEnumerable<T> source, string title)
	{
		var arr = source.ToArray();
		L("");
		LTitle($"{title} (x{arr.Length})");
		for (var i = 0; i < arr.Length; i++)
			L($"  {arr[i]}");
		L("");
	}


	// Trees
	// =====
	public static void LTree<T>(this TNod<T> root, string title, Func<T, string>? strFun = null)
	{
		LTitle(title);
		L(root.LogToString(opt => opt.FormatFun = strFun));
		L("");
	}
}