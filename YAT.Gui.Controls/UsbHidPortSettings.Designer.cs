namespace YAT.Gui.Controls
{
	partial class UsbHidPortSettings
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
            this.checkBox_AutoReconnect = new System.Windows.Forms.CheckBox();
            this.label_AutoReconnectIntervalUnit = new System.Windows.Forms.Label();
            this.textBox_AutoReconnectInterval = new System.Windows.Forms.TextBox();
            this.label_AutoReconnectInterval = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // checkBox_AutoReconnect
            // 
            this.checkBox_AutoReconnect.AutoSize = true;
            this.checkBox_AutoReconnect.Location = new System.Drawing.Point(3, 3);
            this.checkBox_AutoReconnect.Name = "checkBox_AutoReconnect";
            this.checkBox_AutoReconnect.Size = new System.Drawing.Size(232, 17);
            this.checkBox_AutoReconnect.TabIndex = 0;
            this.checkBox_AutoReconnect.Text = "When connection is lost, try to reconnect to";
            this.checkBox_AutoReconnect.UseVisualStyleBackColor = true;
            this.checkBox_AutoReconnect.CheckedChanged += new System.EventHandler(this.checkBox_AutoReconnect_CheckedChanged);
            // 
            // label_AutoReconnectIntervalUnit
            // 
            this.label_AutoReconnectIntervalUnit.AutoSize = true;
            this.label_AutoReconnectIntervalUnit.Location = new System.Drawing.Point(151, 23);
            this.label_AutoReconnectIntervalUnit.Name = "label_AutoReconnectIntervalUnit";
            this.label_AutoReconnectIntervalUnit.Size = new System.Drawing.Size(20, 13);
            this.label_AutoReconnectIntervalUnit.TabIndex = 3;
            this.label_AutoReconnectIntervalUnit.Text = "ms";
            this.label_AutoReconnectIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_AutoReconnectInterval
            // 
            this.textBox_AutoReconnectInterval.Enabled = false;
            this.textBox_AutoReconnectInterval.Location = new System.Drawing.Point(101, 20);
            this.textBox_AutoReconnectInterval.Name = "textBox_AutoReconnectInterval";
            this.textBox_AutoReconnectInterval.Size = new System.Drawing.Size(48, 20);
            this.textBox_AutoReconnectInterval.TabIndex = 2;
            this.textBox_AutoReconnectInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBox_AutoReconnectInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_AutoReconnectInterval_Validating);
            // 
            // label_AutoReconnectInterval
            // 
            this.label_AutoReconnectInterval.AutoSize = true;
            this.label_AutoReconnectInterval.Location = new System.Drawing.Point(34, 23);
            this.label_AutoReconnectInterval.Name = "label_AutoReconnectInterval";
            this.label_AutoReconnectInterval.Size = new System.Drawing.Size(68, 13);
            this.label_AutoReconnectInterval.TabIndex = 1;
            this.label_AutoReconnectInterval.Text = "device every";
            this.label_AutoReconnectInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // UsbHidPortSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label_AutoReconnectInterval);
            this.Controls.Add(this.label_AutoReconnectIntervalUnit);
            this.Controls.Add(this.textBox_AutoReconnectInterval);
            this.Controls.Add(this.checkBox_AutoReconnect);
            this.Name = "UsbHidPortSettings";
            this.Size = new System.Drawing.Size(260, 42);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.UsbHidPortSettings_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_AutoReconnect;
		private System.Windows.Forms.Label label_AutoReconnectIntervalUnit;
		private System.Windows.Forms.TextBox textBox_AutoReconnectInterval;
		private System.Windows.Forms.Label label_AutoReconnectInterval;

	}
}
