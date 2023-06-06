using PowRxVar;

namespace RenderLib.Renderers.Utils.DirectX;

static class BmpUtils
{
    public static (D2D.ID2D1Bitmap, IDisposable) Load(string filename, D2D.ID2D1DeviceContext d2dDeviceCtx, WIC.IWICImagingFactory wicFactory)
    {
        var d = new Disp();
        var decoder = wicFactory.CreateDecoderFromFileName(filename, null, FileAccess.Read, WIC.DecodeOptions.CacheOnLoad);
        var frame = decoder.GetFrame(0);
        var formatConverter = wicFactory.CreateFormatConverter();
        if (formatConverter.Initialize(frame, WIC.PixelFormat.Format32bppPBGRA, WIC.BitmapDitherType.None, null, 0.0, WIC.BitmapPaletteType.MedianCut).Failure)
            throw new InvalidOperationException("formatConverter.Initialize failed");
        var wicBmp = wicFactory.CreateBitmapFromSource(formatConverter, WIC.BitmapCreateCacheOption.CacheOnLoad);
        var d2dBmp = d2dDeviceCtx.CreateBitmapFromWicBitmap(wicBmp).D(d);
        return (d2dBmp, d);
    }
}