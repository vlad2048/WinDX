using FlexBuilder.Structs;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using WinAPI.Kernel32;

namespace FlexBuilder;

static class Program
{
	[STAThread]
	static void Main(string[] args)
	{
		//VarDbg.BreakpointOnDispAlloc(1);

		// TODO: understand why it's needed
		Kernel32Methods.AllocConsole();

		ApplicationConfiguration.Initialize();

		var startupFile = ReadStartupFile(args);

		using (var win = new MainWin(startupFile).Track())
			Application.Run(win);

		VarDbg.CheckForUndisposedDisps(true);
	}

	private static Maybe<StartupFile> ReadStartupFile(string[] args) => args.Length switch
	{
		1 when File.Exists(args[0])
			=> May.Some(new StartupFile(args[0], false)),

		2 when File.Exists(args[0]) && string.Compare(args[1], "delete", StringComparison.InvariantCultureIgnoreCase) == 0
			=> May.Some(new StartupFile(args[0], true)),

		_
			=> May.None<StartupFile>()
	};
}