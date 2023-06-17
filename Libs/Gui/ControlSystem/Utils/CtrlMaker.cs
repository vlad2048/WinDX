using ControlSystem.Structs;
using PowBasics.CollectionsExt;
using PowRxVar;

namespace ControlSystem.Utils;

public static class CtrlMaker
{
	public static Ctrl Make(int nodeCount, Action<NodeState[], RenderArgs> paint)
	{
		var nodeStates = Enumerable.Range(0, nodeCount).SelectToArray(_ => new NodeState());
		return new FunCtrl(nodeStates, paint);
	}

	private class FunCtrl : Ctrl
	{
		public FunCtrl(NodeState[] nodeStates, Action<NodeState[], RenderArgs> paint)
		{
			foreach (var nodeState in nodeStates) nodeState.D(D);
			WhenRender.Subscribe(r =>
			{
				paint(nodeStates, r);
			}).D(D);
		}
	}


	public static void Deconstruct<T>(this T[] arr, out T a)
	{
		a = arr[0];
	}
	public static void Deconstruct<T>(this T[] arr, out T a, out T b)
	{
		a = arr[0];
		b = arr[1];
	}
	public static void Deconstruct<T>(this T[] arr, out T a, out T b, out T c)
	{
		a = arr[0];
		b = arr[1];
		c = arr[2];
	}
	public static void Deconstruct<T>(this T[] arr, out T a, out T b, out T c, out T d)
	{
		a = arr[0];
		b = arr[1];
		c = arr[2];
		d = arr[3];
	}
	public static void Deconstruct<T>(this T[] arr, out T a, out T b, out T c, out T d, out T e)
	{
		a = arr[0];
		b = arr[1];
		c = arr[2];
		d = arr[3];
		e = arr[4];
	}
}