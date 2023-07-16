using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Structs;

namespace Demos.Categories.Layout;

sealed class ScrollDemo : Win
{
	public ScrollDemo() : base(opt => opt.R = new R(-400, 50, 300, 400))
	{
		var nodeRoot = new NodeState("root").D(D);
		var nodeTop = new NodeState("top").D(D);
		var nodeScroll = new NodeState("scroll").D(D);
		var nodeBottom = new NodeState("bottom").D(D);
		var nodesLines = Enumerable.Range(0, 2).Select((e, i) => (e, i)).SelectToArray(t => new NodeState($"line_{t.i}").D(D));

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.FillR(Consts.BackBrush);

				using (r[nodeTop].DimFilFix(50).M) { }

				using (r[nodeScroll].DimFixFil(200).StratStack(Dir.Vert).ScrollX().M)
				{
					r.FillR(Consts.ScrollBrush);
					for (var i = 0; i < nodesLines.Length; i++)
					{
						var nodeLine = nodesLines[i];

						using (r[nodeLine].DimFixFit(500).Marg(i > 0 ? 1 : 0, 0, 0, 0).M)
						{
							r.FillR(Consts.LineBrush);
							r.DrawText($"012345 dscg 6789012 ytwsa 3456789 Line {i} 012345 dscg 6789012 ytwsa 3456789 Line {i}", Consts.Font, Consts.TextColor);
						}
					}
				}

				using (r[nodeBottom].M) { }

			}
		}).D(D);
	}

	private static class Consts
	{
		public static readonly BrushDef BackBrush = new SolidBrushDef(Color.FromArgb(158, 158, 158));
		public static readonly BrushDef ScrollBrush = new SolidBrushDef(Color.FromArgb(237, 237, 237));
		public static readonly BrushDef LineBrush = new SolidBrushDef(Color.FromArgb(166, 166, 166));
		public static readonly FontDef Font = FontDef.Default;
		public static readonly Color TextColor = Color.Black;
	}
}