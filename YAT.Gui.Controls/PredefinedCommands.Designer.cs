namespace YAT.Gui.Controls
{
	partial class PredefinedCommands
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
			this.splitContainer_Commands = new System.Windows.Forms.SplitContainer();
			this.label_Shortcut = new System.Windows.Forms.Label();
			this.splitContainer_Lower = new System.Windows.Forms.SplitContainer();
			this.panel_Commands = new System.Windows.Forms.Panel();
			this.pageButtons = new YAT.Gui.Controls.PredefinedCommandPageButtons();
			this.label_Page = new System.Windows.Forms.Label();
			this.button_PageNext = new System.Windows.Forms.Button();
			this.button_PagePrevious = new System.Windows.Forms.Button();
			this.comboBox_Pages = new System.Windows.Forms.ComboBox();
			this.splitContainer_Commands.Panel1.SuspendLayout();
			this.splitContainer_Commands.Panel2.SuspendLayout();
			this.splitContainer_Commands.SuspendLayout();
			this.splitContainer_Lower.Panel1.SuspendLayout();
			this.splitContainer_Lower.Panel2.SuspendLayout();
			this.splitContainer_Lower.SuspendLayout();
			this.panel_Commands.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer_Commands
			// 
			this.splitContainer_Commands.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Commands.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer_Commands.IsSplitterFixed = true;
			this.splitContainer_Commands.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Commands.Name = "splitContainer_Commands";
			this.splitContainer_Commands.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Commands.Panel1
			// 
			this.splitContainer_Commands.Panel1.Controls.Add(this.label_Shortcut);
			this.splitContainer_Commands.Panel1MinSize = 19;
			// 
			// splitContainer_Commands.Panel2
			// 
			this.splitContainer_Commands.Panel2.Controls.Add(this.splitContainer_Lower);
			this.splitContainer_Commands.Size = new System.Drawing.Size(144, 335);
			this.splitContainer_Commands.SplitterDistance = 19;
			this.splitContainer_Commands.TabIndex = 0;
			this.splitContainer_Commands.TabStop = false;
			// 
			// label_Shortcut
			// 
			this.label_Shortcut.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label_Shortcut.Location = new System.Drawing.Point(0, 0);
			this.label_Shortcut.Name = "label_Shortcut";
			this.label_Shortcut.Size = new System.Drawing.Size(144, 19);
			this.label_Shortcut.TabIndex = 0;
			this.label_Shortcut.Text = "Shift + F1..12";
			this.label_Shortcut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// splitContainer_Lower
			// 
			this.splitContainer_Lower.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Lower.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer_Lower.IsSplitterFixed = true;
			this.splitContainer_Lower.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Lower.Name = "splitContainer_Lower";
			this.splitContainer_Lower.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Lower.Panel1
			// 
			this.splitContainer_Lower.Panel1.Controls.Add(this.panel_Commands);
			// 
			// splitContainer_Lower.Panel2
			// 
			this.splitContainer_Lower.Panel2.Controls.Add(this.label_Page);
			this.splitContainer_Lower.Panel2.Controls.Add(this.button_PageNext);
			this.splitContainer_Lower.Panel2.Controls.Add(this.button_PagePrevious);
			this.splitContainer_Lower.Panel2.Controls.Add(this.comboBox_Pages);
			this.splitContainer_Lower.Panel2MinSize = 48;
			this.splitContainer_Lower.Size = new System.Drawing.Size(144, 312);
			this.splitContainer_Lower.SplitterDistance = 263;
			this.splitContainer_Lower.SplitterWidth = 1;
			this.splitContainer_Lower.TabIndex = 0;
			this.splitContainer_Lower.TabStop = false;
			// 
			// panel_Commands
			// 
			this.panel_Commands.Controls.Add(this.pageButtons);
			this.panel_Commands.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Commands.Location = new System.Drawing.Point(0, 0);
			this.panel_Commands.Name = "panel_Commands";
			this.panel_Commands.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
			this.panel_Commands.Size = new System.Drawing.Size(144, 263);
			this.panel_Commands.TabIndex = 0;
			// 
			// pageButtons
			// 
			this.pageButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pageButtons.Location = new System.Drawing.Point(3, 0);
			this.pageButtons.Name = "pageButtons";
			this.pageButtons.Size = new System.Drawing.Size(138, 263);
			this.pageButtons.TabIndex = 2;
			this.pageButtons.SendCommandRequest += new System.EventHandler<YAT.Gui.Types.PredefinedCommandEventArgs>(this.pageButtons_SendCommandRequest);
			this.pageButtons.DefineCommandRequest += new System.EventHandler<YAT.Gui.Types.PredefinedCommandEventArgs>(this.pageButtons_DefineCommandRequest);
			// 
			// label_Page
			// 
			this.label_Page.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label_Page.Location = new System.Drawing.Point(30, 3);
			this.label_Page.Name = "label_Page";
			this.label_Page.Size = new System.Drawing.Size(84, 15);
			this.label_Page.TabIndex = 1;
			this.label_Page.Text = "Page 99/99";
			this.label_Page.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button_PageNext
			// 
			this.button_PageNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.button_PageNext.Location = new System.Drawing.Point(121, 0);
			this.button_PageNext.Name = "button_PageNext";
			this.button_PageNext.Size = new System.Drawing.Size(20, 21);
			this.button_PageNext.TabIndex = 2;
			this.button_PageNext.Text = ">";
			this.button_PageNext.UseVisualStyleBackColor = true;
			this.button_PageNext.Click += new System.EventHandler(this.button_PageNext_Click);
			// 
			// button_PagePrevious
			// 
			this.button_PagePrevious.Location = new System.Drawing.Point(3, 0);
			this.button_PagePrevious.Name = "button_PagePrevious";
			this.button_PagePrevious.Size = new System.Drawing.Size(20, 21);
			this.button_PagePrevious.TabIndex = 0;
			this.button_PagePrevious.Text = "<";
			this.button_PagePrevious.UseVisualStyleBackColor = true;
			this.button_PagePrevious.Click += new System.EventHandler(this.button_PagePrevious_Click);
			// 
			// comboBox_Pages
			// 
			this.comboBox_Pages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox_Pages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox_Pages.FormattingEnabled = true;
			this.comboBox_Pages.Location = new System.Drawing.Point(3, 24);
			this.comboBox_Pages.Name = "comboBox_Pages";
			this.comboBox_Pages.Size = new System.Drawing.Size(138, 21);
			this.comboBox_Pages.TabIndex = 3;
			this.comboBox_Pages.SelectedIndexChanged += new System.EventHandler(this.comboBox_Pages_SelectedIndexChanged);
			// 
			// PredefinedCommands
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer_Commands);
			this.Name = "PredefinedCommands";
			this.Size = new System.Drawing.Size(144, 335);
			this.splitContainer_Commands.Panel1.ResumeLayout(false);
			this.splitContainer_Commands.Panel2.ResumeLayout(false);
			this.splitContainer_Commands.ResumeLayout(false);
			this.splitContainer_Lower.Panel1.ResumeLayout(false);
			this.splitContainer_Lower.Panel2.ResumeLayout(false);
			this.splitContainer_Lower.ResumeLayout(false);
			this.panel_Commands.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer_Commands;
		private System.Windows.Forms.Label label_Shortcut;
		private System.Windows.Forms.SplitContainer splitContainer_Lower;
		private System.Windows.Forms.ComboBox comboBox_Pages;
		private System.Windows.Forms.Button button_PageNext;
		private System.Windows.Forms.Button button_PagePrevious;
		private System.Windows.Forms.Label label_Page;
		private System.Windows.Forms.Panel panel_Commands;
		private PredefinedCommandPageButtons pageButtons;
	}
}
