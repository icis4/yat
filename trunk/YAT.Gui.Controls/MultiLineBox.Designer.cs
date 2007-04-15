namespace HSR.YAT.Gui.Controls
{
	partial class MultiLineBox
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
			if (disposing && (components != null))
			{
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
			this.textBox_Command = new System.Windows.Forms.TextBox();
			this.label_Remarks = new System.Windows.Forms.Label();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox_Command
			// 
			this.textBox_Command.AcceptsReturn = true;
			this.textBox_Command.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Command.Location = new System.Drawing.Point(12, 12);
			this.textBox_Command.Multiline = true;
			this.textBox_Command.Name = "textBox_Command";
			this.textBox_Command.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox_Command.Size = new System.Drawing.Size(268, 152);
			this.textBox_Command.TabIndex = 0;
			this.textBox_Command.Leave += new System.EventHandler(this.textBox_Command_Leave);
			this.textBox_Command.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_Command_Validating);
			this.textBox_Command.TextChanged += new System.EventHandler(this.textBox_Command_TextChanged);
			// 
			// label_Remarks
			// 
			this.label_Remarks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Remarks.BackColor = System.Drawing.SystemColors.Window;
			this.label_Remarks.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Remarks.Location = new System.Drawing.Point(32, 33);
			this.label_Remarks.Name = "label_Remarks";
			this.label_Remarks.Size = new System.Drawing.Size(224, 109);
			this.label_Remarks.TabIndex = 1;
			this.label_Remarks.Text = "Press Enter to begin a new line.\r\nPress Ctrl+Enter to accept the command.\r\nPress " +
				"Escape to cancel.";
			this.label_Remarks.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(205, 177);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 3;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.Location = new System.Drawing.Point(124, 177);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 2;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// MultiLineBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(292, 212);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.label_Remarks);
			this.Controls.Add(this.textBox_Command);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "MultiLineBox";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Multi Line Command";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.MultiLineBox_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_Command;
		private System.Windows.Forms.Label label_Remarks;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
	}
}