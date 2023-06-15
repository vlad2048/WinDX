using System.Reactive.Disposables;
using System.Reactive.Linq;
using FlexBuilder.Structs;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex.Structs;
using PowBasics.ColorCode;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using RenderLib;
using RenderLib.Renderers;
using RenderLib.Structs;
using SysWinLib;
using SysWinLib.Structs;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using WinAPI.Windows;
using Color = System.Drawing.Color;

namespace FlexBuilder.SetupLogic;

static partial class Setup
{
	private static readonly Sz MAX_WINDOW_SIZE = new(900, 900);

	public static IDisposable DisplayLayout(
		MainWin ui,
		IRoMayVar<Layout> layout,
		UserPrefs userPrefs,
		IRoMayVar<Node> selNode,
		IRoMayVar<Node> hoveredNode,
		Action<FreeSz> winSzMutator
	)
	{
		var d = new Disp();
		var serD = new SerialDisp<Disp>().D(d);

		var isOpen = Var.Make(false).D(d);
		PersistRendererCombo(ui.rendererCombo, userPrefs).D(d);
		ui.showWinBtn.EnableWhenSome(layout).D(d);
		isOpen.Subscribe(open =>
		{
			ui.showWinBtn.Text = open ? "Close Window" : "Open Window";
			ui.redrawBtn.Enabled = open;
		}).D(d);


		ui.showWinBtn.Events().Click
			.Subscribe(_ =>
			{
				if (!isOpen.V)
				{
					// Open button
					// ===========
					serD.Value = null;
					serD.Value = new Disp();
					isOpen.V = true;

					var win = MakeWindow(layout.V.Ensure().TotalSz.Cap(MAX_WINDOW_SIZE), userPrefs).D(serD.Value);

					var renderWinCtx = RendererGetter.Get((RendererType)ui.rendererCombo.SelectedIndex, win).D(win.D);

					PaintWindow(win, layout, renderWinCtx, selNode, hoveredNode);
					HookWinSizeBothWaysAndPersistPos(layout, userPrefs, winSzMutator, win);

					Disposable.Create(() =>
					{
						if (!isOpen.IsDisposed)
							isOpen.V = false;
					}).D(win.D);

					win.Init();
				}
				else
				{
					// Close button
					// ============
					serD.Value = null;
					isOpen.V = false;
				}
			}).D(d);

		return d;
	}

	private static void HookWinSizeBothWaysAndPersistPos(IRoVar<Maybe<Layout>> layout, UserPrefs userPrefs, Action<FreeSz> winSzMutator, SysWin win)
	{
		win.ClientR
			.Subscribe(r => { winSzMutator(FreeSzMaker.FromSz(r.Size)); }).D(win.D);

		win.ScreenPt
			.Throttle(TimeSpan.FromSeconds(1))
			.Subscribe(pt =>
			{
				userPrefs.ExternalWindosPosX = pt.X;
				userPrefs.ExternalWindosPosY = pt.Y;
				userPrefs.Save();
			}).D(win.D);

		layout.SubscribeToSome(lay => win.SetR(new R(Pt.Empty, lay.TotalSz), WindowPositionFlags.SWP_NOMOVE)).D(win.D);
	}


	private static SysWin MakeWindow(Sz sz, UserPrefs userPrefs) => new(opt =>
	{
		opt.CreateWindowParams = new CreateWindowParams
		{
			Name = "Main Win",
			Styles = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_VISIBLE,
			X = userPrefs.ExternalWindosPosX,
			Y = userPrefs.ExternalWindosPosY,
			Width = sz.Width,
			Height = sz.Height,
			ControlStyles = (uint)ControlStyles.OptimizedDoubleBuffer
		};
	});


	private static void PaintWindow(SysWin win,
		IRoVar<Maybe<Layout>> layout,
		IRenderWinCtx renderWinCtx,
		IRoVar<Maybe<Node>> selNode,
		IRoVar<Maybe<Node>> hoveredNode
	)
	{
		win.WhenMsg
			.WhenERASEBKGND().Subscribe(e => e.MarkAsHandled()).D(win.D);

		win.WhenMsg
			.WhenPAINT().Subscribe(_ =>
			{
				if (layout.V.IsNone(out var layoutDef)) return;
				
				using var gfx = renderWinCtx.GetGfx();
				DrawOnWin(gfx, layoutDef, selNode, hoveredNode);
			}).D(win.D);

		Obs.Merge(
				layout.ToUnit(),
				selNode.ToUnit(),
				hoveredNode.ToUnit()
			)
			.Subscribe(_ =>
			{
				User32Helpers.RedrawWindow(win.Handle);
			}).D(win.D);
	}




	private static void DrawOnWin(IGfx gfx, Layout lay, IRoVar<Maybe<Node>> maySelNode, IRoVar<Maybe<Node>> mayHovNode)
	{
		PaintConsts.PaletteStart();

		void Rec(Node node)
		{
			if (!lay.RMap.ContainsKey(node)) throw new ArgumentException("No R found for a node. This shouldn't be possible");

			var rl = lay.RMap[node];

			gfx.FillR(rl, new SolidBrushDef(PaintConsts.GetPaletteColor()));

			foreach (var child in node.Children)
				Rec(child);
		}

		Rec(lay.Root);


		if (maySelNode.V.IsSome(out var selNode) && mayHovNode.V.IsSome(out var hovNode) && selNode == hovNode && lay.RMap.TryGetValue(selNode, out var r))
		{
			gfx.DrawR(r.Enlarge(-2), PaintConsts.SelPen);
			gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPen);
			gfx.DrawR(r.Enlarge(-0), PaintConsts.SelPen);
			gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPenMiddle);
		}
		else
		{
			if (maySelNode.V.IsSome(out selNode) && lay.RMap.TryGetValue(selNode, out r))
			{
				gfx.DrawR(r.Enlarge(-2), PaintConsts.SelPen);
				gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-0), PaintConsts.SelPen);
			}
			if (mayHovNode.V.IsSome(out hovNode) && lay.RMap.TryGetValue(hovNode, out r))
			{
				gfx.DrawR(r.Enlarge(-2), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-0), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPenMiddle);
			}
		}
	}



	private static IDisposable PersistRendererCombo(ComboBox rendererCombo, UserPrefs userPrefs)
	{
		rendererCombo.SelectedIndex = (int)userPrefs.SelectedRenderer;
		return rendererCombo.Events().SelectedIndexChanged.Subscribe(_ =>
		{
			userPrefs.SelectedRenderer = (RendererType)rendererCombo.SelectedIndex;
			userPrefs.Save();
		});
	}
}



file static class PaintConsts
{
	private static int paletteIdx;
	private static readonly Color[] palette = ColorUtils.MakePalette(12, 236, 0.72, 0.98).Prepend(Color.DarkSlateBlue).ToArray();
	public static void PaletteStart() => paletteIdx = 0;
	public static Color GetPaletteColor()
	{
		var col = palette[paletteIdx++];
		paletteIdx %= palette.Length;
		return col;
	}

	public static readonly PenDef SelPen = new(Color.DeepPink, 1);

	public static readonly PenDef HoverPen = new(Color.Black, 1);

	public static readonly PenDef HoverPenMiddle = new(Color.White, 1)
	{
		DashStyle = DashStyleDef.Dash,
	};
}