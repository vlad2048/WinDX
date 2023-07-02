using PowBasics.CollectionsExt;
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
	public INode[] HitFun(Pt pt) => Nodes.Where(e => e.R.V.Contains(pt)).SelectToArray(e => (INode)e);
	public void Invalidate() { }

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