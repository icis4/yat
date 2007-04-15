using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using HSR.Utilities.Settings;
using HSR.YAT.Settings.Application;
using HSR.YAT.Settings.Document;
using HSR.YAT.Gui.Settings;

namespace HSR.YAT.Gui.Forms
{
	public partial class Terminal : Form
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		// startup/update
		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		// MDI
		private string _id;
		private Form _mdiParent;

		// settings
		private DocumentSettingsHandler<DocumentSettings> _settingsHandler;
		private DocumentSettings _settings;
		private bool _handlingTerminalSettingsIsSuspended = false;

		// terminal
		private Domain.Terminal _terminal;

		// predefined
		private List<ToolStripMenuItem> _menuItems_predefined;

		// logs
		private Log.Logs _log;

		// status
		private const string _DefaultStatusText = "";
		private const int _TimedStatusInterval = 2000;
		private const int _RtsLuminescenceInterval = 150;
		private List<ToolStripStatusLabel> _statusLabels_ioControl;

		//------------------------------------------------------------------------------------------
		// Events
		//------------------------------------------------------------------------------------------

		public event EventHandler TerminalChanged;
		public event EventHandler<TerminalSavedEventArgs> TerminalSaved;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		// Important note to ensure proper z-order of this form:
		// - With visual designer, proceed with the following order
		//     1. Place "menuStrip_Terminal" and dock it to top
		//     2. Place "statusStrip_Terminal" and dock it to bottom
		//     3. Place "splitContainer_Terminal" and dock it to fill

		public Terminal()
		{
			InitializeComponent();
			Initialize(new DocumentSettingsHandler<DocumentSettings>());
		}

		public Terminal(DocumentSettingsHandler<DocumentSettings> settingsHandler)
		{
			InitializeComponent();
			Initialize(settingsHandler);
		}

		private void Initialize(DocumentSettingsHandler<DocumentSettings> settingsHandler)
		{
			FixContextMenus();

			InitializePredefinedMenuItems();
			InitializeMonitorMenuItems();
			InitializeIOControlStatusLabels();

			_settingsHandler = settingsHandler;
			_settings = _settingsHandler.Settings;
			AttachSettingsHandlers();

			ApplyWindowSettings();
			LayoutTerminal();

			_terminal = Domain.Factory.TerminalFactory.CreateTerminal(_settings.Terminal);
			AttachTerminalHandlers();

			_log = new Log.Logs(_settings.Log);

			// force settings changed event to set all controls
			// for improved performance, manually suspend/resume handler for terminal settings
			SuspendHandlingTerminalSettings();
			_settings.ClearChanged();
			_settings.ForceChangeEvent();
			ResumeHandlingTerminalSettings();
		}

		#region Properties
		//******************************************************************************************
		// Properties
		//******************************************************************************************

		public string Id
		{
			get { return (_id); }
			set { _id = value; }
		}

		public string IdFromFile
		{
			set { _id = Path.GetFileName(value); }
		}

		public bool IsOpen
		{
			get { return (_terminal.IsOpen); }
		}

		#endregion

		#region Methods
		//******************************************************************************************
		// Methods
		//******************************************************************************************

		public void RequestSaveFile()
		{
			SaveFile();
		}

		public void RequestCloseFile()
		{
			CloseFile();
		}

		public void RequestOpenTerminal()
		{
			OpenTerminal();
		}

		public void RequestCloseTerminal()
		{
			CloseTerminal();
		}

		public void RequestEditTerminalSettings()
		{
			ShowTerminalSettings();
		}

		#endregion

		#region Form Event Handlers
		//******************************************************************************************
		// Form Event Handlers
		//******************************************************************************************

		private void Terminal_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				_mdiParent = MdiParent;

				// immediately set terminal controls so the terminal "looks" nice from the very start
				SetTerminalControls();

				// then begin logging (in case opening of terminal needs to be logged)
				if (_settings.LogIsOpen)
					BeginLog();

