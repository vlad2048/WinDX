using System.Text.Json;
using System.Text.Json.Serialization;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Utils.JsonUtils.Converters;

sealed class FreeSzConverter : JsonConverter<FreeSz>
{
	private record Tup(int X, int Y);

	public override FreeSz Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var obj = doc.Deserialize<Tup>(options)!;
		return new FreeSz(obj.X, obj.Y);
	}

	public override void Write(Utf8JsonWriter writer, FreeSz value, JsonSerializerOptions options)
	{
		var obj = new Tup(value.IsInfinite(Dir.Horz) ? int.MaxValue : value.X, value.IsInfinite(Dir.Vert) ? int.MaxValue : value.Y);
		JsonSerializer.Serialize(writer, obj, options);
	}
}