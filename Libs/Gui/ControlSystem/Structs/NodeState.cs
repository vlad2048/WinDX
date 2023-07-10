using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ControlSystem.Logic.Scrolling_;
using ControlSystem.Logic.Scrolling_.State;
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
	public string Name { get; private set; }

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

	public IObservable<Unit> WhenChanged => Obs.Merge(R.ToUnit(), ScrollState.WhenChanged);
	public IObservable<Unit> WhenInvalidateRequired => ScrollState.WhenInvalidateRequired;

	public NodeState(string? name = null)
	{
		Name = name ?? string.Empty;
		RSrc = Var.Make(PowBasics.Geom.R.Empty).D(D);
		whenEvt = new Subject<IUserEvt>().D(D);
		ScrollState = new ScrollState().D(D);
	}

	public override string ToString() => Name;

	internal void SetNameIFN(string name)
	{
		if (Name == string.Empty)
			Name = name;
	}
}
