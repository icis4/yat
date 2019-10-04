namespace YAT.View.Controls
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
			this.comboBox_Port = new MKY.Windows.Forms.ComboBoxEx();
			this.label_Port = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.label_OnDialogMessage = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_RefreshPorts
			// 
			this.button_RefreshPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_RefreshPorts.Image = global::YAT.View.Controls.Properties.Resources.Image_Tool_arrow_refresh_small_16x16;
			this.button_RefreshPorts.Location = new System.Drawing.Point(258, 22);
			this.button_RefreshPorts.Name = "button_RefreshPorts";
			this.button_RefreshPorts.Size = new System.Drawing.Size(24, 21);
			this.button_RefreshPorts.TabIndex = 3;
			this.toolTip.SetToolTip(this.button_RefreshPorts, "Refresh the list with the serial ports available on the system");
			this.button_RefreshPorts.Click += new System.EventHandler(this.button_RefreshPorts_Click);
			// 
			// comboBox_Port
			// 
			this.comboBox_Port.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Port.ItemHeight = 13;
			this.comboBox_Port.Location = new System.Drawing.Point(6, 22);
			this.comboBox_Port.Name = "comboBox_Port";
			this.comboBox_Port.Size = new System.Drawing.Size(251, 21);
			this.comboBox_Port.TabIndex = 2;
			this.toolTip.SetToolTip(this.comboBox_Port, "Either select a port from the list, or fill in \"COM..\" (COM1..COM65535).");
			this.comboBox_Port.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_Port_Validating);
			// 
			// label_Port
			// 
			this.label_Port.AutoSize = true;
			this.label_Port.Location = new System.Drawing.Point(3, 6);
			this.label_Port.Name = "label_Port";
			this.label_Port.Size = new System.Drawing.Size(58, 13);
			this.label_Port.TabIndex = 0;
			this.label_Port.Text = "&Serial Port:";
			// 
			// label_OnDialogMessage
			// 
			this.label_OnDialogMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_OnDialogMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_OnDialogMessage.ForeColor = System.Drawing.Color.DarkOrange;
			this.label_OnDialogMessage.Location = new System.Drawing.Point(67, 6);
			this.label_OnDialogMessage.Name = "label_OnDialogMessage";
			this.label_OnDialogMessage.Size = new System.Drawing.Size(216, 13);
			this.label_OnDialogMessage.TabIndex = 1;
			this.label_OnDialogMessage.Text = "<Message>";
			this.label_OnDialogMessage.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// SerialPortSelection
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.label_OnDialogMessage);
			this.Controls.Add(this.button_RefreshPorts);
			this.Controls.Add(this.comboBox_Port);
			this.Controls.Add(this.label_Port);
			this.Name = "SerialPortSelection";
			this.Size = new System.Drawing.Size(285, 46);
			this.EnabledChanged += new System.EventHandler(this.SerialPortSelection_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SerialPortSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_RefreshPorts;
		private MKY.Windows.Forms.ComboBoxEx comboBox_Port;
		private System.Windows.Forms.Label label_Port;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label_OnDialogMessage;
	}
}
