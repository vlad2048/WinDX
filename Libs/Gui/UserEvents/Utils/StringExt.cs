namespace UserEvents.Utils;

public static class StringExt
{
	public static string FmtOrEmpty(this string? str, Func<string, string> fun) => str switch
	{
		not null => fun(str),
		null => string.Empty
	};
}