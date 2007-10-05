using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.Event;
using MKY.Utilities.Recent;
using MKY.Utilities.Settings;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Gui.Settings;

namespace YAT.Gui.Forms
{
	public partial class Terminal : Form
	{
		//------------------------------------------------------------------------------------------
		// Constants
		//------------------------------------------------------------------------------------------


		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		// startup/update
		private bool _isStartingUp = true;
		private bool _isSettingControls = false;

		// MDI
		private Guid _guid = Guid.NewGuid();
		private string _userName;
		private Form _mdiParent;

		// settings
		private DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> _terminalSettingsHandler;
		private YAT.Settings.Terminal.TerminalSettingsRoot _terminalSettingsRoot;
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
		// - Within visual designer, proceed in the following order
		//     1. Place "menuStrip_Terminal" and dock it to top
		//     2. Place "statusStrip_Terminal" and dock it to bottom
		//     3. Place "splitContainer_Terminal" and dock it to fill

		public Terminal()
		{
			InitializeComponent();
			Initialize(new DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot>());
		}

		public Terminal(DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> settingsHandler)
		{
			InitializeComponent();
			Initialize(settingsHandler);
		}

		private void Initialize(DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> settingsHandler)
		{
			FixContextMenus();

			InitializePredefinedMenuItems();
			InitializeMonitorMenuItems();
			InitializeIOControlStatusLabels();

			_terminalSettingsHandler = settingsHandler;
			_terminalSettingsRoot = _terminalSettingsHandler.Settings;
			AttachSettingsHandlers();

			ApplyWindowSettings();
			LayoutTerminal();

			_terminal = Domain.Factory.TerminalFactory.CreateTerminal(_terminalSettingsRoot.Terminal);
			AttachTerminalHandlers();

			_log = new Log.Logs(_terminalSettingsRoot.Log);

			// force settings changed event to set all controls
			// for improved performance, manually suspend/resume handler for terminal settings
			SuspendHandlingTerminalSettings();
			_terminalSettingsRoot.ClearChanged();
			_terminalSettingsRoot.ForceChangeEvent();
			ResumeHandlingTerminalSettings();
		}

		#region Properties
		//******************************************************************************************
		// Properties
		//******************************************************************************************

		public Guid Guid
		{
			get { return (_guid); }
			set { _guid = value; }
		}

		public string UserName
		{
			get { return (_userName); }
			set { _userName = value; }
		}

		public string UserNameFromFile
		{
			set { _userName = Path.GetFileName(value); }
		}

		public string SettingsFilePath
		{
			get
			{
				if (_terminalSettingsHandler.SettingsFileExists)
					return (_terminalSettingsHandler.SettingsFilePath);
				else
					return ("");
			}
		}

		public bool AutoSaved
		{
			get { return (_terminalSettingsRoot.AutoSaved); }
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
			SaveTerminalFile();
		}

		public void RequestAutoSaveFile()
		{
			// only perform auto save if no file yet or on previously auto saved files
			if (!_terminalSettingsHandler.SettingsFileExists ||
				(_terminalSettingsHandler.SettingsFileExists && _terminalSettingsRoot.AutoSaved))
				SaveTerminalFile(true);
		}

		public void RequestCloseFile()
		{
			CloseTerminalFile();
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

				// select send command control to enable immediate user input
				SelectSendCommandInput();

				// then begin logging (in case opening of terminal needs to be logged)
				if (_terminalSettingsRoot.LogIsOpen)
					BeginLog();

				// then open terminal
				if (_terminalSettingsRoot.TerminalIsOpen)
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

		private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_terminalSettingsRoot.HaveChanged && _terminalSettingsRoot.AutoSaved)
			{
				SaveTerminalFile(true);
			}
			else if (_terminalSettingsRoot.ExplicitHaveChanged)
			{
				DialogResult dr = MessageBox.Show
					(
					this,
					"Save terminal?",
					Text,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Question
					);

				switch (dr)
				{
					case DialogResult.Yes: SaveTerminalFile(); break;
					case DialogResult.No:                      break;
					default:               e.Cancel = true;    return;
				}
			}

			if (_terminal.IsOpen)
				CloseTerminal(false);

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
			// nothing to do
		}

		private void toolStripMenuItem_TerminalMenu_File_Save_Click(object sender, EventArgs e)
		{
			SaveTerminalFile();
		}

		private void toolStripMenuItem_TerminalMenu_File_SaveAs_Click(object sender, EventArgs e)
		{
			ShowSaveTerminalFileAsDialog();
		}

