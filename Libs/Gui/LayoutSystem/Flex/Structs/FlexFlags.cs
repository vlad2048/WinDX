// ReSharper disable once CheckNamespace
namespace LayoutSystem.Flex.Structs;

public readonly record struct FlexFlags(
	BoolVec Scroll,
	bool Pop
)
{
	public override string ToString() => (Scroll, Pop) switch
	{
		(Scroll: { X: false, Y: false }, false) => "_",
		(Scroll: { X: truer, Y: false }, false) => "ScrollX",
		(Scroll: { X: false, Y: truer }, false) => "ScrollY",
		(Scroll: { X: truer, Y: truer }, false) => "ScrollXY",
		(Scroll: { X: false, Y: false }, truer) => "Pop",
		(Scroll: { X: truer, Y: false }, truer) => "Pop-ScrollX",
		(Scroll: { X: false, Y: truer }, truer) => "Pop-ScrollY",
		(Scroll: { X: truer, Y: truer }, truer) => "Pop-ScrollXY",
	};

	public static readonly FlexFlags None = new(BoolVec.False, false);
}