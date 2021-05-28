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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "Well, any better idea on how to implement the monitor update timeout?")]
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Monitor));
			this.panel_Monitor = new System.Windows.Forms.Panel();
			this.textBox_CopyOfActiveLine = new System.Windows.Forms.TextBox();
			this.fastListBox_Monitor = new MKY.Windows.Forms.FastListBox();
			this.fastListBox_LineNumbers = new MKY.Windows.Forms.FastListBox();
			this.panel_Picture = new System.Windows.Forms.Panel();
			this.label_TimeStatus_Total = new System.Windows.Forms.Label();
			this.label_DataStatus_Unidir = new System.Windows.Forms.Label();
			this.label_DataStatus_BidirTx = new System.Windows.Forms.Label();
			this.label_DataStatus_BidirRx = new System.Windows.Forms.Label();
			this.label_TimeStatus_Active = new System.Windows.Forms.Label();
			this.label_DataStatus_Back = new System.Windows.Forms.Label();
			this.label_TimeStatus_Back = new System.Windows.Forms.Label();
			this.pictureBox_Monitor = new System.Windows.Forms.PictureBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.timer_Opacity = new System.Windows.Forms.Timer(this.components);
			this.timer_MonitorUpdateTimeout = new System.Windows.Forms.Timer(this.components);
			this.timer_DataStatusUpdateTimeout = new System.Windows.Forms.Timer(this.components);
			this.timer_ProcessorLoad = new System.Windows.Forms.Timer(this.components);
			this.panel_Monitor.SuspendLayout();
			this.panel_Picture.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).BeginInit();
			this.SuspendLayout();
			// 
			// panel_Monitor
			// 
			this.panel_Monitor.Controls.Add(this.textBox_CopyOfActiveLine);
			this.panel_Monitor.Controls.Add(this.fastListBox_Monitor);
			this.panel_Monitor.Controls.Add(this.fastListBox_LineNumbers);
			this.panel_Monitor.Controls.Add(this.panel_Picture);
			this.panel_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor.Name = "panel_Monitor";
			this.panel_Monitor.Size = new System.Drawing.Size(300, 300);
			this.panel_Monitor.TabIndex = 0;
			this.panel_Monitor.Tag = "";
			// 
			// textBox_CopyOfActiveLine
			// 
			this.textBox_CopyOfActiveLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox_CopyOfActiveLine.BackColor = System.Drawing.SystemColors.Control;
			this.textBox_CopyOfActiveLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBox_CopyOfActiveLine.Location = new System.Drawing.Point(22, 277);
			this.textBox_CopyOfActiveLine.Name = "textBox_CopyOfActiveLine";
			this.textBox_CopyOfActiveLine.ReadOnly = true;
			this.textBox_CopyOfActiveLine.Size = new System.Drawing.Size(278, 20);
			this.textBox_CopyOfActiveLine.TabIndex = 0;
			this.toolTip.SetToolTip(this.textBox_CopyOfActiveLine, resources.GetString("textBox_CopyOfActiveLine.ToolTip"));
			this.textBox_CopyOfActiveLine.WordWrap = false;
			this.textBox_CopyOfActiveLine.Enter += new System.EventHandler(this.textBox_CopyOfActiveLine_Enter);
			this.textBox_CopyOfActiveLine.Leave += new System.EventHandler(this.textBox_CopyOfActiveLine_Leave);
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
			this.fastListBox_Monitor.Location = new System.Drawing.Point(22, 33);
			this.fastListBox_Monitor.Name = "fastListBox_Monitor";
			this.fastListBox_Monitor.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.fastListBox_Monitor.Size = new System.Drawing.Size(278, 242);
			this.fastListBox_Monitor.TabIndex = 1;
			this.fastListBox_Monitor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_Monitor_DrawItem);
			this.fastListBox_Monitor.SelectedIndexChanged += new System.EventHandler(this.fastListBox_Monitor_SelectedIndexChanged);
			this.fastListBox_Monitor.Leave += new System.EventHandler(this.fastListBox_Monitor_Leave);
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
			this.fastListBox_LineNumbers.Location = new System.Drawing.Point(0, 33);
			this.fastListBox_LineNumbers.Name = "fastListBox_LineNumbers";
			this.fastListBox_LineNumbers.ScrollAlwaysVisible = true;
			this.fastListBox_LineNumbers.Size = new System.Drawing.Size(40, 242);
			this.fastListBox_LineNumbers.TabIndex = 0;
			this.fastListBox_LineNumbers.TabStop = false;
			this.fastListBox_LineNumbers.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.fastListBox_LineNumbers_DrawItem);
			// 
			// panel_Picture
			// 
			this.panel_Picture.Controls.Add(this.label_TimeStatus_Total);
			this.panel_Picture.Controls.Add(this.label_DataStatus_Unidir);
			this.panel_Picture.Controls.Add(this.label_DataStatus_BidirTx);
			this.panel_Picture.Controls.Add(this.label_DataStatus_BidirRx);
			this.panel_Picture.Controls.Add(this.label_TimeStatus_Active);
			this.panel_Picture.Controls.Add(this.label_DataStatus_Back);
			this.panel_Picture.Controls.Add(this.label_TimeStatus_Back);
			this.panel_Picture.Controls.Add(this.pictureBox_Monitor);
			this.panel_Picture.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_Picture.Location = new System.Drawing.Point(0, 0);
			this.panel_Picture.Name = "panel_Picture";
			this.panel_Picture.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
			this.panel_Picture.Size = new System.Drawing.Size(300, 34);
			this.panel_Picture.TabIndex = 0;
			// 
			// label_TimeStatus_Total
			// 
			this.label_TimeStatus_Total.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_TimeStatus_Total.AutoEllipsis = true;
			this.label_TimeStatus_Total.Location = new System.Drawing.Point(0, 15);
			this.label_TimeStatus_Total.Name = "label_TimeStatus_Total";
			this.label_TimeStatus_Total.Size = new System.Drawing.Size(134, 15);
			this.label_TimeStatus_Total.TabIndex = 1;
			this.label_TimeStatus_Total.Text = "m:ss";
			this.label_TimeStatus_Total.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.toolTip.SetToolTip(this.label_TimeStatus_Total, "Total connect time (m:ss)\r\n\r\nDouble-click to reset");
			this.label_TimeStatus_Total.DoubleClick += new System.EventHandler(this.label_TimeStatus_Total_DoubleClick);
			// 
			// label_DataStatus_Unidir
			// 
			this.label_DataStatus_Unidir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_DataStatus_Unidir.AutoEllipsis = true;
			this.label_DataStatus_Unidir.Location = new System.Drawing.Point(166, 8);
			this.label_DataStatus_Unidir.Name = "label_DataStatus_Unidir";
			this.label_DataStatus_Unidir.Size = new System.Drawing.Size(134, 15);
			this.label_DataStatus_Unidir.TabIndex = 5;
			this.label_DataStatus_Unidir.Text = "888 | 888 @ 8/s | 8/s";
			this.label_DataStatus_Unidir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolTip.SetToolTip(this.label_DataStatus_Unidir, "Number of bytes | lines @ bytes | lines per second\r\n\r\nDouble-click to reset");
			this.label_DataStatus_Unidir.DoubleClick += new System.EventHandler(this.label_DataStatus_Unidir_DoubleClick);
			// 
			// label_DataStatus_BidirTx
			// 
			this.label_DataStatus_BidirTx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_DataStatus_BidirTx.AutoEllipsis = true;
			this.label_DataStatus_BidirTx.Location = new System.Drawing.Point(166, 0);
			this.label_DataStatus_BidirTx.Name = "label_DataStatus_BidirTx";
			this.label_DataStatus_BidirTx.Size = new System.Drawing.Size(134, 15);
			this.label_DataStatus_BidirTx.TabIndex = 4;
			this.label_DataStatus_BidirTx.Text = "888 | 888 @ 8/s | 8/s";
			this.label_DataStatus_BidirTx.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.toolTip.SetToolTip(this.label_DataStatus_BidirTx, "Number of bytes | lines @ bytes | lines per second\r\n\r\nDouble-click to reset");
			this.label_DataStatus_BidirTx.DoubleClick += new System.EventHandler(this.label_DataStatus_BidirTx_DoubleClick);
			// 
			// label_DataStatus_BidirRx
			// 
			this.label_DataStatus_BidirRx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_DataStatus_BidirRx.AutoEllipsis = true;
			this.label_DataStatus_BidirRx.Location = new System.Drawing.Point(166, 15);
			this.label_DataStatus_BidirRx.Name = "label_DataStatus_BidirRx";
			this.label_DataStatus_BidirRx.Size = new System.Drawing.Size(134, 15);
			this.label_DataStatus_BidirRx.TabIndex = 6;
			this.label_DataStatus_BidirRx.Text = "888 | 888 @ 8/s | 8/s";
			this.toolTip.SetToolTip(this.label_DataStatus_BidirRx, "Number of bytes | lines @ bytes | lines per second\r\n\r\nDouble-click to reset");
			this.label_DataStatus_BidirRx.DoubleClick += new System.EventHandler(this.label_DataStatus_BidirRx_DoubleClick);
			// 
			// label_TimeStatus_Active
			// 
			this.label_TimeStatus_Active.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_TimeStatus_Active.AutoEllipsis = true;
			this.label_TimeStatus_Active.Location = new System.Drawing.Point(0, 0);
			this.label_TimeStatus_Active.Name = "label_TimeStatus_Active";
			this.label_TimeStatus_Active.Size = new System.Drawing.Size(134, 15);
			this.label_TimeStatus_Active.TabIndex = 0;
			this.label_TimeStatus_Active.Text = "m:ss";
			this.label_TimeStatus_Active.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolTip.SetToolTip(this.label_TimeStatus_Active, "Active connect time (m:ss)\r\n\r\nDouble-click to reset");
			this.label_TimeStatus_Active.DoubleClick += new System.EventHandler(this.label_TimeStatus_Active_DoubleClick);
			// 
			// label_DataStatus_Back
			// 
			this.label_DataStatus_Back.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_DataStatus_Back.Location = new System.Drawing.Point(166, 0);
			this.label_DataStatus_Back.Name = "label_DataStatus_Back";
			this.label_DataStatus_Back.Size = new System.Drawing.Size(134, 30);
			this.label_DataStatus_Back.TabIndex = 7;
			// 
			// label_TimeStatus_Back
			// 
			this.label_TimeStatus_Back.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label_TimeStatus_Back.Location = new System.Drawing.Point(0, 0);
			this.label_TimeStatus_Back.Name = "label_TimeStatus_Back";
			this.label_TimeStatus_Back.Size = new System.Drawing.Size(134, 30);
			this.label_TimeStatus_Back.TabIndex = 2;
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
			// timer_ProcessorLoad
			// 
			this.timer_ProcessorLoad.Enabled = true;
			this.timer_ProcessorLoad.Interval = 151;
			this.timer_ProcessorLoad.Tick += new System.EventHandler(this.timer_ProcessorLoad_Tick);
			// 
			// Monitor
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.panel_Monitor);
			this.Name = "Monitor";
			this.Size = new System.Drawing.Size(300, 300);
			this.Resize += new System.EventHandler(this.Monitor_Resize);
			this.panel_Monitor.ResumeLayout(false);
			this.panel_Monitor.PerformLayout();
			this.panel_Picture.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox_Monitor)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel_Monitor;
		private MKY.Windows.Forms.FastListBox fastListBox_Monitor;
		private System.Windows.Forms.Panel panel_Picture;
		private System.Windows.Forms.PictureBox pictureBox_Monitor;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.Timer timer_Opacity;
		private System.Windows.Forms.Label label_TimeStatus_Active;
		private System.Windows.Forms.Timer timer_MonitorUpdateTimeout;
		private System.Windows.Forms.Timer timer_DataStatusUpdateTimeout;
		private MKY.Windows.Forms.FastListBox fastListBox_LineNumbers;
		private System.Windows.Forms.Label label_DataStatus_Back;
		private System.Windows.Forms.Label label_TimeStatus_Back;
		private System.Windows.Forms.Timer timer_ProcessorLoad;
		private System.Windows.Forms.TextBox textBox_CopyOfActiveLine;
		private System.Windows.Forms.Label label_DataStatus_BidirRx;
		private System.Windows.Forms.Label label_DataStatus_BidirTx;
		private System.Windows.Forms.Label label_DataStatus_Unidir;
		private System.Windows.Forms.Label label_TimeStatus_Total;
	}
}