		private void toolStripMenuItem_TerminalMenu_File_Close_Click(object sender, EventArgs e)
		{
			CloseTerminalFile();
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

		private void toolStripMenuItem_TerminalMenu_Terminal_Clear_Click(object sender, EventArgs e)
		{
			ClearAllMonitors();
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
			toolStripMenuItem_TerminalMenu_Send_Command.Enabled = _terminalSettingsRoot.SendCommand.Command.IsValidCommand;
			toolStripMenuItem_TerminalMenu_Send_File.Enabled = _terminalSettingsRoot.SendCommand.Command.IsValidFilePath;
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
			Domain.TerminalType terminalType = _terminalSettingsRoot.TerminalType;

			// panels
			toolStripMenuItem_TerminalMenu_View_Panels_Tx.Checked = _terminalSettingsRoot.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_Rx.Checked = _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible;

			// disable monitor item if the other monitors are hidden
			toolStripMenuItem_TerminalMenu_View_Panels_Tx.Enabled = (_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible || _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Enabled = (_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible || _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Panels_Rx.Enabled = (_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible || _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible);

			toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedItem = _terminalSettingsRoot.Layout.MonitorOrientation;

			toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Checked = _terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked = _terminalSettingsRoot.Layout.SendFilePanelIsVisible;

			toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = _terminalSettingsRoot.Layout.PredefinedPanelIsVisible;

			// counters
			toolStripMenuItem_TerminalMenu_View_Counters_ShowCounters.Checked = _terminalSettingsRoot.Display.ShowCounters;
			toolStripMenuItem_TerminalMenu_View_Counters_ResetCounters.Enabled = _terminalSettingsRoot.Display.ShowCounters;

			// options
			toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Checked = _terminalSettingsRoot.Display.ShowTimeStamp;
			toolStripMenuItem_TerminalMenu_View_ShowLength.Checked = _terminalSettingsRoot.Display.ShowLength;
			
			bool enabled = (terminalType == Domain.TerminalType.Text);
			toolStripMenuItem_TerminalMenu_View_ShowEol.Enabled = enabled;
			toolStripMenuItem_TerminalMenu_View_ShowEol.Checked = enabled && _terminalSettingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Tx_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible = !_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Bidir_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible = !_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Rx_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.RxMonitorPanelIsVisible = !_terminalSettingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripComboBox_TerminalMenu_View_Panels_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				SetMonitorOrientation((Orientation)toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Predefined_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.PredefinedPanelIsVisible = !_terminalSettingsRoot.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_SendCommand_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.SendCommandPanelIsVisible = !_terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_SendFile_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.SendFilePanelIsVisible = !_terminalSettingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Rearrange_Click(object sender, EventArgs e)
		{
			ViewRearrange();
		}

		private void toolStripMenuItem_TerminalMenu_View_Counters_ShowCounters_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Display.ShowCounters = !_terminalSettingsRoot.Display.ShowCounters;
		}

		private void toolStripMenuItem_TerminalMenu_View_Counters_ResetCounters_Click(object sender, EventArgs e)
		{
			ClearCountStatus();
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTimeStamp_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Display.ShowTimeStamp = !_terminalSettingsRoot.Display.ShowTimeStamp;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowLength_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Display.ShowLength = !_terminalSettingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowEol_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.TextTerminal.ShowEol = !_terminalSettingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_TerminalMenu_View_Format_Click(object sender, EventArgs e)
		{
			ShowFormatSettings();
		}

		#endregion

		#endregion

		#region Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Monitor_Opening(object sender, CancelEventArgs e)
		{
			Domain.TerminalType terminalType = _terminalSettingsRoot.TerminalType;
			Domain.RepositoryType monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			bool isMonitor = (monitorType != Domain.RepositoryType.None);

			toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Checked = _terminalSettingsRoot.Display.ShowTimeStamp;
			toolStripMenuItem_MonitorContextMenu_ShowLength.Checked = _terminalSettingsRoot.Display.ShowLength;

			bool enabled = (terminalType == Domain.TerminalType.Text);
			toolStripMenuItem_MonitorContextMenu_ShowEol.Enabled = enabled;
			toolStripMenuItem_MonitorContextMenu_ShowEol.Checked = enabled && _terminalSettingsRoot.TextTerminal.ShowEol;

			toolStripMenuItem_MonitorContextMenu_Clear.Enabled = isMonitor;

			toolStripMenuItem_MonitorContextMenu_ShowCounters.Checked = _terminalSettingsRoot.Display.ShowCounters;
			toolStripMenuItem_MonitorContextMenu_ResetCounters.Enabled = _terminalSettingsRoot.Display.ShowCounters;

			toolStripMenuItem_MonitorContextMenu_Panels_Tx.Checked = _terminalSettingsRoot.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Checked = _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_Panels_Rx.Checked = _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible;

			// disable "Monitor" item if the other monitors are hidden
			toolStripMenuItem_MonitorContextMenu_Panels_Tx.Enabled = (_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible || _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Enabled = (_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible || _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_Panels_Rx.Enabled = (_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible || _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible);

			// hide "Hide" item if only this monitor is visible
			bool hideIsAllowed = false;
			switch (monitorType)
			{
				case Domain.RepositoryType.Tx:    hideIsAllowed = (_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible || _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible); break;
				case Domain.RepositoryType.Bidir: hideIsAllowed = (_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible || _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible); break;
				case Domain.RepositoryType.Rx:    hideIsAllowed = (_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible || _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible); break;
			}
			toolStripMenuItem_MonitorContextMenu_Hide.Visible = hideIsAllowed;
			toolStripMenuItem_MonitorContextMenu_Hide.Enabled = isMonitor && hideIsAllowed;

			toolStripMenuItem_MonitorContextMenu_SaveToFile.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_Print.Enabled = isMonitor;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTimeStamp_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Display.ShowTimeStamp = !_terminalSettingsRoot.Display.ShowTimeStamp;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLength_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Display.ShowLength = !_terminalSettingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowEol_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.TextTerminal.ShowEol = !_terminalSettingsRoot.TextTerminal.ShowEol;
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
			_terminalSettingsRoot.Display.ShowCounters = !_terminalSettingsRoot.Display.ShowCounters;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetCounters_Click(object sender, EventArgs e)
		{
			ClearCountStatus();
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Tx_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible = !_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Bidir_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible = !_terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Rx_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.RxMonitorPanelIsVisible = !_terminalSettingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Hide_Click(object sender, EventArgs e)
		{
			switch (GetMonitorType(contextMenuStrip_Monitor.SourceControl))
			{
				case Domain.RepositoryType.Tx:    _terminalSettingsRoot.Layout.TxMonitorPanelIsVisible    = false; break;
				case Domain.RepositoryType.Bidir: _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible = false; break;
				case Domain.RepositoryType.Rx:    _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible    = false; break;
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
			_terminalSettingsRoot.Layout.PredefinedPanelIsVisible = false;
		}

		#endregion

		#region Controls Event Handlers > Radix Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Radix Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Radix_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_RadixContextMenu_String.Checked = (_terminalSettingsRoot.Display.Radix == Domain.Radix.String);
			toolStripMenuItem_RadixContextMenu_Char.Checked = (_terminalSettingsRoot.Display.Radix == Domain.Radix.Char);
			toolStripMenuItem_RadixContextMenu_Bin.Checked = (_terminalSettingsRoot.Display.Radix == Domain.Radix.Bin);
			toolStripMenuItem_RadixContextMenu_Oct.Checked = (_terminalSettingsRoot.Display.Radix == Domain.Radix.Oct);
			toolStripMenuItem_RadixContextMenu_Dec.Checked = (_terminalSettingsRoot.Display.Radix == Domain.Radix.Dec);
			toolStripMenuItem_RadixContextMenu_Hex.Checked = (_terminalSettingsRoot.Display.Radix == Domain.Radix.Hex);
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

		#region Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Send_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_SendContextMenu_Command.Enabled = _terminalSettingsRoot.SendCommand.Command.IsValidCommand;
			toolStripMenuItem_SendContextMenu_File.Enabled = _terminalSettingsRoot.SendCommand.Command.IsValidFilePath;

			toolStripMenuItem_SendContextMenu_Panels_SendCommand.Checked = _terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
			toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = _terminalSettingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Command_Click(object sender, EventArgs e)
		{
			SendCommand();
		}

		private void toolStripMenuItem_SendContextMenu_File_Click(object sender, EventArgs e)
		{
			SendFile();
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendCommand_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.SendCommandPanelIsVisible = !_terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendFile_Click(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Layout.SendFilePanelIsVisible = !_terminalSettingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Hide_Click(object sender, EventArgs e)
		{
			Control source = contextMenuStrip_Send.SourceControl;

			if (source == sendCommand)
				_terminalSettingsRoot.Layout.SendCommandPanelIsVisible = false;
			if (source == sendFile)
				_terminalSettingsRoot.Layout.SendFilePanelIsVisible = false;
		}

		#endregion

		#region Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------

		private void splitContainer_TxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
			{
				int widthOrHeight = 0;
				if (_terminalSettingsRoot.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_TxMonitor.Width;
				else
					widthOrHeight = splitContainer_TxMonitor.Height;

				_terminalSettingsRoot.Layout.TxMonitorSplitterRatio = (float)splitContainer_TxMonitor.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_RxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
			{
				int widthOrHeight = 0;
				if (_terminalSettingsRoot.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_RxMonitor.Width;
				else
					widthOrHeight = splitContainer_RxMonitor.Height;

				_terminalSettingsRoot.Layout.RxMonitorSplitterRatio = (float)splitContainer_RxMonitor.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_Predefined_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
				_terminalSettingsRoot.Layout.PredefinedSplitterRatio = (float)splitContainer_Predefined.SplitterDistance / splitContainer_Predefined.Width;
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
				_terminalSettingsRoot.Implicit.Predefined.SelectedPage = predefined.SelectedPage;
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

		#region Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------

		private void sendCommand_CommandChanged(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Implicit.SendCommand.Command = sendCommand.Command;
		}

		private void sendCommand_SendCommandRequest(object sender, EventArgs e)
		{
			SendCommand();
		}

		private void sendFile_CommandChanged(object sender, EventArgs e)
		{
			_terminalSettingsRoot.Implicit.SendFile.Command = sendFile.Command;
		}

		private void sendFile_SendCommandRequest(object sender, EventArgs e)
		{
			SendFile();
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

		#region Settings
		//******************************************************************************************
		// Settings
		//******************************************************************************************

		private void AttachSettingsHandlers()
		{
			_terminalSettingsRoot.ClearChanged();
			_terminalSettingsRoot.Changed += new EventHandler<SettingsEventArgs>(_settings_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Settings Events
		//------------------------------------------------------------------------------------------

		private void _settings_Changed(object sender, SettingsEventArgs e)
		{
			SetTerminalCaption();
			if (e.Inner == null)
			{
				// SettingsRoot changed
				// nothing to do, no need to care about ProductVersion
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Explicit))
			{
				// ExplicitSettings changed
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Implicit))
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
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Terminal))
			{
				// TerminalSettings changed
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.PredefinedCommand))
			{
				// PredefinedCommandSettings changed
				_isSettingControls = true;
				predefined.Pages = _terminalSettingsRoot.PredefinedCommand.Pages;
				_isSettingControls = false;

				SetPredefinedMenuItems();        // ensure that shortcuts are activated
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Format))
			{
				// FormatSettings changed
				ReformatMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Log))
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
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.SendCommand))
			{
				// SendCommandSettings changed
				_isSettingControls = true;
				sendCommand.Command = _terminalSettingsRoot.SendCommand.Command;
				sendCommand.RecentCommands = _terminalSettingsRoot.SendCommand.RecentCommands;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.SendFile))
			{
				// SendFileSettings changed
				_isSettingControls = true;
				sendFile.Command = _terminalSettingsRoot.SendFile.Command;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Predefined))
			{
				// PredefinedSettings changed
				_isSettingControls = true;
				predefined.SelectedPage = _terminalSettingsRoot.Predefined.SelectedPage;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Window))
			{
				// WindowSettings changed
				// nothing to do, windows settings are only saved
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Layout))
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
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.IO))
			{
				// IOSettings changed
				SetIOStatus();
				SetIOControlControls();
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Buffer))
			{
				// BufferSettings changed
				ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Display))
			{
				// DisplaySettings changed
				ReloadMonitors();

				monitor_Tx.ShowCountStatus = _terminalSettingsRoot.Display.ShowCounters;
				monitor_Bidir.ShowCountStatus = _terminalSettingsRoot.Display.ShowCounters;
				monitor_Rx.ShowCountStatus = _terminalSettingsRoot.Display.ShowCounters;
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.Transmit))
			{
				// TransmitSettings changed
				ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.TextTerminal))
			{
				// TextTerminalSettings changed
				if (_terminalSettingsRoot.TerminalType == Domain.TerminalType.Text)
					ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _terminalSettingsRoot.BinaryTerminal))
			{
				// BinaryTerminalSettings changed
				if (_terminalSettingsRoot.TerminalType == Domain.TerminalType.Binary)
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

			monitor_Tx.ShowCountStatus = _terminalSettingsRoot.Display.ShowCounters;
			monitor_Bidir.ShowCountStatus = _terminalSettingsRoot.Display.ShowCounters;
			monitor_Rx.ShowCountStatus = _terminalSettingsRoot.Display.ShowCounters;
		}

