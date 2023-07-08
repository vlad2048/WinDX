namespace Logging;

public static class LogUtils
{
	public static void L(string s)
	{
		Console.WriteLine($"{Thread} {Timestamp} {s}");
	}

	public static void LTitle(string s)
	{
		L("");
		L(s);
		L(new string('=', s.Length));
	}

	private static string Thread
	{
		get
		{
			var thread = System.Threading.Thread.CurrentThread;
			var str = $"{thread.ManagedThreadId}/{thread.Name}".PadRight(24);
			return $"[{str}]";
		}
	}

	private static string Timestamp => $"[{DateTime.Now:HH:mm:ss.fffffff}]";
}