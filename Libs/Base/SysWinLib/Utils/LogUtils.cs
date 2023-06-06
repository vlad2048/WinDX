namespace SysWinLib.Utils;

public static class LogUtils
{
	public static void L(string s)
	{
		//Debug.WriteLine(s);
		Console.WriteLine(s);
	}

	public static void LTitle(string s)
	{
		L("");
		L(s);
		L(new string('=', s.Length));
	}
}