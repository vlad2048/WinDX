using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowRxVar;

namespace ControlSystem.Logic.Scrolling_.Structs;

sealed class Duo : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly ScrollBarCtrl? scrollBarX;
	private readonly ScrollBarCtrl? scrollBarY;
	private readonly ScrollBarCornerCtrl? scrollBarCorner;

	public Duo(StFlexNode st)
	{
		var scroll = st.Flex.Flags.Scroll;
		if (scroll == BoolVec.False) throw new ArgumentException("Impossible");

		scrollBarX = scroll.X ? new ScrollBarCtrl(Dir.Horz, st.State.ScrollState.X, st.State.Evt).D(d) : null;
		scrollBarY = scroll.Y ? new ScrollBarCtrl(Dir.Vert, st.State.ScrollState.Y, st.State.Evt).D(d) : null;
		scrollBarCorner = (scroll == BoolVec.True) ? new ScrollBarCornerCtrl().D(d) : null;
	}

	public ScrollBarCtrl Get(Dir dir) => dir switch
	{
		Dir.Horz => scrollBarX ?? throw new ArgumentException("Impossible"),
		Dir.Vert => scrollBarY ?? throw new ArgumentException("Impossible"),
	};

	public ScrollBarCornerCtrl GetCorner() => scrollBarCorner ?? throw new ArgumentException("Impossible");
}