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
		toolStrip1 = new ToolStrip();
		openFlexBuilderToolBtn = new ToolStripButton();
		statusStrip = new StatusStrip();
		windowsGroupBox = new GroupBox();
		redrawWindowBtn = new Button();
		unselectWindowBtn = new Button();
		layoutGroupBox = new GroupBox();
		demosGroupBox = new GroupBox();
		demosList = new BrightIdeasSoftware.ObjectListView();
		eventsGroupBox = new GroupBox();
		eventDisplayer = new Controls.EventDisplayer();
		showSysCtrlsCheckBox = new CheckBox();
		((System.ComponentModel.ISupportInitialize)layoutTree).BeginInit();
		layoutTreeContextMenu.SuspendLayout();
		toolStrip1.SuspendLayout();
		windowsGroupBox.SuspendLayout();
		layoutGroupBox.SuspendLayout();
		demosGroupBox.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)demosList).BeginInit();
		eventsGroupBox.SuspendLayout();
		SuspendLayout();
		// 
		// winList
		// 
		winList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		winList.FormattingEnabled = true;
		winList.ItemHeight = 15;
		winList.Location = new Point(6, 22);
		winList.Name = "winList";
		winList.Size = new Size(156, 94);
		winList.TabIndex = 0;
		// 
		// layoutTree
		// 
		layoutTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		layoutTree.CellEditUseWholeCell = false;
		layoutTree.ContextMenuStrip = layoutTreeContextMenu;
		layoutTree.Location = new Point(3, 19);
		layoutTree.Name = "layoutTree";
		layoutTree.ShowGroups = false;
		layoutTree.Size = new Size(596, 347);
		layoutTree.TabIndex = 4;
		layoutTree.View = View.Details;
		layoutTree.VirtualMode = true;
		// 
		// layoutTreeContextMenu
		// 
		layoutTreeContextMenu.Items.AddRange(new ToolStripItem[] { trackEventsMenuItem, stopTrackingMenuItem, toolStripSeparator1, stopAllTrackingMenuItem });
		layoutTreeContextMenu.Name = "layoutTreeContextMenu";
		layoutTreeContextMenu.Size = new Size(160, 76);
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
		// toolStrip1
		// 
		toolStrip1.Items.AddRange(new ToolStripItem[] { openFlexBuilderToolBtn });
		toolStrip1.Location = new Point(0, 0);
		toolStrip1.Name = "toolStrip1";
		toolStrip1.Size = new Size(792, 25);
		toolStrip1.TabIndex = 5;
		toolStrip1.Text = "toolStrip1";
		// 
		// openFlexBuilderToolBtn
		// 
		openFlexBuilderToolBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
		openFlexBuilderToolBtn.Image = Resource.ToolBtn_OpenFlexBuilder;
		openFlexBuilderToolBtn.ImageTransparentColor = Color.Magenta;
		openFlexBuilderToolBtn.Name = "openFlexBuilderToolBtn";
		openFlexBuilderToolBtn.Size = new Size(23, 22);
		openFlexBuilderToolBtn.Text = "Open in FlexBuilder";
		// 
		// statusStrip
		// 
		statusStrip.Location = new Point(0, 428);
		statusStrip.Name = "statusStrip";
		statusStrip.Size = new Size(792, 22);
		statusStrip.TabIndex = 6;
		statusStrip.Text = "statusStrip";
		// 
		// windowsGroupBox
		// 
		windowsGroupBox.Controls.Add(redrawWindowBtn);
		windowsGroupBox.Controls.Add(unselectWindowBtn);
		windowsGroupBox.Controls.Add(winList);
		windowsGroupBox.Location = new Point(12, 28);
		windowsGroupBox.Name = "windowsGroupBox";
		windowsGroupBox.Size = new Size(168, 150);
		windowsGroupBox.TabIndex = 7;
		windowsGroupBox.TabStop = false;
		windowsGroupBox.Text = "Windows";
		// 
		// redrawWindowBtn
		// 
		redrawWindowBtn.Location = new Point(6, 121);
		redrawWindowBtn.Name = "redrawWindowBtn";
		redrawWindowBtn.Size = new Size(75, 23);
		redrawWindowBtn.TabIndex = 2;
		redrawWindowBtn.Text = "Redraw";
		redrawWindowBtn.UseVisualStyleBackColor = true;
		// 
		// unselectWindowBtn
		// 
		unselectWindowBtn.Location = new Point(87, 121);
		unselectWindowBtn.Name = "unselectWindowBtn";
		unselectWindowBtn.Size = new Size(75, 23);
		unselectWindowBtn.TabIndex = 1;
		unselectWindowBtn.Text = "Unselect";
		unselectWindowBtn.UseVisualStyleBackColor = true;
		// 
		// layoutGroupBox
		// 
		layoutGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		layoutGroupBox.Controls.Add(showSysCtrlsCheckBox);
		layoutGroupBox.Controls.Add(layoutTree);
		layoutGroupBox.Location = new Point(186, 28);
		layoutGroupBox.Name = "layoutGroupBox";
		layoutGroupBox.Size = new Size(602, 397);
		layoutGroupBox.TabIndex = 8;
		layoutGroupBox.TabStop = false;
		layoutGroupBox.Text = "Layout";
		// 
		// demosGroupBox
		// 
		demosGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		demosGroupBox.Controls.Add(demosList);
		demosGroupBox.Location = new Point(12, 184);
		demosGroupBox.Name = "demosGroupBox";
		demosGroupBox.Size = new Size(168, 238);
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
		demosList.Size = new Size(162, 216);
		demosList.TabIndex = 0;
		demosList.View = View.Details;
		// 
		// eventsGroupBox
		// 
		eventsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
		eventsGroupBox.Controls.Add(eventDisplayer);
		eventsGroupBox.Location = new Point(794, 28);
		eventsGroupBox.Name = "eventsGroupBox";
		eventsGroupBox.Size = new Size(303, 397);
		eventsGroupBox.TabIndex = 10;
		eventsGroupBox.TabStop = false;
		eventsGroupBox.Text = "Events";
		// 
		// eventDisplayer
		// 
		eventDisplayer.Dock = DockStyle.Fill;
		eventDisplayer.Location = new Point(3, 19);
		eventDisplayer.Name = "eventDisplayer";
		eventDisplayer.Size = new Size(297, 375);
		eventDisplayer.TabIndex = 0;
		// 
		// showSysCtrlsCheckBox
		// 
		showSysCtrlsCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		showSysCtrlsCheckBox.AutoSize = true;
		showSysCtrlsCheckBox.Location = new Point(461, 372);
		showSysCtrlsCheckBox.Name = "showSysCtrlsCheckBox";
		showSysCtrlsCheckBox.Size = new Size(141, 19);
		showSysCtrlsCheckBox.TabIndex = 5;
		showSysCtrlsCheckBox.Text = "Show system controls";
		showSysCtrlsCheckBox.UseVisualStyleBackColor = true;
		// 
		// WinSpectorWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(792, 450);
		Controls.Add(eventsGroupBox);
		Controls.Add(demosGroupBox);
		Controls.Add(layoutGroupBox);
		Controls.Add(windowsGroupBox);
		Controls.Add(statusStrip);
		Controls.Add(toolStrip1);
		KeyPreview = true;
		Name = "WinSpectorWin";
		Text = "WinSpector";
		((System.ComponentModel.ISupportInitialize)layoutTree).EndInit();
		layoutTreeContextMenu.ResumeLayout(false);
		toolStrip1.ResumeLayout(false);
		toolStrip1.PerformLayout();
		windowsGroupBox.ResumeLayout(false);
		layoutGroupBox.ResumeLayout(false);
		layoutGroupBox.PerformLayout();
		demosGroupBox.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)demosList).EndInit();
		eventsGroupBox.ResumeLayout(false);
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	public ListBox winList;
	public BrightIdeasSoftware.TreeListView layoutTree;
	private ToolStrip toolStrip1;
	public ToolStripButton openFlexBuilderToolBtn;
	public GroupBox windowsGroupBox;
	public GroupBox layoutGroupBox;
	public StatusStrip statusStrip;
	private Button unselectWindowBtn;
	public GroupBox demosGroupBox;
	public BrightIdeasSoftware.ObjectListView demosList;
	public Button redrawWindowBtn;
	public GroupBox eventsGroupBox;
	public ToolStripMenuItem trackEventsMenuItem;
	public ToolStripMenuItem stopTrackingMenuItem;
	public ToolStripMenuItem stopAllTrackingMenuItem;
	private ToolStripSeparator toolStripSeparator1;
	public ContextMenuStrip layoutTreeContextMenu;
	public Controls.EventDisplayer eventDisplayer;
	public CheckBox showSysCtrlsCheckBox;
}