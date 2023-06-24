using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;

namespace LayoutSystem.Utils;

public static class TreeBuilder
{
	public static Node M(DimVec dim, IStrat? strat = null, params Node[] kids) =>
		Nod.Make(new FlexNode(dim, strat ?? Strats.Fill, Mg.Zero), kids);

	public static TNod<R> A(int x, int y, int width, int height, params TNod<R>[] kids) =>
		Nod.Make(new R(x, y, width, height), kids);
}


class FlexNodeFluent
{
	private DimVec dim = Vec.Fil;
	private ISpec spec = new ScrollSpec(BoolVec.False);
	private IStrat strat = Strats.Fill;
	private Marg marg = Mg.Zero;

	public FlexNodeFluent Dim(DimVec dim_)
	{
		dim = dim_;
		return this;
	}

	public FlexNodeFluent Spec(ISpec spec_)
	{
		spec = spec_;
		return this;
	}

	public FlexNodeFluent Strat(IStrat strat_)
	{
		strat = strat_;
		return this;
	}

	public FlexNodeFluent Marg(Marg marg_)
	{
		marg = marg_;
		return this;
	}

}