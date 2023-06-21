namespace TreePusherLib.ConvertExts;

public static class Tree2Events
{
	public static void ToEvents<T>(this TNod<T> root, ITreeEvtSig<T> evtSig)
	{
		void Rec(TNod<T> node)
		{
			evtSig.SignalPush(node.V);
			foreach (var child in node.Children)
				Rec(child);
			evtSig.SignalPop(node.V);
		}
		Rec(root);
	}
}