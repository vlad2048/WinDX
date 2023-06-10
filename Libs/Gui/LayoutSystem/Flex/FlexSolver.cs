using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex.TreeLogic;
using LayoutSystem.Utils.Exts;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public static class FlexSolver
{
	public static Layout Solve(
		Node rootRaw,
		FreeSz freeSz
	)
	{
		var (root, warnings) = rootRaw.RespectRules(freeSz);
		var rootMargins = root.AddMargins();
		var rMap = MakeRMap(rootMargins, freeSz);
		return new Layout(
			rootRaw,
			rMap.Remap(rootRaw),
			warnings
		);
	}

	private static IReadOnlyDictionary<Node, R> Remap(this IReadOnlyDictionary<Node, R> rMap, Node rootRaw)
	{
		var rs = rMap.Where(e => e.Key.V.Strat is not IStratInternal).SelectToArray(e => e.Value);
		return
			rootRaw.Zip(rs)
				.ToDictionary(
					e => e.First,
					e => e.Second
				);
	}


	// ******************
	// * Compute Layout *
	// ******************
	private static IReadOnlyDictionary<Node, R> MakeRMap(
		Node root,
		FreeSz totalFreeSz
	)
	{
		var rMap = new Dictionary<Node, R>();

		void LayNode(Node node, Pt pos, FreeSz freeSz)
		{
			var layNfo = LayKids(node, freeSz);

			rMap[node] = new R(pos, layNfo.ResolvedSz.CapWith(freeSz));

			foreach (var t in node.Children.Zip(layNfo.Kids))
			{
				var (kid, kidR) = t;
				var kidFreeSz = FreeSz.MakeForKid(kid.V.Dim, kidR);
				kidFreeSz = new FreeSz(kidR.Width, kidR.Height);
				LayNode(kid, pos + kidR.Pos, kidFreeSz);
			}
		}

		LayNode(root, Pt.Empty, totalFreeSz);
		return rMap;
	}

	private static LayNfo LayKids(Node node, FreeSz freeSz)
	{
		freeSz = freeSz.UnbridleScrolls(node);
		var kidDims = node.Children.Map(kid => ResolveKid(kid, freeSz));
		var layNfo = node.V.Strat.Lay(
			node,
			freeSz,
			kidDims
		).CapWithFreeSize(freeSz);
		return layNfo;
	}

	private static FDimVec ResolveKid(Node kid, FreeSz dadFreeSz)
	{
		var kidDim = kid.V.Dim;
		if (kidDim.IsResolvable) return kidDim.ResolveEnsure();

		var kidFreeSz = ComputeFreeSizeForKid(kidDim, dadFreeSz);

		var layNfo = LayKids(kid, kidFreeSz);

		var kidResolvedDim = UseLayoutResultToResolveFitDimensions(kidDim, layNfo.ResolvedSz);
		return kidResolvedDim;
	}

	/// <summary>
	/// Calculate how much free size we should provide a kid Lay() call to resolve its dimensions
	/// If:
	///   - dim.FIT then provide an unbounded space for the layout to deduce its size
	///   - dim.FIL
	/// </summary>
	private static FreeSz ComputeFreeSizeForKid(DimVec kidDim, FreeSz dadFreeSz) =>
		FreeSzMaker.DirFun(
			dir => kidDim.Dir(dir).Typ() switch
			{
				DimType.Fit => int.MaxValue,
				DimType.Fil => dadFreeSz.DirWithInfinites(dir),
				_ => kidDim.Dir(dir)!.Value.Max
			}
		);


	/// <summary>
	/// Keep the original dimensions if they were resolved,
	/// but for the unresolved ones, use the sizes calculated by calling the Layout
	/// </summary>
	private static FDimVec UseLayoutResultToResolveFitDimensions(DimVec kidDim, Sz layoutSz) =>
		FDimVecMaker.DirFun(
			dir => kidDim.Dir(dir).HasValue switch
			{
				truer => kidDim.Dir(dir)!.Value,
				false => D.Fix(layoutSz.Dir(dir))
			}
		);
}