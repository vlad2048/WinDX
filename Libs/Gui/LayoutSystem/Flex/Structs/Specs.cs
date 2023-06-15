namespace LayoutSystem.Flex.Structs;

public interface ISpec { }

/// <summary>
/// Represents a scrollable Fill node <br/>
/// Dim cannot be Fit in an enabled direction
/// </summary>
public readonly record struct ScrollSpec(BoolVec Enabled) : ISpec;

/// <summary>
/// Represents a popup Fill node <br/>
/// Dim cannot be Fil in any direction
/// </summary>
public readonly record struct PopSpec : ISpec;
