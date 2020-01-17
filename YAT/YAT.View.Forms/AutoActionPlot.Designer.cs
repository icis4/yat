namespace YAT.View.Forms
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AutoActionPlot));
			this.button_Close = new System.Windows.Forms.Button();
			this.button_Clear = new System.Windows.Forms.Button();
			this.checkBox_ShowLegend = new System.Windows.Forms.CheckBox();
			this.timer_Update = new System.Windows.Forms.Timer(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_Deactivate = new System.Windows.Forms.Button();
			this.label_UpdateSuspended = new System.Windows.Forms.Label();
			this.scottPlot = new ScottPlot.FormsPlot();
			this.button_FitAxis = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.Location = new System.Drawing.Point(775, 400);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 6;
			this.button_Close.Text = "&Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// button_Clear
			// 
			this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Clear.Location = new System.Drawing.Point(661, 400);
			this.button_Clear.Name = "button_Clear";
			this.button_Clear.Size = new System.Drawing.Size(75, 23);
			this.button_Clear.TabIndex = 4;
			this.button_Clear.Text = "Cl&ear";
			this.button_Clear.UseVisualStyleBackColor = true;
			this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
			// 
			// checkBox_ShowLegend
			// 
			this.checkBox_ShowLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_ShowLegend.AutoSize = true;
			this.checkBox_ShowLegend.Location = new System.Drawing.Point(123, 404);
			this.checkBox_ShowLegend.Name = "checkBox_ShowLegend";
			this.checkBox_ShowLegend.Size = new System.Drawing.Size(92, 17);
			this.checkBox_ShowLegend.TabIndex = 2;
			this.checkBox_ShowLegend.Text = "Show &Legend";
			this.checkBox_ShowLegend.UseVisualStyleBackColor = true;
			this.checkBox_ShowLegend.CheckedChanged += new System.EventHandler(this.checkBox_ShowLegend_CheckedChanged);
			// 
			// timer_Update
			// 
			this.timer_Update.Enabled = true;
			this.timer_Update.Interval = 73;
			this.timer_Update.Tick += new System.EventHandler(this.timer_Update_Tick);
			// 
			// button_Deactivate
			// 
			this.button_Deactivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Deactivate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_delete_16x16;
			this.button_Deactivate.Location = new System.Drawing.Point(742, 400);
			this.button_Deactivate.Name = "button_Deactivate";
			this.button_Deactivate.Size = new System.Drawing.Size(27, 23);
			this.button_Deactivate.TabIndex = 5;
			this.toolTip.SetToolTip(this.button_Deactivate, "Deactivate");
			this.button_Deactivate.UseVisualStyleBackColor = true;
			this.button_Deactivate.Click += new System.EventHandler(this.button_Deactivate_Click);
			// 
			// label_UpdateSuspended
			// 
			this.label_UpdateSuspended.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_UpdateSuspended.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_UpdateSuspended.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_UpdateSuspended.Location = new System.Drawing.Point(221, 396);
			this.label_UpdateSuspended.Name = "label_UpdateSuspended";
			this.label_UpdateSuspended.Size = new System.Drawing.Size(434, 31);
			this.label_UpdateSuspended.TabIndex = 3;
			this.label_UpdateSuspended.Text = "Update is suspended while mouse is on plot\r\n(required for mouse interaction, e.g." +
    " zoom in/out)";
			this.label_UpdateSuspended.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label_UpdateSuspended.Visible = false;
			// 
			// scottPlot
			// 
			this.scottPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.scottPlot.Location = new System.Drawing.Point(12, 12);
			this.scottPlot.Name = "scottPlot";
			this.scottPlot.Size = new System.Drawing.Size(860, 381);
			this.scottPlot.TabIndex = 0;
			this.scottPlot.MouseEntered += new System.EventHandler(this.scottPlot_MouseEntered);
			this.scottPlot.MouseLeft += new System.EventHandler(this.scottPlot_MouseLeft);
			this.scottPlot.MouseMoved += new System.EventHandler(this.scottPlot_MouseMoved);
			// 
			// button_FitAxis
			// 
			this.button_FitAxis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_FitAxis.Location = new System.Drawing.Point(42, 400);
			this.button_FitAxis.Name = "button_FitAxis";
			this.button_FitAxis.Size = new System.Drawing.Size(75, 23);
			this.button_FitAxis.TabIndex = 1;
			this.button_FitAxis.Text = "&Fit Axis";
			this.button_FitAxis.UseVisualStyleBackColor = true;
			this.button_FitAxis.Click += new System.EventHandler(this.button_FitAxis_Click);
			// 
			// AutoActionPlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 441);
			this.Controls.Add(this.button_Deactivate);
			this.Controls.Add(this.button_FitAxis);
			this.Controls.Add(this.label_UpdateSuspended);
			this.Controls.Add(this.checkBox_ShowLegend);
			this.Controls.Add(this.button_Clear);
			this.Controls.Add(this.scottPlot);
			this.Controls.Add(this.button_Close);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AutoActionPlot";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT - [[Terminal] - Plot]";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoActionPlot_FormClosing);
			this.Shown += new System.EventHandler(this.AutoActionPlot_Shown);
			this.BackColorChanged += new System.EventHandler(this.AutoActionPlot_BackColorChanged);
			this.LocationChanged += new System.EventHandler(this.AutoActionPlot_LocationChanged);
			this.SizeChanged += new System.EventHandler(this.AutoActionPlot_SizeChanged);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private ScottPlot.FormsPlot scottPlot;
		private System.Windows.Forms.Button button_Clear;
		private System.Windows.Forms.CheckBox checkBox_ShowLegend;
		private System.Windows.Forms.Timer timer_Update;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label_UpdateSuspended;
		private System.Windows.Forms.Button button_FitAxis;
		private System.Windows.Forms.Button button_Deactivate;
	}
}