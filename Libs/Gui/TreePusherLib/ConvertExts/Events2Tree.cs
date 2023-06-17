using PowBasics.CollectionsExt;
using PowRxVar;

namespace TreePusherLib;

public sealed record IncompleteNodeNfo<T>(
	TNod<T> ParentNod,
	T ChildNode
);

public sealed record ReconstructedTree<T>(
	TNod<T> Root,
	IncompleteNodeNfo<T>[] IncompleteNodes
);

public static class Events2Tree
{
	public static ReconstructedTree<T> ToTree<T>(this ITreeEvtObs<T> evtObs, Action runAction)
	{
		using var d = new Disp();

		var stack = new TreeStack<TNod<T>>();
		var incompleteNodes = new List<IncompleteNodeNfo<T>>();

		evtObs.WhenPush.Subscribe(args =>
		{
			stack.ActOnCurAndPush(() => Nod.Make(args), (_cur, _new) => _cur?.AddChild(_new));
		}).D(d);

		evtObs.WhenPop.Subscribe(args =>
		{
			var top = stack.Pop();
			if (top.V!.Equals(args)) return;	// Happy path

			var topPrev = top;

			while (!stack.IsEmpty)
			{
				top = stack.Pop();
				if (top.V!.Equals(args))
				{
					foreach (var c in top.Children.ToList())
						top.RemoveChild(c);
					incompleteNodes.Add(new IncompleteNodeNfo<T>(top, topPrev.V));
					return;
				}
				topPrev = top;
			}

			throw new ArgumentException("Root node is not closed. This is impossible");
		}).D(d);

		runAction();

		if (stack.RootFound == null)
			throw new ArgumentException("Failed to build Tree from TreeEvents");

		return new ReconstructedTree<T>(
			stack.RootFound,
			incompleteNodes.Distinct().ToArray()
		);
	}

	
	
	private sealed class TreeStack<S> where S : class
	{
		private readonly Stack<S> stack = new();
		private bool isFinished;

		public S? Cur => (stack.Count > 0) switch
		{
			true => stack.ElementAt(0),
			false => null
		};
		public bool IsEmpty => stack.Count == 0;
		public S? RootFound { get; private set; }

		public override string ToString() => "<" + stack.Select(e => $"[{e}]").JoinText(", ") + ">";

		public void ActOnCurAndPush(Func<S> gen, Action<S?, S> action)
		{
			if (isFinished) throw new ArgumentException("Cannot call ActOnCurAndPush once the tree is finished");
			var newElt = gen();
			action(Cur, newElt);
			stack.Push(newElt);
		}

		public void Push(S elt)
		{
			if (isFinished) throw new ArgumentException("Cannot call Push once the tree is finished");
			stack.Push(elt);
		}

		public S Pop()
		{
			if (isFinished) throw new ArgumentException("Cannot call Pop once the tree is finished");
			if (stack.Count == 1) Finish(stack.Peek());
			return stack.Pop();
		}

		private void Finish(S elt)
		{
			if (isFinished) throw new ArgumentException("Cannot call Finish once the tree is finished");
			RootFound = elt;
			isFinished = true;
		}
	}
}