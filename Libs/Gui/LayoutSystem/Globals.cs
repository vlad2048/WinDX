﻿global using static LayoutSystem.FmtConstants;

global using Node = TNod<LayoutSystem.Flex.FlexNode>;
global using Dim = System.Nullable<LayoutSystem.Flex.Structs.FDim>;
global using D = LayoutSystem.Flex.Structs.DimMaker;
global using Vec = LayoutSystem.Flex.Structs.DimVecMaker;

using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("LayoutSystem.Tests")]

namespace LayoutSystem;

static class FmtConstants
{
	public const bool truer = true;
}