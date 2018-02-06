﻿//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Almost Final Version 1.99.95
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2018 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

#region Configuration
//==================================================================================================
// Configuration
//==================================================================================================

#if (DEBUG)

	// Enable debugging of MDI related state changes:
////#define DEBUG_MDI

#endif // DEBUG

#endregion

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
using MKY.Collections.Specialized;
using MKY.Contracts;
using MKY.Diagnostics;
using MKY.Drawing;
using MKY.IO;
using MKY.Settings;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.View.Controls;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "YAT.View.Forms.Terminal.#toolTip", Justification = "This is a bug in FxCop.")]

#endregion

namespace YAT.View.Forms
{
	/// <summary></summary>
	[SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Why not?")]
	public partial class Terminal : Form, IOnFormDeactivateWorkaround
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

		// Scaling:
		private SplitContainerHelper splitContainerHelper = new SplitContainerHelper();

		// Startup/Update/Closing:
		private bool isStartingUp = true;
		private SettingControlsHelper isSettingControls;
		private SettingControlsHelper isUpdatingSplitterRatio;
		private bool isIntegralMdiLayouting = false;
		private ClosingState closingState = ClosingState.None;

		// MDI:
		private Form mdiParent;
		private ContextMenuStripShortcutTargetWorkaround contextMenuStripShortcutTargetWorkaround;

		// Terminal:
		private Model.Terminal terminal;

		// Monitors:
		private Domain.RepositoryType lastMonitorSelection = Domain.RepositoryType.None;

		// Settings:
		private TerminalSettingsRoot settingsRoot;
		private bool handlingTerminalSettingsIsSuspended; // = false; // A simple flag is sufficient as
		                                                              // the form is ISynchronizeInvoke.
		// Status:
		private bool ioStatusIndicatorFlashingIsOn; // = false;

		// Find:
		private string lastFindPattern; // = null;

		// Toolstrip-combobox-validation-workaround (too late invocation of 'Validate' event):
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
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

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoResponseCountChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoActionCountChanged;

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

			// Link and attach to terminal model:
			this.terminal = terminal;
			AttachTerminalEventHandlers();

			// Link and attach to terminal settings:
			this.settingsRoot = this.terminal.SettingsRoot;
			AttachSettingsEventHandlers();

			ApplyWindowSettings();
			LayoutTerminal();

			// Force settings changed event to set all controls.
			// For improved performance, manually suspend/resume handler for terminal settings:
			SuspendHandlingTerminalSettings();
			try
			{
				this.settingsRoot.ClearChanged();
				this.settingsRoot.ForceChangeEvent();
			}
			finally
			{
				ResumeHandlingTerminalSettings();
			}

			DebugMessage("...successfully created.");
		}

		#endregion

		#region Form Scaling
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <summary></summary>
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			this.splitContainerHelper.AdjustScale(factor);

