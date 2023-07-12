namespace ControlSystem.Logic.Scrolling_.Structs.Enum;

public enum ScrollBarState
{
    Disabled,
    NotNeeded,
    NeededButTooSmallToBeVisible,
    Visible,
}


static class ScrollBarStateExt
{
	public static bool IsEnabled(this ScrollBarState st) => st != ScrollBarState.Disabled;
	public static bool IsVisible(this ScrollBarState st) => st == ScrollBarState.Visible;
	public static string Fmt(this ScrollBarState st) => st switch
	{
		ScrollBarState.Disabled => "Disabled",
		ScrollBarState.NotNeeded => "NotNeeded",
		ScrollBarState.NeededButTooSmallToBeVisible => "TooSmall",
		ScrollBarState.Visible => "Visible"
	};
}