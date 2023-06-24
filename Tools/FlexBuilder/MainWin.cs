using System.ComponentModel;
using FlexBuilder.Logic;
using FlexBuilder.Structs;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex;
using LayoutSystem.StructsShared;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using PowWinForms.Utils;

namespace FlexBuilder;

sealed partial class MainWin : Form
{
	public MainWin(Maybe<StartupFile> startupFile)
	{
		InitializeComponent();
		if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

		this.InitRX(d => {
			var ui = this;
			var userPrefs = new UserPrefs().Track();

			var layoutDef = VarMayNoCheck.MakeBnd<LayoutDef>().D(d);
			var layout = layoutDef.Map2(ComputeLayout);

			var selNode = VarMay.Make<Node>().D(d);
			var hovNode = VarMay.Make<Node>().D(d);

			layoutDef.WhenOuter.Subscribe(_ => {
				selNode.V = hovNode.V = May.None<Node>();
			}).D(d);


			var tabD = new SerialDisp<Disp>().D(d);
			Setup.GetSelectedTab(ui, userPrefs, out var selTab).D(d);

			Setup.InitConsole(userPrefs).D(d);
			Setup.LoadSaveLayoutDef(ui, layoutDef, userPrefs, startupFile).D(d);
			Setup.DisplayLayout(ui, layoutDef, layout, userPrefs, selNode, hovNode, WinSzMutator(layoutDef)).D(d);

			Setup.EditTreeInit(ui, layout);
			Setup.DetailsTreeInit(ui, layout);


			selTab.Subscribe(tab => {
				tabD.Value = null;
				tabD.Value = new Disp();
				switch (tab) {
					case TabName.Edit:
						Setup.EditTreeHook(ui, layoutDef, layout, selNode, hovNode).D(tabD.Value);
						break;

					case TabName.Details:
						Setup.DetailsTreeHook(ui, layout, selNode, hovNode).D(tabD.Value);
						break;
				}
			}).D(d);

		});
	}

	private static FlexLayout ComputeLayout(LayoutDef def) => FlexSolver.Solve(def.Root, def.WinSize);

	private static Action<FreeSz> WinSzMutator(IRwMayVar<LayoutDef> layoutDef) => freeSz => {
		if (layoutDef.V.IsNone(out var def)) return;
		layoutDef.V = May.Some(def with { WinSize = freeSz });
	};
}
