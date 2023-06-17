using System.Reactive.Linq;
using BrightIdeasSoftware;
using PowMaybe;
using PowRxVar;
using PowWinForms.TreeEditing.Utils;

namespace FlexBuilder.Utils.Exts;

sealed record TooltipDef(
	string Title,
	string Text,
	ToolTipControl.StandardIcons? Icon
);

static class TreeListViewExts
{
	public static void SetupForNodeType<T>(this TreeListView ctrl)
	{
		ctrl.FullRowSelect = true;
		ctrl.MultiSelect = false;
		ctrl.UseCellFormatEvents = true;
		ctrl.UseOverlays = false;
		ctrl.UseWaitCursorWhenExpanding = false;

		TreeCtrlOps.SetNodGeneric<T>(ctrl);
	}


	public static IDisposable SetRoot<T>(this TreeListView ctrl, IRoVar<Maybe<TNod<T>>> mayRootVar)
	{
		var d = new Disp();
		mayRootVar.Subscribe(mayRoot =>
		{
			switch (mayRoot.IsSome(out var root))
			{
				case true:
					ctrl.SetObjects(new[] { root });
					ctrl.ExpandAll();
					break;

				case false:
					ctrl.ClearObjects();
					ctrl.SelectedIndices.Clear();
					break;
			}
		}).D(d);
		return d;
	}


	public static IDisposable PipeSelectedNodeInto<T>(this TreeListView ctrl, IRwMayVar<TNod<T>> selNode)
	{
		var d = new Disp();

		Obs.Merge(
				ctrl.Events().ItemSelectionChanged.Select(e => e.IsSelected switch
				{
					true => May.Some((TNod<T>)ctrl.SelectedObject),
					false => May.None<TNod<T>>()
				}),
				ctrl.Events().Click.Where(_ => ctrl.GetNodeUnderMouse<T>().IsNone())
					.Select(_ => May.None<TNod<T>>())
			)
			.PipeInto(selNode).D(d);

		return d;
	}

	public static IDisposable PipeHoveredNodeInto<T>(this TreeListView ctrl, IRwMayVar<TNod<T>> hovNode)
	{
		var d = new Disp();

		ctrl.Events().MouseMove.Select(_ => TreeCtrlOps.GetNodeUnderMouse<T>(ctrl))
			.PipeInto(hovNode).D(d);

		return d;
	}

	public static void NotifyNodeChanged<T>(this TreeListView ctrl, TNod<T> node)
	{
		ctrl.RefreshObject(node);
	}
	public static void NotifyNodeAddedAndSelectIt<T>(this TreeListView ctrl, TNod<T> parent, TNod<T> child)
	{
		ctrl.RefreshObject(parent); // needed for non leaf nodes (otherwise it doesn't appear)
		ctrl.Reveal(child, true);
	}
	public static void NotifyNodeRemoved<T>(this TreeListView ctrl, TNod<T> node)
	{
		ctrl.RemoveObject(node);
	}




	public static void AddTextColumn<T>(
		this TreeListView ctrl,
		string name,
		int? width,
		Func<TNod<T>, string> textFun
	) =>
		ctrl.Columns.Add(new OLVColumn(name, name)
		{
			Width = width ?? 60,
			FillsFreeSpace = !width.HasValue,
			AspectGetter = obj => obj switch
			{
				TNod<T> nod => textFun(nod),
				_ => null
			}
		});

	public static void AddTextColumnWithColorAndTooltip<T>(
		this TreeListView ctrl,
		string name,
		int? width,
		Func<TNod<T>, (string, Color?, TooltipDef?)> fun
	)
	{
		ctrl.AddTextColumn<T>(name, width, nod => fun(nod).Item1);

		ctrl.FormatCell += (_, args) =>
		{
			if (args.Column.Text != name) return;
			var nod = (TNod<T>)args.Model;
			var color = fun(nod).Item2;
			if (!color.HasValue) return;
			args.SubItem.ForeColor = color.Value;
		};

		ctrl.CellToolTipShowing += (_, args) =>
		{
			if (args.Column.Text != name) return;
			var nod = (TNod<T>)args.Model;
			var tooltip = fun(nod).Item3;
			if (tooltip == null) return;
			(args.Title, args.Text, args.StandardIcon) = tooltip;
		};
	}



	public static void AddImageColumn<T>(
		this TreeListView ctrl,
		string name,
		int width,
		Func<TNod<T>, Image?> imageFun
	) =>
		ctrl.Columns.Add(new OLVColumn(name, name)
		{
			Width = width,
			AspectGetter = _ => null,
			ImageGetter = obj => obj switch
			{
				TNod<T> nod => imageFun(nod),
				_ => null
			}
		});


	public static void AddImageColumnWithTooltip<T>(
		this TreeListView ctrl,
		string name,
		int width,
		Func<TNod<T>, (Image?, TooltipDef?)> fun
	)
	{
		ctrl.AddImageColumn<T>(name, width, nod => fun(nod).Item1);

		ctrl.CellToolTipShowing += (_, args) =>
		{
			if (args.Column.Text != name) return;
			var nod = (TNod<T>)args.Model;
			var tooltip = fun(nod).Item2;
			if (tooltip == null) return;
			(args.Title, args.Text, args.StandardIcon) = tooltip;
		};
	}




	


	private static Maybe<TNod<T>> GetNodeUnderMouse<T>(this TreeListView ctrl)
	{
		if (ctrl.MouseMoveHitTest == null) return May.None<TNod<T>>();
		var obj = ctrl.MouseMoveHitTest.RowObject;
		if (obj == null) return May.None<TNod<T>>();
		if (obj is not TNod<T> node) return May.None<TNod<T>>();
		return May.Some(node);
	}
}