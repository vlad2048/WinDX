using PowBasics.Geom;
using WinAPI.User32;

namespace ControlSystem.Structs;

public sealed class WinOpt
{
    public string Title { get; set; } = string.Empty;
    public Pt? Pos { get; set; }
    public Sz? Size { get; set; }
    public WindowStyles Styles { get; set; } = WindowStyles.WS_VISIBLE;
    public WindowExStyles ExStyles { get; set; }

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