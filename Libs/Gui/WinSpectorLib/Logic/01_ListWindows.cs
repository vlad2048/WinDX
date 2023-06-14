using ControlSystem;
using PowMaybe;
using PowRxVar;
using PowWinForms.ListBoxSourceListViewing;
using Structs;
using System.Reactive.Linq;
using DynamicData;
using PowBasics.CollectionsExt;

namespace WinSpectorLib.Logic;

static partial class Setup
{
	public static IDisposable ListWindowsAndGetSelectedLayout(WinSpectorWin ui, out IRoMayVar<MixLayout> selLayout)
	{
		var d = new Disp();
		ListBoxSourceListViewer.View(out var selWin, G.WinMan.Wins, ui.winList).D(d);
		selLayout = GetSelWinLayout(selWin).D(d);
		return d;
	}

	private static (IRoMayVar<MixLayout>, IDisposable) GetSelWinLayout(IRoMayVar<Win> selWin)
	{
		var d = new Disp();
		static Func<MixLayout, bool> MatchesSelWin(Maybe<Win> mayWin) => layout => mayWin.IsSome(out var win) && layout.Win == win;
		var layoutEvt = G.WinMan.Win2Layout.Filter(selWin.Select(MatchesSelWin));
		var layout = VarMay.Make<MixLayout>().D(d);

		layoutEvt.Subscribe(cs =>
		{
			var csAdds = cs.WhereToArray(e => e.Reason is ChangeReason.Add or ChangeReason.Update);
			var csDels = cs.WhereToArray(e => e.Reason is ChangeReason.Remove);

			csDels.ForEach(_ => layout.V = May.None<MixLayout>());
			csAdds.ForEach(c => layout.V = May.Some(c.Current));

			/*foreach (var c in cs)
			{
				switch (c.Reason)
				{
					case ChangeReason.Add:
					case ChangeReason.Update:
						layout.V = May.Some(c.Current);
						break;
					case ChangeReason.Remove:
						layout.V = May.None<MixLayout>();
						break;
				}
			}*/
		}).D(d);
		return (layout, d);
	}
}