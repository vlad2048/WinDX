using ControlSystem.Logic.PopupLogic;
using ControlSystem.Structs;
using PowBasics.Geom;
using PowMaybe;

namespace ControlSystem.Logic.UserEventsLogic;

static class NodeHitTester
{
	/// <summary>
	/// Returns the nodes pointed to by the mouse <br/>
	/// The nodes are ordered bottom up
	/// </summary>
	public static Maybe<NodeState> FindNodeAtMouseCoordinates(
		Pt pt,
		Partition layout
	) =>
		layout.AllNodeStates
			.Where(state => layout.RMap[state].Contains(pt))
			.Reverse()
			.FirstOrMaybe();
}