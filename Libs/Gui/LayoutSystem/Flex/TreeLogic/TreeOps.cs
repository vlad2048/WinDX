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
	private const int FALLBACK_LENGTH = 50;

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
			.Check(warnMap, Rule_NoFilInFit)
			.Check(warnMap, Rule_NoFilInScroll)
			.Check(warnMap, Rule_WrapIsFilFit)
			.Check(warnMap, Rule_NoFilInWrap)
			.Check(warnMap, Rule_PopIsNotFil);
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
		public WarningType Type { get; }
		public Warn(bool fixX, bool fixY, WarningType type, string message)
		{
			Dir = (fixX ? WarningDir.Horz : 0) | (fixY ? WarningDir.Vert : 0);
			Type = type;
			Message = message;
		}
	}

	private record Fix(
		Warn Warn,
		FlexNode Flex
	);



	private static FlexWarning MakeWarning(IReadOnlyList<Warn> warns, Node node) => new(
		warns.Select(e => e.Type).Aggregate((WarningType)0, (d1, d2) => d1 | d2),
		warns.Select(e => e.Dir).Aggregate((WarningDir)0, (d1, d2) => d1 | d2),
		node.V.Dim,
		warns.SelectToArray(e => e.Message)
	);

	private static Fix? Rule_NoFilInFit(this Node node)
	{
		if (node.Parent == null) return null;
		var pd = node.Parent.V.Dim;
		var n = node.V;
		var kd = n.Dim;
		var fixX = pd.X.IsFit() && kd.X.IsFil();
		var fixY = pd.Y.IsFit() && kd.Y.IsFil();
		if (!fixX && !fixY) return null;
		return new Fix(
			new Warn(fixX, fixY, WarningType.NoFilInFit, "You cannot have a Fil inside a Fit"),
			n with { Dim = n.Dim.FixDim(fixX, fixY) }
		);
	}


	private static Fix? Rule_NoFilInScroll(this Node node)
	{
		if (node.Parent?.V.Strat is not FillStrat { Spec: ScrollSpec { Enabled: var scrollEnabled } }) return null;
		var n = node.V;
		var kd = n.Dim;
		var fixX = scrollEnabled.X && kd.X.IsFil();
		var fixY = scrollEnabled.Y && kd.Y.IsFil();
		if (!fixX && !fixY) return null;
		return new Fix(
			new Warn(fixX, fixY, WarningType.NoFilInScroll, "You cannot have a Fil inside a Scroll (equivalent to Fit)"),
			n with { Dim = n.Dim.FixDim(fixX, fixY) }
		);
	}


	private static Fix? Rule_WrapIsFilFit(this Node node)
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
			new Warn(fixX, fixY, WarningType.WrapIsFilFit, "A Wrap node needs to be Fil on its MainDir and Fit on its ElseDir"),
			fixedNode
		);
	}


	private static Fix? Rule_NoFilInWrap(this Node node)
	{
		if (node.Parent?.V.Strat is not WrapStrat) return null;
		var n = node.V;
		var fixX = n.Dim.X.Typ() == DimType.Fil;
		var fixY = n.Dim.Y.Typ() == DimType.Fil;
		if (!fixX && !fixY) return null;
		return new Fix(
			
			new Warn(fixX, fixY, WarningType.NoFilInWrap, "A Wrap node kid cannot have a Fil in any direction"),
			n with { Dim = n.Dim.FixDim(fixX, fixY) }
		);
	}


	private static Fix? Rule_PopIsNotFil(this Node node)
	{
		if (node.V.Strat is not FillStrat { Spec: PopSpec }) return null;
		var n = node.V;
		var fixX = n.Dim.X.Typ() == DimType.Fil;
		var fixY = n.Dim.Y.Typ() == DimType.Fil;
		if (!fixX && !fixY) return null;
		return new Fix(
			
			new Warn(fixX, fixY, WarningType.PopIsNotFil, "A Fill Pop node cannot have a Fil in any direction"),
			n with { Dim = n.Dim.FixDim(fixX, fixY) }
		);
	}



	private static DimVec FixDim(this DimVec dim, bool fixX, bool fixY) => (fixX, fixY) switch
	{
		(false, false) => throw new ArgumentException("This function shouldn't be called if there's nothing to fix"),
		(truer, false) => dim with { X = D.Fix(FALLBACK_LENGTH) },
		(false, truer) => dim with { Y = D.Fix(FALLBACK_LENGTH) },
		(truer, truer) => Vec.Fix(FALLBACK_LENGTH, FALLBACK_LENGTH)
	};



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