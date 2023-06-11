using D3DPlay.Rendering.Utils;
using PowBasics.Geom;
using PowRxVar;
using SysWinInterfaces;
using Vortice.DCommon;

namespace D3DPlay.Rendering;

public class RenderAppCtx : IDisposable
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


	public RenderAppCtx()
	{
		DXGIFactory = DXGI.DXGI.CreateDXGIFactory1<DXGI.IDXGIFactory2>().D(d);
		var creationFlags = D3D11.DeviceCreationFlags.BgraSupport;
		if (D3D11.D3D11.SdkLayersAvailable())
			creationFlags |= D3D11.DeviceCreationFlags.Debug;

		(D3DDevice, D3DDeviceCtx, DXGIDevice) = D3D11InitUtils.Helper_D3D11CreateDevice<D3D11.ID3D11Device1, D3D11.ID3D11DeviceContext1, DXGI.IDXGIDevice>(
			null,
			D3D.DriverType.Hardware,
			creationFlags,
			RConsts.FeatureLevels
		).D(d);

		D2DFactory = D2D.D2D1.D2D1CreateFactory<D2D.ID2D1Factory1>(D2D.FactoryType.SingleThreaded, D2D.DebugLevel.Information).D(d);
	}

	public RenderWinCtx GetWinCtx(ISysWinRenderingSupport win) => new(this, win);
}



public class RenderWinCtx : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly SerialDisp<Disp> resizeSerialD = null!;
	private Disp ResizeD => resizeSerialD.Value ?? throw new ArgumentException();

	private readonly ISysWinRenderingSupport win;

	public RenderAppCtx AppCtx { get; }

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


	public RenderWinCtx(RenderAppCtx appCtx, ISysWinRenderingSupport win)
	{
		AppCtx = appCtx;
		this.win = win;

		SwapChain = D3D11InitUtils.Helper_CreateSwapChainForHwnd(
			appCtx.DXGIFactory,
			appCtx.D3DDevice,
			win.Handle,
			win.ClientR.V.Size
		).D(d);

		resizeSerialD = new SerialDisp<Disp>().D(d);
		InitResizeResources();

	}

	public RenderGfx GetGfx() => new(this);


	public void Resize(Sz sz)
	{
		if (!win.IsInit.V) return;

		resizeSerialD.Value = null;
		SwapChain.ResizeBuffers(2, sz.Width, sz.Height, DXGI.Format.B8G8R8A8_UNorm, DXGI.SwapChainFlags.None);
		InitResizeResources();
	}



	private void InitResizeResources()
	{
		resizeSerialD.Value = null;
		resizeSerialD.Value = new Disp();

		BackBufferTexture = SwapChain.GetBuffer<D3D11.ID3D11Texture2D>(0).D(ResizeD);
		RenderTargetView = AppCtx.D3DDevice.CreateRenderTargetView(BackBufferTexture).D(ResizeD);

		var DXGISurface = SwapChain.GetBuffer<DXGI.IDXGISurface>(0).D(ResizeD);
		var renderTargetProperties = new D2D.RenderTargetProperties(
			D2D.RenderTargetType.Default,
			new PixelFormat(DXGI.Format.B8G8R8A8_UNorm, AlphaMode.Ignore),
			0, 0,
			D2D.RenderTargetUsage.None,
			D2D.FeatureLevel.Default
		);
		D2DRenderTarget = AppCtx.D2DFactory.CreateDxgiSurfaceRenderTarget(DXGISurface, renderTargetProperties).D(ResizeD);
	}
}





public class RenderGfx : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public RenderAppCtx AppCtx { get; }
	public RenderWinCtx WinCtx { get; }

	public D2D.ID2D1RenderTarget T { get; }

	public RenderGfx(RenderWinCtx winCtx)
	{
		WinCtx = winCtx;
		AppCtx = WinCtx.AppCtx;
		T = WinCtx.D2DRenderTarget;
	}
}