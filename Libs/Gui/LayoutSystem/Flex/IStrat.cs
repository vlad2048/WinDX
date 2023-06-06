using LayoutSystem.Flex.Structs;

namespace LayoutSystem.Flex;

public interface IStrat
{
	LayNfo Lay(
		Node node,
		FreeSz freeSz,
		DimVec[] kidDims
	);
}

public interface IStratInternal : IStrat { }
