﻿namespace YAT.View.Controls
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
			this.button_MultiLine = new System.Windows.Forms.Button();
			this.button_Send = new System.Windows.Forms.Button();
			this.comboBox_SingleLineText = new System.Windows.Forms.ComboBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_MultiLine
			// 
			this.button_MultiLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_MultiLine.Location = new System.Drawing.Point(328, 3);
			this.button_MultiLine.Name = "button_MultiLine";
			this.button_MultiLine.Size = new System.Drawing.Size(25, 21);
			this.button_MultiLine.TabIndex = 3;
			this.button_MultiLine.Text = "...";
			this.toolTip.SetToolTip(this.button_MultiLine, "Multi-line text");
			this.button_MultiLine.UseVisualStyleBackColor = true;
			this.button_MultiLine.Click += new System.EventHandler(this.button_MultiLine_Click);
			// 
			// button_Send
			// 
			this.button_Send.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Send.Enabled = false;
			this.button_Send.Location = new System.Drawing.Point(3, 3);
			this.button_Send.Name = "button_Send";
			this.button_Send.Size = new System.Drawing.Size(177, 21);
			this.button_Send.TabIndex = 4;
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
			this.comboBox_SingleLineText.Size = new System.Drawing.Size(319, 21);
			this.comboBox_SingleLineText.TabIndex = 1;
			this.toolTip.SetToolTip(this.comboBox_SingleLineText, "Fill-in text, <Enter> to send,\r\ndrop down for recent text,\r\n<...> to enter multi-" +
        "line text");
			this.comboBox_SingleLineText.SelectedIndexChanged += new System.EventHandler(this.comboBox_SingleLineText_SelectedIndexChanged);
			this.comboBox_SingleLineText.TextChanged += new System.EventHandler(this.comboBox_SingleLineText_TextChanged);
			this.comboBox_SingleLineText.Enter += new System.EventHandler(this.comboBox_SingleLineText_Enter);
			this.comboBox_SingleLineText.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboBox_SingleLineText_KeyPress);
			this.comboBox_SingleLineText.Leave += new System.EventHandler(this.comboBox_SingleLineText_Leave);
			this.comboBox_SingleLineText.Validating += new System.ComponentModel.CancelEventHandler(this.comboBox_SingleLineText_Validating);
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.IsSplitterFixed = true;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.button_MultiLine);
			this.splitContainer.Panel1.Controls.Add(this.comboBox_SingleLineText);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.button_Send);
			this.splitContainer.Size = new System.Drawing.Size(540, 27);
			this.splitContainer.SplitterDistance = 356;
			this.splitContainer.SplitterWidth = 1;
			this.splitContainer.TabIndex = 3;
			// 
			// SendText
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer);
			this.Name = "SendText";
			this.Size = new System.Drawing.Size(540, 27);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SendText_Paint);
			this.Enter += new System.EventHandler(this.SendText_Enter);
			this.Leave += new System.EventHandler(this.SendText_Leave);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.SplitContainer splitContainer;
		private System.Windows.Forms.Button button_MultiLine;
		private System.Windows.Forms.Button button_Send;
		private System.Windows.Forms.ComboBox comboBox_SingleLineText;
	}
}
