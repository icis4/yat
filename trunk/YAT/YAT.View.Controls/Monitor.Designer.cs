namespace YAT.View.Controls
{
	partial class Monitor
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
			this.panel_Monitor = new System.Windows.Forms.Panel();
			this.fastListBox_Monitor = new MKY.Windows.Forms.FastListBox();
			this.panel_Picture = new System.Windows.Forms.Panel();
			this.label_DataStatus = new System.Windows.Forms.Label();
			this.label_TimeStatus = new System.Windows.Forms.Label();
			this.label_DataStatusEmpty = new System.Windows.Forms.Label();
			this.label_TimeStatusEmpty = new System.Windows.Forms.Label();
			this.pictureBox_Monitor = new System.Windows.Forms.PictureBox();
			this.fastListBox_LineNumbers = new MKY.Windows.Forms.FastListBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.timer_Opacity = new System.Windows.Forms.Timer(this.components);
			this.timer_MonitorUpdateTimeout = new System.Windows.Forms.Timer(this.components);
			this.timer_DataStatusUpdateTimeout = new System.Windows.Forms.Timer(this.components);
			this.performanceCounter_TotalProcessorLoad = new System.Diagnostics.PerformanceCounter();
			this.timer_TotalProcessorLoad = new System.Windows.Forms.Timer(this.components);
			this.panel_Monitor.SuspendLayout();
			this.panel_Picture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.performanceCounter_TotalProcessorLoad)).BeginInit();
			this.SuspendLayout();
			// 
			// panel_Monitor
			// 
			this.panel_Monitor.Controls.Add(this.fastListBox_Monitor);
			this.panel_Monitor.Controls.Add(this.panel_Picture);
			this.panel_Monitor.Controls.Add(this.fastListBox_LineNumbers);
			this.panel_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor.Name = "panel_Monitor";
			this.panel_Monitor.Size = new System.Drawing.Size(300, 200);
			this.panel_Monitor.TabIndex = 0;
			this.panel_Monitor.Tag = "";
			// 
			// fastListBox_Monitor
			// 
			this.fastListBox_Monitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.fastListBox_Monitor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fastListBox_Monitor.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fastListBox_Monitor.HorizontalScrollbar = true;
			this.fastListBox_Monitor.IntegralHeight = false;
			this.fastListBox_Monitor.Location = new System.Drawing.Point(22, 34);
			this.fastListBox_Monitor.Name = "fastListBox_Monitor";
			this.fastListBox_Monitor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox_Monitor.Size = new System.Drawing.Size(278, 166);
			this.fastListBox_Monitor.TabIndex = 1;
			this.fastListBox_Monitor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_Monitor_DrawItem);
			this.fastListBox_Monitor.Leave += new System.EventHandler(this.fastListBox_Monitor_Leave);
			// 
			// panel_Picture
			// 
			this.panel_Picture.Controls.Add(this.label_DataStatus);
			this.panel_Picture.Controls.Add(this.label_TimeStatus);
			this.panel_Picture.Controls.Add(this.label_DataStatusEmpty);
			this.panel_Picture.Controls.Add(this.label_TimeStatusEmpty);
			this.panel_Picture.Controls.Add(this.pictureBox_Monitor);
			this.panel_Picture.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_Picture.Location = new System.Drawing.Point(0, 0);
			this.panel_Picture.Name = "panel_Picture";
			this.panel_Picture.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.panel_Picture.Size = new System.Drawing.Size(300, 34);
			this.panel_Picture.TabIndex = 0;
			// 
			// label_DataStatus
			// 
			this.label_DataStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_DataStatus.AutoEllipsis = true;
			this.label_DataStatus.Location = new System.Drawing.Point(166, 0);
			this.label_DataStatus.Name = "label_DataStatus";
			this.label_DataStatus.Size = new System.Drawing.Size(134, 30);
			this.label_DataStatus.TabIndex = 2;
			this.label_DataStatus.Text = "888 | 888 @ 8/s | 8/s\r\n888 | 888 @ 8/s | 8/s";
			this.label_DataStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.label_DataStatus, "Number of bytes | lines @ bytes | lines per second");
			// 
			// label_TimeStatus
			// 
			this.label_TimeStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_TimeStatus.AutoEllipsis = true;
			this.label_TimeStatus.Location = new System.Drawing.Point(0, 0);
			this.label_TimeStatus.Name = "label_TimeStatus";
			this.label_TimeStatus.Size = new System.Drawing.Size(134, 30);
			this.label_TimeStatus.TabIndex = 0;
			this.label_TimeStatus.Text = "m:ss\r\nm:ss";
			this.label_TimeStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolTip.SetToolTip(this.label_TimeStatus, "Connect time (m:ss)\r\nTotal connect time (m:ss)");
			// 
			// label_DataStatusEmpty
			// 
			this.label_DataStatusEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_DataStatusEmpty.Location = new System.Drawing.Point(166, 0);
			this.label_DataStatusEmpty.Name = "label_DataStatusEmpty";
			this.label_DataStatusEmpty.Size = new System.Drawing.Size(134, 30);
			this.label_DataStatusEmpty.TabIndex = 3;
			// 
			// label_TimeStatusEmpty
			// 
			this.label_TimeStatusEmpty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_TimeStatusEmpty.Location = new System.Drawing.Point(0, 0);
			this.label_TimeStatusEmpty.Name = "label_TimeStatusEmpty";
			this.label_TimeStatusEmpty.Size = new System.Drawing.Size(134, 30);
			this.label_TimeStatusEmpty.TabIndex = 1;
			// 
			// pictureBox_Monitor
			// 
			this.pictureBox_Monitor.BackgroundImage = global::YAT.View.Controls.Properties.Resources.Image_Monitor_Bidir_28x28_Grey;
			this.pictureBox_Monitor.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.pictureBox_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox_Monitor.Image = global::YAT.View.Controls.Properties.Resources.Image_Monitor_Bidir_28x28_BluePurple;
			this.pictureBox_Monitor.Location = new System.Drawing.Point(0, 0);
			this.pictureBox_Monitor.Name = "pictureBox_Monitor";
			this.pictureBox_Monitor.Size = new System.Drawing.Size(300, 30);
			this.pictureBox_Monitor.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox_Monitor.TabIndex = 1;
			this.pictureBox_Monitor.TabStop = false;
			// 
			// fastListBox_LineNumbers
			// 
			this.fastListBox_LineNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.fastListBox_LineNumbers.BackColor = System.Drawing.SystemColors.Control;
			this.fastListBox_LineNumbers.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.fastListBox_LineNumbers.Font = new System.Drawing.Font("DejaVu Sans Mono", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fastListBox_LineNumbers.ForeColor = System.Drawing.SystemColors.ControlText;
			this.fastListBox_LineNumbers.IntegralHeight = false;
			this.fastListBox_LineNumbers.Location = new System.Drawing.Point(0, 34);
			this.fastListBox_LineNumbers.Name = "fastListBox_LineNumbers";
			this.fastListBox_LineNumbers.ScrollAlwaysVisible = true;
			this.fastListBox_LineNumbers.Size = new System.Drawing.Size(40, 166);
			this.fastListBox_LineNumbers.TabIndex = 0;
			this.fastListBox_LineNumbers.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_LineNumbers_DrawItem);
			// 
			// timer_Opacity
			// 
			this.timer_Opacity.Tick += new System.EventHandler(this.timer_Opacity_Tick);
			// 
			// timer_MonitorUpdateTimeout
			// 
			this.timer_MonitorUpdateTimeout.Tick += new System.EventHandler(this.timer_MonitorUpdateTimeout_Tick);
			// 
			// timer_DataStatusUpdateTimeout
			// 
			this.timer_DataStatusUpdateTimeout.Tick += new System.EventHandler(this.timer_DataStatusUpdateTimeout_Tick);
			// 
			// performanceCounter_TotalProcessorLoad
			// 
			this.performanceCounter_TotalProcessorLoad.CategoryName = "Processor";
			this.performanceCounter_TotalProcessorLoad.CounterName = "% Processor Time";
			this.performanceCounter_TotalProcessorLoad.InstanceName = "_Total";
			// 
			// timer_TotalProcessorLoad
			// 
			this.timer_TotalProcessorLoad.Enabled = true;
			this.timer_TotalProcessorLoad.Interval = 347;
			this.timer_TotalProcessorLoad.Tick += new System.EventHandler(this.timer_TotalProcessorLoad_Tick);
			// 
			// Monitor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.panel_Monitor);
			this.Name = "Monitor";
			this.Size = new System.Drawing.Size(300, 200);
			this.Resize += new System.EventHandler(this.Monitor_Resize);
			this.panel_Monitor.ResumeLayout(false);
			this.panel_Picture.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.performanceCounter_TotalProcessorLoad)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel_Monitor;
		private MKY.Windows.Forms.FastListBox fastListBox_Monitor;
		private System.Windows.Forms.Panel panel_Picture;
		private System.Windows.Forms.PictureBox pictureBox_Monitor;
		private System.Windows.Forms.Label label_DataStatus;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Timer timer_Opacity;
		private System.Windows.Forms.Label label_TimeStatus;
		private System.Windows.Forms.Timer timer_MonitorUpdateTimeout;
		private System.Windows.Forms.Timer timer_DataStatusUpdateTimeout;
		private MKY.Windows.Forms.FastListBox fastListBox_LineNumbers;
		private System.Windows.Forms.Label label_DataStatusEmpty;
		private System.Windows.Forms.Label label_TimeStatusEmpty;
		private System.Diagnostics.PerformanceCounter performanceCounter_TotalProcessorLoad;
		private System.Windows.Forms.Timer timer_TotalProcessorLoad;
	}
}
