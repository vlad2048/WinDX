using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;

namespace ControlSystem;

public sealed class RenderStFlexNodeFluent
{
	internal RenderArgs RenderArgs { get; }
	internal NodeState NodeState { get; }

	internal DimVec DimVal = Vec.Fil;
	internal FlexFlags FlagsVal = FlexFlags.None;
	internal IStrat StratVal = Strats.Fill;
	internal Marg MargVal = Mg.Zero;

	public RenderStFlexNodeFluent(RenderArgs renderArgs, NodeState nodeState)
	{
		RenderArgs = renderArgs;
		NodeState = nodeState;
	}

	public IDisposable M => RenderArgs.Flex(new StFlexNode(
		NodeState,
		new FlexNode(
			DimVal,
			FlagsVal,
			StratVal,
			MargVal
		)
	));
	

	public RenderStFlexNodeFluent Dim(DimVec dim)
	{
		DimVal = dim;
		return this;
	}

	public RenderStFlexNodeFluent Flags(FlexFlags flags)
	{
		FlagsVal = flags;
		return this;
	}

	public RenderStFlexNodeFluent Strat(IStrat strat)
	{
		StratVal = strat;
		return this;
	}

	public RenderStFlexNodeFluent Marg(Marg marg)
	{
		MargVal = marg;
		return this;
	}



	public RenderStFlexNodeFluent DimFit() => Dim(Vec.Fit);
	public RenderStFlexNodeFluent Dim(int x, int y) => Dim(Vec.Fix(x, y));
	public RenderStFlexNodeFluent DimFil() => Dim(Vec.Fil);
	public RenderStFlexNodeFluent DimFitFil() => Dim(Vec.FitFil);
	public RenderStFlexNodeFluent DimFilFit() => Dim(Vec.FilFit);
	public RenderStFlexNodeFluent DimFilFix(int y) => Dim(Vec.FilFix(y));
	public RenderStFlexNodeFluent DimFixFil(int x) => Dim(Vec.FixFil(x));

	public RenderStFlexNodeFluent ScrollX() => Flags(FlagsVal with { Scroll = new BoolVec(true, false) });
	public RenderStFlexNodeFluent ScrollY() => Flags(FlagsVal with { Scroll = new BoolVec(false, true) });
	public RenderStFlexNodeFluent ScrollXY() => Flags(FlagsVal with { Scroll = new BoolVec(true, true) });
	public RenderStFlexNodeFluent Pop() => Flags(FlagsVal with { Pop = true });

	public RenderStFlexNodeFluent StratFill() => Strat(Strats.Fill);
	public RenderStFlexNodeFluent StratStack(Dir mainDir, Align align = Align.Start) => Strat(Strats.Stack(mainDir, align));
	public RenderStFlexNodeFluent StratWrap(Dir mainDir) => Strat(Strats.Wrap(mainDir));

	public RenderStFlexNodeFluent Marg(int v) => Marg(Mg.Mk(v));
	public RenderStFlexNodeFluent Marg(int horz, int vert) => Marg(new Marg(vert, horz, vert, horz));
	public RenderStFlexNodeFluent Marg(int top, int right, int bottom, int left) => Marg(new Marg(top, right, bottom, left));
}
