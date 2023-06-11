using PowBasics.Geom;
using PowRxVar;
using UserEvents;
using UserEvents.Structs;
using UserEvents.Utils;

namespace ControlSystem.Structs;

public class NodeState : INodeState, IDisposable
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