				// then open terminal
				if (_settings.TerminalIsOpen)
					OpenTerminal();
			}
		}

		private void Terminal_LocationChanged(object sender, EventArgs e)
		{
			if (!_isStartingUp)
				SaveWindowSettings();
		}

		private void Terminal_SizeChanged(object sender, EventArgs e)
		{
			if (!_isStartingUp)
				SaveWindowSettings();
		}

		private void Terminal_Closing(object sender, CancelEventArgs e)
		{
			if (_settings.ExplicitHaveChanged)
			{
				DialogResult dr = MessageBox.Show
					(
					this,
					"Save settings?",
					Text,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					);

				switch (dr)
				{
					case DialogResult.Yes: SaveFile(); break;
					case DialogResult.No: break;
					default: e.Cancel = true; return;
				}
			}

			if (_terminal.IsOpen)
				CloseTerminal();

			if (_log.IsOpen)
				EndLog();
		}

		#endregion

		#region Controls Event Handlers
		//******************************************************************************************
		// Controls Event Handlers
		//******************************************************************************************

		#region Controls Event Handlers > Terminal Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu
		//------------------------------------------------------------------------------------------

		#region Controls Event Handlers > Terminal Menu > File
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > File
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_File_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_File_Save.Enabled = _settingsHandler.SettingsFileExists;
		}

		private void toolStripMenuItem_TerminalMenu_File_Save_Click(object sender, EventArgs e)
		{
			SaveFile();
		}

		private void toolStripMenuItem_TerminalMenu_File_SaveAs_Click(object sender, EventArgs e)
		{
			ShowSaveFileAsDialog();
		}

		private void toolStripMenuItem_TerminalMenu_File_Close_Click(object sender, EventArgs e)
		{
			CloseFile();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Terminal
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Terminal
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_Terminal_DropDownOpening(object sender, EventArgs e)
		{
			bool terminalIsOpen = _terminal.IsOpen;
			toolStripMenuItem_TerminalMenu_Terminal_Open.Enabled = !terminalIsOpen;
			toolStripMenuItem_TerminalMenu_Terminal_Close.Enabled = terminalIsOpen;
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Open_Click(object sender, EventArgs e)
		{
			OpenTerminal();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Close_Click(object sender, EventArgs e)
		{
			CloseTerminal();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Settings_Click(object sender, EventArgs e)
		{
			ShowTerminalSettings();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Send
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Send
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_Send_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Send_Command.Enabled = _settings.SendCommand.Command.IsValidCommand;
			toolStripMenuItem_TerminalMenu_Send_File.Enabled = _settings.SendCommand.Command.IsValidFilePath;
		}

		private void toolStripMenuItem_TerminalMenu_Send_Command_Click(object sender, EventArgs e)
		{
			SendCommand();
		}

		private void toolStripMenuItem_TerminalMenu_Send_File_Click(object sender, EventArgs e)
		{
			SendFile();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Monitor
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Monitor
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_Monitor_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Monitor_ShowTimestamp.Checked = _settings.Display.ShowTimestamp;
			toolStripMenuItem_TerminalMenu_Monitor_ShowLength.Checked = _settings.Display.ShowLength;
			toolStripComboBox_TerminalMenu_Monitor_Orientation.SelectedItem = _settings.Layout.MonitorOrientation;

			toolStripMenuItem_TerminalMenu_Monitor_ShowCounters.Checked = _settings.Display.ShowCounters;
			toolStripMenuItem_TerminalMenu_Monitor_ResetCounters.Enabled = _settings.Display.ShowCounters;
		}

		private void toolStripMenuItem_TerminalMenu_Monitor_ShowTimestamp_Click(object sender, EventArgs e)
		{
			_settings.Display.ShowTimestamp = !_settings.Display.ShowTimestamp;
		}

		private void toolStripMenuItem_TerminalMenu_Monitor_ShowLength_Click(object sender, EventArgs e)
		{
			_settings.Display.ShowLength = !_settings.Display.ShowLength;
		}

		private void toolStripComboBox_TerminalMenu_Monitor_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				SetMonitorOrientation((Orientation)toolStripComboBox_TerminalMenu_Monitor_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_TerminalMenu_Monitor_ClearAll_Click(object sender, EventArgs e)
		{
			ClearAllMonitors();
		}

		private void toolStripMenuItem_TerminalMenu_Monitor_ShowCounters_Click(object sender, EventArgs e)
		{
			_settings.Display.ShowCounters = !_settings.Display.ShowCounters;
		}

		private void toolStripMenuItem_TerminalMenu_Monitor_ResetCounters_Click(object sender, EventArgs e)
		{
			ClearCountStatus();
		}

		private void toolStripMenuItem_TerminalMenu_Monitor_Format_Click(object sender, EventArgs e)
		{
			ShowFormatSettings();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Log
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Log
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_Log_DropDownOpening(object sender, EventArgs e)
		{
			bool logIsOpen = _log.IsOpen;
			toolStripMenuItem_TerminalMenu_Log_Begin.Enabled = !logIsOpen;
			toolStripMenuItem_TerminalMenu_Log_End.Enabled = logIsOpen;
		}

		private void toolStripMenuItem_TerminalMenu_Log_Begin_Click(object sender, EventArgs e)
		{
			BeginLog();
		}

		private void toolStripMenuItem_TerminalMenu_Log_End_Click(object sender, EventArgs e)
		{
			EndLog();
		}

		private void toolStripMenuItem_TerminalMenu_Log_Clear_Click(object sender, EventArgs e)
		{
			ClearLog();
		}

		private void toolStripMenuItem_TerminalMenu_Log_Settings_Click(object sender, EventArgs e)
		{
			ShowLogSettings();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > View
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > View
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_View_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_View_SendCommand.Checked = _settings.Layout.SendCommandPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_SendFile.Checked = _settings.Layout.SendFilePanelIsVisible;

			toolStripMenuItem_TerminalMenu_View_Tx.Checked = _settings.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Bidir.Checked = _settings.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Rx.Checked = _settings.Layout.RxMonitorPanelIsVisible;

			// disable monitor item if the other monitors are hidden
			toolStripMenuItem_TerminalMenu_View_Tx.Enabled = (_settings.Layout.BidirMonitorPanelIsVisible || _settings.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Bidir.Enabled = (_settings.Layout.TxMonitorPanelIsVisible || _settings.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Rx.Enabled = (_settings.Layout.TxMonitorPanelIsVisible || _settings.Layout.BidirMonitorPanelIsVisible);

			toolStripMenuItem_TerminalMenu_View_Predefined.Checked = _settings.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_SendCommand_Click(object sender, EventArgs e)
		{
			_settings.Layout.SendCommandPanelIsVisible = !_settings.Layout.SendCommandPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_SendFile_Click(object sender, EventArgs e)
		{
			_settings.Layout.SendFilePanelIsVisible = !_settings.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Tx_Click(object sender, EventArgs e)
		{
			_settings.Layout.TxMonitorPanelIsVisible = !_settings.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Bidir_Click(object sender, EventArgs e)
		{
			_settings.Layout.BidirMonitorPanelIsVisible = !_settings.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Rx_Click(object sender, EventArgs e)
		{
			_settings.Layout.RxMonitorPanelIsVisible = !_settings.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Predefined_Click(object sender, EventArgs e)
		{
			_settings.Layout.PredefinedPanelIsVisible = !_settings.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Rearrange_Click(object sender, EventArgs e)
		{
			ViewRearrange();
		}

		#endregion

		#endregion

		#region Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Send_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_SendContextMenu_Command.Enabled = _settings.SendCommand.Command.IsValidCommand;
			toolStripMenuItem_SendContextMenu_File.Enabled = _settings.SendCommand.Command.IsValidFilePath;

			toolStripMenuItem_SendContextMenu_View_SendCommand.Checked = _settings.Layout.SendCommandPanelIsVisible;
			toolStripMenuItem_SendContextMenu_View_SendFile.Checked = _settings.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Command_Click(object sender, EventArgs e)
		{
			SendCommand();
		}

		private void toolStripMenuItem_SendContextMenu_File_Click(object sender, EventArgs e)
		{
			SendFile();
		}

		private void toolStripMenuItem_SendContextMenu_View_SendCommand_Click(object sender, EventArgs e)
		{
			_settings.Layout.SendCommandPanelIsVisible = !_settings.Layout.SendCommandPanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_View_SendFile_Click(object sender, EventArgs e)
		{
			_settings.Layout.SendFilePanelIsVisible = !_settings.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Hide_Click(object sender, EventArgs e)
		{
			Control source = contextMenuStrip_Send.SourceControl;

			if ((source == sendCommand) || (source == panel_SendCommand))
				_settings.Layout.SendCommandPanelIsVisible = false;
			if ((source == sendFile) || (source == panel_SendFile))
				_settings.Layout.SendFilePanelIsVisible = false;
		}

		#endregion

		#region Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Monitor_Opening(object sender, CancelEventArgs e)
		{
			Domain.RepositoryType monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			bool isMonitor = (monitorType != Domain.RepositoryType.None);

			toolStripMenuItem_MonitorContextMenu_ShowTimestamp.Checked = _settings.Display.ShowTimestamp;
			toolStripMenuItem_MonitorContextMenu_ShowLength.Checked = _settings.Display.ShowLength;

			toolStripMenuItem_MonitorContextMenu_Clear.Enabled = isMonitor;

			toolStripMenuItem_MonitorContextMenu_ShowCounters.Checked = _settings.Display.ShowCounters;
			toolStripMenuItem_MonitorContextMenu_ResetCounters.Enabled = _settings.Display.ShowCounters;

			toolStripMenuItem_MonitorContextMenu_View_Tx.Checked = _settings.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_View_Bidir.Checked = _settings.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_View_Rx.Checked = _settings.Layout.RxMonitorPanelIsVisible;

			// disable "Monitor" item if the other monitors are hidden
			toolStripMenuItem_MonitorContextMenu_View_Tx.Enabled    = (_settings.Layout.BidirMonitorPanelIsVisible || _settings.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_View_Bidir.Enabled = (_settings.Layout.TxMonitorPanelIsVisible || _settings.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_View_Rx.Enabled    = (_settings.Layout.TxMonitorPanelIsVisible || _settings.Layout.BidirMonitorPanelIsVisible);

			// hide "Hide" item if only this monitor is visible
			bool hideIsAllowed = false;
			switch (monitorType)
			{
				case Domain.RepositoryType.Tx:    hideIsAllowed = (_settings.Layout.BidirMonitorPanelIsVisible || _settings.Layout.RxMonitorPanelIsVisible); break;
				case Domain.RepositoryType.Bidir: hideIsAllowed = (_settings.Layout.TxMonitorPanelIsVisible || _settings.Layout.RxMonitorPanelIsVisible); break;
				case Domain.RepositoryType.Rx:    hideIsAllowed = (_settings.Layout.TxMonitorPanelIsVisible || _settings.Layout.BidirMonitorPanelIsVisible); break;
			}
			toolStripMenuItem_MonitorContextMenu_Hide.Visible = hideIsAllowed;
			toolStripMenuItem_MonitorContextMenu_Hide.Enabled = isMonitor && hideIsAllowed;

			toolStripMenuItem_MonitorContextMenu_SaveToFile.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_Print.Enabled = isMonitor;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTimestamp_Click(object sender, EventArgs e)
		{
			_settings.Display.ShowTimestamp = !_settings.Display.ShowTimestamp;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLength_Click(object sender, EventArgs e)
		{
			_settings.Display.ShowLength = !_settings.Display.ShowLength;
		}

		private void toolStripComboBox_MonitorContextMenu_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				SetMonitorOrientation((Orientation)toolStripComboBox_MonitorContextMenu_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_MonitorContextMenu_Clear_Click(object sender, EventArgs e)
		{
			ClearMonitor(GetMonitorType(contextMenuStrip_Monitor.SourceControl));
		}

		private void toolStripMenuItem_MonitorContextMenu_ClearAll_Click(object sender, EventArgs e)
		{
			ClearAllMonitors();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowCounters_Click(object sender, EventArgs e)
		{
			_settings.Display.ShowCounters = !_settings.Display.ShowCounters;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetCounters_Click(object sender, EventArgs e)
		{
			ClearCountStatus();
		}

		private void toolStripMenuItem_MonitorContextMenu_View_Tx_Click(object sender, EventArgs e)
		{
			_settings.Layout.TxMonitorPanelIsVisible = !_settings.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_View_Bidir_Click(object sender, EventArgs e)
		{
			_settings.Layout.BidirMonitorPanelIsVisible = !_settings.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_View_Rx_Click(object sender, EventArgs e)
		{
			_settings.Layout.RxMonitorPanelIsVisible = !_settings.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Hide_Click(object sender, EventArgs e)
		{
			switch (GetMonitorType(contextMenuStrip_Monitor.SourceControl))
			{
				case Domain.RepositoryType.Tx:    _settings.Layout.TxMonitorPanelIsVisible    = false; break;
				case Domain.RepositoryType.Bidir: _settings.Layout.BidirMonitorPanelIsVisible = false; break;
				case Domain.RepositoryType.Rx:    _settings.Layout.RxMonitorPanelIsVisible    = false; break;
			}
		}

		private void toolStripMenuItem_MonitorContextMenu_SaveToFile_Click(object sender, EventArgs e)
		{
			ShowSaveMonitorDialog(GetMonitor(contextMenuStrip_Monitor.SourceControl));
		}

		private void toolStripMenuItem_MonitorContextMenu_CopyToClipboard_Click(object sender, EventArgs e)
		{
			CopyMonitorToClipboard(GetMonitor(contextMenuStrip_Monitor.SourceControl));
		}

		private void toolStripMenuItem_MonitorContextMenu_Print_Click(object sender, EventArgs e)
		{
			ShowPrintMonitorDialog(GetMonitor(contextMenuStrip_Monitor.SourceControl));
		}

		#endregion

		#region Controls Event Handlers > Predefined Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Predefined Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Predefined_Opening(object sender, CancelEventArgs e)
		{
			SetPredefinedMenuItems();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Command_Click(object sender, EventArgs e)
		{
			RequestPredefined(predefined.SelectedPage, int.Parse((string)(((ToolStripMenuItem)sender).Tag)));
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Next_Click(object sender, EventArgs e)
		{
			predefined.NextPage();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Previous_Click(object sender, EventArgs e)
		{
			predefined.PreviousPage();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Define_Click(object sender, EventArgs e)
		{
			ShowPredefinedCommandSettings(1, 1);
		}

		private void toolStripMenuItem_PredefinedContextMenu_Hide_Click(object sender, EventArgs e)
		{
			_settings.Layout.PredefinedPanelIsVisible = false;
		}

		#endregion

		#region Controls Event Handlers > Radix Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Radix Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Radix_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_RadixContextMenu_String.Checked = (_settings.Display.Radix == Domain.Radix.String);
			toolStripMenuItem_RadixContextMenu_Char.Checked = (_settings.Display.Radix == Domain.Radix.Char);
			toolStripMenuItem_RadixContextMenu_Bin.Checked = (_settings.Display.Radix == Domain.Radix.Bin);
			toolStripMenuItem_RadixContextMenu_Oct.Checked = (_settings.Display.Radix == Domain.Radix.Oct);
			toolStripMenuItem_RadixContextMenu_Dec.Checked = (_settings.Display.Radix == Domain.Radix.Dec);
			toolStripMenuItem_RadixContextMenu_Hex.Checked = (_settings.Display.Radix == Domain.Radix.Hex);
		}

		private void toolStripMenuItem_RadixContextMenu_String_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Char_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Bin_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Oct_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Dec_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Hex_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.Radix.Hex);
		}

		#endregion

		#region Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------

		private void splitContainer_Upper_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
				_settings.Layout.UpperSplitterRatio = (float)splitContainer_Upper.SplitterDistance / splitContainer_Upper.Width;
		}

		private void splitContainer_MonitorLeft_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
			{
				int widthOrHeight = 0;
				if (_settings.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_MonitorLeft.Width;
				else
					widthOrHeight = splitContainer_MonitorLeft.Height;

				_settings.Layout.MonitorLeftSplitterRatio = (float)splitContainer_MonitorLeft.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_MonitorRight_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
			{
				int widthOrHeight = 0;
				if (_settings.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_MonitorRight.Width;
				else
					widthOrHeight = splitContainer_MonitorRight.Height;

				_settings.Layout.MonitorRightSplitterRatio = (float)splitContainer_MonitorRight.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_Lower_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
				_settings.Layout.LowerSplitterRatio = (float)splitContainer_Lower.SplitterDistance / splitContainer_Lower.Width;
		}

		#endregion

		#region Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------

		private void sendCommand_CommandChanged(object sender, EventArgs e)
		{
			_settings.Implicit.SendCommand.Command = sendCommand.Command;
		}

		private void sendCommand_SendCommandRequest(object sender, EventArgs e)
		{
			SendCommand();
		}

		private void sendFile_CommandChanged(object sender, EventArgs e)
		{
			_settings.Implicit.SendFile.Command = sendFile.Command;
		}

		private void sendFile_SendCommandRequest(object sender, EventArgs e)
		{
			SendFile();
		}

		#endregion

		#region Controls Event Handlers > Monitor
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor
		//------------------------------------------------------------------------------------------

		private void monitor_Tx_CopyRequest(object sender, System.EventArgs e)
		{
			CopyMonitorToClipboard(monitor_Tx);
		}

		private void monitor_Tx_PrintRequest(object sender, System.EventArgs e)
		{
			PrintMonitor(monitor_Tx);
		}

		private void monitor_Bidir_CopyRequest(object sender, System.EventArgs e)
		{
			CopyMonitorToClipboard(monitor_Bidir);
		}

		private void monitor_Bidir_PrintRequest(object sender, System.EventArgs e)
		{
			PrintMonitor(monitor_Bidir);
		}

		private void monitor_Rx_CopyRequest(object sender, System.EventArgs e)
		{
			CopyMonitorToClipboard(monitor_Rx);
		}

		private void monitor_Rx_PrintRequest(object sender, System.EventArgs e)
		{
			PrintMonitor(monitor_Rx);
		}

		#endregion

		#region Controls Event Handlers > Predefined
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Predefined
		//------------------------------------------------------------------------------------------

		private void predefined_SelectedPageChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				_settings.Implicit.Predefined.SelectedPage = predefined.SelectedPage;
		}

		private void predefined_SendCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			SendPredefined(e.Page, e.Command);
		}

		private void predefined_DefineCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			ShowPredefinedCommandSettings(e.Page, e.Command);
		}

		#endregion

		#region Controls Event Handlers > Status
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status
		//------------------------------------------------------------------------------------------

		private void toolStripStatusLabel_TerminalStatus_IOStatus_Click(object sender, EventArgs e)
		{
			ShowTerminalSettings();
		}

		private void toolStripStatusLabel_TerminalStatus_RTS_Click(object sender, EventArgs e)
		{
			RequestToggleRts();
		}

		private void toolStripStatusLabel_TerminalStatus_DTR_Click(object sender, EventArgs e)
		{
			RequestToggleDtr();
		}

		#endregion

		#endregion

		#region File
		//******************************************************************************************
		// File
		//******************************************************************************************

		private void SaveFile()
		{
			if (_settingsHandler.SettingsFileExists)
				DoSave();
			else
				ShowSaveFileAsDialog();
		}

		private void ShowSaveFileAsDialog()
		{
			SetFixedStatusText("Saving terminal as...");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save " + Id + " As";
			sfd.Filter = ApplicationSettings.Extensions.TerminalFilesFilter;
			sfd.DefaultExt = ApplicationSettings.Extensions.TerminalFilesDefault;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Path.SettingsFilesPath;
			sfd.FileName = Id + "." + sfd.DefaultExt;
			if ((sfd.ShowDialog(this) == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Path.SettingsFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				_settingsHandler.SettingsFilePath = sfd.FileName;
				DoSave();
			}
			else
			{
				ResetStatusText();
			}
		}

		private void DoSave()
		{
			SetFixedStatusText("Saving terminal...");
			try
			{
				_settingsHandler.Save();
				IdFromFile = _settingsHandler.SettingsFilePath;
				OnTerminalSaved(new TerminalSavedEventArgs(_settingsHandler.SettingsFilePath));
				SetTimedStatusText("Terminal saved");
			}
			catch (System.Xml.XmlException ex)
			{
				SetFixedStatusText("Error saving terminal!");
				MessageBox.Show
					(
					this,
					"Unable to save file" + Environment.NewLine + _settingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
					"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
					"File error message: " + ex.InnerException.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
				SetTimedStatusText("Terminal not saved!");
			}
			SetTerminalCaption();
		}

		private void CloseFile()
		{
			Close();
		}

		#endregion

		#region Settings
		//******************************************************************************************
		// Settings
		//******************************************************************************************

		private void AttachSettingsHandlers()
		{
			_settings.ClearChanged();
			_settings.Changed += new EventHandler<SettingsEventArgs>(_settings_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void _settings_Changed(object sender, SettingsEventArgs e)
		{
			SetTerminalCaption();
			if (e.Inner == null)
			{
				// DocumentSettings changed
				// nothing to do, no need to care about ProductVersion
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Explicit))
			{
				// ExplicitSettings changed
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Implicit))
			{
				// ImplicitSettings changed
				HandleImplicitSettings(e.Inner);
			}
		}

		private void HandleExplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// ExplicitSettings changed
				// nothing to do
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Terminal))
			{
				// TerminalSettings changed
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.PredefinedCommand))
			{
				// PredefinedCommandSettings changed
				_isSettingControls = true;
				predefined.Pages = _settings.PredefinedCommand.Pages;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Format))
			{
				// FormatSettings changed
				ReformatMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Log))
			{
				// LogSettings changed
				SetLogControls();
			}
		}

		private void HandleImplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// ImplicitSettings changed
				SetTerminalControls();
				SetLogControls();
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.SendCommand))
			{
				// SendCommandSettings changed
				_isSettingControls = true;
				sendCommand.Command = _settings.SendCommand.Command;
				sendCommand.RecentCommands = _settings.SendCommand.RecentCommands;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.SendFile))
			{
				// SendFileSettings changed
				_isSettingControls = true;
				sendFile.Command = _settings.SendFile.Command;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Predefined))
			{
				// PredefinedSettings changed
				_isSettingControls = true;
				predefined.SelectedPage = _settings.Predefined.SelectedPage;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Window))
			{
				// WindowSettings changed
				// nothing to do, windows settings are only saved
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Layout))
			{
				// LayoutSettings changed
				LayoutTerminal();
			}
		}

		private void HandleTerminalSettings(SettingsEventArgs e)
		{
			if (_handlingTerminalSettingsIsSuspended)
				return;

			if (e.Inner == null)
			{
				// TerminalSettings changed
				SetIOStatus();
				SetIOControlControls();
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.IO))
			{
				// IOSettings changed
				SetIOStatus();
				SetIOControlControls();
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Buffer))
			{
				// BufferSettings changed
				ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Display))
			{
				// DisplaySettings changed
				ReloadMonitors();

				monitor_Tx.ShowCountStatus    = _settings.Display.ShowCounters;
				monitor_Bidir.ShowCountStatus = _settings.Display.ShowCounters;
				monitor_Rx.ShowCountStatus    = _settings.Display.ShowCounters;
			}
			else if (ReferenceEquals(e.Inner.Source, _settings.Transmit))
			{
				// TransmitSettings changed
				ReloadMonitors();
			}
		}

		private void SuspendHandlingTerminalSettings()
		{
			_handlingTerminalSettingsIsSuspended = true;
		}

		private void ResumeHandlingTerminalSettings()
		{
			_handlingTerminalSettingsIsSuspended = false;

			SetIOStatus();
			SetIOControlControls();

			ReloadMonitors();

			monitor_Tx.ShowCountStatus = _settings.Display.ShowCounters;
			monitor_Bidir.ShowCountStatus = _settings.Display.ShowCounters;
			monitor_Rx.ShowCountStatus = _settings.Display.ShowCounters;
		}

		#endregion

		#region View
		//******************************************************************************************
		// View
		//******************************************************************************************

		/// <summary>
		/// Makes sure that context menus are at the right position upon first drop down. This is
		/// a fix, it should be that way by default. However, due to some resaons, they somtimes
		/// appear somewhere at the top-left corner of the screen if this fix isn't done.
		/// </summary>
		/// <remarks>
		/// Is this a .NET bug?
		/// </remarks>
		private void FixContextMenus()
		{
			SuspendLayout();
			contextMenuStrip_Send.OwnerItem = null;
			contextMenuStrip_Radix.OwnerItem = null;
			contextMenuStrip_Monitor.OwnerItem = null;
			contextMenuStrip_Predefined.OwnerItem = null;
			ResumeLayout();
		}

		private void ApplyWindowSettings()
		{
			SuspendLayout();
			WindowState = _settings.Window.State;
			if (WindowState == FormWindowState.Normal)
			{
				Location = _settings.Window.Location;
				Size = _settings.Window.Size;
			}
			ResumeLayout();
		}

		private void SaveWindowSettings()
		{
			_settings.Window.State = WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				_settings.Window.Location = Location;
				_settings.Window.Size = Size;
			}
		}

		private void ViewRearrange()
		{
			// simply set defaults, settings event handler will then call LayoutTerminal()
			_settings.Layout.SetDefaults();
		}

		private void LayoutTerminal()
		{
			_isSettingControls = true;
			SuspendLayout();

			// splitContainer_Terminal and splitContainer_Upper
			if (_settings.Layout.SendCommandPanelIsVisible || _settings.Layout.SendFilePanelIsVisible)
			{
				splitContainer_Terminal.Panel1Collapsed = false;
				splitContainer_Upper.Panel1Collapsed = !_settings.Layout.SendCommandPanelIsVisible;
				splitContainer_Upper.Panel2Collapsed = !_settings.Layout.SendFilePanelIsVisible;
				splitContainer_Upper.SplitterDistance = (int)(_settings.Layout.UpperSplitterRatio * splitContainer_Upper.Width);
			}
			else
			{
				splitContainer_Terminal.Panel1Collapsed = true;
			}

			// splitContainer_Lower
			if (_settings.Layout.PredefinedPanelIsVisible)
			{
				splitContainer_Lower.Panel2Collapsed = false;
				splitContainer_Lower.SplitterDistance = (int)(_settings.Layout.LowerSplitterRatio * splitContainer_Lower.Width);
			}
			else
			{
				splitContainer_Lower.Panel2Collapsed = true;
			}

			// splitContainer_MonitorLeft and splitContainer_MonitorRight
			// one of the panels MUST be visible, if none is visible, then bidir si shown anyway
			bool txIsVisible = _settings.Layout.TxMonitorPanelIsVisible;
			bool bidirIsVisible = _settings.Layout.BidirMonitorPanelIsVisible || (!_settings.Layout.TxMonitorPanelIsVisible && !_settings.Layout.RxMonitorPanelIsVisible);
			bool rxIsVisible = _settings.Layout.RxMonitorPanelIsVisible;

			// orientation
			Orientation orientation = _settings.Layout.MonitorOrientation;
			splitContainer_MonitorLeft.Orientation = orientation;
			splitContainer_MonitorRight.Orientation = orientation;

			if (txIsVisible)
			{
				splitContainer_MonitorLeft.Panel1Collapsed = false;

				int widthOrHeight = 0;
				if (orientation == Orientation.Vertical)
					widthOrHeight = splitContainer_MonitorLeft.Width;
				else
					widthOrHeight = splitContainer_MonitorLeft.Height;

				splitContainer_MonitorLeft.SplitterDistance = (int)(_settings.Layout.MonitorLeftSplitterRatio * widthOrHeight);
			}
			else
			{
				splitContainer_MonitorLeft.Panel1Collapsed = true;
			}
			splitContainer_MonitorLeft.Panel2Collapsed = !(bidirIsVisible || rxIsVisible);

			if (bidirIsVisible)
			{
				splitContainer_MonitorRight.Panel1Collapsed = false;

				int widthOrHeight = 0;
				if (orientation == Orientation.Vertical)
					widthOrHeight = splitContainer_MonitorRight.Width;
				else
					widthOrHeight = splitContainer_MonitorRight.Height;

				splitContainer_MonitorRight.SplitterDistance = (int)(_settings.Layout.MonitorRightSplitterRatio * widthOrHeight);
			}
			else
			{
				splitContainer_MonitorRight.Panel1Collapsed = true;
			}
			splitContainer_MonitorRight.Panel2Collapsed = !rxIsVisible;

			ResumeLayout();
			_isSettingControls = false;
		}

		#endregion

		#region Terminal
		//******************************************************************************************
		// Terminal
		//******************************************************************************************

		#region Terminal > Lifetime
		//------------------------------------------------------------------------------------------
		// Terminal > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachTerminalHandlers()
		{
			_terminal.TerminalChanged += new EventHandler(_terminal_TerminalChanged);
			_terminal.TerminalControlChanged += new EventHandler(_terminal_TerminalControlChanged);
			_terminal.TerminalError += new EventHandler<Domain.TerminalErrorEventArgs>(_terminal_TerminalError);

			_terminal.RawElementSent += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
			_terminal.RawElementReceived += new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
			_terminal.DisplayElementsSent += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
			_terminal.RepositoryCleared += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
		}

		private void DetachTerminalHandlers()
		{
			_terminal.TerminalChanged -= new EventHandler(_terminal_TerminalChanged);
			_terminal.TerminalControlChanged -= new EventHandler(_terminal_TerminalControlChanged);
			_terminal.TerminalError -= new EventHandler<Domain.TerminalErrorEventArgs>(_terminal_TerminalError);

			_terminal.RawElementSent -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementSent);
			_terminal.RawElementReceived -= new EventHandler<Domain.RawElementEventArgs>(_terminal_RawElementReceived);
			_terminal.DisplayElementsSent -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);
			_terminal.RepositoryCleared -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);
		}

		#endregion

		#region Terminal > View
		//------------------------------------------------------------------------------------------
		// Terminal > View
		//------------------------------------------------------------------------------------------

		private void InitializeIOControlStatusLabels()
		{
			_statusLabels_ioControl = new List<ToolStripStatusLabel>();
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_RTS);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_CTS);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_DTR);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_DSR);
			_statusLabels_ioControl.Add(toolStripStatusLabel_TerminalStatus_DCD);
		}

		private void SetTerminalCaption()
		{
			bool isOpen = false;
			bool isConnected = false;

			if (_terminal != null)
			{
				isOpen = _terminal.IsOpen;
				isConnected = _terminal.IsConnected;
			}

			StringBuilder sb = new StringBuilder(Id);

			if ((_settings != null) && _settings.ExplicitHaveChanged)
				sb.Append("*");

			if (_settings != null)
			{
				if (_settings.IOType == Domain.IOType.SerialPort)
				{
					Domain.Settings.SerialPort.SerialPortSettings s = _settings.IO.SerialPort;
					sb.Append(" - ");
					sb.Append(s.PortId.ToString());
					sb.Append(" - ");
					sb.Append(isOpen ? "Open" : "Closed");
				}
				else
				{
					Domain.Settings.Socket.SocketSettings s = _settings.IO.Socket;
					switch (_settings.IOType)
					{
						case Domain.IOType.TcpClient:
							sb.Append(" - ");
							sb.Append(s.ResolvedRemoteIPAddress.ToString());
							sb.Append(":");
							sb.Append(s.RemotePort.ToString());
							sb.Append(" - ");
							sb.Append(isConnected ? "Connected" : "Disconnected");
							break;

						case Domain.IOType.TcpServer:
							sb.Append(" - ");
							sb.Append("Server:");
							sb.Append(s.LocalPort.ToString());
							sb.Append(" - ");
							if (isOpen)
								sb.Append(isConnected ? "Connected" : "Listening");
							else
								sb.Append("Closed");
							break;

						case Domain.IOType.TcpAutoSocket:
							bool isClient = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsClient;
							bool isServer = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsServer;
							if (isOpen)
							{
								if (isClient)
								{
									sb.Append(" - ");
									sb.Append(s.ResolvedRemoteIPAddress.ToString());
									sb.Append(":");
									sb.Append(s.RemotePort.ToString());
									sb.Append(" - ");
									sb.Append(isConnected ? "Connected" : "Disconnected");
								}
								else if (isServer)
								{
									sb.Append(" - ");
									sb.Append("Server:");
									sb.Append(s.LocalPort.ToString());
									sb.Append(" - ");
									sb.Append(isConnected ? "Connected" : "Listening");
								}
								else
								{
									sb.Append(" - ");
									sb.Append("Starting on port ");
									sb.Append(s.RemotePort.ToString());
								}
							}
							else
							{
								sb.Append(" - ");
								sb.Append("AutoSocket:");
								sb.Append(s.RemotePort.ToString());
								sb.Append(" - ");
								sb.Append("Disconnected");
							}
							break;

						case Domain.IOType.Udp:
							sb.Append(" - ");
							sb.Append(s.ResolvedRemoteIPAddress.ToString());
							sb.Append(":");
							sb.Append(s.RemotePort.ToString());
							sb.Append(" - ");
							sb.Append("Receive:");
							sb.Append(s.LocalPort.ToString());
							sb.Append(" - ");
							sb.Append(isOpen ? "Open" : "Closed");
							break;
					}
				}
			}

			Text = sb.ToString();
		}

		private void SetTerminalControls()
		{
			bool isOpen = _terminal.IsOpen;

			// main menu
			toolStripMenuItem_TerminalMenu_Terminal_Open.Enabled = !isOpen;
			toolStripMenuItem_TerminalMenu_Terminal_Close.Enabled = isOpen;

			// terminal panel
			SetTerminalCaption();
			SetIOStatus();
			SetIOControlControls();

			// send panel
			sendCommand.TerminalIsOpen = isOpen;
			sendFile.TerminalIsOpen = isOpen;

			// predefined panel
			predefined.TerminalIsOpen = isOpen;
		}

		private void SetIOStatus()
		{
			bool isOpen = _terminal.IsOpen;
			bool isConnected = _terminal.IsConnected;

			StringBuilder sb = new StringBuilder();

			if (_settings.IOType == Domain.IOType.SerialPort)
			{
				Domain.Settings.SerialPort.SerialPortSettings s = _settings.IO.SerialPort;
				sb.Append("Serial port ");
				sb.Append(s.PortId.ToString());
				sb.Append(" (" + s.Communication.ToString() + ") is ");
				sb.Append(isOpen ? "open" : "closed");
			}
			else
			{
				Domain.Settings.Socket.SocketSettings s = _settings.IO.Socket;
				switch (_settings.IOType)
				{
					case Domain.IOType.TcpClient:
						sb.Append("TCP client is ");
						sb.Append(isConnected ? "connected to " : "disconnected from ");
						sb.Append(s.ResolvedRemoteIPAddress.ToString());
						sb.Append(" on remote port ");
						sb.Append(s.RemotePort.ToString());
						break;

					case Domain.IOType.TcpServer:
						sb.Append("TCP server is ");
						if (isOpen)
							sb.Append(isConnected ? "connected" : "listening");
						else
							sb.Append("closed");
						sb.Append(" on local port ");
						sb.Append(s.LocalPort.ToString());
						break;

					case Domain.IOType.TcpAutoSocket:
						bool isClient = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsClient;
						bool isServer = ((Domain.IO.TcpAutoSocket)(_terminal.UnderlyingIOProvider)).IsServer;
						sb.Append("TCP auto socket is ");
						if (isOpen)
						{
							if (isClient)
							{
								sb.Append("connected to ");
								sb.Append(s.ResolvedRemoteIPAddress.ToString());
								sb.Append(" on remote port ");
								sb.Append(s.RemotePort.ToString());
							}
							else if (isServer)
							{
								sb.Append(isConnected ? "connected" : "listening");
								sb.Append(" on local port ");
								sb.Append(s.LocalPort.ToString());
							}
							else
							{
								sb.Append("starting on port ");
								sb.Append(s.RemotePort.ToString());
							}
						}
						else
						{
							sb.Append("closed on port ");
							sb.Append(s.RemotePort.ToString());
						}
						break;

					case Domain.IOType.Udp:
						sb.Append("UDP socket is ");
						sb.Append(isOpen ? "open" : "closed");
						sb.Append(" for sending to ");
						sb.Append(s.ResolvedRemoteIPAddress.ToString());
						sb.Append(" on remote port ");
						sb.Append(s.RemotePort.ToString());
						sb.Append(" and receiving on local port ");
						sb.Append(s.LocalPort.ToString());
						break;
				}

				Image on = Properties.Resources.Image_On_12x12;
				Image off = Properties.Resources.Image_Off_12x12;

				toolStripStatusLabel_TerminalStatus_Connection.Image = (isConnected ? on : off);
			}

			toolStripStatusLabel_TerminalStatus_IOStatus.Text = sb.ToString();
		}

		private void SetIOControlControls()
		{
			bool isOpen = _terminal.IsOpen;
			bool isSerialPort = (_settings.IOType == Domain.IOType.SerialPort);

			foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
				sl.Visible = isSerialPort;

			if (isSerialPort)
			{
				foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
					sl.Enabled = isOpen;

				Image on = Properties.Resources.Image_On_12x12;
				Image off = Properties.Resources.Image_Off_12x12;

				if (isOpen)
				{
					HSR.IO.Ports.SerialPortControlPins pins;
					pins = ((HSR.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance).ControlPins;

					bool rs485Handshake = (_settings.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.RS485);

					if (rs485Handshake)
					{
						if (pins.Rts)
							TriggerRtsLuminescence();
					}
					else
					{
						toolStripStatusLabel_TerminalStatus_RTS.Image = (pins.Rts ? on : off);
					}

					toolStripStatusLabel_TerminalStatus_CTS.Image = (pins.Cts ? on : off);
					toolStripStatusLabel_TerminalStatus_DTR.Image = (pins.Dtr ? on : off);
					toolStripStatusLabel_TerminalStatus_DSR.Image = (pins.Dsr ? on : off);
					toolStripStatusLabel_TerminalStatus_DCD.Image = (pins.Cd ? on : off);

					bool manualHandshake = (_settings.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual);

					toolStripStatusLabel_TerminalStatus_RTS.ForeColor = (manualHandshake ? SystemColors.ControlText : SystemColors.GrayText);
					toolStripStatusLabel_TerminalStatus_CTS.ForeColor = SystemColors.GrayText;
					toolStripStatusLabel_TerminalStatus_DTR.ForeColor = (manualHandshake ? SystemColors.ControlText : SystemColors.GrayText);
					toolStripStatusLabel_TerminalStatus_DSR.ForeColor = SystemColors.GrayText;
					toolStripStatusLabel_TerminalStatus_DCD.ForeColor = SystemColors.GrayText;
				}
				else
				{
					foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
						sl.Image = off;

					foreach (ToolStripStatusLabel sl in _statusLabels_ioControl)
						sl.ForeColor = SystemColors.ControlText;
				}
			}
		}

		private void TriggerRtsLuminescence()
		{
			timer_RtsLuminescence.Enabled = false;
			toolStripStatusLabel_TerminalStatus_RTS.Image = Properties.Resources.Image_On_12x12;
			timer_RtsLuminescence.Interval = _RtsLuminescenceInterval;
			timer_RtsLuminescence.Enabled = true;
		}

		private void ResetRts()
		{
			Image on = Properties.Resources.Image_On_12x12;
			Image off = Properties.Resources.Image_Off_12x12;

			if (_terminal.IsOpen)
			{
				HSR.IO.Ports.SerialPortControlPins pins;
				pins = ((HSR.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance).ControlPins;

				toolStripStatusLabel_TerminalStatus_RTS.Image = (pins.Rts ? on : off);
			}
			else
			{
				toolStripStatusLabel_TerminalStatus_RTS.Image = off;
			}
		}

		private void timer_RtsLuminescence_Tick(object sender, EventArgs e)
		{
			timer_RtsLuminescence.Enabled = false;
			ResetRts();
		}

		#endregion

		#region Terminal > Open/Close
		//------------------------------------------------------------------------------------------
		// Terminal > Open/Close
		//------------------------------------------------------------------------------------------

		private bool OpenTerminal()
		{
			return (OpenTerminal(true));
		}

		private bool OpenTerminal(bool saveStatus)
		{
			bool success = false;

			Cursor = Cursors.WaitCursor;
			SetFixedStatusText("Opening terminal...");
			Refresh();
			try
			{
				_terminal.Open();
				if (saveStatus) _settings.TerminalIsOpen = _terminal.IsOpen;
				SetTimedStatusText("Terminal opened");
				success = true;
			}
			catch (Exception ex)
			{
				SetFixedStatusText("Error opening terminal!");

				string ioText;
				if (_settings.IOType == Domain.IOType.SerialPort)
					ioText = "Port";
				else
					ioText = "Socket";

				MessageBox.Show
					(
					this,
					"Unable to open terminal:" + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					ioText + " may be in use by another process.",
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				SetTimedStatusText("Terminal not opened!");
				success = false;
			}
			Cursor = Cursors.Default;

			return (success);
		}

		private bool CloseTerminal()
		{
			return (CloseTerminal(true));
		}

		private bool CloseTerminal(bool saveStatus)
		{
			bool success = false;

			Cursor = Cursors.WaitCursor;
			SetFixedStatusText("Closing terminal...");
			Refresh();
			try
			{
				_terminal.Close();
				if (saveStatus) _settings.TerminalIsOpen = _terminal.IsOpen;
				SetTimedStatusText("Terminal closed");
				success = true;
			}
			catch (Exception ex)
			{
				SetTimedStatusText("Error closing terminal!");

				MessageBox.Show
					(
					this,
					"Unable to close terminal:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				SetTimedStatusText("Terminal not closed!");
				success = false;
			}
			Cursor = Cursors.Default;

			return (success);
		}

		#endregion

		#region Terminal > Settings
		//------------------------------------------------------------------------------------------
		// Terminal > Settings
		//------------------------------------------------------------------------------------------

		private void ShowTerminalSettings()
		{
			SetFixedStatusText("Terminal Settings...");

			Gui.Forms.TerminalSettings f = new Gui.Forms.TerminalSettings(_settings.Terminal);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				Domain.Settings.TerminalSettings s = f.SettingsResult;
				if (s.HaveChanged)
				{
					// settings have changed, recreate terminal with new settings
					if (_terminal.IsOpen)
					{
						// terminal is open, re-open it with the new settings
						if (CloseTerminal(false))
						{
							SuspendHandlingTerminalSettings();

							DetachTerminalHandlers();      // detach to suspend events
							_settings.Terminal = s;
							_terminal = Domain.Factory.TerminalFactory.RecreateTerminal(_settings.Terminal, _terminal);
							AttachTerminalHandlers();      // attach and resume events
							_terminal.ReloadRepositories();

							ResumeHandlingTerminalSettings();

							OpenTerminal(false);
							SetTimedStatusText("Terminal settings applied.");
						}
						else
						{
							SetTimedStatusText("Terminal settings not applied!");
						}
					}
					else
					{
						// terminal is closed, simply set the new settings
						SuspendHandlingTerminalSettings();

						DetachTerminalHandlers();          // detach to suspend events
						_settings.Terminal = s;
						_terminal = Domain.Factory.TerminalFactory.RecreateTerminal(_settings.Terminal, _terminal);
						AttachTerminalHandlers();          // attach an resume events
						_terminal.ReloadRepositories();

						ResumeHandlingTerminalSettings();

						SetTimedStatusText("Terminal settings applied.");
					}
				}
				else
				{
					SetTimedStatusText("Terminal settings not changed.");
				}
			}
			else
			{
				ResetStatusText();
			}
		}

		#endregion

		#region Terminal > IO Control
		//------------------------------------------------------------------------------------------
		// Terminal > IO Control
		//------------------------------------------------------------------------------------------

		private void RequestToggleRts()
		{
			if (_settings.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				HSR.IO.Ports.ISerialPort port = (HSR.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleRts();
				_settings.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		private void RequestToggleDtr()
		{
			if (_settings.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				HSR.IO.Ports.ISerialPort port = (HSR.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleDtr();
				_settings.Terminal.IO.SerialPort.DtrEnabled = port.DtrEnable;
			}
		}

		#endregion

		#region Terminal > Send
		//------------------------------------------------------------------------------------------
		// Terminal > Send
		//------------------------------------------------------------------------------------------

		private void Send(byte[] b)
		{
			SetFixedStatusText("Sending " + b.Length + " bytes...");
			try
			{
				_terminal.Send(b);
				SetTimedStatusText(b.Length + " bytes sent");
			}
			catch (System.IO.IOException ex)
			{
				SetFixedStatusText("Error sending " + b.Length + " bytes!");

				string text = "Unable to write to ";
				string title;
				switch (_settings.IOType)
				{
					case Domain.IOType.SerialPort: text += "port"; title = "Serial Port"; break;
					default: text += "socket"; title = "Socket"; break;
				}
				text += ":";
				title += " Error";
				MessageBox.Show
					(
					this,
					text + Environment.NewLine + Environment.NewLine + ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				SetTimedStatusText("Data not sent!");
			}
		}

		private void SendLine(string s)
		{
			SetFixedStatusText("Sending \"" + s + "\"...");
			try
			{
				_terminal.SendLine(s);
				SetTimedStatusText("\"" + s + "\" sent");
			}
			catch (System.IO.IOException ex)
			{
				SetFixedStatusText("Error sending \"" + s + "\"!");

				string text = "Unable to write to ";
				string title;
				switch (_settings.IOType)
				{
					case Domain.IOType.SerialPort: text += "port"; title = "Serial Port"; break;
					default: text += "socket"; title = "Socket"; break;
				}
				text += ":";
				title += " Error";
				MessageBox.Show
					(
					this,
					text + Environment.NewLine + Environment.NewLine + ex.Message,
					title,
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				SetTimedStatusText("Data not sent!");
			}
			catch (Domain.Parser.FormatException ex)
			{
				ResetStatusText();
				MessageBox.Show
					(
					this,
					"Bad data format:" + Environment.NewLine + Environment.NewLine + ex.Message,
					"Format Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _terminal_TerminalChanged(object sender, EventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(new EventArgs());
		}

		private void _terminal_TerminalControlChanged(object sender, EventArgs e)
		{
			SetIOControlControls();
		}

		private void _terminal_TerminalError(object sender, Domain.TerminalErrorEventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(new EventArgs());

			MessageBox.Show
				(
				this,
				"Terminal error:" + Environment.NewLine + Environment.NewLine + e.Message,
				"Terminal Error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error
				);
		}

		private void _terminal_RawElementSent(object sender, Domain.RawElementEventArgs e)
		{
			// counter
			int byteCount = e.Element.Data.Length;
			monitor_Tx.TxByteCountStatus += byteCount;
			monitor_Bidir.TxByteCountStatus += byteCount;

			// log
			if (_log.IsOpen)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawTx);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
			}
		}

		private void _terminal_RawElementReceived(object sender, Domain.RawElementEventArgs e)
		{
			// counter
			int byteCount = e.Element.Data.Length;
			monitor_Bidir.RxByteCountStatus += byteCount;
			monitor_Rx.RxByteCountStatus += byteCount;

			// log
			if (_log.IsOpen)
			{
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawBidir);
				_log.WriteBytes(e.Element.Data, Log.LogStreams.RawRx);
			}
		}

		private void _terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			// display
			monitor_Tx.AddElements(e.Elements);
			monitor_Bidir.AddElements(e.Elements);

			// log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (_log.IsOpen)
				{
					if (de.IsEol)
					{
						_log.WriteEol(Log.LogStreams.NeatTx);
						_log.WriteEol(Log.LogStreams.NeatBidir);
					}
					else
					{
						_log.WriteString(de.Text, Log.LogStreams.NeatTx);
						_log.WriteString(de.Text, Log.LogStreams.NeatBidir);
					}
				}
			}
		}

		private void _terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			// display
			monitor_Bidir.AddElements(e.Elements);
			monitor_Rx.AddElements(e.Elements);

			// log
			foreach (Domain.DisplayElement de in e.Elements)
			{
				if (_log.IsOpen)
				{
					if (de.IsEol)
					{
						_log.WriteEol(Log.LogStreams.NeatBidir);
						_log.WriteEol(Log.LogStreams.NeatRx);
					}
					else
					{
						_log.WriteString(de.Text, Log.LogStreams.NeatBidir);
						_log.WriteString(de.Text, Log.LogStreams.NeatRx);
					}
				}
			}
		}

		private void _terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (e.Lines.Count > 0)
			{
				monitor_Tx.ReplaceLastLine(e.Lines[0]);
				monitor_Bidir.ReplaceLastLine(e.Lines[0]);
			}
			for (int i = 1; i < e.Lines.Count; i++)
			{
				monitor_Tx.AddLine(e.Lines[i]);
				monitor_Bidir.AddLine(e.Lines[i]);
			}

			monitor_Tx.TxLineCountStatus += e.Lines.Count;
			monitor_Bidir.TxLineCountStatus += e.Lines.Count;
		}

		private void _terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (e.Lines.Count > 0)
			{
				monitor_Bidir.ReplaceLastLine(e.Lines[0]);
				monitor_Rx.ReplaceLastLine(e.Lines[0]);
			}
			for (int i = 1; i < e.Lines.Count; i++)
			{
				monitor_Bidir.AddLine(e.Lines[i]);
				monitor_Rx.AddLine(e.Lines[i]);
			}

			monitor_Bidir.RxLineCountStatus += e.Lines.Count;
			monitor_Rx.RxLineCountStatus += e.Lines.Count;
		}

		private void _terminal_RepositoryCleared(object sender, Domain.RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case Domain.RepositoryType.Tx:    monitor_Tx.Clear();    break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.Clear(); break;
				case Domain.RepositoryType.Rx:    monitor_Rx.Clear();    break;
			}
		}

		private void _terminal_RepositoryReloaded(object sender, Domain.RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case Domain.RepositoryType.Tx:    monitor_Tx.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Tx));       break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Bidir)); break;
				case Domain.RepositoryType.Rx:    monitor_Rx.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Rx));       break;
			}
		}

		#endregion

		#endregion

		#region Send
		//******************************************************************************************
		// Send
		//******************************************************************************************

		#region Send > Command
		//------------------------------------------------------------------------------------------
		// Send > Command
		//------------------------------------------------------------------------------------------

		private void SendCommand()
		{
			SendCommand(_settings.SendCommand.Command);
			_settings.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
				(
				new Utilities.Recent.RecentItem<Command>(new Command(_settings.SendCommand.Command))
				);
		}

		private void SendCommand(Command command)
		{
			if (command.IsValidCommand)
			{
				if (command.IsSingleLineCommand)
				{
					if (SendCommandSettings.IsEasterEggCommand(command.SingleLineCommand))
						SendLine(SendCommandSettings.EasterEggCommandText);
					else
						SendLine(command.SingleLineCommand);
				}
				else
				{
					foreach (string line in command.MultiLineCommand)
						SendLine(line);
				}
			}
			else
			{
				SendLine("");
			}
		}

		#endregion

		#region Send > File
		//------------------------------------------------------------------------------------------
		// Send > File
		//------------------------------------------------------------------------------------------

		private void SendFile()
		{
			SendFile(_settings.SendFile.Command);
		}

		private void SendFile(Command command)
		{
			if (!command.IsValidFilePath)
				return;

			string filePath = command.FilePath;

			try
			{
				if (_terminal is Domain.TextTerminal)
				{
					string[] lines;
					if (ApplicationSettings.Extensions.IsXml(System.IO.Path.GetExtension(filePath)))
					{
						// xml
						lines = XmlReader.LinesFromXmlFile(filePath);
					}
					else if (ApplicationSettings.Extensions.IsRtf(System.IO.Path.GetExtension(filePath)))
					{
						// rtf
						lines = RtfReader.LinesFromRtfFile(filePath);
					}
					else
					{
						// text
						using (StreamReader sr = new StreamReader(filePath))
						{
							string s;
							List<string> l = new List<string>();
							while ((s = sr.ReadLine()) != null)
							{
								l.Add(s);
							}
							sr.Close();                   // close file before sending
							lines = l.ToArray();
						}
					}

					foreach (string line in lines)
					{
						SendLine(line);
					}
				}
				else
				{
					using (FileStream fs = File.OpenRead(filePath))
					{
						byte[] a = new byte[(int)fs.Length];
						fs.Read(a, 0, (int)fs.Length);
						fs.Close();                   // close file before sending
						Send(a);
					}
				}
			}
			catch (Exception e)
			{
				MessageBox.Show
					(
					this,
					"Error while accessing file" + Environment.NewLine +
					filePath + Environment.NewLine + Environment.NewLine +
					e.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);
			}
		}

		#endregion

		#endregion

		#region Predefined
		//******************************************************************************************
		// Predefined
		//******************************************************************************************

		private void InitializePredefinedMenuItems()
		{
			_menuItems_predefined = new List<ToolStripMenuItem>(Settings.PredefinedCommandSettings.MaximumCommandsPerPage);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_1);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_2);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_3);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_4);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_5);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_6);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_7);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_8);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_9);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_10);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_11);
			_menuItems_predefined.Add(toolStripMenuItem_PredefinedContextMenu_Command_12);
		}

		private void SetPredefinedMenuItems()
		{
			// pages
			List<PredefinedCommandPage> pages = _settings.PredefinedCommand.Pages;

			if ((pages != null) && (pages.Count > 0))
			{
				toolStripMenuItem_PredefinedContextMenu_Page_Previous.Enabled = (predefined.SelectedPage > pages.Count);
				toolStripMenuItem_PredefinedContextMenu_Page_Next.Enabled = (predefined.SelectedPage < pages.Count);
			}
			else
			{
				toolStripMenuItem_PredefinedContextMenu_Page_Previous.Enabled = false;
				toolStripMenuItem_PredefinedContextMenu_Page_Next.Enabled = false;
			}

			// commands
			List<Command> commands = null;
			if ((pages != null) && (pages.Count > 0))
				commands = _settings.PredefinedCommand.Pages[predefined.SelectedPage - 1].Commands;

			int commandCount = 0;
			if (commands != null)
				commandCount = commands.Count;

			for (int i = 0; i < commandCount; i++)
			{
				bool isDefined = ((commands[i] != null) && !commands[i].IsEmpty);
				bool isValid = (isDefined && _terminal.IsOpen && commands[i].IsValid);

				if (isDefined)
				{
					_menuItems_predefined[i].Text = commands[i].Description;
					_menuItems_predefined[i].Enabled = isValid;
				}
				else
				{
					_menuItems_predefined[i].Text = Command.UndefinedCommandText;
					_menuItems_predefined[i].Enabled = true;
				}
			}
			for (int i = commandCount; i < Settings.PredefinedCommandSettings.MaximumCommandsPerPage; i++)
			{
				_menuItems_predefined[i].Text = Command.UndefinedCommandText;
				_menuItems_predefined[i].Enabled = true;
			}
		}

		private void RequestPredefined(int page, int command)
		{
			List<PredefinedCommandPage> pages = _settings.PredefinedCommand.Pages;
			if (page <= pages.Count)
			{
				bool isDefined = false;
				if (page > 0)
				{
					List<Command> commands = _settings.PredefinedCommand.Pages[page - 1].Commands;
					isDefined =
						(
						(commands != null) &&
						(commands.Count >= command) &&
						(commands[command - 1] != null) &&
						(!commands[command - 1].IsEmpty)
						);
				}
                if (isDefined)
				{
					SendPredefined(page, command);
					return;
				}
			}

			// if command is not defined, show settings dialog
			ShowPredefinedCommandSettings(page, command);
		}

		private void SendPredefined(int page, int command)
		{
			Command c = _settings.PredefinedCommand.Pages[page - 1].Commands[command - 1];

			if (c.IsCommand)
				SendCommand(c);
			else if (c.IsFilePath)
				SendFile(c);
		}

		private void ShowPredefinedCommandSettings(int page, int command)
		{
			PredefinedCommandSettings f = new PredefinedCommandSettings(_settings.PredefinedCommand, page, command);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_settings.PredefinedCommand = f.SettingsResult;
				_settings.Predefined.SelectedPage = f.SelectedPage;
			}
		}

		#endregion

		#region Monitor
		//******************************************************************************************
		// Monitor
		//******************************************************************************************

		#region Monitor > Access
		//------------------------------------------------------------------------------------------
		// Monitor > Access
		//------------------------------------------------------------------------------------------

		private Domain.RepositoryType GetMonitorType(Control source)
		{
			if ((source == monitor_Tx) || (source == panel_Monitor_Tx))
				return (Domain.RepositoryType.Tx);
			if ((source == monitor_Bidir) || (source == panel_Monitor_Bidir))
				return (Domain.RepositoryType.Bidir);
			if ((source == monitor_Rx) || (source == panel_Monitor_Rx))
				return (Domain.RepositoryType.Rx);

			return (Domain.RepositoryType.None);
		}

		private Controls.Monitor GetMonitor(Control source)
		{
			switch (GetMonitorType(source))
			{
				case Domain.RepositoryType.Tx:    return (monitor_Tx);
				case Domain.RepositoryType.Bidir: return (monitor_Bidir);
				case Domain.RepositoryType.Rx:    return (monitor_Rx);
			}
			return (null);
		}

		#endregion

		#region Monitor > View
		//------------------------------------------------------------------------------------------
		// Monitor > View
		//------------------------------------------------------------------------------------------

		private void InitializeMonitorMenuItems()
		{
			_isSettingControls = true;

			toolStripComboBox_TerminalMenu_Monitor_Orientation.Items.Clear();
			toolStripComboBox_TerminalMenu_Monitor_Orientation.Items.Add(Orientation.Vertical);
			toolStripComboBox_TerminalMenu_Monitor_Orientation.Items.Add(Orientation.Horizontal);
			toolStripComboBox_TerminalMenu_Monitor_Orientation.SelectedIndex = 0;

			toolStripComboBox_MonitorContextMenu_Orientation.Items.Clear();
			toolStripComboBox_MonitorContextMenu_Orientation.Items.Add(Orientation.Vertical);
			toolStripComboBox_MonitorContextMenu_Orientation.Items.Add(Orientation.Horizontal);
			toolStripComboBox_MonitorContextMenu_Orientation.SelectedIndex = 0;

			_isSettingControls = false;
		}

		private void SetMonitorRadix(Domain.Radix radix)
		{
			_settings.Display.Radix = radix;
		}

		private void SetMonitorOrientation(Orientation orientation)
		{
			_settings.Layout.MonitorOrientation = orientation;

			SuspendLayout();
			splitContainer_MonitorLeft.Orientation = orientation;
			splitContainer_MonitorRight.Orientation = orientation;
			ResumeLayout();
		}

		private void ReloadMonitors()
		{
			SetFixedStatusText("Reloading...");
			Cursor = Cursors.WaitCursor;
			_terminal.ReloadRepositories();
			Cursor = Cursors.Default;
			SetTimedStatusText("Reloading done");
		}

		private void ReformatMonitors()
		{
			SetFixedStatusText("Reformatting...");
			Cursor = Cursors.WaitCursor;

			monitor_Tx.FormatSettings = _settings.Format;
			monitor_Bidir.FormatSettings = _settings.Format;
			monitor_Rx.FormatSettings = _settings.Format;

			monitor_Tx.Reload(_terminal.RepositoryToDisplayElements(Domain.RepositoryType.Tx));
			monitor_Bidir.Reload(_terminal.RepositoryToDisplayElements(Domain.RepositoryType.Bidir));
			monitor_Rx.Reload(_terminal.RepositoryToDisplayElements(Domain.RepositoryType.Rx));

			Cursor = Cursors.Default;
			SetTimedStatusText("Reformatting done");
		}

		private void ClearMonitor(Domain.RepositoryType repositoryType)
		{
			_terminal.ClearRepository(repositoryType);
		}

		private void ClearAllMonitors()
		{
			_terminal.ClearRepositories();
		}

		private void ClearCountStatus()
		{
			monitor_Tx.ResetCountStatus();
			monitor_Bidir.ResetCountStatus();
			monitor_Rx.ResetCountStatus();
		}

		#endregion

		#region Monitor > Methods
		//------------------------------------------------------------------------------------------
		// Monitor > Methods
		//------------------------------------------------------------------------------------------

		private void ShowFormatSettings()
		{
			Gui.Forms.FormatSettings f = new Gui.Forms.FormatSettings(_settings.Format);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_settings.Format = f.SettingsResult;
			}
		}

		private void ShowSaveMonitorDialog(Controls.Monitor monitor)
		{
			SetFixedStatusText("Saving monitor as...");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save As";
			sfd.Filter = ApplicationSettings.Extensions.TextFilesFilter;
			sfd.DefaultExt = ApplicationSettings.Extensions.TextFilesDefault;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Path.MonitorFilesPath;
			if (sfd.ShowDialog(this) == DialogResult.OK && sfd.FileName.Length > 0)
			{
				Refresh();
				ApplicationSettings.LocalUser.Path.MonitorFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				SaveMonitor(monitor, sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void SaveMonitor(Controls.Monitor monitor, string filePath)
		{
			SetFixedStatusText("Saving monitor...");
			try
			{
				if (ApplicationSettings.Extensions.IsXml(System.IO.Path.GetExtension(filePath)))
					XmlWriter.LinesToXmlFile(monitor.SelectedLines, filePath);
				else if (ApplicationSettings.Extensions.IsRtf(System.IO.Path.GetExtension(filePath)))
					RtfWriter.LinesToRtfFile(monitor.SelectedLines, filePath, _settings.Format, RichTextBoxStreamType.RichText);
				else
					RtfWriter.LinesToRtfFile(monitor.SelectedLines, filePath, _settings.Format, RichTextBoxStreamType.PlainText);

				SetTimedStatusText("Monitor saved");
			}
			catch (System.IO.IOException e)
			{
				SetFixedStatusText("Error saving monitor!");

				MessageBox.Show
					(
					this,
					"Unable to save file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					e.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);

				SetTimedStatusText("Monitor not saved!");
			}
		}

		private void CopyMonitorToClipboard(Controls.Monitor monitor)
		{
			SetFixedStatusText("Copying monitor...");
			RtfWriter.LinesToClipboard(monitor.SelectedLines, _settings.Format);
			SetTimedStatusText("Monitor copied");
		}

		private void ShowPrintMonitorDialog(Controls.Monitor monitor)
		{
			PrintDialog pd = new PrintDialog();
			pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();
			if (pd.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				PrintMonitor(monitor, pd.PrinterSettings);
			}
		}

		private void PrintMonitor(Controls.Monitor monitor)
		{
			PrintMonitor(monitor, new System.Drawing.Printing.PrinterSettings());
		}

		private void PrintMonitor(Controls.Monitor monitor, System.Drawing.Printing.PrinterSettings settings)
		{
			SetFixedStatusText("Printing monitor...");

			try
			{
				RtfPrinter printer = new RtfPrinter(settings);
				printer.Print(RtfWriter.LinesToRichTextBox(monitor.SelectedLines, _settings.Format));
				SetTimedStatusText("Monitor printed");
			}
			catch (Exception e)
			{
				SetFixedStatusText("Error printing monitor!");

				MessageBox.Show
					(
					this,
					"Unable to print monitor." + Environment.NewLine + Environment.NewLine +
					e.Message,
					"Print Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);

				SetTimedStatusText("Monitor not printed!");
			}
		}

		#endregion

		#endregion

		#region Log
		//******************************************************************************************
		// Log
		//******************************************************************************************

		private void SetLogControls()
		{
			bool logSelected = _settings.Log.AnyRawOrNeat;
			bool logOpen = _settings.LogIsOpen;

			toolStripMenuItem_TerminalMenu_Log_Begin.Enabled = logSelected && !logOpen;
			toolStripMenuItem_TerminalMenu_Log_End.Enabled = logSelected && logOpen;
			toolStripMenuItem_TerminalMenu_Log_Clear.Enabled = logSelected && logOpen;
		}

		private void ShowLogSettings()
		{
			Gui.Forms.LogSettings f = new Gui.Forms.LogSettings(_settings.Log);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_settings.Log = f.SettingsResult;
				_log.Settings = _settings.Log;
			}
		}

		private void BeginLog()
		{
			try
			{
				// reapply setting NOW, makes sure that date/time in filenames is refreshed
				_log.Settings = _settings.Log;
				_log.Begin();
				_settings.LogIsOpen = true;
			}
			catch (System.IO.IOException ex)
			{
				MessageBox.Show
					(
					this,
					"Unable to begin log." + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					"Log file may be in use by another process.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
			}
		}

		private void ClearLog()
		{
			try
			{
				_log.Clear();
			}
			catch (System.IO.IOException ex)
			{
				MessageBox.Show
					(
					this,
					"Unable to clear log." + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					"Log file may be in use by another process.",
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
			}
		}

		private void EndLog()
		{
			try
			{
				_log.End();
				_settings.LogIsOpen = false;
			}
			catch (System.IO.IOException ex)
			{
				MessageBox.Show
					(
					this,
					"Unable to end log." + Environment.NewLine + Environment.NewLine +
					ex.Message,
					"Log File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Warning
					);
			}
		}

		#endregion

		#region Event Invoking
		//******************************************************************************************
		// Event Invoking
		//******************************************************************************************

		protected virtual void OnTerminalChanged(EventArgs e)
		{
			Utilities.Event.EventHelper.FireSync(TerminalChanged, this, e);
		}

		protected virtual void OnTerminalSaved(TerminalSavedEventArgs e)
		{
			Utilities.Event.EventHelper.FireSync<TerminalSavedEventArgs>(TerminalSaved, this, e);
		}

		#endregion

		#region Status
		//******************************************************************************************
		// Status
		//******************************************************************************************

		private enum Status
		{
			Default
		}

		private string GetStatusText(Status status)
		{
			switch (status)
			{
				default: return (_DefaultStatusText);
			}
		}

		private void SetFixedStatusText(string text)
		{
			timer_Status.Enabled = false;
			toolStripStatusLabel_TerminalStatus_Status.Text = text;
		}

		private void SetFixedStatus(Status status)
		{
			SetFixedStatusText(GetStatusText(status));
		}

		private void SetTimedStatusText(string text)
		{
			timer_Status.Enabled = false;
			toolStripStatusLabel_TerminalStatus_Status.Text = text;
			timer_Status.Interval = _TimedStatusInterval;
			timer_Status.Enabled = true;
		}

		private void SetTimedStatus(Status status)
		{
			SetTimedStatusText(GetStatusText(status));
		}

		private void ResetStatusText()
		{
			SetFixedStatus(Status.Default);
		}

		private void timer_Status_Tick(object sender, EventArgs e)
		{
			timer_Status.Enabled = false;
			ResetStatusText();
		}

		#endregion
	}
}
