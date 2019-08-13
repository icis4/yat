namespace YAT.Gui.Controls
{
	partial class SerialPortSelection
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
			this.button_RefreshPorts = new System.Windows.Forms.Button();
			this.comboBox_Port = new System.Windows.Forms.ComboBox();
			this.label_Port = new System.Windows.Forms.Label();
			this.timer_ShowScanDialog = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// button_RefreshPorts
			// 
			this.button_RefreshPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshPorts.Location = new System.Drawing.Point(205, 2);
			this.button_RefreshPorts.Name = "button_RefreshPorts";
			this.button_RefreshPorts.Size = new System.Drawing.Size(52, 23);
			this.button_RefreshPorts.TabIndex = 2;
			this.button_RefreshPorts.Text = "Refresh";
			this.button_RefreshPorts.Click += new System.EventHandler(this.button_RefreshPorts_Click);
			// 
			// comboBox_Port
			// 
			this.comboBox_Port.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Port.ItemHeight = 13;
			this.comboBox_Port.Location = new System.Drawing.Point(101, 3);
			this.comboBox_Port.Name = "comboBox_Port";
			this.comboBox_Port.Size = new System.Drawing.Size(98, 21);
			this.comboBox_Port.TabIndex = 1;
			this.comboBox_Port.SelectedIndexChanged += new System.EventHandler(this.comboBox_Port_SelectedIndexChanged);
			// 
			// label_Port
			// 
			this.label_Port.AutoSize = true;
			this.label_Port.Location = new System.Drawing.Point(3, 6);
			this.label_Port.Name = "label_Port";
			this.label_Port.Size = new System.Drawing.Size(55, 13);
			this.label_Port.TabIndex = 0;
			this.label_Port.Text = "COM port:";
			// 
			// timer_ShowScanDialog
			// 
			this.timer_ShowScanDialog.Interval = 500;
			this.timer_ShowScanDialog.Tick += new System.EventHandler(this.timer_ShowScanDialog_Tick);
			// 
			// SerialPortSelection
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.button_RefreshPorts);
			this.Controls.Add(this.comboBox_Port);
			this.Controls.Add(this.label_Port);
			this.Name = "SerialPortSelection";
			this.Size = new System.Drawing.Size(260, 27);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SerialPortSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_RefreshPorts;
		private System.Windows.Forms.ComboBox comboBox_Port;
		private System.Windows.Forms.Label label_Port;
		private System.Windows.Forms.Timer timer_ShowScanDialog;
	}
}