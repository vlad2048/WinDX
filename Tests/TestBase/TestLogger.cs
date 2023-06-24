namespace TestBase;

public static class TestLogger
{
	public static void LArr<T>(IEnumerable<T> source, string title)
	{
		var arr = source.ToArray();
		LTitle($"{title} (x{arr.Length})");
		for (var i = 0; i < arr.Length; i++)
			L($"  [{i}]: {arr[i]}");
		L("");
	}

	public static void LTitle(string s)
	{
		var pad = new string('=', s.Length);
		L(s);
		L(pad);
	}

	public static void L(string s) => Console.WriteLine(s);
}