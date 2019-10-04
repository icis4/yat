using System;
using System.Collections.Generic;
using System.Text;

namespace YAT.View.Forms
{
	partial class BinaryTerminalSettings
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
			this.button_OK = new System.Windows.Forms.Button();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.button_Defaults = new System.Windows.Forms.Button();
			this.groupBox_Display = new System.Windows.Forms.GroupBox();
			this.groupBox_RxDisplay = new System.Windows.Forms.GroupBox();
			this.binaryTerminalSettingsSet_Rx = new YAT.View.Controls.BinaryDisplaySettingsSet();
			this.groupBox_TxDisplay = new System.Windows.Forms.GroupBox();
			this.binaryTerminalSettingsSet_Tx = new YAT.View.Controls.BinaryDisplaySettingsSet();
			this.checkBox_SeparateTxRxDisplay = new System.Windows.Forms.CheckBox();
			this.label_AdvancedSettingsRemark = new System.Windows.Forms.Label();
			this.groupBox_Display.SuspendLayout();
			this.groupBox_RxDisplay.SuspendLayout();
			this.groupBox_TxDisplay.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_OK
			// 
			this.button_OK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button_OK.Location = new System.Drawing.Point(328, 29);
			this.button_OK.Name = "button_OK";
			this.button_OK.Size = new System.Drawing.Size(75, 23);
			this.button_OK.TabIndex = 1;
			this.button_OK.Text = "OK";
			this.button_OK.Click += new System.EventHandler(this.button_OK_Click);
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(328, 58);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 2;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.Click += new System.EventHandler(this.button_Cancel_Click);
			// 
			// button_Defaults
			// 
			this.button_Defaults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Defaults.Location = new System.Drawing.Point(328, 120);
			this.button_Defaults.Name = "button_Defaults";
			this.button_Defaults.Size = new System.Drawing.Size(75, 23);
			this.button_Defaults.TabIndex = 3;
			this.button_Defaults.Text = "&Defaults...";
			this.button_Defaults.Click += new System.EventHandler(this.button_Defaults_Click);
			// 
			// groupBox_Display
			// 
			this.groupBox_Display.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_Display.Controls.Add(this.groupBox_RxDisplay);
			this.groupBox_Display.Controls.Add(this.groupBox_TxDisplay);
			this.groupBox_Display.Controls.Add(this.checkBox_SeparateTxRxDisplay);
			this.groupBox_Display.Location = new System.Drawing.Point(12, 12);
			this.groupBox_Display.Name = "groupBox_Display";
			this.groupBox_Display.Size = new System.Drawing.Size(302, 301);
			this.groupBox_Display.TabIndex = 0;
			this.groupBox_Display.TabStop = false;
			this.groupBox_Display.Text = "Display Settings";
			// 
			// groupBox_RxDisplay
			// 
			this.groupBox_RxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_RxDisplay.Controls.Add(this.binaryTerminalSettingsSet_Rx);
			this.groupBox_RxDisplay.Location = new System.Drawing.Point(6, 176);
			this.groupBox_RxDisplay.Name = "groupBox_RxDisplay";
			this.groupBox_RxDisplay.Size = new System.Drawing.Size(290, 119);
			this.groupBox_RxDisplay.TabIndex = 2;
			this.groupBox_RxDisplay.TabStop = false;
			this.groupBox_RxDisplay.Text = "&Rx";
			// 
			// binaryTerminalSettingsSet_Rx
			// 
			this.binaryTerminalSettingsSet_Rx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.binaryTerminalSettingsSet_Rx.Location = new System.Drawing.Point(9, 19);
			this.binaryTerminalSettingsSet_Rx.Name = "binaryTerminalSettingsSet_Rx";
			this.binaryTerminalSettingsSet_Rx.Size = new System.Drawing.Size(272, 91);
			this.binaryTerminalSettingsSet_Rx.TabIndex = 0;
			this.binaryTerminalSettingsSet_Rx.SettingsChanged += new System.EventHandler(this.binaryTerminalSettingsSet_Rx_SettingsChanged);
			// 
			// groupBox_TxDisplay
			// 
			this.groupBox_TxDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox_TxDisplay.Controls.Add(this.binaryTerminalSettingsSet_Tx);
			this.groupBox_TxDisplay.Location = new System.Drawing.Point(6, 19);
			this.groupBox_TxDisplay.Name = "groupBox_TxDisplay";
			this.groupBox_TxDisplay.Size = new System.Drawing.Size(290, 119);
			this.groupBox_TxDisplay.TabIndex = 0;
			this.groupBox_TxDisplay.TabStop = false;
			this.groupBox_TxDisplay.Text = "&Tx and Rx";
			// 
			// binaryTerminalSettingsSet_Tx
			// 
			this.binaryTerminalSettingsSet_Tx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.binaryTerminalSettingsSet_Tx.Location = new System.Drawing.Point(9, 19);
			this.binaryTerminalSettingsSet_Tx.Name = "binaryTerminalSettingsSet_Tx";
			this.binaryTerminalSettingsSet_Tx.Size = new System.Drawing.Size(272, 91);
			this.binaryTerminalSettingsSet_Tx.TabIndex = 0;
			this.binaryTerminalSettingsSet_Tx.SettingsChanged += new System.EventHandler(this.binaryTerminalSettingsSet_Tx_SettingsChanged);
			// 
			// checkBox_SeparateTxRxDisplay
			// 
			this.checkBox_SeparateTxRxDisplay.AutoSize = true;
			this.checkBox_SeparateTxRxDisplay.Location = new System.Drawing.Point(18, 152);
			this.checkBox_SeparateTxRxDisplay.Name = "checkBox_SeparateTxRxDisplay";
			this.checkBox_SeparateTxRxDisplay.Size = new System.Drawing.Size(175, 17);
			this.checkBox_SeparateTxRxDisplay.TabIndex = 1;
			this.checkBox_SeparateTxRxDisplay.Text = "&Separate settings for Tx and Rx";
			this.checkBox_SeparateTxRxDisplay.UseVisualStyleBackColor = true;
			this.checkBox_SeparateTxRxDisplay.CheckedChanged += new System.EventHandler(this.checkBox_SeparateTxRxDisplay_CheckedChanged);
			// 
			// label_AdvancedSettingsRemark
			// 
			this.label_AdvancedSettingsRemark.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_AdvancedSettingsRemark.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_AdvancedSettingsRemark.Location = new System.Drawing.Point(314, 188);
			this.label_AdvancedSettingsRemark.Name = "label_AdvancedSettingsRemark";
			this.label_AdvancedSettingsRemark.Size = new System.Drawing.Size(103, 125);
			this.label_AdvancedSettingsRemark.TabIndex = 4;
			this.label_AdvancedSettingsRemark.Text = "Also see\r\n[Advanced Settings...]";
			this.label_AdvancedSettingsRemark.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// BinaryTerminalSettings
			// 
			this.AcceptButton = this.button_OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(415, 325);
			this.Controls.Add(this.groupBox_Display);
			this.Controls.Add(this.button_Defaults);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.button_OK);
			this.Controls.Add(this.label_AdvancedSettingsRemark);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BinaryTerminalSettings";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Binary Terminal Settings";
			this.Shown += new System.EventHandler(this.BinaryTerminalSettings_Shown);
			this.groupBox_Display.ResumeLayout(false);
			this.groupBox_Display.PerformLayout();
			this.groupBox_RxDisplay.ResumeLayout(false);
			this.groupBox_TxDisplay.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button button_OK;
		private System.Windows.Forms.Button button_Cancel;
		private System.Windows.Forms.Button button_Defaults;
		private System.Windows.Forms.GroupBox groupBox_Display;
		private System.Windows.Forms.CheckBox checkBox_SeparateTxRxDisplay;
		private System.Windows.Forms.GroupBox groupBox_RxDisplay;
		private YAT.View.Controls.BinaryDisplaySettingsSet binaryTerminalSettingsSet_Rx;
		private System.Windows.Forms.GroupBox groupBox_TxDisplay;
		private YAT.View.Controls.BinaryDisplaySettingsSet binaryTerminalSettingsSet_Tx;
		private System.Windows.Forms.Label label_AdvancedSettingsRemark;
	}
}
