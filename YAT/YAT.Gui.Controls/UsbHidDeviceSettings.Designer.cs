namespace YAT.Gui.Controls
{
	partial class UsbHidDeviceSettings
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.checkBox_AutoReopen = new System.Windows.Forms.CheckBox();
			this.label_AutoReopenIntervalUnit = new System.Windows.Forms.Label();
			this.textBox_AutoReopenInterval = new System.Windows.Forms.TextBox();
			this.label_AutoReopenInterval = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// checkBox_AutoReopen
			// 
			this.checkBox_AutoReopen.AutoSize = true;
			this.checkBox_AutoReopen.Location = new System.Drawing.Point(3, 3);
			this.checkBox_AutoReopen.Name = "checkBox_AutoReopen";
			this.checkBox_AutoReopen.Size = new System.Drawing.Size(217, 17);
			this.checkBox_AutoReopen.TabIndex = 0;
			this.checkBox_AutoReopen.Text = "When connection is lost, try to reopen to";
			this.checkBox_AutoReopen.UseVisualStyleBackColor = true;
			this.checkBox_AutoReopen.CheckedChanged += new System.EventHandler(this.checkBox_AutoReopen_CheckedChanged);
			// 
			// label_AutoReopenIntervalUnit
			// 
			this.label_AutoReopenIntervalUnit.AutoSize = true;
			this.label_AutoReopenIntervalUnit.Location = new System.Drawing.Point(151, 23);
			this.label_AutoReopenIntervalUnit.Name = "label_AutoReopenIntervalUnit";
			this.label_AutoReopenIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_AutoReopenIntervalUnit.TabIndex = 3;
			this.label_AutoReopenIntervalUnit.Text = "ms";
			this.label_AutoReopenIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_AutoReopenInterval
			// 
			this.textBox_AutoReopenInterval.Enabled = false;
			this.textBox_AutoReopenInterval.Location = new System.Drawing.Point(101, 20);
			this.textBox_AutoReopenInterval.Name = "textBox_AutoReopenInterval";
			this.textBox_AutoReopenInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_AutoReopenInterval.TabIndex = 2;
			this.textBox_AutoReopenInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_AutoReopenInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_AutoReopenInterval_Validating);
			// 
			// label_AutoReopenInterval
			// 
			this.label_AutoReopenInterval.AutoSize = true;
			this.label_AutoReopenInterval.Location = new System.Drawing.Point(34, 23);
			this.label_AutoReopenInterval.Name = "label_AutoReopenInterval";
			this.label_AutoReopenInterval.Size = new System.Drawing.Size(68, 13);
			this.label_AutoReopenInterval.TabIndex = 1;
			this.label_AutoReopenInterval.Text = "device every";
			this.label_AutoReopenInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UsbHidDeviceSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label_AutoReopenInterval);
			this.Controls.Add(this.label_AutoReopenIntervalUnit);
			this.Controls.Add(this.textBox_AutoReopenInterval);
			this.Controls.Add(this.checkBox_AutoReopen);
			this.Name = "UsbHidDeviceSettings";
			this.Size = new System.Drawing.Size(260, 42);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UsbHidPortSettings_Paint);
			this.EnabledChanged += new System.EventHandler(this.UsbHidDeviceSettings_EnabledChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_AutoReopen;
		private System.Windows.Forms.Label label_AutoReopenIntervalUnit;
		private System.Windows.Forms.TextBox textBox_AutoReopenInterval;
		private System.Windows.Forms.Label label_AutoReopenInterval;

	}
}
