using System.Reactive;
using ControlSystem.Logic.Scrolling_.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowRxVar;

namespace ControlSystem.Logic.Scrolling_.State;

public sealed class ScrollStateVec : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	public ScrollState X { get; }
	public ScrollState Y { get; }

	internal IObservable<Unit> WhenChanged => Obs.Merge(X.WhenChanged, Y.WhenChanged);
	internal IObservable<IScrollCmd> WhenCmd => Obs.Merge(X.WhenCmd, Y.WhenCmd);

	public Pt ScrollOfs => new (X.ScrollOfs, Y.ScrollOfs);
	public IObservable<Unit> WhenInvalidateRequired => Obs.Merge(X.WhenInvalidateRequired, Y.WhenInvalidateRequired);

	public ScrollStateVec()
	{
		X = new ScrollState(Dir.Horz).D(d);
		Y = new ScrollState(Dir.Vert).D(d);
	}

	internal void UpdateFromLayout(ScrollNfo nfo)
	{
		void Do(Dir dir)
		{
			var st = Get(dir);
			st.UpdateFromLayout(
				nfo.State.Dir(dir),
				nfo.View.Dir(dir),
				nfo.Cont.Dir(dir)
			);
		}

		Do(Dir.Horz);
		Do(Dir.Vert);
	}

	private ScrollState Get(Dir dir) => dir switch
	{
		Dir.Horz => X,
		Dir.Vert => Y
	};
}
