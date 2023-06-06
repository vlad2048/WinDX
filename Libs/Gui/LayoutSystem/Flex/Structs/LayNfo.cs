using LayoutSystem.Utils.Exts;
using PowBasics.Geom;

namespace LayoutSystem.Flex.Structs;


public record LayNfo(
    Sz ResolvedSz,
    R[] Kids
);


static class LayNfoExts
{
	public static LayNfo CapWithFreeSize(this LayNfo nfo, FreeSz free) => new(
		nfo.ResolvedSz.GetIntersection(free),
		nfo.Kids.Map(kidR => kidR.GetIntersection(free))
	);

	private static Sz GetIntersection(this Sz sz, FreeSz free)
	{
		var r = new R(Pt.Empty, sz);
		return r.GetIntersection(free).Size;
	}

	private static R GetIntersection(this R r, FreeSz free)
	{
		var width = free.IsInfinite(Dir.Horz) ? int.MaxValue : free.X;
		var height = free.IsInfinite(Dir.Vert) ? int.MaxValue : free.Y;
		var rFree = new R(Pt.Empty, new Sz(width, height));
		return r.Intersection(rFree);
	}
}