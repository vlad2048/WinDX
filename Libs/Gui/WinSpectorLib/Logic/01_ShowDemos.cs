using System.Reactive.Disposables;
using BrightIdeasSoftware;
using PowMaybe;
using PowRxVar;
using WinSpectorLib.Structs;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ShowDemos(WinSpectorWin ui, DemoNfo[] demos)
	{
		if (demos.Length == 0)
		{
			ui.Controls.Remove(ui.demosGroupBox);
			return Disposable.Empty;
		}
		var d = new Disp();
		var ctrl = ui.demosList;

		SetupColumns(ctrl);

		ctrl.AddObjects(demos);

		ctrl.Events().DoubleClick.Subscribe(_ =>
		{
			var mayDemo = ctrl.GetNodeUnderMouse<DemoNfo>();
			if (mayDemo.IsNone(out var demo)) return;
			demo.Run();
		}).D(d);


		return d;
	}

	private static void SetupColumns(ObjectListView ctrl)
	{
		ctrl.ShowGroups = false;
		ctrl.FullRowSelect = true;
		ctrl.MultiSelect = false;
		ctrl.UseCellFormatEvents = false;
		ctrl.UseOverlays = false;

		ctrl.Columns.Add(new OLVColumn("Name", "Name")
		{
			FillsFreeSpace = true,
		});
	}


	private static Maybe<T> GetNodeUnderMouse<T>(this ObjectListView ctrl)
	{
		if (ctrl.MouseMoveHitTest == null) return May.None<T>();
		var obj = ctrl.MouseMoveHitTest.RowObject;
		if (obj == null) return May.None<T>();
		if (obj is not T node) return May.None<T>();
		return May.Some(node);
	}
}