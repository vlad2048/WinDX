using System.ComponentModel;
using PowRxVar;
using PowWinForms;

namespace LayoutDbgApp;

sealed partial class MainWin : Form
{
	public MainWin()
	{
		InitializeComponent();
		if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

		this.InitRX(d => MainWinSetup.SetupMainWin(this).D(d));
	}
}
