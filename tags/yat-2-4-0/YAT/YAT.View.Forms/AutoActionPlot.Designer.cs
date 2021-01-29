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
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.button_Deactivate = new System.Windows.Forms.Button();
			this.button_DeactivateAndClose = new System.Windows.Forms.Button();
			this.plotView = new OxyPlot.WindowsForms.PlotView();
			this.contextMenuStrip_Plot = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_Plot_CopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Plot_SaveToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator_Plot_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_Plot_ResetAxes = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_Plot_ShowLegend = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator_Plot_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_Plot_Help = new System.Windows.Forms.ToolStripMenuItem();
			this.button_ResetAxes = new System.Windows.Forms.Button();
			this.label_Help = new System.Windows.Forms.Label();
			this.button_SuspendResume = new System.Windows.Forms.Button();
			this.contextMenuStrip_Plot.SuspendLayout();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.Location = new System.Drawing.Point(767, 400);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 8;
			this.button_Close.Text = "&Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// button_Clear
			// 
			this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Clear.Location = new System.Drawing.Point(491, 400);
			this.button_Clear.Name = "button_Clear";
			this.button_Clear.Size = new System.Drawing.Size(75, 23);
			this.button_Clear.TabIndex = 4;
			this.button_Clear.Text = "C&lear";
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
			this.checkBox_ShowLegend.Text = "Show L&egend";
			this.checkBox_ShowLegend.UseVisualStyleBackColor = true;
			this.checkBox_ShowLegend.CheckedChanged += new System.EventHandler(this.checkBox_ShowLegend_CheckedChanged);
			// 
			// button_Deactivate
			// 
			this.button_Deactivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Deactivate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_delete_16x16;
			this.button_Deactivate.Location = new System.Drawing.Point(653, 400);
			this.button_Deactivate.Name = "button_Deactivate";
			this.button_Deactivate.Size = new System.Drawing.Size(27, 23);
			this.button_Deactivate.TabIndex = 6;
			this.toolTip.SetToolTip(this.button_Deactivate, "Deactivate");
			this.button_Deactivate.UseVisualStyleBackColor = true;
			this.button_Deactivate.Click += new System.EventHandler(this.button_Deactivate_Click);
			// 
			// button_DeactivateAndClose
			// 
			this.button_DeactivateAndClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_DeactivateAndClose.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_delete_16x16;
			this.button_DeactivateAndClose.Location = new System.Drawing.Point(686, 400);
			this.button_DeactivateAndClose.Name = "button_DeactivateAndClose";
			this.button_DeactivateAndClose.Size = new System.Drawing.Size(75, 23);
			this.button_DeactivateAndClose.TabIndex = 7;
			this.button_DeactivateAndClose.Text = " + Cl&ose";
			this.button_DeactivateAndClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this.toolTip.SetToolTip(this.button_DeactivateAndClose, "Deactivate and Close");
			this.button_DeactivateAndClose.UseVisualStyleBackColor = true;
			this.button_DeactivateAndClose.Click += new System.EventHandler(this.button_DeactivateAndClose_Click);
			// 
			// plotView
			// 
			this.plotView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.plotView.ContextMenuStrip = this.contextMenuStrip_Plot;
			this.plotView.Location = new System.Drawing.Point(12, 12);
			this.plotView.Name = "plotView";
			this.plotView.PanCursor = System.Windows.Forms.Cursors.Hand;
			this.plotView.Size = new System.Drawing.Size(860, 381);
			this.plotView.TabIndex = 0;
			this.plotView.ZoomHorizontalCursor = System.Windows.Forms.Cursors.SizeWE;
			this.plotView.ZoomRectangleCursor = System.Windows.Forms.Cursors.SizeNWSE;
			this.plotView.ZoomVerticalCursor = System.Windows.Forms.Cursors.SizeNS;
			this.plotView.Paint += new System.Windows.Forms.PaintEventHandler(this.plotView_Paint);
			// 
			// contextMenuStrip_Plot
			// 
			this.contextMenuStrip_Plot.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Plot_CopyToClipboard,
            this.toolStripMenuItem_Plot_SaveToFile,
            this.toolStripSeparator_Plot_1,
            this.toolStripMenuItem_Plot_ResetAxes,
            this.toolStripMenuItem_Plot_ShowLegend,
            this.toolStripSeparator_Plot_2,
            this.toolStripMenuItem_Plot_Help});
			this.contextMenuStrip_Plot.Name = "contextMenuStrip_Plot";
			this.contextMenuStrip_Plot.Size = new System.Drawing.Size(172, 126);
			this.contextMenuStrip_Plot.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Plot_Opening);
			// 
			// toolStripMenuItem_Plot_CopyToClipboard
			// 
			this.toolStripMenuItem_Plot_CopyToClipboard.Name = "toolStripMenuItem_Plot_CopyToClipboard";
			this.toolStripMenuItem_Plot_CopyToClipboard.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_Plot_CopyToClipboard.Text = "Copy to Clipboard";
			this.toolStripMenuItem_Plot_CopyToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_Plot_CopyToClipboard_Click);
			// 
			// toolStripMenuItem_Plot_SaveToFile
			// 
			this.toolStripMenuItem_Plot_SaveToFile.Name = "toolStripMenuItem_Plot_SaveToFile";
			this.toolStripMenuItem_Plot_SaveToFile.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_Plot_SaveToFile.Text = "Save to File";
			this.toolStripMenuItem_Plot_SaveToFile.Click += new System.EventHandler(this.toolStripMenuItem_Plot_SaveToFile_Click);
			// 
			// toolStripSeparator_Plot_1
			// 
			this.toolStripSeparator_Plot_1.Name = "toolStripSeparator_Plot_1";
			this.toolStripSeparator_Plot_1.Size = new System.Drawing.Size(168, 6);
			// 
			// toolStripMenuItem_Plot_ResetAxes
			// 
			this.toolStripMenuItem_Plot_ResetAxes.Name = "toolStripMenuItem_Plot_ResetAxes";
			this.toolStripMenuItem_Plot_ResetAxes.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_Plot_ResetAxes.Text = "Reset Axes";
			this.toolStripMenuItem_Plot_ResetAxes.Click += new System.EventHandler(this.toolStripMenuItem_Plot_ResetAxes_Click);
			// 
			// toolStripMenuItem_Plot_ShowLegend
			// 
			this.toolStripMenuItem_Plot_ShowLegend.Name = "toolStripMenuItem_Plot_ShowLegend";
			this.toolStripMenuItem_Plot_ShowLegend.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_Plot_ShowLegend.Text = "Show Legend";
			this.toolStripMenuItem_Plot_ShowLegend.Click += new System.EventHandler(this.toolStripMenuItem_Plot_ShowLegend_Click);
			// 
			// toolStripSeparator_Plot_2
			// 
			this.toolStripSeparator_Plot_2.Name = "toolStripSeparator_Plot_2";
			this.toolStripSeparator_Plot_2.Size = new System.Drawing.Size(168, 6);
			// 
			// toolStripMenuItem_Plot_Help
			// 
			this.toolStripMenuItem_Plot_Help.Name = "toolStripMenuItem_Plot_Help";
			this.toolStripMenuItem_Plot_Help.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_Plot_Help.Text = "Interaction Help...";
			this.toolStripMenuItem_Plot_Help.Click += new System.EventHandler(this.toolStripMenuItem_Plot_Help_Click);
			// 
			// button_ResetAxes
			// 
			this.button_ResetAxes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_ResetAxes.Location = new System.Drawing.Point(42, 400);
			this.button_ResetAxes.Name = "button_ResetAxes";
			this.button_ResetAxes.Size = new System.Drawing.Size(75, 23);
			this.button_ResetAxes.TabIndex = 1;
			this.button_ResetAxes.Text = "&Reset Axes";
			this.button_ResetAxes.UseVisualStyleBackColor = true;
			this.button_ResetAxes.Click += new System.EventHandler(this.button_ResetAxes_Click);
			// 
			// label_Help
			// 
			this.label_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label_Help.AutoSize = true;
			this.label_Help.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_Help.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_Help.Location = new System.Drawing.Point(231, 405);
			this.label_Help.Name = "label_Help";
			this.label_Help.Size = new System.Drawing.Size(241, 13);
			this.label_Help.TabIndex = 3;
			this.label_Help.Text = "Right-Click on plot for options and interaction help";
			// 
			// button_SuspendResume
			// 
			this.button_SuspendResume.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_SuspendResume.Location = new System.Drawing.Point(572, 400);
			this.button_SuspendResume.Name = "button_SuspendResume";
			this.button_SuspendResume.Size = new System.Drawing.Size(75, 23);
			this.button_SuspendResume.TabIndex = 5;
			this.button_SuspendResume.Text = "S&uspend";
			this.button_SuspendResume.UseVisualStyleBackColor = true;
			this.button_SuspendResume.Click += new System.EventHandler(this.button_SuspendResume_Click);
			// 
			// AutoActionPlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 441);
			this.Controls.Add(this.button_SuspendResume);
			this.Controls.Add(this.button_DeactivateAndClose);
			this.Controls.Add(this.label_Help);
			this.Controls.Add(this.plotView);
			this.Controls.Add(this.button_Deactivate);
			this.Controls.Add(this.button_ResetAxes);
			this.Controls.Add(this.checkBox_ShowLegend);
			this.Controls.Add(this.button_Clear);
			this.Controls.Add(this.button_Close);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(480, 240);
			this.Name = "AutoActionPlot";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "YAT - [[Terminal] - Automatic Action Plot]";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AutoActionPlot_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AutoActionPlot_FormClosed);
			this.Shown += new System.EventHandler(this.AutoActionPlot_Shown);
			this.LocationChanged += new System.EventHandler(this.AutoActionPlot_LocationChanged);
			this.PlotAreaBackColorChanged += new System.EventHandler(this.AutoActionPlot_PlotAreaBackColorChanged);
			this.SizeChanged += new System.EventHandler(this.AutoActionPlot_SizeChanged);
			this.contextMenuStrip_Plot.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button_Close;
		private System.Windows.Forms.Button button_Clear;
		private System.Windows.Forms.CheckBox checkBox_ShowLegend;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Button button_ResetAxes;
		private System.Windows.Forms.Button button_Deactivate;
		private OxyPlot.WindowsForms.PlotView plotView;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Plot;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Plot_CopyToClipboard;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Plot_SaveToFile;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator_Plot_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Plot_ResetAxes;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Plot_ShowLegend;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator_Plot_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Plot_Help;
		private System.Windows.Forms.Label label_Help;
		private System.Windows.Forms.Button button_DeactivateAndClose;
		private System.Windows.Forms.Button button_SuspendResume;
	}
}