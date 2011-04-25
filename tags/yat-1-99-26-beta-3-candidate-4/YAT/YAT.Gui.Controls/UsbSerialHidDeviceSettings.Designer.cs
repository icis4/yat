namespace YAT.Gui.Controls
{
	partial class UsbSerialHidDeviceSettings
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
			this.checkBox_AutoOpen = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// checkBox_AutoOpen
			// 
			this.checkBox_AutoOpen.AutoSize = true;
			this.checkBox_AutoOpen.Location = new System.Drawing.Point(3, 3);
			this.checkBox_AutoOpen.Name = "checkBox_AutoOpen";
			this.checkBox_AutoOpen.Size = new System.Drawing.Size(256, 17);
			this.checkBox_AutoOpen.TabIndex = 0;
			this.checkBox_AutoOpen.Text = "When device is connected, automatically open it";
			this.checkBox_AutoOpen.UseVisualStyleBackColor = true;
			this.checkBox_AutoOpen.CheckedChanged += new System.EventHandler(this.checkBox_AutoOpen_CheckedChanged);
			// 
			// UsbSerialHidDeviceSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkBox_AutoOpen);
			this.Name = "UsbSerialHidDeviceSettings";
			this.Size = new System.Drawing.Size(260, 21);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UsbSerialHidPortSettings_Paint);
			this.EnabledChanged += new System.EventHandler(this.UsbSerialHidDeviceSettings_EnabledChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_AutoOpen;

	}
}
