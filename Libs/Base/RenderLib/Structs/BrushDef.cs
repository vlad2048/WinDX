namespace RenderLib.Structs;

public interface BrushDef { }
public record SolidBrushDef(Color Color) : BrushDef;
public record BmpBrushDef(Bitmap Bmp) : BrushDef;