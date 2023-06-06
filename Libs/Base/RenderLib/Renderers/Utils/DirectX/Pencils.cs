using PowBasics.CollectionsExt;
using PowRxVar;
using RenderLib.Renderers.Utils.DirectX.Exts;
using RenderLib.Structs;
using Brush = Vortice.Direct2D1.ID2D1Brush;

namespace RenderLib.Renderers.Utils.DirectX;

public record PenNfo(
    Brush Brush,
    float Width,
    D2D.ID2D1StrokeStyle1 Style
) : IDisposable
{
    public void Dispose()
    {
        Brush.Dispose();
        Style.Dispose();
    }
}

public class Pencils : IDisposable
{
    private readonly Disp d = new();
    public void Dispose() => d.Dispose();

    private readonly Dictionary<BrushDef, Brush> brushes;
    private readonly Dictionary<PenDef, PenNfo> pens;
    private D2D.ID2D1Factory1 F { get; }
    private D2D.ID2D1RenderTarget T { get; }

    public Pencils(D2D.ID2D1Factory1 D2DFactory, D2D.ID2D1RenderTarget D2DDeviceCtx)
    {
        F = D2DFactory;
        T = D2DDeviceCtx;
        brushes = new Dictionary<BrushDef, Brush>().D(d);
        pens = new Dictionary<PenDef, PenNfo>().D(d);
    }

    public Brush GetBrush(BrushDef def) => brushes.GetOrCreate(def, () => def switch
    {
        SolidBrushDef b => T.CreateSolidColorBrush(
            b.Color.ToColor(),
            new D2D.BrushProperties(1.0f)
        ),
        //BmpBrushDef b => new TextureBrush(b.Bmp),
        _ => throw new ArgumentException()
    });

    public PenNfo GetPen(PenDef def) => pens.GetOrCreate(def, () => new PenNfo(
        GetBrush(new SolidBrushDef(def.Color)),
        def.Width,
        F.CreateStrokeStyle(new D2D.StrokeStyleProperties1
        {
            DashStyle = (D2D.DashStyle)def.DashStyle,
            TransformType = D2D.StrokeTransformType.Hairline,
        })
    ));
}