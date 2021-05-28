namespace YAT.View.Controls
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialPortSettings));
			this.comboBox_StopBits = new System.Windows.Forms.ComboBox();
			this.comboBox_FlowControl = new System.Windows.Forms.ComboBox();
			this.comboBox_Parity = new System.Windows.Forms.ComboBox();
			this.comboBox_DataBits = new System.Windows.Forms.ComboBox();
			this.comboBox_BaudRate = new MKY.Windows.Forms.ComboBoxEx();
			this.label_FlowControl = new System.Windows.Forms.Label();
			this.label_StopBits = new System.Windows.Forms.Label();
			this.label_Parity = new System.Windows.Forms.Label();
			this.label_DataBits = new System.Windows.Forms.Label();
			this.label_BaudRate = new System.Windows.Forms.Label();
			this.label_AutoReopenIntervalUnit = new System.Windows.Forms.Label();
			this.textBox_AutoReopenInterval = new MKY.Windows.Forms.TextBoxEx();
			this.checkBox_AutoReopen = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.checkBox_AliveMonitor = new System.Windows.Forms.CheckBox();
			this.textBox_AliveMonitorInterval = new MKY.Windows.Forms.TextBoxEx();
			this.label_AliveMonitorIntervalUnit = new System.Windows.Forms.Label();
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
			// comboBox_FlowControl
			// 
			this.comboBox_FlowControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_FlowControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_FlowControl.Location = new System.Drawing.Point(3, 130);
			this.comboBox_FlowControl.Name = "comboBox_FlowControl";
			this.comboBox_FlowControl.Size = new System.Drawing.Size(254, 21);
			this.comboBox_FlowControl.TabIndex = 9;
			this.toolTip.SetToolTip(this.comboBox_FlowControl, resources.GetString("comboBox_FlowControl.ToolTip"));
			this.comboBox_FlowControl.SelectedIndexChanged += new System.EventHandler(this.comboBox_FlowControl_SelectedIndexChanged);
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
			this.toolTip.SetToolTip(this.comboBox_BaudRate, "Either select a standard rate from the list, or enter a non-standard rate.\r\n\r\nNon" +
        "-standard rates may not be supported by all serial port devices.");
			this.comboBox_BaudRate.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_BaudRate_Validating);
			// 
			// label_FlowControl
			// 
			this.label_FlowControl.AutoSize = true;
			this.label_FlowControl.Location = new System.Drawing.Point(3, 114);
			this.label_FlowControl.Name = "label_FlowControl";
			this.label_FlowControl.Size = new System.Drawing.Size(68, 13);
			this.label_FlowControl.TabIndex = 8;
			this.label_FlowControl.Text = "Flow Control:";
			// 
			// label_StopBits
			// 
			this.label_StopBits.AutoSize = true;
			this.label_StopBits.Location = new System.Drawing.Point(3, 87);
			this.label_StopBits.Name = "label_StopBits";
			this.label_StopBits.Size = new System.Drawing.Size(52, 13);
			this.label_StopBits.TabIndex = 6;
			this.label_StopBits.Text = "Stop Bits:";
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
			this.label_DataBits.Size = new System.Drawing.Size(53, 13);
			this.label_DataBits.TabIndex = 2;
			this.label_DataBits.Text = "Data Bits:";
			// 
			// label_BaudRate
			// 
			this.label_BaudRate.AutoSize = true;
			this.label_BaudRate.Location = new System.Drawing.Point(3, 6);
			this.label_BaudRate.Name = "label_BaudRate";
			this.label_BaudRate.Size = new System.Drawing.Size(85, 13);
			this.label_BaudRate.TabIndex = 0;
			this.label_BaudRate.Text = "Bits per Second:";
			// 
			// label_AutoReopenIntervalUnit
			// 
			this.label_AutoReopenIntervalUnit.AutoSize = true;
			this.label_AutoReopenIntervalUnit.Location = new System.Drawing.Point(205, 239);
			this.label_AutoReopenIntervalUnit.Name = "label_AutoReopenIntervalUnit";
			this.label_AutoReopenIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_AutoReopenIntervalUnit.TabIndex = 15;
			this.label_AutoReopenIntervalUnit.Text = "ms";
			this.label_AutoReopenIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_AutoReopenInterval
			// 
			this.textBox_AutoReopenInterval.Enabled = false;
			this.textBox_AutoReopenInterval.Location = new System.Drawing.Point(151, 236);
			this.textBox_AutoReopenInterval.Name = "textBox_AutoReopenInterval";
			this.textBox_AutoReopenInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_AutoReopenInterval.TabIndex = 14;
			this.textBox_AutoReopenInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_AutoReopenInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_AutoReopenInterval_Validating);
			// 
			// checkBox_AutoReopen
			// 
			this.checkBox_AutoReopen.AutoSize = true;
			this.checkBox_AutoReopen.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox_AutoReopen.Location = new System.Drawing.Point(6, 219);
			this.checkBox_AutoReopen.Name = "checkBox_AutoReopen";
			this.checkBox_AutoReopen.Size = new System.Drawing.Size(142, 30);
			this.checkBox_AutoReopen.TabIndex = 13;
			this.checkBox_AutoReopen.Text = "When disconnected, try\r\n to reopen the port every";
			this.toolTip.SetToolTip(this.checkBox_AutoReopen, resources.GetString("checkBox_AutoReopen.ToolTip"));
			this.checkBox_AutoReopen.UseVisualStyleBackColor = true;
			this.checkBox_AutoReopen.CheckedChanged += new System.EventHandler(this.checkBox_AutoReopen_CheckedChanged);
			// 
			// checkBox_AliveMonitor
			// 
			this.checkBox_AliveMonitor.AutoSize = true;
			this.checkBox_AliveMonitor.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkBox_AliveMonitor.Location = new System.Drawing.Point(6, 177);
			this.checkBox_AliveMonitor.Name = "checkBox_AliveMonitor";
			this.checkBox_AliveMonitor.Size = new System.Drawing.Size(214, 30);
			this.checkBox_AliveMonitor.TabIndex = 10;
			this.checkBox_AliveMonitor.Text = "When connected, detect disconnect by\r\n monitoring the port every";
			this.toolTip.SetToolTip(this.checkBox_AliveMonitor, resources.GetString("checkBox_AliveMonitor.ToolTip"));
			this.checkBox_AliveMonitor.UseVisualStyleBackColor = true;
			this.checkBox_AliveMonitor.CheckedChanged += new System.EventHandler(this.checkBox_AliveMonitor_CheckedChanged);
			// 
			// textBox_AliveMonitorInterval
			// 
			this.textBox_AliveMonitorInterval.Enabled = false;
			this.textBox_AliveMonitorInterval.Location = new System.Drawing.Point(151, 193);
			this.textBox_AliveMonitorInterval.Name = "textBox_AliveMonitorInterval";
			this.textBox_AliveMonitorInterval.Size = new System.Drawing.Size(48, 20);
			this.textBox_AliveMonitorInterval.TabIndex = 11;
			this.textBox_AliveMonitorInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_AliveMonitorInterval.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_AliveMonitorInterval_Validating);
			// 
			// label_AliveMonitorIntervalUnit
			// 
			this.label_AliveMonitorIntervalUnit.AutoSize = true;
			this.label_AliveMonitorIntervalUnit.Location = new System.Drawing.Point(205, 196);
			this.label_AliveMonitorIntervalUnit.Name = "label_AliveMonitorIntervalUnit";
			this.label_AliveMonitorIntervalUnit.Size = new System.Drawing.Size(20, 13);
			this.label_AliveMonitorIntervalUnit.TabIndex = 12;
			this.label_AliveMonitorIntervalUnit.Text = "ms";
			this.label_AliveMonitorIntervalUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// SerialPortSettings
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.label_AliveMonitorIntervalUnit);
			this.Controls.Add(this.textBox_AliveMonitorInterval);
			this.Controls.Add(this.checkBox_AliveMonitor);
			this.Controls.Add(this.label_AutoReopenIntervalUnit);
			this.Controls.Add(this.textBox_AutoReopenInterval);
			this.Controls.Add(this.checkBox_AutoReopen);
			this.Controls.Add(this.comboBox_StopBits);
			this.Controls.Add(this.comboBox_FlowControl);
			this.Controls.Add(this.comboBox_Parity);
			this.Controls.Add(this.comboBox_DataBits);
			this.Controls.Add(this.comboBox_BaudRate);
			this.Controls.Add(this.label_FlowControl);
			this.Controls.Add(this.label_StopBits);
			this.Controls.Add(this.label_Parity);
			this.Controls.Add(this.label_DataBits);
			this.Controls.Add(this.label_BaudRate);
			this.Name = "SerialPortSettings";
			this.Size = new System.Drawing.Size(260, 259);
			this.EnabledChanged += new System.EventHandler(this.SerialPortSettings_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SerialPortSettings_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox_StopBits;
		private System.Windows.Forms.ComboBox comboBox_FlowControl;
		private System.Windows.Forms.ComboBox comboBox_Parity;
		private System.Windows.Forms.ComboBox comboBox_DataBits;
		private MKY.Windows.Forms.ComboBoxEx comboBox_BaudRate;
		private System.Windows.Forms.Label label_FlowControl;
		private System.Windows.Forms.Label label_StopBits;
		private System.Windows.Forms.Label label_Parity;
		private System.Windows.Forms.Label label_DataBits;
		private System.Windows.Forms.Label label_BaudRate;
		private System.Windows.Forms.Label label_AutoReopenIntervalUnit;
		private MKY.Windows.Forms.TextBoxEx textBox_AutoReopenInterval;
		private System.Windows.Forms.CheckBox checkBox_AutoReopen;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox checkBox_AliveMonitor;
		private MKY.Windows.Forms.TextBoxEx textBox_AliveMonitorInterval;
		private System.Windows.Forms.Label label_AliveMonitorIntervalUnit;
	}
}
