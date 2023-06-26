global using Dim = System.Nullable<LayoutSystem.Flex.Structs.FDim>;
global using D = LayoutSystem.Flex.Structs.DimMaker;
global using Vec = LayoutSystem.Flex.Structs.DimVecMaker;
global using static SimpleDemo.Logger;

namespace SimpleDemo;

static class Logger
{
	public static void L(string s)
	{
		Console.WriteLine(s);
	}
}