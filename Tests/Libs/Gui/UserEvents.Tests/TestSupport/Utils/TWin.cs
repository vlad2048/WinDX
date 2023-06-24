using PowBasics.Geom;
using PowMaybe;
using UserEvents.Structs;

namespace UserEvents.Tests.TestSupport.Utils;

sealed class TWin : IWin
{
	private readonly List<TNode> nodes = new();

	public TNode[] Nodes => nodes.OrderByDescending(e => e.Depth).ToArray();

	// IWin
	// ====
	public IObservable<IUserEvt> Evt { get; }
	public Maybe<INode> HitFun(Pt pt) => Nodes.FirstOrMaybe(e => e.R.V.Contains(pt)).Select(e => (INode)e);

	public TWin(IObservable<IUserEvt> evt)
	{
		Evt = evt;
	}

	public void AddNodes(TNode[] arr) => nodes.AddRange(arr);

	public void DelNodes(TNode[] arr)
	{
		foreach (var elt in arr)
			nodes.Remove(elt);
	}
}