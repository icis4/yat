namespace YAT.Gui.Controls
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
			this.label_RemoteHost = new System.Windows.Forms.Label();
			this.label_RemotePort = new System.Windows.Forms.Label();
			this.textBox_RemotePort = new System.Windows.Forms.TextBox();
			this.comboBox_RemoteHost = new System.Windows.Forms.ComboBox();
			this.textBox_LocalPort = new System.Windows.Forms.TextBox();
			this.label_LocalPort = new System.Windows.Forms.Label();
			this.label_LocalInterface = new System.Windows.Forms.Label();
			this.comboBox_LocalInterface = new System.Windows.Forms.ComboBox();
			this.button_RefreshLocalInterfaces = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label_RemoteHost
			// 
			this.label_RemoteHost.AutoSize = true;
			this.label_RemoteHost.Location = new System.Drawing.Point(3, 6);
			this.label_RemoteHost.Name = "label_RemoteHost";
			this.label_RemoteHost.Size = new System.Drawing.Size(72, 13);
			this.label_RemoteHost.TabIndex = 0;
			this.label_RemoteHost.Text = "Remote Host:";
			// 
			// label_RemotePort
			// 
			this.label_RemotePort.AutoSize = true;
			this.label_RemotePort.Location = new System.Drawing.Point(3, 33);
			this.label_RemotePort.Name = "label_RemotePort";
			this.label_RemotePort.Size = new System.Drawing.Size(92, 13);
			this.label_RemotePort.TabIndex = 2;
			this.label_RemotePort.Text = "Remote TCP port:";
			// 
			// textBox_RemotePort
			// 
			this.textBox_RemotePort.Location = new System.Drawing.Point(101, 30);
			this.textBox_RemotePort.Name = "textBox_RemotePort";
			this.textBox_RemotePort.Size = new System.Drawing.Size(156, 20);
			this.textBox_RemotePort.TabIndex = 3;
			this.textBox_RemotePort.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_RemotePort_Validating);
			// 
			// comboBox_RemoteHost
			// 
			this.comboBox_RemoteHost.Location = new System.Drawing.Point(101, 3);
			this.comboBox_RemoteHost.Name = "comboBox_RemoteHost";
			this.comboBox_RemoteHost.Size = new System.Drawing.Size(156, 21);
			this.comboBox_RemoteHost.TabIndex = 1;
			this.comboBox_RemoteHost.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_RemoteHost_Validating);
			// 
			// textBox_LocalPort
			// 
			this.textBox_LocalPort.Location = new System.Drawing.Point(101, 83);
			this.textBox_LocalPort.Name = "textBox_LocalPort";
			this.textBox_LocalPort.Size = new System.Drawing.Size(156, 20);
			this.textBox_LocalPort.TabIndex = 8;
			this.textBox_LocalPort.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_LocalPort_Validating);
			// 
			// label_LocalPort
			// 
			this.label_LocalPort.AutoSize = true;
			this.label_LocalPort.Location = new System.Drawing.Point(3, 86);
			this.label_LocalPort.Name = "label_LocalPort";
			this.label_LocalPort.Size = new System.Drawing.Size(81, 13);
			this.label_LocalPort.TabIndex = 7;
			this.label_LocalPort.Text = "Local TCP port:";
			// 
			// label_LocalInterface
			// 
			this.label_LocalInterface.AutoSize = true;
			this.label_LocalInterface.Location = new System.Drawing.Point(3, 59);
			this.label_LocalInterface.Name = "label_LocalInterface";
			this.label_LocalInterface.Size = new System.Drawing.Size(81, 13);
			this.label_LocalInterface.TabIndex = 4;
			this.label_LocalInterface.Text = "Local Interface:";
			// 
			// comboBox_LocalInterface
			// 
			this.comboBox_LocalInterface.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_LocalInterface.Location = new System.Drawing.Point(101, 56);
			this.comboBox_LocalInterface.Name = "comboBox_LocalInterface";
			this.comboBox_LocalInterface.Size = new System.Drawing.Size(156, 21);
			this.comboBox_LocalInterface.TabIndex = 5;
			this.comboBox_LocalInterface.SelectedIndexChanged += new System.EventHandler(this.comboBox_LocalInterface_SelectedIndexChanged);
			// 
			// button_RefreshLocalInterfaces
			// 
			this.button_RefreshLocalInterfaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshLocalInterfaces.Image = global::YAT.Gui.Controls.Properties.Resources.Image_Refresh_16x16;
			this.button_RefreshLocalInterfaces.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.button_RefreshLocalInterfaces.Location = new System.Drawing.Point(258, 56);
			this.button_RefreshLocalInterfaces.Name = "button_RefreshLocalInterfaces";
			this.button_RefreshLocalInterfaces.Size = new System.Drawing.Size(24, 21);
			this.button_RefreshLocalInterfaces.TabIndex = 6;
			this.button_RefreshLocalInterfaces.Click += new System.EventHandler(this.button_RefreshLocalInterfaces_Click);
			// 
			// SocketSelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button_RefreshLocalInterfaces);
			this.Controls.Add(this.textBox_LocalPort);
			this.Controls.Add(this.textBox_RemotePort);
			this.Controls.Add(this.comboBox_LocalInterface);
			this.Controls.Add(this.comboBox_RemoteHost);
			this.Controls.Add(this.label_LocalPort);
			this.Controls.Add(this.label_RemotePort);
			this.Controls.Add(this.label_LocalInterface);
			this.Controls.Add(this.label_RemoteHost);
			this.Name = "SocketSelection";
			this.Size = new System.Drawing.Size(285, 106);
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
		private System.Windows.Forms.Label label_LocalInterface;
		private System.Windows.Forms.ComboBox comboBox_LocalInterface;
		private System.Windows.Forms.Button button_RefreshLocalInterfaces;
	}
}
