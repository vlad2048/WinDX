using PowRxVar;
using RenderLib.Renderers;
using RenderLib.Renderers.Direct2D;
using RenderLib.Renderers.Direct2DInDirect3D;
using RenderLib.Renderers.GDIPlus;

namespace RenderLib;

public class RendererGetter
{
	private static readonly Lazy<IRenderAppCtx> gdiplusAppCtx = new(() => new GDIPlus_AppCtx().DisposeOnProgramExit());
	private static readonly Lazy<IRenderAppCtx> direct2dAppCtx = new(() => new Direct2D_AppCtx().DisposeOnProgramExit());
	private static readonly Lazy<IRenderAppCtx> direct2dindirect3dAppCtx = new(() => new Direct2DInDirect3D_AppCtx().DisposeOnProgramExit());

	public static IRenderAppCtx Get(RendererType type) => type switch
	{
		RendererType.GDIPlus => gdiplusAppCtx.Value,
		RendererType.Direct2D => direct2dAppCtx.Value,
		RendererType.Direct2DInDirect3D => direct2dindirect3dAppCtx.Value,
		_ => throw new ArgumentException()
	};
}