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
		winList = new ListBox();
		label1 = new Label();
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
		label1.Location = new Point(12, 9);
		label1.Name = "label1";
		label1.Size = new Size(120, 15);
		label1.TabIndex = 1;
		label1.Text = "Windows";
		label1.TextAlign = ContentAlignment.TopCenter;
		// 
		// WinSpectorWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(label1);
		Controls.Add(winList);
		Name = "WinSpectorWin";
		Text = "WinSpector";
		ResumeLayout(false);
	}

	#endregion

	public ListBox winList;
	private Label label1;
}