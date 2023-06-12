namespace ControlSystem.Utils;

static class TreeExts
{
	public static TNod<U> OfTypeTree<T, U>(this TNod<T> root) where U : T
	{
		if (root.V is U) throw new ArgumentException();
		if (root.Children.Count != 1) throw new ArgumentException();
		if (root.Children[0].V is not U rootUVal) throw new ArgumentException();

		IEnumerable<TNod<U>> FilterChildren(TNod<T> node)
		{
			var list = new List<TNod<U>>();
			foreach (var child in node.Children)
			{
				var childFilteredKids = FilterChildren(child);
				if (child.V is U childU)
					list.Add(Nod.Make(childU, childFilteredKids));
				else
					list.AddRange(childFilteredKids);
			}
			return list;
		}

		return Nod.Make(
			rootUVal,
			FilterChildren(root.Children[0])
		);
	}

	/*public static TNod<V> OfTypeTree<T, U, V>(this TNod<T> root, Func<U, V> mapFun) where U : T
	{
		if (root.V is U) throw new ArgumentException();
		if (root.Children.Count != 1) throw new ArgumentException();
		if (root.Children[0].V is not U rootUVal) throw new ArgumentException();

		IEnumerable<TNod<U>> FilterChildren(TNod<T> node)
		{
			var list = new List<TNod<U>>();
			foreach (var child in node.Children)
			{
				var childFilteredKids = FilterChildren(child);
				if (child.V is U childU)
					list.Add(Nod.Make(childU, childFilteredKids));
				else
					list.AddRange(childFilteredKids);
			}
			return list;
		}

		var rootU = Nod.Make(
			rootUVal,
			FilterChildren(root.Children[0])
		);

		return rootU.Map(mapFun);
	}*/
}