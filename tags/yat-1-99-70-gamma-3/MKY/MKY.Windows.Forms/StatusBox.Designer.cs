namespace MKY.Windows.Forms
{
	partial class StatusBox
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
			this.label_Status1 = new System.Windows.Forms.Label();
			this.label_Status2 = new System.Windows.Forms.Label();
			this.checkBox_Setting = new System.Windows.Forms.CheckBox();
			this.panel_Lower = new System.Windows.Forms.Panel();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.panel_Lower.SuspendLayout();
			this.SuspendLayout();
			// 
			// label_Status1
			// 
			this.label_Status1.AutoSize = true;
			this.label_Status1.Location = new System.Drawing.Point(12, 15);
			this.label_Status1.Name = "label_Status1";
			this.label_Status1.Size = new System.Drawing.Size(55, 13);
			this.label_Status1.TabIndex = 1;
			this.label_Status1.Text = "<Status1>";
			// 
			// label_Status2
			// 
			this.label_Status2.AutoSize = true;
			this.label_Status2.Location = new System.Drawing.Point(12, 30);
			this.label_Status2.Name = "label_Status2";
			this.label_Status2.Size = new System.Drawing.Size(55, 13);
			this.label_Status2.TabIndex = 2;
			this.label_Status2.Text = "<Status2>";
			// 
			// checkBox_Setting
			// 
			this.checkBox_Setting.AutoSize = true;
			this.checkBox_Setting.Checked = true;
			this.checkBox_Setting.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox_Setting.Location = new System.Drawing.Point(15, 56);
			this.checkBox_Setting.Name = "checkBox_Setting";
			this.checkBox_Setting.Size = new System.Drawing.Size(71, 17);
			this.checkBox_Setting.TabIndex = 3;
			this.checkBox_Setting.Text = "<Setting>";
			this.checkBox_Setting.UseVisualStyleBackColor = true;
			this.checkBox_Setting.Visible = false;
			// 
			// panel_Lower
			// 
			this.panel_Lower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel_Lower.BackColor = System.Drawing.SystemColors.Control;
			this.panel_Lower.Controls.Add(this.button_Cancel);
			this.panel_Lower.Location = new System.Drawing.Point(0, 51);
			this.panel_Lower.Name = "panel_Lower";
			this.panel_Lower.Size = new System.Drawing.Size(228, 40);
			this.panel_Lower.TabIndex = 0;
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(141, 9);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 0;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			// 
			// StatusBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(228, 91);
			this.Controls.Add(this.panel_Lower);
			this.Controls.Add(this.checkBox_Setting);
			this.Controls.Add(this.label_Status2);
			this.Controls.Add(this.label_Status1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StatusBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "<Caption>";
			this.panel_Lower.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_Status1;
		private System.Windows.Forms.Label label_Status2;
		private System.Windows.Forms.CheckBox checkBox_Setting;
		private System.Windows.Forms.Panel panel_Lower;
		private System.Windows.Forms.Button button_Cancel;
	}
}