using System.Reactive.Linq;
using BrightIdeasSoftware;
using FlexBuilder.Editors;
using FlexBuilder.Structs;
using FlexBuilder.Utils;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;

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









	/*
	public static IDisposable HookEditTree(
		MainWin ui,
		IFullRwMayBndVar<LayoutDef> layoutDef,
		IRoMayVar<Layout> layout,
		IRwMayVar<Node> selNode,
		IRwMayVar<Node> hovNode,
		bool disableTreeRefresh
	)
	{
		var d = new Disp();

		if (disableTreeRefresh)
		{
			return d;
		}

		ui.redrawStatusBtn.Events().Click.Subscribe(_ => layoutDef.V = layoutDef.V).D(d);

		SetupEditor(
			out var tree,
			out var selNodeLocal,
			out var hovNodeLocal,
			out var evtSig,
			out _,
			ui,
			layout
		).D(d);

		selNodeLocal.PipeTo(selNode);
		hovNodeLocal.PipeTo(hovNode);


		tree.WhenSome().Subscribe(treeVal =>
		{
			if (layoutDef.V.IsNone(out var lay)) return;
			lay = lay with { Root = treeVal };
			layoutDef.SetInner(May.Some(lay));
		}).D(d);


		layoutDef.WhenOuter.SubscribeToSome(layoutDefVal => evtSig.SignalTreeLoaded(layoutDefVal.Root)).D(d);
		layoutDef.WhenOuter.SubscribeToNone(() => evtSig.SignalTreeUnloaded()).D(d);

		selNode.Subscribe(mayNode => ui.nodeEditor.Value.SetOuter(mayNode.Select(e => e.V))).D(d);

		var selNodeL = selNode;
		ui.nodeEditor.Value.WhenInner.Subscribe(mayNode =>
		{
			var nodeContent = mayNode.Ensure();
			var node = selNodeL.V.Ensure();
			evtSig.SignalNodeChanged(node, nodeContent);
		}).D(d);

		return d;
	}



    public static IDisposable SetupEditor(
        out IRoMayVar<Node> tree,
        out IRoMayVar<Node> selNode,
        out IRoMayVar<Node> hoveredNode,
        out ITreeEvtSig<FlexNode> evtSig,
        out ITreeEvtObs<FlexNode> evtObs,
        MainWin ui,
		IRoMayVar<Layout> layout
       )
    {
        var d = new Disp();
        var ctrl = ui.layoutTree;

        TreeEditor.Setup(
            out tree,
            out selNode,
            out evtSig,
            out evtObs,
            ctrl
        ).D(d);

        //evtObs.WhenChanged.Subscribe(e => L($"{e}")).D(d);

        layout.Map2(e => e.TotalSz)
	        .ObserveOnUIThread()
	        .Subscribe(_ => ctrl.Refresh()).D(d);


        SetupContextMenu(ui, evtSig, selNode).D(d);

        hoveredNode = VarMay.Make(
			Obs.Merge(
				ctrl.Events().MouseMove.Select(_ => TreeCtrlOps.GetNodeUnderMouse<FlexNode>(ctrl)),
				evtObs.WhenTreeLoaded().Select(_ => May.None<Node>()),
				evtObs.WhenTreeUnloaded().Select(_ => May.None<Node>())
			)
        ).D(d);

        return d;
    }



    private static IDisposable SetupContextMenu(
        MainWin ui,
        ITreeEvtSig<FlexNode> evtSig,
        IRoMayVar<Node> selNode
    )
    {
        var d = new Disp();

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

        ui.addFillMenuItem.Events().Click.Subscribe(_ =>
        {
            var mayNode = selNode.V;
            if (mayNode.IsNone(out var node)) return;
            var r0 = DimEditor.rnd.Next(30, 160);
            var r1 = DimEditor.rnd.Next(30, 160);
            evtSig.SignalNodeAdded(node, new FlexNode(Vec.Fix(r0, r1), new FillStrat(new ScrollSpec(BoolVec.False)), Mg.Zero));
        }).D(d);

        ui.addStackMenuItem.Events().Click.Subscribe(_ =>
        {
            var mayNode = selNode.V;
            if (mayNode.IsNone(out var node)) return;
            evtSig.SignalNodeAdded(node, new FlexNode(Vec.FilFit, new StackStrat(Dir.Horz, Align.Start), Mg.Zero));
        }).D(d);

        ui.addWrapMenuItem.Events().Click.Subscribe(_ =>
        {
            var mayNode = selNode.V;
            if (mayNode.IsNone(out var node)) return;
            evtSig.SignalNodeAdded(node, new FlexNode(Vec.FilFit, new WrapStrat(Dir.Horz), Mg.Zero));
        }).D(d);

        ui.removeNodeMenuItem.Events().Click.ToUnit().Merge(
                ui.layoutTree.Events().KeyDown.Where(e => e.KeyCode == Keys.Delete).ToUnit()
            )
            .Where(_ => selNode.V.IsSome(out var node) && node.Parent != null)
            .Subscribe(_ =>
            {
                var node = selNode.V.Ensure();
                evtSig.SignalNodeRemoved(node);
            }).D(d);

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


    private static void SetupEditColumns(
	    TreeListView ctrl,
	    IRoMayVar<Layout> layout
	   )
    {
	    ctrl.FullRowSelect = true;
	    ctrl.MultiSelect = false;
	    ctrl.UseCellFormatEvents = true;
	    ctrl.UseOverlays = false;
	    ctrl.UseWaitCursorWhenExpanding = false;

	    var warningIcon = Resource.LayoutTree_Warning;

		ctrl.AddTextColumn<FlexNode>(ColumnNames.Node, null, nod => $"{nod.V.Strat}");

		ctrl.AddTextColumn<FlexNode>(ColumnNames.Width, 80, nod =>
		{
			var warn = WarningDir.Horz.GetDimWarningForColumn(nod, layout);
			return warn switch
			{
				null => nod.V.Dim.X.Fmt(),
				not null => warn.FixedDim.X.Fmt()
			};
		});

		ctrl.AddTextColumn<FlexNode>(ColumnNames.Height, 80, nod =>
		{
			var warn = WarningDir.Vert.GetDimWarningForColumn(nod, layout);
			return warn switch
			{
				null => nod.V.Dim.Y.Fmt(),
				not null => warn.FixedDim.Y.Fmt()
			};
		});

		ctrl.AddTextColumn<FlexNode>(ColumnNames.R, 100, nod =>
		{
			if (layout.V.IsNone(out var lay) || !lay.RMap.TryGetValue(nod, out var r)) return string.Empty;
			return $"{r}";
		});

		ctrl.AddImageColumn<FlexNode>(ColumnNames.Warnings, 65, nod =>
		{
			if (layout.V.IsNone(out var lay)) return null;
			return lay.WarningMap.ContainsKey(nod) switch
			{
				true => warningIcon,
				false => null
			};
		});

		ctrl.FormatCell += (_, args) =>
		{
			if (layout.V.IsNone(out var lay)) return;
			var warn = lay.WarningMap.GetDimWarningForColumn((Node)args.Model, args.Column.GetWarnDir());
			if (warn == null) return;
			args.SubItem.ForeColor = Color.FromArgb(245, 100, 124);
		};

		ctrl.CellToolTipShowing += (_, args) =>
		{
			if (layout.V.IsNone(out var lay)) return;
			var nod = (Node)args.Model;
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
					args.Text = nod.V.Dim.Dir(column.GetDir()).Fmt();
					args.Title = "Original value";
					break;
			}
		};
    }
	*/

    private static WarningDir GetWarnDir(this OLVColumn column) => column.Text switch
    {
	    ColumnName.Width => WarningDir.Horz,
	    ColumnName.Height => WarningDir.Vert,
	    _ => 0
    };

    private static Dir GetDir(this OLVColumn column) => column.Text switch
    {
	    ColumnName.Width => Dir.Horz,
	    ColumnName.Height => Dir.Vert,
	    _ => 0
    };

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