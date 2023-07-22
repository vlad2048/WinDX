using System.Text.Json.Serialization;

namespace FlexBuilder.StructsNodes;

[JsonDerivedType(typeof(FillDrawCmd), typeDiscriminator: "Fill")]
[JsonDerivedType(typeof(BmpDrawCmd), typeDiscriminator: "Bmp")]
interface IDrawCmd { }

sealed record FillDrawCmd(Color Color) : IDrawCmd;

sealed record BmpDrawCmd(string ImageFile) : IDrawCmd;
