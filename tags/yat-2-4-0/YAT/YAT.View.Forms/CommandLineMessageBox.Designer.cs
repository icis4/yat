namespace YAT.View.Forms
{
	partial class CommandLineMessageBox
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CommandLineMessageBox));
			this.button_Close = new System.Windows.Forms.Button();
			this.groupBox_Text = new System.Windows.Forms.GroupBox();
			this.textBox_Text = new System.Windows.Forms.TextBox();
			this.groupBox_Text.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(537, 418);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// groupBox_Text
			// 
			this.groupBox_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Text.Controls.Add(this.textBox_Text);
			this.groupBox_Text.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Text.Name = "groupBox_Text";
			this.groupBox_Text.Size = new System.Drawing.Size(600, 393);
			this.groupBox_Text.TabIndex = 1;
			this.groupBox_Text.TabStop = false;
			// 
			// textBox_Text
			// 
			this.textBox_Text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Text.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textBox_Text.Location = new System.Drawing.Point(6, 19);
			this.textBox_Text.Multiline = true;
			this.textBox_Text.Name = "textBox_Text";
			this.textBox_Text.ReadOnly = true;
			this.textBox_Text.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox_Text.Size = new System.Drawing.Size(588, 368);
			this.textBox_Text.TabIndex = 0;
			this.textBox_Text.Text = "<TEXT>\r\n0123456789012345678901234567890123456789012345678901234567890123456789012" +
    "3456789";
			this.textBox_Text.WordWrap = false;
			// 
			// CommandLineMessageBox
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(624, 453);
			this.Controls.Add(this.groupBox_Text);
			this.Controls.Add(this.button_Close);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "CommandLineMessageBox";
			this.Text = "YAT Command Line Help";
			this.groupBox_Text.ResumeLayout(false);
			this.groupBox_Text.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.GroupBox groupBox_Text;
		private System.Windows.Forms.TextBox textBox_Text;
	}
}