using PowBasics.CollectionsExt;

namespace ControlSystem.Utils;

static class TreeExts
{
	// - removes all nodes matching predicate (and all their descendents)
	// - flatten and return all the remaining nodes
	// - the predicate does not apply to the root - the root is always returned
	public static T[] GetNodesUntil<T>(this TNod<T> root, Func<T, bool> predicate)
	{
		var list = new List<T>();
		void Rec(TNod<T> nod)
		{
			if (nod != root && predicate(nod.V)) return;
			list.Add(nod.V);
			foreach (var kid in nod.Children)
				Rec(kid);
		}
		Rec(root);
		return list.ToArray();
	}


	public static TNod<T> FilterAfterRoot<T>(this TNod<T> root, Func<T, bool> predicate)
	{
		TNod<T> Rec(TNod<T> n) => Nod.Make(
			n.V,
			n.Children.WhereNot(e => predicate(e.V))
		);
		return Rec(root);
	}


	/// <summary>
	/// <![CDATA[
	///    ┌──n1
	///    │
	///C1──┤
	///    │      ┌──C2
	///    └──n2──┤      ┌──n4──n5──n6
	///           └──n3──┤
	///                  └──C3──n7──C4──n8
	///
	/// predicate = node == C*
	/// 
	/// if node = n3 returns C1──n2──n3 along with a pointer to n3 in the new tree
	/// if node = C3 returns C3         along with a pointer to C3 in the new (single node) tree
	/// ]]>
	/// </summary>
	public static (TNod<T> root, TNod<T> child) ExtendUpAndIncluding<T>(this TNod<T> node, Func<T, bool> predicate)
	{
		var childOrig = node;
		var child = Nod.Make(node.V);
		var root = child;
		while (!predicate(root.V))
		{
			var prev = root;
			if (childOrig.Parent == null) throw new ArgumentException("Failed to extend up");
			root = Nod.Make(childOrig.Parent.V, prev);
			childOrig = childOrig.Parent;
		}
		return (root, child);
	}


	/// <summary>
	///
	///    ┌──n1
	///    │
	///C1──┤
	///    │      ┌──C2
	///    └──n2──┤      ┌──n4──n5──n6
	///           └──n3──┤
	///                  └──C3──n7──C4──n8
	///
	/// predicate = node == C*
	/// 
	/// - if node = n2 returns
	///   --------------------
	///        ┌──⬤
	///    n2──┤      ┌──n4──n5──n6
	///        └──n3──┤ 
	///               └──⬤
	///   along with the boundary [C2, C3]
	///
	/// - if node = C3 (ie: predicate(node) == true)
	///   ------------------------------------------
	///   throw an Exception 
	/// 
	/// </summary>
	public static (TNod<T> root, TNod<T>[] boundary) ExtendDownToAndExcluding<T>(this TNod<T> node, Func<T, bool> predicate)
	{
		var boundary = new List<TNod<T>>();

		var curOrig = node;
		var cur = Nod.Make(node.V);
		var root = cur;

		void Rec(TNod<T> nOrig, TNod<T> n)
		{
			foreach (var childOrig in nOrig.Children)
			{
				if (predicate(childOrig.V))
				{
					boundary.Add(childOrig);
				}
				else
				{
					var child = Nod.Make(childOrig.V);
					n.AddChild(child);
					Rec(childOrig, child);
				}
			}
		}

		Rec(curOrig, cur);

		return (
			root,
			boundary.ToArray()
		);
	}



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


	public static TNod<T>[] GetFirstChildrenWhere<T>(this TNod<T> root, Func<T, bool> predicate)
	{
		TNod<T>[] Rec(TNod<T> node) => predicate(node.V) switch
		{
			true => new[] { node },
			false => node.Children.SelectMany(Rec).ToArray()
		};
		return root.Children.SelectMany(Rec).ToArray();
	}
}