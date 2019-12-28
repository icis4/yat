﻿namespace YAT.View.Forms
{
	partial class AutoActionPlot
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoActionPlot));
			this.button_Close = new System.Windows.Forms.Button();
			this.scottPlot = new ScottPlot.FormsPlot();
			this.button_Reset = new System.Windows.Forms.Button();
			this.checkBox_ShowLegend = new System.Windows.Forms.CheckBox();
			this.checkBox_ShowHover = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.Location = new System.Drawing.Point(872, 407);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 0;
			this.button_Close.Text = "&Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// scottPlot
			// 
			this.scottPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scottPlot.Location = new System.Drawing.Point(12, 12);
			this.scottPlot.Name = "scottPlot";
			this.scottPlot.Size = new System.Drawing.Size(935, 382);
			this.scottPlot.TabIndex = 1;
			this.scottPlot.MouseMoved += new System.EventHandler(this.scottPlot_MouseMoved);
			// 
			// button_Reset
			// 
			this.button_Reset.Location = new System.Drawing.Point(791, 407);
			this.button_Reset.Name = "button_Reset";
			this.button_Reset.Size = new System.Drawing.Size(75, 23);
			this.button_Reset.TabIndex = 2;
			this.button_Reset.Text = "&Reset";
			this.button_Reset.UseVisualStyleBackColor = true;
			this.button_Reset.Click += new System.EventHandler(this.button_Reset_Click);
			// 
			// checkBox_ShowLegend
			// 
			this.checkBox_ShowLegend.AutoSize = true;
			this.checkBox_ShowLegend.Location = new System.Drawing.Point(12, 411);
			this.checkBox_ShowLegend.Name = "checkBox_ShowLegend";
			this.checkBox_ShowLegend.Size = new System.Drawing.Size(92, 17);
			this.checkBox_ShowLegend.TabIndex = 3;
			this.checkBox_ShowLegend.Text = "Show &Legend";
			this.checkBox_ShowLegend.UseVisualStyleBackColor = true;
			this.checkBox_ShowLegend.CheckedChanged += new System.EventHandler(this.checkBox_ShowLegend_CheckedChanged);
			// 
			// checkBox_ShowHover
			// 
			this.checkBox_ShowHover.AutoSize = true;
			this.checkBox_ShowHover.Location = new System.Drawing.Point(110, 411);
			this.checkBox_ShowHover.Name = "checkBox_ShowHover";
			this.checkBox_ShowHover.Size = new System.Drawing.Size(130, 17);
			this.checkBox_ShowHover.TabIndex = 4;
			this.checkBox_ShowHover.Text = "Show Value on &Hover";
			this.checkBox_ShowHover.UseVisualStyleBackColor = true;
			this.checkBox_ShowHover.CheckedChanged += new System.EventHandler(this.checkBox_ShowHover_CheckedChanged);
			// 
			// AutoActionPlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(959, 442);
			this.Controls.Add(this.checkBox_ShowHover);
			this.Controls.Add(this.checkBox_ShowLegend);
			this.Controls.Add(this.button_Reset);
			this.Controls.Add(this.scottPlot);
			this.Controls.Add(this.button_Close);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AutoActionPlot";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT - [[Terminal] - Plot]";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private ScottPlot.FormsPlot scottPlot;
		private System.Windows.Forms.Button button_Reset;
		private System.Windows.Forms.CheckBox checkBox_ShowLegend;
		private System.Windows.Forms.CheckBox checkBox_ShowHover;
	}
}