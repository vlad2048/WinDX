using System.Drawing.Drawing2D;
using PowBasics.CollectionsExt;
using PowRxVar;
using RenderLib.Structs;

namespace RenderLib.Renderers.GDIPlus.Utils;

class Pencils : IDisposable
{
	private readonly Disp d = new();
	public void Dispose() => d.Dispose();

	private readonly Dictionary<BrushDef, Brush> brushes;
	private readonly Dictionary<PenDef, Pen> pens;

	public Pencils()
	{
		brushes = new Dictionary<BrushDef, Brush>().D(d);
		pens = new Dictionary<PenDef, Pen>().D(d);
	}

	public Brush GetBrush(BrushDef def) => brushes.GetOrCreate(def, () => def switch
	{
		SolidBrushDef b => new SolidBrush(b.Color),
		BmpBrushDef b => new TextureBrush(b.Bmp),
		_ => throw new ArgumentException()
	});

	public Pen GetPen(PenDef def) => pens.GetOrCreate(def, () =>
		new Pen(def.Color, def.Width)
		{
			DashStyle = (DashStyle)def.DashStyle
		}
	);
}