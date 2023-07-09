using System.Linq.Expressions;
using System.Reactive.Linq;
using PowBasics.QueryExpr_;
using PowRxVar;
using PowWinForms;

namespace WinSpectorLib;

sealed class SpectorPrefs
{
	public bool ShowEvents { get; set; }
	public bool ShowSysCtrls { get; set; }


	public void Save() => Saving?.Invoke(null, EventArgs.Empty);

	public event EventHandler? Saving;
	static SpectorPrefs() =>
		WinFormsUtils.Tracker.Configure<SpectorPrefs>()
			.Properties(e => new
			{
				e.ShowEvents,
				e.ShowSysCtrls,
			})
			.PersistOn(nameof(Saving));
}


static class SpectorPrefsExt
{
	public static (IRwVar<T>, IDisposable) VarMake<T>(this SpectorPrefs prefs, Expression<Func<SpectorPrefs, T>> cfgExpr)
	{
		var d = new Disp();
		var (get, set) = QueryExprUtils.RetrieveGetSet(cfgExpr);
		var rwVar = Var.Make(get(prefs)).D(d);
		rwVar/*.Skip(1)*/.Subscribe(v =>
		{
			set(prefs, v);
			prefs.Save();
		}).D(d);
		return (rwVar, d);
	}
}