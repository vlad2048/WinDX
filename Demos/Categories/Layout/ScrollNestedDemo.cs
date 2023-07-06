using System.Drawing;
using ControlSystem;
using ControlSystem.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using RenderLib.Structs;
using PowRxVar;
using WinAPI.User32;

namespace Demos.Categories.Layout;

sealed class ScrollNestedDemo : Win
{
	public ScrollNestedDemo() : base(opt =>
	{
		opt.R = new R(-400, 50, 300, 400);
		opt.Styles =
			WindowStyles.WS_VISIBLE |
			WindowStyles.WS_CLIPCHILDREN |
			0;
	})
	{
		var nodeRoot = new NodeState("nodeRoot").D(D);
		var nodeTop = new NodeState("nodeTop").D(D);
		var nodeMiddle = new NodeState("nodeMiddle").D(D);
		var nodeBottom = new NodeState("nodeBottom").D(D);
		var nodeScroll = new NodeState("nodeScroll").D(D);
		var nodesLines = Enumerable.Range(0, 2).SelectToArray((_, i) => new NodeState($"nodeLines[{i}]").D(D));

		var nodeScroll2 = new NodeState("nodeScroll2").D(D);
		var nodesLines2 = Enumerable.Range(0, 1).SelectToArray((_, i) => new NodeState($"nodeLines2[{i}]").D(D));

		var nodeScroll3 = new NodeState("nodeScroll3").D(D);
		var nodesLines3 = Enumerable.Range(0, 20).SelectToArray((_, i) => new NodeState($"nodeLines3[{i}]").D(D));

		WhenRender.Subscribe(r =>
		{
			using (r[nodeRoot].StratStack(Dir.Vert).M)
			{
				r.Gfx.FillR(Consts.BackBrush);

				using (r[nodeTop].DimFilFix(50).M)
				{
					r.Gfx.FillR(Consts.InterBrush);
				}

				using (r[nodeScroll].StratStack(Dir.Vert).ScrollXY().Pop().Dim(140, 100).Marg(20).M)
				{
					r.Gfx.FillR(Consts.ScrollBrush);
					for (var i = 0; i < nodesLines.Length; i++)
					{
						var nodeLine = nodesLines[i];
						var marg = new Marg(i > 0 ? 1 : 0, 0, 0, 0);
						using (r[nodeLine].DimFixFit(180).Marg(marg).M)
						{
							//if (i != 0)
							{
								r.Gfx.FillR(Consts.LineBrush);
								r.DrawText($"[{i}] 01234567890123456789 Line {i}", Consts.Font, Consts.TextColor);
							}
							/*else
							{

								using (r[nodeScroll2].Dim(350, 150).StratStack(Dir.Vert).ScrollXY().Pop().Marg(10).M)
								{
									r.Gfx.FillR(Consts.ScrollBrush2);
									for (var i2 = 0; i2 < nodesLines2.Length; i2++)
									{
										var nodeLine2 = nodesLines2[i2];
										var marg2 = new Marg(i2 > 0 ? 1 : 0, 0, 0, 0);
										using (r[nodeLine2].DimFixFit(400).Marg(marg2).M)
										{
											r.Gfx.FillR(Consts.LineBrush2);
											r.DrawText($"[{i2}] Sub123Sub456Sub789Sub123Sub456Sub789Sub123Sub456Sub789 Line {i2}", Consts.Font, Consts.TextColor);
										}
									}
								}
							}*/
						}
					}
				}

				/*using (r[nodeMiddle].DimFilFix(150).M)
				{
					r.Gfx.FillR(Consts.InterBrush);
				}

				using (r[nodeScroll3].StratStack(Dir.Vert).ScrollXY().Dim(140, 100).Marg(20).M)
				{
					r.Gfx.FillR(Consts.ScrollBrush);
					for (var i = 0; i < nodesLines3.Length; i++)
					{
						var nodeLine = nodesLines3[i];
						var marg = new Marg(i > 0 ? 1 : 0, 0, 0, 0);
						using (r[nodeLine].DimFixFit(180).Marg(marg).M)
						{
							r.Gfx.FillR(Consts.LineBrush);
							r.DrawText($"[{i}] ThirdThirdThird Line {i}", Consts.Font, Consts.TextColor);
						}
					}
				}*/

				using (r[nodeBottom].M)
				{
					r.Gfx.FillR(Consts.InterBrush);
				}

			}
		}).D(D);
	}

	private static class Consts
	{
		public static readonly BrushDef BackBrush = new SolidBrushDef(Color.FromArgb(158, 158, 158));
		public static readonly BrushDef InterBrush = new SolidBrushDef(Color.FromArgb(81, 139, 194));
		public static readonly BrushDef ScrollBrush = new SolidBrushDef(Color.FromArgb(237, 237, 237));
		public static readonly BrushDef LineBrush = new SolidBrushDef(Color.FromArgb(166, 166, 166));
		public static readonly BrushDef ScrollBrush2 = new SolidBrushDef(Color.FromArgb(25, 66, 110));
		public static readonly BrushDef LineBrush2 = new SolidBrushDef(Color.FromArgb(102, 151, 204));
		public static readonly FontDef Font = FontDef.Default;
		public static readonly Color TextColor = Color.Black;
	}
}