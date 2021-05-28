namespace MKY.Windows.Forms
{
	partial class ExtendedMessageBox
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
			this.label_Text = new System.Windows.Forms.Label();
			this.checkBox_Check = new System.Windows.Forms.CheckBox();
			this.panel_Lower = new System.Windows.Forms.Panel();
			this.button_C = new System.Windows.Forms.Button();
			this.button_B = new System.Windows.Forms.Button();
			this.button_A = new System.Windows.Forms.Button();
			this.pictureBox_Icon = new System.Windows.Forms.PictureBox();
			this.linkLabel_TextWithLink = new System.Windows.Forms.LinkLabel();
			this.panel_Lower.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Icon)).BeginInit();
			this.SuspendLayout();
			// 
			// label_Text
			// 
			this.label_Text.AutoSize = true;
			this.label_Text.Location = new System.Drawing.Point(58, 34);
			this.label_Text.MaximumSize = new System.Drawing.Size(333, 0);
			this.label_Text.Name = "label_Text";
			this.label_Text.Size = new System.Drawing.Size(40, 13);
			this.label_Text.TabIndex = 1;
			this.label_Text.Text = "<Text>";
			// 
			// checkBox_Check
			// 
			this.checkBox_Check.AutoSize = true;
			this.checkBox_Check.Checked = true;
			this.checkBox_Check.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox_Check.Location = new System.Drawing.Point(12, 87);
			this.checkBox_Check.MaximumSize = new System.Drawing.Size(379, 0);
			this.checkBox_Check.Name = "checkBox_Check";
			this.checkBox_Check.Size = new System.Drawing.Size(90, 17);
			this.checkBox_Check.TabIndex = 3;
			this.checkBox_Check.Text = "<CheckText>";
			this.checkBox_Check.UseVisualStyleBackColor = true;
			this.checkBox_Check.Visible = false;
			// 
			// panel_Lower
			// 
			this.panel_Lower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel_Lower.BackColor = System.Drawing.SystemColors.Control;
			this.panel_Lower.Controls.Add(this.button_C);
			this.panel_Lower.Controls.Add(this.button_B);
			this.panel_Lower.Controls.Add(this.button_A);
			this.panel_Lower.Location = new System.Drawing.Point(0, 81);
			this.panel_Lower.Name = "panel_Lower";
			this.panel_Lower.Size = new System.Drawing.Size(410, 40);
			this.panel_Lower.TabIndex = 0;
			// 
			// button_C
			// 
			this.button_C.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_C.Location = new System.Drawing.Point(159, 9);
			this.button_C.Name = "button_C";
			this.button_C.Size = new System.Drawing.Size(75, 23);
			this.button_C.TabIndex = 0;
			this.button_C.Text = "<C>";
			this.button_C.UseVisualStyleBackColor = true;
			// 
			// button_B
			// 
			this.button_B.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_B.Location = new System.Drawing.Point(240, 9);
			this.button_B.Name = "button_B";
			this.button_B.Size = new System.Drawing.Size(75, 23);
			this.button_B.TabIndex = 1;
			this.button_B.Text = "<B>";
			this.button_B.UseVisualStyleBackColor = true;
			// 
			// button_A
			// 
			this.button_A.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_A.Location = new System.Drawing.Point(321, 9);
			this.button_A.Name = "button_A";
			this.button_A.Size = new System.Drawing.Size(75, 23);
			this.button_A.TabIndex = 2;
			this.button_A.Text = "<A>";
			this.button_A.UseVisualStyleBackColor = true;
			// 
			// pictureBox_Icon
			// 
			this.pictureBox_Icon.Location = new System.Drawing.Point(20, 24);
			this.pictureBox_Icon.Name = "pictureBox_Icon";
			this.pictureBox_Icon.Size = new System.Drawing.Size(32, 32);
			this.pictureBox_Icon.TabIndex = 4;
			this.pictureBox_Icon.TabStop = false;
			// 
			// linkLabel_TextWithLink
			// 
			this.linkLabel_TextWithLink.AutoSize = true;
			this.linkLabel_TextWithLink.Location = new System.Drawing.Point(58, 34);
			this.linkLabel_TextWithLink.MaximumSize = new System.Drawing.Size(333, 0);
			this.linkLabel_TextWithLink.Name = "linkLabel_TextWithLink";
			this.linkLabel_TextWithLink.Size = new System.Drawing.Size(65, 13);
			this.linkLabel_TextWithLink.TabIndex = 2;
			this.linkLabel_TextWithLink.TabStop = true;
			this.linkLabel_TextWithLink.Text = "<Text/Link>";
			this.linkLabel_TextWithLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_TextWithLink_LinkClicked);
			// 
			// ExtendedMessageBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(410, 121);
			this.Controls.Add(this.label_Text);
			this.Controls.Add(this.pictureBox_Icon);
			this.Controls.Add(this.panel_Lower);
			this.Controls.Add(this.checkBox_Check);
			this.Controls.Add(this.linkLabel_TextWithLink);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(426, 99999999);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(128, 140);
			this.Name = "ExtendedMessageBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<Caption>";
			this.panel_Lower.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Icon)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_Text;
		private System.Windows.Forms.CheckBox checkBox_Check;
		private System.Windows.Forms.Panel panel_Lower;
		private System.Windows.Forms.Button button_A;
		private System.Windows.Forms.PictureBox pictureBox_Icon;
		private System.Windows.Forms.LinkLabel linkLabel_TextWithLink;
		private System.Windows.Forms.Button button_C;
		private System.Windows.Forms.Button button_B;
	}
}