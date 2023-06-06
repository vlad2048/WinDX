using PowBasics.Geom;

namespace LayoutSystem.Flex.Structs;

[Flags]
public enum WarningDir
{
	Horz = 1,
	Vert = 2,
}

public record LayoutWarning(
	WarningDir Dir,
	string Message
)
{
	public static LayoutWarning MakeWithDirs(bool fixX, bool fixY, string message) => new(
		(fixX ? WarningDir.Horz : 0) | (fixY ? WarningDir.Vert : 0),
		message
	);
}

public record Layout(
	Node Root,
	IReadOnlyDictionary<Node, R> RMap,
	IReadOnlyDictionary<Node, LayoutWarning[]> WarningMap
)
{
	public Sz TotalSz => RMap[Root].Size;
}


static class LayoutWarningExts
{
	public static void AddWarning(
		this Dictionary<int, List<LayoutWarning>> map,
		int nodeIdx,
		LayoutWarning warning
	)
	{
		if (!map.TryGetValue(nodeIdx, out var list))
			map[nodeIdx] = list = new List<LayoutWarning>();
		list.Add(warning);
	}

	public static IReadOnlyDictionary<Node, LayoutWarning[]> MapBack(
		this Dictionary<int, List<LayoutWarning>> map,
		Node root
	)
	{
		var lookup = root
			.Select((node, idx) => (node, idx))
			.ToDictionary(e => e.idx, e => e.node);
		return map.ToDictionary(e => lookup[e.Key], e => e.Value.ToArray());
	}
}