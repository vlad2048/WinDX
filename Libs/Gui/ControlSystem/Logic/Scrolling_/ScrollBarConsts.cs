using System.Drawing;
using ControlSystem.Logic.Scrolling_.Structs;
using PowBasics.Geom;
using RenderLib.Structs;

namespace ControlSystem.Logic.Scrolling_;

static class ScrollBarConsts
{
	public static readonly PenDef EdgeInnerColor = new(Color.White, 1);
	public static readonly PenDef EdgeOuterColor = new(Color.FromArgb(0xF0, 0xF0, 0xF0), 1);
	public static readonly BrushDef BackColor = new SolidBrushDef(Color.FromArgb(0xF0, 0xF0, 0xF0));
	public static readonly BrushDef EmptyCornerColor = new SolidBrushDef(Color.FromArgb(0xCF, 0xCF, 0xCF));


	public static (Bitmap, BrushDef) GetBtnBmpCol(Dir dir, ScrollBtnDecInc decInc, ScrollBtnState state) =>
	(
		(dir, decInc) switch
		{
			(Dir.Horz, ScrollBtnDecInc.Dec) => state switch
			{
				ScrollBtnState.Normal => btnLeftNormalBmp,
				ScrollBtnState.Hover => btnLeftHoverBmp,
				ScrollBtnState.Press => btnLeftPressBmp,
			},
			(Dir.Horz, ScrollBtnDecInc.Inc) => state switch
			{
				ScrollBtnState.Normal => btnRightNormalBmp,
				ScrollBtnState.Hover => btnRightHoverBmp,
				ScrollBtnState.Press => btnRightPressBmp,
			},
			(Dir.Vert, ScrollBtnDecInc.Dec) => state switch
			{
				ScrollBtnState.Normal => btnUpNormalBmp,
				ScrollBtnState.Hover => btnUpHoverBmp,
				ScrollBtnState.Press => btnUpPressBmp,
			},
			(Dir.Vert, ScrollBtnDecInc.Inc) => state switch
			{
				ScrollBtnState.Normal => btnDownNormalBmp,
				ScrollBtnState.Hover => btnDownHoverBmp,
				ScrollBtnState.Press => btnDownPressBmp,
			},
		},
		GetBtnBackColor(state)
	);

	
	public static BrushDef GetThumbBackColor(ScrollBtnState state) => state switch
	{
		ScrollBtnState.Normal => thumbNormalColor,
		ScrollBtnState.Hover => thumbHoverColor,
		ScrollBtnState.Press => thumbPressColor
	};


	private static BrushDef GetBtnBackColor(ScrollBtnState state) => state switch
	{
		ScrollBtnState.Normal => btnNormalColor,
		ScrollBtnState.Hover => btnHoverColor,
		ScrollBtnState.Press => btnPressColor
	};

	private static readonly BrushDef btnNormalColor = BackColor;
	private static readonly BrushDef btnHoverColor = new SolidBrushDef(Color.FromArgb(0xDA, 0xDA, 0xDA));
	private static readonly BrushDef btnPressColor = new SolidBrushDef(Color.FromArgb(0x60, 0x60, 0x60));

	private static readonly BrushDef thumbNormalColor = new SolidBrushDef(Color.FromArgb(0xCD, 0xCD, 0xCD));
	private static readonly BrushDef thumbHoverColor = new SolidBrushDef(Color.FromArgb(0xA6, 0xA6, 0xA6));
	private static readonly BrushDef thumbPressColor = new SolidBrushDef(Color.FromArgb(0x60, 0x60, 0x60));


	private static readonly Bitmap btnLeftNormalBmp = Resource.scrollbar_left_normal;
	private static readonly Bitmap btnLeftHoverBmp = Resource.scrollbar_left_hover;
	private static readonly Bitmap btnLeftPressBmp = Resource.scrollbar_left_press;

	private static readonly Bitmap btnRightNormalBmp = Resource.scrollbar_right_normal;
	private static readonly Bitmap btnRightHoverBmp = Resource.scrollbar_right_hover;
	private static readonly Bitmap btnRightPressBmp = Resource.scrollbar_right_press;

	private static readonly Bitmap btnUpNormalBmp = Resource.scrollbar_up_normal;
	private static readonly Bitmap btnUpHoverBmp = Resource.scrollbar_up_hover;
	private static readonly Bitmap btnUpPressBmp = Resource.scrollbar_up_press;

	private static readonly Bitmap btnDownNormalBmp = Resource.scrollbar_down_normal;
	private static readonly Bitmap btnDownHoverBmp = Resource.scrollbar_down_hover;
	private static readonly Bitmap btnDownPressBmp = Resource.scrollbar_down_press;
}
