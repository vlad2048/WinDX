namespace WinSpectorLib.Controls;

partial class EventDisplayer
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

	#region Component Designer generated code

	/// <summary> 
	/// Required method for Designer support - do not modify 
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{
		eventListBox = new FlickerFreeListBox();
		SuspendLayout();
		// 
		// eventListBox
		// 
		eventListBox.Dock = DockStyle.Fill;
		eventListBox.DrawMode = DrawMode.OwnerDrawFixed;
		eventListBox.FormattingEnabled = true;
		eventListBox.IntegralHeight = false;
		eventListBox.ItemHeight = 19;
		eventListBox.Location = new Point(0, 0);
		eventListBox.Name = "eventListBox";
		eventListBox.Size = new Size(250, 250);
		eventListBox.TabIndex = 0;
		// 
		// EventDisplayer
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		Controls.Add(eventListBox);
		Name = "EventDisplayer";
		Size = new Size(250, 250);
		ResumeLayout(false);
	}

	#endregion

	private FlickerFreeListBox eventListBox;
}
