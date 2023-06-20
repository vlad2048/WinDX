using System.Reactive.Linq;
using BrightIdeasSoftware;
using FlexBuilder.Editors;
using FlexBuilder.Utils;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.StructsShared;
using LayoutSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms.Utils;

namespace FlexBuilder.Logic;

static partial class Setup
{
	public static void EditTreeInit(
		MainWin ui,
		IRoMayVar<Layout> layout
	)
	{
		var ctrl = ui.layoutTree;
		ctrl.SetupForNodeType<FlexNode>();
		SetupEditColumns(ctrl, layout);
	}



	public static IDisposable EditTreeHook(
		MainWin ui,
		IFullRwMayBndVar<LayoutDef> layoutDef,
		IRoMayVar<Layout> layout,
		IRwMayVar<Node> selNode,
		IRwMayVar<Node> hovNode
	)
	{
		var d = new Disp();
		var ctrl = ui.layoutTree;

		ctrl.SetRoot(layout.Map2(e => e.Root)).D(d);
		ctrl.PipeSelectedNodeInto(selNode).D(d);
		ctrl.PipeHoveredNodeInto(hovNode).D(d);

		EditNode(ui, layoutDef, selNode).D(d);
		SetupContextMenu(ui, layoutDef, selNode).D(d);
		EditLayoutDefWinSize(ui, layoutDef).D(d);

		return d;
	}

	private static IDisposable EditNode(
		MainWin ui,
		IFullRwMayBndVar<LayoutDef> layoutDef,
		IRwMayVar<Node> selNode
	)
	{
		var d = new Disp();
		var ctrl = ui.layoutTree;

		selNode.Subscribe(mayNode => ui.nodeEditor.Value.SetOuter(mayNode.Select(e => e.V))).D(d);

		var selNodeL = selNode;
		ui.nodeEditor.Value.WhenInner.Subscribe(mayNode =>
		{
			var nodeContent = mayNode.Ensure();
			var node = selNodeL.V.Ensure();
			node.ChangeContent(nodeContent);
			layoutDef.SetInner(layoutDef.V);
			ctrl.NotifyNodeChanged(node);
		}).D(d);

		return d;
	}




	private static IDisposable SetupContextMenu(
		MainWin ui,
		IFullRwMayBndVar<LayoutDef> layoutDef,
		IRoMayVar<Node> selNode
	)
	{
		var d = new Disp();
		var ctrl = ui.layoutTree;

		ui.layoutTreeContextMenu.Events().Opening.Subscribe(e =>
		{
			var mayNode = selNode.V;
			if (mayNode.IsNone(out var node))
			{
				e.Cancel = true;
				return;
			}
			ui.removeNodeMenuItem.Enabled = node.Parent != null;
		}).D(d);


		void AddNode(Node parent, FlexNode child)
		{
			var childNode = Nod.Make(child);
			parent.AddChild(childNode);
			layoutDef.SetInner(layoutDef.V);
			ctrl.NotifyNodeAddedAndSelectIt(parent, childNode);
		}

		void RemoveNode(Node node)
		{
			node.Parent!.RemoveChild(node);
			layoutDef.SetInner(layoutDef.V);
			ctrl.NotifyNodeRemoved(node);
		}


		ui.addFillMenuItem.Events().Click.Subscribe(_ =>
		{
			var mayNode = selNode.V;
			if (mayNode.IsNone(out var node)) return;
			var r0 = DimEditor.rnd.Next(30, 160);
			var r1 = DimEditor.rnd.Next(30, 160);
			AddNode(node, new FlexNode(Vec.Fix(r0, r1), new FillStrat(new ScrollSpec(BoolVec.False)), Mg.Zero));
		}).D(d);

		ui.addStackMenuItem.Events().Click.Subscribe(_ =>
		{
			var mayNode = selNode.V;
			if (mayNode.IsNone(out var node)) return;
			AddNode(node, new FlexNode(Vec.FilFit, new StackStrat(Dir.Horz, Align.Start), Mg.Zero));
		}).D(d);

		ui.addWrapMenuItem.Events().Click.Subscribe(_ =>
		{
			var mayNode = selNode.V;
			if (mayNode.IsNone(out var node)) return;
			AddNode(node, new FlexNode(Vec.FilFit, new WrapStrat(Dir.Horz), Mg.Zero));
		}).D(d);

		ui.removeNodeMenuItem.Events().Click.ToUnit().Merge(
				ui.layoutTree.Events().KeyDown.Where(e => e.KeyCode == Keys.Delete).ToUnit()
			)
			.Where(_ => selNode.V.IsSome(out var node) && node.Parent != null)
			.Subscribe(_ =>
			{
				var node = selNode.V.Ensure();
				RemoveNode(node);
			}).D(d);

		return d;
	}





