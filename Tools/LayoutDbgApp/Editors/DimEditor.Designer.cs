namespace LayoutDbgApp.Editors;

partial class DimEditor
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
		dirLabel = new Label();
		typCombo = new ComboBox();
		minNumeric = new NumericUpDown();
		maxNumeric = new NumericUpDown();
		((System.ComponentModel.ISupportInitialize)minNumeric).BeginInit();
		((System.ComponentModel.ISupportInitialize)maxNumeric).BeginInit();
		SuspendLayout();
		// 
		// dirLabel
		// 
		dirLabel.AutoSize = true;
		dirLabel.Location = new Point(1, 6);
		dirLabel.Name = "dirLabel";
		dirLabel.Size = new Size(14, 15);
		dirLabel.TabIndex = 0;
		dirLabel.Text = "X";
		// 
		// typCombo
		// 
		typCombo.FormattingEnabled = true;
		typCombo.Items.AddRange(new object[] { "Fix", "Flt", "Fil", "Fit" });
		typCombo.Location = new Point(21, 3);
		typCombo.Name = "typCombo";
		typCombo.Size = new Size(50, 23);
		typCombo.TabIndex = 1;
		// 
		// minNumeric
		// 
		minNumeric.Increment = new decimal(new int[] { 10, 0, 0, 0 });
		minNumeric.Location = new Point(77, 3);
		minNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		minNumeric.Name = "minNumeric";
		minNumeric.Size = new Size(57, 23);
		minNumeric.TabIndex = 2;
		// 
		// maxNumeric
		// 
		maxNumeric.Increment = new decimal(new int[] { 10, 0, 0, 0 });
		maxNumeric.Location = new Point(140, 3);
		maxNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		maxNumeric.Name = "maxNumeric";
		maxNumeric.Size = new Size(57, 23);
		maxNumeric.TabIndex = 3;
		// 
		// DimEditor
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		Controls.Add(maxNumeric);
		Controls.Add(minNumeric);
		Controls.Add(typCombo);
		Controls.Add(dirLabel);
		Name = "DimEditor";
		Size = new Size(200, 29);
		((System.ComponentModel.ISupportInitialize)minNumeric).EndInit();
		((System.ComponentModel.ISupportInitialize)maxNumeric).EndInit();
		ResumeLayout(false);
		PerformLayout();
	}

	#endregion

	private Label dirLabel;
	private ComboBox typCombo;
	private NumericUpDown minNumeric;
	private NumericUpDown maxNumeric;
}
