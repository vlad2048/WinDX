using System.Drawing;

namespace LayoutSystem.Flex.Details.Structs;

[Flags]
public enum TxtFontStyle
{
	Regular = 0,
	Bold = 1,
	Italic = 2,
	Underline = 4,
	Strikeout = 8
}

public sealed record TxtStyle(
	Color Color,
	float Size,
	TxtFontStyle FontStyle
)
{
	public static readonly TxtStyle Default = new(Color.White, 9, TxtFontStyle.Regular);
}

public sealed record Txt(
	string Text,
	TxtStyle Style
);