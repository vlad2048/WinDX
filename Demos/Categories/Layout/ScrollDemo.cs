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
	public ScrollDemo() : base(opt => opt.R = new R(-300, 50, 150, 200))
	{
		var nodeRoot = new NodeState().D(D);
		var nodeScroll = new NodeState().D(D);
		var nodesLines = Enumerable.Range(0, 20).SelectToArray(_ => new NodeState().D(D));

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.Gfx.FillR(Consts.BackBrush);

				using (r[nodeScroll].StratStack(Dir.Vert).Marg(20).M)
				{
					r.Gfx.FillR(Consts.ScrollBrush);
					for (var i = 0; i < nodesLines.Length; i++)
					{
						var nodeLine = nodesLines[i];

						using (r[nodeLine].DimFilFit().Marg(i > 0 ? 1 : 0, 0, 0, 0).M)
						{
							r.Gfx.FillR(Consts.LineBrush);
							r.DrawText($"Line {i}", Consts.Font, Consts.TextColor);
						}
					}
				}
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