﻿namespace WinSpectorLib.Controls;

partial class ScrollDimStateDisplayer
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
		label1 = new Label();
		label2 = new Label();
		label3 = new Label();
		label4 = new Label();
		label5 = new Label();
		label6 = new Label();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(3, 0);
		label1.Name = "label1";
		label1.Size = new Size(35, 16);
		label1.TabIndex = 0;
		label1.Text = "View";
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(3, 16);
		label2.Name = "label2";
		label2.Size = new Size(35, 16);
		label2.TabIndex = 1;
		label2.Text = "Cont";
		// 
		// label3
		// 
		label3.AutoSize = true;
		label3.Location = new Point(3, 32);
		label3.Name = "label3";
		label3.Size = new Size(35, 16);
		label3.TabIndex = 2;
		label3.Text = "Trak";
		// 
		// label4
		// 
		label4.AutoSize = true;
		label4.Location = new Point(3, 58);
		label4.Name = "label4";
		label4.Size = new Size(70, 16);
		label4.TabIndex = 3;
		label4.Text = "ScrollOfs";
		// 
		// label5
		// 
		label5.AutoSize = true;
		label5.Location = new Point(3, 74);
		label5.Name = "label5";
		label5.Size = new Size(70, 16);
		label5.TabIndex = 4;
		label5.Text = "IsTailing";
		// 
		// label6
		// 
		label6.AutoSize = true;
		label6.Location = new Point(146, 58);
		label6.Name = "label6";
		label6.Size = new Size(28, 16);
		label6.TabIndex = 5;
		label6.Text = "max";
		// 
		// ScrollDimStateDisplayer
		// 
		AutoScaleDimensions = new SizeF(7F, 16F);
		AutoScaleMode = AutoScaleMode.Font;
		Controls.Add(label6);
		Controls.Add(label5);
		Controls.Add(label4);
		Controls.Add(label3);
		Controls.Add(label2);
		Controls.Add(label1);
		Font = new Font("Cascadia Mono", 9F, FontStyle.Regular, GraphicsUnit.Point);
		Name = "ScrollDimStateDisplayer";
		Size = new Size(240, 160);
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Label label1;
	private Label label2;
	private Label label3;
	private Label label4;
	private Label label5;
	private Label label6;
}
