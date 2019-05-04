﻿using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Gui.Forms
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewTerminal));
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_NewTerminal = new System.Windows.Forms.GroupBox();
			this.groupBox_PortSettings = new System.Windows.Forms.GroupBox();
			this.usbSerialHidDeviceSettings = new YAT.Gui.Controls.UsbSerialHidDeviceSettings();
			this.usbSerialHidDeviceSelection = new YAT.Gui.Controls.UsbSerialHidDeviceSelection();
			this.socketSettings = new YAT.Gui.Controls.SocketSettings();
			this.socketSelection = new YAT.Gui.Controls.SocketSelection();
			this.serialPortSettings = new YAT.Gui.Controls.SerialPortSettings();
			this.serialPortSelection = new YAT.Gui.Controls.SerialPortSelection();
			this.terminalSelection = new YAT.Gui.Controls.TerminalSelection();
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
			this.button_Cancel.Location = new System.Drawing.Point(398, 62);
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
			this.button_OK.Location = new System.Drawing.Point(398, 33);
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
			this.groupBox_NewTerminal.Size = new System.Drawing.Size(312, 419);
			this.groupBox_NewTerminal.TabIndex = 0;
			this.groupBox_NewTerminal.TabStop = false;
			// 
			// groupBox_PortSettings
			// 
			this.groupBox_PortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_PortSettings.Controls.Add(this.usbSerialHidDeviceSettings);
			this.groupBox_PortSettings.Controls.Add(this.usbSerialHidDeviceSelection);
			this.groupBox_PortSettings.Controls.Add(this.socketSettings);
			this.groupBox_PortSettings.Controls.Add(this.socketSelection);
			this.groupBox_PortSettings.Controls.Add(this.serialPortSettings);
			this.groupBox_PortSettings.Controls.Add(this.serialPortSelection);
			this.groupBox_PortSettings.Location = new System.Drawing.Point(6, 79);
			this.groupBox_PortSettings.Name = "groupBox_PortSettings";
			this.groupBox_PortSettings.Size = new System.Drawing.Size(300, 303);
			this.groupBox_PortSettings.TabIndex = 1;
			this.groupBox_PortSettings.TabStop = false;
			this.groupBox_PortSettings.Text = "Port Settings";
			// 
			// usbSerialHidDeviceSettings
			// 
			this.usbSerialHidDeviceSettings.Location = new System.Drawing.Point(9, 65);
			this.usbSerialHidDeviceSettings.Name = "usbSerialHidDeviceSettings";
			this.usbSerialHidDeviceSettings.ReportFormat = ((MKY.IO.Usb.SerialHidReportFormat)(resources.GetObject("usbSerialHidDeviceSettings.ReportFormat")));
			this.usbSerialHidDeviceSettings.RxIdUsage = ((MKY.IO.Usb.SerialHidRxIdUsage)(resources.GetObject("usbSerialHidDeviceSettings.RxIdUsage")));
			this.usbSerialHidDeviceSettings.Size = new System.Drawing.Size(285, 232);
			this.usbSerialHidDeviceSettings.TabIndex = 4;
			this.usbSerialHidDeviceSettings.ReportFormatChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_ReportFormatChanged);
			this.usbSerialHidDeviceSettings.RxIdUsageChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_RxIdUsageChanged);
			this.usbSerialHidDeviceSettings.AutoOpenChanged += new System.EventHandler(this.usbSerialHidDeviceSettings_AutoOpenChanged);
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
			this.socketSettings.Location = new System.Drawing.Point(9, 146);
			this.socketSettings.Name = "socketSettings";
			this.socketSettings.Size = new System.Drawing.Size(260, 42);
			this.socketSettings.TabIndex = 3;
			this.socketSettings.TcpClientAutoReconnect = ((MKY.IO.Serial.AutoRetry)(resources.GetObject("socketSettings.TcpClientAutoReconnect")));
			this.socketSettings.TcpClientAutoReconnectChanged += new System.EventHandler(this.socketSettings_TcpClientAutoReconnectChanged);
			// 
			// socketSelection
			// 
			this.socketSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.socketSelection.Location = new System.Drawing.Point(6, 19);
			this.socketSelection.Name = "socketSelection";
			this.socketSelection.Size = new System.Drawing.Size(285, 125);
			this.socketSelection.TabIndex = 1;
			this.socketSelection.RemoteHostChanged += new System.EventHandler(this.socketSelection_RemoteHostChanged);
			this.socketSelection.RemoteTcpPortChanged += new System.EventHandler(this.socketSelection_RemoteTcpPortChanged);
			this.socketSelection.RemoteUdpPortChanged += new System.EventHandler(this.socketSelection_RemoteUdpPortChanged);
			this.socketSelection.LocalInterfaceChanged += new System.EventHandler(this.socketSelection_LocalInterfaceChanged);
			this.socketSelection.LocalTcpPortChanged += new System.EventHandler(this.socketSelection_LocalTcpPortChanged);
			this.socketSelection.LocalUdpPortChanged += new System.EventHandler(this.socketSelection_LocalUdpPortChanged);
			// 
			// serialPortSettings
			// 
			this.serialPortSettings.AutoReopen = ((MKY.IO.Serial.AutoRetry)(resources.GetObject("serialPortSettings.AutoReopen")));
			this.serialPortSettings.Location = new System.Drawing.Point(9, 64);
			this.serialPortSettings.Name = "serialPortSettings";
			this.serialPortSettings.Size = new System.Drawing.Size(260, 232);
			this.serialPortSettings.TabIndex = 2;
			this.serialPortSettings.BaudRateChanged += new System.EventHandler(this.serialPortSettings_BaudRateChanged);
			this.serialPortSettings.DataBitsChanged += new System.EventHandler(this.serialPortSettings_DataBitsChanged);
			this.serialPortSettings.ParityChanged += new System.EventHandler(this.serialPortSettings_ParityChanged);
			this.serialPortSettings.StopBitsChanged += new System.EventHandler(this.serialPortSettings_StopBitsChanged);
			this.serialPortSettings.FlowControlChanged += new System.EventHandler(this.serialPortSettings_FlowControlChanged);
			this.serialPortSettings.AutoReopenChanged += new System.EventHandler(this.serialPortSettings_AutoReopenChanged);
			// 
			// serialPortSelection
			// 
			this.serialPortSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.serialPortSelection.Location = new System.Drawing.Point(6, 19);
			this.serialPortSelection.Name = "serialPortSelection";
			this.serialPortSelection.PortId = new MKY.IO.Ports.SerialPortId(3);
			this.serialPortSelection.Size = new System.Drawing.Size(285, 46);
			this.serialPortSelection.TabIndex = 0;
			this.serialPortSelection.PortIdChanged += new System.EventHandler(this.serialPortSelection_PortIdChanged);
			// 
			// terminalSelection
			// 
			this.terminalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.terminalSelection.Location = new System.Drawing.Point(12, 19);
			this.terminalSelection.Name = "terminalSelection";
			this.terminalSelection.Size = new System.Drawing.Size(260, 54);
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
			this.checkBox_StartTerminal.Location = new System.Drawing.Point(113, 392);
			this.checkBox_StartTerminal.Name = "checkBox_StartTerminal";
			this.checkBox_StartTerminal.Size = new System.Drawing.Size(91, 17);
			this.checkBox_StartTerminal.TabIndex = 2;
			this.checkBox_StartTerminal.Text = "&Open terminal";
			this.checkBox_StartTerminal.CheckedChanged += new System.EventHandler(this.checkBox_StartTerminal_CheckedChanged);
			// 
			// pictureBox_New
			// 
			this.pictureBox_New.Image = global::YAT.Gui.Forms.Properties.Resources.Image_NewDocument_16x16;
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
			this.button_Help.Location = new System.Drawing.Point(398, 161);
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
			this.button_Defaults.Location = new System.Drawing.Point(398, 111);
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
			this.ClientSize = new System.Drawing.Size(485, 439);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Help);
			this.Controls.Add(this.groupBox_NewTerminal);
			this.Controls.Add(this.pictureBox_New);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(1024, 477);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(493, 477);
			this.Name = "NewTerminal";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Terminal";
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
		private YAT.Gui.Controls.TerminalSelection terminalSelection;
		private System.Windows.Forms.CheckBox checkBox_StartTerminal;
		private System.Windows.Forms.Button button_Help;
		private System.Windows.Forms.GroupBox groupBox_PortSettings;
		private YAT.Gui.Controls.SocketSelection socketSelection;
		private YAT.Gui.Controls.SerialPortSelection serialPortSelection;
		private YAT.Gui.Controls.UsbSerialHidDeviceSelection usbSerialHidDeviceSelection;
		private System.Windows.Forms.Button button_Defaults;
		private YAT.Gui.Controls.SerialPortSettings serialPortSettings;
		private YAT.Gui.Controls.SocketSettings socketSettings;
		private YAT.Gui.Controls.UsbSerialHidDeviceSettings usbSerialHidDeviceSettings;
	}
}