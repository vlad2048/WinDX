namespace WinSpectorLib.Structs;

sealed record ShortcutMsg(
	Keys Key
)
{
	public bool Handled { get; set; }
}