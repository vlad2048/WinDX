﻿/*
using System.Drawing;
using System.Reactive.Disposables;
using Demos.Structs;
using PowBasics.Geom;
using PowRxVar;
using WinAPI.Utils;

namespace Demos.Logic;

static class Setup
{
	public static IDisposable InitConsole(UserPrefs userPrefs)
	{
		var d = new Disp();

		ConUtils.Init(userPrefs.ConR.ToR());

		ConUtils.SetColor(Color.DodgerBlue);
		Console.WriteLine("Hello");
		ConUtils.SetColor(Color.PaleGreen);
		Console.WriteLine("PaleGreen");

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
*/
