using BrightIdeasSoftware;
using FlexBuilder.Utils;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Details.Structs;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowMaybe;
using PowRxVar;
using PowWinForms.TreeEditing.Utils;

namespace FlexBuilder.Logic;

static partial class Setup
{
	public static void DetailsTreeInit(
		MainWin ui,
		IRoMayVar<Layout> layout
	)
	{
		var ctrl = ui.detailsTree;
		ctrl.SetupForNodeType<FlexNode>();
		SetupDetailsColumns(ctrl, layout);
	}

	public static IDisposable DetailsTreeHook(
		MainWin ui,
		IRoMayVar<Layout> layout,
		IRwMayVar<Node> selNode,
		IRwMayVar<Node> hovNode
	)
	{
		var d = new Disp();
		var ctrl = ui.detailsTree;
		var rtb = ui.detailsRichTextBox;

		ctrl.SetRoot(layout.Map2(e => e.RootFixed)).D(d);
		ctrl.PipeSelectedNodeInto(selNode).D(d);
		ctrl.PipeHoveredNodeInto(hovNode).D(d);

		TreeCtrlOps.GetSelectedNode<FlexNode>(out var selNodeDetails, ctrl).D(d);

		selNodeDetails.SubscribeToNone(rtb.Clear).D(d);
		selNodeDetails.SubscribeToSome(nod =>
		{
			if (layout.V.IsNone(out var lay)) return;
			if (!lay.DetailsMapFixed.TryGetValue(nod, out var details)) return;
			rtb.ShowDetails(details);
		}).D(d);

		return d;
	}

	private static void SetupDetailsColumns(TreeListView ctrl, IRoMayVar<Layout> layout)
	{
		ctrl.AddTextColumn<FlexNode>(ColumnName.Node, null, nod => $"{nod.V.Strat}");
		ctrl.AddTextColumn<FlexNode>(ColumnName.Width, 80, nod => nod.V.Dim.X.Fmt());
		ctrl.AddTextColumn<FlexNode>(ColumnName.Height, 80, nod => nod.V.Dim.Y.Fmt());
		ctrl.AddTextColumn<FlexNode>(ColumnName.R, 100, nod =>
		{
			if (layout.V.IsNone(out var lay) || !lay.RMapFixed.TryGetValue(nod, out var r)) return string.Empty;
			return $"{r}";
		});
	}

	private static void ShowDetails(this RichTextBox rtb, NodeDetails details)
	{
		var page = details.Page;
		rtb.Clear();
		foreach (var line in page.Lines)
		{
			foreach (var txt in line)
			{
				rtb.PrintTxt(txt);
			}
			rtb.AppendText("\n");
		}

		var str = rtb.Rtf;

		var abc = 123;
	}

	private static readonly FontFamily FontFamily = new("Consolas");
	private record SizeStyle(float Size, FontStyle Style);
	private static SizeStyle ToSizeStyle(this TxtStyle style) => new(style.Size, (FontStyle)style.FontStyle);
	private static readonly Dictionary<SizeStyle, Font> fontMap = new();
	private static Font GetFont(TxtStyle style)
	{
		var sizeStyle = style.ToSizeStyle();
		return fontMap.GetOrCreate(sizeStyle, () => new Font(FontFamily, sizeStyle.Size, sizeStyle.Style));
	}


	private static void PrintTxt(this RichTextBox rtb, Txt txt)
	{
		var selStart = rtb.TextLength;
		rtb.AppendText(txt.Text);
		var selEnd = rtb.TextLength;
		rtb.Select(selStart, selEnd - selStart);
		rtb.SelectionColor = txt.Style.Color;
		rtb.SelectionFont = GetFont(txt.Style);
		rtb.SelectionLength = 0;
	}
}