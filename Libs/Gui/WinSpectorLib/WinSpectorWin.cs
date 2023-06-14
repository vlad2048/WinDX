using System.Reactive.Linq;
using ControlSystem;
using DynamicData;
using PowMaybe;
using PowRxVar;
using PowWinForms;
using Structs;
using WinSpectorLib.Logic;

namespace WinSpectorLib;

sealed partial class WinSpectorWin : Form
{
	public WinSpectorWin()
	{
		InitializeComponent();

		//DoubleBuffered = true;
		//SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

		this.InitRX(d => {

			Setup.ListWindowsAndGetSelectedLayout(this, out var selLayout).D(d);
			Setup.ViewLayout(this, selLayout).D(d);
		});
	}
}
