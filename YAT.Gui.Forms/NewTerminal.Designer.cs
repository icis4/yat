using System;
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
            this.button_Cancel = new System.Windows.Forms.Button();
            this.button_OK = new System.Windows.Forms.Button();
            this.groupBox_NewTerminal = new System.Windows.Forms.GroupBox();
            this.groupBox_PortSettings = new System.Windows.Forms.GroupBox();
            this.usbHidPortSelection = new YAT.Gui.Controls.UsbHidDeviceSelection();
            this.serialPortSelection = new YAT.Gui.Controls.SerialPortSelection();
            this.socketSelection = new YAT.Gui.Controls.SocketSelection();
            this.terminalSelection = new YAT.Gui.Controls.TerminalSelection();
            this.checkBox_StartTerminal = new System.Windows.Forms.CheckBox();
            this.pictureBox_New = new System.Windows.Forms.PictureBox();
            this.button_Help = new System.Windows.Forms.Button();
            this.groupBox_NewTerminal.SuspendLayout();
            this.groupBox_PortSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_New)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(398, 60);
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
            this.groupBox_NewTerminal.Size = new System.Drawing.Size(312, 362);
            this.groupBox_NewTerminal.TabIndex = 0;
            this.groupBox_NewTerminal.TabStop = false;
            // 
            // groupBox_PortSettings
            // 
            this.groupBox_PortSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_PortSettings.Controls.Add(this.usbHidPortSelection);
            this.groupBox_PortSettings.Controls.Add(this.serialPortSelection);
            this.groupBox_PortSettings.Controls.Add(this.socketSelection);
            this.groupBox_PortSettings.Location = new System.Drawing.Point(6, 79);
            this.groupBox_PortSettings.Name = "groupBox_PortSettings";
            this.groupBox_PortSettings.Size = new System.Drawing.Size(300, 246);
            this.groupBox_PortSettings.TabIndex = 1;
            this.groupBox_PortSettings.TabStop = false;
            this.groupBox_PortSettings.Text = "Port &Settings";
            // 
            // usbHidPortSelection
            // 
            this.usbHidPortSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usbHidPortSelection.DeviceId = null;
            this.usbHidPortSelection.Location = new System.Drawing.Point(6, 190);
            this.usbHidPortSelection.Name = "usbHidPortSelection";
            this.usbHidPortSelection.Size = new System.Drawing.Size(285, 46);
            this.usbHidPortSelection.TabIndex = 0;
            this.usbHidPortSelection.DeviceIdChanged += new System.EventHandler(this.usbHidPortSelection_DeviceIdChanged);
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
            // socketSelection
            // 
            this.socketSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.socketSelection.Location = new System.Drawing.Point(6, 65);
            this.socketSelection.Name = "socketSelection";
            this.socketSelection.Size = new System.Drawing.Size(285, 125);
            this.socketSelection.TabIndex = 1;
            this.socketSelection.LocalInterfaceChanged += new System.EventHandler(this.socketSelection_LocalInterfaceChanged);
            this.socketSelection.LocalUdpPortChanged += new System.EventHandler(this.socketSelection_LocalUdpPortChanged);
            this.socketSelection.RemoteHostChanged += new System.EventHandler(this.socketSelection_RemoteHostChanged);
            this.socketSelection.RemotePortChanged += new System.EventHandler(this.socketSelection_RemotePortChanged);
            this.socketSelection.LocalTcpPortChanged += new System.EventHandler(this.socketSelection_LocalTcpPortChanged);
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
            // checkBox_StartTerminal
            // 
            this.checkBox_StartTerminal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox_StartTerminal.AutoSize = true;
            this.checkBox_StartTerminal.Checked = true;
            this.checkBox_StartTerminal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_StartTerminal.Location = new System.Drawing.Point(113, 335);
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
            this.button_Help.Location = new System.Drawing.Point(398, 112);
            this.button_Help.Name = "button_Help";
            this.button_Help.Size = new System.Drawing.Size(75, 23);
            this.button_Help.TabIndex = 3;
            this.button_Help.Text = "Help";
            this.button_Help.UseVisualStyleBackColor = true;
            this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
            // 
            // NewTerminal
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(485, 382);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.groupBox_NewTerminal);
            this.Controls.Add(this.pictureBox_New);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 420);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(493, 420);
            this.Name = "NewTerminal";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Terminal";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NewTerminal_Paint);
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
        private YAT.Gui.Controls.UsbHidDeviceSelection usbHidPortSelection;
	}
}
