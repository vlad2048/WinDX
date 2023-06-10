using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;
using static LayoutSystem.Flex.LayStrats.StackUtilsShared;
using static LayoutSystem.Flex.LayStrats.StackUtils;
// ReSharper disable AccessToModifiedClosure

namespace LayoutSystem.Flex.LayStrats;

public enum Align
{
	Start,
	Middle,
	End,
	Stretch,
}

/// <summary>
/// Stack the kids in a specific direction (MainDir / elseDir) <br/>
///   - children are not allowed to use Fills on MainDir <br/>
///   - respects DimVec Fit and Fix <br/>
///   - uses the kids kids.DimFixSz <br/>
///   - also uses the alignment along elseDir
/// </summary>
public class StackStrat : IStrat
{
	public Dir MainDir { get; }
	public Dir ElseDir => MainDir.Neg();
	public Align Align { get; }

	public StackStrat(Dir mainDir, Align align)
	{
		MainDir = mainDir;
		Align = align;
	}

	public override string ToString() => $"Stack({MainDir} / {Align})";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		/* Stack node assumptions
		 * ======================
		 *   - kids.Main != FIL		REMOVED
		 *
		 * Common assumptions
		 * ==================
		 *   - dad.X = FIT  =>  kids.X != FIL
		 *   - dad.Y = FIT  =>  kids.Y != FIL
		 */
		var dim = node.V.Dim;

		var sz = GeomMaker.MkSzDir(MainDir,
			dim.Dir(MainDir).IsFit() switch
			{
				truer => kidDims.Sum(e => e.Dir(MainDir).Max),
				false => dim.Dir(MainDir)!.Value.Max
			},
			dim.Dir(ElseDir).IsFit() switch
			{
				truer => kidDims.MaxT(e => e.Dir(ElseDir).Max),
				false => dim.Dir(ElseDir)!.Value.Max
			}
		).CapWith(freeSz);

		static FDim GetX(FDimVec vec) => vec.X;
		static FDim GetY(FDimVec vec) => vec.Y;

		var (xDims, yDims) = (
			kidDims.Map(GetX),
			kidDims.Map(GetY)
		);
		var (mainDims, elseDims) = (
			(xDims, yDims).Dir(MainDir),
			(xDims, yDims).Dir(ElseDir)
		);
		var mainLngs = LayoutMain(sz.Dir(MainDir), mainDims);
		var elseSpns = LayoutElse(sz.Dir(ElseDir), elseDims, Align);
		var mainSpns = mainLngs.ScanL((a, b) => a + b, 0).Zip(mainLngs, (p, l) => (p, l));

		return new LayNfo(
			sz,
			mainSpns
				.Zip(elseSpns, (mainSpn, elseSpn) => (mainSpn, elseSpn))
				.Map(t => new R(
					GeomMaker.MkPtDir(MainDir, t.mainSpn.p, t.elseSpn.p),
					GeomMaker.MkSzDir(MainDir, t.mainSpn.l, t.elseSpn.l)
				))
		);
	}
}



static class StackUtilsShared
{
	/// <summary>
	/// Compute the sizes of the kids along the stacking direction
	/// </summary>
	/// <param name="space">Free space available</param>
	/// <param name="kids">Kids resolved DimRs</param>
	/// <returns>Sizes of the kids along the stacking direction</returns>
	public static int[] LayoutMain(int space, FDim[] kids)
	{
		var lngs = kids.Map(e => e.Min);
	
		while (true)
		{
			var arr = kids.Zip(lngs, (kid, lng) => new
			{
				todo = kid.Max == int.MaxValue ? int.MaxValue : kid.Max - lng,
				done = lng,
				mult = kid.Mult
			}).ToArray();
			var free = space - lngs.Sum();
			var todo = kids.Any(kid => kid.Max == int.MaxValue) switch
			{
				truer => free,
				false => Math.Min(free, arr.Sum(t => t.todo))
			};
		
			if (todo <= 0) return lngs;
		
			// find kids that need to grow
			var indices = arr.GetIndicesWhere(t => t.todo > 0);
			var cnt = indices.Length;
		
			// spread todo into them
			var multTotal = indices.Sum(i => arr[i].mult);
			var adds = indices.Map(i => (int)(arr[i].mult * todo / multTotal));
			// correct for division errors
			var rest = (todo - adds.Sum()) % cnt;
			adds = adds.Map((add, i) => i < rest ? add + 1 : add);
		
			// cap with their Max
			adds = indices.Map((i, addI) => adds[addI] = Math.Min(arr[i].todo, adds[addI]));
		
			// add the adds
			indices.ForEach((i, addI) => lngs[i] += adds[addI]);
		}
	}
}



file static class StackUtils
{
	/// <summary>
	/// Compute the position and sizes (spans) of the kids accross the stacking direction
	/// </summary>
	/// <param name="free">Free space available</param>
	/// <param name="dims">Kids resolved DimRs</param>
	/// <param name="align">Alignment</param>
	/// <returns></returns>
	public static (int p, int l)[] LayoutElse(int free, FDim[] dims, Align align)
	{
		var lngs = dims.Map(e => e.Max.Cap(0, free));
		var lngMax = lngs.MaxT();
		return align switch
		{
// @formatter:off
			Align.Start		=> lngs.Map(l => (				 0,			l)),
			Align.Middle	=> lngs.Map(l => ((lngMax - l) / 2,			l)),
			Align.End		=> lngs.Map(l => (      lngMax - l,			l)),
			Align.Stretch	=> lngs.Map(_ => (				 0,	   lngMax)),
// @formatter:on
		};
	}
}