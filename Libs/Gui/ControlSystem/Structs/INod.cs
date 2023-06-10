using LayoutSystem.Flex;
using LayoutSystem.Flex.Structs;
using LayoutSystem.Utils;
using PowBasics.Geom;

namespace ControlSystem.Structs;



public interface INod { }

public class FlexNod : INod
{
	public int? Id { get; init; }
	public FlexNode Flex { get; init; }

	public FlexNod(int? Id, FlexNode Flex)
	{
		this.Id = Id;
		this.Flex = Flex;
	}
	
	public override string ToString() => $"[{(Id.HasValue ? $"{Id.Value}" : "_")}] - {Flex}";
}

public record CtrlNod(Ctrl Ctrl) : INod
{
	public override string ToString() => $"{Ctrl.GetType().Name}";
}


public static class NodMaker
{
	public static INod Flex(
		DimVec dim,
		IStrat strat,
		Marg? marg = null
	) =>
		new FlexNod(
			null,
			new FlexNode(
				dim,
				strat,
				marg ?? Mg.Zero
			)
		);

	public static INod FlexId(
		int id,
		DimVec dim,
		IStrat strat,
		Marg? marg = null
	) =>
		new FlexNod(
			id,
			new FlexNode(
				dim,
				strat,
				marg ?? Mg.Zero
			)
		);

	public static INod Ctrl(
		Ctrl ctrl
	) =>
		new CtrlNod(
			ctrl
		);
}