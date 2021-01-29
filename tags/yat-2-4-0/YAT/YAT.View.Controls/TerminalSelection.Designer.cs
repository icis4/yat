namespace YAT.View.Controls
{
	partial class TerminalSelection
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TerminalSelection));
			this.comboBox_TerminalType = new System.Windows.Forms.ComboBox();
			this.label_TerminalType = new System.Windows.Forms.Label();
			this.comboBox_IOType = new System.Windows.Forms.ComboBox();
			this.label_IOType = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// comboBox_TerminalType
			// 
			this.comboBox_TerminalType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_TerminalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_TerminalType.Location = new System.Drawing.Point(104, 3);
			this.comboBox_TerminalType.Name = "comboBox_TerminalType";
			this.comboBox_TerminalType.Size = new System.Drawing.Size(156, 21);
			this.comboBox_TerminalType.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_TerminalType, resources.GetString("comboBox_TerminalType.ToolTip"));
			this.comboBox_TerminalType.SelectedIndexChanged += new System.EventHandler(this.comboBox_TerminalType_SelectedIndexChanged);
			// 
			// label_TerminalType
			// 
			this.label_TerminalType.AutoSize = true;
			this.label_TerminalType.Location = new System.Drawing.Point(3, 6);
			this.label_TerminalType.Name = "label_TerminalType";
			this.label_TerminalType.Size = new System.Drawing.Size(77, 13);
			this.label_TerminalType.TabIndex = 0;
			this.label_TerminalType.Text = "&Terminal Type:";
			// 
			// comboBox_IOType
			// 
			this.comboBox_IOType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_IOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_IOType.Location = new System.Drawing.Point(104, 30);
			this.comboBox_IOType.Name = "comboBox_IOType";
			this.comboBox_IOType.Size = new System.Drawing.Size(156, 21);
			this.comboBox_IOType.TabIndex = 3;
			this.comboBox_IOType.SelectedIndexChanged += new System.EventHandler(this.comboBox_IOType_SelectedIndexChanged);
			// 
			// label_IOType
			// 
			this.label_IOType.AutoSize = true;
			this.label_IOType.Location = new System.Drawing.Point(3, 33);
			this.label_IOType.Name = "label_IOType";
			this.label_IOType.Size = new System.Drawing.Size(56, 13);
			this.label_IOType.TabIndex = 2;
			this.label_IOType.Text = "&I/O Type:";
			// 
			// TerminalSelection
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.comboBox_TerminalType);
			this.Controls.Add(this.label_TerminalType);
			this.Controls.Add(this.comboBox_IOType);
			this.Controls.Add(this.label_IOType);
			this.Name = "TerminalSelection";
			this.Size = new System.Drawing.Size(263, 54);
			this.EnabledChanged += new System.EventHandler(this.TerminalSelection_EnabledChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TerminalSelection_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox_TerminalType;
		private System.Windows.Forms.Label label_TerminalType;
		private System.Windows.Forms.ComboBox comboBox_IOType;
		private System.Windows.Forms.Label label_IOType;
		private System.Windows.Forms.ToolTip toolTip;
	}
}
