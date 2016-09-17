﻿namespace YAT.View.Controls
{
	partial class BinaryDisplaySettingsSet
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
			this.label_LengthLineBreakUnit = new System.Windows.Forms.Label();
			this.textBox_SequenceLineBreakAfterSequence = new System.Windows.Forms.TextBox();
			this.textBox_LengthLineBreak = new System.Windows.Forms.TextBox();
			this.checkBox_SequenceLineBreakAfter = new System.Windows.Forms.CheckBox();
			this.checkBox_LengthLineBreak = new System.Windows.Forms.CheckBox();
			this.label_TimedLineBreakUnit = new System.Windows.Forms.Label();
			this.textBox_TimedLineBreakTimeout = new System.Windows.Forms.TextBox();
			this.checkBox_TimedLineBreak = new System.Windows.Forms.CheckBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.textBox_SequenceLineBreakBeforeSequence = new System.Windows.Forms.TextBox();
			this.checkBox_SequenceLineBreakBefore = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label_LengthLineBreakUnit
			// 
			this.label_LengthLineBreakUnit.AutoSize = true;
			this.label_LengthLineBreakUnit.Location = new System.Drawing.Point(212, 4);
			this.label_LengthLineBreakUnit.Name = "label_LengthLineBreakUnit";
			this.label_LengthLineBreakUnit.Size = new System.Drawing.Size(32, 13);
			this.label_LengthLineBreakUnit.TabIndex = 2;
			this.label_LengthLineBreakUnit.Text = "bytes";
			this.label_LengthLineBreakUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_SequenceLineBreakAfterSequence
			// 
			this.textBox_SequenceLineBreakAfterSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_SequenceLineBreakAfterSequence.Enabled = false;
			this.textBox_SequenceLineBreakAfterSequence.Location = new System.Drawing.Point(163, 47);
			this.textBox_SequenceLineBreakAfterSequence.Name = "textBox_SequenceLineBreakAfterSequence";
			this.textBox_SequenceLineBreakAfterSequence.Size = new System.Drawing.Size(106, 20);
			this.textBox_SequenceLineBreakAfterSequence.TabIndex = 4;
			this.textBox_SequenceLineBreakAfterSequence.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.toolTip.SetToolTip(this.textBox_SequenceLineBreakAfterSequence, "Can be any sequence of bytes, e.g. \\h(17 00) or <ETB><NUL>");
			this.textBox_SequenceLineBreakAfterSequence.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_SequenceLineBreakAfterSequence_Validating);
			// 
			// textBox_LengthLineBreak
			// 
			this.textBox_LengthLineBreak.Enabled = false;
			this.textBox_LengthLineBreak.Location = new System.Drawing.Point(163, 1);
			this.textBox_LengthLineBreak.Name = "textBox_LengthLineBreak";
			this.textBox_LengthLineBreak.Size = new System.Drawing.Size(48, 20);
			this.textBox_LengthLineBreak.TabIndex = 1;
			this.textBox_LengthLineBreak.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_LengthLineBreak.TextChanged += new System.EventHandler(this.textBox_LengthLineBreak_TextChanged);
			this.textBox_LengthLineBreak.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_LengthLineBreak_Validating);
			// 
			// checkBox_SequenceLineBreakAfter
			// 
			this.checkBox_SequenceLineBreakAfter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_SequenceLineBreakAfter.AutoSize = true;
			this.checkBox_SequenceLineBreakAfter.Location = new System.Drawing.Point(3, 49);
			this.checkBox_SequenceLineBreakAfter.Name = "checkBox_SequenceLineBreakAfter";
			this.checkBox_SequenceLineBreakAfter.Size = new System.Drawing.Size(152, 17);
			this.checkBox_SequenceLineBreakAfter.TabIndex = 3;
			this.checkBox_SequenceLineBreakAfter.Text = "Break lines &after sequence";
			this.checkBox_SequenceLineBreakAfter.CheckedChanged += new System.EventHandler(this.checkBox_SequenceLineBreakAfter_CheckedChanged);
			// 
			// checkBox_LengthLineBreak
			// 
			this.checkBox_LengthLineBreak.AutoSize = true;
			this.checkBox_LengthLineBreak.Location = new System.Drawing.Point(3, 3);
			this.checkBox_LengthLineBreak.Name = "checkBox_LengthLineBreak";
			this.checkBox_LengthLineBreak.Size = new System.Drawing.Size(102, 17);
			this.checkBox_LengthLineBreak.TabIndex = 0;
			this.checkBox_LengthLineBreak.Text = "Break &lines after";
			this.checkBox_LengthLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_LengthLineBreak_CheckedChanged);
			// 
			// label_TimedLineBreakUnit
			// 
			this.label_TimedLineBreakUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_TimedLineBreakUnit.AutoSize = true;
			this.label_TimedLineBreakUnit.Location = new System.Drawing.Point(212, 73);
			this.label_TimedLineBreakUnit.Name = "label_TimedLineBreakUnit";
			this.label_TimedLineBreakUnit.Size = new System.Drawing.Size(20, 13);
			this.label_TimedLineBreakUnit.TabIndex = 7;
			this.label_TimedLineBreakUnit.Text = "ms";
			this.label_TimedLineBreakUnit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox_TimedLineBreakTimeout
			// 
			this.textBox_TimedLineBreakTimeout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_TimedLineBreakTimeout.Enabled = false;
			this.textBox_TimedLineBreakTimeout.Location = new System.Drawing.Point(163, 70);
			this.textBox_TimedLineBreakTimeout.Name = "textBox_TimedLineBreakTimeout";
			this.textBox_TimedLineBreakTimeout.Size = new System.Drawing.Size(48, 20);
			this.textBox_TimedLineBreakTimeout.TabIndex = 6;
			this.textBox_TimedLineBreakTimeout.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textBox_TimedLineBreakTimeout.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_TimedLineBreakTimeout_Validating);
			// 
			// checkBox_TimedLineBreak
			// 
			this.checkBox_TimedLineBreak.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_TimedLineBreak.AutoSize = true;
			this.checkBox_TimedLineBreak.Location = new System.Drawing.Point(3, 72);
			this.checkBox_TimedLineBreak.Name = "checkBox_TimedLineBreak";
			this.checkBox_TimedLineBreak.Size = new System.Drawing.Size(151, 17);
			this.checkBox_TimedLineBreak.TabIndex = 5;
			this.checkBox_TimedLineBreak.Text = "Break lines after ti&meout of";
			this.checkBox_TimedLineBreak.CheckedChanged += new System.EventHandler(this.checkBox_TimedLineBreak_CheckedChanged);
			// 
			// textBox_SequenceLineBreakBeforeSequence
			// 
			this.textBox_SequenceLineBreakBeforeSequence.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textBox_SequenceLineBreakBeforeSequence.Enabled = false;
			this.textBox_SequenceLineBreakBeforeSequence.Location = new System.Drawing.Point(163, 24);
			this.textBox_SequenceLineBreakBeforeSequence.Name = "textBox_SequenceLineBreakBeforeSequence";
			this.textBox_SequenceLineBreakBeforeSequence.Size = new System.Drawing.Size(106, 20);
			this.textBox_SequenceLineBreakBeforeSequence.TabIndex = 9;
			this.toolTip.SetToolTip(this.textBox_SequenceLineBreakBeforeSequence, "Can be any sequence of bytes, e.g. \\h(17 00) or <ETB><NUL>");
			this.textBox_SequenceLineBreakBeforeSequence.Validating += new System.ComponentModel.CancelEventHandler(this.textBox_SequenceLineBreakBeforeSequence_Validating);
			// 
			// checkBox_SequenceLineBreakBefore
			// 
			this.checkBox_SequenceLineBreakBefore.AutoSize = true;
			this.checkBox_SequenceLineBreakBefore.Location = new System.Drawing.Point(3, 26);
			this.checkBox_SequenceLineBreakBefore.Name = "checkBox_SequenceLineBreakBefore";
			this.checkBox_SequenceLineBreakBefore.Size = new System.Drawing.Size(161, 17);
			this.checkBox_SequenceLineBreakBefore.TabIndex = 8;
			this.checkBox_SequenceLineBreakBefore.Text = "Break lines &before sequence";
			this.checkBox_SequenceLineBreakBefore.UseVisualStyleBackColor = true;
			this.checkBox_SequenceLineBreakBefore.CheckedChanged += new System.EventHandler(this.checkBox_SequenceLineBreakBefore_CheckedChanged);
			// 
			// BinaryDisplaySettingsSet
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.textBox_SequenceLineBreakAfterSequence);
			this.Controls.Add(this.textBox_SequenceLineBreakBeforeSequence);
			this.Controls.Add(this.checkBox_SequenceLineBreakBefore);
			this.Controls.Add(this.label_LengthLineBreakUnit);
			this.Controls.Add(this.textBox_LengthLineBreak);
			this.Controls.Add(this.checkBox_SequenceLineBreakAfter);
			this.Controls.Add(this.checkBox_LengthLineBreak);
			this.Controls.Add(this.label_TimedLineBreakUnit);
			this.Controls.Add(this.textBox_TimedLineBreakTimeout);
			this.Controls.Add(this.checkBox_TimedLineBreak);
			this.Name = "BinaryDisplaySettingsSet";
			this.Size = new System.Drawing.Size(272, 91);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label_LengthLineBreakUnit;
		private System.Windows.Forms.TextBox textBox_SequenceLineBreakAfterSequence;
		private System.Windows.Forms.TextBox textBox_LengthLineBreak;
		private System.Windows.Forms.CheckBox checkBox_SequenceLineBreakAfter;
		private System.Windows.Forms.CheckBox checkBox_LengthLineBreak;
		private System.Windows.Forms.Label label_TimedLineBreakUnit;
		private System.Windows.Forms.TextBox textBox_TimedLineBreakTimeout;
		private System.Windows.Forms.CheckBox checkBox_TimedLineBreak;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.CheckBox checkBox_SequenceLineBreakBefore;
		private System.Windows.Forms.TextBox textBox_SequenceLineBreakBeforeSequence;
	}
}
