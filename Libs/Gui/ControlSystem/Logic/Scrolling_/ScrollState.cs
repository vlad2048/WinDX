using PowRxVar;

namespace ControlSystem.Logic.Scrolling_;


public sealed class ScrollDimState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();


	public void DisableScroll()
	{

	}
}


public sealed class ScrollState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public ScrollDimState X { get; }
	public ScrollDimState Y { get; }

	public ScrollState()
	{
		X = new ScrollDimState().D(d);
		Y = new ScrollDimState().D(d);
	}
}
