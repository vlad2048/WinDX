﻿using SysWinLib.Defaults;
using SysWinLib.Structs;

// ReSharper disable once CheckNamespace
namespace SysWinLib;


public class SysWinOpt
{
	public string WinClass { get; set; } = WinClasses.MainWindow;
	public CreateWindowParams CreateWindowParams = new();
	public INCStrat NCStrat { get; set; } = new CustomNCStrat();

	private SysWinOpt()
	{
	}

	internal static SysWinOpt Make(Action<SysWinOpt>? optFun)
	{
		var opt = new SysWinOpt();
		optFun?.Invoke(opt);
		return opt;
	}
}