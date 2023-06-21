using System.Reactive.Linq;
using ControlSystem;
using DynamicData;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using WinSpectorLib.Logic;
using WinSpectorLib.Structs;

namespace WinSpectorLib;

sealed partial class WinSpectorWin : Form
{
	public WinSpectorWin(params DemoNfo[] demos)
	{
		InitializeComponent();

		this.InitRX(d => {
			var ui = this;
			Setup.ShowDemos(ui, demos).D(d);
			Setup.ListWindowsAndGetSelectedLayout(ui, out var selLayout).D(d);
			Setup.ViewLayout(ui, selLayout).D(d);
			Setup.OpenInFlexBuilder(ui, selLayout).D(d);
		});
	}
}
