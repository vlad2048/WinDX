global using static LayoutSystem.FmtConstants;

global using Dim = System.Nullable<LayoutSystem.Flex.Structs.FDim>;
global using D = LayoutSystem.Flex.Structs.DimMaker;
global using Vec = LayoutSystem.Flex.Structs.DimVecMaker;
global using Node = TNod<LayoutSystem.Flex.FlexNode>;

using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("1_LayoutSystem.Tests")]
[assembly:InternalsVisibleTo("3_WinSpectorLib")]
[assembly:InternalsVisibleTo("FlexBuilder")]

namespace LayoutSystem;

static class FmtConstants
{
	public const bool truer = true;
}