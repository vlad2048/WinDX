using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowTrees.Algorithms;

namespace LayoutSystem.Flex.TreeLogic;

static class TreeOps
{
	/// <summary>
	/// Preprocesses the layout tree to avoid non-sensical combinations
	/// </summary>
	public static (Node, IReadOnlyDictionary<int, FlexWarning>) RespectRules(this Node root, FreeSz freeSz) =>
		root
			.Clone()
			.SetRootDims(freeSz)
			.CheckRules();
}


file static class TreeOpsUtils
{
	/// <summary>
	/// The only Dims that make sense for the root node are to match the window size
	/// TODO: add support for Fit dims to size the window on the content of the layout
	/// </summary>
	public static Node SetRootDims(this Node root, FreeSz freeSz) => Nod.Make(
		root.V with { Dim = freeSz.ToDim() },
		root.Children
	);

	public static (Node, IReadOnlyDictionary<int, FlexWarning>) CheckRules(this Node root)
	{
		var warnMap = new Dictionary<int, List<Warn>>();
		var fixedRoot = root
			.Check(warnMap, FlexRules.NoFilInFit)
			.Check(warnMap, FlexRules.NoFilInScroll)
			.Check(warnMap, FlexRules.WrapIsFilFit)
			.Check(warnMap, FlexRules.NoFilInWrap)
			.Check(warnMap, FlexRules.PopIsNotFil);
		var nodes = fixedRoot.ToArray();
		var warnings = warnMap
			.Select(kv => new
			{
				kv.Key,
				Val = MakeWarning(kv.Value, nodes[kv.Key])
			})
			.ToDictionary(e => e.Key, e => e.Val);
		return (
			fixedRoot,
			warnings
		);
	}



	

	private static FlexWarning MakeWarning(IReadOnlyList<Warn> warns, Node node) => new(
		warns.Select(e => e.Type).Aggregate((WarningType)0, (d1, d2) => d1 | d2),
		warns.Select(e => e.Dir).Aggregate((WarningDir)0, (d1, d2) => d1 | d2),
		node.V.Dim,
		warns.SelectToArray(e => e.Message)
	);








	private static Node Check(
		this Node root,
		Dictionary<int, List<Warn>> warnings,
		Func<Node, Fix?> fixFun
	) =>
		root.MapNIdx((node, nodeIdx) =>
		{
			var fix = fixFun(node);
			if (fix == null) return node.V;
			warnings.AddWarning(nodeIdx, fix.Warn);
			return fix.Flex;
		});


	private static void AddWarning(
		this Dictionary<int, List<Warn>> map,
		int nodeIdx,
		Warn warn
	)
	{
		if (!map.TryGetValue(nodeIdx, out var list))
			map[nodeIdx] = list = new List<Warn>();
		list.Add(warn);
	}
}