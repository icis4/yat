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
			this.components = new System.ComponentModel.Container();
			this.splitContainer_Commands = new System.Windows.Forms.SplitContainer();
			this.label_Shortcut = new System.Windows.Forms.Label();
			this.splitContainer_Lower = new System.Windows.Forms.SplitContainer();
			this.panel_Commands = new System.Windows.Forms.Panel();
			this.pageButtons = new YAT.Gui.Controls.PredefinedCommandPageButtons();
			this.label_Page = new System.Windows.Forms.Label();
			this.button_PageNext = new System.Windows.Forms.Button();
			this.button_PagePrevious = new System.Windows.Forms.Button();
			this.comboBox_Pages = new System.Windows.Forms.ComboBox();
			this.contextMenuStrip_Predefined = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_PredefinedContextMenu_Command_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Command_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator_PredefinedContextMenu_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Page = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator_PredefinedContextMenu_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Define = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator_PredefinedContextMenu_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Hide = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer_Commands.Panel1.SuspendLayout();
			this.splitContainer_Commands.Panel2.SuspendLayout();
			this.splitContainer_Commands.SuspendLayout();
			this.splitContainer_Lower.Panel1.SuspendLayout();
			this.splitContainer_Lower.Panel2.SuspendLayout();
			this.splitContainer_Lower.SuspendLayout();
			this.panel_Commands.SuspendLayout();
			this.contextMenuStrip_Predefined.SuspendLayout();
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
			// contextMenuStrip_Predefined
			// 
			this.contextMenuStrip_Predefined.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_Command_1,
            this.toolStripMenuItem_PredefinedContextMenu_Command_2,
            this.toolStripMenuItem_PredefinedContextMenu_Command_3,
            this.toolStripMenuItem_PredefinedContextMenu_Command_4,
            this.toolStripMenuItem_PredefinedContextMenu_Command_5,
            this.toolStripMenuItem_PredefinedContextMenu_Command_6,
            this.toolStripMenuItem_PredefinedContextMenu_Command_7,
            this.toolStripMenuItem_PredefinedContextMenu_Command_8,
            this.toolStripMenuItem_PredefinedContextMenu_Command_9,
            this.toolStripMenuItem_PredefinedContextMenu_Command_10,
            this.toolStripMenuItem_PredefinedContextMenu_Command_11,
            this.toolStripMenuItem_PredefinedContextMenu_Command_12,
            this.toolStripSeparator_PredefinedContextMenu_1,
            this.toolStripMenuItem_PredefinedContextMenu_Page,
            this.toolStripSeparator_PredefinedContextMenu_2,
            this.toolStripMenuItem_PredefinedContextMenu_Define,
            this.toolStripSeparator_PredefinedContextMenu_3,
            this.toolStripMenuItem_PredefinedContextMenu_Hide});
			this.contextMenuStrip_Predefined.Name = "contextMenuStrip_PredefinedCommands";
			this.contextMenuStrip_Predefined.Size = new System.Drawing.Size(225, 374);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Name = "toolStripMenuItem_PredefinedContextMenu_Command_1";
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Text = "&1: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Name = "toolStripMenuItem_PredefinedContextMenu_Command_2";
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Text = "&2: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Name = "toolStripMenuItem_PredefinedContextMenu_Command_3";
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Text = "&3: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Name = "toolStripMenuItem_PredefinedContextMenu_Command_4";
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Text = "&4: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Name = "toolStripMenuItem_PredefinedContextMenu_Command_5";
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Text = "&5: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Name = "toolStripMenuItem_PredefinedContextMenu_Command_6";
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Text = "&6: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Name = "toolStripMenuItem_PredefinedContextMenu_Command_7";
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Text = "&7: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Name = "toolStripMenuItem_PredefinedContextMenu_Command_8";
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Text = "&8: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Name = "toolStripMenuItem_PredefinedContextMenu_Command_9";
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F9)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Text = "&9: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Name = "toolStripMenuItem_PredefinedContextMenu_Command_10";
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Text = "1&0: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Name = "toolStripMenuItem_PredefinedContextMenu_Command_11";
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Text = "11: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Name = "toolStripMenuItem_PredefinedContextMenu_Command_12";
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F12)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Text = "12: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripSeparator_PredefinedContextMenu_1
			// 
			this.toolStripSeparator_PredefinedContextMenu_1.Name = "toolStripSeparator_PredefinedContextMenu_1";
			this.toolStripSeparator_PredefinedContextMenu_1.Size = new System.Drawing.Size(221, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_Page_Next,
            this.toolStripMenuItem_PredefinedContextMenu_Page_Previous});
			this.toolStripMenuItem_PredefinedContextMenu_Page.Name = "toolStripMenuItem_PredefinedContextMenu_Page";
			this.toolStripMenuItem_PredefinedContextMenu_Page.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page.Text = "&Page";
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_Next
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Name = "toolStripMenuItem_PredefinedContextMenu_Page_Next";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Right)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Text = "&Next";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Next_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_Previous
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Name = "toolStripMenuItem_PredefinedContextMenu_Page_Previous";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Left)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Text = "&Previous";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Previous_Click);
			// 
			// toolStripSeparator_PredefinedContextMenu_2
			// 
			this.toolStripSeparator_PredefinedContextMenu_2.Name = "toolStripSeparator_PredefinedContextMenu_2";
			this.toolStripSeparator_PredefinedContextMenu_2.Size = new System.Drawing.Size(221, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Define
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Define.Name = "toolStripMenuItem_PredefinedContextMenu_Define";
			this.toolStripMenuItem_PredefinedContextMenu_Define.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Define.Text = "Define...";
			this.toolStripMenuItem_PredefinedContextMenu_Define.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Define_Click);
			// 
			// toolStripSeparator_PredefinedContextMenu_3
			// 
			this.toolStripSeparator_PredefinedContextMenu_3.Name = "toolStripSeparator_PredefinedContextMenu_3";
			this.toolStripSeparator_PredefinedContextMenu_3.Size = new System.Drawing.Size(221, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Hide
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Name = "toolStripMenuItem_PredefinedContextMenu_Hide";
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Size = new System.Drawing.Size(224, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Text = "Hide";
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Hide_Click);
			// 
			// PredefinedCommands
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ContextMenuStrip = this.contextMenuStrip_Predefined;
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
			this.contextMenuStrip_Predefined.ResumeLayout(false);
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
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Predefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command_12;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator_PredefinedContextMenu_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_Next;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_Previous;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator_PredefinedContextMenu_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Define;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator_PredefinedContextMenu_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Hide;
	}
}
