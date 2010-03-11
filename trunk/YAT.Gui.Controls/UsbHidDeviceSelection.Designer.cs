namespace YAT.Gui.Controls
{
	partial class UsbHidDeviceSelection
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
			this.components = new System.ComponentModel.Container();
			this.button_RefreshPorts = new System.Windows.Forms.Button();
			this.comboBox_Device = new System.Windows.Forms.ComboBox();
			this.label_Device = new System.Windows.Forms.Label();
			this.timer_ShowScanDialog = new System.Windows.Forms.Timer(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// button_RefreshPorts
			// 
			this.button_RefreshPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshPorts.Image = global::YAT.Gui.Controls.Properties.Resources.Image_Refresh_16x16;
			this.button_RefreshPorts.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button_RefreshPorts.Location = new System.Drawing.Point(258, 22);
			this.button_RefreshPorts.Name = "button_RefreshPorts";
			this.button_RefreshPorts.Size = new System.Drawing.Size(24, 21);
			this.button_RefreshPorts.TabIndex = 2;
			this.toolTip.SetToolTip(this.button_RefreshPorts, "Refresh serial port list");
			this.button_RefreshPorts.Click += new System.EventHandler(this.button_RefreshPorts_Click);
			// 
			// comboBox_Device
			// 
			this.comboBox_Device.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Device.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Device.ItemHeight = 13;
			this.comboBox_Device.Location = new System.Drawing.Point(6, 22);
			this.comboBox_Device.Name = "comboBox_Device";
			this.comboBox_Device.Size = new System.Drawing.Size(251, 21);
			this.comboBox_Device.TabIndex = 1;
			// 
			// label_Device
			// 
			this.label_Device.AutoSize = true;
			this.label_Device.Location = new System.Drawing.Point(3, 6);
			this.label_Device.Name = "label_Device";
			this.label_Device.Size = new System.Drawing.Size(112, 13);
			this.label_Device.TabIndex = 0;
			this.label_Device.Text = "USB Ser/HID Device:";
			// 
			// timer_ShowScanDialog
			// 
			this.timer_ShowScanDialog.Interval = 500;
			this.timer_ShowScanDialog.Tick += new System.EventHandler(this.timer_ShowScanDialog_Tick);
			// 
			// UsbHidDeviceSelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button_RefreshPorts);
			this.Controls.Add(this.comboBox_Device);
			this.Controls.Add(this.label_Device);
			this.Name = "UsbHidDeviceSelection";
			this.Size = new System.Drawing.Size(285, 46);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UsbHidPortSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_RefreshPorts;
		private System.Windows.Forms.ComboBox comboBox_Device;
		private System.Windows.Forms.Label label_Device;
		private System.Windows.Forms.Timer timer_ShowScanDialog;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
