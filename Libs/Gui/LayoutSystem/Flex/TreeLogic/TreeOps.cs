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
	/*public static (Node, IReadOnlyDictionary<Node, FlexWarning>) RespectRules(this Node root, FreeSz freeSz) =>
		root
			.Clone()
			.SetRootDims(freeSz)
			.CheckRules();*/

	public static (Node, IReadOnlyDictionary<int, FlexWarning>) RespectRules(this Node root, FreeSz freeSz)
	{
		var (fixedRoot, warnings) =
			root
				.Clone()
				.SetRootDims(freeSz)
				.CheckRules();
		return (fixedRoot, warnings);
	}
}


file static class TreeOpsUtils
{
	private const int FALLBACK_LENGTH = 50;

	/// <summary>
	/// The only Dims that make sense for the root node are to match the window size
	/// TODO: add support for Fit dims to size the window on the content of the layout
	/// </summary>
	public static Node SetRootDims(this Node root, FreeSz freeSz) => Nod.Make(
		root.V with { Dim = freeSz.GetDims() },
		root.Children
	);

	public static (Node, IReadOnlyDictionary<int, FlexWarning>) CheckRules(this Node root)
	{
		var warnMap = new Dictionary<int, List<Warn>>();
		var fixedRoot = root
			.Check(warnMap, FixNoFilInFit)
			.Check(warnMap, FixNoFilInScroll)
			.Check(warnMap, FixWrapDims)
			.Check(warnMap, FixWrapFilKids);
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



	

	private class Warn
	{
		public WarningDir Dir { get; }
		public string Message { get; }
		public Warn(bool fixX, bool fixY, string message)
		{
			Dir = (fixX ? WarningDir.Horz : 0) | (fixY ? WarningDir.Vert : 0);
			Message = message;
		}
	}

	private record Fix(
		Warn Warn,
		FlexNode Flex
	);



	private static FlexWarning MakeWarning(IReadOnlyList<Warn> warns, Node node)
	{
		var dir = warns
			.Select(e => e.Dir)
			.Aggregate((WarningDir)0, (d1, d2) => d1 | d2);
		return new FlexWarning(
			dir,
			node.V.Dim,
			warns.SelectToArray(e => e.Message)
		);
	}

	private static Fix? FixNoFilInFit(this Node node)
	{
		if (node.Parent == null) return null;
		var pd = node.Parent.V.Dim;
		var n = node.V;
		var kd = n.Dim;
		var fixX = pd.X.IsFit() && kd.X.IsFil();
		var fixY = pd.Y.IsFit() && kd.Y.IsFil();
		if (!fixX && !fixY) return null;
		return new Fix(
			new Warn(fixX, fixY, "You cannot have a Fil inside a Fit"),
			n with
			{
				Dim = new DimVec(
					fixX ? D.Fix(FALLBACK_LENGTH) : n.Dim.X,
					fixY ? D.Fix(FALLBACK_LENGTH) : n.Dim.Y
				)
			}
		);
	}


	private static Fix? FixNoFilInScroll(this Node node)
	{
		if (node.Parent?.V.Strat is not FillStrat { ScrollEnabled: var scrollEnabled }) return null;
		var n = node.V;
		var kd = n.Dim;
		var fixX = scrollEnabled.X && kd.X.IsFil();
		var fixY = scrollEnabled.Y && kd.Y.IsFil();
		if (!fixX && !fixY) return null;
		return new Fix(
			new Warn(fixX, fixY, "You cannot have a Fil inside a Scroll (equivalent to Fit)"),
			n with
			{
				Dim = new DimVec(
					fixX ? D.Fix(FALLBACK_LENGTH) : n.Dim.X,
					fixY ? D.Fix(FALLBACK_LENGTH) : n.Dim.Y
				)
			}
		);
	}


	private static Fix? FixWrapDims(this Node node)
	{
		var n = node.V;
		if (n.Strat is not WrapStrat { MainDir: var mainDir }) return null;
// @formatter:off
		var fixX = n.Dim.Dir(mainDir      ).Typ() != DimType.Fil;
		var fixY = n.Dim.Dir(mainDir.Neg()).Typ() != DimType.Fit;
// @formatter:on
		if (!fixX && !fixY) return null;
		var isParentFitInMainDir = node.Parent?.V.Dim.Dir(mainDir).Typ() == DimType.Fit;
		var fixedNode = isParentFitInMainDir switch
		{
			false => n with { Dim = DimVecMaker.MkDir(mainDir, D.Fil, null) },
			truer => n with { Dim = DimVecMaker.MkDir(mainDir, D.Fix(FALLBACK_LENGTH), null) },
		};
		return new Fix(
			new Warn(fixX, fixY, "A Wrap node needs to be Fil on its MainDir and Fit on its ElseDir"),
			fixedNode
		);
	}


	private static Fix? FixWrapFilKids(this Node node)
	{
		if (node.Parent?.V.Strat is not WrapStrat) return null;
		var n = node.V;
		var fixX = n.Dim.X.Typ() == DimType.Fil;
		var fixY = n.Dim.Y.Typ() == DimType.Fil;
		if (!fixX && !fixY) return null;
		return new Fix(
			
			new Warn(fixX, fixY, "A Wrap node kid cannot have a Fil in any direction"),
			n with { Dim = Vec.Fix(FALLBACK_LENGTH, FALLBACK_LENGTH) }
		);
	}



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