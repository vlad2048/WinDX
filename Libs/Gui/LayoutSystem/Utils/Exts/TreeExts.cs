using PowTrees.Algorithms;

namespace LayoutSystem.Utils.Exts;

static class TreeExts
{
	//public static TNod<V> Map2Tree<K, V>(this IReadOnlyDictionary<TNod<K>, V> map, TNod<K> tree) =>
	//	tree.MapN(n => map[n]);


	public static TNod<T> Verify<T>(this TNod<T> root)
	{
		void Err() => throw new ArgumentException();
		void Rec(TNod<T> node, TNod<T>? parent)
		{
			if (node.Parent != parent) Err();
			foreach (var child in node.Children)
				Rec(child, node);
		}
		Rec(root, null);
		return root;
	}
}