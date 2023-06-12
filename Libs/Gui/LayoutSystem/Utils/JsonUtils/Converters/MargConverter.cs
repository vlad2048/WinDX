using System.Text.Json;
using System.Text.Json.Serialization;
using PowBasics.Geom;

namespace LayoutSystem.Utils.JsonUtils.Converters;

public sealed class MargConverter : JsonConverter<Marg>
{
	private record Tup(int Top, int Right, int Bottom, int Left);

	public override Marg Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var obj = doc.Deserialize<Tup>(options)!;
		return new Marg(obj.Top, obj.Right, obj.Bottom, obj.Left);
	}

	public override void Write(Utf8JsonWriter writer, Marg value, JsonSerializerOptions options)
	{
		var obj = new Tup(value.Top, value.Right, value.Bottom, value.Left);
		JsonSerializer.Serialize(writer, obj, options);
	}
}