namespace EventsMonitor.Controls;

partial class DXEventDisplayer
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
		clearBtn = new Button();
		pauseBtn = new Button();
		eventListBox = new FlickerFreeListBox();
		SuspendLayout();
		// 
		// clearBtn
		// 
		clearBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
		clearBtn.Location = new Point(128, 223);
		clearBtn.Name = "clearBtn";
		clearBtn.Size = new Size(119, 23);
		clearBtn.TabIndex = 1;
		clearBtn.Text = "Clear";
		clearBtn.UseVisualStyleBackColor = true;
		// 
		// pauseBtn
		// 
		pauseBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		pauseBtn.Location = new Point(3, 223);
		pauseBtn.Name = "pauseBtn";
		pauseBtn.Size = new Size(119, 23);
		pauseBtn.TabIndex = 2;
		pauseBtn.Text = "Pause";
		pauseBtn.UseVisualStyleBackColor = true;
		// 
		// eventListBox
		// 
		eventListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		eventListBox.DrawMode = DrawMode.OwnerDrawFixed;
		eventListBox.FormattingEnabled = true;
		eventListBox.Location = new Point(3, 3);
		eventListBox.Name = "eventListBox";
		eventListBox.Size = new Size(244, 212);
		eventListBox.TabIndex = 3;
		// 
		// EventDisplayer
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		Controls.Add(eventListBox);
		Controls.Add(pauseBtn);
		Controls.Add(clearBtn);
		Name = "EventDisplayer";
		Size = new Size(250, 250);
		ResumeLayout(false);
	}

	#endregion
	private Button clearBtn;
	private Button pauseBtn;
	private FlickerFreeListBox eventListBox;
}
