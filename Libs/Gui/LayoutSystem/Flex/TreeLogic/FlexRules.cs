using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Flex.TreeLogic;



sealed class Warn
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


sealed record Fix(
	Warn Warn,
	FlexNode Flex
);




static class FlexRules
{
	public static Fix? NoFilInFit(this Node node)
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


	public static Fix? NoFilInScroll(this Node node)
	{
		if (node.Parent == null) return null;
		var parentScroll = node.Parent.V.Flags.Scroll;
		var kidDim = node.V.Dim;

		var fixX = parentScroll.X && kidDim.X.IsFil();
		var fixY = parentScroll.Y && kidDim.Y.IsFil();
		if (!fixX && !fixY) return null;
		return new Fix(
			new Warn(fixX, fixY, WarningType.NoFilInScroll, "You cannot have a Fil inside a Scroll (equivalent to Fit)"),
			node.V with { Dim = kidDim.FixDim(fixX, fixY) }
		);
	}


	public static Fix? WrapIsFilFit(this Node node)
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
			truer => n with { Dim = DimVecMaker.MkDir(mainDir, D.Fix(FlexRulesUtils.FALLBACK_LENGTH), null) },
		};
		return new Fix(
			new Warn(fixX, fixY, WarningType.WrapIsFilFit, "A Wrap node needs to be Fil on its MainDir and Fit on its ElseDir"),
			fixedNode
		);
	}


	public static Fix? NoFilInWrap(this Node node)
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


	public static Fix? PopIsNotFil(this Node node)
	{
		var n = node.V;
		if (!n.Flags.Pop) return null;
		var fixX = n.Dim.X.Typ() == DimType.Fil;
		var fixY = n.Dim.Y.Typ() == DimType.Fil;
		if (!fixX && !fixY) return null;
		return new Fix(
			
			new Warn(fixX, fixY, WarningType.PopIsNotFil, "A Fill Pop node cannot have a Fil in any direction"),
			n with { Dim = n.Dim.FixDim(fixX, fixY) }
		);
	}
}




file static class FlexRulesUtils
{
	public const int FALLBACK_LENGTH = 50;

	
	public static DimVec FixDim(this DimVec dim, bool fixX, bool fixY) => (fixX, fixY) switch
	{
		(false, false) => throw new ArgumentException("This function shouldn't be called if there's nothing to fix"),
		(truer, false) => dim with { X = D.Fix(FALLBACK_LENGTH) },
		(false, truer) => dim with { Y = D.Fix(FALLBACK_LENGTH) },
		(truer, truer) => Vec.Fix(FALLBACK_LENGTH, FALLBACK_LENGTH)
	};
}