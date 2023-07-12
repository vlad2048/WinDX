using System.Reactive.Linq;
using PowRxVar;
using RenderLib;
using RenderLib.Renderers;
using SysWinLib;
using UserEvents;

namespace ControlSystem.Logic.Rendering_;

sealed class RendererSwitcher : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly SysWin sysWin;
	private readonly SerialDisp<IRenderWinCtx> serD;
	private readonly IRoVar<RendererType> renderType;

	public IRenderWinCtx Renderer
	{
		get
		{
			if (serD.Value == null)
				serD.Value = RendererGetter.Get(renderType.V, sysWin);
			return serD.Value;
		}
	}

	public RendererSwitcher(SysWin sysWin, IInvalidator? invalidator)
	{
		this.sysWin = sysWin;
		serD = new SerialDisp<IRenderWinCtx>().D(d);
		renderType = Cfg.SelectVar(e => (RendererType)e.Options.Renderer);

		if (invalidator != null) // mainWin only
			renderType
				.Skip(1).Subscribe(_ =>
				{
					serD.Value = null;
					invalidator.Invalidate(RedrawReason.RendererSwitched);
				}).D(d);
	}
}