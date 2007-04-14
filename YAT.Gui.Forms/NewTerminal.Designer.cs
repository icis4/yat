using System;
using System.Collections.Generic;
using System.Text;

namespace HSR.YAT.Gui.Forms
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
            this.terminalSelection = new HSR.YAT.Gui.Controls.TerminalSelection();
            this.serialPortSelection = new HSR.YAT.Gui.Controls.SerialPortSelection();
            this.checkBox_OpenTerminal = new System.Windows.Forms.CheckBox();
            this.socketSelection = new HSR.YAT.Gui.Controls.SocketSelection();
            this.pictureBox_New = new System.Windows.Forms.PictureBox();
            this.button_Help = new System.Windows.Forms.Button();
            this.groupBox_NewTerminal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_New)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Cancel
            // 
            this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_Cancel.Location = new System.Drawing.Point(361, 60);
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
            this.button_OK.Location = new System.Drawing.Point(361, 33);
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
            this.groupBox_NewTerminal.Controls.Add(this.terminalSelection);
            this.groupBox_NewTerminal.Controls.Add(this.serialPortSelection);
            this.groupBox_NewTerminal.Controls.Add(this.checkBox_OpenTerminal);
            this.groupBox_NewTerminal.Controls.Add(this.socketSelection);
            this.groupBox_NewTerminal.Location = new System.Drawing.Point(73, 12);
            this.groupBox_NewTerminal.Name = "groupBox_NewTerminal";
            this.groupBox_NewTerminal.Size = new System.Drawing.Size(275, 240);
            this.groupBox_NewTerminal.TabIndex = 0;
            this.groupBox_NewTerminal.TabStop = false;
            // 
            // terminalSelection
            // 
            this.terminalSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.terminalSelection.Location = new System.Drawing.Point(6, 19);
            this.terminalSelection.Name = "terminalSelection";
            this.terminalSelection.Size = new System.Drawing.Size(260, 54);
            this.terminalSelection.TabIndex = 0;
            this.terminalSelection.IOTypeChanged += new System.EventHandler(this.terminalSelection_IOTypeChanged);
            this.terminalSelection.TerminalTypeChanged += new System.EventHandler(this.terminalSelection_TerminalTypeChanged);
            // 
            // serialPortSelection
            // 
            this.serialPortSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serialPortSelection.Location = new System.Drawing.Point(6, 73);
            this.serialPortSelection.Name = "serialPortSelection";
            this.serialPortSelection.PortId = new HSR.IO.Ports.SerialPortId(1);
            this.serialPortSelection.Size = new System.Drawing.Size(260, 27);
            this.serialPortSelection.TabIndex = 1;
            this.serialPortSelection.PortIdChanged += new System.EventHandler(this.serialPortSelection_PortIdChanged);
            // 
            // checkBox_OpenTerminal
            // 
            this.checkBox_OpenTerminal.AutoSize = true;
            this.checkBox_OpenTerminal.Checked = true;
            this.checkBox_OpenTerminal.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_OpenTerminal.Location = new System.Drawing.Point(107, 213);
            this.checkBox_OpenTerminal.Name = "checkBox_OpenTerminal";
            this.checkBox_OpenTerminal.Size = new System.Drawing.Size(91, 17);
            this.checkBox_OpenTerminal.TabIndex = 3;
            this.checkBox_OpenTerminal.Text = "&Open terminal";
            this.checkBox_OpenTerminal.CheckedChanged += new System.EventHandler(this.checkBox_OpenTerminal_CheckedChanged);
            // 
            // socketSelection
            // 
            this.socketSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.socketSelection.Location = new System.Drawing.Point(6, 100);
            this.socketSelection.Name = "socketSelection";
            this.socketSelection.Size = new System.Drawing.Size(260, 108);
            this.socketSelection.TabIndex = 2;
            this.socketSelection.LocalHostNameOrAddressChanged += new System.EventHandler(this.socketSelection_LocalHostNameOrAddressChanged);
            this.socketSelection.RemotePortChanged += new System.EventHandler(this.socketSelection_RemotePortChanged);
            this.socketSelection.RemoteHostNameOrAddressChanged += new System.EventHandler(this.socketSelection_RemoteHostNameOrAddressChanged);
            this.socketSelection.LocalPortChanged += new System.EventHandler(this.socketSelection_LocalPortChanged);
            // 
            // pictureBox_New
            // 
            this.pictureBox_New.Image = global::HSR.YAT.Gui.Forms.Properties.Resources.Image_NewDocument_24x24;
            this.pictureBox_New.Location = new System.Drawing.Point(12, 18);
            this.pictureBox_New.Name = "pictureBox_New";
            this.pictureBox_New.Size = new System.Drawing.Size(48, 48);
            this.pictureBox_New.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_New.TabIndex = 40;
            this.pictureBox_New.TabStop = false;
            // 
            // button_Help
            // 
            this.button_Help.Location = new System.Drawing.Point(361, 113);
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
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.button_Cancel;
            this.ClientSize = new System.Drawing.Size(448, 271);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.groupBox_NewTerminal);
            this.Controls.Add(this.pictureBox_New);
            this.Controls.Add(this.button_Cancel);
            this.Controls.Add(this.button_OK);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1024, 298);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(456, 298);
            this.Name = "NewTerminal";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Terminal";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.NewTerminal_Paint);
            this.groupBox_NewTerminal.ResumeLayout(false);
            this.groupBox_NewTerminal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_New)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox_New;
		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.GroupBox groupBox_NewTerminal;
		private HSR.YAT.Gui.Controls.TerminalSelection terminalSelection;
		private HSR.YAT.Gui.Controls.SerialPortSelection serialPortSelection;
		private System.Windows.Forms.CheckBox checkBox_OpenTerminal;
		private HSR.YAT.Gui.Controls.SocketSelection socketSelection;
        private System.Windows.Forms.Button button_Help;
	}
}
