namespace YAT.Gui.Forms
{
	partial class Terminal
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Terminal));
			this.timer_Status = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip_Monitor = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_MonitorContextMenu_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_MonitorContextMenu_Hide = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_Radix = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Radix = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_RadixContextMenu_String = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Char = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_Bin = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Oct = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Dec = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Hex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_TxRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_String = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Char = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_RxRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_String = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Char = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Format = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowLength = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowEol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_SelectNone = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Print = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Radix = new System.Windows.Forms.ToolStripMenuItem();
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
			this.toolStripMenuItem_PredefinedContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Page = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_Separator = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Page_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Page_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Define = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Hide = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Predefined = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Send = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_SendContextMenu_SendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_KeepCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_CopyPredefined = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_SendImmediately = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip_Terminal = new System.Windows.Forms.MenuStrip();
			this.toolStripMenuItem_TerminalMenu_File = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_File_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_File_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_File_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Start = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Print = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Preset = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_PresetContextMenu_Preset_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Command = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_File = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_KeepCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Begin = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_End = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Log_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Log_Settings = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBox_TerminalMenu_View_Panels_Orientation = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowLength = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowEol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Format = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip_Terminal = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel_TerminalStatus_Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_IOStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_ConnectionState = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_Separator1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_RTS = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_CTS = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_DTR = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_DSR = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_DCD = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_Separator2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_Separator3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_OutputBreak = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_InputBreak = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer_Terminal = new System.Windows.Forms.SplitContainer();
			this.splitContainer_Predefined = new System.Windows.Forms.SplitContainer();
			this.panel_Monitor = new System.Windows.Forms.Panel();
			this.groupBox_Monitor = new System.Windows.Forms.GroupBox();
			this.splitContainer_TxMonitor = new System.Windows.Forms.SplitContainer();
			this.panel_Monitor_Tx = new System.Windows.Forms.Panel();
			this.monitor_Tx = new YAT.Gui.Controls.Monitor();
			this.splitContainer_RxMonitor = new System.Windows.Forms.SplitContainer();
			this.panel_Monitor_Bidir = new System.Windows.Forms.Panel();
			this.monitor_Bidir = new YAT.Gui.Controls.Monitor();
			this.panel_Monitor_Rx = new System.Windows.Forms.Panel();
			this.monitor_Rx = new YAT.Gui.Controls.Monitor();
			this.panel_Predefined = new System.Windows.Forms.Panel();
			this.groupBox_Predefined = new System.Windows.Forms.GroupBox();
			this.predefined = new YAT.Gui.Controls.PredefinedCommands();
			this.panel_Send = new System.Windows.Forms.Panel();
			this.send = new YAT.Gui.Controls.Send();
			this.timer_RtsLuminescence = new System.Windows.Forms.Timer(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.contextMenuStrip_Monitor.SuspendLayout();
			this.contextMenuStrip_Radix.SuspendLayout();
			this.contextMenuStrip_Predefined.SuspendLayout();
			this.contextMenuStrip_Send.SuspendLayout();
			this.menuStrip_Terminal.SuspendLayout();
			this.contextMenuStrip_Preset.SuspendLayout();
			this.statusStrip_Terminal.SuspendLayout();
			this.splitContainer_Terminal.Panel1.SuspendLayout();
			this.splitContainer_Terminal.Panel2.SuspendLayout();
			this.splitContainer_Terminal.SuspendLayout();
			this.splitContainer_Predefined.Panel1.SuspendLayout();
			this.splitContainer_Predefined.Panel2.SuspendLayout();
			this.splitContainer_Predefined.SuspendLayout();
			this.panel_Monitor.SuspendLayout();
			this.groupBox_Monitor.SuspendLayout();
			this.splitContainer_TxMonitor.Panel1.SuspendLayout();
			this.splitContainer_TxMonitor.Panel2.SuspendLayout();
			this.splitContainer_TxMonitor.SuspendLayout();
			this.panel_Monitor_Tx.SuspendLayout();
			this.splitContainer_RxMonitor.Panel1.SuspendLayout();
			this.splitContainer_RxMonitor.Panel2.SuspendLayout();
			this.splitContainer_RxMonitor.SuspendLayout();
			this.panel_Monitor_Bidir.SuspendLayout();
			this.panel_Monitor_Rx.SuspendLayout();
			this.panel_Predefined.SuspendLayout();
			this.groupBox_Predefined.SuspendLayout();
			this.panel_Send.SuspendLayout();
			this.SuspendLayout();
			// 
			// timer_Status
			// 
			this.timer_Status.Tick += new System.EventHandler(this.timer_Status_Tick);
			// 
			// contextMenuStrip_Monitor
			// 
			this.contextMenuStrip_Monitor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MonitorContextMenu_Panels,
            this.toolStripMenuItem_MonitorContextMenu_Hide,
            this.toolStripMenuItem_MonitorContextMenu_Separator_1,
            this.toolStripMenuItem_MonitorContextMenu_Radix,
            this.toolStripMenuItem_MonitorContextMenu_Format,
            this.toolStripMenuItem_MonitorContextMenu_Separator_2,
            this.toolStripMenuItem_MonitorContextMenu_ShowRadix,
            this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp,
            this.toolStripMenuItem_MonitorContextMenu_ShowLength,
            this.toolStripMenuItem_MonitorContextMenu_ShowEol,
            this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers,
            this.toolStripMenuItem_MonitorContextMenu_Separator_3,
            this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime,
            this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime,
            this.toolStripMenuItem_MonitorContextMenu_Separator_4,
            this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate,
            this.toolStripMenuItem_MonitorContextMenu_ResetCount,
            this.toolStripMenuItem_MonitorContextMenu_Separator_5,
            this.toolStripMenuItem_MonitorContextMenu_Clear,
            this.toolStripMenuItem_MonitorContextMenu_Separator_6,
            this.toolStripMenuItem_MonitorContextMenu_SelectAll,
            this.toolStripMenuItem_MonitorContextMenu_SelectNone,
            this.toolStripMenuItem_MonitorContextMenu_Separator_7,
            this.toolStripMenuItem_MonitorContextMenu_SaveToFile,
            this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard,
            this.toolStripMenuItem_MonitorContextMenu_Print});
			this.contextMenuStrip_Monitor.Name = "contextMenuStrip_Monitor";
			this.contextMenuStrip_Monitor.Size = new System.Drawing.Size(242, 464);
			this.contextMenuStrip_Monitor.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Monitor_Opening);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MonitorContextMenu_Panels_Tx,
            this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir,
            this.toolStripMenuItem_MonitorContextMenu_Panels_Rx,
            this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1,
            this.toolStripComboBox_MonitorContextMenu_Panels_Orientation});
			this.toolStripMenuItem_MonitorContextMenu_Panels.Name = "toolStripMenuItem_MonitorContextMenu_Panels";
			this.toolStripMenuItem_MonitorContextMenu_Panels.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels.Text = "Panels";
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Tx
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Tx";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Text = "Transmit Panel";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Panels_Tx_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Bidir
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Checked = true;
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Bidir";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Text = "Bidirectional Panel";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Rx
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Rx";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Text = "Receive Panel";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Panels_Rx_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Separator1
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Separator1";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1.Size = new System.Drawing.Size(222, 6);
			// 
			// toolStripComboBox_MonitorContextMenu_Panels_Orientation
			// 
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.Name = "toolStripComboBox_MonitorContextMenu_Panels_Orientation";
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.Size = new System.Drawing.Size(165, 23);
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MonitorContextMenu_Panels_Orientation_SelectedIndexChanged);
			// 
			// toolStripMenuItem_MonitorContextMenu_Hide
			// 
			this.toolStripMenuItem_MonitorContextMenu_Hide.Name = "toolStripMenuItem_MonitorContextMenu_Hide";
			this.toolStripMenuItem_MonitorContextMenu_Hide.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_Hide.Text = "Hide";
			this.toolStripMenuItem_MonitorContextMenu_Hide.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Hide_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_1
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_1.Name = "toolStripMenuItem_MonitorContextMenu_Separator_1";
			this.toolStripMenuItem_MonitorContextMenu_Separator_1.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_Radix
			// 
			this.toolStripMenuItem_MonitorContextMenu_Radix.DropDown = this.contextMenuStrip_Radix;
			this.toolStripMenuItem_MonitorContextMenu_Radix.Name = "toolStripMenuItem_MonitorContextMenu_Radix";
			this.toolStripMenuItem_MonitorContextMenu_Radix.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_Radix.Text = "Radix";
			// 
			// contextMenuStrip_Radix
			// 
			this.contextMenuStrip_Radix.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_RadixContextMenu_String,
            this.toolStripMenuItem_RadixContextMenu_Char,
            this.toolStripMenuItem_RadixContextMenu_Separator_1,
            this.toolStripMenuItem_RadixContextMenu_Bin,
            this.toolStripMenuItem_RadixContextMenu_Oct,
            this.toolStripMenuItem_RadixContextMenu_Dec,
            this.toolStripMenuItem_RadixContextMenu_Hex,
            this.toolStripMenuItem_RadixContextMenu_Separator_2,
            this.toolStripMenuItem_RadixContextMenu_SeparateTxRx,
            this.toolStripMenuItem_RadixContextMenu_Separator_3,
            this.toolStripMenuItem_RadixContextMenu_TxRadix,
            this.toolStripMenuItem_RadixContextMenu_RxRadix});
			this.contextMenuStrip_Radix.Name = "contextMenuStrip_Radix";
			this.contextMenuStrip_Radix.OwnerItem = this.toolStripMenuItem_TerminalMenu_View_Radix;
			this.contextMenuStrip_Radix.Size = new System.Drawing.Size(173, 220);
			this.contextMenuStrip_Radix.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Radix_Opening);
			// 
			// toolStripMenuItem_RadixContextMenu_String
			// 
			this.toolStripMenuItem_RadixContextMenu_String.Checked = true;
			this.toolStripMenuItem_RadixContextMenu_String.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_RadixContextMenu_String.Name = "toolStripMenuItem_RadixContextMenu_String";
			this.toolStripMenuItem_RadixContextMenu_String.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_String.Text = "&String";
			this.toolStripMenuItem_RadixContextMenu_String.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_String_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Char
			// 
			this.toolStripMenuItem_RadixContextMenu_Char.Name = "toolStripMenuItem_RadixContextMenu_Char";
			this.toolStripMenuItem_RadixContextMenu_Char.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_Char.Text = "&Character";
			this.toolStripMenuItem_RadixContextMenu_Char.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Char_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_1
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_1.Name = "toolStripMenuItem_RadixContextMenu_Separator_1";
			this.toolStripMenuItem_RadixContextMenu_Separator_1.Size = new System.Drawing.Size(169, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Bin
			// 
			this.toolStripMenuItem_RadixContextMenu_Bin.Name = "toolStripMenuItem_RadixContextMenu_Bin";
			this.toolStripMenuItem_RadixContextMenu_Bin.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_Bin.Text = "&Binary";
			this.toolStripMenuItem_RadixContextMenu_Bin.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Bin_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Oct
			// 
			this.toolStripMenuItem_RadixContextMenu_Oct.Name = "toolStripMenuItem_RadixContextMenu_Oct";
			this.toolStripMenuItem_RadixContextMenu_Oct.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_Oct.Text = "&Octal";
			this.toolStripMenuItem_RadixContextMenu_Oct.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Oct_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Dec
			// 
			this.toolStripMenuItem_RadixContextMenu_Dec.Name = "toolStripMenuItem_RadixContextMenu_Dec";
			this.toolStripMenuItem_RadixContextMenu_Dec.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_Dec.Text = "&Decimal";
			this.toolStripMenuItem_RadixContextMenu_Dec.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Dec_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Hex
			// 
			this.toolStripMenuItem_RadixContextMenu_Hex.Name = "toolStripMenuItem_RadixContextMenu_Hex";
			this.toolStripMenuItem_RadixContextMenu_Hex.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_Hex.Text = "&Hexadecimal";
			this.toolStripMenuItem_RadixContextMenu_Hex.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Hex_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_2
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_2.Name = "toolStripMenuItem_RadixContextMenu_Separator_2";
			this.toolStripMenuItem_RadixContextMenu_Separator_2.Size = new System.Drawing.Size(169, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_SeparateTxRx
			// 
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Name = "toolStripMenuItem_RadixContextMenu_SeparateTxRx";
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Text = "Separate Tx and Rx";
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_SeparateTxRx_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_3
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_3.Name = "toolStripMenuItem_RadixContextMenu_Separator_3";
			this.toolStripMenuItem_RadixContextMenu_Separator_3.Size = new System.Drawing.Size(169, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_TxRadix
			// 
			this.toolStripMenuItem_RadixContextMenu_TxRadix.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_RadixContextMenu_Tx_String,
            this.toolStripMenuItem_RadixContextMenu_Tx_Char,
            this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1,
            this.toolStripMenuItem_RadixContextMenu_Tx_Bin,
            this.toolStripMenuItem_RadixContextMenu_Tx_Oct,
            this.toolStripMenuItem_RadixContextMenu_Tx_Dec,
            this.toolStripMenuItem_RadixContextMenu_Tx_Hex});
			this.toolStripMenuItem_RadixContextMenu_TxRadix.Name = "toolStripMenuItem_RadixContextMenu_TxRadix";
			this.toolStripMenuItem_RadixContextMenu_TxRadix.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_TxRadix.Text = "&Tx Radix";
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_String
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Checked = true;
			this.toolStripMenuItem_RadixContextMenu_Tx_String.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Name = "toolStripMenuItem_RadixContextMenu_Tx_String";
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Text = "&String";
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_String_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Char
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Name = "toolStripMenuItem_RadixContextMenu_Tx_Char";
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Text = "&Character";
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Char_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Separator_1
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1.Name = "toolStripMenuItem_RadixContextMenu_Tx_Separator_1";
			this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1.Size = new System.Drawing.Size(139, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Bin
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Name = "toolStripMenuItem_RadixContextMenu_Tx_Bin";
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Text = "&Binary";
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Bin_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Oct
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Name = "toolStripMenuItem_RadixContextMenu_Tx_Oct";
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Text = "&Octal";
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Oct_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Dec
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Name = "toolStripMenuItem_RadixContextMenu_Tx_Dec";
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Text = "&Decimal";
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Dec_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Hex
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Name = "toolStripMenuItem_RadixContextMenu_Tx_Hex";
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Text = "&Hexadecimal";
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Hex_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_RxRadix
			// 
			this.toolStripMenuItem_RadixContextMenu_RxRadix.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_RadixContextMenu_Rx_String,
            this.toolStripMenuItem_RadixContextMenu_Rx_Char,
            this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1,
            this.toolStripMenuItem_RadixContextMenu_Rx_Bin,
            this.toolStripMenuItem_RadixContextMenu_Rx_Oct,
            this.toolStripMenuItem_RadixContextMenu_Rx_Dec,
            this.toolStripMenuItem_RadixContextMenu_Rx_Hex});
			this.toolStripMenuItem_RadixContextMenu_RxRadix.Name = "toolStripMenuItem_RadixContextMenu_RxRadix";
			this.toolStripMenuItem_RadixContextMenu_RxRadix.Size = new System.Drawing.Size(172, 22);
			this.toolStripMenuItem_RadixContextMenu_RxRadix.Text = "&Rx Radix";
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_String
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Checked = true;
			this.toolStripMenuItem_RadixContextMenu_Rx_String.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Name = "toolStripMenuItem_RadixContextMenu_Rx_String";
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Text = "&String";
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_String_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Char
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Name = "toolStripMenuItem_RadixContextMenu_Rx_Char";
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Text = "&Character";
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Char_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Separator_1
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1.Name = "toolStripMenuItem_RadixContextMenu_Rx_Separator_1";
			this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1.Size = new System.Drawing.Size(139, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Bin
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Name = "toolStripMenuItem_RadixContextMenu_Rx_Bin";
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Text = "&Binary";
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Bin_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Oct
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Name = "toolStripMenuItem_RadixContextMenu_Rx_Oct";
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Text = "&Octal";
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Oct_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Dec
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Name = "toolStripMenuItem_RadixContextMenu_Rx_Dec";
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Text = "&Decimal";
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Dec_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Hex
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Name = "toolStripMenuItem_RadixContextMenu_Rx_Hex";
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Size = new System.Drawing.Size(142, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Text = "&Hexadecimal";
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Hex_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Format
			// 
			this.toolStripMenuItem_MonitorContextMenu_Format.Name = "toolStripMenuItem_MonitorContextMenu_Format";
			this.toolStripMenuItem_MonitorContextMenu_Format.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_Format.Text = "Format...";
			this.toolStripMenuItem_MonitorContextMenu_Format.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Format_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_2
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_2.Name = "toolStripMenuItem_MonitorContextMenu_Separator_2";
			this.toolStripMenuItem_MonitorContextMenu_Separator_2.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowRadix
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Name = "toolStripMenuItem_MonitorContextMenu_ShowRadix";
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Text = "Show Radix";
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowRadix_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowTimeStamp
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Name = "toolStripMenuItem_MonitorContextMenu_ShowTimeStamp";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Text = "Show Time Stamp";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowLength
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Name = "toolStripMenuItem_MonitorContextMenu_ShowLength";
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Text = "Show Length";
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowLength_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowEol
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Name = "toolStripMenuItem_MonitorContextMenu_ShowEol";
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Text = "Show End Of Line Sequence";
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowEol_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowLineNumbers
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Name = "toolStripMenuItem_MonitorContextMenu_ShowLineNumbers";
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Text = "Show Line Number";
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_3
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_3.Name = "toolStripMenuItem_MonitorContextMenu_Separator_3";
			this.toolStripMenuItem_MonitorContextMenu_Separator_3.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowConnectTime
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Name = "toolStripMenuItem_MonitorContextMenu_ShowConnectTime";
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Text = "Show Connect Time";
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_RestartConnectTime
			// 
			this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime.Name = "toolStripMenuItem_MonitorContextMenu_RestartConnectTime";
			this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime.Text = "Restart Connect Time";
			this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_RestartConnectTime_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_4
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_4.Name = "toolStripMenuItem_MonitorContextMenu_Separator_4";
			this.toolStripMenuItem_MonitorContextMenu_Separator_4.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowCountAndRate
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Name = "toolStripMenuItem_MonitorContextMenu_ShowCountAndRate";
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Text = "Show Byte/Line Count and Rate";
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ResetCount
			// 
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Name = "toolStripMenuItem_MonitorContextMenu_ResetCount";
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Text = "Reset Count";
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ResetCount_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_5
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_5.Name = "toolStripMenuItem_MonitorContextMenu_Separator_5";
			this.toolStripMenuItem_MonitorContextMenu_Separator_5.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_Clear
			// 
			this.toolStripMenuItem_MonitorContextMenu_Clear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem_MonitorContextMenu_Clear.Image")));
			this.toolStripMenuItem_MonitorContextMenu_Clear.Name = "toolStripMenuItem_MonitorContextMenu_Clear";
			this.toolStripMenuItem_MonitorContextMenu_Clear.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_Clear.Text = "Clear";
			this.toolStripMenuItem_MonitorContextMenu_Clear.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Clear_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_6
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_6.Name = "toolStripMenuItem_MonitorContextMenu_Separator_6";
			this.toolStripMenuItem_MonitorContextMenu_Separator_6.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_SelectAll
			// 
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Name = "toolStripMenuItem_MonitorContextMenu_SelectAll";
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Text = "Select All";
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_SelectAll_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_SelectNone
			// 
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Name = "toolStripMenuItem_MonitorContextMenu_SelectNone";
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Text = "Select None";
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_SelectNone_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_7
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_7.Name = "toolStripMenuItem_MonitorContextMenu_Separator_7";
			this.toolStripMenuItem_MonitorContextMenu_Separator_7.Size = new System.Drawing.Size(238, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_SaveToFile
			// 
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Image = global::YAT.Gui.Forms.Properties.Resources.Image_CopyToFolder_16x16;
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Name = "toolStripMenuItem_MonitorContextMenu_SaveToFile";
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Text = "Save to File...";
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_SaveToFile_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_CopyToClipboard
			// 
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Image = global::YAT.Gui.Forms.Properties.Resources.Image_CopyToClipboard_16x16;
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Name = "toolStripMenuItem_MonitorContextMenu_CopyToClipboard";
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Text = "Copy to Clipboard";
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Print
			// 
			this.toolStripMenuItem_MonitorContextMenu_Print.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Print_16x16;
			this.toolStripMenuItem_MonitorContextMenu_Print.Name = "toolStripMenuItem_MonitorContextMenu_Print";
			this.toolStripMenuItem_MonitorContextMenu_Print.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_MonitorContextMenu_Print.Text = "Print...";
			this.toolStripMenuItem_MonitorContextMenu_Print.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Print_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Radix
			// 
			this.toolStripMenuItem_TerminalMenu_View_Radix.DropDown = this.contextMenuStrip_Radix;
			this.toolStripMenuItem_TerminalMenu_View_Radix.Name = "toolStripMenuItem_TerminalMenu_View_Radix";
			this.toolStripMenuItem_TerminalMenu_View_Radix.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_Radix.Text = "R&adix";
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
            this.toolStripMenuItem_PredefinedContextMenu_Separator_1,
            this.toolStripMenuItem_PredefinedContextMenu_Page,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_2,
            this.toolStripMenuItem_PredefinedContextMenu_Define,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_3,
            this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand,
            this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand,
            this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_4,
            this.toolStripMenuItem_PredefinedContextMenu_Hide});
			this.contextMenuStrip_Predefined.Name = "contextMenuStrip_PredefinedCommands";
			this.contextMenuStrip_Predefined.OwnerItem = this.toolStripMenuItem_TerminalMenu_Send_Predefined;
			this.contextMenuStrip_Predefined.Size = new System.Drawing.Size(221, 424);
			this.contextMenuStrip_Predefined.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Predefined_Opening);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Name = "toolStripMenuItem_PredefinedContextMenu_Command_1";
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Text = "&1: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Name = "toolStripMenuItem_PredefinedContextMenu_Command_2";
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Text = "&2: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Name = "toolStripMenuItem_PredefinedContextMenu_Command_3";
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Text = "&3: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Name = "toolStripMenuItem_PredefinedContextMenu_Command_4";
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Text = "&4: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Name = "toolStripMenuItem_PredefinedContextMenu_Command_5";
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Text = "&5: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Name = "toolStripMenuItem_PredefinedContextMenu_Command_6";
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Text = "&6: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Name = "toolStripMenuItem_PredefinedContextMenu_Command_7";
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Text = "&7: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Name = "toolStripMenuItem_PredefinedContextMenu_Command_8";
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Text = "&8: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Name = "toolStripMenuItem_PredefinedContextMenu_Command_9";
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F9)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Text = "&9: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Name = "toolStripMenuItem_PredefinedContextMenu_Command_10";
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Text = "1&0: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Name = "toolStripMenuItem_PredefinedContextMenu_Command_11";
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Text = "11: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Name = "toolStripMenuItem_PredefinedContextMenu_Command_12";
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F12)));
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Text = "12: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Command_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Command_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_1.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_1";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_1.Size = new System.Drawing.Size(217, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_Page_Next,
            this.toolStripMenuItem_PredefinedContextMenu_Page_Previous,
            this.toolStripMenuItem_PredefinedContextMenu_Page_Separator,
            this.toolStripMenuItem_PredefinedContextMenu_Page_1,
            this.toolStripMenuItem_PredefinedContextMenu_Page_2,
            this.toolStripMenuItem_PredefinedContextMenu_Page_3,
            this.toolStripMenuItem_PredefinedContextMenu_Page_4,
            this.toolStripMenuItem_PredefinedContextMenu_Page_5,
            this.toolStripMenuItem_PredefinedContextMenu_Page_6,
            this.toolStripMenuItem_PredefinedContextMenu_Page_7,
            this.toolStripMenuItem_PredefinedContextMenu_Page_8,
            this.toolStripMenuItem_PredefinedContextMenu_Page_9,
            this.toolStripMenuItem_PredefinedContextMenu_Page_10,
            this.toolStripMenuItem_PredefinedContextMenu_Page_11,
            this.toolStripMenuItem_PredefinedContextMenu_Page_12});
			this.toolStripMenuItem_PredefinedContextMenu_Page.Name = "toolStripMenuItem_PredefinedContextMenu_Page";
			this.toolStripMenuItem_PredefinedContextMenu_Page.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page.Text = "&Page";
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_Next
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Name = "toolStripMenuItem_PredefinedContextMenu_Page_Next";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.Right)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Text = "&Next";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Next.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Next_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_Previous
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Name = "toolStripMenuItem_PredefinedContextMenu_Page_Previous";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.Left)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Text = "&Previous";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Previous.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Previous_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_Separator
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_Separator.Name = "toolStripMenuItem_PredefinedContextMenu_Page_Separator";
			this.toolStripMenuItem_PredefinedContextMenu_Page_Separator.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.Name = "toolStripMenuItem_PredefinedContextMenu_Page_1";
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F1)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.Text = "&1: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.Name = "toolStripMenuItem_PredefinedContextMenu_Page_2";
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F2)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.Text = "&2: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.Name = "toolStripMenuItem_PredefinedContextMenu_Page_3";
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F3)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.Text = "&3: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.Name = "toolStripMenuItem_PredefinedContextMenu_Page_4";
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F4)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.Text = "&4: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.Name = "toolStripMenuItem_PredefinedContextMenu_Page_5";
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F5)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.Text = "&5: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.Name = "toolStripMenuItem_PredefinedContextMenu_Page_6";
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F6)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.Text = "&6: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.Name = "toolStripMenuItem_PredefinedContextMenu_Page_7";
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F7)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.Text = "&7: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.Name = "toolStripMenuItem_PredefinedContextMenu_Page_8";
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F8)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.Text = "&8: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.Name = "toolStripMenuItem_PredefinedContextMenu_Page_9";
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F9)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.Text = "&9: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.Name = "toolStripMenuItem_PredefinedContextMenu_Page_10";
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F10)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.Text = "1&0: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.Name = "toolStripMenuItem_PredefinedContextMenu_Page_11";
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F11)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.Text = "11: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.Enabled = false;
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.Name = "toolStripMenuItem_PredefinedContextMenu_Page_12";
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.F12)));
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.Text = "12: <Undefined>";
			this.toolStripMenuItem_PredefinedContextMenu_Page_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Page_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_2.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_2";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_2.Size = new System.Drawing.Size(217, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Define
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Define.Name = "toolStripMenuItem_PredefinedContextMenu_Define";
			this.toolStripMenuItem_PredefinedContextMenu_Define.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Define.Text = "Define...";
			this.toolStripMenuItem_PredefinedContextMenu_Define.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Define_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_3.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_3";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_3.Size = new System.Drawing.Size(217, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand.Name = "toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand";
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand.Text = "Copy to Send Command";
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand.Name = "toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand.Text = "Copy from Send Command";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Name = "toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Text = "Copy from Send File";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_4.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_4";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_4.Size = new System.Drawing.Size(217, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Hide
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Name = "toolStripMenuItem_PredefinedContextMenu_Hide";
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Text = "Hide";
			this.toolStripMenuItem_PredefinedContextMenu_Hide.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Hide_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Predefined
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Predefined.DropDown = this.contextMenuStrip_Predefined;
			this.toolStripMenuItem_TerminalMenu_Send_Predefined.Name = "toolStripMenuItem_TerminalMenu_Send_Predefined";
			this.toolStripMenuItem_TerminalMenu_Send_Predefined.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_TerminalMenu_Send_Predefined.Text = "&Predefined Command";
			// 
			// contextMenuStrip_Send
			// 
			this.contextMenuStrip_Send.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SendContextMenu_SendCommand,
            this.toolStripMenuItem_SendContextMenu_SendFile,
            this.toolStripMenuItem_SendContextMenu_Separator_1,
            this.toolStripMenuItem_SendContextMenu_Panels,
            this.toolStripMenuItem_SendContextMenu_Separator_2,
            this.toolStripMenuItem_SendContextMenu_KeepCommand,
            this.toolStripMenuItem_SendContextMenu_CopyPredefined,
            this.toolStripMenuItem_SendContextMenu_SendImmediately});
			this.contextMenuStrip_Send.Name = "contextMenuStrip_Send";
			this.contextMenuStrip_Send.Size = new System.Drawing.Size(342, 148);
			this.contextMenuStrip_Send.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Send_Opening);
			// 
			// toolStripMenuItem_SendContextMenu_SendCommand
			// 
			this.toolStripMenuItem_SendContextMenu_SendCommand.Name = "toolStripMenuItem_SendContextMenu_SendCommand";
			this.toolStripMenuItem_SendContextMenu_SendCommand.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.toolStripMenuItem_SendContextMenu_SendCommand.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_SendContextMenu_SendCommand.Text = "Send Command";
			this.toolStripMenuItem_SendContextMenu_SendCommand.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendCommand_Click);
			// 
			// toolStripMenuItem_SendContextMenu_SendFile
			// 
			this.toolStripMenuItem_SendContextMenu_SendFile.Name = "toolStripMenuItem_SendContextMenu_SendFile";
			this.toolStripMenuItem_SendContextMenu_SendFile.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.toolStripMenuItem_SendContextMenu_SendFile.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_SendContextMenu_SendFile.Text = "Send File";
			this.toolStripMenuItem_SendContextMenu_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendFile_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_1
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_1.Name = "toolStripMenuItem_SendContextMenu_Separator_1";
			this.toolStripMenuItem_SendContextMenu_Separator_1.Size = new System.Drawing.Size(338, 6);
			// 
			// toolStripMenuItem_SendContextMenu_Panels
			// 
			this.toolStripMenuItem_SendContextMenu_Panels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SendContextMenu_Panels_SendCommand,
            this.toolStripMenuItem_SendContextMenu_Panels_SendFile});
			this.toolStripMenuItem_SendContextMenu_Panels.Name = "toolStripMenuItem_SendContextMenu_Panels";
			this.toolStripMenuItem_SendContextMenu_Panels.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_SendContextMenu_Panels.Text = "Panels";
			// 
			// toolStripMenuItem_SendContextMenu_Panels_SendCommand
			// 
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Checked = true;
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Name = "toolStripMenuItem_SendContextMenu_Panels_SendCommand";
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Size = new System.Drawing.Size(192, 22);
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Text = "Send Command Panel";
			this.toolStripMenuItem_SendContextMenu_Panels_SendCommand.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Panels_SendCommand_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Panels_SendFile
			// 
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = true;
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Name = "toolStripMenuItem_SendContextMenu_Panels_SendFile";
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Size = new System.Drawing.Size(192, 22);
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Text = "Send File Panel";
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Panels_SendFile_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_2
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_2.Name = "toolStripMenuItem_SendContextMenu_Separator_2";
			this.toolStripMenuItem_SendContextMenu_Separator_2.Size = new System.Drawing.Size(338, 6);
			// 
			// toolStripMenuItem_SendContextMenu_KeepCommand
			// 
			this.toolStripMenuItem_SendContextMenu_KeepCommand.Name = "toolStripMenuItem_SendContextMenu_KeepCommand";
			this.toolStripMenuItem_SendContextMenu_KeepCommand.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.K)));
			this.toolStripMenuItem_SendContextMenu_KeepCommand.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_SendContextMenu_KeepCommand.Text = "Keep Command After Send";
			this.toolStripMenuItem_SendContextMenu_KeepCommand.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_KeepCommand_Click);
			// 
			// toolStripMenuItem_SendContextMenu_CopyPredefined
			// 
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Name = "toolStripMenuItem_SendContextMenu_CopyPredefined";
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Text = "Copy Predefined To Send Command";
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_CopyPredefined_Click);
			// 
			// toolStripMenuItem_SendContextMenu_SendImmediately
			// 
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Name = "toolStripMenuItem_SendContextMenu_SendImmediately";
			this.toolStripMenuItem_SendContextMenu_SendImmediately.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.I)));
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Text = "Send Each Character Immediately";
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendImmediately_Click);
			// 
			// menuStrip_Terminal
			// 
			this.menuStrip_Terminal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_File,
            this.toolStripMenuItem_TerminalMenu_Terminal,
            this.toolStripMenuItem_TerminalMenu_Send,
            this.toolStripMenuItem_TerminalMenu_Log,
            this.toolStripMenuItem_TerminalMenu_View});
			this.menuStrip_Terminal.Location = new System.Drawing.Point(0, 0);
			this.menuStrip_Terminal.Name = "menuStrip_Terminal";
			this.menuStrip_Terminal.Size = new System.Drawing.Size(712, 24);
			this.menuStrip_Terminal.TabIndex = 0;
			this.menuStrip_Terminal.Visible = false;
			// 
			// toolStripMenuItem_TerminalMenu_File
			// 
			this.toolStripMenuItem_TerminalMenu_File.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_File_Close,
            this.toolStripMenuItem_TerminalMenu_File_Save,
            this.toolStripMenuItem_TerminalMenu_File_SaveAs});
			this.toolStripMenuItem_TerminalMenu_File.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.toolStripMenuItem_TerminalMenu_File.MergeIndex = 0;
			this.toolStripMenuItem_TerminalMenu_File.Name = "toolStripMenuItem_TerminalMenu_File";
			this.toolStripMenuItem_TerminalMenu_File.Size = new System.Drawing.Size(37, 20);
			this.toolStripMenuItem_TerminalMenu_File.Text = "&File";
			// 
			// toolStripMenuItem_TerminalMenu_File_Close
			// 
			this.toolStripMenuItem_TerminalMenu_File_Close.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_File_Close.MergeIndex = 3;
			this.toolStripMenuItem_TerminalMenu_File_Close.Name = "toolStripMenuItem_TerminalMenu_File_Close";
			this.toolStripMenuItem_TerminalMenu_File_Close.Size = new System.Drawing.Size(138, 22);
			this.toolStripMenuItem_TerminalMenu_File_Close.Text = "&Close";
			this.toolStripMenuItem_TerminalMenu_File_Close.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_Close_Click);
			// 
			// toolStripMenuItem_TerminalMenu_File_Save
			// 
			this.toolStripMenuItem_TerminalMenu_File_Save.Image = global::YAT.Gui.Forms.Properties.Resources.Image_SaveDocument_16x16;
			this.toolStripMenuItem_TerminalMenu_File_Save.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_File_Save.MergeIndex = 6;
			this.toolStripMenuItem_TerminalMenu_File_Save.Name = "toolStripMenuItem_TerminalMenu_File_Save";
			this.toolStripMenuItem_TerminalMenu_File_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.toolStripMenuItem_TerminalMenu_File_Save.Size = new System.Drawing.Size(138, 22);
			this.toolStripMenuItem_TerminalMenu_File_Save.Text = "&Save";
			this.toolStripMenuItem_TerminalMenu_File_Save.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_Save_Click);
			// 
			// toolStripMenuItem_TerminalMenu_File_SaveAs
			// 
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.MergeIndex = 7;
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Name = "toolStripMenuItem_TerminalMenu_File_SaveAs";
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Size = new System.Drawing.Size(138, 22);
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Text = "Save &As...";
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_SaveAs_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Terminal_Start,
            this.toolStripMenuItem_TerminalMenu_Terminal_Stop,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1,
            this.toolStripMenuItem_TerminalMenu_Terminal_Clear,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2,
            this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll,
            this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3,
            this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard,
            this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile,
            this.toolStripMenuItem_TerminalMenu_Terminal_Print,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4,
            this.toolStripMenuItem_TerminalMenu_Terminal_Settings,
            this.toolStripMenuItem_TerminalMenu_Terminal_Presets});
			this.toolStripMenuItem_TerminalMenu_Terminal.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal.MergeIndex = 1;
			this.toolStripMenuItem_TerminalMenu_Terminal.Name = "toolStripMenuItem_TerminalMenu_Terminal";
			this.toolStripMenuItem_TerminalMenu_Terminal.Size = new System.Drawing.Size(66, 20);
			this.toolStripMenuItem_TerminalMenu_Terminal.Text = "&Terminal";
			this.toolStripMenuItem_TerminalMenu_Terminal.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Start
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Image = global::YAT.Gui.Forms.Properties.Resources.Image_ArrowRight_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Name = "toolStripMenuItem_TerminalMenu_Terminal_Start";
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.O)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Text = "&Open Port";
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Start_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Stop
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Stop_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Name = "toolStripMenuItem_TerminalMenu_Terminal_Stop";
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.C)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Text = "C&lose Port";
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Stop_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_1";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1.Size = new System.Drawing.Size(210, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Clear
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem_TerminalMenu_Terminal_Clear.Image")));
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Name = "toolStripMenuItem_TerminalMenu_Terminal_Clear";
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Delete)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Text = "Cl&ear";
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Clear_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_2";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2.Size = new System.Drawing.Size(210, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_SelectAll
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Name = "toolStripMenuItem_TerminalMenu_Terminal_SelectAll";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Text = "Select &All";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_SelectNone
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Name = "toolStripMenuItem_TerminalMenu_Terminal_SelectNone";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.N)));
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Text = "Select &None";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_3";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3.Size = new System.Drawing.Size(210, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Image = global::YAT.Gui.Forms.Properties.Resources.Image_CopyToClipboard_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Name = "toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard";
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Text = "&Copy to Clipboard";
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_SaveToFile
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Image = global::YAT.Gui.Forms.Properties.Resources.Image_CopyToFolder_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Name = "toolStripMenuItem_TerminalMenu_Terminal_SaveToFile";
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Text = "Save to &File...";
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Print
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Print_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Name = "toolStripMenuItem_TerminalMenu_Terminal_Print";
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Text = "&Print...";
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Print_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_4";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4.Size = new System.Drawing.Size(210, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Settings
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Properties_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Name = "toolStripMenuItem_TerminalMenu_Terminal_Settings";
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.S)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Text = "&Settings...";
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Settings_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Presets
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.DropDown = this.contextMenuStrip_Preset;
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.Name = "toolStripMenuItem_TerminalMenu_Terminal_Presets";
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.Size = new System.Drawing.Size(213, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.Text = "Settings P&resets";
			// 
			// contextMenuStrip_Preset
			// 
			this.contextMenuStrip_Preset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PresetContextMenu_Preset_1,
            this.toolStripMenuItem_PresetContextMenu_Preset_2,
            this.toolStripMenuItem_PresetContextMenu_Preset_3,
            this.toolStripMenuItem_PresetContextMenu_Preset_4,
            this.toolStripMenuItem_PresetContextMenu_Preset_5,
            this.toolStripMenuItem_PresetContextMenu_Preset_6});
			this.contextMenuStrip_Preset.Name = "contextMenuStrip_Preset";
			this.contextMenuStrip_Preset.OwnerItem = this.toolStripMenuItem_TerminalMenu_Terminal_Presets;
			this.contextMenuStrip_Preset.Size = new System.Drawing.Size(234, 136);
			this.contextMenuStrip_Preset.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Preset_Opening);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_1
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_1.Name = "toolStripMenuItem_PresetContextMenu_Preset_1";
			this.toolStripMenuItem_PresetContextMenu_Preset_1.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_1.Tag = "1";
			this.toolStripMenuItem_PresetContextMenu_Preset_1.Text = "&1: 2400, 7, Even, 1, None";
			this.toolStripMenuItem_PresetContextMenu_Preset_1.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_2
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_2.Name = "toolStripMenuItem_PresetContextMenu_Preset_2";
			this.toolStripMenuItem_PresetContextMenu_Preset_2.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_2.Tag = "2";
			this.toolStripMenuItem_PresetContextMenu_Preset_2.Text = "&2: 2400, 7, Even, 1, XOn/XOff";
			this.toolStripMenuItem_PresetContextMenu_Preset_2.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_3
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_3.Name = "toolStripMenuItem_PresetContextMenu_Preset_3";
			this.toolStripMenuItem_PresetContextMenu_Preset_3.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_3.Tag = "3";
			this.toolStripMenuItem_PresetContextMenu_Preset_3.Text = "&3: 9600, 8, None, 1, None";
			this.toolStripMenuItem_PresetContextMenu_Preset_3.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_4
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_4.Name = "toolStripMenuItem_PresetContextMenu_Preset_4";
			this.toolStripMenuItem_PresetContextMenu_Preset_4.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_4.Tag = "4";
			this.toolStripMenuItem_PresetContextMenu_Preset_4.Text = "&4: 9600, 8, None, 1, XOn/XOff";
			this.toolStripMenuItem_PresetContextMenu_Preset_4.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_5
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_5.Name = "toolStripMenuItem_PresetContextMenu_Preset_5";
			this.toolStripMenuItem_PresetContextMenu_Preset_5.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_5.Tag = "5";
			this.toolStripMenuItem_PresetContextMenu_Preset_5.Text = "&5: 19200, 8, None, 1, None";
			this.toolStripMenuItem_PresetContextMenu_Preset_5.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_6
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Name = "toolStripMenuItem_PresetContextMenu_Preset_6";
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Tag = "6";
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Text = "&6: 19200, 8, None, 1, XOn/XOff";
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send
			// 
			this.toolStripMenuItem_TerminalMenu_Send.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Send_Command,
            this.toolStripMenuItem_TerminalMenu_Send_File,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_1,
            this.toolStripMenuItem_TerminalMenu_Send_Predefined,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_2,
            this.toolStripMenuItem_TerminalMenu_Send_KeepCommand,
            this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined,
            this.toolStripMenuItem_TerminalMenu_Send_SendImmediately});
			this.toolStripMenuItem_TerminalMenu_Send.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Send.MergeIndex = 2;
			this.toolStripMenuItem_TerminalMenu_Send.Name = "toolStripMenuItem_TerminalMenu_Send";
			this.toolStripMenuItem_TerminalMenu_Send.Size = new System.Drawing.Size(45, 20);
			this.toolStripMenuItem_TerminalMenu_Send.Text = "&Send";
			this.toolStripMenuItem_TerminalMenu_Send.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Command
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Command.Name = "toolStripMenuItem_TerminalMenu_Send_Command";
			this.toolStripMenuItem_TerminalMenu_Send_Command.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.toolStripMenuItem_TerminalMenu_Send_Command.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_TerminalMenu_Send_Command.Text = "&Command";
			this.toolStripMenuItem_TerminalMenu_Send_Command.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_Command_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_File
			// 
			this.toolStripMenuItem_TerminalMenu_Send_File.Name = "toolStripMenuItem_TerminalMenu_Send_File";
			this.toolStripMenuItem_TerminalMenu_Send_File.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.toolStripMenuItem_TerminalMenu_Send_File.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_TerminalMenu_Send_File.Text = "&File";
			this.toolStripMenuItem_TerminalMenu_Send_File.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_File_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_1.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_1";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_1.Size = new System.Drawing.Size(338, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_2.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_2";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_2.Size = new System.Drawing.Size(338, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_KeepCommand
			// 
			this.toolStripMenuItem_TerminalMenu_Send_KeepCommand.Name = "toolStripMenuItem_TerminalMenu_Send_KeepCommand";
			this.toolStripMenuItem_TerminalMenu_Send_KeepCommand.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.K)));
			this.toolStripMenuItem_TerminalMenu_Send_KeepCommand.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_TerminalMenu_Send_KeepCommand.Text = "&Keep Command After Send";
			this.toolStripMenuItem_TerminalMenu_Send_KeepCommand.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_KeepCommand_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_CopyPredefined
			// 
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Name = "toolStripMenuItem_TerminalMenu_Send_CopyPredefined";
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Text = "Copy &Predefined To Send Command";
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_SendImmediately
			// 
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Name = "toolStripMenuItem_TerminalMenu_Send_SendImmediately";
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.I)));
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Size = new System.Drawing.Size(341, 22);
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Text = "Send Each Character &Immediately";
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_SendImmediately_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log
			// 
			this.toolStripMenuItem_TerminalMenu_Log.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Log_Begin,
            this.toolStripMenuItem_TerminalMenu_Log_End,
            this.toolStripMenuItem_TerminalMenu_Log_Separator_1,
            this.toolStripMenuItem_TerminalMenu_Log_Clear,
            this.toolStripMenuItem_TerminalMenu_Log_Separator_2,
            this.toolStripMenuItem_TerminalMenu_Log_Settings});
			this.toolStripMenuItem_TerminalMenu_Log.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log.MergeIndex = 3;
			this.toolStripMenuItem_TerminalMenu_Log.Name = "toolStripMenuItem_TerminalMenu_Log";
			this.toolStripMenuItem_TerminalMenu_Log.Size = new System.Drawing.Size(39, 20);
			this.toolStripMenuItem_TerminalMenu_Log.Text = "&Log";
			this.toolStripMenuItem_TerminalMenu_Log.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Begin
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Begin.Name = "toolStripMenuItem_TerminalMenu_Log_Begin";
			this.toolStripMenuItem_TerminalMenu_Log_Begin.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.B)));
			this.toolStripMenuItem_TerminalMenu_Log_Begin.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_TerminalMenu_Log_Begin.Text = "&Begin";
			this.toolStripMenuItem_TerminalMenu_Log_Begin.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_Begin_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_End
			// 
			this.toolStripMenuItem_TerminalMenu_Log_End.Enabled = false;
			this.toolStripMenuItem_TerminalMenu_Log_End.Name = "toolStripMenuItem_TerminalMenu_Log_End";
			this.toolStripMenuItem_TerminalMenu_Log_End.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.E)));
			this.toolStripMenuItem_TerminalMenu_Log_End.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_TerminalMenu_Log_End.Text = "&End";
			this.toolStripMenuItem_TerminalMenu_Log_End.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_End_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1.Name = "toolStripMenuItem_TerminalMenu_Log_Separator_1";
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1.Size = new System.Drawing.Size(174, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Clear
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Enabled = false;
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Name = "toolStripMenuItem_TerminalMenu_Log_Clear";
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Text = "&Clear";
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_Clear_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2.Name = "toolStripMenuItem_TerminalMenu_Log_Separator_2";
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2.Size = new System.Drawing.Size(174, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Settings
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Name = "toolStripMenuItem_TerminalMenu_Log_Settings";
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Size = new System.Drawing.Size(177, 22);
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Text = "&Settings...";
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_Settings_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View
			// 
			this.toolStripMenuItem_TerminalMenu_View.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_Panels,
            this.toolStripMenuItem_TerminalMenu_View_Separator_1,
            this.toolStripMenuItem_TerminalMenu_View_ConnectTime,
            this.toolStripMenuItem_TerminalMenu_View_CountAndRate,
            this.toolStripMenuItem_TerminalMenu_View_Separator_2,
            this.toolStripMenuItem_TerminalMenu_View_Radix,
            this.toolStripMenuItem_TerminalMenu_View_Separator_3,
            this.toolStripMenuItem_TerminalMenu_View_ShowRadix,
            this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp,
            this.toolStripMenuItem_TerminalMenu_View_ShowLength,
            this.toolStripMenuItem_TerminalMenu_View_ShowEol,
            this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers,
            this.toolStripMenuItem_TerminalMenu_View_Separator_4,
            this.toolStripMenuItem_TerminalMenu_View_Format});
			this.toolStripMenuItem_TerminalMenu_View.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_View.MergeIndex = 4;
			this.toolStripMenuItem_TerminalMenu_View.Name = "toolStripMenuItem_TerminalMenu_View";
			this.toolStripMenuItem_TerminalMenu_View.Size = new System.Drawing.Size(44, 20);
			this.toolStripMenuItem_TerminalMenu_View.Text = "&View";
			this.toolStripMenuItem_TerminalMenu_View.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_Panels_Tx,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Rx,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1,
            this.toolStripComboBox_TerminalMenu_View_Panels_Orientation,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3,
            this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand,
            this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4,
            this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll});
			this.toolStripMenuItem_TerminalMenu_View_Panels.Name = "toolStripMenuItem_TerminalMenu_View_Panels";
			this.toolStripMenuItem_TerminalMenu_View_Panels.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels.Text = "&Panels";
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Tx
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Tx";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Text = "&Transmit Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Tx_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Bidir
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Bidir";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Text = "&Bidirectional Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Rx
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Rx";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Text = "&Receive Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Rx_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_1";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1.Size = new System.Drawing.Size(222, 6);
			// 
			// toolStripComboBox_TerminalMenu_View_Panels_Orientation
			// 
			this.toolStripComboBox_TerminalMenu_View_Panels_Orientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_TerminalMenu_View_Panels_Orientation.Name = "toolStripComboBox_TerminalMenu_View_Panels_Orientation";
			this.toolStripComboBox_TerminalMenu_View_Panels_Orientation.Size = new System.Drawing.Size(165, 23);
			this.toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_View_Panels_Orientation_SelectedIndexChanged);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_2";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2.Size = new System.Drawing.Size(222, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Predefined
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Predefined";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Text = "&Predefined Command Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_3";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3.Size = new System.Drawing.Size(222, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_SendCommand
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Name = "toolStripMenuItem_TerminalMenu_View_Panels_SendCommand";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Text = "Send &Command Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_SendCommand_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_SendFile
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Name = "toolStripMenuItem_TerminalMenu_View_Panels_SendFile";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Text = "Send &File Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_4";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4.Size = new System.Drawing.Size(222, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Name = "toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll";
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Size = new System.Drawing.Size(225, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Text = "R&earrange All";
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Rearrange_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_1.Name = "toolStripMenuItem_TerminalMenu_View_Separator_1";
			this.toolStripMenuItem_TerminalMenu_View_Separator_1.Size = new System.Drawing.Size(218, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ConnectTime
			// 
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime,
            this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime});
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime.Name = "toolStripMenuItem_TerminalMenu_View_ConnectTime";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime.Text = "Connect T&ime";
			// 
			// toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime
			// 
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Name = "toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Size = new System.Drawing.Size(188, 22);
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Text = "&Show Connect Time";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime
			// 
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime.Name = "toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime.Size = new System.Drawing.Size(188, 22);
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime.Text = "&Restart Connect Time";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_CountAndRate
			// 
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate,
            this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount});
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate.Name = "toolStripMenuItem_TerminalMenu_View_CountAndRate";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate.Text = "&Count and Rate";
			// 
			// toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate
			// 
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Name = "toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Text = "&Show Byte/Line Count and Rate";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Name = "toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Size = new System.Drawing.Size(241, 22);
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Text = "&Reset Count";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_2.Name = "toolStripMenuItem_TerminalMenu_View_Separator_2";
			this.toolStripMenuItem_TerminalMenu_View_Separator_2.Size = new System.Drawing.Size(218, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_3.Name = "toolStripMenuItem_TerminalMenu_View_Separator_3";
			this.toolStripMenuItem_TerminalMenu_View_Separator_3.Size = new System.Drawing.Size(218, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowRadix
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Name = "toolStripMenuItem_TerminalMenu_View_ShowRadix";
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Text = "Show &Radix";
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowRadix_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowTimeStamp
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Name = "toolStripMenuItem_TerminalMenu_View_ShowTimeStamp";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Text = "Show &Time Stamp";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowLength
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Name = "toolStripMenuItem_TerminalMenu_View_ShowLength";
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Text = "Show &Length";
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowLength_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowEol
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Name = "toolStripMenuItem_TerminalMenu_View_ShowEol";
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Text = "Show &End Of Line Sequence";
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowEol_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowLineNumbers
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Name = "toolStripMenuItem_TerminalMenu_View_ShowLineNumbers";
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Text = "Show Line &Number";
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_4.Name = "toolStripMenuItem_TerminalMenu_View_Separator_4";
			this.toolStripMenuItem_TerminalMenu_View_Separator_4.Size = new System.Drawing.Size(218, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Format
			// 
			this.toolStripMenuItem_TerminalMenu_View_Format.Name = "toolStripMenuItem_TerminalMenu_View_Format";
			this.toolStripMenuItem_TerminalMenu_View_Format.Size = new System.Drawing.Size(221, 22);
			this.toolStripMenuItem_TerminalMenu_View_Format.Text = "&Format...";
			this.toolStripMenuItem_TerminalMenu_View_Format.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Format_Click);
			// 
			// statusStrip_Terminal
			// 
			this.statusStrip_Terminal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_TerminalStatus_Status,
            this.toolStripStatusLabel_TerminalStatus_IOStatus,
            this.toolStripStatusLabel_TerminalStatus_ConnectionState,
            this.toolStripStatusLabel_TerminalStatus_Separator1,
            this.toolStripStatusLabel_TerminalStatus_RTS,
            this.toolStripStatusLabel_TerminalStatus_CTS,
            this.toolStripStatusLabel_TerminalStatus_DTR,
            this.toolStripStatusLabel_TerminalStatus_DSR,
            this.toolStripStatusLabel_TerminalStatus_DCD,
            this.toolStripStatusLabel_TerminalStatus_Separator2,
            this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff,
            this.toolStripStatusLabel_TerminalStatus_InputXOnXOff,
            this.toolStripStatusLabel_TerminalStatus_Separator3,
            this.toolStripStatusLabel_TerminalStatus_OutputBreak,
            this.toolStripStatusLabel_TerminalStatus_InputBreak});
			this.statusStrip_Terminal.Location = new System.Drawing.Point(0, 488);
			this.statusStrip_Terminal.Name = "statusStrip_Terminal";
			this.statusStrip_Terminal.ShowItemToolTips = true;
			this.statusStrip_Terminal.Size = new System.Drawing.Size(712, 25);
			this.statusStrip_Terminal.TabIndex = 2;
			// 
			// toolStripStatusLabel_TerminalStatus_Status
			// 
			this.toolStripStatusLabel_TerminalStatus_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabel_TerminalStatus_Status.Name = "toolStripStatusLabel_TerminalStatus_Status";
			this.toolStripStatusLabel_TerminalStatus_Status.Size = new System.Drawing.Size(203, 20);
			this.toolStripStatusLabel_TerminalStatus_Status.Spring = true;
			this.toolStripStatusLabel_TerminalStatus_Status.Text = "<Terminal Status>";
			this.toolStripStatusLabel_TerminalStatus_Status.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_Status.ToolTipText = "Terminal Status";
			// 
			// toolStripStatusLabel_TerminalStatus_IOStatus
			// 
			this.toolStripStatusLabel_TerminalStatus_IOStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabel_TerminalStatus_IOStatus.Name = "toolStripStatusLabel_TerminalStatus_IOStatus";
			this.toolStripStatusLabel_TerminalStatus_IOStatus.Size = new System.Drawing.Size(75, 20);
			this.toolStripStatusLabel_TerminalStatus_IOStatus.Text = "<I/O Status>";
			this.toolStripStatusLabel_TerminalStatus_IOStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolStripStatusLabel_TerminalStatus_IOStatus.ToolTipText = "Port Status and Settings";
			this.toolStripStatusLabel_TerminalStatus_IOStatus.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_IOStatus_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_ConnectionState
			// 
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.Name = "toolStripStatusLabel_TerminalStatus_ConnectionState";
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.Size = new System.Drawing.Size(16, 20);
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolStripStatusLabel_TerminalStatus_ConnectionState.ToolTipText = "Port Status";
			// 
			// toolStripStatusLabel_TerminalStatus_Separator1
			// 
			this.toolStripStatusLabel_TerminalStatus_Separator1.AutoSize = false;
			this.toolStripStatusLabel_TerminalStatus_Separator1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_Separator1.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_Separator1.Name = "toolStripStatusLabel_TerminalStatus_Separator1";
			this.toolStripStatusLabel_TerminalStatus_Separator1.Size = new System.Drawing.Size(4, 20);
			// 
			// toolStripStatusLabel_TerminalStatus_RTS
			// 
			this.toolStripStatusLabel_TerminalStatus_RTS.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_RTS.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_RTS.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_RTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_RTS.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_RTS.Name = "toolStripStatusLabel_TerminalStatus_RTS";
			this.toolStripStatusLabel_TerminalStatus_RTS.Size = new System.Drawing.Size(43, 20);
			this.toolStripStatusLabel_TerminalStatus_RTS.Text = "RTS";
			this.toolStripStatusLabel_TerminalStatus_RTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_RTS.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_RTS.ToolTipText = "Request to Send (Out)";
			this.toolStripStatusLabel_TerminalStatus_RTS.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_RTS_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_CTS
			// 
			this.toolStripStatusLabel_TerminalStatus_CTS.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_CTS.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_CTS.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_CTS.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_CTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_CTS.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_CTS.Name = "toolStripStatusLabel_TerminalStatus_CTS";
			this.toolStripStatusLabel_TerminalStatus_CTS.Size = new System.Drawing.Size(44, 20);
			this.toolStripStatusLabel_TerminalStatus_CTS.Text = "CTS";
			this.toolStripStatusLabel_TerminalStatus_CTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_CTS.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_CTS.ToolTipText = "Clear to Send (In)";
			// 
			// toolStripStatusLabel_TerminalStatus_DTR
			// 
			this.toolStripStatusLabel_TerminalStatus_DTR.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_DTR.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_DTR.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Off_12x12;
			this.toolStripStatusLabel_TerminalStatus_DTR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DTR.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_DTR.Name = "toolStripStatusLabel_TerminalStatus_DTR";
			this.toolStripStatusLabel_TerminalStatus_DTR.Size = new System.Drawing.Size(45, 20);
			this.toolStripStatusLabel_TerminalStatus_DTR.Text = "DTR";
			this.toolStripStatusLabel_TerminalStatus_DTR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DTR.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_DTR.ToolTipText = "Data Terminal Ready (Out)";
			this.toolStripStatusLabel_TerminalStatus_DTR.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_DTR_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_DSR
			// 
			this.toolStripStatusLabel_TerminalStatus_DSR.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_DSR.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_DSR.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_DSR.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Off_12x12;
			this.toolStripStatusLabel_TerminalStatus_DSR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DSR.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_DSR.Name = "toolStripStatusLabel_TerminalStatus_DSR";
			this.toolStripStatusLabel_TerminalStatus_DSR.Size = new System.Drawing.Size(44, 20);
			this.toolStripStatusLabel_TerminalStatus_DSR.Text = "DSR";
			this.toolStripStatusLabel_TerminalStatus_DSR.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DSR.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_DSR.ToolTipText = "Data Set Ready (In)";
			// 
			// toolStripStatusLabel_TerminalStatus_DCD
			// 
			this.toolStripStatusLabel_TerminalStatus_DCD.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_DCD.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_DCD.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_DCD.Image = global::YAT.Gui.Forms.Properties.Resources.Image_Off_12x12;
			this.toolStripStatusLabel_TerminalStatus_DCD.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DCD.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_DCD.Name = "toolStripStatusLabel_TerminalStatus_DCD";
			this.toolStripStatusLabel_TerminalStatus_DCD.Size = new System.Drawing.Size(47, 20);
			this.toolStripStatusLabel_TerminalStatus_DCD.Text = "DCD";
			this.toolStripStatusLabel_TerminalStatus_DCD.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DCD.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_DCD.ToolTipText = "Data Carrier Detect (In)";
			// 
			// toolStripStatusLabel_TerminalStatus_Separator2
			// 
			this.toolStripStatusLabel_TerminalStatus_Separator2.AutoSize = false;
			this.toolStripStatusLabel_TerminalStatus_Separator2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_Separator2.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_Separator2.Name = "toolStripStatusLabel_TerminalStatus_Separator2";
			this.toolStripStatusLabel_TerminalStatus_Separator2.Size = new System.Drawing.Size(4, 20);
			// 
			// toolStripStatusLabel_TerminalStatus_OutputXOnXOff
			// 
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Name = "toolStripStatusLabel_TerminalStatus_OutputXOnXOff";
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Size = new System.Drawing.Size(45, 20);
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text = "OXS";
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText = "Output XOn/XOff State (Out)";
			// 
			// toolStripStatusLabel_TerminalStatus_InputXOnXOff
			// 
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Name = "toolStripStatusLabel_TerminalStatus_InputXOnXOff";
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Size = new System.Drawing.Size(39, 20);
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text = "IXS";
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText = "Input XOn/XOff State (In)";
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_InputXOnXOff_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_Separator3
			// 
			this.toolStripStatusLabel_TerminalStatus_Separator3.AutoSize = false;
			this.toolStripStatusLabel_TerminalStatus_Separator3.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_Separator3.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_Separator3.Name = "toolStripStatusLabel_TerminalStatus_Separator3";
			this.toolStripStatusLabel_TerminalStatus_Separator3.Size = new System.Drawing.Size(4, 20);
			// 
			// toolStripStatusLabel_TerminalStatus_OutputBreak
			// 
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.Name = "toolStripStatusLabel_TerminalStatus_OutputBreak";
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.Size = new System.Drawing.Size(45, 20);
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.Text = "OBS";
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.ToolTipText = "Output Break State (Out)";
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_OutputBreak_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_InputBreak
			// 
			this.toolStripStatusLabel_TerminalStatus_InputBreak.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_InputBreak.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Image = global::YAT.Gui.Forms.Properties.Resources.Image_On_12x12;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Name = "toolStripStatusLabel_TerminalStatus_InputBreak";
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Size = new System.Drawing.Size(39, 20);
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Text = "IBS";
			this.toolStripStatusLabel_TerminalStatus_InputBreak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ToolTipText = "Input Break State (In)";
			// 
			// splitContainer_Terminal
			// 
			this.splitContainer_Terminal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Terminal.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer_Terminal.IsSplitterFixed = true;
			this.splitContainer_Terminal.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Terminal.Name = "splitContainer_Terminal";
			this.splitContainer_Terminal.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Terminal.Panel1
			// 
			this.splitContainer_Terminal.Panel1.Controls.Add(this.splitContainer_Predefined);
			// 
			// splitContainer_Terminal.Panel2
			// 
			this.splitContainer_Terminal.Panel2.Controls.Add(this.panel_Send);
			this.splitContainer_Terminal.Panel2MinSize = 97;
			this.splitContainer_Terminal.Size = new System.Drawing.Size(712, 488);
			this.splitContainer_Terminal.SplitterDistance = 390;
			this.splitContainer_Terminal.SplitterWidth = 1;
			this.splitContainer_Terminal.TabIndex = 1;
			this.splitContainer_Terminal.TabStop = false;
			// 
			// splitContainer_Predefined
			// 
			this.splitContainer_Predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Predefined.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Predefined.Name = "splitContainer_Predefined";
			// 
			// splitContainer_Predefined.Panel1
			// 
			this.splitContainer_Predefined.Panel1.Controls.Add(this.panel_Monitor);
			this.splitContainer_Predefined.Panel1MinSize = 164;
			// 
			// splitContainer_Predefined.Panel2
			// 
			this.splitContainer_Predefined.Panel2.Controls.Add(this.panel_Predefined);
			this.splitContainer_Predefined.Panel2MinSize = 136;
			this.splitContainer_Predefined.Size = new System.Drawing.Size(712, 390);
			this.splitContainer_Predefined.SplitterDistance = 536;
			this.splitContainer_Predefined.TabIndex = 0;
			this.splitContainer_Predefined.TabStop = false;
			this.splitContainer_Predefined.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_Predefined_SplitterMoved);
			// 
			// panel_Monitor
			// 
			this.panel_Monitor.Controls.Add(this.groupBox_Monitor);
			this.panel_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor.Name = "panel_Monitor";
			this.panel_Monitor.Padding = new System.Windows.Forms.Padding(3, 3, 1, 0);
			this.panel_Monitor.Size = new System.Drawing.Size(536, 390);
			this.panel_Monitor.TabIndex = 0;
			// 
			// groupBox_Monitor
			// 
			this.groupBox_Monitor.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.groupBox_Monitor.Controls.Add(this.splitContainer_TxMonitor);
			this.groupBox_Monitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_Monitor.Location = new System.Drawing.Point(3, 3);
			this.groupBox_Monitor.Name = "groupBox_Monitor";
			this.groupBox_Monitor.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.groupBox_Monitor.Size = new System.Drawing.Size(532, 387);
			this.groupBox_Monitor.TabIndex = 0;
			this.groupBox_Monitor.TabStop = false;
			this.groupBox_Monitor.Text = "Monitor";
			// 
			// splitContainer_TxMonitor
			// 
			this.splitContainer_TxMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_TxMonitor.Location = new System.Drawing.Point(3, 13);
			this.splitContainer_TxMonitor.Name = "splitContainer_TxMonitor";
			// 
			// splitContainer_TxMonitor.Panel1
			// 
			this.splitContainer_TxMonitor.Panel1.Controls.Add(this.panel_Monitor_Tx);
			this.splitContainer_TxMonitor.Panel1MinSize = 48;
			// 
			// splitContainer_TxMonitor.Panel2
			// 
			this.splitContainer_TxMonitor.Panel2.Controls.Add(this.splitContainer_RxMonitor);
			this.splitContainer_TxMonitor.Panel2MinSize = 100;
			this.splitContainer_TxMonitor.Size = new System.Drawing.Size(526, 371);
			this.splitContainer_TxMonitor.SplitterDistance = 172;
			this.splitContainer_TxMonitor.TabIndex = 0;
			this.splitContainer_TxMonitor.TabStop = false;
			this.splitContainer_TxMonitor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_TxMonitor_SplitterMoved);
			// 
			// panel_Monitor_Tx
			// 
			this.panel_Monitor_Tx.Controls.Add(this.monitor_Tx);
			this.panel_Monitor_Tx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor_Tx.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor_Tx.Name = "panel_Monitor_Tx";
			this.panel_Monitor_Tx.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Monitor_Tx.Size = new System.Drawing.Size(172, 371);
			this.panel_Monitor_Tx.TabIndex = 0;
			// 
			// monitor_Tx
			// 
			this.monitor_Tx.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Tx.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.monitor_Tx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitor_Tx.Location = new System.Drawing.Point(3, 0);
			this.monitor_Tx.Name = "monitor_Tx";
			this.monitor_Tx.RepositoryType = YAT.Domain.RepositoryType.Tx;
			this.monitor_Tx.Size = new System.Drawing.Size(166, 368);
			this.monitor_Tx.TabIndex = 0;
			this.monitor_Tx.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Tx.Enter += new System.EventHandler(this.monitor_Tx_Enter);
			// 
			// splitContainer_RxMonitor
			// 
			this.splitContainer_RxMonitor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_RxMonitor.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_RxMonitor.Name = "splitContainer_RxMonitor";
			// 
			// splitContainer_RxMonitor.Panel1
			// 
			this.splitContainer_RxMonitor.Panel1.Controls.Add(this.panel_Monitor_Bidir);
			this.splitContainer_RxMonitor.Panel1MinSize = 48;
			// 
			// splitContainer_RxMonitor.Panel2
			// 
			this.splitContainer_RxMonitor.Panel2.Controls.Add(this.panel_Monitor_Rx);
			this.splitContainer_RxMonitor.Panel2MinSize = 48;
			this.splitContainer_RxMonitor.Size = new System.Drawing.Size(350, 371);
			this.splitContainer_RxMonitor.SplitterDistance = 175;
			this.splitContainer_RxMonitor.TabIndex = 1;
			this.splitContainer_RxMonitor.TabStop = false;
			this.splitContainer_RxMonitor.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer_RxMonitor_SplitterMoved);
			// 
			// panel_Monitor_Bidir
			// 
			this.panel_Monitor_Bidir.Controls.Add(this.monitor_Bidir);
			this.panel_Monitor_Bidir.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor_Bidir.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor_Bidir.Name = "panel_Monitor_Bidir";
			this.panel_Monitor_Bidir.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Monitor_Bidir.Size = new System.Drawing.Size(175, 371);
			this.panel_Monitor_Bidir.TabIndex = 0;
			// 
			// monitor_Bidir
			// 
			this.monitor_Bidir.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Bidir.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.monitor_Bidir.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitor_Bidir.Location = new System.Drawing.Point(3, 0);
			this.monitor_Bidir.Name = "monitor_Bidir";
			this.monitor_Bidir.RepositoryType = YAT.Domain.RepositoryType.Bidir;
			this.monitor_Bidir.Size = new System.Drawing.Size(169, 368);
			this.monitor_Bidir.TabIndex = 0;
			this.monitor_Bidir.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Bidir.Enter += new System.EventHandler(this.monitor_Bidir_Enter);
			// 
			// panel_Monitor_Rx
			// 
			this.panel_Monitor_Rx.Controls.Add(this.monitor_Rx);
			this.panel_Monitor_Rx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor_Rx.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor_Rx.Name = "panel_Monitor_Rx";
			this.panel_Monitor_Rx.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Monitor_Rx.Size = new System.Drawing.Size(171, 371);
			this.panel_Monitor_Rx.TabIndex = 0;
			// 
			// monitor_Rx
			// 
			this.monitor_Rx.ConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Rx.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.monitor_Rx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitor_Rx.Location = new System.Drawing.Point(3, 0);
			this.monitor_Rx.Name = "monitor_Rx";
			this.monitor_Rx.RepositoryType = YAT.Domain.RepositoryType.Rx;
			this.monitor_Rx.Size = new System.Drawing.Size(165, 368);
			this.monitor_Rx.TabIndex = 0;
			this.monitor_Rx.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Rx.Enter += new System.EventHandler(this.monitor_Rx_Enter);
			// 
			// panel_Predefined
			// 
			this.panel_Predefined.Controls.Add(this.groupBox_Predefined);
			this.panel_Predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Predefined.Location = new System.Drawing.Point(0, 0);
			this.panel_Predefined.Name = "panel_Predefined";
			this.panel_Predefined.Padding = new System.Windows.Forms.Padding(1, 3, 3, 0);
			this.panel_Predefined.Size = new System.Drawing.Size(172, 390);
			this.panel_Predefined.TabIndex = 0;
			// 
			// groupBox_Predefined
			// 
			this.groupBox_Predefined.ContextMenuStrip = this.contextMenuStrip_Predefined;
			this.groupBox_Predefined.Controls.Add(this.predefined);
			this.groupBox_Predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_Predefined.Location = new System.Drawing.Point(1, 3);
			this.groupBox_Predefined.Name = "groupBox_Predefined";
			this.groupBox_Predefined.Padding = new System.Windows.Forms.Padding(3, 9, 3, 3);
			this.groupBox_Predefined.Size = new System.Drawing.Size(168, 387);
			this.groupBox_Predefined.TabIndex = 0;
			this.groupBox_Predefined.TabStop = false;
			this.groupBox_Predefined.Text = "&Predefined Commands";
			// 
			// predefined
			// 
			this.predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.predefined.Location = new System.Drawing.Point(3, 22);
			this.predefined.Name = "predefined";
			this.predefined.Size = new System.Drawing.Size(162, 362);
			this.predefined.TabIndex = 0;
			this.predefined.SelectedPageChanged += new System.EventHandler(this.predefined_SelectedPageChanged);
			this.predefined.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.predefined_DefineCommandRequest);
			this.predefined.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.predefined_SendCommandRequest);
			// 
			// panel_Send
			// 
			this.panel_Send.Controls.Add(this.send);
			this.panel_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Send.Location = new System.Drawing.Point(0, 0);
			this.panel_Send.Name = "panel_Send";
			this.panel_Send.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Send.Size = new System.Drawing.Size(712, 97);
			this.panel_Send.TabIndex = 1;
			// 
			// send
			// 
			this.send.ContextMenuStrip = this.contextMenuStrip_Send;
			this.send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.send.Location = new System.Drawing.Point(3, 0);
			this.send.Name = "send";
			this.send.Size = new System.Drawing.Size(706, 94);
			this.send.TabIndex = 0;
			this.send.EditFocusStateChanged += new System.EventHandler(this.send_EditFocusStateChanged);
			this.send.SendFileCommandRequest += new System.EventHandler(this.send_SendFileCommandRequest);
			this.send.FileCommandChanged += new System.EventHandler(this.send_FileCommandChanged);
			this.send.SendCommandRequest += new System.EventHandler(this.send_SendCommandRequest);
			this.send.CommandChanged += new System.EventHandler(this.send_CommandChanged);
			// 
			// timer_RtsLuminescence
			// 
			this.timer_RtsLuminescence.Tick += new System.EventHandler(this.timer_RtsLuminescence_Tick);
			// 
			// Terminal
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(712, 513);
			this.Controls.Add(this.splitContainer_Terminal);
			this.Controls.Add(this.menuStrip_Terminal);
			this.Controls.Add(this.statusStrip_Terminal);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "Terminal";
			this.Text = "Terminal";
			this.SizeChanged += new System.EventHandler(this.Terminal_SizeChanged);
			this.Shown += new System.EventHandler(this.Terminal_Shown);
			this.Activated += new System.EventHandler(this.Terminal_Activated);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Terminal_FormClosed);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Terminal_FormClosing);
			this.LocationChanged += new System.EventHandler(this.Terminal_LocationChanged);
			this.contextMenuStrip_Monitor.ResumeLayout(false);
			this.contextMenuStrip_Radix.ResumeLayout(false);
			this.contextMenuStrip_Predefined.ResumeLayout(false);
			this.contextMenuStrip_Send.ResumeLayout(false);
			this.menuStrip_Terminal.ResumeLayout(false);
			this.menuStrip_Terminal.PerformLayout();
			this.contextMenuStrip_Preset.ResumeLayout(false);
			this.statusStrip_Terminal.ResumeLayout(false);
			this.statusStrip_Terminal.PerformLayout();
			this.splitContainer_Terminal.Panel1.ResumeLayout(false);
			this.splitContainer_Terminal.Panel2.ResumeLayout(false);
			this.splitContainer_Terminal.ResumeLayout(false);
			this.splitContainer_Predefined.Panel1.ResumeLayout(false);
			this.splitContainer_Predefined.Panel2.ResumeLayout(false);
			this.splitContainer_Predefined.ResumeLayout(false);
			this.panel_Monitor.ResumeLayout(false);
			this.groupBox_Monitor.ResumeLayout(false);
			this.splitContainer_TxMonitor.Panel1.ResumeLayout(false);
			this.splitContainer_TxMonitor.Panel2.ResumeLayout(false);
			this.splitContainer_TxMonitor.ResumeLayout(false);
			this.panel_Monitor_Tx.ResumeLayout(false);
			this.splitContainer_RxMonitor.Panel1.ResumeLayout(false);
			this.splitContainer_RxMonitor.Panel2.ResumeLayout(false);
			this.splitContainer_RxMonitor.ResumeLayout(false);
			this.panel_Monitor_Bidir.ResumeLayout(false);
			this.panel_Monitor_Rx.ResumeLayout(false);
			this.panel_Predefined.ResumeLayout(false);
			this.groupBox_Predefined.ResumeLayout(false);
			this.panel_Send.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Timer timer_Status;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Monitor;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Radix;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowTimeStamp;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowLength;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Clear;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels_Tx;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels_Bidir;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels_Rx;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Hide;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_SaveToFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_CopyToClipboard;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Print;
		private System.Windows.Forms.MenuStrip menuStrip_Terminal;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_File;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_File_Close;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_File_Save;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_File_SaveAs;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Start;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Stop;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Settings;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_Command;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_File;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_Predefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_Begin;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_End;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Log_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_Clear;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Log_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_Settings;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View;
		private System.Windows.Forms.StatusStrip statusStrip_Terminal;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_Status;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_IOStatus;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_RTS;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_CTS;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_DTR;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_DSR;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_DCD;
		private System.Windows.Forms.SplitContainer splitContainer_Terminal;
		private System.Windows.Forms.SplitContainer splitContainer_Predefined;
		private System.Windows.Forms.Panel panel_Monitor;
		private System.Windows.Forms.GroupBox groupBox_Monitor;
		private System.Windows.Forms.SplitContainer splitContainer_TxMonitor;
		private System.Windows.Forms.Panel panel_Predefined;
		private System.Windows.Forms.GroupBox groupBox_Predefined;
		private YAT.Gui.Controls.PredefinedCommands predefined;
		private System.Windows.Forms.Panel panel_Monitor_Tx;
		private YAT.Gui.Controls.Monitor monitor_Tx;
		private System.Windows.Forms.SplitContainer splitContainer_RxMonitor;
		private System.Windows.Forms.Panel panel_Monitor_Bidir;
		private YAT.Gui.Controls.Monitor monitor_Bidir;
		private System.Windows.Forms.Panel panel_Monitor_Rx;
		private YAT.Gui.Controls.Monitor monitor_Rx;
		private System.Windows.Forms.Timer timer_RtsLuminescence;
		private System.Windows.Forms.ToolTip toolTip;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Radix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_String;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Char;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Bin;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Oct;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Dec;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Hex;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ResetCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowCountAndRate;
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
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_Next;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_Previous;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Define;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyToSendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyFromSendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Hide;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Send;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels_SendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels_SendFile;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_ConnectionState;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Tx;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Bidir;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Rx;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Predefined;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_SendCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_SendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Radix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_CountAndRate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Format;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowTimeStamp;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowLength;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_3;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Clear;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_4;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_TerminalMenu_View_Panels_Orientation;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Presets;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowEol;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowEol;
		private System.Windows.Forms.Panel panel_Send;
		private YAT.Gui.Controls.Send send;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Preset;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Format;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_SeparateTxRx;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_TxRadix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_String;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_Char;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Tx_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_Bin;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_Oct;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_Dec;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_Hex;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_RxRadix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_String;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Char;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Rx_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Bin;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Oct;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Dec;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Hex;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_KeepCommand;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_KeepCommand;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_CopyPredefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_CopyPredefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_RestartConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ConnectTime_RestartConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_SelectAll;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_SelectNone;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Print;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_SelectAll;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_SelectNone;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_7;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_SaveToFile;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_MonitorContextMenu_Panels_Orientation;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowRadix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowRadix;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Page_Separator;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page_12;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendImmediately;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_SendImmediately;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_OutputBreak;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_InputBreak;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_Separator2;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_Separator1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Panels_Separator1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_OutputXOnXOff;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_Separator3;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_InputXOnXOff;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowLineNumbers;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowLineNumbers;
	}

	#region Copied Designer Code For Backup
	//------------------------------------------------------------------------------------------
	// Under some circumstances, the send panel gets messed up
	// - Could be, that this happens when sendCommand and sendFile are docked to Fill
	// - Could have some other reason
	// Therefore, following a backup of the designer code
	//------------------------------------------------------------------------------------------

	/*

			// 
			// splitContainer_Terminal
			// 
			this.splitContainer_Terminal.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Terminal.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer_Terminal.IsSplitterFixed = true;
			this.splitContainer_Terminal.Location = new System.Drawing.Point(0, 0);
			this.splitContainer_Terminal.Name = "splitContainer_Terminal";
			this.splitContainer_Terminal.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Terminal.Panel1
			// 
			this.splitContainer_Terminal.Panel1.Controls.Add(this.splitContainer_Predefined);
			// 
			// splitContainer_Terminal.Panel2
			// 
			this.splitContainer_Terminal.Panel2.Controls.Add(this.panel_Send);
			this.splitContainer_Terminal.Panel2MinSize = 97;
			this.splitContainer_Terminal.Size = new System.Drawing.Size(712, 491);
			this.splitContainer_Terminal.SplitterDistance = 393;
			this.splitContainer_Terminal.SplitterWidth = 1;
			this.splitContainer_Terminal.TabIndex = 1;
			this.splitContainer_Terminal.TabStop = false;
			// 
			// panel_Send
			// 
			this.panel_Send.Controls.Add(this.splitContainer_Send);
			this.panel_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Send.Location = new System.Drawing.Point(0, 0);
			this.panel_Send.Name = "panel_Send";
			this.panel_Send.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Send.Size = new System.Drawing.Size(712, 96);
			this.panel_Send.TabIndex = 1;
			// 
			// splitContainer_Send
			// 
			this.splitContainer_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer_Send.IsSplitterFixed = true;
			this.splitContainer_Send.Location = new System.Drawing.Point(3, 0);
			this.splitContainer_Send.Name = "splitContainer_Send";
			this.splitContainer_Send.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer_Send.Panel1
			// 
			this.splitContainer_Send.Panel1.Controls.Add(this.groupBox_SendCommand);
			this.splitContainer_Send.Panel1MinSize = 46;
			// 
			// splitContainer_Send.Panel2
			// 
			this.splitContainer_Send.Panel2.Controls.Add(this.groupBox_SendFile);
			this.splitContainer_Send.Panel2MinSize = 46;
			this.splitContainer_Send.Size = new System.Drawing.Size(706, 93);
			this.splitContainer_Send.SplitterDistance = 46;
			this.splitContainer_Send.SplitterWidth = 1;
			this.splitContainer_Send.TabIndex = 0;
			// 
			// groupBox_SendCommand
			// 
			this.groupBox_SendCommand.Controls.Add(this.sendCommand);
			this.groupBox_SendCommand.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_SendCommand.Location = new System.Drawing.Point(0, 0);
			this.groupBox_SendCommand.Name = "groupBox_SendCommand";
			this.groupBox_SendCommand.Size = new System.Drawing.Size(706, 46);
			this.groupBox_SendCommand.TabIndex = 1;
			this.groupBox_SendCommand.TabStop = false;
			this.groupBox_SendCommand.Text = "Send &Command";
			// 
			// sendCommand
			// 
			this.sendCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			command1.CommandLines = new string[] {
		"<1 lines...> []"};
			command1.DefaultRadix = YAT.Domain.Radix.String;
			command1.Description = "<1 lines...> []";
			command1.FilePath = "";
			command1.IsFilePath = false;
			command1.MultiLineCommand = new string[] {
		"<1 lines...> []"};
			command1.SingleLineCommand = "<1 lines...> []";
			this.sendCommand.Command = command1;
			this.sendCommand.Location = new System.Drawing.Point(3, 12);
			this.sendCommand.Name = "sendCommand";
			this.sendCommand.Size = new System.Drawing.Size(700, 27);
			this.sendCommand.TabIndex = 0;
			this.sendCommand.SendCommandRequest += new System.EventHandler(this.sendCommand_SendCommandRequest);
			this.sendCommand.CommandChanged += new System.EventHandler(this.sendCommand_CommandChanged);
			// 
			// groupBox_SendFile
			// 
			this.groupBox_SendFile.Controls.Add(this.sendFile);
			this.groupBox_SendFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_SendFile.Location = new System.Drawing.Point(0, 0);
			this.groupBox_SendFile.Name = "groupBox_SendFile";
			this.groupBox_SendFile.Size = new System.Drawing.Size(706, 46);
			this.groupBox_SendFile.TabIndex = 1;
			this.groupBox_SendFile.TabStop = false;
			this.groupBox_SendFile.Text = "Send &File";
			// 
			// sendFile
			// 
			this.sendFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			command2.CommandLines = new string[] {
		"<1 lines...> []"};
			command2.DefaultRadix = YAT.Domain.Radix.String;
			command2.Description = "<1 lines...> []";
			command2.FilePath = "";
			command2.IsFilePath = false;
			command2.MultiLineCommand = new string[] {
		"<1 lines...> []"};
			command2.SingleLineCommand = "<1 lines...> []";
			this.sendFile.Command = command2;
			this.sendFile.Location = new System.Drawing.Point(3, 12);
			this.sendFile.Name = "sendFile";
			this.sendFile.Size = new System.Drawing.Size(700, 27);
			this.sendFile.TabIndex = 0;
			this.sendFile.SendCommandRequest += new System.EventHandler(this.sendFile_SendCommandRequest);
			this.sendFile.CommandChanged += new System.EventHandler(this.sendFile_CommandChanged);

			*/

	#endregion

}
