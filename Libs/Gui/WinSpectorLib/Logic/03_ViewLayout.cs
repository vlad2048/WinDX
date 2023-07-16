using BrightIdeasSoftware;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowMaybe;
using PowRxVar;
using PowWinForms.Utils;
using WinSpectorLib.Controls;
using WinSpectorLib.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ViewLayout(
		WinSpectorWin ui,
		IRoMayVar<PartitionSet> selLayout,
		IRwVar<bool> showEvents,
		IRwMayVar<NodeState> trackedState
	)
	{
		var d = new Disp();

		var ctrl = ui.layoutTree;
		PrepareTree(ctrl, selLayout, ui.eventDisplayer);

		var rootMayVar = selLayout.SelectVarMay(e => e.Root);
		ctrl.SetRoot(rootMayVar).D(d);

		var selNode = VarMay.Make<MixNode>().D(d);
		var hovNode = VarMay.Make<MixNode>().D(d);
		ctrl.PipeSelectedNodeInto(selNode).D(d);
		ctrl.PipeHoveredNodeInto(hovNode).D(d);

		Obs.Merge(selNode, hovNode).Subscribe(_ =>
		{
			if (selLayout.V.IsNone(out var selLayoutVal)) return;
			var win = selLayoutVal.Nfo.MainWin;
			win.SpectorDrawState.NodeTreeSel.V = selNode.V;
			win.SpectorDrawState.NodeTreeHov.V = hovNode.V;
		}).D(d);

		SetupContextMenu(ui, selNode, showEvents, trackedState).D(d);

		return d;
	}



	// ***************
	// * Nfo Wrapper *
	// ***************
	/*private sealed record MixNodeWithNfo(
		IMixNode Node,
		int? PopIndex
	);
	private static TNod<MixNodeWithNfo> AddNfoToTree(this MixNode root)
	{
		var map = root.GetPopMapping();
		return root.Map(e => new MixNodeWithNfo(e, map[e]));
	}
	private static IReadOnlyDictionary<IMixNode, int?> GetPopMapping(this MixNode root)
	{
		var map = new Dictionary<IMixNode, int?>();
		var cnt = 0;
		void Rec(MixNode node, int? idx)
		{
			if (node.IsPop())
				idx = cnt++;
			map[node.V] = idx;
			foreach (var kid in node.Children)
				Rec(kid, idx);
		}
		Rec(root, null);
		return map;
	}
	private static bool IsPop(this MixNode node) => node.V is StFlexNode { Flex.Flags.Pop: true };*/



	// ****************
	// * Column Setup *
	// ****************
	private static class ColumnName
	{
		public const string Node = "Node";
		public const string Flags = "Flags";
		public const string Width = "Width";
		public const string Height = "Height";
		public const string R = "R";
		public const string Warnings = "Warnings";
		public const string Tracked = "Tracked";
	}

	private static void PrepareTree(TreeListView ctrl, IRoMayVar<PartitionSet> selLayout, EventDisplayer eventDisplayer)
	{
		var warningIcon = Resource.LayoutTree_Warning;
		var errorIcon = Resource.LayoutTree_Error;

		ctrl.SetupForNodeType<IMixNode>();

		/*Image? GetImage(TNod<IMixNode> nod) => nod.V.PopIndex.HasValue switch
		{
			true => Consts.GetPopIcon(nod.V.PopIndex.Value),
			false => null
		};

		ctrl.AddTextColumnWithImage<IMixNode>(ColumnName.Node, null, nod => nod.V.Node switch
		{
			CtrlNode {Ctrl: var c} => (
				c.GetType().Name,
				GetImage(nod)
			),
			StFlexNode {Flex: var flex} => (
				$"{flex}",
				GetImage(nod)
			),
			_ => (
				"_",
				null
			)
		});*/

		ctrl.AddTextColumn<IMixNode>(ColumnName.Node, null, nod => nod.V switch
		{
			CtrlNode { Ctrl: var c } => c.GetType().Name,
			StFlexNode { Flex: var flex } => $"{flex}",
			_ => "_"
		});

		ctrl.AddTextColumn<IMixNode>(ColumnName.Flags, 100, nod => nod.V switch
		{
			CtrlNode => "_",
			StFlexNode { Flex.Flags: var flags } => $"{flags}",
			_ => "_"
		});


		ctrl.AddTextColumnWithColorAndTooltip<IMixNode>(ColumnName.Width, 80, nod =>
		{
			if (nod.V is not StFlexNode { Flex: var flex, State: var nodState }) return ("_", null, null);
			var warn = selLayout.GetDimWarningForColumn(nodState, WarningDir.Horz);
			return warn switch
			{
				null => (
					flex.Dim.X.Fmt(),
					null,
					null
				),
				not null => (
					warn.FixedDim.X.Fmt(),
					ColorName.Red,
					new TooltipDef(
						"Original value",
						flex.Dim.X.Fmt(),
						null
					)
				)
			};
		});

		ctrl.AddTextColumnWithColorAndTooltip<IMixNode>(ColumnName.Height, 80, nod =>
		{
			if (nod.V is not StFlexNode { Flex: var flex, State: var nodState }) return ("_", null, null);
			var warn = selLayout.GetDimWarningForColumn(nodState, WarningDir.Vert);
			return warn switch
			{
				null => (
					flex.Dim.Y.Fmt(),
					null,
					null
				),
				not null => (
					warn.FixedDim.Y.Fmt(),
					ColorName.Red,
					new TooltipDef(
						"Original value",
						flex.Dim.Y.Fmt(),
						null
					)
				)
			};
		});

		ctrl.AddTextColumn<IMixNode>(ColumnName.R, 100, nod =>
		{
			if (nod.V is not StFlexNode { State: var nodState }) return "_";
			if (selLayout.V.IsNone(out var selLayoutVal)) return "_";
			var r = selLayoutVal.RMap[nodState];
			return $"{r}";
		});

		ctrl.AddImageColumnWithTooltip<IMixNode>(ColumnName.Warnings, 65, nod =>
		{
			if (selLayout.V.IsNone(out var selLayoutVal)) return (null, null);

			switch (nod.V)
			{
				case StFlexNode { State: var nodState }:
					var warn = selLayoutVal.GetDimWarningForColumn(nodState, WarningDir.Horz | WarningDir.Vert);
					if (warn == null) return (null, null);
					return (
						warningIcon,
						new TooltipDef(
							"Layout Warning" + (warn.Messages.Length > 1 ? "s" : ""),
							warn.Messages.JoinText(Environment.NewLine),
							ToolTipControl.StandardIcons.Warning
						)
					);


				case CtrlNode { Ctrl: var ctr }:
					return selLayoutVal.Nfo.UnbalancedCtrls.TryGetValue(ctr, out var errNode) switch
					{
						true => (
							errorIcon,
							new TooltipDef(
								"Unbalanced Ctrl",
								$"{errNode}",
								ToolTipControl.StandardIcons.ErrorLarge
							)
						),
						false => (null, null)
					};

				default:
					return (null, null);
			}

		});

		ctrl.AddTextColumn<IMixNode>(ColumnName.Tracked, 100, nod =>
		{
			if (nod.V is not StFlexNode st) return string.Empty;
			return eventDisplayer.GetTrackedNodeName(st);
		});
	}

	private static FlexWarning? GetDimWarningForColumn(this PartitionSet set, NodeState nodState, WarningDir colDir)
	{
		if (!set.Nfo.WarningMap.TryGetValue(nodState, out var warn)) return null;
		if ((colDir & warn.Dir) == 0) return null;
		return warn;
	}

	private static FlexWarning? GetDimWarningForColumn(this IRoMayVar<PartitionSet> selLayout, NodeState nodState, WarningDir colDir) =>
		selLayout.V.IsSome(out var selLayoutVal) switch
		{
			true => selLayoutVal.GetDimWarningForColumn(nodState, colDir),
			false => null
		};



	// ****************
	// * Context Menu *
	// ****************
	private static IDisposable SetupContextMenu(
		WinSpectorWin ui,
		IRoMayVar<MixNode> selNode,
		IRwVar<bool> showEvents,
		IRwMayVar<NodeState> trackedState
	)
	{
		var d = new Disp();

		ui.layoutTreeContextMenu.Events().Opening.Subscribe(e =>
		{
			var isTracked = selNode.CanTrackNode(out var node) && ui.eventDisplayer.IsNodeTracked(node!);

			ui.trackEventsMenuItem.Enabled = !isTracked && selNode.CanTrackNode();
			ui.stopTrackingMenuItem.Enabled = isTracked;
			ui.stopAllTrackingMenuItem.Enabled = ui.eventDisplayer.IsAnyNodeTracked();
			ui.printStateMenuItem.Enabled = selNode.IsFlexNode();

			var isAny = ui.trackEventsMenuItem.Enabled || ui.stopTrackingMenuItem.Enabled || ui.stopAllTrackingMenuItem.Enabled || ui.printStateMenuItem.Enabled;
			if (!isAny)
				e.Cancel = true;
		}).D(d);


		ui.trackEventsMenuItem.Events().Click.Subscribe(_ =>
		{
			if (!selNode.CanTrackNode(out var node)) return;

			ui.eventDisplayer.TrackNode(node!);
			showEvents.V = true;
		}).D(d);

		ui.stopTrackingMenuItem.Events().Click.Subscribe(_ =>
		{
			if (!selNode.CanTrackNode(out var node)) return;

			ui.eventDisplayer.StopTrackingNode(node!);
		}).D(d);

		ui.stopAllTrackingMenuItem.Events().Click.Subscribe(_ =>
		{
			ui.eventDisplayer.StopAllTracking();
		}).D(d);

		ui.printStateMenuItem.Events().Click.Subscribe(_ =>
		{
			if (!selNode.IsFlexNode()) return;

			var state = ((StFlexNode)selNode.V.Ensure().V).State;
			trackedState.V = May.Some(state);
		}).D(d);

		return d;
	}


	private static bool IsCtrlNode(this IRoMayVar<MixNode> node) => node.V.IsSome(out var nod) && nod.V is CtrlNode;
	private static bool IsFlexNode(this IRoMayVar<MixNode> node) => node.V.IsSome(out var nod) && nod.V is StFlexNode;

	private static bool CanTrackNode(this IRoMayVar<MixNode> node, out IMixNode? n)
	{
		var res = node.CanTrackNode();
		n = res switch
		{
			false => null,
			true => node.V.Ensure().V
		};
		return res;
	}

	private static bool CanTrackNode(this IRoMayVar<MixNode> node) => node.V.IsSome(out var nod) switch
	{
		true => nod.V switch
		{
			StFlexNode => true,
			CtrlNode when nod.Parent == null => true,
			_ => false
		},
		false => false,
	};
}


file static class Consts
{
	public static Bitmap GetPopIcon(int idx) => PopIcons[idx % PopIcons.Length];

	private static readonly Bitmap[] PopIcons =
	{
		Resource.LayoutTree_Pop0,
		Resource.LayoutTree_Pop1,
		Resource.LayoutTree_Pop2,
		Resource.LayoutTree_Pop3,
		Resource.LayoutTree_Pop4,
		Resource.LayoutTree_Pop5,
		Resource.LayoutTree_Pop6,
	};
}
