using PowBasics.Geom;
using Dr = PowBasics.Geom.Dir;

namespace LayoutSystem.Flex.Structs;


// ******************
// * Dimension Type *
// ******************
public enum DType
{
	/// <summary>
	/// Fixed size <br/> 
	/// Min = Max ≠ ∞ <br/>
	/// </summary>
	Fix,

	/// <summary>
	/// Ideal size:    Max <br/>
	/// Required size: Min <br/>
	/// Min &lt; Max &lt; ∞ <br/>
	/// </summary>
	Flt,

	/// <summary>
	/// Fill the parent container 
	/// Min = 0 <br/>
	/// Max = ∞ <br/>
	/// 
	/// Note:
	/// This is not allowed in some contexts
	/// for example if the parent is set to fit to content, then its children
	/// cannot be set to fill the parent in this dimension.
	/// A preprocessing step replace those Fil by Fix(0)
	/// </summary>
	Fil,

	Fit
}

public readonly record struct Dim
{
	private const int INF = int.MaxValue;

	public int Min { get; }
	public int Max { get; }
	public double Mult => 1;
	public DType Type { get; }

	public Dim(int min, int max)
	{
		Min = min;
		Max = max;
// @formatter:off
		var isValid =
			Min >= 0 && Max >= 0 && Max >= Min && (
				(Min == Max && Max != INF) ||
				(Min != Max && Max != INF) ||
				(Min ==   0 && Max == INF)
			);
// @formatter:on
		if (!isValid) throw new ArgumentException();
		Type = (Min == Max, Max == INF) switch
		{
			(truer, false) => DType.Fix,
			(false, false) => DType.Flt,
			(false, truer) => DType.Fil,
			_ => throw new ArgumentException()
		};
	}

	public override string ToString() => Type switch
	{
		DType.Fix => $"Fix({Min})",
		DType.Flt => $"Flt[{Min}-{Max}]",
		DType.Fil => "Fil",
		DType.Fit => throw new ArgumentException("Impossible")
	};
}


// ****************
// * Vector Types *
// ****************
public readonly record struct DimVec(Dim X, Dim Y)
{
	public override string ToString() => $"{X} x {Y}";
}

public readonly record struct DimOptVec(DimOpt X, DimOpt Y)
{
	public bool IsResolvable => X.HasValue && Y.HasValue;
	public DimVec ResolveEnsure() => IsResolvable switch
	{
		true  => new DimVec(X!.Value, Y!.Value),
		false => throw new ArgumentException()
	};
	public override string ToString() => $"{X.Fmt()} x {Y.Fmt()}";
}

public readonly record struct VecBool(bool X, bool Y)
{
	public override string ToString() => $"{X} x {Y}";
	public bool Dir(Dr dir) => dir switch
	{
		Dr.Horz => X,
		Dr.Vert => Y,
	};
	public static VecBool MkDir(Dr dir, bool x, bool y) => dir switch
	{
		Dr.Horz => new(x, y),
		Dr.Vert => new(y, x),
	};
}


// **************
// * Formatting *
// **************
public static class DimFmtExts
{
	public static string Fmt(this DimOpt v) => v.HasValue ? $"{v}" : "Fit";
}


// **********
// * Makers *
// **********
// @formatter:off
public static class D
{
	public static          Dim    Fix(int val         ) => new(val, val         );
	public static          Dim    Flt(int min, int max) => new(min, max         );
	public static readonly Dim    Fil                   =  new(0  , int.MaxValue);
	public static readonly DimOpt Fit                   =  null;
}

public static class Vec
{
	public static          DimOptVec Mk (Dim x, Dim y) => new(x       , y       );
	public static readonly DimOptVec Fit               =  new(null    , null    );
	public static          DimOptVec Fix(int x, int y) => new(D.Fix(x), D.Fix(y));
	public static readonly DimOptVec Fil               =  new(D.Fil   , D.Fil   );

	public static readonly DimOptVec FitFil            =  new(null    , D.Fil   );
	public static readonly DimOptVec FilFit            =  new(D.Fil   , null    );

