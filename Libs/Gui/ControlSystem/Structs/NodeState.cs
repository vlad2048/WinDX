using PowBasics.Geom;
using PowRxVar;
using UserEvents;
using UserEvents.Structs;
using UserEvents.Utils;

namespace ControlSystem.Structs;

public sealed class NodeState : INodeState, IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();


	// **********
	// * Public *
	// **********
	public R R { get; set; } = R.Empty;

	/// <summary>
	/// Translated window events (+ generated MouseEnter/MouseLeave)
	/// assigned by the HitTesting logic
	/// </summary>
	public IRwVar<IUIEvt> EvtSrc { get; }

	public IUIEvt Evt { get; }


	public NodeState()
	{
		(EvtSrc, Evt) = UserEvtGenerator.MakeWithSource().D(d);
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