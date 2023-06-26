using LayoutSystem.Flex.LayStratsInternal;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;

namespace LayoutSystem.Flex.TreeLogic;

static class TreeTransformOps
{
	public static Node AddMargins(this Node root) =>
		(root.V.Marg != Mg.Zero) switch
		{
			truer =>
				Nod.Make(
					root.V with {
						Dim = DimVecMaker.DirFun(
							dir => root.V.Dim.Dir(dir).IsFil() switch
							{
								false => D.Fit,
								truer => D.Fil
							}
						),
						Strat = new MarginStrat()
					},
					Nod.Make(
						root.V,
						root.Children.Select(AddMargins)
					)
				),
			false =>
				Nod.Make(
					root.V,
					root.Children.Select(AddMargins)
				)
		};
}