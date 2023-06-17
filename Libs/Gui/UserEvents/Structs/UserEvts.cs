using PowBasics.Geom;
using WinAPI.User32;
// ReSharper disable WithExpressionModifiesAllMembers

namespace UserEvents.Structs;


public interface IUserEvt { }

public enum MouseBtn
{
	Left,
	Right,
	Middle
};
public interface IUserEvtMouse : IUserEvt { }
public interface IUserEvtMousePos : IUserEvtMouse
{
	Pt Pos { get; }
}

public interface IUserEvtKeyboard : IUserEvt { }

public interface IUserEvtWindow : IUserEvt { }


// @formatter:off
public sealed record MouseButtonDownUserEvt(Pt Pos, MouseBtn Btn)		: IUserEvtMousePos			{ public override string ToString() => $"{Btn} down ({Pos})";							}
public sealed record   MouseButtonUpUserEvt(Pt Pos, MouseBtn Btn)		: IUserEvtMousePos			{ public override string ToString() => $"{Btn} up ({Pos})";								}
public sealed record       MouseMoveUserEvt(Pt Pos)					: IUserEvtMousePos			{ public override string ToString() => $"Move {Pos}";									}
public sealed record      MouseEnterUserEvt(Pt Pos)					: IUserEvtMousePos			{ public override string ToString() => $"Enter {Pos}";									}
public sealed record      MouseLeaveUserEvt							: IUserEvtMouse				{ public override string ToString() => "Leave";											}

public sealed record         KeyDownUserEvt(VirtualKey Key)			: IUserEvtKeyboard			{ public override string ToString() => $"'{Key}' down";									}
public sealed record           KeyUpUserEvt(VirtualKey Key)			: IUserEvtKeyboard			{ public override string ToString() => $"'{Key}' up";									}
public sealed record         KeyCharUserEvt(char Char)					: IUserEvtKeyboard			{ public override string ToString() => $"'{Char}' char";								}

public sealed record        GotFocusUserEvt							: IUserEvtWindow			{ public override string ToString() => "Got focus";										}
public sealed record       LostFocusUserEvt							: IUserEvtWindow			{ public override string ToString() => "Lost focus";									}
public sealed record        ActivateUserEvt(bool WithMouseClick)		: IUserEvtWindow			{ public override string ToString() => "Activate" + (WithMouseClick ? " (mouse)" : "");	}
public sealed record      InactivateUserEvt							: IUserEvtWindow			{ public override string ToString() => "Inactivate";									}
public sealed record     ActivateAppUserEvt							: IUserEvtWindow			{ public override string ToString() => "ActivateApp";									}
public sealed record   InactivateAppUserEvt							: IUserEvtWindow			{ public override string ToString() => "InactivateApp";									}
// @formatter:on


public static class IUserEvtExts
{
// @formatter:off
	public static IUserEvt TranslateMouse(this IUserEvt evt, Pt ofs) => evt switch
	{
		MouseButtonDownUserEvt e => e with { Pos = e.Pos + ofs },
		  MouseButtonUpUserEvt e => e with { Pos = e.Pos + ofs },
		      MouseMoveUserEvt e => e with { Pos = e.Pos + ofs },
		     MouseEnterUserEvt e => e with { Pos = e.Pos + ofs },
		_ => evt
	};
// @formatter:on
}