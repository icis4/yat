//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.35
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2016 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Using
//==================================================================================================
// Using
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Drawing;
using MKY.IO;
using MKY.Settings;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.View.Controls;
using YAT.View.Utilities;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "YAT.View.Forms.Terminal.#toolTip", Justification = "This is a bug in FxCop 1.36.")]

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public partial class Terminal : Form
	{
		#region Types
		//==========================================================================================
		// Types
		//==========================================================================================

		private enum ClosingState
		{
			/// <summary>Normal operation of the form.</summary>
			None,

			/// <summary>Closing has been initiated by a form event.</summary>
			IsClosingFromForm,

			/// <summary>Closing has been initiated by a model event.</summary>
			IsClosingFromModel
		}

		private enum IOStatusIndicatorControl
		{
			/// <summary>I/O indicator is steady.</summary>
			Steady,

			/// <summary>I/O indicator is flashing.</summary>
			Flashing
		}

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		// Status
		private const string DefaultStatusText = "";
		private const int TimedStatusInterval = 2000;
		private const int RfrLuminescenceInterval = 179;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Startup/Update/Closing:
		private bool isStartingUp = true;
		private SettingControlsHelper isSettingControls;
		private ClosingState closingState = ClosingState.None;

		// MDI:
		private Form mdiParent;
		private ContextMenuStripShortcutWorkaround contextMenuStripShortcutWorkaround;

		// Terminal:
		private Model.Terminal terminal;

		// Monitors:
		private Domain.RepositoryType lastMonitorSelection = Domain.RepositoryType.None;

		// Settings:
		private TerminalSettingsRoot settingsRoot;
		private bool handlingTerminalSettingsIsSuspended; // = false; // A simple flag is sufficient as
		                                                              // the form is ISynchronizeInvoke.
		// Status
		private bool ioStatusIndicatorFlashingIsOn; // = false;

		// Toolstrip-combobox-validation-workaround (too late invocation of 'Validate' event):
		private bool terminalMenuValidationWorkaround_UpdateIsSuspended;

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

		/// <summary></summary>
		public Terminal()
			: this(new Model.Terminal())
		{
		}

		/// <summary></summary>
		public Terminal(Model.Terminal terminal)
		{
			DebugMessage("Creating...");

			InitializeComponent();

			FixContextMenus();

			InitializeControls();

			// Link and attach to terminal model.
			this.terminal = terminal;
			AttachTerminalEventHandlers();

			// Link and attach to terminal settings.
			this.settingsRoot = this.terminal.SettingsRoot;
			AttachSettingsEventHandlers();

			ApplyWindowSettings();
			LayoutTerminal();

			// Force settings changed event to set all controls.
			// For improved performance, manually suspend/resume handler for terminal settings
			SuspendHandlingTerminalSettings();
			this.settingsRoot.ClearChanged();
			this.settingsRoot.ForceChangeEvent();
			ResumeHandlingTerminalSettings();

			DebugMessage("...successfully created.");
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <summary></summary>
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this.contextMenuStripShortcutWorkaround.ProcessShortcut(keyData))
				return (true);

			// In addition to predefined shortcuts in the menus, the shortcut Alt+Shift+F1..F12
			// shall copy the according 'Predefined Command' to 'Send Text':
			if ((keyData & Keys.Modifiers) == (Keys.Alt | Keys.Shift))
			{
				int functionKey;
				if (KeysEx.TryConvertFunctionKey(keyData, out functionKey))
				{
					CopyPredefined(functionKey);
					return (true);
				}
			}

			return (base.ProcessCmdKey(ref msg, keyData));
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Initially set controls and validate its contents where needed.
		/// </summary>
		/// <remarks>
		/// The 'Shown' event is only raised the first time a form is displayed; subsequently
		/// minimizing, maximizing, restoring, hiding, showing, or invalidating and repainting will
		/// not raise this event again.
		/// Note that the 'Shown' event is raised after the 'Load' event and will also be raised if
		/// the application is started minimized. Also note that operations called in the 'Shown'
		/// event can depend on a properly drawn form, as the 'Paint' event of this form and its
		/// child controls has been raised before this 'Shown' event.
		/// </remarks>
		private void Terminal_Shown(object sender, EventArgs e)
		{
			this.isStartingUp = false;

			this.mdiParent = MdiParent;

			// Immediately set terminal controls so the terminal "looks" nice from the very start.
			SetTerminalControls();
		}

		private void Terminal_Activated(object sender, EventArgs e)
		{
			monitor_Tx   .Activate();
			monitor_Bidir.Activate();
			monitor_Rx   .Activate();

			// Select send text control to enable immediate user input.
			SelectSendTextInput();
		}

		private void Terminal_Deactivate(object sender, EventArgs e)
		{
			monitor_Tx   .Deactivate();
			monitor_Bidir.Deactivate();
			monitor_Rx   .Deactivate();
		}

		private void Terminal_LocationChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsClosing)
				SaveWindowSettings();
		}

		private void Terminal_SizeChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsClosing)
				SaveWindowSettings();
		}

		/// <remarks>
		/// Attention:
		/// In case of MDI parent/application closing, this FormClosing event is called before
		/// the FormClosing event of the MDI parent. Therefore, this MDI child only has to handle
		/// this event in case of a user triggered form close request.
		/// </remarks>
		private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Prevent multiple calls to Close().
			if (this.closingState == ClosingState.None)
			{
				this.closingState = ClosingState.IsClosingFromForm;

				if (e.CloseReason == CloseReason.UserClosing)
				{
					e.Cancel = (!this.terminal.Close());

					// Revert closing state in case of cancel:
					if (e.Cancel)
						this.closingState = ClosingState.None;
				}
			}
		}

		private void Terminal_FormClosed(object sender, FormClosedEventArgs e)
		{
			DetachTerminalEventHandlers();
			this.terminal = null;

			DetachSettingsEventHandlers();
			this.settingsRoot = null;
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

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_File_SetMenuItems()
		{
			this.isSettingControls.Enter();

			if (TerminalIsAvailable)
				toolStripMenuItem_TerminalMenu_File_Save.Enabled = this.terminal.SettingsFileIsWritable;
			else
				toolStripMenuItem_TerminalMenu_File_Save.Enabled = false;

			this.isSettingControls.Leave();
		}

		private void toolStripMenuItem_TerminalMenu_File_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_File_SetMenuItems();
		}

		private void toolStripMenuItem_TerminalMenu_File_Save_Click(object sender, EventArgs e)
		{
			this.terminal.Save();
		}

		private void toolStripMenuItem_TerminalMenu_File_SaveAs_Click(object sender, EventArgs e)
		{
			ShowSaveTerminalAsFileDialog();
		}

		private void toolStripMenuItem_TerminalMenu_File_Close_Click(object sender, EventArgs e)
		{
			this.terminal.Close();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Terminal
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Terminal
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems()
		{
			this.isSettingControls.Enter();

			// Start/stop
			if (TerminalIsAvailable)
			{
				toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = !this.terminal.IsStarted;
				toolStripMenuItem_TerminalMenu_Terminal_Stop.Enabled  =  this.terminal.IsStarted;
				toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled =  this.terminal.IsBusy;
			}
			else
			{
				toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = false;
				toolStripMenuItem_TerminalMenu_Terminal_Stop.Enabled  = false;
				toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled = false;
			}

			// Edit
			bool monitorIsDefined = (this.lastMonitorSelection != Domain.RepositoryType.None);
			bool editIsNotActive = (!send.EditIsActive);
			toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Enabled       = (monitorIsDefined && editIsNotActive);
			toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Enabled      = (monitorIsDefined && editIsNotActive);
			toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = (monitorIsDefined && editIsNotActive);
			toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Enabled      =  monitorIsDefined;
			toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled           =  monitorIsDefined;

			this.isSettingControls.Leave();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Start_Click(object sender, EventArgs e)
		{
			this.terminal.StartIO();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Stop_Click(object sender, EventArgs e)
		{
			this.terminal.StopIO();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Break_Click(object sender, EventArgs e)
		{
			this.terminal.Break();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Clear_Click(object sender, EventArgs e)
		{
			ClearMonitors();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Refresh_Click(object sender, EventArgs e)
		{
			RefreshMonitors();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_SelectAll_Click(object sender, EventArgs e)
		{
			SelectAllMonitorContents(GetMonitor(this.lastMonitorSelection));
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_SelectNone_Click(object sender, EventArgs e)
		{
			SelectNoneMonitorContents(GetMonitor(this.lastMonitorSelection));
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard_Click(object sender, EventArgs e)
		{
			CopyMonitorToClipboard(GetMonitor(this.lastMonitorSelection));
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_SaveToFile_Click(object sender, EventArgs e)
		{
			ShowSaveMonitorDialog(GetMonitor(this.lastMonitorSelection));
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Print_Click(object sender, EventArgs e)
		{
			ShowPrintMonitorDialog(GetMonitor(this.lastMonitorSelection));
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

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
		{
			this.isSettingControls.Enter();

			// Prepare the menu item properties based on state and settings.
			//
			// Attention:
			// Similar code exists in the following locations:
			//  > View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
			//  > View.Controls.SendText.SetControls()
			// Changes here may have to be applied there too.
			//
			// Main and context menu are separated as there are subtle differences between them.

			string sendTextText = "Text";
			bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText;
			if (this.settingsRoot.Send.SendImmediately)
			{
				switch (this.settingsRoot.TerminalType)
				{
					case Domain.TerminalType.Text: sendTextText = "EOL";    break;
					default: /* Binary or <New> */ sendTextEnabled = false; break;
				}
			}

			bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath;

			// Set the menu item properties:

			toolStripMenuItem_TerminalMenu_Send_Text.Text    = sendTextText;
			toolStripMenuItem_TerminalMenu_Send_Text.Enabled = sendTextEnabled && this.terminal.IsReadyToSend;
			toolStripMenuItem_TerminalMenu_Send_File.Enabled = sendFileEnabled && this.terminal.IsReadyToSend;

			toolStripMenuItem_TerminalMenu_Send_KeepCommand.Checked     = this.settingsRoot.Send.KeepCommand;
			toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Checked  = this.settingsRoot.Send.CopyPredefined;
			toolStripMenuItem_TerminalMenu_Send_SendImmediately.Checked = this.settingsRoot.Send.SendImmediately;

			toolStripMenuItem_TerminalMenu_Send_AutoResponse.Checked          =  this.settingsRoot.AutoResponse.Enabled;
			toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.Checked  = (this.settingsRoot.AutoResponse.TriggerSelection  != Model.Types.Trigger.None);
			toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.Checked = (this.settingsRoot.AutoResponse.ResponseSelection != Model.Types.AutoResponse.None);

			// Attention:
			// Similar code exists in the following location:
			//  > View.Forms.Main.toolStripButton_MainTool_SetControls()
			// Changes here may have to be applied there too.

			if (!this.terminalMenuValidationWorkaround_UpdateIsSuspended)
			{
				toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.Clear();
				toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.AddRange(this.settingsRoot.ValidAutoResponseTriggerItems);

				if (this.settingsRoot.AutoResponse.TriggerSelection != Model.Types.Trigger.Explicit)
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedItem = (Model.Types.TriggerEx)this.settingsRoot.AutoResponse.TriggerSelection;
				else if (this.settingsRoot.AutoResponse.ExplicitTrigger != null)
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text = this.settingsRoot.AutoResponse.ExplicitTrigger.SingleLineText;
				else
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text = Model.Types.Command.DefineCommandText;

				toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.Clear();
				toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.AddRange(this.settingsRoot.ValidAutoResponseResponseItems);

				if (this.settingsRoot.AutoResponse.ResponseSelection != Model.Types.AutoResponse.Explicit)
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedItem = (Model.Types.AutoResponseEx)this.settingsRoot.AutoResponse.ResponseSelection;
				else if (this.settingsRoot.AutoResponse.ExplicitResponse != null)
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text = this.settingsRoot.AutoResponse.ExplicitResponse.SingleLineText;
				else
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text = Model.Types.Command.DefineCommandText;
			}

			this.isSettingControls.Leave();
		}

		private void toolStripMenuItem_TerminalMenu_Send_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Send_SetMenuItems();
		}

		private void toolStripMenuItem_TerminalMenu_Send_Text_Click(object sender, EventArgs e)
		{
			if (!this.settingsRoot.Send.SendImmediately)
				this.terminal.SendText();
			else
				this.terminal.SendPartialTextEol();
		}

		private void toolStripMenuItem_TerminalMenu_Send_File_Click(object sender, EventArgs e)
		{
			this.terminal.SendFile();
		}

		private void toolStripMenuItem_TerminalMenu_Send_KeepCommand_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.KeepCommand = !this.settingsRoot.Send.KeepCommand;
		}

		private void toolStripMenuItem_TerminalMenu_Send_CopyPredefined_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.CopyPredefined = !this.settingsRoot.Send.CopyPredefined;
		}

		private void toolStripMenuItem_TerminalMenu_Send_SendImmediately_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.SendImmediately = !this.settingsRoot.Send.SendImmediately;
		}

		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var trigger = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedItem as Model.Types.TriggerEx);
				if (trigger != null)
					this.settingsRoot.AutoResponse.FromTrigger(trigger);
			}
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				string triggerText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text;
				int invalidTextStart;
				if (Validation.ValidateText(this, "automatic response trigger", triggerText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = true;
						this.settingsRoot.AutoResponse.FromExplicitTriggerText(triggerText);
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = false;
					}
				}
				else
				{
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var response = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedItem as Model.Types.AutoResponseEx);
				if (response != null)
					this.settingsRoot.AutoResponse.FromResponse(response);
			}
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex)
			{
				string responseText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text;
				int invalidTextStart;
				if (Validation.ValidateText(this, "automatic response", responseText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = true;
						this.settingsRoot.AutoResponse.FromExplicitResponseText(responseText);
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = false;
					}
				}
				else
				{
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text.Remove(invalidTextStart);
				}
			}
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Log
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Log
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_Log_SetMenuItems()
		{
			this.isSettingControls.Enter();

			bool logIsEnabled = (this.settingsRoot.Log.Count > 0);
			bool logIsOn      = this.settingsRoot.LogIsOn;

			bool logFileExists = false;
			if (this.terminal != null)
				logFileExists = this.terminal.LogFileExists;

			toolStripMenuItem_TerminalMenu_Log_On.Enabled            = logIsEnabled && !logIsOn;
			toolStripMenuItem_TerminalMenu_Log_Off.Enabled           = logIsEnabled &&  logIsOn;
			toolStripMenuItem_TerminalMenu_Log_OpenFile.Enabled      = logIsEnabled &&  logFileExists;
			toolStripMenuItem_TerminalMenu_Log_OpenDirectory.Enabled = logIsEnabled &&  logFileExists;
			toolStripMenuItem_TerminalMenu_Log_Clear.Enabled         = logIsEnabled && (logIsOn || logFileExists);

			this.isSettingControls.Leave();
		}

		private void toolStripMenuItem_TerminalMenu_Log_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Log_SetMenuItems();
		}

		private void toolStripMenuItem_TerminalMenu_Log_On_Click(object sender, EventArgs e)
		{
			this.terminal.SwitchLogOn();
		}

		private void toolStripMenuItem_TerminalMenu_Log_Off_Click(object sender, EventArgs e)
		{
			this.terminal.SwitchLogOff();
		}

		private void toolStripMenuItem_TerminalMenu_Log_OpenFile_Click(object sender, EventArgs e)
		{
			this.terminal.OpenLogFile();
		}

		private void toolStripMenuItem_TerminalMenu_Log_OpenDirectory_Click(object sender, EventArgs e)
		{
			this.terminal.OpenLogDirectory();
		}

		private void toolStripMenuItem_TerminalMenu_Log_Clear_Click(object sender, EventArgs e)
		{
			this.terminal.ClearLog();
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

		private void toolStripMenuItem_TerminalMenu_View_Initialize()
		{
			this.isSettingControls.Enter();

			toolStripComboBox_TerminalMenu_View_Panels_Orientation.Items.AddRange(OrientationEx.GetItems());

			this.isSettingControls.Leave();
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_View_SetMenuItems()
		{
			this.isSettingControls.Enter();

			bool isText         = ((Domain.TerminalTypeEx)this.settingsRoot.TerminalType).IsText;

			bool isSerialPort   = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;
			bool isUsbSerialHid = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsUsbSerialHid;

			// Layout, disable monitor item if the other monitors are hidden:
			toolStripMenuItem_TerminalMenu_View_Panels_Tx.Enabled    = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_TerminalMenu_View_Panels_Rx.Enabled    = (this.settingsRoot.Layout.TxMonitorPanelIsVisible || this.settingsRoot.Layout.BidirMonitorPanelIsVisible);

			toolStripMenuItem_TerminalMenu_View_Panels_Tx.Checked    = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_Rx.Checked    = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

			toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedItem = (OrientationEx)this.settingsRoot.Layout.MonitorOrientation;

			toolStripMenuItem_TerminalMenu_View_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
			toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked    = this.settingsRoot.Layout.SendFilePanelIsVisible;

			toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = this.settingsRoot.Layout.PredefinedPanelIsVisible;

			// Connect time:
			bool showConnectTime = this.settingsRoot.Status.ShowConnectTime;
			toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Checked  = showConnectTime;
			toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime.Enabled = showConnectTime;

			// Counters:
			bool showCountAndRate = this.settingsRoot.Status.ShowCountAndRate;
			toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Checked = showCountAndRate;
			toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Enabled = showCountAndRate;

			// Display:
			bool isNotString = ((this.settingsRoot.Display.TxRadix != Domain.Radix.String) ||
			                    (this.settingsRoot.Display.RxRadix != Domain.Radix.String));
			toolStripMenuItem_TerminalMenu_View_ShowRadix.Enabled = isNotString; // Attention, same code further below as well as in View.Forms.AdvancedTerminalSettings.
			toolStripMenuItem_TerminalMenu_View_ShowRadix.Checked = isNotString && this.settingsRoot.Display.ShowRadix;

			toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Checked = this.settingsRoot.Display.ShowLineNumbers;
			toolStripMenuItem_TerminalMenu_View_ShowDate.Checked        = this.settingsRoot.Display.ShowDate;
			toolStripMenuItem_TerminalMenu_View_ShowTime.Checked        = this.settingsRoot.Display.ShowTime;
			toolStripMenuItem_TerminalMenu_View_ShowPort.Checked        = this.settingsRoot.Display.ShowPort;
			toolStripMenuItem_TerminalMenu_View_ShowDirection.Checked   = this.settingsRoot.Display.ShowDirection;

			toolStripMenuItem_TerminalMenu_View_ShowEol.Enabled = (isText);
			toolStripMenuItem_TerminalMenu_View_ShowEol.Checked = (isText && this.settingsRoot.TextTerminal.ShowEol);

			toolStripMenuItem_TerminalMenu_View_ShowLength.Checked = this.settingsRoot.Display.ShowLength;

			// Flow control count:
			bool showFlowControlCount = this.settingsRoot.Status.ShowFlowControlCount;
			toolStripMenuItem_TerminalMenu_View_FlowControlCount.Enabled            = (isSerialPort || (isUsbSerialHid && this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff));
			toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount.Checked  = showFlowControlCount;
			toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount.Enabled = showFlowControlCount;

			// Break count:
			bool showBreakCount = this.settingsRoot.Status.ShowBreakCount;
			toolStripMenuItem_TerminalMenu_View_BreakCount.Enabled            = (isSerialPort && this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates);
			toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount.Checked  = showBreakCount;
			toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount.Enabled = showBreakCount;

			this.isSettingControls.Leave();
		}

		private void toolStripMenuItem_TerminalMenu_View_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_View_SetMenuItems();
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Tx_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.TxMonitorPanelIsVisible = !this.settingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Bidir_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.BidirMonitorPanelIsVisible = !this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Rx_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.RxMonitorPanelIsVisible = !this.settingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripComboBox_TerminalMenu_View_Panels_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetMonitorOrientation((OrientationEx)toolStripComboBox_TerminalMenu_View_Panels_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Predefined_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.PredefinedPanelIsVisible = !this.settingsRoot.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_SendText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.SendTextPanelIsVisible = !this.settingsRoot.Layout.SendTextPanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_SendFile_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.SendFilePanelIsVisible = !this.settingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Rearrange_Click(object sender, EventArgs e)
		{
			ViewRearrange();
		}

		private void toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowConnectTime = !this.settingsRoot.Status.ShowConnectTime;
		}

		private void toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime_Click(object sender, EventArgs e)
		{
			this.terminal.ResetConnectTime();
		}

		private void toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowCountAndRate = !this.settingsRoot.Status.ShowCountAndRate;
		}

		private void toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount_Click(object sender, EventArgs e)
		{
			this.terminal.ResetIOCountAndRate();
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowRadix_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowLineNumbers_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowLineNumbers = !this.settingsRoot.Display.ShowLineNumbers;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowDate_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowDate = !this.settingsRoot.Display.ShowDate;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTime_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowTime = !this.settingsRoot.Display.ShowTime;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowPort_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowPort = !this.settingsRoot.Display.ShowPort;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowDirection_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowDirection = !this.settingsRoot.Display.ShowDirection;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowEol_Click(object sender, EventArgs e)
		{
			this.settingsRoot.TextTerminal.ShowEol = !this.settingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowLength_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowLength = !this.settingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowFlowControlCount = !this.settingsRoot.Status.ShowFlowControlCount;
		}

		private void toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount_Click(object sender, EventArgs e)
		{
			this.terminal.ResetFlowControlCount();
		}

		private void toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowBreakCount = !this.settingsRoot.Status.ShowBreakCount;
		}

		private void toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount_Click(object sender, EventArgs e)
		{
			this.terminal.ResetBreakCount();
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

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<ToolStripMenuItem> menuItems_preset;

		private void contextMenuStrip_Preset_Initialize()
		{
			this.menuItems_preset = new List<ToolStripMenuItem>(6); // Preset the required capactiy to improve memory management.
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_1);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_2);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_3);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_4);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_5);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_6);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Preset_SetMenuItems()
		{
			this.isSettingControls.Enter();

			bool isSerialPort = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;

			foreach (ToolStripMenuItem item in this.menuItems_preset)
				item.Enabled = isSerialPort;

			this.isSettingControls.Leave();
		}

		private void contextMenuStrip_Preset_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Preset_SetMenuItems();
		}

		private void toolStripMenuItem_PresetContextMenu_Preset_Click(object sender, EventArgs e)
		{
			RequestPreset(int.Parse((string)(((ToolStripMenuItem)sender).Tag), CultureInfo.InvariantCulture));
		}

		#endregion

		#region Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Monitor_Initialize()
		{
			this.isSettingControls.Enter();

			toolStripComboBox_MonitorContextMenu_Panels_Orientation.Items.AddRange(OrientationEx.GetItems());

			this.isSettingControls.Leave();
		}

		private void contextMenuStrip_Monitor_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Monitor_SetMenuItems();
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Monitor_SetMenuItems()
		{
			this.isSettingControls.Enter();

			Domain.TerminalType terminalType = this.settingsRoot.TerminalType;
			Domain.RepositoryType monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			bool isMonitor = (monitorType != Domain.RepositoryType.None);

			toolStripMenuItem_MonitorContextMenu_Panels_Tx.Checked    = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Checked = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
			toolStripMenuItem_MonitorContextMenu_Panels_Rx.Checked    = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

			// Disable "Monitor" item if the other monitors are hidden
			toolStripMenuItem_MonitorContextMenu_Panels_Tx.Enabled    = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
			toolStripMenuItem_MonitorContextMenu_Panels_Rx.Enabled    = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.BidirMonitorPanelIsVisible);

			toolStripComboBox_MonitorContextMenu_Panels_Orientation.SelectedItem = (OrientationEx)this.settingsRoot.Layout.MonitorOrientation;

			// Hide "Hide" item if only this monitor is visible
			bool hideIsAllowed = false;
			switch (monitorType)
			{
				case Domain.RepositoryType.Tx:    hideIsAllowed = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);    break;
				case Domain.RepositoryType.Bidir: hideIsAllowed = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.RxMonitorPanelIsVisible);    break;
				case Domain.RepositoryType.Rx:    hideIsAllowed = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.BidirMonitorPanelIsVisible); break;
			}
			toolStripMenuItem_MonitorContextMenu_Hide.Visible = hideIsAllowed;
			toolStripMenuItem_MonitorContextMenu_Hide.Enabled = isMonitor && hideIsAllowed;

			bool isNotString = ((this.settingsRoot.Display.TxRadix != Domain.Radix.String) ||
			                    (this.settingsRoot.Display.RxRadix != Domain.Radix.String));
			toolStripMenuItem_MonitorContextMenu_ShowRadix.Enabled = isNotString; // Attention, same code further above as well as in View.Forms.AdvancedTerminalSettings.
			toolStripMenuItem_MonitorContextMenu_ShowRadix.Checked = isNotString && this.settingsRoot.Display.ShowRadix;

			toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Checked = this.settingsRoot.Display.ShowLineNumbers;
			toolStripMenuItem_MonitorContextMenu_ShowDate.Checked        = this.settingsRoot.Display.ShowDate;
			toolStripMenuItem_MonitorContextMenu_ShowTime.Checked        = this.settingsRoot.Display.ShowTime;
			toolStripMenuItem_MonitorContextMenu_ShowPort.Checked        = this.settingsRoot.Display.ShowPort;
			toolStripMenuItem_MonitorContextMenu_ShowDirection.Checked   = this.settingsRoot.Display.ShowDirection;

			bool isText = ((Domain.TerminalTypeEx)terminalType).IsText;
			toolStripMenuItem_MonitorContextMenu_ShowEol.Enabled = isText;
			toolStripMenuItem_MonitorContextMenu_ShowEol.Checked = isText && this.settingsRoot.TextTerminal.ShowEol;

			toolStripMenuItem_MonitorContextMenu_ShowLength.Checked = this.settingsRoot.Display.ShowLength;

			bool showConnectTime = this.settingsRoot.Status.ShowConnectTime;
			toolStripMenuItem_MonitorContextMenu_ShowConnectTime.Checked  = showConnectTime;
			toolStripMenuItem_MonitorContextMenu_ResetConnectTime.Enabled = showConnectTime;

			bool showCountAndRate = this.settingsRoot.Status.ShowCountAndRate;
			toolStripMenuItem_MonitorContextMenu_ShowCountAndRate.Checked  = showCountAndRate;
			toolStripMenuItem_MonitorContextMenu_ResetCount.Enabled        = showCountAndRate;

			toolStripMenuItem_MonitorContextMenu_Clear.Enabled = isMonitor;

			toolStripMenuItem_MonitorContextMenu_SelectAll.Enabled  = isMonitor;
			toolStripMenuItem_MonitorContextMenu_SelectNone.Enabled = isMonitor;

			toolStripMenuItem_MonitorContextMenu_SaveToFile.Enabled      = isMonitor;
			toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Enabled = isMonitor;
			toolStripMenuItem_MonitorContextMenu_Print.Enabled           = isMonitor;

			this.isSettingControls.Leave();
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Tx_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.TxMonitorPanelIsVisible = !this.settingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Bidir_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.BidirMonitorPanelIsVisible = !this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Rx_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.RxMonitorPanelIsVisible = !this.settingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripComboBox_MonitorContextMenu_Panels_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				SetMonitorOrientation((OrientationEx)toolStripComboBox_MonitorContextMenu_Panels_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_MonitorContextMenu_Hide_Click(object sender, EventArgs e)
		{
			switch (GetMonitorType(contextMenuStrip_Monitor.SourceControl))
			{
				case Domain.RepositoryType.Tx:    this.settingsRoot.Layout.TxMonitorPanelIsVisible    = false; break;
				case Domain.RepositoryType.Bidir: this.settingsRoot.Layout.BidirMonitorPanelIsVisible = false; break;
				case Domain.RepositoryType.Rx:    this.settingsRoot.Layout.RxMonitorPanelIsVisible    = false; break;
			}
		}

		private void toolStripMenuItem_MonitorContextMenu_Format_Click(object sender, EventArgs e)
		{
			ShowFormatSettings();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowRadix_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLineNumbers_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowLineNumbers = !this.settingsRoot.Display.ShowLineNumbers;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowDate_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowDate = !this.settingsRoot.Display.ShowDate;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTime_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowTime = !this.settingsRoot.Display.ShowTime;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowPort_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowPort = !this.settingsRoot.Display.ShowPort;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowDirection_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowDirection = !this.settingsRoot.Display.ShowDirection;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowEol_Click(object sender, EventArgs e)
		{
			this.settingsRoot.TextTerminal.ShowEol = !this.settingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLength_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowLength = !this.settingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowConnectTime_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowConnectTime = !this.settingsRoot.Status.ShowConnectTime;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetConnectTime_Click(object sender, EventArgs e)
		{
			this.terminal.ResetConnectTime();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowCountAndRate_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowCountAndRate = !this.settingsRoot.Status.ShowCountAndRate;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetCount_Click(object sender, EventArgs e)
		{
			this.terminal.ResetIOCountAndRate();
		}

		private void toolStripMenuItem_MonitorContextMenu_Clear_Click(object sender, EventArgs e)
		{
			Domain.RepositoryType repositoryType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			if (repositoryType != Domain.RepositoryType.None)
				ClearMonitor(repositoryType);
			else
				throw (new InvalidOperationException("Program execution should never get here, invalid context menu source control received from " + sender.ToString() + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		private void toolStripMenuItem_MonitorContextMenu_Refresh_Click(object sender, EventArgs e)
		{
			Domain.RepositoryType repositoryType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			if (repositoryType != Domain.RepositoryType.None)
				RefreshMonitor(repositoryType);
			else
				throw (new InvalidOperationException("Program execution should never get here, invalid context menu source control received from " + sender.ToString() + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		private void toolStripMenuItem_MonitorContextMenu_SelectAll_Click(object sender, EventArgs e)
		{
			GetMonitor(contextMenuStrip_Monitor.SourceControl).SelectAll();
		}

		private void toolStripMenuItem_MonitorContextMenu_SelectNone_Click(object sender, EventArgs e)
		{
			GetMonitor(contextMenuStrip_Monitor.SourceControl).SelectNone();
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

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Radix_SetMenuItems()
		{
			this.isSettingControls.Enter();

			bool separateTxRx = this.settingsRoot.Display.SeparateTxRxRadix;

			toolStripMenuItem_RadixContextMenu_String.Visible = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Char.Visible   = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Bin.Visible    = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Oct.Visible    = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Dec.Visible    = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Hex.Visible    = !separateTxRx;

			toolStripMenuItem_RadixContextMenu_Separator_1.Visible = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Separator_2.Visible = !separateTxRx;
			toolStripMenuItem_RadixContextMenu_Separator_3.Visible = separateTxRx;

			toolStripMenuItem_RadixContextMenu_SeparateTxRx.Checked = separateTxRx;

			toolStripMenuItem_RadixContextMenu_TxRadix.Visible = separateTxRx;
			toolStripMenuItem_RadixContextMenu_RxRadix.Visible = separateTxRx;

			if (!separateTxRx)
			{
				toolStripMenuItem_RadixContextMenu_String.Checked = (this.settingsRoot.Display.TxRadix == Domain.Radix.String);
				toolStripMenuItem_RadixContextMenu_Char.Checked   = (this.settingsRoot.Display.TxRadix == Domain.Radix.Char);
				toolStripMenuItem_RadixContextMenu_Bin.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Bin);
				toolStripMenuItem_RadixContextMenu_Oct.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Oct);
				toolStripMenuItem_RadixContextMenu_Dec.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Dec);
				toolStripMenuItem_RadixContextMenu_Hex.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Hex);
			}
			else
			{
				toolStripMenuItem_RadixContextMenu_Tx_String.Checked = (this.settingsRoot.Display.TxRadix == Domain.Radix.String);
				toolStripMenuItem_RadixContextMenu_Tx_Char.Checked   = (this.settingsRoot.Display.TxRadix == Domain.Radix.Char);
				toolStripMenuItem_RadixContextMenu_Tx_Bin.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Bin);
				toolStripMenuItem_RadixContextMenu_Tx_Oct.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Oct);
				toolStripMenuItem_RadixContextMenu_Tx_Dec.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Dec);
				toolStripMenuItem_RadixContextMenu_Tx_Hex.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Hex);

				toolStripMenuItem_RadixContextMenu_Rx_String.Checked = (this.settingsRoot.Display.RxRadix == Domain.Radix.String);
				toolStripMenuItem_RadixContextMenu_Rx_Char.Checked   = (this.settingsRoot.Display.RxRadix == Domain.Radix.Char);
				toolStripMenuItem_RadixContextMenu_Rx_Bin.Checked    = (this.settingsRoot.Display.RxRadix == Domain.Radix.Bin);
				toolStripMenuItem_RadixContextMenu_Rx_Oct.Checked    = (this.settingsRoot.Display.RxRadix == Domain.Radix.Oct);
				toolStripMenuItem_RadixContextMenu_Rx_Dec.Checked    = (this.settingsRoot.Display.RxRadix == Domain.Radix.Dec);
				toolStripMenuItem_RadixContextMenu_Rx_Hex.Checked    = (this.settingsRoot.Display.RxRadix == Domain.Radix.Hex);
			}

			this.isSettingControls.Leave();
		}

		private void contextMenuStrip_Radix_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Radix_SetMenuItems();
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

		private void toolStripMenuItem_RadixContextMenu_SeparateTxRx_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.SeparateTxRxRadix = !this.settingsRoot.Display.SeparateTxRxRadix;
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_String_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Char_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Bin_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Oct_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Dec_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Hex_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Hex);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_String_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Char_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Bin_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Oct_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Dec_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Hex_Click(object sender, EventArgs e)
		{
			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Hex);
		}

		#endregion

		#region Controls Event Handlers > Predefined Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Predefined Context Menu
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<ToolStripMenuItem> menuItems_Predefined_Commands;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<ToolStripMenuItem> menuItems_Predefined_Pages;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "'MaxPages' indeed starts with an upper case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private const int menuItems_Predefined_MaxPages = 12;

		private void contextMenuStrip_Predefined_Initialize()
		{
			this.menuItems_Predefined_Commands = new List<ToolStripMenuItem>(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage); // Preset the required capactiy to improve memory management.
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_1);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_2);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_3);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_4);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_5);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_6);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_7);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_8);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_9);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_10);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_11);
			this.menuItems_Predefined_Commands.Add(toolStripMenuItem_PredefinedContextMenu_Command_12);

			this.menuItems_Predefined_Pages = new List<ToolStripMenuItem>(menuItems_Predefined_MaxPages); // Preset the required capactiy to improve memory management.
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_1);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_2);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_3);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_4);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_5);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_6);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_7);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_8);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_9);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_10);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_11);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_12);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Predefined_SetMenuItems()
		{
			this.isSettingControls.Enter();

			// Pages:
			List<Model.Types.PredefinedCommandPage> pages = this.settingsRoot.PredefinedCommand.Pages;

			int pageCount = 0;
			if (pages != null)
				pageCount = pages.Count;

			if (pageCount > 0)
			{
				toolStripMenuItem_PredefinedContextMenu_Page_Previous.Enabled = (predefined.SelectedPage > pages.Count);
				toolStripMenuItem_PredefinedContextMenu_Page_Next.Enabled     = (predefined.SelectedPage < pages.Count);
			}
			else
			{
				toolStripMenuItem_PredefinedContextMenu_Page_Previous.Enabled = false;
				toolStripMenuItem_PredefinedContextMenu_Page_Next.Enabled = false;
			}

			for (int i = 0; i < Math.Min(pageCount, menuItems_Predefined_MaxPages); i++)
			{
				this.menuItems_Predefined_Pages[i].Text      = MenuEx.PrependIndex(i + 1, pages[i].PageName);
				this.menuItems_Predefined_Pages[i].Visible   = true;
				this.menuItems_Predefined_Pages[i].Enabled   = this.terminal.IsOpen;
			}
			for (int i = pageCount; i < menuItems_Predefined_MaxPages; i++)
			{
				this.menuItems_Predefined_Pages[i].Text      = MenuEx.PrependIndex(i + 1, "<Undefined>");
				this.menuItems_Predefined_Pages[i].Visible   = false;
				this.menuItems_Predefined_Pages[i].Enabled   = false;
			}

			// Commands:
			List<Model.Types.Command> commands = null;
			if (pageCount > 0)
				commands = this.settingsRoot.PredefinedCommand.Pages[predefined.SelectedPage - 1].Commands;

			int commandCount = 0;
			if (commands != null)
				commandCount = commands.Count;

			for (int i = 0; i < Math.Min(commandCount, Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage); i++)
			{
				bool isDefined = ((commands[i] != null) && commands[i].IsDefined);
				bool isValid = (isDefined && commands[i].IsValid && this.terminal.IsReadyToSend);

				if (isDefined)
				{
					if (this.menuItems_Predefined_Commands[i].ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
						this.menuItems_Predefined_Commands[i].ForeColor = SystemColors.ControlText;

					if (this.menuItems_Predefined_Commands[i].Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
						this.menuItems_Predefined_Commands[i].Font = SystemFonts.DefaultFont;

					this.menuItems_Predefined_Commands[i].Text = MenuEx.PrependIndex(i + 1, commands[i].Description);
					this.menuItems_Predefined_Commands[i].Enabled = isValid;
				}
				else
				{
					if (this.menuItems_Predefined_Commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						this.menuItems_Predefined_Commands[i].ForeColor = SystemColors.GrayText;

					if (this.menuItems_Predefined_Commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
						this.menuItems_Predefined_Commands[i].Font = DrawingEx.DefaultFontItalic;

					this.menuItems_Predefined_Commands[i].Text = MenuEx.PrependIndex(i + 1, Model.Types.Command.DefineCommandText);
					this.menuItems_Predefined_Commands[i].Enabled = true;
				}
			}
			for (int i = commandCount; i < Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage; i++)
			{
				if (this.menuItems_Predefined_Commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
					this.menuItems_Predefined_Commands[i].ForeColor = SystemColors.GrayText;

				if (this.menuItems_Predefined_Commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
					this.menuItems_Predefined_Commands[i].Font = DrawingEx.DefaultFontItalic;

				this.menuItems_Predefined_Commands[i].Text = MenuEx.PrependIndex(i + 1, Model.Types.Command.DefineCommandText);
				this.menuItems_Predefined_Commands[i].Enabled = true;
			}

			this.isSettingControls.Leave();
		}

		/// <summary>
		/// Temporary reference to command to be copied.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int contextMenuStrip_Predefined_SelectedCommand; // = 0;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Model.Types.Command contextMenuStrip_Predefined_CopyToSendText; // = null;

		private void contextMenuStrip_Predefined_Opening(object sender, CancelEventArgs e)
		{
			if (contextMenuStrip_Predefined.SourceControl == groupBox_Predefined)
			{
				int id = predefined.GetCommandIdFromScreenPoint(new Point(contextMenuStrip_Predefined.Left, contextMenuStrip_Predefined.Top));
				Model.Types.Command c = predefined.GetCommandFromId(id);

				contextMenuStrip_Predefined_SelectedCommand = id;
				contextMenuStrip_Predefined_CopyToSendText = c;

				toolStripMenuItem_PredefinedContextMenu_Separator_3.Visible = true;

				ToolStripMenuItem mi = toolStripMenuItem_PredefinedContextMenu_CopyToSendText;
				mi.Visible = true;
				if (c != null)
				{
					mi.Enabled = (c.IsText || c.IsFilePath);
					if (c.IsText)
						mi.Text = "Copy to Send Text";
					else if (c.IsFilePath)
						mi.Text = "Copy to Send File";
					else
						mi.Text = "Copy";
				}
				else
				{
					mi.Enabled = false;
					mi.Text = "Copy";
				}

				// There is a limitaion in Windows.Forms:
				//  1. Edit command in SendText
				//  2. Right-click to open the predefined context menu
				//     => SendText should get validated, but actuall isn't!
				//
				// Workaround:
				send.ValidateSendTextInput();

				toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Visible = true;
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Enabled = ((id != 0) && (this.settingsRoot.SendText.Command.IsText));
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Visible = true;
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Enabled = ((id != 0) && (this.settingsRoot.SendFile.Command.IsFilePath));
			}
			else
			{
				toolStripMenuItem_PredefinedContextMenu_Separator_3.Visible = false;

				toolStripMenuItem_PredefinedContextMenu_CopyToSendText.Visible = false;
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Visible = false;
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Visible = false;
			}

			contextMenuStrip_Predefined_SetMenuItems();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Command_Click(object sender, EventArgs e)
		{
			SendPredefined(int.Parse((string)(((ToolStripMenuItem)sender).Tag), CultureInfo.InvariantCulture));
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Next_Click(object sender, EventArgs e)
		{
			predefined.NextPage();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Previous_Click(object sender, EventArgs e)
		{
			predefined.PreviousPage();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Click(object sender, EventArgs e)
		{
			predefined.SelectedPage = int.Parse((string)(((ToolStripMenuItem)sender).Tag), CultureInfo.InvariantCulture);
		}

		private void toolStripMenuItem_PredefinedContextMenu_Define_Click(object sender, EventArgs e)
		{
			if (contextMenuStrip_Predefined_SelectedCommand != 0)
				ShowPredefinedCommandSettings(predefined.SelectedPage, contextMenuStrip_Predefined_SelectedCommand);
			else
				ShowPredefinedCommandSettings(predefined.SelectedPage, 1);
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyToSendText_Click(object sender, EventArgs e)
		{
			Model.Types.Command c = contextMenuStrip_Predefined_CopyToSendText;
			if (c != null)
			{
				if (c.IsText)
					this.settingsRoot.SendText.Command = c;
				else if (c.IsFilePath)
					this.settingsRoot.SendFile.Command = c;
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyFromSendText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPage - 1, contextMenuStrip_Predefined_SelectedCommand - 1, this.settingsRoot.SendText.Command);
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile_Click(object sender, EventArgs e)
		{
			this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPage - 1, contextMenuStrip_Predefined_SelectedCommand - 1, this.settingsRoot.SendFile.Command);
		}

		private void toolStripMenuItem_PredefinedContextMenu_Hide_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.PredefinedPanelIsVisible = false;
		}

		#endregion

		#region Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send Context Menu
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Send_SetMenuItems()
		{
			this.isSettingControls.Enter();

			// Prepare the menu item properties based on state and settings.
			//
			// Attention:
			// Similar code exists in the following locations:
			//  > View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
			//  > View.Controls.SendText.SetControls()
			// Changes here may have to be applied there too.
			//
			// Context and main menu are separated as there are subtle differences between them.

			string sendTextText = "Send Text";
			bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText;
			if (this.settingsRoot.Send.SendImmediately)
			{
				switch (this.settingsRoot.TerminalType)
				{
					case Domain.TerminalType.Text: sendTextText = "Send EOL"; break;
					default: /* Binary or <New> */ sendTextEnabled = false;   break;
				}
			}

			bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath;

			// Set the menu item properties:

			toolStripMenuItem_SendContextMenu_SendText.Text    = sendTextText;
			toolStripMenuItem_SendContextMenu_SendText.Enabled = sendTextEnabled && this.terminal.IsReadyToSend;
			toolStripMenuItem_SendContextMenu_SendFile.Enabled = sendFileEnabled && this.terminal.IsReadyToSend;

			toolStripMenuItem_SendContextMenu_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
			toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

			toolStripMenuItem_SendContextMenu_KeepCommand.Checked     = this.settingsRoot.Send.KeepCommand;
			toolStripMenuItem_SendContextMenu_CopyPredefined.Checked  = this.settingsRoot.Send.CopyPredefined;
			toolStripMenuItem_SendContextMenu_SendImmediately.Checked = this.settingsRoot.Send.SendImmediately;

			this.isSettingControls.Leave();
		}

		private void contextMenuStrip_Send_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Send_SetMenuItems();
		}

		private void toolStripMenuItem_SendContextMenu_SendText_Click(object sender, EventArgs e)
		{
			if (!this.settingsRoot.Send.SendImmediately)
				this.terminal.SendText();
			else
				this.terminal.SendPartialTextEol();
		}

		private void toolStripMenuItem_SendContextMenu_SendFile_Click(object sender, EventArgs e)
		{
			this.terminal.SendFile();
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.SendTextPanelIsVisible = !this.settingsRoot.Layout.SendTextPanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendFile_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.SendFilePanelIsVisible = !this.settingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_KeepCommand_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.KeepCommand = !this.settingsRoot.Send.KeepCommand;
		}

		private void toolStripMenuItem_SendContextMenu_CopyPredefined_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.CopyPredefined = !this.settingsRoot.Send.CopyPredefined;
		}

		private void toolStripMenuItem_SendContextMenu_SendImmediately_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.SendImmediately = !this.settingsRoot.Send.SendImmediately;
		}

		#endregion

		#region Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Status_Opening(object sender, CancelEventArgs e)
		{
			this.isSettingControls.Enter();

			bool isSerialPort   = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;
			bool isUsbSerialHid = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsUsbSerialHid;

			// Flow control count:
			bool showFlowControlOptions = (isSerialPort || (isUsbSerialHid && this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff));
			bool showFlowControlCount = this.settingsRoot.Status.ShowFlowControlCount;
			contextMenuStrip_Status_FlowControlCount.Enabled            = showFlowControlOptions;
			contextMenuStrip_Status_FlowControlCount_ShowCount.Visible  = showFlowControlOptions; // Workaround to .NET Windows.Forms bug (child items visible even when parent item is disabled).
			contextMenuStrip_Status_FlowControlCount_ShowCount.Checked  = showFlowControlCount;
			contextMenuStrip_Status_FlowControlCount_ResetCount.Visible = showFlowControlOptions; // Workaround to .NET Windows.Forms bug (child items visible even when parent item is disabled).
			contextMenuStrip_Status_FlowControlCount_ResetCount.Enabled = showFlowControlCount;

			// Break count:
			bool showBreakOptions = (isSerialPort && this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates);
			bool showBreakCount = this.settingsRoot.Status.ShowBreakCount;
			contextMenuStrip_Status_BreakCount.Enabled            = showBreakOptions;
			contextMenuStrip_Status_BreakCount_ShowCount.Visible  = showBreakOptions; // Workaround to .NET Windows.Forms bug (child items visible even when parent item is disabled).
			contextMenuStrip_Status_BreakCount_ShowCount.Checked  = showBreakCount;
			contextMenuStrip_Status_BreakCount_ResetCount.Visible = showBreakOptions; // Workaround to .NET Windows.Forms bug (child items visible even when parent item is disabled).
			contextMenuStrip_Status_BreakCount_ResetCount.Enabled = showBreakCount;

			this.isSettingControls.Leave();
		}

		private void contextMenuStrip_Status_FlowControlCount_ShowCount_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowFlowControlCount = !this.settingsRoot.Status.ShowFlowControlCount;
		}

		private void contextMenuStrip_Status_FlowControlCount_ResetCount_Click(object sender, EventArgs e)
		{
			this.terminal.ResetFlowControlCount();
		}

		private void contextMenuStrip_Status_BreakCount_ShowCount_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowBreakCount = !this.settingsRoot.Status.ShowBreakCount;
		}

		private void contextMenuStrip_Status_BreakCount_ResetCount_Click(object sender, EventArgs e)
		{
			this.terminal.ResetBreakCount();
		}

		#endregion

		#region Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------

		private void splitContainer_TxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!IsStartingUp && !this.isSettingControls && !IsClosing)
			{
				int widthOrHeight = 0;
				if (this.settingsRoot.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_TxMonitor.Width;
				else
					widthOrHeight = splitContainer_TxMonitor.Height;

				this.settingsRoot.Layout.TxMonitorSplitterRatio = (float)splitContainer_TxMonitor.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_RxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!IsStartingUp && !this.isSettingControls && !IsClosing)
			{
				int widthOrHeight = 0;
				if (this.settingsRoot.Layout.MonitorOrientation == Orientation.Vertical)
					widthOrHeight = splitContainer_RxMonitor.Width;
				else
					widthOrHeight = splitContainer_RxMonitor.Height;

				this.settingsRoot.Layout.RxMonitorSplitterRatio = (float)splitContainer_RxMonitor.SplitterDistance / widthOrHeight;
			}
		}

		private void splitContainer_Predefined_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!IsStartingUp && !this.isSettingControls && !IsClosing)
				this.settingsRoot.Layout.PredefinedSplitterRatio = (float)splitContainer_Predefined.SplitterDistance / splitContainer_Predefined.Width;
		}

		#endregion

		#region Controls Event Handlers > Monitor
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor
		//------------------------------------------------------------------------------------------

		private void monitor_Tx_Enter(object sender, EventArgs e)
		{
			// Remember which monitor has been activated last.
			this.lastMonitorSelection = Domain.RepositoryType.Tx;
		}

		private void monitor_Bidir_Enter(object sender, EventArgs e)
		{
			// Remember which monitor has been activated last.
			this.lastMonitorSelection = Domain.RepositoryType.Bidir;
		}

		private void monitor_Rx_Enter(object sender, EventArgs e)
		{
			// Remember which monitor has been activated last.
			this.lastMonitorSelection = Domain.RepositoryType.Rx;
		}

		#endregion

		#region Controls Event Handlers > Predefined
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Predefined
		//------------------------------------------------------------------------------------------

		private void predefined_SelectedPageChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
				this.settingsRoot.Implicit.Predefined.SelectedPage = predefined.SelectedPage;
		}

		private void predefined_SendCommandRequest(object sender, Model.Types.PredefinedCommandEventArgs e)
		{
			this.terminal.SendPredefined(e.Page, e.Command);
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

		private void send_TextCommandChanged(object sender, EventArgs e)
		{
			this.settingsRoot.Implicit.SendText.Command = send.TextCommand;
		}

		/// <remarks>
		/// Ensure that the edit shortcuts such as Ctrl-A are disabled while the send control is
		/// being edited.
		/// </remarks>
		private void send_EditFocusStateChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();
		}

		private void send_SendTextCommandRequest(object sender, EventArgs e)
		{
			this.terminal.SendText();
		}

		private void send_FileCommandChanged(object sender, EventArgs e)
		{
			this.settingsRoot.Implicit.SendFile.Command = send.FileCommand;
		}

		private void send_SendFileCommandRequest(object sender, EventArgs e)
		{
			this.terminal.SendFile();
		}

		private void send_SizeChanged(object sender, EventArgs e)
		{
			LayoutSend();
		}

		#endregion

		#region Controls Event Handlers > Status
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status
		//------------------------------------------------------------------------------------------

		private List<ToolStripStatusLabel> terminalStatusLabels;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Dictionary<ToolStripStatusLabel, string> terminalStatusLabels_DefaultText;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Dictionary<ToolStripStatusLabel, string> terminalStatusLabels_DefaultToolTipText;

		private void toolStripStatusLabel_TerminalStatus_Initialize()
		{
			this.terminalStatusLabels = new List<ToolStripStatusLabel>(12); // Preset the required capactiy to improve memory management.

			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_Separator1);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_RFR);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_CTS);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_DTR);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_DSR);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_DCD);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_Separator2);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_InputXOnXOff);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_OutputXOnXOff);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_Separator3);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_InputBreak);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_OutputBreak);

			this.terminalStatusLabels_DefaultText = new Dictionary<ToolStripStatusLabel, string>(9); // Preset the required capactiy to improve memory management.
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_RFR,           toolStripStatusLabel_TerminalStatus_RFR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_CTS,           toolStripStatusLabel_TerminalStatus_CTS.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DTR,           toolStripStatusLabel_TerminalStatus_DTR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DSR,           toolStripStatusLabel_TerminalStatus_DSR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DCD,           toolStripStatusLabel_TerminalStatus_DCD.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_InputXOnXOff,  toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_OutputXOnXOff, toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_InputBreak,    toolStripStatusLabel_TerminalStatus_InputBreak.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_OutputBreak,   toolStripStatusLabel_TerminalStatus_OutputBreak.Text);

			this.terminalStatusLabels_DefaultToolTipText = new Dictionary<ToolStripStatusLabel, string>(9); // Preset the required capactiy to improve memory management.
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_RFR,           toolStripStatusLabel_TerminalStatus_RFR.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_CTS,           toolStripStatusLabel_TerminalStatus_CTS.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_DTR,           toolStripStatusLabel_TerminalStatus_DTR.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_DSR,           toolStripStatusLabel_TerminalStatus_DSR.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_DCD,           toolStripStatusLabel_TerminalStatus_DCD.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_InputXOnXOff,  toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_OutputXOnXOff, toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_InputBreak,    toolStripStatusLabel_TerminalStatus_InputBreak.ToolTipText);
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_OutputBreak,   toolStripStatusLabel_TerminalStatus_OutputBreak.ToolTipText);
		}

		private void toolStripStatusLabel_TerminalStatus_IOStatus_Click(object sender, EventArgs e)
		{
			ShowTerminalSettings();
		}

		/// <remarks>
		/// Note that label must always be enabled to have the correct appearance, and can thus
		/// always be clicked. It is the responsibility of the underlying Request...() method
		/// to decide whether the request can currently be executed or not.
		/// </remarks>
		private void toolStripStatusLabel_TerminalStatus_RFR_Click(object sender, EventArgs e)
		{
			this.terminal.RequestToggleRfr();

			// Note that label must always be enabled to show the correct pro, and can therefore
		}

		/// <remarks>
		/// Note that label must always be enabled to have the correct appearance, and can thus
		/// always be clicked. It is the responsibility of the underlying Request...() method
		/// to decide whether the request can currently be executed or not.
		/// </remarks>
		private void toolStripStatusLabel_TerminalStatus_DTR_Click(object sender, EventArgs e)
		{
			this.terminal.RequestToggleDtr();
		}

		/// <remarks>
		/// Note that label must always be enabled to have the correct appearance, and can thus
		/// always be clicked. It is the responsibility of the underlying Request...() method
		/// to decide whether the request can currently be executed or not.
		/// </remarks>
		private void toolStripStatusLabel_TerminalStatus_InputXOnXOff_Click(object sender, EventArgs e)
		{
			this.terminal.RequestToggleInputXOnXOff();
		}

		/// <remarks>
		/// Note that label must always be enabled to have the correct appearance, and can thus
		/// always be clicked. It is the responsibility of the underlying Request...() method
		/// to decide whether the request can currently be executed or not.
		/// </remarks>
		private void toolStripStatusLabel_TerminalStatus_OutputBreak_Click(object sender, EventArgs e)
		{
			this.terminal.RequestToggleOutputBreak();
		}

		#endregion

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <summary>
		/// This is the automatically assigned terminal name. The name is either an incrementally
		/// assigned 'Terminal1', 'Terminal2',... or the file name once the terminal has been saved
		/// by the user, e.g. 'MyTerminal.yat'.
		/// </summary>
		public virtual string AutoName
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.AutoName);
				else
					return ("");
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsWritable
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.SettingsFileIsWritable);
				else
					return (true);
			}
		}

		/// <summary></summary>
		public virtual bool IsStopped
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.IsStopped);
				else
					return (true);
			}
		}

		/// <summary></summary>
		public virtual bool IsStarted
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.IsStarted);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool LogIsOn
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.LogIsOn);
				else
					return (false);
			}
		}

		/// <summary></summary>
		public virtual bool LogFileExists
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.LogFileExists);
				else
					return (false);
			}
		}

		private bool IsStartingUp
		{
			get { return (this.isStartingUp); }
		}

		private bool IsClosing
		{
			get { return (this.closingState != ClosingState.None); }
		}

		/// <summary></summary>
		public virtual void RevertClosingState()
		{
			this.closingState = ClosingState.None;
		}

		/// <summary></summary>
		public virtual Model.Terminal UnderlyingTerminal
		{
			get { return (this.terminal); }
		}

		#endregion

		#region Methods
		//------------------------------------------------------------------------------------------
		// Methods
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public virtual bool RequestSaveFile()
		{
			return (this.terminal.Save());
		}

		/// <summary></summary>
		public virtual bool RequestCloseFile()
		{
			return (this.terminal.Close());
		}

		/// <summary></summary>
		public virtual bool RequestStartTerminal()
		{
			return (this.terminal.StartIO());
		}

		/// <summary></summary>
		public virtual bool RequestStopTerminal()
		{
			return (this.terminal.StopIO());
		}

		/// <summary></summary>
		public virtual void RequestEditTerminalSettings()
		{
			ShowTerminalSettings();
		}

		/// <summary></summary>
		public virtual void RequestRadix(Domain.Radix radix)
		{
			this.settingsRoot.Display.TxRadix = radix;
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseVisible()
		{
			this.settingsRoot.AutoResponse.Visible = !this.settingsRoot.AutoResponse.Visible;
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseFromTrigger(Model.Types.TriggerEx trigger)
		{
			this.settingsRoot.AutoResponse.FromTrigger(trigger);
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseFromExplicitTriggerText(string triggerText)
		{
			this.settingsRoot.AutoResponse.FromExplicitTriggerText(triggerText);
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseFromResponse(Model.Types.AutoResponseEx response)
		{
			this.settingsRoot.AutoResponse.FromResponse(response);
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseFromExplicitResponseText(string responseText)
		{
			this.settingsRoot.AutoResponse.FromExplicitResponseText(responseText);
		}

		/// <summary></summary>
		public virtual void RequestClear()
		{
			this.terminal.ClearRepositories();
		}

		/// <summary></summary>
		public virtual void RequestRefresh()
		{
			this.terminal.ReloadRepositories();
		}

		/// <summary></summary>
		public virtual void RequestCopyToClipboard()
		{
			CopyMonitorToClipboard(GetMonitor(this.lastMonitorSelection));
		}

		/// <summary></summary>
		public virtual void RequestSaveToFile()
		{
			ShowSaveMonitorDialog(GetMonitor(this.lastMonitorSelection));
		}

		/// <summary></summary>
		public virtual void RequestPrint()
		{
			ShowPrintMonitorDialog(GetMonitor(this.lastMonitorSelection));
		}

		/// <summary></summary>
		public virtual void RequestEditLogSettings()
		{
			ShowLogSettings();
		}

		/// <summary></summary>
		public virtual void RequestSwitchLogOn()
		{
			this.terminal.SwitchLogOn();
		}

		/// <summary></summary>
		public virtual void RequestSwitchLogOff()
		{
			this.terminal.SwitchLogOff();
		}

		/// <summary></summary>
		public virtual void RequestOpenLogFile()
		{
			this.terminal.OpenLogFile();
		}

		/// <summary></summary>
		public virtual void RequestOpenLogDirectory()
		{
			this.terminal.OpenLogDirectory();
		}

		/// <summary></summary>
		public virtual void RequestEditFormatSettings()
		{
			ShowFormatSettings();
		}

		#endregion

		#region View
		//==========================================================================================
		// View
		//==========================================================================================

		private void FixContextMenus()
		{
			List<ContextMenuStrip> strips = new List<ContextMenuStrip>(6); // Preset the required capactiy to improve memory management.
			strips.Add(contextMenuStrip_Send);
			strips.Add(contextMenuStrip_Radix);
			strips.Add(contextMenuStrip_Monitor);
			strips.Add(contextMenuStrip_Predefined);
			strips.Add(contextMenuStrip_Preset);
			strips.Add(contextMenuStrip_Status);

			// Makes sure that context menus are at the right position upon first drop down. This is
			// a fix, it should be that way by default. However, due to some reasons, they sometimes
			// appear somewhere at the top-left corner of the screen if this fix isn't done.
			SuspendLayout();

			foreach (ContextMenuStrip strip in strips)
				strip.OwnerItem = null;

			ResumeLayout();

			// Also fix the issue with shortcuts defined in context menus:
			int itemCount = 0;
			foreach (ContextMenuStrip strip in strips)
				itemCount += strip.Items.Count;

			this.contextMenuStripShortcutWorkaround = new ContextMenuStripShortcutWorkaround(itemCount); // Preset the required capactiy to improve memory management.
			foreach (ContextMenuStrip strip in strips)
				this.contextMenuStripShortcutWorkaround.Add(strip);
		}

		private void InitializeControls()
		{
			toolStripMenuItem_TerminalMenu_View_Initialize();

			contextMenuStrip_Preset_Initialize();
			contextMenuStrip_Predefined_Initialize();
			contextMenuStrip_Monitor_Initialize();

			toolStripStatusLabel_TerminalStatus_Initialize();
		}

		private void ApplyWindowSettings()
		{
			SuspendLayout();
			WindowState = this.settingsRoot.Window.State;
			if (WindowState == FormWindowState.Normal)
			{
				StartPosition = FormStartPosition.Manual;
				Location      = this.settingsRoot.Window.Location;
				Size          = this.settingsRoot.Window.Size;
			}
			ResumeLayout();
		}

		private void SaveWindowSettings()
		{
			this.settingsRoot.Window.State = WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				this.settingsRoot.Window.Location = Location;
				this.settingsRoot.Window.Size = Size;
			}
		}

		private void ViewRearrange()
		{
			// Simply set defaults, settings event handler will then call LayoutTerminal():
			this.settingsRoot.Layout.SetDefaults();
		}

		private void LayoutTerminal()
		{
			this.isSettingControls.Enter();
			SuspendLayout();

			// splitContainer_Predefined:
			if (this.settingsRoot.Layout.PredefinedPanelIsVisible)
			{
				splitContainer_Predefined.Panel2Collapsed = false;
				splitContainer_Predefined.SplitterDistance = Int32Ex.Limit((int)(this.settingsRoot.Layout.PredefinedSplitterRatio * splitContainer_Predefined.Width), 0, (splitContainer_Predefined.Width - 1));
			}
			else
			{
				splitContainer_Predefined.Panel2Collapsed = true;
			}

			// splitContainer_TxMonitor and splitContainer_RxMonitor:
			// One of the panels MUST be visible, if none is visible, then bidir is shown anyway.
			bool txIsVisible = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
			bool bidirIsVisible = this.settingsRoot.Layout.BidirMonitorPanelIsVisible || (!this.settingsRoot.Layout.TxMonitorPanelIsVisible && !this.settingsRoot.Layout.RxMonitorPanelIsVisible);
			bool rxIsVisible = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

			// Orientation:
			Orientation orientation = this.settingsRoot.Layout.MonitorOrientation;
			splitContainer_TxMonitor.Orientation = orientation;
			splitContainer_RxMonitor.Orientation = orientation;

			// Tx split contains Tx and Bidir+Rx:
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

					splitContainer_TxMonitor.SplitterDistance = Int32Ex.Limit((int)(this.settingsRoot.Layout.TxMonitorSplitterRatio * widthOrHeight), 0, (widthOrHeight - 1));
				}
			}
			else
			{
				splitContainer_TxMonitor.Panel1Collapsed = true;
			}
			splitContainer_TxMonitor.Panel2Collapsed = !(bidirIsVisible || rxIsVisible);

			// Rx split contains Bidir and Rx:
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

					splitContainer_RxMonitor.SplitterDistance = Int32Ex.Limit((int)(this.settingsRoot.Layout.RxMonitorSplitterRatio * widthOrHeight), 0, (widthOrHeight - 1));
				}
			}
			else
			{
				splitContainer_RxMonitor.Panel1Collapsed = true;
			}
			splitContainer_RxMonitor.Panel2Collapsed = !rxIsVisible;

			// splitContainer_Terminal and splitContainer_Send:
			if (this.settingsRoot.Layout.SendTextPanelIsVisible || this.settingsRoot.Layout.SendFilePanelIsVisible)
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

			// Set send panel size depending on one or two sub-panels:
			if (this.settingsRoot.Layout.SendTextPanelIsVisible && this.settingsRoot.Layout.SendFilePanelIsVisible)
			{
				int height = 97;
				splitContainer_Terminal.Panel2MinSize = height;
				splitContainer_Terminal.SplitterDistance = Int32Ex.Limit(splitContainer_Terminal.Height - height - splitContainer_Terminal.SplitterWidth, 0, (splitContainer_Terminal.Height - 1));
			}
			else if (this.settingsRoot.Layout.SendTextPanelIsVisible || this.settingsRoot.Layout.SendFilePanelIsVisible)
			{
				int height = 48;
				splitContainer_Terminal.Panel2MinSize = height;
				splitContainer_Terminal.SplitterDistance = Int32Ex.Limit(splitContainer_Terminal.Height - height - splitContainer_Terminal.SplitterWidth, 0, (splitContainer_Terminal.Height - 1));
			}

			send.CommandPanelIsVisible = this.settingsRoot.Layout.SendTextPanelIsVisible;
			send.FilePanelIsVisible    = this.settingsRoot.Layout.SendFilePanelIsVisible;

			LayoutSend();

			ResumeLayout();
			this.isSettingControls.Leave();
		}

		private void LayoutSend()
		{
			// Calculate absolute splitter position (distance) of predefined splitter,
			// including offset to get send buttons pixel-accurate below predefined buttons:
			const int PredefinedOffset = 6; // 2 x margin of 3 (frame + buttons)
			int absoluteX = splitContainer_Predefined.SplitterDistance + splitContainer_Predefined.Left;
			int relativeX = absoluteX - send.Left + PredefinedOffset;
			send.SplitterDistance = Int32Ex.Limit(relativeX, 0, (send.Width - 1));
		}

		private void SetDisplayControls()
		{
			toolStripMenuItem_TerminalMenu_View_SetMenuItems();
			contextMenuStrip_Radix_SetMenuItems();
		}

		private void SetLayoutControls()
		{
			toolStripMenuItem_TerminalMenu_View_SetMenuItems();
			contextMenuStrip_Monitor_SetMenuItems();
		}

		private void SetPredefinedControls()
		{
			contextMenuStrip_Predefined_SetMenuItems(); // Ensure that shortcuts are activated.

			this.isSettingControls.Enter();
			predefined.TerminalIsReadyToSend = this.terminal.IsReadyToSend;
			this.isSettingControls.Leave();
		}

		private void SetPresetControls()
		{
			contextMenuStrip_Preset_SetMenuItems();
		}

		private void SetSendControls()
		{
			toolStripMenuItem_TerminalMenu_Send_SetMenuItems();
			contextMenuStrip_Send_SetMenuItems();

			this.isSettingControls.Enter();
			send.TerminalType           = this.settingsRoot.TerminalType;
			send.TextCommand            = this.settingsRoot.SendText.Command;
			send.RecentCommands         = this.settingsRoot.SendText.RecentCommands;
			send.FileCommand            = this.settingsRoot.SendFile.Command;
			send.RecentFileCommands     = this.settingsRoot.SendFile.RecentCommands;
			send.SendTextImmediately    = this.settingsRoot.Send.SendImmediately;
			send.TerminalIsReadyToSend  = this.terminal.IsReadyToSend;
			this.isSettingControls.Leave();
		}

		#endregion

		#region Preset
		//==========================================================================================
		// Preset
		//==========================================================================================

		/// <summary>
		/// Set requested preset. Currently, presets are fixed to those listed below.
		/// For future versions, presets could be defined and managed similarly to predefined
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

			MKY.IO.Serial.SerialPort.SerialCommunicationSettings settings = this.settingsRoot.Terminal.IO.SerialPort.Communication;
			settings.SuspendChangeEvent();
			switch (preset)
			{
				case 1: // "2400, 7, Even, 1, None"
				{
					settings.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud002400;
					settings.DataBits    = MKY.IO.Ports.DataBits.Seven;
					settings.Parity      = System.IO.Ports.Parity.Even;
					settings.StopBits    = System.IO.Ports.StopBits.One;
					settings.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
					break;
				}
				case 2: // "2400, 7, Even, 1, XOn/XOff"
				{
					settings.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud002400;
					settings.DataBits    = MKY.IO.Ports.DataBits.Seven;
					settings.Parity      = System.IO.Ports.Parity.Even;
					settings.StopBits    = System.IO.Ports.StopBits.One;
					settings.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
					break;
				}
				case 3: // "9600, 8, None, 1, None"
				{
					settings.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud009600;
					settings.DataBits    = MKY.IO.Ports.DataBits.Eight;
					settings.Parity      = System.IO.Ports.Parity.None;
					settings.StopBits    = System.IO.Ports.StopBits.One;
					settings.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
					break;
				}
				case 4: // "9600, 8, None, 1, XOn/XOff"
				{
					settings.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud009600;
					settings.DataBits    = MKY.IO.Ports.DataBits.Eight;
					settings.Parity      = System.IO.Ports.Parity.None;
					settings.StopBits    = System.IO.Ports.StopBits.One;
					settings.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
					break;
				}
				case 5: // "19200, 8, None, 1, None"
				{
					settings.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud019200;
					settings.DataBits    = MKY.IO.Ports.DataBits.Eight;
					settings.Parity      = System.IO.Ports.Parity.None;
					settings.StopBits    = System.IO.Ports.StopBits.One;
					settings.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
					break;
				}
				case 6: // "19200, 8, None, 1, XOn/XOff"
				{
					settings.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud019200;
					settings.DataBits    = MKY.IO.Ports.DataBits.Eight;
					settings.Parity      = System.IO.Ports.Parity.None;
					settings.StopBits    = System.IO.Ports.StopBits.One;
					settings.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
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
			return (GetMonitor(GetMonitorType(source)));
		}

		private Controls.Monitor GetMonitor(Domain.RepositoryType type)
		{
			switch (type)
			{
				case Domain.RepositoryType.Tx:    return (monitor_Tx);
				case Domain.RepositoryType.Bidir: return (monitor_Bidir);
				case Domain.RepositoryType.Rx:    return (monitor_Rx);
				default:                          return (null);
			}
		}

		#endregion

		#region Monitor Panels > View
		//------------------------------------------------------------------------------------------
		// Monitor Panels > View
		//------------------------------------------------------------------------------------------

		private void SetMonitorRadix(Domain.Radix radix)
		{
			SetMonitorRadix(Domain.IODirection.Tx, radix);
		}

		private void SetMonitorRadix(Domain.IODirection direction, Domain.Radix radix)
		{
			switch (direction)
			{
				case Domain.IODirection.Tx: this.settingsRoot.Display.TxRadix = radix; break;
				case Domain.IODirection.Rx: this.settingsRoot.Display.RxRadix = radix; break;
				default: throw (new NotSupportedException("Program execution should never get here, '" + direction + "' is an invalid direction." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void SetMonitorOrientation(Orientation orientation)
		{
			this.settingsRoot.Layout.MonitorOrientation = orientation;

			SuspendLayout();
			splitContainer_TxMonitor.Orientation = orientation;
			splitContainer_RxMonitor.Orientation = orientation;
			ResumeLayout();
		}

		private void SetMonitorLineCount()
		{
			monitor_Tx.MaxLineCount    = this.settingsRoot.Display.TxMaxLineCount;
			monitor_Bidir.MaxLineCount = this.settingsRoot.Display.BidirMaxLineCount;
			monitor_Rx.MaxLineCount    = this.settingsRoot.Display.RxMaxLineCount;
		}

		private void SetMonitorLineNumbers()
		{
			bool showLineNumbers = this.settingsRoot.Display.ShowLineNumbers;
			monitor_Tx.ShowLineNumbers    = showLineNumbers;
			monitor_Bidir.ShowLineNumbers = showLineNumbers;
			monitor_Rx.ShowLineNumbers    = showLineNumbers;
		}

		private void SetMonitorIOStatus()
		{
			MonitorActivityState activityState = MonitorActivityState.Inactive;
			if (TerminalIsAvailable)
			{
				if (this.terminal.IsStarted)
				{
					if (this.terminal.IsConnected)
						activityState = MonitorActivityState.Active;
					else
						activityState = MonitorActivityState.Pending;
				}
			}
			monitor_Tx.ActivityState    = activityState;
			monitor_Bidir.ActivityState = activityState;
			monitor_Rx.ActivityState    = activityState;
		}

		private void SetMonitorCountAndRateStatus()
		{
			bool showConnectTime = this.settingsRoot.Status.ShowConnectTime;
			monitor_Tx.ShowTimeStatus    = showConnectTime;
			monitor_Bidir.ShowTimeStatus = showConnectTime;
			monitor_Rx.ShowTimeStatus    = showConnectTime;

			bool showCountAndRate = this.settingsRoot.Status.ShowCountAndRate;
			monitor_Tx.ShowDataStatus    = showCountAndRate;
			monitor_Bidir.ShowDataStatus = showCountAndRate;
			monitor_Rx.ShowDataStatus    = showCountAndRate;
		}

		private void ReloadMonitors()
		{
			SetFixedStatusText("Reloading...");
			Cursor = Cursors.WaitCursor;

			this.terminal.ReloadRepositories();

			Cursor = Cursors.Default;
			SetTimedStatusText("Reloading done");
		}

		private void ReformatMonitors()
		{
			SetFixedStatusText("Reformatting...");
			Cursor = Cursors.WaitCursor;

			monitor_Tx.FormatSettings    = this.settingsRoot.Format;
			monitor_Bidir.FormatSettings = this.settingsRoot.Format;
			monitor_Rx.FormatSettings    = this.settingsRoot.Format;

			Cursor = Cursors.Default;
			SetTimedStatusText("Reformatting done");
		}

		private void ClearMonitor(Domain.RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case Domain.RepositoryType.Tx:
				case Domain.RepositoryType.Bidir:
				case Domain.RepositoryType.Rx:
					this.terminal.ClearRepository(repositoryType);
					break;

				default:
					throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Program execution should never get here, '" + repositoryType + "' is an invalid repository type." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void RefreshMonitor(Domain.RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case Domain.RepositoryType.Tx:
				case Domain.RepositoryType.Bidir:
				case Domain.RepositoryType.Rx:
					this.terminal.ReloadRepository(repositoryType);
					break;

				default:
					throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, "Program execution should never get here, '" + repositoryType + "' is an invalid repository type." + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void ClearMonitors()
		{
			this.terminal.ClearRepositories();
		}

		private void RefreshMonitors()
		{
			this.terminal.ReloadRepositories();
		}

		#endregion

		#region Monitor Panels > Methods
		//------------------------------------------------------------------------------------------
		// Monitor Panels > Methods
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowFormatSettings()
		{
			FormatSettings f = new FormatSettings(this.settingsRoot.Format, this.settingsRoot.Display.InfoSeparator, this.settingsRoot.Display.InfoEnclosure);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				this.settingsRoot.Format = f.FormatSettingsResult;
				this.settingsRoot.Display.InfoSeparator = f.InfoSeparatorResult.ToSeparator();
				this.settingsRoot.Display.InfoEnclosure = f.InfoEnclosureResult.ToEnclosure();
			}
		}

		private static void SelectAllMonitorContents(Controls.Monitor monitor)
		{
			monitor.SelectAll();
		}

		private static void SelectNoneMonitorContents(Controls.Monitor monitor)
		{
			monitor.SelectNone();
		}

		private void CopyMonitorToClipboard(Controls.Monitor monitor)
		{
			SetFixedStatusText("Copying data to clipboard...");
			Model.Utilities.RtfWriterHelper.LinesToClipboard(monitor.SelectedLines, this.settingsRoot.Format);
			SetTimedStatusText("Data copied to clipboard");
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSaveMonitorDialog(Controls.Monitor monitor)
		{
			SetFixedStatusText("Preparing to save data...");

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save As";
			string initialExtension = ApplicationSettings.LocalUserSettings.Extensions.MonitorFiles;
			sfd.Filter      = ExtensionHelper.TextFilesFilter;
			sfd.FilterIndex = ExtensionHelper.TextFilesFilterHelper(initialExtension);
			sfd.DefaultExt  = PathEx.DenormalizeExtension(initialExtension);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MonitorFiles;

			DialogResult dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Extensions.MonitorFiles = Path.GetExtension(sfd.FileName);
				ApplicationSettings.LocalUserSettings.Paths.MonitorFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.Save();

				SaveMonitor(monitor, sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		[ModalBehavior(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an explicit user interaction.")]
		private void SaveMonitor(Controls.Monitor monitor, string filePath)
		{
			SetFixedStatusText("Saving selected lines...");
			try
			{
				int requestedCount = monitor.SelectedLines.Count;
				int savedCount;

				if (ExtensionHelper.IsXmlFile(filePath))
				{
#if FALSE // Enable to use the raw instead of neat XML export schema, useful for development purposes of the raw XML schema.
					savedCount = Model.Utilities.XmlWriterHelperRaw.LinesToFile(monitor.SelectedLines, filePath, true);
#else
					savedCount = Model.Utilities.XmlWriterHelperNeat.LinesToFile(monitor.SelectedLines, filePath, true);
#endif
				}
				else if (ExtensionHelper.IsRtfFile(filePath))
				{
					savedCount = Model.Utilities.RtfWriterHelper.LinesToFile(monitor.SelectedLines, filePath, this.settingsRoot.Format);
				}
				else
				{
					savedCount = Model.Utilities.TextWriterHelper.LinesToFile(monitor.SelectedLines, filePath, this.settingsRoot.Format);
				}

				if (savedCount == requestedCount)
				{
					SetTimedStatusText("Selected lines successfully saved");
				}
				else
				{
					SetFixedStatusText("Selected lines only partially saved!");

					StringBuilder sb = new StringBuilder();

					sb.Append("Selected lines only partially saved to file, only ");
					sb.Append(savedCount.ToString(CultureInfo.InvariantCulture));
					sb.Append(" instead of ");
					sb.Append(requestedCount.ToString(CultureInfo.InvariantCulture));
					sb.AppendLine(" could be saved");
					sb.AppendLine();

					sb.Append("This issue should never happen! ");
					sb.Append(MessageHelper.SubmitBug);
					sb.Append(" Attach a screenshot of the monitor as well as the partially saved file to the submitted bug, thanks!");

					MessageBoxEx.Show
					(
						this,
						sb.ToString(),
						"File Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);

					ResetStatusText();
				}
			}
			catch (IOException e)
			{
				SetFixedStatusText("Selected lines not saved!");

				string message =
					"Unable to save selected lines to file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					"System error message:"                 + Environment.NewLine + e.Message;

				MessageBoxEx.Show
				(
					this,
					message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				ResetStatusText();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowPrintMonitorDialog(Controls.Monitor monitor)
		{
			SetFixedStatusText("Preparing to print data...");

			PrintDialog pd = new PrintDialog();
			pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();

			// Note that the PrintDialog class may not work on AMD64 microprocessors unless you set the UseEXDialog property to true (MSDN):
			if (EnvironmentEx.IsWindows64)
				pd.UseEXDialog = true;

			if (pd.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				PrintMonitor(monitor, pd.PrinterSettings);
			}
			else
			{
				ResetStatusText();
			}
		}

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Emphasize line breaks.")]
		private void PrintMonitor(Controls.Monitor monitor, System.Drawing.Printing.PrinterSettings settings)
		{
			SetFixedStatusText("Printing data...");

			using (Model.Utilities.RtfPrinter printer = new Model.Utilities.RtfPrinter(settings))
			{
				try
				{
					printer.Print(monitor.SelectedLines, this.settingsRoot.Format);
					SetTimedStatusText("Data printed");
				}
				catch (System.Drawing.Printing.InvalidPrinterException ex)
				{
					SetFixedStatusText("Data not printed!");

					string message =
						"Unable to print data!" + Environment.NewLine + Environment.NewLine +
						"System error message:" + Environment.NewLine + ex.Message;

					MessageBoxEx.Show
					(
						this,
						message,
						"Print Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);

					ResetStatusText();
				}
			}
		}

#endregion

#endregion

		#region Predefined Panel
		//==========================================================================================
		// Predefined Panel
		//==========================================================================================

		private void SelectPredefinedPanel()
		{
			predefined.Select();
		}

		/// <param name="command">Command 1..max.</param>
		private void CopyPredefined(int command)
		{
			int page = predefined.SelectedPage;
			if (!this.terminal.CopyPredefined(page, command))
			{
				// If command is not valid, show settings dialog.
				ShowPredefinedCommandSettings(page, command);
			}
		}

		/// <param name="command">Command 1..max.</param>
		private void SendPredefined(int command)
		{
			if (this.terminal.IsReadyToSend)
			{
				int page = predefined.SelectedPage;
				if (!this.terminal.SendPredefined(page, command))
				{
					// If command is not valid, show settings dialog.
					ShowPredefinedCommandSettings(page, command);
				}
			}
		}

		/// <summary>
		/// Fixes issue described in bug #300 "Modal forms can be opened multiple times":
		/// When no commands were defined yet, pressing Shift+F1 opens the 'Predefined' form, so far so good.
		/// However, when pressing Shift+F1 again (while the 'Predefined' form is still open), the form opens a second time!
		/// 
		/// Saying hello to StyleCop ;-.
		/// </summary>
		/// <remarks>
		/// Simple bool without any interlocked or monitor protection is totally sufficient,
		/// as this method will always be called from the main thread.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'formIsOpen' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Boolean just *is* 'bool'...")]
		private bool ShowPredefinedCommandSettings_formIsOpen = false;

		/// <param name="page">Page 1..max.</param>
		/// <param name="command">Command 1..max.</param>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowPredefinedCommandSettings(int page, int command)
		{
			if (!ShowPredefinedCommandSettings_formIsOpen)
			{
				ShowPredefinedCommandSettings_formIsOpen = true;

				PredefinedCommandSettings f = new PredefinedCommandSettings(this.settingsRoot.PredefinedCommand, this.settingsRoot.TerminalType, this.settingsRoot.Send.ToParseMode(), page, command);
				if (f.ShowDialog(this) == DialogResult.OK)
				{
					Refresh();
					this.settingsRoot.PredefinedCommand = f.SettingsResult;
					this.settingsRoot.Predefined.SelectedPage = f.SelectedPage;
				}

				ShowPredefinedCommandSettings_formIsOpen = false;
			}
		}

		#endregion

		#region Send Panel
		//==========================================================================================
		// Send Panel
		//==========================================================================================

		private void SelectSendTextInput()
		{
			send.SelectSendTextInput();
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
			if (this.settingsRoot != null)
				this.settingsRoot.Changed += new EventHandler<SettingsEventArgs>(settingsRoot_Changed);
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= new EventHandler<SettingsEventArgs>(settingsRoot_Changed);
		}

		#endregion

		#region Settings > Event Handlers
		//------------------------------------------------------------------------------------------
		// Settings > Event Handlers
		//------------------------------------------------------------------------------------------

		private void settingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			SetTerminalCaption();

			if (e.Inner == null)
			{
				// Nothing to do, no need to care about 'ProductVersion' and such.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Explicit))
			{
				HandleExplicitSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Implicit))
			{
				HandleImplicitSettings(e.Inner);
			}

			OnTerminalChanged(EventArgs.Empty);
		}

		private void HandleExplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal))
			{
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.PredefinedCommand))
			{
				this.isSettingControls.Enter();
				predefined.Pages = this.settingsRoot.PredefinedCommand.Pages;
				this.isSettingControls.Leave();

				SetPredefinedControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Format))
			{
				ReformatMonitors();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Log))
			{
				SetLogControls();
			}
		}

		private void HandleTerminalSettings(SettingsEventArgs e)
		{
			if (this.handlingTerminalSettingsIsSuspended)
				return;

			if (e.Inner == null)
			{
				SetIOStatus();
				SetIOControlControls();
				SetMonitorIOStatus();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.IO))
			{
				SetIOStatus();
				SetIOControlControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Status))
			{
				SetMonitorCountAndRateStatus();
				SetIOControlControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Display))
			{
				SetMonitorLineCount();
				SetMonitorLineNumbers();
				SetDisplayControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Send))
			{
				SetSendControls();
			}
		}

		private void HandleImplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				SetTerminalControls();
				SetLogControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendText))
			{
				SetSendControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendFile))
			{
				SetSendControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Predefined))
			{
				this.isSettingControls.Enter();
				predefined.SelectedPage = this.settingsRoot.Predefined.SelectedPage;
				this.isSettingControls.Leave();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Window))
			{
				// Nothing to do, windows settings are only saved.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Layout))
			{
				LayoutTerminal();
				SetLayoutControls();
			}
		}

		#endregion

		#region Settings > Suspend
		//------------------------------------------------------------------------------------------
		// Settings > Suspend
		//------------------------------------------------------------------------------------------

		private void SuspendHandlingTerminalSettings()
		{
			this.handlingTerminalSettingsIsSuspended = true;
		}

		private void ResumeHandlingTerminalSettings()
		{
			this.handlingTerminalSettingsIsSuspended = false;

			SetIOStatus();
			SetIOControlControls();
			SetMonitorLineCount();
			SetMonitorLineNumbers();
			SetMonitorIOStatus();
			SetMonitorCountAndRateStatus();
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
			if (this.terminal != null)
			{
				this.terminal.IOChanged            += new EventHandler(terminal_IOChanged);
				this.terminal.IOControlChanged     += new EventHandler(terminal_IOControlChanged);
				this.terminal.IOConnectTimeChanged += new EventHandler<TimeSpanEventArgs>(terminal_IOConnectTimeChanged);
				this.terminal.IOCountChanged       += new EventHandler(terminal_IOCountChanged);
				this.terminal.IORateChanged        += new EventHandler(terminal_IORateChanged);
				this.terminal.IOError              += new EventHandler<Domain.IOErrorEventArgs>(terminal_IOError);

				this.terminal.DisplayElementsSent     += new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsSent);
				this.terminal.DisplayElementsReceived += new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsReceived);

				this.terminal.RepositoryCleared  += new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryCleared);
				this.terminal.RepositoryReloaded += new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryReloaded);

				this.terminal.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(terminal_TimedStatusTextRequest);
				this.terminal.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(terminal_FixedStatusTextRequest);
				this.terminal.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(terminal_MessageInputRequest);

				this.terminal.SaveAsFileDialogRequest += new EventHandler<Model.DialogEventArgs>(terminal_SaveAsFileDialogRequest);

				this.terminal.Saved  += new EventHandler<Model.SavedEventArgs>(terminal_Saved);
				this.terminal.Closed += new EventHandler<Model.ClosedEventArgs>(terminal_Closed);
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged            -= new EventHandler(terminal_IOChanged);
				this.terminal.IOControlChanged     -= new EventHandler(terminal_IOControlChanged);
				this.terminal.IOConnectTimeChanged -= new EventHandler<TimeSpanEventArgs>(terminal_IOConnectTimeChanged);
				this.terminal.IOCountChanged       -= new EventHandler(terminal_IOCountChanged);
				this.terminal.IORateChanged        -= new EventHandler(terminal_IORateChanged);
				this.terminal.IOError              -= new EventHandler<Domain.IOErrorEventArgs>(terminal_IOError);

				this.terminal.DisplayElementsSent     -= new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsSent);
				this.terminal.DisplayElementsReceived -= new EventHandler<Domain.DisplayElementsEventArgs>(terminal_DisplayElementsReceived);

				this.terminal.RepositoryCleared  -= new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryCleared);
				this.terminal.RepositoryReloaded -= new EventHandler<Domain.RepositoryEventArgs>(terminal_RepositoryReloaded);

				this.terminal.TimedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(terminal_TimedStatusTextRequest);
				this.terminal.FixedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(terminal_FixedStatusTextRequest);
				this.terminal.MessageInputRequest    -= new EventHandler<Model.MessageInputEventArgs>(terminal_MessageInputRequest);

				this.terminal.SaveAsFileDialogRequest -= new EventHandler<Model.DialogEventArgs>(terminal_SaveAsFileDialogRequest);

				this.terminal.Saved  -= new EventHandler<Model.SavedEventArgs>(terminal_Saved);
				this.terminal.Closed -= new EventHandler<Model.ClosedEventArgs>(terminal_Closed);
			}
		}

		private bool TerminalIsAvailable
		{
			get { return ((this.terminal != null) && (!this.terminal.IsDisposed)); }
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IOChanged(object sender, EventArgs e)
		{
			SetTerminalControls();

			ResetStatusText();
			OnTerminalChanged(EventArgs.Empty);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IOControlChanged(object sender, EventArgs e)
		{
			SetIOControlControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IOConnectTimeChanged(object sender, TimeSpanEventArgs e)
		{
			// Ensure not to handle event during closing anymore. Check 'IsDisposed' first!
			if (!IsDisposed && TerminalIsAvailable)
			{
				TimeSpan activeConnectTime;
				TimeSpan totalConnectTime;

				this.terminal.GetConnectTime(out activeConnectTime, out totalConnectTime);

				monitor_Tx   .SetTimeStatus(activeConnectTime, totalConnectTime);
				monitor_Bidir.SetTimeStatus(activeConnectTime, totalConnectTime);
				monitor_Rx   .SetTimeStatus(activeConnectTime, totalConnectTime);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IOCountChanged(object sender, EventArgs e)
		{
			// Ensure not to handle event during closing anymore. Check 'IsDisposed' first!
			if (!IsDisposed && TerminalIsAvailable)
			{
				int txByteCount = 0;
				int txLineCount = 0;
				int rxByteCount = 0;
				int rxLineCount = 0;

				this.terminal.GetDataCount(out txByteCount, out txLineCount, out rxByteCount, out rxLineCount);

				monitor_Tx   .SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);
				monitor_Bidir.SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);
				monitor_Rx   .SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IORateChanged(object sender, EventArgs e)
		{
			// Ensure not to handle event during closing anymore. Check 'IsDisposed' first!
			if (!IsDisposed && TerminalIsAvailable)
			{
				int txByteRate = 0;
				int txLineRate = 0;
				int rxByteRate = 0;
				int rxLineRate = 0;

				this.terminal.GetDataRate(out txByteRate, out txLineRate, out rxByteRate, out rxLineRate);

				monitor_Tx   .SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
				monitor_Bidir.SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
				monitor_Rx   .SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "StartArgs are considered to decide on behavior.")]
		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(EventArgs.Empty);

			bool showErrorModally = false;
			var main = (this.mdiParent as Main);
			if (main != null)
				showErrorModally = main.UnderlyingMain.StartArgs.KeepOpenOnError;

			if (e.Severity == Domain.IOErrorSeverity.Acceptable) // Handle acceptable issues.
			{
				SetTimedStatusText("Terminal Warning");

				if (showErrorModally)
				{
					MessageBoxEx.Show
					(
						this,
						e.Message,
						"Terminal Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}
			}
			else
			{
				SetFixedStatusText("Terminal Error");

				if (showErrorModally)
				{
					MessageBoxEx.Show
					(
						this,
						e.Message,
						"Terminal Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			monitor_Tx   .AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.
			monitor_Bidir.AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			monitor_Bidir.AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.
			monitor_Rx   .AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_RepositoryCleared(object sender, Domain.RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case Domain.RepositoryType.Tx:    monitor_Tx.Clear();    break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.Clear(); break;
				case Domain.RepositoryType.Rx:    monitor_Rx.Clear();    break;
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_RepositoryReloaded(object sender, Domain.RepositoryEventArgs e)
		{
			switch (e.Repository)
			{
				case Domain.RepositoryType.Tx:    monitor_Tx.AddLines   (this.terminal.RepositoryToDisplayLines(Domain.RepositoryType.Tx));    break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.AddLines(this.terminal.RepositoryToDisplayLines(Domain.RepositoryType.Bidir)); break;
				case Domain.RepositoryType.Rx:    monitor_Rx.AddLines   (this.terminal.RepositoryToDisplayLines(Domain.RepositoryType.Rx));    break;
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveTerminalAsFileDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_Saved(object sender, Model.SavedEventArgs e)
		{
			SetTerminalControls();
			SelectSendTextInput();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_Closed(object sender, Model.ClosedEventArgs e)
		{
			// Prevent multiple calls to Close().
			if (this.closingState == ClosingState.None)
			{
				this.closingState = ClosingState.IsClosingFromModel;
				Close();
			}
		}

		#endregion

		#region Terminal > Methods
		//------------------------------------------------------------------------------------------
		// Terminal > Methods
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private DialogResult ShowSaveTerminalAsFileDialog()
		{
			SetFixedStatusText("Saving terminal as...");

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save " + AutoName + " As";
			sfd.Filter      = ExtensionHelper.TerminalFilesFilter;
			sfd.FilterIndex = ExtensionHelper.TerminalFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.TerminalFile);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;

			// Check wether the terminal has already been saved as a .yat file.
			if (StringEx.EndsWithOrdinalIgnoreCase(AutoName, ExtensionHelper.TerminalFile))
				sfd.FileName = AutoName;
			else
				sfd.FileName = AutoName + PathEx.NormalizeExtension(sfd.DefaultExt);
				// Note that 'DefaultExt' states "The returned string does not include the period."!

			DialogResult dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.Save();

				this.terminal.SaveAs(sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}
			SelectSendTextInput();
			return (dr);
		}

		#endregion

		#region Terminal > Settings
		//------------------------------------------------------------------------------------------
		// Terminal > Settings
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowTerminalSettings()
		{
			SetFixedStatusText("Terminal Settings...");

			// Clone settings to ensure that settings result is a different object than the original settings.
			ExplicitSettings clone = new ExplicitSettings(this.settingsRoot.Explicit);
			TerminalSettings f = new TerminalSettings(clone);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				Settings.Terminal.ExplicitSettings s = f.SettingsResult;
				if (s.HaveChanged)
				{
					SuspendHandlingTerminalSettings();
					this.terminal.SetSettings(s);
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

			SelectSendTextInput();
		}

		#endregion

		#region Terminal > View
		//------------------------------------------------------------------------------------------
		// Terminal > View
		//------------------------------------------------------------------------------------------

		private void SetTerminalControls()
		{
			// Terminal menu.
			toolStripMenuItem_TerminalMenu_File_SetMenuItems();
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();

			// Terminal panel.
			SetTerminalCaption();
			SetIOStatus();
			SetIOControlControls();
			SetMonitorIOStatus();

			// Send.
			SetSendControls();

			// Predefined.
			SetPredefinedControls();

			// Preset.
			SetPresetControls();
		}

		private void SetTerminalCaption()
		{
			if (TerminalIsAvailable)
				Text = this.terminal.Caption;
		}

		private void SetIOStatus()
		{
			Image on  = Properties.Resources.Image_Status_Green_12x12;
			Image off = Properties.Resources.Image_Status_Red_12x12;

			if (TerminalIsAvailable)
			{
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Enabled =  this.terminal.IsStarted;

				if (this.terminal.IsOpen)
				{
					if (this.terminal.IsTransmissive)
					{
						if (this.terminal.IsReadyToSend)
						{
							ResetIOStatusFlashing();
							toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

							if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != on) // Improve performance by only assigning if different.
								toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = on;
						}
						else // sending is ongoing
						{
							toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Flashing;
							//// Do not directly access the image, it will be flashed by the timer below.
							//// Directly accessing the image could result in irregular flashing.
							StartIOStatusFlashing();
						}
					}
					else // can only receive (so far)
					{
						ResetIOStatusFlashing();
						toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

						if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != on) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = on;
					}
				}
				else // is closed
				{
					ResetIOStatusFlashing();
					toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

					if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != off) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = off;
				}

				toolStripStatusLabel_TerminalStatus_IOStatus.Text = this.terminal.IOStatusText;
			}
			else
			{
				ResetIOStatusFlashing();
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Enabled = false;
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

				if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != off) // Improve performance by only assigning if different.
					toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = off;

				toolStripStatusLabel_TerminalStatus_IOStatus.Text = "";
			}
		}

		private void StartIOStatusFlashing()
		{
			timer_IOStatusIndicator.Enabled = true;
		}

		private void timer_IOStatusIndicator_Tick(object sender, EventArgs e)
		{
			if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag != null)
			{
				// Only handle the image if flashing is desired:
				IOStatusIndicatorControl tag = (IOStatusIndicatorControl)toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag;
				if (tag == IOStatusIndicatorControl.Flashing)
				{
					ioStatusIndicatorFlashingIsOn = !ioStatusIndicatorFlashingIsOn; // Toggle flashing phase (initially 'false').

					Image onPhase  = Properties.Resources.Image_Status_Green_12x12;
					Image offPhase = Properties.Resources.Image_Status_Grey_12x12;

					Image phase = (ioStatusIndicatorFlashingIsOn ? onPhase : offPhase);
					if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != phase) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = phase;
				}
			}
		}

		private void ResetIOStatusFlashing()
		{
			timer_IOStatusIndicator.Enabled = false;
			ioStatusIndicatorFlashingIsOn = false; // Reset flashing phase (initially 'false').
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void SetIOControlControls()
		{
			SuspendLayout(); // Prevent flickering when visibility of status labels temporarily changes.

			Image on  = Properties.Resources.Image_Status_Green_12x12;
			Image off = Properties.Resources.Image_Status_Red_12x12;

			bool isOpen = this.terminal.IsOpen;

			bool isSerialPort   = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;
			bool isUsbSerialHid = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsUsbSerialHid;

			if (isSerialPort)
			{
				foreach (ToolStripStatusLabel sl in this.terminalStatusLabels)
				{
					sl.Visible = true;
					sl.Enabled = isOpen;
				}

				foreach (KeyValuePair<ToolStripStatusLabel, string> kvp in this.terminalStatusLabels_DefaultText)
					kvp.Key.Text = kvp.Value;

				foreach (KeyValuePair<ToolStripStatusLabel, string> kvp in this.terminalStatusLabels_DefaultToolTipText)
					kvp.Key.ToolTipText = kvp.Value;

				if (this.settingsRoot.Terminal.Status.ShowFlowControlCount)
				{
					MKY.IO.Ports.SerialPortControlPinCount pinCount = this.terminal.SerialPortControlPinCount;

					toolStripStatusLabel_TerminalStatus_RFR.Text += (" | " + pinCount.RfrDisableCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_CTS.Text += (" | " + pinCount.CtsDisableCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_DTR.Text += (" | " + pinCount.DtrDisableCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_DSR.Text += (" | " + pinCount.DsrDisableCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_DCD.Text += (" | " + pinCount.DcdCount.ToString(CultureInfo.CurrentCulture));

					toolStripStatusLabel_TerminalStatus_RFR.ToolTipText += (" | RFR Disable Count");
					toolStripStatusLabel_TerminalStatus_CTS.ToolTipText += (" | CTS Disable Count");
					toolStripStatusLabel_TerminalStatus_DTR.ToolTipText += (" | DTR Disable Count");
					toolStripStatusLabel_TerminalStatus_DSR.ToolTipText += (" | DSR Disable Count");
					toolStripStatusLabel_TerminalStatus_DCD.ToolTipText += (" | DCD Count");

					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text  += (" | " + this.terminal.SentXOnCount.ToString(CultureInfo.CurrentCulture)     + " | " + this.terminal.SentXOffCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text += (" | " + this.terminal.ReceivedXOnCount.ToString(CultureInfo.CurrentCulture) + " | " + this.terminal.ReceivedXOffCount.ToString(CultureInfo.CurrentCulture));

					toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText  += (" | XOn Count | XOff Count");
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText += (" | XOn Count | XOff Count");
				}

				if (this.settingsRoot.Terminal.Status.ShowBreakCount)
				{
					toolStripStatusLabel_TerminalStatus_InputBreak.Text  += (" | " + this.terminal.InputBreakCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_OutputBreak.Text += (" | " + this.terminal.OutputBreakCount.ToString(CultureInfo.CurrentCulture));

					toolStripStatusLabel_TerminalStatus_InputBreak.ToolTipText  += (" | Input Break Count");
					toolStripStatusLabel_TerminalStatus_OutputBreak.ToolTipText += (" | Output Break Count");
				}

				if (isOpen)
				{
					var pins = new MKY.IO.Ports.SerialPortControlPins();
					bool inputBreak = false;
					bool outputBreak = false;

					var port = (this.terminal.UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
					if (port != null)
					{
						try // Fail-safe implementation, especially catching exceptions while closing.
						{
							pins        = port.ControlPins;
							inputBreak  = port.InputBreak;
							outputBreak = port.OutputBreak;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Failed to retrieve control pin state");
						}
					}
					else
					{
						throw (new InvalidOperationException("Program execution should never get here, the underlying I/O instance is no serial COM port!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}

					bool allowXOnXOff    = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
					bool indicateXOnXOff = allowXOnXOff; // Indication only works if manual XOn/XOff (bug #214).
					bool outputIsXOn     = false;
					bool inputIsXOn      = false;

					var x = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
					if (x != null)
					{
						try // Fail-safe implementation, especially catching exceptions while closing.
						{
						////indicateXOnXOff = x.XOnXOffIsInUse; >> See above (bug #214).
							outputIsXOn     = x.OutputIsXOn;
							inputIsXOn      = x.InputIsXOn;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Failed to retrieve XOn/XOff state");
						}
					}

					if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.SerialPort.SerialFlowControl.RS485)
					{
						if (pins.Rfr)
							TriggerRfrLuminescence();
					}
					else
					{
						Image rfrImage = (pins.Rfr ? on : off);

						if (toolStripStatusLabel_TerminalStatus_RFR.Image != rfrImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_RFR.Image = rfrImage;
					}

					Image ctsImage = (pins.Cts ? on : off);
					Image dtrImage = (pins.Dtr ? on : off);
					Image dsrImage = (pins.Dsr ? on : off);
					Image dcdImage = (pins.Dcd ? on : off);

					if (toolStripStatusLabel_TerminalStatus_CTS.Image != ctsImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_CTS.Image = ctsImage;

					if (toolStripStatusLabel_TerminalStatus_DTR.Image != dtrImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_DTR.Image = dtrImage;

					if (toolStripStatusLabel_TerminalStatus_DSR.Image != dsrImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_DSR.Image = dsrImage;

					if (toolStripStatusLabel_TerminalStatus_DCD.Image != dcdImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_DCD.Image = dcdImage;

					bool allowRfr = !this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesRfrCtsAutomatically;
					bool allowDtr = !this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically;

					Color rfrForeColor = (allowRfr ? SystemColors.ControlText : SystemColors.GrayText);
					Color dtrForeColor = (allowDtr ? SystemColors.ControlText : SystemColors.GrayText);

					if (toolStripStatusLabel_TerminalStatus_RFR.ForeColor != rfrForeColor) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_RFR.ForeColor = rfrForeColor;

					if (toolStripStatusLabel_TerminalStatus_CTS.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_CTS.ForeColor = SystemColors.GrayText;

					if (toolStripStatusLabel_TerminalStatus_DTR.ForeColor != dtrForeColor) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_DTR.ForeColor = dtrForeColor;

					if (toolStripStatusLabel_TerminalStatus_DSR.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_DSR.ForeColor = SystemColors.GrayText;

					if (toolStripStatusLabel_TerminalStatus_DCD.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_DCD.ForeColor = SystemColors.GrayText;

					toolStripStatusLabel_TerminalStatus_Separator2.Visible    = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

					Image inputXOnXOffImage  = (inputIsXOn  ? on : off);
					Image outputXOnXOffImage = (outputIsXOn ? on : off);

					if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image != inputXOnXOffImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = inputXOnXOffImage;

					if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image != outputXOnXOffImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = outputXOnXOffImage;

					Color inputXOnXOffForeColor  = (allowXOnXOff ? SystemColors.ControlText : SystemColors.GrayText);

					if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor != inputXOnXOffForeColor) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor = inputXOnXOffForeColor;

					if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor = SystemColors.GrayText;

					bool indicateBreakStates = this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates;
					bool manualOutputBreak   = this.settingsRoot.Terminal.IO.SerialPortOutputBreakIsModifiable;

					toolStripStatusLabel_TerminalStatus_Separator3.Visible  = indicateBreakStates;
					toolStripStatusLabel_TerminalStatus_InputBreak.Visible  = indicateBreakStates;
					toolStripStatusLabel_TerminalStatus_OutputBreak.Visible = indicateBreakStates;

					Image inputBreakImage  = (inputBreak  ? off : on);
					Image outputBreakImage = (outputBreak ? off : on);

					if (toolStripStatusLabel_TerminalStatus_InputBreak.Image != inputBreakImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputBreak.Image = inputBreakImage;

					if (toolStripStatusLabel_TerminalStatus_OutputBreak.Image != outputBreakImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputBreak.Image = outputBreakImage;

					if (toolStripStatusLabel_TerminalStatus_InputBreak.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputBreak.ForeColor = SystemColors.GrayText;

					Color manualBreakColor = (manualOutputBreak ? SystemColors.ControlText : SystemColors.GrayText);

					if (toolStripStatusLabel_TerminalStatus_OutputBreak.ForeColor != manualBreakColor) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputBreak.ForeColor = manualBreakColor;

					// Attention:
					// Do not modify the 'Enabled' property. Labels must always be enabled,
					// otherwise picture get's greyed out, but it must either be green or red.
					// Instead of modifying 'Enabled', YAT.Model.Terminal.RequestToggle...()
					// checks whether an operation is allowed.
				}
				else
				{
					// By default, all are disabled:

					foreach (ToolStripStatusLabel sl in this.terminalStatusLabels)
					{
						if (sl.Image != off) // Improve performance by only assigning if different.
							sl.Image = off;

						if (sl.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
							sl.ForeColor = SystemColors.GrayText;
					}

					// Exceptions:

					bool indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
					toolStripStatusLabel_TerminalStatus_Separator2.Visible    = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

					bool indicateBreakStates = this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates;
					toolStripStatusLabel_TerminalStatus_Separator3.Visible  = indicateBreakStates;
					toolStripStatusLabel_TerminalStatus_InputBreak.Visible  = indicateBreakStates;
					toolStripStatusLabel_TerminalStatus_OutputBreak.Visible = indicateBreakStates;
				}
			}
			else if (isUsbSerialHid)
			{
				foreach (ToolStripStatusLabel sl in this.terminalStatusLabels)
				{
					sl.Visible = false;
					sl.Enabled = isOpen;
				}

				foreach (KeyValuePair<ToolStripStatusLabel, string> kvp in this.terminalStatusLabels_DefaultText)
					kvp.Key.Text = kvp.Value;

				foreach (KeyValuePair<ToolStripStatusLabel, string> kvp in this.terminalStatusLabels_DefaultToolTipText)
					kvp.Key.ToolTipText = kvp.Value;

				if (this.settingsRoot.Terminal.Status.ShowFlowControlCount)
				{
					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text  += (" | " + this.terminal.SentXOnCount.ToString(CultureInfo.CurrentCulture)     + " | " + this.terminal.SentXOffCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text += (" | " + this.terminal.ReceivedXOnCount.ToString(CultureInfo.CurrentCulture) + " | " + this.terminal.ReceivedXOffCount.ToString(CultureInfo.CurrentCulture));

					toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText  += (" | XOn Count | XOff Count");
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText += (" | XOn Count | XOff Count");
				}

				if (isOpen)
				{
					bool allowXOnXOff    = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
					bool indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff;
					bool outputIsXOn     = false;
					bool inputIsXOn      = false;

					var x = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
					if (x != null)
					{
						try // Fail-safe implementation, especially catching exceptions while closing.
						{
							indicateXOnXOff = x.XOnXOffIsInUse;
							outputIsXOn     = x.OutputIsXOn;
							inputIsXOn      = x.InputIsXOn;
						}
						catch (Exception ex)
						{
							DebugEx.WriteException(GetType(), ex, "Failed to retrieve XOn/XOff state");
						}
					}

					toolStripStatusLabel_TerminalStatus_Separator2.Visible    = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

					Image inputXOnXOffImage = (inputIsXOn ? on : off);
					Image outputXOnXOffImage = (outputIsXOn ? on : off);

					if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image != inputXOnXOffImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = inputXOnXOffImage;

					if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image != outputXOnXOffImage) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = outputXOnXOffImage;

					Color inputXOnXOffForeColor = (allowXOnXOff ? SystemColors.ControlText : SystemColors.GrayText);

					if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor != inputXOnXOffForeColor) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor = inputXOnXOffForeColor;

					if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor = SystemColors.GrayText;

					// Attention:
					// Do not modify the 'Enabled' property. Labels must always be enabled,
					// otherwise picture get's greyed out, but it must either be green or red.
					// Instead of modifying 'Enabled', YAT.Model.Terminal.RequestToggle...()
					// checks whether an operation is allowed.
				}
				else
				{
					bool indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff;
					toolStripStatusLabel_TerminalStatus_Separator2.Visible    = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

					if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image != off) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = off;

					if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image != off) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = off;

					if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor = SystemColors.GrayText;

					if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor = SystemColors.GrayText;
				}
			}
			else
			{
				foreach (ToolStripStatusLabel sl in this.terminalStatusLabels)
					sl.Visible = false;
			}

			ResumeLayout();
		}

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "The timer just invokes a single-shot callback to show the RFR state for a longer period that it is actually active.")]
		private void TriggerRfrLuminescence()
		{
			timer_RfrLuminescence.Enabled = false;

			Image on = Properties.Resources.Image_Status_Green_12x12;
			if (toolStripStatusLabel_TerminalStatus_RFR.Image != on) // Improve performance by only assigning if different.
				toolStripStatusLabel_TerminalStatus_RFR.Image = on;

			timer_RfrLuminescence.Interval = RfrLuminescenceInterval;
			timer_RfrLuminescence.Enabled = true;
		}

		private void timer_RfrLuminescence_Tick(object sender, EventArgs e)
		{
			timer_RfrLuminescence.Enabled = false;
			ResetRfr();
		}

		private void ResetRfr()
		{
			bool isOpen = this.terminal.IsOpen;

			Image on  = Properties.Resources.Image_Status_Green_12x12;
			Image off = Properties.Resources.Image_Status_Red_12x12;

			if (isOpen)
			{
				var pins = new MKY.IO.Ports.SerialPortControlPins();

				var port = (this.terminal.UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
				if (port != null)
					pins = port.ControlPins;

				Image rfr = (pins.Rfr ? on : off);
				if (toolStripStatusLabel_TerminalStatus_RFR.Image != rfr) // Improve performance by only assigning if different.
					toolStripStatusLabel_TerminalStatus_RFR.Image = rfr;
			}
			else
			{
				Image rfr = off;
				if (toolStripStatusLabel_TerminalStatus_RFR.Image != off) // Improve performance by only assigning if different.
					toolStripStatusLabel_TerminalStatus_RFR.Image = off;
			}
		}

		#endregion

		#endregion

		#region Log
		//==========================================================================================
		// Log
		//==========================================================================================

		private void SetLogControls()
		{
			toolStripMenuItem_TerminalMenu_Log_SetMenuItems();
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowLogSettings()
		{
			LogSettings f = new LogSettings(this.settingsRoot.Log);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();
				this.settingsRoot.Log = f.SettingsResult;
			}

			SelectSendTextInput();
		}

		#endregion

		#region Event Invoking
		//==========================================================================================
		// Event Invoking
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTerminalChanged(EventArgs e)
		{
			EventHelper.FireSync(Changed, this, e);
		}

		/// <summary></summary>
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

		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "status", Justification = "Prepared for future use.")]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Prepared for future use.")]
		private string GetStatusText(Status status)
		{
			switch (status)
			{
				default: return (DefaultStatusText);
			}
		}

		private void SetFixedStatusText(string text)
		{
			timer_StatusText.Enabled = false;
			toolStripStatusLabel_TerminalStatus_Status.Text = text;
		}

		private void SetFixedStatus(Status status)
		{
			SetFixedStatusText(GetStatusText(status));
		}

		private void SetTimedStatusText(string text)
		{
			timer_StatusText.Enabled = false;
			toolStripStatusLabel_TerminalStatus_Status.Text = text;
			timer_StatusText.Interval = TimedStatusInterval;
			timer_StatusText.Enabled = true;
		}

		private void SetTimedStatus(Status status)
		{
			SetTimedStatusText(GetStatusText(status));
		}

		private void ResetStatusText()
		{
			SetFixedStatus(Status.Default);
		}

		private void timer_StatusText_Tick(object sender, EventArgs e)
		{
			timer_StatusText.Enabled = false;
			ResetStatusText();
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
		{
			string guid;
			if (this.terminal != null)
				guid = this.terminal.Guid.ToString();
			else
				guid = "<None>";

			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.InvariantCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.InvariantInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.InvariantCulture),
					GetType(),
					"",
					"[" + guid + "]",
					message
				)
			);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
