namespace MKY.YAT.Gui.Controls
{
	partial class SerialPortSettings
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
			this.comboBox_StopBits = new System.Windows.Forms.ComboBox();
			this.comboBox_Handshake = new System.Windows.Forms.ComboBox();
			this.comboBox_Parity = new System.Windows.Forms.ComboBox();
			this.comboBox_DataBits = new System.Windows.Forms.ComboBox();
			this.comboBox_BaudRate = new System.Windows.Forms.ComboBox();
			this.label_FlowControl = new System.Windows.Forms.Label();
			this.label_StopBits = new System.Windows.Forms.Label();
			this.label_Parity = new System.Windows.Forms.Label();
			this.label_DataBits = new System.Windows.Forms.Label();
			this.label_BaudRate = new System.Windows.Forms.Label();
			this.label_HandshakeRemarks_1 = new System.Windows.Forms.Label();
			this.label_HandshakeRemarks_2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// comboBox_StopBits
			// 
			this.comboBox_StopBits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_StopBits.Location = new System.Drawing.Point(101, 84);
			this.comboBox_StopBits.Name = "comboBox_StopBits";
			this.comboBox_StopBits.Size = new System.Drawing.Size(156, 21);
			this.comboBox_StopBits.TabIndex = 7;
			this.comboBox_StopBits.SelectedIndexChanged += new System.EventHandler(this.comboBox_StopBits_SelectedIndexChanged);
			// 
			// comboBox_Handshake
			// 
			this.comboBox_Handshake.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Handshake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Handshake.Location = new System.Drawing.Point(3, 130);
			this.comboBox_Handshake.Name = "comboBox_Handshake";
			this.comboBox_Handshake.Size = new System.Drawing.Size(254, 21);
			this.comboBox_Handshake.TabIndex = 9;
			this.comboBox_Handshake.SelectedIndexChanged += new System.EventHandler(this.comboBox_Handshake_SelectedIndexChanged);
			// 
			// comboBox_Parity
			// 
			this.comboBox_Parity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Parity.Location = new System.Drawing.Point(101, 57);
			this.comboBox_Parity.Name = "comboBox_Parity";
			this.comboBox_Parity.Size = new System.Drawing.Size(156, 21);
			this.comboBox_Parity.TabIndex = 5;
			this.comboBox_Parity.SelectedIndexChanged += new System.EventHandler(this.comboBox_Parity_SelectedIndexChanged);
			// 
			// comboBox_DataBits
			// 
			this.comboBox_DataBits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_DataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_DataBits.Location = new System.Drawing.Point(101, 30);
			this.comboBox_DataBits.Name = "comboBox_DataBits";
			this.comboBox_DataBits.Size = new System.Drawing.Size(156, 21);
			this.comboBox_DataBits.TabIndex = 3;
			this.comboBox_DataBits.SelectedIndexChanged += new System.EventHandler(this.comboBox_DataBits_SelectedIndexChanged);
			// 
			// comboBox_BaudRate
			// 
			this.comboBox_BaudRate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_BaudRate.Location = new System.Drawing.Point(101, 3);
			this.comboBox_BaudRate.Name = "comboBox_BaudRate";
			this.comboBox_BaudRate.Size = new System.Drawing.Size(156, 21);
			this.comboBox_BaudRate.TabIndex = 1;
			this.comboBox_BaudRate.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_BaudRate_Validating);
			this.comboBox_BaudRate.SelectedIndexChanged += new System.EventHandler(this.comboBox_BaudRate_SelectedIndexChanged);
			// 
			// label_FlowControl
			// 
			this.label_FlowControl.AutoSize = true;
			this.label_FlowControl.Location = new System.Drawing.Point(3, 114);
			this.label_FlowControl.Name = "label_FlowControl";
			this.label_FlowControl.Size = new System.Drawing.Size(65, 13);
			this.label_FlowControl.TabIndex = 8;
			this.label_FlowControl.Text = "Handshake:";
			// 
			// label_StopBits
			// 
			this.label_StopBits.AutoSize = true;
			this.label_StopBits.Location = new System.Drawing.Point(3, 87);
			this.label_StopBits.Name = "label_StopBits";
			this.label_StopBits.Size = new System.Drawing.Size(51, 13);
			this.label_StopBits.TabIndex = 6;
			this.label_StopBits.Text = "Stop bits:";
			// 
			// label_Parity
			// 
			this.label_Parity.AutoSize = true;
			this.label_Parity.Location = new System.Drawing.Point(3, 60);
			this.label_Parity.Name = "label_Parity";
			this.label_Parity.Size = new System.Drawing.Size(36, 13);
			this.label_Parity.TabIndex = 4;
			this.label_Parity.Text = "Parity:";
			// 
			// label_DataBits
			// 
			this.label_DataBits.AutoSize = true;
			this.label_DataBits.Location = new System.Drawing.Point(3, 33);
			this.label_DataBits.Name = "label_DataBits";
			this.label_DataBits.Size = new System.Drawing.Size(52, 13);
			this.label_DataBits.TabIndex = 2;
			this.label_DataBits.Text = "Data bits:";
			// 
			// label_BaudRate
			// 
			this.label_BaudRate.AutoSize = true;
			this.label_BaudRate.Location = new System.Drawing.Point(3, 6);
			this.label_BaudRate.Name = "label_BaudRate";
			this.label_BaudRate.Size = new System.Drawing.Size(83, 13);
			this.label_BaudRate.TabIndex = 0;
			this.label_BaudRate.Text = "Bits per second:";
			// 
			// label_HandshakeRemarks_1
			// 
			this.label_HandshakeRemarks_1.AutoSize = true;
			this.label_HandshakeRemarks_1.Location = new System.Drawing.Point(3, 154);
			this.label_HandshakeRemarks_1.Name = "label_HandshakeRemarks_1";
			this.label_HandshakeRemarks_1.Size = new System.Drawing.Size(46, 26);
			this.label_HandshakeRemarks_1.TabIndex = 10;
			this.label_HandshakeRemarks_1.Text = "Manual:\r\nRS-485:";
			// 
			// label_HandshakeRemarks_2
			// 
			this.label_HandshakeRemarks_2.AutoSize = true;
			this.label_HandshakeRemarks_2.Location = new System.Drawing.Point(55, 154);
			this.label_HandshakeRemarks_2.Name = "label_HandshakeRemarks_2";
			this.label_HandshakeRemarks_2.Size = new System.Drawing.Size(160, 26);
			this.label_HandshakeRemarks_2.TabIndex = 11;
			this.label_HandshakeRemarks_2.Text = "RTS / DTR can be set manually\r\nRTS is set high while sending\r\n";
			// 
			// SerialPortSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label_HandshakeRemarks_2);
			this.Controls.Add(this.label_HandshakeRemarks_1);
			this.Controls.Add(this.comboBox_StopBits);
			this.Controls.Add(this.comboBox_Handshake);
			this.Controls.Add(this.comboBox_Parity);
			this.Controls.Add(this.comboBox_DataBits);
			this.Controls.Add(this.comboBox_BaudRate);
			this.Controls.Add(this.label_FlowControl);
			this.Controls.Add(this.label_StopBits);
			this.Controls.Add(this.label_Parity);
			this.Controls.Add(this.label_DataBits);
			this.Controls.Add(this.label_BaudRate);
			this.Name = "SerialPortSettings";
			this.Size = new System.Drawing.Size(260, 186);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox_StopBits;
		private System.Windows.Forms.ComboBox comboBox_Handshake;
		private System.Windows.Forms.ComboBox comboBox_Parity;
		private System.Windows.Forms.ComboBox comboBox_DataBits;
		private System.Windows.Forms.ComboBox comboBox_BaudRate;
		private System.Windows.Forms.Label label_FlowControl;
		private System.Windows.Forms.Label label_StopBits;
		private System.Windows.Forms.Label label_Parity;
		private System.Windows.Forms.Label label_DataBits;
		private System.Windows.Forms.Label label_BaudRate;
		private System.Windows.Forms.Label label_HandshakeRemarks_1;
		private System.Windows.Forms.Label label_HandshakeRemarks_2;
	}
}
