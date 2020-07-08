using MKY;

namespace YAT.View.Forms
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Terminal));
			this.timer_StatusText = new System.Windows.Forms.Timer(this.components);
			this.contextMenuStrip_Monitor = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_MonitorContextMenu_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_MonitorContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
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
			this.toolStripMenuItem_RadixContextMenu_Unicode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_TxRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_String = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Char = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Tx_Unicode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_RxRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_String = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Char = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_RadixContextMenu_Rx_Unicode = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Radix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowDevice = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowDirection = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowEol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowLength = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_ShowDuration = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_Format = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_8 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Refresh = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_9 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_SelectNone = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_10 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Print = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_Separator_11 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_MonitorContextMenu_Find = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_FindNext = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_MonitorContextMenu_FindPrevious = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Predefined = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_PredefinedContextMenu_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Layout = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripComboBox_PredefinedContextMenu_Layout = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Command = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Command = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_CommandContextMenu_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_CommandContextMenu_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_13 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_14 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_15 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_16 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_17 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_18 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_19 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_20 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_21 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_22 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_23 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_24 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_36 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_48 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_60 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_72 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_84 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_96 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_12 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_13 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_14 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_15 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_16 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_17 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_18 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_19 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_20 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_21 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_22 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_23 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_24 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_36 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_48 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_60 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_72 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_84 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_96 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Cut = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Copy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Paste = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Define = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_Page = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Page = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_PageContextMenu_Next = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_Previous = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_Separator = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PageContextMenu_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PageContextMenu_9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Send = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_SendContextMenu_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Panels_SendText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_SendText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_AllowConcurrency = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_KeepSendText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_SendImmediately = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_SkipEmptyLines = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_SendContextMenu_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_SendContextMenu_CopyPredefined = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip_Terminal = new MKY.Windows.Forms.MenuStripEx();
			this.toolStripMenuItem_TerminalMenu_File = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_File_Close = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_File_Save = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_File_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Start = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_Break = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Print = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_Find = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Preset = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.toolStripMenuItem_PresetContextMenu_Preset_1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_PresetContextMenu_Preset_8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Text = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_File = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_KeepSendText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger = new MKY.Windows.Forms.ToolStripComboBoxEx();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_On = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Off = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Log_Clear = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_Log_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_Log_Settings = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripComboBox_TerminalMenu_View_Panels_PageLayout = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_5 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowDevice = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowDirection = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowEol = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowLength = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ShowDuration = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_6 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_7 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_BreakCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_Separator_8 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem_TerminalMenu_View_Format = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip_Terminal = new MKY.Windows.Forms.StatusStripEx();
			this.contextMenuStrip_Status = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.contextMenuStrip_Status_FlowControlCount = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Status_FlowControlCount_ShowCount = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Status_FlowControlCount_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Status_BreakCount = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Status_BreakCount_ShowCount = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Status_BreakCount_ResetCount = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripStatusLabel_TerminalStatus_Status = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_IOStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_Separator1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_RTS = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_CTS = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_DTR = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_DSR = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_DCD = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_Separator2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_Separator3 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_InputBreak = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel_TerminalStatus_OutputBreak = new System.Windows.Forms.ToolStripStatusLabel();
			this.splitContainer_Terminal = new System.Windows.Forms.SplitContainer();
			this.splitContainer_Predefined = new System.Windows.Forms.SplitContainer();
			this.panel_Monitor = new System.Windows.Forms.Panel();
			this.groupBox_Monitor = new System.Windows.Forms.GroupBox();
			this.splitContainer_TxMonitor = new System.Windows.Forms.SplitContainer();
			this.panel_Monitor_Tx = new System.Windows.Forms.Panel();
			this.monitor_Tx = new YAT.View.Controls.Monitor();
			this.splitContainer_RxMonitor = new System.Windows.Forms.SplitContainer();
			this.panel_Monitor_Bidir = new System.Windows.Forms.Panel();
			this.monitor_Bidir = new YAT.View.Controls.Monitor();
			this.panel_Monitor_Rx = new System.Windows.Forms.Panel();
			this.monitor_Rx = new YAT.View.Controls.Monitor();
			this.panel_Predefined = new System.Windows.Forms.Panel();
			this.groupBox_Predefined = new System.Windows.Forms.GroupBox();
			this.predefined = new YAT.View.Controls.PredefinedCommands();
			this.panel_Send = new System.Windows.Forms.Panel();
			this.send = new YAT.View.Controls.Send();
			this.timer_RtsLuminescence = new System.Windows.Forms.Timer(this.components);
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.timer_IOStatusIndicator = new System.Windows.Forms.Timer(this.components);
			this.timer_AutoActionCountUpdate = new System.Windows.Forms.Timer(this.components);
			this.timer_AutoResponseCountUpdate = new System.Windows.Forms.Timer(this.components);
			this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_Monitor.SuspendLayout();
			this.contextMenuStrip_Radix.SuspendLayout();
			this.contextMenuStrip_Predefined.SuspendLayout();
			this.contextMenuStrip_Command.SuspendLayout();
			this.contextMenuStrip_Page.SuspendLayout();
			this.contextMenuStrip_Send.SuspendLayout();
			this.menuStrip_Terminal.SuspendLayout();
			this.contextMenuStrip_Preset.SuspendLayout();
			this.statusStrip_Terminal.SuspendLayout();
			this.contextMenuStrip_Status.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Terminal)).BeginInit();
			this.splitContainer_Terminal.Panel1.SuspendLayout();
			this.splitContainer_Terminal.Panel2.SuspendLayout();
			this.splitContainer_Terminal.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Predefined)).BeginInit();
			this.splitContainer_Predefined.Panel1.SuspendLayout();
			this.splitContainer_Predefined.Panel2.SuspendLayout();
			this.splitContainer_Predefined.SuspendLayout();
			this.panel_Monitor.SuspendLayout();
			this.groupBox_Monitor.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_TxMonitor)).BeginInit();
			this.splitContainer_TxMonitor.Panel1.SuspendLayout();
			this.splitContainer_TxMonitor.Panel2.SuspendLayout();
			this.splitContainer_TxMonitor.SuspendLayout();
			this.panel_Monitor_Tx.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_RxMonitor)).BeginInit();
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
			// timer_StatusText
			// 
			this.timer_StatusText.Tick += new System.EventHandler(this.timer_StatusText_Tick);
			// 
			// contextMenuStrip_Monitor
			// 
			this.contextMenuStrip_Monitor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_MonitorContextMenu_Panels,
            this.toolStripMenuItem_MonitorContextMenu_Separator_1,
            this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime,
            this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime,
            this.toolStripMenuItem_MonitorContextMenu_Separator_2,
            this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate,
            this.toolStripMenuItem_MonitorContextMenu_ResetCount,
            this.toolStripMenuItem_MonitorContextMenu_Separator_3,
            this.toolStripMenuItem_MonitorContextMenu_Radix,
            this.toolStripMenuItem_MonitorContextMenu_ShowRadix,
            this.toolStripMenuItem_MonitorContextMenu_Separator_4,
            this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers,
            this.toolStripMenuItem_MonitorContextMenu_Separator_5,
            this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp,
            this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan,
            this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta,
            this.toolStripMenuItem_MonitorContextMenu_ShowDevice,
            this.toolStripMenuItem_MonitorContextMenu_ShowDirection,
            this.toolStripMenuItem_MonitorContextMenu_ShowEol,
            this.toolStripMenuItem_MonitorContextMenu_ShowLength,
            this.toolStripMenuItem_MonitorContextMenu_ShowDuration,
            this.toolStripMenuItem_MonitorContextMenu_Separator_6,
            this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine,
            this.toolStripMenuItem_MonitorContextMenu_Separator_7,
            this.toolStripMenuItem_MonitorContextMenu_Format,
            this.toolStripMenuItem_MonitorContextMenu_Separator_8,
            this.toolStripMenuItem_MonitorContextMenu_Clear,
            this.toolStripMenuItem_MonitorContextMenu_Refresh,
            this.toolStripMenuItem_MonitorContextMenu_Separator_9,
            this.toolStripMenuItem_MonitorContextMenu_SelectAll,
            this.toolStripMenuItem_MonitorContextMenu_SelectNone,
            this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard,
            this.toolStripMenuItem_MonitorContextMenu_Separator_10,
            this.toolStripMenuItem_MonitorContextMenu_SaveToFile,
            this.toolStripMenuItem_MonitorContextMenu_Print,
            this.toolStripMenuItem_MonitorContextMenu_Separator_11,
            this.toolStripMenuItem_MonitorContextMenu_Find,
            this.toolStripMenuItem_MonitorContextMenu_FindNext,
            this.toolStripMenuItem_MonitorContextMenu_FindPrevious});
			this.contextMenuStrip_Monitor.Name = "contextMenuStrip_Monitor";
			this.contextMenuStrip_Monitor.Size = new System.Drawing.Size(267, 686);
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
			this.toolStripMenuItem_MonitorContextMenu_Panels.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels.Text = "Panels";
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Tx
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Tx";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Text = "Unidirectional Send (Tx) Panel";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Tx.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Panels_Tx_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Bidir
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Checked = true;
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Bidir";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Text = "Bidirectional Send/Receive Panel";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Panels_Bidir_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Rx
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Rx";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Text = "Unidirectional Receive (Rx) Panel";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Rx.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Panels_Rx_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Panels_Separator1
			// 
			this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1.Name = "toolStripMenuItem_MonitorContextMenu_Panels_Separator1";
			this.toolStripMenuItem_MonitorContextMenu_Panels_Separator1.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripComboBox_MonitorContextMenu_Panels_Orientation
			// 
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.Name = "toolStripComboBox_MonitorContextMenu_Panels_Orientation";
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.Size = new System.Drawing.Size(165, 23);
			this.toolStripComboBox_MonitorContextMenu_Panels_Orientation.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_MonitorContextMenu_Panels_Orientation_SelectedIndexChanged);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_1
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_1.Name = "toolStripMenuItem_MonitorContextMenu_Separator_1";
			this.toolStripMenuItem_MonitorContextMenu_Separator_1.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowConnectTime
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Name = "toolStripMenuItem_MonitorContextMenu_ShowConnectTime";
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Text = "Show Connect Time";
			this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowConnectTime_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ResetConnectTime
			// 
			this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime.Name = "toolStripMenuItem_MonitorContextMenu_ResetConnectTime";
			this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime.Text = "Reset Connect Time";
			this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ResetConnectTime_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_2
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_2.Name = "toolStripMenuItem_MonitorContextMenu_Separator_2";
			this.toolStripMenuItem_MonitorContextMenu_Separator_2.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowCountAndRate
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Name = "toolStripMenuItem_MonitorContextMenu_ShowCountAndRate";
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Text = "Show Byte/Line Count/Rate";
			this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowCountAndRate_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ResetCount
			// 
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Name = "toolStripMenuItem_MonitorContextMenu_ResetCount";
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Text = "Reset Byte/Line Count/Rate";
			this.toolStripMenuItem_MonitorContextMenu_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ResetCount_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_3
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_3.Name = "toolStripMenuItem_MonitorContextMenu_Separator_3";
			this.toolStripMenuItem_MonitorContextMenu_Separator_3.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_Radix
			// 
			this.toolStripMenuItem_MonitorContextMenu_Radix.DropDown = this.contextMenuStrip_Radix;
			this.toolStripMenuItem_MonitorContextMenu_Radix.Name = "toolStripMenuItem_MonitorContextMenu_Radix";
			this.toolStripMenuItem_MonitorContextMenu_Radix.Size = new System.Drawing.Size(266, 22);
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
            this.toolStripMenuItem_RadixContextMenu_Unicode,
            this.toolStripMenuItem_RadixContextMenu_Separator_3,
            this.toolStripMenuItem_RadixContextMenu_SeparateTxRx,
            this.toolStripMenuItem_RadixContextMenu_Separator_4,
            this.toolStripMenuItem_RadixContextMenu_TxRadix,
            this.toolStripMenuItem_RadixContextMenu_RxRadix});
			this.contextMenuStrip_Radix.Name = "contextMenuStrip_Radix";
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
			this.contextMenuStrip_Radix.OwnerItem = this.toolStripMenuItem_TerminalMenu_View_Radix;
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
			this.contextMenuStrip_Radix.Size = new System.Drawing.Size(151, 248);
			this.contextMenuStrip_Radix.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Radix_Opening);
			// 
			// toolStripMenuItem_RadixContextMenu_String
			// 
			this.toolStripMenuItem_RadixContextMenu_String.Checked = true;
			this.toolStripMenuItem_RadixContextMenu_String.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_RadixContextMenu_String.Name = "toolStripMenuItem_RadixContextMenu_String";
			this.toolStripMenuItem_RadixContextMenu_String.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_String.Text = "&String";
			this.toolStripMenuItem_RadixContextMenu_String.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_String_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Char
			// 
			this.toolStripMenuItem_RadixContextMenu_Char.Name = "toolStripMenuItem_RadixContextMenu_Char";
			this.toolStripMenuItem_RadixContextMenu_Char.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_Char.Text = "&Character";
			this.toolStripMenuItem_RadixContextMenu_Char.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Char_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_1
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_1.Name = "toolStripMenuItem_RadixContextMenu_Separator_1";
			this.toolStripMenuItem_RadixContextMenu_Separator_1.Size = new System.Drawing.Size(148, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Bin
			// 
			this.toolStripMenuItem_RadixContextMenu_Bin.Name = "toolStripMenuItem_RadixContextMenu_Bin";
			this.toolStripMenuItem_RadixContextMenu_Bin.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_Bin.Text = "&Binary";
			this.toolStripMenuItem_RadixContextMenu_Bin.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Bin_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Oct
			// 
			this.toolStripMenuItem_RadixContextMenu_Oct.Name = "toolStripMenuItem_RadixContextMenu_Oct";
			this.toolStripMenuItem_RadixContextMenu_Oct.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_Oct.Text = "&Octal";
			this.toolStripMenuItem_RadixContextMenu_Oct.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Oct_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Dec
			// 
			this.toolStripMenuItem_RadixContextMenu_Dec.Name = "toolStripMenuItem_RadixContextMenu_Dec";
			this.toolStripMenuItem_RadixContextMenu_Dec.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_Dec.Text = "&Decimal";
			this.toolStripMenuItem_RadixContextMenu_Dec.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Dec_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Hex
			// 
			this.toolStripMenuItem_RadixContextMenu_Hex.Name = "toolStripMenuItem_RadixContextMenu_Hex";
			this.toolStripMenuItem_RadixContextMenu_Hex.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_Hex.Text = "&Hexadecimal";
			this.toolStripMenuItem_RadixContextMenu_Hex.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Hex_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_2
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_2.Name = "toolStripMenuItem_RadixContextMenu_Separator_2";
			this.toolStripMenuItem_RadixContextMenu_Separator_2.Size = new System.Drawing.Size(148, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Unicode
			// 
			this.toolStripMenuItem_RadixContextMenu_Unicode.Name = "toolStripMenuItem_RadixContextMenu_Unicode";
			this.toolStripMenuItem_RadixContextMenu_Unicode.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_Unicode.Text = "&Unicode";
			this.toolStripMenuItem_RadixContextMenu_Unicode.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Unicode_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_3
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_3.Name = "toolStripMenuItem_RadixContextMenu_Separator_3";
			this.toolStripMenuItem_RadixContextMenu_Separator_3.Size = new System.Drawing.Size(148, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_SeparateTxRx
			// 
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Name = "toolStripMenuItem_RadixContextMenu_SeparateTxRx";
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Text = "Separate Tx/Rx";
			this.toolStripMenuItem_RadixContextMenu_SeparateTxRx.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_SeparateTxRx_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Separator_4
			// 
			this.toolStripMenuItem_RadixContextMenu_Separator_4.Name = "toolStripMenuItem_RadixContextMenu_Separator_4";
			this.toolStripMenuItem_RadixContextMenu_Separator_4.Size = new System.Drawing.Size(148, 6);
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
            this.toolStripMenuItem_RadixContextMenu_Tx_Hex,
            this.toolStripMenuItem_RadixContextMenu_Tx_Unicode});
			this.toolStripMenuItem_RadixContextMenu_TxRadix.Name = "toolStripMenuItem_RadixContextMenu_TxRadix";
			this.toolStripMenuItem_RadixContextMenu_TxRadix.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_TxRadix.Text = "&Tx Radix";
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_String
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Checked = true;
			this.toolStripMenuItem_RadixContextMenu_Tx_String.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Name = "toolStripMenuItem_RadixContextMenu_Tx_String";
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Text = "&String";
			this.toolStripMenuItem_RadixContextMenu_Tx_String.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_String_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Char
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Name = "toolStripMenuItem_RadixContextMenu_Tx_Char";
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Text = "&Character";
			this.toolStripMenuItem_RadixContextMenu_Tx_Char.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Char_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Separator_1
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1.Name = "toolStripMenuItem_RadixContextMenu_Tx_Separator_1";
			this.toolStripMenuItem_RadixContextMenu_Tx_Separator_1.Size = new System.Drawing.Size(140, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Bin
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Name = "toolStripMenuItem_RadixContextMenu_Tx_Bin";
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Text = "&Binary";
			this.toolStripMenuItem_RadixContextMenu_Tx_Bin.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Bin_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Oct
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Name = "toolStripMenuItem_RadixContextMenu_Tx_Oct";
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Text = "&Octal";
			this.toolStripMenuItem_RadixContextMenu_Tx_Oct.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Oct_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Dec
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Name = "toolStripMenuItem_RadixContextMenu_Tx_Dec";
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Text = "&Decimal";
			this.toolStripMenuItem_RadixContextMenu_Tx_Dec.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Dec_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Hex
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Name = "toolStripMenuItem_RadixContextMenu_Tx_Hex";
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Text = "&Hexadecimal";
			this.toolStripMenuItem_RadixContextMenu_Tx_Hex.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Hex_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Tx_Unicode
			// 
			this.toolStripMenuItem_RadixContextMenu_Tx_Unicode.Name = "toolStripMenuItem_RadixContextMenu_Tx_Unicode";
			this.toolStripMenuItem_RadixContextMenu_Tx_Unicode.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Tx_Unicode.Text = "&Unicode";
			this.toolStripMenuItem_RadixContextMenu_Tx_Unicode.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Tx_Unicode_Click);
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
            this.toolStripMenuItem_RadixContextMenu_Rx_Hex,
            this.toolStripMenuItem_RadixContextMenu_Rx_Unicode});
			this.toolStripMenuItem_RadixContextMenu_RxRadix.Name = "toolStripMenuItem_RadixContextMenu_RxRadix";
			this.toolStripMenuItem_RadixContextMenu_RxRadix.Size = new System.Drawing.Size(151, 22);
			this.toolStripMenuItem_RadixContextMenu_RxRadix.Text = "&Rx Radix";
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_String
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Checked = true;
			this.toolStripMenuItem_RadixContextMenu_Rx_String.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Name = "toolStripMenuItem_RadixContextMenu_Rx_String";
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Text = "&String";
			this.toolStripMenuItem_RadixContextMenu_Rx_String.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_String_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Char
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Name = "toolStripMenuItem_RadixContextMenu_Rx_Char";
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Text = "&Character";
			this.toolStripMenuItem_RadixContextMenu_Rx_Char.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Char_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Separator_1
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1.Name = "toolStripMenuItem_RadixContextMenu_Rx_Separator_1";
			this.toolStripMenuItem_RadixContextMenu_Rx_Separator_1.Size = new System.Drawing.Size(140, 6);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Bin
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Name = "toolStripMenuItem_RadixContextMenu_Rx_Bin";
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Text = "&Binary";
			this.toolStripMenuItem_RadixContextMenu_Rx_Bin.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Bin_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Oct
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Name = "toolStripMenuItem_RadixContextMenu_Rx_Oct";
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Text = "&Octal";
			this.toolStripMenuItem_RadixContextMenu_Rx_Oct.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Oct_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Dec
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Name = "toolStripMenuItem_RadixContextMenu_Rx_Dec";
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Text = "&Decimal";
			this.toolStripMenuItem_RadixContextMenu_Rx_Dec.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Dec_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Hex
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Name = "toolStripMenuItem_RadixContextMenu_Rx_Hex";
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Text = "&Hexadecimal";
			this.toolStripMenuItem_RadixContextMenu_Rx_Hex.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Hex_Click);
			// 
			// toolStripMenuItem_RadixContextMenu_Rx_Unicode
			// 
			this.toolStripMenuItem_RadixContextMenu_Rx_Unicode.Name = "toolStripMenuItem_RadixContextMenu_Rx_Unicode";
			this.toolStripMenuItem_RadixContextMenu_Rx_Unicode.Size = new System.Drawing.Size(143, 22);
			this.toolStripMenuItem_RadixContextMenu_Rx_Unicode.Text = "&Unicode";
			this.toolStripMenuItem_RadixContextMenu_Rx_Unicode.Click += new System.EventHandler(this.toolStripMenuItem_RadixContextMenu_Rx_Unicode_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Radix
			// 
			this.toolStripMenuItem_TerminalMenu_View_Radix.DropDown = this.contextMenuStrip_Radix;
			this.toolStripMenuItem_TerminalMenu_View_Radix.Name = "toolStripMenuItem_TerminalMenu_View_Radix";
			this.toolStripMenuItem_TerminalMenu_View_Radix.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_Radix.Text = "&Radix";
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowRadix
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Name = "toolStripMenuItem_MonitorContextMenu_ShowRadix";
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Text = "Show Radix";
			this.toolStripMenuItem_MonitorContextMenu_ShowRadix.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowRadix_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_4
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_4.Name = "toolStripMenuItem_MonitorContextMenu_Separator_4";
			this.toolStripMenuItem_MonitorContextMenu_Separator_4.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowLineNumbers
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Name = "toolStripMenuItem_MonitorContextMenu_ShowLineNumbers";
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Text = "Show Line Numbers (Total Absolute)";
			this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowLineNumbers_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_5
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_5.Name = "toolStripMenuItem_MonitorContextMenu_Separator_5";
			this.toolStripMenuItem_MonitorContextMenu_Separator_5.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowTimeStamp
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Name = "toolStripMenuItem_MonitorContextMenu_ShowTimeStamp";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Text = "Show Time Stamp";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.ToolTipText = "The absolute moment in time.\r\n\r\nFormat can be configured in [View > Format...].";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowTimeStamp_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowTimeSpan
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.Name = "toolStripMenuItem_MonitorContextMenu_ShowTimeSpan";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.Text = "Show Time Span";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.ToolTipText = "The time that passed relative to [Connect Time].\r\n\r\nFormat can be configured in [" +
    "View > Format...].";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowTimeSpan_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowTimeDelta
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.Name = "toolStripMenuItem_MonitorContextMenu_ShowTimeDelta";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.Text = "Show Time Delta";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.ToolTipText = "The time that passed relative to the last line.\r\n\r\nFormat can be configured in [V" +
    "iew > Format...].";
			this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowTimeDelta_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowDevice
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowDevice.Name = "toolStripMenuItem_MonitorContextMenu_ShowDevice";
			this.toolStripMenuItem_MonitorContextMenu_ShowDevice.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowDevice.Text = "Show I/O Device";
			this.toolStripMenuItem_MonitorContextMenu_ShowDevice.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowDevice_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowDirection
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowDirection.Name = "toolStripMenuItem_MonitorContextMenu_ShowDirection";
			this.toolStripMenuItem_MonitorContextMenu_ShowDirection.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowDirection.Text = "Show Direction";
			this.toolStripMenuItem_MonitorContextMenu_ShowDirection.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowDirection_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowEol
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Name = "toolStripMenuItem_MonitorContextMenu_ShowEol";
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Text = "Show EOL Sequence";
			this.toolStripMenuItem_MonitorContextMenu_ShowEol.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowEol_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowLength
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Name = "toolStripMenuItem_MonitorContextMenu_ShowLength";
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Text = "Show Length (Byte Count)";
			this.toolStripMenuItem_MonitorContextMenu_ShowLength.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowLength_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowDuration
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowDuration.Name = "toolStripMenuItem_MonitorContextMenu_ShowDuration";
			this.toolStripMenuItem_MonitorContextMenu_ShowDuration.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowDuration.Text = "Show Duration (Line)";
			this.toolStripMenuItem_MonitorContextMenu_ShowDuration.ToolTipText = "The duration from first to last byte of the line.\r\n\r\nFormat can be configured in " +
    "[Format...] further below.";
			this.toolStripMenuItem_MonitorContextMenu_ShowDuration.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowDuration_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_6
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_6.Name = "toolStripMenuItem_MonitorContextMenu_Separator_6";
			this.toolStripMenuItem_MonitorContextMenu_Separator_6.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine
			// 
			this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine.Name = "toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine";
			this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine.Text = "Show Copy of Active Line";
			this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_7
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_7.Name = "toolStripMenuItem_MonitorContextMenu_Separator_7";
			this.toolStripMenuItem_MonitorContextMenu_Separator_7.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_Format
			// 
			this.toolStripMenuItem_MonitorContextMenu_Format.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_font_16x16;
			this.toolStripMenuItem_MonitorContextMenu_Format.Name = "toolStripMenuItem_MonitorContextMenu_Format";
			this.toolStripMenuItem_MonitorContextMenu_Format.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_Format.Text = "Format...";
			this.toolStripMenuItem_MonitorContextMenu_Format.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Format_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_8
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_8.Name = "toolStripMenuItem_MonitorContextMenu_Separator_8";
			this.toolStripMenuItem_MonitorContextMenu_Separator_8.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_Clear
			// 
			this.toolStripMenuItem_MonitorContextMenu_Clear.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_lightning_16x16;
			this.toolStripMenuItem_MonitorContextMenu_Clear.Name = "toolStripMenuItem_MonitorContextMenu_Clear";
			this.toolStripMenuItem_MonitorContextMenu_Clear.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_Clear.Text = "Clear";
			this.toolStripMenuItem_MonitorContextMenu_Clear.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Clear_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Refresh
			// 
			this.toolStripMenuItem_MonitorContextMenu_Refresh.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_refresh_16x16;
			this.toolStripMenuItem_MonitorContextMenu_Refresh.Name = "toolStripMenuItem_MonitorContextMenu_Refresh";
			this.toolStripMenuItem_MonitorContextMenu_Refresh.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_Refresh.Text = "Refresh";
			this.toolStripMenuItem_MonitorContextMenu_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Refresh_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_9
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_9.Name = "toolStripMenuItem_MonitorContextMenu_Separator_9";
			this.toolStripMenuItem_MonitorContextMenu_Separator_9.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_SelectAll
			// 
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Name = "toolStripMenuItem_MonitorContextMenu_SelectAll";
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Text = "Select All";
			this.toolStripMenuItem_MonitorContextMenu_SelectAll.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_SelectAll_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_SelectNone
			// 
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Name = "toolStripMenuItem_MonitorContextMenu_SelectNone";
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Text = "Select None";
			this.toolStripMenuItem_MonitorContextMenu_SelectNone.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_SelectNone_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_CopyToClipboard
			// 
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_text_exports_16x16;
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Name = "toolStripMenuItem_MonitorContextMenu_CopyToClipboard";
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Text = "Copy to Clipboard";
			this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_CopyToClipboard_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_10
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_10.Name = "toolStripMenuItem_MonitorContextMenu_Separator_10";
			this.toolStripMenuItem_MonitorContextMenu_Separator_10.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_SaveToFile
			// 
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_save_16x16;
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Name = "toolStripMenuItem_MonitorContextMenu_SaveToFile";
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Text = "Save to File...";
			this.toolStripMenuItem_MonitorContextMenu_SaveToFile.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_SaveToFile_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Print
			// 
			this.toolStripMenuItem_MonitorContextMenu_Print.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_printer_16x16;
			this.toolStripMenuItem_MonitorContextMenu_Print.Name = "toolStripMenuItem_MonitorContextMenu_Print";
			this.toolStripMenuItem_MonitorContextMenu_Print.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_Print.Text = "Print...";
			this.toolStripMenuItem_MonitorContextMenu_Print.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Print_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_Separator_11
			// 
			this.toolStripMenuItem_MonitorContextMenu_Separator_11.Name = "toolStripMenuItem_MonitorContextMenu_Separator_11";
			this.toolStripMenuItem_MonitorContextMenu_Separator_11.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_MonitorContextMenu_Find
			// 
			this.toolStripMenuItem_MonitorContextMenu_Find.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_tab_search_16x16;
			this.toolStripMenuItem_MonitorContextMenu_Find.Name = "toolStripMenuItem_MonitorContextMenu_Find";
			this.toolStripMenuItem_MonitorContextMenu_Find.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_Find.Text = "Find...";
			this.toolStripMenuItem_MonitorContextMenu_Find.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_Find_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_FindNext
			// 
			this.toolStripMenuItem_MonitorContextMenu_FindNext.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_rows_insert_below_word_16x16;
			this.toolStripMenuItem_MonitorContextMenu_FindNext.Name = "toolStripMenuItem_MonitorContextMenu_FindNext";
			this.toolStripMenuItem_MonitorContextMenu_FindNext.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_FindNext.Text = "Find Next";
			this.toolStripMenuItem_MonitorContextMenu_FindNext.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_FindNext_Click);
			// 
			// toolStripMenuItem_MonitorContextMenu_FindPrevious
			// 
			this.toolStripMenuItem_MonitorContextMenu_FindPrevious.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_rows_insert_above_word_16x16;
			this.toolStripMenuItem_MonitorContextMenu_FindPrevious.Name = "toolStripMenuItem_MonitorContextMenu_FindPrevious";
			this.toolStripMenuItem_MonitorContextMenu_FindPrevious.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_MonitorContextMenu_FindPrevious.Text = "Find Previous";
			this.toolStripMenuItem_MonitorContextMenu_FindPrevious.Click += new System.EventHandler(this.toolStripMenuItem_MonitorContextMenu_FindPrevious_Click);
			// 
			// contextMenuStrip_Predefined
			// 
			this.contextMenuStrip_Predefined.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_Panels,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_1,
            this.toolStripMenuItem_PredefinedContextMenu_Layout,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_2,
            this.toolStripMenuItem_PredefinedContextMenu_Command,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_3,
            this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText,
            this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile,
            this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_4,
            this.toolStripMenuItem_PredefinedContextMenu_Cut,
            this.toolStripMenuItem_PredefinedContextMenu_Copy,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath,
            this.toolStripMenuItem_PredefinedContextMenu_Paste,
            this.toolStripMenuItem_PredefinedContextMenu_Clear,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_5,
            this.toolStripMenuItem_PredefinedContextMenu_Define,
            this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_6,
            this.toolStripMenuItem_PredefinedContextMenu_Page,
            this.toolStripMenuItem_PredefinedContextMenu_Separator_7,
            this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard,
            this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard,
            this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile,
            this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile,
            this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile});
			this.contextMenuStrip_Predefined.Name = "contextMenuStrip_PredefinedCommands";
			this.contextMenuStrip_Predefined.Size = new System.Drawing.Size(258, 574);
			this.contextMenuStrip_Predefined.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Predefined_Opening);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Panels
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Panels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined});
			this.toolStripMenuItem_PredefinedContextMenu_Panels.Name = "toolStripMenuItem_PredefinedContextMenu_Panels";
			this.toolStripMenuItem_PredefinedContextMenu_Panels.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Panels.Text = "Panels";
			// 
			// toolStripMenuItem_PredefinedContextMenu_Panels_Predefined
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Checked = true;
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Name = "toolStripMenuItem_PredefinedContextMenu_Panels_Predefined";
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Size = new System.Drawing.Size(228, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Text = "Predefined Commands Panel";
			this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Panels_Predefined_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_1.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_1";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_1.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Layout
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Layout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox_PredefinedContextMenu_Layout});
			this.toolStripMenuItem_PredefinedContextMenu_Layout.Name = "toolStripMenuItem_PredefinedContextMenu_Layout";
			this.toolStripMenuItem_PredefinedContextMenu_Layout.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Layout.Text = "Layout";
			// 
			// toolStripComboBox_PredefinedContextMenu_Layout
			// 
			this.toolStripComboBox_PredefinedContextMenu_Layout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_PredefinedContextMenu_Layout.Name = "toolStripComboBox_PredefinedContextMenu_Layout";
			this.toolStripComboBox_PredefinedContextMenu_Layout.Size = new System.Drawing.Size(121, 23);
			this.toolStripComboBox_PredefinedContextMenu_Layout.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_PredefinedContextMenu_Layout_SelectedIndexChanged);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_2.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_2";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_2.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Command
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Command.DropDown = this.contextMenuStrip_Command;
			this.toolStripMenuItem_PredefinedContextMenu_Command.Name = "toolStripMenuItem_PredefinedContextMenu_Command";
			this.toolStripMenuItem_PredefinedContextMenu_Command.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Command.Text = "Command";
			// 
			// contextMenuStrip_Command
			// 
			this.contextMenuStrip_Command.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_CommandContextMenu_1,
            this.toolStripMenuItem_CommandContextMenu_2,
            this.toolStripMenuItem_CommandContextMenu_3,
            this.toolStripMenuItem_CommandContextMenu_4,
            this.toolStripMenuItem_CommandContextMenu_5,
            this.toolStripMenuItem_CommandContextMenu_6,
            this.toolStripMenuItem_CommandContextMenu_7,
            this.toolStripMenuItem_CommandContextMenu_8,
            this.toolStripMenuItem_CommandContextMenu_9,
            this.toolStripMenuItem_CommandContextMenu_10,
            this.toolStripMenuItem_CommandContextMenu_11,
            this.toolStripMenuItem_CommandContextMenu_12});
			this.contextMenuStrip_Command.Name = "contextMenuStrip_Command";
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
			this.contextMenuStrip_Command.OwnerItem = this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands;
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
			this.contextMenuStrip_Command.Size = new System.Drawing.Size(221, 268);
			this.contextMenuStrip_Command.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Command_Opening);
			// 
			// toolStripMenuItem_CommandContextMenu_1
			// 
			this.toolStripMenuItem_CommandContextMenu_1.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_1.Name = "toolStripMenuItem_CommandContextMenu_1";
			this.toolStripMenuItem_CommandContextMenu_1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F1)));
			this.toolStripMenuItem_CommandContextMenu_1.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_1.Tag = "1";
			this.toolStripMenuItem_CommandContextMenu_1.Text = "&1: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_1.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_2
			// 
			this.toolStripMenuItem_CommandContextMenu_2.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_2.Name = "toolStripMenuItem_CommandContextMenu_2";
			this.toolStripMenuItem_CommandContextMenu_2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F2)));
			this.toolStripMenuItem_CommandContextMenu_2.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_2.Tag = "2";
			this.toolStripMenuItem_CommandContextMenu_2.Text = "&2: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_2.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_3
			// 
			this.toolStripMenuItem_CommandContextMenu_3.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_3.Name = "toolStripMenuItem_CommandContextMenu_3";
			this.toolStripMenuItem_CommandContextMenu_3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			this.toolStripMenuItem_CommandContextMenu_3.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_3.Tag = "3";
			this.toolStripMenuItem_CommandContextMenu_3.Text = "&3: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_3.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_4
			// 
			this.toolStripMenuItem_CommandContextMenu_4.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_4.Name = "toolStripMenuItem_CommandContextMenu_4";
			this.toolStripMenuItem_CommandContextMenu_4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F4)));
			this.toolStripMenuItem_CommandContextMenu_4.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_4.Tag = "4";
			this.toolStripMenuItem_CommandContextMenu_4.Text = "&4: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_4.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_5
			// 
			this.toolStripMenuItem_CommandContextMenu_5.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_5.Name = "toolStripMenuItem_CommandContextMenu_5";
			this.toolStripMenuItem_CommandContextMenu_5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
			this.toolStripMenuItem_CommandContextMenu_5.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_5.Tag = "5";
			this.toolStripMenuItem_CommandContextMenu_5.Text = "&5: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_5.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_6
			// 
			this.toolStripMenuItem_CommandContextMenu_6.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_6.Name = "toolStripMenuItem_CommandContextMenu_6";
			this.toolStripMenuItem_CommandContextMenu_6.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F6)));
			this.toolStripMenuItem_CommandContextMenu_6.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_6.Tag = "6";
			this.toolStripMenuItem_CommandContextMenu_6.Text = "&6: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_6.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_7
			// 
			this.toolStripMenuItem_CommandContextMenu_7.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_7.Name = "toolStripMenuItem_CommandContextMenu_7";
			this.toolStripMenuItem_CommandContextMenu_7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F7)));
			this.toolStripMenuItem_CommandContextMenu_7.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_7.Tag = "7";
			this.toolStripMenuItem_CommandContextMenu_7.Text = "&7: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_7.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_8
			// 
			this.toolStripMenuItem_CommandContextMenu_8.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_8.Name = "toolStripMenuItem_CommandContextMenu_8";
			this.toolStripMenuItem_CommandContextMenu_8.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F8)));
			this.toolStripMenuItem_CommandContextMenu_8.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_8.Tag = "8";
			this.toolStripMenuItem_CommandContextMenu_8.Text = "&8: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_8.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_9
			// 
			this.toolStripMenuItem_CommandContextMenu_9.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_9.Name = "toolStripMenuItem_CommandContextMenu_9";
			this.toolStripMenuItem_CommandContextMenu_9.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F9)));
			this.toolStripMenuItem_CommandContextMenu_9.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_9.Tag = "9";
			this.toolStripMenuItem_CommandContextMenu_9.Text = "&9: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_9.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_10
			// 
			this.toolStripMenuItem_CommandContextMenu_10.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_10.Name = "toolStripMenuItem_CommandContextMenu_10";
			this.toolStripMenuItem_CommandContextMenu_10.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F10)));
			this.toolStripMenuItem_CommandContextMenu_10.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_10.Tag = "10";
			this.toolStripMenuItem_CommandContextMenu_10.Text = "1&0: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_10.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_11
			// 
			this.toolStripMenuItem_CommandContextMenu_11.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_11.Name = "toolStripMenuItem_CommandContextMenu_11";
			this.toolStripMenuItem_CommandContextMenu_11.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
			this.toolStripMenuItem_CommandContextMenu_11.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_11.Tag = "11";
			this.toolStripMenuItem_CommandContextMenu_11.Text = "11: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_11.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_CommandContextMenu_12
			// 
			this.toolStripMenuItem_CommandContextMenu_12.Enabled = false;
			this.toolStripMenuItem_CommandContextMenu_12.Name = "toolStripMenuItem_CommandContextMenu_12";
			this.toolStripMenuItem_CommandContextMenu_12.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F12)));
			this.toolStripMenuItem_CommandContextMenu_12.Size = new System.Drawing.Size(220, 22);
			this.toolStripMenuItem_CommandContextMenu_12.Tag = "12";
			this.toolStripMenuItem_CommandContextMenu_12.Text = "12: <Undefined>";
			this.toolStripMenuItem_CommandContextMenu_12.Click += new System.EventHandler(this.toolStripMenuItem_CommandContextMenu_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_3.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_3";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_3.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyFromSendText
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Name = "toolStripMenuItem_PredefinedContextMenu_CopyFromSendText";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.ShortcutKeyDisplayString = "";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Text = "Copy from Send Text";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendText_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Name = "toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Text = "Copy from Send File";
			this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile.Name = "toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile";
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile.Text = "Copy to Send Text/File";
			this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96,
            this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97});
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo.Text = "Copy to";
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_1";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1.Text = "1";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_2";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2.Text = "2";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_3";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3.Text = "3";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_4";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4.Text = "4";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_5";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5.Text = "5";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_6";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6.Text = "6";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_7";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7.Text = "7";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_8";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8.Text = "8";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_9";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9.Text = "9";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_10";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Text = "10";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_11";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Text = "11";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_12";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Text = "12";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_13
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_13";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Tag = "13";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Text = "13";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_14
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_14";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Tag = "14";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Text = "14";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_15
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_15";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Tag = "15";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Text = "15";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_16
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_16";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Tag = "16";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Text = "16";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_17
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_17";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Tag = "17";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Text = "17";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_18
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_18";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Tag = "18";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Text = "18";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_19
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_19";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Tag = "19";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Text = "19";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_20
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_20";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Tag = "20";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Text = "20";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_21
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_21";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Tag = "21";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Text = "21";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_22
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_22";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Tag = "22";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Text = "22";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_23
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_23";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Tag = "23";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Text = "23";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_24";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Tag = "24";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Text = "24";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_25
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_25";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Tag = "25";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Text = "25";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_37
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_37";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Tag = "37";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Text = "37";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_49
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_49";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Tag = "49";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Text = "49";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_61
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_61";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Tag = "61";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Text = "61";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_73
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_73";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Tag = "73";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Text = "73";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_85
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_85";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Tag = "85";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Text = "85";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTo_97
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTo_97";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Tag = "97";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Text = "97";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96,
            this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97});
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo.Text = "Move to";
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_1";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1.Text = "1";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_2";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2.Text = "2";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_3";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3.Text = "3";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_4";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4.Text = "4";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_5";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5.Text = "5";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_6";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6.Text = "6";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_7";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7.Text = "7";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_8";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8.Text = "8";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_9";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9.Text = "9";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_10";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Text = "10";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_11";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Text = "11";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_12";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Text = "12";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_13
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_13";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Tag = "13";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Text = "13";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_14
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_14";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Tag = "14";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Text = "14";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_15
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_15";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Tag = "15";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Text = "15";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_16
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_16";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Tag = "16";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Text = "16";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_17
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_17";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Tag = "17";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Text = "17";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_18
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_18";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Tag = "18";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Text = "18";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_19
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_19";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Tag = "19";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Text = "19";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_20
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_20";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Tag = "20";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Text = "20";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_21
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_21";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Tag = "21";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Text = "21";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_22
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_22";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Tag = "22";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Text = "22";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_23
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_23";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Tag = "23";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Text = "23";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_24";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Tag = "24";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Text = "24";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_25
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_25";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Tag = "25";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Text = "25";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_37
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_37";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Tag = "37";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Text = "37";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_49
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_49";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Tag = "49";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Text = "49";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_61
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_61";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Tag = "61";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Text = "61";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_73
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_73";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Tag = "73";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Text = "73";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_85
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_85";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Tag = "85";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Text = "85";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_MoveTo_97
			// 
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Name = "toolStripMenuItem_PredefinedContextMenu_MoveTo_97";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Tag = "97";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Text = "97";
			this.toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_1,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_2,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_3,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_4,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_5,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_6,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_7,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_8,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_9,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_10,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_11,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_12,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_13,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_14,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_15,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_16,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_17,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_18,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_19,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_20,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_21,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_22,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_23,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_24,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_36,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_48,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_60,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_72,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_84,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96,
            this.toolStripMenuItem_PredefinedContextMenu_UpBy_96});
			this.toolStripMenuItem_PredefinedContextMenu_UpBy.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy.Text = "Up by";
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_1.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_1";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_1.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_1.Text = "1";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_2.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_2";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_2.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_2.Text = "2";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_3.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_3";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_3.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_3.Text = "3";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_4.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_4";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_4.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_4.Text = "4";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_5.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_5";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_5.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_5.Text = "5";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_6.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_6";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_6.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_6.Text = "6";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_7.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_7";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_7.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_7.Text = "7";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_8.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_8";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_8.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_8.Text = "8";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_9.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_9";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_9.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_9.Text = "9";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_10.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_10";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_10.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_10.Text = "10";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_11.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_11";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_11.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_11.Text = "11";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_12.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_12";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_12.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_12.Text = "12";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_13
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_13.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_13";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_13.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_13.Tag = "13";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_13.Text = "13";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_13.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_14
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_14.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_14";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_14.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_14.Tag = "14";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_14.Text = "14";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_14.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_15
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_15.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_15";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_15.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_15.Tag = "15";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_15.Text = "15";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_15.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_16
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_16.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_16";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_16.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_16.Tag = "16";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_16.Text = "16";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_16.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_17
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_17.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_17";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_17.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_17.Tag = "17";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_17.Text = "17";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_17.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_18
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_18.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_18";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_18.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_18.Tag = "18";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_18.Text = "18";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_18.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_19
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_19.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_19";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_19.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_19.Tag = "19";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_19.Text = "19";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_19.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_20
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_20.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_20";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_20.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_20.Tag = "20";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_20.Text = "20";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_20.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_21
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_21.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_21";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_21.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_21.Tag = "21";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_21.Text = "21";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_21.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_22
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_22.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_22";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_22.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_22.Tag = "22";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_22.Text = "22";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_22.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_23
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_23.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_23";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_23.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_23.Tag = "23";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_23.Text = "23";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_23.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_24.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_24";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_24.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_24.Tag = "24";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_24.Text = "24";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_24.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_36
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_36.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_36";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_36.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_36.Tag = "36";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_36.Text = "36";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_36.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_48
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_48.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_48";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_48.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_48.Tag = "48";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_48.Text = "48";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_48.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_60
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_60.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_60";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_60.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_60.Tag = "60";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_60.Text = "60";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_60.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_72
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_72.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_72";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_72.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_72.Tag = "72";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_72.Text = "72";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_72.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_84
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_84.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_84";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_84.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_84.Tag = "84";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_84.Text = "84";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_84.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_UpBy_96
			// 
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_96.Name = "toolStripMenuItem_PredefinedContextMenu_UpBy_96";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_96.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_96.Tag = "96";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_96.Text = "96";
			this.toolStripMenuItem_PredefinedContextMenu_UpBy_96.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_1,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_2,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_3,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_4,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_5,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_6,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_7,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_8,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_9,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_10,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_11,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_12,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_13,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_14,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_15,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_16,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_17,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_18,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_19,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_20,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_21,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_22,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_23,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_24,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_36,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_48,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_60,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_72,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_84,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96,
            this.toolStripMenuItem_PredefinedContextMenu_DownBy_96});
			this.toolStripMenuItem_PredefinedContextMenu_DownBy.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy.Text = "Down by";
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_1
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_1.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_1";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_1.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_1.Tag = "1";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_1.Text = "1";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_1.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_2
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_2.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_2";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_2.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_2.Tag = "2";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_2.Text = "2";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_2.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_3
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_3.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_3";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_3.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_3.Tag = "3";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_3.Text = "3";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_3.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_4.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_4";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_4.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_4.Tag = "4";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_4.Text = "4";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_4.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_5.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_5";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_5.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_5.Tag = "5";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_5.Text = "5";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_5.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_6.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_6";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_6.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_6.Tag = "6";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_6.Text = "6";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_6.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_7.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_7";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_7.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_7.Tag = "7";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_7.Text = "7";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_7.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_8
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_8.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_8";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_8.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_8.Tag = "8";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_8.Text = "8";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_8.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_9
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_9.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_9";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_9.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_9.Tag = "9";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_9.Text = "9";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_9.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_10
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_10.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_10";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_10.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_10.Tag = "10";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_10.Text = "10";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_10.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_11
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_11.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_11";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_11.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_11.Tag = "11";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_11.Text = "11";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_11.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_12
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_12.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_12";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_12.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_12.Tag = "12";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_12.Text = "12";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_12.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_13
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_13.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_13";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_13.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_13.Tag = "13";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_13.Text = "13";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_13.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_14
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_14.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_14";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_14.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_14.Tag = "14";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_14.Text = "14";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_14.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_15
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_15.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_15";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_15.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_15.Tag = "15";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_15.Text = "15";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_15.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_16
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_16.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_16";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_16.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_16.Tag = "16";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_16.Text = "16";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_16.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_17
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_17.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_17";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_17.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_17.Tag = "17";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_17.Text = "17";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_17.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_18
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_18.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_18";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_18.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_18.Tag = "18";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_18.Text = "18";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_18.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_19
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_19.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_19";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_19.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_19.Tag = "19";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_19.Text = "19";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_19.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_20
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_20.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_20";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_20.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_20.Tag = "20";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_20.Text = "20";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_20.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_21
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_21.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_21";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_21.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_21.Tag = "21";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_21.Text = "21";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_21.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_22
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_22.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_22";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_22.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_22.Tag = "22";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_22.Text = "22";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_22.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_23
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_23.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_23";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_23.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_23.Tag = "23";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_23.Text = "23";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_23.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_24
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_24.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_24";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_24.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_24.Tag = "24";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_24.Text = "24";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_24.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_36
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_36.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_36";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_36.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_36.Tag = "36";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_36.Text = "36";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_36.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_48
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_48.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_48";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_48.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_48.Tag = "48";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_48.Text = "48";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_48.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_60
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_60.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_60";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_60.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_60.Tag = "60";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_60.Text = "60";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_60.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_72
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_72.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_72";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_72.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_72.Tag = "72";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_72.Text = "72";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_72.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_84
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_84.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_84";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_84.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_84.Tag = "84";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_84.Text = "84";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_84.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96.Size = new System.Drawing.Size(83, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_DownBy_96
			// 
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_96.Name = "toolStripMenuItem_PredefinedContextMenu_DownBy_96";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_96.Size = new System.Drawing.Size(86, 22);
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_96.Tag = "96";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_96.Text = "96";
			this.toolStripMenuItem_PredefinedContextMenu_DownBy_96.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_4
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_4.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_4";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_4.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Cut
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Cut.Name = "toolStripMenuItem_PredefinedContextMenu_Cut";
			this.toolStripMenuItem_PredefinedContextMenu_Cut.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Cut.Text = "Cut to Clipboard";
			this.toolStripMenuItem_PredefinedContextMenu_Cut.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Cut_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Copy
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Copy.Name = "toolStripMenuItem_PredefinedContextMenu_Copy";
			this.toolStripMenuItem_PredefinedContextMenu_Copy.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Copy.Text = "Copy to Clipboard";
			this.toolStripMenuItem_PredefinedContextMenu_Copy.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Copy_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.Name = "toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.Text = "Copy Text or File Path to Clipboard";
			this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.ToolTipText = resources.GetString("toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.ToolTipText");
			this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Paste
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Paste.Name = "toolStripMenuItem_PredefinedContextMenu_Paste";
			this.toolStripMenuItem_PredefinedContextMenu_Paste.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Paste.Text = "Paste from Clipboard";
			this.toolStripMenuItem_PredefinedContextMenu_Paste.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Paste_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Clear
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Clear.Name = "toolStripMenuItem_PredefinedContextMenu_Clear";
			this.toolStripMenuItem_PredefinedContextMenu_Clear.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Clear.Text = "Clear";
			this.toolStripMenuItem_PredefinedContextMenu_Clear.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Clear_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_5
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_5.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_5";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_5.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Define
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Define.Name = "toolStripMenuItem_PredefinedContextMenu_Define";
			this.toolStripMenuItem_PredefinedContextMenu_Define.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
			this.toolStripMenuItem_PredefinedContextMenu_Define.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Define.Text = "Define...";
			this.toolStripMenuItem_PredefinedContextMenu_Define.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_Define_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_6
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_6.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_6";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_6.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Page
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Page.DropDown = this.contextMenuStrip_Page;
			this.toolStripMenuItem_PredefinedContextMenu_Page.Name = "toolStripMenuItem_PredefinedContextMenu_Page";
			this.toolStripMenuItem_PredefinedContextMenu_Page.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_Page.Text = "Page";
			// 
			// contextMenuStrip_Page
			// 
			this.contextMenuStrip_Page.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PageContextMenu_Next,
            this.toolStripMenuItem_PageContextMenu_Previous,
            this.toolStripMenuItem_PageContextMenu_Separator,
            this.toolStripMenuItem_PageContextMenu_1,
            this.toolStripMenuItem_PageContextMenu_2,
            this.toolStripMenuItem_PageContextMenu_3,
            this.toolStripMenuItem_PageContextMenu_4,
            this.toolStripMenuItem_PageContextMenu_5,
            this.toolStripMenuItem_PageContextMenu_6,
            this.toolStripMenuItem_PageContextMenu_7,
            this.toolStripMenuItem_PageContextMenu_8,
            this.toolStripMenuItem_PageContextMenu_9});
			this.contextMenuStrip_Page.Name = "contextMenuStrip_Page";
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
			this.contextMenuStrip_Page.OwnerItem = this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage;
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
			this.contextMenuStrip_Page.Size = new System.Drawing.Size(198, 252);
			this.contextMenuStrip_Page.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Page_Opening);
			// 
			// toolStripMenuItem_PageContextMenu_Next
			// 
			this.toolStripMenuItem_PageContextMenu_Next.Name = "toolStripMenuItem_PageContextMenu_Next";
			this.toolStripMenuItem_PageContextMenu_Next.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Right)));
			this.toolStripMenuItem_PageContextMenu_Next.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_Next.Text = "&Next";
			this.toolStripMenuItem_PageContextMenu_Next.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_Next_Click);
			// 
			// toolStripMenuItem_PageContextMenu_Previous
			// 
			this.toolStripMenuItem_PageContextMenu_Previous.Name = "toolStripMenuItem_PageContextMenu_Previous";
			this.toolStripMenuItem_PageContextMenu_Previous.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.Left)));
			this.toolStripMenuItem_PageContextMenu_Previous.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_Previous.Text = "&Previous";
			this.toolStripMenuItem_PageContextMenu_Previous.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_Previous_Click);
			// 
			// toolStripMenuItem_PageContextMenu_Separator
			// 
			this.toolStripMenuItem_PageContextMenu_Separator.Name = "toolStripMenuItem_PageContextMenu_Separator";
			this.toolStripMenuItem_PageContextMenu_Separator.Size = new System.Drawing.Size(194, 6);
			// 
			// toolStripMenuItem_PageContextMenu_1
			// 
			this.toolStripMenuItem_PageContextMenu_1.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_1.Name = "toolStripMenuItem_PageContextMenu_1";
			this.toolStripMenuItem_PageContextMenu_1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D1)));
			this.toolStripMenuItem_PageContextMenu_1.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_1.Tag = "1";
			this.toolStripMenuItem_PageContextMenu_1.Text = "&1: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_1.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_2
			// 
			this.toolStripMenuItem_PageContextMenu_2.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_2.Name = "toolStripMenuItem_PageContextMenu_2";
			this.toolStripMenuItem_PageContextMenu_2.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D2)));
			this.toolStripMenuItem_PageContextMenu_2.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_2.Tag = "2";
			this.toolStripMenuItem_PageContextMenu_2.Text = "&2: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_2.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_3
			// 
			this.toolStripMenuItem_PageContextMenu_3.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_3.Name = "toolStripMenuItem_PageContextMenu_3";
			this.toolStripMenuItem_PageContextMenu_3.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D3)));
			this.toolStripMenuItem_PageContextMenu_3.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_3.Tag = "3";
			this.toolStripMenuItem_PageContextMenu_3.Text = "&3: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_3.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_4
			// 
			this.toolStripMenuItem_PageContextMenu_4.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_4.Name = "toolStripMenuItem_PageContextMenu_4";
			this.toolStripMenuItem_PageContextMenu_4.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D4)));
			this.toolStripMenuItem_PageContextMenu_4.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_4.Tag = "4";
			this.toolStripMenuItem_PageContextMenu_4.Text = "&4: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_4.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_5
			// 
			this.toolStripMenuItem_PageContextMenu_5.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_5.Name = "toolStripMenuItem_PageContextMenu_5";
			this.toolStripMenuItem_PageContextMenu_5.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D5)));
			this.toolStripMenuItem_PageContextMenu_5.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_5.Tag = "5";
			this.toolStripMenuItem_PageContextMenu_5.Text = "&5: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_5.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_6
			// 
			this.toolStripMenuItem_PageContextMenu_6.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_6.Name = "toolStripMenuItem_PageContextMenu_6";
			this.toolStripMenuItem_PageContextMenu_6.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D6)));
			this.toolStripMenuItem_PageContextMenu_6.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_6.Tag = "6";
			this.toolStripMenuItem_PageContextMenu_6.Text = "&6: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_6.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_7
			// 
			this.toolStripMenuItem_PageContextMenu_7.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_7.Name = "toolStripMenuItem_PageContextMenu_7";
			this.toolStripMenuItem_PageContextMenu_7.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D7)));
			this.toolStripMenuItem_PageContextMenu_7.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_7.Tag = "7";
			this.toolStripMenuItem_PageContextMenu_7.Text = "&7: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_7.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_8
			// 
			this.toolStripMenuItem_PageContextMenu_8.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_8.Name = "toolStripMenuItem_PageContextMenu_8";
			this.toolStripMenuItem_PageContextMenu_8.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D8)));
			this.toolStripMenuItem_PageContextMenu_8.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_8.Tag = "8";
			this.toolStripMenuItem_PageContextMenu_8.Text = "&8: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_8.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PageContextMenu_9
			// 
			this.toolStripMenuItem_PageContextMenu_9.Enabled = false;
			this.toolStripMenuItem_PageContextMenu_9.Name = "toolStripMenuItem_PageContextMenu_9";
			this.toolStripMenuItem_PageContextMenu_9.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D9)));
			this.toolStripMenuItem_PageContextMenu_9.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_PageContextMenu_9.Tag = "9";
			this.toolStripMenuItem_PageContextMenu_9.Text = "&9: <Undefined>";
			this.toolStripMenuItem_PageContextMenu_9.Click += new System.EventHandler(this.toolStripMenuItem_PageContextMenu_I_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_Separator_7
			// 
			this.toolStripMenuItem_PredefinedContextMenu_Separator_7.Name = "toolStripMenuItem_PredefinedContextMenu_Separator_7";
			this.toolStripMenuItem_PredefinedContextMenu_Separator_7.Size = new System.Drawing.Size(254, 6);
			// 
			// toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard
			// 
			this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_text_exports_16x16;
			this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard.Name = "toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard";
			this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard.Text = "Copy Page(s) to Clipboard...";
			this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard
			// 
			this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_text_imports_16x16;
			this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard.Name = "toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard";
			this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard.Text = "Paste Page(s) from Clipboard...";
			this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile
			// 
			this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_export_16x16;
			this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile.Name = "toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile";
			this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile.Text = "Export Page(s) to File...";
			this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile
			// 
			this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_import_16x16;
			this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile.Name = "toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile";
			this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile.Text = "Import Page(s) from File...";
			this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile_Click);
			// 
			// toolStripMenuItem_PredefinedContextMenu_LinkPageToFile
			// 
			this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_link_16x16;
			this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile.Name = "toolStripMenuItem_PredefinedContextMenu_LinkPageToFile";
			this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile.Text = "Link Page to File...";
			this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_LinkPageToFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_PredefinedCommands
			// 
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands.DropDown = this.contextMenuStrip_Command;
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands.Name = "toolStripMenuItem_TerminalMenu_Send_PredefinedCommands";
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands.Text = "Predefined &Commands";
			// 
			// toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage
			// 
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage.DropDown = this.contextMenuStrip_Page;
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage.Name = "toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage";
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage.Text = "Predefined Commands Pa&ge";
			// 
			// contextMenuStrip_Send
			// 
			this.contextMenuStrip_Send.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SendContextMenu_Panels,
            this.toolStripMenuItem_SendContextMenu_Separator_1,
            this.toolStripMenuItem_SendContextMenu_SendText,
            this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol,
            this.toolStripMenuItem_SendContextMenu_SendFile,
            this.toolStripMenuItem_SendContextMenu_Separator_2,
            this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard,
            this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard,
            this.toolStripMenuItem_SendContextMenu_Separator_3,
            this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix,
            this.toolStripMenuItem_SendContextMenu_AllowConcurrency,
            this.toolStripMenuItem_SendContextMenu_Separator_4,
            this.toolStripMenuItem_SendContextMenu_KeepSendText,
            this.toolStripMenuItem_SendContextMenu_SendImmediately,
            this.toolStripMenuItem_SendContextMenu_EnableEscapesForText,
            this.toolStripMenuItem_SendContextMenu_Separator_5,
            this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText,
            this.toolStripMenuItem_SendContextMenu_Separator_6,
            this.toolStripMenuItem_SendContextMenu_SkipEmptyLines,
            this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile,
            this.toolStripMenuItem_SendContextMenu_Separator_7,
            this.toolStripMenuItem_SendContextMenu_CopyPredefined});
			this.contextMenuStrip_Send.Name = "contextMenuStrip_Send";
			this.contextMenuStrip_Send.Size = new System.Drawing.Size(334, 376);
			this.contextMenuStrip_Send.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Send_Opening);
			// 
			// toolStripMenuItem_SendContextMenu_Panels
			// 
			this.toolStripMenuItem_SendContextMenu_Panels.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_SendContextMenu_Panels_SendText,
            this.toolStripMenuItem_SendContextMenu_Panels_SendFile});
			this.toolStripMenuItem_SendContextMenu_Panels.Name = "toolStripMenuItem_SendContextMenu_Panels";
			this.toolStripMenuItem_SendContextMenu_Panels.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_Panels.Text = "Panels";
			// 
			// toolStripMenuItem_SendContextMenu_Panels_SendText
			// 
			this.toolStripMenuItem_SendContextMenu_Panels_SendText.Checked = true;
			this.toolStripMenuItem_SendContextMenu_Panels_SendText.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_SendContextMenu_Panels_SendText.Name = "toolStripMenuItem_SendContextMenu_Panels_SendText";
			this.toolStripMenuItem_SendContextMenu_Panels_SendText.Size = new System.Drawing.Size(156, 22);
			this.toolStripMenuItem_SendContextMenu_Panels_SendText.Text = "Send Text Panel";
			this.toolStripMenuItem_SendContextMenu_Panels_SendText.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Panels_SendText_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Panels_SendFile
			// 
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = true;
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Name = "toolStripMenuItem_SendContextMenu_Panels_SendFile";
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Size = new System.Drawing.Size(156, 22);
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Text = "Send File Panel";
			this.toolStripMenuItem_SendContextMenu_Panels_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_Panels_SendFile_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_1
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_1.Name = "toolStripMenuItem_SendContextMenu_Separator_1";
			this.toolStripMenuItem_SendContextMenu_Separator_1.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_SendText
			// 
			this.toolStripMenuItem_SendContextMenu_SendText.Name = "toolStripMenuItem_SendContextMenu_SendText";
			this.toolStripMenuItem_SendContextMenu_SendText.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.toolStripMenuItem_SendContextMenu_SendText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_SendText.Text = "Send Text";
			this.toolStripMenuItem_SendContextMenu_SendText.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendText_Click);
			// 
			// toolStripMenuItem_SendContextMenu_SendTextWithoutEol
			// 
			this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Name = "toolStripMenuItem_SendContextMenu_SendTextWithoutEol";
			this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
			this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Text = "Send Text w/o EOL";
			this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendTextWithoutEol_Click);
			// 
			// toolStripMenuItem_SendContextMenu_SendFile
			// 
			this.toolStripMenuItem_SendContextMenu_SendFile.Name = "toolStripMenuItem_SendContextMenu_SendFile";
			this.toolStripMenuItem_SendContextMenu_SendFile.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.toolStripMenuItem_SendContextMenu_SendFile.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_SendFile.Text = "Send File";
			this.toolStripMenuItem_SendContextMenu_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendFile_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_2
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_2.Name = "toolStripMenuItem_SendContextMenu_Separator_2";
			this.toolStripMenuItem_SendContextMenu_Separator_2.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_CopyTextToClipboard
			// 
			this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard.Name = "toolStripMenuItem_SendContextMenu_CopyTextToClipboard";
			this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard.Text = "Copy Text to Clipboard";
			this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_CopyTextToClipboard_Click);
			// 
			// toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard
			// 
			this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard.Name = "toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard";
			this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard.Text = "Copy File Path to Clipboard";
			this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_3
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_3.Name = "toolStripMenuItem_SendContextMenu_Separator_3";
			this.toolStripMenuItem_SendContextMenu_Separator_3.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix
			// 
			this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Name = "toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix";
			this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Text = "Use Explicit Default Radix";
			this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.ToolTipText = "When enabled, the default radix can explicitly be selected.\r\nWhen disabled, the d" +
    "efault radix is \'String\'.\r\nApplies to [Send Text], [Send File] as well as [Prede" +
    "fined Commands].";
			this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix_Click);
			// 
			// toolStripMenuItem_SendContextMenu_AllowConcurrency
			// 
			this.toolStripMenuItem_SendContextMenu_AllowConcurrency.Name = "toolStripMenuItem_SendContextMenu_AllowConcurrency";
			this.toolStripMenuItem_SendContextMenu_AllowConcurrency.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_AllowConcurrency.Text = "Allow Concurrent Sending";
			this.toolStripMenuItem_SendContextMenu_AllowConcurrency.ToolTipText = resources.GetString("toolStripMenuItem_SendContextMenu_AllowConcurrency.ToolTipText");
			this.toolStripMenuItem_SendContextMenu_AllowConcurrency.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_AllowConcurrency_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_4
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_4.Name = "toolStripMenuItem_SendContextMenu_Separator_4";
			this.toolStripMenuItem_SendContextMenu_Separator_4.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_KeepSendText
			// 
			this.toolStripMenuItem_SendContextMenu_KeepSendText.Name = "toolStripMenuItem_SendContextMenu_KeepSendText";
			this.toolStripMenuItem_SendContextMenu_KeepSendText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.K)));
			this.toolStripMenuItem_SendContextMenu_KeepSendText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_KeepSendText.Text = "Keep [Send Text] after Send";
			this.toolStripMenuItem_SendContextMenu_KeepSendText.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_KeepSendText_Click);
			// 
			// toolStripMenuItem_SendContextMenu_SendImmediately
			// 
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Name = "toolStripMenuItem_SendContextMenu_SendImmediately";
			this.toolStripMenuItem_SendContextMenu_SendImmediately.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Text = "Send Each Character Immediately";
			this.toolStripMenuItem_SendContextMenu_SendImmediately.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SendImmediately_Click);
			// 
			// toolStripMenuItem_SendContextMenu_EnableEscapesForText
			// 
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForText.Name = "toolStripMenuItem_SendContextMenu_EnableEscapesForText";
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForText.Text = "Enable <...> and \\... Escapes on [Send Text]";
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForText.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_EnableEscapesForText_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_5
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_5.Name = "toolStripMenuItem_SendContextMenu_Separator_5";
			this.toolStripMenuItem_SendContextMenu_Separator_5.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_ExpandMultiLineText
			// 
			this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText.Name = "toolStripMenuItem_SendContextMenu_ExpandMultiLineText";
			this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
			this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText.Text = "Expand Multi-Line Text";
			this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_ExpandMultiLineText_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_6
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_6.Name = "toolStripMenuItem_SendContextMenu_Separator_6";
			this.toolStripMenuItem_SendContextMenu_Separator_6.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_SkipEmptyLines
			// 
			this.toolStripMenuItem_SendContextMenu_SkipEmptyLines.Name = "toolStripMenuItem_SendContextMenu_SkipEmptyLines";
			this.toolStripMenuItem_SendContextMenu_SkipEmptyLines.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.M)));
			this.toolStripMenuItem_SendContextMenu_SkipEmptyLines.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_SkipEmptyLines.Text = "Skip Empty Lines on [Send File]";
			this.toolStripMenuItem_SendContextMenu_SkipEmptyLines.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_SkipEmptyLines_Click);
			// 
			// toolStripMenuItem_SendContextMenu_EnableEscapesForFile
			// 
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Name = "toolStripMenuItem_SendContextMenu_EnableEscapesForFile";
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Text = "Enable <...> and \\... Escapes on [Send File]";
			this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_EnableEscapesForFile_Click);
			// 
			// toolStripMenuItem_SendContextMenu_Separator_7
			// 
			this.toolStripMenuItem_SendContextMenu_Separator_7.Name = "toolStripMenuItem_SendContextMenu_Separator_7";
			this.toolStripMenuItem_SendContextMenu_Separator_7.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_SendContextMenu_CopyPredefined
			// 
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Name = "toolStripMenuItem_SendContextMenu_CopyPredefined";
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Text = "Copy Predefined to [Send Text/File]";
			this.toolStripMenuItem_SendContextMenu_CopyPredefined.Click += new System.EventHandler(this.toolStripMenuItem_SendContextMenu_CopyPredefined_Click);
			// 
			// menuStrip_Terminal
			// 
			this.menuStrip_Terminal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_File,
            this.toolStripMenuItem_TerminalMenu_Terminal,
            this.toolStripMenuItem_TerminalMenu_Send,
            this.toolStripMenuItem_TerminalMenu_Receive,
            this.toolStripMenuItem_TerminalMenu_Log,
            this.toolStripMenuItem_TerminalMenu_View});
			this.menuStrip_Terminal.Location = new System.Drawing.Point(0, 0);
			this.menuStrip_Terminal.Name = "menuStrip_Terminal";
			this.menuStrip_Terminal.Size = new System.Drawing.Size(884, 24);
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
			this.toolStripMenuItem_TerminalMenu_File.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_File_Close
			// 
			this.toolStripMenuItem_TerminalMenu_File_Close.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_delete_16x16;
			this.toolStripMenuItem_TerminalMenu_File_Close.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_File_Close.MergeIndex = 3;
			this.toolStripMenuItem_TerminalMenu_File_Close.Name = "toolStripMenuItem_TerminalMenu_File_Close";
			this.toolStripMenuItem_TerminalMenu_File_Close.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem_TerminalMenu_File_Close.Text = "&Close Terminal";
			this.toolStripMenuItem_TerminalMenu_File_Close.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_Close_Click);
			// 
			// toolStripMenuItem_TerminalMenu_File_Save
			// 
			this.toolStripMenuItem_TerminalMenu_File_Save.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_save_16x16;
			this.toolStripMenuItem_TerminalMenu_File_Save.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_File_Save.MergeIndex = 6;
			this.toolStripMenuItem_TerminalMenu_File_Save.Name = "toolStripMenuItem_TerminalMenu_File_Save";
			this.toolStripMenuItem_TerminalMenu_File_Save.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.toolStripMenuItem_TerminalMenu_File_Save.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem_TerminalMenu_File_Save.Text = "&Save Terminal";
			this.toolStripMenuItem_TerminalMenu_File_Save.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_Save_Click);
			// 
			// toolStripMenuItem_TerminalMenu_File_SaveAs
			// 
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_save_as_16x16;
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.MergeIndex = 7;
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Name = "toolStripMenuItem_TerminalMenu_File_SaveAs";
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Size = new System.Drawing.Size(186, 22);
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Text = "Save Terminal &As...";
			this.toolStripMenuItem_TerminalMenu_File_SaveAs.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_File_SaveAs_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Terminal_Start,
            this.toolStripMenuItem_TerminalMenu_Terminal_Stop,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1,
            this.toolStripMenuItem_TerminalMenu_Terminal_Break,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2,
            this.toolStripMenuItem_TerminalMenu_Terminal_Clear,
            this.toolStripMenuItem_TerminalMenu_Terminal_Refresh,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3,
            this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll,
            this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4,
            this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard,
            this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile,
            this.toolStripMenuItem_TerminalMenu_Terminal_Print,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_5,
            this.toolStripMenuItem_TerminalMenu_Terminal_Find,
            this.toolStripMenuItem_TerminalMenu_Terminal_FindNext,
            this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious,
            this.toolStripMenuItem_TerminalMenu_Terminal_Separator_6,
            this.toolStripMenuItem_TerminalMenu_Terminal_Settings,
            this.toolStripMenuItem_TerminalMenu_Terminal_Presets});
			this.toolStripMenuItem_TerminalMenu_Terminal.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.toolStripMenuItem_TerminalMenu_Terminal.MergeIndex = 1;
			this.toolStripMenuItem_TerminalMenu_Terminal.Name = "toolStripMenuItem_TerminalMenu_Terminal";
			this.toolStripMenuItem_TerminalMenu_Terminal.Size = new System.Drawing.Size(64, 20);
			this.toolStripMenuItem_TerminalMenu_Terminal.Text = "&Terminal";
			this.toolStripMenuItem_TerminalMenu_Terminal.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Start
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_accept_button_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.MergeIndex = 0;
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Name = "toolStripMenuItem_TerminalMenu_Terminal_Start";
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.O)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Text = "&Open/Start";
			this.toolStripMenuItem_TerminalMenu_Terminal_Start.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Start_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Stop
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_delete_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.MergeIndex = 1;
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Name = "toolStripMenuItem_TerminalMenu_Terminal_Stop";
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Text = "C&lose/Stop";
			this.toolStripMenuItem_TerminalMenu_Terminal_Stop.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Stop_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1.MergeIndex = 2;
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_1";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_1.Size = new System.Drawing.Size(211, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Break
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.MergeIndex = 3;
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.Name = "toolStripMenuItem_TerminalMenu_Terminal_Break";
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.Text = "&Break Operation";
			this.toolStripMenuItem_TerminalMenu_Terminal_Break.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Break_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2.MergeIndex = 4;
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_2";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_2.Size = new System.Drawing.Size(211, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Clear
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_lightning_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.MergeIndex = 5;
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Name = "toolStripMenuItem_TerminalMenu_Terminal_Clear";
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.L)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Text = "Cl&ear";
			this.toolStripMenuItem_TerminalMenu_Terminal_Clear.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Clear_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Refresh
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_refresh_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.MergeIndex = 6;
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.Name = "toolStripMenuItem_TerminalMenu_Terminal_Refresh";
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh";
			this.toolStripMenuItem_TerminalMenu_Terminal_Refresh.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Refresh_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_3";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_3.Size = new System.Drawing.Size(211, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_SelectAll
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Name = "toolStripMenuItem_TerminalMenu_Terminal_SelectAll";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Text = "Select &All";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_SelectAll_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_SelectNone
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Name = "toolStripMenuItem_TerminalMenu_Terminal_SelectNone";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Text = "Select &None";
			this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_SelectNone_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_4";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_4.Size = new System.Drawing.Size(211, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_text_exports_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Name = "toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard";
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Text = "&Copy to Clipboard";
			this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_SaveToFile
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_save_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Name = "toolStripMenuItem_TerminalMenu_Terminal_SaveToFile";
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Text = "Save &to File...";
			this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_SaveToFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Print
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_printer_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Name = "toolStripMenuItem_TerminalMenu_Terminal_Print";
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Text = "&Print...";
			this.toolStripMenuItem_TerminalMenu_Terminal_Print.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Print_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_5
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_5.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_5";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_5.Size = new System.Drawing.Size(211, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Find
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Find.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_tab_search_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Find.Name = "toolStripMenuItem_TerminalMenu_Terminal_Find";
			this.toolStripMenuItem_TerminalMenu_Terminal_Find.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Find.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Find.Text = "&Find...";
			this.toolStripMenuItem_TerminalMenu_Terminal_Find.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Find_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_FindNext
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_rows_insert_below_word_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext.Name = "toolStripMenuItem_TerminalMenu_Terminal_FindNext";
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext.Text = "F&ind Next";
			this.toolStripMenuItem_TerminalMenu_Terminal_FindNext.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_FindNext_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_FindPrevious
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_table_rows_insert_above_word_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.Name = "toolStripMenuItem_TerminalMenu_Terminal_FindPrevious";
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.Text = "Fin&d Previous";
			this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_FindPrevious_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Separator_6
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_6.Name = "toolStripMenuItem_TerminalMenu_Terminal_Separator_6";
			this.toolStripMenuItem_TerminalMenu_Terminal_Separator_6.Size = new System.Drawing.Size(211, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Settings
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_application_edit_16x16;
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Name = "toolStripMenuItem_TerminalMenu_Terminal_Settings";
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Text = "&Settings...";
			this.toolStripMenuItem_TerminalMenu_Terminal_Settings.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Terminal_Settings_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Terminal_Presets
			// 
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.DropDown = this.contextMenuStrip_Preset;
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.Name = "toolStripMenuItem_TerminalMenu_Terminal_Presets";
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.Size = new System.Drawing.Size(214, 22);
			this.toolStripMenuItem_TerminalMenu_Terminal_Presets.Text = "Settin&gs Presets";
			// 
			// contextMenuStrip_Preset
			// 
			this.contextMenuStrip_Preset.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_PresetContextMenu_Preset_1,
            this.toolStripMenuItem_PresetContextMenu_Preset_2,
            this.toolStripMenuItem_PresetContextMenu_Preset_3,
            this.toolStripMenuItem_PresetContextMenu_Preset_4,
            this.toolStripMenuItem_PresetContextMenu_Preset_5,
            this.toolStripMenuItem_PresetContextMenu_Preset_6,
            this.toolStripMenuItem_PresetContextMenu_Preset_7,
            this.toolStripMenuItem_PresetContextMenu_Preset_8});
			this.contextMenuStrip_Preset.Name = "contextMenuStrip_Preset";
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
			this.contextMenuStrip_Preset.OwnerItem = this.toolStripMenuItem_TerminalMenu_Terminal_Presets;
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
			this.contextMenuStrip_Preset.Size = new System.Drawing.Size(234, 180);
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
			this.toolStripMenuItem_PresetContextMenu_Preset_2.Text = "&2: 2400, 7, Even, 1, Software";
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
			this.toolStripMenuItem_PresetContextMenu_Preset_4.Text = "&4: 9600, 8, None, 1, Software";
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
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Text = "&6: 19200, 8, None, 1, Software";
			this.toolStripMenuItem_PresetContextMenu_Preset_6.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_7
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_7.Name = "toolStripMenuItem_PresetContextMenu_Preset_7";
			this.toolStripMenuItem_PresetContextMenu_Preset_7.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_7.Tag = "7";
			this.toolStripMenuItem_PresetContextMenu_Preset_7.Text = "&7: 115200, 8, None, 1, None";
			this.toolStripMenuItem_PresetContextMenu_Preset_7.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_PresetContextMenu_Preset_8
			// 
			this.toolStripMenuItem_PresetContextMenu_Preset_8.Name = "toolStripMenuItem_PresetContextMenu_Preset_8";
			this.toolStripMenuItem_PresetContextMenu_Preset_8.Size = new System.Drawing.Size(233, 22);
			this.toolStripMenuItem_PresetContextMenu_Preset_8.Tag = "8";
			this.toolStripMenuItem_PresetContextMenu_Preset_8.Text = "&8: 115200, 8, None, 1, Software";
			this.toolStripMenuItem_PresetContextMenu_Preset_8.Click += new System.EventHandler(this.toolStripMenuItem_PresetContextMenu_Preset_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send
			// 
			this.toolStripMenuItem_TerminalMenu_Send.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Send_Text,
            this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol,
            this.toolStripMenuItem_TerminalMenu_Send_File,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_1,
            this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix,
            this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_2,
            this.toolStripMenuItem_TerminalMenu_Send_KeepSendText,
            this.toolStripMenuItem_TerminalMenu_Send_SendImmediately,
            this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_3,
            this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_4,
            this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines,
            this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_5,
            this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommands,
            this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage,
            this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_6,
            this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined,
            this.toolStripMenuItem_TerminalMenu_Send_Separator_7,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse});
			this.toolStripMenuItem_TerminalMenu_Send.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Send.MergeIndex = 2;
			this.toolStripMenuItem_TerminalMenu_Send.Name = "toolStripMenuItem_TerminalMenu_Send";
			this.toolStripMenuItem_TerminalMenu_Send.Size = new System.Drawing.Size(45, 20);
			this.toolStripMenuItem_TerminalMenu_Send.Text = "&Send";
			this.toolStripMenuItem_TerminalMenu_Send.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Text
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Text.Name = "toolStripMenuItem_TerminalMenu_Send_Text";
			this.toolStripMenuItem_TerminalMenu_Send_Text.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.toolStripMenuItem_TerminalMenu_Send_Text.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_Text.Text = "&Text";
			this.toolStripMenuItem_TerminalMenu_Send_Text.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_Text_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_TextWithoutEol
			// 
			this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Name = "toolStripMenuItem_TerminalMenu_Send_TextWithoutEol";
			this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
			this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Text = "Text w/&o EOL";
			this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_TextWithoutEol_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_File
			// 
			this.toolStripMenuItem_TerminalMenu_Send_File.Name = "toolStripMenuItem_TerminalMenu_Send_File";
			this.toolStripMenuItem_TerminalMenu_Send_File.ShortcutKeys = System.Windows.Forms.Keys.F4;
			this.toolStripMenuItem_TerminalMenu_Send_File.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_File.Text = "&File";
			this.toolStripMenuItem_TerminalMenu_Send_File.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_File_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_1.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_1";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_1.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix
			// 
			this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Name = "toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix";
			this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Text = "&Use Explicit Default Radix";
			this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.ToolTipText = "When enabled, the default radix can explicitly be selected.\r\nWhen disabled, the d" +
    "efault radix is \'String\'.\r\nApplies to [Send Text], [Send File] as well as [Prede" +
    "fined Commands].";
			this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AllowConcurrency
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.Name = "toolStripMenuItem_TerminalMenu_Send_AllowConcurrency";
			this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.Text = "Allo&w Concurrent Sending";
			this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.ToolTipText = resources.GetString("toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.ToolTipText");
			this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AllowConcurrency_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_2.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_2";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_2.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_KeepSendText
			// 
			this.toolStripMenuItem_TerminalMenu_Send_KeepSendText.Name = "toolStripMenuItem_TerminalMenu_Send_KeepSendText";
			this.toolStripMenuItem_TerminalMenu_Send_KeepSendText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.K)));
			this.toolStripMenuItem_TerminalMenu_Send_KeepSendText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_KeepSendText.Text = "&Keep [Send Text] after Send";
			this.toolStripMenuItem_TerminalMenu_Send_KeepSendText.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_KeepSendText_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_SendImmediately
			// 
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Name = "toolStripMenuItem_TerminalMenu_Send_SendImmediately";
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.I)));
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Text = "Send Each Character &Immediately";
			this.toolStripMenuItem_TerminalMenu_Send_SendImmediately.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_SendImmediately_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText
			// 
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Name = "toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText";
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Text = "&Enable <...> and \\... Escapes on [Send Text]";
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_3.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_3";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_3.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText
			// 
			this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Name = "toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText";
			this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
			this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Text = "E&xpand Multi-Line Text";
			this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_4.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_4";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_4.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines
			// 
			this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Name = "toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines";
			this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.M)));
			this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Text = "&Skip Empty Lines on [Send File]";
			this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile
			// 
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Name = "toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile";
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Text = "E&nable <...> and \\... Escapes on [Send File]";
			this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_5
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_5.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_5";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_5.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine
			// 
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine.Name = "toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine";
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine.Text = "Predefined Commands &Define...";
			this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_6
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_6.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_6";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_6.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_CopyPredefined
			// 
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Name = "toolStripMenuItem_TerminalMenu_Send_CopyPredefined";
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.P)));
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Text = "Copy &Predefined to [Send Text/File]";
			this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_CopyPredefined_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_Separator_7
			// 
			this.toolStripMenuItem_TerminalMenu_Send_Separator_7.Name = "toolStripMenuItem_TerminalMenu_Send_Separator_7";
			this.toolStripMenuItem_TerminalMenu_Send_Separator_7.Size = new System.Drawing.Size(330, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate});
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_autoresponders_16x16;
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse.Size = new System.Drawing.Size(333, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse.Text = "&Automatic Response";
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex});
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.Size = new System.Drawing.Size(199, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.Text = "&Trigger/Pattern";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.ToolTipText = "The trigger or pattern that initiates an automatic response";
			// 
			// toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger
			// 
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Name = "toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger";
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Size = new System.Drawing.Size(180, 23);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.ToolTipText = resources.GetString("toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.ToolTipText");
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_SelectedIndexChanged);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Enter += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_Enter);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Leave += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_Leave);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.TextChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_TextChanged);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_formatting_text_containing_16x16;
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Text = "Use &Text";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.ToolTipText = "By default, the trigger is based on the byte sequence of a command.\r\nThis option " +
    "switches to a trigger based on text.";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_match_character_literally_16x16;
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Text = "&Case Sensitive";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.ToolTipText = "Case Sensitive\r\nSame as \"(?-i)\" when using regex";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_literal_text_16x16;
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Text = "Whole &Word";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.ToolTipText = "Whole Word\r\nSame as \"\\bSomeWord\\b\" when using regex";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.Text = "Enable Regular &Expression";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.ToolTipText = "Google for \".NET Regular Expression Quick Reference\" for syntax.";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response,
            this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace});
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.Size = new System.Drawing.Size(199, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.Text = "&Response";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.ToolTipText = "The response that is automatically sent";
			// 
			// toolStripComboBox_TerminalMenu_Send_AutoResponse_Response
			// 
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Name = "toolStripComboBox_TerminalMenu_Send_AutoResponse_Response";
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Size = new System.Drawing.Size(180, 23);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.ToolTipText = "Enable / Disable Automatic Response,\r\neither refer to one of the commands,\r\nor fi" +
    "ll-in any text.";
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_SelectedIndexChanged);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Enter += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_Enter);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Leave += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_Leave);
			this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.TextChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_TextChanged);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Text = "Enable &Replace";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.ToolTipText = resources.GetString("toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.ToolTipTe" +
        "xt");
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate
			// 
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_email_delete_16x16;
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Name = "toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Size = new System.Drawing.Size(199, 22);
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Text = "&Deactivate";
			this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Receive
			// 
			this.toolStripMenuItem_TerminalMenu_Receive.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction});
			this.toolStripMenuItem_TerminalMenu_Receive.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Receive.MergeIndex = 3;
			this.toolStripMenuItem_TerminalMenu_Receive.Name = "toolStripMenuItem_TerminalMenu_Receive";
			this.toolStripMenuItem_TerminalMenu_Receive.Size = new System.Drawing.Size(59, 20);
			this.toolStripMenuItem_TerminalMenu_Receive.Text = "&Receive";
			this.toolStripMenuItem_TerminalMenu_Receive.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Receive_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger,
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action,
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate});
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_16x16;
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction.Size = new System.Drawing.Size(168, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction.Text = "&Automatic Action";
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger,
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText,
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive,
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord,
            this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex});
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.Text = "&Trigger/Pattern";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.ToolTipText = "The trigger or pattern that initiates an action";
			// 
			// toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger
			// 
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Name = "toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger";
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Size = new System.Drawing.Size(180, 23);
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.ToolTipText = resources.GetString("toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.ToolTipText");
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_SelectedIndexChanged);
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Enter += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_Enter);
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Leave += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_Leave);
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.TextChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_TextChanged);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_formatting_text_containing_16x16;
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Text = "Use &Text";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.ToolTipText = "By default, the trigger is based on the byte sequence of a command.\r\nThis option " +
    "switches to a trigger based on text.";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_match_character_literally_16x16;
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Text = "&Case Sensitive";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.ToolTipText = "Case Sensitive\r\nSame as \"(?-i)\" when using regex";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_token_literal_text_16x16;
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Text = "Whole &Word";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.ToolTipText = "Whole Word\r\nSame as \"\\bSomeWord\\b\" when using regex";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.Size = new System.Drawing.Size(240, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.Text = "Enable Regular &Expression";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.ToolTipText = "Google for \".NET Regular Expression Quick Reference\" for syntax.";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action});
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.Text = "&Action";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.ToolTipText = "The action that is automatically executed";
			// 
			// toolStripComboBox_TerminalMenu_Receive_AutoAction_Action
			// 
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Name = "toolStripComboBox_TerminalMenu_Receive_AutoAction_Action";
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Size = new System.Drawing.Size(180, 23);
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.ToolTipText = resources.GetString("toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.ToolTipText");
			this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_Receive_AutoAction_Action_SelectedIndexChanged);
			// 
			// toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate
			// 
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_comments_delete_16x16;
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Name = "toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.E)));
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Size = new System.Drawing.Size(197, 22);
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Text = "&Deactivate";
			this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log
			// 
			this.toolStripMenuItem_TerminalMenu_Log.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_Log_On,
            this.toolStripMenuItem_TerminalMenu_Log_Off,
            this.toolStripMenuItem_TerminalMenu_Log_Separator_1,
            this.toolStripMenuItem_TerminalMenu_Log_OpenFile,
            this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory,
            this.toolStripMenuItem_TerminalMenu_Log_Separator_2,
            this.toolStripMenuItem_TerminalMenu_Log_Clear,
            this.toolStripMenuItem_TerminalMenu_Log_Separator_3,
            this.toolStripMenuItem_TerminalMenu_Log_Settings});
			this.toolStripMenuItem_TerminalMenu_Log.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
			this.toolStripMenuItem_TerminalMenu_Log.MergeIndex = 4;
			this.toolStripMenuItem_TerminalMenu_Log.Name = "toolStripMenuItem_TerminalMenu_Log";
			this.toolStripMenuItem_TerminalMenu_Log.Size = new System.Drawing.Size(39, 20);
			this.toolStripMenuItem_TerminalMenu_Log.Text = "&Log";
			this.toolStripMenuItem_TerminalMenu_Log.DropDownOpening += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_DropDownOpening);
			// 
			// toolStripMenuItem_TerminalMenu_Log_On
			// 
			this.toolStripMenuItem_TerminalMenu_Log_On.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_accept_document_16x16;
			this.toolStripMenuItem_TerminalMenu_Log_On.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log_On.MergeIndex = 0;
			this.toolStripMenuItem_TerminalMenu_Log_On.Name = "toolStripMenuItem_TerminalMenu_Log_On";
			this.toolStripMenuItem_TerminalMenu_Log_On.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
			this.toolStripMenuItem_TerminalMenu_Log_On.Size = new System.Drawing.Size(250, 22);
			this.toolStripMenuItem_TerminalMenu_Log_On.Text = "O&n";
			this.toolStripMenuItem_TerminalMenu_Log_On.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_On_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Off
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Off.Enabled = false;
			this.toolStripMenuItem_TerminalMenu_Log_Off.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_delete_16x16;
			this.toolStripMenuItem_TerminalMenu_Log_Off.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log_Off.MergeIndex = 1;
			this.toolStripMenuItem_TerminalMenu_Log_Off.Name = "toolStripMenuItem_TerminalMenu_Log_Off";
			this.toolStripMenuItem_TerminalMenu_Log_Off.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
			this.toolStripMenuItem_TerminalMenu_Log_Off.Size = new System.Drawing.Size(250, 22);
			this.toolStripMenuItem_TerminalMenu_Log_Off.Text = "O&ff";
			this.toolStripMenuItem_TerminalMenu_Log_Off.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_Off_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1.MergeAction = System.Windows.Forms.MergeAction.Replace;
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1.MergeIndex = 4;
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1.Name = "toolStripMenuItem_TerminalMenu_Log_Separator_1";
			this.toolStripMenuItem_TerminalMenu_Log_Separator_1.Size = new System.Drawing.Size(247, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Log_OpenFile
			// 
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.Enabled = false;
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_magnify_16x16;
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.MergeIndex = 5;
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.Name = "toolStripMenuItem_TerminalMenu_Log_OpenFile";
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.Size = new System.Drawing.Size(250, 22);
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.Text = "&Open File(s) in Editor...";
			this.toolStripMenuItem_TerminalMenu_Log_OpenFile.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_OpenFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_OpenDirectory
			// 
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_folder_explorer_16x16;
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.MergeIndex = 6;
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.Name = "toolStripMenuItem_TerminalMenu_Log_OpenDirectory";
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.Size = new System.Drawing.Size(250, 22);
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.Text = "O&pen Log Folder in File Browser...";
			this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_OpenDirectory_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2.MergeIndex = 7;
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2.Name = "toolStripMenuItem_TerminalMenu_Log_Separator_2";
			this.toolStripMenuItem_TerminalMenu_Log_Separator_2.Size = new System.Drawing.Size(247, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Clear
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Enabled = false;
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_lightning_16x16;
			this.toolStripMenuItem_TerminalMenu_Log_Clear.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_Log_Clear.MergeIndex = 8;
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Name = "toolStripMenuItem_TerminalMenu_Log_Clear";
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Size = new System.Drawing.Size(250, 22);
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Text = "&Clear";
			this.toolStripMenuItem_TerminalMenu_Log_Clear.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_Clear_Click);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Separator_3.Name = "toolStripMenuItem_TerminalMenu_Log_Separator_3";
			this.toolStripMenuItem_TerminalMenu_Log_Separator_3.Size = new System.Drawing.Size(247, 6);
			// 
			// toolStripMenuItem_TerminalMenu_Log_Settings
			// 
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_page_white_edit_16x16;
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Name = "toolStripMenuItem_TerminalMenu_Log_Settings";
			this.toolStripMenuItem_TerminalMenu_Log_Settings.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Size = new System.Drawing.Size(250, 22);
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Text = "&Settings...";
			this.toolStripMenuItem_TerminalMenu_Log_Settings.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_Log_Settings_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View
			// 
			this.toolStripMenuItem_TerminalMenu_View.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_Panels,
            this.toolStripMenuItem_TerminalMenu_View_Separator_1,
            this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime,
            this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime,
            this.toolStripMenuItem_TerminalMenu_View_Separator_2,
            this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate,
            this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount,
            this.toolStripMenuItem_TerminalMenu_View_Separator_3,
            this.toolStripMenuItem_TerminalMenu_View_Radix,
            this.toolStripMenuItem_TerminalMenu_View_ShowRadix,
            this.toolStripMenuItem_TerminalMenu_View_Separator_4,
            this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers,
            this.toolStripMenuItem_TerminalMenu_View_Separator_5,
            this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp,
            this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan,
            this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta,
            this.toolStripMenuItem_TerminalMenu_View_ShowDevice,
            this.toolStripMenuItem_TerminalMenu_View_ShowDirection,
            this.toolStripMenuItem_TerminalMenu_View_ShowEol,
            this.toolStripMenuItem_TerminalMenu_View_ShowLength,
            this.toolStripMenuItem_TerminalMenu_View_ShowDuration,
            this.toolStripMenuItem_TerminalMenu_View_Separator_6,
            this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine,
            this.toolStripMenuItem_TerminalMenu_View_Separator_7,
            this.toolStripMenuItem_TerminalMenu_View_FlowControlCount,
            this.toolStripMenuItem_TerminalMenu_View_BreakCount,
            this.toolStripMenuItem_TerminalMenu_View_Separator_8,
            this.toolStripMenuItem_TerminalMenu_View_Format,
            this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting});
			this.toolStripMenuItem_TerminalMenu_View.MergeAction = System.Windows.Forms.MergeAction.Insert;
			this.toolStripMenuItem_TerminalMenu_View.MergeIndex = 5;
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
            this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3,
            this.toolStripComboBox_TerminalMenu_View_Panels_PageLayout,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4,
            this.toolStripMenuItem_TerminalMenu_View_Panels_SendText,
            this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile,
            this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_5,
            this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll});
			this.toolStripMenuItem_TerminalMenu_View_Panels.Name = "toolStripMenuItem_TerminalMenu_View_Panels";
			this.toolStripMenuItem_TerminalMenu_View_Panels.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels.Text = "&Panels";
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Tx
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Tx";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Text = "Unidirectional &Send (Tx) Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Tx.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Tx_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Bidir
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Bidir";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Text = "&Bidirectional Send/Receive Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Bidir_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Rx
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Rx";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Text = "Unidirectional &Receive (Rx) Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Rx.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Rx_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_1";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_1.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation
			// 
			this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation.Name = "toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation";
			this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation.Size = new System.Drawing.Size(165, 23);
			this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation_SelectedIndexChanged);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_2";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_2.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Predefined
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Predefined";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Text = "&Predefined Commands Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Predefined_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_3";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_3.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripComboBox_TerminalMenu_View_Panels_PageLayout
			// 
			this.toolStripComboBox_TerminalMenu_View_Panels_PageLayout.Name = "toolStripComboBox_TerminalMenu_View_Panels_PageLayout";
			this.toolStripComboBox_TerminalMenu_View_Panels_PageLayout.Size = new System.Drawing.Size(165, 23);
			this.toolStripComboBox_TerminalMenu_View_Panels_PageLayout.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox_TerminalMenu_View_Panels_PageLayout_SelectedIndexChanged);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_4";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_4.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_SendText
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText.Name = "toolStripMenuItem_TerminalMenu_View_Panels_SendText";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText.Text = "Send &Text Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendText.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_SendText_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_SendFile
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked = true;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Name = "toolStripMenuItem_TerminalMenu_View_Panels_SendFile";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Text = "Send &File Panel";
			this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_SendFile_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_Separator_5
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_5.Name = "toolStripMenuItem_TerminalMenu_View_Panels_Separator_5";
			this.toolStripMenuItem_TerminalMenu_View_Panels_Separator_5.Size = new System.Drawing.Size(244, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll
			// 
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Name = "toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll";
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Size = new System.Drawing.Size(247, 22);
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Text = "R&earrange All";
			this.toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Panels_Rearrange_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_1
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_1.Name = "toolStripMenuItem_TerminalMenu_View_Separator_1";
			this.toolStripMenuItem_TerminalMenu_View_Separator_1.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime
			// 
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Name = "toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Text = "Show &Connect Time";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime
			// 
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime.Name = "toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime.Text = "Reset Connect Time";
			this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_2
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_2.Name = "toolStripMenuItem_TerminalMenu_View_Separator_2";
			this.toolStripMenuItem_TerminalMenu_View_Separator_2.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate
			// 
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Name = "toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Text = "S&how Byte/Line Count/Rate";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Name = "toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Text = "Reset Byte/Line Count/Rate";
			this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_3
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_3.Name = "toolStripMenuItem_TerminalMenu_View_Separator_3";
			this.toolStripMenuItem_TerminalMenu_View_Separator_3.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowRadix
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Name = "toolStripMenuItem_TerminalMenu_View_ShowRadix";
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Text = "Show R&adix";
			this.toolStripMenuItem_TerminalMenu_View_ShowRadix.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowRadix_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_4
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_4.Name = "toolStripMenuItem_TerminalMenu_View_Separator_4";
			this.toolStripMenuItem_TerminalMenu_View_Separator_4.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowLineNumbers
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Name = "toolStripMenuItem_TerminalMenu_View_ShowLineNumbers";
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Text = "Show Line &Numbers (Total Absolute)";
			this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowLineNumbers_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_5
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_5.Name = "toolStripMenuItem_TerminalMenu_View_Separator_5";
			this.toolStripMenuItem_TerminalMenu_View_Separator_5.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowTimeStamp
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Name = "toolStripMenuItem_TerminalMenu_View_ShowTimeStamp";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Text = "Show T&ime Stamp";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.ToolTipText = "The absolute moment in time.\r\n\r\nFormat can be configured in [Format...] further b" +
    "elow.";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowTimeStamp_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowTimeSpan
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.Name = "toolStripMenuItem_TerminalMenu_View_ShowTimeSpan";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.Text = "Show Ti&me Span";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.ToolTipText = "The time that passed relative to [Connect Time].\r\n\r\nFormat can be configured in [" +
    "Format...] further below.";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowTimeSpan_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowTimeDelta
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.Name = "toolStripMenuItem_TerminalMenu_View_ShowTimeDelta";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.Text = "Show Tim&e Delta";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.ToolTipText = "The time that passed relative to the last line.\r\n\r\nFormat can be configured in [F" +
    "ormat...] further below.";
			this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowTimeDelta_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowDevice
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowDevice.Name = "toolStripMenuItem_TerminalMenu_View_ShowDevice";
			this.toolStripMenuItem_TerminalMenu_View_ShowDevice.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowDevice.Text = "Show I/O De&vice";
			this.toolStripMenuItem_TerminalMenu_View_ShowDevice.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowDevice_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowDirection
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowDirection.Name = "toolStripMenuItem_TerminalMenu_View_ShowDirection";
			this.toolStripMenuItem_TerminalMenu_View_ShowDirection.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowDirection.Text = "Show &Direction";
			this.toolStripMenuItem_TerminalMenu_View_ShowDirection.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowDirection_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowEol
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Name = "toolStripMenuItem_TerminalMenu_View_ShowEol";
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Text = "Show EOL Se&quence";
			this.toolStripMenuItem_TerminalMenu_View_ShowEol.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowEol_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowLength
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Name = "toolStripMenuItem_TerminalMenu_View_ShowLength";
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Text = "Show &Length (Byte Count)";
			this.toolStripMenuItem_TerminalMenu_View_ShowLength.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowLength_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowDuration
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowDuration.Name = "toolStripMenuItem_TerminalMenu_View_ShowDuration";
			this.toolStripMenuItem_TerminalMenu_View_ShowDuration.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowDuration.Text = "Show D&uration (Line)";
			this.toolStripMenuItem_TerminalMenu_View_ShowDuration.ToolTipText = "The duration from first to last byte of the line.\r\n\r\nFormat can be configured in " +
    "[Format...] further below.";
			this.toolStripMenuItem_TerminalMenu_View_ShowDuration.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowDuration_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_6
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_6.Name = "toolStripMenuItem_TerminalMenu_View_Separator_6";
			this.toolStripMenuItem_TerminalMenu_View_Separator_6.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine
			// 
			this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine.Name = "toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine";
			this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine.Text = "Show Cop&y of Active Line";
			this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_7
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_7.Name = "toolStripMenuItem_TerminalMenu_View_Separator_7";
			this.toolStripMenuItem_TerminalMenu_View_Separator_7.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_FlowControlCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount,
            this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount});
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount.Name = "toolStripMenuItem_TerminalMenu_View_FlowControlCount";
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount.Text = "Flo&w Control Count";
			// 
			// toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount.Name = "toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount";
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount.Size = new System.Drawing.Size(139, 22);
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount.Text = "&Show Count";
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount.Name = "toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount";
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount.Size = new System.Drawing.Size(139, 22);
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount.Text = "&Reset Count";
			this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_BreakCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_BreakCount.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount,
            this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount});
			this.toolStripMenuItem_TerminalMenu_View_BreakCount.Name = "toolStripMenuItem_TerminalMenu_View_BreakCount";
			this.toolStripMenuItem_TerminalMenu_View_BreakCount.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_BreakCount.Text = "Brea&k Count";
			// 
			// toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount.Name = "toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount";
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount.Size = new System.Drawing.Size(139, 22);
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount.Text = "&Show Count";
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount
			// 
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount.Name = "toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount";
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount.Size = new System.Drawing.Size(139, 22);
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount.Text = "&Reset Count";
			this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_Separator_8
			// 
			this.toolStripMenuItem_TerminalMenu_View_Separator_8.Name = "toolStripMenuItem_TerminalMenu_View_Separator_8";
			this.toolStripMenuItem_TerminalMenu_View_Separator_8.Size = new System.Drawing.Size(263, 6);
			// 
			// toolStripMenuItem_TerminalMenu_View_Format
			// 
			this.toolStripMenuItem_TerminalMenu_View_Format.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_font_16x16;
			this.toolStripMenuItem_TerminalMenu_View_Format.Name = "toolStripMenuItem_TerminalMenu_View_Format";
			this.toolStripMenuItem_TerminalMenu_View_Format.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_Format.Text = "&Format...";
			this.toolStripMenuItem_TerminalMenu_View_Format.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_Format_Click);
			// 
			// toolStripMenuItem_TerminalMenu_View_ToggleFormatting
			// 
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Image = global::YAT.View.Forms.Properties.Resources.Image_Tool_font_delete_16x16;
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Name = "toolStripMenuItem_TerminalMenu_View_ToggleFormatting";
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.T)));
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Size = new System.Drawing.Size(266, 22);
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Text = "Toggle Formatting";
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.ToolTipText = "Disable formatting when data throughput slows down the application too much.";
			this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Click += new System.EventHandler(this.toolStripMenuItem_TerminalMenu_View_ToggleFormatting_Click);
			// 
			// statusStrip_Terminal
			// 
			this.statusStrip_Terminal.ContextMenuStrip = this.contextMenuStrip_Status;
			this.statusStrip_Terminal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_TerminalStatus_Status,
            this.toolStripStatusLabel_TerminalStatus_IOStatus,
            this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator,
            this.toolStripStatusLabel_TerminalStatus_Separator1,
            this.toolStripStatusLabel_TerminalStatus_RTS,
            this.toolStripStatusLabel_TerminalStatus_CTS,
            this.toolStripStatusLabel_TerminalStatus_DTR,
            this.toolStripStatusLabel_TerminalStatus_DSR,
            this.toolStripStatusLabel_TerminalStatus_DCD,
            this.toolStripStatusLabel_TerminalStatus_Separator2,
            this.toolStripStatusLabel_TerminalStatus_InputXOnXOff,
            this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff,
            this.toolStripStatusLabel_TerminalStatus_Separator3,
            this.toolStripStatusLabel_TerminalStatus_InputBreak,
            this.toolStripStatusLabel_TerminalStatus_OutputBreak});
			this.statusStrip_Terminal.Location = new System.Drawing.Point(0, 537);
			this.statusStrip_Terminal.Name = "statusStrip_Terminal";
			this.statusStrip_Terminal.ShowItemToolTips = true;
			this.statusStrip_Terminal.Size = new System.Drawing.Size(884, 25);
			this.statusStrip_Terminal.TabIndex = 1;
			// 
			// contextMenuStrip_Status
			// 
			this.contextMenuStrip_Status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuStrip_Status_FlowControlCount,
            this.contextMenuStrip_Status_BreakCount});
			this.contextMenuStrip_Status.Name = "contextMenuStrip_Status";
			this.contextMenuStrip_Status.Size = new System.Drawing.Size(179, 48);
			this.contextMenuStrip_Status.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Status_Opening);
			// 
			// contextMenuStrip_Status_FlowControlCount
			// 
			this.contextMenuStrip_Status_FlowControlCount.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuStrip_Status_FlowControlCount_ShowCount,
            this.contextMenuStrip_Status_FlowControlCount_ResetCount});
			this.contextMenuStrip_Status_FlowControlCount.Name = "contextMenuStrip_Status_FlowControlCount";
			this.contextMenuStrip_Status_FlowControlCount.Size = new System.Drawing.Size(178, 22);
			this.contextMenuStrip_Status_FlowControlCount.Text = "Flo&w Control Count";
			// 
			// contextMenuStrip_Status_FlowControlCount_ShowCount
			// 
			this.contextMenuStrip_Status_FlowControlCount_ShowCount.Name = "contextMenuStrip_Status_FlowControlCount_ShowCount";
			this.contextMenuStrip_Status_FlowControlCount_ShowCount.Size = new System.Drawing.Size(139, 22);
			this.contextMenuStrip_Status_FlowControlCount_ShowCount.Text = "&Show Count";
			this.contextMenuStrip_Status_FlowControlCount_ShowCount.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_FlowControlCount_ShowCount_Click);
			// 
			// contextMenuStrip_Status_FlowControlCount_ResetCount
			// 
			this.contextMenuStrip_Status_FlowControlCount_ResetCount.Name = "contextMenuStrip_Status_FlowControlCount_ResetCount";
			this.contextMenuStrip_Status_FlowControlCount_ResetCount.Size = new System.Drawing.Size(139, 22);
			this.contextMenuStrip_Status_FlowControlCount_ResetCount.Text = "&Reset Count";
			this.contextMenuStrip_Status_FlowControlCount_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_FlowControlCount_ResetCount_Click);
			// 
			// contextMenuStrip_Status_BreakCount
			// 
			this.contextMenuStrip_Status_BreakCount.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuStrip_Status_BreakCount_ShowCount,
            this.contextMenuStrip_Status_BreakCount_ResetCount});
			this.contextMenuStrip_Status_BreakCount.Name = "contextMenuStrip_Status_BreakCount";
			this.contextMenuStrip_Status_BreakCount.Size = new System.Drawing.Size(178, 22);
			this.contextMenuStrip_Status_BreakCount.Text = "Brea&k Count";
			// 
			// contextMenuStrip_Status_BreakCount_ShowCount
			// 
			this.contextMenuStrip_Status_BreakCount_ShowCount.Name = "contextMenuStrip_Status_BreakCount_ShowCount";
			this.contextMenuStrip_Status_BreakCount_ShowCount.Size = new System.Drawing.Size(139, 22);
			this.contextMenuStrip_Status_BreakCount_ShowCount.Text = "&Show Count";
			this.contextMenuStrip_Status_BreakCount_ShowCount.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_BreakCount_ShowCount_Click);
			// 
			// contextMenuStrip_Status_BreakCount_ResetCount
			// 
			this.contextMenuStrip_Status_BreakCount_ResetCount.Name = "contextMenuStrip_Status_BreakCount_ResetCount";
			this.contextMenuStrip_Status_BreakCount_ResetCount.Size = new System.Drawing.Size(139, 22);
			this.contextMenuStrip_Status_BreakCount_ResetCount.Text = "&Reset Count";
			this.contextMenuStrip_Status_BreakCount_ResetCount.Click += new System.EventHandler(this.toolStripMenuItem_StatusContextMenu_BreakCount_ResetCount_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_Status
			// 
			this.toolStripStatusLabel_TerminalStatus_Status.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabel_TerminalStatus_Status.Name = "toolStripStatusLabel_TerminalStatus_Status";
			this.toolStripStatusLabel_TerminalStatus_Status.Size = new System.Drawing.Size(380, 20);
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
			this.toolStripStatusLabel_TerminalStatus_IOStatus.ToolTipText = "I/O Status and Settings";
			this.toolStripStatusLabel_TerminalStatus_IOStatus.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_IOStatus_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_IOStatusIndicator
			// 
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Name = "toolStripStatusLabel_TerminalStatus_IOStatusIndicator";
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Size = new System.Drawing.Size(16, 20);
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.toolStripStatusLabel_TerminalStatus_IOStatusIndicator.ToolTipText = "I/O Status\r\n\r\ngreen = open/connected\r\ngreen flashing = busy\r\nyellow = suspended\r\n" +
    "red = disconnected\r\ngrey = closed";
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
			this.toolStripStatusLabel_TerminalStatus_RTS.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
			this.toolStripStatusLabel_TerminalStatus_RTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_RTS.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_RTS.Name = "toolStripStatusLabel_TerminalStatus_RTS";
			this.toolStripStatusLabel_TerminalStatus_RTS.Size = new System.Drawing.Size(41, 20);
			this.toolStripStatusLabel_TerminalStatus_RTS.Text = "RTS";
			this.toolStripStatusLabel_TerminalStatus_RTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_RTS.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_RTS.ToolTipText = "Request To Send (Out)\r\nSame as RTR (Ready To Receive)\r\nAlso known as RFR (Ready F" +
    "or Receiving)";
			this.toolStripStatusLabel_TerminalStatus_RTS.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_RTS_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_CTS
			// 
			this.toolStripStatusLabel_TerminalStatus_CTS.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_CTS.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_CTS.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_CTS.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
			this.toolStripStatusLabel_TerminalStatus_CTS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_CTS.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_CTS.Name = "toolStripStatusLabel_TerminalStatus_CTS";
			this.toolStripStatusLabel_TerminalStatus_CTS.Size = new System.Drawing.Size(43, 20);
			this.toolStripStatusLabel_TerminalStatus_CTS.Text = "CTS";
			this.toolStripStatusLabel_TerminalStatus_CTS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_CTS.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_CTS.ToolTipText = "Clear To Send (In)";
			// 
			// toolStripStatusLabel_TerminalStatus_DTR
			// 
			this.toolStripStatusLabel_TerminalStatus_DTR.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_DTR.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_DTR.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Red_12x12;
			this.toolStripStatusLabel_TerminalStatus_DTR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_DTR.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_DTR.Name = "toolStripStatusLabel_TerminalStatus_DTR";
			this.toolStripStatusLabel_TerminalStatus_DTR.Size = new System.Drawing.Size(43, 20);
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
			this.toolStripStatusLabel_TerminalStatus_DSR.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Red_12x12;
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
			this.toolStripStatusLabel_TerminalStatus_DCD.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Red_12x12;
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
			// toolStripStatusLabel_TerminalStatus_InputXOnXOff
			// 
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Name = "toolStripStatusLabel_TerminalStatus_InputXOnXOff";
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Size = new System.Drawing.Size(39, 20);
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text = "IXS";
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText = "Input XOn/XOff State (Out)";
			this.toolStripStatusLabel_TerminalStatus_InputXOnXOff.Click += new System.EventHandler(this.toolStripStatusLabel_TerminalStatus_InputXOnXOff_Click);
			// 
			// toolStripStatusLabel_TerminalStatus_OutputXOnXOff
			// 
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Name = "toolStripStatusLabel_TerminalStatus_OutputXOnXOff";
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Size = new System.Drawing.Size(45, 20);
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text = "OXS";
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText = "Output XOn/XOff State (In)";
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
			// toolStripStatusLabel_TerminalStatus_InputBreak
			// 
			this.toolStripStatusLabel_TerminalStatus_InputBreak.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_InputBreak.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ForeColor = System.Drawing.SystemColors.GrayText;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Name = "toolStripStatusLabel_TerminalStatus_InputBreak";
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Size = new System.Drawing.Size(39, 20);
			this.toolStripStatusLabel_TerminalStatus_InputBreak.Text = "IBS";
			this.toolStripStatusLabel_TerminalStatus_InputBreak.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
			this.toolStripStatusLabel_TerminalStatus_InputBreak.ToolTipText = "Input Break State (In)";
			// 
			// toolStripStatusLabel_TerminalStatus_OutputBreak
			// 
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this.toolStripStatusLabel_TerminalStatus_OutputBreak.Image = global::YAT.View.Forms.Properties.Resources.Image_Status_Green_12x12;
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
			this.splitContainer_Terminal.Panel2MinSize = 96;
			this.splitContainer_Terminal.Size = new System.Drawing.Size(884, 537);
			this.splitContainer_Terminal.SplitterDistance = 440;
			this.splitContainer_Terminal.SplitterWidth = 1;
			this.splitContainer_Terminal.TabIndex = 0;
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
			this.splitContainer_Predefined.Panel2MinSize = 144;
			this.splitContainer_Predefined.Size = new System.Drawing.Size(884, 440);
			this.splitContainer_Predefined.SplitterDistance = 651;
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
			this.panel_Monitor.Size = new System.Drawing.Size(651, 440);
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
			this.groupBox_Monitor.Size = new System.Drawing.Size(647, 437);
			this.groupBox_Monitor.TabIndex = 0;
			this.groupBox_Monitor.TabStop = false;
			this.groupBox_Monitor.Text = "&Monitor";
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
			this.splitContainer_TxMonitor.Size = new System.Drawing.Size(641, 421);
			this.splitContainer_TxMonitor.SplitterDistance = 209;
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
			this.panel_Monitor_Tx.Size = new System.Drawing.Size(209, 421);
			this.panel_Monitor_Tx.TabIndex = 0;
			// 
			// monitor_Tx
			// 
			this.monitor_Tx.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Tx.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.monitor_Tx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitor_Tx.Location = new System.Drawing.Point(3, 0);
			this.monitor_Tx.Name = "monitor_Tx";
			this.monitor_Tx.RepositoryType = YAT.Domain.RepositoryType.Tx;
			this.monitor_Tx.Size = new System.Drawing.Size(203, 418);
			this.monitor_Tx.TabIndex = 0;
			this.monitor_Tx.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Tx.TextFocusedChanged += new System.EventHandler(this.monitor_TextFocusedChanged);
			this.monitor_Tx.FindChanged += new System.EventHandler(this.monitor_FindChanged);
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
			this.splitContainer_RxMonitor.Size = new System.Drawing.Size(428, 421);
			this.splitContainer_RxMonitor.SplitterDistance = 211;
			this.splitContainer_RxMonitor.TabIndex = 0;
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
			this.panel_Monitor_Bidir.Size = new System.Drawing.Size(211, 421);
			this.panel_Monitor_Bidir.TabIndex = 0;
			// 
			// monitor_Bidir
			// 
			this.monitor_Bidir.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Bidir.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.monitor_Bidir.DebugEnabled = true;
			this.monitor_Bidir.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitor_Bidir.Location = new System.Drawing.Point(3, 0);
			this.monitor_Bidir.Name = "monitor_Bidir";
			this.monitor_Bidir.RepositoryType = YAT.Domain.RepositoryType.Bidir;
			this.monitor_Bidir.Size = new System.Drawing.Size(205, 418);
			this.monitor_Bidir.TabIndex = 0;
			this.monitor_Bidir.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Bidir.TextFocusedChanged += new System.EventHandler(this.monitor_TextFocusedChanged);
			this.monitor_Bidir.FindChanged += new System.EventHandler(this.monitor_FindChanged);
			this.monitor_Bidir.Enter += new System.EventHandler(this.monitor_Bidir_Enter);
			// 
			// panel_Monitor_Rx
			// 
			this.panel_Monitor_Rx.Controls.Add(this.monitor_Rx);
			this.panel_Monitor_Rx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Monitor_Rx.Location = new System.Drawing.Point(0, 0);
			this.panel_Monitor_Rx.Name = "panel_Monitor_Rx";
			this.panel_Monitor_Rx.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Monitor_Rx.Size = new System.Drawing.Size(213, 421);
			this.panel_Monitor_Rx.TabIndex = 0;
			// 
			// monitor_Rx
			// 
			this.monitor_Rx.ActiveConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Rx.ContextMenuStrip = this.contextMenuStrip_Monitor;
			this.monitor_Rx.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monitor_Rx.Location = new System.Drawing.Point(3, 0);
			this.monitor_Rx.Name = "monitor_Rx";
			this.monitor_Rx.RepositoryType = YAT.Domain.RepositoryType.Rx;
			this.monitor_Rx.Size = new System.Drawing.Size(207, 418);
			this.monitor_Rx.TabIndex = 0;
			this.monitor_Rx.TotalConnectTime = System.TimeSpan.Parse("00:00:00");
			this.monitor_Rx.TextFocusedChanged += new System.EventHandler(this.monitor_TextFocusedChanged);
			this.monitor_Rx.FindChanged += new System.EventHandler(this.monitor_FindChanged);
			this.monitor_Rx.Enter += new System.EventHandler(this.monitor_Rx_Enter);
			// 
			// panel_Predefined
			// 
			this.panel_Predefined.Controls.Add(this.groupBox_Predefined);
			this.panel_Predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Predefined.Location = new System.Drawing.Point(0, 0);
			this.panel_Predefined.Name = "panel_Predefined";
			this.panel_Predefined.Padding = new System.Windows.Forms.Padding(1, 3, 3, 0);
			this.panel_Predefined.Size = new System.Drawing.Size(229, 440);
			this.panel_Predefined.TabIndex = 0;
			// 
			// groupBox_Predefined
			// 
			this.groupBox_Predefined.ContextMenuStrip = this.contextMenuStrip_Predefined;
			this.groupBox_Predefined.Controls.Add(this.predefined);
			this.groupBox_Predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox_Predefined.Location = new System.Drawing.Point(1, 3);
			this.groupBox_Predefined.Name = "groupBox_Predefined";
			this.groupBox_Predefined.Size = new System.Drawing.Size(225, 437);
			this.groupBox_Predefined.TabIndex = 0;
			this.groupBox_Predefined.TabStop = false;
			this.groupBox_Predefined.Text = "&Predefined Commands";
			// 
			// predefined
			// 
			this.predefined.Dock = System.Windows.Forms.DockStyle.Fill;
			this.predefined.Location = new System.Drawing.Point(3, 16);
			this.predefined.Name = "predefined";
			this.predefined.Size = new System.Drawing.Size(219, 418);
			this.predefined.TabIndex = 0;
			this.predefined.SelectedPageIdChanged += new System.EventHandler(this.predefined_SelectedPageChanged);
			this.predefined.SendCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.predefined_SendCommandRequest);
			this.predefined.DefineCommandRequest += new System.EventHandler<YAT.Model.Types.PredefinedCommandEventArgs>(this.predefined_DefineCommandRequest);
			// 
			// panel_Send
			// 
			this.panel_Send.Controls.Add(this.send);
			this.panel_Send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_Send.Location = new System.Drawing.Point(0, 0);
			this.panel_Send.Name = "panel_Send";
			this.panel_Send.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
			this.panel_Send.Size = new System.Drawing.Size(884, 96);
			this.panel_Send.TabIndex = 0;
			// 
			// send
			// 
			this.send.ContextMenuStrip = this.contextMenuStrip_Send;
			this.send.Dock = System.Windows.Forms.DockStyle.Fill;
			this.send.Location = new System.Drawing.Point(3, 0);
			this.send.Name = "send";
			this.send.SendSplitterDistance = 654;
			this.send.Size = new System.Drawing.Size(878, 93);
			this.send.TabIndex = 0;
			this.send.TextCommandChanged += new System.EventHandler(this.send_TextCommandChanged);
			this.send.TextFocusedChanged += new System.EventHandler(this.send_TextFocusChanged);
			this.send.SendTextCommandRequest += new System.EventHandler<YAT.View.Controls.SendTextOptionEventArgs>(this.send_SendTextCommandRequest);
			this.send.FileCommandChanged += new System.EventHandler(this.send_FileCommandChanged);
			this.send.SendFileCommandRequest += new System.EventHandler(this.send_SendFileCommandRequest);
			this.send.SizeChanged += new System.EventHandler(this.send_SizeChanged);
			// 
			// timer_RtsLuminescence
			// 
			this.timer_RtsLuminescence.Tick += new System.EventHandler(this.timer_RtsLuminescence_Tick);
			// 
			// timer_IOStatusIndicator
			// 
			this.timer_IOStatusIndicator.Tick += new System.EventHandler(this.timer_IOStatusIndicator_Tick);
			// 
			// timer_AutoActionCountUpdate
			// 
			this.timer_AutoActionCountUpdate.Interval = 73;
			this.timer_AutoActionCountUpdate.Tick += new System.EventHandler(this.timer_AutoActionCountUpdate_Tick);
			// 
			// timer_AutoResponseCountUpdate
			// 
			this.timer_AutoResponseCountUpdate.Interval = 73;
			this.timer_AutoResponseCountUpdate.Tick += new System.EventHandler(this.timer_AutoResponseCountUpdate_Tick);
			// 
			// toolStripMenuItem_PredefinedContextMenu_HideShowUndefined
			// 
			this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined.Name = "toolStripMenuItem_PredefinedContextMenu_HideShowUndefined";
			this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined.Size = new System.Drawing.Size(257, 22);
			this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined.Text = "Hide Undefined Commands";
			this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined.Click += new System.EventHandler(this.toolStripMenuItem_PredefinedContextMenu_HideShowUndefined_Click);
			// 
			// Terminal
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 562);
			this.Controls.Add(this.splitContainer_Terminal);
			this.Controls.Add(this.menuStrip_Terminal);
			this.Controls.Add(this.statusStrip_Terminal);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.Name = "Terminal";
			this.Text = "Terminal";
			this.Activated += new System.EventHandler(this.Terminal_Activated);
			this.Deactivate += new System.EventHandler(this.Terminal_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Terminal_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Terminal_FormClosed);
			this.Shown += new System.EventHandler(this.Terminal_Shown);
			this.LocationChanged += new System.EventHandler(this.Terminal_LocationChanged);
			this.SizeChanged += new System.EventHandler(this.Terminal_SizeChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Terminal_KeyDown);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Terminal_KeyUp);
			this.contextMenuStrip_Monitor.ResumeLayout(false);
			this.contextMenuStrip_Radix.ResumeLayout(false);
			this.contextMenuStrip_Predefined.ResumeLayout(false);
			this.contextMenuStrip_Command.ResumeLayout(false);
			this.contextMenuStrip_Page.ResumeLayout(false);
			this.contextMenuStrip_Send.ResumeLayout(false);
			this.menuStrip_Terminal.ResumeLayout(false);
			this.menuStrip_Terminal.PerformLayout();
			this.contextMenuStrip_Preset.ResumeLayout(false);
			this.statusStrip_Terminal.ResumeLayout(false);
			this.statusStrip_Terminal.PerformLayout();
			this.contextMenuStrip_Status.ResumeLayout(false);
			this.splitContainer_Terminal.Panel1.ResumeLayout(false);
			this.splitContainer_Terminal.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Terminal)).EndInit();
			this.splitContainer_Terminal.ResumeLayout(false);
			this.splitContainer_Predefined.Panel1.ResumeLayout(false);
			this.splitContainer_Predefined.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_Predefined)).EndInit();
			this.splitContainer_Predefined.ResumeLayout(false);
			this.panel_Monitor.ResumeLayout(false);
			this.groupBox_Monitor.ResumeLayout(false);
			this.splitContainer_TxMonitor.Panel1.ResumeLayout(false);
			this.splitContainer_TxMonitor.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_TxMonitor)).EndInit();
			this.splitContainer_TxMonitor.ResumeLayout(false);
			this.panel_Monitor_Tx.ResumeLayout(false);
			this.splitContainer_RxMonitor.Panel1.ResumeLayout(false);
			this.splitContainer_RxMonitor.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer_RxMonitor)).EndInit();
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

		private System.Windows.Forms.Timer timer_StatusText;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Monitor;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Radix;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Clear;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels_Tx;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels_Bidir;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Panels_Rx;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_SaveToFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_CopyToClipboard;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Print;
		private MKY.Windows.Forms.MenuStripEx menuStrip_Terminal;
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
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_Text;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_File;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_PredefinedCommands;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_On;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_Off;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_Clear;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_Settings;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View;
		private MKY.Windows.Forms.StatusStripEx statusStrip_Terminal;
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
		private YAT.View.Controls.PredefinedCommands predefined;
		private System.Windows.Forms.Panel panel_Monitor_Tx;
		private YAT.View.Controls.Monitor monitor_Tx;
		private System.Windows.Forms.SplitContainer splitContainer_RxMonitor;
		private System.Windows.Forms.Panel panel_Monitor_Bidir;
		private YAT.View.Controls.Monitor monitor_Bidir;
		private System.Windows.Forms.Panel panel_Monitor_Rx;
		private YAT.View.Controls.Monitor monitor_Rx;
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
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Unicode;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ResetCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowCountAndRate;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Predefined;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Page;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Define;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyFromSendText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Panels;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Send;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels_SendText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_Panels_SendFile;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_TerminalStatus_IOStatusIndicator;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Tx;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Bidir;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Rx;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_Predefined;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_SendText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_SendFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Panels_RearrangeAll;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Radix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_Format;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowTimeStamp;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowTimeSpan;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowTimeDelta;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowLength;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_3;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Clear;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_4;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Presets;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowEol;
		private System.Windows.Forms.Panel panel_Send;
		private YAT.View.Controls.Send send;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Preset;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PresetContextMenu_Preset_8;
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
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Tx_Unicode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_RxRadix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_String;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Char;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Rx_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Bin;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Oct;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Dec;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Hex;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_RadixContextMenu_Rx_Unicode;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_KeepSendText;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_KeepSendText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_CopyPredefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_CopyPredefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ResetConnectTime;
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
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_FlowControlCount;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_BreakCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Status;
		private System.Windows.Forms.ToolStripMenuItem contextMenuStrip_Status_FlowControlCount;
		private System.Windows.Forms.ToolStripMenuItem contextMenuStrip_Status_FlowControlCount_ShowCount;
		private System.Windows.Forms.ToolStripMenuItem contextMenuStrip_Status_FlowControlCount_ResetCount;
		private System.Windows.Forms.ToolStripMenuItem contextMenuStrip_Status_BreakCount;
		private System.Windows.Forms.ToolStripMenuItem contextMenuStrip_Status_BreakCount_ShowCount;
		private System.Windows.Forms.ToolStripMenuItem contextMenuStrip_Status_BreakCount_ResetCount;
		private System.Windows.Forms.Timer timer_IOStatusIndicator;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Break;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Refresh;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Log_Separator_1;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Log_Separator_2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Log_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_OpenFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowDirection;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Log_OpenDirectory;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Refresh;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_TerminalMenu_Send_AutoResponse_Response;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowDevice;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ToggleFormatting;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_TextWithoutEol;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SendTextWithoutEol;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_ExpandMultiLineText;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_5;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_RadixContextMenu_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_SkipEmptyLines;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_5;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_Find;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_FindNext;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Terminal_FindPrevious;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Terminal_Separator_6;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_Find;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_FindNext;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_FindPrevious;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowRadix;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowTimeStamp;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowTimeSpan;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowTimeDelta;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowDevice;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowDirection;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowEol;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowLength;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger;
		private MKY.Windows.Forms.ToolStripComboBoxEx toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_TerminalMenu_Receive_AutoAction_Action;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_EnableEscapesForText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_EnableEscapesForFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_6;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_Send_Separator_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowDuration;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowDuration;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_View_ShowLineNumbers;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Separator_8;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_MonitorContextMenu_Separator_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_MonitorContextMenu_ShowLineNumbers;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Panels_Predefined;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_12;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_13;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_14;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_15;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_16;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_17;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_18;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_19;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_20;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_21;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_22;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_23;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_24;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_25;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_37;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_49;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_61;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_73;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_85;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_MoveTo_97;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_12;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_13;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_14;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_15;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_16;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_17;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_18;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_19;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_20;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_21;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_22;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_23;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_24;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_25;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_37;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_49;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_61;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_73;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_85;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTo_97;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Clear;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_LinkPageToFile;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_12;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_13;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_14;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_15;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_16;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_17;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_18;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_19;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_20;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_21;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_22;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_23;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_24;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_36;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_48;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_60;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_72;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_84;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_UpBy_96;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_12;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_13;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_14;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_15;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_16;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_17;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_18;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_19;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_20;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_21;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_22;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_23;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_24;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_36;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_48;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_60;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_72;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_84;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_DownBy_96;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Cut;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Copy;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Paste;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Command;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PredefinedContextMenu_Separator_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_Layout;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Command;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_11;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_CommandContextMenu_12;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_PredefinedContextMenu_Layout;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_Page;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_Next;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_Previous;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_PageContextMenu_Separator;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PageContextMenu_9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine;
		private System.Windows.Forms.ToolStripComboBox toolStripComboBox_TerminalMenu_View_Panels_PageLayout;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_TerminalMenu_View_Panels_Separator_5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_AllowConcurrency;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_TerminalMenu_Send_AllowConcurrency;
		private System.Windows.Forms.Timer timer_AutoActionCountUpdate;
		private System.Windows.Forms.Timer timer_AutoResponseCountUpdate;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_CopyTextToClipboard;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem_SendContextMenu_Separator_7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_PredefinedContextMenu_HideShowUndefined;
	}
}
