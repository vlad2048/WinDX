using ControlSystem;
using PowMaybe;
using PowRxVar;
using PowWinForms.ListBoxSourceListViewing;
using System.Reactive.Linq;
using ControlSystem.Structs;
using DynamicData;
using PowBasics.CollectionsExt;
using PowWinForms.Utils;
using WinFormsTooling.Utils.Exts;
using WinSpectorLib.Utils;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ListWindowsAndGetSelectedLayout(WinSpectorWin ui, out IRoMayVar<MixLayout> selLayout)
	{
		var d = new Disp();
		ListBoxSourceListViewer.View(out var selWin, G.WinMan.Wins, ui.winList).D(d);
		selLayout = GetSelWinLayout(selWin).D(d);
		ui.redrawWindowBtn.EnableWhenSome(selWin).D(d);
		ui.redrawWindowBtn.Events().Click.Subscribe(_ => selWin.V.Ensure().Invalidate()).D(d);
		return d;
	}

	private static (IRoMayVar<MixLayout>, IDisposable) GetSelWinLayout(IRoMayVar<Win> selWin)
	{
		var d = new Disp();
		static Func<MixLayout, bool> MatchesSelWin(Maybe<Win> mayWin) => layout => mayWin.IsSome(out var win) && layout.Win == win;
		var layoutEvt = G.WinMan.Win2Layout.Filter(selWin.Select(MatchesSelWin));
		var layout = VarMayNoCheck.Make<MixLayout>().D(d);

		layoutEvt.Subscribe(cs =>
		{
			var csAdds = cs.WhereToArray(e => e.Reason is ChangeReason.Add or ChangeReason.Update);
			var csDels = cs.WhereToArray(e => e.Reason is ChangeReason.Remove);
			csDels.ForEach(_ => layout.V = May.None<MixLayout>());
			csAdds.ForEach(c => layout.V = May.Some(c.Current));
		}).D(d);

		return (layout, d);
	}
}