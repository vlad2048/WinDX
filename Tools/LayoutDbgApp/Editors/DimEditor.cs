using System.ComponentModel;
using System.Reactive.Linq;
using LayoutDbgApp.Utils.Exts;
using LayoutSystem.Flex.Structs;
using PowBasics.Geom;
using PowMaybe;
using PowRxVar;
using PowWinForms;

namespace LayoutDbgApp.Editors;

partial class DimEditor : UserControl
{
	private Dir dir;
	private bool eventsEnabled;

	public static readonly Random rnd = new();

	[Browsable(true)]
	public Dir Dir {
		get => dir;
		set { dir = value; dirLabel.Text = Dir == Dir.Horz ? "X" : "Y"; }
	}


	public IRwBndVar<Maybe<DimOpt>> Value { get; }


	public DimEditor()
	{
		InitializeComponent();

		var rxVar = Var.MakeBnd(May.None<DimOpt>());
		Value = rxVar.ToRwBndVar();

		this.InitRX(d =>
		{
			rxVar.D(d);

			rxVar.EditInner(
				enableUI: on => typCombo.Visible = minNumeric.Visible = maxNumeric.Visible = on,
				setUI: Set,
				UI2Val: Observable.Merge(
					typCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select(_ => (DType)typCombo.SelectedIndex)
						.Select<DType, DimOpt>(typ => {
							var rndMin = rnd.Next(30, 160);
							var rndMax = rnd.Next(rndMin + 1, 200);
							return typ switch {
								DType.Fix => D.Fix(rnd.Next(30, 160)),
								DType.Flt => D.Flt(rndMin, rndMax),
								DType.Fil => D.Fil,
								DType.Fit => null
							};
						}),
					Observable.Merge(
							minNumeric.Events().ValueChanged,
							maxNumeric.Events().ValueChanged
						).Where(_ => eventsEnabled)
						.Select(_ => {
							var typ = (DType)typCombo.SelectedIndex;
							var min = (int)minNumeric.Value;
							var max = (int)maxNumeric.Value;
							return typ switch {
								DType.Fix => D.Fix(min),
								DType.Flt => D.Flt(min, max),
								DType.Fil => D.Fil,
								DType.Fit => (DimOpt)null
							};
						})
				)

			).D(d);
		});
	}

	private void Set(DimOpt v)
	{
		eventsEnabled = false;
		var typ = v.Typ();
		typCombo.SelectedIndex = (int)typ;
		switch (typ) {
			case DType.Fix: {
				var dim = v!.Value;
				minNumeric.Value = dim.Min;
				(minNumeric.Visible, maxNumeric.Visible) = (true, false);
				break;
			}

			case DType.Flt: {
				var dim = v!.Value;
				(minNumeric.Value, maxNumeric.Value) = (dim.Min, dim.Max);
				(minNumeric.Visible, maxNumeric.Visible) = (true, true);
				break;
			}

			case DType.Fil: {
				(minNumeric.Visible, maxNumeric.Visible) = (false, false);
				break;
			}

			case DType.Fit: {
				(minNumeric.Visible, maxNumeric.Visible) = (false, false);
				break;
			}
		}
		eventsEnabled = true;
	}
}
