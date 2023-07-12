using ControlSystem.Logic.Scrolling_.Structs.Enum;
using PowBasics.Geom;

namespace ControlSystem.Logic.Scrolling_.Structs;

sealed record ScrollNfo(
    (ScrollBarState X, ScrollBarState Y) State,
    Sz View,
    Sz Cont,
    R ViewR
)
{
    public static readonly ScrollNfo Empty = new(
        (ScrollBarState.Disabled, ScrollBarState.Disabled),
        Sz.Empty,
        Sz.Empty,
        R.Empty
    );
}