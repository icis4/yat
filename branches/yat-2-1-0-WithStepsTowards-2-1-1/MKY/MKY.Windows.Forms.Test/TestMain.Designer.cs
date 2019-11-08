namespace MKY.Windows.Forms.Test
{
	partial class TestMain
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
			this.button_TextInputControls = new System.Windows.Forms.Button();
			this.button_ListBoxControls = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button_TextInputControls
			// 
			this.button_TextInputControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_TextInputControls.Location = new System.Drawing.Point(12, 12);
			this.button_TextInputControls.Name = "button_TextInputControls";
			this.button_TextInputControls.Size = new System.Drawing.Size(260, 23);
			this.button_TextInputControls.TabIndex = 0;
			this.button_TextInputControls.Text = "Test &Text Input Controls";
			this.button_TextInputControls.UseVisualStyleBackColor = true;
			this.button_TextInputControls.Click += new System.EventHandler(this.button_TextInputControls_Click);
			// 
			// button_ListBoxControls
			// 
			this.button_ListBoxControls.Location = new System.Drawing.Point(12, 41);
			this.button_ListBoxControls.Name = "button_ListBoxControls";
			this.button_ListBoxControls.Size = new System.Drawing.Size(260, 23);
			this.button_ListBoxControls.TabIndex = 1;
			this.button_ListBoxControls.Text = "Test &ListBox Controls";
			this.button_ListBoxControls.UseVisualStyleBackColor = true;
			this.button_ListBoxControls.Click += new System.EventHandler(this.button_ListBoxControls_Click);
			// 
			// TestMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 75);
			this.Controls.Add(this.button_ListBoxControls);
			this.Controls.Add(this.button_TextInputControls);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "TestMain";
			this.Text = "TestMain";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_TextInputControls;
		private System.Windows.Forms.Button button_ListBoxControls;
	}
}