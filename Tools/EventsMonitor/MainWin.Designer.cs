namespace EventsMonitor;

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
		label1 = new Label();
		formEventDisplayer = new Controls.EventDisplayer();
		SuspendLayout();
		// 
		// label1
		// 
		label1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
		label1.Location = new Point(12, 9);
		label1.Name = "label1";
		label1.Size = new Size(250, 35);
		label1.TabIndex = 1;
		label1.Text = "Form";
		label1.TextAlign = ContentAlignment.TopCenter;
		// 
		// formEventDisplayer
		// 
		formEventDisplayer.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
		formEventDisplayer.Location = new Point(12, 47);
		formEventDisplayer.Name = "formEventDisplayer";
		formEventDisplayer.Size = new Size(250, 391);
		formEventDisplayer.TabIndex = 2;
		// 
		// MainWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 450);
		Controls.Add(formEventDisplayer);
		Controls.Add(label1);
		Name = "MainWin";
		Text = "Events Monitor";
		ResumeLayout(false);
	}

	#endregion
	private Label label1;
	private Controls.EventDisplayer formEventDisplayer;
}
