namespace HSR.YAT.Gui.Controls
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
			this.checkBox_TcpClientAutoReconnect = new System.Windows.Forms.CheckBox();
			this.label_TcpClientAutoReconnectIntervalUnit = new System.Windows.Forms.Label();
			this.textBox_TcpClientAutoReconnectInterval = new System.Windows.Forms.TextBox();
			this.label_TcpClientAutoReconnectInterval = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// checkBox_TcpClientAutoReconnect
			// 
			this.checkBox_TcpClientAutoReconnect.AutoSize = true;
			this.checkBox_TcpClientAutoReconnect.Location = new System.Drawing.Point(3, 3);
			this.checkBox_TcpClientAutoReconnect.Name = "checkBox_TcpClientAutoReconnect";
			this.checkBox_TcpClientAutoReconnect.Size = new System.Drawing.Size(232, 17);
			this.checkBox_TcpClientAutoReconnect.TabIndex = 0;
			this.checkBox_TcpClientAutoReconnect.Text = "When connection is lost, try to reconnect to";
			this.checkBox_TcpClientAutoReconnect.UseVisualStyleBackColor = true;
			this.checkBox_TcpClientAutoReconnect.CheckedChanged += new System.EventHandler(this.checkBox_TcpClientAutoReconnect_CheckedChanged);
			// 
			// label_TcpClientAutoReconnectIntervalUnit
			// 
			this.label_TcpClientAutoReconnectIntervalUnit.AutoSize = true;
			this.label_TcpClientAutoReconnectIntervalUnit.Location = new System.Drawing.Point(151, 23);
			this.label_TcpClientAutoReconnectIntervalUnit.Name = "label_TcpClientAutoReconnectIntervalUnit";
			this.label_TcpClientAutoReconnectIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_TcpClientAutoReconnectIntervalUnit.TabIndex = 3;
			this.label_TcpClientAutoReconnectIntervalUnit.Text = "ms";
			this.label_TcpClientAutoReconnectIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_TcpClientAutoReconnectInterval
			// 
			this.textBox_TcpClientAutoReconnectInterval.Enabled = false;
			this.textBox_TcpClientAutoReconnectInterval.Location = new System.Drawing.Point(101, 20);
			this.textBox_TcpClientAutoReconnectInterval.Name = "textBox_TcpClientAutoReconnectInterval";
			this.textBox_TcpClientAutoReconnectInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_TcpClientAutoReconnectInterval.TabIndex = 2;
			this.textBox_TcpClientAutoReconnectInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_TcpClientAutoReconnectInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TcpClientAutoReconnectInterval_Validating);
			// 
			// label_TcpClientAutoReconnectInterval
			// 
			this.label_TcpClientAutoReconnectInterval.AutoSize = true;
			this.label_TcpClientAutoReconnectInterval.Location = new System.Drawing.Point(34, 23);
			this.label_TcpClientAutoReconnectInterval.Name = "label_TcpClientAutoReconnectInterval";
			this.label_TcpClientAutoReconnectInterval.Size = new System.Drawing.Size(65, 13);
			this.label_TcpClientAutoReconnectInterval.TabIndex = 1;
			this.label_TcpClientAutoReconnectInterval.Text = "server every";
			this.label_TcpClientAutoReconnectInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SocketSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label_TcpClientAutoReconnectInterval);
			this.Controls.Add(this.label_TcpClientAutoReconnectIntervalUnit);
			this.Controls.Add(this.textBox_TcpClientAutoReconnectInterval);
			this.Controls.Add(this.checkBox_TcpClientAutoReconnect);
			this.Name = "SocketSettings";
			this.Size = new System.Drawing.Size(260, 42);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SocketSettings_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_TcpClientAutoReconnect;
		private System.Windows.Forms.Label label_TcpClientAutoReconnectIntervalUnit;
		private System.Windows.Forms.TextBox textBox_TcpClientAutoReconnectInterval;
		private System.Windows.Forms.Label label_TcpClientAutoReconnectInterval;

	}
}
