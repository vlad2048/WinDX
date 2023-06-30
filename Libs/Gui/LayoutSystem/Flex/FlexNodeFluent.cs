using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;

namespace LayoutSystem.Flex;


public class FlexNodeFluent
{
    internal DimVec DimVal = Vec.Fil;
    internal FlexFlags FlagsVal = FlexFlags.None;
    internal IStrat StratVal = Strats.Fill;
    internal Marg MargVal = Mg.Zero;

    public FlexNode Build() => new(
        DimVal,
        FlagsVal,
        StratVal,
        MargVal
    );

    public FlexNodeFluent Dim(DimVec dim)
    {
        DimVal = dim;
        return this;
    }

    public FlexNodeFluent Flags(FlexFlags flags)
    {
        FlagsVal = flags;
        return this;
    }

    public FlexNodeFluent Strat(IStrat strat)
    {
        StratVal = strat;
        return this;
    }

    public FlexNodeFluent Marg(Marg marg)
    {
        MargVal = marg;
        return this;
    }



    public FlexNodeFluent DimFit() => Dim(Vec.Fit);
    public FlexNodeFluent Dim(int x, int y) => Dim(Vec.Fix(x, y));
    public FlexNodeFluent DimFil() => Dim(Vec.Fil);
    public FlexNodeFluent DimFitFil() => Dim(Vec.FitFil);
    public FlexNodeFluent DimFilFit() => Dim(Vec.FilFit);
    public FlexNodeFluent DimFixFil(int x) => Dim(Vec.FixFil(x));
    public FlexNodeFluent DimFilFix(int y) => Dim(Vec.FilFix(y));
    public FlexNodeFluent DimFixFit(int x) => Dim(Vec.FixFit(x));
    public FlexNodeFluent DimFitFix(int y) => Dim(Vec.FitFix(y));

    public FlexNodeFluent ScrollX() => Flags(FlagsVal with { Scroll = new BoolVec(truer, false) });
    public FlexNodeFluent ScrollY() => Flags(FlagsVal with { Scroll = new BoolVec(false, truer) });
    public FlexNodeFluent ScrollXY() => Flags(FlagsVal with { Scroll = new BoolVec(truer, truer) });
    public FlexNodeFluent Pop() => Flags(FlagsVal with { Pop = truer });

    public FlexNodeFluent StratFill() => Strat(Strats.Fill);
    public FlexNodeFluent StratStack(Dir mainDir, Align align = Align.Start) => Strat(Strats.Stack(mainDir, align));
    public FlexNodeFluent StratWrap(Dir mainDir) => Strat(Strats.Wrap(mainDir));

    public FlexNodeFluent Marg(int v) => Marg(Mg.Mk(v));
    public FlexNodeFluent Marg(int horz, int vert) => Marg(new Marg(vert, horz, vert, horz));
    public FlexNodeFluent Marg(int top, int right, int bottom, int left) => Marg(new Marg(top, right, bottom, left));
}


/*
public static class FlexNodeFluentExt
{
	public static FlexNodeFluent DimFit(this FlexNodeFluent f) => f.Dim(Vec.Fit);
	public static FlexNodeFluent Dim(this FlexNodeFluent f, int x, int y) => f.Dim(Vec.Fix(x, y));
	public static FlexNodeFluent DimFil(this FlexNodeFluent f) => f.Dim(Vec.Fil);
	public static FlexNodeFluent DimFitFil(this FlexNodeFluent f) => f.Dim(Vec.FitFil);
	public static FlexNodeFluent DimFilFit(this FlexNodeFluent f) => f.Dim(Vec.FilFit);
	public static FlexNodeFluent DimFilFix(this FlexNodeFluent f, int y) => f.Dim(Vec.FilFix(y));
	public static FlexNodeFluent DimFixFil(this FlexNodeFluent f, int x) => f.Dim(Vec.FixFil(x));

	public static FlexNodeFluent ScrollX(this FlexNodeFluent f) => f.Flags(f.FlagsVal with { Scroll = new BoolVec(truer, false) });
	public static FlexNodeFluent ScrollY(this FlexNodeFluent f) => f.Flags(f.FlagsVal with { Scroll = new BoolVec(false, truer) });
	public static FlexNodeFluent ScrollXY(this FlexNodeFluent f) => f.Flags(f.FlagsVal with { Scroll = new BoolVec(truer, truer) });
	public static FlexNodeFluent Pop(this FlexNodeFluent f) => f.Flags(f.FlagsVal with { Pop = truer });

	public static FlexNodeFluent StratFill(this FlexNodeFluent f) => f.Strat(Strats.Fill);
	public static FlexNodeFluent StratStack(this FlexNodeFluent f, Dir mainDir, Align align = Align.Start) => f.Strat(Strats.Stack(mainDir, align));
	public static FlexNodeFluent StratWrap(this FlexNodeFluent f, Dir mainDir) => f.Strat(Strats.Wrap(mainDir));

	public static FlexNodeFluent Marg(this FlexNodeFluent f, int v) => f.Marg(Mg.Mk(v));
	public static FlexNodeFluent Marg(this FlexNodeFluent f, int horz, int vert) => f.Marg(new Marg(vert, horz, vert, horz));
	public static FlexNodeFluent Marg(this FlexNodeFluent f, int top, int right, int bottom, int left) => f.Marg(new Marg(top, right, bottom, left));
}
*/