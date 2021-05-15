namespace YAT.View.Controls
{
	partial class SocketSettings
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SocketSettings));
			this.radioButton_UdpServerSendMode_None = new System.Windows.Forms.RadioButton();
			this.radioButton_UdpServerSendMode_First = new System.Windows.Forms.RadioButton();
			this.radioButton_UdpServerSendMode_MostRecent = new System.Windows.Forms.RadioButton();
			this.panel_Tcp = new System.Windows.Forms.Panel();
			this.label_TcpClientAutoReconnectInterval = new System.Windows.Forms.Label();
			this.label_TcpClientAutoReconnectIntervalUnit = new System.Windows.Forms.Label();
			this.textBox_TcpClientAutoReconnectInterval = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_TcpClientAutoReconnect = new System.Windows.Forms.CheckBox();
			this.panel_Udp = new System.Windows.Forms.Panel();
			this.label_UdpServerSendMode = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.panel_Tcp.SuspendLayout();
			this.panel_Udp.SuspendLayout();
			this.SuspendLayout();
			// 
			// radioButton_UdpServerSendMode_None
			// 
			this.radioButton_UdpServerSendMode_None.AutoSize = true;
			this.radioButton_UdpServerSendMode_None.Location = new System.Drawing.Point(6, 57);
			this.radioButton_UdpServerSendMode_None.Name = "radioButton_UdpServerSendMode_None";
			this.radioButton_UdpServerSendMode_None.Size = new System.Drawing.Size(102, 17);
			this.radioButton_UdpServerSendMode_None.TabIndex = 1;
			this.radioButton_UdpServerSendMode_None.TabStop = true;
			this.radioButton_UdpServerSendMode_None.Text = "Send is &disabled";
			this.radioButton_UdpServerSendMode_None.UseVisualStyleBackColor = true;
			this.radioButton_UdpServerSendMode_None.CheckedChanged += new System.EventHandler(this.radioButton_UdpServerSendMode_None_CheckedChanged);
			// 
			// radioButton_UdpServerSendMode_First
			// 
			this.radioButton_UdpServerSendMode_First.AutoSize = true;
			this.radioButton_UdpServerSendMode_First.Location = new System.Drawing.Point(6, 38);
			this.radioButton_UdpServerSendMode_First.Name = "radioButton_UdpServerSendMode_First";
			this.radioButton_UdpServerSendMode_First.Size = new System.Drawing.Size(159, 17);
			this.radioButton_UdpServerSendMode_First.TabIndex = 2;
			this.radioButton_UdpServerSendMode_First.TabStop = true;
			this.radioButton_UdpServerSendMode_First.Text = "Send to the &first active client";
			this.radioButton_UdpServerSendMode_First.UseVisualStyleBackColor = true;
			this.radioButton_UdpServerSendMode_First.CheckedChanged += new System.EventHandler(this.radioButton_UdpServerSendMode_First_CheckedChanged);
			// 
			// radioButton_UdpServerSendMode_MostRecent
			// 
			this.radioButton_UdpServerSendMode_MostRecent.AutoSize = true;
			this.radioButton_UdpServerSendMode_MostRecent.Location = new System.Drawing.Point(6, 19);
			this.radioButton_UdpServerSendMode_MostRecent.Name = "radioButton_UdpServerSendMode_MostRecent";
			this.radioButton_UdpServerSendMode_MostRecent.Size = new System.Drawing.Size(205, 17);
			this.radioButton_UdpServerSendMode_MostRecent.TabIndex = 3;
			this.radioButton_UdpServerSendMode_MostRecent.TabStop = true;
			this.radioButton_UdpServerSendMode_MostRecent.Text = "Send to the &most recently active client";
			this.radioButton_UdpServerSendMode_MostRecent.UseVisualStyleBackColor = true;
			this.radioButton_UdpServerSendMode_MostRecent.CheckedChanged += new System.EventHandler(this.radioButton_UdpServerSendMode_MostRecent_CheckedChanged);
			// 
			// panel_Tcp
			// 
			this.panel_Tcp.Controls.Add(this.label_TcpClientAutoReconnectInterval);
			this.panel_Tcp.Controls.Add(this.label_TcpClientAutoReconnectIntervalUnit);
			this.panel_Tcp.Controls.Add(this.textBox_TcpClientAutoReconnectInterval);
			this.panel_Tcp.Controls.Add(this.checkBox_TcpClientAutoReconnect);
			this.panel_Tcp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Tcp.Location = new System.Drawing.Point(0, 0);
			this.panel_Tcp.Name = "panel_Tcp";
			this.panel_Tcp.Size = new System.Drawing.Size(260, 80);
			this.panel_Tcp.TabIndex = 0;
			// 
			// label_TcpClientAutoReconnectInterval
			// 
			this.label_TcpClientAutoReconnectInterval.AutoSize = true;
			this.label_TcpClientAutoReconnectInterval.Location = new System.Drawing.Point(34, 23);
			this.label_TcpClientAutoReconnectInterval.Name = "label_TcpClientAutoReconnectInterval";
			this.label_TcpClientAutoReconnectInterval.Size = new System.Drawing.Size(103, 13);
			this.label_TcpClientAutoReconnectInterval.TabIndex = 1;
			this.label_TcpClientAutoReconnectInterval.Text = "server approx. every";
			this.label_TcpClientAutoReconnectInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label_TcpClientAutoReconnectIntervalUnit
			// 
			this.label_TcpClientAutoReconnectIntervalUnit.AutoSize = true;
			this.label_TcpClientAutoReconnectIntervalUnit.Location = new System.Drawing.Point(189, 23);
			this.label_TcpClientAutoReconnectIntervalUnit.Name = "label_TcpClientAutoReconnectIntervalUnit";
			this.label_TcpClientAutoReconnectIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_TcpClientAutoReconnectIntervalUnit.TabIndex = 3;
			this.label_TcpClientAutoReconnectIntervalUnit.Text = "ms";
			this.label_TcpClientAutoReconnectIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_TcpClientAutoReconnectInterval
			// 
			this.textBox_TcpClientAutoReconnectInterval.Enabled = false;
			this.textBox_TcpClientAutoReconnectInterval.Location = new System.Drawing.Point(139, 20);
			this.textBox_TcpClientAutoReconnectInterval.Name = "textBox_TcpClientAutoReconnectInterval";
			this.textBox_TcpClientAutoReconnectInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_TcpClientAutoReconnectInterval.TabIndex = 2;
			this.textBox_TcpClientAutoReconnectInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_TcpClientAutoReconnectInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TcpClientAutoReconnectInterval_Validating);
			// 
			// checkBox_TcpClientAutoReconnect
			// 
			this.checkBox_TcpClientAutoReconnect.AutoSize = true;
			this.checkBox_TcpClientAutoReconnect.Location = new System.Drawing.Point(3, 3);
			this.checkBox_TcpClientAutoReconnect.Name = "checkBox_TcpClientAutoReconnect";
			this.checkBox_TcpClientAutoReconnect.Size = new System.Drawing.Size(232, 17);
			this.checkBox_TcpClientAutoReconnect.TabIndex = 0;
			this.checkBox_TcpClientAutoReconnect.Text = "When connection is lost, try to reconnect to";
			this.toolTip.SetToolTip(this.checkBox_TcpClientAutoReconnect, resources.GetString("checkBox_TcpClientAutoReconnect.ToolTip"));
			this.checkBox_TcpClientAutoReconnect.UseVisualStyleBackColor = true;
			this.checkBox_TcpClientAutoReconnect.CheckedChanged += new System.EventHandler(this.checkBox_TcpClientAutoReconnect_CheckedChanged);
			// 
			// panel_Udp
			// 
			this.panel_Udp.Controls.Add(this.label_UdpServerSendMode);
			this.panel_Udp.Controls.Add(this.radioButton_UdpServerSendMode_MostRecent);
			this.panel_Udp.Controls.Add(this.radioButton_UdpServerSendMode_None);
			this.panel_Udp.Controls.Add(this.radioButton_UdpServerSendMode_First);
			this.panel_Udp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Udp.Location = new System.Drawing.Point(0, 0);
			this.panel_Udp.Name = "panel_Udp";
			this.panel_Udp.Size = new System.Drawing.Size(260, 80);
			this.panel_Udp.TabIndex = 1;
			// 
			// label_UdpServerSendMode
			// 
			this.label_UdpServerSendMode.AutoSize = true;
			this.label_UdpServerSendMode.Location = new System.Drawing.Point(3, 3);
			this.label_UdpServerSendMode.Name = "label_UdpServerSendMode";
			this.label_UdpServerSendMode.Size = new System.Drawing.Size(99, 13);
			this.label_UdpServerSendMode.TabIndex = 0;
			this.label_UdpServerSendMode.Text = "Server Send Mode:";
			// 
			// SocketSettings
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.panel_Udp);
			this.Controls.Add(this.panel_Tcp);
			this.Name = "SocketSettings";
			this.Size = new System.Drawing.Size(260, 80);
			this.EnabledChanged += new System.EventHandler(this.SocketSettings_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SocketSettings_Paint);
			this.panel_Tcp.ResumeLayout(false);
			this.panel_Tcp.PerformLayout();
			this.panel_Udp.ResumeLayout(false);
			this.panel_Udp.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.RadioButton radioButton_UdpServerSendMode_MostRecent;
		private System.Windows.Forms.RadioButton radioButton_UdpServerSendMode_First;
		private System.Windows.Forms.RadioButton radioButton_UdpServerSendMode_None;
		private System.Windows.Forms.Panel panel_Tcp;
		private System.Windows.Forms.Panel panel_Udp;
		private System.Windows.Forms.Label label_UdpServerSendMode;
		private System.Windows.Forms.Label label_TcpClientAutoReconnectInterval;
		private System.Windows.Forms.Label label_TcpClientAutoReconnectIntervalUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_TcpClientAutoReconnectInterval;
		private System.Windows.Forms.CheckBox checkBox_TcpClientAutoReconnect;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
