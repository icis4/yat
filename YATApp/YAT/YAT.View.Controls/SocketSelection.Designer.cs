namespace YAT.View.Controls
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
			this.comboBox_RemotePort = new MKY.Windows.Forms.ComboBoxEx();
			this.comboBox_RemoteHost = new MKY.Windows.Forms.ComboBoxEx();
			this.comboBox_LocalPort = new MKY.Windows.Forms.ComboBoxEx();
			this.label_LocalPort = new System.Windows.Forms.Label();
			this.label_LocalInterface = new System.Windows.Forms.Label();
			this.comboBox_LocalInterface = new System.Windows.Forms.ComboBox();
			this.button_RefreshLocalInterfaces = new System.Windows.Forms.Button();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.comboBox_LocalFilter = new MKY.Windows.Forms.ComboBoxEx();
			this.label_OnDialogMessage = new System.Windows.Forms.Label();
			this.label_LocalFilter = new System.Windows.Forms.Label();
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
			// comboBox_RemotePort
			// 
			this.comboBox_RemotePort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_RemotePort.Location = new System.Drawing.Point(101, 30);
			this.comboBox_RemotePort.Name = "comboBox_RemotePort";
			this.comboBox_RemotePort.Size = new System.Drawing.Size(156, 21);
			this.comboBox_RemotePort.TabIndex = 3;
			this.comboBox_RemotePort.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_RemotePort_Validating);
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
			// comboBox_LocalPort
			// 
			this.comboBox_LocalPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_LocalPort.Location = new System.Drawing.Point(101, 102);
			this.comboBox_LocalPort.Name = "comboBox_LocalPort";
			this.comboBox_LocalPort.Size = new System.Drawing.Size(156, 21);
			this.comboBox_LocalPort.TabIndex = 9;
			this.comboBox_LocalPort.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_LocalPort_Validating);
			// 
			// label_LocalPort
			// 
			this.label_LocalPort.AutoSize = true;
			this.label_LocalPort.Location = new System.Drawing.Point(3, 105);
			this.label_LocalPort.Name = "label_LocalPort";
			this.label_LocalPort.Size = new System.Drawing.Size(82, 13);
			this.label_LocalPort.TabIndex = 8;
			this.label_LocalPort.Text = "&Local TCP Port:";
			// 
			// label_LocalInterface
			// 
			this.label_LocalInterface.AutoSize = true;
			this.label_LocalInterface.Location = new System.Drawing.Point(3, 59);
			this.label_LocalInterface.Name = "label_LocalInterface";
			this.label_LocalInterface.Size = new System.Drawing.Size(81, 13);
			this.label_LocalInterface.TabIndex = 4;
			this.label_LocalInterface.Text = "Local &Interface:";
			// 
			// comboBox_LocalInterface
			// 
			this.comboBox_LocalInterface.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_LocalInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_LocalInterface.Location = new System.Drawing.Point(6, 75);
			this.comboBox_LocalInterface.Name = "comboBox_LocalInterface";
			this.comboBox_LocalInterface.Size = new System.Drawing.Size(251, 21);
			this.comboBox_LocalInterface.TabIndex = 6;
			this.comboBox_LocalInterface.SelectedIndexChanged += new System.EventHandler(this.comboBox_LocalInterface_SelectedIndexChanged);
			// 
			// button_RefreshLocalInterfaces
			// 
			this.button_RefreshLocalInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshLocalInterfaces.Image = global::YAT.View.Controls.Properties.Resources.Image_Tool_arrow_refresh_small_16x16;
			this.button_RefreshLocalInterfaces.Location = new System.Drawing.Point(258, 75);
			this.button_RefreshLocalInterfaces.Name = "button_RefreshLocalInterfaces";
			this.button_RefreshLocalInterfaces.Size = new System.Drawing.Size(24, 21);
			this.button_RefreshLocalInterfaces.TabIndex = 7;
			this.button_RefreshLocalInterfaces.UseVisualStyleBackColor = true;
			this.button_RefreshLocalInterfaces.Click += new System.EventHandler(this.button_RefreshLocalInterfaces_Click);
			// 
			// comboBox_LocalFilter
			// 
			this.comboBox_LocalFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_LocalFilter.Location = new System.Drawing.Point(101, 128);
			this.comboBox_LocalFilter.Name = "comboBox_LocalFilter";
			this.comboBox_LocalFilter.Size = new System.Drawing.Size(156, 21);
			this.comboBox_LocalFilter.TabIndex = 11;
			this.toolTip.SetToolTip(this.comboBox_LocalFilter, "Either select a preset from the list, or fill in any IPv4 or IPv6 address.\r\n\r\nCon" +
        "tact YAT via \"Help > Request Feature\" to request additional presets.");
			this.comboBox_LocalFilter.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_LocalFilter_Validating);
			// 
			// label_OnDialogMessage
			// 
			this.label_OnDialogMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_OnDialogMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_OnDialogMessage.ForeColor = System.Drawing.Color.DarkOrange;
			this.label_OnDialogMessage.Location = new System.Drawing.Point(90, 59);
			this.label_OnDialogMessage.Name = "label_OnDialogMessage";
			this.label_OnDialogMessage.Size = new System.Drawing.Size(195, 13);
			this.label_OnDialogMessage.TabIndex = 5;
			this.label_OnDialogMessage.Text = "<Message>";
			this.label_OnDialogMessage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label_LocalFilter
			// 
			this.label_LocalFilter.AutoSize = true;
			this.label_LocalFilter.Location = new System.Drawing.Point(3, 131);
			this.label_LocalFilter.Name = "label_LocalFilter";
			this.label_LocalFilter.Size = new System.Drawing.Size(61, 13);
			this.label_LocalFilter.TabIndex = 10;
			this.label_LocalFilter.Text = "Local &Filter:";
			// 
			// SocketSelection
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.label_LocalFilter);
			this.Controls.Add(this.label_OnDialogMessage);
			this.Controls.Add(this.comboBox_LocalFilter);
			this.Controls.Add(this.button_RefreshLocalInterfaces);
			this.Controls.Add(this.comboBox_LocalPort);
			this.Controls.Add(this.comboBox_RemotePort);
			this.Controls.Add(this.comboBox_LocalInterface);
			this.Controls.Add(this.comboBox_RemoteHost);
			this.Controls.Add(this.label_LocalPort);
			this.Controls.Add(this.label_RemotePort);
			this.Controls.Add(this.label_LocalInterface);
			this.Controls.Add(this.label_RemoteHost);
			this.Name = "SocketSelection";
			this.Size = new System.Drawing.Size(285, 156);
			this.EnabledChanged += new System.EventHandler(this.SocketSelection_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SocketSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_RemoteHost;
		private System.Windows.Forms.Label label_RemotePort;
		private MKY.Windows.Forms.ComboBoxEx comboBox_RemotePort;
		private MKY.Windows.Forms.ComboBoxEx comboBox_RemoteHost;
		private MKY.Windows.Forms.ComboBoxEx comboBox_LocalPort;
		private System.Windows.Forms.Label label_LocalPort;
		private System.Windows.Forms.Label label_LocalInterface;
		private System.Windows.Forms.ComboBox comboBox_LocalInterface;
		private System.Windows.Forms.Button button_RefreshLocalInterfaces;
		private System.Windows.Forms.ToolTip toolTip;
		private MKY.Windows.Forms.ComboBoxEx comboBox_LocalFilter;
		private System.Windows.Forms.Label label_OnDialogMessage;
		private System.Windows.Forms.Label label_LocalFilter;
	}
}
