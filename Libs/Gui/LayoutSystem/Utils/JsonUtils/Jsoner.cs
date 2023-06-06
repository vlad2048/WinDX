using System.Text.Json;
using LayoutSystem.Flex;
using LayoutSystem.Utils.JsonUtils.Converters;
using PowBasics.Geom.Serializers;
using PowMaybeErr;
using PowTrees.Serializer;

namespace LayoutSystem.Utils.JsonUtils;

public static class Jsoner
{
    private static readonly JsonSerializerOptions jsonOpt = new()
    {
        WriteIndented = true,
    };

    static Jsoner()
    {
        jsonOpt.Converters.Add(new TNodSerializer<FlexNode>());
        jsonOpt.Converters.Add(new DimConverter());
        jsonOpt.Converters.Add(new StratConverter());
        jsonOpt.Converters.Add(new FreeSzConverter());
        jsonOpt.Converters.Add(new MargConverter());
        jsonOpt.Converters.Add(new PtSerializer());
        jsonOpt.Converters.Add(new RSerializer());
        jsonOpt.Converters.Add(new SzSerializer());
    }

    public static string Ser<T>(T obj) => JsonSerializer.Serialize(obj, jsonOpt);

    public static T Deser<T>(string str) => JsonSerializer.Deserialize<T>(str, jsonOpt)!;

    public static void Save<T>(string filename, T obj)
    {
	    var str = Ser(obj);
		File.WriteAllText(filename, str);
    }

    public static MaybeErr<T> Load<T>(string filename)
    {
	    if (!File.Exists(filename)) return MayErr.None<T>($"File not found ({filename})");
	    try
	    {
		    var str = File.ReadAllText(filename);
		    var obj = Deser<T>(str);
		    if (obj is null) return MayErr.None<T>($"Deserialization returned null ({filename})");
		    return MayErr.Some(obj);
	    }
	    catch (Exception ex)
	    {
		    return MayErr.None<T>($"Error deserializing ({filename}): {ex.Message}");
	    }
    }
}
