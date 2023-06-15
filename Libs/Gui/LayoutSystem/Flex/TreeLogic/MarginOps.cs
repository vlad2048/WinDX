using LayoutSystem.Flex.LayStratsInternal;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;

namespace LayoutSystem.Flex.TreeLogic;

static class MarginOps
{
	public static Node AddMargins(this Node root) =>
		(root.V.Marg != Mg.Zero) switch
		{
			truer =>
				Nod.Make(
					new FlexNode(
						DimVecMaker.DirFun(
							dir => root.V.Dim.Dir(dir).IsFil() switch
							{
								false => D.Fit,
								truer => D.Fil
							}
						),
						new MarginStrat(),
						root.V.Marg
					),
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


	/*public static Node RemoveMargins(this Node root)
	{
		Node Mk(Node node) => Nod.Make(node.V, node.Children.Select(RemoveMargins));

		return (root.V.Strat is IStratInternal) switch
		{
			truer => Mk(root.Children.Single()),
			false => Mk(root)
		};
	}*/
}