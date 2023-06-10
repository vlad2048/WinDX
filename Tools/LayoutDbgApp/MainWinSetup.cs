using LayoutDbgApp.SetupLogic;
using LayoutDbgApp.Structs;
using LayoutDbgApp.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
using PowWinForms;

namespace LayoutDbgApp;

static class MainWinSetup
{
	private const bool DISABLE_TREE_REFRESH = false;

	public static IDisposable SetupMainWin(MainWin ui)
	{
		var d = new Disp();

		var layoutDef = Var.MakeBndNoCheck(May.None<LayoutDef>()).D(d);
		var layout = layoutDef.Map2(ComputeLayout);

		ui.redrawBtn.Events().Click.Subscribe(_ => layoutDef.V = layoutDef.V).D(d);

		var userPrefs = new UserPrefs().Track();

		IRoVar<Maybe<Node>> selNode;
		IRoVar<Maybe<Node>> hoveredNode;
		if (!DISABLE_TREE_REFRESH)
		{
			Setup.EditLayoutDefTree(ui, layoutDef, layout, out selNode, out hoveredNode).D(d);
		}
		else
			// ReSharper disable once HeuristicUnreachableCode
		{
			selNode = Var.Make(May.None<Node>()).D(d);
			hoveredNode = Var.Make(May.None<Node>()).D(d);
		}

		Setup.EditLayoutDefWinSize(ui, layoutDef).D(d);
		Setup.DisplayCalcWinSize(ui, layout);
		Setup.LoadSaveLayoutDef(ui, layoutDef, userPrefs).D(d);
		Setup.DisplayLayout(ui, layout, userPrefs, selNode, hoveredNode, WinSzMutator(layoutDef)).D(d);

		return d;
	}

	private static Layout ComputeLayout(LayoutDef def) => FlexSolver.Solve(def.Root, def.WinSize);

	private static Action<FreeSz> WinSzMutator(IRwVar<Maybe<LayoutDef>> layoutDef) => freeSz =>
	{
		if (layoutDef.V.IsNone(out var def)) return;
		layoutDef.V = May.Some(def with { WinSize = freeSz });
	};
}