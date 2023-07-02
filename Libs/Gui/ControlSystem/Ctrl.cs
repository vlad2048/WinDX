using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using RenderLib.Renderers;
using RenderLib.Structs;

namespace ControlSystem;


public class Ctrl : IDisposable
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();


	// ***********
	// * Private *
	// ***********
	private readonly ISubject<RenderArgs> whenRender;


	// *************
	// * Protected *
	// *************
	/// <summary>
	/// Render event
	/// </summary>
	protected IObservable<RenderArgs> WhenRender => whenRender.AsObservable();


	// ************
	// * Internal *
	// ************
	internal IRwMayVar<Win> WinSrc { get; }

	internal void SignalRender(RenderArgs e) => whenRender.OnNext(e);


	// **********
	// * Public *
	// **********
	/// <summary>
	/// Points to the window this Ctrl is attached to
	/// </summary>
	public IRoMayVar<Win> Win => WinSrc.ToReadOnlyMay();


	public Ctrl()
	{
		WinSrc = VarMay.Make<Win>().D(D);
		whenRender = new Subject<RenderArgs>().D(D);
	}
}


public static class CtrlExt
{
	public static void Invalidate(this Ctrl ctrl)
	{
		if (!ctrl.Win.V.IsSome(out var win)) return;
		win.InvalidateAll();
	}
}



public static class IGfxExt
{
	public static void FillR(this IGfx gfx, BrushDef brush) => gfx.FillR(gfx.R, brush);
	public static void DrawR(this IGfx gfx, PenDef pen) => gfx.DrawR(gfx.R, pen);
	public static void FillDrawR(this IGfx gfx, BrushDef brush, PenDef pen) => gfx.FillDrawR(gfx.R, brush, pen);

	public static void FillDrawR(this IGfx gfx, R r, BrushDef brush, PenDef pen)
	{
		gfx.FillR(r, brush);
		gfx.DrawR(r, pen);
	}
}
