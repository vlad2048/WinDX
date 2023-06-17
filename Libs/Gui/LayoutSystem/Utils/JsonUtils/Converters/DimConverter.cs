using System.Text.Json;
using System.Text.Json.Serialization;
using LayoutSystem.Flex.Structs;

namespace LayoutSystem.Utils.JsonUtils.Converters;

sealed class DimConverter : JsonConverter<FDim>
{
	private sealed record Tup(int Min, int Max);

	public override FDim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var obj = doc.Deserialize<Tup>(options)!;
		return new FDim(obj.Min, obj.Max);
	}

	public override void Write(Utf8JsonWriter writer, FDim value, JsonSerializerOptions options)
	{
		var obj = new Tup(value.Min, value.Max);
		JsonSerializer.Serialize(writer, obj, options);
	}
}