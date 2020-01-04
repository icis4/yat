//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at https://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Revision$
// $Date$
// $Author$
// ------------------------------------------------------------------------------------------------
// YAT Version 2.1.1 Development
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2020 Matthias Kläy.
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
using System.Runtime.InteropServices;
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
using MKY.Text.RegularExpressions;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Settings.Application;
using YAT.Settings.Model;
using YAT.View.Controls;
using YAT.View.Utilities;

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
		private const int RtsLuminescenceInterval = 179;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Scaling:
		private SizeHelper sendSizeHelper = new SizeHelper();
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
		private string lastFindPattern; // = null; Remark: Using "Pattern" instead of "TextOrPattern" for simplicity.

		// Auto:
		private AutoContentState autoActionTriggerState;    // = AutoContentState.Neutral;
		private AutoContentState autoResponseTriggerState;  // = AutoContentState.Neutral;
		private AutoContentState autoResponseResponseState; // = AutoContentState.Neutral;
		private AutoActionPlot autoActionPlotForm;

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
		public event EventHandler AutoActionSettingsChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<AutoContentState>> AutoActionTriggerStateChanged;

	/////// <summary></summary>
	////public event EventHandler<EventArgs<AutoContentState>> AutoActionTriggerStateChanged is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoActionCountChanged;

		/// <summary></summary>
		public event EventHandler AutoResponseSettingsChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<AutoContentState>> AutoResponseTriggerStateChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<AutoContentState>> AutoResponseResponseStateChanged;

		/// <summary></summary>
		public event EventHandler<EventArgs<int>> AutoResponseCountChanged;

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
			LayoutTerminal(); // Apply the layout before the initial 'Paint' event.

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
			this.sendSizeHelper.AdjustScale(factor);
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

			// In addition to predefined shortcuts in the menus, the shortcut [Ctrl+Shift+F1..F12]
			// shall copy the according 'Predefined Command' to 'Send Text':
			if ((keyData & Keys.Modifiers) == (Keys.Control | Keys.Shift))
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

			LayoutTerminal(); // Reapply layouting for proper behavior 'Send' panel if [Send Text | File] is hidden. \remind (2018-04-08 / MKY) bug #412 "Issue with send panel".

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
		////toolStripComboBox_TerminalMenu_Receive_AutoAction_Action   is a standard ToolStripComboBox.
		////toolStripComboBox_TerminalMenu_View_Panels_Orientation     is a standard ToolStripComboBox.
		////toolStripComboBox_TerminalMenu_View_LineNumbers_Selection  is a standard ToolStripComboBox.
		////toolStripComboBox_MonitorContextMenu_Panels_Orientation    is a standard ToolStripComboBox.
		////toolStripComboBox_MonitorContextMenu_LineNumbers_Selection is a standard ToolStripComboBox.

			send.OnFormDeactivateWorkaround();
		}

		private void Terminal_LocationChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsIntegraMdiLayouting && !IsClosing)
				UpdateWindowSettings();
		}

		private void Terminal_SizeChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsIntegraMdiLayouting && !IsClosing)
				UpdateWindowSettings();
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
				UpdateWindowSettings();
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
			// Skip if WinForms has already determined to cancel:
			if (e.Cancel)
				return;

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
					toolStripMenuItem_TerminalMenu_Terminal_Stop .Enabled =  this.terminal.IsStarted;

					toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled =  this.terminal.SendingIsBusy;
					toolStripMenuItem_TerminalMenu_Terminal_Clear.Enabled =  monitorIsDefined;

					if (this.settingsRoot.Layout.VisibleMonitorPanelCount <= 1)
					{
						toolStripMenuItem_TerminalMenu_Terminal_Clear  .Text = "Cl&ear";   // Indicating "All" for a single
						toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh"; //   panel would be confusing.
					}
					else
					{
						toolStripMenuItem_TerminalMenu_Terminal_Clear  .Text = "Cl&ear All";
						toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh All";
					}
				}
				else
				{
					toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Stop .Enabled = false;

					toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Clear.Enabled = false;

					toolStripMenuItem_TerminalMenu_Terminal_Clear  .Text = "Cl&ear";   // Same as in designer generated code,
					toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh"; //   by default only bidir is visible.
				}

				toolStripMenuItem_TerminalMenu_Terminal_SelectAll .Enabled = (monitorIsDefined && textIsNotFocused); // [Ctrl+A]
				toolStripMenuItem_TerminalMenu_Terminal_SelectNone.Enabled = (monitorIsDefined && textIsNotFocused); // [Ctrl+Delete]

				toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = (monitorIsDefined && textIsNotFocused); // [Ctrl+C]
				toolStripMenuItem_TerminalMenu_Terminal_SaveToFile     .Enabled =  monitorIsDefined;
				toolStripMenuItem_TerminalMenu_Terminal_Print          .Enabled =  monitorIsDefined;

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
				var pages = this.settingsRoot.PredefinedCommand.Pages;

				int pageCount = 0;
				if (pages != null)
					pageCount = pages.Count;

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
				bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText(this.settingsRoot.Send.Text.ToParseMode());
				if (this.settingsRoot.Send.Text.SendImmediately)
				{
					if (isTextTerminal)
						sendTextText = "EOL";
					else
						sendTextEnabled = false;
				}

				bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath(Path.GetDirectoryName(this.terminal.SettingsFilePath));

				// Set the menu item properties:

				toolStripMenuItem_TerminalMenu_Send_Text.Text              =  sendTextText;
				toolStripMenuItem_TerminalMenu_Send_Text.Enabled           = (sendTextEnabled && this.terminal.IsReadyToSend);
				toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Enabled = (sendTextEnabled && this.terminal.IsReadyToSend && !this.settingsRoot.SendText.Command.IsMultiLineText && !this.settingsRoot.Send.Text.SendImmediately);
				toolStripMenuItem_TerminalMenu_Send_File.Enabled           = (sendFileEnabled && this.terminal.IsReadyToSend);

				toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Checked = this.settingsRoot.Send.UseExplicitDefaultRadix;
				toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.Checked        = this.settingsRoot.Send.AllowConcurrency;

				toolStripMenuItem_TerminalMenu_Send_KeepSendText.Enabled         =  !this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_KeepSendText.Checked         = (!this.settingsRoot.Send.Text.SendImmediately && this.settingsRoot.Send.Text.KeepSendText);
				toolStripMenuItem_TerminalMenu_Send_SendImmediately.Checked      =   this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Checked =   this.settingsRoot.Send.Text.EnableEscapes;

				toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Enabled =  this.settingsRoot.SendText.Command.IsMultiLineText;

				toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines.Checked       = this.settingsRoot.Send.File.SkipEmptyLines;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile.Checked = this.settingsRoot.Send.File.EnableEscapes;

				toolStripMenuItem_TerminalMenu_Send_CopyPredefined.Checked = this.settingsRoot.Send.CopyPredefined;

				toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage.Enabled = (pageCount > 0);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_SetMenuItems(); // See remark of that method.

				// Note that 'AutoAction' is implemented in 'Receive'.
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Separated to ease updating on settings change.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_TerminalMenu_Send_AutoResponse.Checked          = this.settingsRoot.AutoResponse.IsActive;
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger.Checked  = this.settingsRoot.AutoResponse.Trigger.IsActive;
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response.Checked = this.settingsRoot.AutoResponse.Response.IsActive;

				// Attention:
				// Similar code exists in...
				// ...toolStripMenuItem_TerminalMenu_Receive_SetMenuItems()
				// ...View.Forms.Main.toolStripButton_MainTool_SetControls()
				// Changes here may have to be applied there too.

				bool triggerTextIsSupported     = false;
				bool triggerRegexIsSupported    = false;
				bool responseReplaceIsSupported = false;

				if (!this.terminalMenuValidationWorkaround_UpdateIsSuspended)
				{
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.Clear();
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.AddRange(this.settingsRoot.GetValidAutoResponseTriggerItems());
					var trigger = this.settingsRoot.AutoResponse.Trigger;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger, trigger, new Command(trigger).SingleLineText); // No explicit default radix available (yet).

					triggerTextIsSupported  = trigger.TextIsSupported;
					triggerRegexIsSupported = trigger.RegexIsSupported;

					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.Clear();
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.AddRange(this.settingsRoot.GetValidAutoResponseItems(Path.GetDirectoryName(this.terminal.SettingsFilePath)));
					var response = this.settingsRoot.AutoResponse.Response;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Send_AutoResponse_Response, response, new Command(response).SingleLineText); // No explicit default radix available (yet).

					responseReplaceIsSupported = response.ReplaceIsSupported;
				}

				SetAutoResponseTriggerStateControls();
				SetAutoResponseResponseStateControls();

				var triggerUseText        = this.settingsRoot.AutoResponse.TriggerOptions.UseText;
				var triggerCaseSensitive  = this.settingsRoot.AutoResponse.TriggerOptions.CaseSensitive;
				var triggerWholeWord      = this.settingsRoot.AutoResponse.TriggerOptions.WholeWord;
				var triggerEnableRegex    = this.settingsRoot.AutoResponse.TriggerOptions.EnableRegex;
				var responseEnableReplace = this.settingsRoot.AutoResponse.ResponseOptions.EnableReplace;

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Checked = (triggerTextIsSupported && triggerUseText);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText.Enabled =  triggerTextIsSupported;

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Checked = (triggerTextIsSupported && triggerUseText && triggerCaseSensitive);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Checked = (triggerTextIsSupported && triggerUseText && triggerWholeWord);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord.Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex.Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex && responseReplaceIsSupported && responseEnableReplace);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex && responseReplaceIsSupported);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Enabled = (this.settingsRoot.AutoResponse.Trigger.IsActive || this.settingsRoot.AutoResponse.Response.IsActive);

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
			if (!this.settingsRoot.Send.Text.SendImmediately)
				this.terminal.SendText();
			else
				this.terminal.SendPartialTextEol();
		}

		private void toolStripMenuItem_TerminalMenu_Send_TextWithoutEol_Click(object sender, EventArgs e)
		{
			if (!this.settingsRoot.Send.Text.SendImmediately)
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

		private void toolStripMenuItem_TerminalMenu_Send_AllowConcurrency_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.AllowConcurrency = !this.settingsRoot.Send.AllowConcurrency;
		}

		private void toolStripMenuItem_TerminalMenu_Send_KeepSendText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.Text.KeepSendText = !this.settingsRoot.Send.Text.KeepSendText;
		}

		private void toolStripMenuItem_TerminalMenu_Send_SendImmediately_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.Text.SendImmediately = !this.settingsRoot.Send.Text.SendImmediately;
		}

		private void toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.Text.EnableEscapes = !this.settingsRoot.Send.Text.EnableEscapes;
		}

		private void toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText_Click(object sender, EventArgs e)
		{
			this.settingsRoot.SendText.ExpandMultiLineText();
		}

		private void toolStripMenuItem_TerminalMenu_Send_SkipEmptyLines_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.File.SkipEmptyLines = !this.settingsRoot.Send.File.SkipEmptyLines;
		}

		private void toolStripMenuItem_TerminalMenu_Send_EnableEscapesForFile_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.File.EnableEscapes = !this.settingsRoot.Send.File.EnableEscapes;
		}

		private void toolStripMenuItem_TerminalMenu_Send_CopyPredefined_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Send.CopyPredefined = !this.settingsRoot.Send.CopyPredefined;
		}

		// While the purpose of
		// ...toolStripMenuItem_TerminalMenu_Send_PredefinedCommands...
		// ...toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsPage...
		// ...toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine...
		// ...is questionable in the 'Send' menu, they must be there to activate the shortcuts.

		private void toolStripMenuItem_TerminalMenu_Send_PredefinedCommandsDefine_Click(object sender, EventArgs e)
		{
			ShowPredefinedCommandSettings(predefined.SelectedPageId, 1);
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
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				var triggerText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerText))
				{
					if (!RequestAutoResponseValidateTriggerTextSilently(triggerText))
					{
						AutoResponseTriggerState = AutoContentState.Invalid;
						return; // Skip request. Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).

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

			AutoResponseTriggerState = AutoContentState.Neutral;
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerUseText();
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerCaseSensitive();
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerWholeWord();
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerEnableRegex();
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
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex)
			{
				var responseText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text;
				if (!string.IsNullOrEmpty(responseText))
				{
					if (!ValidationHelper.ValidateTextSilently(responseText, Domain.Parser.Modes.AllEscapes))
					{
						toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.BackColor = SystemColors.ControlDark;
						toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.ForeColor = SystemColors.ControlText;
						return; // Skip request. Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).

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

			toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.BackColor = SystemColors.Window;
			toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.ForeColor = SystemColors.WindowText;
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseResponseEnableReplace();
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
				toolStripMenuItem_TerminalMenu_Send_AutoAction_SetMenuItems(); // See remark of that method.

				// Note that 'AutoResponse' is implemented in 'Send'.
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Separated to ease updating on settings change.
		/// </remarks>
		private void toolStripMenuItem_TerminalMenu_Send_AutoAction_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_TerminalMenu_Receive_AutoAction.Checked         = this.settingsRoot.AutoAction.IsActive;
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger.Checked = this.settingsRoot.AutoAction.Trigger.IsActive;
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Action.Checked  = this.settingsRoot.AutoAction.Action.IsActive;

				// Attention:
				// Similar code exists in...
				// ...toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
				// ...View.Forms.Main.toolStripButton_MainTool_SetControls()
				// Changes here may have to be applied there too.

				bool triggerTextIsSupported  = false;
				bool triggerRegexIsSupported = false;

				if (!this.terminalMenuValidationWorkaround_UpdateIsSuspended)
				{
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Items.Clear();
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Items.AddRange(this.settingsRoot.GetValidAutoActionTriggerItems());
					var trigger = this.settingsRoot.AutoAction.Trigger;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger, trigger, new Command(trigger).SingleLineText); // No explicit default radix available (yet).

					triggerTextIsSupported  = trigger.TextIsSupported;
					triggerRegexIsSupported = trigger.RegexIsSupported;

					toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Items.Clear();
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Items.AddRange(this.settingsRoot.GetValidAutoActionItems());
					var action = this.settingsRoot.AutoAction.Action;
					ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Receive_AutoAction_Action, action, new Command(action).SingleLineText); // No explicit default radix available (yet).
				}

				SetAutoActionTriggerStateControls();
			////SetAutoActionActionStateControls() is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

				var triggerUseText       = this.settingsRoot.AutoAction.TriggerOptions.UseText;
				var triggerCaseSensitive = this.settingsRoot.AutoAction.TriggerOptions.CaseSensitive;
				var triggerWholeWord     = this.settingsRoot.AutoAction.TriggerOptions.WholeWord;
				var triggerEnableRegex   = this.settingsRoot.AutoAction.TriggerOptions.EnableRegex;

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Checked = (triggerTextIsSupported && triggerUseText);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText.Enabled =  triggerTextIsSupported;

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Checked = (triggerTextIsSupported && triggerUseText && triggerCaseSensitive);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Checked = (triggerTextIsSupported && triggerUseText && triggerWholeWord);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord.Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex.Enabled =  triggerTextIsSupported && triggerUseText && triggerRegexIsSupported;

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Enabled = (this.settingsRoot.AutoAction.Trigger.IsActive || this.settingsRoot.AutoAction.Action.IsActive);
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
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				var triggerText = toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerText))
				{
					if (!ValidationHelper.ValidateTextSilently(triggerText, Domain.Parser.Modes.RadixAndAsciiEscapes))
					{
						toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.BackColor = SystemColors.ControlDark;
						toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.ForeColor = SystemColors.ControlText;
						return; // Skip request. Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).

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

			toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.BackColor = SystemColors.Window;
			toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.ForeColor = SystemColors.WindowText;
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionUseText();
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionCaseSensitive();
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionWholeWord();
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionEnableRegex();
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
				toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation.Items.AddRange(OrientationEx.GetItems());
				toolStripComboBox_TerminalMenu_View_Panels_PageLayout.Items.AddRange(PredefinedCommandPageLayoutEx.GetItems());
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
				bool isText       = true;
				bool isSerialPort = true;

				if (this.settingsRoot != null)
				{
					isText       = (this.settingsRoot.TerminalType == Domain.TerminalType.Text);
					isSerialPort = (this.settingsRoot.IOType       == Domain.IOType.SerialPort);
				}

				// Layout, disable monitor item if the other monitors are hidden:
				toolStripMenuItem_TerminalMenu_View_Panels_Tx.Enabled    = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
				toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
				toolStripMenuItem_TerminalMenu_View_Panels_Rx.Enabled    = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.BidirMonitorPanelIsVisible);

				toolStripMenuItem_TerminalMenu_View_Panels_Tx.Checked    = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
				toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
				toolStripMenuItem_TerminalMenu_View_Panels_Rx.Checked    = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation, (OrientationEx)this.settingsRoot.Layout.MonitorOrientation);

				toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = this.settingsRoot.Layout.PredefinedPanelIsVisible;

				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_View_Panels_PageLayout, (PredefinedCommandPageLayoutEx)this.settingsRoot.PredefinedCommand.PageLayout);

				toolStripMenuItem_TerminalMenu_View_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
				toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

				// Connect time:
				bool showConnectTime = this.settingsRoot.Status.ShowConnectTime;
				toolStripMenuItem_TerminalMenu_View_ConnectTime_ShowConnectTime.Checked  = showConnectTime;
				toolStripMenuItem_TerminalMenu_View_ConnectTime_ResetConnectTime.Enabled = showConnectTime;

				// Counters:
				bool showCountAndRate = this.settingsRoot.Status.ShowCountAndRate;
				toolStripMenuItem_TerminalMenu_View_CountAndRate_ShowCountAndRate.Checked = showCountAndRate;
				toolStripMenuItem_TerminalMenu_View_CountAndRate_ResetCount.Enabled = showCountAndRate;

				// Display:
				bool isShowable = (this.settingsRoot.Display.TxRadixIsShowable ||
				                   this.settingsRoot.Display.RxRadixIsShowable);
				toolStripMenuItem_TerminalMenu_View_ShowRadix.Enabled =  isShowable; // Attention: This 'isShowable' restriction also exists further below as well as in 'View.Forms.AdvancedTerminalSettings'.
				toolStripMenuItem_TerminalMenu_View_ShowRadix.Checked = (isShowable && this.settingsRoot.Display.ShowRadix);

				string showLineNumbersText = "&Show Line Numbers (" + (Domain.Utilities.LineNumberSelectionEx)this.settingsRoot.Display.LineNumberSelection + ")";
				toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Text    = showLineNumbersText;
				toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Checked = this.settingsRoot.Display.ShowLineNumbers;
				toolStripMenuItem_TerminalMenu_View_ShowTimeStamp.Checked   = this.settingsRoot.Display.ShowTimeStamp;
				toolStripMenuItem_TerminalMenu_View_ShowTimeSpan.Checked    = this.settingsRoot.Display.ShowTimeSpan;
				toolStripMenuItem_TerminalMenu_View_ShowTimeDelta.Checked   = this.settingsRoot.Display.ShowTimeDelta;
				toolStripMenuItem_TerminalMenu_View_ShowDevice.Checked      = this.settingsRoot.Display.ShowDevice;
				toolStripMenuItem_TerminalMenu_View_ShowDirection.Checked   = this.settingsRoot.Display.ShowDirection;

				toolStripMenuItem_TerminalMenu_View_ShowEol.Enabled = (isText);
				toolStripMenuItem_TerminalMenu_View_ShowEol.Checked = (isText && this.settingsRoot.TextTerminal.ShowEol);

				string showLengthText = "Show &Length (" + (Domain.Utilities.LengthSelectionEx)this.settingsRoot.Display.LengthSelection + ")";
				toolStripMenuItem_TerminalMenu_View_ShowLength.Text              = showLengthText;
				toolStripMenuItem_TerminalMenu_View_ShowLength.Checked           = this.settingsRoot.Display.ShowLength;
				toolStripMenuItem_TerminalMenu_View_ShowDuration.Checked         = this.settingsRoot.Display.ShowDuration;
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

		private void toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetMonitorOrientation((OrientationEx)toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation.SelectedItem);
		}

		private void toolStripMenuItem_TerminalMenu_View_Panels_Predefined_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Layout.PredefinedPanelIsVisible = !this.settingsRoot.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripComboBox_TerminalMenu_View_Panels_PageLayout_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetPageLayout((PredefinedCommandPageLayoutEx)toolStripComboBox_TerminalMenu_View_Panels_PageLayout.SelectedItem);
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

			// The 'terminal_IOCount/RateChanged_Promptly' events are not used because of the reason
			// described in the remarks of 'terminal_RawChunkSent/Received' of 'Model.Terminal'.
			// Instead, the update is done by the 'terminal_DisplayElements[Tx|Bidir|Rx]Added' and
			// 'terminal_DisplayLines[Tx|Bidir|Rx]Added' handlers. As a consequence, the update must
			// manually be triggered here:

			SetDataCountAndRateStatus();
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowRadix_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowLineNumbers_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowLineNumbers = !this.settingsRoot.Display.ShowLineNumbers;
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

		private void toolStripMenuItem_TerminalMenu_View_ShowDevice_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in 'View.Forms.AdvancedTerminalSetting.checkBox_ShowDevice_CheckedChanged()'!
			// Changes here must most likely be applied there too.

			if (toolStripMenuItem_TerminalMenu_View_ShowDevice.Checked && !this.settingsRoot.Display.DeviceLineBreakEnabled)
			{
				var isServerSocket = this.settingsRoot.IO.IOTypeIsServerSocket;
				if (isServerSocket) // Attention: This 'isServerSocket' restriction is also implemented at other locations!
				{
					var dr = MessageBoxEx.Show
					(
						this,
						"To enable this setting, lines must be broken when I/O device changes.",
						"Incompatible Setting",
						MessageBoxButtons.OKCancel,
						MessageBoxIcon.Information
					);

					if (dr == DialogResult.OK)
					{
						this.settingsRoot.Display.ShowDevice             = true;
						this.settingsRoot.Display.DeviceLineBreakEnabled = true;
					}
				}
				else
				{
					// Silently make setting compatible:
					this.settingsRoot.Display.ShowDevice             = true;
					this.settingsRoot.Display.DeviceLineBreakEnabled = true;
				}
			}
			else
			{
				this.settingsRoot.Display.ShowDevice = !this.settingsRoot.Display.ShowDevice;
			}
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

		private void toolStripMenuItem_TerminalMenu_View_ShowDuration_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowDuration = !this.settingsRoot.Display.ShowDuration;
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
				bool isSerialPort = true;
				if (this.settingsRoot != null)
					isSerialPort = (this.settingsRoot.IOType == Domain.IOType.SerialPort);

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

			RequestPreset(ToolStripMenuItemEx.TagToInt32(sender)); // Attention, 'ToolStripMenuItem' is no 'Control'!
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

				ToolStripComboBoxHelper.Select(toolStripComboBox_MonitorContextMenu_Panels_Orientation, (OrientationEx)this.settingsRoot.Layout.MonitorOrientation);

				bool isShowable = (this.settingsRoot.Display.TxRadixIsShowable ||
				                   this.settingsRoot.Display.RxRadixIsShowable);
				toolStripMenuItem_MonitorContextMenu_ShowRadix.Enabled =  isShowable; // Attention: This 'isShowable' restriction also exists further above as well as in 'View.Forms.AdvancedTerminalSettings'.
				toolStripMenuItem_MonitorContextMenu_ShowRadix.Checked = (isShowable && this.settingsRoot.Display.ShowRadix);

				string showLineNumbersText = "Show Line Numbers (" + (Domain.Utilities.LineNumberSelectionEx)this.settingsRoot.Display.LineNumberSelection + ")";
				toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Text    = showLineNumbersText;
				toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Checked = this.settingsRoot.Display.ShowLineNumbers;
				toolStripMenuItem_MonitorContextMenu_ShowTimeStamp.Checked   = this.settingsRoot.Display.ShowTimeStamp;
				toolStripMenuItem_MonitorContextMenu_ShowTimeSpan.Checked    = this.settingsRoot.Display.ShowTimeSpan;
				toolStripMenuItem_MonitorContextMenu_ShowTimeDelta.Checked   = this.settingsRoot.Display.ShowTimeDelta;
				toolStripMenuItem_MonitorContextMenu_ShowDevice.Checked      = this.settingsRoot.Display.ShowDevice;
				toolStripMenuItem_MonitorContextMenu_ShowDirection.Checked   = this.settingsRoot.Display.ShowDirection;

				bool isText = (terminalType == Domain.TerminalType.Text);
				toolStripMenuItem_MonitorContextMenu_ShowEol.Enabled = (isText);
				toolStripMenuItem_MonitorContextMenu_ShowEol.Checked = (isText && this.settingsRoot.TextTerminal.ShowEol);

				string showLengthText = "Show Length (" + (Domain.Utilities.LengthSelectionEx)this.settingsRoot.Display.LengthSelection + ")";
				toolStripMenuItem_MonitorContextMenu_ShowLength.Text              = showLengthText;
				toolStripMenuItem_MonitorContextMenu_ShowLength.Checked           = this.settingsRoot.Display.ShowLength;
				toolStripMenuItem_MonitorContextMenu_ShowDuration.Checked         = this.settingsRoot.Display.ShowDuration;
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

			// The 'terminal_IOCount/RateChanged_Promptly' events are not used because of the reason
			// described in the remarks of 'terminal_RawChunkSent/Received' of 'Model.Terminal'.
			// Instead, the update is done by the 'terminal_DisplayElements[Tx|Bidir|Rx]Added' and
			// 'terminal_DisplayLines[Tx|Bidir|Rx]Added' handlers. As a consequence, the update must
			// manually be triggered here:

			SetDataCountAndRateStatus();
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowRadix_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_MonitorContextMenu_ShowLineNumbers_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowLineNumbers = !this.settingsRoot.Display.ShowLineNumbers;
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

		private void toolStripMenuItem_MonitorContextMenu_ShowDevice_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowDevice = !this.settingsRoot.Display.ShowDevice;
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

		private void toolStripMenuItem_MonitorContextMenu_ShowDuration_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Display.ShowDuration = !this.settingsRoot.Display.ShowDuration;
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

		private void contextMenuStrip_Predefined_Initialize()
		{
			this.isSettingControls.Enter();
			try
			{
				toolStripComboBox_PredefinedContextMenu_Layout.Items.AddRange(PredefinedCommandPageLayoutEx.GetItems());
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int contextMenuStrip_Predefined_SelectedCommandId; // = 0;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "'c' = Command.")]
		private void contextMenuStrip_Predefined_Opening(object sender, CancelEventArgs e)
		{
			this.isSettingControls.Enter();
			try
			{
				// Attention:
				// Similar code exists in...
				// ...View.Forms.PredefinedCommandSettings.contextMenuStrip_Commands_Opening()
				// Changes here may have to be applied there too.

				PredefinedCommandPageLayoutEx pageLayoutEx = this.settingsRoot.PredefinedCommand.PageLayout;
				var np = pageLayoutEx.CommandCapacityPerPage;
				var id = predefined.GetCommandIdFromLocation(Cursor.Position);
				var c  = predefined.GetCommandFromId(id);
				var cIsDefined = ((id != 0) && (c != null) && (c.IsDefined));

				contextMenuStrip_Predefined_SelectedCommandId = id;

				toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Checked = this.settingsRoot.Layout.PredefinedPanelIsVisible;

				ToolStripComboBoxHelper.Select(toolStripComboBox_PredefinedContextMenu_Layout, pageLayoutEx);

				toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Enabled = ((id != 0) && (this.settingsRoot.SendText.Command != null) && (this.settingsRoot.SendText.Command.IsText));
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Enabled = ((id != 0) && (this.settingsRoot.SendFile.Command != null) && (this.settingsRoot.SendFile.Command.IsFilePath));

				var mi = toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile;
				mi.Visible = true;
				if (c != null)
				{
					mi.Enabled = (c.IsText || c.IsFilePath);
					if (c.IsText)
						mi.Text = "Copy to Send Text";
					else if (c.IsFilePath)
						mi.Text = "Copy to Send File";
					else
						mi.Text = "Copy to Send"; // Omitting "Text|File" since it rather confuses than explains.
				}
				else
				{
					mi.Enabled = false;
					mi.Text = "Copy to Send"; // Omitting "Text|File" since it rather confuses than explains.
				}

				// There is a limitaion in Windows.Forms:
				//  1. Edit command in SendText
				//  2. Right-click to open the predefined context menu
				//     => SendText should get validated, but actually isn't!
				//
				// Workaround:
				send.ValidateSendTextInput();

				toolStripMenuItem_PredefinedContextMenu_CopyTo.Enabled = cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo.Enabled = cIsDefined;

				toolStripMenuItem_PredefinedContextMenu_CopyTo_1 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_1 .Enabled =           (cIsDefined && (id != 1));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_2 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_2 .Enabled =           (cIsDefined && (id != 2));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_3 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_3 .Enabled =           (cIsDefined && (id != 3));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_4 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_4 .Enabled =           (cIsDefined && (id != 4));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_5 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_5 .Enabled =           (cIsDefined && (id != 5));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_6 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_6 .Enabled =           (cIsDefined && (id != 6));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_7 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_7 .Enabled =           (cIsDefined && (id != 7));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_8 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_8 .Enabled =           (cIsDefined && (id != 8));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_9 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_9 .Enabled =           (cIsDefined && (id != 9));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_10.Enabled =           (cIsDefined && (id != 10));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_11.Enabled =           (cIsDefined && (id != 11));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_CopyTo_12.Enabled =           (cIsDefined && (id != 12));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_12.Visible = (cIsDefined && (np >= 13));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Visible =           (cIsDefined && (np >= 13));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_13.Enabled =           (cIsDefined && (id != 13));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Visible =           (cIsDefined && (np >= 14));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_14.Enabled =           (cIsDefined && (id != 14));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Visible =           (cIsDefined && (np >= 15));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_15.Enabled =           (cIsDefined && (id != 15));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Visible =           (cIsDefined && (np >= 16));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_16.Enabled =           (cIsDefined && (id != 16));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Visible =           (cIsDefined && (np >= 17));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_17.Enabled =           (cIsDefined && (id != 17));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Visible =           (cIsDefined && (np >= 18));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_18.Enabled =           (cIsDefined && (id != 18));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Visible =           (cIsDefined && (np >= 19));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_19.Enabled =           (cIsDefined && (id != 19));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Visible =           (cIsDefined && (np >= 20));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_20.Enabled =           (cIsDefined && (id != 20));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Visible =           (cIsDefined && (np >= 21));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_21.Enabled =           (cIsDefined && (id != 21));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Visible =           (cIsDefined && (np >= 22));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_22.Enabled =           (cIsDefined && (id != 22));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Visible =           (cIsDefined && (np >= 23));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_23.Enabled =           (cIsDefined && (id != 23));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Visible =           (cIsDefined && (np >= 24));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_24.Enabled =           (cIsDefined && (id != 24));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_24.Visible = (cIsDefined && (np >= 25));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Visible =           (cIsDefined && (np >= 25));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_25.Enabled =           (cIsDefined && (id != 25));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_36.Visible = (cIsDefined && (np >= 37));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Visible =           (cIsDefined && (np >= 37));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_37.Enabled =           (cIsDefined && (id != 37));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_48.Visible = (cIsDefined && (np >= 49));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Visible =           (cIsDefined && (np >= 49));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_49.Enabled =           (cIsDefined && (id != 49));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_60.Visible = (cIsDefined && (np >= 61));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Visible =           (cIsDefined && (np >= 61));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_61.Enabled =           (cIsDefined && (id != 61));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_72.Visible = (cIsDefined && (np >= 73));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Visible =           (cIsDefined && (np >= 73));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_73.Enabled =           (cIsDefined && (id != 73));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_84.Visible = (cIsDefined && (np >= 85));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Visible =           (cIsDefined && (np >= 85));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_85.Enabled =           (cIsDefined && (id != 85));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_Separator_96.Visible = (cIsDefined && (np >= 97));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Visible =           (cIsDefined && (np >= 97));
				toolStripMenuItem_PredefinedContextMenu_CopyTo_97.Enabled =           (cIsDefined && (id != 97));

				toolStripMenuItem_PredefinedContextMenu_MoveTo_1 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_1 .Enabled =           (cIsDefined && (id != 1));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_2 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_2 .Enabled =           (cIsDefined && (id != 2));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_3 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_3 .Enabled =           (cIsDefined && (id != 3));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_4 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_4 .Enabled =           (cIsDefined && (id != 4));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_5 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_5 .Enabled =           (cIsDefined && (id != 5));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_6 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_6 .Enabled =           (cIsDefined && (id != 6));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_7 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_7 .Enabled =           (cIsDefined && (id != 7));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_8 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_8 .Enabled =           (cIsDefined && (id != 8));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_9 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_9 .Enabled =           (cIsDefined && (id != 9));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_10.Enabled =           (cIsDefined && (id != 10));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_11.Enabled =           (cIsDefined && (id != 11));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_MoveTo_12.Enabled =           (cIsDefined && (id != 12));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_12.Visible = (cIsDefined && (np >= 13));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Visible =           (cIsDefined && (np >= 13));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_13.Enabled =           (cIsDefined && (id != 13));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Visible =           (cIsDefined && (np >= 14));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_14.Enabled =           (cIsDefined && (id != 14));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Visible =           (cIsDefined && (np >= 15));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_15.Enabled =           (cIsDefined && (id != 15));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Visible =           (cIsDefined && (np >= 16));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_16.Enabled =           (cIsDefined && (id != 16));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Visible =           (cIsDefined && (np >= 17));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_17.Enabled =           (cIsDefined && (id != 17));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Visible =           (cIsDefined && (np >= 18));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_18.Enabled =           (cIsDefined && (id != 18));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Visible =           (cIsDefined && (np >= 19));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_19.Enabled =           (cIsDefined && (id != 19));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Visible =           (cIsDefined && (np >= 20));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_20.Enabled =           (cIsDefined && (id != 20));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Visible =           (cIsDefined && (np >= 21));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_21.Enabled =           (cIsDefined && (id != 21));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Visible =           (cIsDefined && (np >= 22));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_22.Enabled =           (cIsDefined && (id != 22));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Visible =           (cIsDefined && (np >= 23));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_23.Enabled =           (cIsDefined && (id != 23));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Visible =           (cIsDefined && (np >= 24));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_24.Enabled =           (cIsDefined && (id != 24));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_24.Visible = (cIsDefined && (np >= 25));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Visible =           (cIsDefined && (np >= 25));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_25.Enabled =           (cIsDefined && (id != 25));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_36.Visible = (cIsDefined && (np >= 37));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Visible =           (cIsDefined && (np >= 37));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_37.Enabled =           (cIsDefined && (id != 37));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_48.Visible = (cIsDefined && (np >= 49));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Visible =           (cIsDefined && (np >= 49));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_49.Enabled =           (cIsDefined && (id != 49));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_60.Visible = (cIsDefined && (np >= 61));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Visible =           (cIsDefined && (np >= 61));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_61.Enabled =           (cIsDefined && (id != 61));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_72.Visible = (cIsDefined && (np >= 73));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Visible =           (cIsDefined && (np >= 73));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_73.Enabled =           (cIsDefined && (id != 73));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_84.Visible = (cIsDefined && (np >= 85));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Visible =           (cIsDefined && (np >= 85));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_85.Enabled =           (cIsDefined && (id != 85));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_Separator_96.Visible = (cIsDefined && (np >= 96));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Visible =           (cIsDefined && (np >= 96));
				toolStripMenuItem_PredefinedContextMenu_MoveTo_97.Enabled =           (cIsDefined && (id != 96));

				toolStripMenuItem_PredefinedContextMenu_UpBy  .Enabled = cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy.Enabled = cIsDefined;

				toolStripMenuItem_PredefinedContextMenu_UpBy_1 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_2 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_3 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_4 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_5 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_6 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_7 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_8 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_9 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_10.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_11.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_12.Visible = (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_12.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_13.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_14.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_15.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_16.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_17.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_18.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_19.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_20.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_21.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_22.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_23.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_24.Visible = (cIsDefined && (np > 24));
				toolStripMenuItem_PredefinedContextMenu_UpBy_24.Visible =           (cIsDefined && (np > 24));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_36.Visible = (cIsDefined && (np > 36));
				toolStripMenuItem_PredefinedContextMenu_UpBy_36.Visible =           (cIsDefined && (np > 36));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_48.Visible = (cIsDefined && (np > 48));
				toolStripMenuItem_PredefinedContextMenu_UpBy_48.Visible =           (cIsDefined && (np > 48));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_60.Visible = (cIsDefined && (np > 60));
				toolStripMenuItem_PredefinedContextMenu_UpBy_60.Visible =           (cIsDefined && (np > 60));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_72.Visible = (cIsDefined && (np > 72));
				toolStripMenuItem_PredefinedContextMenu_UpBy_72.Visible =           (cIsDefined && (np > 72));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_84.Visible = (cIsDefined && (np > 84));
				toolStripMenuItem_PredefinedContextMenu_UpBy_84.Visible =           (cIsDefined && (np > 84));
				toolStripMenuItem_PredefinedContextMenu_UpBy_Separator_96.Visible = (cIsDefined && (np > 96));
				toolStripMenuItem_PredefinedContextMenu_UpBy_96.Visible =           (cIsDefined && (np > 96));

				toolStripMenuItem_PredefinedContextMenu_DownBy_1 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_2 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_3 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_4 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_5 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_6 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_7 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_8 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_9 .Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_10.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_11.Visible =            cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_12.Visible = (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_12.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_13.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_14.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_15.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_16.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_17.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_18.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_19.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_20.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_21.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_22.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_23.Visible =           (cIsDefined && (np > 12));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_24.Visible = (cIsDefined && (np > 24));
				toolStripMenuItem_PredefinedContextMenu_DownBy_24.Visible =           (cIsDefined && (np > 24));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_36.Visible = (cIsDefined && (np > 36));
				toolStripMenuItem_PredefinedContextMenu_DownBy_36.Visible =           (cIsDefined && (np > 36));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_48.Visible = (cIsDefined && (np > 48));
				toolStripMenuItem_PredefinedContextMenu_DownBy_48.Visible =           (cIsDefined && (np > 48));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_60.Visible = (cIsDefined && (np > 60));
				toolStripMenuItem_PredefinedContextMenu_DownBy_60.Visible =           (cIsDefined && (np > 60));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_72.Visible = (cIsDefined && (np > 72));
				toolStripMenuItem_PredefinedContextMenu_DownBy_72.Visible =           (cIsDefined && (np > 72));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_84.Visible = (cIsDefined && (np > 84));
				toolStripMenuItem_PredefinedContextMenu_DownBy_84.Visible =           (cIsDefined && (np > 84));
				toolStripMenuItem_PredefinedContextMenu_DownBy_Separator_96.Visible = (cIsDefined && (np > 96));
				toolStripMenuItem_PredefinedContextMenu_DownBy_96.Visible =           (cIsDefined && (np > 96));

				toolStripMenuItem_PredefinedContextMenu_Cut  .Enabled = cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_Copy .Enabled = cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_Paste.Enabled = ((id != 0) && (CommandSettingsClipboardHelper.ClipboardContainsText));
				toolStripMenuItem_PredefinedContextMenu_Clear.Enabled = cIsDefined;

				var hasPages = (this.settingsRoot.PredefinedCommand.Pages.Count >= 1);
				toolStripMenuItem_PredefinedContextMenu_CopyToClipboard.Enabled = hasPages;
				toolStripMenuItem_PredefinedContextMenu_ExportToFile     .Enabled = hasPages;

				if (this.settingsRoot.PredefinedCommand.Pages.Count <= 1)
				{
					toolStripMenuItem_PredefinedContextMenu_CopyToClipboard    .Text = "Copy Page to Clipboard...";
				////toolStripMenuItem_PredefinedContextMenu_ImportFromClipboard.Text = "Paste Page(s) from Clipboard..." is fixed.
					toolStripMenuItem_PredefinedContextMenu_ExportToFile       .Text = "Export Page to File...";
				////toolStripMenuItem_PredefinedContextMenu_ImportFromFile     .Text = "Import Page(s) from File..." is fixed.
				////toolStripMenuItem_PredefinedContextMenu_LinkToFile         .Text = "Link Page to File..." is fixed.
				}
				else
				{
					toolStripMenuItem_PredefinedContextMenu_CopyToClipboard    .Text = "Copy Page(s) to Clipboard...";
				////toolStripMenuItem_PredefinedContextMenu_ImportFromClipboard.Text = "Paste Page(s) from Clipboard..." is fixed.
					toolStripMenuItem_PredefinedContextMenu_ExportToFile       .Text = "Export Page(s) to File...";
				////toolStripMenuItem_PredefinedContextMenu_ImportFromFile     .Text = "Import Page(s) from File..." is fixed.
				////toolStripMenuItem_PredefinedContextMenu_LinkToFile         .Text = "Link Page to File..." is fixed.
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_Panels_Predefined_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.PredefinedPanelIsVisible = !this.settingsRoot.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripComboBox_PredefinedContextMenu_Layout_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			SetPageLayout((PredefinedCommandPageLayoutEx)toolStripComboBox_PredefinedContextMenu_Layout.SelectedItem);
		}

		// While the purpose of
		// ...toolStripComboBox_PredefinedContextMenu_Command...
		// ...is questionable in the 'Predefined' context menu, it is there as kind of title for the items below.

		private void toolStripMenuItem_PredefinedContextMenu_CopyFromSendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var c = this.settingsRoot.SendText.Command;
			if (c != null)
			{
				c = new Command(c); // Clone to ensure decoupling.
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, contextMenuStrip_Predefined_SelectedCommandId - 1, c);
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var c = this.settingsRoot.SendFile.Command;
			if (c != null)
			{
				c = new Command(c); // Clone to ensure decoupling.
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, contextMenuStrip_Predefined_SelectedCommandId - 1, c);
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			var sc = predefined.GetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId);
			if (sc != null)
			{
				sc = new Command(sc); // Clone to ensure decoupling.
				if (sc.IsText)
					this.settingsRoot.SendText.Command = sc;
				else if (sc.IsFilePath)
					this.settingsRoot.SendFile.Command = sc;
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + @"Invalid command """ + sc.ToDiagnosticsString() + @"""!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_CopyTo_I_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.toolStripMenuItem_CommandContextMenu_CopyTo_I_Click()
			// Changes here may have to be applied there too.

			var targetCommandIndex = (ToolStripMenuItemEx.TagToInt32(sender) - 1); // Attention, 'ToolStripMenuItem' is no 'Control'!

			var sc = predefined.GetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId);
			if (sc != null)
			{
				sc = new Command(sc); // Clone to ensure decoupling.              // Replace target by selected:
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, targetCommandIndex, sc);
			}
			else
			{                                                                         // Clear target:
				this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, targetCommandIndex);
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.toolStripMenuItem_PredefinedContextMenu_MoveTo_I_Click()
			// Changes here may have to be applied there too.

			var targetCommandIndex = (ToolStripMenuItemEx.TagToInt32(sender) - 1); // Attention, 'ToolStripMenuItem' is no 'Control'!

			var sc = predefined.GetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId);
			if (sc != null)
			{
				this.settingsRoot.PredefinedCommand.SuspendChangeEvent();
				try
				{
					sc = new Command(sc); // Clone to ensure decoupling.              // Replace target by selected:
					this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, targetCommandIndex, sc); // Clear selected:
					this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, (contextMenuStrip_Predefined_SelectedCommandId - 1));
				}
				finally
				{
					this.settingsRoot.PredefinedCommand.ResumeChangeEvent();
				}
			}
			else
			{                                                                         // Clear target:
				this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, targetCommandIndex);
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_UpBy_N_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.toolStripMenuItem_CommandContextMenu_UpBy_N_Click()
			// Changes here may have to be applied there too.

			this.settingsRoot.PredefinedCommand.SuspendChangeEvent();
			try
			{
				int lastCommandIdPerPage = ((PredefinedCommandPageLayoutEx)this.settingsRoot.PredefinedCommand.PageLayout).CommandCapacityPerPage;
				int selectedCommandId = contextMenuStrip_Predefined_SelectedCommandId;
				int n = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
				for (int i = 0; i < n; i++)
				{
					Up(selectedCommandId, lastCommandIdPerPage);

					selectedCommandId--;
					if (selectedCommandId < PredefinedCommandPage.FirstCommandIdPerPage)
						selectedCommandId =                       lastCommandIdPerPage;
				}
			}
			finally
			{
				this.settingsRoot.PredefinedCommand.ResumeChangeEvent();
			}
		}

		private void Up(int selectedCommandId, int lastCommandIdPerPage)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.Up()
			// Changes here may have to be applied there too.

			var sc = predefined.GetCommandFromId(selectedCommandId);
			if (sc != null)
				sc = new Command(sc); // Clone to ensure decoupling.

			var targetCommandId = ((selectedCommandId > PredefinedCommandPage.FirstCommandIdPerPage) ? (selectedCommandId - 1) : (lastCommandIdPerPage));
			var tc = predefined.GetCommandFromId(targetCommandId);
			if (tc != null)
				tc = new Command(tc); // Clone to ensure decoupling.

			if (tc != null)                                                       // Replace selected by target:
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, selectedCommandId - 1, tc);
			else                                                                      // Clear selected:
				this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, selectedCommandId - 1);

			if (sc != null)                                                       // Replace target by selected:
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, targetCommandId - 1, sc);
			else                                                                      // Clear target:
				this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, targetCommandId - 1);
		}

		private void toolStripMenuItem_PredefinedContextMenu_DownBy_N_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.toolStripMenuItem_CommandContextMenu_DownBy_N_Click()
			// Changes here may have to be applied there too.

			this.settingsRoot.PredefinedCommand.SuspendChangeEvent();
			try
			{
				int lastCommandIdPerPage = ((PredefinedCommandPageLayoutEx)this.settingsRoot.PredefinedCommand.PageLayout).CommandCapacityPerPage;
				int selectedCommandId = contextMenuStrip_Predefined_SelectedCommandId;
				int n = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
				for (int i = 0; i < n; i++)
				{
					Down(selectedCommandId, lastCommandIdPerPage);

					selectedCommandId++;
					if (selectedCommandId >                       lastCommandIdPerPage)
						selectedCommandId = PredefinedCommandPage.FirstCommandIdPerPage;
				}
			}
			finally
			{
				this.settingsRoot.PredefinedCommand.ResumeChangeEvent();
			}
		}

		private void Down(int selectedCommandId, int lastCommandIdPerPage)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.Down()
			// Changes here may have to be applied there too.

			var sc = predefined.GetCommandFromId(selectedCommandId);
			if (sc != null)
				sc = new Command(sc); // Clone to ensure decoupling.

			var targetCommandId = ((selectedCommandId < lastCommandIdPerPage) ? (selectedCommandId + 1) : (PredefinedCommandPage.FirstCommandIdPerPage));
			var tc = predefined.GetCommandFromId(targetCommandId);
			if (tc != null)
				tc = new Command(tc); // Clone to ensure decoupling.

			if (tc != null)                                                       // Replace selected by target:
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, selectedCommandId - 1, tc);
			else                                                                      // Clear selected:
				this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, selectedCommandId - 1);

			if (sc != null)                                                       // Replace target by selected:
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, targetCommandId - 1, sc);
			else                                                                      // Clear target:
				this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, targetCommandId - 1);
		}

		private void toolStripMenuItem_PredefinedContextMenu_Cut_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.toolStripMenuItem_PredefinedContextMenu_Cut_Click()
			// Changes here may have to be applied there too.

			var sc = predefined.GetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId);
			if (sc != null)
			{
				SetFixedStatusText("Preparing cutting to clipboard...");
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case cutting takes long.
				SetFixedStatusText("Cutting to clipboard...");
				if (CommandSettingsClipboardHelper.TrySet(this, sc))
				{
					this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, (contextMenuStrip_Predefined_SelectedCommandId - 1));

					Cursor = Cursors.Default;
					SetTimedStatusText("Cutting to clipboard done");
				}
				else
				{
					Cursor = Cursors.Default;
					SetFixedStatusText("Cutting to clipboard failed!");
				}
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_Copy_Click(object sender, EventArgs e)
		{
			// Attention:
			// Similar code exists in...
			// ...View.Forms.PredefinedCommandSettings.toolStripMenuItem_PredefinedContextMenu_Copy_Click()
			// Changes here may have to be applied there too.

			var sc = predefined.GetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId);
			if (sc != null)
			{
				SetFixedStatusText("Preparing copying to clipboard...");
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case copying takes long.
				SetFixedStatusText("Copying to clipboard...");
				if (CommandSettingsClipboardHelper.TrySet(this, sc))
				{
					Cursor = Cursors.Default;
					SetTimedStatusText("Copying to clipboard done");
				}
				else
				{
					Cursor = Cursors.Default;
					SetFixedStatusText("Copying to clipboard failed!");
				}
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_Paste_Click(object sender, EventArgs e)
		{
			Command cc;
			SetFixedStatusText("Pasting from clipboard..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			if (CommandSettingsClipboardHelper.TryGet(this, out cc))
			{
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, contextMenuStrip_Predefined_SelectedCommandId - 1, cc);
				SetTimedStatusText("Pasting from clipboard done");
			}
			else
			{
				SetFixedStatusText("Pasting from clipboard failed!");
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_Clear_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.PredefinedCommand.ClearCommand(predefined.SelectedPageIndex, contextMenuStrip_Predefined_SelectedCommandId - 1);
		}

		private void toolStripMenuItem_PredefinedContextMenu_Define_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			if (contextMenuStrip_Predefined_SelectedCommandId != 0)
				ShowPredefinedCommandSettings(predefined.SelectedPageId, contextMenuStrip_Predefined_SelectedCommandId);
			else
				ShowPredefinedCommandSettings(predefined.SelectedPageId, 1);
		}

		// While the purpose of
		// ...toolStripComboBox_PredefinedContextMenu_Page...
		// ...is questionable in the 'Predefined' context menu, it is there as kind of title for the items below.

		private void toolStripMenuItem_PredefinedContextMenu_CopyToClipboard_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Copying to clipboard..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			if (CommandPagesSettingsClipboardHelper.TrySet(this, this.settingsRoot.PredefinedCommand, predefined.SelectedPageId))
				SetTimedStatusText("Copying to clipboard done");
			else
				SetFixedStatusText("Copying to clipboard failed!");
		}

		private void toolStripMenuItem_PredefinedContextMenu_PasteFromClipboard_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Pasting from clipboard..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			Model.Settings.PredefinedCommandSettings predefinedCommandNew;
			if (CommandPagesSettingsClipboardHelper.TryGetAndImport(this, this.settingsRoot.PredefinedCommand, out predefinedCommandNew))
			{
				this.settingsRoot.PredefinedCommand = predefinedCommandNew;
				//// settingsRoot_Changed() will update the form.
				SetTimedStatusText("Pasting from clipboard done");
			}
			else
			{
				SetFixedStatusText("Pasting from clipboard failed!");
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_ExportToFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Exporting to file..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			if (CommandPagesSettingsFileHelper.TryExport(this, this.settingsRoot.PredefinedCommand, predefined.SelectedPageId, IndicatedName))
				SetTimedStatusText("Exporting to file done");
			else
				SetFixedStatusText("Exporting to file failed!");
		}

		private void toolStripMenuItem_PredefinedContextMenu_ImportFromFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Importing from file..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			Model.Settings.PredefinedCommandSettings predefinedCommandNew;
			if (CommandPagesSettingsFileHelper.TryLoadAndImport(this, this.settingsRoot.PredefinedCommand, out predefinedCommandNew))
			{
				this.settingsRoot.PredefinedCommand = predefinedCommandNew;
				//// settingsRoot_Changed() will update the form.
				SetTimedStatusText("Importing from file done");
			}
			else
			{
				SetFixedStatusText("Importing from file failed!"); // Opposed to clipboard above, TryLoadAndImport() already outputs a detailed error message.
			}
		}

		private void toolStripMenuItem_PredefinedContextMenu_LinkToFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Linking to file..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			Model.Settings.PredefinedCommandSettings predefinedCommandNew;
			if (CommandPagesSettingsFileLinkHelper.TryLoadAndLink(this, this.settingsRoot.PredefinedCommand, predefined.SelectedPageId, out predefinedCommandNew))
			{
				this.settingsRoot.PredefinedCommand = predefinedCommandNew;
				//// settingsRoot_Changed() will update the form.
				SetTimedStatusText("Linking to file done");
			}
			else
			{
				SetFixedStatusText("Linking to file failed!");
			}
		}

		#endregion

		#region Controls Event Handlers > Command Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Command Context Menu
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<ToolStripMenuItem> menuItems_Commands;

		private void contextMenuStrip_Command_Initialize()
		{
			this.menuItems_Commands = new List<ToolStripMenuItem>(PredefinedCommandPage.CommandCapacityWithShortcut); // Preset the required capacity to improve memory management.
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_1);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_2);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_3);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_4);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_5);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_6);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_7);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_8);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_9);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_10);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_11);
			this.menuItems_Commands.Add(toolStripMenuItem_CommandContextMenu_12);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		/// <remarks>
		/// In cases where only <see cref="Control.Visible"/> and/or <see cref="Control.Enabled"/>
		/// need to be updated, this method shall be called with <paramref name="updateAppearance"/>
		/// set to <c>false</c>, resulting in significantly improved performance.
		/// </remarks>
		private void contextMenuStrip_Command_SetMenuItems(bool updateAppearance = true)
		{
			this.isSettingControls.Enter();
			try
			{
				// Attention:
				// Similar code exists in...
				// ...View.Controls.PredefinedCommandButtonSet.SetCommandTextControls()
				// ...View.Controls.PredefinedCommandButtonSet.SetCommandStateControls()
				// ...View.Controls.PredefinedCommandButtonSet.CommandRequest()
				// ...View.Forms.PredefinedCommandSettings.SetPageControls()
				// Changes here may have to be applied there too.

				var pages = this.settingsRoot.PredefinedCommand.Pages;

				int pageCount = 0;
				if (pages != null)
					pageCount = pages.Count;

				List<Command> commands = null;
				if (pageCount > 0)
					commands = this.settingsRoot.PredefinedCommand.Pages[predefined.SelectedPageIndex].Commands;

				int commandCount = 0;
				if (commands != null)
					commandCount = commands.Count;

				for (int i = 0; i < PredefinedCommandPage.CommandCapacityWithShortcut; i++)
				{
					bool isDefined =
					(
						(i < commandCount) &&
						(commands[i] != null) &&
						(commands[i].IsDefined)
					);

					if (isDefined)
					{
						if (updateAppearance)
						{
							if (this.menuItems_Commands[i].ForeColor != SystemColors.ControlText) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].ForeColor = SystemColors.ControlText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
							                                                   //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
							if (this.menuItems_Commands[i].Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].Font = SystemFonts.DefaultFont;  // Improves because 'Font' is managed by a 'PropertyStore'.

							this.menuItems_Commands[i].Text = MenuEx.PrependIndex(i + 1, commands[i].Description);
						}

						bool isValid = (this.terminal.IsReadyToSend && commands[i].IsValid(this.settingsRoot.Send.Text.ToParseMode(), this.terminal.SettingsFilePath));
						this.menuItems_Commands[i].Enabled = isValid;
					}
					else
					{
						if (updateAppearance)
						{
							if (this.menuItems_Commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].ForeColor = SystemColors.GrayText;  // Improves because 'ForeColor' is managed by a 'PropertyStore'.
							                                                 //// Time consuming operation! See 'DrawingEx.DefaultFontItalic' for background!
							if (this.menuItems_Commands[i].Font != DrawingEx.DefaultFontItalic) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].Font = DrawingEx.DefaultFontItalic;  // Improves because 'Font' is managed by a 'PropertyStore'.

							this.menuItems_Commands[i].Text = MenuEx.PrependIndex(i + 1, Command.DefineCommandText);
						}

						this.menuItems_Commands[i].Enabled = true;
					}
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void contextMenuStrip_Command_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Command_SetMenuItems();
		}

		private void toolStripMenuItem_CommandContextMenu_I_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SendPredefined(ToolStripMenuItemEx.TagToInt32(sender)); // Attention, 'ToolStripMenuItem' is no 'Control'!
		}

		#endregion

		#region Controls Event Handlers > Page Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Page Context Menu
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<ToolStripMenuItem> menuItems_Pages;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "'MaxPages' indeed starts with an upper case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private const int menuItems_Page_MaxPagesWithMenuItem = 9;

		private void contextMenuStrip_Page_Initialize()
		{
			this.menuItems_Pages = new List<ToolStripMenuItem>(menuItems_Page_MaxPagesWithMenuItem); // Preset the required capacity to improve memory management.
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_1);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_2);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_3);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_4);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_5);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_6);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_7);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_8);
			this.menuItems_Pages.Add(toolStripMenuItem_PageContextMenu_9);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		/// <remarks>
		/// In cases where only <see cref="Control.Visible"/> and/or <see cref="Control.Enabled"/>
		/// need to be updated, this method shall be called with <paramref name="updateAppearance"/>
		/// set to <c>false</c>, resulting in significantly improved performance.
		/// </remarks>
		private void contextMenuStrip_Page_SetMenuItems(bool updateAppearance = true)
		{
			this.isSettingControls.Enter();
			try
			{
				var pages = this.settingsRoot.PredefinedCommand.Pages;

				int pageCount = 0;
				if (pages != null)
					pageCount = pages.Count;

				if (pageCount > 0)
				{
					toolStripMenuItem_PageContextMenu_Previous .Visible = true;
					toolStripMenuItem_PageContextMenu_Previous .Enabled = (predefined.SelectedPageId > 1);
					toolStripMenuItem_PageContextMenu_Next     .Visible = true;
					toolStripMenuItem_PageContextMenu_Next     .Enabled = (predefined.SelectedPageId < pageCount);
					toolStripMenuItem_PageContextMenu_Separator.Visible = true;
				}
				else
				{
					toolStripMenuItem_PageContextMenu_Previous .Visible = false;
					toolStripMenuItem_PageContextMenu_Previous .Enabled = false;
					toolStripMenuItem_PageContextMenu_Next     .Visible = false;
					toolStripMenuItem_PageContextMenu_Next     .Enabled = false;
					toolStripMenuItem_PageContextMenu_Separator.Visible = false;
				}

				for (int i = 0; i < Math.Min(pageCount, menuItems_Page_MaxPagesWithMenuItem); i++)
				{
					if (updateAppearance)
						this.menuItems_Pages[i].Text = MenuEx.PrependIndex(i + 1, PredefinedCommandPage.CaptionOrFallback(pages[i], (i + 1)));

					this.menuItems_Pages[i].Visible = true;
					this.menuItems_Pages[i].Enabled = (pageCount > 1); // No need to navigate a single page.
				}

				for (int i = pageCount; i < menuItems_Page_MaxPagesWithMenuItem; i++)
				{
					if (updateAppearance)
						this.menuItems_Pages[i].Text = MenuEx.PrependIndex(i + 1, "<Undefined>");

					this.menuItems_Pages[i].Visible = false;
					this.menuItems_Pages[i].Enabled = false;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void contextMenuStrip_Page_Opening(object sender, CancelEventArgs e)
		{
			contextMenuStrip_Page_SetMenuItems();
		}

		private void toolStripMenuItem_PageContextMenu_Next_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			predefined.NextPage();
		}

		private void toolStripMenuItem_PageContextMenu_Previous_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			predefined.PreviousPage();
		}

		private void toolStripMenuItem_PageContextMenu_I_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			predefined.SelectedPageId = ToolStripMenuItemEx.TagToInt32(sender); // Attention, 'ToolStripMenuItem' is no 'Control'!
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
				bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText(this.settingsRoot.Send.Text.ToParseMode());
				if (this.settingsRoot.Send.Text.SendImmediately)
				{
					if (isTextTerminal)
						sendTextText = "Send EOL";
					else
						sendTextEnabled = false;
				}

				bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath(Path.GetDirectoryName(this.terminal.SettingsFilePath));

				// Set the menu item properties:

				toolStripMenuItem_SendContextMenu_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
				toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

				toolStripMenuItem_SendContextMenu_SendText.Text              =  sendTextText;
				toolStripMenuItem_SendContextMenu_SendText.Enabled           = (sendTextEnabled && this.terminal.IsReadyToSend);
				toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Enabled = (sendTextEnabled && this.terminal.IsReadyToSend && !this.settingsRoot.SendText.Command.IsMultiLineText && !this.settingsRoot.Send.Text.SendImmediately);
				toolStripMenuItem_SendContextMenu_SendFile.Enabled           = (sendFileEnabled && this.terminal.IsReadyToSend);

				toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Checked = this.settingsRoot.Send.UseExplicitDefaultRadix;
				toolStripMenuItem_SendContextMenu_AllowConcurrency.Checked        = this.settingsRoot.Send.AllowConcurrency;

				toolStripMenuItem_SendContextMenu_KeepSendText.Enabled         =  !this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_SendContextMenu_KeepSendText.Checked         = (!this.settingsRoot.Send.Text.SendImmediately && this.settingsRoot.Send.Text.KeepSendText);
				toolStripMenuItem_SendContextMenu_SendImmediately.Checked      =   this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_SendContextMenu_EnableEscapesForText.Checked =   this.settingsRoot.Send.Text.EnableEscapes;

				toolStripMenuItem_SendContextMenu_SkipEmptyLines.Checked       = this.settingsRoot.Send.File.SkipEmptyLines;
				toolStripMenuItem_SendContextMenu_EnableEscapesForFile.Checked = this.settingsRoot.Send.File.EnableEscapes;

				toolStripMenuItem_SendContextMenu_CopyPredefined.Checked =  this.settingsRoot.Send.CopyPredefined;

				toolStripMenuItem_SendContextMenu_ExpandMultiLineText.Enabled =  this.settingsRoot.SendText.Command.IsMultiLineText;
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

			if (!this.settingsRoot.Send.Text.SendImmediately)
				this.terminal.SendText();
			else
				this.terminal.SendPartialTextEol();
		}

		private void toolStripMenuItem_SendContextMenu_SendTextWithoutEol_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			if (!this.settingsRoot.Send.Text.SendImmediately)
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

		private void toolStripMenuItem_SendContextMenu_AllowConcurrency_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.AllowConcurrency = !this.settingsRoot.Send.AllowConcurrency;
		}

		private void toolStripMenuItem_SendContextMenu_KeepSendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.Text.KeepSendText = !this.settingsRoot.Send.Text.KeepSendText;
		}

		private void toolStripMenuItem_SendContextMenu_SendImmediately_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.Text.SendImmediately = !this.settingsRoot.Send.Text.SendImmediately;
		}

		private void toolStripMenuItem_SendContextMenu_EnableEscapesForText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.Text.EnableEscapes = !this.settingsRoot.Send.Text.EnableEscapes;
		}

		private void toolStripMenuItem_SendContextMenu_EnableEscapesForFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.File.EnableEscapes = !this.settingsRoot.Send.File.EnableEscapes;
		}

		private void toolStripMenuItem_SendContextMenu_SkipEmptyLines_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.File.SkipEmptyLines = !this.settingsRoot.Send.File.SkipEmptyLines;
		}

		private void toolStripMenuItem_SendContextMenu_CopyPredefined_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Send.CopyPredefined = !this.settingsRoot.Send.CopyPredefined;
		}

		private void toolStripMenuItem_SendContextMenu_ExpandMultiLineText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.SendText.ExpandMultiLineText();
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
				bool isSerialPort = true;
				if (this.settingsRoot != null)
					isSerialPort = (this.settingsRoot.IOType == Domain.IOType.SerialPort);

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
				this.settingsRoot.Implicit.Predefined.SelectedPageId = predefined.SelectedPageId;
		}

		private void predefined_SendCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			if (this.terminal != null)
				this.terminal.SendPredefined(e.PageId, e.CommandId);
		}

		private void predefined_DefineCommandRequest(object sender, PredefinedCommandEventArgs e)
		{
			ShowPredefinedCommandSettings(e.PageId, e.CommandId);
		}

		#endregion

		#region Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Send
		//------------------------------------------------------------------------------------------

		private void send_TextCommandChanged(object sender, EventArgs e)
		{
			if (this.settingsRoot != null)
				this.settingsRoot.Implicit.SendText.Command = new Command(send.TextCommand); // Clone to ensure decoupling.
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
				this.settingsRoot.Implicit.SendFile.Command = new Command(send.FileCommand); // Clone to ensure decoupling.
		}

		private void send_SendFileCommandRequest(object sender, EventArgs e)
		{
			if (this.terminal != null)
				this.terminal.SendFile();
		}

		private void send_SizeChanged(object sender, EventArgs e)
		{
			AdjustSendSplitter();
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
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_RTS);
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
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_RTS,           toolStripStatusLabel_TerminalStatus_RTS.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_CTS,           toolStripStatusLabel_TerminalStatus_CTS.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DTR,           toolStripStatusLabel_TerminalStatus_DTR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DSR,           toolStripStatusLabel_TerminalStatus_DSR.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_DCD,           toolStripStatusLabel_TerminalStatus_DCD.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_InputXOnXOff,  toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_OutputXOnXOff, toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_InputBreak,    toolStripStatusLabel_TerminalStatus_InputBreak.Text);
			this.terminalStatusLabels_DefaultText.Add(toolStripStatusLabel_TerminalStatus_OutputBreak,   toolStripStatusLabel_TerminalStatus_OutputBreak.Text);

			this.terminalStatusLabels_DefaultToolTipText = new Dictionary<ToolStripStatusLabel, string>(9); // Preset the required capacity to improve memory management.
			this.terminalStatusLabels_DefaultToolTipText.Add(toolStripStatusLabel_TerminalStatus_RTS,           toolStripStatusLabel_TerminalStatus_RTS.ToolTipText);
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
		private void toolStripStatusLabel_TerminalStatus_RTS_Click(object sender, EventArgs e)
		{
			if (this.terminal != null)
				this.terminal.RequestToggleRts();

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
		/// The indicated terminal name. The name is either an incrementally assigned 'Terminal1',
		/// 'Terminal2',... or the file name once the terminal has been saved by the user, e.g.
		/// 'MyTerminal.yat'.
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

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		public virtual void LeaveFindOnEdit(string pattern)
		{
			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			monitor.ResetFindOnEdit();
		}

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
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

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
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

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
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

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		private void ShowNotFoundMessage(string pattern, bool isFirst)
		{
			var text = new StringBuilder();
			text.Append("The specified ");

			if (ApplicationSettings.RoamingUserSettings.Find.Options.EnableRegex)
				text.Append("pattern");
			else
				text.Append("text");

			text.Append(@" """);
			text.Append(pattern);
			text.Append(@""" has not been found");

			if (!isFirst)
				text.Append(" anymore");

			text.Append(".");

			var caption = new StringBuilder();
			if (ApplicationSettings.RoamingUserSettings.Find.Options.EnableRegex)
				caption.Append("Pattern");
			else
				caption.Append("Text");

			caption.Append(" Not Found");

			MessageBoxEx.Show
			(
				this,
				text.ToString(),
				caption.ToString(),
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
		public virtual AutoContentState AutoActionTriggerState
		{
			get { return (this.autoActionTriggerState); }
			set
			{
				if (this.autoActionTriggerState != value)
				{
					this.autoActionTriggerState = value;
					SetAutoActionTriggerStateControls();
					OnAutoActionTriggerStateChanged();
				}
			}
		}

		private void SetAutoActionTriggerStateControls()
		{
			switch (this.autoActionTriggerState)
			{
				case AutoContentState.Neutral:
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.BackColor = SystemColors.Window;
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.ForeColor = SystemColors.WindowText;
					break;

				case AutoContentState.Invalid:
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.BackColor = SystemColors.ControlDark;
					toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.ForeColor = SystemColors.ControlText;
					break;

				default:
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.autoActionTriggerState + "' is an automatic content state that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>Could also be located in <see cref="Model.Terminal"/>.</remarks>
		public virtual bool RequestAutoActionValidateTriggerTextSilently(string triggerTextOrRegexPattern)
		{
			if (this.settingsRoot.AutoAction.IsByteSequenceTriggered)
			{
				return (ValidationHelper.ValidateTextSilently(triggerTextOrRegexPattern, Domain.Parser.Modes.RadixAndAsciiEscapes));
			}
			else // IsTextTriggered
			{
				if (!this.settingsRoot.AutoAction.TriggerOptions.EnableRegex) // 'CaseSensitive' and 'WholeWord' are irrelevant for validation.
					return (!string.IsNullOrEmpty(triggerTextOrRegexPattern));
				else                                          // EnableRegex
					return (RegexEx.TryValidatePattern(triggerTextOrRegexPattern));
			}
		}

		/// <summary></summary>
		public virtual void RequestAutoActionTrigger(AutoTriggerEx trigger)
		{
			if (trigger == AutoTrigger.AnyLine)
			{
				var text = new StringBuilder();

				switch ((AutoAction)(this.settingsRoot.AutoAction.Action))
				{
					case AutoAction.Highlight:
					{
						text.AppendLine("Trigger cannot be set to [Any Line] when action is [Highlight]!");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being highlighted.");
						break;
					}

					case AutoAction.Suppress:
					{
						text.AppendLine("Trigger cannot be set to [Any Line] when action is [Suppress]!");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being suppressed.");
						break;
					}

					default: // Accept change of trigger:
					{
						this.settingsRoot.AutoAction.Trigger = trigger;
						break;
					}
				}

				if (text.Length > 0)
				{
					MessageBoxEx.Show
					(
						this,
						text.ToString(),
						"Currently Invalid Trigger",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation
					);
				}
			}
			else // Accept change of trigger:
			{
				this.settingsRoot.AutoAction.Trigger = trigger;
			}
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionUseText()
		{
			var options = this.settingsRoot.AutoAction.TriggerOptions;
			options.UseText = !options.UseText; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoAction.TriggerOptions = options;

			RevalidateAutoActionTriggerTextSilently();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionCaseSensitive()
		{
			var options = this.settingsRoot.AutoAction.TriggerOptions;
			options.CaseSensitive = !options.CaseSensitive; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoAction.TriggerOptions = options;

			RevalidateAutoActionTriggerTextSilently();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionWholeWord()
		{
			var options = this.settingsRoot.AutoAction.TriggerOptions;
			options.WholeWord = !options.WholeWord; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoAction.TriggerOptions = options;

			RevalidateAutoActionTriggerTextSilently();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionEnableRegex()
		{
			var options = this.settingsRoot.AutoAction.TriggerOptions;
			options.EnableRegex = !options.EnableRegex; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoAction.TriggerOptions = options;

			RevalidateAutoActionTriggerTextSilently();
		}

		/// <summary></summary>
		protected virtual void RevalidateAutoActionTriggerTextSilently()
		{
			if (RequestAutoActionValidateTriggerTextSilently(this.settingsRoot.AutoAction.Trigger))
				AutoActionTriggerState = AutoContentState.Neutral;
			else
				AutoActionTriggerState = AutoContentState.Invalid;
		}

	////public virtual AutoContentState AutoActionTriggerState                               is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private void SetAutoActionTriggerState(AutoContentState state)                       is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////public virtual bool RequestAutoActionValidateTriggerTextSilently(string triggerText) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

		/// <summary></summary>
		public virtual void RequestAutoActionAction(AutoActionEx action)
		{
			if (this.settingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine)
			{
				var text = new StringBuilder();

				switch ((AutoAction)action)
				{
					case AutoAction.Highlight:
					{
						text.AppendLine("Action cannot be set to [Highlight] when trigger is [Any Line]!");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being highlighted.");
						break;
					}

					case AutoAction.Suppress:
					{
						text.AppendLine("Action cannot be set to [Suppress] when trigger is [Any Line]!");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being suppressed.");
						break;
					}

					default: // Accept change of action:
					{
						this.settingsRoot.AutoAction.Action = action;
						break;
					}
				}

				if (text.Length > 0)
				{
					MessageBoxEx.Show
					(
						this,
						text.ToString(),
						"Currently Invalid Action",
						MessageBoxButtons.OK,
						MessageBoxIcon.Exclamation
					);
				}
			}
			else // Accept change of action:
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
		public virtual AutoContentState AutoResponseTriggerState
		{
			get { return (this.autoResponseTriggerState); }
			set
			{
				if (this.autoResponseTriggerState != value)
				{
					this.autoResponseTriggerState = value;
					SetAutoResponseTriggerStateControls();
					OnAutoResponseTriggerStateChanged();
				}
			}
		}

		private void SetAutoResponseTriggerStateControls()
		{
			switch (this.autoResponseTriggerState)
			{
				case AutoContentState.Neutral:
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.BackColor = SystemColors.Window;
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.ForeColor = SystemColors.WindowText;
					break;

				case AutoContentState.Invalid:
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.BackColor = SystemColors.ControlDark;
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.ForeColor = SystemColors.ControlText;
					break;

				default:
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.autoResponseTriggerState + "' is an automatic content state that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <remarks>Could also be located in <see cref="Model.Terminal"/>.</remarks>
		public virtual bool RequestAutoResponseValidateTriggerTextSilently(string triggerTextOrRegexPattern)
		{
			if (this.settingsRoot.AutoResponse.IsByteSequenceTriggered)
			{
				return (ValidationHelper.ValidateTextSilently(triggerTextOrRegexPattern, Domain.Parser.Modes.RadixAndAsciiEscapes));
			}
			else // IsTextTriggered
			{
				if (!this.settingsRoot.AutoResponse.TriggerOptions.EnableRegex) // 'CaseSensitive' and 'WholeWord' are irrelevant for validation.
					return (!string.IsNullOrEmpty(triggerTextOrRegexPattern));
				else                                            // EnableRegex
					return (RegexEx.TryValidatePattern(triggerTextOrRegexPattern));
			}
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseTrigger(AutoTriggerEx trigger)
		{
			this.settingsRoot.AutoResponse.Trigger = trigger;
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerUseText()
		{
			var options = this.settingsRoot.AutoResponse.TriggerOptions;
			options.UseText = !options.UseText; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoResponse.TriggerOptions = options;

			RevalidateAutoResponseTriggerTextSilently();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerCaseSensitive()
		{
			var options = this.settingsRoot.AutoResponse.TriggerOptions;
			options.CaseSensitive = !options.CaseSensitive; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoResponse.TriggerOptions = options;

			RevalidateAutoResponseTriggerTextSilently();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerWholeWord()
		{
			var options = this.settingsRoot.AutoResponse.TriggerOptions;
			options.WholeWord = !options.WholeWord; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoResponse.TriggerOptions = options;

			RevalidateAutoResponseTriggerTextSilently();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerEnableRegex()
		{
			var options = this.settingsRoot.AutoResponse.TriggerOptions;
			options.EnableRegex = !options.EnableRegex; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoResponse.TriggerOptions = options;

			RevalidateAutoResponseTriggerTextSilently();
		}

		/// <summary></summary>
		protected virtual void RevalidateAutoResponseTriggerTextSilently()
		{
			if (RequestAutoResponseValidateTriggerTextSilently(this.settingsRoot.AutoResponse.Trigger))
				AutoResponseTriggerState = AutoContentState.Neutral;
			else
				AutoResponseTriggerState = AutoContentState.Invalid;
		}

		/// <summary></summary>
		public virtual AutoContentState AutoResponseResponseState
		{
			get { return (this.autoResponseResponseState); }
			set
			{
				if (this.autoResponseResponseState != value)
				{
					this.autoResponseResponseState = value;
					SetAutoResponseResponseStateControls();
					OnAutoResponseResponseStateChanged();
				}
			}
		}

		private void SetAutoResponseResponseStateControls()
		{
			switch (this.autoResponseResponseState)
			{
				case AutoContentState.Neutral:
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.BackColor = SystemColors.Window;
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.ForeColor = SystemColors.WindowText;
					break;

				case AutoContentState.Invalid:
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.BackColor = SystemColors.ControlDark;
					toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.ForeColor = SystemColors.ControlText;
					break;

				default:
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.autoResponseResponseState + "' is an automatic content state that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		public virtual bool RequestAutoResponseValidateResponseTextSilently(string responseText)
		{
			return (ValidationHelper.ValidateTextSilently(responseText, Domain.Parser.Modes.RadixAndAsciiEscapes));
		}

		/// <summary></summary>
		public virtual void RequestAutoResponseResponse(AutoResponseEx response)
		{
			this.settingsRoot.AutoResponse.Response = response;
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseResponseEnableReplace()
		{
			var options = this.settingsRoot.AutoResponse.ResponseOptions;
			options.EnableReplace = !options.EnableReplace; // Settings member must be changed to let the changed event be raised!
			this.settingsRoot.AutoResponse.ResponseOptions = options;

		////RevalidateAutoResponseResponseTextSilently() is not required as Regex.Replace() will accept any text.
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
			var strips = new List<ContextMenuStrip>(8); // Preset the required capacity to improve memory management.
			strips.Add(contextMenuStrip_Command);
			strips.Add(contextMenuStrip_Monitor);
			strips.Add(contextMenuStrip_Page);
			strips.Add(contextMenuStrip_Predefined);
			strips.Add(contextMenuStrip_Preset);
			strips.Add(contextMenuStrip_Radix);
			strips.Add(contextMenuStrip_Send);
			strips.Add(contextMenuStrip_Status);

			// Makes sure that context menus are at the right position upon first drop down. This is
			// a fix, it should be that way by default. However, due to some reasons, they sometimes
			// appear somewhere at the top-left corner of the screen if this fix isn't done.
			SuspendLayout();

			foreach (var strip in strips)
				strip.OwnerItem = null;

			ResumeLayout(false);

			// Also fix the issue with shortcuts defined in context menus:
			int itemCount = 0;
			foreach (var strip in strips)
				itemCount += strip.Items.Count;

			this.contextMenuStripShortcutTargetWorkaround = new ContextMenuStripShortcutTargetWorkaround(itemCount); // Preset the required capacity to improve memory management.
			foreach (var strip in strips)
				this.contextMenuStripShortcutTargetWorkaround.Add(strip);
		}

		private void InitializeControls()
		{
			toolStripMenuItem_TerminalMenu_View_Initialize();

			contextMenuStrip_Command_Initialize();
			contextMenuStrip_Monitor_Initialize();
			contextMenuStrip_Page_Initialize();
			contextMenuStrip_Predefined_Initialize();
			contextMenuStrip_Preset_Initialize();
		////contextMenuStrip_Radix_Initialize()  is not implemented/needed (yet).
		////contextMenuStrip_Send_Initialize()   is not implemented/needed (yet).
		////contextMenuStrip_Status_Initialize() is not implemented/needed (yet).

			toolStripStatusLabel_TerminalStatus_Initialize();
		}

		private void ApplyWindowSettings()
		{
			WindowState = this.settingsRoot.Window.State;
			if (WindowState == FormWindowState.Normal)
			{
				SuspendLayout(); // Useful as the 'Size' and 'Location' properties will get changed.
				try
				{
					StartPosition = FormStartPosition.Manual;
					Location      = this.settingsRoot.Window.Location;
					Size          = this.settingsRoot.Window.Size;
				}
				finally
				{
					ResumeLayout(false);
				}
			}
		}

		private void UpdateWindowSettings()
		{
			this.settingsRoot.Window.State = WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				this.settingsRoot.Window.Location = Location;
				this.settingsRoot.Window.Size     = Size;
			}
		}

		private void ViewRearrange()
		{
			// Simply set defaults, settings event handler will then call LayoutTerminal():
			this.settingsRoot.Layout.SetDefaults();
		}

		private void LayoutTerminal()
		{
			SuspendLayout(); // Useful as the 'Size' and 'Location' properties will get changed.
			this.isSettingControls.Enter();
			try
			{
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
						int limitedDistance;
						if (SplitContainerHelper.TryLimitSplitterDistance(splitContainer_Predefined, distance, out limitedDistance))
						{
							if (splitContainer_Predefined.SplitterDistance != limitedDistance)
								splitContainer_Predefined.SplitterDistance = limitedDistance;
						}
					#if DEBUG
						else
						{
							Debugger.Break(); // See debug output for issue and instructions!
						}
					#endif
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
							int limitedDistance;
							if (SplitContainerHelper.TryLimitSplitterDistance(splitContainer_TxMonitor, distance, out limitedDistance))
							{
								if (splitContainer_TxMonitor.SplitterDistance != limitedDistance)
									splitContainer_TxMonitor.SplitterDistance = limitedDistance;
							}
						#if DEBUG
							else
							{
								Debugger.Break(); // See debug output for issue and instructions!
							}
						#endif
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
							int limitedDistance;
							if (SplitContainerHelper.TryLimitSplitterDistance(splitContainer_RxMonitor, distance, out limitedDistance))
							{
								if (splitContainer_RxMonitor.SplitterDistance != limitedDistance)
									splitContainer_RxMonitor.SplitterDistance = limitedDistance;
							}
						#if DEBUG
							else
							{
								Debugger.Break(); // See debug output for issue and instructions!
							}
						#endif
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
							if (bidirIsVisible) // Bidir (center) is priority #1.
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
								if (bidirIsVisible) // Bidir (center) is priority #1.
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
								if (bidirIsVisible) // Bidir (center) is priority #1.
									this.lastMonitorSelection = Domain.RepositoryType.Bidir;
								else if (txIsVisible)
									this.lastMonitorSelection = Domain.RepositoryType.Tx;
								else
									throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "One of the monitors must be visible!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
							}

							break;
						}

						default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + this.lastMonitorSelection + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

				if (this.settingsRoot.Layout.SendTextPanelIsVisible || this.settingsRoot.Layout.SendFilePanelIsVisible)
				{
					int height;
					if (this.settingsRoot.Layout.SendTextPanelIsVisible && this.settingsRoot.Layout.SendFilePanelIsVisible)
						height = (int)Math.Ceiling(this.sendSizeHelper.Scale.Height * (Send.DesignedFullHeight + 3)); // Full + margin of 3.
					else
						height = (int)Math.Ceiling(this.sendSizeHelper.Scale.Height * (Send.DesignedHalfHeight + 3)); // Half + margin of 3.

					// Adjust send panel size depending on one or two sub-panels:
					if (splitContainer_Terminal.Panel2MinSize != height)
						splitContainer_Terminal.Panel2MinSize = height;

					int distance = (splitContainer_Terminal.Height - height - splitContainer_Terminal.SplitterWidth);
					int limitedDistance;
					if (SplitContainerHelper.TryLimitSplitterDistance(splitContainer_Terminal, distance, out limitedDistance))
					{
						if (splitContainer_Terminal.SplitterDistance != limitedDistance)
							splitContainer_Terminal.SplitterDistance = limitedDistance;
					}
				#if DEBUG
					else
					{
						Debugger.Break(); // See debug output for issue and instructions!
					}
				#endif
				}

				AdjustSendSplitter();
			}
			finally
			{
				this.isSettingControls.Leave();
				ResumeLayout(false);
			}
		}

		private void AdjustSendSplitter()
		{
			// Calculate absolute splitter position (distance) of predefined splitter,
			// including offset to get send buttons pixel-accurate below predefined buttons:
			const int PredefinedOffset = 6; // 2 x margin of 3 (frame + buttons)
			int absoluteX = splitContainer_Predefined.SplitterDistance + splitContainer_Predefined.Left;
			int relativeX = absoluteX - send.Left + PredefinedOffset;
			send.SendSplitterDistance = Int32Ex.Limit(relativeX, 0, Math.Max((send.Width - 1), 0)); // 'max' must be 0 or above.
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
			contextMenuStrip_Command_SetMenuItems(false); // Ensure that shortcuts are activated.
			contextMenuStrip_Page_SetMenuItems(false);    // Ensure that shortcuts are activated.

			this.isSettingControls.Enter();
			predefined.SuspendCommandStateUpdate();
			try
			{
				predefined.ParseModeForText      = this.settingsRoot.Send.Text.ToParseMode();
				predefined.RootDirectoryForFile  = Path.GetDirectoryName(this.terminal.SettingsFilePath);
				predefined.TerminalIsReadyToSend = this.terminal.IsReadyToSend;
			}
			finally
			{
				predefined.ResumeCommandStateUpdate();
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoActionControls()
		{
			toolStripMenuItem_TerminalMenu_Send_AutoAction_SetMenuItems();
		}

		private void SetAutoResponseControls()
		{
			toolStripMenuItem_TerminalMenu_Send_AutoResponse_SetMenuItems();
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
				send.ParseModeForText        = this.settingsRoot.Send.Text.ToParseMode();
				send.SendTextImmediately     = this.settingsRoot.Send.Text.SendImmediately;
				send.RootDirectoryForFile    = Path.GetDirectoryName(this.terminal.SettingsFilePath);
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

		/// <remarks>
		/// Currently, presets are limited to those hardcoded below.
		/// </remarks>
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
				var es = this.settingsRoot.Explicit;
				var scs = es.Terminal.IO.SerialPort.Communication;
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
					this.terminal.ApplyTerminalSettings(es); // \ToDo: Not a good solution, should be called in Model.Terminal.HandleTerminalSettings(), but that gets called too often => FR #309.
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

				default: throw (new ArgumentOutOfRangeException("direction", direction, MessageHelper.InvalidExecutionPreamble + "'" + direction + "' is a direction that is not valid!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void SetMonitorOrientation(Orientation orientation)
		{
			this.settingsRoot.Layout.MonitorOrientation = orientation;

			SuspendLayout(); // Useful as the 'Size' and 'Location' properties will get changed.
			try
			{
				splitContainer_TxMonitor.Orientation = orientation;
				splitContainer_RxMonitor.Orientation = orientation;
			}
			finally
			{
				ResumeLayout(false);
			}
		}

		private void SetMonitorSettings()
		{
			SuspendLayout(); // Useful as the 'Size' and 'Location' properties will get changed.
			try
			{
				monitor_Tx   .MaxLineCount = this.settingsRoot.Display.MaxLineCount;
				monitor_Bidir.MaxLineCount = this.settingsRoot.Display.MaxLineCount;
				monitor_Rx   .MaxLineCount = this.settingsRoot.Display.MaxLineCount;

				monitor_Tx   .ShowLineNumbers = this.settingsRoot.Display.ShowLineNumbers;
				monitor_Bidir.ShowLineNumbers = this.settingsRoot.Display.ShowLineNumbers;
				monitor_Rx   .ShowLineNumbers = this.settingsRoot.Display.ShowLineNumbers;

				monitor_Tx   .LineNumberSelection = this.settingsRoot.Display.LineNumberSelection;
				monitor_Bidir.LineNumberSelection = this.settingsRoot.Display.LineNumberSelection;
				monitor_Rx   .LineNumberSelection = this.settingsRoot.Display.LineNumberSelection;

				monitor_Tx   .ShowCopyOfActiveLine = this.settingsRoot.Display.ShowCopyOfActiveLine;
				monitor_Bidir.ShowCopyOfActiveLine = this.settingsRoot.Display.ShowCopyOfActiveLine;
				monitor_Rx   .ShowCopyOfActiveLine = this.settingsRoot.Display.ShowCopyOfActiveLine;
			}
			finally
			{
				ResumeLayout(false);
			}
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

		private void ClearMonitor(Domain.RepositoryType repositoryType)
		{
			try
			{
				SetFixedStatusText("Clearing...");
				Cursor = Cursors.WaitCursor;

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
						throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				Cursor = Cursors.Default;
				SetTimedStatusText("Clearing done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Clearing failed!");
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void ClearMonitors()
		{
			try
			{
				SetFixedStatusText("Clearing...");
				Cursor = Cursors.WaitCursor;

				this.terminal.ClearRepositories();

				Cursor = Cursors.Default;
				SetTimedStatusText("Clearing done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Clearing failed!");
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void RefreshMonitor(Domain.RepositoryType repositoryType)
		{
			try
			{
				SetFixedStatusText("Refreshing...");
				Cursor = Cursors.WaitCursor;

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
						throw (new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}

				Cursor = Cursors.Default;
				SetTimedStatusText("Refreshing done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Refreshing failed!");
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void RefreshMonitors()
		{
			try
			{
				SetFixedStatusText("Refreshing...");
				Cursor = Cursors.WaitCursor;

				this.terminal.RefreshRepositories();

				Cursor = Cursors.Default;
				SetTimedStatusText("Refreshing done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Refreshing failed!");
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void ReformatMonitors()
		{
			try
			{
				SetFixedStatusText("Reformatting...");
				Cursor = Cursors.WaitCursor;
				                            //// Clone settings to ensure decoupling:
				monitor_Tx   .FormatSettings = new Format.Settings.FormatSettings(this.settingsRoot.Format);
				monitor_Bidir.FormatSettings = new Format.Settings.FormatSettings(this.settingsRoot.Format);
				monitor_Rx   .FormatSettings = new Format.Settings.FormatSettings(this.settingsRoot.Format);

				Cursor = Cursors.Default;
				SetTimedStatusText("Reformatting done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Reformatting failed!");
			}
		}

		#endregion

		#region Monitor Panels > Methods
		//------------------------------------------------------------------------------------------
		// Monitor Panels > Methods
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowFormatSettings()
		{
			int[] customColors = ApplicationSettings.RoamingUserSettings.View.CustomColorsToWin32();

			var f = new FormatSettings(this.settingsRoot.Format, customColors,
			                           this.settingsRoot.Display.ContentSeparator, this.settingsRoot.Display.InfoSeparator, this.settingsRoot.Display.InfoEnclosure,
			                           this.settingsRoot.Display.TimeStampUseUtc, this.settingsRoot.Display.TimeStampFormat, this.settingsRoot.Display.TimeSpanFormat, this.settingsRoot.Display.TimeDeltaFormat, this.settingsRoot.Display.TimeDurationFormat);

			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				this.settingsRoot.Format = f.FormatSettingsResult;

				if (!ArrayEx.ValuesEqual(customColors, f.CustomColors))
				{
					ApplicationSettings.RoamingUserSettings.View.UpdateCustomColorsFromWin32(f.CustomColors);
					ApplicationSettings.SaveRoamingUserSettings();
				}

				this.settingsRoot.Display.ContentSeparator = f.ContentSeparatorResult;
				this.settingsRoot.Display.InfoSeparator    = f.InfoSeparatorResult;
				this.settingsRoot.Display.InfoEnclosure    = f.InfoEnclosureResult;

				this.settingsRoot.Display.TimeStampUseUtc    = f.TimeStampUseUtcResult;
				this.settingsRoot.Display.TimeStampFormat    = f.TimeStampFormatResult;
				this.settingsRoot.Display.TimeSpanFormat     = f.TimeSpanFormatResult;
				this.settingsRoot.Display.TimeDeltaFormat    = f.TimeDeltaFormatResult;
				this.settingsRoot.Display.TimeDurationFormat = f.TimeDurationFormatResult;
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
			try
			{
				SetFixedStatusText("Preparing copying...");
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case copying takes long.
				SetFixedStatusText("Copying selected lines to clipboard...");
				RtfWriterHelper.CopyLinesToClipboard(monitor.SelectedLines, this.settingsRoot.Format);
				Cursor = Cursors.Default;
				SetTimedStatusText("Copying done");
			}
			catch (ExternalException) // The clipboard could not be cleared. This typically
			{                         // occurs when it is being used by another process.
				Cursor = Cursors.Default;
				SetFixedStatusText("Copying failed!");
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowSaveMonitorDialog(Controls.Monitor monitor)
		{
			SetFixedStatusText("Preparing to save data...");

			string initialExtension = ApplicationSettings.RoamingUserSettings.Extensions.MonitorFiles;

			var sfd = new SaveFileDialog();
			sfd.Title       = "Save Monitor As";
			sfd.Filter      = ExtensionHelper.TextFilesFilter; // Fixed to text files since monitor displays lines.
			sfd.FilterIndex = ExtensionHelper.TextFilesFilterHelper(initialExtension);
			sfd.DefaultExt  = PathEx.DenormalizeExtension(initialExtension);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MonitorFiles;

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				ApplicationSettings.RoamingUserSettings.Extensions.MonitorFiles = Path.GetExtension(sfd.FileName);
				ApplicationSettings.LocalUserSettings.Paths.MonitorFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();
				ApplicationSettings.SaveRoamingUserSettings();

				Refresh(); // Ensure that form has been refreshed before continuing.
				SaveMonitor(monitor, sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		[ModalBehaviorContract(ModalBehavior.OnlyInCaseOfUserInteraction, Approval = "Only shown in case of an explicit user interaction.")]
		private void SaveMonitor(Controls.Monitor monitor, string filePath)
		{
			SetFixedStatusText("Saving selected lines...");
			try
			{
				int requestedCount = monitor.SelectedLines.Count;
				int savedCount;

				if (ExtensionHelper.IsXmlFile(filePath))
				{
				#if FALSE // Enable to use raw instead of text XML export schema, useful for development purposes of the raw XML schema.
					savedCount = Log.Utilities.XmlWriterHelperRaw.SaveLinesToFile(monitor.SelectedLines, filePath, true);
				#else
					savedCount = Log.Utilities.XmlWriterHelperText.SaveLinesToFile(monitor.SelectedLines, filePath, true);
				#endif
				}
				else if (ExtensionHelper.IsRtfFile(filePath))
				{
					savedCount = RtfWriterHelper.SaveLinesToFile(monitor.SelectedLines, filePath, this.settingsRoot.Format);
				}
				else
				{
					savedCount = TextWriterHelper.SaveLinesToFile(monitor.SelectedLines, filePath, this.settingsRoot.Format);
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

				var sb = new StringBuilder();
				sb.AppendLine("Unable to save selected lines to file");
				sb.AppendLine(filePath);
				sb.AppendLine();
				sb.AppendLine("System error message:");
				sb.Append    (e.Message);

				MessageBoxEx.Show
				(
					this,
					sb.ToString(),
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);

				ResetStatusText();
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
				Refresh(); // Ensure that form has been refreshed before continuing.
				PrintMonitor(monitor, pd.PrinterSettings);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void PrintMonitor(Controls.Monitor monitor, System.Drawing.Printing.PrinterSettings settings)
		{
			SetFixedStatusText("Printing data...");

			using (var printer = new RtfPrinter(settings))
			{
				try
				{
					printer.Print(monitor.SelectedLines, this.settingsRoot.Format);
					SetTimedStatusText("Data printed");
				}
				catch (System.Drawing.Printing.InvalidPrinterException ex)
				{
					SetFixedStatusText("Data not printed!");

					var sb = new StringBuilder();
					sb.AppendLine("Unable to print data!");
					sb.AppendLine();
					sb.AppendLine("System error message:");
					sb.Append    (ex.Message);

					MessageBoxEx.Show
					(
						this,
						sb.ToString(),
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

		private void SetPageLayout(PredefinedCommandPageLayout layout)
		{
			Model.Settings.PredefinedCommandSettings predefinedCommandNew;
			if (CommandPagesSettingsChangeHelper.TryChange(this, this.settingsRoot.PredefinedCommand, layout, out predefinedCommandNew))
			{
				this.settingsRoot.PredefinedCommand = predefinedCommandNew;
				//// settingsRoot_Changed() will update the form.
			}
		}

		private void SelectPredefinedPanel()
		{
			predefined.Select();
		}

		/// <param name="commandId">Command 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		private void CopyPredefined(int commandId)
		{
			int pageId = predefined.SelectedPageId;
			if (!this.terminal.CopyPredefined(pageId, commandId))
			{
				// If command is invalid, show settings dialog.
				ShowPredefinedCommandSettings(pageId, commandId);
			}
		}

		/// <param name="commandId">Command 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		private void SendPredefined(int commandId)
		{
			if (this.terminal.IsReadyToSend)
			{
				int pageId = predefined.SelectedPageId;
				if (!this.terminal.SendPredefined(pageId, commandId))
				{
					// If command is invalid, show settings dialog.
					ShowPredefinedCommandSettings(pageId, commandId);
				}
			}
		}

		/// <param name="pageId">Page 1..<see cref="PredefinedCommandPageCollection.MaxCapacity"/>.</param>
		/// <param name="commandId">Command 1..<see cref="PredefinedCommandPage.MaxCommandCapacityPerPage"/>.</param>
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowPredefinedCommandSettings(int pageId, int commandId)
		{
			var f = new PredefinedCommandSettings
			(
				this.settingsRoot.PredefinedCommand,
				this.settingsRoot.TerminalType,
				this.settingsRoot.Send.UseExplicitDefaultRadix,
				this.settingsRoot.Send.Text.ToParseMode(),
				Path.GetDirectoryName(this.terminal.SettingsFilePath),
				pageId,
				commandId,
				IndicatedName
			);

			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				this.settingsRoot.PredefinedCommand = f.SettingsResult;
				this.settingsRoot.Predefined.SelectedPageId = f.SelectedPageId;
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
			{
				this.settingsRoot.Changed += settingsRoot_Changed;

				// Initialize 'Changed' detectors:
				this.settingsRoot_Changed_txMonitorPanelIsVisibleOld    = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
				this.settingsRoot_Changed_bidirMonitorPanelIsVisibleOld = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
				this.settingsRoot_Changed_rxMonitorPanelIsVisibleOld    = this.settingsRoot.Layout.RxMonitorPanelIsVisible;
			}
		}

		private void DetachSettingsEventHandlers()
		{
			if (this.settingsRoot != null)
			{
				this.settingsRoot.Changed -= settingsRoot_Changed;
			}
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
				{              // Layout must be changed first! Otherwise pages may not be complete!
					predefined.PageLayout = this.settingsRoot.PredefinedCommand.PageLayout;
					predefined.Pages      = this.settingsRoot.PredefinedCommand.Pages;
				}              // See remarks of this property!
				finally
				{
					this.isSettingControls.Leave();
				}

				SetPredefinedControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoAction))
			{
				SetAutoActionControls();
				OnAutoActionSettingsChanged(EventArgs.Empty);
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoResponse))
			{
				SetAutoResponseControls();
				OnAutoResponseSettingsChanged(EventArgs.Empty);
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
					predefined.SelectedPageId = this.settingsRoot.Predefined.SelectedPageId;
				}
				finally
				{
					this.isSettingControls.Leave();
				}

				SetPredefinedControls();
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Window))
			{
				// Nothing to do, window settings are only saved.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Layout))
			{
				if (this.settingsRoot_Changed_txMonitorPanelIsVisibleOld != this.settingsRoot.Layout.TxMonitorPanelIsVisible) {
					this.settingsRoot_Changed_txMonitorPanelIsVisibleOld = this.settingsRoot.Layout.TxMonitorPanelIsVisible;

					if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
						RefreshMonitor(Domain.RepositoryType.Tx);
				}

				if (this.settingsRoot_Changed_bidirMonitorPanelIsVisibleOld != this.settingsRoot.Layout.BidirMonitorPanelIsVisible) {
					this.settingsRoot_Changed_bidirMonitorPanelIsVisibleOld = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;

					if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
						RefreshMonitor(Domain.RepositoryType.Bidir);
				}

				if (this.settingsRoot_Changed_rxMonitorPanelIsVisibleOld != this.settingsRoot.Layout.RxMonitorPanelIsVisible) {
					this.settingsRoot_Changed_rxMonitorPanelIsVisibleOld = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

					if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
						RefreshMonitor(Domain.RepositoryType.Rx);
				}

				LayoutTerminal();
				SetLayoutControls();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'txMonitorPanelIsVisibleOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_txMonitorPanelIsVisibleOld = Model.Settings.LayoutSettings.TxMonitorPanelIsVisibleDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'txMonitorPanelIsVisibleOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_bidirMonitorPanelIsVisibleOld = Model.Settings.LayoutSettings.BidirMonitorPanelIsVisibleDefault;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "'txMonitorPanelIsVisibleOld' does start with a lower case letter.")]
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool settingsRoot_Changed_rxMonitorPanelIsVisibleOld = Model.Settings.LayoutSettings.RxMonitorPanelIsVisibleDefault;

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
				this.terminal.IOChanged               += terminal_IOChanged;
				this.terminal.IOControlChanged        += terminal_IOControlChanged;
				this.terminal.IOConnectTimeChanged    += terminal_IOConnectTimeChanged;
			////this.terminal.IOCountChanged_Promptly += terminal_IOCountChanged_Promptly is not used, see further below for reason.
			////this.terminal.IORateChanged_Promptly  += terminal_IORateChanged_Promptly  is not used, see further below for reason.
				this.terminal.IORateChanged_Decimated += terminal_IORateChanged_Decimated;
				this.terminal.IOError                 += terminal_IOError;

			////this.terminal.SendingIsOngoingChanged += terminal_SendingIsOngoingChanged is not needed yet.
				this.terminal.SendingIsBusyChanged    += terminal_SendingIsBusyChanged;

				this.terminal.DisplayElementsTxAdded          += terminal_DisplayElementsTxAdded;
				this.terminal.DisplayElementsBidirAdded       += terminal_DisplayElementsBidirAdded;
				this.terminal.DisplayElementsRxAdded          += terminal_DisplayElementsRxAdded;
				this.terminal.CurrentDisplayLineTxReplaced    += terminal_CurrentDisplayLineTxReplaced;
				this.terminal.CurrentDisplayLineBidirReplaced += terminal_CurrentDisplayLineBidirReplaced;
				this.terminal.CurrentDisplayLineRxReplaced    += terminal_CurrentDisplayLineRxReplaced;
				this.terminal.CurrentDisplayLineTxCleared     += terminal_CurrentDisplayLineTxCleared;
				this.terminal.CurrentDisplayLineBidirCleared  += terminal_CurrentDisplayLineBidirCleared;
				this.terminal.CurrentDisplayLineRxCleared     += terminal_CurrentDisplayLineRxCleared;
				this.terminal.DisplayLinesTxAdded             += terminal_DisplayLinesTxAdded;
				this.terminal.DisplayLinesBidirAdded          += terminal_DisplayLinesBidirAdded;
				this.terminal.DisplayLinesRxAdded             += terminal_DisplayLinesRxAdded;

				this.terminal.RepositoryTxCleared             += terminal_RepositoryTxCleared;
				this.terminal.RepositoryBidirCleared          += terminal_RepositoryBidirCleared;
				this.terminal.RepositoryRxCleared             += terminal_RepositoryRxCleared;
				this.terminal.RepositoryTxReloaded            += terminal_RepositoryTxReloaded;
				this.terminal.RepositoryBidirReloaded         += terminal_RepositoryBidirReloaded;
				this.terminal.RepositoryRxReloaded            += terminal_RepositoryRxReloaded;

				this.terminal.AutoActionPlotRequest           += terminal_AutoActionPlotRequest;
				this.terminal.AutoActionCountChanged          += terminal_AutoActionCountChanged;
				this.terminal.AutoResponseCountChanged        += terminal_AutoResponseCountChanged;

				this.terminal.FixedStatusTextRequest             += terminal_FixedStatusTextRequest;
				this.terminal.TimedStatusTextRequest             += terminal_TimedStatusTextRequest;
				this.terminal.ResetStatusTextRequest             += terminal_ResetStatusTextRequest;
				this.terminal.CursorRequest                      += terminal_CursorRequest;
				this.terminal.MessageInputRequest                += terminal_MessageInputRequest;
				this.terminal.SaveAsFileDialogRequest            += terminal_SaveAsFileDialogRequest;
				this.terminal.SaveCommandPageAsFileDialogRequest += terminal_SaveCommandPageAsFileDialogRequest;
				this.terminal.OpenCommandPageFileDialogRequest   += terminal_OpenCommandPageFileDialogRequest;

				this.terminal.Saved                              += terminal_Saved;
				this.terminal.Closed                             += terminal_Closed;
			}
		}

		private void DetachTerminalEventHandlers()
		{
			if (this.terminal != null)
			{
				this.terminal.IOChanged               -= terminal_IOChanged;
				this.terminal.IOControlChanged        -= terminal_IOControlChanged;
				this.terminal.IOConnectTimeChanged    -= terminal_IOConnectTimeChanged;
			////this.terminal.IOCountChanged_Promptly -= terminal_IOCountChanged_Promptly is not used, see further below for reason.
			////this.terminal.IORateChanged_Promptly  -= terminal_IORateChanged_Promptly  is not used, see further below for reason.
				this.terminal.IORateChanged_Decimated -= terminal_IORateChanged_Decimated;
				this.terminal.IOError                 -= terminal_IOError;

			////this.terminal.SendingIsOngoingChanged -= terminal_SendingIsOngoingChanged is not needed yet.
				this.terminal.SendingIsBusyChanged    -= terminal_SendingIsBusyChanged;

				this.terminal.DisplayElementsTxAdded          -= terminal_DisplayElementsTxAdded;
				this.terminal.DisplayElementsBidirAdded       -= terminal_DisplayElementsBidirAdded;
				this.terminal.DisplayElementsRxAdded          -= terminal_DisplayElementsRxAdded;
				this.terminal.CurrentDisplayLineTxReplaced    -= terminal_CurrentDisplayLineTxReplaced;
				this.terminal.CurrentDisplayLineBidirReplaced -= terminal_CurrentDisplayLineBidirReplaced;
				this.terminal.CurrentDisplayLineRxReplaced    -= terminal_CurrentDisplayLineRxReplaced;
				this.terminal.CurrentDisplayLineTxCleared     -= terminal_CurrentDisplayLineTxCleared;
				this.terminal.CurrentDisplayLineBidirCleared  -= terminal_CurrentDisplayLineBidirCleared;
				this.terminal.CurrentDisplayLineRxCleared     -= terminal_CurrentDisplayLineRxCleared;
				this.terminal.DisplayLinesTxAdded             -= terminal_DisplayLinesTxAdded;
				this.terminal.DisplayLinesBidirAdded          -= terminal_DisplayLinesBidirAdded;
				this.terminal.DisplayLinesRxAdded             -= terminal_DisplayLinesRxAdded;

				this.terminal.RepositoryTxCleared             -= terminal_RepositoryTxCleared;
				this.terminal.RepositoryBidirCleared          -= terminal_RepositoryBidirCleared;
				this.terminal.RepositoryRxCleared             -= terminal_RepositoryRxCleared;
				this.terminal.RepositoryTxReloaded            -= terminal_RepositoryTxReloaded;
				this.terminal.RepositoryBidirReloaded         -= terminal_RepositoryBidirReloaded;
				this.terminal.RepositoryRxReloaded            -= terminal_RepositoryRxReloaded;

				this.terminal.AutoActionPlotRequest           -= terminal_AutoActionPlotRequest;
				this.terminal.AutoActionCountChanged          -= terminal_AutoActionCountChanged;
				this.terminal.AutoResponseCountChanged        -= terminal_AutoResponseCountChanged;

				this.terminal.FixedStatusTextRequest             -= terminal_FixedStatusTextRequest;
				this.terminal.TimedStatusTextRequest             -= terminal_TimedStatusTextRequest;
				this.terminal.ResetStatusTextRequest             -= terminal_ResetStatusTextRequest;
				this.terminal.CursorRequest                      -= terminal_CursorRequest;
				this.terminal.MessageInputRequest                -= terminal_MessageInputRequest;
				this.terminal.SaveAsFileDialogRequest            -= terminal_SaveAsFileDialogRequest;
				this.terminal.SaveCommandPageAsFileDialogRequest -= terminal_SaveCommandPageAsFileDialogRequest;
				this.terminal.OpenCommandPageFileDialogRequest   -= terminal_OpenCommandPageFileDialogRequest;

				this.terminal.Saved                              -= terminal_Saved;
				this.terminal.Closed                             -= terminal_Closed;
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

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IOChanged(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetTerminalControls();

			OnTerminalChanged(EventArgs.Empty);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IOControlChanged(object sender, Domain.IOControlEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetIOControlControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
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

		// The 'terminal_IOCount/RateChanged_Promptly' events are not used because of the reason
		// described in the remarks of 'terminal_RawChunkSent/Received' of 'Model.Terminal'.
		// Instead, the update is done by the 'terminal_DisplayElements[Tx|Bidir|Rx]Added' and
		// 'terminal_DisplayLines[Tx|Bidir|Rx]Added' handlers.
		//
		// 'terminal_IORateChanged_Decimated' is fine.

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IORateChanged_Decimated(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (TerminalIsAvailable)
			{
				int txByteRate, txLineRate, rxByteRate, rxLineRate = 0;

				this.terminal.GetDataRate(out txByteRate, out txLineRate, out rxByteRate, out rxLineRate);

				monitor_Tx   .SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
				monitor_Bidir.SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
				monitor_Rx   .SetDataRateStatus(txByteRate, txLineRate, rxByteRate, rxLineRate);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[ModalBehaviorContract(ModalBehavior.InCaseOfNonUserError, Approval = "StartArgs are considered to decide on behavior.")]
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
				SetFixedStatusText("Terminal Error!");

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

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_SendingIsBusyChanged(object sender, EventArgs<bool> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetIOStatus();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataCountAndRateStatus()
		{
			SetDataCountAndRateStatusSent();
			SetDataCountAndRateStatusReceived();
		}

		/// <remarks>
		/// Named 'Sent' to emphasize relation to 'Tx' as well as 'Bidir' monitor.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataCountAndRateStatusSent()
		{
			if (!TerminalIsAvailable)
				return; // Ensure not to handle events during closing anymore.

			int txByte, txLine, rxByte, rxLine = 0;

			this.terminal.GetDataCount(out txByte, out txLine, out rxByte, out rxLine);
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)    { monitor_Tx   .SetDataCountStatus(txByte, txLine, rxByte, rxLine); }
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible) { monitor_Bidir.SetDataCountStatus(txByte, txLine, rxByte, rxLine); }

			this.terminal.GetDataRate(out txByte, out txLine, out rxByte, out rxLine);
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)    { monitor_Tx.   SetDataRateStatus(txByte, txLine, rxByte, rxLine); }
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible) { monitor_Bidir.SetDataRateStatus(txByte, txLine, rxByte, rxLine); }
		}

		/// <remarks>
		/// Named 'Received' to emphasize relation to 'Rx' as well as 'Bidir' monitor.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataCountAndRateStatusReceived()
		{
			if (!TerminalIsAvailable)
				return; // Ensure not to handle events during closing anymore.

			int txByte, txLine, rxByte, rxLine = 0;

			this.terminal.GetDataCount(out txByte, out txLine, out rxByte, out rxLine);
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible) { monitor_Bidir.SetDataCountStatus(txByte, txLine, rxByte, rxLine); }
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)    { monitor_Rx.   SetDataCountStatus(txByte, txLine, rxByte, rxLine); }

			this.terminal.GetDataRate(out txByte, out txLine, out rxByte, out rxLine);
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible) { monitor_Bidir.SetDataRateStatus(txByte, txLine, rxByte, rxLine); }
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)    { monitor_Rx.   SetDataRateStatus(txByte, txLine, rxByte, rxLine); }
		}

		/// <summary>
		/// 'Normally', the display is updated by the 'DisplayElements[Tx|Bidir|Rx]Added' events,
		/// except for those cases where processing is limited to 'DisplayLines[Bidir|Rx][Added|Reloaded]':
		/// <list type="bullet">
		/// <item><description>AutoAction: Filter/Suppress.</description></item>
		/// <item><description>AutoAction: Text based triggers.</description></item>
		/// <item><description>AutoReponse: Text based triggers.</description></item>
		/// </list>
		/// Not the perfect solution, but considered good enough, although it doesn't fully work when both
		/// automatic action and response are active. But then highlighting becomes limited anyway...
		/// </summary>
		private bool UseDisplayElementsAdded
		{
			get
			{
				var autoActionCondition = (settingsRoot.AutoAction.IsActive && (settingsRoot.AutoAction.Trigger != AutoTrigger.AnyLine) &&
				                           settingsRoot.AutoAction.IsByteSequenceTriggered && // Text based triggering is evaluated in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].
				                           settingsRoot.AutoAction.IsNeitherFilterNorSuppress); // Filter/Suppress is limited to be processed in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].

				var autoResponseCondition = (settingsRoot.AutoResponse.IsActive && (settingsRoot.AutoResponse.Trigger != AutoTrigger.AnyLine) &&
				                             settingsRoot.AutoResponse.IsByteSequenceTriggered); // Text based triggering is evaluated in terminal_DisplayLines[Bidir|Rx][Added|Reloaded].

				return (autoActionCondition || autoResponseCondition);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsTxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (UseDisplayElementsAdded) // See propery for background.
			{
				if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
					monitor_Tx.AddElements(e.Elements);
			}

			SetDataCountAndRateStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsBidirAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (UseDisplayElementsAdded) // See propery for background.
			{
				if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
					monitor_Bidir.AddElements(e.Elements);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsRxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (UseDisplayElementsAdded) // See propery for background.
			{
				if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
					monitor_Rx.AddElements(e.Elements);
			}

			SetDataCountAndRateStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.ReplaceCurrentLine(e.Elements);

			SetDataCountAndRateStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.ReplaceCurrentLine(e.Elements);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.ReplaceCurrentLine(e.Elements);

			SetDataCountAndRateStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.ClearCurrentLine();

			SetDataCountAndRateStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.ClearCurrentLine();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.ClearCurrentLine();

			SetDataCountAndRateStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesTxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (!UseDisplayElementsAdded) // See propery for background.
			{
				if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
					monitor_Tx.AddLines(e.Lines);
			}

			SetDataCountAndRateStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesBidirAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (!UseDisplayElementsAdded) // See propery for background.
			{
				if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
					monitor_Bidir.AddLines(e.Lines);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesRxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (!UseDisplayElementsAdded) // See propery for background.
			{
				if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
					monitor_Rx.AddLines(e.Lines);
			}

			SetDataCountAndRateStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.Clear();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.Clear();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxCleared(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.Clear();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.AddLines(e.Lines);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.AddLines(e.Lines);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.AddLines(e.Lines);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_AutoActionPlotRequest(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			AutoActionPlot();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_AutoActionCountChanged(object sender, EventArgs<int> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnAutoActionCountChanged(e);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_AutoResponseCountChanged(object sender, EventArgs<int> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			OnAutoResponseCountChanged(e);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_FixedStatusTextRequest(object sender, EventArgs<string> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetFixedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_TimedStatusTextRequest(object sender, EventArgs<string> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetTimedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_ResetStatusTextRequest(object sender, EventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			ResetStatusText();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			Refresh(); // Ensure that form has been refreshed before showing the message box.
			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			e.Result = ShowSaveTerminalAsFileDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_SaveCommandPageAsFileDialogRequest(object sender, Model.FilePathDialogEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			string filePathNew;
			e.Result = CommandPagesSettingsFileLinkHelper.ShowSaveAsFileDialog(this, e.FilePathOld, out filePathNew);
			e.FilePathNew = filePathNew;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_OpenCommandPageFileDialogRequest(object sender, Model.FilePathDialogEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			string filePathNew;
			e.Result = CommandPagesSettingsFileLinkHelper.ShowOpenFileDialog(this, e.FilePathOld, out filePathNew);
			e.FilePathNew = filePathNew;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			Cursor = e.Value;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_Saved(object sender, Model.SavedEventArgs e)
		{
			if (IsDisposed)
				return; // Ensure not to handle events during closing anymore.

			SetTerminalControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private DialogResult ShowSaveTerminalAsFileDialog()
		{
			SetFixedStatusText("Saving terminal as...");

			var sfd = new SaveFileDialog();
			sfd.Title       = "Save " + IndicatedName + " As";
			sfd.Filter      = ExtensionHelper.TerminalFilesFilter;
			sfd.FilterIndex = ExtensionHelper.TerminalFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.TerminalExtension);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;

			// Check whether the terminal has already been saved as a .yat file:
			if (StringEx.EndsWithOrdinalIgnoreCase(IndicatedName, ExtensionHelper.TerminalExtension))
				sfd.FileName = IndicatedName;
			else
				sfd.FileName = IndicatedName + PathEx.NormalizeExtension(sfd.DefaultExt); // Note that 'DefaultExt' states "the returned string does not include the period".

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Refresh(); // Ensure that form has been refreshed before continuing.
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowTerminalSettings()
		{
			SetFixedStatusText("Terminal Settings...");

			var f = new TerminalSettings(this.settingsRoot.Explicit);

			f.TerminalId     = this.terminal.SequentialId;
			f.TerminalIsOpen = this.terminal.IsOpen;

			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				var fsr = f.SettingsResult;
				if (fsr.HaveChanged)
				{
					SuspendHandlingTerminalSettings();
					try
					{
						this.terminal.ApplyTerminalSettings(fsr); // \ToDo: Not a good solution, should be called in Model.Terminal.HandleTerminalSettings(), but that gets called too often => FR #309.
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

			// AutoAction:
			SetAutoActionPlotFormCaption();
		}

		private void SetTerminalCaption()
		{
			if (TerminalIsAvailable)
				Text = this.terminal.Caption;
			else
				Text = "";
		}

		private void SetIOStatus()
		{
			Image green  = Properties.Resources.Image_Status_Green_12x12;
			Image yellow = Properties.Resources.Image_Status_Yellow_12x12;
			Image red    = Properties.Resources.Image_Status_Red_12x12;

			if (TerminalIsAvailable)
			{
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Enabled =  this.terminal.IsStarted;

				if (this.terminal.IsOpen)
				{
					if (this.terminal.IsTransmissive)
					{
						if (!this.terminal.SendingIsBusy)
						{
							ResetIOStatusFlashing();
							toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

							if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != green) // Improve performance by only assigning if different.
								toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = green;
						}
						else // sending is busy
						{
							toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Flashing;
							StartIOStatusFlashing();
							//// Do not directly access the image, it will be flashed by the timer below.
							//// Directly accessing the image could result in irregular flashing.
						}
					}
					else // can only receive (so far)
					{
						ResetIOStatusFlashing();
						toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

						if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != yellow) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = yellow;
					}
				}
				else // is closed
				{
					ResetIOStatusFlashing();
					toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

					if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != red) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = red;
				}

				toolStripStatusLabel_TerminalStatus_IOStatus.Text = this.terminal.IOStatusText;
			}
			else // 'TerminalIsNotAvailable'
			{
				ResetIOStatusFlashing();
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Enabled = false;
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

				if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != red) // Improve performance by only assigning if different.
					toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = red;

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
			try
			{
				Image on  = Properties.Resources.Image_Status_Green_12x12;
				Image off = Properties.Resources.Image_Status_Red_12x12;

				bool isOpen = ((this.terminal != null) && (this.terminal.IsOpen));

				bool isSerialPort   = false;
				bool isUsbSerialHid = false;

				if (this.settingsRoot != null)
				{
					isSerialPort   = (this.settingsRoot.IOType == Domain.IOType.SerialPort);
					isUsbSerialHid = (this.settingsRoot.IOType == Domain.IOType.UsbSerialHid);
				}

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

						toolStripStatusLabel_TerminalStatus_RTS.Text += (" | " + pinCount.RtsDisableCount.ToString(CultureInfo.CurrentCulture));
						toolStripStatusLabel_TerminalStatus_CTS.Text += (" | " + pinCount.CtsDisableCount.ToString(CultureInfo.CurrentCulture));
						toolStripStatusLabel_TerminalStatus_DTR.Text += (" | " + pinCount.DtrDisableCount.ToString(CultureInfo.CurrentCulture));
						toolStripStatusLabel_TerminalStatus_DSR.Text += (" | " + pinCount.DsrDisableCount.ToString(CultureInfo.CurrentCulture));
						toolStripStatusLabel_TerminalStatus_DCD.Text += (" | " + pinCount.DcdCount       .ToString(CultureInfo.CurrentCulture));

						toolStripStatusLabel_TerminalStatus_RTS.ToolTipText += (" | Disable Count");
						toolStripStatusLabel_TerminalStatus_CTS.ToolTipText += (" | Disable Count");
						toolStripStatusLabel_TerminalStatus_DTR.ToolTipText += (" | Disable Count");
						toolStripStatusLabel_TerminalStatus_DSR.ToolTipText += (" | Disable Count");
						toolStripStatusLabel_TerminalStatus_DCD.ToolTipText += (" | Count");

						var sentXOnCount      = ((this.terminal != null) ? (this.terminal.SentXOnCount)      : (0));
						var sentXOffCount     = ((this.terminal != null) ? (this.terminal.SentXOffCount)     : (0));
						var receivedXOnCount  = ((this.terminal != null) ? (this.terminal.ReceivedXOnCount)  : (0));
						var receivedXOffCount = ((this.terminal != null) ? (this.terminal.ReceivedXOffCount) : (0));

						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text  += (" | " + sentXOnCount    .ToString(CultureInfo.CurrentCulture) + " | " + sentXOffCount    .ToString(CultureInfo.CurrentCulture));
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text += (" | " + receivedXOnCount.ToString(CultureInfo.CurrentCulture) + " | " + receivedXOffCount.ToString(CultureInfo.CurrentCulture));

						toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText  += (" | XOn Count | XOff Count");
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText += (" | XOn Count | XOff Count");
					}

					if (this.settingsRoot.Terminal.Status.ShowBreakCount)
					{
						var inputBreakCount      = ((this.terminal != null) ? (this.terminal.InputBreakCount)  : (0));
						var outputBreakCount     = ((this.terminal != null) ? (this.terminal.OutputBreakCount) : (0));

						toolStripStatusLabel_TerminalStatus_InputBreak.Text  += (" | " + inputBreakCount .ToString(CultureInfo.CurrentCulture));
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
						bool inputIsXOn      = false;
						bool outputIsXOn     = false;

						var x = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
						if (x != null)
						{
							try // Fail-safe implementation, especially catching exceptions while closing.
							{
							////indicateXOnXOff = x.XOnXOffIsInUse; >> See above (bug #214).
								inputIsXOn      = x.InputIsXOn;
								outputIsXOn     = x.OutputIsXOn;
							}
							catch (Exception ex)
							{
								DebugEx.WriteException(GetType(), ex, "Failed to retrieve XOn/XOff state");
							}
						}

						if (this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControl == MKY.IO.Serial.SerialPort.SerialFlowControl.RS485)
						{
							if (pins.Rts)
								TriggerRtsLuminescence();
						}
						else
						{
							Image rtsImage = (pins.Rts ? on : off);

							if (toolStripStatusLabel_TerminalStatus_RTS.Image != rtsImage) // Improve performance by only assigning if different.
								toolStripStatusLabel_TerminalStatus_RTS.Image = rtsImage;
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

						bool allowRts = !this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesRtsCtsAutomatically;
						bool allowDtr = !this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically;

						Color rtsForeColor = (allowRts ? SystemColors.ControlText : SystemColors.GrayText);
						Color dtrForeColor = (allowDtr ? SystemColors.ControlText : SystemColors.GrayText);

						if (toolStripStatusLabel_TerminalStatus_RTS.ForeColor != rtsForeColor) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_RTS.ForeColor = rtsForeColor;

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
					else // = isClosed
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

						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Text  += (" | " + sentXOnCount    .ToString(CultureInfo.CurrentCulture) + " | " + sentXOffCount    .ToString(CultureInfo.CurrentCulture));
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Text += (" | " + receivedXOnCount.ToString(CultureInfo.CurrentCulture) + " | " + receivedXOffCount.ToString(CultureInfo.CurrentCulture));

						toolStripStatusLabel_TerminalStatus_InputXOnXOff.ToolTipText  += (" | XOn Count | XOff Count");
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ToolTipText += (" | XOn Count | XOff Count");
					}

					if (isOpen)
					{
						bool allowXOnXOff    = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
						bool indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff;
						bool inputIsXOn      = false;
						bool outputIsXOn     = false;

						var x = (this.terminal.UnderlyingIOProvider as MKY.IO.Serial.IXOnXOffHandler);
						if (x != null)
						{
							try // Fail-safe implementation, especially catching exceptions while closing.
							{
								indicateXOnXOff = x.XOnXOffIsInUse;
								inputIsXOn      = x.InputIsXOn;
								outputIsXOn     = x.OutputIsXOn;
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
					else // = isClosed
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
			}
			finally
			{
				ResumeLayout(false);
			}
		}

		[SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Justification = "The timer just invokes a single-shot callback.")]
		private void TriggerRtsLuminescence()
		{
			timer_RtsLuminescence.Enabled = false;

			Image on = Properties.Resources.Image_Status_Green_12x12;
			if (toolStripStatusLabel_TerminalStatus_RTS.Image != on) // Improve performance by only assigning if different.
				toolStripStatusLabel_TerminalStatus_RTS.Image = on;

			timer_RtsLuminescence.Interval = RtsLuminescenceInterval;
			timer_RtsLuminescence.Enabled = true;
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_RtsLuminescence_Tick(object sender, EventArgs e)
		{
			timer_RtsLuminescence.Enabled = false;
			ResetRts();
		}

		private void ResetRts()
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

				Image rts = (pins.Rts ? on : off);
				if (toolStripStatusLabel_TerminalStatus_RTS.Image != rts) // Improve performance by only assigning if different.
					toolStripStatusLabel_TerminalStatus_RTS.Image = rts;
			}
			else
			{
				if (toolStripStatusLabel_TerminalStatus_RTS.Image != off) // Improve performance by only assigning if different.
					toolStripStatusLabel_TerminalStatus_RTS.Image = off;
			}
		}

		#endregion

		#region Terminal > AutoActionPlot
		//------------------------------------------------------------------------------------------
		// Terminal > AutoActionPlot
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		protected virtual void AutoActionPlot()
		{
			// No additional synchronization is needed, the event handler and thus this method
			// already gets synchronized onto the main thread.

			if (this.autoActionPlotForm == null)
			{
				DeactivateAutoActionPlotRequestEvent(); // See method remarks!

				this.autoActionPlotForm = new AutoActionPlot(this.terminal);
				this.autoActionPlotForm.Text = ComposeAutoActionPlotFormText();
				this.autoActionPlotForm.FormClosing += AutoActionPlotForm_FormClosing;
				this.autoActionPlotForm.Show(this);
			}
			else
			{
				this.autoActionPlotForm.Text = ComposeAutoActionPlotFormText();
			}
		}

		private void SetAutoActionPlotFormCaption()
		{
			if (this.autoActionPlotForm != null)
				this.autoActionPlotForm.Text = ComposeAutoActionPlotFormText();
		}

		/// <summary></summary>
		protected virtual string ComposeAutoActionPlotFormText()
		{
			// Note:
			// Same "YAT - [Caption]" as for MDI main form.

			var sb = new StringBuilder(ApplicationEx.ProductName); // "YAT" or "YATConsole" shall be indicated in main title bar.

			if (this.terminal != null)
			{
				sb.Append(" - [");
				sb.Append(this.terminal.ComposeInvariantCaption("Automatic Action Plot"));
				sb.Append("]");
			}

			return (sb.ToString());
		}

		private void AutoActionPlotForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.autoActionPlotForm = null;

			ReactivateAutoActionPlotRequestEvent();
		}

		/// <remarks>
		/// Required to prevent unnecessary delays when <see cref="Model.Terminal"/> invokes
		/// the <see cref="Model.Terminal.AutoActionPlotRequest"/> event, which will be
		/// will be synchronized onto the main thread, which in case of massive sending
		/// or receiving is already heavily loaded by the monitor update.
		/// </remarks>
		private void DeactivateAutoActionPlotRequestEvent()
		{
			this.terminal.AutoActionPlotRequest -= terminal_AutoActionPlotRequest;
		}

		private void ReactivateAutoActionPlotRequestEvent()
		{
			this.terminal.AutoActionPlotRequest += terminal_AutoActionPlotRequest;
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowLogSettings()
		{
			var f = new LogSettings(this.settingsRoot.Log, this.settingsRoot.LogIsOn);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				if ( this.settingsRoot.LogIsOn && f.RequestSwitchOff)
					RequestSwitchLogOff(); // Switch off before changing settings, an empty log may otherwise result.

				this.settingsRoot.Log = f.SettingsResult;

				if (!this.settingsRoot.LogIsOn && f.RequestSwitchOn)
					RequestSwitchLogOn();
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
		protected virtual void OnAutoActionSettingsChanged(EventArgs e)
		{
			EventHelper.RaiseSync(AutoActionSettingsChanged, this, e);
		}

		/// <remarks>Not using event args parameter for simplicity.</remarks>
		protected virtual void OnAutoActionTriggerStateChanged()
		{
			EventHelper.RaiseSync<EventArgs<AutoContentState>>(AutoActionTriggerStateChanged, this, new EventArgs<AutoContentState>(this.autoActionTriggerState));
		}

	/////// <remarks>Not using event args parameter for simplicity.</remarks>
	////protected virtual void OnAutoActionActionStateChanged() is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////{
	////	EventHelper.RaiseSync<EventArgs<AutoContentState>>(AutoActionActionStateChanged, this, , new EventArgs<AutoContentState>(this.autoActionActionState));
	////}

		/// <summary></summary>
		protected virtual void OnAutoActionCountChanged(EventArgs<int> e)
		{
			EventHelper.RaiseSync<EventArgs<int>>(AutoActionCountChanged, this, e);
		}

		/// <summary></summary>
		protected virtual void OnAutoResponseSettingsChanged(EventArgs e)
		{
			EventHelper.RaiseSync(AutoResponseSettingsChanged, this, e);
		}

		/// <remarks>Not using event args parameter for simplicity.</remarks>
		protected virtual void OnAutoResponseTriggerStateChanged()
		{
			EventHelper.RaiseSync<EventArgs<AutoContentState>>(AutoResponseTriggerStateChanged, this, new EventArgs<AutoContentState>(this.autoResponseTriggerState));
		}

		/// <remarks>Not using event args parameter for simplicity.</remarks>
		protected virtual void OnAutoResponseResponseStateChanged()
		{
			EventHelper.RaiseSync<EventArgs<AutoContentState>>(AutoResponseResponseStateChanged, this, new EventArgs<AutoContentState>(this.autoResponseResponseState));
		}

		/// <summary></summary>
		protected virtual void OnAutoResponseCountChanged(EventArgs<int> e)
		{
			EventHelper.RaiseSync<EventArgs<int>>(AutoResponseCountChanged, this, e);
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
