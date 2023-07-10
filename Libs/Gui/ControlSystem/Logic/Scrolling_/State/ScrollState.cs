using System.Reactive;
using System.Reactive.Linq;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowRxVar;

namespace ControlSystem.Logic.Scrolling_.State;

public sealed class ScrollState : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public ScrollDimState X { get; }
	public ScrollDimState Y { get; }

	internal IObservable<Unit> WhenChanged => Obs.Merge(X.WhenChanged, Y.WhenChanged);
	internal IObservable<IScrollCmd> WhenCmd => Obs.Merge(X.WhenCmd, Y.WhenCmd);

	public Pt ScrollOfs => new (X.ScrollOfs, Y.ScrollOfs);
	public IObservable<Unit> WhenInvalidateRequired => Obs.Merge(X.WhenInvalidateRequired, Y.WhenInvalidateRequired);

	public ScrollState()
	{
		X = new ScrollDimState(Dir.Horz).D(d);
		Y = new ScrollDimState(Dir.Vert).D(d);
	}

	internal void UpdateFromLayout(ScrollNfo nfo)
	{
		void Do(Dir dir)
		{
			var st = Get(dir);
			st.UpdateFromLayout(
				nfo.Enabled.Dir(dir),
				nfo.Visible.Dir(dir),
				nfo.ViewSz.Dir(dir),
				nfo.ContSz.Dir(dir)
			);
		}

		Do(Dir.Horz);
		Do(Dir.Vert);
	}

	private ScrollDimState Get(Dir dir) => dir switch
	{
		Dir.Horz => X,
		Dir.Vert => Y
	};
}
