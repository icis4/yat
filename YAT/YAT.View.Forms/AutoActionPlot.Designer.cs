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
			this.timer_PlotUpdate = new System.Windows.Forms.Timer(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.label_UpdateSuspended = new System.Windows.Forms.Label();
			this.scottPlot = new ScottPlot.FormsPlot();
			this.SuspendLayout();
			// 
			// button_Close
			// 
			this.button_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Close.Location = new System.Drawing.Point(797, 406);
			this.button_Close.Name = "button_Close";
			this.button_Close.Size = new System.Drawing.Size(75, 23);
			this.button_Close.TabIndex = 4;
			this.button_Close.Text = "&Close";
			this.button_Close.UseVisualStyleBackColor = true;
			this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
			// 
			// button_Clear
			// 
			this.button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Clear.Location = new System.Drawing.Point(716, 406);
			this.button_Clear.Name = "button_Clear";
			this.button_Clear.Size = new System.Drawing.Size(75, 23);
			this.button_Clear.TabIndex = 3;
			this.button_Clear.Text = "C&lear";
			this.button_Clear.UseVisualStyleBackColor = true;
			this.button_Clear.Click += new System.EventHandler(this.button_Clear_Click);
			// 
			// checkBox_ShowLegend
			// 
			this.checkBox_ShowLegend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBox_ShowLegend.AutoSize = true;
			this.checkBox_ShowLegend.Location = new System.Drawing.Point(12, 410);
			this.checkBox_ShowLegend.Name = "checkBox_ShowLegend";
			this.checkBox_ShowLegend.Size = new System.Drawing.Size(92, 17);
			this.checkBox_ShowLegend.TabIndex = 1;
			this.checkBox_ShowLegend.Text = "Show &Legend";
			this.checkBox_ShowLegend.UseVisualStyleBackColor = true;
			this.checkBox_ShowLegend.CheckedChanged += new System.EventHandler(this.checkBox_ShowLegend_CheckedChanged);
			// 
			// timer_PlotUpdate
			// 
			this.timer_PlotUpdate.Enabled = true;
			this.timer_PlotUpdate.Interval = 75;
			this.timer_PlotUpdate.Tick += new System.EventHandler(this.timer_PlotUpdate_Tick);
			// 
			// label_UpdateSuspended
			// 
			this.label_UpdateSuspended.AutoSize = true;
			this.label_UpdateSuspended.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label_UpdateSuspended.ForeColor = System.Drawing.SystemColors.GrayText;
			this.label_UpdateSuspended.Location = new System.Drawing.Point(184, 411);
			this.label_UpdateSuspended.Name = "label_UpdateSuspended";
			this.label_UpdateSuspended.Size = new System.Drawing.Size(444, 13);
			this.label_UpdateSuspended.TabIndex = 2;
			this.label_UpdateSuspended.Text = "Update is suspended while mouse is on plot (required for mouse interaction, e.g. " +
    "zoom in/out)";
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
			// AutoActionPlot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 441);
			this.Controls.Add(this.label_UpdateSuspended);
			this.Controls.Add(this.checkBox_ShowLegend);
			this.Controls.Add(this.button_Clear);
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
		private System.Windows.Forms.Button button_Clear;
		private System.Windows.Forms.CheckBox checkBox_ShowLegend;
		private System.Windows.Forms.Timer timer_PlotUpdate;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Label label_UpdateSuspended;
	}
}