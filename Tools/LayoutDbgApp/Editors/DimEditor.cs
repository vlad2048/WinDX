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


	public IRwBndVar<Maybe<Dim>> Value { get; }


	public DimEditor()
	{
		InitializeComponent();

		var rxVar = Var.MakeBnd(May.None<Dim>());
		Value = rxVar.ToRwBndVar();

		this.InitRX(d =>
		{
			rxVar.D(d);

			rxVar.EditInner(
				enableUI: on => typCombo.Visible = minNumeric.Visible = maxNumeric.Visible = on,
				setUI: Set,
				UI2Val: Observable.Merge(
					typCombo.Events().SelectedIndexChanged.Where(_ => eventsEnabled).Select(_ => (DimType)typCombo.SelectedIndex)
						.Select<DimType, Dim>(typ => {
							var rndMin = rnd.Next(30, 160);
							var rndMax = rnd.Next(rndMin + 1, 200);
							return typ switch {
								DimType.Fix => D.Fix(rnd.Next(30, 160)),
								DimType.Flt => D.Flt(rndMin, rndMax),
								DimType.Fil => D.Fil,
								DimType.Fit => null
							};
						}),
					Observable.Merge(
							minNumeric.Events().ValueChanged,
							maxNumeric.Events().ValueChanged
						).Where(_ => eventsEnabled)
						.Select(_ => {
							var typ = (DimType)typCombo.SelectedIndex;
							var min = (int)minNumeric.Value;
							var max = (int)maxNumeric.Value;
							return typ switch {
								DimType.Fix => D.Fix(min),
								DimType.Flt => D.Flt(min, max),
								DimType.Fil => D.Fil,
								DimType.Fit => (Dim)null
							};
						})
				)

			).D(d);
		});
	}

	private void Set(Dim v)
	{
		eventsEnabled = false;
		var typ = v.Typ();
		typCombo.SelectedIndex = (int)typ;
		switch (typ) {
			case DimType.Fix: {
				var dim = v!.Value;
				minNumeric.Value = dim.Min;
				(minNumeric.Visible, maxNumeric.Visible) = (true, false);
				break;
			}

			case DimType.Flt: {
				var dim = v!.Value;
				(minNumeric.Value, maxNumeric.Value) = (dim.Min, dim.Max);
				(minNumeric.Visible, maxNumeric.Visible) = (true, true);
				break;
			}

			case DimType.Fil: {
				(minNumeric.Visible, maxNumeric.Visible) = (false, false);
				break;
			}

			case DimType.Fit: {
				(minNumeric.Visible, maxNumeric.Visible) = (false, false);
				break;
			}
		}
		eventsEnabled = true;
	}
}
