namespace YAT.View.Forms
{
	partial class ReleaseNotes
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReleaseNotes));
			this.button_Close = new System.Windows.Forms.Button();
			this.groupBox_ReleaseNotes = new System.Windows.Forms.GroupBox();
			this.textBox_ReleaseNotes = new System.Windows.Forms.TextBox();
			this.groupBox_ReleaseNotes.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(677, 418);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// groupBox_ReleaseNotes
			// 
			this.groupBox_ReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_ReleaseNotes.Controls.Add(this.textBox_ReleaseNotes);
			this.groupBox_ReleaseNotes.Location = new System.Drawing.Point(12, 12);
			this.groupBox_ReleaseNotes.Name = "groupBox_ReleaseNotes";
			this.groupBox_ReleaseNotes.Size = new System.Drawing.Size(740, 393);
			this.groupBox_ReleaseNotes.TabIndex = 1;
			this.groupBox_ReleaseNotes.TabStop = false;
			// 
			// textBox_ReleaseNotes
			// 
			this.textBox_ReleaseNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_ReleaseNotes.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox_ReleaseNotes.Location = new System.Drawing.Point(6, 19);
			this.textBox_ReleaseNotes.Multiline = true;
			this.textBox_ReleaseNotes.Name = "textBox_ReleaseNotes";
			this.textBox_ReleaseNotes.ReadOnly = true;
			this.textBox_ReleaseNotes.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox_ReleaseNotes.Size = new System.Drawing.Size(728, 368);
			this.textBox_ReleaseNotes.TabIndex = 0;
			this.textBox_ReleaseNotes.Text = "<RELEASE NOTES>\r\n0123456789012345678901234567890123456789012345678901234567890123" +
    "456789012345678901234567890123456789";
			this.textBox_ReleaseNotes.WordWrap = false;
			// 
			// ReleaseNotes
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(764, 453);
			this.Controls.Add(this.groupBox_ReleaseNotes);
			this.Controls.Add(this.button_Close);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ReleaseNotes";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT Release Notes";
			this.groupBox_ReleaseNotes.ResumeLayout(false);
			this.groupBox_ReleaseNotes.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.GroupBox groupBox_ReleaseNotes;
		private System.Windows.Forms.TextBox textBox_ReleaseNotes;
	}
}