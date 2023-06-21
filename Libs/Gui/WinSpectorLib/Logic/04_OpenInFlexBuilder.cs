using System.Diagnostics;
using ControlSystem.Structs;
using ControlSystem.Utils;
using LayoutSystem.StructsShared;
using LayoutSystem.Utils.JsonUtils;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
using Structs;
using WinFormsTooling.Utils.Exts;
using Environment = System.Environment;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable OpenInFlexBuilder(WinSpectorWin ui, IRoMayVar<MixLayout> selLayout)
	{
		var d = new Disp();

		ui.openFlexBuilderToolBtn.EnableWhenSome(selLayout).D(d);

		// TODO: understand why ToolStrip and StatusStrip buttons Click events take 2 clicks to fire
		// (use MouseDown maybe)
		ui.openFlexBuilderToolBtn.Events().Click.Subscribe(_ =>
		{
			var layout = selLayout.V.Ensure();
			var layoutDef = new LayoutDef(
				layout.WinSize,
				layout.MixRoot
					.OfTypeTree<IMixNode, StFlexNode>()
					.Map(e => e.Flex)
			);
			var tempFile = Path.GetTempFileName();
			Jsoner.Save(tempFile, layoutDef);

			var flexBuilderExe = GetFlexBuilderExe();

			var procNfo = new ProcessStartInfo(flexBuilderExe, $@"""{tempFile}"" delete");
			Process.Start(procNfo);
		}).D(d);

		return d;
	}

	private static string GetFlexBuilderExe()
	{
		var folder = Environment.CurrentDirectory;
		while (folder != null && Directory.GetFiles(folder, "WinDX.sln").Length == 0)
			folder = Path.GetDirectoryName(folder);
		if (folder == null) throw new ArgumentException("Cannot find FlexBuilder.exe folder");
		var exe = Path.Combine(folder, "Tools", "FlexBuilder", "bin", "Debug", "net7.0-windows", "FlexBuilder.exe");
		if (!File.Exists(exe)) throw new ArgumentException("Cannot find FlexBuilder.exe");
		return exe;
	}
}