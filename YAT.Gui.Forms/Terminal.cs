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

namespace YAT.Gui.Forms
{
	public partial class Terminal : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// startup/update
		private bool _isStartingUp = true;
		private bool _isSettingControls = false;
		private bool _isClosingFromForm = false;
		private bool _isClosingFromModel = false;

		// MDI
		private Form _mdiParent;

        // preset
        private List<ToolStripMenuItem> _menuItems_preset;

		// predefined
		private List<ToolStripMenuItem> _menuItems_predefined;

		// terminal
		private Model.Terminal _terminal;

		// settings
		private TerminalSettingsRoot _settingsRoot;
		private bool _handlingTerminalSettingsIsSuspended = false;

		// status
		private const string _DefaultStatusText = "";
		private const int _TimedStatusInterval = 2000;
		private const int _RtsLuminescenceInterval = 150;
		private List<ToolStripStatusLabel> _statusLabels_ioControl;

		#endregion

		#region Events
		//==========================================================================================
		// Events
		//==========================================================================================

		/// <summary></summary>
		public event EventHandler Changed;
		/// <summary></summary>
		public event EventHandler<Model.SavedEventArgs> Saved;

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		// Important note to ensure proper z-order of this form:
		// - Within visual designer, proceed in the following order
		//     1. Place "menuStrip_Terminal" and dock it to top
		//     2. Place "statusStrip_Terminal" and dock it to bottom
		//     3. Place "splitContainer_Terminal" and dock it to fill

		public Terminal()
		{
			InitializeComponent();
			Initialize(new Model.Terminal());
		}

		public Terminal(Model.Terminal terminal)
		{
			InitializeComponent();
			Initialize(terminal);
		}

		private void Initialize(Model.Terminal terminal)
		{
			FixContextMenus();

            InitializePresetMenuItems();
			InitializePredefinedMenuItems();
			InitializeMonitorMenuItems();
			InitializeIOControlStatusLabels();

			// link and attach to terminal model
			_terminal = terminal;
			AttachTerminalEventHandlers();

			// link and attach to terminal settings
			_settingsRoot = _terminal.SettingsRoot;
			AttachSettingsEventHandlers();

			ApplyWindowSettings();
			LayoutTerminal();

			// force settings changed event to set all controls
			// for improved performance, manually suspend/resume handler for terminal settings
			SuspendHandlingTerminalSettings();
			_settingsRoot.ClearChanged();
			_settingsRoot.ForceChangeEvent();
			ResumeHandlingTerminalSettings();
		}

		#endregion

		#region MDI Parent
		//******************************************************************************************
		// MDI Parent
		//******************************************************************************************

		#region MDI Parent > Properties
		//------------------------------------------------------------------------------------------
		// MDI Parent > Properties
		//------------------------------------------------------------------------------------------

		public string UserName
		{
			get { return (_terminal.UserName); }
		}

		public bool IsOpen
		{
			get { return (_terminal.IsOpen); }
		}

		public Model.Terminal UnderlyingTerminal
		{
			get { return (_terminal); }
		}

		#endregion

		#region MDI Parent > Methods
		//------------------------------------------------------------------------------------------
		// MDI Parent > Methods
		//------------------------------------------------------------------------------------------

		public bool RequestSaveFile()
		{
			return (_terminal.Save());
		}

		public bool RequestCloseFile()
		{
			return (_terminal.Close());
		}

		public bool RequestOpenTerminal()
		{
			return (_terminal.OpenIO());
		}

		public bool RequestCloseTerminal()
		{
			return (_terminal.CloseIO());
		}

		public void RequestEditTerminalSettings()
		{
			ShowTerminalSettings();
		}

		#endregion

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

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

				// then start terminal
				_terminal.Start();
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

		/// <remarks>
		/// Attention:
		/// In case of MDI parent/application closing, this FormClosing event is called before
		/// the FormClosing event of the MDI parent. Therefore, this MDI child has to handle
		/// such events differently, i.e. auto save the terminal but only in case of non-user
		/// closing.
		/// </remarks>
		private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
		{
			// prevent multiple calls to Close()
			if (!_isClosingFromModel)
			{
				_isClosingFromForm = true;
				if (e.CloseReason == CloseReason.UserClosing)
					e.Cancel = (!_terminal.Close());
				else
					e.Cancel = (!_terminal.Close(true));
			}
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

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
			_terminal.Save();
		}

		private void toolStripMenuItem_TerminalMenu_File_SaveAs_Click(object sender, EventArgs e)
		{
			ShowSaveTerminalAsFileDialog();
		}

