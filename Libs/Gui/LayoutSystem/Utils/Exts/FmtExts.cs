using LayoutSystem.Flex.LayStrats;
using PowBasics.Geom;

namespace LayoutSystem.Utils.Exts;

static class FmtExts
{
	public static string Fmt(this Dir dir) => dir switch
	{
		Dir.Horz => "⬌",
		Dir.Vert => "⬍",
	};

	// @formatter:off
	public static string Fmt(this Align align) => align switch
	{
		Align.Start		=> "▄__",
		Align.Middle	=> "_▄_",
		Align.End		=> "__▄",
		Align.Stretch	=> "▄▄▄",
	};
	// @formatter:on
}