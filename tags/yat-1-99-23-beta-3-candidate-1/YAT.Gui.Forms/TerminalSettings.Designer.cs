using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Gui.Forms
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
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.groupBox_Settings = new System.Windows.Forms.GroupBox();
			this.button_AdvancedSettings = new System.Windows.Forms.Button();
			this.button_TextOrBinarySettings = new System.Windows.Forms.Button();
			this.terminalSelection = new YAT.Gui.Controls.TerminalSelection();
			this.groupBox_PortSettings = new System.Windows.Forms.GroupBox();
			this.socketSettings = new YAT.Gui.Controls.SocketSettings();
			this.serialPortSelection = new YAT.Gui.Controls.SerialPortSelection();
			this.serialPortSettings = new YAT.Gui.Controls.SerialPortSettings();
			this.socketSelection = new YAT.Gui.Controls.SocketSelection();
			this.menuItem_Port = new System.Windows.Forms.ContextMenu();
			this.menuItem_Port_Search = new System.Windows.Forms.MenuItem();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.button_Help = new System.Windows.Forms.Button();
			this.groupBox_Settings.SuspendLayout();
			this.groupBox_PortSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(480, 33);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(480, 62);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// groupBox_Settings
			// 
			this.groupBox_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Settings.Controls.Add(this.button_AdvancedSettings);
			this.groupBox_Settings.Controls.Add(this.button_TextOrBinarySettings);
			this.groupBox_Settings.Controls.Add(this.terminalSelection);
			this.groupBox_Settings.Controls.Add(this.groupBox_PortSettings);
			this.groupBox_Settings.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Settings.Name = "groupBox_Settings";
			this.groupBox_Settings.Size = new System.Drawing.Size(453, 388);
			this.groupBox_Settings.TabIndex = 0;
			this.groupBox_Settings.TabStop = false;
			// 
			// button_AdvancedSettings
			// 
			this.button_AdvancedSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_AdvancedSettings.Location = new System.Drawing.Point(323, 351);
			this.button_AdvancedSettings.Name = "button_AdvancedSettings";
			this.button_AdvancedSettings.Size = new System.Drawing.Size(114, 23);
			this.button_AdvancedSettings.TabIndex = 4;
			this.button_AdvancedSettings.Text = "&Advanced Settings...";
			this.button_AdvancedSettings.Click += new System.EventHandler(this.button_AdvancedSettings_Click);
			// 
			// button_TextOrBinarySettings
			// 
			this.button_TextOrBinarySettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_TextOrBinarySettings.Location = new System.Drawing.Point(323, 21);
			this.button_TextOrBinarySettings.Name = "button_TextOrBinarySettings";
			this.button_TextOrBinarySettings.Size = new System.Drawing.Size(114, 23);
			this.button_TextOrBinarySettings.TabIndex = 3;
			this.button_TextOrBinarySettings.Text = "&Text Settings...";
			this.button_TextOrBinarySettings.Click += new System.EventHandler(this.button_TextOrBinarySettings_Click);
			// 
			// terminalSelection
			// 
			this.terminalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.terminalSelection.Location = new System.Drawing.Point(12, 19);
			this.terminalSelection.Name = "terminalSelection";
			this.terminalSelection.Size = new System.Drawing.Size(260, 54);
			this.terminalSelection.TabIndex = 0;
			this.terminalSelection.IOTypeChanged += new System.EventHandler(this.terminalSelection_IOTypeChanged);
			this.terminalSelection.TerminalTypeChanged += new System.EventHandler(this.terminalSelection_TerminalTypeChanged);
			// 
			// groupBox_PortSettings
			// 
			this.groupBox_PortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_PortSettings.Controls.Add(this.socketSettings);
			this.groupBox_PortSettings.Controls.Add(this.serialPortSelection);
			this.groupBox_PortSettings.Controls.Add(this.serialPortSettings);
			this.groupBox_PortSettings.Controls.Add(this.socketSelection);
			this.groupBox_PortSettings.Location = new System.Drawing.Point(6, 79);
			this.groupBox_PortSettings.Name = "groupBox_PortSettings";
			this.groupBox_PortSettings.Size = new System.Drawing.Size(300, 303);
			this.groupBox_PortSettings.TabIndex = 1;
			this.groupBox_PortSettings.TabStop = false;
			this.groupBox_PortSettings.Text = "Port &Settings";
			// 
			// socketSettings
			// 
			this.socketSettings.Location = new System.Drawing.Point(6, 146);
			this.socketSettings.Name = "socketSettings";
			this.socketSettings.Size = new System.Drawing.Size(260, 42);
			this.socketSettings.TabIndex = 3;
			this.socketSettings.TcpClientAutoReconnectChanged += new System.EventHandler(this.socketSettings_TcpClientAutoReconnectChanged);
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
			// serialPortSettings
			// 
			this.serialPortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.serialPortSettings.Location = new System.Drawing.Point(6, 64);
			this.serialPortSettings.Name = "serialPortSettings";
			this.serialPortSettings.Size = new System.Drawing.Size(260, 232);
			this.serialPortSettings.TabIndex = 1;
			this.serialPortSettings.ParityChanged += new System.EventHandler(this.serialPortSettings_ParityChanged);
			this.serialPortSettings.BaudRateChanged += new System.EventHandler(this.serialPortSettings_BaudRateChanged);
			this.serialPortSettings.AutoReopenChanged += new System.EventHandler(this.serialPortSettings_AutoReopenChanged);
			this.serialPortSettings.FlowControlChanged += new System.EventHandler(this.serialPortSettings_FlowControlChanged);
			this.serialPortSettings.StopBitsChanged += new System.EventHandler(this.serialPortSettings_StopBitsChanged);
			this.serialPortSettings.DataBitsChanged += new System.EventHandler(this.serialPortSettings_DataBitsChanged);
			// 
			// socketSelection
			// 
			this.socketSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.socketSelection.Location = new System.Drawing.Point(6, 19);
			this.socketSelection.Name = "socketSelection";
			this.socketSelection.Size = new System.Drawing.Size(285, 125);
			this.socketSelection.TabIndex = 2;
			this.socketSelection.LocalInterfaceChanged += new System.EventHandler(this.socketSelection_LocalInterfaceChanged);
			this.socketSelection.LocalUdpPortChanged += new System.EventHandler(this.socketSelection_LocalUdpPortChanged);
			this.socketSelection.RemoteHostChanged += new System.EventHandler(this.socketSelection_RemoteHostChanged);
			this.socketSelection.RemotePortChanged += new System.EventHandler(this.socketSelection_RemotePortChanged);
			this.socketSelection.LocalTcpPortChanged += new System.EventHandler(this.socketSelection_LocalTcpPortChanged);
			// 
			// menuItem_Port
			// 
			this.menuItem_Port.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem_Port_Search});
			// 
			// menuItem_Port_Search
			// 
			this.menuItem_Port_Search.Index = 0;
			this.menuItem_Port_Search.Text = "Search For Ports...";
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(480, 111);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// button_Help
			// 
			this.button_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Help.Location = new System.Drawing.Point(480, 161);
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
			this.ClientSize = new System.Drawing.Size(567, 403);
			this.Controls.Add(this.button_Help);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.groupBox_Settings);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(1024, 439);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(575, 439);
			this.Name = "TerminalSettings";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Terminal Settings";
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TerminalSettings_Paint);
			this.groupBox_Settings.ResumeLayout(false);
			this.groupBox_PortSettings.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.GroupBox groupBox_PortSettings;
		private System.Windows.Forms.GroupBox groupBox_Settings;
		private System.Windows.Forms.ContextMenu menuItem_Port;
		private System.Windows.Forms.MenuItem menuItem_Port_Search;
		private YAT.Gui.Controls.TerminalSelection terminalSelection;
		private YAT.Gui.Controls.SerialPortSettings serialPortSettings;
		private System.Windows.Forms.Button button_AdvancedSettings;
		private System.Windows.Forms.Button button_TextOrBinarySettings;
		private YAT.Gui.Controls.SocketSelection socketSelection;
		private YAT.Gui.Controls.SerialPortSelection serialPortSelection;
		private YAT.Gui.Controls.SocketSettings socketSettings;
		private System.Windows.Forms.Button button_Help;
	}
}
