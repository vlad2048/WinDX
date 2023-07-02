namespace ControlSystem.Logic.Scrolling_.Structs;

interface IScrollCmd
{
	ScrollBtnDecInc DecInc { get; }
}

sealed record UnitScrollCmd(ScrollBtnDecInc DecInc) : IScrollCmd;
sealed record WheelScrollCmd(ScrollBtnDecInc DecInc) : IScrollCmd;
sealed record PageScrollCmd(ScrollBtnDecInc DecInc) : IScrollCmd;


static class ScrollCmdExt
{
	public static int GetSign(this IScrollCmd cmd) => cmd.DecInc switch
	{
		ScrollBtnDecInc.Dec => -1,
		ScrollBtnDecInc.Inc => +1,
	};
}