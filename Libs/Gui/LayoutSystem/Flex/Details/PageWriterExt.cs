using System.Drawing;
using LayoutSystem.Flex.Details.Structs;
using LayoutSystem.Flex.LayStratsInternal;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace LayoutSystem.Flex.Details;
using W = PageWriter;

static class C
{
	private const float SzNormal = 10;
	private const float SzBig = 12;

	public static readonly TxtStyle Default = TxtStyle.Default;


	public static readonly TxtStyle Soft = new(Color.FromArgb(103, 103, 103), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle PosVal = new(Color.FromArgb(192, 196, 194), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle LayKidsTitle = new(Color.FromArgb(103, 194, 227), SzNormal, TxtFontStyle.Bold | TxtFontStyle.Underline);

	public static readonly TxtStyle Key = new(Color.FromArgb(103, 194, 227), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle Free = new(Color.FromArgb(228, 241, 249), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle Node = new(Color.FromArgb(225, 77, 195), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle NodeStratQuestion = new(Color.FromArgb(218, 246, 67), SzNormal, TxtFontStyle.Regular);

	public static readonly TxtStyle DimResolved = new(Color.FromArgb(99, 242, 126), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle DimUnresolved = new(Color.FromArgb(243, 77, 100), SzNormal, TxtFontStyle.Regular);

	public static readonly TxtStyle Crossed = new(Color.FromArgb(243, 77, 100), SzNormal, TxtFontStyle.Strikeout);

	public static readonly TxtStyle FinalResultTitle = new(Color.FromArgb(255, 255, 255), SzBig, TxtFontStyle.Bold | TxtFontStyle.Underline);
	public static readonly TxtStyle FinalResult = new(Color.FromArgb(255, 255, 255), SzBig, TxtFontStyle.Bold);
	public static readonly TxtStyle FinalLayNfoResolvedSz = new(Color.FromArgb(240, 43, 197), SzBig, TxtFontStyle.Bold);
	public static readonly TxtStyle SoftBig = new(Color.FromArgb(103, 103, 103), SzBig, TxtFontStyle.Bold);
	public static readonly TxtStyle PosValBig = new(Color.FromArgb(192, 196, 194), SzBig, TxtFontStyle.Bold);
	public static readonly TxtStyle FreeBig = new(Color.FromArgb(228, 241, 249), SzBig, TxtFontStyle.Bold);

	public static readonly TxtStyle KeyBig = new(Color.FromArgb(103, 194, 227), SzBig, TxtFontStyle.Regular);

	public static readonly TxtStyle IsPop = new(Color.FromArgb(237, 143, 76), SzNormal, TxtFontStyle.Regular);
	public static readonly TxtStyle IsPopBig = new(Color.FromArgb(237, 143, 76), SzBig, TxtFontStyle.Bold);
}


static class PageWriterExt
{
	public static void LayNodeInputPos(this W w, Pt pos)
	{
		w.Write("pos:", C.Soft);
		w.WriteLine(pos.f(), C.PosVal);
	}

	public static void LayKidsInputs(this W w, Node node, FreeSz scrFreeSz, FreeSz freeSz)
	{
		w.WriteLine();
		w.WriteLine("LayKids:", C.LayKidsTitle);

		w.Write("node: ", C.Key);
		w.WriteLine(node.f(), C.Node);

		w.Write("free: ", C.Key);
		w.Write(scrFreeSz.f(), C.Free);
		if (scrFreeSz != freeSz)
			w.Write($"  {freeSz.f()}", C.Crossed);
		w.WriteLine();
		w.WriteSeparator();
	}

	public static void KidIsPop(this W w, int kidIdx)
	{
		w.Write($"kid[{kidIdx}]:    ", C.Key);
		w.WriteLine("IsPop", C.IsPop);
	}

	public static void KidResolvedOrNot(this W w, Node kid, int kidIdx)
	{
		w.Write($"kid[{kidIdx}]:    ", C.Key);
		w.Write(kid.V.Strat.f(kid.V.Marg) + " ", C.NodeStratQuestion);
		void Prn(Dim d, Dir dir) => w.Write(d.f(dir), d.IsFit() ? C.DimUnresolved : C.DimResolved);
		Prn(kid.V.Dim.X, Dir.Horz);
		w.Write("ₓ", C.Soft);
		Prn(kid.V.Dim.Y, Dir.Vert);
	}

	public static void KidFreeSz(this W w, FreeSz kidFreeSz) => w.WriteLine($"    {kidFreeSz.f()}", C.Free);


	public static void ResolveKidOutput(this W w, Node kid, int kidIdx, FDimVec kidResolvedDim)
	{
		w.WriteLine();
		w.Write($"kid[{kidIdx}]:    ", C.Key);
		w.Write(kid.V.Strat.f(kid.V.Marg) + " ", C.NodeStratQuestion);
		void Prn(Dim d, Dir dir) => w.Write(d.f(dir), d.IsFit() ? C.DimUnresolved : C.DimResolved);
		Prn(kidResolvedDim.X, Dir.Horz);
		w.Write("ₓ", C.Soft);
		Prn(kidResolvedDim.Y, Dir.Vert);
		w.WriteLine();
	}

	public static void LayKidsOutput(this W w, LayNfo layNfoCapped, LayNfo layNfo, FreeSz scrFreeSz, bool topLevel)
	{
		w.WriteSeparator();
		switch (topLevel)
		{
			case truer:
				if (!layNfoCapped.IsSame(layNfo))
					w.WriteLine(layNfo.f(), C.Crossed);
				w.Write(layNfoCapped.ResolvedSz.f(), C.FinalLayNfoResolvedSz);
				w.WriteLine($" - [{layNfoCapped.Kids.Select(e => e.f()).JoinText("; ")}]", C.NodeStratQuestion);
				break;

			case false:
				var sz = layNfo.ResolvedSz;
				var szCapped = layNfoCapped.ResolvedSz;
				if (szCapped != sz)
					w.WriteLine(sz.f(), C.Crossed);
				w.WriteLine(szCapped.f(), C.NodeStratQuestion);
				break;
		}
	}


	public static void FinalR(this W w, Pt pos, Sz layNfoResolvedSz, FreeSz freeSz)
	{
		var cappedSz = layNfoResolvedSz.CapWith(freeSz);
		if (cappedSz != layNfoResolvedSz)
		{
			w.Write("pos:", C.SoftBig);
			w.Write(pos.f(), C.PosValBig);
			w.Write(" - ", C.SoftBig);
			w.Write(layNfoResolvedSz.f(), C.FinalLayNfoResolvedSz);

			w.Write("  capped with ", C.SoftBig);
			w.WriteLine(freeSz.f(), C.FreeBig);
		}

		w.WriteLine();
		w.WriteLine("Result:", C.FinalResultTitle);
		w.WriteLine();

		w.Write("  R = ", C.FinalResult);
		w.Write("pos:", C.SoftBig);
		w.Write(pos.f(), C.PosValBig);
		w.Write(" - ", C.SoftBig);
		w.WriteLine(cappedSz.f(), C.FinalLayNfoResolvedSz);
		w.WriteLine();
	}

	public static void FinalKidRecurse(this W w, int kidIdx, Pt kidPos, FreeSz kidFreeSz, bool isPop)
	{
		w.Write($"  kid[{kidIdx}]: ", C.KeyBig);
		w.Write("pos:", C.SoftBig);
		w.Write(kidPos.f(), C.PosValBig);
		w.Write($" {kidFreeSz.f()}", C.FreeBig);
		if (isPop)
			w.Write("   IsPop", C.IsPopBig);
		w.WriteLine();
	}



	public static void WriteSeparator(this W w) => w.WriteLine(new string('-', 40), C.Soft);

	public static string f(this Dim v, Dir dir) => (v.Typ() switch
	{
		DimType.Fix => $"Fix({v!.Value.Min})",
		DimType.Flt => $"Flt({v!.Value.Min}-{v.Value.Max})",
		DimType.Fil => "Fil",
		DimType.Fit => "Fit",
	}).p(8, dir);
	public static string f(this DimVec v) => $"{v.X.f(Dir.Horz)}ₓ{v.Y.f(Dir.Vert)}";

	public static string f(this FDim v, Dir dir) => ((Dim)v).f(dir);
	public static string f(this FDimVec v) => $"{v.X.f(Dir.Horz)}ₓ{v.Y.f(Dir.Vert)}";

	public static string f(this Sz v) => $"{v.Width.p(3)}ₓ{v.Height.p(3)}";
	public static string f(this Pt v) => $"{v.X},{v.Y}";
	public static string f(this R v) => $"({v.Pos.f()} {v.Size.f()})";
	public static string f(this FreeSz v)
	{
		static string o(int? v) => v.HasValue switch
		{
			true => $"{v.Value}",
			false => "_"
		};
		return $"fr({o(v.X)}ₓ{o(v.Y)})";
	}
	public static string f(this LayNfo v) => $"{v.ResolvedSz.f()} - [{v.Kids.Select(e => e.f()).JoinText("; ")}]";

	public static string f(this IStrat v, Marg mg)
	{
		static string fm(Marg m) => (m.Left == m.Right && m.Left == m.Top && m.Left == m.Bottom) switch
		{
			truer => $"{m.Left}",
			false => $"↑{m.Top},→{m.Right},↓{m.Bottom},←{m.Left}"
		};
		var mgStr = (v is MarginStrat && mg != Mg.Zero) switch
		{
			truer => $"({fm(mg)})",
			false => string.Empty,
		};
		return $"{v}{mgStr}".pr(12);
	}

	public static string f(this FlexNode v) => $"{v.Strat.f(v.Marg)}  {v.Dim.f()}";
	public static string f(this Node v) => v.V.f();

	private static string p<T>(this T v, int p) => $"{v}".PadLeft(p);
	private static string pr<T>(this T v, int p) => $"{v}".PadRight(p);

	private static string p<T>(this T v, int p, Dir dir) => dir switch
	{
		Dir.Horz => $"{v}".PadLeft(p),
		Dir.Vert => $"{v}".PadRight(p)
	};


	public static void WriteLine(this W w) => w.WriteLine(string.Empty, TxtStyle.Default);
}