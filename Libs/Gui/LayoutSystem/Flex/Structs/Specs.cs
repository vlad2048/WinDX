using System.Text.Json.Serialization;

namespace LayoutSystem.Flex.Structs;

[JsonDerivedType(typeof(ScrollSpec), typeDiscriminator: "Scroll")]
[JsonDerivedType(typeof(PopSpec), typeDiscriminator: "Pop")]
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
