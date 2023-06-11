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
public record MouseButtonDownUserEvt(Pt Pos, MouseBtn Btn)		: IUserEvtMousePos			{ public override string ToString() => $"{Btn} down ({Pos})";							}
public record   MouseButtonUpUserEvt(Pt Pos, MouseBtn Btn)		: IUserEvtMousePos			{ public override string ToString() => $"{Btn} up ({Pos})";								}
public record       MouseMoveUserEvt(Pt Pos)					: IUserEvtMousePos			{ public override string ToString() => $"Move {Pos}";									}
public record      MouseEnterUserEvt(Pt Pos)					: IUserEvtMousePos			{ public override string ToString() => $"Enter {Pos}";									}
public record      MouseLeaveUserEvt							: IUserEvtMouse				{ public override string ToString() => "Leave";											}

public record         KeyDownUserEvt(VirtualKey Key)			: IUserEvtKeyboard			{ public override string ToString() => $"'{Key}' down";									}
public record           KeyUpUserEvt(VirtualKey Key)			: IUserEvtKeyboard			{ public override string ToString() => $"'{Key}' up";									}
public record         KeyCharUserEvt(char Char)					: IUserEvtKeyboard			{ public override string ToString() => $"'{Char}' char";								}

public record        GotFocusUserEvt							: IUserEvtWindow			{ public override string ToString() => "Got focus";										}
public record       LostFocusUserEvt							: IUserEvtWindow			{ public override string ToString() => "Lost focus";									}
public record        ActivateUserEvt(bool WithMouseClick)		: IUserEvtWindow			{ public override string ToString() => "Activate" + (WithMouseClick ? " (mouse)" : "");	}
public record      InactivateUserEvt							: IUserEvtWindow			{ public override string ToString() => "Inactivate";									}
public record     ActivateAppUserEvt							: IUserEvtWindow			{ public override string ToString() => "ActivateApp";									}
public record   InactivateAppUserEvt							: IUserEvtWindow			{ public override string ToString() => "InactivateApp";									}
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