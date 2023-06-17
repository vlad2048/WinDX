using LayoutSystem.Flex.Details.Structs;

namespace LayoutSystem.Flex.Details;

sealed class PageWriter
{
	private const int IndentSize = 4;

	private readonly List<Txt> curLine = new();
	private readonly List<Txt[]> lines = new();
	private int indent;

	public void Indent() => indent++;
	public void Unindent() => indent = Math.Max(0, indent - 1);

	public void Write(string text, TxtStyle style)
	{
		if (curLine.Count == 0 && indent > 0)
			curLine.Add(new Txt(new string(' ', indent * IndentSize), TxtStyle.Default));
		curLine.Add(new Txt(text, style));
	}

	public void WriteLine(string text, TxtStyle style)
	{
		Write(text, style);
		FinishLine();
	}

	public TxtPage GetPage()
	{
		if (curLine.Count > 0)
			FinishLine();
		return new TxtPage(lines.ToArray());
	}

	private void FinishLine()
	{
		lines.Add(curLine.ToArray());
		curLine.Clear();
	}
}