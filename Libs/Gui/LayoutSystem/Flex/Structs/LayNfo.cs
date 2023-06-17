using LayoutSystem.Utils.Exts;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace LayoutSystem.Flex.Structs;


public sealed record LayNfo(
	Sz ResolvedSz,
	R[] Kids
)
{
	public override string ToString() => $"{ResolvedSz} - [{Kids.Select(e => $"({e})").JoinText()}]";

	public bool IsSame(LayNfo l) =>
		l.ResolvedSz == ResolvedSz &&
		l.Kids.Length == Kids.Length &&
		l.Kids.Zip(Kids).All(t => t.First == t.Second);
}


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
		var width = free.X ?? int.MaxValue;
		var height = free.Y ?? int.MaxValue;
		var rFree = new R(Pt.Empty, new Sz(width, height));
		return r.Intersection(rFree);
	}
}