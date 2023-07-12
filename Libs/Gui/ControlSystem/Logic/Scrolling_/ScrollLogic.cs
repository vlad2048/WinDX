using ControlSystem.Logic.Popup_.Structs;
using ControlSystem.Logic.Scrolling_.Utils;

namespace ControlSystem.Logic.Scrolling_;

static class ScrollLogic
{
	public static PartitionSet HandleScrolling(this PartitionSet partitionSet, ScrollMan scrollMan) =>
		partitionSet
			.AddScrollBars(scrollMan)
			.ApplyScrollOffsets();

}