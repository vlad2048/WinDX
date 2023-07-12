using System.Reactive.Disposables;
using System.Reactive.Linq;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex.Structs;
using LayoutSystem.StructsShared;
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
using WinFormsTooling.Utils.Exts;
using Color = System.Drawing.Color;

namespace FlexBuilder.Logic;

static partial class Setup
{
	private static readonly Sz MAX_WINDOW_SIZE = new(900, 900);

	public static IDisposable DisplayLayout(
		MainWin ui,
		IRwMayVar<LayoutDef> layoutDefForRedraw,
		IRoMayVar<FlexLayout> layout,
		UserPrefs userPrefs,
		IRoMayVar<Node> selNode,
		IRoMayVar<Node> hoveredNode,
		Action<FreeSz> winSzMutator
	)
	{
		var d = new Disp();
		var serD = new SerialDisp<Disp>().D(d);

		ui.redrawStatusBtn.Events().Click.Subscribe(_ => layoutDefForRedraw.V = layoutDefForRedraw.V).D(d);

		var isOpen = Var.Make(userPrefs.ExternalWindowVisible).D(d);

		var renderer = Var.Make(
			userPrefs.SelectedRenderer,
			Obs.Merge(
				ui.gdiplusStatusItem.Events().Click.Select(_ => RendererType.GDIPlus),
				ui.direct2dStatusItem.Events().Click.Select(_ => RendererType.Direct2D),
				ui.direct2dindirect3dStatusItem.Events().Click.Select(_ => RendererType.Direct2DInDirect3D)
			)
		).D(d);
		renderer.Subscribe(r =>
		{
			userPrefs.SelectedRenderer = r;
			userPrefs.Save();
		}).D(d);

		layout.Subscribe(may => ui.calcWinSzStatusLabel.Text = may.IsSome(out var lay) switch
		{
			true => $"{lay.TotalSz}",
			false => "_"
		});



		void Show()
		{
			// Open button
			// ===========
			serD.Value = null;
			serD.Value = new Disp();
			isOpen.V = true;

			var win = MakeWindow(layout.V.Ensure().TotalSz.Cap(MAX_WINDOW_SIZE), userPrefs).D(serD.Value);

			var renderWinCtx = RendererGetter.Get(renderer.V, win).D(win.D);

			PaintWindow(win, layout, renderWinCtx, selNode, hoveredNode);
			HookWinSizeBothWaysAndPersistPos(layout, userPrefs, winSzMutator, win);

			Disposable.Create(() =>
			{
				if (!isOpen.IsDisposed)
					isOpen.V = false;
			}).D(win.D);

			win.Init();
		}

		void Hide()
		{
			// Close button
			// ============
			serD.Value = null;
			isOpen.V = false;
		}

		//PersistRendererCombo(ui.rendererCombo, userPrefs).D(d);

		if (isOpen.V)
			Show();


		ui.showWinStatusBtn.EnableWhenSome(layout).D(d);
		isOpen.Subscribe(open =>
		{
			ui.showWinStatusBtn.Text = open ? "Hide" : "Show";
			ui.redrawStatusBtn.Enabled = open;
			userPrefs.ExternalWindowVisible = open;
			userPrefs.Save();
		}).D(d);


		ui.showWinStatusBtn.Events().Click
			.Subscribe(_ =>
			{
				if (!isOpen.V)
					Show();
				else
					Hide();
			}).D(d);

		return d;
	}

	private static void HookWinSizeBothWaysAndPersistPos(IRoMayVar<FlexLayout> layout, UserPrefs userPrefs, Action<FreeSz> winSzMutator, SysWin win)
	{
		win.ClientSz
			.Subscribe(sz => { winSzMutator(FreeSzMaker.FromSz(sz)); }).D(win.D);

		win.ScreenPt
			.Throttle(TimeSpan.FromSeconds(1))
			.Subscribe(pt =>
			{
				userPrefs.ExternalWindosPos = (pt.X, pt.Y);
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
			X = userPrefs.ExternalWindosPos.Item1,
			Y = userPrefs.ExternalWindosPos.Item2,
			Width = sz.Width,
			Height = sz.Height,
			ControlStyles = (uint)ControlStyles.OptimizedDoubleBuffer
		};
	});


	private static void PaintWindow(SysWin win,
		IRoMayVar<FlexLayout> layout,
		IRenderWinCtx renderWinCtx,
		IRoMayVar<Node> selNode,
		IRoMayVar<Node> hoveredNode
	)
	{
		win.WhenMsg
			.WhenERASEBKGND().Subscribe(e => e.MarkAsHandled()).D(win.D);

		win.WhenMsg
			.WhenPAINT().Subscribe(_ =>
			{
				if (layout.V.IsNone(out var layoutDef)) return;
				
				using var gfx = renderWinCtx.GetGfx(false);
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




	private static void DrawOnWin(IGfx gfx, FlexLayout lay, IRoMayVar<Node> maySelNode, IRoMayVar<Node> mayHovNode)
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


		bool GetR(Node node, out R rl)
		{
			var rOpt = (lay.RMap.ContainsKey(node), lay.RMapFixed.ContainsKey(node)) switch
			{
				(true, _) => lay.RMap[node],
				(_, true) => lay.RMapFixed[node],
				_ => (R?)null
			};
			rl = rOpt ?? R.Empty;
			return rOpt.HasValue;
		}

		if (maySelNode.V.IsSome(out var selNode) && mayHovNode.V.IsSome(out var hovNode) && selNode == hovNode && GetR(selNode, out var r))
		{
			gfx.DrawR(r.Enlarge(-2), PaintConsts.SelPen);
			gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPen);
			gfx.DrawR(r.Enlarge(-0), PaintConsts.SelPen);
			gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPenMiddle);
		}
		else
		{
			if (maySelNode.V.IsSome(out selNode) && GetR(selNode, out r))
			{
				gfx.DrawR(r.Enlarge(-2), PaintConsts.SelPen);
				gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-0), PaintConsts.SelPen);
			}
			if (mayHovNode.V.IsSome(out hovNode) && GetR(hovNode, out r))
			{
				gfx.DrawR(r.Enlarge(-2), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-0), PaintConsts.HoverPen);
				gfx.DrawR(r.Enlarge(-1), PaintConsts.HoverPenMiddle);
			}
		}
	}



	/*private static IDisposable PersistRendererCombo(ComboBox rendererCombo, UserPrefs userPrefs)
	{
		rendererCombo.SelectedIndex = (int)userPrefs.SelectedRenderer;
		return rendererCombo.Events().SelectedIndexChanged.Subscribe(_ =>
		{
			userPrefs.SelectedRenderer = (RendererType)rendererCombo.SelectedIndex;
			userPrefs.Save();
		});
	}*/
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