using System.Reactive.Linq;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex;
using LayoutSystem.Flex.LayStrats;
using LayoutSystem.Utils.Exts;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms;

namespace FlexBuilder.Editors;


sealed partial class NodeEditor : UserControl
{
	private bool eventsEnabled;

	public IRwMayBndVar<FlexNode> Value { get; }
	public NodeEditor()
	{
		InitializeComponent();

		var rxVar = VarMay.MakeBnd<FlexNode>().D(this);
		Value = rxVar.ToRwMayBndVar();

		this.InitRX(d => {
			rxVar.EditInner(
				enableUI: on => Visible = on,
				setUI: Set,
				UI2Val:
				Obs.Merge(
						stratCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select<EventArgs, Func<FlexNode, FlexNode>>(_ => {
							IStrat strat = stratCombo.SelectedIndex switch {
								0 => new FillStrat(),
								1 => new StackStrat(Dir.Horz, Align.Start),
								2 => new WrapStrat(Dir.Horz),
								_ => throw new ArgumentException()
							};
							return node => node with { Strat = strat };
						}),

						// @formatter:off

						horzDimEditor.Value.WhenInner.WhenSome()			.Select<Dim,		Func<FlexNode, FlexNode>>(val =>	node => node with { Dim = node.Dim with { X = val }																		}),
						vertDimEditor.Value.WhenInner.WhenSome()			.Select<Dim,		Func<FlexNode, FlexNode>>(val =>	node => node with { Dim = node.Dim with { Y = val }																		}),

						flagsPopCheckBox	.Events().CheckedChanged		.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Flags = node.Flags with { Pop = flagsPopCheckBox.Checked }											}),
						flagsScrollXCheckBox.Events().CheckedChanged		.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Flags = node.Flags with { Scroll = node.Flags.Scroll with { X = flagsScrollXCheckBox.Checked } }	}),
						flagsScrollYCheckBox.Events().CheckedChanged		.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Flags = node.Flags with { Scroll = node.Flags.Scroll with { Y = flagsScrollYCheckBox.Checked } }	}),

						stratDirCombo		.Events().SelectedIndexChanged	.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratMainDir(node.Strat, (Dir)stratDirCombo.SelectedIndex)							}),
						stratAlignCombo		.Events().SelectedIndexChanged	.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Strat = ChangeStratAlign(node.Strat, (Align)stratAlignCombo.SelectedIndex)							}),

						margUpNumeric		.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgUp((int)margUpNumeric.Value)														}),
						margRightNumeric	.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgRight((int)margRightNumeric.Value)												}),
						margDownNumeric		.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgDown((int)margDownNumeric.Value)													}),
						margLeftNumeric		.Events().ValueChanged			.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.MgLeft((int)margLeftNumeric.Value)													}),
						margMinusBtn		.Events().Click					.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.Enlarge(-10)																		}),
						margPlusBtn			.Events().Click					.Select<EventArgs,	Func<FlexNode, FlexNode>>(_ =>		node => node with { Marg = node.Marg.Enlarge(10)																		})

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

		flagsPopCheckBox.Checked = node.Flags.Pop;
		flagsScrollXCheckBox.Checked = node.Flags.Scroll.X;
		flagsScrollYCheckBox.Checked = node.Flags.Scroll.Y;

		switch (node.Strat) {
			case FillStrat s:
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
