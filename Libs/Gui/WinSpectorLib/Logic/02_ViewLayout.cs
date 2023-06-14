using _3_WinSpectorLib;
using BrightIdeasSoftware;
using ControlSystem.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms.TreeEditing.Utils;
using PowWinForms.Utils;
using Structs;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ViewLayout(WinSpectorWin ui, IRoMayVar<MixLayout> selLayout)
	{
		var d = new Disp();
		var ctrl = ui.layoutTree;

		PrepareTree(ctrl, selLayout);

		selLayout.WhenNone().ObserveOnWinFormsUIThread().Subscribe(_ => TreeCtrlOps.NotifyTreeUnloaded(ctrl)).D(d);
		selLayout.WhenSome().ObserveOnWinFormsUIThread().Subscribe(lay => TreeCtrlOps.NotifyTreeLoaded(ctrl, lay.MixRoot)).D(d);

		return d;
	}

	private static class ColumnNames
	{
		public const string Node = "Node";
		public const string Width = "Width";
		public const string Height = "Height";
		public const string R = "R";
		public const string Warnings = "Warnings";
	}

	private static void PrepareTree(TreeListView ctrl, IRoMayVar<MixLayout> selLayout)
	{
		ctrl.FullRowSelect = true;
		ctrl.MultiSelect = false;
		ctrl.UseCellFormatEvents = true;
		ctrl.UseOverlays = false;
		ctrl.UseWaitCursorWhenExpanding = false;

		var warningIcon = Resource.LayoutTree_Warning;

		TreeCtrlOps.SetNodGeneric<IMixNode>(ctrl);

		void AddImageColumn(string name, int width, Func<MixNode, Image?> imgFun) =>
			ctrl.Columns.Add(new OLVColumn(name, name)
				{
					Width = width,
					AspectGetter = _ => null,
					ImageGetter = obj =>
					{
						if (obj is not MixNode nod) return null;
						return imgFun(nod);
					}
				}
			);

		void AddTextColumn(string name, int? width, Func<MixNode, string> textFun) =>
			ctrl.Columns.Add(new OLVColumn(name, name)
			{
				Width = width ?? 60,
				FillsFreeSpace = !width.HasValue,
				AspectGetter = obj =>
				{
					if (obj is not MixNode nod) return null;
					return textFun(nod);
				}
			});
		
		
		AddTextColumn(ColumnNames.Node, null, nod => nod.V switch
		{
			CtrlNode {Ctrl: var c} => c.GetType().Name,
			StFlexNode {Flex: var flex} => $"{flex}",
			_ => "_",
		});

		AddTextColumn(ColumnNames.Width, 80, nod =>
		{
			if (nod.V is not StFlexNode { Flex: var flex }) return "_";
			var warn = WarningDir.Horz.GetDimWarningForColumn(nod, selLayout);
			return warn switch
			{
				null => flex.Dim.X.Fmt(),
				not null => warn.FixedDim.X.Fmt()
			};
		});

		AddTextColumn(ColumnNames.Height, 80, nod =>
		{
			if (nod.V is not StFlexNode { Flex: var flex }) return "_";
			var warn = WarningDir.Vert.GetDimWarningForColumn(nod, selLayout);
			return warn switch
			{
				null => flex.Dim.Y.Fmt(),
				not null => warn.FixedDim.Y.Fmt()
			};
		});

		AddTextColumn(ColumnNames.R, 100, nod =>
		{
			if (selLayout.V.IsNone(out var lay)) return "_";
			if (nod.V is not StFlexNode { State: var nodeState }) return "_";
			var r = lay.RMap[nodeState];
			return $"{r}";
		});
		
		AddImageColumn(ColumnNames.Warnings, 65, nod =>
		{
			if (selLayout.V.IsNone(out var lay)) return null;
			if (nod.V is not StFlexNode {State: var nodeState}) return null;
			return lay.WarningMap.ContainsKey(nodeState) switch
			{
				true => warningIcon,
				false => null
			};
		});


		ctrl.FormatCell += (_, args) =>
		{
			if (selLayout.V.IsNone(out var lay)) return;
			var warn = lay.WarningMap.GetDimWarningForColumn((MixNode)args.Model, args.Column.GetWarnDir());
			if (warn == null) return;
			args.SubItem.ForeColor = Color.FromArgb(245, 100, 124);
		};

		ctrl.CellToolTipShowing += (_, args) =>
		{
			if (selLayout.V.IsNone(out var lay)) return;
			var nod = (MixNode)args.Model;
			var column = args.Column;

			var warn = lay.WarningMap.GetDimWarningForColumn(nod, WarningDir.Horz | WarningDir.Vert);
			if (warn == null) return;

			switch (column.Text)
			{
				case ColumnNames.Warnings:
					args.Text = warn.Messages.JoinText(Environment.NewLine);
					args.Title = "Layout Warning" + (warn.Messages.Length > 1 ? "s" : "");
					args.StandardIcon = ToolTipControl.StandardIcons.Warning;
					break;
				case ColumnNames.Width:
				case ColumnNames.Height:
					warn = lay.WarningMap.GetDimWarningForColumn(nod, column.GetWarnDir());
					if (warn == null) return;
					if (nod.V is not StFlexNode { Flex: var flex }) return;
					args.Text = flex.Dim.Dir(column.GetDir()).Fmt();
					args.Title = "Original value";
					break;
			}
		};
	}

	private static WarningDir GetWarnDir(this OLVColumn column) => column.Text switch
	{
		ColumnNames.Width => WarningDir.Horz,
		ColumnNames.Height => WarningDir.Vert,
		_ => 0
	};

	private static Dir GetDir(this OLVColumn column) => column.Text switch
	{
		ColumnNames.Width => Dir.Horz,
		ColumnNames.Height => Dir.Vert,
		_ => 0
	};

	private static FlexWarning? GetDimWarningForColumn(this IReadOnlyDictionary<NodeState, FlexWarning> warningMap, MixNode nod, WarningDir colDir)
	{
		if (nod.V is not StFlexNode { State: var nodeState } || !warningMap.TryGetValue(nodeState, out var warn)) return null;
		if ((colDir & warn.Dir) == 0) return null;
		return warn;
	}

	private static FlexWarning? GetDimWarningForColumn(this WarningDir colDir, MixNode nod, IRoMayVar<MixLayout> layout) => layout.V.IsSome(out var lay) switch
	{
		true => lay.WarningMap.GetDimWarningForColumn(nod, colDir),
		false => null
	};
}