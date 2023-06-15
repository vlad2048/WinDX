namespace FlexBuilder.Editors;

partial class NodeEditor
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
		horzDimEditor = new DimEditor();
		vertDimEditor = new DimEditor();
		stratCombo = new ComboBox();
		stratDirCombo = new ComboBox();
		stratAlignCombo = new ComboBox();
		groupBox1 = new GroupBox();
		specScrollYCheckBox = new CheckBox();
		specScrollXCheckBox = new CheckBox();
		groupBox2 = new GroupBox();
		margMinusBtn = new Button();
		margPlusBtn = new Button();
		margRightNumeric = new NumericUpDown();
		margDownNumeric = new NumericUpDown();
		margLeftNumeric = new NumericUpDown();
		margUpNumeric = new NumericUpDown();
		specGroupBox = new GroupBox();
		specTypeCombo = new ComboBox();
		groupBox1.SuspendLayout();
		groupBox2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)margRightNumeric).BeginInit();
		((System.ComponentModel.ISupportInitialize)margDownNumeric).BeginInit();
		((System.ComponentModel.ISupportInitialize)margLeftNumeric).BeginInit();
		((System.ComponentModel.ISupportInitialize)margUpNumeric).BeginInit();
		specGroupBox.SuspendLayout();
		SuspendLayout();
		// 
		// horzDimEditor
		// 
		horzDimEditor.Dir = PowBasics.Geom.Dir.Horz;
		horzDimEditor.Location = new Point(3, 3);
		horzDimEditor.Name = "horzDimEditor";
		horzDimEditor.Size = new Size(200, 29);
		horzDimEditor.TabIndex = 0;
		// 
		// vertDimEditor
		// 
		vertDimEditor.Dir = PowBasics.Geom.Dir.Vert;
		vertDimEditor.Location = new Point(3, 38);
		vertDimEditor.Name = "vertDimEditor";
		vertDimEditor.Size = new Size(200, 29);
		vertDimEditor.TabIndex = 1;
		// 
		// stratCombo
		// 
		stratCombo.FormattingEnabled = true;
		stratCombo.Items.AddRange(new object[] { "Fill", "Stack", "Wrap" });
		stratCombo.Location = new Point(6, 22);
		stratCombo.Name = "stratCombo";
		stratCombo.Size = new Size(80, 23);
		stratCombo.TabIndex = 2;
		// 
		// stratDirCombo
		// 
		stratDirCombo.FormattingEnabled = true;
		stratDirCombo.Items.AddRange(new object[] { "Horz", "Vert" });
		stratDirCombo.Location = new Point(6, 51);
		stratDirCombo.Name = "stratDirCombo";
		stratDirCombo.Size = new Size(80, 23);
		stratDirCombo.TabIndex = 3;
		// 
		// stratAlignCombo
		// 
		stratAlignCombo.FormattingEnabled = true;
		stratAlignCombo.Items.AddRange(new object[] { "Start", "Middle", "End", "Stretch" });
		stratAlignCombo.Location = new Point(92, 51);
		stratAlignCombo.Name = "stratAlignCombo";
		stratAlignCombo.Size = new Size(80, 23);
		stratAlignCombo.TabIndex = 4;
		// 
		// groupBox1
		// 
		groupBox1.Controls.Add(specGroupBox);
		groupBox1.Controls.Add(stratCombo);
		groupBox1.Controls.Add(stratAlignCombo);
		groupBox1.Controls.Add(stratDirCombo);
		groupBox1.Location = new Point(3, 185);
		groupBox1.Name = "groupBox1";
		groupBox1.Size = new Size(200, 161);
		groupBox1.TabIndex = 5;
		groupBox1.TabStop = false;
		groupBox1.Text = "Strat";
		// 
		// specScrollYCheckBox
		// 
		specScrollYCheckBox.AutoSize = true;
		specScrollYCheckBox.Location = new Point(101, 47);
		specScrollYCheckBox.Name = "specScrollYCheckBox";
		specScrollYCheckBox.Size = new Size(65, 19);
		specScrollYCheckBox.TabIndex = 6;
		specScrollYCheckBox.Text = "Scroll Y";
		specScrollYCheckBox.UseVisualStyleBackColor = true;
		// 
		// specScrollXCheckBox
		// 
		specScrollXCheckBox.AutoSize = true;
		specScrollXCheckBox.Location = new Point(101, 22);
		specScrollXCheckBox.Name = "specScrollXCheckBox";
		specScrollXCheckBox.Size = new Size(65, 19);
		specScrollXCheckBox.TabIndex = 5;
		specScrollXCheckBox.Text = "Scroll X";
		specScrollXCheckBox.UseVisualStyleBackColor = true;
		// 
		// groupBox2
		// 
		groupBox2.Controls.Add(margMinusBtn);
		groupBox2.Controls.Add(margPlusBtn);
		groupBox2.Controls.Add(margRightNumeric);
		groupBox2.Controls.Add(margDownNumeric);
		groupBox2.Controls.Add(margLeftNumeric);
		groupBox2.Controls.Add(margUpNumeric);
		groupBox2.Location = new Point(3, 73);
		groupBox2.Name = "groupBox2";
		groupBox2.Size = new Size(200, 106);
		groupBox2.TabIndex = 6;
		groupBox2.TabStop = false;
		groupBox2.Text = "Margin";
		// 
		// margMinusBtn
		// 
		margMinusBtn.FlatStyle = FlatStyle.Flat;
		margMinusBtn.Font = new Font("Consolas", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
		margMinusBtn.Location = new Point(119, 28);
		margMinusBtn.Margin = new Padding(0);
		margMinusBtn.Name = "margMinusBtn";
		margMinusBtn.Size = new Size(38, 54);
		margMinusBtn.TabIndex = 2;
		margMinusBtn.Text = "-";
		margMinusBtn.UseVisualStyleBackColor = true;
		// 
		// margPlusBtn
		// 
		margPlusBtn.FlatStyle = FlatStyle.Flat;
		margPlusBtn.Font = new Font("Consolas", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
		margPlusBtn.Location = new Point(159, 28);
		margPlusBtn.Margin = new Padding(0);
		margPlusBtn.Name = "margPlusBtn";
		margPlusBtn.Size = new Size(38, 54);
		margPlusBtn.TabIndex = 2;
		margPlusBtn.Text = "+";
		margPlusBtn.UseVisualStyleBackColor = true;
		// 
		// margRightNumeric
		// 
		margRightNumeric.Location = new Point(71, 49);
		margRightNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		margRightNumeric.Name = "margRightNumeric";
		margRightNumeric.Size = new Size(45, 23);
		margRightNumeric.TabIndex = 1;
		// 
		// margDownNumeric
		// 
		margDownNumeric.Location = new Point(41, 75);
		margDownNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		margDownNumeric.Name = "margDownNumeric";
		margDownNumeric.Size = new Size(45, 23);
		margDownNumeric.TabIndex = 1;
		// 
		// margLeftNumeric
		// 
		margLeftNumeric.Location = new Point(6, 49);
		margLeftNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		margLeftNumeric.Name = "margLeftNumeric";
		margLeftNumeric.Size = new Size(45, 23);
		margLeftNumeric.TabIndex = 1;
		// 
		// margUpNumeric
		// 
		margUpNumeric.Location = new Point(41, 22);
		margUpNumeric.Maximum = new decimal(new int[] { 2147483646, 0, 0, 0 });
		margUpNumeric.Name = "margUpNumeric";
		margUpNumeric.Size = new Size(45, 23);
		margUpNumeric.TabIndex = 0;
		// 
		// specGroupBox
		// 
		specGroupBox.Controls.Add(specTypeCombo);
		specGroupBox.Controls.Add(specScrollYCheckBox);
		specGroupBox.Controls.Add(specScrollXCheckBox);
		specGroupBox.Location = new Point(6, 80);
		specGroupBox.Name = "specGroupBox";
		specGroupBox.Size = new Size(188, 73);
		specGroupBox.TabIndex = 7;
		specGroupBox.TabStop = false;
		specGroupBox.Text = "Spec";
		// 
		// specTypeCombo
		// 
		specTypeCombo.FormattingEnabled = true;
		specTypeCombo.Items.AddRange(new object[] { "Scroll", "Pop" });
		specTypeCombo.Location = new Point(6, 31);
		specTypeCombo.Name = "specTypeCombo";
		specTypeCombo.Size = new Size(74, 23);
		specTypeCombo.TabIndex = 0;
		// 
		// NodeEditor
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		Controls.Add(groupBox2);
		Controls.Add(groupBox1);
		Controls.Add(vertDimEditor);
		Controls.Add(horzDimEditor);
		Name = "NodeEditor";
		Size = new Size(203, 348);
		groupBox1.ResumeLayout(false);
		groupBox2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)margRightNumeric).EndInit();
		((System.ComponentModel.ISupportInitialize)margDownNumeric).EndInit();
		((System.ComponentModel.ISupportInitialize)margLeftNumeric).EndInit();
		((System.ComponentModel.ISupportInitialize)margUpNumeric).EndInit();
		specGroupBox.ResumeLayout(false);
		specGroupBox.PerformLayout();
		ResumeLayout(false);
	}

	#endregion

	private DimEditor horzDimEditor;
	private DimEditor vertDimEditor;
	private ComboBox stratCombo;
	private ComboBox stratDirCombo;
	private ComboBox stratAlignCombo;
	private GroupBox groupBox1;
	private GroupBox groupBox2;
	public Button margMinusBtn;
	public Button margPlusBtn;
	public NumericUpDown margRightNumeric;
	public NumericUpDown margDownNumeric;
	public NumericUpDown margLeftNumeric;
	public NumericUpDown margUpNumeric;
	public CheckBox specScrollXCheckBox;
	public CheckBox specScrollYCheckBox;
	public GroupBox specGroupBox;
	public ComboBox specTypeCombo;
}
