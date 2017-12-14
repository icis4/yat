namespace YAT.View.Controls
{
	partial class UsbSerialHidDeviceSelection
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
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.label_OnDialogMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_RefreshPorts
			// 
			this.button_RefreshPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshPorts.Image = global::YAT.View.Controls.Properties.Resources.Image_Tool_arrow_refresh_small_16x16;
			this.button_RefreshPorts.Location = new System.Drawing.Point(258, 22);
			this.button_RefreshPorts.Name = "button_RefreshPorts";
			this.button_RefreshPorts.Size = new System.Drawing.Size(24, 21);
			this.button_RefreshPorts.TabIndex = 3;
			this.toolTip.SetToolTip(this.button_RefreshPorts, "Refresh device list");
			this.button_RefreshPorts.Click += new System.EventHandler(this.button_RefreshPorts_Click);
			// 
			// comboBox_Device
			// 
			this.comboBox_Device.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Device.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Device.ItemHeight = 13;
			this.comboBox_Device.Location = new System.Drawing.Point(3, 22);
			this.comboBox_Device.Name = "comboBox_Device";
			this.comboBox_Device.Size = new System.Drawing.Size(254, 21);
			this.comboBox_Device.TabIndex = 2;
			this.comboBox_Device.SelectedIndexChanged += new System.EventHandler(this.comboBox_Device_SelectedIndexChanged);
			// 
			// label_Device
			// 
			this.label_Device.AutoSize = true;
			this.label_Device.Location = new System.Drawing.Point(3, 6);
			this.label_Device.Name = "label_Device";
			this.label_Device.Size = new System.Drawing.Size(66, 13);
			this.label_Device.TabIndex = 0;
			this.label_Device.Text = "HID De&vice:";
			// 
			// label_OnDialogMessage
			// 
			this.label_OnDialogMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_OnDialogMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_OnDialogMessage.ForeColor = System.Drawing.Color.DarkOrange;
			this.label_OnDialogMessage.Location = new System.Drawing.Point(75, 6);
			this.label_OnDialogMessage.Name = "label_OnDialogMessage";
			this.label_OnDialogMessage.Size = new System.Drawing.Size(210, 13);
			this.label_OnDialogMessage.TabIndex = 1;
			this.label_OnDialogMessage.Text = "<Message>";
			this.label_OnDialogMessage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// UsbSerialHidDeviceSelection
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.label_OnDialogMessage);
			this.Controls.Add(this.button_RefreshPorts);
			this.Controls.Add(this.comboBox_Device);
			this.Controls.Add(this.label_Device);
			this.Name = "UsbSerialHidDeviceSelection";
			this.Size = new System.Drawing.Size(285, 46);
			this.EnabledChanged += new System.EventHandler(this.UsbSerialHidDeviceSelection_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.UsbSerialHidPortSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_RefreshPorts;
		private System.Windows.Forms.ComboBox comboBox_Device;
		private System.Windows.Forms.Label label_Device;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label_OnDialogMessage;
	}
}
