namespace LayoutSystem.Flex.Structs;


[Flags]
public enum WarningDir
{
	Horz = 1,
	Vert = 2,
}


public record LayoutWarning(
	WarningDir Dir,
	string Message
)
{
	public static LayoutWarning MakeWithDirs(bool fixX, bool fixY, string message) => new(
		(fixX ? WarningDir.Horz : 0) | (fixY ? WarningDir.Vert : 0),
		message
	);
}