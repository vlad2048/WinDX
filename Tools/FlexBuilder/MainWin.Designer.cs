namespace FlexBuilder;

partial class MainWin
{
	/// <summary>
	///  Required designer variable.
	/// </summary>
	private System.ComponentModel.IContainer components = null;

	/// <summary>
	///  Clean up any resources being used.
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
	///  Required method for Designer support - do not modify
	///  the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
		layoutTree = new BrightIdeasSoftware.TreeListView();
		layoutTreeContextMenu = new ContextMenuStrip(components);
		addFillMenuItem = new ToolStripMenuItem();
		addStackMenuItem = new ToolStripMenuItem();
		addWrapMenuItem = new ToolStripMenuItem();
		removeNodeMenuItem = new ToolStripMenuItem();
		nodeEditor = new Editors.NodeEditor();
		menuStrip = new MenuStrip();
		fileToolStripMenuItem = new ToolStripMenuItem();
		fileNewItem = new ToolStripMenuItem();
		fileOpenItem = new ToolStripMenuItem();
		fileSaveItem = new ToolStripMenuItem();
		fileSaveAsItem = new ToolStripMenuItem();
		toolStripSeparator1 = new ToolStripSeparator();
		fileExitItem = new ToolStripMenuItem();
		renderingToolStripMenuItem = new ToolStripMenuItem();
		renderingEnabledItem = new ToolStripMenuItem();
		renderingRedrawItem = new ToolStripMenuItem();
		renderingWindowSizeItem = new ToolStripMenuItem();
		winDimsXNumeric = new NumericUpDown();
		statusStrip = new StatusStrip();
		showWinStatusBtn = new ToolStripDropDownButton();
		redrawStatusBtn = new ToolStripDropDownButton();
		rendererStatusCombo = new ToolStripDropDownButton();
		gdiplusStatusItem = new ToolStripMenuItem();
		direct2dStatusItem = new ToolStripMenuItem();
		direct2dindirect3dStatusItem = new ToolStripMenuItem();
		calcWinSzStatusLabel = new ToolStripStatusLabel();
		winDimsXCheckBox = new CheckBox();
		groupBox1 = new GroupBox();
		label3 = new Label();
		winDimsYCheckBox = new CheckBox();
		winDimsYNumeric = new NumericUpDown();
		label2 = new Label();
		groupBox2 = new GroupBox();
		tabControl = new TabControl();
		editTab = new TabPage();
		detailsTab = new TabPage();
		splitContainer = new SplitContainer();
		detailsTree = new BrightIdeasSoftware.TreeListView();
		detailsRichTextBox = new RichTextBox();
		performanceTab = new TabPage();
		renderingRendererItem = new ToolStripMenuItem();
		renderingRendererGDIPlusItem = new ToolStripMenuItem();
		renderingRendererDirect2DItem = new ToolStripMenuItem();
		renderingRendererDirect3DInDirect2DItem = new ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)layoutTree).BeginInit();
		layoutTreeContextMenu.SuspendLayout();
		menuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)winDimsXNumeric).BeginInit();
		statusStrip.SuspendLayout();
		groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)winDimsYNumeric).BeginInit();
		groupBox2.SuspendLayout();
		tabControl.SuspendLayout();
		editTab.SuspendLayout();
		detailsTab.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
		splitContainer.Panel1.SuspendLayout();
		splitContainer.Panel2.SuspendLayout();
		splitContainer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)detailsTree).BeginInit();
		SuspendLayout();
		// 
		// layoutTree
		// 
		layoutTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		layoutTree.CellEditUseWholeCell = false;
		layoutTree.ContextMenuStrip = layoutTreeContextMenu;
		layoutTree.Location = new Point(6, 6);
		layoutTree.Name = "layoutTree";
		layoutTree.ShowGroups = false;
		layoutTree.Size = new Size(595, 494);
		layoutTree.TabIndex = 5;
		layoutTree.View = View.Details;
		layoutTree.VirtualMode = true;
		// 
		// layoutTreeContextMenu
		// 
		layoutTreeContextMenu.Items.AddRange(new ToolStripItem[] { addFillMenuItem, addStackMenuItem, addWrapMenuItem, removeNodeMenuItem });
		layoutTreeContextMenu.Name = "layoutTreeContextMenu";
		layoutTreeContextMenu.Size = new Size(158, 92);
		// 
		// addFillMenuItem
		// 
		addFillMenuItem.Name = "addFillMenuItem";
		addFillMenuItem.Size = new Size(157, 22);
		addFillMenuItem.Text = "Add Fill node";
		// 
		// addStackMenuItem
		// 
		addStackMenuItem.Name = "addStackMenuItem";
		addStackMenuItem.Size = new Size(157, 22);
		addStackMenuItem.Text = "Add Stack node";
		// 
		// addWrapMenuItem
		// 
		addWrapMenuItem.Name = "addWrapMenuItem";
		addWrapMenuItem.Size = new Size(157, 22);
		addWrapMenuItem.Text = "Add Wrap node";
		// 
		// removeNodeMenuItem
		// 
		removeNodeMenuItem.Name = "removeNodeMenuItem";
		removeNodeMenuItem.Size = new Size(157, 22);
		removeNodeMenuItem.Text = "Remove node";
		// 
		// nodeEditor
		// 
		nodeEditor.Location = new Point(6, 22);
		nodeEditor.Name = "nodeEditor";
		nodeEditor.Size = new Size(203, 356);
		nodeEditor.TabIndex = 8;
		// 
		// menuStrip
		// 
		menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, renderingToolStripMenuItem });
		menuStrip.Location = new Point(0, 0);
		menuStrip.Name = "menuStrip";
		menuStrip.Size = new Size(838, 24);
		menuStrip.TabIndex = 9;
		menuStrip.Text = "menuStrip1";
		// 
		// fileToolStripMenuItem
		// 
		fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fileNewItem, fileOpenItem, fileSaveItem, fileSaveAsItem, toolStripSeparator1, fileExitItem });
		fileToolStripMenuItem.Name = "fileToolStripMenuItem";
		fileToolStripMenuItem.Size = new Size(37, 20);
		fileToolStripMenuItem.Text = "&File";
		// 
		// fileNewItem
		// 
		fileNewItem.Name = "fileNewItem";
		fileNewItem.Size = new Size(123, 22);
		fileNewItem.Text = "&New";
		// 
		// fileOpenItem
		// 
		fileOpenItem.Name = "fileOpenItem";
		fileOpenItem.Size = new Size(123, 22);
		fileOpenItem.Text = "&Open...";
		// 
		// fileSaveItem
		// 
		fileSaveItem.Name = "fileSaveItem";
		fileSaveItem.Size = new Size(123, 22);
		fileSaveItem.Text = "&Save";
		// 
		// fileSaveAsItem
		// 
		fileSaveAsItem.Name = "fileSaveAsItem";
		fileSaveAsItem.Size = new Size(123, 22);
		fileSaveAsItem.Text = "Save &As...";
		// 
		// toolStripSeparator1
		// 
		toolStripSeparator1.Name = "toolStripSeparator1";
		toolStripSeparator1.Size = new Size(120, 6);
		// 
		// fileExitItem
		// 
		fileExitItem.Name = "fileExitItem";
		fileExitItem.Size = new Size(123, 22);
		fileExitItem.Text = "E&xit";
		// 
		// renderingToolStripMenuItem
		// 
		renderingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { renderingEnabledItem, renderingRedrawItem, renderingWindowSizeItem, renderingRendererItem });
		renderingToolStripMenuItem.Name = "renderingToolStripMenuItem";
		renderingToolStripMenuItem.Size = new Size(73, 20);
		renderingToolStripMenuItem.Text = "&Rendering";
		// 
		// renderingEnabledItem
		// 
		renderingEnabledItem.Name = "renderingEnabledItem";
		renderingEnabledItem.Size = new Size(180, 22);
		renderingEnabledItem.Text = "Enabled (Ctrl E)";
		// 
		// renderingRedrawItem
		// 
		renderingRedrawItem.Name = "renderingRedrawItem";
		renderingRedrawItem.Size = new Size(180, 22);
		renderingRedrawItem.Text = "Redraw (Ctrl R)";
		// 
		// renderingWindowSizeItem
		// 
		renderingWindowSizeItem.Name = "renderingWindowSizeItem";
		renderingWindowSizeItem.Size = new Size(180, 22);
		renderingWindowSizeItem.Text = "Window &Size...";
		// 
		// winDimsXNumeric
		// 
		winDimsXNumeric.Increment = new decimal(new int[] { 10, 0, 0, 0 });
		winDimsXNumeric.Location = new Point(47, 22);
		winDimsXNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		winDimsXNumeric.Name = "winDimsXNumeric";
		winDimsXNumeric.Size = new Size(57, 23);
		winDimsXNumeric.TabIndex = 11;
		// 
		// statusStrip
		// 
		statusStrip.Items.AddRange(new ToolStripItem[] { showWinStatusBtn, redrawStatusBtn, rendererStatusCombo, calcWinSzStatusLabel });
		statusStrip.Location = new Point(0, 558);
		statusStrip.Name = "statusStrip";
		statusStrip.Size = new Size(838, 41);
		statusStrip.TabIndex = 13;
		statusStrip.Text = "statusStrip";
		// 
		// showWinStatusBtn
		// 
		showWinStatusBtn.BackColor = SystemColors.ControlDark;
		showWinStatusBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
		showWinStatusBtn.Image = (Image)resources.GetObject("showWinStatusBtn.Image");
		showWinStatusBtn.ImageTransparentColor = Color.Magenta;
		showWinStatusBtn.Margin = new Padding(5, 7, 5, 5);
		showWinStatusBtn.Name = "showWinStatusBtn";
		showWinStatusBtn.Padding = new Padding(5);
		showWinStatusBtn.ShowDropDownArrow = false;
		showWinStatusBtn.Size = new Size(50, 29);
		showWinStatusBtn.Text = "Show";
		// 
		// redrawStatusBtn
		// 
		redrawStatusBtn.BackColor = SystemColors.ControlDark;
		redrawStatusBtn.DisplayStyle = ToolStripItemDisplayStyle.Text;
		redrawStatusBtn.Image = (Image)resources.GetObject("redrawStatusBtn.Image");
		redrawStatusBtn.ImageTransparentColor = Color.Magenta;
		redrawStatusBtn.Margin = new Padding(5, 7, 5, 5);
		redrawStatusBtn.Name = "redrawStatusBtn";
		redrawStatusBtn.Padding = new Padding(5);
		redrawStatusBtn.ShowDropDownArrow = false;
		redrawStatusBtn.Size = new Size(60, 29);
		redrawStatusBtn.Text = "Redraw";
		// 
		// rendererStatusCombo
		// 
		rendererStatusCombo.DisplayStyle = ToolStripItemDisplayStyle.Text;
		rendererStatusCombo.DropDownItems.AddRange(new ToolStripItem[] { gdiplusStatusItem, direct2dStatusItem, direct2dindirect3dStatusItem });
		rendererStatusCombo.Image = (Image)resources.GetObject("rendererStatusCombo.Image");
		rendererStatusCombo.ImageTransparentColor = Color.Magenta;
		rendererStatusCombo.Name = "rendererStatusCombo";
		rendererStatusCombo.Size = new Size(67, 39);
		rendererStatusCombo.Text = "Renderer";
		// 
		// gdiplusStatusItem
		// 
		gdiplusStatusItem.Name = "gdiplusStatusItem";
		gdiplusStatusItem.Size = new Size(174, 22);
		gdiplusStatusItem.Text = "GDIPlus";
		// 
		// direct2dStatusItem
		// 
		direct2dStatusItem.Name = "direct2dStatusItem";
		direct2dStatusItem.Size = new Size(174, 22);
		direct2dStatusItem.Text = "Direct2D";
		// 
		// direct2dindirect3dStatusItem
		// 
		direct2dindirect3dStatusItem.Name = "direct2dindirect3dStatusItem";
		direct2dindirect3dStatusItem.Size = new Size(174, 22);
		direct2dindirect3dStatusItem.Text = "Direct2DInDirect3D";
		// 
		// calcWinSzStatusLabel
		// 
		calcWinSzStatusLabel.Name = "calcWinSzStatusLabel";
		calcWinSzStatusLabel.Size = new Size(72, 36);
		calcWinSzStatusLabel.Text = "Size 123x456";
		// 
		// winDimsXCheckBox
		// 
		winDimsXCheckBox.AutoSize = true;
		winDimsXCheckBox.Location = new Point(26, 27);
		winDimsXCheckBox.Name = "winDimsXCheckBox";
		winDimsXCheckBox.Size = new Size(15, 14);
		winDimsXCheckBox.TabIndex = 18;
		winDimsXCheckBox.UseVisualStyleBackColor = true;
		// 
		// groupBox1
		// 
		groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		groupBox1.Controls.Add(label3);
		groupBox1.Controls.Add(winDimsYCheckBox);
		groupBox1.Controls.Add(winDimsYNumeric);
		groupBox1.Controls.Add(label2);
		groupBox1.Controls.Add(winDimsXCheckBox);
		groupBox1.Controls.Add(winDimsXNumeric);
		groupBox1.Location = new Point(607, 6);
		groupBox1.Name = "groupBox1";
		groupBox1.Size = new Size(217, 89);
		groupBox1.TabIndex = 19;
		groupBox1.TabStop = false;
		groupBox1.Text = "Window dimensions";
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Location = new Point(6, 59);
		label3.Name = "label3";
		label3.Size = new Size(14, 15);
		label3.TabIndex = 19;
		label3.Text = "Y";
		// 
		// winDimsYCheckBox
		// 
		winDimsYCheckBox.AutoSize = true;
		winDimsYCheckBox.Location = new Point(26, 60);
		winDimsYCheckBox.Name = "winDimsYCheckBox";
		winDimsYCheckBox.Size = new Size(15, 14);
		winDimsYCheckBox.TabIndex = 21;
		winDimsYCheckBox.UseVisualStyleBackColor = true;
		// 
		// winDimsYNumeric
		// 
		winDimsYNumeric.Increment = new decimal(new int[] { 10, 0, 0, 0 });
		winDimsYNumeric.Location = new Point(47, 55);
		winDimsYNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		winDimsYNumeric.Name = "winDimsYNumeric";
		winDimsYNumeric.Size = new Size(57, 23);
		winDimsYNumeric.TabIndex = 20;
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(6, 26);
		label2.Name = "label2";
		label2.Size = new Size(14, 15);
		label2.TabIndex = 0;
		label2.Text = "X";
		// 
		// groupBox2
		// 
		groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		groupBox2.Controls.Add(nodeEditor);
		groupBox2.Location = new Point(607, 101);
		groupBox2.Name = "groupBox2";
		groupBox2.Size = new Size(217, 384);
		groupBox2.TabIndex = 20;
		groupBox2.TabStop = false;
		groupBox2.Text = "Node";
		// 
		// tabControl
		// 
		tabControl.Controls.Add(editTab);
		tabControl.Controls.Add(detailsTab);
		tabControl.Controls.Add(performanceTab);
		tabControl.Dock = DockStyle.Fill;
		tabControl.Location = new Point(0, 24);
		tabControl.Name = "tabControl";
		tabControl.SelectedIndex = 0;
		tabControl.Size = new Size(838, 534);
		tabControl.TabIndex = 24;
		// 
		// editTab
		// 
		editTab.Controls.Add(layoutTree);
		editTab.Controls.Add(groupBox1);
		editTab.Controls.Add(groupBox2);
		editTab.Location = new Point(4, 24);
		editTab.Name = "editTab";
		editTab.Padding = new Padding(3);
		editTab.Size = new Size(830, 506);
		editTab.TabIndex = 0;
		editTab.Text = "Edit";
		editTab.UseVisualStyleBackColor = true;
		// 
		// detailsTab
		// 
		detailsTab.Controls.Add(splitContainer);
		detailsTab.Location = new Point(4, 24);
		detailsTab.Name = "detailsTab";
		detailsTab.Padding = new Padding(3);
		detailsTab.Size = new Size(830, 506);
		detailsTab.TabIndex = 1;
		detailsTab.Text = "Details";
		detailsTab.UseVisualStyleBackColor = true;
		// 
		// splitContainer
		// 
		splitContainer.BorderStyle = BorderStyle.Fixed3D;
		splitContainer.Dock = DockStyle.Fill;
		splitContainer.Location = new Point(3, 3);
		splitContainer.Name = "splitContainer";
		// 
		// splitContainer.Panel1
		// 
		splitContainer.Panel1.Controls.Add(detailsTree);
		// 
		// splitContainer.Panel2
		// 
		splitContainer.Panel2.Controls.Add(detailsRichTextBox);
		splitContainer.Size = new Size(824, 500);
		splitContainer.SplitterDistance = 274;
		splitContainer.TabIndex = 1;
		// 
		// detailsTree
		// 
		detailsTree.CellEditUseWholeCell = false;
		detailsTree.Dock = DockStyle.Fill;
		detailsTree.Location = new Point(0, 0);
		detailsTree.Name = "detailsTree";
		detailsTree.ShowGroups = false;
		detailsTree.Size = new Size(270, 496);
		detailsTree.TabIndex = 0;
		detailsTree.View = View.Details;
		detailsTree.VirtualMode = true;
		// 
		// detailsRichTextBox
		// 
		detailsRichTextBox.BackColor = Color.FromArgb(0, 0, 64);
		detailsRichTextBox.Dock = DockStyle.Fill;
		detailsRichTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
		detailsRichTextBox.ForeColor = Color.White;
		detailsRichTextBox.Location = new Point(0, 0);
		detailsRichTextBox.Name = "detailsRichTextBox";
		detailsRichTextBox.ReadOnly = true;
		detailsRichTextBox.Size = new Size(542, 496);
		detailsRichTextBox.TabIndex = 0;
		detailsRichTextBox.Text = "";
		// 
		// performanceTab
		// 
		performanceTab.Location = new Point(4, 24);
		performanceTab.Name = "performanceTab";
		performanceTab.Size = new Size(830, 506);
		performanceTab.TabIndex = 2;
		performanceTab.Text = "Performance";
		performanceTab.UseVisualStyleBackColor = true;
		// 
		// renderingRendererItem
		// 
		renderingRendererItem.DropDownItems.AddRange(new ToolStripItem[] { renderingRendererGDIPlusItem, renderingRendererDirect2DItem, renderingRendererDirect3DInDirect2DItem });
		renderingRendererItem.Name = "renderingRendererItem";
		renderingRendererItem.Size = new Size(180, 22);
		renderingRendererItem.Text = "&Renderer...";
		// 
		// renderingRendererGDIPlusItem
		// 
		renderingRendererGDIPlusItem.Name = "renderingRendererGDIPlusItem";
		renderingRendererGDIPlusItem.Size = new Size(180, 22);
		renderingRendererGDIPlusItem.Text = "GDIPlus";
		// 
		// renderingRendererDirect2DItem
		// 
		renderingRendererDirect2DItem.Name = "renderingRendererDirect2DItem";
		renderingRendererDirect2DItem.Size = new Size(180, 22);
		renderingRendererDirect2DItem.Text = "Direct2D";
		// 
		// renderingRendererDirect3DInDirect2DItem
		// 
		renderingRendererDirect3DInDirect2DItem.Name = "renderingRendererDirect3DInDirect2DItem";
		renderingRendererDirect3DInDirect2DItem.Size = new Size(180, 22);
		renderingRendererDirect3DInDirect2DItem.Text = "Direct3DInDirect2D";
		// 
		// MainWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(838, 599);
		Controls.Add(tabControl);
		Controls.Add(statusStrip);
		Controls.Add(menuStrip);
		MainMenuStrip = menuStrip;
		MinimumSize = new Size(781, 614);
		Name = "MainWin";
		SizeGripStyle = SizeGripStyle.Hide;
		Text = "Flex Builder";
		((System.ComponentModel.ISupportInitialize)layoutTree).EndInit();
		layoutTreeContextMenu.ResumeLayout(false);
		menuStrip.ResumeLayout(false);
		menuStrip.PerformLayout();
		((System.ComponentModel.ISupportInitialize)winDimsXNumeric).EndInit();
		statusStrip.ResumeLayout(false);
		statusStrip.PerformLayout();
		groupBox1.ResumeLayout(false);
		groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)winDimsYNumeric).EndInit();
		groupBox2.ResumeLayout(false);
		tabControl.ResumeLayout(false);
		editTab.ResumeLayout(false);
		detailsTab.ResumeLayout(false);
		splitContainer.Panel1.ResumeLayout(false);
		splitContainer.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
		splitContainer.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)detailsTree).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion
	public ContextMenuStrip layoutTreeContextMenu;
	public ToolStripMenuItem addFillMenuItem;
	public ToolStripMenuItem addStackMenuItem;
	public ToolStripMenuItem addWrapMenuItem;
	public BrightIdeasSoftware.TreeListView layoutTree;
	public ToolStripMenuItem removeNodeMenuItem;
	public Editors.NodeEditor nodeEditor;
	private MenuStrip menuStrip;
	private ToolStripMenuItem fileToolStripMenuItem;
	private ToolStripSeparator toolStripSeparator1;
	public ToolStripMenuItem fileOpenItem;
	public ToolStripMenuItem fileSaveItem;
	public ToolStripMenuItem fileSaveAsItem;
	public ToolStripMenuItem fileExitItem;
	public NumericUpDown winDimsXNumeric;
	public ToolStripMenuItem fileNewItem;
	public StatusStrip statusStrip;
	private GroupBox groupBox1;
	private Label label2;
	private Label label3;
	public NumericUpDown winDimsYNumeric;
	private GroupBox groupBox2;
	public CheckBox winDimsXCheckBox;
	public CheckBox winDimsYCheckBox;
	public TabControl tabControl;
	private TabPage editTab;
	private TabPage detailsTab;
	public ToolStripDropDownButton redrawStatusBtn;
	public ToolStripMenuItem gdiplusStatusItem;
	public ToolStripMenuItem direct2dStatusItem;
	public ToolStripMenuItem direct2dindirect3dStatusItem;
	public SplitContainer splitContainer;
	public BrightIdeasSoftware.TreeListView detailsTree;
	public ToolStripDropDownButton rendererStatusCombo;
	public ToolStripStatusLabel calcWinSzStatusLabel;
	public ToolStripDropDownButton showWinStatusBtn;
	public RichTextBox detailsRichTextBox;
	private TabPage performanceTab;
	private ToolStripMenuItem renderingToolStripMenuItem;
	public ToolStripMenuItem renderingEnabledItem;
	public ToolStripMenuItem renderingRedrawItem;
	public ToolStripMenuItem renderingWindowSizeItem;
	private ToolStripMenuItem renderingRendererItem;
	public ToolStripMenuItem renderingRendererGDIPlusItem;
	public ToolStripMenuItem renderingRendererDirect2DItem;
	public ToolStripMenuItem renderingRendererDirect3DInDirect2DItem;
}
