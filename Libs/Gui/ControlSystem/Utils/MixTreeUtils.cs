using ControlSystem.Structs;

namespace ControlSystem.Utils;

static class MixTreeUtils
{
	public static IReadOnlyDictionary<MixNode, Ctrl> BuildMixNode2CtrlMap(MixNode root)
	{
		var map = new Dictionary<MixNode, Ctrl>();
		void Rec(MixNode node, Ctrl ctrlCur)
		{
			if (node.V is CtrlNode { Ctrl: var ctrlNext })
				ctrlCur = ctrlNext;
			map[node] = ctrlCur;
			foreach (var kid in node.Children)
				Rec(kid, ctrlCur);
		}
		if (root.V is not CtrlNode { Ctrl: var ctrlRoot }) throw new ArgumentException("The root should always be a CtrlNode");
		Rec(root, ctrlRoot);
		return map;
	}
}