using System.Reactive.Linq;
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
		var showEvents = Var.Make(false).D(this);

		this.InitRX(d => {
			var ui = this;
			Setup.ShowDemos(ui, demos).D(d);
			Setup.ListWindowsAndGetSelectedLayout(ui, out var selLayout).D(d);
			Setup.ViewLayout(ui, selLayout, showEvents).D(d);
			Setup.OpenInFlexBuilder(ui, selLayout).D(d);

			eventDisplayer.SetShowEvents(showEvents, selLayout);


			showEvents.Skip(1).Subscribe(show => {
				SuspendLayout();
				layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
				eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;

				const int delta = 1125 - 808;
				switch (show)
				{
					case false:
						Width -= delta;
						break;
					case true:
						Width += delta;
						break;
				}

				eventsGroupBox.Left = layoutGroupBox.Right + 6;
				layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
				ResumeLayout();
			}).D(d);
		});
	}
}
