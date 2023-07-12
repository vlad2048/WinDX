using ControlSystem.Logic.Scrolling_.Structs.Enum;
using PowBasics.Geom;

namespace ControlSystem.Logic.Scrolling_.Structs;

interface IScrollCmd
{
	Dir Dir { get; }
	ScrollBtnDecInc DecInc { get; }
}

sealed record UnitScrollCmd(Dir Dir, ScrollBtnDecInc DecInc) : IScrollCmd
{
	public override string ToString() => $"ScrollUnit {DecInc.Fmt(Dir)}";
}
sealed record WheelScrollCmd(Dir Dir, ScrollBtnDecInc DecInc) : IScrollCmd
{
	public override string ToString() => $"ScrollWheel {DecInc.Fmt(Dir)}";
}
sealed record PageScrollCmd(Dir Dir, ScrollBtnDecInc DecInc) : IScrollCmd
{
	public override string ToString() => $"ScrollPage {DecInc.Fmt(Dir)}";
}


static class ScrollCmdExt
{
	public static int GetSign(this IScrollCmd cmd) => cmd.DecInc switch
	{
		ScrollBtnDecInc.Dec => -1,
		ScrollBtnDecInc.Inc => +1,
	};
}
