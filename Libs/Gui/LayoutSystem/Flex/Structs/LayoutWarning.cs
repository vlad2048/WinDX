namespace LayoutSystem.Flex.Structs;


[Flags]
public enum WarningDir
{
	Horz = 1,
	Vert = 2,
}

public record FlexWarning(
	WarningDir Dir,
	DimVec FixedDim,
	string[] Messages
);
