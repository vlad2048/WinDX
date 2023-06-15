using FlexBuilder.SetupLogic;
using FlexBuilder.Structs;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowMaybe;
using PowRxVar;
using PowWinForms;

#pragma warning disable CS0162

namespace FlexBuilder;

static class MainWinSetup
{
	private const bool DISABLE_TREE_REFRESH = false;

	public static IDisposable SetupMainWin(MainWin ui)
	{
		var d = new Disp();

		var userPrefs = new UserPrefs().Track();

		var layoutDef = VarMayNoCheck.MakeBnd<LayoutDef>().D(d);
		var layout = layoutDef.Map2(ComputeLayout);
		ui.redrawBtn.Events().Click.Subscribe(_ => layoutDef.V = layoutDef.V).D(d);

		Setup.InitConsole(userPrefs).D(d);
		Setup.EditLayoutDefTree(ui, layoutDef, layout, out var selNode, out var hoveredNode, DISABLE_TREE_REFRESH).D(d);
		Setup.EditLayoutDefWinSize(ui, layoutDef).D(d);
		Setup.DisplayCalcWinSize(ui, layout);
		Setup.LoadSaveLayoutDef(ui, layoutDef, userPrefs).D(d);
		Setup.DisplayLayout(ui, layout, userPrefs, selNode, hoveredNode, WinSzMutator(layoutDef)).D(d);

		return d;
	}

	private static Layout ComputeLayout(LayoutDef def) => FlexSolver.Solve(def.Root, def.WinSize);

	private static Action<FreeSz> WinSzMutator(IRwMayVar<LayoutDef> layoutDef) => freeSz =>
	{
		if (layoutDef.V.IsNone(out var def)) return;
		layoutDef.V = May.Some(def with { WinSize = freeSz });
	};
}