		#endregion

		#region File
		//******************************************************************************************
		// File
		//******************************************************************************************

		private void SaveTerminalFile()
		{
			SaveTerminalFile(false);
		}

		private void SaveTerminalFile(bool autoSave)
		{
			if (autoSave)
			{
				DoSaveTerminalFile(true);
			}
			else
			{
				if (_terminalSettingsHandler.SettingsFilePathIsValid && !_terminalSettingsHandler.Settings.AutoSaved)
					DoSaveTerminalFile(false);
				else
					ShowSaveTerminalFileAsDialog();
			}
		}

		private void ShowSaveTerminalFileAsDialog()
		{
			SetFixedStatusText("Saving terminal as...");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save " + UserName + " As";
			sfd.Filter = ExtensionSettings.TerminalFilesFilter;
			sfd.DefaultExt = ExtensionSettings.TerminalFiles;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.TerminalFilesPath;
			sfd.FileName = UserName + "." + sfd.DefaultExt;
			if ((sfd.ShowDialog(this) == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.TerminalFilesPath = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUser();

				string autoSaveFilePathToDelete = "";
				if (_terminalSettingsRoot.AutoSaved)
					autoSaveFilePathToDelete = _terminalSettingsHandler.SettingsFilePath;

				_terminalSettingsHandler.SettingsFilePath = sfd.FileName;
				DoSaveTerminalFile(false, autoSaveFilePathToDelete);
			}
			else
			{
				ResetStatusText();
			}

			SelectSendCommandInput();
		}

		private void DoSaveTerminalFile(bool autoSave)
		{
			DoSaveTerminalFile(autoSave, "");
		}

		private void DoSaveTerminalFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			if (!autoSave)
				SetFixedStatusText("Saving terminal...");

			try
			{
				if (autoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveTerminalFileNamePrefix + Guid.ToString() + ExtensionSettings.TerminalFiles;
					if (!_terminalSettingsHandler.SettingsFilePathIsValid)
						_terminalSettingsHandler.SettingsFilePath = autoSaveFilePath;
				}
				_terminalSettingsHandler.Settings.AutoSaved = autoSave;
				_terminalSettingsHandler.Save();

				if (!autoSave)
					UserNameFromFile = _terminalSettingsHandler.SettingsFilePath;

				OnTerminalSaved(new TerminalSavedEventArgs(_terminalSettingsHandler.SettingsFilePath, autoSave));

				if (!autoSave)
					SetTimedStatusText("Terminal saved");

				// try to delete existing auto save file
				try
				{
					if (File.Exists(autoSaveFilePathToDelete))
						File.Delete(autoSaveFilePathToDelete);
				}
				catch (Exception)
				{
				}
			}
			catch (System.Xml.XmlException ex)
			{
				if (!autoSave)
				{
					SetFixedStatusText("Error saving terminal!");
					MessageBox.Show
						(
						this,
						"Unable to save file" + Environment.NewLine + _terminalSettingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					SetTimedStatusText("Terminal not saved!");
				}
			}
			SetTerminalCaption();

			SelectSendCommandInput();
		}

		private void CloseTerminalFile()
		{
			Close();
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
			WindowState = _terminalSettingsRoot.Window.State;
			if (WindowState == FormWindowState.Normal)
			{
				StartPosition = FormStartPosition.Manual;
				Location      = _terminalSettingsRoot.Window.Location;
				Size          = _terminalSettingsRoot.Window.Size;
			}
			ResumeLayout();
		}

		private void SaveWindowSettings()
		{
			_terminalSettingsRoot.Window.State = WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				_terminalSettingsRoot.Window.Location = Location;
				_terminalSettingsRoot.Window.Size = Size;
			}
		}

