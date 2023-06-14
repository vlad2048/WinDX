﻿global using Dim = System.Nullable<LayoutSystem.Flex.Structs.FDim>;
global using D = LayoutSystem.Flex.Structs.DimMaker;
global using Vec = LayoutSystem.Flex.Structs.DimVecMaker;
global using Obs = System.Reactive.Linq.Observable;
global using Node = TNod<LayoutSystem.Flex.FlexNode>;
global using MixNode = TNod<ControlSystem.Structs.IMixNode>;
using System.Runtime.CompilerServices;
using ControlSystem.Singletons.WinMan_;
using PowRxVar;

[assembly:InternalsVisibleTo("3_WinSpectorLib")]
[assembly:InternalsVisibleTo("ControlSystem.Tests")]

namespace ControlSystem;

static class G
{
	public static WinMan WinMan { get; private set; } = null!;

	private static void Init()
	{
		serD.Value = null;
		var d = serD.Value = new Disp();

		WinMan = new WinMan().D(d);
	}

	public static void ReinitForTests() => Init();

	private static readonly SerialDisp<Disp> serD = new SerialDisp<Disp>().DisposeOnProgramExit();
	static G() => Init();
}