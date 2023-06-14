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
		label1 = new Label();
		label2 = new Label();
		layoutTree = new BrightIdeasSoftware.TreeListView();
		((System.ComponentModel.ISupportInitialize)layoutTree).BeginInit();
		SuspendLayout();
		// 
		// winList
		// 
		winList.FormattingEnabled = true;
		winList.ItemHeight = 15;
		winList.Location = new Point(12, 27);
		winList.Name = "winList";
		winList.Size = new Size(120, 94);
		winList.TabIndex = 0;
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(12, 9);
		label1.Name = "label1";
		label1.Size = new Size(56, 15);
		label1.TabIndex = 1;
		label1.Text = "Windows";
		label1.TextAlign = ContentAlignment.TopCenter;
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(12, 133);
		label2.Name = "label2";
		label2.Size = new Size(90, 15);
		label2.TabIndex = 3;
		label2.Text = "Window Layout";
		label2.TextAlign = ContentAlignment.TopCenter;
		// 
		// layoutTree
		// 
		layoutTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		layoutTree.CellEditUseWholeCell = false;
		layoutTree.Location = new Point(11, 151);
		layoutTree.Name = "layoutTree";
		layoutTree.ShowGroups = false;
		layoutTree.Size = new Size(777, 287);
		layoutTree.TabIndex = 4;
		layoutTree.View = View.Details;
		layoutTree.VirtualMode = true;
		// 
		// WinSpectorWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(layoutTree);
		Controls.Add(label2);
		Controls.Add(label1);
		Controls.Add(winList);
		Name = "WinSpectorWin";
		Text = "WinSpector";
		((System.ComponentModel.ISupportInitialize)layoutTree).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	public ListBox winList;
	private Label label1;
	private Label label2;
	public BrightIdeasSoftware.TreeListView layoutTree;
}