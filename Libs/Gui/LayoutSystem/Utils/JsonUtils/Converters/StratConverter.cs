using System.Text.Json;
using System.Text.Json.Serialization;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.LayStratsInternal;
using PowBasics.Geom;

namespace LayoutSystem.Utils.JsonUtils.Converters;

public class StratConverter : JsonConverter<IStrat>
{
	private enum StratType
	{
		Fill,
		Stack,
		Wrap,
		Margin,
	}

	private record StratNfo(
		StratType Type,
		Dir StackMainDir,
		Align StackAlign,
		Dir WrapMainDir
	)
	{
		public static readonly StratNfo Empty = new(StratType.Fill, Dir.Horz, Align.Start, Dir.Horz);
	}

	private static StratNfo Strat2Nfo(IStrat s) => s switch
	{
		FillStrat => StratNfo.Empty with
		{
			Type = StratType.Fill
		},
		StackStrat { MainDir: var mainDir, Align: var align } => StratNfo.Empty with
		{
			Type = StratType.Stack,
			StackMainDir = mainDir,
			StackAlign = align
		},
		WrapStrat { MainDir: var mainDir } => StratNfo.Empty with
		{
			Type = StratType.Wrap,
			WrapMainDir = mainDir
		},
		MarginStrat => StratNfo.Empty with
		{
			Type = StratType.Margin
		},
		_ => throw new ArgumentException()
	};

	private static IStrat Nfo2Strat(StratNfo s) => s.Type switch
	{
		StratType.Fill => new FillStrat(),
		StratType.Stack => new StackStrat(s.StackMainDir, s.StackAlign),
		StratType.Wrap => new WrapStrat(s.WrapMainDir),
		StratType.Margin => new MarginStrat(),
		_ => throw new ArgumentException()
	};

	public override IStrat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var obj = doc.Deserialize<StratNfo>(options)!;
		return Nfo2Strat(obj);
	}

	public override void Write(Utf8JsonWriter writer, IStrat value, JsonSerializerOptions options)
	{
		var obj = Strat2Nfo(value);
		JsonSerializer.Serialize(writer, obj, options);
	}
}