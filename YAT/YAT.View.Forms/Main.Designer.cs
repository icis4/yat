using MKY;

namespace YAT.View.Forms
{
	partial class Main : IDisposableEx // Implemented based on Control.IsDisposed :-)
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
			DebugMessage("Disposing..."); // Additional debug message indicating the sequence of disposal (model -vs- view).

			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			DebugMessage("...successfully disposed."); // Additional debug message indicating the sequence of disposal (model -vs- view).
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.timer_Status = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_MainContextMenu_File_New = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainContextMenu_File_Open = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainContextMenu_File_Recent = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_FileRecent = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_FileRecentContextMenu_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_FileRecentContextMenu_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainContextMenu_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Recent = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip_Main = new MKY.Windows.Forms.MenuStripEx();
			this.toolStripMenuItem_MainMenu_File = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_New = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Open = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_CloseAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_SaveAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_Workspace = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Workspace_New = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Workspace_Open = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_Workspace_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_Workspace_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_Workspace_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_Workspace_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_Preferences = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_File_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_File_Exit = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Log = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Log_AllOn = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Log_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Log_AllOff = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Log_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Log_AllClear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_Separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Window_Automatic = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_Cascade = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_TileVertical = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_Separator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Window_Minimize = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Window_Maximize = new System.Windows.Forms.ToolStripMenuItem();
		#if (WITH_SCRIPTING)
			this.toolStripMenuItem_MainMenu_Script = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Script_Panel = new System.Windows.Forms.ToolStripMenuItem();
		#endif
			this.toolStripMenuItem_MainMenu_Help = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_Contents = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
		#if !(WITH_SCRIPTING)
			this.toolStripMenuItem_MainMenu_Help_ReleaseNotes = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Help_RequestSupport = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_RequestFeature = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_SubmitBug = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Help_Update = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MainMenu_Help_Donate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MainMenu_Help_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
		#endif
			this.toolStripMenuItem_MainMenu_Help_About = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip_Main = new MKY.Windows.Forms.StatusStripEx();
			this.contextMenuStrip_Status = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_StatusContextMenu_ShowTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_StatusContextMenu_ShowChrono = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_StatusContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_StatusContextMenu_Preferences = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripStatusLabel_MainStatus_Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_MainStatus_TerminalInfo = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_MainStatus_Time = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_MainStatus_Chrono = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip_Main = new MKY.Windows.Forms.ToolStripEx();
			this.toolStripButton_MainTool_File_New = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_File_Open = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_File_Save = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_File_SaveWorkspace = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Terminal_Settings = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Terminal_Start = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Terminal_Stop = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Radix_String = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Radix_Char = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Radix_Bin = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Radix_Oct = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Radix_Dec = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Radix_Hex = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Radix_Unicode = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Terminal_Clear = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Terminal_Refresh = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Terminal_CopyToClipboard = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Terminal_SaveToFile = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Terminal_Print = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Find_ShowHide = new System.Windows.Forms.ToolStripButton();
			this.toolStripComboBox_MainTool_Find_Pattern = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripButton_MainTool_Find_CaseSensitive = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Find_WholeWord = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Find_EnableRegex = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Find_Next = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Find_Previous = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Log_Settings = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Log_On = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Log_Off = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Log_OpenFile = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_Log_OpenDirectory = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_AutoAction_ShowHide = new System.Windows.Forms.ToolStripButton();
			this.toolStripComboBox_MainTool_AutoAction_Trigger = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex = new System.Windows.Forms.ToolStripButton();
			this.toolStripComboBox_MainTool_AutoAction_Action = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripLabel_MainTool_AutoAction_Count = new System.Windows.Forms.ToolStripLabel();
			this.toolStripButton_MainTool_AutoAction_Deactivate = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_8 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_AutoResponse_ShowHide = new System.Windows.Forms.ToolStripButton();
			this.toolStripComboBox_MainTool_AutoResponse_Trigger = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord = new System.Windows.Forms.ToolStripButton();
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex = new System.Windows.Forms.ToolStripButton();
			this.toolStripComboBox_MainTool_AutoResponse_Response = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace = new System.Windows.Forms.ToolStripButton();
			this.toolStripLabel_MainTool_AutoResponse_Count = new System.Windows.Forms.ToolStripLabel();
			this.toolStripButton_MainTool_AutoResponse_Deactivate = new System.Windows.Forms.ToolStripButton();
			this.toolStripMenuItem_MainTool_Separator_9 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Terminal_Format = new System.Windows.Forms.ToolStripButton();
		#if (WITH_SCRIPTING)
			this.toolStripMenuItem_MainTool_Separator_10 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripButton_MainTool_Script_ShowHide = new System.Windows.Forms.ToolStripButton();
		#endif
			this.toolStripPanel_Top = new System.Windows.Forms.ToolStripPanel();
			this.toolStripPanel_Right = new System.Windows.Forms.ToolStripPanel();
			this.toolStripPanel_Left = new System.Windows.Forms.ToolStripPanel();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.chronometer_Main = new MKY.Windows.Forms.Chronometer(this.components);
			this.timer_Time = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip_Main.SuspendLayout();
			this.contextMenuStrip_FileRecent.SuspendLayout();
			this.menuStrip_Main.SuspendLayout();
			this.statusStrip_Main.SuspendLayout();
			this.contextMenuStrip_Status.SuspendLayout();
			this.toolStrip_Main.SuspendLayout();
			this.toolStripPanel_Top.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer_Status
			// 
			this.timer_Status.Tick += new System.EventHandler(this.timer_Status_Tick);
			// 
			// contextMenuStrip_Main
			// 
			this.contextMenuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainContextMenu_File_New,
            this.toolStripMenuItem_MainContextMenu_File_Open,
            this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace,
            this.toolStripMenuItem_MainContextMenu_Separator_1,
            this.toolStripMenuItem_MainContextMenu_File_Recent,
            this.toolStripMenuItem_MainContextMenu_Separator_2,
            this.toolStripMenuItem_MainContextMenu_File_Exit});
			this.contextMenuStrip_Main.Name = "contextMenuStrip_Main";
			this.contextMenuStrip_Main.Size = new System.Drawing.Size(174, 126);
			this.contextMenuStrip_Main.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Main_Opening);
			// 
			// toolStripMenuItem_MainContextMenu_File_New
			// 
			this.toolStripMenuItem_MainContextMenu_File_New.Name = "toolStripMenuItem_MainContextMenu_File_New";
			this.toolStripMenuItem_MainContextMenu_File_New.Size = new System.Drawing.Size(173, 22);
			this.toolStripMenuItem_MainContextMenu_File_New.Text = "New...";
			this.toolStripMenuItem_MainContextMenu_File_New.Click += new System.EventHandler(this.toolStripMenuItem_MainContextMenu_File_New_Click);
			// 
			// toolStripMenuItem_MainContextMenu_File_Open
			// 
			this.toolStripMenuItem_MainContextMenu_File_Open.Name = "toolStripMenuItem_MainContextMenu_File_Open";
			this.toolStripMenuItem_MainContextMenu_File_Open.Size = new System.Drawing.Size(173, 22);
			this.toolStripMenuItem_MainContextMenu_File_Open.Text = "Open...";
			this.toolStripMenuItem_MainContextMenu_File_Open.Click += new System.EventHandler(this.toolStripMenuItem_MainContextMenu_File_Open_Click);
			// 
			// toolStripMenuItem_MainContextMenu_File_OpenWorkspace
			// 
			this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace.Name = "toolStripMenuItem_MainContextMenu_File_OpenWorkspace";
			this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace.Size = new System.Drawing.Size(173, 22);
			this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace.Text = "Open Workspace...";
			this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace.Click += new System.EventHandler(this.toolStripMenuItem_MainContextMenu_File_OpenWorkspace_Click);
			// 
			// toolStripMenuItem_MainContextMenu_Separator_1
			// 
			this.toolStripMenuItem_MainContextMenu_Separator_1.Name = "toolStripMenuItem_MainContextMenu_Separator_1";
			this.toolStripMenuItem_MainContextMenu_Separator_1.Size = new System.Drawing.Size(170, 6);
			// 
			// toolStripMenuItem_MainContextMenu_File_Recent
			// 
			this.toolStripMenuItem_MainContextMenu_File_Recent.DropDown = this.contextMenuStrip_FileRecent;
			this.toolStripMenuItem_MainContextMenu_File_Recent.Enabled = false;
			this.toolStripMenuItem_MainContextMenu_File_Recent.Name = "toolStripMenuItem_MainContextMenu_File_Recent";
			this.toolStripMenuItem_MainContextMenu_File_Recent.Size = new System.Drawing.Size(173, 22);
			this.toolStripMenuItem_MainContextMenu_File_Recent.Text = "Recent";
			// 
			// contextMenuStrip_FileRecent
			// 
			this.contextMenuStrip_FileRecent.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_FileRecentContextMenu_1,
            this.toolStripMenuItem_FileRecentContextMenu_2,
            this.toolStripMenuItem_FileRecentContextMenu_3,
            this.toolStripMenuItem_FileRecentContextMenu_4,
            this.toolStripMenuItem_FileRecentContextMenu_5,
            this.toolStripMenuItem_FileRecentContextMenu_6,
            this.toolStripMenuItem_FileRecentContextMenu_7,
            this.toolStripMenuItem_FileRecentContextMenu_8});
			this.contextMenuStrip_FileRecent.Name = "contextMenuStrip_FileRecent";
			// BEGIN OF ATTENTION BLOCK
			//
			//
			//
			// Attention block is likely to be removed by designer.
			// Block intentionally enlarged for not missing when diffing.
			//
			//
			//
			// A context strip's 'OwnerItem' must be set to the main menu,...
			this.contextMenuStrip_FileRecent.OwnerItem = this.toolStripMenuItem_MainMenu_File_Recent;
			// ...associated main shortcuts would otherwise be routed to the first rather than the active MDI child!
			//
			//
			//
			// Attention block is likely to be removed by designer.
			// Block intentionally enlarged for not missing when diffing.
			//
			//
			//
			// END OF ATTENTION BLOCK
			this.contextMenuStrip_FileRecent.Size = new System.Drawing.Size(87, 180);
			this.contextMenuStrip_FileRecent.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_FileRecent_Opening);
			// 
			// toolStripMenuItem_FileRecentContextMenu_1
			// 
			this.toolStripMenuItem_FileRecentContextMenu_1.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_1.Name = "toolStripMenuItem_FileRecentContextMenu_1";
			this.toolStripMenuItem_FileRecentContextMenu_1.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_1.Tag = "1";
			this.toolStripMenuItem_FileRecentContextMenu_1.Text = "1: ";
			this.toolStripMenuItem_FileRecentContextMenu_1.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_1.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_2
			// 
			this.toolStripMenuItem_FileRecentContextMenu_2.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_2.Name = "toolStripMenuItem_FileRecentContextMenu_2";
			this.toolStripMenuItem_FileRecentContextMenu_2.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_2.Tag = "2";
			this.toolStripMenuItem_FileRecentContextMenu_2.Text = "2: ";
			this.toolStripMenuItem_FileRecentContextMenu_2.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_2.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_3
			// 
			this.toolStripMenuItem_FileRecentContextMenu_3.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_3.Name = "toolStripMenuItem_FileRecentContextMenu_3";
			this.toolStripMenuItem_FileRecentContextMenu_3.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_3.Tag = "3";
			this.toolStripMenuItem_FileRecentContextMenu_3.Text = "3: ";
			this.toolStripMenuItem_FileRecentContextMenu_3.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_3.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_4
			// 
			this.toolStripMenuItem_FileRecentContextMenu_4.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_4.Name = "toolStripMenuItem_FileRecentContextMenu_4";
			this.toolStripMenuItem_FileRecentContextMenu_4.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_4.Tag = "4";
			this.toolStripMenuItem_FileRecentContextMenu_4.Text = "4: ";
			this.toolStripMenuItem_FileRecentContextMenu_4.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_4.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_5
			// 
			this.toolStripMenuItem_FileRecentContextMenu_5.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_5.Name = "toolStripMenuItem_FileRecentContextMenu_5";
			this.toolStripMenuItem_FileRecentContextMenu_5.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_5.Tag = "5";
			this.toolStripMenuItem_FileRecentContextMenu_5.Text = "5: ";
			this.toolStripMenuItem_FileRecentContextMenu_5.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_5.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_6
			// 
			this.toolStripMenuItem_FileRecentContextMenu_6.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_6.Name = "toolStripMenuItem_FileRecentContextMenu_6";
			this.toolStripMenuItem_FileRecentContextMenu_6.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_6.Tag = "6";
			this.toolStripMenuItem_FileRecentContextMenu_6.Text = "6: ";
			this.toolStripMenuItem_FileRecentContextMenu_6.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_6.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_7
			// 
			this.toolStripMenuItem_FileRecentContextMenu_7.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_7.Name = "toolStripMenuItem_FileRecentContextMenu_7";
			this.toolStripMenuItem_FileRecentContextMenu_7.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_7.Tag = "7";
			this.toolStripMenuItem_FileRecentContextMenu_7.Text = "7: ";
			this.toolStripMenuItem_FileRecentContextMenu_7.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_7.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_FileRecentContextMenu_8
			// 
			this.toolStripMenuItem_FileRecentContextMenu_8.Enabled = false;
			this.toolStripMenuItem_FileRecentContextMenu_8.Name = "toolStripMenuItem_FileRecentContextMenu_8";
			this.toolStripMenuItem_FileRecentContextMenu_8.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_FileRecentContextMenu_8.Tag = "8";
			this.toolStripMenuItem_FileRecentContextMenu_8.Text = "8: ";
			this.toolStripMenuItem_FileRecentContextMenu_8.Visible = false;
			this.toolStripMenuItem_FileRecentContextMenu_8.Click += new System.EventHandler(this.toolStripMenuItem_FileRecentContextMenu_Click);
			// 
			// toolStripMenuItem_MainContextMenu_Separator_2
			// 
			this.toolStripMenuItem_MainContextMenu_Separator_2.Name = "toolStripMenuItem_MainContextMenu_Separator_2";
			this.toolStripMenuItem_MainContextMenu_Separator_2.Size = new System.Drawing.Size(170, 6);
			// 
			// toolStripMenuItem_MainContextMenu_File_Exit
			// 
			this.toolStripMenuItem_MainContextMenu_File_Exit.Name = "toolStripMenuItem_MainContextMenu_File_Exit";
			this.toolStripMenuItem_MainContextMenu_File_Exit.Size = new System.Drawing.Size(173, 22);
			this.toolStripMenuItem_MainContextMenu_File_Exit.Text = "Exit";
			this.toolStripMenuItem_MainContextMenu_File_Exit.Click += new System.EventHandler(this.toolStripMenuItem_MainContextMenu_File_Exit_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Recent
			// 
			this.toolStripMenuItem_MainMenu_File_Recent.DropDown = this.contextMenuStrip_FileRecent;
			this.toolStripMenuItem_MainMenu_File_Recent.Enabled = false;
			this.toolStripMenuItem_MainMenu_File_Recent.Name = "toolStripMenuItem_MainMenu_File_Recent";
			this.toolStripMenuItem_MainMenu_File_Recent.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_Recent.Text = "&Recent";
			// 
			// menuStrip_Main
			// 
			this.menuStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_File,
            this.toolStripMenuItem_MainMenu_Log,
            this.toolStripMenuItem_MainMenu_Window,
		#if (WITH_SCRIPTING)
            this.toolStripMenuItem_MainMenu_Script,
		#endif
            this.toolStripMenuItem_MainMenu_Help});
			this.menuStrip_Main.Location = new System.Drawing.Point(0, 0);
			this.menuStrip_Main.MdiWindowListItem = this.toolStripMenuItem_MainMenu_Window;
			this.menuStrip_Main.Name = "menuStrip_Main";
			this.menuStrip_Main.Size = new System.Drawing.Size(896, 24);
			this.menuStrip_Main.TabIndex = 0;
			// 
			// toolStripMenuItem_MainMenu_File
			// 
			this.toolStripMenuItem_MainMenu_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_File_New,
            this.toolStripMenuItem_MainMenu_File_Open,
            this.toolStripMenuItem_MainMenu_File_Separator_1,
            this.toolStripMenuItem_MainMenu_File_CloseAll,
            this.toolStripMenuItem_MainMenu_File_Separator_2,
            this.toolStripMenuItem_MainMenu_File_SaveAll,
            this.toolStripMenuItem_MainMenu_File_Separator_3,
            this.toolStripMenuItem_MainMenu_File_Workspace,
            this.toolStripMenuItem_MainMenu_File_Separator_4,
            this.toolStripMenuItem_MainMenu_File_Preferences,
            this.toolStripMenuItem_MainMenu_File_Separator_5,
            this.toolStripMenuItem_MainMenu_File_Recent,
            this.toolStripMenuItem_MainMenu_File_Separator_6,
            this.toolStripMenuItem_MainMenu_File_Exit});
			this.toolStripMenuItem_MainMenu_File.MergeIndex = 0;
			this.toolStripMenuItem_MainMenu_File.Name = "toolStripMenuItem_MainMenu_File";
			this.toolStripMenuItem_MainMenu_File.Size = new System.Drawing.Size(37, 20);
			this.toolStripMenuItem_MainMenu_File.Text = "&File";
			this.toolStripMenuItem_MainMenu_File.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_DropDownOpening);
			// 
			// toolStripMenuItem_MainMenu_File_New
			// 
			this.toolStripMenuItem_MainMenu_File_New.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_add_16x16;
			this.toolStripMenuItem_MainMenu_File_New.MergeIndex = 0;
			this.toolStripMenuItem_MainMenu_File_New.Name = "toolStripMenuItem_MainMenu_File_New";
			this.toolStripMenuItem_MainMenu_File_New.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.toolStripMenuItem_MainMenu_File_New.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_New.Text = "&New Terminal...";
			this.toolStripMenuItem_MainMenu_File_New.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_New_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Open
			// 
			this.toolStripMenuItem_MainMenu_File_Open.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_folder_add_16x16;
			this.toolStripMenuItem_MainMenu_File_Open.MergeIndex = 1;
			this.toolStripMenuItem_MainMenu_File_Open.Name = "toolStripMenuItem_MainMenu_File_Open";
			this.toolStripMenuItem_MainMenu_File_Open.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.toolStripMenuItem_MainMenu_File_Open.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_Open.Text = "&Open...";
			this.toolStripMenuItem_MainMenu_File_Open.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Open_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_1
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_1.MergeIndex = 2;
			this.toolStripMenuItem_MainMenu_File_Separator_1.Name = "toolStripMenuItem_MainMenu_File_Separator_1";
			this.toolStripMenuItem_MainMenu_File_Separator_1.Size = new System.Drawing.Size(239, 6);
			// 
			// toolStripMenuItem_MainMenu_File_CloseAll
			// 
			this.toolStripMenuItem_MainMenu_File_CloseAll.Enabled = false;
			this.toolStripMenuItem_MainMenu_File_CloseAll.MergeIndex = 4;
			this.toolStripMenuItem_MainMenu_File_CloseAll.Name = "toolStripMenuItem_MainMenu_File_CloseAll";
			this.toolStripMenuItem_MainMenu_File_CloseAll.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_CloseAll.Text = "Clos&e All Terminals";
			this.toolStripMenuItem_MainMenu_File_CloseAll.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_CloseAll_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_2
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_2.MergeIndex = 5;
			this.toolStripMenuItem_MainMenu_File_Separator_2.Name = "toolStripMenuItem_MainMenu_File_Separator_2";
			this.toolStripMenuItem_MainMenu_File_Separator_2.Size = new System.Drawing.Size(239, 6);
			// 
			// toolStripMenuItem_MainMenu_File_SaveAll
			// 
			this.toolStripMenuItem_MainMenu_File_SaveAll.Enabled = false;
			this.toolStripMenuItem_MainMenu_File_SaveAll.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_save_all_16x16;
			this.toolStripMenuItem_MainMenu_File_SaveAll.MergeIndex = 8;
			this.toolStripMenuItem_MainMenu_File_SaveAll.Name = "toolStripMenuItem_MainMenu_File_SaveAll";
			this.toolStripMenuItem_MainMenu_File_SaveAll.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
			this.toolStripMenuItem_MainMenu_File_SaveAll.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_SaveAll.Text = "Save A&ll Terminals";
			this.toolStripMenuItem_MainMenu_File_SaveAll.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_SaveAll_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_3
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_3.MergeIndex = 9;
			this.toolStripMenuItem_MainMenu_File_Separator_3.Name = "toolStripMenuItem_MainMenu_File_Separator_3";
			this.toolStripMenuItem_MainMenu_File_Separator_3.Size = new System.Drawing.Size(239, 6);
			// 
			// toolStripMenuItem_MainMenu_File_Workspace
			// 
			this.toolStripMenuItem_MainMenu_File_Workspace.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_File_Workspace_New,
            this.toolStripMenuItem_MainMenu_File_Workspace_Open,
            this.toolStripMenuItem_MainMenu_File_Separator_Workspace_1,
            this.toolStripMenuItem_MainMenu_File_Workspace_Close,
            this.toolStripMenuItem_MainMenu_File_Separator_Workspace_2,
            this.toolStripMenuItem_MainMenu_File_Workspace_Save,
            this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs});
			this.toolStripMenuItem_MainMenu_File_Workspace.Name = "toolStripMenuItem_MainMenu_File_Workspace";
			this.toolStripMenuItem_MainMenu_File_Workspace.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_Workspace.Text = "&Workspace";
			this.toolStripMenuItem_MainMenu_File_Workspace.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Workspace_DropDownOpening);
			// 
			// toolStripMenuItem_MainMenu_File_Workspace_New
			// 
			this.toolStripMenuItem_MainMenu_File_Workspace_New.Name = "toolStripMenuItem_MainMenu_File_Workspace_New";
			this.toolStripMenuItem_MainMenu_File_Workspace_New.Size = new System.Drawing.Size(175, 22);
			this.toolStripMenuItem_MainMenu_File_Workspace_New.Text = "&New";
			this.toolStripMenuItem_MainMenu_File_Workspace_New.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Workspace_New_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Workspace_Open
			// 
			this.toolStripMenuItem_MainMenu_File_Workspace_Open.Name = "toolStripMenuItem_MainMenu_File_Workspace_Open";
			this.toolStripMenuItem_MainMenu_File_Workspace_Open.Size = new System.Drawing.Size(175, 22);
			this.toolStripMenuItem_MainMenu_File_Workspace_Open.Text = "&Open...";
			this.toolStripMenuItem_MainMenu_File_Workspace_Open.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Workspace_Open_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_Workspace_1
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_Workspace_1.Name = "toolStripMenuItem_MainMenu_File_Separator_Workspace_1";
			this.toolStripMenuItem_MainMenu_File_Separator_Workspace_1.Size = new System.Drawing.Size(172, 6);
			// 
			// toolStripMenuItem_MainMenu_File_Workspace_Close
			// 
			this.toolStripMenuItem_MainMenu_File_Workspace_Close.Name = "toolStripMenuItem_MainMenu_File_Workspace_Close";
			this.toolStripMenuItem_MainMenu_File_Workspace_Close.Size = new System.Drawing.Size(175, 22);
			this.toolStripMenuItem_MainMenu_File_Workspace_Close.Text = "&Close";
			this.toolStripMenuItem_MainMenu_File_Workspace_Close.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Workspace_Close_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_Workspace_2
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_Workspace_2.Name = "toolStripMenuItem_MainMenu_File_Separator_Workspace_2";
			this.toolStripMenuItem_MainMenu_File_Separator_Workspace_2.Size = new System.Drawing.Size(172, 6);
			// 
			// toolStripMenuItem_MainMenu_File_Workspace_Save
			// 
			this.toolStripMenuItem_MainMenu_File_Workspace_Save.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_layer_save_16x16;
			this.toolStripMenuItem_MainMenu_File_Workspace_Save.Name = "toolStripMenuItem_MainMenu_File_Workspace_Save";
			this.toolStripMenuItem_MainMenu_File_Workspace_Save.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
			this.toolStripMenuItem_MainMenu_File_Workspace_Save.Size = new System.Drawing.Size(175, 22);
			this.toolStripMenuItem_MainMenu_File_Workspace_Save.Text = "&Save";
			this.toolStripMenuItem_MainMenu_File_Workspace_Save.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Workspace_Save_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Workspace_SaveAs
			// 
			this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Name = "toolStripMenuItem_MainMenu_File_Workspace_SaveAs";
			this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Size = new System.Drawing.Size(175, 22);
			this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Text = "Save &As...";
			this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Workspace_SaveAs_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_4
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_4.Name = "toolStripMenuItem_MainMenu_File_Separator_4";
			this.toolStripMenuItem_MainMenu_File_Separator_4.Size = new System.Drawing.Size(239, 6);
			// 
			// toolStripMenuItem_MainMenu_File_Preferences
			// 
			this.toolStripMenuItem_MainMenu_File_Preferences.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_setting_tools_16x16;
			this.toolStripMenuItem_MainMenu_File_Preferences.Name = "toolStripMenuItem_MainMenu_File_Preferences";
			this.toolStripMenuItem_MainMenu_File_Preferences.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_Preferences.Text = "P&references...";
			this.toolStripMenuItem_MainMenu_File_Preferences.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Preferences_Click);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_5
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_5.Name = "toolStripMenuItem_MainMenu_File_Separator_5";
			this.toolStripMenuItem_MainMenu_File_Separator_5.Size = new System.Drawing.Size(239, 6);
			// 
			// toolStripMenuItem_MainMenu_File_Separator_6
			// 
			this.toolStripMenuItem_MainMenu_File_Separator_6.Name = "toolStripMenuItem_MainMenu_File_Separator_6";
			this.toolStripMenuItem_MainMenu_File_Separator_6.Size = new System.Drawing.Size(239, 6);
			// 
			// toolStripMenuItem_MainMenu_File_Exit
			// 
			this.toolStripMenuItem_MainMenu_File_Exit.Name = "toolStripMenuItem_MainMenu_File_Exit";
			this.toolStripMenuItem_MainMenu_File_Exit.Size = new System.Drawing.Size(242, 22);
			this.toolStripMenuItem_MainMenu_File_Exit.Text = "E&xit";
			this.toolStripMenuItem_MainMenu_File_Exit.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_File_Exit_Click);
			// 
			// toolStripMenuItem_MainMenu_Log
			// 
			this.toolStripMenuItem_MainMenu_Log.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_Log_AllOn,
            this.toolStripMenuItem_MainMenu_Log_Separator_1,
            this.toolStripMenuItem_MainMenu_Log_AllOff,
            this.toolStripMenuItem_MainMenu_Log_Separator_2,
            this.toolStripMenuItem_MainMenu_Log_AllClear});
			this.toolStripMenuItem_MainMenu_Log.MergeIndex = 4;
			this.toolStripMenuItem_MainMenu_Log.Name = "toolStripMenuItem_MainMenu_Log";
			this.toolStripMenuItem_MainMenu_Log.Size = new System.Drawing.Size(39, 20);
			this.toolStripMenuItem_MainMenu_Log.Text = "&Log";
			this.toolStripMenuItem_MainMenu_Log.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_MainMenu_Log_DropDownOpening);
			// 
			// toolStripMenuItem_MainMenu_Log_AllOn
			// 
			this.toolStripMenuItem_MainMenu_Log_AllOn.MergeIndex = 1;
			this.toolStripMenuItem_MainMenu_Log_AllOn.Name = "toolStripMenuItem_MainMenu_Log_AllOn";
			this.toolStripMenuItem_MainMenu_Log_AllOn.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_MainMenu_Log_AllOn.Text = "&All Terminals On";
			this.toolStripMenuItem_MainMenu_Log_AllOn.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Log_AllOn_Click);
			// 
			// toolStripMenuItem_MainMenu_Log_Separator_1
			// 
			this.toolStripMenuItem_MainMenu_Log_Separator_1.MergeIndex = 2;
			this.toolStripMenuItem_MainMenu_Log_Separator_1.Name = "toolStripMenuItem_MainMenu_Log_Separator_1";
			this.toolStripMenuItem_MainMenu_Log_Separator_1.Size = new System.Drawing.Size(168, 6);
			// 
			// toolStripMenuItem_MainMenu_Log_AllOff
			// 
			this.toolStripMenuItem_MainMenu_Log_AllOff.MergeIndex = 4;
			this.toolStripMenuItem_MainMenu_Log_AllOff.Name = "toolStripMenuItem_MainMenu_Log_AllOff";
			this.toolStripMenuItem_MainMenu_Log_AllOff.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_MainMenu_Log_AllOff.Text = "A&ll Terminals Off";
			this.toolStripMenuItem_MainMenu_Log_AllOff.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Log_AllOff_Click);
			// 
			// toolStripMenuItem_MainMenu_Log_Separator_2
			// 
			this.toolStripMenuItem_MainMenu_Log_Separator_2.MergeIndex = 5;
			this.toolStripMenuItem_MainMenu_Log_Separator_2.Name = "toolStripMenuItem_MainMenu_Log_Separator_2";
			this.toolStripMenuItem_MainMenu_Log_Separator_2.Size = new System.Drawing.Size(168, 6);
			// 
			// toolStripMenuItem_MainMenu_Log_AllClear
			// 
			this.toolStripMenuItem_MainMenu_Log_AllClear.MergeIndex = 10;
			this.toolStripMenuItem_MainMenu_Log_AllClear.Name = "toolStripMenuItem_MainMenu_Log_AllClear";
			this.toolStripMenuItem_MainMenu_Log_AllClear.Size = new System.Drawing.Size(171, 22);
			this.toolStripMenuItem_MainMenu_Log_AllClear.Text = "All Terminals Clea&r";
			this.toolStripMenuItem_MainMenu_Log_AllClear.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Log_AllClear_Click);
			// 
			// toolStripMenuItem_MainMenu_Window
			// 
			this.toolStripMenuItem_MainMenu_Window.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop,
            this.toolStripMenuItem_MainMenu_Window_Separator1,
            this.toolStripMenuItem_MainMenu_Window_Automatic,
            this.toolStripMenuItem_MainMenu_Window_Cascade,
            this.toolStripMenuItem_MainMenu_Window_TileHorizontal,
            this.toolStripMenuItem_MainMenu_Window_TileVertical,
            this.toolStripMenuItem_MainMenu_Window_Separator2,
            this.toolStripMenuItem_MainMenu_Window_Minimize,
            this.toolStripMenuItem_MainMenu_Window_Maximize});
			this.toolStripMenuItem_MainMenu_Window.MergeIndex = 6;
			this.toolStripMenuItem_MainMenu_Window.Name = "toolStripMenuItem_MainMenu_Window";
			this.toolStripMenuItem_MainMenu_Window.Size = new System.Drawing.Size(63, 20);
			this.toolStripMenuItem_MainMenu_Window.Text = "&Window";
			this.toolStripMenuItem_MainMenu_Window.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_DropDownOpening);
			// 
			// toolStripMenuItem_MainMenu_Window_AlwaysOnTop
			// 
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_new_window_16x16;
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Name = "toolStripMenuItem_MainMenu_Window_AlwaysOnTop";
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.T)));
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Text = "Always On &Top";
			this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_AlwaysOnTop_Click);
			// 
			// toolStripMenuItem_MainMenu_Window_Separator1
			// 
			this.toolStripMenuItem_MainMenu_Window_Separator1.Name = "toolStripMenuItem_MainMenu_Window_Separator1";
			this.toolStripMenuItem_MainMenu_Window_Separator1.Size = new System.Drawing.Size(218, 6);
			// 
			// toolStripMenuItem_MainMenu_Window_Automatic
			// 
			this.toolStripMenuItem_MainMenu_Window_Automatic.Enabled = false;
			this.toolStripMenuItem_MainMenu_Window_Automatic.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_lightning_16x16;
			this.toolStripMenuItem_MainMenu_Window_Automatic.Name = "toolStripMenuItem_MainMenu_Window_Automatic";
			this.toolStripMenuItem_MainMenu_Window_Automatic.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
			this.toolStripMenuItem_MainMenu_Window_Automatic.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_Automatic.Text = "&Automatic";
			this.toolStripMenuItem_MainMenu_Window_Automatic.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_Automatic_Click);
			// 
			// toolStripMenuItem_MainMenu_Window_Cascade
			// 
			this.toolStripMenuItem_MainMenu_Window_Cascade.Enabled = false;
			this.toolStripMenuItem_MainMenu_Window_Cascade.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_cascade_16x16;
			this.toolStripMenuItem_MainMenu_Window_Cascade.Name = "toolStripMenuItem_MainMenu_Window_Cascade";
			this.toolStripMenuItem_MainMenu_Window_Cascade.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItem_MainMenu_Window_Cascade.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_Cascade.Text = "&Cascade";
			this.toolStripMenuItem_MainMenu_Window_Cascade.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_Cascade_Click);
			// 
			// toolStripMenuItem_MainMenu_Window_TileHorizontal
			// 
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = false;
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_tile_horizontal_16x16;
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.Name = "toolStripMenuItem_MainMenu_Window_TileHorizontal";
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.H)));
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.Text = "Tile &Horizontal";
			this.toolStripMenuItem_MainMenu_Window_TileHorizontal.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_TileHorizontal_Click);
			// 
			// toolStripMenuItem_MainMenu_Window_TileVertical
			// 
			this.toolStripMenuItem_MainMenu_Window_TileVertical.Enabled = false;
			this.toolStripMenuItem_MainMenu_Window_TileVertical.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_tile_vertical_16x16;
			this.toolStripMenuItem_MainMenu_Window_TileVertical.Name = "toolStripMenuItem_MainMenu_Window_TileVertical";
			this.toolStripMenuItem_MainMenu_Window_TileVertical.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.V)));
			this.toolStripMenuItem_MainMenu_Window_TileVertical.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_TileVertical.Text = "Tile &Vertical";
			this.toolStripMenuItem_MainMenu_Window_TileVertical.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_TileVertical_Click);
			// 
			// toolStripMenuItem_MainMenu_Window_Separator2
			// 
			this.toolStripMenuItem_MainMenu_Window_Separator2.Name = "toolStripMenuItem_MainMenu_Window_Separator2";
			this.toolStripMenuItem_MainMenu_Window_Separator2.Size = new System.Drawing.Size(218, 6);
			// 
			// toolStripMenuItem_MainMenu_Window_Minimize
			// 
			this.toolStripMenuItem_MainMenu_Window_Minimize.Enabled = false;
			this.toolStripMenuItem_MainMenu_Window_Minimize.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_put_16x16;
			this.toolStripMenuItem_MainMenu_Window_Minimize.Name = "toolStripMenuItem_MainMenu_Window_Minimize";
			this.toolStripMenuItem_MainMenu_Window_Minimize.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
			this.toolStripMenuItem_MainMenu_Window_Minimize.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_Minimize.Text = "Mi&nimize";
			this.toolStripMenuItem_MainMenu_Window_Minimize.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_Minimize_Click);
			// 
			// toolStripMenuItem_MainMenu_Window_Maximize
			// 
			this.toolStripMenuItem_MainMenu_Window_Maximize.Enabled = false;
			this.toolStripMenuItem_MainMenu_Window_Maximize.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_get_16x16;
			this.toolStripMenuItem_MainMenu_Window_Maximize.Name = "toolStripMenuItem_MainMenu_Window_Maximize";
			this.toolStripMenuItem_MainMenu_Window_Maximize.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.X)));
			this.toolStripMenuItem_MainMenu_Window_Maximize.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_MainMenu_Window_Maximize.Text = "Ma&ximize";
			this.toolStripMenuItem_MainMenu_Window_Maximize.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Window_Maximize_Click);
		#if (WITH_SCRIPTING)
			// 
			// toolStripMenuItem_MainMenu_Script
			// 
			this.toolStripMenuItem_MainMenu_Script.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_Script_Panel});
			this.toolStripMenuItem_MainMenu_Script.MergeIndex = 7;
			this.toolStripMenuItem_MainMenu_Script.Name = "toolStripMenuItem_MainMenu_Script";
			this.toolStripMenuItem_MainMenu_Script.Size = new System.Drawing.Size(50, 20);
			this.toolStripMenuItem_MainMenu_Script.Text = "S&cript";
			// 
			// toolStripMenuItem_MainMenu_Script_Panel
			// 
			this.toolStripMenuItem_MainMenu_Script_Panel.Name = "toolStripMenuItem_MainMenu_Script_Panel";
			this.toolStripMenuItem_MainMenu_Script_Panel.Size = new System.Drawing.Size(169, 22);
			this.toolStripMenuItem_MainMenu_Script_Panel.Text = "&Show Script Panel";
			this.toolStripMenuItem_MainMenu_Script_Panel.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Script_Panel_Click);
		#endif // WITH_SCRIPTING
			// 
			// toolStripMenuItem_MainMenu_Help
			// 
			this.toolStripMenuItem_MainMenu_Help.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MainMenu_Help_Contents,
            this.toolStripMenuItem_MainMenu_Help_Separator_1,
		#if !(WITH_SCRIPTING)
            this.toolStripMenuItem_MainMenu_Help_ReleaseNotes,
            this.toolStripMenuItem_MainMenu_Help_Separator_2,
            this.toolStripMenuItem_MainMenu_Help_RequestSupport,
            this.toolStripMenuItem_MainMenu_Help_RequestFeature,
            this.toolStripMenuItem_MainMenu_Help_SubmitBug,
            this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback,
            this.toolStripMenuItem_MainMenu_Help_Separator_3,
            this.toolStripMenuItem_MainMenu_Help_Update,
            this.toolStripMenuItem_MainMenu_Help_Separator_4,
            this.toolStripMenuItem_MainMenu_Help_Donate,
            this.toolStripMenuItem_MainMenu_Help_Separator_5,
		#endif // WITH_SCRIPTING
            this.toolStripMenuItem_MainMenu_Help_About});
			this.toolStripMenuItem_MainMenu_Help.MergeIndex = 8;
			this.toolStripMenuItem_MainMenu_Help.Name = "toolStripMenuItem_MainMenu_Help";
			this.toolStripMenuItem_MainMenu_Help.Size = new System.Drawing.Size(44, 20);
			this.toolStripMenuItem_MainMenu_Help.Text = "&Help";
			// 
			// toolStripMenuItem_MainMenu_Help_Contents
			// 
			this.toolStripMenuItem_MainMenu_Help_Contents.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_help_16x16;
			this.toolStripMenuItem_MainMenu_Help_Contents.Name = "toolStripMenuItem_MainMenu_Help_Contents";
			this.toolStripMenuItem_MainMenu_Help_Contents.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.toolStripMenuItem_MainMenu_Help_Contents.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_Contents.Text = "&Contents...";
			this.toolStripMenuItem_MainMenu_Help_Contents.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_Contents_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_Separator_1
			// 
			this.toolStripMenuItem_MainMenu_Help_Separator_1.Name = "toolStripMenuItem_MainMenu_Help_Separator_1";
			this.toolStripMenuItem_MainMenu_Help_Separator_1.Size = new System.Drawing.Size(178, 6);
		#if !(WITH_SCRIPTING)
			// 
			// toolStripMenuItem_MainMenu_Help_ReleaseNotes
			// 
			this.toolStripMenuItem_MainMenu_Help_ReleaseNotes.Name = "toolStripMenuItem_MainMenu_Help_ReleaseNotes";
			this.toolStripMenuItem_MainMenu_Help_ReleaseNotes.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_ReleaseNotes.Text = "&Release Notes...";
			this.toolStripMenuItem_MainMenu_Help_ReleaseNotes.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_Separator_2
			// 
			this.toolStripMenuItem_MainMenu_Help_Separator_2.Name = "toolStripMenuItem_MainMenu_Help_Separator_2";
			this.toolStripMenuItem_MainMenu_Help_Separator_2.Size = new System.Drawing.Size(178, 6);
			// 
			// toolStripMenuItem_MainMenu_Help_RequestSupport
			// 
			this.toolStripMenuItem_MainMenu_Help_RequestSupport.Name = "toolStripMenuItem_MainMenu_Help_RequestSupport";
			this.toolStripMenuItem_MainMenu_Help_RequestSupport.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_RequestSupport.Text = "Request &Support...";
			this.toolStripMenuItem_MainMenu_Help_RequestSupport.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_RequestSupport_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_RequestFeature
			// 
			this.toolStripMenuItem_MainMenu_Help_RequestFeature.Name = "toolStripMenuItem_MainMenu_Help_RequestFeature";
			this.toolStripMenuItem_MainMenu_Help_RequestFeature.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_RequestFeature.Text = "Request &Feature...";
			this.toolStripMenuItem_MainMenu_Help_RequestFeature.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_RequestFeature_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_SubmitBug
			// 
			this.toolStripMenuItem_MainMenu_Help_SubmitBug.Name = "toolStripMenuItem_MainMenu_Help_SubmitBug";
			this.toolStripMenuItem_MainMenu_Help_SubmitBug.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_SubmitBug.Text = "Submit &Bug...";
			this.toolStripMenuItem_MainMenu_Help_SubmitBug.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_SubmitBug_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_AnyOtherFeedback
			// 
			this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback.Name = "toolStripMenuItem_MainMenu_Help_AnyOtherFeedback";
			this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback.Text = "Any Other &Feedback...";
			this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_AnyOtherFeedback_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_Separator_3
			// 
			this.toolStripMenuItem_MainMenu_Help_Separator_3.Name = "toolStripMenuItem_MainMenu_Help_Separator_3";
			this.toolStripMenuItem_MainMenu_Help_Separator_3.Size = new System.Drawing.Size(178, 6);
			// 
			// toolStripMenuItem_MainMenu_Help_Update
			// 
			this.toolStripMenuItem_MainMenu_Help_Update.Name = "toolStripMenuItem_MainMenu_Help_Update";
			this.toolStripMenuItem_MainMenu_Help_Update.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_Update.Text = "&Update...";
			this.toolStripMenuItem_MainMenu_Help_Update.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_Update_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_Separator_4
			// 
			this.toolStripMenuItem_MainMenu_Help_Separator_4.Name = "toolStripMenuItem_MainMenu_Help_Separator_4";
			this.toolStripMenuItem_MainMenu_Help_Separator_4.Size = new System.Drawing.Size(178, 6);
			// 
			// toolStripMenuItem_MainMenu_Help_Donate
			// 
			this.toolStripMenuItem_MainMenu_Help_Donate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.toolStripMenuItem_MainMenu_Help_Donate.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.toolStripMenuItem_MainMenu_Help_Donate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_gift_add_16x16;
			this.toolStripMenuItem_MainMenu_Help_Donate.Name = "toolStripMenuItem_MainMenu_Help_Donate";
			this.toolStripMenuItem_MainMenu_Help_Donate.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_Donate.Text = "&Donate... Thanks!";
			this.toolStripMenuItem_MainMenu_Help_Donate.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_Donate_Click);
			// 
			// toolStripMenuItem_MainMenu_Help_Separator_5
			// 
			this.toolStripMenuItem_MainMenu_Help_Separator_5.Name = "toolStripMenuItem_MainMenu_Help_Separator_5";
			this.toolStripMenuItem_MainMenu_Help_Separator_5.Size = new System.Drawing.Size(178, 6);
		#endif // WITH_SCRIPTING
			// 
			// toolStripMenuItem_MainMenu_Help_About
			// 
		#if !(WITH_SCRIPTING)
			this.toolStripMenuItem_MainMenu_Help_About.Image = global::YAT.View.Forms.Properties.Resources.Image_YAT_16x16;
		#else
			this.toolStripMenuItem_MainMenu_Help_About.Image = global::YAT.View.Forms.Properties.Resources.Image_Albatros_64x64;
		#endif
			this.toolStripMenuItem_MainMenu_Help_About.Name = "toolStripMenuItem_MainMenu_Help_About";
			this.toolStripMenuItem_MainMenu_Help_About.Size = new System.Drawing.Size(181, 22);
			this.toolStripMenuItem_MainMenu_Help_About.Text = "&About...";
			this.toolStripMenuItem_MainMenu_Help_About.Click += new System.EventHandler(this.toolStripMenuItem_MainMenu_Help_About_Click);
			// 
			// statusStrip_Main
			// 
			this.statusStrip_Main.ContextMenuStrip = this.contextMenuStrip_Status;
			this.statusStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_MainStatus_Status,
            this.toolStripStatusLabel_MainStatus_TerminalInfo,
            this.toolStripStatusLabel_MainStatus_Time,
            this.toolStripStatusLabel_MainStatus_Chrono});
			this.statusStrip_Main.Location = new System.Drawing.Point(0, 621);
			this.statusStrip_Main.Name = "statusStrip_Main";
			this.statusStrip_Main.ShowItemToolTips = true;
			this.statusStrip_Main.Size = new System.Drawing.Size(896, 24);
			this.statusStrip_Main.TabIndex = 1;
			// 
			// contextMenuStrip_Status
			// 
			this.contextMenuStrip_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo,
            this.toolStripMenuItem_StatusContextMenu_ShowTime,
            this.toolStripMenuItem_StatusContextMenu_ShowChrono,
            this.toolStripMenuItem_StatusContextMenu_Separator_1,
            this.toolStripMenuItem_StatusContextMenu_Preferences});
			this.contextMenuStrip_Status.Name = "contextMenuStrip_Status";
			this.contextMenuStrip_Status.Size = new System.Drawing.Size(178, 98);
			this.contextMenuStrip_Status.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Status_Opening);
			// 
			// toolStripMenuItem_StatusContextMenu_ShowTerminalInfo
			// 
			this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Name = "toolStripMenuItem_StatusContextMenu_ShowTerminalInfo";
			this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Text = "Show Terminal IDs";
			this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_ShowTerminalInfo_Click);
			// 
			// toolStripMenuItem_StatusContextMenu_ShowTime
			// 
			this.toolStripMenuItem_StatusContextMenu_ShowTime.Checked = true;
			this.toolStripMenuItem_StatusContextMenu_ShowTime.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_StatusContextMenu_ShowTime.Name = "toolStripMenuItem_StatusContextMenu_ShowTime";
			this.toolStripMenuItem_StatusContextMenu_ShowTime.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_StatusContextMenu_ShowTime.Text = "Show Local Time";
			this.toolStripMenuItem_StatusContextMenu_ShowTime.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_ShowTime_Click);
			// 
			// toolStripMenuItem_StatusContextMenu_ShowChrono
			// 
			this.toolStripMenuItem_StatusContextMenu_ShowChrono.Checked = true;
			this.toolStripMenuItem_StatusContextMenu_ShowChrono.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_StatusContextMenu_ShowChrono.Name = "toolStripMenuItem_StatusContextMenu_ShowChrono";
			this.toolStripMenuItem_StatusContextMenu_ShowChrono.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_StatusContextMenu_ShowChrono.Text = "Show Chronometer";
			this.toolStripMenuItem_StatusContextMenu_ShowChrono.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_ShowChrono_Click);
			// 
			// toolStripMenuItem_StatusContextMenu_Separator_1
			// 
			this.toolStripMenuItem_StatusContextMenu_Separator_1.Name = "toolStripMenuItem_StatusContextMenu_Separator_1";
			this.toolStripMenuItem_StatusContextMenu_Separator_1.Size = new System.Drawing.Size(174, 6);
			// 
			// toolStripMenuItem_StatusContextMenu_Preferences
			// 
			this.toolStripMenuItem_StatusContextMenu_Preferences.Name = "toolStripMenuItem_StatusContextMenu_Preferences";
			this.toolStripMenuItem_StatusContextMenu_Preferences.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_StatusContextMenu_Preferences.Text = "Preferences...";
			this.toolStripMenuItem_StatusContextMenu_Preferences.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_Preferences_Click);
			// 
			// toolStripStatusLabel_MainStatus_Status
			// 
			this.toolStripStatusLabel_MainStatus_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabel_MainStatus_Status.Name = "toolStripStatusLabel_MainStatus_Status";
			this.toolStripStatusLabel_MainStatus_Status.Size = new System.Drawing.Size(775, 19);
			this.toolStripStatusLabel_MainStatus_Status.Spring = true;
			this.toolStripStatusLabel_MainStatus_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_MainStatus_Status.ToolTipText = "Program Status";
			// 
			// toolStripStatusLabel_MainStatus_TerminalInfo
			// 
			this.toolStripStatusLabel_MainStatus_TerminalInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabel_MainStatus_TerminalInfo.Name = "toolStripStatusLabel_MainStatus_TerminalInfo";
			this.toolStripStatusLabel_MainStatus_TerminalInfo.Size = new System.Drawing.Size(0, 19);
			this.toolStripStatusLabel_MainStatus_TerminalInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_MainStatus_TerminalInfo.ToolTipText = "Active Terminal (Auto Name / Sequential ID / Dynamic ID / Fixed ID)";
			// 
			// toolStripStatusLabel_MainStatus_Time
			// 
			this.toolStripStatusLabel_MainStatus_Time.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_MainStatus_Time.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_MainStatus_Time.Name = "toolStripStatusLabel_MainStatus_Time";
			this.toolStripStatusLabel_MainStatus_Time.Size = new System.Drawing.Size(53, 19);
			this.toolStripStatusLabel_MainStatus_Time.Text = "23:59:59";
			this.toolStripStatusLabel_MainStatus_Time.ToolTipText = "Local Time";
			// 
			// toolStripStatusLabel_MainStatus_Chrono
			// 
			this.toolStripStatusLabel_MainStatus_Chrono.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_MainStatus_Chrono.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_MainStatus_Chrono.DoubleClickEnabled = true;
			this.toolStripStatusLabel_MainStatus_Chrono.Name = "toolStripStatusLabel_MainStatus_Chrono";
			this.toolStripStatusLabel_MainStatus_Chrono.Size = new System.Drawing.Size(53, 19);
			this.toolStripStatusLabel_MainStatus_Chrono.Text = "0:00.000";
			this.toolStripStatusLabel_MainStatus_Chrono.ToolTipText = "Chronometer (m:ss.ths),\r\nclick to Start/Stop,\r\ndouble-click to Reset";
			this.toolStripStatusLabel_MainStatus_Chrono.Click += new System.EventHandler(this.toolStripStatusLabel_MainStatus_Chrono_Click);
			this.toolStripStatusLabel_MainStatus_Chrono.DoubleClick += new System.EventHandler(this.toolStripStatusLabel_MainStatus_Chrono_DoubleClick);
			// 
			// toolStrip_Main
			// 
			this.toolStrip_Main.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_MainTool_File_New,
            this.toolStripButton_MainTool_File_Open,
            this.toolStripButton_MainTool_File_Save,
            this.toolStripButton_MainTool_File_SaveWorkspace,
            this.toolStripMenuItem_MainTool_Separator_1,
            this.toolStripButton_MainTool_Terminal_Settings,
            this.toolStripMenuItem_MainTool_Separator_2,
            this.toolStripButton_MainTool_Terminal_Start,
            this.toolStripButton_MainTool_Terminal_Stop,
            this.toolStripMenuItem_MainTool_Separator_3,
            this.toolStripButton_MainTool_Radix_String,
            this.toolStripButton_MainTool_Radix_Char,
            this.toolStripButton_MainTool_Radix_Bin,
            this.toolStripButton_MainTool_Radix_Oct,
            this.toolStripButton_MainTool_Radix_Dec,
            this.toolStripButton_MainTool_Radix_Hex,
            this.toolStripButton_MainTool_Radix_Unicode,
            this.toolStripMenuItem_MainTool_Separator_4,
            this.toolStripButton_MainTool_Terminal_Clear,
            this.toolStripButton_MainTool_Terminal_Refresh,
            this.toolStripButton_MainTool_Terminal_CopyToClipboard,
            this.toolStripButton_MainTool_Terminal_SaveToFile,
            this.toolStripButton_MainTool_Terminal_Print,
            this.toolStripMenuItem_MainTool_Separator_5,
            this.toolStripButton_MainTool_Find_ShowHide,
            this.toolStripComboBox_MainTool_Find_Pattern,
            this.toolStripButton_MainTool_Find_CaseSensitive,
            this.toolStripButton_MainTool_Find_WholeWord,
            this.toolStripButton_MainTool_Find_EnableRegex,
            this.toolStripButton_MainTool_Find_Next,
            this.toolStripButton_MainTool_Find_Previous,
            this.toolStripMenuItem_MainTool_Separator_6,
            this.toolStripButton_MainTool_Log_Settings,
            this.toolStripButton_MainTool_Log_On,
            this.toolStripButton_MainTool_Log_Off,
            this.toolStripButton_MainTool_Log_OpenFile,
            this.toolStripButton_MainTool_Log_OpenDirectory,
            this.toolStripMenuItem_MainTool_Separator_7,
            this.toolStripButton_MainTool_AutoAction_ShowHide,
            this.toolStripComboBox_MainTool_AutoAction_Trigger,
            this.toolStripButton_MainTool_AutoAction_Trigger_UseText,
            this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive,
            this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord,
            this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex,
            this.toolStripComboBox_MainTool_AutoAction_Action,
            this.toolStripLabel_MainTool_AutoAction_Count,
            this.toolStripButton_MainTool_AutoAction_Deactivate,
            this.toolStripMenuItem_MainTool_Separator_8,
            this.toolStripButton_MainTool_AutoResponse_ShowHide,
            this.toolStripComboBox_MainTool_AutoResponse_Trigger,
            this.toolStripButton_MainTool_AutoResponse_Trigger_UseText,
            this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive,
            this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord,
            this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex,
            this.toolStripComboBox_MainTool_AutoResponse_Response,
            this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace,
            this.toolStripLabel_MainTool_AutoResponse_Count,
            this.toolStripButton_MainTool_AutoResponse_Deactivate,
            this.toolStripMenuItem_MainTool_Separator_9,
		#if !(WITH_SCRIPTING)
            this.toolStripButton_MainTool_Terminal_Format});
		#else
            this.toolStripButton_MainTool_Terminal_Format,
            this.toolStripMenuItem_MainTool_Separator_10,
            this.toolStripButton_MainTool_Script_ShowHide});
		#endif
			this.toolStrip_Main.Location = new System.Drawing.Point(3, 0);
			this.toolStrip_Main.Name = "toolStrip_Main";
		#if !(WITH_SCRIPTING)
			this.toolStrip_Main.Size = new System.Drawing.Size(723, 25);
		#else
			this.toolStrip_Main.Size = new System.Drawing.Size(752, 25);
		#endif
			this.toolStrip_Main.TabIndex = 0;
			// 
			// toolStripButton_MainTool_File_New
			// 
			this.toolStripButton_MainTool_File_New.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_File_New.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_add_16x16;
			this.toolStripButton_MainTool_File_New.Name = "toolStripButton_MainTool_File_New";
			this.toolStripButton_MainTool_File_New.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_File_New.Text = "New Terminal...";
			this.toolStripButton_MainTool_File_New.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolStripButton_MainTool_File_New.Click += new System.EventHandler(this.toolStripButton_MainTool_File_New_Click);
			// 
			// toolStripButton_MainTool_File_Open
			// 
			this.toolStripButton_MainTool_File_Open.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_File_Open.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_folder_add_16x16;
			this.toolStripButton_MainTool_File_Open.Name = "toolStripButton_MainTool_File_Open";
			this.toolStripButton_MainTool_File_Open.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_File_Open.Text = "Open File...";
			this.toolStripButton_MainTool_File_Open.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolStripButton_MainTool_File_Open.Click += new System.EventHandler(this.toolStripButton_MainTool_File_Open_Click);
			// 
			// toolStripButton_MainTool_File_Save
			// 
			this.toolStripButton_MainTool_File_Save.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_File_Save.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_save_16x16;
			this.toolStripButton_MainTool_File_Save.Name = "toolStripButton_MainTool_File_Save";
			this.toolStripButton_MainTool_File_Save.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_File_Save.Text = "Save Terminal";
			this.toolStripButton_MainTool_File_Save.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolStripButton_MainTool_File_Save.Click += new System.EventHandler(this.toolStripButton_MainTool_File_Save_Click);
			// 
			// toolStripButton_MainTool_File_SaveWorkspace
			// 
			this.toolStripButton_MainTool_File_SaveWorkspace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_File_SaveWorkspace.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_layer_save_16x16;
			this.toolStripButton_MainTool_File_SaveWorkspace.Name = "toolStripButton_MainTool_File_SaveWorkspace";
			this.toolStripButton_MainTool_File_SaveWorkspace.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_File_SaveWorkspace.Text = "Save Workspace";
			this.toolStripButton_MainTool_File_SaveWorkspace.Click += new System.EventHandler(this.toolStripButton_MainTool_File_SaveWorkspace_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_1
			// 
			this.toolStripMenuItem_MainTool_Separator_1.Name = "toolStripMenuItem_MainTool_Separator_1";
			this.toolStripMenuItem_MainTool_Separator_1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Terminal_Settings
			// 
			this.toolStripButton_MainTool_Terminal_Settings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Settings.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_edit_16x16;
			this.toolStripButton_MainTool_Terminal_Settings.Name = "toolStripButton_MainTool_Terminal_Settings";
			this.toolStripButton_MainTool_Terminal_Settings.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Settings.Text = "Terminal Settings...";
			this.toolStripButton_MainTool_Terminal_Settings.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolStripButton_MainTool_Terminal_Settings.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Settings_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_2
			// 
			this.toolStripMenuItem_MainTool_Separator_2.Name = "toolStripMenuItem_MainTool_Separator_2";
			this.toolStripMenuItem_MainTool_Separator_2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Terminal_Start
			// 
			this.toolStripButton_MainTool_Terminal_Start.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Start.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_accept_button_16x16;
			this.toolStripButton_MainTool_Terminal_Start.Name = "toolStripButton_MainTool_Terminal_Start";
			this.toolStripButton_MainTool_Terminal_Start.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Start.Text = "Open/Start Terminal";
			this.toolStripButton_MainTool_Terminal_Start.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolStripButton_MainTool_Terminal_Start.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Start_Click);
			// 
			// toolStripButton_MainTool_Terminal_Stop
			// 
			this.toolStripButton_MainTool_Terminal_Stop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Stop.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_delete_16x16;
			this.toolStripButton_MainTool_Terminal_Stop.Name = "toolStripButton_MainTool_Terminal_Stop";
			this.toolStripButton_MainTool_Terminal_Stop.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Stop.Text = "Close/Stop Terminal";
			this.toolStripButton_MainTool_Terminal_Stop.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.toolStripButton_MainTool_Terminal_Stop.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Stop_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_3
			// 
			this.toolStripMenuItem_MainTool_Separator_3.Name = "toolStripMenuItem_MainTool_Separator_3";
			this.toolStripMenuItem_MainTool_Separator_3.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Radix_String
			// 
			this.toolStripButton_MainTool_Radix_String.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_String.Name = "toolStripButton_MainTool_Radix_String";
			this.toolStripButton_MainTool_Radix_String.Size = new System.Drawing.Size(25, 22);
			this.toolStripButton_MainTool_Radix_String.Text = "Str";
			this.toolStripButton_MainTool_Radix_String.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_String.ToolTipText = "Radix: String";
			this.toolStripButton_MainTool_Radix_String.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_String_Click);
			// 
			// toolStripButton_MainTool_Radix_Char
			// 
			this.toolStripButton_MainTool_Radix_Char.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_Char.Name = "toolStripButton_MainTool_Radix_Char";
			this.toolStripButton_MainTool_Radix_Char.Size = new System.Drawing.Size(30, 22);
			this.toolStripButton_MainTool_Radix_Char.Text = "Chr";
			this.toolStripButton_MainTool_Radix_Char.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_Char.ToolTipText = "Radix: Character";
			this.toolStripButton_MainTool_Radix_Char.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_Char_Click);
			// 
			// toolStripButton_MainTool_Radix_Bin
			// 
			this.toolStripButton_MainTool_Radix_Bin.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_Bin.Name = "toolStripButton_MainTool_Radix_Bin";
			this.toolStripButton_MainTool_Radix_Bin.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Radix_Bin.Text = "2";
			this.toolStripButton_MainTool_Radix_Bin.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_Bin.ToolTipText = "Radix: Binary";
			this.toolStripButton_MainTool_Radix_Bin.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_Bin_Click);
			// 
			// toolStripButton_MainTool_Radix_Oct
			// 
			this.toolStripButton_MainTool_Radix_Oct.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_Oct.Name = "toolStripButton_MainTool_Radix_Oct";
			this.toolStripButton_MainTool_Radix_Oct.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Radix_Oct.Text = "8";
			this.toolStripButton_MainTool_Radix_Oct.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_Oct.ToolTipText = "Radix: Octal";
			this.toolStripButton_MainTool_Radix_Oct.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_Oct_Click);
			// 
			// toolStripButton_MainTool_Radix_Dec
			// 
			this.toolStripButton_MainTool_Radix_Dec.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_Dec.Name = "toolStripButton_MainTool_Radix_Dec";
			this.toolStripButton_MainTool_Radix_Dec.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Radix_Dec.Text = "10";
			this.toolStripButton_MainTool_Radix_Dec.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_Dec.ToolTipText = "Radix: Decimal";
			this.toolStripButton_MainTool_Radix_Dec.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_Dec_Click);
			// 
			// toolStripButton_MainTool_Radix_Hex
			// 
			this.toolStripButton_MainTool_Radix_Hex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_Hex.Name = "toolStripButton_MainTool_Radix_Hex";
			this.toolStripButton_MainTool_Radix_Hex.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Radix_Hex.Text = "16";
			this.toolStripButton_MainTool_Radix_Hex.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_Hex.ToolTipText = "Radix: Hexadecimal";
			this.toolStripButton_MainTool_Radix_Hex.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_Hex_Click);
			// 
			// toolStripButton_MainTool_Radix_Unicode
			// 
			this.toolStripButton_MainTool_Radix_Unicode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Radix_Unicode.Name = "toolStripButton_MainTool_Radix_Unicode";
			this.toolStripButton_MainTool_Radix_Unicode.Size = new System.Drawing.Size(27, 22);
			this.toolStripButton_MainTool_Radix_Unicode.Text = "U+";
			this.toolStripButton_MainTool_Radix_Unicode.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.toolStripButton_MainTool_Radix_Unicode.ToolTipText = "Radix: Unicode";
			this.toolStripButton_MainTool_Radix_Unicode.Click += new System.EventHandler(this.toolStripButton_MainTool_Radix_Unicode_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_4
			// 
			this.toolStripMenuItem_MainTool_Separator_4.Name = "toolStripMenuItem_MainTool_Separator_4";
			this.toolStripMenuItem_MainTool_Separator_4.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Terminal_Clear
			// 
			this.toolStripButton_MainTool_Terminal_Clear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Clear.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_lightning_16x16;
			this.toolStripButton_MainTool_Terminal_Clear.Name = "toolStripButton_MainTool_Terminal_Clear";
			this.toolStripButton_MainTool_Terminal_Clear.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Clear.Text = "Clear Terminal Monitor";
			this.toolStripButton_MainTool_Terminal_Clear.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Clear_Click);
			// 
			// toolStripButton_MainTool_Terminal_Refresh
			// 
			this.toolStripButton_MainTool_Terminal_Refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Refresh.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_refresh_16x16;
			this.toolStripButton_MainTool_Terminal_Refresh.Name = "toolStripButton_MainTool_Terminal_Refresh";
			this.toolStripButton_MainTool_Terminal_Refresh.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Refresh.Text = "Refresh Terminal Monitor";
			this.toolStripButton_MainTool_Terminal_Refresh.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Refresh_Click);
			// 
			// toolStripButton_MainTool_Terminal_CopyToClipboard
			// 
			this.toolStripButton_MainTool_Terminal_CopyToClipboard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_CopyToClipboard.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_text_exports_16x16;
			this.toolStripButton_MainTool_Terminal_CopyToClipboard.Name = "toolStripButton_MainTool_Terminal_CopyToClipboard";
			this.toolStripButton_MainTool_Terminal_CopyToClipboard.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_CopyToClipboard.Text = "Copy to Clipboard";
			this.toolStripButton_MainTool_Terminal_CopyToClipboard.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_CopyToClipboard_Click);
			// 
			// toolStripButton_MainTool_Terminal_SaveToFile
			// 
			this.toolStripButton_MainTool_Terminal_SaveToFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_SaveToFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_save_16x16;
			this.toolStripButton_MainTool_Terminal_SaveToFile.Name = "toolStripButton_MainTool_Terminal_SaveToFile";
			this.toolStripButton_MainTool_Terminal_SaveToFile.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_SaveToFile.Text = "Save to File...";
			this.toolStripButton_MainTool_Terminal_SaveToFile.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_SaveToFile_Click);
			// 
			// toolStripButton_MainTool_Terminal_Print
			// 
			this.toolStripButton_MainTool_Terminal_Print.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Print.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_printer_16x16;
			this.toolStripButton_MainTool_Terminal_Print.Name = "toolStripButton_MainTool_Terminal_Print";
			this.toolStripButton_MainTool_Terminal_Print.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Print.Text = "Print...";
			this.toolStripButton_MainTool_Terminal_Print.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Print_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_5
			// 
			this.toolStripMenuItem_MainTool_Separator_5.Name = "toolStripMenuItem_MainTool_Separator_5";
			this.toolStripMenuItem_MainTool_Separator_5.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Find_ShowHide
			// 
			this.toolStripButton_MainTool_Find_ShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Find_ShowHide.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_tab_search_16x16;
			this.toolStripButton_MainTool_Find_ShowHide.Name = "toolStripButton_MainTool_Find_ShowHide";
			this.toolStripButton_MainTool_Find_ShowHide.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Find_ShowHide.Text = "Find...";
			this.toolStripButton_MainTool_Find_ShowHide.Click += new System.EventHandler(this.toolStripButton_MainTool_Find_ShowHide_Click);
			// 
			// toolStripComboBox_MainTool_Find_Pattern
			// 
			this.toolStripComboBox_MainTool_Find_Pattern.Name = "toolStripComboBox_MainTool_Find_Pattern";
			this.toolStripComboBox_MainTool_Find_Pattern.Size = new System.Drawing.Size(160, 27);
			this.toolStripComboBox_MainTool_Find_Pattern.ToolTipText = resources.GetString("toolStripComboBox_MainTool_Find_Pattern.ToolTipText");
			this.toolStripComboBox_MainTool_Find_Pattern.Visible = false;
			this.toolStripComboBox_MainTool_Find_Pattern.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MainTool_Find_Pattern_SelectedIndexChanged);
			this.toolStripComboBox_MainTool_Find_Pattern.Enter += new System.EventHandler(this.toolStripComboBox_MainTool_Find_Pattern_Enter);
			this.toolStripComboBox_MainTool_Find_Pattern.Leave += new System.EventHandler(this.toolStripComboBox_MainTool_Find_Pattern_Leave);
			this.toolStripComboBox_MainTool_Find_Pattern.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_Find_Pattern_KeyDown);
			this.toolStripComboBox_MainTool_Find_Pattern.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_Find_Pattern_KeyUp);
			this.toolStripComboBox_MainTool_Find_Pattern.TextChanged += new System.EventHandler(this.toolStripComboBox_MainTool_Find_Pattern_TextChanged);
			// 
			// toolStripButton_MainTool_Find_CaseSensitive
			// 
			this.toolStripButton_MainTool_Find_CaseSensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Find_CaseSensitive.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_match_character_literally_16x16;
			this.toolStripButton_MainTool_Find_CaseSensitive.Name = "toolStripButton_MainTool_Find_CaseSensitive";
			this.toolStripButton_MainTool_Find_CaseSensitive.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_Find_CaseSensitive.ToolTipText = "Find Case Sensitive\r\nSame as \"(?-i)\" when using regex.\r\n[Alt+C] (while in find)";
			this.toolStripButton_MainTool_Find_CaseSensitive.Visible = false;
			this.toolStripButton_MainTool_Find_CaseSensitive.Click += new System.EventHandler(this.toolStripButton_MainTool_Find_CaseSensitive_Click);
			// 
			// toolStripButton_MainTool_Find_WholeWord
			// 
			this.toolStripButton_MainTool_Find_WholeWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Find_WholeWord.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_literal_text_16x16;
			this.toolStripButton_MainTool_Find_WholeWord.Name = "toolStripButton_MainTool_Find_WholeWord";
			this.toolStripButton_MainTool_Find_WholeWord.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_Find_WholeWord.ToolTipText = "Find Whole Word\r\nSame as \"\\bSomeWord\\b\" when using regex.\r\n[Alt+W] (while in find" +
    ")";
			this.toolStripButton_MainTool_Find_WholeWord.Visible = false;
			this.toolStripButton_MainTool_Find_WholeWord.Click += new System.EventHandler(this.toolStripButton_MainTool_Find_WholeWord_Click);
			// 
			// toolStripButton_MainTool_Find_EnableRegex
			// 
			this.toolStripButton_MainTool_Find_EnableRegex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_Find_EnableRegex.Name = "toolStripButton_MainTool_Find_EnableRegex";
			this.toolStripButton_MainTool_Find_EnableRegex.Size = new System.Drawing.Size(27, 24);
			this.toolStripButton_MainTool_Find_EnableRegex.Text = "(.*)";
			this.toolStripButton_MainTool_Find_EnableRegex.ToolTipText = "Enable Regular Expression\r\n[Alt+E] (while in find)\r\n\r\nGoogle for \".NET Regular Ex" +
    "pression Quick Reference\" for syntax.";
			this.toolStripButton_MainTool_Find_EnableRegex.Visible = false;
			this.toolStripButton_MainTool_Find_EnableRegex.Click += new System.EventHandler(this.toolStripButton_MainTool_Find_EnableRegex_Click);
			// 
			// toolStripButton_MainTool_Find_Next
			// 
			this.toolStripButton_MainTool_Find_Next.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Find_Next.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_rows_insert_below_word_16x16;
			this.toolStripButton_MainTool_Find_Next.Name = "toolStripButton_MainTool_Find_Next";
			this.toolStripButton_MainTool_Find_Next.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_Find_Next.Text = "Find Next";
			this.toolStripButton_MainTool_Find_Next.ToolTipText = "Find Next\r\n[Enter] / [Ctrl+F/N] / [Alt+F/N] (while in find)\r\n[Alt+Shift+N] (alway" +
    "s)";
			this.toolStripButton_MainTool_Find_Next.Visible = false;
			this.toolStripButton_MainTool_Find_Next.Click += new System.EventHandler(this.toolStripButton_MainTool_Find_Next_Click);
			// 
			// toolStripButton_MainTool_Find_Previous
			// 
			this.toolStripButton_MainTool_Find_Previous.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Find_Previous.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_rows_insert_above_word_16x16;
			this.toolStripButton_MainTool_Find_Previous.Name = "toolStripButton_MainTool_Find_Previous";
			this.toolStripButton_MainTool_Find_Previous.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_Find_Previous.Text = "Find Previous";
			this.toolStripButton_MainTool_Find_Previous.ToolTipText = "Find Previous\r\n[Ctrl+P] / [Alt+P] (while in find)\r\n[Alt+Shift+P] (always)";
			this.toolStripButton_MainTool_Find_Previous.Visible = false;
			this.toolStripButton_MainTool_Find_Previous.Click += new System.EventHandler(this.toolStripButton_MainTool_Find_Previous_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_6
			// 
			this.toolStripMenuItem_MainTool_Separator_6.Name = "toolStripMenuItem_MainTool_Separator_6";
			this.toolStripMenuItem_MainTool_Separator_6.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Log_Settings
			// 
			this.toolStripButton_MainTool_Log_Settings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Log_Settings.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_edit_16x16;
			this.toolStripButton_MainTool_Log_Settings.Name = "toolStripButton_MainTool_Log_Settings";
			this.toolStripButton_MainTool_Log_Settings.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Log_Settings.Text = "Log Settings...";
			this.toolStripButton_MainTool_Log_Settings.Click += new System.EventHandler(this.toolStripButton_MainTool_Log_Settings_Click);
			// 
			// toolStripButton_MainTool_Log_On
			// 
			this.toolStripButton_MainTool_Log_On.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Log_On.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_accept_document_16x16;
			this.toolStripButton_MainTool_Log_On.Name = "toolStripButton_MainTool_Log_On";
			this.toolStripButton_MainTool_Log_On.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Log_On.Text = "Switch Log On";
			this.toolStripButton_MainTool_Log_On.Click += new System.EventHandler(this.toolStripButton_MainTool_Log_On_Click);
			// 
			// toolStripButton_MainTool_Log_Off
			// 
			this.toolStripButton_MainTool_Log_Off.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Log_Off.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_delete_16x16;
			this.toolStripButton_MainTool_Log_Off.Name = "toolStripButton_MainTool_Log_Off";
			this.toolStripButton_MainTool_Log_Off.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Log_Off.Text = "Switch Log Off";
			this.toolStripButton_MainTool_Log_Off.Click += new System.EventHandler(this.toolStripButton_MainTool_Log_Off_Click);
			// 
			// toolStripButton_MainTool_Log_OpenFile
			// 
			this.toolStripButton_MainTool_Log_OpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Log_OpenFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_magnify_16x16;
			this.toolStripButton_MainTool_Log_OpenFile.Name = "toolStripButton_MainTool_Log_OpenFile";
			this.toolStripButton_MainTool_Log_OpenFile.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Log_OpenFile.Text = "Open Log File(s) in Editor...";
			this.toolStripButton_MainTool_Log_OpenFile.Click += new System.EventHandler(this.toolStripButton_MainTool_Log_Open_Click);
			// 
			// toolStripButton_MainTool_Log_OpenDirectory
			// 
			this.toolStripButton_MainTool_Log_OpenDirectory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Log_OpenDirectory.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_folder_explorer_16x16;
			this.toolStripButton_MainTool_Log_OpenDirectory.Name = "toolStripButton_MainTool_Log_OpenDirectory";
			this.toolStripButton_MainTool_Log_OpenDirectory.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Log_OpenDirectory.Text = "Open Log Folder in File Browser...";
			this.toolStripButton_MainTool_Log_OpenDirectory.Click += new System.EventHandler(this.toolStripButton_MainTool_Log_OpenDirectory_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_7
			// 
			this.toolStripMenuItem_MainTool_Separator_7.Name = "toolStripMenuItem_MainTool_Separator_7";
			this.toolStripMenuItem_MainTool_Separator_7.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_AutoAction_ShowHide
			// 
			this.toolStripButton_MainTool_AutoAction_ShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoAction_ShowHide.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_16x16;
			this.toolStripButton_MainTool_AutoAction_ShowHide.Name = "toolStripButton_MainTool_AutoAction_ShowHide";
			this.toolStripButton_MainTool_AutoAction_ShowHide.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_AutoAction_ShowHide.Text = "Show Automatic Action";
			this.toolStripButton_MainTool_AutoAction_ShowHide.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoAction_ShowHide_Click);
			// 
			// toolStripComboBox_MainTool_AutoAction_Trigger
			// 
			this.toolStripComboBox_MainTool_AutoAction_Trigger.Name = "toolStripComboBox_MainTool_AutoAction_Trigger";
			this.toolStripComboBox_MainTool_AutoAction_Trigger.Size = new System.Drawing.Size(160, 27);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.ToolTipText = resources.GetString("toolStripComboBox_MainTool_AutoAction_Trigger.ToolTipText");
			this.toolStripComboBox_MainTool_AutoAction_Trigger.Visible = false;
			this.toolStripComboBox_MainTool_AutoAction_Trigger.DropDown += new System.EventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_DropDown);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_SelectedIndexChanged);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.Enter += new System.EventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_Enter);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.Leave += new System.EventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_Leave);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_KeyDown);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_KeyUp);
			this.toolStripComboBox_MainTool_AutoAction_Trigger.TextChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoAction_Trigger_TextChanged);
			// 
			// toolStripButton_MainTool_AutoAction_Trigger_UseText
			// 
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_formatting_text_containing_16x16;
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.Name = "toolStripButton_MainTool_AutoAction_Trigger_UseText";
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.ToolTipText = "Use Text\r\nBy default, the trigger is based on the byte sequence of a command.\r\nTh" +
    "is option switches to a trigger based on text.\r\n[Alt+T] (while editing)";
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.Visible = false;
			this.toolStripButton_MainTool_AutoAction_Trigger_UseText.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoAction_Trigger_UseText_Click);
			// 
			// toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive
			// 
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_match_character_literally_16x16;
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Name = "toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive";
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.ToolTipText = "Case Sensitive\r\nSame as \"(?-i)\" when using regex.\r\n[Alt+C] (while editing)";
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Visible = false;
			this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive_Click);
			// 
			// toolStripButton_MainTool_AutoAction_Trigger_WholeWord
			// 
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_literal_text_16x16;
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Name = "toolStripButton_MainTool_AutoAction_Trigger_WholeWord";
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.ToolTipText = "Whole Word\r\nSame as \"\\bSomeWord\\b\" when using regex.\r\n[Alt+W] (while editing)";
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Visible = false;
			this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoAction_Trigger_WholeWord_Click);
			// 
			// toolStripButton_MainTool_AutoAction_Trigger_EnableRegex
			// 
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Name = "toolStripButton_MainTool_AutoAction_Trigger_EnableRegex";
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Size = new System.Drawing.Size(27, 24);
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Text = "(.*)";
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.ToolTipText = "Enable Regular Expression\r\n[Alt+E] (while editing)\r\n\r\nGoogle for \".NET Regular Ex" +
    "pression Quick Reference\" for syntax.";
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Visible = false;
			this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoAction_Trigger_EnableRegex_Click);
			// 
			// toolStripComboBox_MainTool_AutoAction_Action
			// 
			this.toolStripComboBox_MainTool_AutoAction_Action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_MainTool_AutoAction_Action.Name = "toolStripComboBox_MainTool_AutoAction_Action";
			this.toolStripComboBox_MainTool_AutoAction_Action.Size = new System.Drawing.Size(160, 27);
			this.toolStripComboBox_MainTool_AutoAction_Action.ToolTipText = resources.GetString("toolStripComboBox_MainTool_AutoAction_Action.ToolTipText");
			this.toolStripComboBox_MainTool_AutoAction_Action.Visible = false;
			this.toolStripComboBox_MainTool_AutoAction_Action.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoAction_Action_SelectedIndexChanged);
			// 
			// toolStripLabel_MainTool_AutoAction_Count
			// 
			this.toolStripLabel_MainTool_AutoAction_Count.Name = "toolStripLabel_MainTool_AutoAction_Count";
			this.toolStripLabel_MainTool_AutoAction_Count.Size = new System.Drawing.Size(21, 24);
			this.toolStripLabel_MainTool_AutoAction_Count.Text = "(0)";
			this.toolStripLabel_MainTool_AutoAction_Count.ToolTipText = "Automatic Action Count\r\nClick to Reset";
			this.toolStripLabel_MainTool_AutoAction_Count.Visible = false;
			this.toolStripLabel_MainTool_AutoAction_Count.Click += new System.EventHandler(this.toolStripLabel_MainTool_AutoAction_Count_Click);
			// 
			// toolStripButton_MainTool_AutoAction_Deactivate
			// 
			this.toolStripButton_MainTool_AutoAction_Deactivate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoAction_Deactivate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_delete_16x16;
			this.toolStripButton_MainTool_AutoAction_Deactivate.Name = "toolStripButton_MainTool_AutoAction_Deactivate";
			this.toolStripButton_MainTool_AutoAction_Deactivate.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoAction_Deactivate.Text = "Deactivate Automatic Action";
			this.toolStripButton_MainTool_AutoAction_Deactivate.Visible = false;
			this.toolStripButton_MainTool_AutoAction_Deactivate.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoAction_Deactivate_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_8
			// 
			this.toolStripMenuItem_MainTool_Separator_8.Name = "toolStripMenuItem_MainTool_Separator_8";
			this.toolStripMenuItem_MainTool_Separator_8.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_AutoResponse_ShowHide
			// 
			this.toolStripButton_MainTool_AutoResponse_ShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoResponse_ShowHide.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_autoresponders_16x16;
			this.toolStripButton_MainTool_AutoResponse_ShowHide.Name = "toolStripButton_MainTool_AutoResponse_ShowHide";
			this.toolStripButton_MainTool_AutoResponse_ShowHide.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_AutoResponse_ShowHide.Text = "Show Automatic Response";
			this.toolStripButton_MainTool_AutoResponse_ShowHide.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_ShowHide_Click);
			// 
			// toolStripComboBox_MainTool_AutoResponse_Trigger
			// 
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.Name = "toolStripComboBox_MainTool_AutoResponse_Trigger";
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.Size = new System.Drawing.Size(160, 27);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.ToolTipText = resources.GetString("toolStripComboBox_MainTool_AutoResponse_Trigger.ToolTipText");
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.Visible = false;
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.DropDown += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_DropDown);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_SelectedIndexChanged);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.Enter += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_Enter);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.Leave += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_Leave);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_KeyDown);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_KeyUp);
			this.toolStripComboBox_MainTool_AutoResponse_Trigger.TextChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Trigger_TextChanged);
			// 
			// toolStripButton_MainTool_AutoResponse_Trigger_UseText
			// 
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_formatting_text_containing_16x16;
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.Name = "toolStripButton_MainTool_AutoResponse_Trigger_UseText";
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.ToolTipText = "Use Text\r\nBy default, the trigger is based on the byte sequence of a command.\r\nTh" +
    "is option switches to a trigger based on text.\r\n[Alt+T] (while editing)";
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.Visible = false;
			this.toolStripButton_MainTool_AutoResponse_Trigger_UseText.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_Trigger_UseText_Click);
			// 
			// toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive
			// 
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_match_character_literally_16x16;
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Name = "toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive";
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.ToolTipText = "Case Sensitive\r\nSame as \"(?-i)\" when using regex.\r\n[Alt+C] (while editing)";
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Visible = false;
			this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive_Click);
			// 
			// toolStripButton_MainTool_AutoResponse_Trigger_WholeWord
			// 
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_literal_text_16x16;
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Name = "toolStripButton_MainTool_AutoResponse_Trigger_WholeWord";
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Size = new System.Drawing.Size(23, 24);
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.ToolTipText = "Whole Word\r\nSame as \"\\bSomeWord\\b\" when using regex.\r\n[Alt+W] (while editing)";
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Visible = false;
			this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_Trigger_WholeWord_Click);
			// 
			// toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex
			// 
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Name = "toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex";
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Size = new System.Drawing.Size(27, 24);
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Text = "(.*)";
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.ToolTipText = "Enable Regular Expression\r\n[Alt+E] (while editing)\r\n\r\nGoogle for \".NET Regular Ex" +
    "pression Quick Reference\" for syntax.";
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Visible = false;
			this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex_Click);
			// 
			// toolStripComboBox_MainTool_AutoResponse_Response
			// 
			this.toolStripComboBox_MainTool_AutoResponse_Response.Name = "toolStripComboBox_MainTool_AutoResponse_Response";
			this.toolStripComboBox_MainTool_AutoResponse_Response.Size = new System.Drawing.Size(160, 25);
			this.toolStripComboBox_MainTool_AutoResponse_Response.ToolTipText = "Enable / Disable Automatic Response,\r\neither refer to one of the commands,\r\nor fi" +
    "ll-in any text.";
			this.toolStripComboBox_MainTool_AutoResponse_Response.Visible = false;
			this.toolStripComboBox_MainTool_AutoResponse_Response.DropDown += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_DropDown);
			this.toolStripComboBox_MainTool_AutoResponse_Response.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_SelectedIndexChanged);
			this.toolStripComboBox_MainTool_AutoResponse_Response.Enter += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_Enter);
			this.toolStripComboBox_MainTool_AutoResponse_Response.Leave += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_Leave);
			this.toolStripComboBox_MainTool_AutoResponse_Response.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_KeyDown);
			this.toolStripComboBox_MainTool_AutoResponse_Response.KeyUp += new System.Windows.Forms.KeyEventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_KeyUp);
			this.toolStripComboBox_MainTool_AutoResponse_Response.TextChanged += new System.EventHandler(this.toolStripComboBox_MainTool_AutoResponse_Response_TextChanged);
			// 
			// toolStripButton_MainTool_AutoResponse_Response_EnableReplace
			// 
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Name = "toolStripButton_MainTool_AutoResponse_Response_EnableReplace";
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Text = "$i";
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.ToolTipText = resources.GetString("toolStripButton_MainTool_AutoResponse_Response_EnableReplace.ToolTipText");
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Visible = false;
			this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_Response_EnableReplace_Click);
			// 
			// toolStripLabel_MainTool_AutoResponse_Count
			// 
			this.toolStripLabel_MainTool_AutoResponse_Count.Name = "toolStripLabel_MainTool_AutoResponse_Count";
			this.toolStripLabel_MainTool_AutoResponse_Count.Size = new System.Drawing.Size(21, 22);
			this.toolStripLabel_MainTool_AutoResponse_Count.Text = "(0)";
			this.toolStripLabel_MainTool_AutoResponse_Count.ToolTipText = "Automatic Response Count\r\nClick to Reset";
			this.toolStripLabel_MainTool_AutoResponse_Count.Visible = false;
			this.toolStripLabel_MainTool_AutoResponse_Count.Click += new System.EventHandler(this.toolStripLabel_MainTool_AutoResponse_Count_Click);
			// 
			// toolStripButton_MainTool_AutoResponse_Deactivate
			// 
			this.toolStripButton_MainTool_AutoResponse_Deactivate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_AutoResponse_Deactivate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_email_delete_16x16;
			this.toolStripButton_MainTool_AutoResponse_Deactivate.Name = "toolStripButton_MainTool_AutoResponse_Deactivate";
			this.toolStripButton_MainTool_AutoResponse_Deactivate.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_AutoResponse_Deactivate.Text = "Deactivate Automatic Response";
			this.toolStripButton_MainTool_AutoResponse_Deactivate.Visible = false;
			this.toolStripButton_MainTool_AutoResponse_Deactivate.Click += new System.EventHandler(this.toolStripButton_MainTool_AutoResponse_Deactivate_Click);
			// 
			// toolStripMenuItem_MainTool_Separator_9
			// 
			this.toolStripMenuItem_MainTool_Separator_9.Name = "toolStripMenuItem_MainTool_Separator_9";
			this.toolStripMenuItem_MainTool_Separator_9.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Terminal_Format
			// 
			this.toolStripButton_MainTool_Terminal_Format.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Terminal_Format.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_font_16x16;
			this.toolStripButton_MainTool_Terminal_Format.Name = "toolStripButton_MainTool_Terminal_Format";
			this.toolStripButton_MainTool_Terminal_Format.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Terminal_Format.Text = "Format Settings...";
			this.toolStripButton_MainTool_Terminal_Format.Click += new System.EventHandler(this.toolStripButton_MainTool_Terminal_Format_Click);
		#if (WITH_SCRIPTING)
			// 
			// toolStripMenuItem_MainTool_Separator_10
			// 
			this.toolStripMenuItem_MainTool_Separator_10.Name = "toolStripMenuItem_MainTool_Separator_10";
			this.toolStripMenuItem_MainTool_Separator_10.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButton_MainTool_Script_ShowHide
			// 
			this.toolStripButton_MainTool_Script_ShowHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButton_MainTool_Script_ShowHide.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_script_16x16;
			this.toolStripButton_MainTool_Script_ShowHide.Name = "toolStripButton_MainTool_Script_ShowHide";
			this.toolStripButton_MainTool_Script_ShowHide.Size = new System.Drawing.Size(23, 22);
			this.toolStripButton_MainTool_Script_ShowHide.Text = "Show Script Panel";
			this.toolStripButton_MainTool_Script_ShowHide.Click += new System.EventHandler(this.toolStripButton_MainTool_Script_ShowHide_Click);
		#endif
			// 
			// toolStripPanel_Top
			// 
			this.toolStripPanel_Top.Controls.Add(this.toolStrip_Main);
			this.toolStripPanel_Top.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolStripPanel_Top.Location = new System.Drawing.Point(0, 24);
			this.toolStripPanel_Top.Name = "toolStripPanel_Top";
			this.toolStripPanel_Top.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.toolStripPanel_Top.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.toolStripPanel_Top.Size = new System.Drawing.Size(896, 25);
			// 
			// toolStripPanel_Right
			// 
			this.toolStripPanel_Right.Dock = System.Windows.Forms.DockStyle.Right;
			this.toolStripPanel_Right.Location = new System.Drawing.Point(896, 49);
			this.toolStripPanel_Right.Name = "toolStripPanel_Right";
			this.toolStripPanel_Right.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.toolStripPanel_Right.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.toolStripPanel_Right.Size = new System.Drawing.Size(0, 572);
			// 
			// toolStripPanel_Left
			// 
			this.toolStripPanel_Left.Dock = System.Windows.Forms.DockStyle.Left;
			this.toolStripPanel_Left.Location = new System.Drawing.Point(0, 49);
			this.toolStripPanel_Left.Name = "toolStripPanel_Left";
			this.toolStripPanel_Left.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.toolStripPanel_Left.RowMargin = new System.Windows.Forms.Padding(0, 3, 0, 0);
			this.toolStripPanel_Left.Size = new System.Drawing.Size(0, 572);
			// 
			// chronometer_Main
			// 
			this.chronometer_Main.TimeSpanChanged += new System.EventHandler<MKY.TimeSpanEventArgs>(this.chronometer_Main_TimeSpanChanged);
			// 
			// timer_Time
			// 
			this.timer_Time.Interval = 50;
			this.timer_Time.Tick += new System.EventHandler(this.timer_Time_Tick);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(896, 645);
			this.ContextMenuStrip = this.contextMenuStrip_Main;
			this.Controls.Add(this.toolStripPanel_Left);
			this.Controls.Add(this.toolStripPanel_Right);
			this.Controls.Add(this.toolStripPanel_Top);
			this.Controls.Add(this.menuStrip_Main);
			this.Controls.Add(this.statusStrip_Main);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menuStrip_Main;
			this.Name = "Main";
		#if !(WITH_SCRIPTING)
			this.Text = "YAT";
		#else
			this.Text = "Albatros";
		#endif
			this.Deactivate += new System.EventHandler(this.Main_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
			this.MdiChildActivate += new System.EventHandler(this.Main_MdiChildActivate);
			this.Shown += new System.EventHandler(this.Main_Shown);
			this.LocationChanged += new System.EventHandler(this.Main_LocationChanged);
			this.SizeChanged += new System.EventHandler(this.Main_SizeChanged);
			this.Resize += new System.EventHandler(this.Main_Resize);
			this.contextMenuStrip_Main.ResumeLayout(false);
			this.contextMenuStrip_FileRecent.ResumeLayout(false);
			this.menuStrip_Main.ResumeLayout(false);
			this.menuStrip_Main.PerformLayout();
			this.statusStrip_Main.ResumeLayout(false);
			this.statusStrip_Main.PerformLayout();
			this.contextMenuStrip_Status.ResumeLayout(false);
			this.toolStrip_Main.ResumeLayout(false);
			this.toolStrip_Main.PerformLayout();
			this.toolStripPanel_Top.ResumeLayout(false);
			this.toolStripPanel_Top.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timer_Status;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Main;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainContextMenu_File_New;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainContextMenu_File_Open;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainContextMenu_File_Recent;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainContextMenu_File_Exit;
		private MKY.Windows.Forms.MenuStripEx menuStrip_Main;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_New;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Open;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_CloseAll;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_SaveAll;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Recent;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Exit;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_TileHorizontal;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_Cascade;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_TileVertical;
	#if (WITH_SCRIPTING)
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Script;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Script_Panel;
	#endif
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_Contents;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Help_Separator_1;
	#if !(WITH_SCRIPTING)
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_ReleaseNotes;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Help_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_RequestSupport;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_RequestFeature;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_SubmitBug;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Help_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_Update;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Help_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_Donate;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Help_Separator_5;
	#endif // WITH_SCRIPTING
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_About;
		private MKY.Windows.Forms.StatusStripEx statusStrip_Main;
		private MKY.Windows.Forms.ToolStripEx toolStrip_Main;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_File_New;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_File_Open;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_File_Save;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_1;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Start;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Stop;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_2;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Settings;
		private System.Windows.Forms.ToolStripPanel toolStripPanel_Top;
		private System.Windows.Forms.ToolStripPanel toolStripPanel_Right;
		private System.Windows.Forms.ToolStripPanel toolStripPanel_Left;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_FileRecent;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_FileRecentContextMenu_8;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_MainStatus_Status;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_MainStatus_Chrono;
		private MKY.Windows.Forms.Chronometer chronometer_Main;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainContextMenu_File_OpenWorkspace;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Preferences;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Workspace;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Workspace_New;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Workspace_Open;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_Workspace_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Workspace_Close;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_File_Separator_Workspace_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Workspace_Save;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_File_Workspace_SaveAs;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Clear;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_SaveToFile;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_CopyToClipboard;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Print;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_3;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_Bin;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_Oct;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_Dec;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_Hex;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_Unicode;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_4;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_Char;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Radix_String;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_MainStatus_TerminalInfo;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Status;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_StatusContextMenu_ShowTerminalInfo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_StatusContextMenu_ShowTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_StatusContextMenu_ShowChrono;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_StatusContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_StatusContextMenu_Preferences;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Log_On;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Log_Off;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Log_Settings;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_5;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_6;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Format;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Terminal_Refresh;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Log;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Log_AllOn;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Log_AllOff;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Log_Separator_1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Log_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Log_AllClear;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_Maximize;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Window_Separator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_Minimize;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_File_SaveWorkspace;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Log_OpenFile;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Log_OpenDirectory;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_Automatic;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Window_AlwaysOnTop;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainMenu_Window_Separator2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_7;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_ShowHide;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_MainTool_AutoResponse_Response;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_MainTool_AutoResponse_Trigger;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_Trigger_UseText;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_Trigger_WholeWord;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_Response_EnableReplace;
		private System.Windows.Forms.ToolStripLabel toolStripLabel_MainTool_AutoResponse_Count;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoResponse_Deactivate;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_8;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Find_ShowHide;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_MainTool_Find_Pattern;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Find_Next;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Find_Previous;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Find_CaseSensitive;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Find_WholeWord;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Find_EnableRegex;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoAction_ShowHide;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_MainTool_AutoAction_Trigger;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoAction_Trigger_UseText;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoAction_Trigger_WholeWord;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoAction_Trigger_EnableRegex;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_MainTool_AutoAction_Action;
		private System.Windows.Forms.ToolStripLabel toolStripLabel_MainTool_AutoAction_Count;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_AutoAction_Deactivate;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_9;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_MainStatus_Time;
		private System.Windows.Forms.Timer timer_Time;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MainMenu_Help_AnyOtherFeedback;
	#if (WITH_SCRIPTING)
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MainTool_Separator_10;
		private System.Windows.Forms.ToolStripButton toolStripButton_MainTool_Script_ShowHide;
	#endif
	}
}
