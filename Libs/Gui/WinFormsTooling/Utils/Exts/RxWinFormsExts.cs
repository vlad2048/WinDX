﻿using PowMaybe;

namespace WinFormsTooling.Utils.Exts;

public static class RxWinFormsExts
{
	public static void Test()
	{
	}
	public static IDisposable EnableWhenSome<T>(this Control ctrl, IObservable<Maybe<T>> obsMay) => obsMay.Subscribe(may => ctrl.Enabled = may.IsSome());
	public static IDisposable EnableWhenSome<T>(this ToolStripItem ctrl, IObservable<Maybe<T>> obsMay) => obsMay.Subscribe(may => ctrl.Enabled = may.IsSome());
}