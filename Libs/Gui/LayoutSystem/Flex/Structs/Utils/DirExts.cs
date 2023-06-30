using System.Diagnostics;
using Dr = PowBasics.Geom.Dir;

// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public static class DirExts
{
	[DebuggerStepThrough]
	public static Dim Dir(this DimVec vec, Dr dir) => dir switch
	{
		Dr.Horz => vec.X,
		Dr.Vert => vec.Y,
		_ => throw new ArgumentException()
	};

	[DebuggerStepThrough]
	public static FDim Dir(this FDimVec sz, Dr dir) => dir switch
	{
		Dr.Horz => sz.X,
		Dr.Vert => sz.Y,
		_ => throw new ArgumentException()
	};

	[DebuggerStepThrough]
	public static bool Dir(this BoolVec vec, Dr dir) => dir switch
	{
		Dr.Horz => vec.X,
		Dr.Vert => vec.Y,
	};

	[DebuggerStepThrough]
	public static int? Dir(this FreeSz freeSz, Dr dir) => dir switch
	{
		Dr.Horz => freeSz.X,
		Dr.Vert => freeSz.Y,
	};

	[DebuggerStepThrough]
	public static T Dir<T>(this (T, T) t, Dr dir) => dir switch
	{
		Dr.Horz => t.Item1,
		Dr.Vert => t.Item2,
	};
}