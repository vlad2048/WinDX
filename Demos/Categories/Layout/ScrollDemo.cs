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
		var nodeRoot = new NodeState().D(D);
		var nodeTop = new NodeState().D(D);
		var nodeScroll = new NodeState().D(D);
		var nodeBottom = new NodeState().D(D);
		var nodesLines = Enumerable.Range(0, 29).SelectToArray(_ => new NodeState().D(D));

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.FillR(Consts.BackBrush);

				using (r[nodeTop].DimFilFix(50).M) { }

				using (r[nodeScroll].StratStack(Dir.Vert).ScrollXY().Marg(20).M)
				{
					r.FillR(Consts.ScrollBrush);
					for (var i = 0; i < nodesLines.Length; i++)
					{
						var nodeLine = nodesLines[i];

						using (r[nodeLine].DimFixFit(180).Marg(i > 0 ? 1 : 0, 0, 0, 0).M)
						{
							r.FillR(Consts.LineBrush);
							r.DrawText($"01234567890123456789 Line {i}", Consts.Font, Consts.TextColor);
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