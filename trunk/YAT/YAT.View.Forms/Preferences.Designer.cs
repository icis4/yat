namespace YAT.View.Forms
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_OK = new System.Windows.Forms.Button();
			this.groupBox_UsbDevices = new System.Windows.Forms.GroupBox();
			this.checkBox_AskForAlternateUsbDevice = new System.Windows.Forms.CheckBox();
			this.label_UsbDeviceAvailability = new System.Windows.Forms.Label();
			this.checkBox_MatchUsbSerial = new System.Windows.Forms.CheckBox();
			this.label_UsbDeviceDiscovery = new System.Windows.Forms.Label();
			this.groupBox_Main = new System.Windows.Forms.GroupBox();
			this.checkBox_ShowTime = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowChrono = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowTerminalInfo = new System.Windows.Forms.CheckBox();
			this.groupBox_SerialPorts = new System.Windows.Forms.GroupBox();
			this.checkBox_AskForAlternateSerialPort = new System.Windows.Forms.CheckBox();
			this.label_SerialPortAvailability = new System.Windows.Forms.Label();
			this.checkBox_RetrieveSerialPortCaptions = new System.Windows.Forms.CheckBox();
			this.label_SerialPortDiscovery = new System.Windows.Forms.Label();
			this.checkBox_DetectSerialPortsInUse = new System.Windows.Forms.CheckBox();
			this.groupBox_Workspace = new System.Windows.Forms.GroupBox();
			this.checkBox_UseRelativePaths = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoSaveWorkspace = new System.Windows.Forms.CheckBox();
			this.checkBox_AutoOpenWorkspace = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.checkBox_AskForAlternateNetworkInterface = new System.Windows.Forms.CheckBox();
			this.groupBox_NetworkInterfaces = new System.Windows.Forms.GroupBox();
			this.label_NetworkInterfaceAvailability = new System.Windows.Forms.Label();
			this.groupBox_Terminals = new System.Windows.Forms.GroupBox();
			this.checkBox_NotifyNonAvailableIO = new System.Windows.Forms.CheckBox();
			this.label_IOAvailability = new System.Windows.Forms.Label();
			this.groupBox_UsbDevices.SuspendLayout();
			this.groupBox_Main.SuspendLayout();
			this.groupBox_SerialPorts.SuspendLayout();
			this.groupBox_Workspace.SuspendLayout();
			this.groupBox_NetworkInterfaces.SuspendLayout();
			this.groupBox_Terminals.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_Defaults.Location = new System.Drawing.Point(24, 338);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 8;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.UseVisualStyleBackColor = true;
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(479, 338);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 7;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(398, 338);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 6;
			this.button_OK.Text = "OK";
			this.button_OK.UseVisualStyleBackColor = true;
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// groupBox_UsbDevices
			// 
			this.groupBox_UsbDevices.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_UsbDevices.Controls.Add(this.checkBox_AskForAlternateUsbDevice);
			this.groupBox_UsbDevices.Controls.Add(this.label_UsbDeviceAvailability);
			this.groupBox_UsbDevices.Controls.Add(this.checkBox_MatchUsbSerial);
			this.groupBox_UsbDevices.Controls.Add(this.label_UsbDeviceDiscovery);
			this.groupBox_UsbDevices.Location = new System.Drawing.Point(305, 219);
			this.groupBox_UsbDevices.Name = "groupBox_UsbDevices";
			this.groupBox_UsbDevices.Size = new System.Drawing.Size(249, 108);
			this.groupBox_UsbDevices.TabIndex = 5;
			this.groupBox_UsbDevices.TabStop = false;
			this.groupBox_UsbDevices.Text = "&USB Ser/HID Devices";
			// 
			// checkBox_AskForAlternateUsbDevice
			// 
			this.checkBox_AskForAlternateUsbDevice.AutoSize = true;
			this.checkBox_AskForAlternateUsbDevice.Location = new System.Drawing.Point(12, 80);
			this.checkBox_AskForAlternateUsbDevice.Name = "checkBox_AskForAlternateUsbDevice";
			this.checkBox_AskForAlternateUsbDevice.Size = new System.Drawing.Size(224, 17);
			this.checkBox_AskForAlternateUsbDevice.TabIndex = 3;
			this.checkBox_AskForAlternateUsbDevice.Text = "...ask whether to switch to another de&vice";
			this.toolTip.SetToolTip(this.checkBox_AskForAlternateUsbDevice, "When disabled and device is no longer available,\r\nthe application will silently s" +
        "top the terminal.");
			this.checkBox_AskForAlternateUsbDevice.UseVisualStyleBackColor = true;
			this.checkBox_AskForAlternateUsbDevice.CheckedChanged += new System.EventHandler(this.checkBox_AskForAlternateUsbDevice_CheckedChanged);
			// 
			// label_UsbDeviceAvailability
			// 
			this.label_UsbDeviceAvailability.AutoSize = true;
			this.label_UsbDeviceAvailability.Location = new System.Drawing.Point(9, 62);
			this.label_UsbDeviceAvailability.Name = "label_UsbDeviceAvailability";
			this.label_UsbDeviceAvailability.Size = new System.Drawing.Size(194, 13);
			this.label_UsbDeviceAvailability.TabIndex = 2;
			this.label_UsbDeviceAvailability.Text = "When a device is no longer available,...";
			// 
			// checkBox_MatchUsbSerial
			// 
			this.checkBox_MatchUsbSerial.AutoSize = true;
			this.checkBox_MatchUsbSerial.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox_MatchUsbSerial.Location = new System.Drawing.Point(12, 37);
			this.checkBox_MatchUsbSerial.Name = "checkBox_MatchUsbSerial";
			this.checkBox_MatchUsbSerial.Size = new System.Drawing.Size(213, 17);
			this.checkBox_MatchUsbSerial.TabIndex = 1;
			this.checkBox_MatchUsbSerial.Text = "...take s&erial number/string into account";
			this.toolTip.SetToolTip(this.checkBox_MatchUsbSerial, resources.GetString("checkBox_MatchUsbSerial.ToolTip"));
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
			this.groupBox_Main.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Main.Controls.Add(this.checkBox_ShowTime);
			this.groupBox_Main.Controls.Add(this.checkBox_ShowChrono);
			this.groupBox_Main.Controls.Add(this.checkBox_ShowTerminalInfo);
			this.groupBox_Main.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Main.Name = "groupBox_Main";
			this.groupBox_Main.Size = new System.Drawing.Size(281, 119);
			this.groupBox_Main.TabIndex = 0;
			this.groupBox_Main.TabStop = false;
			this.groupBox_Main.Text = "&Main";
			// 
			// checkBox_ShowTime
			// 
			this.checkBox_ShowTime.AutoSize = true;
			this.checkBox_ShowTime.Location = new System.Drawing.Point(12, 56);
			this.checkBox_ShowTime.Name = "checkBox_ShowTime";
			this.checkBox_ShowTime.Size = new System.Drawing.Size(160, 17);
			this.checkBox_ShowTime.TabIndex = 1;
			this.checkBox_ShowTime.Text = "Show local t&ime in status bar";
			this.checkBox_ShowTime.UseVisualStyleBackColor = true;
			this.checkBox_ShowTime.CheckedChanged += new System.EventHandler(this.checkBox_ShowTime_CheckedChanged);
			// 
			// checkBox_ShowChrono
			// 
			this.checkBox_ShowChrono.AutoSize = true;
			this.checkBox_ShowChrono.Location = new System.Drawing.Point(12, 79);
			this.checkBox_ShowChrono.Name = "checkBox_ShowChrono";
			this.checkBox_ShowChrono.Size = new System.Drawing.Size(175, 17);
			this.checkBox_ShowChrono.TabIndex = 2;
			this.checkBox_ShowChrono.Text = "Show c&hronometer in status bar";
			this.checkBox_ShowChrono.UseVisualStyleBackColor = true;
			this.checkBox_ShowChrono.CheckedChanged += new System.EventHandler(this.checkBox_ShowChrono_CheckedChanged);
			// 
			// checkBox_ShowTerminalInfo
			// 
			this.checkBox_ShowTerminalInfo.AutoSize = true;
			this.checkBox_ShowTerminalInfo.Location = new System.Drawing.Point(12, 33);
			this.checkBox_ShowTerminalInfo.Name = "checkBox_ShowTerminalInfo";
			this.checkBox_ShowTerminalInfo.Size = new System.Drawing.Size(171, 17);
			this.checkBox_ShowTerminalInfo.TabIndex = 0;
			this.checkBox_ShowTerminalInfo.Text = "Show &terminal IDs in status bar";
			this.checkBox_ShowTerminalInfo.UseVisualStyleBackColor = true;
			this.checkBox_ShowTerminalInfo.CheckedChanged += new System.EventHandler(this.checkBox_ShowTerminalInfo_CheckedChanged);
			// 
			// groupBox_SerialPorts
			// 
			this.groupBox_SerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_SerialPorts.Controls.Add(this.checkBox_AskForAlternateSerialPort);
			this.groupBox_SerialPorts.Controls.Add(this.label_SerialPortAvailability);
			this.groupBox_SerialPorts.Controls.Add(this.checkBox_RetrieveSerialPortCaptions);
			this.groupBox_SerialPorts.Controls.Add(this.label_SerialPortDiscovery);
			this.groupBox_SerialPorts.Controls.Add(this.checkBox_DetectSerialPortsInUse);
			this.groupBox_SerialPorts.Location = new System.Drawing.Point(305, 12);
			this.groupBox_SerialPorts.Name = "groupBox_SerialPorts";
			this.groupBox_SerialPorts.Size = new System.Drawing.Size(249, 131);
			this.groupBox_SerialPorts.TabIndex = 3;
			this.groupBox_SerialPorts.TabStop = false;
			this.groupBox_SerialPorts.Text = "&Serial COM Ports";
			// 
			// checkBox_AskForAlternateSerialPort
			// 
			this.checkBox_AskForAlternateSerialPort.AutoSize = true;
			this.checkBox_AskForAlternateSerialPort.Location = new System.Drawing.Point(12, 103);
			this.checkBox_AskForAlternateSerialPort.Name = "checkBox_AskForAlternateSerialPort";
			this.checkBox_AskForAlternateSerialPort.Size = new System.Drawing.Size(210, 17);
			this.checkBox_AskForAlternateSerialPort.TabIndex = 4;
			this.checkBox_AskForAlternateSerialPort.Text = "...ask whether to switch to another &port";
			this.toolTip.SetToolTip(this.checkBox_AskForAlternateSerialPort, "When disabled and port is no longer available,\r\nthe application will silently sto" +
        "p the terminal.");
			this.checkBox_AskForAlternateSerialPort.UseVisualStyleBackColor = true;
			this.checkBox_AskForAlternateSerialPort.CheckedChanged += new System.EventHandler(this.checkBox_AskForAlternateSerialPort_CheckedChanged);
			// 
			// label_SerialPortAvailability
			// 
			this.label_SerialPortAvailability.AutoSize = true;
			this.label_SerialPortAvailability.Location = new System.Drawing.Point(9, 85);
			this.label_SerialPortAvailability.Name = "label_SerialPortAvailability";
			this.label_SerialPortAvailability.Size = new System.Drawing.Size(180, 13);
			this.label_SerialPortAvailability.TabIndex = 3;
			this.label_SerialPortAvailability.Text = "When a port is no longer available,...";
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
			this.checkBox_DetectSerialPortsInUse.Text = "...d&etect ports that are in use";
			this.toolTip.SetToolTip(this.checkBox_DetectSerialPortsInUse, "On certain computers, discovery of serial COM ports takes several seconds.\r\nIn su" +
        "ch cases it can be useful to disable one or both of these options.");
			this.checkBox_DetectSerialPortsInUse.UseVisualStyleBackColor = true;
			this.checkBox_DetectSerialPortsInUse.CheckedChanged += new System.EventHandler(this.checkBox_DetectSerialPortsInUse_CheckedChanged);
			// 
			// groupBox_Workspace
			// 
			this.groupBox_Workspace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Workspace.Controls.Add(this.checkBox_UseRelativePaths);
			this.groupBox_Workspace.Controls.Add(this.checkBox_AutoSaveWorkspace);
			this.groupBox_Workspace.Controls.Add(this.checkBox_AutoOpenWorkspace);
			this.groupBox_Workspace.Location = new System.Drawing.Point(12, 137);
			this.groupBox_Workspace.Name = "groupBox_Workspace";
			this.groupBox_Workspace.Size = new System.Drawing.Size(281, 119);
			this.groupBox_Workspace.TabIndex = 1;
			this.groupBox_Workspace.TabStop = false;
			this.groupBox_Workspace.Text = "&Workspace";
			this.toolTip.SetToolTip(this.groupBox_Workspace, "Workspace = one or more terminals, incl. their window positions");
			// 
			// checkBox_UseRelativePaths
			// 
			this.checkBox_UseRelativePaths.AutoSize = true;
			this.checkBox_UseRelativePaths.Location = new System.Drawing.Point(12, 79);
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
			this.checkBox_AutoSaveWorkspace.Location = new System.Drawing.Point(12, 56);
			this.checkBox_AutoSaveWorkspace.Name = "checkBox_AutoSaveWorkspace";
			this.checkBox_AutoSaveWorkspace.Size = new System.Drawing.Size(206, 17);
			this.checkBox_AutoSaveWorkspace.TabIndex = 1;
			this.checkBox_AutoSaveWorkspace.Text = "&Save current workspace automatically";
			this.toolTip.SetToolTip(this.checkBox_AutoSaveWorkspace, "This setting applies to manual application execution only.\r\nAutomatic runs (i.e. " +
        "/tt, /tf, /s command line options) neither\r\nautomatically open nor save workspac" +
        "e and terminal(s).");
			this.checkBox_AutoSaveWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoSaveWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoSaveWorkspace_CheckedChanged);
			// 
			// checkBox_AutoOpenWorkspace
			// 
			this.checkBox_AutoOpenWorkspace.AutoSize = true;
			this.checkBox_AutoOpenWorkspace.Location = new System.Drawing.Point(12, 33);
			this.checkBox_AutoOpenWorkspace.Name = "checkBox_AutoOpenWorkspace";
			this.checkBox_AutoOpenWorkspace.Size = new System.Drawing.Size(208, 17);
			this.checkBox_AutoOpenWorkspace.TabIndex = 0;
			this.checkBox_AutoOpenWorkspace.Text = "&Open last active workspace on startup";
			this.toolTip.SetToolTip(this.checkBox_AutoOpenWorkspace, "This setting applies to manual application execution only.\r\nAutomatic runs (i.e. " +
        "/tt, /tf, /s command line options) neither\r\nautomatically open nor save workspac" +
        "e and terminal(s).");
			this.checkBox_AutoOpenWorkspace.UseVisualStyleBackColor = true;
			this.checkBox_AutoOpenWorkspace.CheckedChanged += new System.EventHandler(this.checkBox_AutoOpenWorkspace_CheckedChanged);
			// 
			// checkBox_AskForAlternateNetworkInterface
			// 
			this.checkBox_AskForAlternateNetworkInterface.AutoSize = true;
			this.checkBox_AskForAlternateNetworkInterface.Location = new System.Drawing.Point(12, 37);
			this.checkBox_AskForAlternateNetworkInterface.Name = "checkBox_AskForAlternateNetworkInterface";
			this.checkBox_AskForAlternateNetworkInterface.Size = new System.Drawing.Size(233, 17);
			this.checkBox_AskForAlternateNetworkInterface.TabIndex = 1;
			this.checkBox_AskForAlternateNetworkInterface.Text = "...ask whether to switch to another i&nterface";
			this.toolTip.SetToolTip(this.checkBox_AskForAlternateNetworkInterface, "When disabled and device is no longer available,\r\nthe application will silently s" +
        "top the terminal.");
			this.checkBox_AskForAlternateNetworkInterface.UseVisualStyleBackColor = true;
			this.checkBox_AskForAlternateNetworkInterface.CheckedChanged += new System.EventHandler(this.checkBox_AskForAlternateNetworkInterface_CheckedChanged);
			// 
			// groupBox_NetworkInterfaces
			// 
			this.groupBox_NetworkInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_NetworkInterfaces.Controls.Add(this.checkBox_AskForAlternateNetworkInterface);
			this.groupBox_NetworkInterfaces.Controls.Add(this.label_NetworkInterfaceAvailability);
			this.groupBox_NetworkInterfaces.Location = new System.Drawing.Point(305, 149);
			this.groupBox_NetworkInterfaces.Name = "groupBox_NetworkInterfaces";
			this.groupBox_NetworkInterfaces.Size = new System.Drawing.Size(249, 64);
			this.groupBox_NetworkInterfaces.TabIndex = 4;
			this.groupBox_NetworkInterfaces.TabStop = false;
			this.groupBox_NetworkInterfaces.Text = "&Network Interfaces";
			// 
			// label_NetworkInterfaceAvailability
			// 
			this.label_NetworkInterfaceAvailability.AutoSize = true;
			this.label_NetworkInterfaceAvailability.Location = new System.Drawing.Point(9, 19);
			this.label_NetworkInterfaceAvailability.Name = "label_NetworkInterfaceAvailability";
			this.label_NetworkInterfaceAvailability.Size = new System.Drawing.Size(209, 13);
			this.label_NetworkInterfaceAvailability.TabIndex = 0;
			this.label_NetworkInterfaceAvailability.Text = "When an interface is no longer available,...";
			// 
			// groupBox_Terminals
			// 
			this.groupBox_Terminals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Terminals.Controls.Add(this.checkBox_NotifyNonAvailableIO);
			this.groupBox_Terminals.Controls.Add(this.label_IOAvailability);
			this.groupBox_Terminals.Location = new System.Drawing.Point(12, 262);
			this.groupBox_Terminals.Name = "groupBox_Terminals";
			this.groupBox_Terminals.Size = new System.Drawing.Size(281, 65);
			this.groupBox_Terminals.TabIndex = 2;
			this.groupBox_Terminals.TabStop = false;
			this.groupBox_Terminals.Text = "&Terminals";
			// 
			// checkBox_NotifyNonAvailableIO
			// 
			this.checkBox_NotifyNonAvailableIO.AutoSize = true;
			this.checkBox_NotifyNonAvailableIO.Location = new System.Drawing.Point(12, 37);
			this.checkBox_NotifyNonAvailableIO.Name = "checkBox_NotifyNonAvailableIO";
			this.checkBox_NotifyNonAvailableIO.Size = new System.Drawing.Size(144, 17);
			this.checkBox_NotifyNonAvailableIO.TabIndex = 4;
			this.checkBox_NotifyNonAvailableIO.Text = "...show an error message";
			this.checkBox_NotifyNonAvailableIO.UseVisualStyleBackColor = true;
			this.checkBox_NotifyNonAvailableIO.CheckedChanged += new System.EventHandler(this.checkBox_NotifyNonAvailableIO_CheckedChanged);
			// 
			// label_IOAvailability
			// 
			this.label_IOAvailability.AutoSize = true;
			this.label_IOAvailability.Location = new System.Drawing.Point(9, 19);
			this.label_IOAvailability.Name = "label_IOAvailability";
			this.label_IOAvailability.Size = new System.Drawing.Size(263, 13);
			this.label_IOAvailability.TabIndex = 3;
			this.label_IOAvailability.Text = "When a port/interface/device is no longer available,...";
			// 
			// Preferences
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(566, 373);
			this.Controls.Add(this.groupBox_Terminals);
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
			this.groupBox_Terminals.ResumeLayout(false);
			this.groupBox_Terminals.PerformLayout();
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
		private System.Windows.Forms.Label label_SerialPortAvailability;
		private System.Windows.Forms.CheckBox checkBox_AskForAlternateUsbDevice;
		private System.Windows.Forms.Label label_UsbDeviceAvailability;
		private System.Windows.Forms.GroupBox groupBox_NetworkInterfaces;
		private System.Windows.Forms.CheckBox checkBox_AskForAlternateNetworkInterface;
		private System.Windows.Forms.Label label_NetworkInterfaceAvailability;
		private System.Windows.Forms.CheckBox checkBox_ShowTime;
		private System.Windows.Forms.GroupBox groupBox_Terminals;
		private System.Windows.Forms.Label label_IOAvailability;
		private System.Windows.Forms.CheckBox checkBox_NotifyNonAvailableIO;
	}
}