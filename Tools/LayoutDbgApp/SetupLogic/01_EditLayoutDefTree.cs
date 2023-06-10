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
using PowTrees.Algorithms;
using PowWinForms.TreeEditing;
using PowWinForms.TreeEditing.Structs;
using PowWinForms.TreeEditing.Utils;

namespace LayoutDbgApp.SetupLogic;

static partial class Setup
{
	public static IDisposable EditLayoutDefTree(
		MainWin ui,
		IFullRwBndVar<Maybe<LayoutDef>> layoutDef,
		IRoVar<Maybe<Layout>> layout,
		out IRoVar<Maybe<Node>> selNode,
		out IRoVar<Maybe<Node>> hoveredNode
	)
	{
		var d = new Disp();

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


		/*Observable.Merge(
				evtObs.WhenNodeAdded().ToUnit(),
				evtObs.WhenNodeRemoved().ToUnit(),
				evtObs.WhenNodeChanged().ToUnit()
			)
			.Subscribe(_ =>
			{
				var lay = layoutDef.V.Ensure();
				lay = lay with { Root = tree.V.Ensure() };
				L("[Edit        ] Def <- " + lay.Root.LogToString());
				layoutDef.SetInner(May.Some(lay));
			}).D(d);*/



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
        out IRoVar<Maybe<Node>> tree,
        out IRoVar<Maybe<Node>> selNode,
        out IRoVar<Maybe<Node>> hoveredNode,
        out ITreeEvtSig<FlexNode> evtSig,
        out ITreeEvtObs<FlexNode> evtObs,
        MainWin ui,
		IRoVar<Maybe<Layout>> layout
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

        hoveredNode = Var.Make(
			May.None<Node>(),
			Observable.Merge(
				ctrl.Events().MouseMove.Select(_ => TreeCtrlOps.GetNodeUnderMouse<FlexNode>(ctrl)),
				evtObs.WhenTreeLoaded().Select(_ => May.None<Node>()),
				evtObs.WhenTreeUnloaded().Select(_ => May.None<Node>())
			)
        ).D(d);

        return d;
    }



    private static void SetupColumns(
	    TreeListView ctrl,
	    IRoVar<Maybe<Layout>> layout
	   )
    {
		//void AddCol(string name, int width,)

		ctrl.UseCellFormatEvents = true;

		ctrl.FormatCell += (_, args) =>
		{
			if (layout.V.IsNone(out var lay)) return;
			var warnings = lay.WarningMap.GetMatchingWarnings((Node)args.Model, args.Column);
			if (warnings.Length == 0) return;

			args.SubItem.ForeColor = Color.FromArgb(245, 100, 124);
		};

		ctrl.CellToolTipShowing += (_, args) =>
		{
			if (layout.V.IsNone(out var lay)) return;
			var warnings = lay.WarningMap.GetMatchingWarnings((Node)args.Model, args.Column);
			if (warnings.Length == 0) return;

			args.Text = warnings.Select(e => e.Message).JoinText(Environment.NewLine);
			args.Title = "Layout Warning" + (warnings.Length > 1 ? "s" : "");
			args.StandardIcon = ToolTipControl.StandardIcons.Warning;
		};

        ctrl.Columns.Add(new OLVColumn("Node", "Node")
        {
            FillsFreeSpace = true,
			
            AspectGetter = obj =>
            {
                if (obj is not Node nod) return "_";
                return $"{nod.V.Strat}";
            }
        });
        ctrl.Columns.Add(new OLVColumn("Width", "Width")
        {
	        Width = 80,
	        AspectGetter = obj =>
	        {
		        if (obj is not Node nod) return "_";
		        return nod.V.Dim.X.Fmt();
	        }
        });
        ctrl.Columns.Add(new OLVColumn("Height", "Height")
        {
	        Width = 80,
	        AspectGetter = obj =>
	        {
		        if (obj is not Node nod) return "_";
		        return nod.V.Dim.Y.Fmt();
	        }
        });
        ctrl.Columns.Add(new OLVColumn("R", "R")
        {
            Width = 100,
            AspectGetter = obj =>
            {
                if (obj is not Node nod) return "_";
                if (layout.V.IsNone(out var lay)) return "err_0";
                if (!lay.RMap.TryGetValue(nod, out var r)) return "err_1";
                return $"{r}";
            }
        });
    }

    private static LayoutWarning[] GetMatchingWarnings(this IReadOnlyDictionary<Node, LayoutWarning[]> warnings, Node node, OLVColumn column)
    {
	    if (!warnings.TryGetValue(node, out var arr)) return Array.Empty<LayoutWarning>();
	    WarningDir? colDir = column.Text switch
	    {
		    "Width" => WarningDir.Horz,
		    "Height" => WarningDir.Vert,
		    _ => null
	    };
	    if (colDir == null) return Array.Empty<LayoutWarning>();
	    return arr.WhereToArray(e => e.Dir.HasFlag(colDir.Value));
    }


    private static IDisposable SetupContextMenu(
        MainWin ui,
        ITreeEvtSig<FlexNode> evtSig,
        IRoVar<Maybe<Node>> selNode
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
            evtSig.SignalNodeAdded(node, new FlexNode(Vec.Fix(r0, r1), new FillStrat(), Mg.Zero));
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

        ui.addScrollMenuItem.Events().Click.Subscribe(_ =>
        {
	        var mayNode = selNode.V;
	        if (mayNode.IsNone(out var node)) return;
	        evtSig.SignalNodeAdded(node, new FlexNode(Vec.Fit, new ScrollStrat(new BoolVec(false, true)), Mg.Zero));
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
}