		private void toolStripMenuItem_TerminalMenu_File_Close_Click(object sender, EventArgs e)
		{
			_terminal.Close();
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
			_terminal.OpenIO();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Close_Click(object sender, EventArgs e)
		{
			_terminal.CloseIO();
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
			toolStripMenuItem_TerminalMenu_Send_Command.Enabled = _settingsRoot.SendCommand.Command.IsValidCommand;
			toolStripMenuItem_TerminalMenu_Send_File.Enabled = _settingsRoot.SendCommand.Command.IsValidFilePath;
		}

		private void toolStripMenuItem_TerminalMenu_Send_Command_Click(object sender, EventArgs e)
		{
			_terminal.SendCommand();
		}

		private void toolStripMenuItem_TerminalMenu_Send_File_Click(object sender, EventArgs e)
		{
			_terminal.SendFile();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Log
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Log
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_TerminalMenu_Log_DropDownOpening(object sender, EventArgs e)
		{
			bool logIsOpen = _terminal.LogIsOpen;
			toolStripMenuItem_TerminalMenu_Log_Begin.Enabled = !logIsOpen;
			toolStripMenuItem_TerminalMenu_Log_End.Enabled = logIsOpen;
		}

		private void toolStripMenuItem_TerminalMenu_Log_Begin_Click(object sender, EventArgs e)
		{
			_terminal.BeginLog();
		}

		private void toolStripMenuItem_TerminalMenu_Log_End_Click(object sender, EventArgs e)
		{
			_terminal.EndLog();
		}

		private void toolStripMenuItem_TerminalMenu_Log_Clear_Click(object sender, EventArgs e)
		{
			_terminal.ClearLog();
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
			Domain.TerminalType terminalType = _settingsRoot.TerminalType;

			// panels
			toolStripMenuItem_TerminalMenu_View_Panels_Tx.Checked = _settingsRoot.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = _settingsRoot.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_Rx.Checked = _settingsRoot.Layout.RxMonitorPanelIsVisible;

			// disable monitor item if the other monitors are hidden
			toolStripMenuItem_TerminalMenu_View_Panels_Tx.Enabled = (_settingsRoot.Layout.BidirMonitorPanelIsVisible || _settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Enabled = (_settingsRoot.Layout.TxMonitorPanelIsVisible || _settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Panels_Rx.Enabled = (_settingsRoot.Layout.TxMonitorPanelIsVisible || _settingsRoot.Layout.BidirMonitorPanelIsVisible);

			toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedItem = _settingsRoot.Layout.MonitorOrientation;

			toolStripMenuItem_TerminalMenu_View_Panels_SendCommand.Checked = _settingsRoot.Layout.SendCommandPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked = _settingsRoot.Layout.SendFilePanelIsVisible;

			toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = _settingsRoot.Layout.PredefinedPanelIsVisible;

			// counters
			toolStripMenuItem_TerminalMenu_View_Counters_ShowCounters.Checked = _settingsRoot.Display.ShowCounters;
			toolStripMenuItem_TerminalMenu_View_Counters_ResetCounters.Enabled = _settingsRoot.Display.ShowCounters;

			// options
			toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Checked = _settingsRoot.Display.ShowTimeStamp;
			toolStripMenuItem_TerminalMenu_View_ShowLength.Checked = _settingsRoot.Display.ShowLength;
			
			bool enabled = (terminalType == Domain.TerminalType.Text);
			toolStripMenuItem_TerminalMenu_View_ShowEol.Enabled = enabled;
			toolStripMenuItem_TerminalMenu_View_ShowEol.Checked = enabled && _settingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Tx_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.TxMonitorPanelIsVisible = !_settingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Bidir_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.BidirMonitorPanelIsVisible = !_settingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Rx_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.RxMonitorPanelIsVisible = !_settingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripComboBox_TerminalMenu_View_Panels_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!_isSettingControls)
				SetMonitorOrientation((Orientation)toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Predefined_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.PredefinedPanelIsVisible = !_settingsRoot.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_SendCommand_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.SendCommandPanelIsVisible = !_settingsRoot.Layout.SendCommandPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_SendFile_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.SendFilePanelIsVisible = !_settingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Rearrange_Click(object sender, EventArgs e)
		{
			ViewRearrange();
		}

		private void toolStripMenuItem_TerminalMenu_View_Counters_ShowCounters_Click(object sender, EventArgs e)
		{
			_settingsRoot.Display.ShowCounters = !_settingsRoot.Display.ShowCounters;
		}

		private void toolStripMenuItem_TerminalMenu_View_Counters_ResetCounters_Click(object sender, EventArgs e)
		{
			ClearCountStatus();
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTimeStamp_Click(object sender, EventArgs e)
		{
			_settingsRoot.Display.ShowTimeStamp = !_settingsRoot.Display.ShowTimeStamp;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowLength_Click(object sender, EventArgs e)
		{
			_settingsRoot.Display.ShowLength = !_settingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowEol_Click(object sender, EventArgs e)
		{
			_settingsRoot.TextTerminal.ShowEol = !_settingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_TerminalMenu_View_Format_Click(object sender, EventArgs e)
		{
			ShowFormatSettings();
		}

		#endregion

		#endregion

        #region Controls Event Handlers > Preset Context Menu
        //------------------------------------------------------------------------------------------
        // Controls Event Handlers > Preset Context Menu
        //------------------------------------------------------------------------------------------

        private void contextMenuStrip_Preset_Opening(object sender, CancelEventArgs e)
        {
            SetPresetMenuItems();
        }

        private void toolStripMenuItem_PresetContextMenu_Preset_Click(object sender, EventArgs e)
        {
            RequestPreset(int.Parse((string)(((ToolStripMenuItem)sender).Tag)));
        }

        #endregion

        #region Controls Event Handlers > Monitor Context Menu
        //------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Monitor_Opening(object sender, CancelEventArgs e)
		{
			Domain.TerminalType terminalType = _settingsRoot.TerminalType;
			Domain.RepositoryType monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			bool isMonitor = (monitorType != Domain.RepositoryType.None);

			toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Checked = _settingsRoot.Display.ShowTimeStamp;
			toolStripMenuItem_MonitorContextMenu_ShowLength.Checked = _settingsRoot.Display.ShowLength;

			bool enabled = (terminalType == Domain.TerminalType.Text);
			toolStripMenuItem_MonitorContextMenu_ShowEol.Enabled = enabled;
			toolStripMenuItem_MonitorContextMenu_ShowEol.Checked = enabled && _settingsRoot.TextTerminal.ShowEol;

			toolStripMenuItem_MonitorContextMenu_Clear.Enabled = isMonitor;

			toolStripMenuItem_MonitorContextMenu_ShowCounters.Checked = _settingsRoot.Display.ShowCounters;
			toolStripMenuItem_MonitorContextMenu_ResetCounters.Enabled = _settingsRoot.Display.ShowCounters;

			toolStripMenuItem_MonitorContextMenu_Panels_Tx.Checked = _settingsRoot.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Checked = _settingsRoot.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_Panels_Rx.Checked = _settingsRoot.Layout.RxMonitorPanelIsVisible;

			// disable "Monitor" item if the other monitors are hidden
			toolStripMenuItem_MonitorContextMenu_Panels_Tx.Enabled = (_settingsRoot.Layout.BidirMonitorPanelIsVisible || _settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Enabled = (_settingsRoot.Layout.TxMonitorPanelIsVisible || _settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_Panels_Rx.Enabled = (_settingsRoot.Layout.TxMonitorPanelIsVisible || _settingsRoot.Layout.BidirMonitorPanelIsVisible);

			// hide "Hide" item if only this monitor is visible
			bool hideIsAllowed = false;
			switch (monitorType)
			{
				case Domain.RepositoryType.Tx:    hideIsAllowed = (_settingsRoot.Layout.BidirMonitorPanelIsVisible || _settingsRoot.Layout.RxMonitorPanelIsVisible); break;
				case Domain.RepositoryType.Bidir: hideIsAllowed = (_settingsRoot.Layout.TxMonitorPanelIsVisible || _settingsRoot.Layout.RxMonitorPanelIsVisible); break;
				case Domain.RepositoryType.Rx:    hideIsAllowed = (_settingsRoot.Layout.TxMonitorPanelIsVisible || _settingsRoot.Layout.BidirMonitorPanelIsVisible); break;
			}
			toolStripMenuItem_MonitorContextMenu_Hide.Visible = hideIsAllowed;
			toolStripMenuItem_MonitorContextMenu_Hide.Enabled = isMonitor && hideIsAllowed;

			toolStripMenuItem_MonitorContextMenu_SaveToFile.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_Print.Enabled = isMonitor;
		}

		private void toolStripMenuItem_MonitorContextMenu_Format_Click(object sender, EventArgs e)
		{
			ShowFormatSettings();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTimeStamp_Click(object sender, EventArgs e)
		{
			_settingsRoot.Display.ShowTimeStamp = !_settingsRoot.Display.ShowTimeStamp;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLength_Click(object sender, EventArgs e)
		{
			_settingsRoot.Display.ShowLength = !_settingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowEol_Click(object sender, EventArgs e)
		{
			_settingsRoot.TextTerminal.ShowEol = !_settingsRoot.TextTerminal.ShowEol;
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
			_settingsRoot.Display.ShowCounters = !_settingsRoot.Display.ShowCounters;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetCounters_Click(object sender, EventArgs e)
		{
			ClearCountStatus();
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Tx_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.TxMonitorPanelIsVisible = !_settingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Bidir_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.BidirMonitorPanelIsVisible = !_settingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Rx_Click(object sender, EventArgs e)
		{
			_settingsRoot.Layout.RxMonitorPanelIsVisible = !_settingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Hide_Click(object sender, EventArgs e)
		{
			switch (GetMonitorType(contextMenuStrip_Monitor.SourceControl))
			{
				case Domain.RepositoryType.Tx:    _settingsRoot.Layout.TxMonitorPanelIsVisible    = false; break;
				case Domain.RepositoryType.Bidir: _settingsRoot.Layout.BidirMonitorPanelIsVisible = false; break;
				case Domain.RepositoryType.Rx:    _settingsRoot.Layout.RxMonitorPanelIsVisible    = false; break;
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

		#region Controls Event Handlers > Radix Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Radix Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Radix_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_RadixContextMenu_String.Checked = (_settingsRoot.Display.Radix == Domain.Radix.String);
			toolStripMenuItem_RadixContextMenu_Char.Checked = (_settingsRoot.Display.Radix == Domain.Radix.Char);
			toolStripMenuItem_RadixContextMenu_Bin.Checked = (_settingsRoot.Display.Radix == Domain.Radix.Bin);
			toolStripMenuItem_RadixContextMenu_Oct.Checked = (_settingsRoot.Display.Radix == Domain.Radix.Oct);
			toolStripMenuItem_RadixContextMenu_Dec.Checked = (_settingsRoot.Display.Radix == Domain.Radix.Dec);
			toolStripMenuItem_RadixContextMenu_Hex.Checked = (_settingsRoot.Display.Radix == Domain.Radix.Hex);
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
            _settingsRoot.Layout.PredefinedPanelIsVisible = false;
        }

        #endregion

        #region Controls Event Handlers > Send Context Menu
        //------------------------------------------------------------------------------------------
        // Controls Event Handlers > Send Context Menu
        //------------------------------------------------------------------------------------------

        private void contextMenuStrip_Send_Opening(object sender, CancelEventArgs e)
        {
            toolStripMenuItem_SendContextMenu_SendCommand.Enabled = _settingsRoot.SendCommand.Command.IsValidCommand;
            toolStripMenuItem_SendContextMenu_SendFile.Enabled = _settingsRoot.SendCommand.Command.IsValidFilePath;

            toolStripMenuItem_SendContextMenu_Panels_SendCommand.Checked = _settingsRoot.Layout.SendCommandPanelIsVisible;
            toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = _settingsRoot.Layout.SendFilePanelIsVisible;
        }

        private void toolStripMenuItem_SendContextMenu_SendCommand_Click(object sender, EventArgs e)
        {
            _terminal.SendCommand();
        }

        private void toolStripMenuItem_SendContextMenu_SendFile_Click(object sender, EventArgs e)
        {
			_terminal.SendFile();
        }

        private void toolStripMenuItem_SendContextMenu_Panels_SendCommand_Click(object sender, EventArgs e)
        {
            _settingsRoot.Layout.SendCommandPanelIsVisible = !_settingsRoot.Layout.SendCommandPanelIsVisible;
        }

        private void toolStripMenuItem_SendContextMenu_Panels_SendFile_Click(object sender, EventArgs e)
        {
            _settingsRoot.Layout.SendFilePanelIsVisible = !_settingsRoot.Layout.SendFilePanelIsVisible;
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
				if (_settingsRoot.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_TxMonitor.Width;
				else
					widthOrHeight = splitContainer_TxMonitor.Height;

				_settingsRoot.Layout.TxMonitorSplitterRatio = (float)splitContainer_TxMonitor.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_RxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
			{
				int widthOrHeight = 0;
				if (_settingsRoot.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_RxMonitor.Width;
				else
					widthOrHeight = splitContainer_RxMonitor.Height;

				_settingsRoot.Layout.RxMonitorSplitterRatio = (float)splitContainer_RxMonitor.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_Predefined_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!_isStartingUp && !_isSettingControls)
				_settingsRoot.Layout.PredefinedSplitterRatio = (float)splitContainer_Predefined.SplitterDistance / splitContainer_Predefined.Width;
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
				_settingsRoot.Implicit.Predefined.SelectedPage = predefined.SelectedPage;
		}

		private void predefined_SendCommandRequest(object sender, Model.Types.PredefinedCommandEventArgs e)
		{
			SendPredefined(e.Page, e.Command);
		}

		private void predefined_DefineCommandRequest(object sender, Model.Types.PredefinedCommandEventArgs e)
		{
			ShowPredefinedCommandSettings(e.Page, e.Command);
		}

		#endregion

		#region Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------

		private void send_CommandChanged(object sender, EventArgs e)
		{
			_settingsRoot.Implicit.SendCommand.Command = send.Command;
		}

		private void send_SendCommandRequest(object sender, EventArgs e)
		{
			_terminal.SendCommand();
		}

		private void send_FileCommandChanged(object sender, EventArgs e)
		{
			_settingsRoot.Implicit.SendFile.Command = send.FileCommand;
		}

		private void send_SendFileCommandRequest(object sender, EventArgs e)
		{
			_terminal.SendFile();
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
			_terminal.RequestToggleRts();
		}

		private void toolStripStatusLabel_TerminalStatus_DTR_Click(object sender, EventArgs e)
		{
			_terminal.RequestToggleDtr();
		}

		#endregion

		#endregion

		#region View
		//==========================================================================================
		// View
		//==========================================================================================

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
			WindowState = _settingsRoot.Window.State;
			if (WindowState == FormWindowState.Normal)
			{
				StartPosition = FormStartPosition.Manual;
				Location      = _settingsRoot.Window.Location;
				Size          = _settingsRoot.Window.Size;
			}
			ResumeLayout();
		}

		private void SaveWindowSettings()
		{
			_settingsRoot.Window.State = WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				_settingsRoot.Window.Location = Location;
				_settingsRoot.Window.Size = Size;
			}
		}

		private void ViewRearrange()
		{
			// simply set defaults, settings event handler will then call LayoutTerminal()
			_settingsRoot.Layout.SetDefaults();
		}

		private void LayoutTerminal()
		{
			_isSettingControls = true;
			SuspendLayout();

			// splitContainer_Predefined
			if (_settingsRoot.Layout.PredefinedPanelIsVisible)
			{
				splitContainer_Predefined.Panel2Collapsed = false;
				splitContainer_Predefined.SplitterDistance = (int)(_settingsRoot.Layout.PredefinedSplitterRatio * splitContainer_Predefined.Width);
			}
			else
			{
				splitContainer_Predefined.Panel2Collapsed = true;
			}

			// splitContainer_TxMonitor and splitContainer_RxMonitor
			// one of the panels MUST be visible, if none is visible, then bidir is shown anyway
			bool txIsVisible = _settingsRoot.Layout.TxMonitorPanelIsVisible;
			bool bidirIsVisible = _settingsRoot.Layout.BidirMonitorPanelIsVisible || (!_settingsRoot.Layout.TxMonitorPanelIsVisible && !_settingsRoot.Layout.RxMonitorPanelIsVisible);
			bool rxIsVisible = _settingsRoot.Layout.RxMonitorPanelIsVisible;

			// orientation
			Orientation orientation = _settingsRoot.Layout.MonitorOrientation;
			splitContainer_TxMonitor.Orientation = orientation;
			splitContainer_RxMonitor.Orientation = orientation;

			// Tx split contains Tx and BiDir&Rx
			if (txIsVisible)
			{
				splitContainer_TxMonitor.Panel1Collapsed = false;

				if (bidirIsVisible || rxIsVisible)
				{
					int widthOrHeight = 0;
					if (orientation == Orientation.Vertical)
						widthOrHeight = splitContainer_TxMonitor.Width;
					else
						widthOrHeight = splitContainer_TxMonitor.Height;

					splitContainer_TxMonitor.SplitterDistance = (int)(_settingsRoot.Layout.TxMonitorSplitterRatio * widthOrHeight);
				}
			}
			else
			{
				splitContainer_TxMonitor.Panel1Collapsed = true;
			}
			splitContainer_TxMonitor.Panel2Collapsed = !(bidirIsVisible || rxIsVisible);

			// Rx split contains BiDir and Rx
			if (bidirIsVisible)
			{
				splitContainer_RxMonitor.Panel1Collapsed = false;

				if (rxIsVisible)
				{
					int widthOrHeight = 0;
					if (orientation == Orientation.Vertical)
						widthOrHeight = splitContainer_RxMonitor.Width;
					else
						widthOrHeight = splitContainer_RxMonitor.Height;

					splitContainer_RxMonitor.SplitterDistance = (int)(_settingsRoot.Layout.RxMonitorSplitterRatio * widthOrHeight);
				}
			}
			else
			{
				splitContainer_RxMonitor.Panel1Collapsed = true;
			}
			splitContainer_RxMonitor.Panel2Collapsed = !rxIsVisible;

			// splitContainer_Terminal and splitContainer_SendCommand
			if (_settingsRoot.Layout.SendCommandPanelIsVisible || _settingsRoot.Layout.SendFilePanelIsVisible)
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

			// set send panel size depending on one or two sub-panels
			if (_settingsRoot.Layout.SendCommandPanelIsVisible && _settingsRoot.Layout.SendFilePanelIsVisible)
			{
				int height = 97;
				splitContainer_Terminal.Panel2MinSize = height;
				splitContainer_Terminal.SplitterDistance = splitContainer_Terminal.Height - height - splitContainer_Terminal.SplitterWidth;
			}
			else if (_settingsRoot.Layout.SendCommandPanelIsVisible || _settingsRoot.Layout.SendFilePanelIsVisible)
			{
				int height = 48;
				splitContainer_Terminal.Panel2MinSize = height;
				splitContainer_Terminal.SplitterDistance = splitContainer_Terminal.Height - height - splitContainer_Terminal.SplitterWidth;
			}

            send.CommandPanelIsVisible = _settingsRoot.Layout.SendCommandPanelIsVisible;
            send.FilePanelIsVisible = _settingsRoot.Layout.SendFilePanelIsVisible;
            send.SplitterRatio = _settingsRoot.Layout.PredefinedSplitterRatio;

			ResumeLayout();
			_isSettingControls = false;
		}

		#endregion

		#region Preset
		//==========================================================================================
		// Preset
		//==========================================================================================

		private void InitializePresetMenuItems()
		{
			_menuItems_preset = new List<ToolStripMenuItem>();
			_menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_1);
			_menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_2);
			_menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_3);
			_menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_4);
			_menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_5);
			_menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_6);
		}

		private void SetPresetMenuItems()
		{
			bool isSerialPort = (_settingsRoot.IOType == Domain.IOType.SerialPort);

			foreach (ToolStripMenuItem item in _menuItems_preset)
				item.Enabled = isSerialPort;
		}

		/// <summary>
		/// Set requested preset. Currently, presets are fixed to those listed below.
		/// For future versions, presets could be defined and managed similay to predefined
		/// commands.
		/// </summary>
		private void RequestPreset(int preset)
		{
			string presetString = "";
			switch (preset)
			{
				case 1:	presetString =  "2400, 7, Even, 1, None";     break;
				case 2:	presetString =  "2400, 7, Even, 1, XOn/XOff"; break;
				case 3:	presetString =  "9600, 8, None, 1, None";     break;
				case 4:	presetString =  "9600, 8, None, 1, XOn/XOff"; break;
				case 5:	presetString = "19200, 8, None, 1, None";     break;
				case 6:	presetString = "19200, 8, None, 1, XOn/XOff"; break;
			}

			Domain.Settings.SerialPort.SerialCommunicationSettings settings = _settingsRoot.Terminal.IO.SerialPort.Communication;
			settings.SuspendChangeEvent();
			switch (preset)
			{
				case 1: // "2400, 7, Even, 1, None"
				{
					settings.BaudRate  = (MKY.IO.Ports.XBaudRate)MKY.IO.Ports.BaudRate.Baud002400;
					settings.DataBits  = MKY.IO.Ports.DataBits.Seven;
					settings.Parity    = System.IO.Ports.Parity.Even;
					settings.StopBits  = System.IO.Ports.StopBits.One;
					settings.FlowControl = Domain.IO.FlowControl.None;
					break;
				}
				case 2: // "2400, 7, Even, 1, XOn/XOff"
				{
					settings.BaudRate  = (MKY.IO.Ports.XBaudRate)MKY.IO.Ports.BaudRate.Baud002400;
					settings.DataBits  = MKY.IO.Ports.DataBits.Seven;
					settings.Parity    = System.IO.Ports.Parity.Even;
					settings.StopBits  = System.IO.Ports.StopBits.One;
					settings.FlowControl = Domain.IO.FlowControl.XOnXOff;
					break;
				}
				case 3: // "9600, 8, None, 1, None"
				{
					settings.BaudRate  = (MKY.IO.Ports.XBaudRate)MKY.IO.Ports.BaudRate.Baud009600;
					settings.DataBits  = MKY.IO.Ports.DataBits.Eight;
					settings.Parity    = System.IO.Ports.Parity.None;
					settings.StopBits  = System.IO.Ports.StopBits.One;
					settings.FlowControl = Domain.IO.FlowControl.None;
					break;
				}
				case 4: // "9600, 8, None, 1, XOn/XOff"
				{
					settings.BaudRate  = (MKY.IO.Ports.XBaudRate)MKY.IO.Ports.BaudRate.Baud009600;
					settings.DataBits  = MKY.IO.Ports.DataBits.Eight;
					settings.Parity    = System.IO.Ports.Parity.None;
					settings.StopBits  = System.IO.Ports.StopBits.One;
					settings.FlowControl = Domain.IO.FlowControl.XOnXOff;
					break;
				}
				case 5: // "19200, 8, None, 1, None"
				{
					settings.BaudRate  = (MKY.IO.Ports.XBaudRate)MKY.IO.Ports.BaudRate.Baud019200;
					settings.DataBits  = MKY.IO.Ports.DataBits.Eight;
					settings.Parity    = System.IO.Ports.Parity.None;
					settings.StopBits  = System.IO.Ports.StopBits.One;
					settings.FlowControl = Domain.IO.FlowControl.None;
					break;
				}
				case 6: // "19200, 8, None, 1, XOn/XOff"
				{
					settings.BaudRate  = (MKY.IO.Ports.XBaudRate)MKY.IO.Ports.BaudRate.Baud019200;
					settings.DataBits  = MKY.IO.Ports.DataBits.Eight;
					settings.Parity    = System.IO.Ports.Parity.None;
					settings.StopBits  = System.IO.Ports.StopBits.One;
					settings.FlowControl = Domain.IO.FlowControl.XOnXOff;
					break;
				}
			}
			settings.ResumeChangeEvent(true);

			SetTimedStatusText("Terminal settings set to " + presetString + ".");
		}

		#endregion

		#region Monitor Panels
		//==========================================================================================
		// Monitor Panels
		//==========================================================================================

		#region Monitor Panels > Access
		//------------------------------------------------------------------------------------------
		// Monitor Panels > Access
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

		#region Monitor Panels > View
		//------------------------------------------------------------------------------------------
		// Monitor Panels > View
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
			_settingsRoot.Display.Radix = radix;
		}

		private void SetMonitorOrientation(Orientation orientation)
		{
			_settingsRoot.Layout.MonitorOrientation = orientation;

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

			monitor_Tx.FormatSettings    = _settingsRoot.Format;
			monitor_Bidir.FormatSettings = _settingsRoot.Format;
			monitor_Rx.FormatSettings    = _settingsRoot.Format;

			monitor_Tx.Reload   (_terminal.RepositoryToDisplayElements(Domain.RepositoryType.Tx));
			monitor_Bidir.Reload(_terminal.RepositoryToDisplayElements(Domain.RepositoryType.Bidir));
			monitor_Rx.Reload   (_terminal.RepositoryToDisplayElements(Domain.RepositoryType.Rx));

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
			_terminal.ResetCount();
		}

		#endregion

		#region Monitor Panels > Methods
		//------------------------------------------------------------------------------------------
		// Monitor Panels > Methods
		//------------------------------------------------------------------------------------------

		private void ShowFormatSettings()
		{
			Gui.Forms.FormatSettings f = new Gui.Forms.FormatSettings(_settingsRoot.Format);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_settingsRoot.Format = f.SettingsResult;
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
					Model.Utilities.XmlWriter.LinesToXmlFile(monitor.SelectedLines, filePath);
				else if (ExtensionSettings.IsRtfFile(System.IO.Path.GetExtension(filePath)))
					Model.Utilities.RtfWriter.LinesToRtfFile(monitor.SelectedLines, filePath, _settingsRoot.Format, RichTextBoxStreamType.RichText);
				else
					Model.Utilities.RtfWriter.LinesToRtfFile(monitor.SelectedLines, filePath, _settingsRoot.Format, RichTextBoxStreamType.PlainText);

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
			Model.Utilities.RtfWriter.LinesToClipboard(monitor.SelectedLines, _settingsRoot.Format);
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
				Model.Utilities.RtfPrinter printer = new Model.Utilities.RtfPrinter(settings);
				printer.Print(Model.Utilities.RtfWriter.LinesToRichTextBox(monitor.SelectedLines, _settingsRoot.Format));
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

		#region Predefined Panel
		//==========================================================================================
		// Predefined Panel
		//==========================================================================================

		private void InitializePredefinedMenuItems()
		{
			_menuItems_predefined = new List<ToolStripMenuItem>(Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage);
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
			List<Model.Types.PredefinedCommandPage> pages = _settingsRoot.PredefinedCommand.Pages;

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
			List<Model.Types.Command> commands = null;
			if ((pages != null) && (pages.Count > 0))
				commands = _settingsRoot.PredefinedCommand.Pages[predefined.SelectedPage - 1].Commands;

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
					_menuItems_predefined[i].Text = Model.Types.Command.UndefinedCommandText;
					_menuItems_predefined[i].Enabled = true;
				}
			}
			for (int i = commandCount; i < Model.Settings.PredefinedCommandSettings.MaximumCommandsPerPage; i++)
			{
				_menuItems_predefined[i].Text = Model.Types.Command.UndefinedCommandText;
				_menuItems_predefined[i].Enabled = true;
			}
		}

		private void RequestPredefined(int page, int command)
		{
			List<Model.Types.PredefinedCommandPage> pages = _settingsRoot.PredefinedCommand.Pages;
			if (page <= pages.Count)
			{
				bool isDefined = false;
				if (page > 0)
				{
					List<Model.Types.Command> commands = _settingsRoot.PredefinedCommand.Pages[page - 1].Commands;
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
			Model.Types.Command c = _settingsRoot.PredefinedCommand.Pages[page - 1].Commands[command - 1];

			if (c.IsCommand)
				_terminal.SendCommand(c);
			else if (c.IsFilePath)
				_terminal.SendFile(c);
		}

		private void ShowPredefinedCommandSettings(int page, int command)
		{
			PredefinedCommandSettings f = new PredefinedCommandSettings(_settingsRoot.PredefinedCommand, page, command);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_settingsRoot.PredefinedCommand = f.SettingsResult;
				_settingsRoot.Predefined.SelectedPage = f.SelectedPage;
			}
		}

		#endregion

		#region Send Panel
		//==========================================================================================
		// Send Panel
		//==========================================================================================

		private void SelectSendCommandInput()
		{
			send.SelectSendCommandInput();
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		#region Settings > Lifetime
		//------------------------------------------------------------------------------------------
		// Settings > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachSettingsEventHandlers()
		{
			_settingsRoot.Changed += new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			_settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(_settingsRoot_Changed);
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			SetTerminalCaption();
			if (e.Inner == null)
			{
				// SettingsRoot changed
				// nothing to do, no need to care about ProductVersion
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Explicit))
			{
				// ExplicitSettings changed
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Implicit))
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
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Terminal))
			{
				// TerminalSettings changed
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.PredefinedCommand))
			{
				// PredefinedCommandSettings changed
				_isSettingControls = true;
				predefined.Pages = _settingsRoot.PredefinedCommand.Pages;
				_isSettingControls = false;

				SetPredefinedMenuItems();        // ensure that shortcuts are activated
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Format))
			{
				// FormatSettings changed
				ReformatMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Log))
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
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.SendCommand))
			{
				// SendCommandSettings changed
				_isSettingControls = true;
				send.Command = _settingsRoot.SendCommand.Command;
				send.RecentCommands = _settingsRoot.SendCommand.RecentCommands;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.SendFile))
			{
				// SendFileSettings changed
				_isSettingControls = true;
				send.FileCommand = _settingsRoot.SendFile.Command;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Predefined))
			{
				// PredefinedSettings changed
				_isSettingControls = true;
				predefined.SelectedPage = _settingsRoot.Predefined.SelectedPage;
				_isSettingControls = false;
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Window))
			{
				// WindowSettings changed
				// nothing to do, windows settings are only saved
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Layout))
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
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.IO))
			{
				// IOSettings changed
				SetIOStatus();
				SetIOControlControls();
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Buffer))
			{
				// BufferSettings changed
				ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Display))
			{
				// DisplaySettings changed
				ReloadMonitors();

				monitor_Tx.ShowCountStatus    = _settingsRoot.Display.ShowCounters;
				monitor_Bidir.ShowCountStatus = _settingsRoot.Display.ShowCounters;
				monitor_Rx.ShowCountStatus    = _settingsRoot.Display.ShowCounters;
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.Transmit))
			{
				// TransmitSettings changed
				ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.TextTerminal))
			{
				// TextTerminalSettings changed
				if (_settingsRoot.TerminalType == Domain.TerminalType.Text)
					ReloadMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, _settingsRoot.BinaryTerminal))
			{
				// BinaryTerminalSettings changed
				if (_settingsRoot.TerminalType == Domain.TerminalType.Binary)
					ReloadMonitors();
			}
		}

		#endregion

		#region Settings > Suspend
		//------------------------------------------------------------------------------------------
		// Settings > Suspend
		//------------------------------------------------------------------------------------------

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

			monitor_Tx.ShowCountStatus = _settingsRoot.Display.ShowCounters;
			monitor_Bidir.ShowCountStatus = _settingsRoot.Display.ShowCounters;
			monitor_Rx.ShowCountStatus = _settingsRoot.Display.ShowCounters;
		}

		#endregion

		#endregion

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		#region Terminal > Lifetime
		//------------------------------------------------------------------------------------------
		// Terminal > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachTerminalEventHandlers()
		{
			_terminal.IOChanged        += new EventHandler(_terminal_IOChanged);
			_terminal.IOControlChanged += new EventHandler(_terminal_IOControlChanged);
			_terminal.IOCountChanged   += new EventHandler(_terminal_IOCountChanged);
			_terminal.IOError          += new EventHandler<Domain.ErrorEventArgs>(_terminal_IOError);

			_terminal.DisplayElementsSent     += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent        += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived    += new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);

			_terminal.RepositoryCleared  += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded += new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);

			_terminal.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(_terminal_TimedStatusTextRequest);
			_terminal.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(_terminal_FixedStatusTextRequest);
			_terminal.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(_terminal_MessageInputRequest);

			_terminal.SaveAsFileDialogRequest += new EventHandler<Model.DialogEventArgs>(_terminal_SaveAsFileDialogRequest);

			_terminal.Saved  += new EventHandler<Model.SavedEventArgs>(_terminal_Saved);
			_terminal.Closed += new EventHandler<Model.ClosedEventArgs>(_terminal_Closed);
		}

		private void DetachTerminalEventHandlers()
		{
			_terminal.IOChanged        -= new EventHandler(_terminal_IOChanged);
			_terminal.IOControlChanged -= new EventHandler(_terminal_IOControlChanged);
			_terminal.IOCountChanged   -= new EventHandler(_terminal_IOCountChanged);
			_terminal.IOError          -= new EventHandler<Domain.ErrorEventArgs>(_terminal_IOError);

			_terminal.DisplayElementsSent     -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsSent);
			_terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(_terminal_DisplayElementsReceived);
			_terminal.DisplayLinesSent        -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesSent);
			_terminal.DisplayLinesReceived    -= new EventHandler<Domain.DisplayLinesEventArgs>(_terminal_DisplayLinesReceived);

			_terminal.RepositoryCleared  -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryCleared);
			_terminal.RepositoryReloaded -= new EventHandler<Domain.RepositoryEventArgs>(_terminal_RepositoryReloaded);

			_terminal.TimedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(_terminal_TimedStatusTextRequest);
			_terminal.FixedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(_terminal_FixedStatusTextRequest);
			_terminal.MessageInputRequest    -= new EventHandler<Model.MessageInputEventArgs>(_terminal_MessageInputRequest);

			_terminal.SaveAsFileDialogRequest -= new EventHandler<Model.DialogEventArgs>(_terminal_SaveAsFileDialogRequest);

			_terminal.Saved  -= new EventHandler<Model.SavedEventArgs>(_terminal_Saved);
			_terminal.Closed -= new EventHandler<Model.ClosedEventArgs>(_terminal_Closed);
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _terminal_IOChanged(object sender, EventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(new EventArgs());
		}

		private void _terminal_IOControlChanged(object sender, EventArgs e)
		{
			SetIOControlControls();
		}

		private void _terminal_IOCountChanged(object sender, EventArgs e)
		{
			int txByteCount = _terminal.TxByteCount;
			int rxByteCount = _terminal.RxByteCount;

			int txLineCount = _terminal.TxLineCount;
			int rxLineCount = _terminal.RxLineCount;

			monitor_Tx.TxByteCountStatus    = txByteCount;
			monitor_Tx.TxLineCountStatus    = txLineCount;
			monitor_Bidir.TxByteCountStatus = txByteCount;
			monitor_Bidir.TxLineCountStatus = txLineCount;

			monitor_Bidir.RxByteCountStatus = rxByteCount;
			monitor_Bidir.RxLineCountStatus = rxLineCount;
			monitor_Rx.RxByteCountStatus    = rxByteCount;
			monitor_Rx.RxLineCountStatus    = rxLineCount;
		}

		private void _terminal_IOError(object sender, Domain.ErrorEventArgs e)
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

		private void _terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			// display elements immediately
			monitor_Tx.AddElements(e.Elements);
			monitor_Bidir.AddElements(e.Elements);
		}

		private void _terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			// display elements immediately
			monitor_Bidir.AddElements(e.Elements);
			monitor_Rx.AddElements(e.Elements);
		}

		private void _terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			// replace lines when they are complete
			for (int i = 0; i < e.Lines.Count; i++)
			{
				int offset = e.Lines.Count - i; // e.g. offset = 1 replaces last line
				monitor_Tx.ReplaceLine(offset, e.Lines[i]);
				monitor_Bidir.ReplaceLine(offset, e.Lines[i]);
			}
		}

		private void _terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			// replace lines when they are complete
			for (int i = 0; i < e.Lines.Count; i++)
			{
				int offset = e.Lines.Count - i; // e.g. offset = 1 replaces last line
				monitor_Bidir.ReplaceLine(offset, e.Lines[i]);
				monitor_Rx.ReplaceLine(offset, e.Lines[i]);
			}
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
				case Domain.RepositoryType.Tx:    monitor_Tx.AddLines   (_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Tx));    break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.AddLines(_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Bidir)); break;
				case Domain.RepositoryType.Rx:    monitor_Rx.AddLines   (_terminal.RepositoryToDisplayLines(Domain.RepositoryType.Rx));    break;
			}
		}

		private void _terminal_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		private void _terminal_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		private void _terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = MessageBox.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		private void _terminal_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveTerminalAsFileDialog();
		}

		private void _terminal_Saved(object sender, Model.SavedEventArgs e)
		{
			SetTerminalControls();
			SelectSendCommandInput();
		}

		private void _terminal_Closed(object sender, Model.ClosedEventArgs e)
		{
			// prevent multiple calls to Close()
			if (!_isClosingFromForm)
			{
				_isClosingFromModel = true;
				Close();
			}
		}

		#endregion

		#region Terminal > Methods
		//------------------------------------------------------------------------------------------
		// Terminal > Methods
		//------------------------------------------------------------------------------------------

		private DialogResult ShowSaveTerminalAsFileDialog()
		{
			SetFixedStatusText("Saving terminal as...");

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save " + UserName + " As";
			sfd.Filter = ExtensionSettings.TerminalFilesFilter;
			sfd.DefaultExt = ExtensionSettings.TerminalFiles;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.TerminalFilesPath;
			sfd.FileName = UserName + "." + sfd.DefaultExt;

			DialogResult dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.TerminalFilesPath = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUser();

				_terminal.SaveAs(sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}
			SelectSendCommandInput();
			return (dr);
		}

		#endregion

		#region Terminal > Settings
		//------------------------------------------------------------------------------------------
		// Terminal > Settings
		//------------------------------------------------------------------------------------------

		private void ShowTerminalSettings()
		{
			SetFixedStatusText("Terminal Settings...");

			Gui.Forms.TerminalSettings f = new Gui.Forms.TerminalSettings(_settingsRoot.Terminal);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				Domain.Settings.TerminalSettings s = f.SettingsResult;
				if (s.HaveChanged)
				{
					SuspendHandlingTerminalSettings();
					_terminal.SetSettings(s);
					ResumeHandlingTerminalSettings();
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

			if ((_settingsRoot != null) && _settingsRoot.ExplicitHaveChanged)
				sb.Append("*");

			if (_settingsRoot != null)
			{
				if (_settingsRoot.IOType == Domain.IOType.SerialPort)
				{
					Domain.Settings.SerialPort.SerialPortSettings s = _settingsRoot.IO.SerialPort;
					sb.Append(" - ");
					sb.Append(s.PortId.ToString());
					sb.Append(" - ");
					sb.Append(isOpen ? "Open" : "Closed");
				}
				else
				{
					Domain.Settings.Socket.SocketSettings s = _settingsRoot.IO.Socket;
					switch (_settingsRoot.IOType)
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
			send.TerminalIsOpen = isOpen;

			// predefined panel
			predefined.TerminalIsOpen = isOpen;
		}

		private void SetIOStatus()
		{
			bool isOpen = _terminal.IsOpen;
			bool isConnected = _terminal.IsConnected;

			StringBuilder sb = new StringBuilder();

			if (_settingsRoot.IOType == Domain.IOType.SerialPort)
			{
				Domain.Settings.SerialPort.SerialPortSettings s = _settingsRoot.IO.SerialPort;
				sb.Append("Serial port ");
				sb.Append(s.PortId.ToString());
				sb.Append(" (" + s.Communication.ToString() + ") is ");
				sb.Append(isOpen ? "open" : "closed");

				toolStripStatusLabel_TerminalStatus_Connection.Visible = false;
			}
			else
			{
				Domain.Settings.Socket.SocketSettings s = _settingsRoot.IO.Socket;
				switch (_settingsRoot.IOType)
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
						{
							if (isConnected)
							{
								Domain.IO.TcpServer server = (Domain.IO.TcpServer)_terminal.UnderlyingIOProvider;
								int count = server.ConnectedClientCount;

								sb.Append("connected to ");
								sb.Append(count.ToString());
								if (count == 1)
									sb.Append(" client");
								else
									sb.Append(" clients");
							}
							else
							{
								sb.Append("listening");
							}
						}
						else
						{
							sb.Append("closed");
						}
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
			bool isSerialPort = (_settingsRoot.IOType == Domain.IOType.SerialPort);

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

					bool rs485FlowControl = (_settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == Domain.IO.FlowControl.RS485);

					if (rs485FlowControl)
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

					bool manualFlowControl = (_settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == Domain.IO.FlowControl.Manual);

					toolStripStatusLabel_TerminalStatus_RTS.ForeColor = (manualFlowControl ? SystemColors.ControlText : SystemColors.GrayText);
					toolStripStatusLabel_TerminalStatus_CTS.ForeColor = SystemColors.GrayText;
					toolStripStatusLabel_TerminalStatus_DTR.ForeColor = (manualFlowControl ? SystemColors.ControlText : SystemColors.GrayText);
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

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

		private void SetLogControls()
		{
			bool logSelected = _settingsRoot.Log.AnyRawOrNeat;
			bool logOpen = _settingsRoot.LogIsOpen;

			toolStripMenuItem_TerminalMenu_Log_Begin.Enabled = logSelected && !logOpen;
			toolStripMenuItem_TerminalMenu_Log_End.Enabled = logSelected && logOpen;
			toolStripMenuItem_TerminalMenu_Log_Clear.Enabled = logSelected && logOpen;
		}

		private void ShowLogSettings()
		{
			Gui.Forms.LogSettings f = new Gui.Forms.LogSettings(_settingsRoot.Log);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				_terminal.SetLogSettings(f.SettingsResult);
			}

			SelectSendCommandInput();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		protected virtual void OnTerminalChanged(EventArgs e)
		{
			EventHelper.FireSync(Changed, this, e);
		}

		protected virtual void OnTerminalSaved(Model.SavedEventArgs e)
		{
			EventHelper.FireSync<Model.SavedEventArgs>(Saved, this, e);
		}

		#endregion

		#region Status
		//==========================================================================================
		// Status
		//==========================================================================================

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
