using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace ControlSystem.Utils;

static class TextMeasureApplier
{
	public static MixNode ApplyTextMeasures(this MixNode root)
	{

		MixNode Rec(MixNode nod)
		{
			var node = nod.V;

			switch (node)
			{
				case CtrlNode:
					return Nod.Make(
						node,
						nod.Children
							.WhereNot(e => e.V is TextMeasureNode)
							.Select(Rec)
					);

				case StFlexNode flexNode:
					var nodesMeasure = nod.Children.Select(e => e.V).OfType<TextMeasureNode>().ToArray();
					var nodesOther = nod.Children.WhereNot(e => e.V is TextMeasureNode).Select(Rec);
					if (nodesMeasure.Length == 0)
					{
						return Nod.Make(node, nodesOther);
					}
					else
					{
						var dim = flexNode.Flex.Dim;
						var sizes = nodesMeasure.SelectToArray(e => e.Size);
						var sz = new Sz(sizes.Max(e => e.Width), sizes.Max(e => e.Height));
						var dimFitted = DimVecMaker.DirFun(dir =>
							dim.Dir(dir).Typ() switch
							{
								DimType.Fit => DimMaker.Fix(sz.Dir(dir)),
								_ => dim.Dir(dir)
							}
						);
						var flexNodeFitted = flexNode with { Flex = flexNode.Flex with { Dim = dimFitted } };
						return Nod.Make<IMixNode>(flexNodeFitted, nodesOther);
					}

				case TextMeasureNode:
					throw new ArgumentException("Impossible");

				default:
					throw new ArgumentException("Unknown MixNode type");
			}
		}

		return Rec(root);
	}
}