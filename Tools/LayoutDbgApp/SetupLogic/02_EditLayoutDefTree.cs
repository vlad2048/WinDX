using System.Reactive.Linq;
using BrightIdeasSoftware;
using LayoutDbgApp.Editors;
using LayoutDbgApp.Structs;
using LayoutDbgApp.Utils.Exts;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Flex;
using LayoutSystem.Utils;
using PowBasics.CollectionsExt;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms.TreeEditing;
using PowWinForms.TreeEditing.Structs;
using PowWinForms.TreeEditing.Utils;

namespace LayoutDbgApp.SetupLogic;

static partial class Setup
{
	public static IDisposable EditLayoutDefTree(
		MainWin ui,
		IFullRwMayBndVar<LayoutDef> layoutDef,
		IRoMayVar<Layout> layout,
		out IRoMayVar<Node> selNode,
		out IRoMayVar<Node> hoveredNode,
		bool disableTreeRefresh
	)
	{
		var d = new Disp();

		if (disableTreeRefresh)
		{
			selNode = VarMay.Make<Node>().D(d);
			hoveredNode = VarMay.Make<Node>().D(d);
			return d;
		}

		SetupEditor(
			out var tree,
			out selNode,
			out hoveredNode,
			out var evtSig,
			out var evtObs,
			ui,
			layout
		).D(d);


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


        SetupColumns(ctrl, layout);
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
            evtSig.SignalNodeAdded(node, new FlexNode(Vec.Fix(r0, r1), new FillStrat(BoolVec.False), Mg.Zero));
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


    private static void SetupColumns(
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

	    void AddImageColumn(string name, int width, Func<Node, Image?> imgFun) =>
		    ctrl.Columns.Add(new OLVColumn(name, name)
			    {
				    Width = width,
				    AspectGetter = _ => null,
				    ImageGetter = obj =>
				    {
					    if (obj is not Node nod) return null;
					    return imgFun(nod);
				    }
			    }
		    );

	    void AddTextColumn(string name, int? width, Func<Node, string> textFun) =>
		    ctrl.Columns.Add(new OLVColumn(name, name)
		    {
			    Width = width ?? 60,
			    FillsFreeSpace = !width.HasValue,
			    AspectGetter = obj =>
			    {
				    if (obj is not Node nod) return null;
				    return textFun(nod);
			    }
		    });

		AddTextColumn(ColumnNames.Node, null, nod => $"{nod.V.Strat}");

		AddTextColumn(ColumnNames.Width, 80, nod =>
		{
			var warn = WarningDir.Horz.GetDimWarningForColumn(nod, layout);
			return warn switch
			{
				null => nod.V.Dim.X.Fmt(),
				not null => warn.FixedDim.X.Fmt()
			};
		});

		AddTextColumn(ColumnNames.Height, 80, nod =>
		{
			var warn = WarningDir.Vert.GetDimWarningForColumn(nod, layout);
			return warn switch
			{
				null => nod.V.Dim.Y.Fmt(),
				not null => warn.FixedDim.Y.Fmt()
			};
		});

		AddTextColumn(ColumnNames.R, 100, nod =>
		{
			if (layout.V.IsNone(out var lay)) return "err_0";
			if (!lay.RMap.TryGetValue(nod, out var r)) return "err_1";
			return $"{r}";
		});

		AddImageColumn(ColumnNames.Warnings, 65, nod =>
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

    private static FlexWarning? GetDimWarningForColumn(this IReadOnlyDictionary<Node, FlexWarning> warningMap, Node nod, WarningDir colDir)
    {
	    if (!warningMap.TryGetValue(nod, out var warn)) return null;
	    if ((colDir & warn.Dir) == 0) return null;
	    return warn;
    }

    private static FlexWarning? GetDimWarningForColumn(this WarningDir colDir, Node nod, IRoMayVar<Layout> layout) => layout.V.IsSome(out var lay) switch
    {
	    true => lay.WarningMap.GetDimWarningForColumn(nod, colDir),
	    false => null
    };
}