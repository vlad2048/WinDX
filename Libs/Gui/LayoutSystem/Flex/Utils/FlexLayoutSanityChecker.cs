using LayoutSystem.Flex.Structs;

namespace LayoutSystem.Flex.Utils;

static class FlexLayoutSanityChecker
{
	public static FlexLayout CheckSanity(this FlexLayout layout)
	{
		var root = layout.Root;
		var rMap = layout.RMap;

		var rootCnt = root.Count();
		var rMapCnt = rMap.Count;

		if (rMapCnt != rootCnt) throw new ArgumentException($"MixLayout sanity check failed. rootCnt={rootCnt} rMapCnt={rMapCnt}");
		if (root.Any(e => !rMap.ContainsKey(e))) throw new ArgumentException("MixLayout sanity check failed: Failed to find node in RMap");

		return layout;
	}
}