			base.ScaleControl(factor, specified);
		}

		#endregion

		#region Form Special Keys
		//==========================================================================================
		// Form Special Keys
		//==========================================================================================

		/// <remarks>
		/// In case of pressing a modifier key (e.g. [Shift]), this method is invoked twice! Both
		/// invocations will state msg=0x100 (WM_KEYDOWN)! See:
		/// https://msdn.microsoft.com/en-us/library/system.windows.forms.control.processcmdkey.aspx:
		/// The ProcessCmdKey method first determines whether the control has a ContextMenu, and if
		/// so, enables the ContextMenu to process the command key. If the command key is not a menu
		/// shortcut and the control has a parent, the key is passed to the parent's ProcessCmdKey
		/// method. The net effect is that command keys are "bubbled" up the control hierarchy. In
		/// addition to the key the user pressed, the key data also indicates which, if any, modifier
		/// keys were pressed at the same time as the key. Modifier keys include the SHIFT, CTRL, and
		/// ALT keys.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "StyleCop isn't able to skip URLs...")]
		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this.contextMenuStripShortcutTargetWorkaround.ProcessCmdKey(keyData))
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

			this.splitContainerHelper.PerformScaling(this);

			// Immediately set terminal controls so the terminal "looks nice" from the very start:
			SetTerminalControls();
		}

		private void Terminal_Activated(object sender, EventArgs e)
		{
			DebugMdi("Activated");

			monitor_Tx   .Activate();
			monitor_Bidir.Activate();
			monitor_Rx   .Activate();

			// Enable immediate user input:
			send.SelectAndPrepareUserInput();
		}

		private void Terminal_Deactivate(object sender, EventArgs e)
		{
			DebugMdi("Deactivated");

			monitor_Tx   .Deactivate();
			monitor_Bidir.Deactivate();
			monitor_Rx   .Deactivate();

			// Apply the workaround when switching among the forms of the MDI application:
			OnFormDeactivateWorkaround();

			// Deselect other controls to prepare user input for 'Activated' above:
			send.StandbyInUserInput();
		}

		/// <remarks>See remarks in <see cref="ComboBoxEx"/>.</remarks>
		public virtual void OnFormDeactivateWorkaround()
		{
			toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger .OnFormDeactivateWorkaround();
			toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.OnFormDeactivateWorkaround();
			toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.OnFormDeactivateWorkaround();
		////toolStripComboBox_TerminalMenu_Receive_AutoAction_Action is a standard ToolStripComboBox.
		////toolStripComboBox_TerminalMenu_View_Panels_Orientation   is a standard ToolStripComboBox.
		////toolStripComboBox_MonitorContextMenu_Panels_Orientation  is a standard ToolStripComboBox.

			send.OnFormDeactivateWorkaround();
		}

		private void Terminal_LocationChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsIntegraMdiLayouting && !IsClosing)
				SaveWindowSettings();
		}

		private void Terminal_SizeChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsIntegraMdiLayouting && !IsClosing)
				SaveWindowSettings();
		}

		/// <summary>
		/// Notifies the terminal that its <see cref="FormWindowState"/> has changed.
		/// </summary>
		/// <remarks>
		/// Neither the 'LocationChanged' nor 'SizeChanged' event is raised when only
		/// the <see cref="FormWindowState"/> has changed but no resizes occurs.
		/// </remarks>
		public void NotifyWindowStateChanged()
		{
			if (!IsStartingUp && !IsIntegraMdiLayouting && !IsClosing)
				SaveWindowSettings();
		}

		/// <remarks>Requires that <see cref="Form.KeyPreview"/> is enabled.</remarks>
		private void Terminal_KeyDown(object sender, KeyEventArgs e)
		{
			if (send != null)
			{
				if (send.ContainsFocus)
					send.NotifyKeyDown(e); // Somewhat ugly workaround to handle key events...
			}
		}

		/// <remarks>Requires that <see cref="Form.KeyPreview"/> is enabled.</remarks>
		private void Terminal_KeyUp(object sender, KeyEventArgs e)
		{
			if (send != null)
			{
				if (send.ContainsFocus)
					send.NotifyKeyUp(e); // Somewhat ugly workaround to handle key events...
			}
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

		/// <remarks>
		/// Not really sure whether handling here is required in any case. Normally, a terminal
		/// signals via <see cref="terminal_Closed"/>. However, instead of verifying every possible
		/// case, simply detach here too.
		/// </remarks>
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
			try
			{
				if (TerminalIsAvailable)
					toolStripMenuItem_TerminalMenu_File_Save.Enabled = !this.terminal.SettingsFileIsReadOnly;
				else
					toolStripMenuItem_TerminalMenu_File_Save.Enabled = false;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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
			try
			{
				bool monitorIsDefined = (this.lastMonitorSelection != Domain.RepositoryType.None);
				bool textIsNotFocused = !(send         .TextFocused ||
				                          monitor_Tx   .TextFocused ||
				                          monitor_Bidir.TextFocused ||
				                          monitor_Rx   .TextFocused); // Required to suppress standard key shortcuts
				                                                      // [Ctrl+A], [Ctrl+C] and [Ctrl+Delete] below.
				if (TerminalIsAvailable)
				{
					toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = !this.terminal.IsStarted;
					toolStripMenuItem_TerminalMenu_Terminal_Stop.Enabled  =  this.terminal.IsStarted;

					toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled =  this.terminal.IsBusy;
					toolStripMenuItem_TerminalMenu_Terminal_Clear.Enabled =  monitorIsDefined;
				}
				else
				{
					toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Stop.Enabled  = false;

					toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Clear.Enabled = false;
				}

				toolStripMenuItem_TerminalMenu_Terminal_SelectAll.Enabled       = (monitorIsDefined && textIsNotFocused); // [Ctrl+A]
				toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Enabled      = (monitorIsDefined && textIsNotFocused); // [Ctrl+Delete]

				toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = (monitorIsDefined && textIsNotFocused); // [Ctrl+C]
				toolStripMenuItem_TerminalMenu_Terminal_SaveToFile.Enabled      =  monitorIsDefined;
				toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled           =  monitorIsDefined;

				toolStripMenuItem_TerminalMenu_Terminal_FindNext    .Enabled = (monitorIsDefined && FindNextIsFeasible);
				toolStripMenuItem_TerminalMenu_Terminal_FindPrevious.Enabled = (monitorIsDefined && FindPreviousIsFeasible);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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

		private void toolStripMenuItem_TerminalMenu_Terminal_Find_Click(object sender, EventArgs e)
		{
			RequestFind();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_FindNext_Click(object sender, EventArgs e)
		{
			RequestFindNext();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_FindPrevious_Click(object sender, EventArgs e)
		{
			RequestFindPrevious();
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
			bool isTextTerminal = (this.settingsRoot.TerminalType == Domain.TerminalType.Text);

			this.isSettingControls.Enter();
			try
			{
				// Prepare the menu item properties based on state and settings.
				//
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
				// ...View.Controls.SendText.SetControls()
				// Changes here may have to be applied there too.
				//
				// Main and context menu are separated as there are subtle differences between them.

				string sendTextText = "Text";
				bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText;
				if (this.settingsRoot.Send.SendImmediately)
				{
					if (isTextTerminal)
						sendTextText = "EOL";
					else
						sendTextEnabled = false;
				}

				bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath;

				// Set the menu item properties:

				toolStripMenuItem_TerminalMenu_Send_Text.Text              = sendTextText;
				toolStripMenuItem_TerminalMenu_Send_Text.Enabled           = sendTextEnabled && this.terminal.IsReadyToSend;
				toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Enabled = sendTextEnabled && this.terminal.IsReadyToSend && !this.settingsRoot.SendText.Command.IsMultiLineText && !this.settingsRoot.Send.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_File.Enabled           = sendFileEnabled && this.terminal.IsReadyToSend;

				toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Checked = this.settingsRoot.Send.UseExplicitDefaultRadix;

				toolStripMenuItem_TerminalMenu_Send_KeepSendText.Enabled         = !this.settingsRoot.Send.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_KeepSendText.Checked         = !this.settingsRoot.Send.SendImmediately && this.settingsRoot.Send.KeepSendText;
				toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Checked       =  this.settingsRoot.Send.CopyPredefined;
				toolStripMenuItem_TerminalMenu_Send_SendImmediately.Checked      =  this.settingsRoot.Send.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Checked =  this.settingsRoot.Send.EnableEscapes;

				toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Enabled  =  this.settingsRoot.SendText.Command.IsMultiLineText;

				toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Enabled       = isTextTerminal;
				toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Checked       = this.settingsRoot.TextTerminal.SendFile.SkipEmptyLines;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Enabled = isTextTerminal;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Checked = this.settingsRoot.TextTerminal.SendFile.EnableEscapes;

				toolStripMenuItem_TerminalMenu_Send_AutoResponse.Checked          = this.settingsRoot.AutoResponse.IsActive;
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.Checked  = this.settingsRoot.AutoResponse.TriggerIsActive;
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.Checked = this.settingsRoot.AutoResponse.ResponseIsActive;

				// Attention:
				// Similar code exists in...
				// ...toolStripMenuItem_TerminalMenu_Receive_SetMenuItems()
				// ...View.Forms.Main.toolStripButton_MainTool_SetControls()
				// Changes here may have to be applied there too.

				if (!this.terminalMenuValidationWorkaround_UpdateIsSuspended)
				{
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.Clear();
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.AddRange(this.settingsRoot.GetValidAutoTriggerItems());

					var trigger = this.settingsRoot.AutoResponse.Trigger;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger, trigger, new Command(trigger).SingleLineText); // No explicit default radix available (yet).

					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.Clear();
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.AddRange(this.settingsRoot.GetValidAutoResponseItems());

					var response = this.settingsRoot.AutoResponse.Response;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Send_AutoResponse_Response, response, new Command(response).SingleLineText); // No explicit default radix available (yet).
				}

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Enabled = this.settingsRoot.AutoResponse.IsActive;

				// Note that 'AutoAction' is implemented in 'Receive'.
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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

		private void toolStripMenuItem_TerminalMenu_Send_TextWithoutEol_Click(object sender, EventArgs e)
		{
			if (!this.settingsRoot.Send.SendImmediately)
				this.terminal.SendTextWithoutEol();
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + @"""Send Text w/o EOL"" is invalid when ""Send Immediately"" is active!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		private void toolStripMenuItem_TerminalMenu_Send_File_Click(object sender, EventArgs e)
		{
			this.terminal.SendFile();
		}

		private void toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.UseExplicitDefaultRadix = !this.settingsRoot.Send.UseExplicitDefaultRadix;
		}

		private void toolStripMenuItem_TerminalMenu_Send_KeepSendText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.KeepSendText = !this.settingsRoot.Send.KeepSendText;
		}

		private void toolStripMenuItem_TerminalMenu_Send_CopyPredefined_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.CopyPredefined = !this.settingsRoot.Send.CopyPredefined;
		}

		private void toolStripMenuItem_TerminalMenu_Send_SendImmediately_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.SendImmediately = !this.settingsRoot.Send.SendImmediately;
		}

		private void toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.EnableEscapes = !this.settingsRoot.Send.EnableEscapes;
		}

		private void toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.SendText.ExpandMultiLineText();
		}

		private void toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines_Click(object sender, EventArgs e)
		{
			this.settingsRoot.TextTerminal.SendFile.SkipEmptyLines = !this.settingsRoot.TextTerminal.SendFile.SkipEmptyLines;
		}

		private void toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile_Click(object sender, EventArgs e)
		{
			this.settingsRoot.TextTerminal.SendFile.EnableEscapes = !this.settingsRoot.TextTerminal.SendFile.EnableEscapes;
		}

		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
				RequestAutoResponseTrigger(trigger);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			// Attention, 'isSettingControls' must only be checked further below!

			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				string triggerText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text;
				int invalidTextStart;
				if (Utilities.ValidationHelper.ValidateText(this, "automatic response trigger", triggerText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							RequestAutoResponseTrigger(triggerText);
						}
						finally
						{
							this.terminalMenuValidationWorkaround_UpdateIsSuspended = false;
						}
					}
				}
				else
				{
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text = triggerText.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var response = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedItem as AutoResponseEx);
			if (response != null)
				RequestAutoResponseResponse(response);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			// Attention, 'isSettingControls' must only be checked further below!

			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex)
			{
				string responseText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text;
				int invalidTextStart;
				if (Utilities.ValidationHelper.ValidateText(this, "automatic response", responseText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							RequestAutoResponseResponse(responseText);
						}
						finally
						{
							this.terminalMenuValidationWorkaround_UpdateIsSuspended = false;
						}
					}
				}
				else
				{
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text = responseText.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate_Click(object sender, EventArgs e)
		{
			RequestAutoResponseDeactivate();
		}

		#endregion

		#region Controls Event Handlers > Terminal Menu > Receive
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Terminal Menu > Receive
		//------------------------------------------------------------------------------------------

		// Note that 'AutoResponse' is implemented in 'Send'.

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_Receive_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_TerminalMenu_Receive_AutoAction.Checked          = this.settingsRoot.AutoAction.IsActive;
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.Checked  = this.settingsRoot.AutoAction.TriggerIsActive;
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.Checked = this.settingsRoot.AutoAction.ActionIsActive;

				// Attention:
				// Similar code exists in...
				// ...toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
				// ...View.Forms.Main.toolStripButton_MainTool_SetControls()
				// Changes here may have to be applied there too.

				if (!this.terminalMenuValidationWorkaround_UpdateIsSuspended)
				{
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Items.Clear();
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Items.AddRange(this.settingsRoot.GetValidAutoTriggerItems());

					var trigger = this.settingsRoot.AutoAction.Trigger;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger, trigger, new Command(trigger).SingleLineText); // No explicit default radix available (yet).

					toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Items.Clear();
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Items.AddRange(this.settingsRoot.GetValidAutoActionItems());

					var response = this.settingsRoot.AutoAction.Action;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Receive_AutoAction_Action, response, new Command(response).SingleLineText); // No explicit default radix available (yet).
				}

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Enabled = this.settingsRoot.AutoAction.IsActive;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_TerminalMenu_Receive_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Receive_SetMenuItems();
		}

		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
				RequestAutoActionTrigger(trigger);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_TextChanged(object sender, EventArgs e)
		{
			// Attention, 'isSettingControls' must only be checked further below!

			if (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				string triggerText = toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Text;
				int invalidTextStart;
				if (Utilities.ValidationHelper.ValidateText(this, "automatic action trigger", triggerText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.terminalMenuValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							RequestAutoActionTrigger(triggerText);
						}
						finally
						{
							this.terminalMenuValidationWorkaround_UpdateIsSuspended = false;
						}
					}
				}
				else
				{
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Text = triggerText.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Action_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var action = (toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.SelectedItem as AutoActionEx);
			if (action != null)
				RequestAutoActionAction(action);
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate_Click(object sender, EventArgs e)
		{
			RequestAutoActionDeactivate();
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
			try
			{
				// Attention:
				// Similar code exists in Main.toolStripMenuItem_MainMenu_Log_SetMenuItems().
				// Changes here may have to be applied there too.

				bool logIsEnabled = (this.settingsRoot.Log.Count > 0);
				bool logIsOn      =  this.settingsRoot.LogIsOn;

				bool logFileExists = false;
				if (this.terminal != null)
					logFileExists = this.terminal.LogFileExists;

				toolStripMenuItem_TerminalMenu_Log_On.Enabled       = (logIsEnabled && !logIsOn);
				toolStripMenuItem_TerminalMenu_Log_Off.Enabled      = (logIsEnabled &&  logIsOn);
				toolStripMenuItem_TerminalMenu_Log_OpenFile.Enabled = (logIsEnabled &&  logFileExists);
				toolStripMenuItem_TerminalMenu_Log_Clear.Enabled    = (logIsEnabled &&  logIsOn);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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
			try
			{
				toolStripComboBox_TerminalMenu_View_Panels_Orientation.Items.AddRange(OrientationEx.GetItems());
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_View_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				bool isText       = ((Domain.TerminalTypeEx)this.settingsRoot.TerminalType).IsText;

				bool isSerialPort = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;

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
				bool isShowable = ((this.settingsRoot.Display.TxRadixIsShowable) ||
				                   (this.settingsRoot.Display.RxRadixIsShowable));
				toolStripMenuItem_TerminalMenu_View_ShowRadix.Enabled = isShowable; // Attention: Same code further below as well as in 'View.Forms.AdvancedTerminalSettings'.
				toolStripMenuItem_TerminalMenu_View_ShowRadix.Checked = isShowable && this.settingsRoot.Display.ShowRadix;

				toolStripMenuItem_TerminalMenu_View_ShowBufferLineNumbers.Checked = this.settingsRoot.Display.ShowBufferLineNumbers;
				toolStripMenuItem_TerminalMenu_View_ShowTotalLineNumbers.Checked  = this.settingsRoot.Display.ShowTotalLineNumbers;
				toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Checked         = this.settingsRoot.Display.ShowTimeStamp;
				toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.Checked          = this.settingsRoot.Display.ShowTimeDelta;
				toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.Checked         = this.settingsRoot.Display.ShowTimeSpan;
				toolStripMenuItem_TerminalMenu_View_ShowPort.Checked              = this.settingsRoot.Display.ShowPort;
				toolStripMenuItem_TerminalMenu_View_ShowDirection.Checked         = this.settingsRoot.Display.ShowDirection;

				toolStripMenuItem_TerminalMenu_View_ShowEol.Enabled = (isText);
				toolStripMenuItem_TerminalMenu_View_ShowEol.Checked = (isText && this.settingsRoot.TextTerminal.ShowEol);

				toolStripMenuItem_TerminalMenu_View_ShowLength.Checked           = this.settingsRoot.Display.ShowLength;
				toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine.Checked = this.settingsRoot.Display.ShowCopyOfActiveLine;

				// Flow control count:
				bool showFlowControlCount = this.settingsRoot.Status.ShowFlowControlCount;
				toolStripMenuItem_TerminalMenu_View_FlowControlCount.Enabled            = this.settingsRoot.Terminal.IO.FlowControlIsInUse;
				toolStripMenuItem_TerminalMenu_View_FlowControlCount_ShowCount.Checked  = showFlowControlCount;
				toolStripMenuItem_TerminalMenu_View_FlowControlCount_ResetCount.Enabled = showFlowControlCount;

				// Break count:
				bool showBreakCount = this.settingsRoot.Status.ShowBreakCount;
				toolStripMenuItem_TerminalMenu_View_BreakCount.Enabled            = (isSerialPort && this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates);
				toolStripMenuItem_TerminalMenu_View_BreakCount_ShowCount.Checked  = showBreakCount;
				toolStripMenuItem_TerminalMenu_View_BreakCount_ResetCount.Enabled = showBreakCount;

				// Format:
				if (this.settingsRoot.Format.FormattingEnabled)
				{
					toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Text = "Disable Forma&tting";
					toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Image = Properties.Resources.Image_Tool_font_delete_16x16;
				}
				else
				{
					toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Text = "Enable Forma&tting";
					toolStripMenuItem_TerminalMenu_View_ToggleFormatting.Image = Properties.Resources.Image_Tool_font_add_16x16;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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
			if (this.isSettingControls)
				return;

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

			// 'terminal_IOCount/RateChanged_Promptly' are is not used because of the reasons
			// described in the remarks of 'terminal_RawChunkSent/Received' of 'Model.Terminal'.
			// Instead, the update is done by the 'terminal_DisplayElementsSent/Received' and
			// 'terminal_DisplayLinesSent/Received' handlers further below. As a consequence,
			// the update must manually be triggered:

			SetDataCountAndRateStatus();
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowRadix_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowBufferLineNumbers_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowBufferLineNumbers = !this.settingsRoot.Display.ShowBufferLineNumbers;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTotalLineNumbers_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowTotalLineNumbers = !this.settingsRoot.Display.ShowTotalLineNumbers;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTimeStamp_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowTimeStamp = !this.settingsRoot.Display.ShowTimeStamp;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTimeSpan_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowTimeSpan = !this.settingsRoot.Display.ShowTimeSpan;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowTimeDelta_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowTimeDelta = !this.settingsRoot.Display.ShowTimeDelta;
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

		private void toolStripMenuItem_TerminalMenu_View_ShowCopyOfActiveLine_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowCopyOfActiveLine = !this.settingsRoot.Display.ShowCopyOfActiveLine;
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

		private void toolStripMenuItem_TerminalMenu_View_ToggleFormatting_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Format.FormattingEnabled = !this.settingsRoot.Format.FormattingEnabled;
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
			this.menuItems_preset = new List<ToolStripMenuItem>(8); // Preset the required capacity to improve memory management.
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_1);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_2);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_3);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_4);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_5);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_6);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_7);
			this.menuItems_preset.Add(toolStripMenuItem_PresetContextMenu_Preset_8);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Preset_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				bool isSerialPort = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;

				foreach (ToolStripMenuItem item in this.menuItems_preset)
					item.Enabled = isSerialPort;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void contextMenuStrip_Preset_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Preset_SetMenuItems();
		}

		private void toolStripMenuItem_PresetContextMenu_Preset_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			RequestPreset(ToolStripMenuItemEx.TagToIndex(sender)); // Attention, 'ToolStripMenuItem' is no 'Control'!
		}

		#endregion

		#region Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Monitor Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Monitor_Initialize()
		{
			this.isSettingControls.Enter();
			try
			{
				toolStripComboBox_MonitorContextMenu_Panels_Orientation.Items.AddRange(OrientationEx.GetItems());
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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
			try
			{
				var terminalType = this.settingsRoot.TerminalType;
				var monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
				var isMonitor = (monitorType != Domain.RepositoryType.None);

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
					case Domain.RepositoryType.None: /* Nothing to do. */ break;

					case Domain.RepositoryType.Tx:    hideIsAllowed = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);    break;
					case Domain.RepositoryType.Bidir: hideIsAllowed = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.RxMonitorPanelIsVisible);    break;
					case Domain.RepositoryType.Rx:    hideIsAllowed = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.BidirMonitorPanelIsVisible); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + monitorType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
				toolStripMenuItem_MonitorContextMenu_Hide.Visible = hideIsAllowed;
				toolStripMenuItem_MonitorContextMenu_Hide.Enabled = isMonitor && hideIsAllowed;

				bool isShowable = ((this.settingsRoot.Display.TxRadixIsShowable) ||
				                   (this.settingsRoot.Display.RxRadixIsShowable));
				toolStripMenuItem_MonitorContextMenu_ShowRadix.Enabled = isShowable; // Attention: Same code further above as well as in 'View.Forms.AdvancedTerminalSettings'.
				toolStripMenuItem_MonitorContextMenu_ShowRadix.Checked = isShowable && this.settingsRoot.Display.ShowRadix;

				toolStripMenuItem_MonitorContextMenu_ShowBufferLineNumbers.Checked = this.settingsRoot.Display.ShowBufferLineNumbers;
				toolStripMenuItem_MonitorContextMenu_ShowTotalLineNumbers.Checked  = this.settingsRoot.Display.ShowTotalLineNumbers;
				toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Checked         = this.settingsRoot.Display.ShowTimeStamp;
				toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.Checked          = this.settingsRoot.Display.ShowTimeSpan;
				toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.Checked         = this.settingsRoot.Display.ShowTimeDelta;
				toolStripMenuItem_MonitorContextMenu_ShowPort.Checked              = this.settingsRoot.Display.ShowPort;
				toolStripMenuItem_MonitorContextMenu_ShowDirection.Checked         = this.settingsRoot.Display.ShowDirection;

				bool isText = ((Domain.TerminalTypeEx)terminalType).IsText;
				toolStripMenuItem_MonitorContextMenu_ShowEol.Enabled = isText;
				toolStripMenuItem_MonitorContextMenu_ShowEol.Checked = isText && this.settingsRoot.TextTerminal.ShowEol;

				toolStripMenuItem_MonitorContextMenu_ShowLength.Checked           = this.settingsRoot.Display.ShowLength;
				toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine.Checked = this.settingsRoot.Display.ShowCopyOfActiveLine;

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

				toolStripMenuItem_MonitorContextMenu_FindNext    .Enabled = (isMonitor && FindNextIsFeasible);
				toolStripMenuItem_MonitorContextMenu_FindPrevious.Enabled = (isMonitor && FindPreviousIsFeasible);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Tx_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.TxMonitorPanelIsVisible = !this.settingsRoot.Layout.TxMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Bidir_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.BidirMonitorPanelIsVisible = !this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Rx_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.RxMonitorPanelIsVisible = !this.settingsRoot.Layout.RxMonitorPanelIsVisible;
		}

		private void toolStripComboBox_MonitorContextMenu_Panels_Orientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			if (this.isSettingControls)
				return;

			SetMonitorOrientation((OrientationEx)toolStripComboBox_MonitorContextMenu_Panels_Orientation.SelectedItem);
		}

		private void toolStripMenuItem_MonitorContextMenu_Hide_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			switch (monitorType)
			{
				case Domain.RepositoryType.None: /* Nothing to do. */ break;

				case Domain.RepositoryType.Tx:    this.settingsRoot.Layout.TxMonitorPanelIsVisible    = false; break;
				case Domain.RepositoryType.Bidir: this.settingsRoot.Layout.BidirMonitorPanelIsVisible = false; break;
				case Domain.RepositoryType.Rx:    this.settingsRoot.Layout.RxMonitorPanelIsVisible    = false; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + monitorType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void toolStripMenuItem_MonitorContextMenu_Format_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			ShowFormatSettings();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowConnectTime_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Status.ShowConnectTime = !this.settingsRoot.Status.ShowConnectTime;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetConnectTime_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.terminal.ResetConnectTime();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowCountAndRate_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Status.ShowCountAndRate = !this.settingsRoot.Status.ShowCountAndRate;
		}

		private void toolStripMenuItem_MonitorContextMenu_ResetCount_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.terminal.ResetIOCountAndRate();

			// 'terminal_IOCount/RateChanged_Promptly' are is not used because of the reasons
			// described in the remarks of 'terminal_RawChunkSent/Received' of 'Model.Terminal'.
			// Instead, the update is done by the 'terminal_DisplayElementsSent/Received' and
			// 'terminal_DisplayLinesSent/Received' handlers further below. As a consequence,
			// the update must manually be triggered:

			SetDataCountAndRateStatus();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowRadix_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowBufferLineNumbers_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowBufferLineNumbers = !this.settingsRoot.Display.ShowBufferLineNumbers;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTotalLineNumbers_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowTotalLineNumbers = !this.settingsRoot.Display.ShowTotalLineNumbers;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTimeStamp_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowTimeStamp = !this.settingsRoot.Display.ShowTimeStamp;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTimeSpan_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowTimeSpan = !this.settingsRoot.Display.ShowTimeSpan;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowTimeDelta_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowTimeDelta = !this.settingsRoot.Display.ShowTimeDelta;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowPort_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowPort = !this.settingsRoot.Display.ShowPort;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowDirection_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowDirection = !this.settingsRoot.Display.ShowDirection;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowEol_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.TextTerminal.ShowEol = !this.settingsRoot.TextTerminal.ShowEol;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLength_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowLength = !this.settingsRoot.Display.ShowLength;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowCopyOfActiveLine_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowCopyOfActiveLine = !this.settingsRoot.Display.ShowCopyOfActiveLine;
		}

		private void toolStripMenuItem_MonitorContextMenu_Clear_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var repositoryType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			if (repositoryType != Domain.RepositoryType.None)
				ClearMonitor(repositoryType);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid context menu source control received from " + sender.ToString() + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		private void toolStripMenuItem_MonitorContextMenu_Refresh_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var repositoryType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
			if (repositoryType != Domain.RepositoryType.None)
				RefreshMonitor(repositoryType);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Invalid context menu source control received from " + sender.ToString() + "!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		private void toolStripMenuItem_MonitorContextMenu_SelectAll_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			GetMonitor(contextMenuStrip_Monitor.SourceControl).SelectAll();
		}

		private void toolStripMenuItem_MonitorContextMenu_SelectNone_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			GetMonitor(contextMenuStrip_Monitor.SourceControl).SelectNone();
		}

		private void toolStripMenuItem_MonitorContextMenu_SaveToFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			ShowSaveMonitorDialog(GetMonitor(contextMenuStrip_Monitor.SourceControl));
		}

		private void toolStripMenuItem_MonitorContextMenu_CopyToClipboard_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			CopyMonitorToClipboard(GetMonitor(contextMenuStrip_Monitor.SourceControl));
		}

		private void toolStripMenuItem_MonitorContextMenu_Print_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			ShowPrintMonitorDialog(GetMonitor(contextMenuStrip_Monitor.SourceControl));
		}

		private void toolStripMenuItem_MonitorContextMenu_Find_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			RequestFind();
		}

		private void toolStripMenuItem_MonitorContextMenu_FindNext_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			RequestFindNext();
		}

		private void toolStripMenuItem_MonitorContextMenu_FindPrevious_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			RequestFindPrevious();
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
			try
			{
				bool separateTxRx = this.settingsRoot.Display.SeparateTxRxRadix;

				toolStripMenuItem_RadixContextMenu_String.Visible  = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Char.Visible    = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Bin.Visible     = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Oct.Visible     = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Dec.Visible     = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Hex.Visible     = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Unicode.Visible = !separateTxRx;

				toolStripMenuItem_RadixContextMenu_Separator_1.Visible = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Separator_2.Visible = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Separator_3.Visible = !separateTxRx;
				toolStripMenuItem_RadixContextMenu_Separator_4.Visible =  separateTxRx;

				toolStripMenuItem_RadixContextMenu_SeparateTxRx.Checked = separateTxRx;

				toolStripMenuItem_RadixContextMenu_TxRadix.Visible = separateTxRx;
				toolStripMenuItem_RadixContextMenu_RxRadix.Visible = separateTxRx;

				if (!separateTxRx)
				{
					toolStripMenuItem_RadixContextMenu_String.Checked  = (this.settingsRoot.Display.TxRadix == Domain.Radix.String);
					toolStripMenuItem_RadixContextMenu_Char.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Char);
					toolStripMenuItem_RadixContextMenu_Bin.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Bin);
					toolStripMenuItem_RadixContextMenu_Oct.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Oct);
					toolStripMenuItem_RadixContextMenu_Dec.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Dec);
					toolStripMenuItem_RadixContextMenu_Hex.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Hex);
					toolStripMenuItem_RadixContextMenu_Unicode.Checked = (this.settingsRoot.Display.TxRadix == Domain.Radix.Unicode);
				}
				else
				{
					toolStripMenuItem_RadixContextMenu_Tx_String.Checked  = (this.settingsRoot.Display.TxRadix == Domain.Radix.String);
					toolStripMenuItem_RadixContextMenu_Tx_Char.Checked    = (this.settingsRoot.Display.TxRadix == Domain.Radix.Char);
					toolStripMenuItem_RadixContextMenu_Tx_Bin.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Bin);
					toolStripMenuItem_RadixContextMenu_Tx_Oct.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Oct);
					toolStripMenuItem_RadixContextMenu_Tx_Dec.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Dec);
					toolStripMenuItem_RadixContextMenu_Tx_Hex.Checked     = (this.settingsRoot.Display.TxRadix == Domain.Radix.Hex);
					toolStripMenuItem_RadixContextMenu_Tx_Unicode.Checked = (this.settingsRoot.Display.TxRadix == Domain.Radix.Unicode);

					toolStripMenuItem_RadixContextMenu_Rx_String.Checked  = (this.settingsRoot.Display.RxRadix == Domain.Radix.String);
					toolStripMenuItem_RadixContextMenu_Rx_Char.Checked    = (this.settingsRoot.Display.RxRadix == Domain.Radix.Char);
					toolStripMenuItem_RadixContextMenu_Rx_Bin.Checked     = (this.settingsRoot.Display.RxRadix == Domain.Radix.Bin);
					toolStripMenuItem_RadixContextMenu_Rx_Oct.Checked     = (this.settingsRoot.Display.RxRadix == Domain.Radix.Oct);
					toolStripMenuItem_RadixContextMenu_Rx_Dec.Checked     = (this.settingsRoot.Display.RxRadix == Domain.Radix.Dec);
					toolStripMenuItem_RadixContextMenu_Rx_Hex.Checked     = (this.settingsRoot.Display.RxRadix == Domain.Radix.Hex);
					toolStripMenuItem_RadixContextMenu_Rx_Unicode.Checked = (this.settingsRoot.Display.RxRadix == Domain.Radix.Unicode);
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void contextMenuStrip_Radix_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Radix_SetMenuItems();
		}

		private void toolStripMenuItem_RadixContextMenu_String_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Char_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Bin_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Oct_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Dec_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Hex_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.Hex);
		}

		private void toolStripMenuItem_RadixContextMenu_Unicode_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.Radix.Unicode);
		}

		private void toolStripMenuItem_RadixContextMenu_SeparateTxRx_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.SeparateTxRxRadix = !this.settingsRoot.Display.SeparateTxRxRadix;
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_String_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Char_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Bin_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Oct_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Dec_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Hex_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Hex);
		}

		private void toolStripMenuItem_RadixContextMenu_Tx_Unicode_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Tx, Domain.Radix.Unicode);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_String_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Char_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Bin_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Oct_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Dec_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Hex_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Hex);
		}

		private void toolStripMenuItem_RadixContextMenu_Rx_Unicode_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix(Domain.IODirection.Rx, Domain.Radix.Unicode);
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
		private const int menuItems_Predefined_MaxPagesWithMenuItem = 9;

		private void contextMenuStrip_Predefined_Initialize()
		{
			this.menuItems_Predefined_Commands = new List<ToolStripMenuItem>(Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage); // Preset the required capacity to improve memory management.
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

			this.menuItems_Predefined_Pages = new List<ToolStripMenuItem>(menuItems_Predefined_MaxPagesWithMenuItem); // Preset the required capacity to improve memory management.
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_1);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_2);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_3);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_4);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_5);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_6);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_7);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_8);
			this.menuItems_Predefined_Pages.Add(toolStripMenuItem_PredefinedContextMenu_Page_9);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Predefined_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				// Pages:
				var pages = this.settingsRoot.PredefinedCommand.Pages;

				int pageCount = 0;
				if (pages != null)
					pageCount = pages.Count;

				if (pageCount > 0)
				{
					toolStripMenuItem_PredefinedContextMenu_Page_Previous.Enabled  = (predefined.SelectedPage > 1);
					toolStripMenuItem_PredefinedContextMenu_Page_Next    .Enabled  = (predefined.SelectedPage < pageCount);
					toolStripMenuItem_PredefinedContextMenu_Page_Separator.Visible = true;
				}
				else
				{
					toolStripMenuItem_PredefinedContextMenu_Page_Previous.Enabled  = false;
					toolStripMenuItem_PredefinedContextMenu_Page_Next    .Enabled  = false;
					toolStripMenuItem_PredefinedContextMenu_Page_Separator.Visible = false;
				}

				for (int i = 0; i < Math.Min(pageCount, menuItems_Predefined_MaxPagesWithMenuItem); i++)
				{
					this.menuItems_Predefined_Pages[i].Text    =  MenuEx.PrependIndex(i + 1, pages[i].PageName);
					this.menuItems_Predefined_Pages[i].Visible =  true;
					this.menuItems_Predefined_Pages[i].Enabled = (pageCount > 1); // No need to navigate a single page.
				}

				for (int i = pageCount; i < menuItems_Predefined_MaxPagesWithMenuItem; i++)
				{
					this.menuItems_Predefined_Pages[i].Text    =  MenuEx.PrependIndex(i + 1, "<Undefined>");
					this.menuItems_Predefined_Pages[i].Visible =  false;
					this.menuItems_Predefined_Pages[i].Enabled =  false;
				}

				// Commands:
				List<Command> commands = null;
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

						this.menuItems_Predefined_Commands[i].Text = MenuEx.PrependIndex(i + 1, Command.DefineCommandText);
						this.menuItems_Predefined_Commands[i].Enabled = true;
					}
				}

				for (int i = commandCount; i < Model.Settings.PredefinedCommandSettings.MaxCommandsPerPage; i++)
				{
					if (this.menuItems_Predefined_Commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
						this.menuItems_Predefined_Commands[i].ForeColor = SystemColors.GrayText;

					if (this.menuItems_Predefined_Commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
						this.menuItems_Predefined_Commands[i].Font = DrawingEx.DefaultFontItalic;

					this.menuItems_Predefined_Commands[i].Text = MenuEx.PrependIndex(i + 1, Command.DefineCommandText);
					this.menuItems_Predefined_Commands[i].Enabled = true;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <summary>
		/// Temporary reference to command to be copied.
		/// </summary>
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int contextMenuStrip_Predefined_SelectedCommand; // = 0;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Command contextMenuStrip_Predefined_CopyToSendText; // = null;

		private void contextMenuStrip_Predefined_Opening(object sender, CancelEventArgs e)
		{
			if (contextMenuStrip_Predefined.SourceControl == groupBox_Predefined)
			{
				var id = predefined.GetCommandIdFromLocation(new Point(contextMenuStrip_Predefined.Left, contextMenuStrip_Predefined.Top));
				var c = predefined.GetCommandFromId(id);

				contextMenuStrip_Predefined_SelectedCommand = id;
				contextMenuStrip_Predefined_CopyToSendText = c;

				toolStripMenuItem_PredefinedContextMenu_Separator_3.Visible = true;

				var mi = toolStripMenuItem_PredefinedContextMenu_CopyToSendText;
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
				//     => SendText should get validated, but actually isn't!
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
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SendPredefined(ToolStripMenuItemEx.TagToIndex(sender)); // Attention, 'ToolStripMenuItem' is no 'Control'!
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Next_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			predefined.NextPage();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Previous_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			predefined.PreviousPage();
		}

		private void toolStripMenuItem_PredefinedContextMenu_Page_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			predefined.SelectedPage = ToolStripMenuItemEx.TagToIndex(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
		}

		private void toolStripMenuItem_PredefinedContextMenu_Define_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			if (contextMenuStrip_Predefined_SelectedCommand != 0)
				ShowPredefinedCommandSettings(predefined.SelectedPage, contextMenuStrip_Predefined_SelectedCommand);
			else
				ShowPredefinedCommandSettings(predefined.SelectedPage, 1);
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyToSendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var c = new Command(contextMenuStrip_Predefined_CopyToSendText); // Clone command to ensure decoupling.
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
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var c = new Command(this.settingsRoot.SendText.Command); // Clone command to ensure decoupling.
			this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPage - 1, contextMenuStrip_Predefined_SelectedCommand - 1, c);
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var c = new Command(this.settingsRoot.SendFile.Command); // Clone command to ensure decoupling.
			this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPage - 1, contextMenuStrip_Predefined_SelectedCommand - 1, c);
		}

		private void toolStripMenuItem_PredefinedContextMenu_Hide_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

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
			bool isTextTerminal = (this.settingsRoot.TerminalType == Domain.TerminalType.Text);

			this.isSettingControls.Enter();
			try
			{
				// Prepare the menu item properties based on state and settings.
				//
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
				// ...View.Controls.SendText.SetControls()
				// Changes here may have to be applied there too.
				//
				// Context and main menu are separated as there are subtle differences between them.

				string sendTextText = "Send Text";
				bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText;
				if (this.settingsRoot.Send.SendImmediately)
				{
					if (isTextTerminal)
						sendTextText = "Send EOL";
					else
						sendTextEnabled = false;
				}

				bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath;

				// Set the menu item properties:

				toolStripMenuItem_SendContextMenu_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
				toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

				toolStripMenuItem_SendContextMenu_SendText.Text              = sendTextText;
				toolStripMenuItem_SendContextMenu_SendText.Enabled           = sendTextEnabled && this.terminal.IsReadyToSend;
				toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Enabled = sendTextEnabled && this.terminal.IsReadyToSend && !this.settingsRoot.SendText.Command.IsMultiLineText && !this.settingsRoot.Send.SendImmediately;
				toolStripMenuItem_SendContextMenu_SendFile.Enabled           = sendFileEnabled && this.terminal.IsReadyToSend;

				toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Checked = this.settingsRoot.Send.UseExplicitDefaultRadix;

				toolStripMenuItem_SendContextMenu_KeepSendText.Enabled         = !this.settingsRoot.Send.SendImmediately;
				toolStripMenuItem_SendContextMenu_KeepSendText.Checked         = !this.settingsRoot.Send.SendImmediately && this.settingsRoot.Send.KeepSendText;
				toolStripMenuItem_SendContextMenu_CopyPredefined.Checked       =  this.settingsRoot.Send.CopyPredefined;
				toolStripMenuItem_SendContextMenu_SendImmediately.Checked      =  this.settingsRoot.Send.SendImmediately;
				toolStripMenuItem_SendContextMenu_EnableEscapesForText.Checked =  this.settingsRoot.Send.EnableEscapes;

				toolStripMenuItem_SendContextMenu_ExpandMultiLineText.Enabled  =  this.settingsRoot.SendText.Command.IsMultiLineText;

				toolStripMenuItem_SendContextMenu_SkipEmptyLines.Enabled       = isTextTerminal;
				toolStripMenuItem_SendContextMenu_SkipEmptyLines.Checked       = this.settingsRoot.TextTerminal.SendFile.SkipEmptyLines;
				toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Enabled = isTextTerminal;
				toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Checked = this.settingsRoot.TextTerminal.SendFile.EnableEscapes;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void contextMenuStrip_Send_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Send_SetMenuItems();
		}

		private void toolStripMenuItem_SendContextMenu_SendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			if (!this.settingsRoot.Send.SendImmediately)
				this.terminal.SendText();
			else
				this.terminal.SendPartialTextEol();
		}

		private void toolStripMenuItem_SendContextMenu_SendTextWithoutEol_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			if (!this.settingsRoot.Send.SendImmediately)
				this.terminal.SendTextWithoutEol();
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + @"""Send Text w/o EOL"" is invalid when ""Send Immediately"" is active!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		private void toolStripMenuItem_SendContextMenu_SendFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.terminal.SendFile();
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.SendTextPanelIsVisible = !this.settingsRoot.Layout.SendTextPanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_Panels_SendFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.SendFilePanelIsVisible = !this.settingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.UseExplicitDefaultRadix = !this.settingsRoot.Send.UseExplicitDefaultRadix;
		}

		private void toolStripMenuItem_SendContextMenu_KeepSendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.KeepSendText = !this.settingsRoot.Send.KeepSendText;
		}

		private void toolStripMenuItem_SendContextMenu_CopyPredefined_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.CopyPredefined = !this.settingsRoot.Send.CopyPredefined;
		}

		private void toolStripMenuItem_SendContextMenu_SendImmediately_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.SendImmediately = !this.settingsRoot.Send.SendImmediately;
		}

		private void toolStripMenuItem_SendContextMenu_EnableEscapesForText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.EnableEscapes = !this.settingsRoot.Send.EnableEscapes;
		}

		private void toolStripMenuItem_SendContextMenu_ExpandMultiLineText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.SendText.ExpandMultiLineText();
		}

		private void toolStripMenuItem_SendContextMenu_EnableEscapesForFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.TextTerminal.SendFile.EnableEscapes = !this.settingsRoot.TextTerminal.SendFile.EnableEscapes;
		}

		private void toolStripMenuItem_SendContextMenu_SkipEmptyLines_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.TextTerminal.SendFile.SkipEmptyLines = !this.settingsRoot.TextTerminal.SendFile.SkipEmptyLines;
		}

		#endregion

		#region Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Status_Opening(object sender, CancelEventArgs e)
		{
			this.isSettingControls.Enter();
			try
			{
				bool isSerialPort = ((Domain.IOTypeEx)this.settingsRoot.IOType).IsSerialPort;

				// Flow control count:
				bool showFlowControlOptions = this.settingsRoot.Terminal.IO.FlowControlIsInUse;
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
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_StatusContextMenu_FlowControlCount_ShowCount_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Status.ShowFlowControlCount = !this.settingsRoot.Status.ShowFlowControlCount;
		}

		private void toolStripMenuItem_StatusContextMenu_FlowControlCount_ResetCount_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.terminal.ResetFlowControlCount();
		}

		private void toolStripMenuItem_StatusContextMenu_BreakCount_ShowCount_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Status.ShowBreakCount = !this.settingsRoot.Status.ShowBreakCount;
		}

		private void toolStripMenuItem_StatusContextMenu_BreakCount_ResetCount_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.terminal.ResetBreakCount();
		}

		#endregion

		#region Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Panel Layout
		//------------------------------------------------------------------------------------------

		private void splitContainer_TxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!IsStartingUp && !this.isSettingControls && !IsIntegraMdiLayouting && !IsClosing)
			{
				// No need to 'splitContainerHelper.CalculateUnscaledDistanceFromScaled()' since no
				// panel of 'splitContainer_TxMonitor' is fixed. Code if this was the case:
			////int unscaledDistance = this.splitContainerHelper.CalculateUnscaledDistanceFromScaled(splitContainer_TxMonitor, splitContainer_TxMonitor.SplitterDistance);

				int distance = splitContainer_TxMonitor.SplitterDistance;
				int widthOrHeight = OrientationEx.SizeToWidthOrHeight(splitContainer_TxMonitor, this.settingsRoot.Layout.MonitorOrientation);

				if (this.settingsRoot != null)
				{
					this.isUpdatingSplitterRatio.Enter();
					try
					{
						this.settingsRoot.Layout.TxMonitorSplitterRatio = (float)distance / widthOrHeight;
					}
					finally
					{
						this.isUpdatingSplitterRatio.Leave();
					}
				}
			}
		}

		private void splitContainer_RxMonitor_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!IsStartingUp && !this.isSettingControls && !IsIntegraMdiLayouting && !IsClosing)
			{
				// No need to 'splitContainerHelper.CalculateUnscaledDistanceFromScaled()' since no
				// panel of 'splitContainer_RxMonitor' is fixed. Code if this was the case:
			////int unscaledDistance = this.splitContainerHelper.CalculateUnscaledDistanceFromScaled(splitContainer_RxMonitor, splitContainer_RxMonitor.SplitterDistance);

				int distance = splitContainer_RxMonitor.SplitterDistance;
				int widthOrHeight = OrientationEx.SizeToWidthOrHeight(splitContainer_RxMonitor, this.settingsRoot.Layout.MonitorOrientation);

				if (this.settingsRoot != null)
				{
					this.isUpdatingSplitterRatio.Enter();
					try
					{
						this.settingsRoot.Layout.RxMonitorSplitterRatio = (float)distance / widthOrHeight;
					}
					finally
					{
						this.isUpdatingSplitterRatio.Leave();
					}
				}
			}
		}

		private void splitContainer_Predefined_SplitterMoved(object sender, SplitterEventArgs e)
		{
			if (!IsStartingUp && !this.isSettingControls && !IsIntegraMdiLayouting && !IsClosing)
			{
				// No need to 'splitContainerHelper.CalculateUnscaledDistanceFromScaled()' since no
				// panel of 'splitContainer_Predefined' is fixed. Code if this was the case:
			////int unscaledDistance = this.splitContainerHelper.CalculateUnscaledDistanceFromScaled(splitContainer_Predefined, splitContainer_Predefined.SplitterDistance);

				int distance = splitContainer_Predefined.SplitterDistance;
				int width = splitContainer_Predefined.Width;

				if (this.settingsRoot != null)
				{
					this.isUpdatingSplitterRatio.Enter();
					try
					{
						this.settingsRoot.Layout.PredefinedSplitterRatio = (float)distance / width;
					}
					finally
					{
						this.isUpdatingSplitterRatio.Leave();
					}
				}
			}
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

		/// <remarks>
		/// Ensure that the edit shortcuts such as [Ctrl+A] and [Ctrl+C] are disabled while the send
		/// control is being edited.
		/// </remarks>
		private void monitor_TextFocusedChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();
		}

		/// <remarks>
		/// Ensure that the find shortcuts [*Modifiers*+F] are enabled/disabled properly.
		/// </remarks>
		private void monitor_FindChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();
		}

		#endregion

		#region Controls Event Handlers > Predefined
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Predefined
		//------------------------------------------------------------------------------------------

		private void predefined_SelectedPageChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (this.settingsRoot != null)
				this.settingsRoot.Implicit.Predefined.SelectedPage = predefined.SelectedPage;
		}

		private void predefined_SendCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			if (this.terminal != null)
				this.terminal.SendPredefined(e.Page, e.Command);
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

		private void send_TextCommandChanged(object sender, EventArgs e)
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Implicit.SendText.Command = send.TextCommand;
		}

		/// <remarks>
		/// Ensure that the edit shortcuts such as [Ctrl+A] and [Ctrl+C] are disabled while the send
		/// control is being edited.
		/// </remarks>
		private void send_TextFocusChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();
		}

		private void send_SendTextCommandRequest(object sender, SendTextOptionEventArgs e)
		{
			if (this.terminal != null)
			{
				switch (e.Value)
				{
					case SendTextOption.Normal:     this.terminal.SendText();           break;
					case SendTextOption.WithoutEol: this.terminal.SendTextWithoutEol(); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + e.Value.ToString() + "' is an option that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
		}

		private void send_FileCommandChanged(object sender, EventArgs e)
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Implicit.SendFile.Command = send.FileCommand;
		}

		private void send_SendFileCommandRequest(object sender, EventArgs e)
		{
			if (this.terminal != null)
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
			this.terminalStatusLabels = new List<ToolStripStatusLabel>(12); // Preset the required capacity to improve memory management.

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

			this.terminalStatusLabels_DefaultText = new Dictionary<ToolStripStatusLabel, string>(9); // Preset the required capacity to improve memory management.
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_RFR,           toolStripStatusLabel_TerminalStatus_RFR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_CTS,           toolStripStatusLabel_TerminalStatus_CTS.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DTR,           toolStripStatusLabel_TerminalStatus_DTR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DSR,           toolStripStatusLabel_TerminalStatus_DSR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DCD,           toolStripStatusLabel_TerminalStatus_DCD.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_InputXOnXOff,  toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_OutputXOnXOff, toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_InputBreak,    toolStripStatusLabel_TerminalStatus_InputBreak.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_OutputBreak,   toolStripStatusLabel_TerminalStatus_OutputBreak.Text);

			this.terminalStatusLabels_DefaultToolTipText = new Dictionary<ToolStripStatusLabel, string>(9); // Preset the required capacity to improve memory management.
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
			if (this.terminal != null)
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
			if (this.terminal != null)
				this.terminal.RequestToggleDtr();
		}

		/// <remarks>
		/// Note that label must always be enabled to have the correct appearance, and can thus
		/// always be clicked. It is the responsibility of the underlying Request...() method
		/// to decide whether the request can currently be executed or not.
		/// </remarks>
		private void toolStripStatusLabel_TerminalStatus_InputXOnXOff_Click(object sender, EventArgs e)
		{
			if (this.terminal != null)
				this.terminal.RequestToggleInputXOnXOff();
		}

		/// <remarks>
		/// Note that label must always be enabled to have the correct appearance, and can thus
		/// always be clicked. It is the responsibility of the underlying Request...() method
		/// to decide whether the request can currently be executed or not.
		/// </remarks>
		private void toolStripStatusLabel_TerminalStatus_OutputBreak_Click(object sender, EventArgs e)
		{
			if (this.terminal != null)
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
		/// <remarks>
		/// Using term "IndicatedName" because <see cref="Control.Name"/> already exists.
		/// </remarks>
		public virtual string IndicatedName
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.IndicatedName);
				else
					return ("");
			}
		}

		/// <summary></summary>
		public virtual bool SettingsFileIsReadOnly
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.SettingsFileIsReadOnly);
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
		public virtual bool AllLogsAreOn
		{
			get
			{
				if (TerminalIsAvailable)
					return (this.terminal.AllLogsAreOn);
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

		private bool IsIntegraMdiLayouting
		{
			get { return (this.isIntegralMdiLayouting); }
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Layouting", Justification = "'Layouting' is a correct English term.")]
		public virtual void NotifyIntegralMdiLayouting(bool isLayouting)
		{
			this.isIntegralMdiLayouting = isLayouting;
		}

		private bool IsClosing
		{
			get { return (this.closingState != ClosingState.None); }
		}

		/// <summary></summary>
		public virtual void NotifyClosingFromForm()
		{
			this.closingState = ClosingState.IsClosingFromForm;
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
			if (this.terminal != null)
				return (this.terminal.Save());

			return (false);
		}

		/// <summary></summary>
		public virtual bool RequestCloseFile()
		{
			if (this.terminal != null)
				return (this.terminal.Close());

			return (false);
		}

		/// <summary></summary>
		public virtual bool RequestStartTerminal()
		{
			if (this.terminal != null)
				return (this.terminal.StartIO());

			return (false);
		}

		/// <summary></summary>
		public virtual bool RequestStopTerminal()
		{
			if (this.terminal != null)
				return (this.terminal.StopIO());

			return (false);
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
		public virtual void RequestClear()
		{
			if (this.terminal != null)
				this.terminal.ClearRepositories();
		}

		/// <summary></summary>
		public virtual void RequestRefresh()
		{
			if (this.terminal != null)
				this.terminal.RefreshRepositories();
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
		protected virtual void RequestFind()
		{
			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestFind();
		}

		/// <summary></summary>
		protected virtual bool FindNextIsFeasible
		{
			get
			{
				var main = (this.mdiParent as Main);
				if (main != null)
					return (main.FindNextIsFeasible);
				else
					return (false);
			}
		}

		/// <summary></summary>
		protected virtual bool FindPreviousIsFeasible
		{
			get
			{
				var main = (this.mdiParent as Main);
				if (main != null)
					return (main.FindPreviousIsFeasible);
				else
					return (false);
			}
		}

		/// <summary></summary>
		protected virtual void RequestFindNext()
		{
			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestFindNext();
		}

		/// <summary></summary>
		protected virtual void RequestFindPrevious()
		{
			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestFindPrevious();
		}

		/// <summary></summary>
		public virtual void EmptyFindOnEdit()
		{
			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = "";
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			monitor.ResetFindOnEdit();
		}

		/// <summary></summary>
		public virtual void LeaveFindOnEdit(string pattern)
		{
			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			monitor.ResetFindOnEdit();
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual FindResult TryFindOnEdit(string pattern, out FindDirection resultingDirection)
		{
			// The active pattern semms not have to be saved each time, it is saved on LeaveFindOnEdit() anyway.
			// But, when anything else changes (e.g. find options, or the terminal is closed/opened) the pattern would get reset.

			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			if (monitor.TryFindOnEdit(pattern, ApplicationSettings.RoamingUserSettings.Find.Options, out resultingDirection))
			{
				this.lastFindPattern = pattern;

				return (FindResult.Found);
			}
			else
			{
				bool isFirst = (pattern != this.lastFindPattern);

				return (isFirst ? FindResult.NotFoundAtAll : FindResult.NotFoundAnymore);
			}
		}

		/// <summary></summary>
		public virtual FindResult TryFindNext(string pattern, bool messageBoxIsPermissible)
		{
			// The active pattern wouldn't have to be saved each time, it is saved on LeaveFindOnEdit() anyway.
			// But, the recent has to be saved each time, as the time stamp changes. Thus, saving both anyway.

			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.RoamingUserSettings.Find.RecentPatterns.Add(new RecentItem<string>(pattern));
			ApplicationSettings.RoamingUserSettings.Find.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			if (monitor.TryFindNext(pattern, ApplicationSettings.RoamingUserSettings.Find.Options))
			{
				this.lastFindPattern = pattern;

				return (FindResult.Found);
			}
			else
			{
				bool isFirst = (pattern != this.lastFindPattern);

				if (messageBoxIsPermissible)
					ShowNotFoundMessage(pattern, isFirst);

				return (isFirst ? FindResult.NotFoundAtAll : FindResult.NotFoundAnymore);
			}
		}

		/// <summary></summary>
		public virtual FindResult TryFindPrevious(string pattern, bool messageBoxIsPermissible)
		{
			// The active pattern wouldn't have to be saved each time, it is saved on LeaveFindOnEdit() anyway.
			// But, the recent has to be saved each time, as the time stamp changes. Thus, saving both anyway.

			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.RoamingUserSettings.Find.RecentPatterns.Add(new RecentItem<string>(pattern));
			ApplicationSettings.RoamingUserSettings.Find.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			if (monitor.TryFindPrevious(pattern, ApplicationSettings.RoamingUserSettings.Find.Options))
			{
				this.lastFindPattern = pattern;

				return (FindResult.Found);
			}
			else
			{
				bool isFirst = (pattern != this.lastFindPattern);

				if (messageBoxIsPermissible)
					ShowNotFoundMessage(pattern, isFirst);

				return (isFirst ? FindResult.NotFoundAtAll : FindResult.NotFoundAnymore);
			}
		}

		private void ShowNotFoundMessage(string pattern, bool isFirst)
		{
			var text = new StringBuilder();
			text.Append(@"The specified pattern """);
			text.Append(pattern);
			text.Append(@""" has not been found");

			if (!isFirst)
				text.Append(" anymore");

			text.Append(".");

			MessageBoxEx.Show
			(
				this,
				text.ToString(),
				"Pattern Not Found",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information
			);
		}

		/// <summary></summary>
		public virtual void RequestEditLogSettings()
		{
			ShowLogSettings();
		}

		/// <summary></summary>
		public virtual void RequestSwitchLogOn()
		{
			if (this.terminal != null)
				this.terminal.SwitchLogOn();
		}

		/// <summary></summary>
		public virtual void RequestSwitchLogOff()
		{
			if (this.terminal != null)
				this.terminal.SwitchLogOff();
		}

		/// <summary></summary>
		public virtual void RequestOpenLogFile()
		{
			if (this.terminal != null)
				this.terminal.OpenLogFile();
		}

		/// <summary></summary>
		public virtual void RequestOpenLogDirectory()
		{
			if (this.terminal != null)
				this.terminal.OpenLogDirectory();
		}

		/// <summary></summary>
		public virtual void RequestAutoActionTrigger(AutoTriggerEx trigger)
		{
			if ((trigger == AutoTrigger.AnyLine) && (this.settingsRoot.AutoAction.Action == AutoAction.Highlight))
			{
				var text = new StringBuilder();
				text.AppendLine("Trigger cannot be set to 'Any Line' if action is 'Highlight Only'!");
				text.AppendLine();
				text.Append    ("Reason: Lines are not highlighted if trigger is 'Any Line' as that would result in all received lines highlighted.");

				MessageBoxEx.Show
				(
					this,
					text.ToString(),
					"Currently Invalid Trigger",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);
			}
			else
			{
				this.settingsRoot.AutoAction.Trigger = trigger;
			}
		}

		/// <summary></summary>
		public virtual void RequestAutoActionAction(AutoActionEx action)
		{
			if ((action == AutoAction.Highlight) && (this.settingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine))
			{
				var text = new StringBuilder();
				text.AppendLine("Action cannot be set to 'Highlight Only' if trigger is 'Any Line'!");
				text.AppendLine();
				text.Append    ("Reason: Lines are not highlighted if trigger is 'Any Line' as that would result in all received lines highlighted.");

				MessageBoxEx.Show
				(
					this,
					text.ToString(),
					"Currently Invalid Action",
					MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation
				);
			}
			else
			{
				this.settingsRoot.AutoAction.Action = action;
			}
		}

		/// <summary></summary>
		public virtual void RequestAutoActionResetCount()
		{
			if (this.terminal != null)
				this.terminal.ResetAutoActionCount();
		}

		/// <summary></summary>
		public virtual void RequestAutoActionDeactivate()
		{
			if (this.terminal != null)
				this.terminal.DeactivateAutoAction();
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseTrigger(AutoTriggerEx trigger)
		{
			this.settingsRoot.AutoResponse.Trigger = trigger;
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseResponse(AutoResponseEx response)
		{
			this.settingsRoot.AutoResponse.Response = response;
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseResetCount()
		{
			if (this.terminal != null)
				this.terminal.ResetAutoResponseCount();
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseDeactivate()
		{
			if (this.terminal != null)
				this.terminal.DeactivateAutoResponse();
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
			var strips = new List<ContextMenuStrip>(6); // Preset the required capacity to improve memory management.
			strips.Add(contextMenuStrip_Preset);
			strips.Add(contextMenuStrip_Monitor);
			strips.Add(contextMenuStrip_Radix);
			strips.Add(contextMenuStrip_Predefined);
			strips.Add(contextMenuStrip_Send);
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

			this.contextMenuStripShortcutTargetWorkaround = new ContextMenuStripShortcutTargetWorkaround(itemCount); // Preset the required capacity to improve memory management.
			foreach (ContextMenuStrip strip in strips)
				this.contextMenuStripShortcutTargetWorkaround.Add(strip);
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
			try
			{
				SuspendLayout();

				if (!this.isUpdatingSplitterRatio) // Required to prevent recursion/reverting of predefined splitter distance due to rounding errors.
				{
					// splitContainer_Predefined:
					if (this.settingsRoot.Layout.PredefinedPanelIsVisible)
					{
						splitContainer_Predefined.Panel2Collapsed = false;

						// No need to 'splitContainerHelper.CalculateScaledDistanceFromUnscaled()' since no
						// panel of 'splitContainer_Predefined' is fixed. Code if this was the case:
					////int unscaledDistance = (int)((this.settingsRoot.Layout.PredefinedSplitterRatio * splitContainer_Predefined.Width) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
					////int scaledDistance = this.splitContainerHelper.CalculateScaledDistanceFromUnscaled(splitContainer_Predefined, unscaledDistance);

						int distance = (int)((this.settingsRoot.Layout.PredefinedSplitterRatio * splitContainer_Predefined.Width) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.

						splitContainer_Predefined.SplitterDistance = Int32Ex.Limit(distance, 0, (splitContainer_Predefined.Width - 1));
					}
					else
					{
						splitContainer_Predefined.Panel2Collapsed = true;
					}

					// splitContainer_Tx/RxMonitor:

					// Orientation:
					var orientation = this.settingsRoot.Layout.MonitorOrientation;
					splitContainer_TxMonitor.Orientation = orientation;
					splitContainer_RxMonitor.Orientation = orientation;

					// One of the panels MUST be visible, if none is visible, then bidir is shown anyway.
					bool txIsVisible    = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
					bool bidirIsVisible = this.settingsRoot.Layout.BidirMonitorPanelIsVisible || (!this.settingsRoot.Layout.TxMonitorPanelIsVisible && !this.settingsRoot.Layout.RxMonitorPanelIsVisible);
					bool rxIsVisible    = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

					// Tx split contains Tx and Bidir+Rx:
					if (txIsVisible)
					{
						splitContainer_TxMonitor.Panel1Collapsed = false;

						if (bidirIsVisible || rxIsVisible)
						{
							int widthOrHeight = OrientationEx.SizeToWidthOrHeight(splitContainer_TxMonitor, orientation);

							// No need to 'splitContainerHelper.CalculateScaledDistanceFromUnscaled()' since no
							// panel of 'splitContainer_TxMonitor' is fixed. Code if this was the case:
						////int unscaledDistance = (int)((this.settingsRoot.Layout.TxMonitorSplitterRatio * widthOrHeight) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
						////int scaledDistance = this.splitContainerHelper.CalculateScaledDistanceFromUnscaled(splitContainer_TxMonitor, unscaledDistance);

							int distance = (int)((this.settingsRoot.Layout.TxMonitorSplitterRatio * widthOrHeight) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.

							splitContainer_TxMonitor.SplitterDistance = Int32Ex.Limit(distance, 0, (widthOrHeight - 1));
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
							int widthOrHeight = OrientationEx.SizeToWidthOrHeight(splitContainer_RxMonitor, orientation);

							// No need to 'splitContainerHelper.CalculateScaledDistanceFromUnscaled()' since no
							// panel of 'splitContainer_RxMonitor' is fixed. Code if this was the case:
						////int unscaledDistance = (int)((this.settingsRoot.Layout.RxMonitorSplitterRatio * widthOrHeight) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.
						////int scaledDistance = this.splitContainerHelper.CalculateScaledDistanceFromUnscaled(splitContainer_RxMonitor, unscaledDistance);

							int distance = (int)((this.settingsRoot.Layout.RxMonitorSplitterRatio * widthOrHeight) + 0.5f); // Minimalistic rounding is sufficient and more performant, since Math.Round() doesn't provide a 'float' overload.

							splitContainer_RxMonitor.SplitterDistance = Int32Ex.Limit(distance, 0, (widthOrHeight - 1));
						}
					}
					else
					{
						splitContainer_RxMonitor.Panel1Collapsed = true;
					}
					splitContainer_RxMonitor.Panel2Collapsed = !rxIsVisible;

					// Update last monitor selection:
					switch (this.lastMonitorSelection)
					{
						case Domain.RepositoryType.None: // This is the case on startup.
						{
							if (bidirIsVisible) // BiDir (center) is priority #1.
								this.lastMonitorSelection = Domain.RepositoryType.Bidir;
							else if (txIsVisible) // Tx (left) is priority #2.
								this.lastMonitorSelection = Domain.RepositoryType.Tx;
							else if (rxIsVisible)
								this.lastMonitorSelection = Domain.RepositoryType.Rx;
							else
								throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "One of the monitors must be visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

							break;
						}

						case Domain.RepositoryType.Tx:
						{
							if (!txIsVisible)
							{
								if (bidirIsVisible) // BiDir (center) is priority #1.
									this.lastMonitorSelection = Domain.RepositoryType.Bidir;
								else if (rxIsVisible)
									this.lastMonitorSelection = Domain.RepositoryType.Rx;
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "One of the monitors must be visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}

							break;
						}

						case Domain.RepositoryType.Bidir:
						{
							if (!bidirIsVisible)
							{
								if (txIsVisible) // Tx (left) is priority #2.
									this.lastMonitorSelection = Domain.RepositoryType.Tx;
								else if (rxIsVisible)
									this.lastMonitorSelection = Domain.RepositoryType.Rx;
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "One of the monitors must be visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}

							break;
						}

						case Domain.RepositoryType.Rx:
						{
							if (!rxIsVisible)
							{
								if (bidirIsVisible) // BiDir (center) is priority #1.
									this.lastMonitorSelection = Domain.RepositoryType.Bidir;
								else if (txIsVisible)
									this.lastMonitorSelection = Domain.RepositoryType.Tx;
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "One of the monitors must be visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}

							break;
						}

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.lastMonitorSelection + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}

				// splitContainer_Terminal and splitContainer_Send:
				if (this.settingsRoot.Layout.SendTextPanelIsVisible || this.settingsRoot.Layout.SendFilePanelIsVisible)
				{
					splitContainer_Terminal.Panel2Collapsed = false;
					panel_Monitor   .Padding = new Padding(3, 3, 1, 0);
					panel_Predefined.Padding = new Padding(1, 3, 3, 0);
				}
				else
				{
					splitContainer_Terminal.Panel2Collapsed = true;
					panel_Monitor   .Padding = new Padding(3, 3, 1, 3);
					panel_Predefined.Padding = new Padding(1, 3, 3, 3);
				}

				send.TextPanelIsVisible = this.settingsRoot.Layout.SendTextPanelIsVisible;
				send.FilePanelIsVisible = this.settingsRoot.Layout.SendFilePanelIsVisible;

				// Adjust send panel size depending on one or two sub-panels:
				if (splitContainer_Terminal.Panel2MinSize != panel_Send.Height)
					splitContainer_Terminal.Panel2MinSize = panel_Send.Height;

				// Local scope for 'distance':
				{
					int distance = splitContainer_Terminal.Height - panel_Send.Height - splitContainer_Terminal.SplitterWidth;
					if (splitContainer_Terminal.SplitterDistance != distance)
						splitContainer_Terminal.SplitterDistance = Int32Ex.Limit(distance, 0, (splitContainer_Terminal.Height - 1));
				}

				LayoutSend();

				ResumeLayout();
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void LayoutSend()
		{
			// Calculate absolute splitter position (distance) of predefined splitter,
			// including offset to get send buttons pixel-accurate below predefined buttons:
			const int PredefinedOffset = 6; // 2 x margin of 3 (frame + buttons)
			int absoluteX = splitContainer_Predefined.SplitterDistance + splitContainer_Predefined.Left;
			int relativeX = absoluteX - send.Left + PredefinedOffset;
			send.SendSplitterDistance = Int32Ex.Limit(relativeX, 0, (send.Width - 1));
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

		private void SetPresetControls()
		{
			contextMenuStrip_Preset_SetMenuItems();
		}

		/// <remarks>
		/// Only sets those predefined settings that are dependent on the terminal state.
		/// Pages are set in the settings handler.
		/// </remarks>
		private void SetPredefinedControls()
		{
			contextMenuStrip_Predefined_SetMenuItems(); // Ensure that shortcuts are activated.

			this.isSettingControls.Enter();
			try
			{
				predefined.TerminalIsReadyToSend = this.terminal.IsReadyToSend;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetSendControls()
		{
			toolStripMenuItem_TerminalMenu_Send_SetMenuItems();
			contextMenuStrip_Send_SetMenuItems();

			this.isSettingControls.Enter();
			try
			{
				send.TextCommand             = this.settingsRoot.SendText.Command;
				send.RecentTextCommands      = this.settingsRoot.SendText.RecentCommands;
				send.FileCommand             = this.settingsRoot.SendFile.Command;
				send.RecentFileCommands      = this.settingsRoot.SendFile.RecentCommands;
				send.TerminalType            = this.settingsRoot.TerminalType;
				send.UseExplicitDefaultRadix = this.settingsRoot.Send.UseExplicitDefaultRadix;
				send.ParseModeForText        = this.settingsRoot.Send.ToParseMode();
				send.SendTextImmediately     = this.settingsRoot.Send.SendImmediately;
				send.TerminalIsReadyToSend   = this.terminal.IsReadyToSend;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_Print_EnabledToRestore; // = false;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_Find_EnabledToRestore; // = false;

		/// <summary>
		/// Suspends the [Control+F/N/P] shortcuts.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FNP", Justification = "FNP refers to these three specific keys.")]
		public virtual void SuspendCtrlFNPShortcuts()
		{
			toolStripMenuItem_TerminalMenu_Terminal_Print_EnabledToRestore = toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled;
			toolStripMenuItem_TerminalMenu_Terminal_Find_EnabledToRestore  = toolStripMenuItem_TerminalMenu_Terminal_Find.Enabled;
		
			toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled = false;
			toolStripMenuItem_TerminalMenu_Terminal_Find.Enabled  = false;
		
			// Could be implemented more cleverly, by iterating over all potential shortcut controls
			// and then handle those that use one of the shortcuts in question. However, that would
			// be an overkill, thus using this straight-forward implementation.
		}

		/// <summary>
		/// Resumes the [Control+F/N/P] shortcuts.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FNP", Justification = "FNP refers to these three specific keys.")]
		public virtual void ResumeCtrlFNPShortcuts()
		{
			toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled = toolStripMenuItem_TerminalMenu_Terminal_Print_EnabledToRestore;
			toolStripMenuItem_TerminalMenu_Terminal_Find.Enabled  = toolStripMenuItem_TerminalMenu_Terminal_Find_EnabledToRestore;
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
				case 1: presetString =   "2400, 7, Even, 1, None";     break;
				case 2: presetString =   "2400, 7, Even, 1, Software"; break;
				case 3: presetString =   "9600, 8, None, 1, None";     break;
				case 4: presetString =   "9600, 8, None, 1, Software"; break;
				case 5: presetString =  "19200, 8, None, 1, None";     break;
				case 6: presetString =  "19200, 8, None, 1, Software"; break;
				case 7: presetString = "115200, 8, None, 1, None";     break;
				case 8: presetString = "115200, 8, None, 1, Software"; break;

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + preset + "' is a preset that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			SuspendHandlingTerminalSettings();
			try
			{
				var ts = this.settingsRoot.Explicit.Terminal;
				var scs = ts.IO.SerialPort.Communication;
				scs.SuspendChangeEvent();
				try
				{
					switch (preset)
					{
						case 1: // "2400, 7, Even, 1, None"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud002400;
							scs.DataBits    = MKY.IO.Ports.DataBits.Seven;
							scs.Parity      = System.IO.Ports.Parity.Even;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 2: // "2400, 7, Even, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud002400;
							scs.DataBits    = MKY.IO.Ports.DataBits.Seven;
							scs.Parity      = System.IO.Ports.Parity.Even;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
							break;
						}
						case 3: // "9600, 8, None, 1, None"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud009600;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 4: // "9600, 8, None, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud009600;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
							break;
						}
						case 5: // "19200, 8, None, 1, None"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud019200;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 6: // "19200, 8, None, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud019200;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
							break;
						}
						case 7: // "115200, 8, None, 1, None"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud115200;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 8: // "115200, 8, None, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud115200;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
							break;
						}
					}
				}
				finally
				{
					scs.ResumeChangeEvent();
				}

				if (scs.HaveChanged)
				{
					this.terminal.ApplySettings(ts);
					SetTimedStatusText("Terminal settings set to " + presetString + ".");
				}
				else
				{
					SetTimedStatusText("Terminal settings not changed.");
				}
			}
			finally
			{
				ResumeHandlingTerminalSettings();
			}
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
			if ((source == monitor_Tx)    || (source == panel_Monitor_Tx))    return (Domain.RepositoryType.Tx);
			if ((source == monitor_Bidir) || (source == panel_Monitor_Bidir)) return (Domain.RepositoryType.Bidir);
			if ((source == monitor_Rx)    || (source == panel_Monitor_Rx))    return (Domain.RepositoryType.Rx);

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

				default: return (null);
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

		private void SetMonitorSettings()
		{
			monitor_Tx   .MaxLineCount = this.settingsRoot.Display.MaxLineCount;
			monitor_Bidir.MaxLineCount = this.settingsRoot.Display.MaxLineCount;
			monitor_Rx   .MaxLineCount = this.settingsRoot.Display.MaxLineCount;

			bool showBufferLineNumbers = this.settingsRoot.Display.ShowBufferLineNumbers;
			bool showTotalLineNumbers  = this.settingsRoot.Display.ShowTotalLineNumbers;

			monitor_Tx   .SetLineNumbers(showBufferLineNumbers, showTotalLineNumbers);
			monitor_Bidir.SetLineNumbers(showBufferLineNumbers, showTotalLineNumbers);
			monitor_Rx   .SetLineNumbers(showBufferLineNumbers, showTotalLineNumbers);

			monitor_Tx   .ShowCopyOfActiveLine = this.settingsRoot.Display.ShowCopyOfActiveLine;
			monitor_Bidir.ShowCopyOfActiveLine = this.settingsRoot.Display.ShowCopyOfActiveLine;
			monitor_Rx   .ShowCopyOfActiveLine = this.settingsRoot.Display.ShowCopyOfActiveLine;
		}

		private void SetMonitorIOStatus()
		{
			var activityState = MonitorActivityState.Inactive;
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
			monitor_Tx   .ActivityState = activityState;
			monitor_Bidir.ActivityState = activityState;
			monitor_Rx   .ActivityState = activityState;
		}

		private void SetMonitorCountAndRateStatus()
		{
			bool showConnectTime = this.settingsRoot.Status.ShowConnectTime;
			monitor_Tx   .ShowTimeStatus = showConnectTime;
			monitor_Bidir.ShowTimeStatus = showConnectTime;
			monitor_Rx   .ShowTimeStatus = showConnectTime;

			bool showCountAndRate = this.settingsRoot.Status.ShowCountAndRate;
			monitor_Tx   .ShowDataStatus = showCountAndRate;
			monitor_Bidir.ShowDataStatus = showCountAndRate;
			monitor_Rx   .ShowDataStatus = showCountAndRate;
		}

		private void ReloadMonitors()
		{
			SetFixedStatusText("Reloading...");
			Cursor = Cursors.WaitCursor;

			this.terminal.RefreshRepositories();

			Cursor = Cursors.Default;
			SetTimedStatusText("Reloading done");
		}

		private void ReformatMonitors()
		{
			SetFixedStatusText("Reformatting...");
			Cursor = Cursors.WaitCursor;

			                                // Clone settings to ensure decoupling:
			monitor_Tx   .FormatSettings = new Format.Settings.FormatSettings(this.settingsRoot.Format);
			monitor_Bidir.FormatSettings = new Format.Settings.FormatSettings(this.settingsRoot.Format);
			monitor_Rx   .FormatSettings = new Format.Settings.FormatSettings(this.settingsRoot.Format);

			Cursor = Cursors.Default;
			SetTimedStatusText("Reformatting done");
		}

		private void ClearMonitor(Domain.RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case Domain.RepositoryType.None:
					// Nothing to do.
					break;

				case Domain.RepositoryType.Tx:
				case Domain.RepositoryType.Bidir:
				case Domain.RepositoryType.Rx:
					this.terminal.ClearRepository(repositoryType);
					break;

				default:
					throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void RefreshMonitor(Domain.RepositoryType repositoryType)
		{
			switch (repositoryType)
			{
				case Domain.RepositoryType.None:
					// Nothing to do.
					break;

				case Domain.RepositoryType.Tx:
				case Domain.RepositoryType.Bidir:
				case Domain.RepositoryType.Rx:
					this.terminal.RefreshRepository(repositoryType);
					break;

				default:
					throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void ClearMonitors()
		{
			this.terminal.ClearRepositories();
		}

		private void RefreshMonitors()
		{
			this.terminal.RefreshRepositories();
		}

		#endregion

		#region Monitor Panels > Methods
		//------------------------------------------------------------------------------------------
		// Monitor Panels > Methods
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowFormatSettings()
		{
			int[] customColors = ApplicationSettings.RoamingUserSettings.View.CustomColorsToWin32();

			var f = new FormatSettings(this.settingsRoot.Format, customColors, this.settingsRoot.Display.InfoSeparator, this.settingsRoot.Display.InfoEnclosure, this.settingsRoot.Display.TimeStampUseUtc, this.settingsRoot.Display.TimeStampFormat, this.settingsRoot.Display.TimeSpanFormat, this.settingsRoot.Display.TimeDeltaFormat);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();
				this.settingsRoot.Format = f.FormatSettingsResult;

				if (!ArrayEx.ElementsEqual(customColors, f.CustomColors))
				{
					ApplicationSettings.RoamingUserSettings.View.UpdateCustomColorsFromWin32(f.CustomColors);
					ApplicationSettings.SaveRoamingUserSettings();
				}

				this.settingsRoot.Display.InfoSeparator = f.InfoSeparatorResult;
				this.settingsRoot.Display.InfoEnclosure = f.InfoEnclosureResult;

				this.settingsRoot.Display.TimeStampUseUtc = f.TimeStampUseUtcResult;
				this.settingsRoot.Display.TimeStampFormat = f.TimeStampFormatResult;
				this.settingsRoot.Display.TimeSpanFormat  = f.TimeSpanFormatResult;
				this.settingsRoot.Display.TimeDeltaFormat = f.TimeDeltaFormatResult;
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
			Utilities.RtfWriterHelper.LinesToClipboard(monitor.SelectedLines, this.settingsRoot.Format);
			SetTimedStatusText("Data copied to clipboard");
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSaveMonitorDialog(Controls.Monitor monitor)
		{
			SetFixedStatusText("Preparing to save data...");

			string initialExtension = ApplicationSettings.LocalUserSettings.Extensions.MonitorFiles;

			var sfd = new SaveFileDialog();
			sfd.Title = "Save As";
			sfd.Filter      = ExtensionHelper.TextFilesFilter;
			sfd.FilterIndex = ExtensionHelper.TextFilesFilterHelper(initialExtension);
			sfd.DefaultExt  = PathEx.DenormalizeExtension(initialExtension);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MonitorFiles;

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Extensions.MonitorFiles = Path.GetExtension(sfd.FileName);
				ApplicationSettings.LocalUserSettings.Paths.MonitorFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

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
					savedCount = Log.Utilities.XmlWriterHelperRaw.LinesToFile(monitor.SelectedLines, filePath, true);
				#else
					savedCount = Log.Utilities.XmlWriterHelperNeat.LinesToFile(monitor.SelectedLines, filePath, true);
				#endif
				}
				else if (ExtensionHelper.IsRtfFile(filePath))
				{
					savedCount = Utilities.RtfWriterHelper.LinesToFile(monitor.SelectedLines, filePath, this.settingsRoot.Format);
				}
				else
				{
					savedCount = Utilities.TextWriterHelper.LinesToFile(monitor.SelectedLines, filePath, this.settingsRoot.Format);
				}

				if (savedCount == requestedCount)
				{
					SetTimedStatusText("Selected lines successfully saved");
				}
				else
				{
					SetFixedStatusText("Selected lines only partially saved!");

					var sb = new StringBuilder();

					sb.Append("Selected lines only partially saved to file, only ");
					sb.Append(savedCount.ToString(CultureInfo.CurrentCulture));
					sb.Append(" instead of ");
					sb.Append(requestedCount.ToString(CultureInfo.CurrentCulture));
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

			var pd = new PrintDialog();
			pd.PrinterSettings = new System.Drawing.Printing.PrinterSettings();

			// Note that the PrintDialog class may not work on AMD64 microprocessors unless you set the UseEXDialog property to true (MSDN):
			if (EnvironmentEx.IsStandardWindows64)
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

			using (var printer = new Utilities.RtfPrinter(settings))
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
						MessageBoxIcon.Error
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
				// If command is invalid, show settings dialog.
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
					// If command is invalid, show settings dialog.
					ShowPredefinedCommandSettings(page, command);
				}
			}
		}

		/// <param name="page">Page 1..max.</param>
		/// <param name="command">Command 1..max.</param>
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowPredefinedCommandSettings(int page, int command)
		{
			var f = new PredefinedCommandSettings
			(
				this.settingsRoot.PredefinedCommand,
				this.settingsRoot.TerminalType,
				this.settingsRoot.Send.UseExplicitDefaultRadix,
				this.settingsRoot.Send.ToParseMode(),
				page,
				command
			);

			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();
				this.settingsRoot.PredefinedCommand = f.SettingsResult;
				this.settingsRoot.Predefined.SelectedPage = f.SelectedPage;
			}
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
				this.settingsRoot.Changed += settingsRoot_Changed;
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Changed -= settingsRoot_Changed;
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

				OnTerminalChanged(EventArgs.Empty);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Implicit))
			{
				HandleImplicitSettings(e.Inner);

				// Do not invoke 'OnTerminalChanged' for implicit settings.
			}
		}

		private void HandleExplicitSettings(SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				SetTerminalControls();
				SetLogControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Terminal))
			{
				HandleTerminalSettings(e.Inner);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.PredefinedCommand))
			{
				this.isSettingControls.Enter();
				try
				{
					predefined.Pages = this.settingsRoot.PredefinedCommand.Pages;
				}
				finally
				{
					this.isSettingControls.Leave();
				}

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
				SetMonitorSettings();
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
				// Nothing to do.
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
				try
				{
					predefined.SelectedPage = this.settingsRoot.Predefined.SelectedPage;
				}
				finally
				{
					this.isSettingControls.Leave();
				}

				SetPredefinedControls();
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
			SetMonitorSettings();
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
				this.terminal.IOChanged                += terminal_IOChanged;
				this.terminal.IOControlChanged         += terminal_IOControlChanged;
				this.terminal.IOConnectTimeChanged     += terminal_IOConnectTimeChanged;
			////this.terminal.IOCountChanged_Promptly  += terminal_IOCountChanged_Promptly; // See further below for reason.
			////this.terminal.IORateChanged_Promptly   += terminal_IORateChanged_Promptly;  // See further below for reason.
				this.terminal.IORateChanged_Decimated  += terminal_IORateChanged_Decimated;
				this.terminal.IOError                  += terminal_IOError;

				this.terminal.DisplayElementsSent      += terminal_DisplayElementsSent;
				this.terminal.DisplayElementsReceived  += terminal_DisplayElementsReceived;
				this.terminal.DisplayLinesSent         += terminal_DisplayLinesSent;
				this.terminal.DisplayLinesReceived     += terminal_DisplayLinesReceived;

				this.terminal.RepositoryCleared        += terminal_RepositoryCleared;
				this.terminal.RepositoryReloaded       += terminal_RepositoryReloaded;

				this.terminal.AutoResponseCountChanged += terminal_AutoResponseCountChanged;
				this.terminal.AutoActionCountChanged   += terminal_AutoActionCountChanged;

				this.terminal.FixedStatusTextRequest   += terminal_FixedStatusTextRequest;
				this.terminal.TimedStatusTextRequest   += terminal_TimedStatusTextRequest;
				this.terminal.ResetStatusTextRequest   += terminal_ResetStatusTextRequest;
				this.terminal.MessageInputRequest      += terminal_MessageInputRequest;
				this.terminal.SaveAsFileDialogRequest  += terminal_SaveAsFileDialogRequest;
				this.terminal.CursorRequest            += terminal_CursorRequest;

				this.terminal.Saved                    += terminal_Saved;
				this.terminal.Closed                   += terminal_Closed;
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged               -= terminal_IOChanged;
				this.terminal.IOControlChanged        -= terminal_IOControlChanged;
				this.terminal.IOConnectTimeChanged    -= terminal_IOConnectTimeChanged;
			////this.terminal.IOCountChanged_Promptly -= terminal_IOCountChanged_Promptly; // See further below for reason.
			////this.terminal.IORateChanged_Promptly  -= terminal_IORateChanged_Promptly;  // See further below for reason.
				this.terminal.IORateChanged_Decimated -= terminal_IORateChanged_Decimated;
				this.terminal.IOError                 -= terminal_IOError;

				this.terminal.DisplayElementsSent     -= terminal_DisplayElementsSent;
				this.terminal.DisplayElementsReceived -= terminal_DisplayElementsReceived;
				this.terminal.DisplayLinesSent        -= terminal_DisplayLinesSent;
				this.terminal.DisplayLinesReceived    -= terminal_DisplayLinesReceived;

				this.terminal.RepositoryCleared       -= terminal_RepositoryCleared;
				this.terminal.RepositoryReloaded      -= terminal_RepositoryReloaded;

				this.terminal.AutoResponseCountChanged -= terminal_AutoResponseCountChanged;
				this.terminal.AutoActionCountChanged   -= terminal_AutoActionCountChanged;

				this.terminal.FixedStatusTextRequest  -= terminal_FixedStatusTextRequest;
				this.terminal.TimedStatusTextRequest  -= terminal_TimedStatusTextRequest;
				this.terminal.ResetStatusTextRequest  -= terminal_ResetStatusTextRequest;
				this.terminal.MessageInputRequest     -= terminal_MessageInputRequest;
				this.terminal.SaveAsFileDialogRequest -= terminal_SaveAsFileDialogRequest;
				this.terminal.CursorRequest           -= terminal_CursorRequest;

				this.terminal.Saved                   -= terminal_Saved;
				this.terminal.Closed                  -= terminal_Closed;
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
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetTerminalControls();

			OnTerminalChanged(EventArgs.Empty);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IOControlChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetIOControlControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IOConnectTimeChanged(object sender, TimeSpanEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
			{
				TimeSpan activeConnectTime;
				TimeSpan totalConnectTime;

				this.terminal.GetConnectTime(out activeConnectTime, out totalConnectTime);

				monitor_Tx   .SetTimeStatus(activeConnectTime, totalConnectTime);
				monitor_Bidir.SetTimeStatus(activeConnectTime, totalConnectTime);
				monitor_Rx   .SetTimeStatus(activeConnectTime, totalConnectTime);
			}
		}

		// 'terminal_IOCount/RateChanged_Promptly' are is not used because of the reasons described
		// in the remarks of 'terminal_RawChunkSent/Received' of 'Model.Terminal'. Instead, the update
		// is done by the 'terminal_DisplayElementsSent/Received' and 'terminal_DisplayLinesSent/Received'
		// handlers further below.
		//
		// 'terminal_IORateChanged_Decimated' is fine.

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_IORateChanged_Decimated(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
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
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

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

		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetSentDataCountAndRateStatus()
		{
			int txByteCount = 0;
			int txLineCount = 0;
			int rxByteCount = 0;
			int rxLineCount = 0;

			this.terminal.GetDataCount(out txByteCount, out txLineCount, out rxByteCount, out rxLineCount);

			monitor_Tx   .SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);
			monitor_Bidir.SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);

			int txByteRate = 0;
			int txLineRate = 0;
			int rxByteRate = 0;
			int rxLineRate = 0;

			this.terminal.GetDataRate(out txByteRate, out txLineRate, out rxByteRate, out rxLineRate);

			monitor_Tx   .SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
			monitor_Bidir.SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetReceivedDataCountAndRateStatus()
		{
			int txByteCount = 0;
			int txLineCount = 0;
			int rxByteCount = 0;
			int rxLineCount = 0;

			this.terminal.GetDataCount(out txByteCount, out txLineCount, out rxByteCount, out rxLineCount);

			monitor_Bidir.SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);
			monitor_Rx   .SetDataCountStatus(txByteCount, txLineCount, rxByteCount, rxLineCount);

			int txByteRate = 0;
			int txLineRate = 0;
			int rxByteRate = 0;
			int rxLineRate = 0;

			this.terminal.GetDataRate(out txByteRate, out txLineRate, out rxByteRate, out rxLineRate);

			monitor_Bidir.SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
			monitor_Rx   .SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataCountAndRateStatus()
		{
			SetSentDataCountAndRateStatus();
			SetReceivedDataCountAndRateStatus();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsSent(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
			{
				monitor_Tx   .AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.
				monitor_Bidir.AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.

				SetSentDataCountAndRateStatus();
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayElementsReceived(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
			{
				monitor_Bidir.AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.
				monitor_Rx   .AddElements(e.Elements.Clone()); // Clone elements to ensure decoupling from event source.

				SetReceivedDataCountAndRateStatus();
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesReceived", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayLinesSent(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
				SetSentDataCountAndRateStatus();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesSent", Rationale = "The raw terminal synchronizes sending/receiving.")]
		private void terminal_DisplayLinesReceived(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
				SetReceivedDataCountAndRateStatus();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_RepositoryCleared(object sender, EventArgs<Domain.RepositoryType> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			switch (e.Value)
			{
				case Domain.RepositoryType.None:  /* Nothing to do. */   break;

				case Domain.RepositoryType.Tx:    monitor_Tx   .Clear(); break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.Clear(); break;
				case Domain.RepositoryType.Rx:    monitor_Rx   .Clear(); break;

				default: throw (new ArgumentOutOfRangeException("e", e.Value, MessageHelper.InvalidExecutionPreamble + "'" + e.Value + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_RepositoryReloaded(object sender, EventArgs<Domain.RepositoryType> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			switch (e.Value)
			{
				case Domain.RepositoryType.None:  /* Nothing to do. */                                                                         break;

				case Domain.RepositoryType.Tx:    monitor_Tx   .AddLines(this.terminal.RepositoryToDisplayLines(Domain.RepositoryType.Tx));    break;
				case Domain.RepositoryType.Bidir: monitor_Bidir.AddLines(this.terminal.RepositoryToDisplayLines(Domain.RepositoryType.Bidir)); break;
				case Domain.RepositoryType.Rx:    monitor_Rx   .AddLines(this.terminal.RepositoryToDisplayLines(Domain.RepositoryType.Rx));    break;

				default: throw (new ArgumentOutOfRangeException("e", e.Value, MessageHelper.InvalidExecutionPreamble + "'" + e.Value + "' is a repository type that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_AutoResponseCountChanged(object sender, EventArgs<int> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnAutoResponseCountChanged(e);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_AutoActionCountChanged(object sender, EventArgs<int> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnAutoActionCountChanged(e);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_FixedStatusTextRequest(object sender, EventArgs<string> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetFixedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_TimedStatusTextRequest(object sender, EventArgs<string> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetTimedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_ResetStatusTextRequest(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			ResetStatusText();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			e.Result = ShowSaveTerminalAsFileDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			Cursor = e.Value;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_Saved(object sender, Model.SavedEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetTerminalControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void terminal_Closed(object sender, Model.ClosedEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			DetachTerminalEventHandlers();
			this.terminal = null;

			DetachSettingsEventHandlers();
			this.settingsRoot = null;

			if (this.closingState == ClosingState.None) // Prevent multiple calls to Close().
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

			var sfd = new SaveFileDialog();
			sfd.Title = "Save " + IndicatedName + " As";
			sfd.Filter      = ExtensionHelper.TerminalFilesFilter;
			sfd.FilterIndex = ExtensionHelper.TerminalFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.TerminalFile);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;

			// Check whether the terminal has already been saved as a .yat file.
			if (StringEx.EndsWithOrdinalIgnoreCase(IndicatedName, ExtensionHelper.TerminalFile))
				sfd.FileName = IndicatedName;
			else
				sfd.FileName = IndicatedName + PathEx.NormalizeExtension(sfd.DefaultExt); // Note that 'DefaultExt' states "The returned string does not include the period."!

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				this.terminal.SaveAs(sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}

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

			var f = new TerminalSettings(this.settingsRoot.Explicit);

			f.TerminalId     = this.terminal.SequentialIndex;
			f.TerminalIsOpen = this.terminal.IsOpen;

			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();

				var fsr = f.SettingsResult;
				if (fsr.HaveChanged)
				{
					SuspendHandlingTerminalSettings();
					try
					{
						this.terminal.ApplySettings(fsr.Terminal);
					}
					finally
					{
						ResumeHandlingTerminalSettings();
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

		#region Terminal > View
		//------------------------------------------------------------------------------------------
		// Terminal > View
		//------------------------------------------------------------------------------------------

		private void SetTerminalControls()
		{
			// Terminal menu:
			toolStripMenuItem_TerminalMenu_File_SetMenuItems();     // No dedicated methods (yet).
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems(); // No dedicated methods (yet).

			// Terminal panel:
			SetTerminalCaption();
			SetIOStatus();
			SetIOControlControls();
			SetMonitorIOStatus();

			// Predefined:
			SetPredefinedControls();

			// Send:
			SetSendControls();

			// Preset:
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
			else // 'TerminalIsNotAvailable'
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

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
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

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Well...")]
		private void SetIOControlControls()
		{
			SuspendLayout(); // Prevent flickering when visibility of status labels temporarily changes.

			Image on  = Properties.Resources.Image_Status_Green_12x12;
			Image off = Properties.Resources.Image_Status_Red_12x12;

			bool isOpen = ((this.terminal != null) ? (this.terminal.IsOpen) : (false));

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
					var pinCount = ((this.terminal != null) ? (this.terminal.SerialPortControlPinCount) : (new MKY.IO.Ports.SerialPortControlPinCount()));

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

					var sentXOnCount      = ((this.terminal != null) ? (this.terminal.SentXOnCount)      : (0));
					var sentXOffCount     = ((this.terminal != null) ? (this.terminal.SentXOffCount)     : (0));
					var receivedXOnCount  = ((this.terminal != null) ? (this.terminal.ReceivedXOnCount)  : (0));
					var receivedXOffCount = ((this.terminal != null) ? (this.terminal.ReceivedXOffCount) : (0));

					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text  += (" | " + sentXOnCount.ToString(CultureInfo.CurrentCulture)     + " | " + sentXOffCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text += (" | " + receivedXOnCount.ToString(CultureInfo.CurrentCulture) + " | " + receivedXOffCount.ToString(CultureInfo.CurrentCulture));

					toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText  += (" | XOn Count | XOff Count");
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText += (" | XOn Count | XOff Count");
				}

				if (this.settingsRoot.Terminal.Status.ShowBreakCount)
				{
					var inputBreakCount      = ((this.terminal != null) ? (this.terminal.InputBreakCount)  : (0));
					var outputBreakCount     = ((this.terminal != null) ? (this.terminal.OutputBreakCount) : (0));

					toolStripStatusLabel_TerminalStatus_InputBreak.Text  += (" | " + inputBreakCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_OutputBreak.Text += (" | " + outputBreakCount.ToString(CultureInfo.CurrentCulture));

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
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "The underlying I/O instance is no serial COM port!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
					var sentXOnCount      = ((this.terminal != null) ? (this.terminal.SentXOnCount)      : (0));
					var sentXOffCount     = ((this.terminal != null) ? (this.terminal.SentXOffCount)     : (0));
					var receivedXOnCount  = ((this.terminal != null) ? (this.terminal.ReceivedXOnCount)  : (0));
					var receivedXOffCount = ((this.terminal != null) ? (this.terminal.ReceivedXOffCount) : (0));

					toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text  += (" | " + sentXOnCount.ToString(CultureInfo.CurrentCulture)     + " | " + sentXOffCount.ToString(CultureInfo.CurrentCulture));
					toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text += (" | " + receivedXOnCount.ToString(CultureInfo.CurrentCulture) + " | " + receivedXOffCount.ToString(CultureInfo.CurrentCulture));

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

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "The timer just invokes a single-shot callback.")]
		private void TriggerRfrLuminescence()
		{
			timer_RfrLuminescence.Enabled = false;

			Image on = Properties.Resources.Image_Status_Green_12x12;
			if (toolStripStatusLabel_TerminalStatus_RFR.Image != on) // Improve performance by only assigning if different.
				toolStripStatusLabel_TerminalStatus_RFR.Image = on;

			timer_RfrLuminescence.Interval = RfrLuminescenceInterval;
			timer_RfrLuminescence.Enabled = true;
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_RfrLuminescence_Tick(object sender, EventArgs e)
		{
			timer_RfrLuminescence.Enabled = false;
			ResetRfr();
		}

		private void ResetRfr()
		{
			bool isOpen = ((this.terminal != null) ? (this.terminal.IsOpen) : (false));

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
			var f = new LogSettings(this.settingsRoot.Log);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();
				this.settingsRoot.Log = f.SettingsResult;
			}
		}

		#endregion

		#region Event Raising
		//==========================================================================================
		// Event Raising
		//==========================================================================================

		/// <summary></summary>
		protected virtual void OnTerminalChanged(EventArgs e)
		{
			EventHelper.RaiseSync(Changed, this, e);
		}

		/// <summary></summary>
		protected virtual void OnTerminalSaved(Model.SavedEventArgs e)
		{
			EventHelper.RaiseSync<Model.SavedEventArgs>(Saved, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoResponseCountChanged(EventArgs<int> e)
		{
			EventHelper.RaiseSync<EventArgs<int>>(AutoResponseCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoActionCountChanged(EventArgs<int> e)
		{
			EventHelper.RaiseSync<EventArgs<int>>(AutoActionCountChanged, this, e);
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

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
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
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					"",
					"[" + guid + "]",
					message
				)
			);
		}

		[Conditional("DEBUG_MDI")]
		private void DebugMdi(string message)
		{
			DebugMessage(message);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