	public static          DimOptVec FilFix(int y)     => new(D.Fil   , D.Fix(y));
	public static          DimOptVec FixFil(int x)     => new(D.Fix(x), D.Fil   );
}
// @formatter:on


// ************************
// * Converter Extensions *
// ************************
static class DimConvertExts
{
	public static DimVec ResolveWithSizeFailover(this DimOptVec dim, Sz sz) => new(
		dim.X ?? D.Fix(sz.Width),
		dim.Y ?? D.Fix(sz.Height)
	);
	public static DimVec ToDimVec(this Sz sz) => new(D.Fix(sz.Width), D.Fix(sz.Height));
	//public static DimOptVec ToDimOptVec(this Sz sz) => new(D.Fix(sz.Width), D.Fix(sz.Height));
}


// **************
// * Extractors *
// **************
public static class DimExtractors
{
	public static int GetX(Sz sz) => sz.Width;
	public static int GetY(Sz sz) => sz.Height;
	public static Dim GetX(DimVec vec) => vec.X;
	public static Dim GetY(DimVec vec) => vec.Y;
	public static DimOpt GetX(DimOptVec vec) => vec.X;
	public static DimOpt GetY(DimOptVec vec) => vec.Y;

	public static bool IsFit(this DimOpt d) => d.Typ() == DType.Fit;
	public static bool IsFil(this DimOpt d) => d.Typ() == DType.Fil;

	public static DType Typ(this DimOpt d) => d.HasValue switch
	{
		truer => d.Value.Type,
		false => DType.Fit
	};
}


// *******************
// * Apply functions *
// *******************
static class DimApplyExts
{
	public static DimOptVec Apply(this DimOptVec vec, Func<DimOpt, DimOpt> fun) => new(
		fun(vec.X),
		fun(vec.Y)
	);
	public static DimVec Apply(this DimVec vec, Func<Dim, Dim> fun) => new(
		fun(vec.X),
		fun(vec.Y)
	);
	public static DimVec Apply(this DimOptVec vec, Func<DimOpt, Dim> fun) => new(
		fun(vec.X),
		fun(vec.Y)
	);
}


// **************
// * Dir Makers *
// **************
static class DirMakers
{
	public static Pt MkPt(Dr dir, int x, int y) => dir switch
	{
		Dr.Horz => new(x, y),
		Dr.Vert => new(y, x),
	};

	public static Sz MkSz(Dr dir, int x, int y) => dir switch
	{
		Dr.Horz => new(x, y),
		Dr.Vert => new(y, x),
	};

	public static DimVec MkDimVec(Dr dir, Dim x, Dim y) => dir switch
	{
		Dr.Horz => new(x, y),
		Dr.Vert => new(y, x),
	};

	public static DimOptVec MkDimOptVec(Dr dir, DimOpt x, DimOpt y) => dir switch
	{
		Dr.Horz => new(x, y),
		Dr.Vert => new(y, x),
	};
}


// ******************
// * Dir Extensions *
// ******************
public static class DirExts
{
	public static T Dir<T>(this (T, T) t, Dr dir) => dir switch
	{
		Dr.Horz => t.Item1,
		Dr.Vert => t.Item2,
	};

	public static DimOpt Dir(this DimOptVec vec, Dr dir) => dir switch
	{
		Dr.Horz => vec.X,
		Dr.Vert => vec.Y,
		_ => throw new ArgumentException()
	};

	public static Dim Dir(this DimVec sz, Dr dir) => dir switch
	{
		Dr.Horz => sz.X,
		Dr.Vert => sz.Y,
		_ => throw new ArgumentException()
	};
}


// ******************
// * Cap Extensions *
// ******************
static class DimCapExts
{
	public static int Cap(this int v, int min, int max) => Math.Max(Math.Min(v, max), min);

	public static int CapMax(this int v, int max) => Math.Min(v, max);
	
	public static int Cap0(this int v) => Math.Max(v, 0);

	public static int Cap(this int v, DimOpt d) => d.HasValue switch
	{
		truer => v.Cap(d.Value),
		false => v,
	};

	public static int Cap(this int v, Dim d) => v.Cap(d.Min, d.Max);
}