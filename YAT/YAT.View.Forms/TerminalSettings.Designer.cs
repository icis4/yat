using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
{
	partial class TerminalSettings
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
			this.components = new System.ComponentModel.Container();
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.button_AdvancedSettings = new System.Windows.Forms.Button();
			this.button_TextOrBinarySettings = new System.Windows.Forms.Button();
			this.terminalSelection = new YAT.View.Controls.TerminalSelection();
			this.groupBox_IOSettings = new System.Windows.Forms.GroupBox();
			this.serialPortSettings = new YAT.View.Controls.SerialPortSettings();
			this.serialPortSelection = new YAT.View.Controls.SerialPortSelection();
			this.socketSelection = new YAT.View.Controls.SocketSelection();
			this.usbSerialHidDeviceSelection = new YAT.View.Controls.UsbSerialHidDeviceSelection();
			this.socketSettings = new YAT.View.Controls.SocketSettings();
			this.usbSerialHidDeviceSettings = new YAT.View.Controls.UsbSerialHidDeviceSettings();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Help = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_IOSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(477, 33);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Text = "OK";
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(477, 62);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Text = "Cancel";
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.button_AdvancedSettings);
			this.groupBox_Settings.Controls.Add(this.button_TextOrBinarySettings);
			this.groupBox_Settings.Controls.Add(this.terminalSelection);
			this.groupBox_Settings.Controls.Add(this.groupBox_IOSettings);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(450, 414);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// button_AdvancedSettings
			// 
			this.button_AdvancedSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_AdvancedSettings.Location = new System.Drawing.Point(320, 377);
			this.button_AdvancedSettings.Name = "button_AdvancedSettings";
			this.button_AdvancedSettings.Size = new System.Drawing.Size(114, 23);
			this.button_AdvancedSettings.TabIndex = 5;
			this.button_AdvancedSettings.Text = "&Advanced Settings...";
			this.toolTip.SetToolTip(this.button_AdvancedSettings, "Advanced display, radix, communication, send, receive and user settings.");
			this.button_AdvancedSettings.UseVisualStyleBackColor = true;
			this.button_AdvancedSettings.Click += new System.EventHandler(this.button_AdvancedSettings_Click);
			// 
			// button_TextOrBinarySettings
			// 
			this.button_TextOrBinarySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_TextOrBinarySettings.Location = new System.Drawing.Point(320, 21);
			this.button_TextOrBinarySettings.Name = "button_TextOrBinarySettings";
			this.button_TextOrBinarySettings.Size = new System.Drawing.Size(114, 23);
			this.button_TextOrBinarySettings.TabIndex = 2;
			this.button_TextOrBinarySettings.Text = "Te&xt Settings...";
			this.toolTip.SetToolTip(this.button_TextOrBinarySettings, "Text terminal dependent settings such as encoding, EOL (end-of-line) and comments" +
        ".");
			this.button_TextOrBinarySettings.UseVisualStyleBackColor = true;
			this.button_TextOrBinarySettings.Click += new System.EventHandler(this.button_TextOrBinarySettings_Click);
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
			// groupBox_IOSettings
			// 
			this.groupBox_IOSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_IOSettings.Controls.Add(this.serialPortSettings);
			this.groupBox_IOSettings.Controls.Add(this.serialPortSelection);
			this.groupBox_IOSettings.Controls.Add(this.socketSelection);
			this.groupBox_IOSettings.Controls.Add(this.usbSerialHidDeviceSelection);
			this.groupBox_IOSettings.Controls.Add(this.socketSettings);
			this.groupBox_IOSettings.Controls.Add(this.usbSerialHidDeviceSettings);
			this.groupBox_IOSettings.Location = new System.Drawing.Point(6, 79);
			this.groupBox_IOSettings.Name = "groupBox_IOSettings";
			this.groupBox_IOSettings.Size = new System.Drawing.Size(297, 329);
			this.groupBox_IOSettings.TabIndex = 1;
			this.groupBox_IOSettings.TabStop = false;
			this.groupBox_IOSettings.Text = "I/O Settings";
			// 
			// serialPortSettings
			// 
			this.serialPortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.serialPortSettings.Location = new System.Drawing.Point(6, 64);
			this.serialPortSettings.Name = "serialPortSettings";
			this.serialPortSettings.Size = new System.Drawing.Size(260, 259);
			this.serialPortSettings.TabIndex = 1;
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
			this.socketSelection.Size = new System.Drawing.Size(285, 152);
			this.socketSelection.TabIndex = 2;
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
			this.usbSerialHidDeviceSelection.Size = new System.Drawing.Size(285, 45);
			this.usbSerialHidDeviceSelection.TabIndex = 4;
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
			this.usbSerialHidDeviceSettings.TabIndex = 3;
			this.usbSerialHidDeviceSettings.PresetChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_PresetChanged);
			this.usbSerialHidDeviceSettings.ReportFormatChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_ReportFormatChanged);
			this.usbSerialHidDeviceSettings.RxFilterUsageChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_RxFilterUsageChanged);
			this.usbSerialHidDeviceSettings.FlowControlChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_FlowControlChanged);
			this.usbSerialHidDeviceSettings.AutoOpenChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_AutoOpenChanged);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(477, 111);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.UseVisualStyleBackColor = true;
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Help
			// 
			this.button_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Help.Location = new System.Drawing.Point(477, 161);
			this.button_Help.Name = "button_Help";
			this.button_Help.Size = new System.Drawing.Size(75, 23);
			this.button_Help.TabIndex = 4;
			this.button_Help.Text = "Help";
			this.button_Help.UseVisualStyleBackColor = true;
			this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
			// 
			// TerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(564, 436);
			this.Controls.Add(this.button_Help);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Settings);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(580, 475);
			this.Name = "TerminalSettings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Terminal Settings";
			this.Deactivate += new System.EventHandler(this.TerminalSettings_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TerminalSettings_FormClosing);
			this.Shown += new System.EventHandler(this.TerminalSettings_Shown);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_IOSettings.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.GroupBox groupBox_IOSettings;
		private System.Windows.Forms.GroupBox groupBox_Settings;
		private YAT.View.Controls.TerminalSelection terminalSelection;
		private YAT.View.Controls.SerialPortSettings serialPortSettings;
		private System.Windows.Forms.Button button_AdvancedSettings;
		private System.Windows.Forms.Button button_TextOrBinarySettings;
		private YAT.View.Controls.SocketSelection socketSelection;
		private YAT.View.Controls.SerialPortSelection serialPortSelection;
		private YAT.View.Controls.SocketSettings socketSettings;
		private System.Windows.Forms.Button button_Help;
		private YAT.View.Controls.UsbSerialHidDeviceSelection usbSerialHidDeviceSelection;
		private YAT.View.Controls.UsbSerialHidDeviceSettings usbSerialHidDeviceSettings;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
