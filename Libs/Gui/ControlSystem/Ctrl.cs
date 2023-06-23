using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Structs;
using PowMaybe;
using PowRxVar;

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
	internal IRwVar<Maybe<Win>> WinSrc { get; }

	internal void SignalRender(RenderArgs e) => whenRender.OnNext(e);


	// **********
	// * Public *
	// **********
	/// <summary>
	/// Points to the window this Ctrl is attached to
	/// </summary>
	public IRoVar<Maybe<Win>> Win => WinSrc.ToReadOnly();


	public Ctrl()
	{
		WinSrc = Var.Make(May.None<Win>()).D(D);
		whenRender = new Subject<RenderArgs>().D(D);
	}
}
