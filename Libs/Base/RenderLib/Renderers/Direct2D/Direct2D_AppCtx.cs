using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PowRxVar;
using PowBasics.Geom;
using RenderLib.Structs;
using WinAPI.Utils.Exts;
using RenderLib.Renderers.Utils.DirectX;
using SysWinInterfaces;

namespace RenderLib.Renderers.Direct2D;


// **********
// * AppCtx *
// **********
public sealed class Direct2D_AppCtx : IRenderAppCtxWithDispose
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	// D2D
	// ===
	public D2D.ID2D1Factory1 D2DFactory { get; }

	internal Direct2D_AppCtx()
	{
		D2DFactory = D2D.D2D1.D2D1CreateFactory<D2D.ID2D1Factory1>(D2D.FactoryType.SingleThreaded, D2D.DebugLevel.Information).D(d);
	}

	public IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win) => new Direct2D_WinCtx(win, this);
}


// **********
// * WinCtx *
// **********
public sealed class Direct2D_WinCtx : IRenderWinCtx
{
	//private const string ImageFilename = @"C:\Dev_Explore\WinDX\_infos\images\pixart.png";

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly SerialDisp<Disp> resizeSerialD;
	private Disp ResizeD => resizeSerialD.Value ?? throw new ArgumentException();


	private readonly ISysWinRenderingSupport win;
	internal readonly Direct2D_AppCtx appCtx;


	internal Pencils Pencils { get; private set; } = null!;

	// D2D
	// ===
	public D2D.ID2D1HwndRenderTarget D2DHwndRenderTarget { get; private set; } = null!;


	public D2D.ID2D1Factory1 D2DFactory => appCtx.D2DFactory;


	internal Direct2D_WinCtx(ISysWinRenderingSupport win, Direct2D_AppCtx appCtx)
	{
		this.win = win;
		this.appCtx = appCtx;
		resizeSerialD = new SerialDisp<Disp>().D(d);

		win.WhenInit().Subscribe(_ =>
		{
			InitResizeResources();
		}).D(d);

		win.ClientR
			.Where(clientR => clientR is { Width: >= 1, Height: >= 1 } && win.IsInit.V)
			.Subscribe(clientR =>
			{
				Resize(clientR.Size);
			}).D(d);

		d.D(win.D);
	}


	public void Resize(Sz sz)
	{
		if (!win.IsInit.V) return;

		resizeSerialD.Value = null;
		InitResizeResources();
	}

	public IGfx GetGfx() => new Direct2D_Gfx(win, this);




	private void InitResizeResources()
	{
		resizeSerialD.Value = null;
		resizeSerialD.Value = new Disp();

		var sz = win.ClientR.V.Size;
		//var dpi = User32Methods.GetDpiForWindow(win.Handle);
		var renderTargetProperties = new D2D.RenderTargetProperties(
			/*D2D.RenderTargetType.Default,
			new PixelFormat(DXGI.Format.Unknown, AlphaMode.Unknown),
			dpi, dpi,
			D2D.RenderTargetUsage.None,
			D2D.FeatureLevel.Default*/
		);
		var hwndRenderTargetProperties = new D2D.HwndRenderTargetProperties
		{
			Hwnd = win.Handle,
			PixelSize = new Size(sz.Width, sz.Height),
			PresentOptions = D2D.PresentOptions.None,
		};
		D2DHwndRenderTarget = appCtx.D2DFactory.CreateHwndRenderTarget(
			renderTargetProperties,
			hwndRenderTargetProperties
		).D(ResizeD);

		Pencils = new Pencils(appCtx.D2DFactory, D2DHwndRenderTarget).D(ResizeD);
	}
}


// *******
// * Gfx *
// *******
public sealed class Direct2D_Gfx : IGfx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISysWinRenderingSupport win;
	private readonly Pencils pencils;

	public D2D.ID2D1HwndRenderTarget T { get; }
	public D2D.ID2D1Factory1 D2DFactory { get; }

	public R R { get; set; }

	internal Direct2D_Gfx(ISysWinRenderingSupport win, Direct2D_WinCtx winCtx)
	{
		this.win = win;
		R = win.ClientR.V;
		T = winCtx.D2DHwndRenderTarget;
		D2DFactory = winCtx.appCtx.D2DFactory;
		pencils = winCtx.Pencils;

		T.BeginDraw();
		Disposable.Create(() =>
		{
			T.EndDraw();
		}).D(d);
	}

	public void Dbg()
	{
	}

	public void FillR(R r, BrushDef brush)
	{
		if (!win.IsInit.V) return;
		T.FillRectangle(r.ToDrawRect(), pencils.GetBrush(brush));
	}

	public void DrawR(R r, PenDef penDef)
	{
		if (!win.IsInit.V) return;

		var pen = pencils.GetPen(penDef);

		var f = new RectangleF(r.X + 0.5f, r.Y + 0.5f, r.Width, r.Height);

		T.DrawRectangle(f, pen.Brush, pen.Width, pen.Style);
	}

	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
		if (!win.IsInit.V) return;
		var pen = pencils.GetPen(penDef);

		var m = (int)pen.Width % 2 == 0 ? 0.5f : 0.0f;
		var isHorz = a.Y == b.Y;
		var isVert = a.X == b.X;
		var (ox, oy) = (isHorz, isVert) switch
		{
			(true, false) => (0.0f, 0.5f - m),
			(false, true) => (0.5f - m, 0.0f),
			_ => (0.0f, 0.0f)
		};

		var p0 = new Vector2(a.X + ox, a.Y + oy);
		var p1 = new Vector2(b.X + ox, b.Y + oy);

		T.DrawLine(p0, p1, pen.Brush, pen.Width, pen.Style);
	}
}
