﻿namespace YAT.View.Forms
{
	partial class Preferences
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
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_UsbDevices = new System.Windows.Forms.GroupBox();
			this.checkBox_AskForAlternateUsbDevice = new System.Windows.Forms.CheckBox();
			this.label_UsbDeviceAlternate = new System.Windows.Forms.Label();
			this.checkBox_MatchUsbSerial = new System.Windows.Forms.CheckBox();
			this.label_UsbDeviceDiscovery = new System.Windows.Forms.Label();
			this.groupBox_Main = new System.Windows.Forms.GroupBox();
			this.checkBox_ShowChrono = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTerminalInfo = new System.Windows.Forms.CheckBox();
			this.groupBox_SerialPorts = new System.Windows.Forms.GroupBox();
			this.checkBox_AskForAlternateSerialPort = new System.Windows.Forms.CheckBox();
			this.label_SerialPortAlternate = new System.Windows.Forms.Label();
			this.checkBox_RetrieveSerialPortCaptions = new System.Windows.Forms.CheckBox();
			this.label_SerialPortDiscovery = new System.Windows.Forms.Label();
			this.checkBox_DetectSerialPortsInUse = new System.Windows.Forms.CheckBox();
			this.groupBox_Workspace = new System.Windows.Forms.GroupBox();
			this.checkBox_UseRelativePaths = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoSaveWorkspace = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoOpenWorkspace = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.groupBox_NetworkInterfaces = new System.Windows.Forms.GroupBox();
			this.checkBox_AskForAlternateNetworkInterface = new System.Windows.Forms.CheckBox();
			this.label_NetworkInterfaceAlternate = new System.Windows.Forms.Label();
			this.groupBox_UsbDevices.SuspendLayout();
			this.groupBox_Main.SuspendLayout();
			this.groupBox_SerialPorts.SuspendLayout();
			this.groupBox_Workspace.SuspendLayout();
			this.groupBox_NetworkInterfaces.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(270, 103);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 7;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(270, 56);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 6;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(270, 27);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 5;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_UsbDevices
			// 
			this.groupBox_UsbDevices.Controls.Add(this.checkBox_AskForAlternateUsbDevice);
			this.groupBox_UsbDevices.Controls.Add(this.label_UsbDeviceAlternate);
			this.groupBox_UsbDevices.Controls.Add(this.checkBox_MatchUsbSerial);
			this.groupBox_UsbDevices.Controls.Add(this.label_UsbDeviceDiscovery);
			this.groupBox_UsbDevices.Location = new System.Drawing.Point(12, 394);
			this.groupBox_UsbDevices.Name = "groupBox_UsbDevices";
			this.groupBox_UsbDevices.Size = new System.Drawing.Size(246, 108);
			this.groupBox_UsbDevices.TabIndex = 4;
			this.groupBox_UsbDevices.TabStop = false;
			this.groupBox_UsbDevices.Text = "USB Ser/HID Devices";
			// 
			// checkBox_AskForAlternateUsbDevice
			// 
			this.checkBox_AskForAlternateUsbDevice.AutoSize = true;
			this.checkBox_AskForAlternateUsbDevice.Location = new System.Drawing.Point(12, 80);
			this.checkBox_AskForAlternateUsbDevice.Name = "checkBox_AskForAlternateUsbDevice";
			this.checkBox_AskForAlternateUsbDevice.Size = new System.Drawing.Size(224, 17);
			this.checkBox_AskForAlternateUsbDevice.TabIndex = 3;
			this.checkBox_AskForAlternateUsbDevice.Text = "...ask whether to switch to another device";
			this.toolTip.SetToolTip(this.checkBox_AskForAlternateUsbDevice, "When disabled and device is no longer available,\r\nthe application will silently s" +
        "top the terminal.");
			this.checkBox_AskForAlternateUsbDevice.UseVisualStyleBackColor = true;
			this.checkBox_AskForAlternateUsbDevice.CheckedChanged += new System.EventHandler(this.checkBox_AskForAlternateUsbDevice_CheckedChanged);
			// 
			// label_UsbDeviceAlternate
			// 
			this.label_UsbDeviceAlternate.AutoSize = true;
			this.label_UsbDeviceAlternate.Location = new System.Drawing.Point(9, 62);
			this.label_UsbDeviceAlternate.Name = "label_UsbDeviceAlternate";
			this.label_UsbDeviceAlternate.Size = new System.Drawing.Size(194, 13);
			this.label_UsbDeviceAlternate.TabIndex = 2;
			this.label_UsbDeviceAlternate.Text = "When a device is no longer available,...";
			// 
			// checkBox_MatchUsbSerial
			// 
			this.checkBox_MatchUsbSerial.AutoSize = true;
			this.checkBox_MatchUsbSerial.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox_MatchUsbSerial.Location = new System.Drawing.Point(12, 37);
			this.checkBox_MatchUsbSerial.Name = "checkBox_MatchUsbSerial";
			this.checkBox_MatchUsbSerial.Size = new System.Drawing.Size(173, 17);
			this.checkBox_MatchUsbSerial.TabIndex = 1;
			this.checkBox_MatchUsbSerial.Text = "...take s&erial string into account";
			this.toolTip.SetToolTip(this.checkBox_MatchUsbSerial, "Enabled: Discovery takes VID, PID and serial string into account.\r\nDisabled: Disc" +
        "overy is limited to VID and PID.");
			this.checkBox_MatchUsbSerial.UseVisualStyleBackColor = true;
			this.checkBox_MatchUsbSerial.CheckedChanged += new System.EventHandler(this.checkBox_MatchUsbSerial_CheckedChanged);
			// 
			// label_UsbDeviceDiscovery
			// 
			this.label_UsbDeviceDiscovery.AutoSize = true;
			this.label_UsbDeviceDiscovery.Location = new System.Drawing.Point(9, 19);
			this.label_UsbDeviceDiscovery.Name = "label_UsbDeviceDiscovery";
			this.label_UsbDeviceDiscovery.Size = new System.Drawing.Size(145, 13);
			this.label_UsbDeviceDiscovery.TabIndex = 0;
			this.label_UsbDeviceDiscovery.Text = "When discovering devices,...";
			// 
			// groupBox_Main
			// 
			this.groupBox_Main.Controls.Add(this.checkBox_ShowChrono);
			this.groupBox_Main.Controls.Add(this.checkBox_ShowTerminalInfo);
			this.groupBox_Main.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Main.Name = "groupBox_Main";
			this.groupBox_Main.Size = new System.Drawing.Size(245, 70);
			this.groupBox_Main.TabIndex = 0;
			this.groupBox_Main.TabStop = false;
			this.groupBox_Main.Text = "Main";
			// 
			// checkBox_ShowChrono
			// 
			this.checkBox_ShowChrono.AutoSize = true;
			this.checkBox_ShowChrono.Location = new System.Drawing.Point(12, 42);
			this.checkBox_ShowChrono.Name = "checkBox_ShowChrono";
			this.checkBox_ShowChrono.Size = new System.Drawing.Size(175, 17);
			this.checkBox_ShowChrono.TabIndex = 1;
			this.checkBox_ShowChrono.Text = "Show c&hronometer in status bar";
			this.checkBox_ShowChrono.UseVisualStyleBackColor = true;
			this.checkBox_ShowChrono.CheckedChanged += new System.EventHandler(this.checkBox_ShowChrono_CheckedChanged);
			// 
			// checkBox_ShowTerminalInfo
			// 
			this.checkBox_ShowTerminalInfo.AutoSize = true;
			this.checkBox_ShowTerminalInfo.Location = new System.Drawing.Point(12, 19);
			this.checkBox_ShowTerminalInfo.Name = "checkBox_ShowTerminalInfo";
			this.checkBox_ShowTerminalInfo.Size = new System.Drawing.Size(188, 17);
			this.checkBox_ShowTerminalInfo.TabIndex = 0;
			this.checkBox_ShowTerminalInfo.Text = "Show &terminal indices in status bar";
			this.checkBox_ShowTerminalInfo.UseVisualStyleBackColor = true;
			this.checkBox_ShowTerminalInfo.CheckedChanged += new System.EventHandler(this.checkBox_ShowTerminalInfo_CheckedChanged);
			// 
			// groupBox_SerialPorts
			// 
			this.groupBox_SerialPorts.Controls.Add(this.checkBox_AskForAlternateSerialPort);
			this.groupBox_SerialPorts.Controls.Add(this.label_SerialPortAlternate);
			this.groupBox_SerialPorts.Controls.Add(this.checkBox_RetrieveSerialPortCaptions);
			this.groupBox_SerialPorts.Controls.Add(this.label_SerialPortDiscovery);
			this.groupBox_SerialPorts.Controls.Add(this.checkBox_DetectSerialPortsInUse);
			this.groupBox_SerialPorts.Location = new System.Drawing.Point(12, 187);
			this.groupBox_SerialPorts.Name = "groupBox_SerialPorts";
			this.groupBox_SerialPorts.Size = new System.Drawing.Size(246, 131);
			this.groupBox_SerialPorts.TabIndex = 2;
			this.groupBox_SerialPorts.TabStop = false;
			this.groupBox_SerialPorts.Text = "Serial COM Ports";
			// 
			// checkBox_AskForAlternateSerialPort
			// 
			this.checkBox_AskForAlternateSerialPort.AutoSize = true;
			this.checkBox_AskForAlternateSerialPort.Location = new System.Drawing.Point(12, 103);
			this.checkBox_AskForAlternateSerialPort.Name = "checkBox_AskForAlternateSerialPort";
			this.checkBox_AskForAlternateSerialPort.Size = new System.Drawing.Size(210, 17);
			this.checkBox_AskForAlternateSerialPort.TabIndex = 4;
			this.checkBox_AskForAlternateSerialPort.Text = "...ask whether to switch to another port";
			this.toolTip.SetToolTip(this.checkBox_AskForAlternateSerialPort, "When disabled and port is no longer available,\r\nthe application will silently sto" +
        "p the terminal.");
			this.checkBox_AskForAlternateSerialPort.UseVisualStyleBackColor = true;
			this.checkBox_AskForAlternateSerialPort.CheckedChanged += new System.EventHandler(this.checkBox_AskForAlternateSerialPort_CheckedChanged);
			// 
			// label_SerialPortAlternate
			// 
			this.label_SerialPortAlternate.AutoSize = true;
			this.label_SerialPortAlternate.Location = new System.Drawing.Point(9, 85);
			this.label_SerialPortAlternate.Name = "label_SerialPortAlternate";
			this.label_SerialPortAlternate.Size = new System.Drawing.Size(180, 13);
			this.label_SerialPortAlternate.TabIndex = 3;
			this.label_SerialPortAlternate.Text = "When a port is no longer available,...";
			// 
			// checkBox_RetrieveSerialPortCaptions
			// 
			this.checkBox_RetrieveSerialPortCaptions.AutoSize = true;
			this.checkBox_RetrieveSerialPortCaptions.Location = new System.Drawing.Point(12, 37);
			this.checkBox_RetrieveSerialPortCaptions.Name = "checkBox_RetrieveSerialPortCaptions";
			this.checkBox_RetrieveSerialPortCaptions.Size = new System.Drawing.Size(192, 17);
			this.checkBox_RetrieveSerialPortCaptions.TabIndex = 1;
			this.checkBox_RetrieveSerialPortCaptions.Text = "...retrieve port &captions from system";
			this.toolTip.SetToolTip(this.checkBox_RetrieveSerialPortCaptions, "On certain computers, discovery of serial COM ports takes several seconds.\r\nIn su" +
        "ch cases it can be useful to disable one or both of these options.");
			this.checkBox_RetrieveSerialPortCaptions.UseVisualStyleBackColor = true;
			this.checkBox_RetrieveSerialPortCaptions.CheckedChanged += new System.EventHandler(this.checkBox_RetrieveSerialPortCaptions_CheckedChanged);
			// 
			// label_SerialPortDiscovery
			// 
			this.label_SerialPortDiscovery.AutoSize = true;
			this.label_SerialPortDiscovery.Location = new System.Drawing.Point(9, 19);
			this.label_SerialPortDiscovery.Name = "label_SerialPortDiscovery";
			this.label_SerialPortDiscovery.Size = new System.Drawing.Size(131, 13);
			this.label_SerialPortDiscovery.TabIndex = 0;
			this.label_SerialPortDiscovery.Text = "When discovering ports,...";
			this.toolTip.SetToolTip(this.label_SerialPortDiscovery, "On certain computers, discovery of serial COM ports takes several seconds.\r\nIn su" +
        "ch cases it can be useful to disable one or both of these options.");
			// 
			// checkBox_DetectSerialPortsInUse
			// 
			this.checkBox_DetectSerialPortsInUse.AutoSize = true;
			this.checkBox_DetectSerialPortsInUse.Location = new System.Drawing.Point(12, 60);
			this.checkBox_DetectSerialPortsInUse.Name = "checkBox_DetectSerialPortsInUse";
			this.checkBox_DetectSerialPortsInUse.Size = new System.Drawing.Size(161, 17);
			this.checkBox_DetectSerialPortsInUse.TabIndex = 2;
			this.checkBox_DetectSerialPortsInUse.Text = "...&detect ports that are in use";
			this.toolTip.SetToolTip(this.checkBox_DetectSerialPortsInUse, "On certain computers, discovery of serial COM ports takes several seconds.\r\nIn su" +
        "ch cases it can be useful to disable one or both of these options.");
			this.checkBox_DetectSerialPortsInUse.UseVisualStyleBackColor = true;
			this.checkBox_DetectSerialPortsInUse.CheckedChanged += new System.EventHandler(this.checkBox_DetectSerialPortsInUse_CheckedChanged);
			// 
			// groupBox_Workspace
			// 
			this.groupBox_Workspace.Controls.Add(this.checkBox_UseRelativePaths);
			this.groupBox_Workspace.Controls.Add(this.checkBox_AutoSaveWorkspace);
			this.groupBox_Workspace.Controls.Add(this.checkBox_AutoOpenWorkspace);
			this.groupBox_Workspace.Location = new System.Drawing.Point(12, 88);
			this.groupBox_Workspace.Name = "groupBox_Workspace";
			this.groupBox_Workspace.Size = new System.Drawing.Size(246, 93);
			this.groupBox_Workspace.TabIndex = 1;
			this.groupBox_Workspace.TabStop = false;
			this.groupBox_Workspace.Text = "Workspace";
			// 
			// checkBox_UseRelativePaths
			// 
			this.checkBox_UseRelativePaths.AutoSize = true;
			this.checkBox_UseRelativePaths.Location = new System.Drawing.Point(12, 65);
			this.checkBox_UseRelativePaths.Name = "checkBox_UseRelativePaths";
			this.checkBox_UseRelativePaths.Size = new System.Drawing.Size(229, 17);
			this.checkBox_UseRelativePaths.TabIndex = 2;
			this.checkBox_UseRelativePaths.Text = "Use &relative paths when saving workspace";
			this.checkBox_UseRelativePaths.UseVisualStyleBackColor = true;
			this.checkBox_UseRelativePaths.CheckedChanged += new System.EventHandler(this.checkBox_UseRelativePaths_CheckedChanged);
			// 
			// checkBox_AutoSaveWorkspace
			// 
			this.checkBox_AutoSaveWorkspace.AutoSize = true;
			this.checkBox_AutoSaveWorkspace.Location = new System.Drawing.Point(12, 42);
			this.checkBox_AutoSaveWorkspace.Name = "checkBox_AutoSaveWorkspace";
			this.checkBox_AutoSaveWorkspace.Size = new System.Drawing.Size(206, 17);
			this.checkBox_AutoSaveWorkspace.TabIndex = 1;
			this.checkBox_AutoSaveWorkspace.Text = "&Save current workspace automatically";
			this.checkBox_AutoSaveWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoSaveWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoSaveWorkspace_CheckedChanged);
			// 
			// checkBox_AutoOpenWorkspace
			// 
			this.checkBox_AutoOpenWorkspace.AutoSize = true;
			this.checkBox_AutoOpenWorkspace.Location = new System.Drawing.Point(12, 19);
			this.checkBox_AutoOpenWorkspace.Name = "checkBox_AutoOpenWorkspace";
			this.checkBox_AutoOpenWorkspace.Size = new System.Drawing.Size(208, 17);
			this.checkBox_AutoOpenWorkspace.TabIndex = 0;
			this.checkBox_AutoOpenWorkspace.Text = "&Open last active workspace on startup";
			this.checkBox_AutoOpenWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoOpenWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoOpenWorkspace_CheckedChanged);
			// 
			// groupBox_NetworkInterfaces
			// 
			this.groupBox_NetworkInterfaces.Controls.Add(this.checkBox_AskForAlternateNetworkInterface);
			this.groupBox_NetworkInterfaces.Controls.Add(this.label_NetworkInterfaceAlternate);
			this.groupBox_NetworkInterfaces.Location = new System.Drawing.Point(12, 324);
			this.groupBox_NetworkInterfaces.Name = "groupBox_NetworkInterfaces";
			this.groupBox_NetworkInterfaces.Size = new System.Drawing.Size(246, 64);
			this.groupBox_NetworkInterfaces.TabIndex = 3;
			this.groupBox_NetworkInterfaces.TabStop = false;
			this.groupBox_NetworkInterfaces.Text = "Network Interfaces";
			// 
			// checkBox_AskForAlternateNetworkInterface
			// 
			this.checkBox_AskForAlternateNetworkInterface.AutoSize = true;
			this.checkBox_AskForAlternateNetworkInterface.Location = new System.Drawing.Point(12, 37);
			this.checkBox_AskForAlternateNetworkInterface.Name = "checkBox_AskForAlternateNetworkInterface";
			this.checkBox_AskForAlternateNetworkInterface.Size = new System.Drawing.Size(233, 17);
			this.checkBox_AskForAlternateNetworkInterface.TabIndex = 1;
			this.checkBox_AskForAlternateNetworkInterface.Text = "...ask whether to switch to another interface";
			this.toolTip.SetToolTip(this.checkBox_AskForAlternateNetworkInterface, "When disabled and device is no longer available,\r\nthe application will silently s" +
        "top the terminal.");
			this.checkBox_AskForAlternateNetworkInterface.UseVisualStyleBackColor = true;
			this.checkBox_AskForAlternateNetworkInterface.CheckedChanged += new System.EventHandler(this.checkBox_AskForAlternateNetworkInterface_CheckedChanged);
			// 
			// label_NetworkInterfaceAlternate
			// 
			this.label_NetworkInterfaceAlternate.AutoSize = true;
			this.label_NetworkInterfaceAlternate.Location = new System.Drawing.Point(9, 19);
			this.label_NetworkInterfaceAlternate.Name = "label_NetworkInterfaceAlternate";
			this.label_NetworkInterfaceAlternate.Size = new System.Drawing.Size(209, 13);
			this.label_NetworkInterfaceAlternate.TabIndex = 0;
			this.label_NetworkInterfaceAlternate.Text = "When an interface is no longer available,...";
			// 
			// Preferences
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(357, 514);
			this.Controls.Add(this.groupBox_NetworkInterfaces);
			this.Controls.Add(this.groupBox_UsbDevices);
			this.Controls.Add(this.groupBox_SerialPorts);
			this.Controls.Add(this.groupBox_Main);
			this.Controls.Add(this.groupBox_Workspace);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Preferences";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Preferences";
			this.Shown += new System.EventHandler(this.Preferences_Shown);
			this.groupBox_UsbDevices.ResumeLayout(false);
			this.groupBox_UsbDevices.PerformLayout();
			this.groupBox_Main.ResumeLayout(false);
			this.groupBox_Main.PerformLayout();
			this.groupBox_SerialPorts.ResumeLayout(false);
			this.groupBox_SerialPorts.PerformLayout();
			this.groupBox_Workspace.ResumeLayout(false);
			this.groupBox_Workspace.PerformLayout();
			this.groupBox_NetworkInterfaces.ResumeLayout(false);
			this.groupBox_NetworkInterfaces.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.GroupBox groupBox_Workspace;
		private System.Windows.Forms.CheckBox checkBox_AutoSaveWorkspace;
		private System.Windows.Forms.CheckBox checkBox_AutoOpenWorkspace;
		private System.Windows.Forms.GroupBox groupBox_SerialPorts;
		private System.Windows.Forms.CheckBox checkBox_DetectSerialPortsInUse;
		private System.Windows.Forms.CheckBox checkBox_UseRelativePaths;
		private System.Windows.Forms.GroupBox groupBox_Main;
		private System.Windows.Forms.CheckBox checkBox_ShowChrono;
		private System.Windows.Forms.CheckBox checkBox_ShowTerminalInfo;
		private System.Windows.Forms.Label label_SerialPortDiscovery;
		private System.Windows.Forms.CheckBox checkBox_RetrieveSerialPortCaptions;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.GroupBox groupBox_UsbDevices;
		private System.Windows.Forms.CheckBox checkBox_MatchUsbSerial;
		private System.Windows.Forms.Label label_UsbDeviceDiscovery;
		private System.Windows.Forms.CheckBox checkBox_AskForAlternateSerialPort;
		private System.Windows.Forms.Label label_SerialPortAlternate;
		private System.Windows.Forms.CheckBox checkBox_AskForAlternateUsbDevice;
		private System.Windows.Forms.Label label_UsbDeviceAlternate;
		private System.Windows.Forms.GroupBox groupBox_NetworkInterfaces;
		private System.Windows.Forms.CheckBox checkBox_AskForAlternateNetworkInterface;
		private System.Windows.Forms.Label label_NetworkInterfaceAlternate;
	}
}