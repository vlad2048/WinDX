using System.Reactive.Linq;
using LayoutDbgApp.Utils.Exts;
using LayoutSystem.Flex;
using PowWinForms;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.LayStratsInternal;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;

namespace LayoutDbgApp.Editors;


partial class NodeEditor : UserControl
{
	private bool eventsEnabled;

	public IRwBndVar<Maybe<FlexNode>> Value { get; }
	public NodeEditor()
	{
		InitializeComponent();
		var rxVar = Var.MakeBnd(May.None<FlexNode>());
		Value = rxVar.ToRwBndVar();

		this.InitRX(d =>
		{
			rxVar.D(d);

			rxVar.EditInner(
				enableUI: on => Visible = on,
				setUI: Set,
				UI2Val: Observable.Merge(
					stratCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ => {
						IStrat strat = stratCombo.SelectedIndex switch {
							0 => new FillStrat(),
							1 => new StackStrat(Dir.Horz, Align.Start),
							2 => new WrapStrat(Dir.Horz),
							3 => new MarginStrat(),
							_ => throw new ArgumentException()
						};
						return node => node with { Strat = strat };
					}),

					stratDirCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Strat = ChangeStratMainDir(node.Strat, (Dir)stratDirCombo.SelectedIndex) }
					),

					stratAlignCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Strat = ChangeStratAlign(node.Strat, (Align)stratAlignCombo.SelectedIndex) }
					),

					horzDimEditor.Value.WhenInner.WhenSome().Where(_ => eventsEnabled).Select<DimOpt, Func<FlexNode, FlexNode>>(val =>
						node => node with { Dim = node.Dim with { X = val } }
					),

					vertDimEditor.Value.WhenInner.WhenSome().Where(_ => eventsEnabled).Select<DimOpt, Func<FlexNode, FlexNode>>(val =>
						node => node with { Dim = node.Dim with { Y = val } }
					),


					margUpNumeric.Events().ValueChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Marg = node.Marg.MgUp((int)margUpNumeric.Value) }
					),
					margRightNumeric.Events().ValueChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Marg = node.Marg.MgRight((int)margRightNumeric.Value) }
					),
					margDownNumeric.Events().ValueChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Marg = node.Marg.MgDown((int)margDownNumeric.Value) }
					),
					margLeftNumeric.Events().ValueChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Marg = node.Marg.MgLeft((int)margLeftNumeric.Value) }
					),

					margMinusBtn.Events().Click.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Marg = node.Marg.Enlarge(-10) }
					),

					margPlusBtn.Events().Click.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						node => node with { Marg = node.Marg.Enlarge(10) }
					)
				)
			).D(d);
		});
	}


	private void Set(FlexNode node)
	{
		eventsEnabled = false;
		horzDimEditor.Value.V = May.Some(node.Dim.X);
		vertDimEditor.Value.V = May.Some(node.Dim.Y);
		switch (node.Strat) {
			case FillStrat:
				stratCombo.SelectedIndex = 0;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (false, false);
				break;

			case StackStrat s:
				stratCombo.SelectedIndex = 1;
				stratDirCombo.SelectedIndex = (int)s.MainDir;
				stratAlignCombo.SelectedIndex = (int)s.Align;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (true, true);
				break;

			case WrapStrat s:
				stratCombo.SelectedIndex = 2;
				stratDirCombo.SelectedIndex = (int)s.MainDir;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (true, false);
				break;

			case MarginStrat:
				stratCombo.SelectedIndex = 3;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (false, false);
				break;
		}

		margUpNumeric.Value = node.Marg.Top;
		margRightNumeric.Value = node.Marg.Right;
		margDownNumeric.Value = node.Marg.Bottom;
		margLeftNumeric.Value = node.Marg.Left;

		eventsEnabled = true;
	}


	private static IStrat ChangeStratMainDir(IStrat strat, Dir mainDir) => strat switch {
		StackStrat s => new StackStrat(mainDir, s.Align),
		WrapStrat => new WrapStrat(mainDir),
		_ => throw new ArgumentException()
	};

	private static IStrat ChangeStratAlign(IStrat strat, Align align) => strat switch {
		StackStrat s => new StackStrat(s.MainDir, align),
		_ => throw new ArgumentException()
	};
}









