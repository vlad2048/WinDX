using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Utils;
using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using PowBasics.CollectionsExt;
using PowBasics.Geom;

namespace ControlSystem.Structs;


public interface IMixNode { }

public sealed record CtrlNode(Ctrl Ctrl) : IMixNode
{
	public override string ToString() => Ctrl.GetType().Name;
}

public sealed record StFlexNode(NodeState State, FlexNode Flex) : IMixNode
{
	public override string ToString() => $"{Flex}";
}

public sealed record TextMeasureNode(Sz Size) : IMixNode
{
	public override string ToString() => $"text:{Size}";
}


static class MixNodeExt
{
	public static bool IsCtrl(this IMixNode node) => node is CtrlNode;
	public static bool IsCtrl(this MixNode nod) => nod.V.IsCtrl();
	public static Ctrl GetCtrl(this IMixNode node) => ((CtrlNode)node).Ctrl;
	public static Ctrl GetCtrl(this MixNode nod) => nod.V.GetCtrl();
	public static Ctrl[] GetAllCtrls(this MixNode nod) => nod.Where(e => e.IsCtrl()).SelectToArray(e => e.GetCtrl());
	public static Ctrl[] GetAllCtrlsUntil(this MixNode nod, Func<IMixNode, bool> predicate) => nod.GetNodesUntil(predicate).GetAllCtrls();
	private static Ctrl[] GetAllCtrls(this IMixNode[] nodes) => nodes.Where(e => e.IsCtrl()).SelectToArray(e => e.GetCtrl());

	public static bool IsNodeState(this IMixNode node) => node is StFlexNode;
	public static bool IsNodeState(this MixNode nod) => nod.V.IsNodeState();
	public static NodeState GetNodeState(this IMixNode node) => ((StFlexNode)node).State;
	public static NodeState GetNodeState(this MixNode nod) => nod.V.GetNodeState();
	public static NodeState[] GetAllNodeStates(this MixNode nod) => nod.Where(e => e.IsNodeState()).SelectToArray(e => e.GetNodeState());
	public static NodeState[] GetAllNodeStatesUntil(this MixNode nod, Func<IMixNode, bool> predicate) => nod.GetNodesUntil(predicate).GetAllNodeStates();
	private static NodeState[] GetAllNodeStates(this IMixNode[] nodes) => nodes.Where(e => e.IsNodeState()).SelectToArray(e => e.GetNodeState());

	public static bool IsPop(this MixNode node) => node.V.IsPop();
	public static bool IsPop(this IMixNode node) => node is StFlexNode { Flex.Flags.Pop: true };


	public static NodeState[] GetAllNodeStatesWithScrollingEnabled(this MixNode nod) =>
		nod
			.Where(e => e.IsNodeState() && ((StFlexNode)e.V).Flex.Flags.Scroll != BoolVec.False)
			.SelectToArray(e => e.GetNodeState());


	public static R GetCtrlR(this MixNode nod, PartitionSet set)
	{
		if (!nod.IsCtrl()) throw new ArgumentException("This function should only be called for a Ctrl node");
		return nod.GetFirstChildrenWhere(e => e.IsNodeState()).Select(e => set.RMap[e.GetNodeState()]).Union();
	}

	public static IReadOnlyDictionary<NodeState, MixNode> Make_NodeState_2_MixNode_Map(this MixNode root) =>
		root
			.Where(e => e.V is StFlexNode)
			.ToDictionary(e => ((StFlexNode)e.V).State);

	public static IReadOnlyDictionary<Ctrl, MixNode> Make_Ctrl_2_MixNode_Map(this MixNode root) =>
		root
			.Where(e => e.V is CtrlNode)
			.ToDictionary(e => ((CtrlNode)e.V).Ctrl);

	public static Ctrl FindParentCtrl(this MixNode nod)
	{
		var cur = nod.Parent;
		while (cur != null)
		{
			if (cur.IsCtrl())
				return cur.GetCtrl();
			cur = cur.Parent;
		}
		throw new ArgumentException("Impossible");
	}
}