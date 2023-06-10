using System.Reactive.Linq;
using System.Reactive.Subjects;
using LayoutSystem.Flex;
using LayoutSystem.Utils;
using PowMaybe;
using PowRxVar;
using RenderLib.Renderers;
using SysWinInterfaces;

namespace ControlSystem;


public class Ctrl : IDisposable
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();

	private readonly IRwVar<Maybe<Win>> win;
	private readonly ISubject<IGfx> whenRender;

	/// <summary>
	/// Render event
	/// </summary>
	protected IObservable<IGfx> WhenRender => whenRender.AsObservable();

	/// <summary>
	/// Points to the window this Ctrl is attached to
	/// </summary>
	public IRoVar<Maybe<Win>> Win => win.ToReadOnly();
	
	/// <summary>
	/// The root FlexNode of this Ctrl
	/// </summary>
	public FlexNode FlexRoot { get; init; }


	public Ctrl()
	{
		win = Var.Make(May.None<Win>()).D(D);
		whenRender = new Subject<IGfx>().D(D);
		FlexRoot = new FlexNode(Vec.Fix(50, 50), Strats.Fill, Mg.Zero);
	}
}


public class Win : Ctrl
{

}


/*
public record LayNode(FlexNode FlexNode);
public record CtrlLayNode(FlexNode FlexNode, ICtrl Ctrl) : LayNode(FlexNode);


interface ICtrlLogic : IDisposable
{

}
*/


/*
public interface ICtrl : IDisposable
{
	Disp D { get; }
	TNod<LayNode> Root { get; }
	IRoVar<Maybe<IWin>> Win { get; }
}

public class Ctrl : ICtrl
{
	private readonly IRwVar<Maybe<IWin>> win;

	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();
	public TNod<LayNode> Root { get; init; } = Nod.Make(new LayNode(new FlexNode(Vec.Fix(50, 50), Mg.Zero, Strats.Fill)));
	public IRoVar<Maybe<IWin>> Win => win.ToReadOnly();

	public Ctrl()
	{
		win = Var.Make(May.None<IWin>()).D(D);
	}
}

public class CheckboxCtrl : Ctrl
{
	public CheckboxCtrl()
	{
		Root = Nod.Make(new LayNode(new FlexNode(Vec.Fix(50, 50), Mg.Zero, Strats.Fill)));
	}
}



public interface IWin : ICtrl
{
	ISysWin SysWin { get; }
}
*/