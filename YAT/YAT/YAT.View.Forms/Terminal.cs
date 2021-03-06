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
// YAT Version 2.4.1
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2021 Matthias Kläy.
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
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
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

[module: SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",                     Scope = "member",   Target = "YAT.View.Forms.Terminal.#InitializeComponent()", MessageId = "System.TimeSpan.Parse(System.String)", Justification = "Designer generated!")]
[module: SuppressMessage("Microsoft.Mobility",      "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Scope = "member",   Target = "YAT.View.Forms.Terminal.#InitializeComponent()", Justification = "The timer is only used for a well-defined interval.")]
[module: SuppressMessage("Microsoft.Naming",        "CA1703:ResourceStringsShouldBeSpelledCorrectly",    Scope = "resource", Target = "YAT.View.Forms.Terminal.resources", MessageId = "A-Za-z", Justification = "This is a valid regular expression.")]
[module: SuppressMessage("Microsoft.Naming",        "CA1703:ResourceStringsShouldBeSpelledCorrectly",    Scope = "resource", Target = "YAT.View.Forms.Terminal.resources", MessageId = "parseable",  Justification = "'parseable' is a correct English term.")]
[module: SuppressMessage("Microsoft.Performance",   "CA1823:AvoidUnusedPrivateFields",                   Scope = "member",   Target = "YAT.View.Forms.Terminal.#toolTip", Justification = "This is a bug in FxCop.")]

#endregion

namespace YAT.View.Forms
{
	/// <summary>
	/// The most important form of this application.
	/// </summary>
	/// <remarks>
	/// <code>#if (WITH_SCRIPTING)</code>
	/// The icon of this form is fixed to the YAT icon because the Albatros icon is very large
	/// (~ 500x500 pixels) and would result in a super-enlarged title bar! Looks really funny...
	/// <code>#endif</code>
	/// </remarks>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Albatros' is a name.")]
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
		private const int RtsLuminescenceDuration = 200; // Well above the natural 150 ms, but still quite promt.

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Scaling:
		private SizeHelper sendSizeHelper = new SizeHelper();
		private SplitContainerHelper splitContainerHelper = new SplitContainerHelper();

		// Initiating/Update/Closing:
		private bool isInitiating = true;
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
		// Find:
		private string lastFindPattern; // = null; Remark: Using "Pattern" instead of "TextOrPattern" for simplicity.

		// Auto:
		private AutoContentState autoActionTriggerState;      // = AutoContentState.Neutral;
		private bool autoActionTriggerValidationIsOngoing;    // = false;
	////private AutoContentState autoActionActionState;       // = AutoContentState.Neutral; is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private bool autoActionActionValidationIsOngoing;     // = false;                    is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
		private AutoContentState autoResponseTriggerState;    // = AutoContentState.Neutral;
		private bool autoResponseTriggerValidationIsOngoing;  // = false;
		private AutoContentState autoResponseResponseState;   // = AutoContentState.Neutral;
		private bool autoResponseResponseValidationIsOngoing; // = false;
		private AutoActionPlot   autoActionPlotForm;

		// View:
		private int findShortcutsCtrlFNPLSuspendedCount;      // = 0;
		private int editShortcutsCtrlACVDeleteSuspendedCount; // = 0;

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

		/// <summary></summary>
		public event EventHandler<EventArgs<bool>> FindAllSuccessChanged;

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

			InitializeComponent(); // Takes quite a while with the complexity this form has gotten...
			FixContextMenus();
			InitializeControls();

			// Link and attach to terminal model:
			this.terminal = terminal;
			AttachTerminalEventHandlers();
			DebugDecorateControls();

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
			this.isInitiating = false;

			this.mdiParent = MdiParent;

			PerformSplitContainerScalingWorkaround(); // See "SplitContainerHelper" for background.

			LayoutTerminal(); // Reapply layouting for proper behavior "Send" panel if [Send Text | File] is hidden. \remind (2018-04-08 / MKY) bug #412 "Issue with send panel".

			SetTerminalControls(); // Immediately set terminal controls so the terminal "looks nice" from the very start.

			ApplyFindAllIfActive();
		}

		private void ApplyFindAllIfActive()
		{
			if (this.settingsRoot.Find.AllIsActive)
			{
				var pattern = ApplicationSettings.RoamingUserSettings.Find.ActivePattern;
				var options = ApplicationSettings.RoamingUserSettings.Find.Options;

				monitor_Tx   .ActivateFindAll(pattern, options);
				monitor_Bidir.ActivateFindAll(pattern, options);
				monitor_Rx   .ActivateFindAll(pattern, options);
			}
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
			if (!IsInitiating && !IsIntegralMdiLayouting && !IsClosing)
				UpdateWindowSettings();
		}

		private void Terminal_SizeChanged(object sender, EventArgs e)
		{
			if (!IsInitiating && !IsIntegralMdiLayouting && !IsClosing)
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
			if (!IsInitiating && !IsIntegralMdiLayouting && !IsClosing)
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

		/// <remarks>
		/// Must manually be called by <see cref="Main"/> menu because the terminal's [File] menu is
		/// merged into the main's [File] menu! This seems to be a limitation of WinForms.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is an FxCop false-positive because the method is manually called as remarked above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "tool", Justification = "This is an FxCop false-positive because the method is manually called as remarked above.")]
		public void toolStripMenuItem_TerminalMenu_File_DropDownOpening(object sender, EventArgs e)
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
				if (TerminalIsAvailable) // which also means "SettingsRootIsAvailable".
				{
					toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = !this.terminal.IsStarted;
					toolStripMenuItem_TerminalMenu_Terminal_Stop .Enabled =  this.terminal.IsStarted;

					toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled =  this.terminal.IsSendingForSomeTime;
					toolStripMenuItem_TerminalMenu_Terminal_Clear.Enabled = (monitorIsDefined &&                     (this.findShortcutsCtrlFNPLSuspendedCount == 0)); // [Ctrl+L]

					if (this.settingsRoot.Layout.VisibleMonitorPanelCount <= 1)
					{
						toolStripMenuItem_TerminalMenu_Terminal_Clear  .Text = "C&lear";   // Indicating "All" for a single
						toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh"; //   panel would be confusing.
					}
					else
					{
						toolStripMenuItem_TerminalMenu_Terminal_Clear  .Text = "Cl&ear All";
						toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh All";
					}

					toolStripMenuItem_TerminalMenu_Terminal_SelectAll      .Enabled = (monitorIsDefined && textIsNotFocused && (this.editShortcutsCtrlACVDeleteSuspendedCount == 0)); // [Ctrl+A]
					toolStripMenuItem_TerminalMenu_Terminal_SelectNone     .Enabled = (monitorIsDefined && textIsNotFocused && (this.editShortcutsCtrlACVDeleteSuspendedCount == 0)); // [Ctrl+Delete]

					toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = (monitorIsDefined && textIsNotFocused && (this.editShortcutsCtrlACVDeleteSuspendedCount == 0)); // [Ctrl+C]
					toolStripMenuItem_TerminalMenu_Terminal_SaveToFile     .Enabled =  monitorIsDefined;
					toolStripMenuItem_TerminalMenu_Terminal_Print          .Enabled = (monitorIsDefined &&                     (this.findShortcutsCtrlFNPLSuspendedCount      == 0)); // [Ctrl+P]

					toolStripMenuItem_TerminalMenu_Terminal_FindNext       .Enabled = (monitorIsDefined && FindNextIsFeasible);
					toolStripMenuItem_TerminalMenu_Terminal_FindPrevious   .Enabled = (monitorIsDefined && FindPreviousIsFeasible);
					toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll  .Enabled = (this.settingsRoot.Find.AllIsActive ? true : (monitorIsDefined && FindAllIsFeasible));
					toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll  .Checked =  this.settingsRoot.Find.AllIsActive; // Similar code exists in View.Forms.Main.SetFindStateAndControls(). Changes here may have to be applied there too.
					toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll  .Text    = (this.settingsRoot.Find.AllIsActive ? "Deactivate" : "Activate") + " Find A&ll";
				}
				else
				{
					toolStripMenuItem_TerminalMenu_Terminal_Start.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Stop .Enabled = false;

					toolStripMenuItem_TerminalMenu_Terminal_Break.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Clear.Enabled = false;

					toolStripMenuItem_TerminalMenu_Terminal_Clear  .Text = "C&lear";   // Indicating "All" for a single
					toolStripMenuItem_TerminalMenu_Terminal_Refresh.Text = "&Refresh"; //   panel would be confusing.

					toolStripMenuItem_TerminalMenu_Terminal_SelectAll      .Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_SelectNone     .Enabled = false;

					toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_SaveToFile     .Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_Print          .Enabled = false;

					toolStripMenuItem_TerminalMenu_Terminal_FindNext       .Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_FindPrevious   .Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll  .Enabled = false;
					toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll  .Checked = false;
					toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll  .Text    = "Activate Find A&ll";
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Must manually be called by <see cref="Main"/> menu because the terminal's [File] menu is
		/// merged into the main's [File] menu! This seems to be a limitation of WinForms.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is an FxCop false-positive because the method is manually called as remarked above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "tool", Justification = "This is an FxCop false-positive because the method is manually called as remarked above.")]
		public void toolStripMenuItem_TerminalMenu_Terminal_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Start_Click(object sender, EventArgs e)
		{
			this.terminal.Start();
		}

		private void toolStripMenuItem_TerminalMenu_Terminal_Stop_Click(object sender, EventArgs e)
		{
			this.terminal.Stop();
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

		private void toolStripMenuItem_TerminalMenu_Terminal_ToggleFindAll_Click(object sender, EventArgs e)
		{
			RequestToggleFindAll();
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
				// Main and context menu are separated as there are subtle differences between them.
				//
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
				// ...View.Controls.SendText.SetSendControls()
				// Changes here may have to be applied there too.

				string sendTextText = "Text";
				bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText(this.settingsRoot.Send.Text.ToParseMode());
				if (this.settingsRoot.Send.Text.SendImmediately)
				{
					if (isTextTerminal)
						sendTextText = "EOL";
					else
						sendTextEnabled = false;
				}

				bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath(PathEx.GetDirectoryPath(this.terminal.SettingsFilePath));

				// Set the menu item properties:
				//
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
				// ...View.Forms.AdvancedTerminalSettings.SetControls()
				// Changes here may have to be applied there too.

				toolStripMenuItem_TerminalMenu_Send_Text.Text              =  sendTextText;
				toolStripMenuItem_TerminalMenu_Send_Text.Enabled           = (sendTextEnabled && this.terminal.IsReadyToSendForSomeTime); // Using 'ForSomeTime' reduces flickering.
				toolStripMenuItem_TerminalMenu_Send_TextWithoutEol.Enabled = (sendTextEnabled && this.terminal.IsReadyToSendForSomeTime && !this.settingsRoot.SendText.Command.IsMultiLineText && !this.settingsRoot.Send.Text.SendImmediately);
				toolStripMenuItem_TerminalMenu_Send_File.Enabled           = (sendFileEnabled && this.terminal.IsReadyToSendForSomeTime);

				toolStripMenuItem_TerminalMenu_Send_UseExplicitDefaultRadix.Checked = this.settingsRoot.Send.UseExplicitDefaultRadix;
				toolStripMenuItem_TerminalMenu_Send_AllowConcurrency.Checked        = this.settingsRoot.Send.AllowConcurrency;

				toolStripMenuItem_TerminalMenu_Send_KeepSendText.Enabled         =  !this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_KeepSendText.Checked         = (!this.settingsRoot.Send.Text.SendImmediately && this.settingsRoot.Send.Text.KeepSendText);
				toolStripMenuItem_TerminalMenu_Send_SendImmediately.Checked      =   this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Enabled =  !this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_TerminalMenu_Send_EnableEscapesForText.Checked = (!this.settingsRoot.Send.Text.SendImmediately && this.settingsRoot.Send.Text.EnableEscapes);

				toolStripMenuItem_TerminalMenu_Send_ExpandMultiLineText.Enabled  = this.settingsRoot.SendText.Command.IsMultiLineText;

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

				var tis = new List<AutoTriggerEx>(this.settingsRoot.GetValidAutoResponseTriggerItems());            // Common and recent patterns/triggers are always shown, though they
				tis.AddRange(AutoTriggerEx.CommonRegexCapturePatterns.Select(x => new AutoTriggerEx(x)).ToArray()); // could be limited depending on the options, but that is incomprehensive.
				tis.AddRange(ApplicationSettings.RoamingUserSettings.AutoResponse.RecentExplicitTriggers.ConvertAll(new Converter<RecentItem<string>, AutoTriggerEx>((x) => { return (x.Item); })));

				toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.Clear();
				toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Items.AddRange(tis.ToArray());
				var trigger = this.settingsRoot.AutoResponse.Trigger;
				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger, trigger, trigger);

				var triggerTextIsSupported  = trigger.TextIsSupported;
				var triggerRegexIsSupported = trigger.RegexIsSupported;

				var ris = new List<AutoResponseEx>(this.settingsRoot.GetValidAutoResponseItems(PathEx.GetDirectoryPath(this.terminal.SettingsFilePath)));
				ris.AddRange(AutoResponseEx.CommonRegexReplacementPatterns.Select(x => new AutoResponseEx(x)).ToArray());
				ris.AddRange(ApplicationSettings.RoamingUserSettings.AutoResponse.RecentExplicitResponses.ConvertAll(new Converter<RecentItem<string>, AutoResponseEx>((x) => { return (x.Item); })));

				toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.Clear();
				toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Items.AddRange(ris.ToArray());
				var response = this.settingsRoot.AutoResponse.Response;
				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Send_AutoResponse_Response, response, response);

				var responseReplaceIsSupported = response.ReplaceIsSupported;

				SetAutoResponseTriggerStateControls();
				SetAutoResponseResponseStateControls();

				SetAutoResponseTriggerOptionControls(triggerTextIsSupported, triggerRegexIsSupported);
				SetAutoResponseResponseOptionControls(triggerTextIsSupported, triggerRegexIsSupported, responseReplaceIsSupported);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Deactivate.Enabled = this.settingsRoot.AutoResponse.IsActive;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoResponseTriggerStateControls()
		{
			this.isSettingControls.Enter();
			try
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
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.autoResponseTriggerState + "' is an automatic content state that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoResponseResponseStateControls()
		{
			this.isSettingControls.Enter();
			try
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
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.autoResponseResponseState + "' is an automatic content state that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Required to allow/disallow changing options while editing a not yet validated trigger.
		/// The "TextChanged" event will allow, the 'Leave' event (for explicit triggers) or the
		/// "SelectedIndexChanged" event (for predefined triggers) will again disallow.
		/// </remarks>
		private void SetAutoResponseTriggerOptionControls(bool triggerTextIsSupported, bool triggerRegexIsSupported)
		{
			this.isSettingControls.Enter();
			try
			{
				var triggerUseText       = this.settingsRoot.AutoResponse.TriggerOptions.UseText;
				var triggerCaseSensitive = this.settingsRoot.AutoResponse.TriggerOptions.CaseSensitive;
				var triggerWholeWord     = this.settingsRoot.AutoResponse.TriggerOptions.WholeWord;
				var triggerEnableRegex   = this.settingsRoot.AutoResponse.TriggerOptions.EnableRegex;

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText      .Checked = (triggerTextIsSupported && triggerUseText);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText      .Enabled =  triggerTextIsSupported;

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Checked = (triggerTextIsSupported && triggerUseText && triggerCaseSensitive);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive.Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord    .Checked = (triggerTextIsSupported && triggerUseText && triggerWholeWord);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord    .Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex  .Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex  .Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ResetAutoResponseTriggerOptionControls()
		{
			var trigger = this.settingsRoot.AutoResponse.Trigger;
			SetAutoResponseTriggerOptionControls(trigger.TextIsSupported, trigger.RegexIsSupported);
		}

		/// <remarks>
		/// Required to allow/disallow changing options while editing a not yet validated trigger.
		/// The "TextChanged" event will allow, the 'Leave' event (for explicit triggers) or the
		/// "SelectedIndexChanged" event (for predefined triggers) will again disallow.
		/// </remarks>
		private void SetAutoResponseResponseOptionControls(bool triggerTextIsSupported, bool triggerRegexIsSupported, bool responseReplaceIsSupported)
		{
			this.isSettingControls.Enter();
			try
			{
				var triggerUseText        = this.settingsRoot.AutoResponse.TriggerOptions.UseText;
				var triggerEnableRegex    = this.settingsRoot.AutoResponse.TriggerOptions.EnableRegex;
				var responseEnableReplace = this.settingsRoot.AutoResponse.ResponseOptions.EnableReplace;

				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex && responseReplaceIsSupported && responseEnableReplace);
				toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace.Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex && responseReplaceIsSupported);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ResetAutoResponseResponseOptionControls()
		{
			var trigger  = this.settingsRoot.AutoResponse.Trigger;
			var response = this.settingsRoot.AutoResponse.Response;
			SetAutoResponseResponseOptionControls(trigger.TextIsSupported, trigger.RegexIsSupported, response.ReplaceIsSupported);
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

		/// <remarks>
		/// Note that the "SelectedIndexChanged" event is not raised when changing from a listed
		/// item to dedicated text, i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change must
		/// be handled in the "TextChanged" and/or "Enter/Leave" events.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
			{
			////if (RequestAutoResponseValidateTrigger(trigger)) is not needed (yet).
				{
					if (trigger.IsExplicit)
						RequestAutoResponseAdjustTriggerOptionsSilently(trigger);

					ActivateAutoResponseTrigger(trigger);
				}
			////else
			////{
			////	SetAutoResponseControls(); // Revert trigger.
			////}
			}
		}

		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_Enter(object sender, EventArgs e)
		{
			this.autoResponseTriggerValidationIsOngoing = false;
		}

		/// <remarks>
		/// Note that this 'Leave' event has a peculiar behavior:
		/// <list type="bullet">
		/// <item><description>On [Tab] jumping to the first option, the event is invoked immediately.</description></item>
		/// <item><description>Clicking another item of the toolbar, the event is not invoked!</description></item>
		/// <item><description>Clicking somewhere outside the toolbar, the event is invoked immediately.</description></item>
		/// </list>
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_Leave(object sender, EventArgs e)
		{
			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndex != ControlEx.InvalidIndex)
				ResetAutoResponseTriggerOptionControls();

			if (!this.autoResponseTriggerValidationIsOngoing) // Revalidation may already be ongoing triggered by clicking on option.
				RevalidateAndRequestAutoResponseTrigger();
		}

		/// <remarks>
		/// The "TextChanged" instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// Directly using the underlying <see cref="ToolStripComboBox.ComboBox"/>'es event doesn't help either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'es' relates to preceeding tag.")]
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex) // A new trigger is being added.
			{
				var triggerTextOrRegexPattern = toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text;

				RequestAutoResponseAdjustTriggerOptionsSilently(triggerTextOrRegexPattern); // Preset with best-effort manner.
				SetAutoResponseTriggerOptionControls(true, true); // Allow changing options while editing a not yet validated trigger!

				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					if (!RequestAutoResponseValidateTriggerTextSilently(triggerTextOrRegexPattern))
					{
						AutoResponseTriggerState = AutoContentState.Invalid;
						return; // Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).
				}
			}

			AutoResponseTriggerState = AutoContentState.Neutral;
		}

		/// <remarks>
		/// See remark at "TextChanged" event handler above.
		/// </remarks>
		private void RevalidateAndRequestAutoResponseTrigger()
		{
			var selectedIndex = toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedIndex;
			var selectedItem = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
			                  //// Not listed             or                            listed explicit tigger.
			if ((selectedIndex == ControlEx.InvalidIndex) || ((selectedItem != null) && selectedItem.IsExplicit))
			{
				var triggerTextOrRegexPattern = toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					int invalidTextStart;
					int invalidTextLength;

					this.autoResponseTriggerValidationIsOngoing = true;
					var success = RequestAutoResponseValidateTriggerText(triggerTextOrRegexPattern, out invalidTextStart, out invalidTextLength);
					this.autoResponseTriggerValidationIsOngoing = false;

					if (!success)
					{
						SetAutoResponseTriggerOptionControls(true, true); // Allow changing options while editing a not yet validated trigger!
						AutoResponseTriggerState = AutoContentState.Invalid;
						toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Focus();
						toolStripComboBox_TerminalMenu_Send_AutoResponse_Trigger.Select(invalidTextStart, invalidTextLength);
						return;
					}
				}

				ActivateAutoResponseTrigger(triggerTextOrRegexPattern);
			}

			AutoResponseTriggerState = AutoContentState.Neutral;
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_UseText_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerUseTextAndRevalidate();
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_CaseSensitive_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerCaseSensitiveAndRevalidate();
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_WholeWord_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerWholeWordAndRevalidate();
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Trigger_EnableRegex_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerEnableRegexAndRevalidate();
		}

		/// <remarks>
		/// Note that the "SelectedIndexChanged" event is not raised when changing from a listed
		/// item to dedicated text, i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change must
		/// be handled in the "TextChanged" and/or "Enter/Leave" events.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var response = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedItem as AutoResponseEx);
			if (response != null)
			{
			////if (RequestAutoResponseValidateResponse(response)) is not needed (yet).
				{
					if (response.IsExplicit)
						RequestAutoResponseAdjustResponseOptionsSilently(response);

					ActivateAutoResponseResponse(response);
				}
			////else
			////{
			////	SetAutoResponseChildControls(); // Revert resopnse.
			////}
			}
		}

		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_Enter(object sender, EventArgs e)
		{
			this.autoResponseResponseValidationIsOngoing = false;
		}

		/// <remarks>
		/// Note that this 'Leave' event has a peculiar behavior:
		/// <list type="bullet">
		/// <item><description>On [Tab] jumping to the first option, the event is invoked immediately.</description></item>
		/// <item><description>Clicking another item of the toolbar, the event is not invoked!</description></item>
		/// <item><description>Clicking somewhere outside the toolbar, the event is invoked immediately.</description></item>
		/// </list>
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_Leave(object sender, EventArgs e)
		{
			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndex != ControlEx.InvalidIndex)
				ResetAutoResponseResponseOptionControls();

			if (!this.autoResponseResponseValidationIsOngoing) // Revalidation may already be ongoing triggered by clicking on option.
				RevalidateAndRequestAutoResponseResponse();
		}

		/// <remarks>
		/// The "TextChanged" instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// Directly using the underlying <see cref="ToolStripComboBox.ComboBox"/>'es event doesn't help either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'es' relates to preceeding tag.")]
		private void toolStripComboBox_TerminalMenu_Send_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex) // A new response is being added.
			{
				var responseText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text;

				RequestAutoResponseAdjustResponseOptionsSilently(responseText); // Preset with best-effort manner.
				SetAutoResponseResponseOptionControls(true, true, true); // Allow changing options while editing a not yet validated response!

				if (!string.IsNullOrEmpty(responseText))
				{
					if (!RequestAutoResponseValidateResponseTextSilently(responseText))
					{
						AutoResponseResponseState = AutoContentState.Invalid;
						return; // Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).
				}
			}

			AutoResponseResponseState = AutoContentState.Neutral;
		}

		/// <remarks>
		/// See remark at "TextChanged" event handler above.
		/// </remarks>
		private void RevalidateAndRequestAutoResponseResponse()
		{
			var selectedIndex = toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedIndex;
			var selectedItem = (toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.SelectedItem as AutoTriggerEx);
			                  //// Not listed             or                            listed explicit response.
			if ((selectedIndex == ControlEx.InvalidIndex) || ((selectedItem != null) && selectedItem.IsExplicit))
			{
				var responseText = toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Text;
				if (!string.IsNullOrEmpty(responseText))
				{
					int invalidTextStart;
					int invalidTextLength;

					this.autoResponseResponseValidationIsOngoing = true;
					var success = RequestAutoResponseValidateResponseText(responseText, out invalidTextStart, out invalidTextLength);
					this.autoResponseResponseValidationIsOngoing = false;

					if (!success)
					{
						SetAutoResponseResponseOptionControls(true, true, true); // Allow changing options while editing a not yet validated trigger!
						AutoResponseResponseState = AutoContentState.Invalid;
						toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Focus();
						toolStripComboBox_TerminalMenu_Send_AutoResponse_Response.Select(invalidTextStart, invalidTextLength);
						return;
					}
				}

				ActivateAutoResponseResponse(responseText);
			}

			AutoResponseResponseState = AutoContentState.Neutral;
		}

		private void toolStripMenuItem_TerminalMenu_Send_AutoResponse_Response_EnableReplace_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseResponseEnableReplaceAndRevalidate();
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
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_SetMenuItems(); // See remark of that method.

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
		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_SetMenuItems()
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

				var tis = new List<AutoTriggerEx>(this.settingsRoot.GetValidAutoActionTriggerItems());              // Common and recent patterns/triggers are always shown, though they
				tis.AddRange(AutoTriggerEx.CommonRegexCapturePatterns.Select(x => new AutoTriggerEx(x)).ToArray()); // could be limited depending on the options, but that is incomprehensive.
				tis.AddRange(ApplicationSettings.RoamingUserSettings.AutoAction.RecentExplicitTriggers.ConvertAll(new Converter<RecentItem<string>, AutoTriggerEx>((x) => { return (x.Item); })));

				toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Items.Clear();
				toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Items.AddRange(tis.ToArray());
				var trigger = this.settingsRoot.AutoAction.Trigger;
				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger, trigger, trigger);

				var triggerTextIsSupported  = trigger.TextIsSupported;
				var triggerRegexIsSupported = trigger.RegexIsSupported;

			////var ais = new List<AutoActionEx>(this.settingsRoot.GetValidAutoActionItems()); is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
			////ais.AddRange(AutoActionEx.CommonActions.Select(x => new AutoActionEx(x)).ToArray());
			////ais.AddRange(ApplicationSettings.RoamingUserSettings.AutoAction.RecentExplicitActions.ConvertAll(new Converter<RecentItem<string>, AutoActionEx>((x) => { return (x.Item); })));

				toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Items.Clear();
				toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.Items.AddRange(this.settingsRoot.GetValidAutoActionItems());
				var action = this.settingsRoot.AutoAction.Action;
				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_Receive_AutoAction_Action, action, action);

				SetAutoActionTriggerStateControls();
			////SetAutoActionActionStateControls() is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

				SetAutoActionTriggerOptionControls(triggerTextIsSupported, triggerRegexIsSupported);
			////SetAutoActionActionOptionControls() is not needed (yet) because there are no such options (yet).

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Deactivate.Enabled = this.settingsRoot.AutoAction.IsActive;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoActionTriggerStateControls()
		{
			this.isSettingControls.Enter();
			try
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
						throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.autoActionTriggerState + "' is an automatic content state that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Required to allow/disallow changing options while editing a not yet validated trigger.
		/// The "TextChanged" event will allow, the 'Leave' event (for explicit triggers) or the
		/// "SelectedIndexChanged" event (for predefined triggers) will again disallow.
		/// </remarks>
		private void SetAutoActionTriggerOptionControls(bool triggerTextIsSupported, bool triggerRegexIsSupported)
		{
			this.isSettingControls.Enter();
			try
			{
				var triggerUseText       = this.settingsRoot.AutoAction.TriggerOptions.UseText;
				var triggerCaseSensitive = this.settingsRoot.AutoAction.TriggerOptions.CaseSensitive;
				var triggerWholeWord     = this.settingsRoot.AutoAction.TriggerOptions.WholeWord;
				var triggerEnableRegex   = this.settingsRoot.AutoAction.TriggerOptions.EnableRegex;

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText      .Checked = (triggerTextIsSupported && triggerUseText);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText      .Enabled =  triggerTextIsSupported;

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Checked = (triggerTextIsSupported && triggerUseText && triggerCaseSensitive);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive.Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord    .Checked = (triggerTextIsSupported && triggerUseText && triggerWholeWord);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord    .Enabled = (triggerTextIsSupported && triggerUseText);

				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex  .Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex);
				toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex  .Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ResetAutoActionTriggerOptionControls()
		{
			var trigger = this.settingsRoot.AutoAction.Trigger;
			SetAutoActionTriggerOptionControls(trigger.TextIsSupported, trigger.RegexIsSupported);
		}

		private void toolStripMenuItem_TerminalMenu_Receive_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Receive_SetMenuItems();
		}

		/// <remarks>
		/// Note that the "SelectedIndexChanged" event is not raised when changing from a listed
		/// item to dedicated text, i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change must
		/// be handled in the "TextChanged" and/or "Enter/Leave" events.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
			{
				if (RequestAutoActionValidateTrigger(trigger))
				{
					if (trigger.IsExplicit)
						RequestAutoActionAdjustTriggerOptionsSilently(trigger);

					ActivateAutoActionTrigger(trigger);
				}
				else
				{
					SetAutoActionControls(); // Revert trigger.
				}
			}
		}

		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_Enter(object sender, EventArgs e)
		{
			this.autoActionTriggerValidationIsOngoing = false;
		}

		/// <remarks>
		/// Note that this 'Leave' event has a peculiar behavior:
		/// <list type="bullet">
		/// <item><description>On [Tab] jumping to the first option, the event is invoked immediately.</description></item>
		/// <item><description>Clicking another item of the toolbar, the event is not invoked!</description></item>
		/// <item><description>Clicking somewhere outside the toolbar, the event is invoked immediately.</description></item>
		/// </list>
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_Leave(object sender, EventArgs e)
		{
			if (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedIndex != ControlEx.InvalidIndex)
				ResetAutoActionTriggerOptionControls();

			if (!this.autoActionTriggerValidationIsOngoing) // Revalidation may already be ongoing triggered by clicking on option.
				RevalidateAndRequestAutoActionTrigger();
		}

		/// <remarks>
		/// The "TextChanged" instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// Directly using the underlying <see cref="ToolStripComboBox.ComboBox"/>'es event doesn't help either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'es' relates to preceeding tag.")]
		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedIndex == ControlEx.InvalidIndex) // A new trigger is being added.
			{
				var triggerTextOrRegexPattern = toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Text;

				RequestAutoActionAdjustTriggerOptionsSilently(triggerTextOrRegexPattern); // Preset with best-effort manner.
				SetAutoActionTriggerOptionControls(true, true); // Allow changing options while editing a not yet validated trigger!

				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					if (!RequestAutoActionValidateTriggerTextSilently(triggerTextOrRegexPattern))
					{
						AutoActionTriggerState = AutoContentState.Invalid;
						return; // Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).
				}
			}

			AutoActionTriggerState = AutoContentState.Neutral;
		}

		/// <remarks>
		/// See remark at "TextChanged" event handler above.
		/// </remarks>
		private void RevalidateAndRequestAutoActionTrigger()
		{
			var selectedIndex = toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedIndex;
			var selectedItem = (toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.SelectedItem as AutoTriggerEx);
			                  //// Not listed             or                            listed explicit tigger.
			if ((selectedIndex == ControlEx.InvalidIndex) || ((selectedItem != null) && selectedItem.IsExplicit))
			{
				var triggerTextOrRegexPattern = toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					int invalidTextStart;
					int invalidTextLength;

					this.autoActionTriggerValidationIsOngoing = true;
					var success = RequestAutoActionValidateTriggerText(triggerTextOrRegexPattern, out invalidTextStart, out invalidTextLength);
					this.autoActionTriggerValidationIsOngoing = false;

					if (!success)
					{
						SetAutoActionTriggerOptionControls(true, true); // Allow changing options while editing a not yet validated trigger!
						AutoActionTriggerState = AutoContentState.Invalid;
						toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Focus();
						toolStripComboBox_TerminalMenu_Receive_AutoAction_Trigger.Select(invalidTextStart, invalidTextLength);
						return;
					}
				}

				ActivateAutoActionTrigger(triggerTextOrRegexPattern);
			}

			AutoActionTriggerState = AutoContentState.Neutral;
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_UseText_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerUseTextAndRevalidate();
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_CaseSensitive_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerCaseSensitiveAndRevalidate();
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_WholeWord_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerWholeWordAndRevalidate();
		}

		private void toolStripMenuItem_TerminalMenu_Receive_AutoAction_Trigger_EnableRegex_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerEnableRegexAndRevalidate();
		}

		/// <remarks>
		/// Note that the "SelectedIndexChanged" event is not raised when changing from a listed
		/// item to dedicated text, i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change must
		/// be handled in the "TextChanged" and/or "Enter/Leave" events.
		/// </remarks>
		private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Action_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var action = (toolStripComboBox_TerminalMenu_Receive_AutoAction_Action.SelectedItem as AutoActionEx);
			if (action != null)
			{
				if (RequestAutoActionValidateAction(action))
					ActivateAutoActionAction(action);
				else
					SetAutoActionControls(); // Revert action.
			}
		}

	////private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Action_Enter(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Action_Leave(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private void toolStripComboBox_TerminalMenu_Receive_AutoAction_Action_TextChanged(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

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

		/// <remarks>
		/// Must manually be called by <see cref="Main"/> menu because the terminal's [File] menu is
		/// merged into the main's [File] menu! This seems to be a limitation of WinForms.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = "This is an FxCop false-positive because the method is manually called as remarked above.")]
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "tool", Justification = "This is an FxCop false-positive because the method is manually called as remarked above.")]
		public void toolStripMenuItem_TerminalMenu_Log_DropDownOpening(object sender, EventArgs e)
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
				// Attention:
				// Similar code exists in contextMenuStrip_Monitor_SetMenuItems() further below!
				// Changes here must most likely be applied there too.

				bool isText       = true;
				bool isSerialPort = true;

				if (this.settingsRoot != null)
				{
					isText       = (this.settingsRoot.TerminalType == Domain.TerminalType.Text);
					isSerialPort = (this.settingsRoot.IOType       == Domain.IOType.SerialPort);
				}

				toolStripMenuItem_TerminalMenu_View_Panels_Tx   .Checked = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
				toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Checked = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
				toolStripMenuItem_TerminalMenu_View_Panels_Rx   .Checked = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

				// A panel must not be hideable if it is the only one:
				toolStripMenuItem_TerminalMenu_View_Panels_Tx   .Enabled = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
				toolStripMenuItem_TerminalMenu_View_Panels_Bidir.Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
				toolStripMenuItem_TerminalMenu_View_Panels_Rx   .Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.BidirMonitorPanelIsVisible);

				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_View_Panels_MonitorOrientation, (OrientationEx)this.settingsRoot.Layout.MonitorOrientation);

				toolStripMenuItem_TerminalMenu_View_Panels_Predefined.Checked = this.settingsRoot.Layout.PredefinedPanelIsVisible;

				ToolStripComboBoxHelper.Select(toolStripComboBox_TerminalMenu_View_Panels_PageLayout, (PredefinedCommandPageLayoutEx)this.settingsRoot.PredefinedCommand.PageLayout);

				toolStripMenuItem_TerminalMenu_View_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
				toolStripMenuItem_TerminalMenu_View_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

				// Connect time:
				bool showConnectTime = this.settingsRoot.Status.ShowConnectTime;
				toolStripMenuItem_TerminalMenu_View_ConnectTime_Show.Checked  = showConnectTime;
				toolStripMenuItem_TerminalMenu_View_ConnectTime_Reset.Enabled = showConnectTime;

				// Counters:
				bool showCountAndRate = this.settingsRoot.Status.ShowCountAndRate;
				toolStripMenuItem_TerminalMenu_View_CountAndRate_Show.Checked  = showCountAndRate;
				toolStripMenuItem_TerminalMenu_View_CountAndRate_Reset.Enabled = showCountAndRate;

				// Display:
				bool isShowable = (this.settingsRoot.Display.TxRadixIsShowable ||
				                   this.settingsRoot.Display.RxRadixIsShowable);
				toolStripMenuItem_TerminalMenu_View_ShowRadix.Enabled =  isShowable; // Attention: This 'isShowable' restriction also exists further below as well as in 'View.Forms.AdvancedTerminalSettings'.
				toolStripMenuItem_TerminalMenu_View_ShowRadix.Checked = (isShowable && this.settingsRoot.Display.ShowRadix);

				var lineNumberSelection = this.settingsRoot.Display.LineNumberSelection;
				string showLineNumbersText = "&Show Line Numbers (" + (Domain.Utilities.LineNumberSelectionEx)lineNumberSelection + ")";
				bool resetLineNumbersEnabled = ((lineNumberSelection != Domain.Utilities.LineNumberSelection.Buffer) && (MaxMonitorLineNumberOffset > 0));
				toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Text     = showLineNumbersText;
				toolStripMenuItem_TerminalMenu_View_ShowLineNumbers.Checked  = this.settingsRoot.Display.ShowLineNumbers;
				toolStripMenuItem_TerminalMenu_View_ResetLineNumbers.Enabled = resetLineNumbersEnabled;

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

		private void toolStripMenuItem_TerminalMenu_View_ConnectTime_Show_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowConnectTime = !this.settingsRoot.Status.ShowConnectTime;
		}

		private void toolStripMenuItem_TerminalMenu_View_ConnectTime_Reset_Click(object sender, EventArgs e)
		{
			this.terminal.ResetConnectTime();
		}

		private void toolStripMenuItem_TerminalMenu_View_CountAndRate_Show_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Status.ShowCountAndRate = !this.settingsRoot.Status.ShowCountAndRate;
		}

		private void toolStripMenuItem_TerminalMenu_View_CountAndRate_Reset_Click(object sender, EventArgs e)
		{
			this.terminal.ResetCountAndRate();

			// The "terminal_IOCount/RateChanged_Promptly" events are not used because of the reason
			// described in the remarks of "terminal_RawChunkSent/Received" of "Model.Terminal".
			// Instead, the update is done by the "terminal_DisplayElements[Tx|Bidir|Rx]Added" and
			// "terminal_DisplayLines[Tx|Bidir|Rx]Added" handlers. As a consequence, the update must
			// manually be triggered here:

			SetDataStatus();
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowRadix_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowRadix = !this.settingsRoot.Display.ShowRadix;
		}

		private void toolStripMenuItem_TerminalMenu_View_ShowLineNumbers_Click(object sender, EventArgs e)
		{
			this.settingsRoot.Display.ShowLineNumbers = !this.settingsRoot.Display.ShowLineNumbers;
		}

		private void toolStripMenuItem_TerminalMenu_View_ResetLineNumbers_Click(object sender, EventArgs e)
		{
			ResetMonitorLineNumbers();
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
			// Similar code exists in View.Forms.AdvancedTerminalSetting.checkBox_ShowDevice_CheckedChanged()!
			// Changes here must most likely be applied there too.

			if (toolStripMenuItem_TerminalMenu_View_ShowDevice.Checked && !this.settingsRoot.Display.DeviceLineBreakEnabled)
			{
				var isServerSocket = this.settingsRoot.IO.IOTypeIsServerSocket;
				if (isServerSocket) // Attention: This "isServerSocket" restriction is also implemented at other locations!
				{
					var dr = MessageBoxEx.Show
					(
						this,
						"To enable this setting, lines must be broken when I/O device changes.",
						"Incompatible Settings",
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
				// Attention:
				// Similar code exists in toolStripMenuItem_TerminalMenu_View_SetMenuItems() further above!
				// Changes here must most likely be applied there too.

				var terminalType = this.settingsRoot.TerminalType;
				var monitorType = GetMonitorType(contextMenuStrip_Monitor.SourceControl);
				var isMonitor = (monitorType != Domain.RepositoryType.None);

				toolStripMenuItem_MonitorContextMenu_Panels_Tx   .Checked = this.settingsRoot.Layout.TxMonitorPanelIsVisible;
				toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Checked = this.settingsRoot.Layout.BidirMonitorPanelIsVisible;
				toolStripMenuItem_MonitorContextMenu_Panels_Rx   .Checked = this.settingsRoot.Layout.RxMonitorPanelIsVisible;

				// A panel must not be hideable if it is the only one:
				toolStripMenuItem_MonitorContextMenu_Panels_Tx   .Enabled = (this.settingsRoot.Layout.BidirMonitorPanelIsVisible || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
				toolStripMenuItem_MonitorContextMenu_Panels_Bidir.Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.RxMonitorPanelIsVisible);
				toolStripMenuItem_MonitorContextMenu_Panels_Rx   .Enabled = (this.settingsRoot.Layout.TxMonitorPanelIsVisible    || this.settingsRoot.Layout.BidirMonitorPanelIsVisible);

				ToolStripComboBoxHelper.Select(toolStripComboBox_MonitorContextMenu_Panels_Orientation, (OrientationEx)this.settingsRoot.Layout.MonitorOrientation);

				// In order to not necessarily having to use the main menu (longer mouse path), non-monitor panels shall also be manageable in the monitor context menu:
				toolStripMenuItem_MonitorContextMenu_Panels_Predefined.Checked = this.settingsRoot.Layout.PredefinedPanelIsVisible;

				toolStripMenuItem_MonitorContextMenu_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
				toolStripMenuItem_MonitorContextMenu_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

				bool isShowable = (this.settingsRoot.Display.TxRadixIsShowable ||
				                   this.settingsRoot.Display.RxRadixIsShowable);
				toolStripMenuItem_MonitorContextMenu_ShowRadix.Enabled =  isShowable; // Attention: This 'isShowable' restriction also exists further above as well as in 'View.Forms.AdvancedTerminalSettings'.
				toolStripMenuItem_MonitorContextMenu_ShowRadix.Checked = (isShowable && this.settingsRoot.Display.ShowRadix);

				var lineNumberSelection = this.settingsRoot.Display.LineNumberSelection;
				string showLineNumbersText = "Show Line Numbers (" + (Domain.Utilities.LineNumberSelectionEx)lineNumberSelection + ")";
				bool resetLineNumbersEnabled = ((lineNumberSelection != Domain.Utilities.LineNumberSelection.Buffer) && (MaxMonitorLineNumberOffset > 0));
				toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Text     = showLineNumbersText;
				toolStripMenuItem_MonitorContextMenu_ShowLineNumbers.Checked  = this.settingsRoot.Display.ShowLineNumbers;
				toolStripMenuItem_MonitorContextMenu_ResetLineNumbers.Enabled = resetLineNumbersEnabled;

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
				toolStripMenuItem_MonitorContextMenu_ResetCountAndRate.Enabled = showCountAndRate;

				toolStripMenuItem_MonitorContextMenu_Clear.Enabled = isMonitor;

				toolStripMenuItem_MonitorContextMenu_SelectAll.Enabled  = isMonitor;
				toolStripMenuItem_MonitorContextMenu_SelectNone.Enabled = isMonitor;

				toolStripMenuItem_MonitorContextMenu_SaveToFile.Enabled      = isMonitor;
				toolStripMenuItem_MonitorContextMenu_CopyToClipboard.Enabled = isMonitor;
				toolStripMenuItem_MonitorContextMenu_Print.Enabled           = isMonitor;
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

		private void toolStripMenuItem_MonitorContextMenu_Panels_Predefined_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.PredefinedPanelIsVisible = !this.settingsRoot.Layout.PredefinedPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_SendText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.SendTextPanelIsVisible = !this.settingsRoot.Layout.SendTextPanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_SendFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.Layout.SendFilePanelIsVisible = !this.settingsRoot.Layout.SendFilePanelIsVisible;
		}

		private void toolStripMenuItem_MonitorContextMenu_Panels_Rearrange_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			ViewRearrange();
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

		private void toolStripMenuItem_MonitorContextMenu_ResetCountAndRate_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.terminal.ResetCountAndRate();

			// The "terminal_IOCount/RateChanged_Promptly" events are not used because of the reason
			// described in the remarks of "terminal_RawChunkSent/Received" of "Model.Terminal".
			// Instead, the update is done by the "terminal_DisplayElements[Tx|Bidir|Rx]Added" and
			// "terminal_DisplayLines[Tx|Bidir|Rx]Added" handlers. As a consequence, the update must
			// manually be triggered here:

			SetDataStatus();
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

		private void toolStripMenuItem_MonitorContextMenu_ResetLineNumbers_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			ResetMonitorLineNumbers();
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

			SetMonitorRadix_SameTxRx(Domain.Radix.String);
		}

		private void toolStripMenuItem_RadixContextMenu_Char_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix_SameTxRx(Domain.Radix.Char);
		}

		private void toolStripMenuItem_RadixContextMenu_Bin_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix_SameTxRx(Domain.Radix.Bin);
		}

		private void toolStripMenuItem_RadixContextMenu_Oct_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix_SameTxRx(Domain.Radix.Oct);
		}

		private void toolStripMenuItem_RadixContextMenu_Dec_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix_SameTxRx(Domain.Radix.Dec);
		}

		private void toolStripMenuItem_RadixContextMenu_Hex_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix_SameTxRx(Domain.Radix.Hex);
		}

		private void toolStripMenuItem_RadixContextMenu_Unicode_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetMonitorRadix_SameTxRx(Domain.Radix.Unicode);
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

				int id = 0;
				Command c = null;
				bool cIsDefined = false;

				if (predefined.TryGetCommandIdFromLocation(Cursor.Position, out id))
				{
					if (predefined.TryGetCommandFromId(id, out c))
						cIsDefined = c.IsDefined;
				}

				PredefinedCommandPageLayoutEx pageLayoutEx = this.settingsRoot.PredefinedCommand.PageLayout;
				var np = pageLayoutEx.CommandCapacityPerPage;

				contextMenuStrip_Predefined_SelectedCommandId = id;

				toolStripMenuItem_PredefinedContextMenu_Panels_Predefined.Checked = this.settingsRoot.Layout.PredefinedPanelIsVisible;

				ToolStripComboBoxHelper.Select(toolStripComboBox_PredefinedContextMenu_Layout, pageLayoutEx);

				toolStripMenuItem_PredefinedContextMenu_CopyFromSendText.Enabled = ((id != 0) && (this.settingsRoot.SendText.Command != null) && (this.settingsRoot.SendText.Command.IsText));
				toolStripMenuItem_PredefinedContextMenu_CopyFromSendFile.Enabled = ((id != 0) && (this.settingsRoot.SendFile.Command != null) && (this.settingsRoot.SendFile.Command.IsFilePath));

				var mi = toolStripMenuItem_PredefinedContextMenu_CopyToSendTextOrFile;
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

				// There is a limitation in Windows.Forms:
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

				toolStripMenuItem_PredefinedContextMenu_Cut               .Enabled = cIsDefined;
				toolStripMenuItem_PredefinedContextMenu_Copy              .Enabled = cIsDefined;
			////toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath.Enabled = cIsDefined is handled below.
				toolStripMenuItem_PredefinedContextMenu_Paste             .Enabled = ((id != 0) && (CommandSettingsClipboardHelper.ClipboardContainsText));
				toolStripMenuItem_PredefinedContextMenu_Clear             .Enabled = cIsDefined;

				mi = toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath;
				if (c != null)
				{
					mi.Enabled = (c.IsText || c.IsFilePath);
					if (c.IsText)
						mi.Text = "Copy Text to Clipboard";
					else if (c.IsFilePath)
						mi.Text = "Copy File Path to Clipboard";
					else
						mi.Text = "Copy Text or File Path to Clipboard";
				}
				else
				{
					mi.Enabled = false;
					mi.Text = "Copy Text or File Path to Clipboard";
				}

				var hasPages = (this.settingsRoot.PredefinedCommand.Pages.Count >= 1);
				toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard.Enabled = hasPages;
				toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile   .Enabled = hasPages;

				if (this.settingsRoot.PredefinedCommand.Pages.Count <= 1)
				{
					toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard    .Text = "Copy Page to Clipboard...";
				////toolStripMenuItem_PredefinedContextMenu_ImportPagesFromClipboard.Text = "Paste Page(s) from Clipboard..." is fixed.
					toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile       .Text = "Export Page to File...";
				////toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile     .Text = "Import Page(s) from File..." is fixed.
				////toolStripMenuItem_PredefinedContextMenu_LinkPageToFile          .Text = "Link Page to File..." is fixed.
				}
				else
				{
					toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard    .Text = "Copy Page(s) to Clipboard...";
				////toolStripMenuItem_PredefinedContextMenu_ImportPagesFromClipboard.Text = "Paste Page(s) from Clipboard..." is fixed.
					toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile       .Text = "Export Page(s) to File...";
				////toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile     .Text = "Import Page(s) from File..." is fixed.
				////toolStripMenuItem_PredefinedContextMenu_LinkPageToFile          .Text = "Link Page to File..." is fixed.
				}

				if (this.settingsRoot.PredefinedCommand.HideUndefinedCommands)
					toolStripMenuItem_PredefinedContextMenu_HideShowUndefined.Text = "Show Undefined Commands";
				else
					toolStripMenuItem_PredefinedContextMenu_HideShowUndefined.Text = "Hide Undefined Commands";
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

			Command c;
			if (predefined.TryGetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId, out c))
			{
				c = new Command(c); // Clone to ensure decoupling.
				if (c.IsText)
					this.settingsRoot.SendText.Command = c;
				else if (c.IsFilePath)
					this.settingsRoot.SendFile.Command = c;
				else
					throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + @"Invalid command """ + c.ToDiagnosticsString() + @"""!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

			Command c;
			if (predefined.TryGetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId, out c))
			{
				c = new Command(c); // Clone to ensure decoupling.               // Replace target by selected:
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, targetCommandIndex, c);
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

			Command c;
			if (predefined.TryGetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId, out c))
			{
				this.settingsRoot.PredefinedCommand.SuspendChangeEvent();
				try
				{
					c = new Command(c); // Clone to ensure decoupling.               // Replace target by selected:
					this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, targetCommandIndex, c); // Clear selected:
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

			Command sc = null; // s = source
			if (predefined.TryGetCommandFromId(selectedCommandId, out sc))
				sc = new Command(sc); // Clone to ensure decoupling.

			var targetCommandId = ((selectedCommandId > PredefinedCommandPage.FirstCommandIdPerPage) ? (selectedCommandId - 1) : (lastCommandIdPerPage));

			Command tc = null; // t = target
			if (predefined.TryGetCommandFromId(targetCommandId, out tc))
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

			Command sc = null; // s = source
			if (predefined.TryGetCommandFromId(selectedCommandId, out sc))
				sc = new Command(sc); // Clone to ensure decoupling.

			var targetCommandId = ((selectedCommandId < lastCommandIdPerPage) ? (selectedCommandId + 1) : (PredefinedCommandPage.FirstCommandIdPerPage));

			Command tc = null; // t = target
			if (predefined.TryGetCommandFromId(targetCommandId, out tc))
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

			Command c;
			if (predefined.TryGetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId, out c))
			{
				SetFixedStatusText("Preparing cutting to clipboard...");
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case cutting takes long.
				SetFixedStatusText("Cutting to clipboard...");
				if (CommandSettingsClipboardHelper.TrySet(this, c))
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

			Command c;
			if (predefined.TryGetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId, out c))
			{
				SetFixedStatusText("Preparing copying to clipboard...");
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case copying takes long.
				SetFixedStatusText("Copying to clipboard...");
				if (CommandSettingsClipboardHelper.TrySet(this, c))
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

		private void toolStripMenuItem_PredefinedContextMenu_CopyTextOrFilePath_Click(object sender, EventArgs e)
		{
			Command c;
			if (predefined.TryGetCommandFromId(contextMenuStrip_Predefined_SelectedCommandId, out c))
			{
				SetFixedStatusText("Preparing copying to clipboard...");
				Cursor = Cursors.WaitCursor;
				Clipboard.Clear(); // Prevent handling errors in case copying takes long.
				SetFixedStatusText("Copying to clipboard...");
				if (CommandSettingsClipboardHelper.TrySetTextOrFilePath(this, c))
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
			Command c;
			SetFixedStatusText("Pasting from clipboard..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			if (CommandSettingsClipboardHelper.TryGet(this, out c))
			{
				this.settingsRoot.PredefinedCommand.SetCommand(predefined.SelectedPageIndex, contextMenuStrip_Predefined_SelectedCommandId - 1, c);
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

		private void toolStripMenuItem_PredefinedContextMenu_HideShowUndefined_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.PredefinedCommand.HideUndefinedCommands = !this.settingsRoot.PredefinedCommand.HideUndefinedCommands;
		}

		// While the purpose of
		// ...toolStripComboBox_PredefinedContextMenu_Page...
		// ...is questionable in the 'Predefined' context menu, it is there as kind of title for the items below.

		private void toolStripMenuItem_PredefinedContextMenu_CopyPagesToClipboard_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Copying to clipboard..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			if (CommandPagesSettingsClipboardHelper.TrySet(this, this.settingsRoot.PredefinedCommand, predefined.SelectedPageId))
				SetTimedStatusText("Copying to clipboard done");
			else
				SetFixedStatusText("Copying to clipboard failed!");
		}

		private void toolStripMenuItem_PredefinedContextMenu_PastePagesFromClipboard_Click(object sender, EventArgs e)
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

		private void toolStripMenuItem_PredefinedContextMenu_ExportPagesToFile_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			SetFixedStatusText("Exporting to file..."); // Do not set Cursor = Cursors.WaitCursor as that would result in WaitCursor on MessageBox!
			if (CommandPagesSettingsFileHelper.TryExport(this, this.settingsRoot.PredefinedCommand, predefined.SelectedPageId, IndicatedName))
				SetTimedStatusText("Exporting to file done");
			else
				SetFixedStatusText("Exporting to file failed!");
		}

		private void toolStripMenuItem_PredefinedContextMenu_ImportPagesFromFile_Click(object sender, EventArgs e)
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

		private void toolStripMenuItem_PredefinedContextMenu_LinkPageToFile_Click(object sender, EventArgs e)
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
								this.menuItems_Commands[i].ForeColor = SystemColors.ControlText;  // Improves because "ForeColor" is managed by a "PropertyStore".
							                                                   //// Time consuming operation! See "FontEx.DefaultFontItalic" for background!
							if (this.menuItems_Commands[i].Font != SystemFonts.DefaultFont) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].Font = SystemFonts.DefaultFont;  // Improves because "Font" is managed by a "PropertyStore".

							this.menuItems_Commands[i].Text = MenuEx.PrependIndex(i + 1, commands[i].Description);
						}
						                               //// Using 'ForSomeTime' reduces flickering.
						bool isValid = (this.terminal.IsReadyToSendForSomeTime && commands[i].IsValid(this.settingsRoot.Send.Text.ToParseMode(), this.terminal.SettingsFilePath));
						this.menuItems_Commands[i].Enabled = isValid;
					}
					else
					{
						if (updateAppearance)
						{
							if (this.menuItems_Commands[i].ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].ForeColor = SystemColors.GrayText;  // Improves because "ForeColor" is managed by a "PropertyStore".
							                                              //// Time consuming operation! See "FontEx.DefaultFontItalic" for background!
							if (this.menuItems_Commands[i].Font != FontEx.DefaultFontItalic) // Improve performance by only assigning if different.
								this.menuItems_Commands[i].Font = FontEx.DefaultFontItalic;  // Improves because "Font" is managed by a "PropertyStore".

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
				// Context and main menu are separated as there are subtle differences between them.
				//
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
				// ...View.Controls.SendText.SetSendControls()
				// Changes here may have to be applied there too.

				string sendTextText = "Send Text";
				bool sendTextEnabled = this.settingsRoot.SendText.Command.IsValidText(this.settingsRoot.Send.Text.ToParseMode());
				if (this.settingsRoot.Send.Text.SendImmediately)
				{
					if (isTextTerminal)
						sendTextText = "Send EOL";
					else
						sendTextEnabled = false;
				}

				bool sendFileEnabled = this.settingsRoot.SendFile.Command.IsValidFilePath(PathEx.GetDirectoryPath(this.terminal.SettingsFilePath));

				// Set the menu item properties
				//
				// Attention:
				// Similar code exists in...
				// ...View.Forms.Terminal.contextMenuStrip_Send_SetMenuItems()
				// ...View.Forms.AdvancedTerminalSettings.SetControls()
				// Changes here may have to be applied there too.

				toolStripMenuItem_SendContextMenu_Panels_SendText.Checked = this.settingsRoot.Layout.SendTextPanelIsVisible;
				toolStripMenuItem_SendContextMenu_Panels_SendFile.Checked = this.settingsRoot.Layout.SendFilePanelIsVisible;

				toolStripMenuItem_SendContextMenu_SendText.Text              =  sendTextText;
				toolStripMenuItem_SendContextMenu_SendText.Enabled           = (sendTextEnabled && this.terminal.IsReadyToSendForSomeTime); // Using 'ForSomeTime' reduces flickering.
				toolStripMenuItem_SendContextMenu_SendTextWithoutEol.Enabled = (sendTextEnabled && this.terminal.IsReadyToSendForSomeTime && !this.settingsRoot.SendText.Command.IsMultiLineText && !this.settingsRoot.Send.Text.SendImmediately);
				toolStripMenuItem_SendContextMenu_SendFile.Enabled           = (sendFileEnabled && this.terminal.IsReadyToSendForSomeTime);

				toolStripMenuItem_SendContextMenu_CopyTextToClipboard    .Enabled = sendTextEnabled;
				toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard.Enabled = sendFileEnabled;

				toolStripMenuItem_SendContextMenu_Clear_CurrentText    .Enabled = sendTextEnabled;
				toolStripMenuItem_SendContextMenu_Clear_RecentTexts    .Enabled = sendTextEnabled;
				toolStripMenuItem_SendContextMenu_Clear_CurrentFilePath.Enabled = sendFileEnabled;
				toolStripMenuItem_SendContextMenu_Clear_RecentFilePaths.Enabled = sendFileEnabled;

				toolStripMenuItem_SendContextMenu_UseExplicitDefaultRadix.Checked = this.settingsRoot.Send.UseExplicitDefaultRadix;
				toolStripMenuItem_SendContextMenu_AllowConcurrency.Checked        = this.settingsRoot.Send.AllowConcurrency;

				toolStripMenuItem_SendContextMenu_KeepSendText.Enabled         =  !this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_SendContextMenu_KeepSendText.Checked         = (!this.settingsRoot.Send.Text.SendImmediately && this.settingsRoot.Send.Text.KeepSendText);
				toolStripMenuItem_SendContextMenu_SendImmediately.Checked      =   this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_SendContextMenu_EnableEscapesForText.Enabled =  !this.settingsRoot.Send.Text.SendImmediately;
				toolStripMenuItem_SendContextMenu_EnableEscapesForText.Checked = (!this.settingsRoot.Send.Text.SendImmediately && this.settingsRoot.Send.Text.EnableEscapes);

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

		private void toolStripMenuItem_SendContextMenu_CopyTextToClipboard_Click(object sender, EventArgs e)
		{
			SetFixedStatusText("Preparing copying to clipboard...");
			Cursor = Cursors.WaitCursor;
			Clipboard.Clear(); // Prevent handling errors in case copying takes long.
			SetFixedStatusText("Copying text to clipboard...");
			if (CommandSettingsClipboardHelper.TrySetTextOrFilePath(this, send.TextCommand)) // No need for a dedicated TrySetText() method.
			{
				Cursor = Cursors.Default;
				SetTimedStatusText("Copying text to clipboard done");
			}
			else
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Copying text to clipboard failed!");
			}
		}

		private void toolStripMenuItem_SendContextMenu_CopyFilePathToClipboard_Click(object sender, EventArgs e)
		{
			SetFixedStatusText("Preparing copying to clipboard...");
			Cursor = Cursors.WaitCursor;
			Clipboard.Clear(); // Prevent handling errors in case copying takes long.
			SetFixedStatusText("Copying file path to clipboard...");
			if (CommandSettingsClipboardHelper.TrySetTextOrFilePath(this, send.FileCommand)) // No need for a dedicated TrySetFilePath() method.
			{
				Cursor = Cursors.Default;
				SetTimedStatusText("Copying file path to clipboard done");
			}
			else
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Copying file path to clipboard failed!");
			}
		}

		private void toolStripMenuItem_SendContextMenu_Clear_CurrentText_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.SendText.ClearCommand();
		}

		private void toolStripMenuItem_SendContextMenu_Clear_RecentTexts_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.SendText.ClearRecentCommands();
		}

		private void toolStripMenuItem_SendContextMenu_Clear_CurrentFilePath_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.SendFile.ClearCommand();
		}

		private void toolStripMenuItem_SendContextMenu_Clear_RecentFilePaths_Click(object sender, EventArgs e)
		{
			if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
				return;

			this.settingsRoot.SendFile.ClearRecentCommands();
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
			if (!IsInitiating && !this.isSettingControls && !IsIntegralMdiLayouting && !IsClosing)
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
			if (!IsInitiating && !this.isSettingControls && !IsIntegralMdiLayouting && !IsClosing)
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
			if (!IsInitiating && !this.isSettingControls && !IsIntegralMdiLayouting && !IsClosing)
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
			this.lastMonitorSelection = Domain.RepositoryType.Tx;
		}

		private void monitor_Bidir_Enter(object sender, EventArgs e)
		{
			this.lastMonitorSelection = Domain.RepositoryType.Bidir;
		}

		private void monitor_Rx_Enter(object sender, EventArgs e)
		{
			this.lastMonitorSelection = Domain.RepositoryType.Rx;
		}

		private void monitor_TextFocusedChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems(); // Ensure the edit shortcuts such as [Ctrl+A] and [Ctrl+C] are disabled while the send control is being edited.
		}

		private void monitor_SelectedLinesChanged(object sender, EventArgs<int> e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems(); // Ensure the find shortcuts [Alt+Shift+N/P/L] are enabled when a single line is selected.
		}

		private void monitor_FindItemStateChanged(object sender, EventArgs e)
		{
			toolStripMenuItem_TerminalMenu_Terminal_SetMenuItems(); // Ensure the find shortcuts [*Modifiers*+F] are enabled/disabled properly.
		}

		private void monitor_Tx_FindAllSuccessChanged(object sender, EventArgs<bool> e)
		{
			if (!this.settingsRoot.Layout.BidirMonitorPanelIsVisible) // Bidir monitor has precedence.
				OnFindAllSuccessChanged(e);
		}

		private void monitor_Bidir_FindAllSuccessChanged(object sender, EventArgs<bool> e)
		{
			OnFindAllSuccessChanged(e); // Bidir has precedence.
		}

		private void monitor_Rx_FindAllSuccessChanged(object sender, EventArgs<bool> e)
		{
			if (!this.settingsRoot.Layout.BidirMonitorPanelIsVisible && !this.settingsRoot.Layout.TxMonitorPanelIsVisible) // Bidir and Tx monitors have precedence.
				OnFindAllSuccessChanged(e);
		}

		private void monitor_Tx_FindAllDeactivatedWithinMonitor(object sender, EventArgs e)
		{
			if (this.settingsRoot.Find.AllIsActive)
			{
				this.settingsRoot.Find.AllIsActive = false;

				monitor_Bidir.DeactivateFindAll();
				monitor_Rx   .DeactivateFindAll();
			}
		}

		private void monitor_Bidir_FindAllDeactivatedWithinMonitor(object sender, EventArgs e)
		{
			if (this.settingsRoot.Find.AllIsActive)
			{
				this.settingsRoot.Find.AllIsActive = false;

				monitor_Tx   .DeactivateFindAll();
				monitor_Rx   .DeactivateFindAll();
			}
		}

		private void monitor_Rx_FindAllDeactivatedWithinMonitor(object sender, EventArgs e)
		{
			if (this.settingsRoot.Find.AllIsActive)
			{
				this.settingsRoot.Find.AllIsActive = false;

				monitor_Tx   .DeactivateFindAll();
				monitor_Bidir.DeactivateFindAll();
			}
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

		private void send_SendTextCommandRequest(object sender, EventArgs<SendTextOption> e)
		{
			if (this.terminal != null)
			{
				switch (e.Value)
				{
					case SendTextOption.Normal:     this.terminal.SendText();           break;
					case SendTextOption.WithoutEol: this.terminal.SendTextWithoutEol(); break;

					default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + e.Value.ToString() + "' is an option that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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

			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_Separator2);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_RTS);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_CTS);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_DTR);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_DSR);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_DCD);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_Separator3);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_InputXOnXOff);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_OutputXOnXOff);
			this.terminalStatusLabels.Add(toolStripStatusLabel_TerminalStatus_Separator4);
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
		/// The indicated terminal name. The name is either an incrementally assigned "Terminal1",
		/// "Terminal2",... or the file name once the terminal has been saved by the user, e.g.
		/// "MyTerminal.yat".
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

		/// <remarks>
		/// Using term "initiating" for distinction with "initialize", "create" and "load" which are
		/// already occupied by the .NET WinForms terminology.
		/// </remarks>
		/// <remarks>
		/// Property for orthogonality with <see cref="IsClosing"/>.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		private bool IsInitiating
		{
			get { return (this.isInitiating); }
		}

		private bool IsIntegralMdiLayouting
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
				return (this.terminal.Start());

			return (false);
		}

		/// <summary></summary>
		public virtual bool RequestStopTerminal()
		{
			if (this.terminal != null)
				return (this.terminal.Stop());

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
			SetMonitorRadix_SameTxRx(radix);
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
		protected virtual bool TryGetFindTextFromMonitorButOnlyIfNotInFind(out string text)
		{
			if (this.findShortcutsCtrlFNPLSuspendedCount == 0) // Do not preset/overwrite when already in [Find]!
			{
				var monitor = GetMonitor(this.lastMonitorSelection);
				if (monitor != null)
				{
					var selectedLines = monitor.SelectedLines;
					if (selectedLines.Count == 1)
					{
						text = selectedLines[0].ContentText;
						return (true);
					}

					var selectedText = monitor.SelectedTextInCopyOfActiveLine;
					if (!string.IsNullOrEmpty(selectedText))
					{
						text = selectedText;
						return (true);
					}
				}
			}

			text = null;
			return (false);
		}

		/// <summary></summary>
		protected virtual void RequestFind()
		{
			string text = null;
			TryGetFindTextFromMonitorButOnlyIfNotInFind(out text); // Will stay "null" if no text is available or if in find.

			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestFind(text);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "MDI 'Terminal' requires that MDI parent is 'Main'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual bool FindNextIsFeasible
		{
			get
			{
				string text;
				if (TryGetFindTextFromMonitorButOnlyIfNotInFind(out text))
					return (true);

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
				string text;
				if (TryGetFindTextFromMonitorButOnlyIfNotInFind(out text))
					return (true);

				var main = (this.mdiParent as Main);
				if (main != null)
					return (main.FindPreviousIsFeasible);
				else
					return (false);
			}
		}

		/// <remarks>
		/// Not name "ToggleFindAll..." as it shall only indicate activation.
		/// </remarks>
		protected virtual bool FindAllIsFeasible
		{
			get
			{
				string text;
				if (TryGetFindTextFromMonitorButOnlyIfNotInFind(out text))
					return (true);

				var main = (this.mdiParent as Main);
				if (main != null)
					return (main.FindAllIsFeasible);
				else
					return (false);
			}
		}

		/// <summary></summary>
		protected virtual void RequestFindNext()
		{
			string text = null;
			TryGetFindTextFromMonitorButOnlyIfNotInFind(out text); // Will stay "null" if no text is available or if in find.

			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestFindNext(text);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "MDI 'Terminal' requires that MDI parent is 'Main'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void RequestFindPrevious()
		{
			string text = null;
			TryGetFindTextFromMonitorButOnlyIfNotInFind(out text); // Will stay "null" if no text is available or if in find.

			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestFindPrevious(text);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "MDI 'Terminal' requires that MDI parent is 'Main'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		protected virtual void RequestToggleFindAll()
		{
			string text = null;
			TryGetFindTextFromMonitorButOnlyIfNotInFind(out text); // Will stay "null" if no text is available or if in find.

			var main = (this.mdiParent as Main);
			if (main != null)
				main.RequestToggleFindAll(text);
			else
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "MDI 'Terminal' requires that MDI parent is 'Main'!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
		}

		/// <summary></summary>
		public virtual void EmptyFindOnEdit()
		{
			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = "";
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			monitor.EmptyFindOnEdit();
		}

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		public virtual void LeaveFindOnEdit(string pattern)
		{
			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.SaveRoamingUserSettings();

			var monitor = GetMonitor(this.lastMonitorSelection);
			monitor.LeaveFindOnEdit();
		}

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual FindResult TryFindOnEdit(string pattern, out FindDirection resultingDirection)
		{
			// The active pattern seems to not have to be saved each time, it is saved on LeaveFindOnEdit().
			// But, when anything else changes (e.g. find options, or the terminal is closed/opened) the pattern would get reset.

			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
		////ApplicationSettings.RoamingUserSettings.Find.RecentPatterns.Add(new RecentItem<string>(pattern)); // But "OnEdit" shall not update the recents,
		////ApplicationSettings.RoamingUserSettings.Find.SetChanged();                                        // thus no manual change is required.
			ApplicationSettings.SaveRoamingUserSettings();

			var options = ApplicationSettings.RoamingUserSettings.Find.Options;

			if (!FindAllIsActive) // "Normal" case:
			{
				var monitor = GetMonitor(this.lastMonitorSelection);
				if (monitor.TryFindOnEdit(pattern, options, out resultingDirection))
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
			else // FindAllIsActive:
			{
				var success = true;
				success |= monitor_Tx   .TryFindOnEdit(pattern, options, out resultingDirection);
				success |= monitor_Bidir.TryFindOnEdit(pattern, options, out resultingDirection);
				success |= monitor_Rx   .TryFindOnEdit(pattern, options, out resultingDirection);
				if (success)
				{
					this.lastFindPattern = pattern;

					return (FindResult.Found);
				}
				else
				{
					return (FindResult.NotFoundAtAll);
				}
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

			var options = ApplicationSettings.RoamingUserSettings.Find.Options;

			var monitor = GetMonitor(this.lastMonitorSelection);
			if (monitor.TryFindNext(pattern, options))
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

			var options = ApplicationSettings.RoamingUserSettings.Find.Options;

			var monitor = GetMonitor(this.lastMonitorSelection);
			if (monitor.TryFindPrevious(pattern, options))
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
		public virtual FindResult ActivateFindAll(string pattern)
		{
			this.settingsRoot.Find.AllIsActive = true;

			// The active pattern wouldn't have to be saved each time, it is saved on LeaveFindOnEdit() anyway.
			// But, the recent has to be saved each time, as the time stamp changes. Thus, saving both anyway.

			ApplicationSettings.RoamingUserSettings.Find.ActivePattern = pattern;
			ApplicationSettings.RoamingUserSettings.Find.RecentPatterns.Add(new RecentItem<string>(pattern));
			ApplicationSettings.RoamingUserSettings.Find.SetChanged(); // Manual change required because underlying collection is modified.
			ApplicationSettings.SaveRoamingUserSettings();

			var options = ApplicationSettings.RoamingUserSettings.Find.Options;

			var success = true;  // Activate in all monitors in any case, monitor could be made visible later.
			success |= monitor_Tx   .ActivateFindAll(pattern, options);
			success |= monitor_Bidir.ActivateFindAll(pattern, options);
			success |= monitor_Rx   .ActivateFindAll(pattern, options);
			if (success)
			{
				this.lastFindPattern = pattern;

				return (FindResult.Found);
			}
			else
			{
				return (FindResult.NotFoundAtAll);
			}
		}

		/// <summary></summary>
		public virtual void DeactivateFindAll()
		{
			if (this.settingsRoot.Find.AllIsActive) // Required to prevent subsequent deactivation on next/previous.
			{
				this.settingsRoot.Find.AllIsActive = false;

				monitor_Tx   .DeactivateFindAll();
				monitor_Bidir.DeactivateFindAll();
				monitor_Rx   .DeactivateFindAll();
			}
		}

		/// <summary></summary>
		public virtual bool FindAllIsActive
		{
			get
			{
				if (this.settingsRoot != null) // Such simple get-property shall also be available on e.g. closing.
					return (this.settingsRoot.Find.AllIsActive);
				else
					return (false);
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

		/// <summary></summary>
		public virtual bool RequestAutoActionValidateTrigger(AutoTriggerEx trigger)
		{
			var text = new StringBuilder();

			switch ((AutoAction)(this.settingsRoot.AutoAction.Action))
			{
				case AutoAction.Highlight:
				{
					if (trigger == AutoTrigger.AnyLine)
					{
						text.AppendLine("Trigger cannot be set to [Any Line] when action is [Highlight].");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being highlighted.");
					}
					break;
				}

				case AutoAction.Suppress:
				{
					if (trigger == AutoTrigger.AnyLine)
					{
						text.AppendLine("Trigger cannot be set to [Any Line] when action is [Suppress].");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being suppressed.");
					}
					break;
				}

				case AutoAction.LineChartIndex:
				case AutoAction.LineChartTime:
				case AutoAction.LineChartTimeStamp:
				case AutoAction.ScatterPlot:
				case AutoAction.HistogramHorizontal:
				case AutoAction.HistogramVertical:
				{
					if (!trigger.RegexIsSupported)
					{
						text.Append    ("Trigger cannot be set to ");
						text.Append    (trigger);
						text.AppendLine(" because [Line/Scatter/Histogram] require that trigger is based on a regular expression.");
						text.AppendLine();
						text.Append    (@"Reason: Regular expression captures (e.g. ""([-+]?\d+)"") are used to retrieve the values to plot.");
					}
					break;
				}

				default:
				{
					// Nothing to do.
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

				return (false);
			}
			else
			{
				return (true);
			}
		}

		/// <remarks>
		/// Always succeeds with the (yet) available options, no need to revalidate (yet).
		/// </remarks>
		/// <remarks>
		/// There is a limitation yet: This method is only called by the "SelectedIndexChanged" event
		/// handlers. But that event is not raised when changing from a listed item to dedicated text,
		/// i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change would have to be handled in the
		/// "TextChanged" and/or "Enter/Leave" events, but that would interfere with option settings
		/// explicitly done by the user. Accepting this limitation.
		/// </remarks>
		/// <remarks>
		/// Could also be located in <see cref="Model.Terminal"/>.
		/// </remarks>
		public virtual void RequestAutoActionAdjustTriggerOptionsSilently(AutoTriggerEx trigger)
		{
			if (trigger.IsExplicit)
			{
				var options = this.settingsRoot.AutoAction.TriggerOptions;

				if (!ValidationHelper.ValidateTextSilently(trigger, Domain.Parser.Mode.RadixAndAsciiEscapes)) {
					options.UseText = true;

					if (!RegexEx.TryValidatePattern(trigger)) {
						options.EnableRegex = false;
					}
					else if (RegexEx.LikelyContainsAnyPattern(trigger)) {
						options.EnableRegex = true;
					}
				}

				this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
				{                                // Settings member must be changed to let the changed event be raised!
					this.settingsRoot.AutoAction.TriggerOptions = options;
				}
				this.settingsRoot.AutoAction.ResumeChangeEvent(false); // Event will be raised on revalidation or request.
			}
		}

		/// <remarks>Could also be located in <see cref="Model.Terminal"/>.</remarks>
		public virtual bool RequestAutoActionValidateTriggerTextSilently(string triggerTextOrRegexPattern)
		{
			if (!this.settingsRoot.AutoAction.TriggerOptions.UseText)
			{
				return (ValidationHelper.ValidateTextSilently(triggerTextOrRegexPattern, Domain.Parser.Mode.RadixAndAsciiEscapes));
			}
			else                                          // UseText:
			{
				if (!this.settingsRoot.AutoAction.TriggerOptions.EnableRegex) // "CaseSensitive" and "WholeWord" are irrelevant for validation.
					return (!string.IsNullOrEmpty(triggerTextOrRegexPattern));
				else                                          // EnableRegex:
					return (RegexEx.TryValidatePattern(triggerTextOrRegexPattern));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool RequestAutoActionValidateTriggerText(string triggerTextOrRegexPattern, out int invalidTextStart, out int invalidTextLength)
		{
			if (!this.settingsRoot.AutoAction.TriggerOptions.UseText)
			{
				return (ValidationHelper.ValidateText(this, "trigger", triggerTextOrRegexPattern, out invalidTextStart, out invalidTextLength, Domain.Parser.Mode.RadixAndAsciiEscapes));
			}
			else                                          // UseText:
			{
				invalidTextStart = 0; // Not way to detect this (yet).
				invalidTextLength = triggerTextOrRegexPattern.Length;

				if (!this.settingsRoot.AutoAction.TriggerOptions.EnableRegex) // "CaseSensitive" and "WholeWord" are irrelevant for validation.
					return (!string.IsNullOrEmpty(triggerTextOrRegexPattern));
				else                                          // EnableRegex:
					return (RegexEx.TryValidatePattern(triggerTextOrRegexPattern));
			}
		}

		/// <summary></summary>
		public virtual void ActivateAutoActionTrigger(AutoTriggerEx trigger)
		{
			this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for trigger and/or previously changed options.
			{
				this.settingsRoot.AutoAction.Trigger = trigger;
			}
			this.settingsRoot.AutoAction.ResumeChangeEvent(); // Event will be raised for trigger and/or previously changed options.

			if (trigger.IsExplicit && !AutoTriggerEx.CommonRegexCapturePatterns.Contains(trigger))
			{
				ApplicationSettings.RoamingUserSettings.AutoAction.RecentExplicitTriggers.Add(new RecentItem<string>(trigger));
				ApplicationSettings.RoamingUserSettings.AutoAction.SetChanged(); // Manual change required because underlying collection is modified.
				ApplicationSettings.SaveRoamingUserSettings();
			}
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerUseTextAndRevalidate()
		{
			RequestToggleAutoActionTriggerUseText();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionTriggerUseText()
		{
			this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoAction.TriggerOptions;
				options.UseText = !options.UseText; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoAction.TriggerOptions = options;
			}
			this.settingsRoot.AutoAction.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerCaseSensitiveAndRevalidate()
		{
			RequestToggleAutoActionTriggerCaseSensitive();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionTriggerCaseSensitive()
		{
			this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoAction.TriggerOptions;
				options.CaseSensitive = !options.CaseSensitive; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoAction.TriggerOptions = options;
			}
			this.settingsRoot.AutoAction.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerWholeWordAndRevalidate()
		{
			RequestToggleAutoActionTriggerWholeWord();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionTriggerWholeWord()
		{
			this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoAction.TriggerOptions;
				options.WholeWord = !options.WholeWord; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoAction.TriggerOptions = options;
			}
			this.settingsRoot.AutoAction.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerEnableRegexAndRevalidate()
		{
			RequestToggleAutoActionTriggerEnableRegex();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoActionTriggerEnableRegex()
		{
			this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoAction.TriggerOptions;
				options.EnableRegex = !options.EnableRegex; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoAction.TriggerOptions = options;
			}
			this.settingsRoot.AutoAction.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

	////public virtual AutoContentState AutoActionTriggerState is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

		/// <summary></summary>
		public virtual bool RequestAutoActionValidateAction(AutoActionEx action)
		{
			var text = new StringBuilder();

			switch ((AutoAction)action)
			{
				case AutoAction.Highlight:
				{
					if (this.settingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine)
					{
						text.AppendLine("Action cannot be set to [Highlight] when trigger is [Any Line]!");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being highlighted.");
					}
					break;
				}

				case AutoAction.Suppress:
				{
					if (this.settingsRoot.AutoAction.Trigger == AutoTrigger.AnyLine)
					{
						text.AppendLine("Action cannot be set to [Suppress] when trigger is [Any Line]!");
						text.AppendLine();
						text.Append    ("Reason: Such action would result in all received lines being suppressed.");
					}
					break;
				}

				case AutoAction.LineChartIndex:
				case AutoAction.LineChartTime:
				case AutoAction.LineChartTimeStamp:
				case AutoAction.ScatterPlot:
				case AutoAction.HistogramHorizontal:
				case AutoAction.HistogramVertical:
				{
					if (!this.settingsRoot.AutoAction.Trigger.RegexIsSupported)
					{
						text.Append    ("Action cannot be set to ");
						text.Append    (action);
						text.AppendLine(" when trigger is not based on a regular expression.");
						text.AppendLine();
						text.Append    (@"Reason: Regular expression captures (e.g. ""([-+]?\d+)"") are used to retrieve the values to plot.");
					}
					break;
				}

				default: // Accept change of action:
				{
					// Nothing to do.
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

				return (false);
			}
			else
			{
				return (true);
			}
		}

		/// <summary></summary>
		public virtual void ActivateAutoActionAction(AutoActionEx action)
		{
			this.settingsRoot.AutoAction.SuspendChangeEvent(); // Prevent duplicate events for trigger and/or previously changed options.
			{
				this.settingsRoot.AutoAction.Action = action;
			}
			this.settingsRoot.AutoAction.ResumeChangeEvent(); // Event will be raised for trigger and/or previously changed options.
		}

		/// <summary></summary>
		public virtual void RequestAutoActionResetCount()
		{
			if (this.terminal != null)
				this.terminal.ResetAutoActionCount();
		}

		/// <summary></summary>
		public virtual void RequestAutoActionSuspend()
		{
			if (this.terminal != null)
				this.terminal.SuspendAutoAction();
		}

		/// <summary></summary>
		public virtual void RequestAutoActionResume()
		{
			if (this.terminal != null)
				this.terminal.ResumeAutoAction();
		}

		/// <summary></summary>
		public virtual void RequestAutoActionDeactivate()
		{
			AutoActionTriggerState = AutoContentState.Neutral;
		////AutoActionActionState  = AutoContentState.Neutral; is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

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

	////public virtual bool RequestAutoResponseValidateTrigger(AutoTriggerEx trigger) is not needed (yet)

		/// <remarks>
		/// Always succeeds with the (yet) available options, no need to revalidate (yet).
		/// </remarks>
		/// <remarks>
		/// There is a limitation yet: This method is only called by the "SelectedIndexChanged" event
		/// handlers. But that event is not raised when changing from a listed item to dedicated text,
		/// i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change would have to be handled in the
		/// "TextChanged" and/or "Enter/Leave" events, but that would interfere with option settings
		/// explicitly done by the user. Accepting this limitation.
		/// </remarks>
		/// <remarks>
		/// Could also be located in <see cref="Model.Terminal"/>.
		/// </remarks>
		public virtual void RequestAutoResponseAdjustTriggerOptionsSilently(AutoTriggerEx trigger)
		{
			if (trigger.IsExplicit)
			{
				var options = this.settingsRoot.AutoResponse.TriggerOptions;

				if (!ValidationHelper.ValidateTextSilently(trigger, Domain.Parser.Mode.RadixAndAsciiEscapes)) {
					options.UseText = true;

					if (!RegexEx.TryValidatePattern(trigger)) {
						options.EnableRegex = false;
					}
					else if (RegexEx.LikelyContainsAnyPattern(trigger)) {
						options.EnableRegex = true;
					}
				}

				this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
				{                                  // Settings member must be changed to let the changed event be raised!
					this.settingsRoot.AutoResponse.TriggerOptions = options;
				}
				this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation or request.
			}
		}

		/// <remarks>Could also be located in <see cref="Model.Terminal"/>.</remarks>
		public virtual bool RequestAutoResponseValidateTriggerTextSilently(string triggerTextOrRegexPattern)
		{
			if (!this.settingsRoot.AutoResponse.TriggerOptions.UseText)
			{
				return (ValidationHelper.ValidateTextSilently(triggerTextOrRegexPattern, Domain.Parser.Mode.RadixAndAsciiEscapes));
			}
			else                                            // UseText:
			{
				if (!this.settingsRoot.AutoResponse.TriggerOptions.EnableRegex) // "CaseSensitive" and "WholeWord" are irrelevant for validation.
					return (!string.IsNullOrEmpty(triggerTextOrRegexPattern));
				else                                            // EnableRegex:
					return (RegexEx.TryValidatePattern(triggerTextOrRegexPattern));
			}
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool RequestAutoResponseValidateTriggerText(string triggerTextOrRegexPattern, out int invalidTextStart, out int invalidTextLength)
		{
			if (!this.settingsRoot.AutoResponse.TriggerOptions.UseText)
			{
				return (ValidationHelper.ValidateText(this, "trigger", triggerTextOrRegexPattern, out invalidTextStart, out invalidTextLength, Domain.Parser.Mode.RadixAndAsciiEscapes));
			}
			else                                            // UseText:
			{
				invalidTextStart = 0; // Not way to detect this (yet).
				invalidTextLength = triggerTextOrRegexPattern.Length;

				if (!this.settingsRoot.AutoResponse.TriggerOptions.EnableRegex) // "CaseSensitive" and "WholeWord" are irrelevant for validation.
					return (!string.IsNullOrEmpty(triggerTextOrRegexPattern));
				else                                            // EnableRegex:
					return (RegexEx.TryValidatePattern(triggerTextOrRegexPattern));
			}
		}

		/// <summary></summary>
		public virtual void ActivateAutoResponseTrigger(AutoTriggerEx trigger)
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for trigger and/or previously changed options.
			{
				this.settingsRoot.AutoResponse.Trigger = trigger;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(); // Event will be raised for trigger and/or previously changed options.

			if (trigger.IsExplicit && !AutoTriggerEx.CommonRegexCapturePatterns.Contains(trigger))
			{
				ApplicationSettings.RoamingUserSettings.AutoResponse.RecentExplicitTriggers.Add(new RecentItem<string>(trigger));
				ApplicationSettings.RoamingUserSettings.AutoResponse.SetChanged(); // Manual change required because underlying collection is modified.
				ApplicationSettings.SaveRoamingUserSettings();
			}
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerUseTextAndRevalidate()
		{
			RequestToggleAutoResponseTriggerUseText();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerUseText()
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoResponse.TriggerOptions;
				options.UseText = !options.UseText; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoResponse.TriggerOptions = options;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerCaseSensitiveAndRevalidate()
		{
			RequestToggleAutoResponseTriggerCaseSensitive();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerCaseSensitive()
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoResponse.TriggerOptions;
				options.CaseSensitive = !options.CaseSensitive; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoResponse.TriggerOptions = options;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerWholeWordAndRevalidate()
		{
			RequestToggleAutoResponseTriggerWholeWord();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerWholeWord()
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoResponse.TriggerOptions;
				options.WholeWord = !options.WholeWord; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoResponse.TriggerOptions = options;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation.
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerEnableRegexAndRevalidate()
		{
			RequestToggleAutoResponseTriggerEnableRegex();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseTriggerEnableRegex()
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and trigger.
			{
				var options = this.settingsRoot.AutoResponse.TriggerOptions;
				options.EnableRegex = !options.EnableRegex; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoResponse.TriggerOptions = options;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation.
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

	////public virtual bool RequestAutoResponseValidateResponse(AutoResponseEx response) is not needed (yet)

		/// <remarks>
		/// Always succeeds with the (yet) available options, no need to revalidate (yet).
		/// </remarks>
		/// <remarks>
		/// There is a limitation yet: This method is only called by the "SelectedIndexChanged" event
		/// handlers. But that event is not raised when changing from a listed item to dedicated text,
		/// i.e. to <see cref="ControlEx.InvalidIndex"/>. Such change would have to be handled in the
		/// "TextChanged" and/or "Enter/Leave" events, but that would interfere with option settings
		/// explicitly done by the user. Accepting this limitation.
		/// </remarks>
		/// <remarks>
		/// Could also be located in <see cref="Model.Terminal"/>.
		/// </remarks>
		public virtual void RequestAutoResponseAdjustResponseOptionsSilently(AutoResponseEx response)
		{
			if (response.IsExplicit)
			{
				var options = this.settingsRoot.AutoResponse.ResponseOptions;

				var m = Regex.Match(response, @"\$\d");
				if (m.Success)
					options.EnableReplace = true;

				this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and response.
				{                                  // Settings member must be changed to let the changed event be raised!
					this.settingsRoot.AutoResponse.ResponseOptions = options;
				}
				this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation or request.
			}
		}

		/// <remarks>Could also be located in <see cref="Model.Terminal"/>.</remarks>
		public virtual bool RequestAutoResponseValidateResponseTextSilently(string responseText)
		{
			return (ValidationHelper.ValidateTextSilently(responseText, Domain.Parser.Mode.RadixAndAsciiEscapes));
		}

		/// <summary></summary>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Multiple return values are required, and 'out' is preferred to 'ref'.")]
		public virtual bool RequestAutoResponseValidateResponseText(string responseText, out int invalidTextStart, out int invalidTextLength)
		{
			return (ValidationHelper.ValidateText(this, "response", responseText,  out invalidTextStart, out invalidTextLength, Domain.Parser.Mode.RadixAndAsciiEscapes));
		}

		/// <summary></summary>
		public virtual void ActivateAutoResponseResponse(AutoResponseEx response)
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for response and/or previously changed options.
			{
				this.settingsRoot.AutoResponse.Response = response;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(); // Event will be raised for response and/or previously changed options.

			if (response.IsExplicit && !AutoResponseEx.CommonRegexReplacementPatterns.Contains(response))
			{
				ApplicationSettings.RoamingUserSettings.AutoResponse.RecentExplicitResponses.Add(new RecentItem<string>(response));
				ApplicationSettings.RoamingUserSettings.AutoResponse.SetChanged(); // Manual change required because underlying collection is modified.
				ApplicationSettings.SaveRoamingUserSettings();
			}
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseResponseEnableReplaceAndRevalidate()
		{
			RequestToggleAutoResponseResponseEnableReplace();
			RevalidateAndRequestAutoResponseResponse();
		}

		/// <summary></summary>
		public virtual void RequestToggleAutoResponseResponseEnableReplace()
		{
			this.settingsRoot.AutoResponse.SuspendChangeEvent(); // Prevent duplicate events for options and response.
			{
				var options = this.settingsRoot.AutoResponse.ResponseOptions;
				options.EnableReplace = !options.EnableReplace; // Settings member must be changed to let the changed event be raised!
				this.settingsRoot.AutoResponse.ResponseOptions = options;
			}
			this.settingsRoot.AutoResponse.ResumeChangeEvent(false); // Event will be raised on revalidation.
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
			AutoResponseTriggerState  = AutoContentState.Neutral;
			AutoResponseResponseState = AutoContentState.Neutral;

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

		[Conditional("DEBUG")]
		private void DebugDecorateControls()
		{
			var terminalCaption = "Terminal #" + this.terminal.SequentialId.ToString("D2", CultureInfo.CurrentCulture);

			monitor_Tx   .DebugCaption = "[Tx]    of " + terminalCaption;
			monitor_Bidir.DebugCaption = "[Bidir] of " + terminalCaption;
			monitor_Rx   .DebugCaption = "[Rx]    of " + terminalCaption;

			// Will result in e.g.
			//  @ 12:34:56.789 @ Thread #001 : YAT.View.Controls.Monitor [Bidir] of Terminal #01 : ClearSelected() @ fastListBox_Monitor_Leave
		}

		private void ApplyWindowSettings()
		{
			WindowState = this.settingsRoot.Window.State;
			if (WindowState == FormWindowState.Normal)
			{
				SuspendLayout(); // Useful as "Size" and "Location" will get changed.
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

		private void PerformSplitContainerScalingWorkaround()
		{
			this.isSettingControls.Enter();
			try
			{
				this.splitContainerHelper.PerformScaling(this);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void LayoutTerminal()
		{
			SuspendLayout(); // Useful as "Size" and "Location" will get changed.
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
					#if (DEBUG)
						else
						{
							Debugger.Break(); // See debug output for issue and potential root cause.
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
						#if (DEBUG)
							else
							{
								Debugger.Break(); // See debug output for issue and potential root cause.
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
						#if (DEBUG)
							else
							{
								Debugger.Break(); // See debug output for issue and potential root cause.
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

						default: throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "'" + this.lastMonitorSelection + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
				#if (DEBUG)
					else
					{
						Debugger.Break(); // See debug output for issue and potential root cause.
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
			send.SendSplitterDistance = Int32Ex.Limit(relativeX, 0, Math.Max(0, (send.Width - 1))); // "max" must be 0 or above.
		}

		private void ViewRearrange()
		{
			// Simply set defaults, settings event handler will then call LayoutTerminal():
			this.settingsRoot.Layout.SetDefaults();
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
				predefined.ParseModeForText                 = this.settingsRoot.Send.Text.ToParseMode();
				predefined.RootDirectoryForFile             = PathEx.GetDirectoryPath(this.terminal.SettingsFilePath);
				predefined.TerminalIsReadyToSendForSomeTime = this.terminal.IsReadyToSendForSomeTime; // Using 'ForSomeTime' reduces flickering.
			}
			finally
			{
				predefined.ResumeCommandStateUpdate();
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoActionControls()
		{
			toolStripMenuItem_TerminalMenu_Receive_AutoAction_SetMenuItems();

			this.isSettingControls.Enter();
			try
			{
				timer_AutoActionCountUpdate.Enabled = this.settingsRoot.AutoAction.IsActive;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoResponseControls()
		{
			toolStripMenuItem_TerminalMenu_Send_AutoResponse_SetMenuItems();

			this.isSettingControls.Enter();
			try
			{
				timer_AutoResponseCountUpdate.Enabled = this.settingsRoot.AutoResponse.IsActive;
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
				send.TextCommand                      = this.settingsRoot.SendText.Command;
				send.RecentTextCommands               = this.settingsRoot.SendText.RecentCommands;
				send.FileCommand                      = this.settingsRoot.SendFile.Command;
				send.RecentFileCommands               = this.settingsRoot.SendFile.RecentCommands;
				send.TerminalType                     = this.settingsRoot.TerminalType;
				send.UseExplicitDefaultRadix          = this.settingsRoot.Send.UseExplicitDefaultRadix;
				send.ParseModeForText                 = this.settingsRoot.Send.Text.ToParseMode();
				send.SendTextImmediately              = this.settingsRoot.Send.Text.SendImmediately;
				send.RootDirectoryForFile             = PathEx.GetDirectoryPath(this.terminal.SettingsFilePath);
				send.TerminalIsReadyToSendForSomeTime = this.terminal.IsReadyToSendForSomeTime; // Using 'ForSomeTime' reduces flickering.
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_Print_EnabledToRestore = true; // The default value. Should be backed up first, but could not, see "not get notified" below.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_Find_EnabledToRestore = true; // The default value. Should be backed up first, but could not, see "not get notified" below.

		/// <summary>
		/// Suspends the [Ctrl+F/N/P/L] find shortcuts.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FNPL", Justification = "FNPL refers to these four specific keys.")]
		public virtual void SuspendFindShortcutsCtrlFNPL()
		{
			if (this.findShortcutsCtrlFNPLSuspendedCount < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Counter has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (this.findShortcutsCtrlFNPLSuspendedCount == 0)
			{
				toolStripMenuItem_TerminalMenu_Terminal_Print_EnabledToRestore = toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled;
				toolStripMenuItem_TerminalMenu_Terminal_Find_EnabledToRestore  = toolStripMenuItem_TerminalMenu_Terminal_Find.Enabled;

				toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled = false; // [Ctrl+P]
				toolStripMenuItem_TerminalMenu_Terminal_Find.Enabled  = false; // [Ctrl+F]
			}

			this.findShortcutsCtrlFNPLSuspendedCount++; // No need for "lock"/"Interlocked...()" as WinForms is synchronized onto main thread.

			// Could be implemented more cleverly, by iterating over all potential shortcut controls
			// and then handle those that use one of the shortcuts in question. However, that would
			// be an overkill, thus using this straight-forward implementation.
		}

		/// <summary>
		/// Resumes the [Ctrl+F/N/P/L] find shortcuts.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "FNPL", Justification = "FNPL refers to these four specific keys.")]
		public virtual void ResumeFindShortcutsCtrlFNPL()
		{
			this.findShortcutsCtrlFNPLSuspendedCount--; // No need for "lock"/"Interlocked...()" as WinForms is synchronized onto main thread.

			if (this.findShortcutsCtrlFNPLSuspendedCount < 0) // Main form will call this method also for newly opened
			{                                                 // terminals which did not get notified about Suspend() yet!
				this.findShortcutsCtrlFNPLSuspendedCount = 0; // Simply reset (conservative implementation). Do not throw!
			////throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Counter has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (this.findShortcutsCtrlFNPLSuspendedCount == 0)
			{
				toolStripMenuItem_TerminalMenu_Terminal_Print.Enabled = toolStripMenuItem_TerminalMenu_Terminal_Print_EnabledToRestore;
				toolStripMenuItem_TerminalMenu_Terminal_Find.Enabled  = toolStripMenuItem_TerminalMenu_Terminal_Find_EnabledToRestore;
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_SelectAll_EnabledToRestore = true; // The default value. Should be backed up first, but could not, see "not get notified" below.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_SelectNone_EnabledToRestore = true; // The default value. Should be backed up first, but could not, see "not get notified" below.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard_EnabledToRestore = true; // The default value. Should be backed up first, but could not, see "not get notified" below.

		/// <summary>
		/// Suspends the [Ctrl+A/C/V/Delete] standard edit shortcuts.
		/// </summary>
		/// <remarks>
		/// Named "V" even though [Ctrl+V] shortcut is not used by the form yet, for making intention more obvious.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ACV", Justification = "ACV refers to these three specific keys.")]
		public virtual void SuspendEditShortcutsCtrlACVDelete()
		{
			if (this.editShortcutsCtrlACVDeleteSuspendedCount < 0)
				throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Counter has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));

			if (this.editShortcutsCtrlACVDeleteSuspendedCount == 0)
			{
				toolStripMenuItem_TerminalMenu_Terminal_SelectAll_EnabledToRestore       = toolStripMenuItem_TerminalMenu_Terminal_SelectAll      .Enabled;
				toolStripMenuItem_TerminalMenu_Terminal_SelectNone_EnabledToRestore      = toolStripMenuItem_TerminalMenu_Terminal_SelectNone     .Enabled;
				toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard_EnabledToRestore = toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled;

				toolStripMenuItem_TerminalMenu_Terminal_SelectAll      .Enabled = false; // [Ctrl+A]
				toolStripMenuItem_TerminalMenu_Terminal_SelectNone     .Enabled = false; // [Ctrl+Delete]
				toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = false; // [Ctrl+C]
			}

			this.editShortcutsCtrlACVDeleteSuspendedCount++; // No need for "lock"/"Interlocked...()" as WinForms is synchronized onto main thread.

			// Could be implemented more cleverly, by iterating over all potential shortcut controls
			// and then handle those that use one of the shortcuts in question. However, that would
			// be an overkill, thus using this straight-forward implementation.
		}

		/// <summary>
		/// Resumes the [Ctrl+A/C/V/Delete] standard edit shortcuts.
		/// </summary>
		/// <remarks>
		/// Named "V" even though [Ctrl+V] shortcut is not used by the form yet, for making intention more obvious.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ACV", Justification = "ACV refers to these three specific keys.")]
		public virtual void ResumeEditShortcutsCtrlACVDelete()
		{
			this.editShortcutsCtrlACVDeleteSuspendedCount--; // No need for "lock"/"Interlocked...()" as WinForms is synchronized onto main thread.

			if (this.editShortcutsCtrlACVDeleteSuspendedCount < 0) // Main form will call this method also for newly opened
			{                                                      // terminals which did not get notified about Suspend() yet!
				this.editShortcutsCtrlACVDeleteSuspendedCount = 0; // Simply reset (conservative implementation). Do not throw!
			////throw (new InvalidOperationException(MessageHelper.InvalidExecutionPreamble + "Counter has fallen below 0!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}

			if (this.editShortcutsCtrlACVDeleteSuspendedCount == 0)
			{
				toolStripMenuItem_TerminalMenu_Terminal_SelectAll      .Enabled = toolStripMenuItem_TerminalMenu_Terminal_SelectAll_EnabledToRestore;
				toolStripMenuItem_TerminalMenu_Terminal_SelectNone     .Enabled = toolStripMenuItem_TerminalMenu_Terminal_SelectNone_EnabledToRestore;
				toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard.Enabled = toolStripMenuItem_TerminalMenu_Terminal_CopyToClipboard_EnabledToRestore;
			}
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

				default: throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + preset + "' is a preset that is not (yet) supported here!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
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
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud2400;
							scs.DataBits    = MKY.IO.Ports.DataBits.Seven;
							scs.Parity      = System.IO.Ports.Parity.Even;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 2: // "2400, 7, Even, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud2400;
							scs.DataBits    = MKY.IO.Ports.DataBits.Seven;
							scs.Parity      = System.IO.Ports.Parity.Even;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
							break;
						}
						case 3: // "9600, 8, None, 1, None"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud9600;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 4: // "9600, 8, None, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud9600;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.Software;
							break;
						}
						case 5: // "19200, 8, None, 1, None"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud19200;
							scs.DataBits    = MKY.IO.Ports.DataBits.Eight;
							scs.Parity      = System.IO.Ports.Parity.None;
							scs.StopBits    = System.IO.Ports.StopBits.One;
							scs.FlowControl = MKY.IO.Serial.SerialPort.SerialFlowControl.None;
							break;
						}
						case 6: // "19200, 8, None, 1, Software"
						{
							scs.BaudRate    = (MKY.IO.Ports.BaudRateEx)MKY.IO.Ports.BaudRate.Baud19200;
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

		/// <summary>
		/// Sets <see cref="Domain.Settings.DisplaySettings.TxRadix"/> and
		/// <see cref="Domain.Settings.DisplaySettings.RxRadix"/> in case
		/// <see cref="Domain.Settings.DisplaySettings.SeparateTxRxRadix"/> is inactive.
		/// </summary>
		private void SetMonitorRadix_SameTxRx(Domain.Radix radix)
		{
			this.settingsRoot.Display.TxRadix = radix; // Wouldn't be necessary to set both, 'RxRadix {get}' is redirected
			this.settingsRoot.Display.RxRadix = radix; // to 'TxRadix {get}' anyway. However, setting both is more logical
		}                                              // and less error-prone in case 'RxRadix' would no longer redirect.

		/// <summary>
		/// Sets <see cref="Domain.Settings.DisplaySettings.TxRadix"/> or
		/// <see cref="Domain.Settings.DisplaySettings.RxRadix"/> in case
		/// <see cref="Domain.Settings.DisplaySettings.SeparateTxRxRadix"/> is active.
		/// </summary>
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

			SuspendLayout(); // Useful as "Size" and "Location" will get changed.
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
			SuspendLayout(); // Useful as "Size" and "Location" will get changed.
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

		private long MaxMonitorLineNumberOffset
		{
			get
			{
				long maxOffset = this.monitor_Bidir.LineNumberOffset;

				if (maxOffset < this.monitor_Tx.LineNumberOffset)
					maxOffset = this.monitor_Tx.LineNumberOffset;

				if (maxOffset < this.monitor_Rx.LineNumberOffset)
					maxOffset = this.monitor_Rx.LineNumberOffset;

				return (maxOffset);
			}
		}

		private void ResetMonitorLineNumbers()
		{
			this.monitor_Tx   .ResetLineNumbers();
			this.monitor_Bidir.ResetLineNumbers();
			this.monitor_Rx   .ResetLineNumbers();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		private void ClearMonitor(Domain.RepositoryType repositoryType)
		{
			Exception exception = null;

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
						exception = new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug);
						break;
				}

				Cursor = Cursors.Default;
				SetTimedStatusText("Clearing done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Clearing failed!");
			}

			if (exception != null) // Outside try/catch since throw is intended.
				throw (exception);
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
			Exception exception = null;

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
						exception = new ArgumentOutOfRangeException("repositoryType", repositoryType, MessageHelper.InvalidExecutionPreamble + "'" + repositoryType + "' is an invalid repository type!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug);
						break;
				}

				Cursor = Cursors.Default;
				SetTimedStatusText("Refreshing done");
			}
			catch
			{
				Cursor = Cursors.Default;
				SetFixedStatusText("Refreshing failed!");
			}

			if (exception != null) // Outside try/catch since throw is intended.
				throw (exception);
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

		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma",                       Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration", Justification = "There are too many parameters to pass.")]
		[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines",      Justification = "There are too many parameters to pass.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowFormatSettings()
		{
			int[] customColors = ApplicationSettings.RoamingUserSettings.Color.CustomColorsToWin32();
			bool showMonospacedFontsOnly = ApplicationSettings.RoamingUserSettings.Font.ShowMonospacedOnly;

			var f = new FormatSettings(this.settingsRoot.Format, customColors,
			                           this.settingsRoot.Display.ContentSeparator, this.settingsRoot.Display.InfoSeparator, this.settingsRoot.Display.InfoEnclosure,
			                           this.settingsRoot.Display.TimeStampUseUtc, this.settingsRoot.Display.TimeStampFormat, this.settingsRoot.Display.TimeSpanFormat, this.settingsRoot.Display.TimeDeltaFormat, this.settingsRoot.Display.TimeDurationFormat,
			                           showMonospacedFontsOnly);

			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				this.settingsRoot.Format = f.FormatSettingsResult;

				bool roamingUserSettingsHaveToBeSaved = false;

				if (!ArrayEx.ValuesEqual(customColors, f.CustomColors))
				{
					ApplicationSettings.RoamingUserSettings.Color.UpdateCustomColorsFromWin32(f.CustomColors);

					roamingUserSettingsHaveToBeSaved = true;
				}

				this.settingsRoot.Display.ContentSeparator = f.ContentSeparatorResult;
				this.settingsRoot.Display.InfoSeparator    = f.InfoSeparatorResult;
				this.settingsRoot.Display.InfoEnclosure    = f.InfoEnclosureResult;

				this.settingsRoot.Display.TimeStampUseUtc    = f.TimeStampUseUtcResult;
				this.settingsRoot.Display.TimeStampFormat    = f.TimeStampFormatResult;
				this.settingsRoot.Display.TimeSpanFormat     = f.TimeSpanFormatResult;
				this.settingsRoot.Display.TimeDeltaFormat    = f.TimeDeltaFormatResult;
				this.settingsRoot.Display.TimeDurationFormat = f.TimeDurationFormatResult;

				if (showMonospacedFontsOnly != f.ShowMonospacedFontsOnlyResult)
				{
					ApplicationSettings.RoamingUserSettings.Font.ShowMonospacedOnly = f.ShowMonospacedFontsOnlyResult;

					roamingUserSettingsHaveToBeSaved = true;
				}

				if (roamingUserSettingsHaveToBeSaved)
				{
					ApplicationSettings.SaveRoamingUserSettings();
				}
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
				SetFixedStatusText("Copying lines to clipboard...");
				RtfWriterHelper.CopyLinesToClipboard(monitor.SelectedOrAllLines, this.settingsRoot.Format);
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
				ApplicationSettings.LocalUserSettings.Paths.MonitorFiles = PathEx.GetDirectoryPath(sfd.FileName);
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
			SetFixedStatusText("Saving lines...");
			try
			{
				int requestedCount = monitor.SelectedOrAllLines.Count;
				int savedCount;

				if (ExtensionHelper.IsXmlFile(filePath))
				{
				#if (FALSE) // Enable to use raw instead of text XML export schema, useful for development purposes of the raw XML schema.
					savedCount = Log.Utilities.XmlWriterHelperRaw.SaveLinesToFile(monitor.SelectedOrAllLines, filePath, true);
				#else
					savedCount = Log.Utilities.XmlWriterHelperText.SaveLinesToFile(monitor.SelectedOrAllLines, filePath, true);
				#endif
				}
				else if (ExtensionHelper.IsRtfFile(filePath))
				{
					savedCount = RtfWriterHelper.SaveLinesToFile(monitor.SelectedOrAllLines, filePath, this.settingsRoot.Format);
				}
				else
				{
					savedCount = TextWriterHelper.SaveLinesToFile(monitor.SelectedOrAllLines, filePath, this.settingsRoot.Format);
				}

				if (savedCount == requestedCount)
				{
					SetTimedStatusText("Lines successfully saved");
				}
				else
				{
					SetFixedStatusText("Lines only partially saved!");

					var sb = new StringBuilder();

					sb.Append("Lines only partially saved to file, only ");
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
				SetFixedStatusText("Lines not saved!");

				var sb = new StringBuilder();
				sb.AppendLine("Unable to save lines to file");
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
			SetFixedStatusText("Printing lines...");

			using (var printer = new RtfPrinter(settings))
			{
				try
				{
					printer.Print(monitor.SelectedOrAllLines, this.settingsRoot.Format);
					SetTimedStatusText("Lines printed");
				}
				catch (System.Drawing.Printing.InvalidPrinterException ex)
				{
					SetFixedStatusText("Lines not printed!");

					var sb = new StringBuilder();
					sb.AppendLine("Unable to print lines!");
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
			int pageId = predefined.SelectedPageId;
			if (!this.terminal.SendPredefined(pageId, commandId))
			{
				// If command is invalid, show settings dialog.
				ShowPredefinedCommandSettings(pageId, commandId);
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
				PathEx.GetDirectoryPath(this.terminal.SettingsFilePath),
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
			SetCaption();

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
				{         //// PageLayout must be changed first! Otherwise pages may not be complete!
					predefined.PageLayout            = this.settingsRoot.PredefinedCommand.PageLayout;
					predefined.Pages                 = this.settingsRoot.PredefinedCommand.Pages;
					      //// Pages property always sets, not only when the properly has changed. See remarks of this property.

					predefined.HideUndefinedCommands = this.settingsRoot.PredefinedCommand.HideUndefinedCommands;
				}
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
				OnAutoActionCountChanged(new EventArgs<int>(this.terminal.AutoActionCount)); // Needed because the 'terminal_AutoAction/ResponseCountChanged_Promptly' events are not used.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.AutoResponse))
			{
				SetAutoResponseControls();
				OnAutoResponseSettingsChanged(EventArgs.Empty);
				OnAutoResponseCountChanged(new EventArgs<int>(this.terminal.AutoResponseCount)); // Needed because the 'terminal_AutoAction/ResponseCountChanged_Promptly' events are not used.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Format))
			{
				ReformatMonitors();
				SetAutoActionPlotFormat();
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
				SetPredefinedControls(); // Needed for 'Send.Text.ToParseMode()'.
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
				SetPredefinedControls(); // Potentially set 'IsReadyToSendForSomeTime'.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.SendFile))
			{
				SetSendControls();
				SetPredefinedControls(); // Potentially set 'IsReadyToSendForSomeTime'.
			}
			else if (ReferenceEquals(e.Inner.Source, this.settingsRoot.Predefined))
			{
				this.isSettingControls.Enter(); // See remarks in SetPredefinedControls().
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

			////this.terminal.IsSendingChanged            += terminal_IsSendingChanged is not needed yet.
				this.terminal.IsSendingForSomeTimeChanged += terminal_IsSendingForSomeTimeChanged;

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

				this.terminal.AutoActionPlotRequest_Promptly    += terminal_AutoActionPlotRequest_Promptly; // is OK to be used, only used once for opening the form.
			////this.terminal.AutoActionCountChanged_Promptly   += terminal_AutoActionCountChanged_Promptly;   is not used, see 'terminal_AutoAction/ResponseCountChanged_Promptly' further below for reason.
			////this.terminal.AutoResponseCountChanged_Promptly += terminal_AutoResponseCountChanged_Promptly; is not used, see 'terminal_AutoAction/ResponseCountChanged_Promptly' further below for reason.

				this.terminal.FixedStatusTextRequest             += terminal_FixedStatusTextRequest;
				this.terminal.TimedStatusTextRequest             += terminal_TimedStatusTextRequest;
				this.terminal.ResetStatusTextRequest             += terminal_ResetStatusTextRequest;
				this.terminal.CursorRequest                      += terminal_CursorRequest;
				this.terminal.MessageInputRequest                += terminal_MessageInputRequest;
				this.terminal.ExtendedMessageInputRequest        += terminal_ExtendedMessageInputRequest;
				this.terminal.FontDialogRequest                  += terminal_FontDialogRequest;
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

			////this.terminal.IsSendingChanged            -= terminal_IsSendingChanged is not needed yet.
				this.terminal.IsSendingForSomeTimeChanged -= terminal_IsSendingForSomeTimeChanged;

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

				this.terminal.AutoActionPlotRequest_Promptly    -= terminal_AutoActionPlotRequest_Promptly; // is OK to be used, only used once for opening the form.
			////this.terminal.AutoActionCountChanged_Promptly   -= terminal_AutoActionCountChanged_Promptly;   is not used, see 'terminal_AutoAction/ResponseCountChanged_Promptly' further below for reason.
			////this.terminal.AutoResponseCountChanged_Promptly -= terminal_AutoResponseCountChanged_Promptly; is not used, see 'terminal_AutoAction/ResponseCountChanged_Promptly' further below for reason.

				this.terminal.FixedStatusTextRequest             -= terminal_FixedStatusTextRequest;
				this.terminal.TimedStatusTextRequest             -= terminal_TimedStatusTextRequest;
				this.terminal.ResetStatusTextRequest             -= terminal_ResetStatusTextRequest;
				this.terminal.CursorRequest                      -= terminal_CursorRequest;
				this.terminal.MessageInputRequest                -= terminal_MessageInputRequest;
				this.terminal.ExtendedMessageInputRequest        -= terminal_ExtendedMessageInputRequest;
				this.terminal.FontDialogRequest                  -= terminal_FontDialogRequest;
				this.terminal.SaveAsFileDialogRequest            -= terminal_SaveAsFileDialogRequest;
				this.terminal.SaveCommandPageAsFileDialogRequest -= terminal_SaveCommandPageAsFileDialogRequest;
				this.terminal.OpenCommandPageFileDialogRequest   -= terminal_OpenCommandPageFileDialogRequest;

				this.terminal.Saved                              -= terminal_Saved;
				this.terminal.Closed                             -= terminal_Closed;
			}
		}

		private bool TerminalIsAvailable
		{
			get { return ((this.terminal != null) && (this.terminal.IsUndisposed)); }
		}

		#endregion

		#region Terminal > Event Handlers
		//------------------------------------------------------------------------------------------
		// Terminal > Event Handlers
		//------------------------------------------------------------------------------------------

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IOChanged(object sender, EventArgs e)
		{
			SetTerminalControls();

			OnTerminalChanged(EventArgs.Empty);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IOControlChanged(object sender, Domain.IOControlEventArgs e)
		{
			SetIOControlControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IOConnectTimeChanged(object sender, TimeSpanEventArgs e)
		{
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

		// The "terminal_IOCount/RateChanged_Promptly" events are not used because of the reason
		// described in the remarks of "terminal_RawChunkSent/Received" of "Model.Terminal".
		// Instead, the update is done by the "terminal_DisplayElements[Tx|Bidir|Rx]Added" and
		// "terminal_DisplayLines[Tx|Bidir|Rx]Added" handlers.
		//
		// "terminal_IORateChanged_Decimated" is fine.

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IORateChanged_Decimated(object sender, EventArgs e)
		{
			if (TerminalIsAvailable)
			{
				var status = this.terminal.DataStatus;

				monitor_Tx   .SetDataStatus(status); // Attention, the complete status must be set here, because
				monitor_Bidir.SetDataStatus(status); // in case of too long lines, no display elements will get
				monitor_Rx   .SetDataStatus(status); // added anymore, i.e. counts would no longer get updated.
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[ModalBehaviorContract(ModalBehavior.InCaseOfNonUserError, Approval = "LaunchArgs are considered to decide on behavior.")]
		private void terminal_IOError(object sender, Domain.IOErrorEventArgs e)
		{
			SetTerminalControls();
			OnTerminalChanged(EventArgs.Empty);

			bool showErrorModally = false;
			var main = (this.mdiParent as Main);
			if (main != null)
				showErrorModally = main.UnderlyingMain.LaunchArgs.KeepOpenOnNonSuccess;

			if (e.Severity == Domain.IOErrorSeverity.Acceptable) // Handle acceptable issues.
			{
				SetTimedStatusText("Terminal Warning"); // Simply replicate message box caption, opposed to standard texts which are sentences or fragments.

				if (showErrorModally)
				{
					terminal_IOError_MessageBoxShow
					(
						e.Message,
						"Terminal Warning",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
					);
				}
			}
			else
			{
				SetFixedStatusText("Terminal Error"); // Simply replicate message box caption, opposed to standard texts which are sentences or fragments.

				if (showErrorModally)
				{
					terminal_IOError_MessageBoxShow
					(
						e.Message,
						"Terminal Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<string> terminal_IOError_MessageBoxShowActiveMessages = new List<string>(); // No preset needed, default behavior is good enough.

		private void terminal_IOError_MessageBoxShow(string message, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton = MessageBoxDefaultButton.Button1)
		{
			// Prevent repeating errors from being repeated. In the past this has happend e.g. in
			// case of framing errors on invalid baud rate. This has resulted message boxes across
			// the whole screen...

			if (!terminal_IOError_MessageBoxShowActiveMessages.Contains(message)) // No need for locking as WinForms is synchronized onto main thread.
			{
				terminal_IOError_MessageBoxShowActiveMessages.Add(message); // Just checking message (and not also the icon, i.e. the severity) is considered good enough.

				MessageBoxEx.Show
				(
					this,
					message,
					caption,
					buttons,
					icon,
					defaultButton
				);

				terminal_IOError_MessageBoxShowActiveMessages.Remove(message);
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_IsSendingForSomeTimeChanged(object sender, EventArgs<bool> e)
		{
			SetTerminalControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataStatus()
		{
			SetDataStatusSent();
			SetDataStatusReceived();
		}

		/// <remarks>
		/// Named 'Sent' to emphasize relation to 'Tx' as well as 'Bidir' monitor.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataStatusSent()
		{
			if (!TerminalIsAvailable) // Ensure to not handle event during closing anymore.
				return;

			var status = this.terminal.DataStatus;
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)    { monitor_Tx   .SetDataStatus(status); }
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible) { monitor_Bidir.SetDataStatus(status); }
		}

		/// <remarks>
		/// Named 'Received' to emphasize relation to 'Rx' as well as 'Bidir' monitor.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "See event handlers below.")]
		private void SetDataStatusReceived()
		{
			if (!TerminalIsAvailable) // Ensure to not handle event during closing anymore.
				return;

			var status = this.terminal.DataStatus;
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible) { monitor_Bidir.SetDataStatus(status); }
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)    { monitor_Rx.   SetDataStatus(status); }
		}

		/// <remarks>
		/// See <see cref="Model.Terminal.AutoTriggerIsActiveButLimitedToLine"/> for backgound.
		/// </remarks>
		protected virtual bool UseDisplayElementsAddedEventsForAddingToMonitors
		{
			get { return (!this.terminal.AutoTriggerIsActiveButLimitedToLine); }
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsTxAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (UseDisplayElementsAddedEventsForAddingToMonitors) // See property for background.
			{
				if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
					monitor_Tx.AddElements(e.Elements);
			}

			SetDataStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayElementsRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayElementsBidirAdded(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (UseDisplayElementsAddedEventsForAddingToMonitors) // See property for background.
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
			if (UseDisplayElementsAddedEventsForAddingToMonitors) // See property for background.
			{
				if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
					monitor_Rx.AddElements(e.Elements);
			}

			SetDataStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.ReplaceCurrentLine(e.Elements);

			SetDataStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.ReplaceCurrentLine(e.Elements);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirReplaced", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxReplaced(object sender, Domain.DisplayElementsEventArgs e)
		{
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.ReplaceCurrentLine(e.Elements);

			SetDataStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineTxCleared(object sender, EventArgs e)
		{
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.ClearCurrentLine();

			SetDataStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineBidirCleared(object sender, EventArgs e)
		{
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.ClearCurrentLine();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.CurrentDisplayLineBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_CurrentDisplayLineRxCleared(object sender, EventArgs e)
		{
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.ClearCurrentLine();

			SetDataStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesBidirAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesTxAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (!UseDisplayElementsAddedEventsForAddingToMonitors) // See property for background.
			{
				if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
					monitor_Tx.AddLines(e.Lines);
			}

			SetDataStatusSent();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesTxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.DisplayLinesRxAdded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_DisplayLinesBidirAdded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (!UseDisplayElementsAddedEventsForAddingToMonitors) // See property for background.
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
			if (!UseDisplayElementsAddedEventsForAddingToMonitors) // See property for background.
			{
				if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
					monitor_Rx.AddLines(e.Lines);
			}

			SetDataStatusReceived();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxCleared(object sender, EventArgs e)
		{
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible)
				monitor_Tx.Clear();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirCleared(object sender, EventArgs e)
		{
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible)
				monitor_Bidir.Clear();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirCleared", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxCleared(object sender, EventArgs e)
		{
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible)
				monitor_Rx.Clear();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryTxReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (this.settingsRoot.Layout.TxMonitorPanelIsVisible && (e.Lines.Count > 0)) // Reload may be triggered with an empty repository, no need to "AddLines()" then.
				monitor_Tx.AddLines(e.Lines);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryRxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryBidirReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (this.settingsRoot.Layout.BidirMonitorPanelIsVisible && (e.Lines.Count > 0)) // Reload may be triggered with an empty repository, no need to "AddLines()" then.
				monitor_Bidir.AddLines(e.Lines);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryTxReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		[CallingContract(IsAlwaysSequentialIncluding = "Terminal.RepositoryBidirReloaded", Rationale = "The terminal synchronizes display element/line processing.")]
		private void terminal_RepositoryRxReloaded(object sender, Domain.DisplayLinesEventArgs e)
		{
			if (this.settingsRoot.Layout.RxMonitorPanelIsVisible && (e.Lines.Count > 0)) // Reload may be triggered with an empty repository, no need to "AddLines()" then.
				monitor_Rx.AddLines(e.Lines);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_AutoActionPlotRequest_Promptly(object sender, EventArgs e)
		{
			AutoActionPlot();
		}

		// The 'terminal_AutoAction/ResponseCountChanged_Promptly' events are not used because in
		// case of fast continuous data synchronizing the events from the AutoAction/ResponseThread
		// onto the MainThread will lead to no longer updating monitors (and plot) because the whole
		// MainThread CPU capacity is spent with updating the quickly changing counts. Therefore
		// polling the counts by 'timer_AutoAction/ResponseCountUpdate_Tick' instead.

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int timer_AutoActionCountUpdate_Tick_CountOld; // = 0;

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_AutoActionCountUpdate_Tick(object sender, EventArgs e)
		{
			if (this.terminal != null)
			{
				var count = this.terminal.AutoActionCount;

				if (timer_AutoActionCountUpdate_Tick_CountOld != count) {
					timer_AutoActionCountUpdate_Tick_CountOld = count;

					OnAutoActionCountChanged(new EventArgs<int>(count));
				}
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private int timer_AutoResponseCountUpdate_Tick_CountOld; // = 0;

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_AutoResponseCountUpdate_Tick(object sender, EventArgs e)
		{
			if (this.terminal != null)
			{
				var count = this.terminal.AutoResponseCount;

				if (timer_AutoResponseCountUpdate_Tick_CountOld != count) {
					timer_AutoResponseCountUpdate_Tick_CountOld = count;

					OnAutoResponseCountChanged(new EventArgs<int>(count));
				}
			}
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_FixedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetFixedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_TimedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetTimedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_ResetStatusTextRequest(object sender, EventArgs e)
		{
			ResetStatusText();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void terminal_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			Refresh(); // Ensure that form has been refreshed before showing the message box.
			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void terminal_ExtendedMessageInputRequest(object sender, Model.ExtendedMessageInputEventArgs e)
		{
			var checkValue = e.CheckValue;
			var dr = ExtendedMessageBox.Show(this, e.Text, e.Links, e.Caption, e.CheckText, ref checkValue, e.Buttons, e.Icon, e.DefaultButton);
			e.CheckValue = checkValue;
			e.Result = dr;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_FontDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowTerminalFontDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveTerminalAsFileDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_SaveCommandPageAsFileDialogRequest(object sender, Model.FilePathDialogEventArgs e)
		{
			string filePathNew;
			e.Result = CommandPagesSettingsFileLinkHelper.ShowSaveAsFileDialog(this, e.FilePathOld, out filePathNew);
			e.FilePathNew = filePathNew;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_OpenCommandPageFileDialogRequest(object sender, Model.FilePathDialogEventArgs e)
		{
			string filePathNew;
			e.Result = CommandPagesSettingsFileLinkHelper.ShowOpenFileDialog(this, e.FilePathOld, out filePathNew);
			e.FilePathNew = filePathNew;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			Cursor = e.Value;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_Saved(object sender, Model.SavedEventArgs e)
		{
			SetTerminalControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void terminal_Closed(object sender, Model.ClosedEventArgs e)
		{
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
			SetCaption(); // Only terminal dependent.
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
			SetAutoActionPlotCaption();
		}

		private void SetCaption()
		{
			if (TerminalIsAvailable)
				Text = this.terminal.Caption;
			else
				Text = "";
		}

		private void SetIOStatus()
		{
			var green  = Properties.Resources.Image_Status_Green_12x12;
			var yellow = Properties.Resources.Image_Status_Yellow_12x12;
			var red    = Properties.Resources.Image_Status_Red_12x12;

			if (TerminalIsAvailable)
			{
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Enabled = this.terminal.IsStarted;

				if (this.terminal.IsOpen)
				{
					if (this.terminal.IsTransmissive)
					{
						if (!this.terminal.IsSendingForSomeTime)   // Not checking for 'IsSending' as that a) might distract user
						////!this.terminal.IsReceivingForSomeTime) // and b) consume unncessary CPU time (draw LED quickly twice).
						{
							ResetIOStatusFlashing();
							SetIOStatusSteady(green);
						}
						else
						{
							StartIOStatusFlashing(green);
						}
					}
					else // IsOpen && !IsTransmissive => can only send *or* receive (so far)
					{
						if (!this.terminal.IsSendingForSomeTime)   // Not checking for 'IsSending' as that a) might distract user
						////!this.terminal.IsReceivingForSomeTime) // and b) consume unncessary CPU time (draw LED quickly twice).
						{
							ResetIOStatusFlashing();
							SetIOStatusSteady(yellow);
						}
						else
						{
							StartIOStatusFlashing(yellow);
						}
					}
				}
				else // is closed
				{
					ResetIOStatusFlashing();
					SetIOStatusSteady(red);
				}

				toolStripStatusLabel_TerminalStatus_IOStatus.Text = this.terminal.IOStatusText;
			}
			else // "TerminalIsNotAvailable"
			{
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Enabled = false;

				ResetIOStatusFlashing();
				SetIOStatusSteady(red);

				toolStripStatusLabel_TerminalStatus_IOStatus.Text = "";
			}
		}

		private void SetIOStatusSteady(Image steadyImage)
		{
			toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Steady;

			if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != steadyImage) // Improve performance by only assigning if different.
				toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = steadyImage;
		}

		private void StartIOStatusFlashing(Image onPhaseImage)
		{
			toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag = IOStatusIndicatorControl.Flashing;

			// Do not directly access the image, it will be flashed by the timer below.
			// Directly accessing the image could result in irregular flashing.

			timer_IOStatusIndicator_flashingOnPhaseImage = onPhaseImage;

			timer_IOStatusIndicator.Enabled = true;
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private Image timer_IOStatusIndicator_flashingOnPhaseImage; // = null;

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private bool timer_IOStatusIndicator_flashingIsOn; // = false;

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_IOStatusIndicator_Tick(object sender, EventArgs e)
		{
			if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag != null) // Cannot use "as" because the status is an enum, i.e. not a nullable type.
			{
				var tag = (IOStatusIndicatorControl)toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Tag;
				if (tag == IOStatusIndicatorControl.Flashing) // Only handle the image if flashing is desired:
				{
					timer_IOStatusIndicator_flashingIsOn = !timer_IOStatusIndicator_flashingIsOn; // Toggle flashing phase (initially 'false').

					var onPhase  = this.timer_IOStatusIndicator_flashingOnPhaseImage;
					var offPhase = Properties.Resources.Image_Status_Grey_12x12;
					var current  = (timer_IOStatusIndicator_flashingIsOn ? onPhase : offPhase);

					if (toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image != current) // Improve performance by only assigning if different.
						toolStripStatusLabel_TerminalStatus_IOStatusIndicator.Image = current;
				}
			}
		}

		private void ResetIOStatusFlashing()
		{
			timer_IOStatusIndicator.Enabled = false;
			timer_IOStatusIndicator_flashingIsOn = false; // Reset flashing phase (initially 'false').
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation completes in any case.")]
		[SuppressMessage("Microsoft.Performance", "CA1809:AvoidExcessiveLocals", Justification = "Agree, could be refactored. Could be.")]
		private void SetIOControlControls()
		{
			SuspendLayout(); // Prevent flickering when visibility of status labels temporarily changes.
			try
			{
				var on  = Properties.Resources.Image_Status_Green_12x12;
				var off = Properties.Resources.Image_Status_Red_12x12;

				var isOpen = ((this.terminal != null) && (this.terminal.IsOpen));

				var isSerialPort   = false;
				var isUsbSerialHid = false;

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
						var inputBreak = false;
						var outputBreak = false;

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

						var allowXOnXOff    = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
						var indicateXOnXOff = allowXOnXOff; // Indication only works if manual XOn/XOff (bug #214).
						var inputIsXOn      = false;
						var outputIsXOn     = false;

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

						var ctsImage = (pins.Cts ? on : off);
						var dtrImage = (pins.Dtr ? on : off);
						var dsrImage = (pins.Dsr ? on : off);
						var dcdImage = (pins.Dcd ? on : off);

						if (toolStripStatusLabel_TerminalStatus_CTS.Image != ctsImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_CTS.Image = ctsImage;

						if (toolStripStatusLabel_TerminalStatus_DTR.Image != dtrImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_DTR.Image = dtrImage;

						if (toolStripStatusLabel_TerminalStatus_DSR.Image != dsrImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_DSR.Image = dsrImage;

						if (toolStripStatusLabel_TerminalStatus_DCD.Image != dcdImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_DCD.Image = dcdImage;

						var allowRts = !this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesRtsCtsAutomatically;
						var allowDtr = !this.settingsRoot.Terminal.IO.SerialPort.Communication.FlowControlManagesDtrDsrAutomatically;

						var rtsForeColor = (allowRts ? SystemColors.ControlText : SystemColors.GrayText);
						var dtrForeColor = (allowDtr ? SystemColors.ControlText : SystemColors.GrayText);

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

						toolStripStatusLabel_TerminalStatus_Separator3.Visible    = indicateXOnXOff;
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

						var inputXOnXOffImage  = (inputIsXOn  ? on : off);
						var outputXOnXOffImage = (outputIsXOn ? on : off);

						if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image != inputXOnXOffImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = inputXOnXOffImage;

						if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image != outputXOnXOffImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = outputXOnXOffImage;

						var inputXOnXOffForeColor  = (allowXOnXOff ? SystemColors.ControlText : SystemColors.GrayText);

						if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor != inputXOnXOffForeColor) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_InputXOnXOff.ForeColor = inputXOnXOffForeColor;

						if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_OutputXOnXOff.ForeColor = SystemColors.GrayText;

						var indicateBreakStates = this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates;
						var manualOutputBreak   = this.settingsRoot.Terminal.IO.SerialPortOutputBreakIsModifiable;

						toolStripStatusLabel_TerminalStatus_Separator4.Visible  = indicateBreakStates;
						toolStripStatusLabel_TerminalStatus_InputBreak.Visible  = indicateBreakStates;
						toolStripStatusLabel_TerminalStatus_OutputBreak.Visible = indicateBreakStates;

						var inputBreakImage  = (inputBreak  ? off : on);
						var outputBreakImage = (outputBreak ? off : on);

						if (toolStripStatusLabel_TerminalStatus_InputBreak.Image != inputBreakImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_InputBreak.Image = inputBreakImage;

						if (toolStripStatusLabel_TerminalStatus_OutputBreak.Image != outputBreakImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_OutputBreak.Image = outputBreakImage;

						if (toolStripStatusLabel_TerminalStatus_InputBreak.ForeColor != SystemColors.GrayText) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_InputBreak.ForeColor = SystemColors.GrayText;

						var manualBreakColor = (manualOutputBreak ? SystemColors.ControlText : SystemColors.GrayText);

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

						var indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
						toolStripStatusLabel_TerminalStatus_Separator3.Visible    = indicateXOnXOff;
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

						var indicateBreakStates = this.settingsRoot.Terminal.IO.IndicateSerialPortBreakStates;
						toolStripStatusLabel_TerminalStatus_Separator4.Visible  = indicateBreakStates;
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
						var allowXOnXOff    = this.settingsRoot.Terminal.IO.FlowControlManagesXOnXOffManually;
						var indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff;
						var inputIsXOn      = false;
						var outputIsXOn     = false;

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

						toolStripStatusLabel_TerminalStatus_Separator3.Visible    = indicateXOnXOff;
						toolStripStatusLabel_TerminalStatus_InputXOnXOff.Visible  = indicateXOnXOff;
						toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Visible = indicateXOnXOff;

						var inputXOnXOffImage = (inputIsXOn ? on : off);
						var outputXOnXOffImage = (outputIsXOn ? on : off);

						if (toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image != inputXOnXOffImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_InputXOnXOff.Image = inputXOnXOffImage;

						if (toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image != outputXOnXOffImage) // Improve performance by only assigning if different.
							toolStripStatusLabel_TerminalStatus_OutputXOnXOff.Image = outputXOnXOffImage;

						var inputXOnXOffForeColor = (allowXOnXOff ? SystemColors.ControlText : SystemColors.GrayText);

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
						var indicateXOnXOff = this.settingsRoot.Terminal.IO.FlowControlUsesXOnXOff;
						toolStripStatusLabel_TerminalStatus_Separator3.Visible    = indicateXOnXOff;
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

			var on = Properties.Resources.Image_Status_Green_12x12;
			if (toolStripStatusLabel_TerminalStatus_RTS.Image != on) // Improve performance by only assigning if different.
				toolStripStatusLabel_TerminalStatus_RTS.Image = on;

			timer_RtsLuminescence.Interval = RtsLuminescenceDuration; // Could also be fixed by the designer, doen't matter much.
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
			var on  = Properties.Resources.Image_Status_Green_12x12;
			var off = Properties.Resources.Image_Status_Red_12x12;

			var isOpen = ((this.terminal != null) ? (this.terminal.IsOpen) : (false));
			if (isOpen)
			{
				var pins = new MKY.IO.Ports.SerialPortControlPins();

				var port = (this.terminal.UnderlyingIOInstance as MKY.IO.Ports.ISerialPort);
				if (port != null)
					pins = port.ControlPins;

				var rts = (pins.Rts ? on : off);
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

		#region Terminal > Settings
		//------------------------------------------------------------------------------------------
		// Terminal > Settings
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private DialogResult ShowTerminalFontDialog()
		{                                                                          // Restrict to monospaced fonts no matter what "RoamingUserSettings.Font.ShowMonospacedOnly"
			Font fontSelectedAndConfirmed;                                         // is configured to. The user does not have the option to configure the behavior here.
			var dr = FormatSettings.ShowFontDialog(this, settingsRoot.Format.Font, true, out fontSelectedAndConfirmed);
			if (dr == DialogResult.OK)
			{
				if ((fontSelectedAndConfirmed.Name != settingsRoot.Format.Font.Name) ||
				    (fontSelectedAndConfirmed.Size != settingsRoot.Format.Font.Size))
				{
					settingsRoot.Format.Font = fontSelectedAndConfirmed;
				}
			}

			return (dr);
		}

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
				ApplicationSettings.LocalUserSettings.Paths.MainFiles = PathEx.GetDirectoryPath(sfd.FileName);
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

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowTerminalSettings()
		{
			SetFixedStatusText("Terminal settings...");

			var f = new TerminalSettings(this.settingsRoot.Explicit);

			// Meta information needed for e.g. "(in use by this terminal)":
			f.TerminalId             = this.terminal.SequentialId;
			f.TerminalIsOpen         = this.terminal.IsOpen;
			f.TerminalSerialPortName = this.terminal.IOSerialPortName;

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
				this.autoActionPlotForm.Text = ComposeAutoActionPlotFormCaption();
				this.autoActionPlotForm.SuspendAutoAction    += AutoActionPlotForm_SuspendAutoAction;
				this.autoActionPlotForm.ResumeAutoAction     += AutoActionPlotForm_ResumeAutoAction;
				this.autoActionPlotForm.DeactivateAutoAction += AutoActionPlotForm_DeactivateAutoAction;
				this.autoActionPlotForm.FormClosing          += AutoActionPlotForm_FormClosing;
				this.autoActionPlotForm.Show(this);
			}
			else
			{
				this.autoActionPlotForm.Text = ComposeAutoActionPlotFormCaption();
			}
		}

		private void SetAutoActionPlotCaption()
		{
			if (this.autoActionPlotForm != null)
				this.autoActionPlotForm.Text = ComposeAutoActionPlotFormCaption();
		}

		private void SetAutoActionPlotFormat()
		{
			if (this.autoActionPlotForm != null)
			{
				if (this.terminal != null)
					this.autoActionPlotForm.PlotAreaBackColor = this.terminal.SettingsRoot.Format.BackColor;
			}
		}

		/// <summary></summary>
		protected virtual string ComposeAutoActionPlotFormCaption()
		{
			// "YAT - [IndicatedName]" same as for MDI main form:
			string indicatedName = null;
			if (this.terminal != null) // Name becomes a modified terminal caption.
				indicatedName = this.terminal.ComposeCaption("Automatic Action Plot");

			return (Model.Utilities.CaptionHelper.ComposeMain(indicatedName));
		}

		private void AutoActionPlotForm_SuspendAutoAction(object sender, EventArgs e)
		{
			RequestAutoActionSuspend();
		}

		private void AutoActionPlotForm_ResumeAutoAction(object sender, EventArgs e)
		{
			RequestAutoActionResume();
		}

		private void AutoActionPlotForm_DeactivateAutoAction(object sender, EventArgs e)
		{
			RequestAutoActionDeactivate();
		}

		private void AutoActionPlotForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			this.autoActionPlotForm = null;

			ReactivateAutoActionPlotRequestEvent();
		}

		/// <remarks>
		/// Required to prevent unnecessary delays when <see cref="Model.Terminal"/> invokes
		/// the <see cref="Model.Terminal.AutoActionPlotRequest_Promptly"/> event, which will be
		/// will be synchronized onto the main thread, which in case of massive sending
		/// or receiving is already heavily loaded by the monitor update.
		/// </remarks>
		private void DeactivateAutoActionPlotRequestEvent()
		{
			if (this.terminal != null)
				this.terminal.AutoActionPlotRequest_Promptly -= terminal_AutoActionPlotRequest_Promptly;
		}

		private void ReactivateAutoActionPlotRequestEvent()
		{
			if (this.terminal != null)
				this.terminal.AutoActionPlotRequest_Promptly += terminal_AutoActionPlotRequest_Promptly;
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

		/// <summary></summary>
		protected virtual void OnFindAllSuccessChanged(EventArgs<bool> e)
		{
			EventHelper.RaiseSync<EventArgs<bool>>(FindAllSuccessChanged, this, e);
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

		private void SetFixedStatus(Status status)
		{
			SetFixedStatusText(GetStatusText(status));
		}

		private void SetFixedStatusText(string text)
		{
			timer_StatusText.Enabled = false;

			toolStripStatusLabel_TerminalStatus_Status.Text = text;
		}

		private void SetTimedStatusText(string text)
		{
			SetFixedStatusText(text);

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
			ResetStatusText();
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with the formatted message, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected void DebugMessage(string format, params object[] args)
		{
			DebugMessage(string.Format(CultureInfo.CurrentCulture, format, args));
		}

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			string id;
			if (this.terminal != null)
				id = "#" + this.terminal.SequentialId.ToString("D2", CultureInfo.CurrentCulture);
			else
				id = "N/A";

			string guid;
			if (this.terminal != null)
				guid = this.terminal.Guid.ToString();
			else
				guid = "N/A";

			Debug.WriteLine
			(
				string.Format
				(
					CultureInfo.CurrentCulture,
					" @ {0} @ Thread #{1} : {2,36} {3,3} {4,-38} : {5}",
					DateTime.Now.ToString("HH:mm:ss.fff", DateTimeFormatInfo.CurrentInfo),
					Thread.CurrentThread.ManagedThreadId.ToString("D3", CultureInfo.CurrentCulture),
					GetType(),
					id,
					"[" + guid + "]",
					message
				)
			);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
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
