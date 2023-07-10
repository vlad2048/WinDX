using PowBasics.Geom;

namespace ControlSystem.Logic.Scrolling_.Structs;

enum ScrollBtnDecInc
{
	Dec,
	Inc
}


static class ScrollBtnDecIncExt
{
	public static string Fmt(this ScrollBtnDecInc d, Dir dir) => (d, dir) switch
	{
		(ScrollBtnDecInc.Dec, Dir.Horz) => "left",
		(ScrollBtnDecInc.Dec, Dir.Vert) => "up",
		(ScrollBtnDecInc.Inc, Dir.Horz) => "right",
		(ScrollBtnDecInc.Inc, Dir.Vert) => "down",
	};
}