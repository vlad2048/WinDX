namespace LayoutSystem.Flex.Structs;


[Flags]
public enum WarningDir
{
	Horz = 1,
	Vert = 2,
}

[Flags]
public enum WarningType
{
	NoFilInFit = 1,
	NoFilInScroll = 2,
	WrapIsFilFit = 4,
	NoFilInWrap = 8,
	PopIsNotFil = 16,
}

public record FlexWarning(
	WarningType Type,
	WarningDir Dir,
	DimVec FixedDim,
	string[] Messages
);
