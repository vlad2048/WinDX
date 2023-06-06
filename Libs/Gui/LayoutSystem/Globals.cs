global using Node = TNod<LayoutSystem.Flex.FlexNode>;
global using DimOpt = System.Nullable<LayoutSystem.Flex.Structs.Dim>;
global using static LayoutSystem.FmtConstants;
using System.Runtime.CompilerServices;
[assembly:InternalsVisibleTo("LayoutSystem.Tests")]

namespace LayoutSystem;

static class FmtConstants
{
	public const bool truer = true;
}