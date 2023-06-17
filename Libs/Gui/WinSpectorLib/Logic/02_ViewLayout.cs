using _3_WinSpectorLib;
using BrightIdeasSoftware;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowMaybe;
using PowRxVar;
using PowWinForms.Utils;
using Structs;
using WinSpectorLib.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ViewLayout(WinSpectorWin ui, IRoMayVar<MixLayout> selLayout)
	{
		var d = new Disp();
		var ctrl = ui.layoutTree;

		PrepareTree(ctrl, selLayout);

		ctrl.SetRoot(selLayout.Map2(e => e.MixRoot)).D(d);

		return d;
	}


	private static class ColumnName
	{
		public const string Node = "Node";
		public const string Width = "Width";
		public const string Height = "Height";
		public const string R = "R";
		public const string Warnings = "Warnings";
	}

	private static void PrepareTree(TreeListView ctrl, IRoMayVar<MixLayout> layout)
	{
		var warningIcon = Resource.LayoutTree_Warning;
		var errorIcon = Resource.LayoutTree_Error;

		ctrl.SetupForNodeType<IMixNode>();


		ctrl.AddTextColumn<IMixNode>(ColumnName.Node, null, nod => nod.V switch
		{
			CtrlNode {Ctrl: var c} => c.GetType().Name,
			StFlexNode {Flex: var flex} => $"{flex}",
			_ => "_",
		});

		ctrl.AddTextColumnWithColorAndTooltip<IMixNode>(ColumnName.Width, 80, nod =>
		{
			if (nod.V is not StFlexNode { Flex: var flex, State: var nodState }) return ("_", null, null);
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

		ctrl.AddTextColumnWithColorAndTooltip<IMixNode>(ColumnName.Height, 80, nod =>
		{
			if (nod.V is not StFlexNode { Flex: var flex, State: var nodState }) return ("_", null, null);
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

		ctrl.AddTextColumn<IMixNode>(ColumnName.R, 100, nod =>
		{
			if (nod.V is not StFlexNode { State: var nodState }) return "_";
			if (layout.V.IsNone(out var lay)) return "_";
			var r = lay.RMap[nodState];
			return $"{r}";
		});

		ctrl.AddImageColumnWithTooltip<IMixNode>(ColumnName.Warnings, 65, nod =>
		{
			if (layout.V.IsNone(out var lay)) return (null, null);

			switch (nod.V)
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
}