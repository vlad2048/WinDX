using PowBasics.Geom;

namespace Structs;

public class WinOpt
{
    public string Title { get; set; } = string.Empty;
    public Pt? Pos { get; set; }
    public Sz? Size { get; set; }

    public R R
    {
        set
        {
            Pos = value.Pos;
            Size = value.Size;
        }
    }

    private WinOpt() { }

    public static WinOpt Build(Action<WinOpt>? optFun)
    {
        var opt = new WinOpt();
        optFun?.Invoke(opt);
        return opt;
    }
}