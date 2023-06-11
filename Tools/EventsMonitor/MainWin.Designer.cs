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
		basicControl = new Controls.BasicControl();
		ctrlEventDisplayer = new Controls.EventDisplayer();
		label2 = new Label();
		dxFormEventDisplayer = new Controls.DXEventDisplayer();
		label3 = new Label();
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
		formEventDisplayer.Location = new Point(12, 47);
		formEventDisplayer.Name = "formEventDisplayer";
		formEventDisplayer.Size = new Size(250, 330);
		formEventDisplayer.TabIndex = 2;
		// 
		// basicControl
		// 
		basicControl.Location = new Point(580, 47);
		basicControl.Name = "basicControl";
		basicControl.Size = new Size(150, 150);
		basicControl.TabIndex = 3;
		// 
		// ctrlEventDisplayer
		// 
		ctrlEventDisplayer.Location = new Point(268, 47);
		ctrlEventDisplayer.Name = "ctrlEventDisplayer";
		ctrlEventDisplayer.Size = new Size(250, 330);
		ctrlEventDisplayer.TabIndex = 4;
		// 
		// label2
		// 
		label2.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
		label2.Location = new Point(268, 9);
		label2.Name = "label2";
		label2.Size = new Size(250, 35);
		label2.TabIndex = 1;
		label2.Text = "Ctrl";
		label2.TextAlign = ContentAlignment.TopCenter;
		// 
		// dxFormEventDisplayer
		// 
		dxFormEventDisplayer.Location = new Point(12, 445);
		dxFormEventDisplayer.Name = "dxFormEventDisplayer";
		dxFormEventDisplayer.Size = new Size(250, 330);
		dxFormEventDisplayer.TabIndex = 5;
		// 
		// label3
		// 
		label3.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
		label3.Location = new Point(12, 407);
		label3.Name = "label3";
		label3.Size = new Size(250, 35);
		label3.TabIndex = 1;
		label3.Text = "DX Form";
		label3.TextAlign = ContentAlignment.TopCenter;
		// 
		// MainWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 790);
		Controls.Add(dxFormEventDisplayer);
		Controls.Add(ctrlEventDisplayer);
		Controls.Add(basicControl);
		Controls.Add(formEventDisplayer);
		Controls.Add(label2);
		Controls.Add(label3);
		Controls.Add(label1);
		KeyPreview = true;
		Name = "MainWin";
		Text = "Events Monitor";
		ResumeLayout(false);
	}

	#endregion
	private Label label1;
	private Controls.EventDisplayer formEventDisplayer;
	private Controls.BasicControl basicControl;
	private Controls.EventDisplayer ctrlEventDisplayer;
	private Label label2;
	private Controls.DXEventDisplayer dxFormEventDisplayer;
	private Label label3;
}
