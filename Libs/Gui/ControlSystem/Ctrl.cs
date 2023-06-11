using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using RenderLib.Renderers;

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
	internal IRwVar<Maybe<Win>> WinRW { get; }
	internal void SignalRender(RenderArgs e) => whenRender.OnNext(e);


	// **********
	// * Public *
	// **********
	/// <summary>
	/// Points to the window this Ctrl is attached to
	/// </summary>
	public IRoVar<Maybe<Win>> Win => WinRW.ToReadOnly();
	
	public NodeState RootState { get; }
	public FlexNode RootFlex { get; }


	public Ctrl()
	{
		WinRW = Var.Make(May.None<Win>()).D(D);
		whenRender = new Subject<RenderArgs>().D(D);
		RootState = new NodeState().D(D);
		RootFlex = new FlexNode(Vec.Fix(50, 50), Strats.Fill, Mg.Zero);
	}
}
