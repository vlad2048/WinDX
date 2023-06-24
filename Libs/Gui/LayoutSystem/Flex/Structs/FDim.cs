namespace LayoutSystem.Flex.Structs;


// ******************
// * Dimension Type *
// ******************
public enum DimType
{
	/// <summary>
	/// Fixed size <br/> 
	/// Min = Max ≠ ∞ <br/>
	/// </summary>
	Fix,

	/// <summary>
	/// Ideal size:    Max <br/>
	/// Required size: Min <br/>
	/// <![CDATA[  Min < Max < ∞  ]]> <br/>
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

public readonly record struct FDim
{
	private const int INF = int.MaxValue;

	public int Min { get; }
	public int Max { get; }
	public double Mult => 1;
	public DimType Type { get; }

	public FDim(int min, int max)
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
			(truer, false) => DimType.Fix,
			(false, false) => DimType.Flt,
			(false, truer) => DimType.Fil,
			_ => throw new ArgumentException()
		};
	}

	public override string ToString() => Type switch
	{
		DimType.Fix => $"Fix({Min})",
		DimType.Flt => $"Flt({Min}-{Max})",
		DimType.Fil => "Fil",
		DimType.Fit => throw new ArgumentException("Impossible")
	};
}
