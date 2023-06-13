namespace WinSpectorLib;

public static class WinSpector
{
	public static void Run()
	{
		var winSpectorWin = new WinSpectorWin();
		Application.Run(winSpectorWin);
	}
}