using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
{
	partial class NewTerminal
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
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_NewTerminal = new System.Windows.Forms.GroupBox();
			this.groupBox_PortSettings = new System.Windows.Forms.GroupBox();
			this.serialPortSettings = new YAT.View.Controls.SerialPortSettings();
			this.serialPortSelection = new YAT.View.Controls.SerialPortSelection();
			this.socketSelection = new YAT.View.Controls.SocketSelection();
			this.usbSerialHidDeviceSelection = new YAT.View.Controls.UsbSerialHidDeviceSelection();
			this.socketSettings = new YAT.View.Controls.SocketSettings();
			this.usbSerialHidDeviceSettings = new YAT.View.Controls.UsbSerialHidDeviceSettings();
			this.terminalSelection = new YAT.View.Controls.TerminalSelection();
			this.checkBox_StartTerminal = new System.Windows.Forms.CheckBox();
			this.pictureBox_New = new System.Windows.Forms.PictureBox();
			this.button_Help = new System.Windows.Forms.Button();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.groupBox_NewTerminal.SuspendLayout();
			this.groupBox_PortSettings.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_New)).BeginInit();
			this.SuspendLayout();
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(395, 62);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(395, 33);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_NewTerminal
			// 
			this.groupBox_NewTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_NewTerminal.Controls.Add(this.groupBox_PortSettings);
			this.groupBox_NewTerminal.Controls.Add(this.terminalSelection);
			this.groupBox_NewTerminal.Controls.Add(this.checkBox_StartTerminal);
			this.groupBox_NewTerminal.Location = new System.Drawing.Point(73, 12);
			this.groupBox_NewTerminal.Name = "groupBox_NewTerminal";
			this.groupBox_NewTerminal.Size = new System.Drawing.Size(309, 445);
			this.groupBox_NewTerminal.TabIndex = 0;
			this.groupBox_NewTerminal.TabStop = false;
			// 
			// groupBox_PortSettings
			// 
			this.groupBox_PortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_PortSettings.Controls.Add(this.serialPortSettings);
			this.groupBox_PortSettings.Controls.Add(this.serialPortSelection);
			this.groupBox_PortSettings.Controls.Add(this.socketSelection);
			this.groupBox_PortSettings.Controls.Add(this.usbSerialHidDeviceSelection);
			this.groupBox_PortSettings.Controls.Add(this.socketSettings);
			this.groupBox_PortSettings.Controls.Add(this.usbSerialHidDeviceSettings);
			this.groupBox_PortSettings.Location = new System.Drawing.Point(6, 79);
			this.groupBox_PortSettings.Name = "groupBox_PortSettings";
			this.groupBox_PortSettings.Size = new System.Drawing.Size(297, 329);
			this.groupBox_PortSettings.TabIndex = 1;
			this.groupBox_PortSettings.TabStop = false;
			this.groupBox_PortSettings.Text = "Port Settings";
			// 
			// serialPortSettings
			// 
			this.serialPortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.serialPortSettings.Location = new System.Drawing.Point(6, 64);
			this.serialPortSettings.Name = "serialPortSettings";
			this.serialPortSettings.Size = new System.Drawing.Size(260, 259);
			this.serialPortSettings.TabIndex = 2;
			this.serialPortSettings.BaudRateChanged += new System.EventHandler(this.serialPortSettings_BaudRateChanged);
			this.serialPortSettings.DataBitsChanged += new System.EventHandler(this.serialPortSettings_DataBitsChanged);
			this.serialPortSettings.ParityChanged += new System.EventHandler(this.serialPortSettings_ParityChanged);
			this.serialPortSettings.StopBitsChanged += new System.EventHandler(this.serialPortSettings_StopBitsChanged);
			this.serialPortSettings.FlowControlChanged += new System.EventHandler(this.serialPortSettings_FlowControlChanged);
			this.serialPortSettings.AliveMonitorChanged += new System.EventHandler(this.serialPortSettings_AliveMonitorChanged);
			this.serialPortSettings.AutoReopenChanged += new System.EventHandler(this.serialPortSettings_AutoReopenChanged);
			// 
			// serialPortSelection
			// 
			this.serialPortSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.serialPortSelection.Location = new System.Drawing.Point(6, 19);
			this.serialPortSelection.Name = "serialPortSelection";
			this.serialPortSelection.PortId = new MKY.IO.Ports.SerialPortId(1);
			this.serialPortSelection.Size = new System.Drawing.Size(285, 46);
			this.serialPortSelection.TabIndex = 0;
			this.serialPortSelection.PortIdChanged += new System.EventHandler(this.serialPortSelection_PortIdChanged);
			// 
			// socketSelection
			// 
			this.socketSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.socketSelection.Location = new System.Drawing.Point(6, 19);
			this.socketSelection.Name = "socketSelection";
			this.socketSelection.Size = new System.Drawing.Size(282, 152);
			this.socketSelection.TabIndex = 1;
			this.socketSelection.RemoteHostChanged += new System.EventHandler(this.socketSelection_RemoteHostChanged);
			this.socketSelection.RemoteTcpPortChanged += new System.EventHandler(this.socketSelection_RemoteTcpPortChanged);
			this.socketSelection.RemoteUdpPortChanged += new System.EventHandler(this.socketSelection_RemoteUdpPortChanged);
			this.socketSelection.LocalInterfaceChanged += new System.EventHandler(this.socketSelection_LocalInterfaceChanged);
			this.socketSelection.LocalFilterChanged += new System.EventHandler(this.socketSelection_LocalFilterChanged);
			this.socketSelection.LocalTcpPortChanged += new System.EventHandler(this.socketSelection_LocalTcpPortChanged);
			this.socketSelection.LocalUdpPortChanged += new System.EventHandler(this.socketSelection_LocalUdpPortChanged);
			// 
			// usbSerialHidDeviceSelection
			// 
			this.usbSerialHidDeviceSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.usbSerialHidDeviceSelection.DeviceInfo = null;
			this.usbSerialHidDeviceSelection.Location = new System.Drawing.Point(6, 19);
			this.usbSerialHidDeviceSelection.Name = "usbSerialHidDeviceSelection";
			this.usbSerialHidDeviceSelection.Size = new System.Drawing.Size(285, 46);
			this.usbSerialHidDeviceSelection.TabIndex = 0;
			this.usbSerialHidDeviceSelection.DeviceInfoChanged += new System.EventHandler(this.usbSerialHidDeviceSelection_DeviceInfoChanged);
			// 
			// socketSettings
			// 
			this.socketSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.socketSettings.Location = new System.Drawing.Point(6, 173);
			this.socketSettings.Name = "socketSettings";
			this.socketSettings.Size = new System.Drawing.Size(260, 148);
			this.socketSettings.TabIndex = 3;
			this.socketSettings.TcpClientAutoReconnectChanged += new System.EventHandler(this.socketSettings_TcpClientAutoReconnectChanged);
			this.socketSettings.UdpServerSendModeChanged += new System.EventHandler(this.socketSettings_UdpServerSendModeChanged);
			// 
			// usbSerialHidDeviceSettings
			// 
			this.usbSerialHidDeviceSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.usbSerialHidDeviceSettings.Location = new System.Drawing.Point(6, 65);
			this.usbSerialHidDeviceSettings.Name = "usbSerialHidDeviceSettings";
			this.usbSerialHidDeviceSettings.Size = new System.Drawing.Size(285, 258);
			this.usbSerialHidDeviceSettings.TabIndex = 4;
			this.usbSerialHidDeviceSettings.PresetChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_PresetChanged);
			this.usbSerialHidDeviceSettings.ReportFormatChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_ReportFormatChanged);
			this.usbSerialHidDeviceSettings.RxFilterUsageChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_RxFilterUsageChanged);
			this.usbSerialHidDeviceSettings.FlowControlChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_FlowControlChanged);
			this.usbSerialHidDeviceSettings.AutoOpenChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_AutoOpenChanged);
			// 
			// terminalSelection
			// 
			this.terminalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.terminalSelection.Location = new System.Drawing.Point(9, 19);
			this.terminalSelection.Name = "terminalSelection";
			this.terminalSelection.Size = new System.Drawing.Size(263, 54);
			this.terminalSelection.TabIndex = 0;
			this.terminalSelection.TerminalTypeChanged += new System.EventHandler(this.terminalSelection_TerminalTypeChanged);
			this.terminalSelection.IOTypeChanged += new System.EventHandler(this.terminalSelection_IOTypeChanged);
			// 
			// checkBox_StartTerminal
			// 
			this.checkBox_StartTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_StartTerminal.AutoSize = true;
			this.checkBox_StartTerminal.Checked = true;
			this.checkBox_StartTerminal.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox_StartTerminal.Location = new System.Drawing.Point(74, 418);
			this.checkBox_StartTerminal.Name = "checkBox_StartTerminal";
			this.checkBox_StartTerminal.Size = new System.Drawing.Size(165, 17);
			this.checkBox_StartTerminal.TabIndex = 2;
			this.checkBox_StartTerminal.Text = "&Open/Start the New Terminal";
			this.checkBox_StartTerminal.CheckedChanged += new System.EventHandler(this.checkBox_StartTerminal_CheckedChanged);
			// 
			// pictureBox_New
			// 
			this.pictureBox_New.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_add_32x32;
			this.pictureBox_New.Location = new System.Drawing.Point(12, 18);
			this.pictureBox_New.Name = "pictureBox_New";
			this.pictureBox_New.Size = new System.Drawing.Size(48, 48);
			this.pictureBox_New.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox_New.TabIndex = 40;
			this.pictureBox_New.TabStop = false;
			// 
			// button_Help
			// 
			this.button_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Help.Location = new System.Drawing.Point(395, 161);
			this.button_Help.Name = "button_Help";
			this.button_Help.Size = new System.Drawing.Size(75, 23);
			this.button_Help.TabIndex = 3;
			this.button_Help.Text = "Help";
			this.button_Help.UseVisualStyleBackColor = true;
			this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(395, 111);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 41;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// NewTerminal
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(482, 464);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Help);
			this.Controls.Add(this.groupBox_NewTerminal);
			this.Controls.Add(this.pictureBox_New);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(498, 503);
			this.Name = "NewTerminal";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Terminal";
			this.Deactivate += new System.EventHandler(this.NewTerminal_Deactivate);
			this.Shown += new System.EventHandler(this.NewTerminal_Shown);
			this.groupBox_NewTerminal.ResumeLayout(false);
			this.groupBox_NewTerminal.PerformLayout();
			this.groupBox_PortSettings.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_New)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox_New;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.GroupBox groupBox_NewTerminal;
		private YAT.View.Controls.TerminalSelection terminalSelection;
		private System.Windows.Forms.CheckBox checkBox_StartTerminal;
		private System.Windows.Forms.Button button_Help;
		private System.Windows.Forms.GroupBox groupBox_PortSettings;
		private YAT.View.Controls.SocketSelection socketSelection;
		private YAT.View.Controls.SerialPortSelection serialPortSelection;
		private YAT.View.Controls.UsbSerialHidDeviceSelection usbSerialHidDeviceSelection;
		private System.Windows.Forms.Button button_Defaults;
		private YAT.View.Controls.SerialPortSettings serialPortSettings;
		private YAT.View.Controls.SocketSettings socketSettings;
		private YAT.View.Controls.UsbSerialHidDeviceSettings usbSerialHidDeviceSettings;
	}
}
