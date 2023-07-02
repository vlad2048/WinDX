using LayoutSystem.Flex.Details;
using LayoutSystem.Flex.Details.Structs;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex.TreeLogic;
using LayoutSystem.Flex.Utils;
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
		).SanityCheck();
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

			var layNfo = LayKidsWithScrollHandling(node, freeSz, write);

			rMap[node] = new R(pos, layNfo.ResolvedSz);
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


	private static LayNfo LayKidsWithScrollHandling(Node node, FreeSz freeSz, PageWriter write)
	{
		var hasKids = node.Children.Any();

		switch (node.V.Flags.Scroll, hasKids)
		{
			case (_, false):
			case ((false, false), _):
				return LayKids(node, freeSz, write);

			case ((truer, truer), truer):
			{
				var layNfoDad = LayKids(node, freeSz, write);
				var layNfoKids = LayKids(node, new FreeSz(null, null), write);
				var layNfo = new LayNfo(layNfoDad.ResolvedSz, layNfoKids.Kids);
				return layNfo;
			}

			case ((false, truer), truer):
			case ((truer, false), truer):
			{
				var layNfoDad = LayKids(node, freeSz, write);
				var nodeSz = layNfoDad.ResolvedSz;
				var scrollDir = node.V.Flags.Scroll.X ? Dir.Horz : Dir.Vert;

				var freeSzNoScroll = FreeSzMaker.DirFun(dir => dir == scrollDir ? null : nodeSz.Dir(dir));
				var layNfoKidsNoScroll = LayKids(node, freeSzNoScroll, write);
				var layNfoKids = layNfoKidsNoScroll;
				var scrollNeeded = layNfoKidsNoScroll.Kids.Union().Size.Dir(scrollDir) > nodeSz.Dir(scrollDir);

				if (scrollNeeded)
				{
					var freeSzScroll = FreeSzMaker.DirFun(dir => dir == scrollDir ? null : Math.Max(0, nodeSz.Dir(dir) - FlexFlags.ScrollBarCrossDims.Dir(dir)));
					var layNfoKidsScroll = LayKids(node, freeSzScroll, write);
					layNfoKids = layNfoKidsScroll;
				}

				var layNfo = new LayNfo(layNfoDad.ResolvedSz, layNfoKids.Kids);
				return layNfo;
			}
		}
	}



	private static LayNfo LayKids(Node node, FreeSz freeSz, PageWriter write)
	{
		write.LayKidsInputs(node, freeSz);

		var kidDims = node.Children.Map((kid, kidIdx) => ResolveKid(kid, kidIdx, freeSz, write));

		var layNfo = node.V.Strat.Lay(
			node,
			freeSz,
			kidDims
		);
		var layNfoCapped = layNfo.CapWithFreeSize(freeSz);

		write.LayKidsOutput(layNfo);

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

		var layNfo = LayKids(kid, kidFreeSz, write);

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
	private static IReadOnlyDictionary<Node, V> Remap<V>(this IReadOnlyDictionary<Node, V> rMap, Node rootRaw)
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
}