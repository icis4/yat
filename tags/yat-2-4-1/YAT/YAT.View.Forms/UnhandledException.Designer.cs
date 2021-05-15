using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
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
			this.textBox_Exception = new System.Windows.Forms.TextBox();
			this.button_CopyToClipboard = new System.Windows.Forms.Button();
			this.button_Instructions = new System.Windows.Forms.Button();
			this.groupBox_Message = new System.Windows.Forms.GroupBox();
			this.label_Explanation = new System.Windows.Forms.Label();
			this.groupBox_Exception.SuspendLayout();
			this.groupBox_Message.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_Close.Location = new System.Drawing.Point(507, 445);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 4;
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Text = "Close";
			// 
			// groupBox_Exception
			// 
			this.groupBox_Exception.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Exception.Controls.Add(this.textBox_Exception);
			this.groupBox_Exception.Location = new System.Drawing.Point(12, 84);
			this.groupBox_Exception.Name = "groupBox_Exception";
			this.groupBox_Exception.Size = new System.Drawing.Size(570, 346);
			this.groupBox_Exception.TabIndex = 1;
			this.groupBox_Exception.TabStop = false;
			this.groupBox_Exception.Text = "Exception";
			// 
			// textBox_Exception
			// 
			this.textBox_Exception.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_Exception.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox_Exception.Location = new System.Drawing.Point(9, 19);
			this.textBox_Exception.Multiline = true;
			this.textBox_Exception.Name = "textBox_Exception";
			this.textBox_Exception.ReadOnly = true;
			this.textBox_Exception.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox_Exception.Size = new System.Drawing.Size(555, 321);
			this.textBox_Exception.TabIndex = 3;
			this.textBox_Exception.Text = "<EXCEPTION>";
			this.textBox_Exception.WordWrap = false;
			// 
			// button_CopyToClipboard
			// 
			this.button_CopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_CopyToClipboard.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_text_exports_16x16;
			this.button_CopyToClipboard.Location = new System.Drawing.Point(12, 445);
			this.button_CopyToClipboard.Name = "button_CopyToClipboard";
			this.button_CopyToClipboard.Size = new System.Drawing.Size(176, 23);
			this.button_CopyToClipboard.TabIndex = 2;
			this.button_CopyToClipboard.Text = "&Copy Exception to Clipboard";
			this.button_CopyToClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.button_CopyToClipboard.UseVisualStyleBackColor = true;
			this.button_CopyToClipboard.Click += new System.EventHandler(this.button_CopyToClipboard_Click);
			// 
			// button_Instructions
			// 
			this.button_Instructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_Instructions.Location = new System.Drawing.Point(194, 445);
			this.button_Instructions.Name = "button_Instructions";
			this.button_Instructions.Size = new System.Drawing.Size(176, 23);
			this.button_Instructions.TabIndex = 3;
			this.button_Instructions.Text = "&Instructions on Bug Submission";
			this.button_Instructions.UseVisualStyleBackColor = true;
			this.button_Instructions.Click += new System.EventHandler(this.button_Instructions_Click);
			// 
			// groupBox_Message
			// 
			this.groupBox_Message.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Message.Controls.Add(this.label_Explanation);
			this.groupBox_Message.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Message.Name = "groupBox_Message";
			this.groupBox_Message.Size = new System.Drawing.Size(570, 68);
			this.groupBox_Message.TabIndex = 0;
			this.groupBox_Message.TabStop = false;
			// 
			// label_Explanation
			// 
			this.label_Explanation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
			this.label_Explanation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Explanation.Location = new System.Drawing.Point(6, 16);
			this.label_Explanation.Name = "label_Explanation";
			this.label_Explanation.Size = new System.Drawing.Size(558, 49);
			this.label_Explanation.TabIndex = 1;
			this.label_Explanation.Text = "An unhandled exception occurred while running YAT.";
			// 
			// UnhandledException
			// 
			this.AcceptButton = this.button_Close;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Close;
			this.ClientSize = new System.Drawing.Size(594, 480);
			this.Controls.Add(this.groupBox_Message);
			this.Controls.Add(this.button_Instructions);
			this.Controls.Add(this.button_CopyToClipboard);
			this.Controls.Add(this.groupBox_Exception);
			this.Controls.Add(this.button_Close);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "UnhandledException";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "<Caption>";
			this.Load += new System.EventHandler(this.UnhandledException_Load);
			this.groupBox_Exception.ResumeLayout(false);
			this.groupBox_Exception.PerformLayout();
			this.groupBox_Message.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.GroupBox groupBox_Exception;
		private System.Windows.Forms.TextBox textBox_Exception;
		private System.Windows.Forms.Button button_CopyToClipboard;
		private System.Windows.Forms.Button button_Instructions;
		private System.Windows.Forms.GroupBox groupBox_Message;
		private System.Windows.Forms.Label label_Explanation;
	}
}
