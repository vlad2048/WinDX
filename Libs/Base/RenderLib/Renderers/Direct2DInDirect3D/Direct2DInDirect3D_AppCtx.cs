using System.Numerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using PowBasics.Geom;
using PowRxVar;
using RenderLib.Renderers.Utils.DirectX;
using RenderLib.Structs;
using SysWinInterfaces;
using Vortice.DCommon;
using WinAPI.Utils.Exts;

namespace RenderLib.Renderers.Direct2DInDirect3D;


/*

                                                   D2DFactory.CreateDxgiSurfaceRenderTarget() ──────╮                                                             
                                                                                                    │                                                             
                                                                                                    │                                                             
                                                                                                    │                                                             
       DXGIFactory.CreateSwapChainForHwnd() ──────╮           .GetBuffer(0)                         │       
                                                  │         ╔════════════════ DXGISurface ══ D2DRenderTarget
                                                  │         ║                                                                                                                                                              
                                        ╔══ DXGISwapChain ══╣                                                                                                                    
                                        ║                   ║ .GetBuffer(0)                                                                                                                    
                      ╔══ D3D11Device ══╣                   ╚════════════════ D3D11Texture2D ══ D3DRenderTargetView                                                                                                                    
                      ║                 ║                                                                │                                                  
                      ║                 ╚══ DXGIDevice ══╗                                               │                                                                 
D3D11CreateDevice() ══╣                                  ║          D3DDevice.CreateRenderTargetView()───╯                                  
                      ║                                  ║                                                                                  
                      ╚══ D3D11DeviceContext             ║                                                                                                       
                                                         ║                                                           
                                                         ║                                                           
                                                         ║                                                           
                                                         ║                       .CreateTargetForHwnd()                                 
                                                         ╚══ DCompositionDevice ════════════════════════ DCompositionTarget                                                     
                                                                     │                                               
                                                                     │                                               
                                        DCompositionCreateDevice()───╯                                                    


*/


// **********
// * AppCtx *
// **********
public class Direct2DInDirect3D_AppCtx : IRenderAppCtx
{
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


	internal Direct2DInDirect3D_AppCtx()
	{
		DXGIFactory = DXGI.DXGI.CreateDXGIFactory1<DXGI.IDXGIFactory2>().D(d);

		(D3DDevice, D3DDeviceCtx, DXGIDevice) = D3D11InitUtils.Helper_D3D11CreateDevice<D3D11.ID3D11Device1, D3D11.ID3D11DeviceContext1, DXGI.IDXGIDevice>().D(d);

		D2DFactory = D2D.D2D1.D2D1CreateFactory<D2D.ID2D1Factory1>(D2D.FactoryType.SingleThreaded, D2D.DebugLevel.Information).D(d);
	}

	public IRenderWinCtx GetWinCtx(ISysWinRenderingSupport win) => new Direct2DInDirect3D_WinCtx(win, this);
}


// **********
// * WinCtx *
// **********
public class Direct2DInDirect3D_WinCtx : IRenderWinCtx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly SerialDisp<Disp> resizeSerialD;


	private readonly ISysWinRenderingSupport win;
	public Direct2DInDirect3D_AppCtx AppCtx { get; }


	internal Pencils Pencils { get; private set; } = null!;

	// DXGI
	// ====
	public DXGI.IDXGISwapChain1 SwapChain { get; private set; } = null!; // built from DXGIFactory + D3DDevice + win.Handle

	// D3D11
	// =====
	public D3D11.ID3D11Texture2D BackBufferTexture { get; private set; } = null!;
	public D3D11.ID3D11RenderTargetView RenderTargetView { get; private set; } = null!;

	// D2D
	// ===
	public D2D.ID2D1RenderTarget D2DRenderTarget { get; private set; } = null!;


	internal Direct2DInDirect3D_WinCtx(ISysWinRenderingSupport win, Direct2DInDirect3D_AppCtx appCtx)
	{
		this.win = win;
		AppCtx = appCtx;
		resizeSerialD = new SerialDisp<Disp>().D(d);

		win.WhenInit().Subscribe(_ =>
		{
			SwapChain = D3D11InitUtils.Helper_CreateSwapChainForHwnd(
				appCtx.DXGIFactory,
				appCtx.D3DDevice,
				win.Handle,
				win.ClientR.V.Size
			).D(d);

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
		SwapChain.ResizeBuffers(2, sz.Width, sz.Height, DXGI.Format.B8G8R8A8_UNorm, DXGI.SwapChainFlags.None);
		InitResizeResources();
	}

	public IGfx GetGfx() => new Direct2DInDirect3D_Gfx(win, this);




	private void InitResizeResources()
	{
		resizeSerialD.Value = null;
		var resizeD = new Disp();
		resizeSerialD.Value = resizeD;

		BackBufferTexture = SwapChain.GetBuffer<D3D11.ID3D11Texture2D>(0).D(resizeD);
		RenderTargetView = AppCtx.D3DDevice.CreateRenderTargetView(BackBufferTexture).D(resizeD);

		var DXGISurface = SwapChain.GetBuffer<DXGI.IDXGISurface>(0).D(resizeD);
		var renderTargetProperties = new D2D.RenderTargetProperties(
			D2D.RenderTargetType.Default,
			new PixelFormat(DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore),
			0, 0,
			D2D.RenderTargetUsage.None,
			D2D.FeatureLevel.Default
		);
		D2DRenderTarget = AppCtx.D2DFactory.CreateDxgiSurfaceRenderTarget(DXGISurface, renderTargetProperties).D(resizeD);

		Pencils = new Pencils(AppCtx.D2DFactory, D2DRenderTarget).D(resizeD);
	}
}


// *******
// * Gfx *
// *******
public class Direct2DInDirect3D_Gfx : IGfx
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ISysWinRenderingSupport win;
	public Direct2DInDirect3D_AppCtx AppCtx { get; }
	public Direct2DInDirect3D_WinCtx WinCtx { get; }
	public Pencils Pencils { get; }

	public D2D.ID2D1RenderTarget T { get; }
	public D2D.ID2D1Factory1 D2DFactory { get; }

	public R R { get; }

	internal Direct2DInDirect3D_Gfx(ISysWinRenderingSupport win, Direct2DInDirect3D_WinCtx winCtx)
	{
		this.win = win;
		WinCtx = winCtx;
		AppCtx = winCtx.AppCtx;
		Pencils = winCtx.Pencils;
		R = win.ClientR.V;
		T = winCtx.D2DRenderTarget;
		D2DFactory = winCtx.AppCtx.D2DFactory;

		T.BeginDraw();
		Disposable.Create(() =>
		{
			T.EndDraw();
			WinCtx.SwapChain.Present(0);
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

