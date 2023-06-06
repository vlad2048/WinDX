using LayoutSystem.Flex.LayStratsInternal;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;

namespace LayoutSystem.Flex.TreeLogic;

static class MarginOps
{
	public static Node AddMargins(this Node root)
	{
		DimOpt MkDir(DimOptVec nodeDims, Dir dir) => nodeDims.Dir(dir).IsFil() switch
		{
			false => D.Fit,
			truer => D.Fil
		};

		Node Rec(Node node) => (node.V.Marg != Mg.Zero) switch
		{
			truer =>
				Nod.Make(
					new FlexNode(
						new DimOptVec(
							MkDir(node.V.Dim, Dir.Horz),
							MkDir(node.V.Dim, Dir.Vert)
						),
						node.V.Marg,
						new MarginStrat()
					),
					Nod.Make(
						node.V,
						node.Children.Select(Rec)
					)
				),
			false =>
				Nod.Make(
					node.V,
					node.Children.Select(Rec)
				)
		};

		return Rec(root);
	}

	public static Node RemoveMargins(this Node root)
	{
		Node Mk(Node node) => Nod.Make(node.V, node.Children.Select(RemoveMargins));

		return (root.V.Strat is IStratInternal) switch
		{
			truer => Mk(root.Children.Single()),
			false => Mk(root)
		};
	}
}