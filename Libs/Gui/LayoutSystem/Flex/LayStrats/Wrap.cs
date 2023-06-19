using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;
using System.Text.Json.Serialization;
using static LayoutSystem.Flex.LayStrats.WrapUtils;

namespace LayoutSystem.Flex.LayStrats;

/// <summary>
/// Stack the kids in a specific direction (MainDir / elseDir)
/// but wraps to new rows(/columns) when running out of space
///   - DimVec is ignored, instead we use
///     - if MainDir = Horz -> (Fix, Fit)
///     - if MainDir = Vert -> (Fit, Fix)
///   - children are not allowed to use Fills
///   - if a child is bigger than mainDim along MainDir we cap it to mainDim
///   - returns the space we need to lay all the children along elseDir in the Strat result
/// </summary>
public sealed class WrapStrat : IStrat
{
	public Dir MainDir { get; }

	[JsonIgnore]
	public Dir ElseDir => MainDir.Neg();

	public WrapStrat(Dir mainDir)
	{
		MainDir = mainDir;
	}

	public override string ToString() => $"Wrap({MainDir.Fmt()})";

	public LayNfo Lay(
		Node node,
		FreeSz freeSz,
		FDimVec[] kidDims
	)
	{
		/* Wrap node assumptions (strictly stronger than common assumptions)
		 * =====================
		 *   - dad .Main != FIT
		 *   - dad .Else  = FIT
		 *   - kids.Main != FIL
		 *   - kids.Else != FIL
		 */
		if (!freeSz.Dir(MainDir).HasValue) throw new ArgumentException("This should be impossible (I forgot why right now)");

		var dim = node.V.Dim;
		//if (freeSz.IsInfinite(MainDir) || !freeSz.IsInfinite(ElseDir) || !dim.Dir(MainDir).HasValue || dim.Dir(ElseDir).HasValue) throw new ArgumentException("Wrap node assumptions broken");

		// Possible because none of the kids are FILs
		var kidSizes = kidDims.Map(kidDim => new Sz(kidDim.X.Max, kidDim.Y.Max));

		var mainDim = dim.Dir(MainDir) ?? throw new ArgumentException("A Wrap node cannot be Fit along its main direction");
		var mainLng = ResolveDim(mainDim, freeSz.Dir(MainDir)!.Value);
		var rows = WrapKids(kidSizes, MainDir, mainLng);
		var elseLng = rows.Sum(row => row.Max(kid => kid.Dir(ElseDir)));

		var rowHeights = rows.Map(row => row.Max(kid => kid.Dir(ElseDir)));
		var rowYs = rowHeights.ScanL((a, b) => a + b, 0);
		var kidWidthsByRow = rows.Map(row => row.Map(kid => kid.Dir(MainDir)));
		var kidXsByRow = kidWidthsByRow.Map(kidWidths => kidWidths.ScanL((a, b) => a + b, 0));

		var rowYRowHeights = rowYs.Zip(rowHeights, (y, rowHeight) => (y, rowHeight));

		return new LayNfo(
			GeomMaker.MkSzDir(MainDir, mainLng, elseLng),
			rowYRowHeights.Zip(kidXsByRow, (yRowHeight, xs) => (yRowHeight, xs))
				.SelectMany(t => t.xs.Select(x => new
				{
					x,
					t.yRowHeight.y,
					t.yRowHeight.rowHeight
				}))
				.Zip(kidSizes, (nfo, sz) => (nfo, sz))
				.Map(t => new R(
					GeomMaker.MkPtDir(MainDir, t.nfo.x, t.nfo.y),
					t.sz
				))
		);
	}
}


file static class WrapUtils
{
	public static int ResolveDim(FDim dim, int free) => free.Cap(dim.Min, dim.Max);

	public static Sz[][] WrapKids(Sz[] kids, Dir mainDir, int mainLng)
	{
		var res = new List<Sz[]>();
		var cur = new List<Sz>();
		if (kids.Length == 0) return res.ToArray();
		if (kids.Length == 1)
		{
			res.Add(kids);
			return res.ToArray();
		}

		void AddCur()
		{
			if (cur.Count > 0)
			{
				res.Add(cur.ToArray());
				cur.Clear();
			}
		}

		var x = 0;
		foreach (var kid in kids)
		{
			var kidLng = kid.Dir(mainDir);
			var isNewLine = x == 0;
			var isTooBig = x + kidLng > mainLng;
			if (isTooBig && !isNewLine)
			{
				x = 0;
				AddCur();
			}

			cur.Add(kid);
			x += kidLng;
		}

		AddCur();
		return res.ToArray();
	}
}