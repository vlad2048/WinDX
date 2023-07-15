namespace WinSpectorLib;

partial class WinSpectorWin
{
	/// <summary>
	/// Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	/// Clean up any resources being used.
	/// </summary>
	/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	protected override void Dispose(bool disposing)
	{
		if (disposing && (components != null)) {
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	#region Windows Form Designer generated code

	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		winList = new ListBox();
		layoutTree = new BrightIdeasSoftware.TreeListView();
		layoutTreeContextMenu = new ContextMenuStrip(components);
		trackEventsMenuItem = new ToolStripMenuItem();
		stopTrackingMenuItem = new ToolStripMenuItem();
		toolStripSeparator1 = new ToolStripSeparator();
		stopAllTrackingMenuItem = new ToolStripMenuItem();
		toolStripSeparator2 = new ToolStripSeparator();
		printStateMenuItem = new ToolStripMenuItem();
		statusStrip = new StatusStrip();
		windowsGroupBox = new GroupBox();
		layoutGroupBox = new GroupBox();
		demosGroupBox = new GroupBox();
		demosList = new BrightIdeasSoftware.ObjectListView();
		eventsGroupBox = new GroupBox();
		eventDisplayer = new Controls.EventDisplayer();
		menuStrip1 = new MenuStrip();
		windowsToolStripMenuItem = new ToolStripMenuItem();
		windowRedrawItem = new ToolStripMenuItem();
		windowUnselectItem = new ToolStripMenuItem();
		windowLogRedrawItem = new ToolStripMenuItem();
		windowLogNextRedrawItem = new ToolStripMenuItem();
		windowLogNext2RedrawsItem = new ToolStripMenuItem();
		windowResizeItem = new ToolStripMenuItem();
		layoutToolStripMenuItem = new ToolStripMenuItem();
		layoutOpenInFlexBuilderItem = new ToolStripMenuItem();
		toolStripSeparator3 = new ToolStripSeparator();
		stopPrintingTrackedNodeStateMenuItem = new ToolStripMenuItem();
		eventsToolStripMenuItem = new ToolStripMenuItem();
		eventsShowItem = new ToolStripMenuItem();
		eventsEnabledItem = new ToolStripMenuItem();
		eventsClearItem = new ToolStripMenuItem();
		toolStripSeparator4 = new ToolStripSeparator();
		windowClearConsoleItem = new ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)layoutTree).BeginInit();
		layoutTreeContextMenu.SuspendLayout();
		windowsGroupBox.SuspendLayout();
		layoutGroupBox.SuspendLayout();
		demosGroupBox.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)demosList).BeginInit();
		eventsGroupBox.SuspendLayout();
		menuStrip1.SuspendLayout();
		SuspendLayout();
		// 
		// winList
		// 
		winList.Dock = DockStyle.Fill;
		winList.FormattingEnabled = true;
		winList.ItemHeight = 15;
		winList.Location = new Point(3, 19);
		winList.Name = "winList";
		winList.Size = new Size(162, 161);
		winList.TabIndex = 0;
		// 
		// layoutTree
		// 
		layoutTree.CellEditUseWholeCell = false;
		layoutTree.ContextMenuStrip = layoutTreeContextMenu;
		layoutTree.Dock = DockStyle.Fill;
		layoutTree.Location = new Point(3, 19);
		layoutTree.Name = "layoutTree";
		layoutTree.ShowGroups = false;
		layoutTree.Size = new Size(562, 534);
		layoutTree.TabIndex = 4;
		layoutTree.View = View.Details;
		layoutTree.VirtualMode = true;
		// 
		// layoutTreeContextMenu
		// 
		layoutTreeContextMenu.Items.AddRange(new ToolStripItem[] { trackEventsMenuItem, stopTrackingMenuItem, toolStripSeparator1, stopAllTrackingMenuItem, toolStripSeparator2, printStateMenuItem });
		layoutTreeContextMenu.Name = "layoutTreeContextMenu";
		layoutTreeContextMenu.Size = new Size(160, 104);
		// 
		// trackEventsMenuItem
		// 
		trackEventsMenuItem.Name = "trackEventsMenuItem";
		trackEventsMenuItem.Size = new Size(159, 22);
		trackEventsMenuItem.Text = "Track events";
		// 
		// stopTrackingMenuItem
		// 
		stopTrackingMenuItem.Name = "stopTrackingMenuItem";
		stopTrackingMenuItem.Size = new Size(159, 22);
		stopTrackingMenuItem.Text = "Stop tracking";
		// 
		// toolStripSeparator1
		// 
		toolStripSeparator1.Name = "toolStripSeparator1";
		toolStripSeparator1.Size = new Size(156, 6);
		// 
		// stopAllTrackingMenuItem
		// 
		stopAllTrackingMenuItem.Name = "stopAllTrackingMenuItem";
		stopAllTrackingMenuItem.Size = new Size(159, 22);
		stopAllTrackingMenuItem.Text = "Stop all tracking";
		// 
		// toolStripSeparator2
		// 
		toolStripSeparator2.Name = "toolStripSeparator2";
		toolStripSeparator2.Size = new Size(156, 6);
		// 
		// printStateMenuItem
		// 
		printStateMenuItem.Name = "printStateMenuItem";
		printStateMenuItem.Size = new Size(159, 22);
		printStateMenuItem.Text = "Print State";
		// 
		// statusStrip
		// 
		statusStrip.Location = new Point(0, 592);
		statusStrip.Name = "statusStrip";
		statusStrip.Size = new Size(1075, 22);
		statusStrip.TabIndex = 6;
		statusStrip.Text = "statusStrip";
		// 
		// windowsGroupBox
		// 
		windowsGroupBox.Controls.Add(winList);
		windowsGroupBox.Location = new Point(12, 27);
		windowsGroupBox.Name = "windowsGroupBox";
		windowsGroupBox.Size = new Size(168, 183);
		windowsGroupBox.TabIndex = 7;
		windowsGroupBox.TabStop = false;
		windowsGroupBox.Text = "Windows";
		// 
		// layoutGroupBox
		// 
		layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		layoutGroupBox.Controls.Add(layoutTree);
		layoutGroupBox.Location = new Point(186, 27);
		layoutGroupBox.Name = "layoutGroupBox";
		layoutGroupBox.Size = new Size(568, 556);
		layoutGroupBox.TabIndex = 8;
		layoutGroupBox.TabStop = false;
		layoutGroupBox.Text = "Layout";
		// 
		// demosGroupBox
		// 
		demosGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		demosGroupBox.Controls.Add(demosList);
		demosGroupBox.Location = new Point(12, 216);
		demosGroupBox.Name = "demosGroupBox";
		demosGroupBox.Size = new Size(168, 367);
		demosGroupBox.TabIndex = 9;
		demosGroupBox.TabStop = false;
		demosGroupBox.Text = "Demos";
		// 
		// demosList
		// 
		demosList.CellEditUseWholeCell = false;
		demosList.Dock = DockStyle.Fill;
		demosList.FullRowSelect = true;
		demosList.Location = new Point(3, 19);
		demosList.Name = "demosList";
		demosList.Size = new Size(162, 345);
		demosList.TabIndex = 0;
		demosList.View = View.Details;
		// 
		// eventsGroupBox
		// 
		eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
		eventsGroupBox.Controls.Add(eventDisplayer);
		eventsGroupBox.Location = new Point(760, 27);
		eventsGroupBox.Name = "eventsGroupBox";
		eventsGroupBox.Size = new Size(303, 556);
		eventsGroupBox.TabIndex = 10;
		eventsGroupBox.TabStop = false;
		eventsGroupBox.Text = "Events";
		// 
		// eventDisplayer
		// 
		eventDisplayer.Dock = DockStyle.Fill;
		eventDisplayer.Location = new Point(3, 19);
		eventDisplayer.Name = "eventDisplayer";
		eventDisplayer.Size = new Size(297, 534);
		eventDisplayer.TabIndex = 0;
		// 
		// menuStrip1
		// 
		menuStrip1.Items.AddRange(new ToolStripItem[] { windowsToolStripMenuItem, layoutToolStripMenuItem, eventsToolStripMenuItem });
		menuStrip1.Location = new Point(0, 0);
		menuStrip1.Name = "menuStrip1";
		menuStrip1.Size = new Size(1075, 24);
		menuStrip1.TabIndex = 11;
		menuStrip1.Text = "menuStrip1";
		// 
		// windowsToolStripMenuItem
		// 
		windowsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { windowRedrawItem, windowUnselectItem, windowLogRedrawItem, windowLogNextRedrawItem, windowLogNext2RedrawsItem, windowResizeItem, toolStripSeparator4, windowClearConsoleItem });
		windowsToolStripMenuItem.Name = "windowsToolStripMenuItem";
		windowsToolStripMenuItem.Size = new Size(68, 20);
		windowsToolStripMenuItem.Text = "&Windows";
		// 
		// windowRedrawItem
		// 
		windowRedrawItem.Name = "windowRedrawItem";
		windowRedrawItem.Size = new Size(188, 22);
		windowRedrawItem.Text = "&Redraw";
		// 
		// windowUnselectItem
		// 
		windowUnselectItem.Name = "windowUnselectItem";
		windowUnselectItem.Size = new Size(188, 22);
		windowUnselectItem.Text = "&Unselect";
		// 
		// windowLogRedrawItem
		// 
		windowLogRedrawItem.Name = "windowLogRedrawItem";
		windowLogRedrawItem.Size = new Size(188, 22);
		windowLogRedrawItem.Text = "Log Redraw (Ctrl R)";
		// 
		// windowLogNextRedrawItem
		// 
		windowLogNextRedrawItem.Name = "windowLogNextRedrawItem";
		windowLogNextRedrawItem.Size = new Size(188, 22);
		windowLogNextRedrawItem.Text = "Log Next Redraw";
		// 
		// windowLogNext2RedrawsItem
		// 
		windowLogNext2RedrawsItem.Name = "windowLogNext2RedrawsItem";
		windowLogNext2RedrawsItem.Size = new Size(188, 22);
		windowLogNext2RedrawsItem.Text = "Log Next 2 Redraws";
		// 
		// windowResizeItem
		// 
		windowResizeItem.Name = "windowResizeItem";
		windowResizeItem.Size = new Size(188, 22);
		windowResizeItem.Text = "Re&size... (Ctrl +/-)";
		// 
		// layoutToolStripMenuItem
		// 
		layoutToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { layoutOpenInFlexBuilderItem, toolStripSeparator3, stopPrintingTrackedNodeStateMenuItem });
		layoutToolStripMenuItem.Name = "layoutToolStripMenuItem";
		layoutToolStripMenuItem.Size = new Size(55, 20);
		layoutToolStripMenuItem.Text = "&Layout";
		// 
		// layoutOpenInFlexBuilderItem
		// 
		layoutOpenInFlexBuilderItem.Name = "layoutOpenInFlexBuilderItem";
		layoutOpenInFlexBuilderItem.Size = new Size(202, 22);
		layoutOpenInFlexBuilderItem.Text = "Open in &FlexBuilder";
		// 
		// toolStripSeparator3
		// 
		toolStripSeparator3.Name = "toolStripSeparator3";
		toolStripSeparator3.Size = new Size(199, 6);
		// 
		// stopPrintingTrackedNodeStateMenuItem
		// 
		stopPrintingTrackedNodeStateMenuItem.Name = "stopPrintingTrackedNodeStateMenuItem";
		stopPrintingTrackedNodeStateMenuItem.Size = new Size(202, 22);
		stopPrintingTrackedNodeStateMenuItem.Text = "Stop tracking node state";
		// 
		// eventsToolStripMenuItem
		// 
		eventsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { eventsShowItem, eventsEnabledItem, eventsClearItem });
		eventsToolStripMenuItem.Name = "eventsToolStripMenuItem";
		eventsToolStripMenuItem.Size = new Size(53, 20);
		eventsToolStripMenuItem.Text = "&Events";
		// 
		// eventsShowItem
		// 
		eventsShowItem.CheckOnClick = true;
		eventsShowItem.Name = "eventsShowItem";
		eventsShowItem.Size = new Size(133, 22);
		eventsShowItem.Text = "Show (S)";
		// 
		// eventsEnabledItem
		// 
		eventsEnabledItem.Name = "eventsEnabledItem";
		eventsEnabledItem.Size = new Size(133, 22);
		eventsEnabledItem.Text = "Enabled (E)";
		// 
		// eventsClearItem
		// 
		eventsClearItem.Name = "eventsClearItem";
		eventsClearItem.Size = new Size(133, 22);
		eventsClearItem.Text = "Clear (C)";
		// 
		// toolStripSeparator4
		// 
		toolStripSeparator4.Name = "toolStripSeparator4";
		toolStripSeparator4.Size = new Size(185, 6);
		// 
		// windowClearConsoleItem
		// 
		windowClearConsoleItem.Name = "windowClearConsoleItem";
		windowClearConsoleItem.Size = new Size(188, 22);
		windowClearConsoleItem.Text = "Clear Console (Ctrl C)";
		// 
		// WinSpectorWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(1075, 614);
		Controls.Add(eventsGroupBox);
		Controls.Add(demosGroupBox);
		Controls.Add(layoutGroupBox);
		Controls.Add(windowsGroupBox);
		Controls.Add(statusStrip);
		Controls.Add(menuStrip1);
		KeyPreview = true;
		MainMenuStrip = menuStrip1;
		Name = "WinSpectorWin";
		Text = "WinSpector";
		((System.ComponentModel.ISupportInitialize)layoutTree).EndInit();
		layoutTreeContextMenu.ResumeLayout(false);
		windowsGroupBox.ResumeLayout(false);
		layoutGroupBox.ResumeLayout(false);
		demosGroupBox.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)demosList).EndInit();
		eventsGroupBox.ResumeLayout(false);
		menuStrip1.ResumeLayout(false);
		menuStrip1.PerformLayout();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	public ListBox winList;
	public BrightIdeasSoftware.TreeListView layoutTree;
	public GroupBox windowsGroupBox;
	public GroupBox layoutGroupBox;
	public StatusStrip statusStrip;
	public GroupBox demosGroupBox;
	public BrightIdeasSoftware.ObjectListView demosList;
	public GroupBox eventsGroupBox;
	public ToolStripMenuItem trackEventsMenuItem;
	public ToolStripMenuItem stopTrackingMenuItem;
	public ToolStripMenuItem stopAllTrackingMenuItem;
	private ToolStripSeparator toolStripSeparator1;
	public ContextMenuStrip layoutTreeContextMenu;
	private MenuStrip menuStrip1;
	private ToolStripMenuItem windowsToolStripMenuItem;
	public ToolStripMenuItem windowRedrawItem;
	public ToolStripMenuItem windowUnselectItem;
	private ToolStripMenuItem layoutToolStripMenuItem;
	private ToolStripMenuItem eventsToolStripMenuItem;
	public ToolStripMenuItem eventsShowItem;
	public ToolStripMenuItem eventsEnabledItem;
	public ToolStripMenuItem eventsClearItem;
	public Controls.EventDisplayer eventDisplayer;
	public ToolStripMenuItem layoutOpenInFlexBuilderItem;
	private ToolStripSeparator toolStripSeparator2;
	public ToolStripMenuItem printStateMenuItem;
	private ToolStripSeparator toolStripSeparator3;
	public ToolStripMenuItem stopPrintingTrackedNodeStateMenuItem;
	public ToolStripMenuItem windowLogRedrawItem;
	public ToolStripMenuItem windowLogNextRedrawItem;
	public ToolStripMenuItem windowLogNext2RedrawsItem;
	public ToolStripMenuItem windowResizeItem;
	private ToolStripSeparator toolStripSeparator4;
	public ToolStripMenuItem windowClearConsoleItem;
}