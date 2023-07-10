using System.Reactive.Linq;
using PowMaybe;
using PowRxVar;
using PowWinForms.ListBoxSourceListViewing;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using PowTrees.Algorithms;
using UserEvents;
using WinFormsTooling.Utils.Exts;
using ControlSystem.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ListWindowsAndGetVirtualTree(WinSpectorWin ui, out IRoMayVar<MixLayout> selLayout, IRoVar<bool> showSysCtrls)
	{
		var d = new Disp();
		ListBoxSourceListViewer.View(out var selWin, WinMan.MainWins.Items, ui.winList).D(d);

		var parts = selWin.SwitchMayVar(e => e.PartitionSetVar);

		selLayout = VarMay.Make(
			Obs.Merge(
				parts.ToUnit(),
				showSysCtrls.ToUnit()
			)
				.Select(_ => parts.V.IsSome(out var partsSet) switch
				{
					true => May.Some(BuildVirtualTree(partsSet, showSysCtrls)),
					false => May.None<MixLayout>()
				})
		).D(d);

		ui.windowRedrawItem.EnableWhenSome(selWin).D(d);
		ui.windowLogRedrawItem.EnableWhenSome(selWin).D(d);
		ui.windowLogNextRedrawItem.EnableWhenSome(selWin).D(d);
		ui.windowLogNext2RedrawsItem.EnableWhenSome(selWin).D(d);

		ui.windowRedrawItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw);
		}).D(d);

		ui.windowLogRedrawItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.SpectorDrawState.SetRenderCountToLog(1);
			win.Invalidator.Invalidate(RedrawReason.SpectorRequestFullRedraw);
		}).D(d);

		ui.windowLogNextRedrawItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.SpectorDrawState.SetRenderCountToLog(1);
		}).D(d);

		ui.windowLogNext2RedrawsItem.Events().Click.Subscribe(_ =>
		{
			var win = selWin.V.Ensure();
			win.SpectorDrawState.SetRenderCountToLog(2);
		}).D(d);

		return d;
	}


	private static MixLayout BuildVirtualTree(PartitionSet parts, IRoVar<bool> showSysCtrls)
	{
		var lay = parts.MixLayout;
		var root = lay.MixRoot.Clone();
		if (!showSysCtrls.V) return lay;

		var rootMap = root.Make_NodeState_2_MixNode_Map();
		var extras = parts.Partitions.Select(e => e.SysPartition.Forest).Merge();
		foreach (var (nodeState, nodes) in extras)
		{
			var nodeDad = rootMap[nodeState];
			nodeDad.AddChildren(nodes);
		}

		return lay with
		{
			MixRoot = root,
			NodeMap = root.Make_NodeState_2_MixNode_Map(),
			RMap = parts.Partitions.SelectMany(e => new[] { e.RMap, e.SysPartition.RMap }).Merge(),
			Ctrl2NodMap = root.Make_Ctrl_2_MixNode_Map()
		};
	}
}