		private void ViewRearrange()
		{
			// simply set defaults, settings event handler will then call LayoutTerminal()
			_terminalSettingsRoot.Layout.SetDefaults();
		}

		private void LayoutTerminal()
		{
			_isSettingControls = true;
			SuspendLayout();

			// splitContainer_Predefined
			if (_terminalSettingsRoot.Layout.PredefinedPanelIsVisible)
			{
				splitContainer_Predefined.Panel2Collapsed = false;
				splitContainer_Predefined.SplitterDistance = (int)(_terminalSettingsRoot.Layout.PredefinedSplitterRatio * splitContainer_Predefined.Width);
			}
			else
			{
				splitContainer_Predefined.Panel2Collapsed = true;
			}

			// splitContainer_TxMonitor and splitContainer_RxMonitor
			// one of the panels MUST be visible, if none is visible, then bidir si shown anyway
			bool txIsVisible = _terminalSettingsRoot.Layout.TxMonitorPanelIsVisible;
			bool bidirIsVisible = _terminalSettingsRoot.Layout.BidirMonitorPanelIsVisible || (!_terminalSettingsRoot.Layout.TxMonitorPanelIsVisible && !_terminalSettingsRoot.Layout.RxMonitorPanelIsVisible);
			bool rxIsVisible = _terminalSettingsRoot.Layout.RxMonitorPanelIsVisible;

			// orientation
			Orientation orientation = _terminalSettingsRoot.Layout.MonitorOrientation;
			splitContainer_TxMonitor.Orientation = orientation;
			splitContainer_RxMonitor.Orientation = orientation;

			if (txIsVisible)
			{
				splitContainer_TxMonitor.Panel1Collapsed = false;

				int widthOrHeight = 0;
				if (orientation == Orientation.Vertical)
					widthOrHeight = splitContainer_TxMonitor.Width;
				else
					widthOrHeight = splitContainer_TxMonitor.Height;

				splitContainer_TxMonitor.SplitterDistance = (int)(_terminalSettingsRoot.Layout.TxMonitorSplitterRatio * widthOrHeight);
			}
			else
			{
				splitContainer_TxMonitor.Panel1Collapsed = true;
			}
			splitContainer_TxMonitor.Panel2Collapsed = !(bidirIsVisible || rxIsVisible);

			if (bidirIsVisible)
			{
				splitContainer_RxMonitor.Panel1Collapsed = false;

				int widthOrHeight = 0;
				if (orientation == Orientation.Vertical)
					widthOrHeight = splitContainer_RxMonitor.Width;
				else
					widthOrHeight = splitContainer_RxMonitor.Height;

				splitContainer_RxMonitor.SplitterDistance = (int)(_terminalSettingsRoot.Layout.RxMonitorSplitterRatio * widthOrHeight);
			}
			else
			{
				splitContainer_RxMonitor.Panel1Collapsed = true;
			}
			splitContainer_RxMonitor.Panel2Collapsed = !rxIsVisible;

			// splitContainer_Terminal and splitContainer_SendCommand
			if (_terminalSettingsRoot.Layout.SendCommandPanelIsVisible || _terminalSettingsRoot.Layout.SendFilePanelIsVisible)
			{
				splitContainer_Terminal.Panel2Collapsed = false;
				panel_Monitor.Padding = new System.Windows.Forms.Padding(3, 3, 1, 0);
				panel_Predefined.Padding = new System.Windows.Forms.Padding(1, 3, 3, 0);
			}
			else
			{
				splitContainer_Terminal.Panel2Collapsed = true;
				panel_Monitor.Padding = new System.Windows.Forms.Padding(3, 3, 1, 3);
				panel_Predefined.Padding = new System.Windows.Forms.Padding(1, 3, 3, 3);
			}

			if (_terminalSettingsRoot.Layout.SendCommandPanelIsVisible && _terminalSettingsRoot.Layout.SendFilePanelIsVisible)
			{
				splitContainer_Terminal.Panel2MinSize = 97;
				splitContainer_Terminal.SplitterDistance = 393;
			}
			else if (_terminalSettingsRoot.Layout.SendCommandPanelIsVisible || _terminalSettingsRoot.Layout.SendFilePanelIsVisible)
			{
				splitContainer_Terminal.Panel2MinSize = 48;
				splitContainer_Terminal.SplitterDistance = 442;
			}

			splitContainer_Send.Panel1Collapsed = !_terminalSettingsRoot.Layout.SendCommandPanelIsVisible;
			splitContainer_Send.Panel2Collapsed = !_terminalSettingsRoot.Layout.SendFilePanelIsVisible;

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

			StringBuilder sb = new StringBuilder(UserName);

			if ((_terminalSettingsRoot != null) && _terminalSettingsRoot.ExplicitHaveChanged)
				sb.Append("*");

			if (_terminalSettingsRoot != null)
			{
				if (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort)
				{
					Domain.Settings.SerialPort.SerialPortSettings s = _terminalSettingsRoot.IO.SerialPort;
					sb.Append(" - ");
					sb.Append(s.PortId.ToString());
					sb.Append(" - ");
					sb.Append(isOpen ? "Open" : "Closed");
				}
				else
				{
					Domain.Settings.Socket.SocketSettings s = _terminalSettingsRoot.IO.Socket;
					switch (_terminalSettingsRoot.IOType)
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

			if (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort)
			{
				Domain.Settings.SerialPort.SerialPortSettings s = _terminalSettingsRoot.IO.SerialPort;
				sb.Append("Serial port ");
				sb.Append(s.PortId.ToString());
				sb.Append(" (" + s.Communication.ToString() + ") is ");
				sb.Append(isOpen ? "open" : "closed");

				toolStripStatusLabel_TerminalStatus_Connection.Visible = false;
			}
			else
			{
				Domain.Settings.Socket.SocketSettings s = _terminalSettingsRoot.IO.Socket;
				switch (_terminalSettingsRoot.IOType)
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

				toolStripStatusLabel_TerminalStatus_Connection.Visible = true;
				toolStripStatusLabel_TerminalStatus_Connection.Image = (isConnected ? on : off);
			}

			toolStripStatusLabel_TerminalStatus_IOStatus.Text = sb.ToString();
		}

		private void SetIOControlControls()
		{
			bool isOpen = _terminal.IsOpen;
			bool isSerialPort = (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort);

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
					MKY.IO.Ports.SerialPortControlPins pins;
					pins = ((MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance).ControlPins;

					bool rs485Handshake = (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.RS485);

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

					bool manualHandshake = (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual);

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
				MKY.IO.Ports.SerialPortControlPins pins;
				pins = ((MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance).ControlPins;

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

				if (saveStatus)
					_terminalSettingsRoot.TerminalIsOpen = _terminal.IsOpen;

				SetTimedStatusText("Terminal opened");
				success = true;
			}
			catch (Exception ex)
			{
				SetFixedStatusText("Error opening terminal!");

				string ioText;
				if (_terminalSettingsRoot.IOType == Domain.IOType.SerialPort)
					ioText = "Port";
				else
					ioText = "Socket";

				MessageBox.Show
					(
					this,
					"Unable to open terminal:" + Environment.NewLine + Environment.NewLine +
					ex.Message + Environment.NewLine + Environment.NewLine +
					ioText + " could be in use by another process.",
					"Terminal Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
					);

				SetTimedStatusText("Terminal not opened!");
				success = false;
			}
			Cursor = Cursors.Default;

			SelectSendCommandInput();

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

				if (saveStatus)
					_terminalSettingsRoot.TerminalIsOpen = _terminal.IsOpen;

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

			Gui.Forms.TerminalSettings f = new Gui.Forms.TerminalSettings(_terminalSettingsRoot.Terminal);
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
							_terminalSettingsRoot.Terminal = s;
							_terminal = Domain.Factory.TerminalFactory.RecreateTerminal(_terminalSettingsRoot.Terminal, _terminal);
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
						_terminalSettingsRoot.Terminal = s;
						_terminal = Domain.Factory.TerminalFactory.RecreateTerminal(_terminalSettingsRoot.Terminal, _terminal);
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

			SelectSendCommandInput();
		}

		#endregion

		#region Terminal > IO Control
		//------------------------------------------------------------------------------------------
		// Terminal > IO Control
		//------------------------------------------------------------------------------------------

		private void RequestToggleRts()
		{
			if (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleRts();
				_terminalSettingsRoot.Terminal.IO.SerialPort.RtsEnabled = port.RtsEnable;
			}
		}

		private void RequestToggleDtr()
		{
			if (_terminalSettingsRoot.Terminal.IO.SerialPort.Communication.Handshake == Domain.IO.Handshake.Manual)
			{
				MKY.IO.Ports.ISerialPort port = (MKY.IO.Ports.ISerialPort)_terminal.UnderlyingIOInstance;
				port.ToggleDtr();
				_terminalSettingsRoot.Terminal.IO.SerialPort.DtrEnabled = port.DtrEnable;
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
				switch (_terminalSettingsRoot.IOType)
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
				switch (_terminalSettingsRoot.IOType)
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

		private void SelectSendCommandInput()
		{
			sendCommand.Select();
			sendCommand.SelectInput();
		}

		private void SendCommand()
		{
			SendCommand(_terminalSettingsRoot.SendCommand.Command);
			_terminalSettingsRoot.SendCommand.RecentCommands.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary
				(
				new RecentItem<Command>(new Command(_terminalSettingsRoot.SendCommand.Command))
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
			SendFile(_terminalSettingsRoot.SendFile.Command);
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
					if (ExtensionSettings.IsXmlFile(System.IO.Path.GetExtension(filePath)))
					{
						// xml
						lines = XmlReader.LinesFromXmlFile(filePath);
					}
					else if (ExtensionSettings.IsRtfFile(System.IO.Path.GetExtension(filePath)))
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
			List<PredefinedCommandPage> pages = _terminalSettingsRoot.PredefinedCommand.Pages;

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
				commands = _terminalSettingsRoot.PredefinedCommand.Pages[predefined.SelectedPage - 1].Commands;

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
			List<PredefinedCommandPage> pages = _terminalSettingsRoot.PredefinedCommand.Pages;
			if (page <= pages.Count)
			{
				bool isDefined = false;
				if (page > 0)
				{
					List<Command> commands = _terminalSettingsRoot.PredefinedCommand.Pages[page - 1].Commands;
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
			Command c = _terminalSettingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];

			if (c.IsCommand)
				SendCommand(c);
			else if (c.IsFilePath)
				SendFile(c);
		}

		private void ShowPredefinedCommandSettings(int page, int command)
		{
			PredefinedCommandSettings f = new PredefinedCommandSettings(_terminalSettingsRoot.PredefinedCommand, page, command);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_terminalSettingsRoot.PredefinedCommand = f.SettingsResult;
				_terminalSettingsRoot.Predefined.SelectedPage = f.SelectedPage;
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

			toolStripComboBox_TerminalMenu_View_Panels_Orientation.Items.Clear();
			toolStripComboBox_TerminalMenu_View_Panels_Orientation.Items.Add(Orientation.Vertical);
			toolStripComboBox_TerminalMenu_View_Panels_Orientation.Items.Add(Orientation.Horizontal);
			toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedIndex = 0;

			toolStripComboBox_MonitorContextMenu_Orientation.Items.Clear();
			toolStripComboBox_MonitorContextMenu_Orientation.Items.Add(Orientation.Vertical);
			toolStripComboBox_MonitorContextMenu_Orientation.Items.Add(Orientation.Horizontal);
			toolStripComboBox_MonitorContextMenu_Orientation.SelectedIndex = 0;

			_isSettingControls = false;
		}

		private void SetMonitorRadix(Domain.Radix radix)
		{
			_terminalSettingsRoot.Display.Radix = radix;
		}

		private void SetMonitorOrientation(Orientation orientation)
		{
			_terminalSettingsRoot.Layout.MonitorOrientation = orientation;

			SuspendLayout();
			splitContainer_TxMonitor.Orientation = orientation;
			splitContainer_RxMonitor.Orientation = orientation;
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

			monitor_Tx.FormatSettings = _terminalSettingsRoot.Format;
			monitor_Bidir.FormatSettings = _terminalSettingsRoot.Format;
			monitor_Rx.FormatSettings = _terminalSettingsRoot.Format;

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
			Gui.Forms.FormatSettings f = new Gui.Forms.FormatSettings(_terminalSettingsRoot.Format);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_terminalSettingsRoot.Format = f.SettingsResult;
			}
		}

		private void ShowSaveMonitorDialog(Controls.Monitor monitor)
		{
			SetFixedStatusText("Saving monitor as...");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save As";
			sfd.Filter = ExtensionSettings.TextFilesFilter;
			sfd.DefaultExt = ExtensionSettings.TextFilesDefault;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.MonitorFilesPath;
			if (sfd.ShowDialog(this) == DialogResult.OK && sfd.FileName.Length > 0)
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.MonitorFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUser();

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
				if (ExtensionSettings.IsXmlFile(System.IO.Path.GetExtension(filePath)))
					XmlWriter.LinesToXmlFile(monitor.SelectedLines, filePath);
				else if (ExtensionSettings.IsRtfFile(System.IO.Path.GetExtension(filePath)))
					RtfWriter.LinesToRtfFile(monitor.SelectedLines, filePath, _terminalSettingsRoot.Format, RichTextBoxStreamType.RichText);
				else
					RtfWriter.LinesToRtfFile(monitor.SelectedLines, filePath, _terminalSettingsRoot.Format, RichTextBoxStreamType.PlainText);

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
			RtfWriter.LinesToClipboard(monitor.SelectedLines, _terminalSettingsRoot.Format);
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
				printer.Print(RtfWriter.LinesToRichTextBox(monitor.SelectedLines, _terminalSettingsRoot.Format));
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
			bool logSelected = _terminalSettingsRoot.Log.AnyRawOrNeat;
			bool logOpen = _terminalSettingsRoot.LogIsOpen;

			toolStripMenuItem_TerminalMenu_Log_Begin.Enabled = logSelected && !logOpen;
			toolStripMenuItem_TerminalMenu_Log_End.Enabled = logSelected && logOpen;
			toolStripMenuItem_TerminalMenu_Log_Clear.Enabled = logSelected && logOpen;
		}

		private void ShowLogSettings()
		{
			Gui.Forms.LogSettings f = new Gui.Forms.LogSettings(_terminalSettingsRoot.Log);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_terminalSettingsRoot.Log = f.SettingsResult;
				_log.Settings = _terminalSettingsRoot.Log;
			}

			SelectSendCommandInput();
		}

		private void BeginLog()
		{
			try
			{
				// reapply setting NOW, makes sure that date/time in filenames is refreshed
				_log.Settings = _terminalSettingsRoot.Log;
				_log.Begin();
				_terminalSettingsRoot.LogIsOpen = true;
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
			EndLog(true);
		}

		private void EndLog(bool saveStatus)
		{
			try
			{
				_log.End();

				if (saveStatus)
					_terminalSettingsRoot.LogIsOpen = false;
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
			EventHelper.FireSync(TerminalChanged, this, e);
		}

		protected virtual void OnTerminalSaved(TerminalSavedEventArgs e)
		{
			EventHelper.FireSync<TerminalSavedEventArgs>(TerminalSaved, this, e);
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
