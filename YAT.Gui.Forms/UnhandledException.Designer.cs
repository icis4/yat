using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.Gui.Forms
{
	partial class UnhandledException
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button_Close = new System.Windows.Forms.Button();
			this.groupBox_Exception = new System.Windows.Forms.GroupBox();
			this.textBox_Source = new System.Windows.Forms.TextBox();
			this.textBox_Message = new System.Windows.Forms.TextBox();
			this.label_Message = new System.Windows.Forms.Label();
			this.label_Type = new System.Windows.Forms.Label();
			this.label_Source = new System.Windows.Forms.Label();
			this.textBox_Type = new System.Windows.Forms.TextBox();
			this.label_Stack = new System.Windows.Forms.Label();
			this.textBox_Stack = new System.Windows.Forms.TextBox();
			this.button_CopyToClipboard = new System.Windows.Forms.Button();
			this.linkLabel_Explanation = new System.Windows.Forms.LinkLabel();
			this.groupBox_Exception.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(507, 417);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 3;
			this.button_Close.Text = "Close";
			// 
			// groupBox_Exception
			// 
			this.groupBox_Exception.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Exception.Controls.Add(this.textBox_Source);
			this.groupBox_Exception.Controls.Add(this.textBox_Message);
			this.groupBox_Exception.Controls.Add(this.label_Message);
			this.groupBox_Exception.Controls.Add(this.label_Type);
			this.groupBox_Exception.Controls.Add(this.label_Source);
			this.groupBox_Exception.Controls.Add(this.textBox_Type);
			this.groupBox_Exception.Controls.Add(this.label_Stack);
			this.groupBox_Exception.Controls.Add(this.textBox_Stack);
			this.groupBox_Exception.Location = new System.Drawing.Point(12, 56);
			this.groupBox_Exception.Name = "groupBox_Exception";
			this.groupBox_Exception.Size = new System.Drawing.Size(570, 346);
			this.groupBox_Exception.TabIndex = 1;
			this.groupBox_Exception.TabStop = false;
			this.groupBox_Exception.Text = "Exception";
			// 
			// textBox_Source
			// 
			this.textBox_Source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Source.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_Source.Location = new System.Drawing.Point(68, 144);
			this.textBox_Source.Name = "textBox_Source";
			this.textBox_Source.ReadOnly = true;
			this.textBox_Source.Size = new System.Drawing.Size(496, 13);
			this.textBox_Source.TabIndex = 5;
			this.textBox_Source.Text = "<SOURCE>";
			// 
			// textBox_Message
			// 
			this.textBox_Message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Message.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_Message.Location = new System.Drawing.Point(68, 48);
			this.textBox_Message.Multiline = true;
			this.textBox_Message.Name = "textBox_Message";
			this.textBox_Message.ReadOnly = true;
			this.textBox_Message.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox_Message.Size = new System.Drawing.Size(496, 88);
			this.textBox_Message.TabIndex = 3;
			this.textBox_Message.Text = "<MESSAGE>";
			this.textBox_Message.WordWrap = false;
			// 
			// label_Message
			// 
			this.label_Message.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Message.AutoSize = true;
			this.label_Message.Location = new System.Drawing.Point(12, 48);
			this.label_Message.Name = "label_Message";
			this.label_Message.Size = new System.Drawing.Size(53, 13);
			this.label_Message.TabIndex = 2;
			this.label_Message.Text = "Message:";
			// 
			// label_Type
			// 
			this.label_Type.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Type.AutoSize = true;
			this.label_Type.Location = new System.Drawing.Point(12, 24);
			this.label_Type.Name = "label_Type";
			this.label_Type.Size = new System.Drawing.Size(34, 13);
			this.label_Type.TabIndex = 0;
			this.label_Type.Text = "Type:";
			// 
			// label_Source
			// 
			this.label_Source.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Source.AutoSize = true;
			this.label_Source.Location = new System.Drawing.Point(12, 144);
			this.label_Source.Name = "label_Source";
			this.label_Source.Size = new System.Drawing.Size(44, 13);
			this.label_Source.TabIndex = 4;
			this.label_Source.Text = "Source:";
			// 
			// textBox_Type
			// 
			this.textBox_Type.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Type.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_Type.Location = new System.Drawing.Point(68, 24);
			this.textBox_Type.Name = "textBox_Type";
			this.textBox_Type.ReadOnly = true;
			this.textBox_Type.Size = new System.Drawing.Size(496, 13);
			this.textBox_Type.TabIndex = 1;
			this.textBox_Type.Text = "<TYPE>";
			// 
			// label_Stack
			// 
			this.label_Stack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Stack.AutoSize = true;
			this.label_Stack.Location = new System.Drawing.Point(12, 168);
			this.label_Stack.Name = "label_Stack";
			this.label_Stack.Size = new System.Drawing.Size(38, 13);
			this.label_Stack.TabIndex = 6;
			this.label_Stack.Text = "Stack:";
			// 
			// textBox_Stack
			// 
			this.textBox_Stack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Stack.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_Stack.Location = new System.Drawing.Point(68, 168);
			this.textBox_Stack.Multiline = true;
			this.textBox_Stack.Name = "textBox_Stack";
			this.textBox_Stack.ReadOnly = true;
			this.textBox_Stack.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox_Stack.Size = new System.Drawing.Size(496, 172);
			this.textBox_Stack.TabIndex = 7;
			this.textBox_Stack.Text = "<STACK>";
			this.textBox_Stack.WordWrap = false;
			// 
			// button_CopyToClipboard
			// 
			this.button_CopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_CopyToClipboard.Image = global::YAT.Gui.Forms.Properties.Resources.Image_CopyToClipboard_16x16;
			this.button_CopyToClipboard.Location = new System.Drawing.Point(12, 417);
			this.button_CopyToClipboard.Name = "button_CopyToClipboard";
			this.button_CopyToClipboard.Size = new System.Drawing.Size(176, 23);
			this.button_CopyToClipboard.TabIndex = 2;
			this.button_CopyToClipboard.Text = "&Copy Exception to Clipboard";
			this.button_CopyToClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.button_CopyToClipboard.Click += new System.EventHandler(this.button_CopyToClipboard_Click);
			// 
			// linkLabel_Explanation
			// 
			this.linkLabel_Explanation.LinkArea = new System.Windows.Forms.LinkArea(0, 0);
			this.linkLabel_Explanation.Location = new System.Drawing.Point(24, 16);
			this.linkLabel_Explanation.Name = "linkLabel_Explanation";
			this.linkLabel_Explanation.Size = new System.Drawing.Size(552, 30);
			this.linkLabel_Explanation.TabIndex = 0;
			this.linkLabel_Explanation.Text = "An unhandled exception occured in YAT. Please report this exception to YAT > Trac" +
				"ker > Bugs on SourceForge.net to give us valuable feedback to continuously impro" +
				"ve YAT.";
			this.linkLabel_Explanation.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// UnhandledException
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(594, 452);
			this.Controls.Add(this.linkLabel_Explanation);
			this.Controls.Add(this.button_CopyToClipboard);
			this.Controls.Add(this.groupBox_Exception);
			this.Controls.Add(this.button_Close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UnhandledException";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Unhandled Exception";
			this.Load += new System.EventHandler(this.UnhandledException_Load);
			this.groupBox_Exception.ResumeLayout(false);
			this.groupBox_Exception.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBox_Source;
		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.GroupBox groupBox_Exception;
		private System.Windows.Forms.Label label_Message;
		private System.Windows.Forms.Label label_Type;
		private System.Windows.Forms.Label label_Source;
		private System.Windows.Forms.TextBox textBox_Type;
		private System.Windows.Forms.Label label_Stack;
		private System.Windows.Forms.TextBox textBox_Stack;
		private System.Windows.Forms.TextBox textBox_Message;
		private System.Windows.Forms.Button button_CopyToClipboard;
		private System.Windows.Forms.LinkLabel linkLabel_Explanation;
	}
}