	private static IDisposable EditLayoutDefWinSize(MainWin ui, IFullRwMayBndVar<LayoutDef> layout)
	{
		var xVal = ui.winDimsXNumeric;
		var yVal = ui.winDimsYNumeric;
		var xOn = ui.winDimsXCheckBox;
		var yOn = ui.winDimsYCheckBox;

		LayoutDef mut(Dir dir, int v)
		{
			var lay = layout.V.Ensure();
			return lay with { WinSize = lay.WinSize.ChangeComponent(dir, v) };
		}

		return
			layout.EditInner(
				enableUI:
				on => xVal.Visible = yVal.Visible = xOn.Visible = yOn.Visible = on,

				setUI:
				v =>
				{
					var s = v.WinSize;
					var xEnabled = s.Dir(Dir.Horz).HasValue;
					var yEnabled = s.Dir(Dir.Vert).HasValue;
					xOn.Checked = xEnabled;
					yOn.Checked = yEnabled;
					xVal.Enabled = xEnabled;
					yVal.Enabled = yEnabled;
					if (xEnabled) xVal.Value = s.X!.Value;
					if (yEnabled) yVal.Value = s.Y!.Value;
				},

				UI2Val:
				Obs.Merge(
					xOn.Events().CheckedChanged.Select(_ => mut(Dir.Horz, xOn.Checked ? (int)xVal.Value : int.MaxValue)),
					yOn.Events().CheckedChanged.Select(_ => mut(Dir.Vert, yOn.Checked ? (int)yVal.Value : int.MaxValue)),
					xVal.Events().ValueChanged.Select(_ => mut(Dir.Horz, (int)xVal.Value)),
					yVal.Events().ValueChanged.Select(_ => mut(Dir.Vert, (int)yVal.Value))
				)
			);
	}





	private static void SetupEditColumns(
	    TreeListView ctrl,
	    IRoMayVar<Layout> layout
	   )
    {
	    var warningIcon = Resource.LayoutTree_Warning;

		ctrl.AddTextColumn<FlexNode>(ColumnName.Node, null, nod => $"{nod.V.Strat}");

		ctrl.AddTextColumnWithColorAndTooltip<FlexNode>(ColumnName.Width, 80, nod =>
		{
			var warn = layout.GetDimWarningForColumn(nod, WarningDir.Horz);
			return warn switch
			{
				null => (
					nod.V.Dim.X.Fmt(),
					null,
					null
				),
				not null => (
					warn.FixedDim.X.Fmt(),
					ColorName.Red,
					new TooltipDef(
						"Original value",
						nod.V.Dim.X.Fmt(),
						null
					)
				)
			};
		});

		ctrl.AddTextColumnWithColorAndTooltip<FlexNode>(ColumnName.Height, 80, nod =>
		{
			var warn = layout.GetDimWarningForColumn(nod, WarningDir.Vert);
			return warn switch
			{
				null => (
					nod.V.Dim.Y.Fmt(),
					null,
					null
				),
				not null => (
					warn.FixedDim.Y.Fmt(),
					ColorName.Red,
					new TooltipDef(
						"Original value",
						nod.V.Dim.Y.Fmt(),
						null
					)
				)
			};
		});

		ctrl.AddTextColumn<FlexNode>(ColumnName.R, 100, nod => (layout.V.IsSome(out var lay) && lay.RMap.ContainsKey(nod)) switch
		{
			true => $"{lay!.RMap[nod]}",
			false => string.Empty
		});

		ctrl.AddImageColumnWithTooltip<FlexNode>(ColumnName.Warnings, 65, nod =>
		{
			if (layout.V.IsNone(out var lay)) return (null, null);
			var warn = lay.WarningMap.GetDimWarningForColumn(nod, WarningDir.Horz | WarningDir.Vert);
			if (warn == null) return (null, null);
			return (
				warningIcon,
				new TooltipDef(
					"Layout Warning" + (warn.Messages.Length > 1 ? "s" : ""),
					warn.Messages.JoinText(Environment.NewLine),
					ToolTipControl.StandardIcons.Warning
				)
			);
		});
    }
	

    private static FlexWarning? GetDimWarningForColumn(this IReadOnlyDictionary<Node, FlexWarning> warningMap, Node nod, WarningDir colDir)
    {
	    if (!warningMap.TryGetValue(nod, out var warn)) return null;
	    if ((colDir & warn.Dir) == 0) return null;
	    return warn;
    }

    private static FlexWarning? GetDimWarningForColumn(this IRoMayVar<Layout> layout, Node nod, WarningDir colDir) =>
	    layout.V.IsSome(out var lay) switch
	    {
		    true => lay.WarningMap.GetDimWarningForColumn(nod, colDir),
		    false => null
	    };
}