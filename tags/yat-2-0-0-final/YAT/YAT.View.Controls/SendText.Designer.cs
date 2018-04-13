namespace YAT.View.Controls
{
	partial class SendText
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
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_SetMultiLineText = new System.Windows.Forms.Button();
			this.button_Send = new System.Windows.Forms.Button();
			this.comboBox_SingleLineText = new MKY.Windows.Forms.ComboBoxEx();
			this.comboBox_ExplicitDefaultRadix = new MKY.Windows.Forms.ComboBoxEx();
			this.splitContainer_Send = new System.Windows.Forms.SplitContainer();
			this.splitContainer_ExplicitDefaultRadix = new System.Windows.Forms.SplitContainer();
			this.panel_ExplicitDefaultRadix = new System.Windows.Forms.Panel();
			this.panel_Command = new System.Windows.Forms.Panel();
			this.panel_Send = new System.Windows.Forms.Panel();
			this.splitContainer_Send.Panel1.SuspendLayout();
			this.splitContainer_Send.Panel2.SuspendLayout();
			this.splitContainer_Send.SuspendLayout();
			this.splitContainer_ExplicitDefaultRadix.Panel1.SuspendLayout();
			this.splitContainer_ExplicitDefaultRadix.Panel2.SuspendLayout();
			this.splitContainer_ExplicitDefaultRadix.SuspendLayout();
			this.panel_ExplicitDefaultRadix.SuspendLayout();
			this.panel_Command.SuspendLayout();
			this.panel_Send.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_SetMultiLineText
			// 
			this.button_SetMultiLineText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SetMultiLineText.Location = new System.Drawing.Point(247, 3);
			this.button_SetMultiLineText.Name = "button_SetMultiLineText";
			this.button_SetMultiLineText.Size = new System.Drawing.Size(25, 21);
			this.button_SetMultiLineText.TabIndex = 1;
			this.button_SetMultiLineText.Text = "...";
			this.toolTip.SetToolTip(this.button_SetMultiLineText, "Enter multi-line text.\r\n\r\nLines will be sent sequentially,\r\nby default, the EOL s" +
        "equence\r\nwill be appended to each line.");
			this.button_SetMultiLineText.UseVisualStyleBackColor = true;
			this.button_SetMultiLineText.Click += new System.EventHandler(this.button_SetMultiLineText_Click);
			// 
			// button_Send
			// 
			this.button_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Send.Enabled = false;
			this.button_Send.Location = new System.Drawing.Point(3, 3);
			this.button_Send.Name = "button_Send";
			this.button_Send.Size = new System.Drawing.Size(177, 21);
			this.button_Send.TabIndex = 0;
			this.button_Send.Text = "Send Text (F3)";
			this.toolTip.SetToolTip(this.button_Send, "Send text");
			this.button_Send.Click += new System.EventHandler(this.button_Send_Click);
			// 
			// comboBox_SingleLineText
			// 
			this.comboBox_SingleLineText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_SingleLineText.Location = new System.Drawing.Point(3, 3);
			this.comboBox_SingleLineText.Name = "comboBox_SingleLineText";
			this.comboBox_SingleLineText.Size = new System.Drawing.Size(238, 21);
			this.comboBox_SingleLineText.TabIndex = 0;
			this.toolTip.SetToolTip(this.comboBox_SingleLineText, "Fill-in text, <Enter> to send,\r\ndrop down for recent text,\r\n[...] to enter multi-" +
        "line text");
			this.comboBox_SingleLineText.SelectedIndexChanged += new System.EventHandler(this.comboBox_SingleLineText_SelectedIndexChanged);
			this.comboBox_SingleLineText.TextChanged += new System.EventHandler(this.comboBox_SingleLineText_TextChanged);
			this.comboBox_SingleLineText.Enter += new System.EventHandler(this.comboBox_SingleLineText_Enter);
			this.comboBox_SingleLineText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_SingleLineText_KeyPress);
			this.comboBox_SingleLineText.Leave += new System.EventHandler(this.comboBox_SingleLineText_Leave);
			this.comboBox_SingleLineText.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_SingleLineText_Validating);
			// 
			// comboBox_ExplicitDefaultRadix
			// 
			this.comboBox_ExplicitDefaultRadix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_ExplicitDefaultRadix.Location = new System.Drawing.Point(3, 3);
			this.comboBox_ExplicitDefaultRadix.Name = "comboBox_ExplicitDefaultRadix";
			this.comboBox_ExplicitDefaultRadix.Size = new System.Drawing.Size(74, 21);
			this.comboBox_ExplicitDefaultRadix.TabIndex = 0;
			this.toolTip.SetToolTip(this.comboBox_ExplicitDefaultRadix, "Select the radix which is used by default,\r\ni.e. without an escape sequence.");
			this.comboBox_ExplicitDefaultRadix.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_ExplicitDefaultRadix_Validating);
			// 
			// splitContainer_Send
			// 
			this.splitContainer_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Send.IsSplitterFixed = true;
			this.splitContainer_Send.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Send.Name = "splitContainer_Send";
			// 
			// splitContainer_Send.Panel1
			// 
			this.splitContainer_Send.Panel1.Controls.Add(this.splitContainer_ExplicitDefaultRadix);
			// 
			// splitContainer_Send.Panel2
			// 
			this.splitContainer_Send.Panel2.Controls.Add(this.panel_Send);
			this.splitContainer_Send.Size = new System.Drawing.Size(540, 27);
			this.splitContainer_Send.SplitterDistance = 356;
			this.splitContainer_Send.SplitterWidth = 1;
			this.splitContainer_Send.TabIndex = 0;
			// 
			// splitContainer_ExplicitDefaultRadix
			// 
			this.splitContainer_ExplicitDefaultRadix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_ExplicitDefaultRadix.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer_ExplicitDefaultRadix.IsSplitterFixed = true;
			this.splitContainer_ExplicitDefaultRadix.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_ExplicitDefaultRadix.Name = "splitContainer_ExplicitDefaultRadix";
			// 
			// splitContainer_ExplicitDefaultRadix.Panel1
			// 
			this.splitContainer_ExplicitDefaultRadix.Panel1.Controls.Add(this.panel_ExplicitDefaultRadix);
			// 
			// splitContainer_ExplicitDefaultRadix.Panel2
			// 
			this.splitContainer_ExplicitDefaultRadix.Panel2.Controls.Add(this.panel_Command);
			this.splitContainer_ExplicitDefaultRadix.Size = new System.Drawing.Size(356, 27);
			this.splitContainer_ExplicitDefaultRadix.SplitterDistance = 80;
			this.splitContainer_ExplicitDefaultRadix.SplitterWidth = 1;
			this.splitContainer_ExplicitDefaultRadix.TabIndex = 0;
			// 
			// panel_ExplicitDefaultRadix
			// 
			this.panel_ExplicitDefaultRadix.Controls.Add(this.comboBox_ExplicitDefaultRadix);
			this.panel_ExplicitDefaultRadix.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_ExplicitDefaultRadix.Location = new System.Drawing.Point(0, 0);
			this.panel_ExplicitDefaultRadix.Name = "panel_ExplicitDefaultRadix";
			this.panel_ExplicitDefaultRadix.Size = new System.Drawing.Size(80, 27);
			this.panel_ExplicitDefaultRadix.TabIndex = 0;
			// 
			// panel_Command
			// 
			this.panel_Command.Controls.Add(this.button_SetMultiLineText);
			this.panel_Command.Controls.Add(this.comboBox_SingleLineText);
			this.panel_Command.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Command.Location = new System.Drawing.Point(0, 0);
			this.panel_Command.Name = "panel_Command";
			this.panel_Command.Size = new System.Drawing.Size(275, 27);
			this.panel_Command.TabIndex = 0;
			// 
			// panel_Send
			// 
			this.panel_Send.Controls.Add(this.button_Send);
			this.panel_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Send.Location = new System.Drawing.Point(0, 0);
			this.panel_Send.Name = "panel_Send";
			this.panel_Send.Size = new System.Drawing.Size(183, 27);
			this.panel_Send.TabIndex = 0;
			// 
			// SendText
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.splitContainer_Send);
			this.Name = "SendText";
			this.Size = new System.Drawing.Size(540, 27);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SendText_Paint);
			this.Enter += new System.EventHandler(this.SendText_Enter);
			this.Leave += new System.EventHandler(this.SendText_Leave);
			this.splitContainer_Send.Panel1.ResumeLayout(false);
			this.splitContainer_Send.Panel2.ResumeLayout(false);
			this.splitContainer_Send.ResumeLayout(false);
			this.splitContainer_ExplicitDefaultRadix.Panel1.ResumeLayout(false);
			this.splitContainer_ExplicitDefaultRadix.Panel2.ResumeLayout(false);
			this.splitContainer_ExplicitDefaultRadix.ResumeLayout(false);
			this.panel_ExplicitDefaultRadix.ResumeLayout(false);
			this.panel_Command.ResumeLayout(false);
			this.panel_Send.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.SplitContainer splitContainer_Send;
		private System.Windows.Forms.Button button_SetMultiLineText;
		private System.Windows.Forms.Button button_Send;
		private MKY.Windows.Forms.ComboBoxEx comboBox_SingleLineText;
		private System.Windows.Forms.SplitContainer splitContainer_ExplicitDefaultRadix;
		private MKY.Windows.Forms.ComboBoxEx comboBox_ExplicitDefaultRadix;
		private System.Windows.Forms.Panel panel_Command;
		private System.Windows.Forms.Panel panel_Send;
		private System.Windows.Forms.Panel panel_ExplicitDefaultRadix;
	}
}
