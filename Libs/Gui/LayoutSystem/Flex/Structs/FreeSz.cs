using LayoutSystem.Flex.LayStrats;
using PowBasics.Geom;
using Dr = PowBasics.Geom.Dir;

namespace LayoutSystem.Flex.Structs;

public readonly record struct FreeSz
{
	private const int Inf = int.MaxValue;
	private const string InfMsg = "Cannot access an infinite dimension";

	private readonly int x;
	private readonly int y;

	public int X => x != Inf ? x : throw new ArgumentException(InfMsg);
	public int Y => y != Inf ? y : throw new ArgumentException(InfMsg);

	public FreeSz(int x, int y)
	{
		this.x = x;
		this.y = y;
		if (x < 0 || y < 0) throw new ArgumentException("Dimensions cannot be negative");
	}

	public override string ToString()
	{
		static string Fmt(int v) => v == Inf ? "inf" : $"{v}";
		return $"{Fmt(x)} x {Fmt(y)}";
	}

	public bool IsInfinite(Dr dir) => dir switch
	{
		Dr.Horz => x == Inf,
		Dr.Vert => y == Inf,
	};

	public int DirWithInfinites(Dr dir) => dir switch
	{
		Dr.Horz => x,
		Dr.Vert => y,
	};

	public FreeSz ChangeComponent(Dr dir, int v) => dir switch
	{
		Dr.Horz => new(v, y),
		Dr.Vert => new(x, v),
	};

	public DimVec GetDims() => new(
		IsInfinite(Dr.Horz) ? D.Fit : D.Fix(X),
		IsInfinite(Dr.Vert) ? D.Fit : D.Fix(Y)
	);

	public static FreeSz MkDir(Dr dir, int x, int y) => dir switch
	{
		Dr.Horz => new(x, y),
		Dr.Vert => new(y, x)
	};

	public FreeSz BoundWith(FreeSz dad) => new(
		Math.Min(x, dad.x),
		Math.Min(y, dad.y)
	);

	public static FreeSz MakeForKid(DimVec dim, R r) => FreeSzMaker.DirFun(
		dir => dim.Dir(dir).IsFit() switch
		{
			truer => int.MaxValue,
			false => r.Dir(dir)
		}
	);


	public FreeSz UnbridleScrolls(Node node)
	{
		var f = this;
		return node.V.Strat switch
		{
			ScrollStrat { Enabled: var enabled } => FreeSzMaker.DirFun(dir => enabled.Dir(dir) ? int.MaxValue : f.x),
			_ => this
		};
	}
}


static class FreeSzExts
{
	public static Sz CapWith(this Sz sz, FreeSz free) => new(
		free.IsInfinite(Dr.Horz) ? sz.Width : Math.Min(sz.Width, free.X),
		free.IsInfinite(Dr.Vert) ? sz.Height : Math.Min(sz.Height, free.Y)
	);
}
