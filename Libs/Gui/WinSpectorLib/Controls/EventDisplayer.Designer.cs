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
		pauseBtn = new Button();
		clearBtn = new Button();
		hideBtn = new Button();
		SuspendLayout();
		// 
		// eventListBox
		// 
		eventListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
		eventListBox.DrawMode = DrawMode.OwnerDrawFixed;
		eventListBox.FormattingEnabled = true;
		eventListBox.Location = new Point(3, 3);
		eventListBox.Name = "eventListBox";
		eventListBox.Size = new Size(244, 212);
		eventListBox.TabIndex = 0;
		// 
		// pauseBtn
		// 
		pauseBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		pauseBtn.Location = new Point(3, 221);
		pauseBtn.Name = "pauseBtn";
		pauseBtn.Size = new Size(79, 23);
		pauseBtn.TabIndex = 1;
		pauseBtn.Text = "Pause (P)";
		pauseBtn.UseVisualStyleBackColor = true;
		// 
		// clearBtn
		// 
		clearBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		clearBtn.Location = new Point(88, 221);
		clearBtn.Name = "clearBtn";
		clearBtn.Size = new Size(79, 23);
		clearBtn.TabIndex = 2;
		clearBtn.Text = "Clear (C)";
		clearBtn.UseVisualStyleBackColor = true;
		// 
		// hideBtn
		// 
		hideBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
		hideBtn.Location = new Point(172, 221);
		hideBtn.Name = "hideBtn";
		hideBtn.Size = new Size(75, 23);
		hideBtn.TabIndex = 3;
		hideBtn.Text = "Hide (H)";
		hideBtn.UseVisualStyleBackColor = true;
		// 
		// EventDisplayer
		// 
		AutoScaleDimensions = new SizeF(7F, 15F);
		AutoScaleMode = AutoScaleMode.Font;
		Controls.Add(hideBtn);
		Controls.Add(clearBtn);
		Controls.Add(pauseBtn);
		Controls.Add(eventListBox);
		Name = "EventDisplayer";
		Size = new Size(250, 250);
		ResumeLayout(false);
	}

	#endregion

	private FlickerFreeListBox eventListBox;
	private Button pauseBtn;
	private Button clearBtn;
	private Button hideBtn;
}
