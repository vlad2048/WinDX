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
		basicControl1 = new Controls.BasicControl();
		ctrlEventDisplayer1 = new Controls.EventDisplayer();
		label2 = new Label();
		dxFormEventDisplayer = new Controls.DXEventDisplayer();
		label3 = new Label();
		ctrlEventDisplayer2 = new Controls.EventDisplayer();
		basicControl2 = new Controls.BasicControl();
		label4 = new Label();
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
		// basicControl1
		// 
		basicControl1.Location = new Point(580, 47);
		basicControl1.Name = "basicControl1";
		basicControl1.Size = new Size(150, 150);
		basicControl1.TabIndex = 3;
		// 
		// ctrlEventDisplayer1
		// 
		ctrlEventDisplayer1.Location = new Point(268, 47);
		ctrlEventDisplayer1.Name = "ctrlEventDisplayer1";
		ctrlEventDisplayer1.Size = new Size(250, 330);
		ctrlEventDisplayer1.TabIndex = 4;
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
		// ctrlEventDisplayer2
		// 
		ctrlEventDisplayer2.Location = new Point(268, 445);
		ctrlEventDisplayer2.Name = "ctrlEventDisplayer2";
		ctrlEventDisplayer2.Size = new Size(250, 330);
		ctrlEventDisplayer2.TabIndex = 8;
		// 
		// basicControl2
		// 
		basicControl2.Location = new Point(580, 445);
		basicControl2.Name = "basicControl2";
		basicControl2.Size = new Size(150, 150);
		basicControl2.TabIndex = 7;
		// 
		// label4
		// 
		label4.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
		label4.Location = new Point(268, 407);
		label4.Name = "label4";
		label4.Size = new Size(250, 35);
		label4.TabIndex = 6;
		label4.Text = "Ctrl";
		label4.TextAlign = ContentAlignment.TopCenter;
		// 
		// MainWin
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(800, 790);
		Controls.Add(ctrlEventDisplayer2);
		Controls.Add(basicControl2);
		Controls.Add(label4);
		Controls.Add(dxFormEventDisplayer);
		Controls.Add(ctrlEventDisplayer1);
		Controls.Add(basicControl1);
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
	private Controls.BasicControl basicControl1;
	private Controls.EventDisplayer ctrlEventDisplayer1;
	private Label label2;
	private Controls.DXEventDisplayer dxFormEventDisplayer;
	private Label label3;
	private Controls.EventDisplayer ctrlEventDisplayer2;
	private Controls.BasicControl basicControl2;
	private Label label4;
}
