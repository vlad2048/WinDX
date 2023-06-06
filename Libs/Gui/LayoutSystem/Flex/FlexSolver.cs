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

			rMap[node] = new R(pos, layNfo.ResolvedSz);

			foreach (var t in node.Children.Zip(layNfo.Kids))
			{
				var (kid, kidR) = t;
				var kidFreeSz = FreeSz.MakeForKid(kid.V.Dim, kidR);
				LayNode(kid, pos + kidR.Pos, kidFreeSz);
			}
		}

		LayNode(root, Pt.Empty, totalFreeSz);
		return rMap;
	}

	private static LayNfo LayKids(Node node, FreeSz freeSz)
	{
		var kidDims = node.Children.Map(kid => ResolveKid(kid, freeSz));
		var layNfo = node.V.Strat.Lay(
			node,
			freeSz,
			kidDims
		).CapWithFreeSize(freeSz);
		return layNfo;
	}

	private static DimVec ResolveKid(Node kid, FreeSz dadFreeSz)
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
	private static FreeSz ComputeFreeSizeForKid(DimOptVec kidDim, FreeSz dadFreeSz)
	{
		int Mk(Dir dir) => kidDim.Dir(dir).Typ() switch
		{
			DType.Fit => int.MaxValue,
			DType.Fil => dadFreeSz.DirWithInfinites(dir),
			_ => kidDim.Dir(dir)!.Value.Max
		};
		return new FreeSz(Mk(Dir.Horz), Mk(Dir.Vert));
	}

	/// <summary>
	/// Keep the original dimensions if they were resolved,
	/// but for the unresolved ones, use the sizes calculated by calling the Layout
	/// </summary>
	private static DimVec UseLayoutResultToResolveFitDimensions(DimOptVec kidDim, Sz layoutSz)
	{
		Dim Mk(Dir dir) => kidDim.Dir(dir).HasValue switch
		{
			truer => kidDim.Dir(dir)!.Value,
			false => D.Fix(layoutSz.Dir(dir))
		};
		return new DimVec(Mk(Dir.Horz), Mk(Dir.Vert));
	}
}