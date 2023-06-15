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
		layoutTree = new BrightIdeasSoftware.TreeListView();
		layoutTreeContextMenu = new ContextMenuStrip(components);
		addFillMenuItem = new ToolStripMenuItem();
		addStackMenuItem = new ToolStripMenuItem();
		addWrapMenuItem = new ToolStripMenuItem();
		removeNodeMenuItem = new ToolStripMenuItem();
		nodeEditor = new Editors.NodeEditor();
		menuStrip = new MenuStrip();
		fileToolStripMenuItem = new ToolStripMenuItem();
		newToolStripMenuItem = new ToolStripMenuItem();
		openToolStripMenuItem = new ToolStripMenuItem();
		saveToolStripMenuItem = new ToolStripMenuItem();
		saveAsToolStripMenuItem = new ToolStripMenuItem();
		toolStripSeparator1 = new ToolStripSeparator();
		quitToolStripMenuItem = new ToolStripMenuItem();
		label1 = new Label();
		winDimsXNumeric = new NumericUpDown();
		statusStrip = new StatusStrip();
		showWinBtn = new Button();
		rendererCombo = new ComboBox();
		redrawBtn = new Button();
		winDimsXCheckBox = new CheckBox();
		groupBox1 = new GroupBox();
		label3 = new Label();
		winDimsYCheckBox = new CheckBox();
		winDimsYNumeric = new NumericUpDown();
		label2 = new Label();
		groupBox2 = new GroupBox();
		calcWinSzLabel = new Label();
		((System.ComponentModel.ISupportInitialize)layoutTree).BeginInit();
		layoutTreeContextMenu.SuspendLayout();
		menuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)winDimsXNumeric).BeginInit();
		groupBox1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)winDimsYNumeric).BeginInit();
		groupBox2.SuspendLayout();
		SuspendLayout();
		// 
		// layoutTree
		// 
		layoutTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		layoutTree.CellEditUseWholeCell = false;
		layoutTree.ContextMenuStrip = layoutTreeContextMenu;
		layoutTree.Location = new Point(12, 56);
		layoutTree.Name = "layoutTree";
		layoutTree.ShowGroups = false;
		layoutTree.Size = new Size(531, 410);
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
		nodeEditor.Size = new Size(203, 290);
		nodeEditor.TabIndex = 8;
		// 
		// menuStrip
		// 
		menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
		menuStrip.Location = new Point(0, 0);
		menuStrip.Name = "menuStrip";
		menuStrip.Size = new Size(778, 24);
		menuStrip.TabIndex = 9;
		menuStrip.Text = "menuStrip1";
		// 
		// fileToolStripMenuItem
		// 
		fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, quitToolStripMenuItem });
		fileToolStripMenuItem.Name = "fileToolStripMenuItem";
		fileToolStripMenuItem.Size = new Size(37, 20);
		fileToolStripMenuItem.Text = "&File";
		// 
		// newToolStripMenuItem
		// 
		newToolStripMenuItem.Name = "newToolStripMenuItem";
		newToolStripMenuItem.Size = new Size(123, 22);
		newToolStripMenuItem.Text = "&New";
		// 
		// openToolStripMenuItem
		// 
		openToolStripMenuItem.Name = "openToolStripMenuItem";
		openToolStripMenuItem.Size = new Size(123, 22);
		openToolStripMenuItem.Text = "&Open...";
		// 
		// saveToolStripMenuItem
		// 
		saveToolStripMenuItem.Name = "saveToolStripMenuItem";
		saveToolStripMenuItem.Size = new Size(123, 22);
		saveToolStripMenuItem.Text = "&Save";
		// 
		// saveAsToolStripMenuItem
		// 
		saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
		saveAsToolStripMenuItem.Size = new Size(123, 22);
		saveAsToolStripMenuItem.Text = "Save &As...";
		// 
		// toolStripSeparator1
		// 
		toolStripSeparator1.Name = "toolStripSeparator1";
		toolStripSeparator1.Size = new Size(120, 6);
		// 
		// quitToolStripMenuItem
		// 
		quitToolStripMenuItem.Name = "quitToolStripMenuItem";
		quitToolStripMenuItem.Size = new Size(123, 22);
		quitToolStripMenuItem.Text = "E&xit";
		// 
		// label1
		// 
		label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		label1.AutoSize = true;
		label1.Location = new Point(549, 31);
		label1.Name = "label1";
		label1.Size = new Size(55, 15);
		label1.TabIndex = 10;
		label1.Text = "WinDims";
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
		statusStrip.Location = new Point(0, 469);
		statusStrip.Name = "statusStrip";
		statusStrip.Size = new Size(778, 22);
		statusStrip.TabIndex = 13;
		statusStrip.Text = "statusStrip";
		// 
		// showWinBtn
		// 
		showWinBtn.Location = new Point(12, 27);
		showWinBtn.Name = "showWinBtn";
		showWinBtn.Size = new Size(118, 23);
		showWinBtn.TabIndex = 15;
		showWinBtn.Text = "Show Window";
		showWinBtn.UseVisualStyleBackColor = true;
		// 
		// rendererCombo
		// 
		rendererCombo.FormattingEnabled = true;
		rendererCombo.Items.AddRange(new object[] { "GDIPlus", "Direct2D", "Direct2DInDirect3D" });
		rendererCombo.Location = new Point(136, 27);
		rendererCombo.Name = "rendererCombo";
		rendererCombo.Size = new Size(121, 23);
		rendererCombo.TabIndex = 16;
		// 
		// redrawBtn
		// 
		redrawBtn.Location = new Point(263, 26);
		redrawBtn.Name = "redrawBtn";
		redrawBtn.Size = new Size(75, 23);
		redrawBtn.TabIndex = 17;
		redrawBtn.Text = "Redraw";
		redrawBtn.UseVisualStyleBackColor = true;
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
		groupBox1.Location = new Point(549, 56);
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
		groupBox2.Location = new Point(549, 151);
		groupBox2.Name = "groupBox2";
		groupBox2.Size = new Size(217, 314);
		groupBox2.TabIndex = 20;
		groupBox2.TabStop = false;
		groupBox2.Text = "Node";
		// 
		// calcWinSzLabel
		// 
		calcWinSzLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
		calcWinSzLabel.AutoSize = true;
		calcWinSzLabel.Location = new Point(640, 31);
		calcWinSzLabel.Name = "calcWinSzLabel";
		calcWinSzLabel.Size = new Size(49, 15);
		calcWinSzLabel.TabIndex = 22;
		calcWinSzLabel.Text = "123x456";
		// 
		// MainWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(778, 491);
		Controls.Add(calcWinSzLabel);
		Controls.Add(groupBox2);
		Controls.Add(redrawBtn);
		Controls.Add(groupBox1);
		Controls.Add(rendererCombo);
		Controls.Add(showWinBtn);
		Controls.Add(statusStrip);
		Controls.Add(menuStrip);
		Controls.Add(label1);
		Controls.Add(layoutTree);
		MainMenuStrip = menuStrip;
		Name = "MainWin";
		SizeGripStyle = SizeGripStyle.Hide;
		Text = "Layout Debugger";
		((System.ComponentModel.ISupportInitialize)layoutTree).EndInit();
		layoutTreeContextMenu.ResumeLayout(false);
		menuStrip.ResumeLayout(false);
		menuStrip.PerformLayout();
		((System.ComponentModel.ISupportInitialize)winDimsXNumeric).EndInit();
		groupBox1.ResumeLayout(false);
		groupBox1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)winDimsYNumeric).EndInit();
		groupBox2.ResumeLayout(false);
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
	private Label label1;
	public ToolStripMenuItem openToolStripMenuItem;
	public ToolStripMenuItem saveToolStripMenuItem;
	public ToolStripMenuItem saveAsToolStripMenuItem;
	public ToolStripMenuItem quitToolStripMenuItem;
	public NumericUpDown winDimsXNumeric;
	public ToolStripMenuItem newToolStripMenuItem;
	public StatusStrip statusStrip;
	public ComboBox rendererCombo;
	public Button showWinBtn;
	public Button redrawBtn;
	private GroupBox groupBox1;
	private Label label2;
	private Label label3;
	public NumericUpDown winDimsYNumeric;
	private GroupBox groupBox2;
	public CheckBox winDimsXCheckBox;
	public CheckBox winDimsYCheckBox;
	public Label calcWinSzLabel;
}
