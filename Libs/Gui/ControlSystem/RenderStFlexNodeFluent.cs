using ControlSystem.Logic.Rendering_;
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
	public RenderStFlexNodeFluent DimFixFil(int x) => Dim(Vec.FixFil(x));
	public RenderStFlexNodeFluent DimFilFix(int y) => Dim(Vec.FilFix(y));
	public RenderStFlexNodeFluent DimFixFit(int x) => Dim(Vec.FixFit(x));
	public RenderStFlexNodeFluent DimFitFix(int y) => Dim(Vec.FitFix(y));

	public RenderStFlexNodeFluent Dim(Dir dir, int x, int y) => Dim(Vec.Fix(dir, x, y));
	public RenderStFlexNodeFluent DimFitFil(Dir dir) => Dim(Vec.FitFilD(dir));
	public RenderStFlexNodeFluent DimFilFit(Dir dir) => Dim(Vec.FilFitD(dir));
	public RenderStFlexNodeFluent DimFixFil(Dir dir, int x) => Dim(Vec.FixFil(dir, x));
	public RenderStFlexNodeFluent DimFilFix(Dir dir, int y) => Dim(Vec.FilFix(dir, y));
	public RenderStFlexNodeFluent DimFixFit(Dir dir, int x) => Dim(Vec.FixFit(dir, x));
	public RenderStFlexNodeFluent DimFitFix(Dir dir, int y) => Dim(Vec.FitFix(dir, y));

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

	public RenderStFlexNodeFluent Marg(Dir dir, int horz, int vert) => dir switch
	{
		Dir.Horz => Marg(new Marg(vert, horz, vert, horz)),
		Dir.Vert => Marg(new Marg(horz, vert, horz, vert)),
	};
}
