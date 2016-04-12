﻿namespace YAT.Gui.Controls
{
	partial class SocketSelection
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
			this.label_RemoteHost = new System.Windows.Forms.Label();
			this.label_RemotePort = new System.Windows.Forms.Label();
			this.textBox_RemotePort = new System.Windows.Forms.TextBox();
			this.comboBox_RemoteHost = new System.Windows.Forms.ComboBox();
			this.textBox_LocalPort = new System.Windows.Forms.TextBox();
			this.label_LocalPort = new System.Windows.Forms.Label();
			this.label_LocalAddress = new System.Windows.Forms.Label();
			this.comboBox_LocalInterface = new System.Windows.Forms.ComboBox();
			this.button_RefreshLocalInterfaces = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.comboBox_LocalFilter = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label_RemoteHost
			// 
			this.label_RemoteHost.AutoSize = true;
			this.label_RemoteHost.Location = new System.Drawing.Point(3, 6);
			this.label_RemoteHost.Name = "label_RemoteHost";
			this.label_RemoteHost.Size = new System.Drawing.Size(72, 13);
			this.label_RemoteHost.TabIndex = 0;
			this.label_RemoteHost.Text = "Remote &Host:";
			// 
			// label_RemotePort
			// 
			this.label_RemotePort.AutoSize = true;
			this.label_RemotePort.Location = new System.Drawing.Point(3, 33);
			this.label_RemotePort.Name = "label_RemotePort";
			this.label_RemotePort.Size = new System.Drawing.Size(93, 13);
			this.label_RemotePort.TabIndex = 2;
			this.label_RemotePort.Text = "&Remote TCP Port:";
			// 
			// textBox_RemotePort
			// 
			this.textBox_RemotePort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_RemotePort.Location = new System.Drawing.Point(101, 30);
			this.textBox_RemotePort.Name = "textBox_RemotePort";
			this.textBox_RemotePort.Size = new System.Drawing.Size(156, 20);
			this.textBox_RemotePort.TabIndex = 3;
			this.textBox_RemotePort.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_RemotePort_Validating);
			// 
			// comboBox_RemoteHost
			// 
			this.comboBox_RemoteHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_RemoteHost.Location = new System.Drawing.Point(101, 3);
			this.comboBox_RemoteHost.Name = "comboBox_RemoteHost";
			this.comboBox_RemoteHost.Size = new System.Drawing.Size(156, 21);
			this.comboBox_RemoteHost.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_RemoteHost, "Either select a preset from the list, or fill in any IPv4 or IPv6 address.\r\n\r\nCon" +
        "tact YAT via \"Help > Request Feature\" to request additional presets.");
			this.comboBox_RemoteHost.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_RemoteHost_Validating);
			// 
			// textBox_LocalPort
			// 
			this.textBox_LocalPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_LocalPort.Location = new System.Drawing.Point(101, 110);
			this.textBox_LocalPort.Name = "textBox_LocalPort";
			this.textBox_LocalPort.Size = new System.Drawing.Size(156, 20);
			this.textBox_LocalPort.TabIndex = 9;
			this.textBox_LocalPort.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_LocalPort_Validating);
			// 
			// label_LocalPort
			// 
			this.label_LocalPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_LocalPort.AutoSize = true;
			this.label_LocalPort.Location = new System.Drawing.Point(3, 113);
			this.label_LocalPort.Name = "label_LocalPort";
			this.label_LocalPort.Size = new System.Drawing.Size(82, 13);
			this.label_LocalPort.TabIndex = 8;
			this.label_LocalPort.Text = "&Local TCP Port:";
			// 
			// label_LocalAddress
			// 
			this.label_LocalAddress.AutoSize = true;
			this.label_LocalAddress.Location = new System.Drawing.Point(3, 59);
			this.label_LocalAddress.Name = "label_LocalAddress";
			this.label_LocalAddress.Size = new System.Drawing.Size(81, 13);
			this.label_LocalAddress.TabIndex = 4;
			this.label_LocalAddress.Text = "Local &Interface:";
			// 
			// comboBox_LocalInterface
			// 
			this.comboBox_LocalInterface.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_LocalInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_LocalInterface.Location = new System.Drawing.Point(6, 83);
			this.comboBox_LocalInterface.Name = "comboBox_LocalInterface";
			this.comboBox_LocalInterface.Size = new System.Drawing.Size(251, 21);
			this.comboBox_LocalInterface.TabIndex = 6;
			this.comboBox_LocalInterface.SelectedIndexChanged += new System.EventHandler(this.comboBox_LocalInterface_SelectedIndexChanged);
			// 
			// button_RefreshLocalInterfaces
			// 
			this.button_RefreshLocalInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshLocalInterfaces.Image = global::YAT.Gui.Controls.Properties.Resources.Image_Tool_arrow_refresh_small_16x16;
			this.button_RefreshLocalInterfaces.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button_RefreshLocalInterfaces.Location = new System.Drawing.Point(258, 83);
			this.button_RefreshLocalInterfaces.Name = "button_RefreshLocalInterfaces";
			this.button_RefreshLocalInterfaces.Size = new System.Drawing.Size(24, 21);
			this.button_RefreshLocalInterfaces.TabIndex = 7;
			this.button_RefreshLocalInterfaces.Click += new System.EventHandler(this.button_RefreshLocalInterfaces_Click);
			// 
			// comboBox_LocalFilter
			// 
			this.comboBox_LocalFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_LocalFilter.Location = new System.Drawing.Point(101, 83);
			this.comboBox_LocalFilter.Name = "comboBox_LocalFilter";
			this.comboBox_LocalFilter.Size = new System.Drawing.Size(156, 21);
			this.comboBox_LocalFilter.TabIndex = 5;
			this.toolTip.SetToolTip(this.comboBox_LocalFilter, "Either select a preset from the list, or fill in any IPv4 or IPv6 address.\r\n\r\nCon" +
        "tact YAT via \"Help > Request Feature\" to request additional presets.");
			this.comboBox_LocalFilter.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_LocalFilter_Validating);
			// 
			// SocketSelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboBox_LocalFilter);
			this.Controls.Add(this.button_RefreshLocalInterfaces);
			this.Controls.Add(this.textBox_LocalPort);
			this.Controls.Add(this.textBox_RemotePort);
			this.Controls.Add(this.comboBox_LocalInterface);
			this.Controls.Add(this.comboBox_RemoteHost);
			this.Controls.Add(this.label_LocalPort);
			this.Controls.Add(this.label_RemotePort);
			this.Controls.Add(this.label_LocalAddress);
			this.Controls.Add(this.label_RemoteHost);
			this.Name = "SocketSelection";
			this.Size = new System.Drawing.Size(285, 133);
			this.EnabledChanged += new System.EventHandler(this.SocketSelection_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SocketSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_RemoteHost;
		private System.Windows.Forms.Label label_RemotePort;
		private System.Windows.Forms.TextBox textBox_RemotePort;
		private System.Windows.Forms.ComboBox comboBox_RemoteHost;
		private System.Windows.Forms.TextBox textBox_LocalPort;
		private System.Windows.Forms.Label label_LocalPort;
		private System.Windows.Forms.Label label_LocalAddress;
		private System.Windows.Forms.ComboBox comboBox_LocalInterface;
		private System.Windows.Forms.Button button_RefreshLocalInterfaces;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ComboBox comboBox_LocalFilter;
	}
}
