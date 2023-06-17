using System.Drawing;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LayoutSystem.Utils.JsonUtils.Converters;

sealed class ColorConverter : JsonConverter<Color>
{
	private record Tup(byte A, byte R, byte G, byte B);

	public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var obj = doc.Deserialize<Tup>(options)!;
		return Color.FromArgb(obj.A, obj.R, obj.G, obj.B);
	}

	public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
	{
		var obj = new Tup(value.A, value.R, value.G, value.B);
		JsonSerializer.Serialize(writer, obj, options);
	}
}