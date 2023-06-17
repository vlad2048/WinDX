using System.Reactive.Linq;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms;

namespace FlexBuilder.Editors;


sealed partial class NodeEditor : UserControl
{
	private bool eventsEnabled;

	public IRwBndVar<Maybe<FlexNode>> Value { get; }
	public NodeEditor()
	{
		InitializeComponent();

		var rxVar = Var.MakeBnd(May.None<FlexNode>()).D(this);
		Value = rxVar.ToRwBndVar();

		this.InitRX(d => {
			rxVar.EditInner(
				enableUI: on => Visible = on,
				setUI: Set,
				UI2Val:
				Obs.Merge(
						stratCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ =>
						{
							IStrat strat = stratCombo.SelectedIndex switch
							{
								0 => new FillStrat(new ScrollSpec(BoolVec.False)),
								1 => new StackStrat(Dir.Horz, Align.Start),
								2 => new WrapStrat(Dir.Horz),
								_ => throw new ArgumentException()
							};
							return node => node with { Strat = strat };
						}),

						// @formatter:off
						horzDimEditor.Value.WhenInner.WhenSome()			.Select<Dim,		Func<FlexNode, FlexNode>>(val =>	node => node with { Dim = node.Dim with { X = val } }),
						vertDimEditor.Value.WhenInner.WhenSome()			.Select<Dim,		Func<FlexNode, FlexNode>>(val =>	node => node with { Dim = node.Dim with { Y = val } }),
						stratDirCombo		.Events().SelectedIndexChanged	.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratMainDir(node.Strat, (Dir)stratDirCombo.SelectedIndex) }),
						stratAlignCombo		.Events().SelectedIndexChanged	.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratAlign(node.Strat, (Align)stratAlignCombo.SelectedIndex) }),
						margUpNumeric		.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgUp((int)margUpNumeric.Value) }),
						margRightNumeric	.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgRight((int)margRightNumeric.Value) }),
						margDownNumeric		.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgDown((int)margDownNumeric.Value) }),
						margLeftNumeric		.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgLeft((int)margLeftNumeric.Value) }),
						margMinusBtn		.Events().Click					.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.Enlarge(-10) }),
						margPlusBtn			.Events().Click					.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.Enlarge(10) }),

						specTypeCombo		.Events().SelectedIndexChanged	.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratFillSpec(node.Strat, specTypeCombo.SelectedIndex) }),
						specScrollXCheckBox	.Events().CheckedChanged		.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratFillScrollX(node.Strat, specScrollXCheckBox.Checked) }),
						specScrollYCheckBox	.Events().CheckedChanged		.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratFillScrollY(node.Strat, specScrollYCheckBox.Checked) })
						// @formatter:on
					)
					.Where(_ => eventsEnabled)
			).D(d);
		});
	}


	private void Set(FlexNode node)
	{
		eventsEnabled = false;
		horzDimEditor.Value.V = May.Some(node.Dim.X);
		vertDimEditor.Value.V = May.Some(node.Dim.Y);
		switch (node.Strat) {
			case FillStrat s:
				stratCombo.SelectedIndex = 0;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (false, false);
				specTypeCombo.SelectedIndex = s.Spec switch
				{
					ScrollSpec => 0,
					PopSpec => 1,
					_ => throw new ArgumentException()
				};
				specGroupBox.Visible = true;
				switch (s.Spec)
				{
					case ScrollSpec {Enabled: var scrollEnabled}:
						(specScrollXCheckBox.Checked, specScrollYCheckBox.Checked) = (scrollEnabled.X, scrollEnabled.Y);
						(specScrollXCheckBox.Visible, specScrollYCheckBox.Visible) = (true, true);
						break;
					case PopSpec:
						(specScrollXCheckBox.Visible, specScrollYCheckBox.Visible) = (false, false);
						break;
				}
				break;

			case StackStrat s:
				stratCombo.SelectedIndex = 1;
				stratDirCombo.SelectedIndex = (int)s.MainDir;
				stratAlignCombo.SelectedIndex = (int)s.Align;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (true, true);
				specGroupBox.Visible = false;
				break;

			case WrapStrat s:
				stratCombo.SelectedIndex = 2;
				stratDirCombo.SelectedIndex = (int)s.MainDir;
				(stratDirCombo.Visible, stratAlignCombo.Visible) = (true, false);
				specGroupBox.Visible = false;
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

	private static IStrat ChangeStratFillSpec(IStrat strat, int specTypeIdx) => strat switch
	{
		FillStrat => new FillStrat(specTypeIdx switch
		{
			0 => new ScrollSpec(new BoolVec(false, false)),
			1 => new PopSpec(),
			_ => throw new ArgumentException()
		}),
		_ => throw new ArgumentException()
	};


	private static IStrat ChangeStratFillScrollX(IStrat strat, bool enabled) => strat switch {
		FillStrat { Spec: ScrollSpec s } => new FillStrat(new ScrollSpec(s.Enabled with { X = enabled })),
		_ => throw new ArgumentException()
	};
	private static IStrat ChangeStratFillScrollY(IStrat strat, bool enabled) => strat switch {
		FillStrat { Spec: ScrollSpec s } => new FillStrat(new ScrollSpec(s.Enabled with { Y = enabled })),
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