using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowTrees.Algorithms;

namespace LayoutSystem.Flex.TreeLogic;

static class TreeOps
{
	/// <summary>
	/// Preprocesses the layout tree to avoid non-sensical combinations
	/// </summary>
	public static (Node, IReadOnlyDictionary<Node, LayoutWarning[]>) RespectRules(this Node root, FreeSz freeSz)
	{
		var warnings = new Dictionary<int, List<LayoutWarning>>();
		var respectfulRoot = root
			.Clone()
			.SetRootDims(freeSz)
			.CheckRules(warnings);
		return (
			respectfulRoot,
			warnings.MapBack(root)
		);
	}
}


file static class TreeOpsUtils
{
	private const int FALLBACK_LENGTH = 50;

	private record Fix(
		LayoutWarning Warning,
		FlexNode Flex
	);

	/// <summary>
	/// The only Dims that make sense for the root node are to match the window size
	/// TODO: add support for Fit dims to size the window on the content of the layout
	/// </summary>
	public static Node SetRootDims(this Node root, FreeSz freeSz) => Nod.Make(
		root.V with { Dim = freeSz.GetDims() },
		root.Children
	);

	public static Node CheckRules(this Node root, Dictionary<int, List<LayoutWarning>> warnings) =>
		root
			.Check(warnings, FixNoFilInFit)
			.Check(warnings, FixNoFilInScroll)
			.Check(warnings, FixWrapDims)
			.Check(warnings, FixWrapFilKids);


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
			LayoutWarning.MakeWithDirs(fixX, fixY, "You cannot have a Fil inside a Fit"),
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
		if (node.Parent == null) return null;
		if (node.Parent.V.Strat is not FillStrat { ScrollEnabled: var scrollEnabled }) return null;
		var n = node.V;
		var kd = n.Dim;
		var fixX = scrollEnabled.X && kd.X.IsFil();
		var fixY = scrollEnabled.Y && kd.Y.IsFil();
		if (!fixX && !fixY) return null;
		return new Fix(
			LayoutWarning.MakeWithDirs(fixX, fixY, "You cannot have a Fil inside a Scroll (equivalent to Fit)"),
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
			LayoutWarning.MakeWithDirs(fixX, fixY, "A Wrap node needs to be Fil on its MainDir and Fit on its ElseDir"),
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
			LayoutWarning.MakeWithDirs(fixX, fixY, "A Wrap node kid cannot have a Fil in any direction"),
			n with { Dim = Vec.Fix(FALLBACK_LENGTH, FALLBACK_LENGTH) }
		);
	}



	private static Node Check(
		this Node root,
		Dictionary<int, List<LayoutWarning>> warnings,
		Func<Node, Fix?> fixFun
	) =>
		root.MapNIdx((node, nodeIdx) =>
		{
			var fix = fixFun(node);
			if (fix == null) return node.V;
			warnings.AddWarning(nodeIdx, fix.Warning);
			return fix.Flex;
		});
}