using LayoutSystem.Flex.Details;
using LayoutSystem.Flex.Details.Structs;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex.TreeLogic;
using LayoutSystem.Utils.Exts;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace LayoutSystem.Flex;

public static class FlexSolver
{
	public static FlexLayout Solve(
		Node root,
		FreeSz freeSz
	)
	{
		var (rootFixed, warnings) = root.RespectRules(freeSz);

		rootFixed = rootFixed.AddMargins();

		var (rMap, detailsMap) = ComputeLayout(rootFixed, freeSz);
		return new FlexLayout(
			root,
			freeSz,
			rMap.Remap(root),
			warnings.Remap(root),

			rootFixed,
			rMap,
			detailsMap
		);
	}


	// ******************
	// * Compute Layout *
	// ******************
	private static (IReadOnlyDictionary<Node, R>, IReadOnlyDictionary<Node, NodeDetails>) ComputeLayout(
		Node root,
		FreeSz totalFreeSz
	)
	{
		var rMap = new Dictionary<Node, R>();
		var detailsMap = new Dictionary<Node, NodeDetails>();

		void LayNode(Node node, Pt pos, FreeSz freeSz)
		{
			var write = new PageWriter();
			write.LayNodeInputPos(pos);

			var layNfo = LayKids(node, freeSz, write, true);

			rMap[node] = new R(pos, layNfo.ResolvedSz.CapWith(freeSz));
			write.FinalR(pos, layNfo.ResolvedSz, freeSz);

			var kidIdx = 0;
			foreach (var t in node.Children.Zip(layNfo.Kids))
			{
				var (kid, kidR) = t;

				var kidPos = pos + kidR.Pos;
				var kidFreeSz = kid.IsPop() switch
				{
					false => FreeSzMaker.FromSz(kidR.Size),
					truer => FreeSzMaker.ForPop(kid)
				};

				write.FinalKidRecurse(kidIdx++, kidPos, kidFreeSz, kid.IsPop());
				LayNode(kid, kidPos, kidFreeSz);
			}

			detailsMap[node] = new NodeDetails(write.GetPage());
		}

		LayNode(root, Pt.Empty, totalFreeSz);
		return (rMap, detailsMap);
	}

	private static LayNfo LayKids(Node node, FreeSz freeSz, PageWriter write, bool topLevel)
	{
		var scrFreeSz = freeSz.UnbridleScrolls(node);

		write.LayKidsInputs(node, scrFreeSz, freeSz);

		var kidDims = node.Children.Map((kid, kidIdx) => ResolveKid(kid, kidIdx, scrFreeSz, write));

		var layNfo = node.V.Strat.Lay(
			node,
			scrFreeSz,
			kidDims
		);
		var layNfoCapped = layNfo.CapWithFreeSize(scrFreeSz);

		write.LayKidsOutput(layNfoCapped, layNfo, scrFreeSz, topLevel);

		return layNfoCapped;
	}

	private static FDimVec ResolveKid(Node kid, int kidIdx, FreeSz dadFreeSz, PageWriter write)
	{
		if (kid.IsPop())
		{
			write.KidIsPop(kidIdx);
			return new FDimVec(D.Fix(0), D.Fix(0));
		}

		var kidDim = kid.V.Dim;
		write.KidResolvedOrNot(kid, kidIdx);

		if (kidDim.IsResolvable)
		{
			write.WriteLine();
			return kidDim.ResolveEnsure();
		}

		// Calculate how much free size we should provide a kid Lay() call to resolve its dimensions
		// If:
		//   - dim.FIT -> provide an unbounded space for the layout to deduce its size
		//   - dim.FIL -> provide all the free size the parent has
		//   - dim.FIX -> use the kid's fix size
		var kidFreeSz = FreeSzMaker.DirFun(
			dir => kidDim.Dir(dir).Typ() switch
			{
				DimType.Fit => null,
				DimType.Fil => dadFreeSz.Dir(dir),
				_ => kidDim.Dir(dir)!.Value.Max
			}
		);

		write.KidFreeSz(kidFreeSz);
		write.Indent();

		var layNfo = LayKids(kid, kidFreeSz, write, false);

		// Keep the original dimensions if they were resolved,
		// but for the unresolved ones, use the sizes calculated by calling the Layout
		var kidResolvedDim = FDimVecMaker.DirFun(
			dir => kidDim.Dir(dir).HasValue switch
			{
				truer => kidDim.Dir(dir)!.Value,
				false => D.Fix(layNfo.ResolvedSz.Dir(dir))
			}
		);

		write.Unindent();

		write.ResolveKidOutput(kid, kidIdx, kidResolvedDim);

		return kidResolvedDim;
	}


	
	// *************
	// * Remapping *
	// *************
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

	private static IReadOnlyDictionary<Node, FlexWarning> Remap(this IReadOnlyDictionary<int, FlexWarning> warningMap, Node rootRaw)
	{
		var nodes = rootRaw.ToArray();
		return warningMap.MapKeys(e => nodes[e]);
	}



	private static bool IsPop(this Node node) => node.V.Flags.Pop;



	/*
	/// <summary>
	/// Calculate how much free size we should provide a kid Lay() call to resolve its dimensions
	/// If:
	///   - dim.FIT -> provide an unbounded space for the layout to deduce its size
	///   - dim.FIL -> provide all the free size the parent has
	///	  - dim.FIX -> use the kid's fix size
	/// </summary>
	private static FreeSz ComputeFreeSizeForKid(DimVec kidDim, FreeSz dadFreeSz) =>
		FreeSzMaker.DirFun(
			dir => kidDim.Dir(dir).Typ() switch
			{
				DimType.Fit => null,
				DimType.Fil => dadFreeSz.Dir(dir),
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


	private static Node ZeroPopSize(this Node node) => node.IsPop() switch
	{
		truer => node.CloneWithNewContent(node.V with { Dim = Vec.Fix(0, 0) }),
		false => node
	};

	private static bool IsPop(this Node node) => node.V.Strat is FillStrat { Spec: PopSpec };
	*/
}