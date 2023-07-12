namespace WinSpectorLib.Controls;

partial class ResizeDialog
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
		label1 = new Label();
		label2 = new Label();
		widthUpDown = new NumericUpDown();
		heightUpDown = new NumericUpDown();
		addBtn = new Button();
		subBtn = new Button();
		closeBtn = new Button();
		widthLabel = new Label();
		heightLabel = new Label();
		((System.ComponentModel.ISupportInitialize)widthUpDown).BeginInit();
		((System.ComponentModel.ISupportInitialize)heightUpDown).BeginInit();
		SuspendLayout();
		// 
		// label1
		// 
		label1.AutoSize = true;
		label1.Location = new Point(12, 9);
		label1.Name = "label1";
		label1.Size = new Size(42, 15);
		label1.TabIndex = 0;
		label1.Text = "Width:";
		// 
		// label2
		// 
		label2.AutoSize = true;
		label2.Location = new Point(12, 38);
		label2.Name = "label2";
		label2.Size = new Size(46, 15);
		label2.TabIndex = 1;
		label2.Text = "Height:";
		// 
		// widthUpDown
		// 
		widthUpDown.Location = new Point(64, 7);
		widthUpDown.Name = "widthUpDown";
		widthUpDown.Size = new Size(56, 23);
		widthUpDown.TabIndex = 7;
		// 
		// heightUpDown
		// 
		heightUpDown.Location = new Point(64, 36);
		heightUpDown.Name = "heightUpDown";
		heightUpDown.Size = new Size(56, 23);
		heightUpDown.TabIndex = 8;
		// 
		// addBtn
		// 
		addBtn.Location = new Point(12, 65);
		addBtn.Name = "addBtn";
		addBtn.Size = new Size(57, 23);
		addBtn.TabIndex = 4;
		addBtn.Text = "Add (+)";
		addBtn.UseVisualStyleBackColor = true;
		// 
		// subBtn
		// 
		subBtn.Location = new Point(75, 65);
		subBtn.Name = "subBtn";
		subBtn.Size = new Size(57, 23);
		subBtn.TabIndex = 5;
		subBtn.Text = "Sub (-)";
		subBtn.UseVisualStyleBackColor = true;
		// 
		// closeBtn
		// 
		closeBtn.DialogResult = DialogResult.OK;
		closeBtn.Location = new Point(138, 65);
		closeBtn.Name = "closeBtn";
		closeBtn.Size = new Size(57, 23);
		closeBtn.TabIndex = 6;
		closeBtn.Text = "Close";
		closeBtn.UseVisualStyleBackColor = true;
		// 
		// widthLabel
		// 
		widthLabel.AutoSize = true;
		widthLabel.Location = new Point(148, 9);
		widthLabel.Name = "widthLabel";
		widthLabel.Size = new Size(25, 15);
		widthLabel.TabIndex = 2;
		widthLabel.Text = "250";
		// 
		// heightLabel
		// 
		heightLabel.AutoSize = true;
		heightLabel.Location = new Point(148, 38);
		heightLabel.Name = "heightLabel";
		heightLabel.Size = new Size(25, 15);
		heightLabel.TabIndex = 3;
		heightLabel.Text = "300";
		// 
		// ResizeDialog
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		ClientSize = new Size(207, 100);
		Controls.Add(heightLabel);
		Controls.Add(widthLabel);
		Controls.Add(closeBtn);
		Controls.Add(subBtn);
		Controls.Add(addBtn);
		Controls.Add(heightUpDown);
		Controls.Add(widthUpDown);
		Controls.Add(label2);
		Controls.Add(label1);
		KeyPreview = true;
		Name = "ResizeDialog";
		Text = "Resize Window";
		((System.ComponentModel.ISupportInitialize)widthUpDown).EndInit();
		((System.ComponentModel.ISupportInitialize)heightUpDown).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Label label1;
	private Label label2;
	private NumericUpDown widthUpDown;
	private NumericUpDown heightUpDown;
	private Button addBtn;
	private Button subBtn;
	private Button closeBtn;
	private Label widthLabel;
	private Label heightLabel;
}