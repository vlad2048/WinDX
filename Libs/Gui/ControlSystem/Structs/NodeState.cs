using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Scrolling_;
using PowBasics.Geom;
using PowRxVar;
using UserEvents;
using UserEvents.Structs;

namespace ControlSystem.Structs;




public sealed class NodeState : INodeStateUserEventsSupport, IDisposable
{
	public Disp D { get; } = new();
	public void Dispose() => D.Dispose();

	private readonly ISubject<IUserEvt> whenEvt;

	// ************
	// * Internal *
	// ************
	/// <summary>
	/// Source for R
	/// </summary>
	internal IRwVar<R> RSrc { get; }

	// **********
	// * Public *
	// **********
	/// <summary>
	/// Rectangle in main window coordinates where the node is drawn. <br/>
	/// ● Assigned during the window layout (PAINT event). <br/>
	/// ● For nodes in Popup windows, refers to the original rectangle relative to the main window. <br/>
	/// </summary>
	public IRoVar<R> R => RSrc.ToReadOnly();


	/// <summary>
	/// Translated window events (+ generated MouseEnter/MouseLeave). <br/>
	/// </summary>
	public IObservable<IUserEvt> Evt => whenEvt.AsObservable();

	/// <summary>
	/// Source for Evt. <br/>
	/// ● Assigned by the HitTesting logic. <br/>
	/// ● It should be internal, but we can't as it's accessed from the UserEvents project through the INodeStateUserEventsSupport interface. <br/>
	/// </summary>
	public void DispatchEvt(IUserEvt evt) => whenEvt.OnNext(evt);

	public ScrollState ScrollState { get; }

	public NodeState()
	{
		RSrc = Var.Make(PowBasics.Geom.R.Empty).D(D);
		whenEvt = new Subject<IUserEvt>().D(D);
		ScrollState = new ScrollState().D(D);
	}
}


public static class NodeStateMaker
{
	public static (NodeState, NodeState, NodeState, NodeState, NodeState, NodeState, NodeState, NodeState) Make8(IRoDispBase d)
	{
		var n0 = new NodeState().D(d);
		var n1 = new NodeState().D(d);
		var n2 = new NodeState().D(d);
		var n3 = new NodeState().D(d);
		var n4 = new NodeState().D(d);
		var n5 = new NodeState().D(d);
		var n6 = new NodeState().D(d);
		var n7 = new NodeState().D(d);
		return (n0, n1, n2, n3, n4, n5, n6, n7);
	}
}