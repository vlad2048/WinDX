using BrightIdeasSoftware;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowMaybe;
using PowRxVar;
using PowTrees.Algorithms;
using PowWinForms.Utils;
using WinSpectorLib.Controls;
using WinSpectorLib.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ViewLayout(WinSpectorWin ui, IRoMayVar<MixLayout> selLayout, IRwVar<bool> showEvents)
	{
		var d = new Disp();
		var ctrl = ui.layoutTree;
		PrepareTree(ctrl, selLayout, ui.eventDisplayer);
		ctrl.SetRoot(selLayout.Map2(e => e.MixRoot.AddNfoToTree())).D(d);

		var selNodeNfo = VarMay.Make<TNod<MixNodeWithNfo>>().D(d);
		var hovNodeNfo = VarMay.Make<TNod<MixNodeWithNfo>>().D(d);
		ctrl.PipeSelectedNodeInto(selNodeNfo).D(d);
		ctrl.PipeHoveredNodeInto(hovNodeNfo).D(d);
		var selNode = selNodeNfo.SelectVarMay(e => e.Map(f => f.Node));
		var hovNode = hovNodeNfo.SelectVarMay(e => e.Map(f => f.Node));

		Obs.Merge(selNode, hovNode).Subscribe(_ =>
		{
			if (selLayout.V.IsNone(out var lay)) return;
			var win = lay.Win;
			win.SpectorDrawState.SelNode.V = selNode.V;
			win.SpectorDrawState.HovNode.V = hovNode.V;
		}).D(d);

		SetupContextMenu(ui, selNode, showEvents).D(d);

		return d;
	}



	// ***************
	// * Nfo Wrapper *
	// ***************
	private sealed record MixNodeWithNfo(
		IMixNode Node,
		int? PopIndex
	);
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
	private static bool IsPop(this MixNode node) => node.V is StFlexNode { Flex.Flags.Pop: true };
	private static TNod<MixNodeWithNfo> AddNfoToTree(this MixNode root)
	{
		var map = root.GetPopMapping();
		return root.Map(e => new MixNodeWithNfo(e, map[e]));
	}



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

	private static void PrepareTree(TreeListView ctrl, IRoMayVar<MixLayout> layout, EventDisplayer eventDisplayer)
	{
		var warningIcon = Resource.LayoutTree_Warning;
		var errorIcon = Resource.LayoutTree_Error;

		ctrl.SetupForNodeType<MixNodeWithNfo>();

		Image? GetImage(TNod<MixNodeWithNfo> nod) => nod.V.PopIndex.HasValue switch
		{
			true => Consts.GetPopIcon(nod.V.PopIndex.Value),
			false => null
		};

		ctrl.AddTextColumnWithImage<MixNodeWithNfo>(ColumnName.Node, null, nod => nod.V.Node switch
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
		});

		ctrl.AddTextColumn<MixNodeWithNfo>(ColumnName.Flags, 100, nod => nod.V.Node switch
		{
			CtrlNode => "_",
			StFlexNode { Flex.Flags: var flags } => $"{flags}",
			_ => "_"
		});


		ctrl.AddTextColumnWithColorAndTooltip<MixNodeWithNfo>(ColumnName.Width, 80, nod =>
		{
			if (nod.V.Node is not StFlexNode { Flex: var flex, State: var nodState }) return ("_", null, null);
			var warn = layout.GetDimWarningForColumn(nodState, WarningDir.Horz);
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

		ctrl.AddTextColumnWithColorAndTooltip<MixNodeWithNfo>(ColumnName.Height, 80, nod =>
		{
			if (nod.V.Node is not StFlexNode { Flex: var flex, State: var nodState }) return ("_", null, null);
			var warn = layout.GetDimWarningForColumn(nodState, WarningDir.Vert);
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

		ctrl.AddTextColumn<MixNodeWithNfo>(ColumnName.R, 100, nod =>
		{
			if (nod.V.Node is not StFlexNode { State: var nodState }) return "_";
			if (layout.V.IsNone(out var lay)) return "_";
			var r = lay.RMap[nodState];
			return $"{r}";
		});

		ctrl.AddImageColumnWithTooltip<MixNodeWithNfo>(ColumnName.Warnings, 65, nod =>
		{
			if (layout.V.IsNone(out var lay)) return (null, null);

			switch (nod.V.Node)
			{
				case StFlexNode { State: var nodState }:
					var warn = lay.WarningMap.GetDimWarningForColumn(nodState, WarningDir.Horz | WarningDir.Vert);
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
					return lay.UnbalancedCtrls.TryGetValue(ctr, out var errNode) switch
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

		ctrl.AddTextColumn<MixNodeWithNfo>(ColumnName.Tracked, 100, nod =>
		{
			if (nod.V.Node is not StFlexNode st) return string.Empty;
			return eventDisplayer.GetTrackedNodeName(st);
		});
	}

	private static FlexWarning? GetDimWarningForColumn(this IReadOnlyDictionary<NodeState, FlexWarning> warningMap, NodeState nodState, WarningDir colDir)
	{
		if (!warningMap.TryGetValue(nodState, out var warn)) return null;
		if ((colDir & warn.Dir) == 0) return null;
		return warn;
	}

	private static FlexWarning? GetDimWarningForColumn(this IRoMayVar<MixLayout> layout, NodeState nodState, WarningDir colDir) =>
		layout.V.IsSome(out var lay) switch
		{
			true => lay.WarningMap.GetDimWarningForColumn(nodState, colDir),
			false => null
		};



	// ****************
	// * Context Menu *
	// ****************
	private static IDisposable SetupContextMenu(
		WinSpectorWin ui,
		IRoMayVar<MixNode> selNode,
		IRwVar<bool> showEvents
	)
	{
		var d = new Disp();

		ui.layoutTreeContextMenu.Events().Opening.Subscribe(e =>
		{
			if (!selNode.CanTrackNode(out var node))
			{
				e.Cancel = true;
				return;
			}

			var isTracked = ui.eventDisplayer.IsNodeTracked(node!);

			ui.trackEventsMenuItem.Enabled = !isTracked;
			ui.stopTrackingMenuItem.Enabled = isTracked;
			ui.stopAllTrackingMenuItem.Enabled = ui.eventDisplayer.IsAnyNodeTracked();

		}).D(d);


		ui.trackEventsMenuItem.Events().Click.Subscribe(_ =>
		{
			if (!selNode.CanTrackNode(out var node))
				return;
			ui.eventDisplayer.TrackNode(node!);
			showEvents.V = true;
		}).D(d);

		ui.stopTrackingMenuItem.Events().Click.Subscribe(_ =>
		{
			if (!selNode.CanTrackNode(out var node))
				return;
			ui.eventDisplayer.StopTrackingNode(node!);
		}).D(d);

		ui.stopAllTrackingMenuItem.Events().Click.Subscribe(_ =>
		{
			ui.eventDisplayer.StopAllTracking();
		}).D(d);

		return d;
	}



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