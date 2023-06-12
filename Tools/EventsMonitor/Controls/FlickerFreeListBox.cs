﻿namespace EventsMonitor.Controls;

sealed class FlickerFreeListBox : ListBox
{
	public FlickerFreeListBox()
	{
		SetStyle(
			ControlStyles.OptimizedDoubleBuffer |
			ControlStyles.ResizeRedraw |
			ControlStyles.UserPaint,
			true);
		DrawMode = DrawMode.OwnerDrawFixed;
	}
	protected override void OnDrawItem(DrawItemEventArgs e)
	{
		if (e.Font == null) throw new NullReferenceException(nameof(e.Font));

		if (Items.Count > 0) {
			e.DrawBackground();
			e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));
		}
		base.OnDrawItem(e);
	}
	protected override void OnPaint(PaintEventArgs e)
	{
		Region iRegion = new Region(e.ClipRectangle);
		e.Graphics.FillRegion(new SolidBrush(BackColor), iRegion);
		if (Items.Count > 0) {
			for (int i = 0; i < Items.Count; ++i) {
				Rectangle irect = GetItemRectangle(i);
				if (e.ClipRectangle.IntersectsWith(irect)) {
					if ((SelectionMode == SelectionMode.One && SelectedIndex == i)
					|| (SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i))
					|| (SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i))) {
						OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
							irect, i,
							DrawItemState.Selected, ForeColor,
							BackColor));
					} else {
						OnDrawItem(new DrawItemEventArgs(e.Graphics, Font,
							irect, i,
							DrawItemState.Default, ForeColor,
							BackColor));
					}
					iRegion.Complement(irect);
				}
			}
		}
		base.OnPaint(e);
	}
}