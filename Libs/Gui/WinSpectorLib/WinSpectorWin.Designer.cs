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
		toolStrip1 = new ToolStrip();
		openFlexBuilderToolBtn = new ToolStripButton();
		statusStrip = new StatusStrip();
		windowsGroupBox = new GroupBox();
		unselectWindowBtn = new Button();
		layoutGroupBox = new GroupBox();
		demosGroupBox = new GroupBox();
		demosList = new BrightIdeasSoftware.ObjectListView();
		((System.ComponentModel.ISupportInitialize)layoutTree).BeginInit();
		toolStrip1.SuspendLayout();
		windowsGroupBox.SuspendLayout();
		layoutGroupBox.SuspendLayout();
		demosGroupBox.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)demosList).BeginInit();
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
		layoutTree.CellEditUseWholeCell = false;
		layoutTree.Dock = DockStyle.Fill;
		layoutTree.Location = new Point(3, 19);
		layoutTree.Name = "layoutTree";
		layoutTree.ShowGroups = false;
		layoutTree.Size = new Size(596, 375);
		layoutTree.TabIndex = 4;
		layoutTree.View = View.Details;
		layoutTree.VirtualMode = true;
		// 
		// toolStrip1
		// 
		toolStrip1.Items.AddRange(new ToolStripItem[] { openFlexBuilderToolBtn });
		toolStrip1.Location = new Point(0, 0);
		toolStrip1.Name = "toolStrip1";
		toolStrip1.Size = new Size(800, 25);
		toolStrip1.TabIndex = 5;
		toolStrip1.Text = "toolStrip1";
		// 
		// openFlexBuilderToolBtn
		// 
		openFlexBuilderToolBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
		openFlexBuilderToolBtn.Image = _3_WinSpectorLib.Resource.ToolBtn_OpenFlexBuilder;
		openFlexBuilderToolBtn.ImageTransparentColor = Color.Magenta;
		openFlexBuilderToolBtn.Name = "openFlexBuilderToolBtn";
		openFlexBuilderToolBtn.Size = new Size(23, 22);
		openFlexBuilderToolBtn.Text = "Open in FlexBuilder";
		// 
		// statusStrip
		// 
		statusStrip.Location = new Point(0, 428);
		statusStrip.Name = "statusStrip";
		statusStrip.Size = new Size(800, 22);
		statusStrip.TabIndex = 6;
		statusStrip.Text = "statusStrip";
		// 
		// windowsGroupBox
		// 
		windowsGroupBox.Controls.Add(unselectWindowBtn);
		windowsGroupBox.Controls.Add(winList);
		windowsGroupBox.Location = new Point(12, 28);
		windowsGroupBox.Name = "windowsGroupBox";
		windowsGroupBox.Size = new Size(168, 150);
		windowsGroupBox.TabIndex = 7;
		windowsGroupBox.TabStop = false;
		windowsGroupBox.Text = "Windows";
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
		// WinSpectorWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(demosGroupBox);
		Controls.Add(layoutGroupBox);
		Controls.Add(windowsGroupBox);
		Controls.Add(statusStrip);
		Controls.Add(toolStrip1);
		Name = "WinSpectorWin";
		Text = "WinSpector";
		((System.ComponentModel.ISupportInitialize)layoutTree).EndInit();
		toolStrip1.ResumeLayout(false);
		toolStrip1.PerformLayout();
		windowsGroupBox.ResumeLayout(false);
		layoutGroupBox.ResumeLayout(false);
		demosGroupBox.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)demosList).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	public ListBox winList;
	public BrightIdeasSoftware.TreeListView layoutTree;
	private ToolStrip toolStrip1;
	public ToolStripButton openFlexBuilderToolBtn;
	public GroupBox windowsGroupBox;
	private GroupBox layoutGroupBox;
	public StatusStrip statusStrip;
	private Button unselectWindowBtn;
	public GroupBox demosGroupBox;
	public BrightIdeasSoftware.ObjectListView demosList;
}