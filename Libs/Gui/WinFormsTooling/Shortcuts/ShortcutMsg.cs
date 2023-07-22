namespace WinFormsTooling.Shortcuts;

public sealed record ShortcutMsg(
	Keys Key
)
{
	public bool Handled { get; set; }
}