﻿using System.Reactive.Linq;
using FlexBuilder.Structs;
using FlexBuilder.Utils.Exts;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;

namespace FlexBuilder.SetupLogic;

static partial class Setup
{
	public static IDisposable EditLayoutDefWinSize(MainWin ui, IFullRwMayBndVar<LayoutDef> layout)
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


	public static IDisposable DisplayCalcWinSize(MainWin ui, IRoMayVar<Layout> layout) =>
		layout.Subscribe(may => ui.calcWinSzLabel.Text = may.IsSome(out var lay) switch
		{
			true => $"{lay.TotalSz}",
			false => "_"
		});
}