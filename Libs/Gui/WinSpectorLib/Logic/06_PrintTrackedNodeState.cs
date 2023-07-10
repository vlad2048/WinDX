using System.Reactive.Linq;
using ControlSystem.Logic.Scrolling_.Structs;
using ControlSystem.Structs;
using PowBasics.ColorCode;
using PowBasics.ColorCode.Utils;
using PowMaybe;
using PowRxVar;

namespace WinSpectorLib.Logic;


static partial class Setup
{
	public static IDisposable PrintTrackedNodeState(
		WinSpectorWin ui,
		IRwMayVar<NodeState> trackedState
	)
	{
		var d = new Disp();

		trackedState.Select(e => e.IsSome()).Subscribe(e => ui.stopPrintingTrackedNodeStateMenuItem.Enabled = e).D(d);
		ui.stopPrintingTrackedNodeStateMenuItem.Events().Click.Subscribe(_ => trackedState.V = May.None<NodeState>()).D(d);

		trackedState
			.Select(mayState => mayState.IsSome(out var state) switch
			{
				true => state.WhenChanged.Select(_ => state),
				false => Obs.Never<NodeState>()
			})
			.Switch()
			.Subscribe(state =>
			{
				var writer = new TxtWriter();
				writer.PrintNodeState(state);
				writer.Txt.PrintToConsole();
			}).D(d);

		trackedState
			.Select(mayState => mayState.IsSome(out var state) switch
			{
				true => state.ScrollState.WhenCmd,
				false => Obs.Never<IScrollCmd>()
			})
			.Switch()
			.Subscribe(cmd =>
			{
				var writer = new TxtWriter();
				writer.PrintScrollCmd(cmd);
				writer.Txt.PrintToConsole();
			}).D(d);

		return d;
	}
}




file static class WriterExt
{
	private const int PadHeader = 12;
	private const int PadValues = 12;

    public static void PrintNodeState(this ITxtWriter w, NodeState s)
    {
        var x = s.ScrollState.X;
        var y = s.ScrollState.Y;
        var (isX, isY) = (x.Enabled, y.Enabled);


        w.WriteLine();

        /*w.Write("R: ", C.RKey);
        w.WriteLine($"{s.R.V}", C.RVal);
        var lng = $"R: {s.R.V}".Length;
		w.WriteLine(new string('-', lng), C.RKey);

        P("", "ScrollX", "ScrollY", C.Title);*/

		w.Write($"{s.R.V}".PadRight(PadHeader), C.RVal);
		w.Write("ScrollX".PadLeft(PadValues), C.Title);
		w.Write("ScrollY".PadLeft(PadValues), C.Title);
		w.WriteLine();

        P("enabled", x.Enabled, y.Enabled, C.Key);
        POpt("visible", x.Visible, y.Visible, C.Key);
        POpt("View", x.ViewLng, y.ViewLng, C.Val);
        POpt("Cont", x.ContLng, y.ContLng, C.Val);
		POpt("ScrollOfs", x.ScrollOfs, y.ScrollOfs, C.Val);
        POpt("IsTailing", x.IsTailing, y.IsTailing, C.Val);

        w.WriteLine(new string('-', PadHeader + 2 * PadValues), C.Sep);

        POpt("MaxScrollOfs", x.MaxScrollOfs, y.MaxScrollOfs, C.ValMinor);
        POpt("Trak", x.TrakLng, y.TrakLng, C.Val);
        POpt("PageSize", x.PageSize, y.PageSize, C.ValMinor);


        Color Adj(string str, Color origCol) => str switch
        {
            "True" => C.True,
            "False" => C.False,
            _ => origCol
        };
        void P(string header, object colXObj, object colYObj, Color valCol)
        {
            var (colX, colY) = ($"{colXObj}", $"{colYObj}");
            w.Write(header.PadRight(PadHeader), C.Key);
            w.Write(colX.PadLeft(PadValues), Adj(colX, valCol));
            w.Write(colY.PadLeft(PadValues), Adj(colY, valCol));
            w.WriteLine();
        }
        void POpt(string header, object colXObj, object colYObj, Color valCol)
        {
            var (colX, colY) = ($"{colXObj}", $"{colYObj}");
            w.Write(header.PadRight(PadHeader), C.Key);
            w.Write((isX ? colX : "").PadLeft(PadValues), Adj(colX, valCol));
            w.Write((isY ? colY : "").PadLeft(PadValues), Adj(colY, valCol));
            w.WriteLine();
        }
    }


    public static void PrintScrollCmd(this ITxtWriter w, IScrollCmd cmd)
    {
		w.WriteLine($"[{cmd}]", C.Cmd);
    }




    private static class C
    {
        public static readonly Color RKey = Color.FromArgb(184, 156, 55);
        public static readonly Color RVal = Color.FromArgb(224, 232, 132);

        public static readonly Color Title = Color.FromArgb(255, 130, 220);

        public static readonly Color Key = Color.FromArgb(110, 110, 110);
        public static readonly Color Val = Color.FromArgb(42, 126, 201);
        public static readonly Color ValMinor = Color.FromArgb(17, 74, 125);
        public static readonly Color False = Color.FromArgb(224, 103, 90);
        public static readonly Color True = Color.FromArgb(95, 227, 152);

        public static readonly Color Sep = Color.FromArgb(30, 21, 112);



        public static readonly Color Cmd = Color.FromArgb(212, 75, 214);
    }
}
