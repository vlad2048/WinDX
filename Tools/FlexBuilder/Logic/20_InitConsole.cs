using System.Reactive.Disposables;
using FlexBuilder.Structs;
using PowBasics.Geom;
using PowRxVar;
using WinAPI.Utils;

namespace FlexBuilder.Logic;

static partial class Setup
{
	public const bool EnableConsole = false;

	public static IDisposable InitConsole(UserPrefs userPrefs)
	{
		var d = new Disp();
		if (!EnableConsole) return d;

		ConUtils.Init(userPrefs.ConR.ToR());

		Disposable.Create(() =>
		{
			userPrefs.ConR = ConUtils.GetR().ToTuple();
			userPrefs.Save();
		}).D(d);

		return d;
	}
}


file static class InitConsoleUtils
{
	public static R ToR(this (int, int, int, int) t) => new(t.Item1, t.Item2, t.Item3, t.Item4);
	public static (int, int, int, int) ToTuple(this R r) => new(r.X, r.Y, r.Width, r.Height);
}