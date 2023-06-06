using System.Text.Json;
using System.Text.Json.Serialization;
using LayoutSystem.Flex.Structs;

namespace LayoutSystem.Utils.JsonUtils.Converters;

public class DimConverter : JsonConverter<Dim>
{
	private record Tup(int Min, int Max);

	public override Dim Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var obj = doc.Deserialize<Tup>(options)!;
		return new Dim(obj.Min, obj.Max);
	}

	public override void Write(Utf8JsonWriter writer, Dim value, JsonSerializerOptions options)
	{
		var obj = new Tup(value.Min, value.Max);
		JsonSerializer.Serialize(writer, obj, options);
	}
}