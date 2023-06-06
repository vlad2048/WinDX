using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PowRxVar;
using PowBasics.Geom;
using RenderLib.Structs;
using SysWinLib;
using WinAPI.User32;
using WinAPI.Utils.Exts;
using RenderLib.Renderers.Utils.DirectX;

namespace RenderLib.Renderers.Direct2D;


// **********
// * AppCtx *
// **********
public class Direct2D_AppCtx : IRenderAppCtx
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
public class Direct2D_WinCtx : IRenderWinCtx
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
		var dpi = User32Methods.GetDpiForWindow(win.Handle);
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
public class Direct2D_Gfx : IGfx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISysWinRenderingSupport win;
	private readonly Direct2D_WinCtx winCtx;
	public Pencils Pencils { get; }

	public D2D.ID2D1HwndRenderTarget T { get; }
	public D2D.ID2D1Factory1 D2DFactory { get; }

	public R R { get; }

	internal Direct2D_Gfx(ISysWinRenderingSupport win, Direct2D_WinCtx winCtx)
	{
		this.win = win;
		this.winCtx = winCtx;
		R = win.ClientR.V;
		T = winCtx.D2DHwndRenderTarget;
		D2DFactory = winCtx.appCtx.D2DFactory;
		Pencils = winCtx.Pencils;

		T.BeginDraw();
		Disposable.Create(() =>
		{
			T.EndDraw();
		}).D(d);
	}

	public void Dbg()
	{
		if (!win.IsInit.V) return;
	}

	public void FillR(R r, BrushDef brush)
	{
		if (!win.IsInit.V) return;
		T.FillRectangle(r.ToDrawRect(), Pencils.GetBrush(brush));
	}

	public void DrawR(R r, PenDef penDef)
	{
		if (!win.IsInit.V) return;

		var pen = Pencils.GetPen(penDef);

		var f = new RectangleF(r.X + 0.5f, r.Y + 0.5f, r.Width, r.Height);

		T.DrawRectangle(f, pen.Brush, pen.Width, pen.Style);
	}

	public void DrawLine(Pt a, Pt b, PenDef penDef)
	{
		if (!win.IsInit.V) return;
		var pen = Pencils.GetPen(penDef);

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
















/*
// **********
// * AppCtx *
// **********
public class Direct2D_AppCtx : IRenderAppCtx
{
	private static readonly D3D.FeatureLevel[] featureLevels =
	{
		D3D.FeatureLevel.Level_11_1,
		D3D.FeatureLevel.Level_11_0,
		D3D.FeatureLevel.Level_10_1,
		D3D.FeatureLevel.Level_10_0
	};

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	// DXGI
	// ====
	public DXGI.IDXGIFactory2 DXGIFactory { get; }
	public DXGI.IDXGIDevice DXGIDevice { get; } // queried from D3DDevice

	// D3D11
	// =====
	public D3D11.ID3D11Device1 D3DDevice { get; }
	public D3D11.ID3D11DeviceContext1 D3DDeviceCtx { get; }
		
	// D2D
	// ===
	public D2D.ID2D1Factory1 D2DFactory { get; }
	public D2D.ID2D1Device D2DDevice { get; }
	public D2D.ID2D1DeviceContext D2DDeviceCtx { get; }

	// WIC
	// ===
	public WIC.IWICImagingFactory WicFactory { get; }

	// ANIM
	// ====
	public ANIM.IUIAnimationManager2 AnimManager { get; }
	public ANIM.IUIAnimationTransitionLibrary2 AnimTransitionLibrary { get; }
	public ANIM.IUIAnimationVariable2 AnimVariable { get; }

	// DCOMP
	// =====
	public DCOMP.IDCompositionDevice DCompDevice { get; }


	internal readonly Pencils pencils;

	internal Direct2D_AppCtx()
	{
		DXGIFactory = DXGI.DXGI.CreateDXGIFactory1<DXGI.IDXGIFactory2>().D(d);
		var creationFlags = D3D11.DeviceCreationFlags.BgraSupport;
		if (D3D11.D3D11.SdkLayersAvailable())
			creationFlags |= D3D11.DeviceCreationFlags.Debug;

		(D3DDevice, D3DDeviceCtx, DXGIDevice) = D3D11InitUtils.Helper_D3D11CreateDevice<D3D11.ID3D11Device1, D3D11.ID3D11DeviceContext1, DXGI.IDXGIDevice>(
			null,
			D3D.DriverType.Hardware,
			creationFlags,
			featureLevels
		).D(d);

		D2DFactory = D2D.D2D1.D2D1CreateFactory<D2D.ID2D1Factory1>(D2D.FactoryType.SingleThreaded, D2D.DebugLevel.Information).D(d);
		D2DDevice = D2DFactory.CreateDevice(DXGIDevice).D(d);
		D2DDeviceCtx = D2DDevice.CreateDeviceContext(D2D.DeviceContextOptions.None).D(d);

		D2DFactory.CreateHwndRenderTarget()

		WicFactory = new WIC.IWICImagingFactory2().D(d);

		AnimManager = new ANIM.IUIAnimationManager2().D(d);
		AnimTransitionLibrary = new ANIM.IUIAnimationTransitionLibrary2().D(d);
		AnimVariable = AnimManager.CreateAnimationVariable(0.0).D(d);

		DCompDevice = DCOMP.DComp.DCompositionCreateDevice<DCOMP.IDCompositionDevice>(DXGIDevice).D(d);


		pencils = new Pencils(D2DDeviceCtx).D(d);
	}

	public IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win) => new Direct2D_WinCtx(win, this);
}


// **********
// * WinCtx *
// **********
public class Direct2D_WinCtx : IRenderWinCtx
{
	private const string ImageFilename = @"C:\Dev_Explore\WinDX\_infos\images\pixart.png";

	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private SerialDisp<Disp> resizeSerialD = null!;
	private Disp ResizeD => resizeSerialD.Value ?? throw new ArgumentException();


	private readonly ISysWinRenderingSupport win;
	internal readonly Direct2D_AppCtx appCtx;

	// DXGI
	// ====
	public DXGI.IDXGISwapChain1 SwapChain { get; private set; } = null!; // built from DXGIFactory + D3DDevice + win.Handle

	// D3D11
	// =====
	public D3D11.ID3D11Texture2D BackBufferTexture { get; private set; } = null!;
	public D3D11.ID3D11RenderTargetView RenderTargetView { get; private set; } = null!;

	// DCOMP
	// =====
	public DCOMP.IDCompositionTarget DCompTarget { get; private set; } = null!;
	public DCOMP.IDCompositionVisual DCompVisual { get; private set; } = null!;
	public DCOMP.IDCompositionVisual DCompVisualChild { get; private set; } = null!;


	internal Direct2D_WinCtx(ISysWinRenderingSupport win, Direct2D_AppCtx appCtx)
	{
		this.win = win;
		this.appCtx = appCtx;

		win.WhenInit().Subscribe(_ =>
		{
			if (appCtx.DCompDevice.CreateTargetForHwnd(win.Handle, true, out var dCompTarget).Failure)
				throw new InvalidOperationException("DCompDevice.CreateTargetForHwnd failed");
			DCompTarget = dCompTarget.D(d);
			DCompVisual = appCtx.DCompDevice.CreateVisual().D(d);
			DCompVisualChild = appCtx.DCompDevice.CreateVisual().D(d);
			if (DCompVisual.AddVisual(DCompVisualChild, false, null).Failure)
				throw new InvalidOperationException("DCompVisual.AddVisual failed");

			var dpi = User32Methods.GetDpiForWindow(win.Handle);
			var dcompSurf = CreateSurfaceFromFile(ImageFilename, dpi).D(d);
			DCompVisualChild.SetContent(dcompSurf);
			DCompTarget.SetRoot(DCompVisual);
			appCtx.DCompDevice.Commit();

			SwapChain = D3D11InitUtils.Helper_CreateSwapChainForHwnd(
				appCtx.DXGIFactory,
				appCtx.D3DDevice,
				win.Handle,
				win.ClientR.V.Size
			).D(d);

			resizeSerialD = new SerialDisp<Disp>().D(d);
			InitResizeResources();
		}).D(d);

		win.ClientR
			.Where(clientR => clientR is { Width: >= 1, Height: >= 1 } && win.IsInit.V)
			.Subscribe(clientR =>
			{
				Resize(clientR.Size);
			}).D(d);

		win.WhenDisposed.Subscribe(_ =>
		{
			d.Dispose();
		}).D(d);
	}


	public void Resize(Sz sz)
	{
		if (!win.IsInit.V) return;

		resizeSerialD.Value = null;
		SwapChain.ResizeBuffers(2, sz.Width, sz.Height, DXGI.Format.B8G8R8A8_UNorm, DXGI.SwapChainFlags.None);
		InitResizeResources();
	}

	public IGfx GetGfx(nint hdc) => new Direct2D_Gfx(win, hdc, this);




	private void InitResizeResources()
	{
		resizeSerialD.Value = null;
		resizeSerialD.Value = new Disp();

		BackBufferTexture = SwapChain.GetBuffer<D3D11.ID3D11Texture2D>(0).D(ResizeD);
		RenderTargetView = appCtx.D3DDevice.CreateRenderTargetView(BackBufferTexture).D(ResizeD);

		
	}


	private (DCOMP.IDCompositionSurface, IDisposable) CreateSurfaceFromFile(string filename, int dpi)
	{
		using var localD = new Disp();
		var d2dBmp = BmpUtils.Load(filename, appCtx.D2DDeviceCtx, appCtx.WicFactory).D(localD);
		return appCtx.DCompDevice.Helper_CreateAndPaintSurface(
			d2dBmp.Size,
			dpi,
			appCtx.D2DDeviceCtx,
			(gfx, ofs) =>
			{
				var dstR = new RectangleF(ofs.X, ofs.Y, 256, 256);
				var srcR = new RectangleF(0, 0, 256, 256);
				gfx.DrawBitmap(
					d2dBmp,
					dstR,
					1,
					D2D.BitmapInterpolationMode.Linear,
					srcR
				);
			}
		);
	}

}


// *******
// * Gfx *
// *******
public class Direct2D_Gfx : IGfx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISysWinRenderingSupport win;
	private readonly nint hdc;
	private readonly Direct2D_WinCtx winCtx;

	public R R { get; }

	internal Direct2D_Gfx(ISysWinRenderingSupport win, nint hdc, Direct2D_WinCtx winCtx)
	{
		this.win = win;
		this.winCtx = winCtx;
		this.hdc = hdc;
		R = win.ClientR.V;

		winCtx.appCtx.D2DDeviceCtx.BeginDraw();
		Disposable.Create(() =>
		{
			winCtx.appCtx.D2DDeviceCtx.EndDraw();
		}).D(d);
	}

	public void Dbg()
	{
		if (!win.IsInit.V) return;
		winCtx.appCtx.D3DDeviceCtx.ClearRenderTargetView(winCtx.RenderTargetView, Colors.CornflowerBlue);
		winCtx.SwapChain.Present(1, DXGI.PresentFlags.None);

	}

	public void FillR(R r, BrushDef brush)
	{
		if (!win.IsInit.V) return;
		winCtx.appCtx.D2DDeviceCtx.FillRectangle(r.ToDrawRect(), winCtx.appCtx.pencils.GetBrush(brush));
		//gfx.FillRectangle(pencils.GetBrush(brush), r.ToRect());
	}

	public void DrawR(R r, PenDef pen)
	{
		if (!win.IsInit.V) return;
		//gfx.DrawRectangle(pencils.GetPen(pen), r.ReduceDimsByOne().ToRect());
	}
}
*/