/*partial class NodeEditor : UserControl
{
	private bool eventsEnabled;

	public IRwBndVar<Maybe<FlexNode>> Value { get; }


	public NodeEditor()
	{
		InitializeComponent();
		var d = this.MakeD();
		var rxVar = Var.MakeBnd(May.None<FlexNode>()).D(d);
		Value = rxVar.ToRwBndVar();

		this.Events().HandleCreated.Subscribe(_ => {

			Value.WhenOuter.Subscribe(val => {
				SetValue(val);
			}).D(d);

			stratCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Subscribe(_ => {
				var node = Value.V.Ensure();
				IStrat strat = stratCombo.SelectedIndex switch {
					0 => new FillStrat(),
					1 => new StackStrat(Dir.Horz, Align.Start),
					2 => new WrapStrat(Dir.Horz),
					_ => throw new ArgumentException()
				};
				var val = May.Some(node with { Strat = strat });
				SetValue(val);
				rxVar.SetInner(val);
			}).D(d);

			stratDirCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Subscribe(_ => {
				var node = Value.V.Ensure();
				IStrat strat = node.Strat switch {
					StackStrat s => new StackStrat((Dir)stratDirCombo.SelectedIndex, s.Align),
					WrapStrat => new WrapStrat((Dir)stratDirCombo.SelectedIndex),
					_ => throw new ArgumentException()
				};
				var val = May.Some(node with { Strat = strat });
				SetValue(val);
				rxVar.SetInner(val);
			}).D(d);

			stratAlignCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Subscribe(_ => {
				var node = Value.V.Ensure();
				IStrat strat = node.Strat switch {
					StackStrat s => new StackStrat(s.MainDir, (Align)stratAlignCombo.SelectedIndex),
					_ => throw new ArgumentException()
				};
				var val = May.Some(node with { Strat = strat });
				SetValue(val);
				rxVar.SetInner(val);
			}).D(d);

			horzDimEditor.WhenChanged.Where(_ => eventsEnabled).Subscribe(dimOpt => {
				var node = Value.V.Ensure();
				var val = May.Some(node with { Dim = node.Dim with { X = dimOpt } });
				SetValue(val);
				rxVar.SetInner(val);
			}).D(d);

			vertDimEditor.WhenChanged.Where(_ => eventsEnabled).Subscribe(dimOpt => {
				var node = Value.V.Ensure();
				var val = May.Some(node with { Dim = node.Dim with { Y = dimOpt } });
				SetValue(val);
				rxVar.SetInner(val);
			}).D(d);

		}).D(d);
	}


	private void SetValue(Maybe<FlexNode> mayNode)
	{
		eventsEnabled = false;
		if (mayNode.IsSome(out var node)) {
			Visible = true;
			horzDimEditor.SetValue(node.Dim.X);
			vertDimEditor.SetValue(node.Dim.Y);
			switch (node.Strat) {
				case FillStrat:
					stratCombo.SelectedIndex = 0;
					(stratDirCombo.Visible, stratAlignCombo.Visible) = (false, false);
					break;

				case StackStrat s:
					stratCombo.SelectedIndex = 1;
					stratDirCombo.SelectedIndex = (int)s.MainDir;
					stratAlignCombo.SelectedIndex = (int)s.Align;
					(stratDirCombo.Visible, stratAlignCombo.Visible) = (true, true);
					break;

				case WrapStrat s:
					stratCombo.SelectedIndex = 2;
					stratDirCombo.SelectedIndex = (int)s.MainDir;
					(stratDirCombo.Visible, stratAlignCombo.Visible) = (true, false);
					break;
			}
		} else {
			Visible = false;
		}
		eventsEnabled = true;
	}
}
*/