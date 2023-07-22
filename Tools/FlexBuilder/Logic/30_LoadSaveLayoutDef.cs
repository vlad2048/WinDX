using System.Reactive.Linq;
using FlexBuilder.Structs;
using LayoutSystem.StructsShared;
using LayoutSystem.Utils.JsonUtils;
using PowMaybe;
using PowMaybeErr;
using PowRxVar;
using PowWinForms.Utils;

namespace FlexBuilder.Logic;

static partial class Setup
{
	public static IDisposable LoadSaveLayoutDef(
		MainWin ui,
		IFullRwMayBndVar<LayoutDef> layout,
		UserPrefs userPrefs,
		Maybe<StartupFile> mayStartupFile
	)
	{
		var d = new Disp();

		var openedFilename = Var.Make(May.None<string>()).D(d);
		var isModified = Var.Make(false).D(d);


		void LoadFile(string file)
		{
			var mayLayoutVal = Jsoner.Load<LayoutDef>(file);
			if (mayLayoutVal.IsNone(out var layoutVal, out var err))
			{
				MessageBox.Show(err, "Error loading layout", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			layout.V = May.Some(layoutVal);
			openedFilename.V = May.Some(file);
			isModified.V = false;

			userPrefs.LastFolder = Path.GetDirectoryName(file);
			userPrefs.OpenFile = file;
			userPrefs.Save();
		}

		void SaveCurrent()
		{
			var filename = openedFilename.V.Ensure();
			var layoutVal = layout.V.Ensure();
			Jsoner.Save(filename, layoutVal);
			isModified.V = false;
		}


		void SaveAs(string file)
		{
			var layoutVal = layout.V.Ensure();
			Jsoner.Save(file, layoutVal);
			openedFilename.V = May.Some(file);
			isModified.V = false;

			userPrefs.LastFolder = Path.GetDirectoryName(file);
			userPrefs.OpenFile = file;
			userPrefs.Save();
		}

		if (mayStartupFile.IsSome(out var startupFile))
		{
			LoadFile(startupFile.Filename);
			if (startupFile.DeleteAfterOpen)
				File.Delete(startupFile.Filename);
		}
		else
		{
			if (userPrefs.OpenFile != null && File.Exists(userPrefs.OpenFile))
				LoadFile(userPrefs.OpenFile);
		}


		//ui.statusStrip.AddVar("Layout", layout.Select(e => e.IsSome())).D(d);
		//ui.statusStrip.AddVar("Modified", isModified).D(d);
		//ui.statusStrip.AddVar("Last", lastOpenFilename).D(d);

		layout.WhenInner.Subscribe(_ => isModified.V = true).D(d);

		Obs.Merge(
			layout.ToUnit(),
			openedFilename.ToUnit(),
			isModified.ToUnit()
		)
			.ObserveOnWinFormsUIThread()
			.Subscribe(_ =>
			{
				ui.fileSaveItem.Enabled = layout.V.IsSome() && isModified.V;
				ui.fileSaveAsItem.Enabled = layout.V.IsSome();
				var titleSuffix = (layout.V.IsSome(), openedFilename.V.IsSome(out var name), isModified.V) switch
				{
					(false, _, _) => string.Empty,
					(true, false, false) => " - [Untitled]",
					(true, false, true) => " - [Untitled *]",
					(true, true, false) => $" - [{Path.GetFileName(name)}]",
					(true, true, true) => $" - [{Path.GetFileName(name)} *]",
				};
				ui.Text = $"Flex Builder{titleSuffix}";
			}).D(d);

		ui.fileNewItem.Events().Click.Subscribe(_ =>
		{
			layout.V = May.Some(LayoutDef.Default);
			openedFilename.V = May.None<string>();
			isModified.V = true;
		}).D(d);

		ui.fileOpenItem.Events().Click.Subscribe(_ =>
		{
			var dlg = new OpenFileDialog
			{
				DefaultExt = ".json",
				Filter = "Layout Files|*.json",
				InitialDirectory = userPrefs.LastFolder,
			};
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				LoadFile(dlg.FileName);
			}
		}).D(d);

		ui.fileSaveItem.Events().Click.Where(_ => openedFilename.V.IsSome()).Subscribe(_ =>
		{
			SaveCurrent();
		}).D(d);

		Obs.Merge(
			ui.fileSaveItem.Events().Click.Where(_ => openedFilename.V.IsNone()),
			ui.fileSaveAsItem.Events().Click
		)
			.Subscribe(_ =>
			{
				var dlg = new SaveFileDialog
				{
					DefaultExt = ".json",
					Filter = "Layout Files|*.json",
					InitialDirectory = userPrefs.LastFolder,
				};
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					SaveAs(dlg.FileName);
				}
			}).D(d);

		ui.fileExitItem.Events().Click.Subscribe(_ => ui.Close()).D(d);

		return d;
	}
}