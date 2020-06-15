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
// YAT Version 2.2.0 Development
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

	// Enable debugging of state changes and validation related to user input:
////#define DEBUG_FIND

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
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Collections.Specialized;
using MKY.Contracts;
using MKY.IO;
using MKY.Settings;
using MKY.Text.RegularExpressions;
using MKY.Windows.Forms;

#if (WITH_SCRIPTING)
using MT.Albatros.Core;
#endif

using YAT.Application.Settings;
using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Model.Utilities;
using YAT.Settings.Application;
using YAT.Settings.Model;
using YAT.View.Controls;
using YAT.View.Utilities;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges", Scope = "member", Target = "YAT.View.Forms.Main.#InitializeComponent()", Justification = "Well, any better idea on how to implement a millisecond update ticker? And, the timer is only used if the user choses so.")]
[module: SuppressMessage("Microsoft.Naming", "CA1703:ResourceStringsShouldBeSpelledCorrectly", Scope = "resource", Target = "YAT.View.Forms.Main.resources", MessageId = "A-Za-z", Justification = "This is a valid regular expression.")]
[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "YAT.View.Forms.Main.#toolTip", Justification = "This is a bug in FxCop.")]

#endregion

namespace YAT.View.Forms
{
	/// <summary>
	/// Main form, provides setup dialogs and hosts terminal forms (MDI forms).
	/// </summary>
	public partial class Main : Form
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

		#endregion

		#region Constants
		//==========================================================================================
		// Constants
		//==========================================================================================

		// Status:
		private const int TimedStatusInterval = 2000;

		#endregion

		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// Startup/Update/Closing:
		private bool isStartingUp = true;
		private SettingControlsHelper isSettingControls;
		private bool isLayoutingMdi = false;
		private bool invokeLayout = false;
		private ClosingState closingState = ClosingState.None;
		private Model.MainResult result = Model.MainResult.Success;

		// MDI:
		private ContextMenuStripShortcutTargetWorkaround contextMenuStripShortcutTargetWorkaround;

		// Model:
		private Model.Main main;
		private Model.Workspace workspace;

		// Find:
		private FindDirection findDirection; // = FindDirection.Undetermined;
		private FindResult findResult;       // = FindResult.Reset;
		private bool findNextIsFeasible;     // = false
		private bool findPreviousIsFeasible; // = false

		// Auto:
		private bool autoActionTriggerValidationIsOngoing;    // = false;
	////private bool autoActionActionValidationIsOngoing;     // = false; is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
		private bool autoResponseTriggerValidationIsOngoing;  // = false;
		private bool autoResponseResponseValidationIsOngoing; // = false;

	#if (WITH_SCRIPTING)
		// Scripting:
		private bool scriptDialogIsOpen = false;
	#endif

		// Toolstrip-combobox-validation-workaround (too late invocation of 'Validate' event):
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool mainToolValidationWorkaround_UpdateIsSuspended; // = false

		#endregion

		#region Object Lifetime
		//==========================================================================================
		// Object Lifetime
		//==========================================================================================

		// Important note to ensure proper z-order of this form:
		// - With visual designer, proceed in the following order
		//     1. Add control ToolStripPanel to the toolbox
		//     2. Place three panels onto the form
		//     3. Place the toolstrip into "toolStripPanel_Top"
		//     4. Dock "toolStripPanel_Left" and "toolStripPanel_Right"
		//       (Dock "toolStripPanel_Bottom")
		//     5. Dock "toolStripPanel_Top"
		//     6. Dock "statusStrip_Main" to bottom
		//     7. Dock "menuStrip_Main" to top
		// - (In source code, proceed according to the MenuStrip class example in the MSDN)

		/// <summary></summary>
		public Main(Model.Main main)
		{
			DebugMessage("Creating...");

			InitializeComponent();
			FixContextMenus();
			InitializeControls();

			// Register this form as the main form:
			NativeMessageHandler.RegisterMainForm(this);

			// Link and attach to main model:
			this.main = main;
			AttachMainEventHandlers();
			Text = this.main.IndicatedName;

			// Link and attach to user settings:
			AttachUserSettingsEventHandlers();

			ApplyWindowSettingsAccordingToStartupState();

			DebugMessage("...successfully created.");
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		/// <remarks>Property for orthogonality with <see cref="IsClosing"/>.</remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'orthogonality' is a correct English term.")]
		private bool IsStartingUp
		{
			get { return (this.isStartingUp); }
		}

		private bool IsClosing
		{
			get { return (this.closingState != ClosingState.None); }
		}

		/// <summary></summary>
		public Model.MainResult Result
		{
			get { return (this.result); }
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
		/// Note that this main form is only created when YAT is run WITH a view. If YAT is run
		/// WITHOUT a view, <see cref="Model.Main.Start"/> is called by either
		/// YAT.Application.Main.RunFullyFromConsole() or YAT.Application.Main.RunInvisible().
		/// </remarks>
		[ModalBehaviorContract(ModalBehavior.InCaseOfNonUserError, Approval = "StartArgs are considered to decide on behavior.")]
		private void Main_Shown(object sender, EventArgs e)
		{
			this.isStartingUp = false;

			SetControls();

			this.isLayoutingMdi = true;      // Temporarily notify MDI layouting to prevent that
			this.result = this.main.Start(); // initial layouting overwrites the workspace settings.
			this.isLayoutingMdi = false;

			if (this.result != Model.MainResult.Success)
			{
				bool showErrorModally = this.main.StartArgs.Interactive;
				bool keepOpenOnError  = this.main.StartArgs.KeepOpenOnError;

				switch (this.result)
				{
					case Model.MainResult.CommandLineError:
					{
						if (showErrorModally)
						{
							var name = ApplicationEx.ExecutableNameWithoutExtension;

							var sb = new StringBuilder();
							sb.Append(ApplicationEx.ProductName); // "YAT" or "YATConsole", as indicated in main title bar.
							sb.Append(" could not be started because the given command line is invalid!");
							sb.AppendLine();
							sb.AppendLine();
							sb.Append(@"Use """ + name + @"[.exe] /?"" for command line help.");

							MessageBoxEx.Show
							(
								this,
								sb.ToString(),
								"Invalid Command Line",
								MessageBoxButtons.OK,
								MessageBoxIcon.Exclamation
							);
						}
						break;
					}

					case Model.MainResult.ApplicationStartError:
					{
						if (showErrorModally)
						{
							var sb = new StringBuilder();
							sb.Append(ApplicationEx.ProductName); // "YAT" or "YATConsole", as indicated in main title bar.
							sb.Append(" could not be started with the given settings!");

							if (!string.IsNullOrEmpty(this.main.StartArgs.ErrorMessage))
							{
								sb.AppendLine();
								sb.AppendLine();
								sb.Append(this.main.StartArgs.ErrorMessage);
							}

							MessageBoxEx.Show
							(
								this,
								sb.ToString(),
								"Start Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error
							);
						}
						break;
					}

					case Model.MainResult.ApplicationStartCancel:
					{
						if (showErrorModally)
						{
							// Nothing to do, user intentionally canceled.
						}
						break;
					}

					case Model.MainResult.ApplicationRunError:
					{
						if (showErrorModally)
						{
							var sb = new StringBuilder();
							sb.Append(ApplicationEx.ProductName); // "YAT" or "YATConsole", as indicated in main title bar.
							sb.Append(" could not execute the requested operation!");

							MessageBoxEx.Show
							(
								this,
								sb.ToString(),
								"Execution Error",
								MessageBoxButtons.OK,
								MessageBoxIcon.Error
							);
						}
						break;
					}

					// Do nothing in the following cases:
					case Model.MainResult.ApplicationExitError:
					case Model.MainResult.UnhandledException:
					default:
					{
						break;
					}
				}

				if (!keepOpenOnError)
					Close();
			}
			else // Model.MainResult.Success:
			{
				if (this.workspace.TerminalCount == 0)
				{
					// If workspace is empty, and the new terminal dialog is requested, display it:
					if (this.main.StartArgs.ShowNewTerminalDialog)
					{
						// Let those settings that are given by the command line args be modified/overridden:
						var processedSettings = new Model.Settings.NewTerminalSettings(ApplicationSettings.LocalUserSettings.NewTerminal);
						if (this.main.ProcessCommandLineArgsIntoExistingNewTerminalSettings(processedSettings))
							ShowNewTerminalDialog(processedSettings);
						else
							ShowNewTerminalDialog();
					}
				}
				else
				{
					// If workspace contains terminals, and if requested, arrange them accordingly:
					if (this.main.StartArgs.TileHorizontal)
						LayoutWorkspace(WorkspaceLayout.TileHorizontal);
					else if (this.main.StartArgs.TileVertical)
						LayoutWorkspace(WorkspaceLayout.TileVertical);
					else
						LayoutWorkspace();
				}
			}
		}

		private void Main_LocationChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsClosing)
				UpdateWindowSettings(true);
		}

		private void Main_SizeChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsClosing)
				UpdateWindowSettings(false);
		}

		private void Main_Resize(object sender, EventArgs e)
		{
			if (!IsStartingUp && !IsClosing)
				ResizeWorkspace();
		}

		private void Main_MdiChildActivate(object sender, EventArgs e)
		{
			if (ActiveMdiChild != null)
			{
				this.workspace.ActivateTerminal(((Terminal)ActiveMdiChild).UnderlyingTerminal);

				// Activate the MDI child, to ensure that shortcuts effect the desired terminal:
				ActiveMdiChild.BringToFront();

				if (this.invokeLayout)
				{
					this.invokeLayout = false;
					LayoutWorkspace();

					// There is a limitation here:
					//
					// The 'FormClosed' event is invoked before the MDI child has been disposed of.
					// Thus, layouting would take the just-about-to-be-closed terminal into account.
					// The workaround with 'invokeLayout' unfortunately doesn't work as well, as the
					// close form is still active when this 'MdiChildActivate' event is invoked...
					//
					// Keeping this limitation, shall again be checked after upgrading to .NET 4+.
				}

				SetTimedStatus(Status.ChildActivated);
				SetTerminalInfoText(this.workspace.ActiveTerminalInfoText);
			}
			else
			{
				if (this.invokeLayout)
				{
					this.invokeLayout = false;
					ResetTerminalLayout(); // Closing all terminals shall reset the layout to 'Automatic'.
				}

			////SetTimedStatus(Status.ChildClosed) is called by 'terminalMdiChild_FormClosed()'.
				SetTerminalInfoText("");
			}

			SetChildControls();
		}

		private void Main_Deactivate(object sender, EventArgs e)
		{
			DebugMdi("Deactivated");

			toolStripComboBox_MainTool_Find_Pattern         .OnFormDeactivateWorkaround();
			toolStripComboBox_MainTool_AutoAction_Trigger   .OnFormDeactivateWorkaround();
		////toolStripComboBox_MainTool_AutoAction_Action is a standard ToolStripComboBox.
			toolStripComboBox_MainTool_AutoResponse_Trigger .OnFormDeactivateWorkaround();
			toolStripComboBox_MainTool_AutoResponse_Response.OnFormDeactivateWorkaround();

			// Also notify the active MDI child about switching to another application:
			var f = (ActiveMdiChild as IOnFormDeactivateWorkaround);
			if (f != null)
				f.OnFormDeactivateWorkaround();

			// Do not notify the other MDI children, as their ComboBoxes would keep the state
			// without having focus, which is somewhat undefined.
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Skip if WinForms has already determined to cancel:
			if (e.Cancel)
				return;

			// Prevent multiple calls to Exit()/Close():
			if (this.closingState == ClosingState.None)
			{
				this.closingState = ClosingState.IsClosingFromForm;

				// Also notify MDI children about closing:
				foreach (var f in MdiChildren)
				{
					var t = (f as Terminal);
					if (t != null)
						t.NotifyClosingFromForm();
				}

				// Save window settings (which will save the local user settings) before proceeding
				// (and potentially modify the local user settings):
				SaveWindowSettings();

				bool cancel;
				this.main.Exit(out cancel); // Only need to handle cancel on form here, no need to
				if (cancel)                 // deal with result, that is handled in main_Exited().
				{
					e.Cancel = true;

					// Revert closing state in case of cancel:
					this.closingState = ClosingState.None;

					// Also revert closing state for MDI children:
					foreach (var f in MdiChildren)
					{
						var t = (f as Terminal);
						if (t != null)
							t.RevertClosingState();
					}
				}
			}
		}

		/// <remarks>
		/// Not really sure whether handling here is required in any case. Normally, workspace as
		/// well as main signal via <see cref="workspace_Closed"/> and <see cref="main_Exited"/>.
		/// However, instead of verifying every possible case, simply detach here too.
		/// </remarks>
		private void Main_FormClosed(object sender, FormClosedEventArgs e)
		{
			DetachWorkspaceEventHandlers();
			this.workspace = null;

			DetachMainEventHandlers();
			this.main = null;

			DetachUserSettingsEventHandlers();
		}

		#endregion

		#region Controls Event Handlers
		//==========================================================================================
		// Controls Event Handlers
		//==========================================================================================

		#region Controls Event Handlers > Main Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu
		//------------------------------------------------------------------------------------------

		#region Controls Event Handlers > Main Menu > File
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > File
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_File_SetChildMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				bool childIsReady = (ActiveMdiChild != null);
				toolStripMenuItem_MainMenu_File_CloseAll.Enabled = childIsReady;
				toolStripMenuItem_MainMenu_File_SaveAll.Enabled  = childIsReady;
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
		private void toolStripMenuItem_MainMenu_File_SetRecentMenuItems()
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_MainMenu_File_Recent.Enabled = (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_File_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_File_SetChildMenuItems();
			toolStripMenuItem_MainMenu_File_SetRecentMenuItems();

			this.isSettingControls.Enter(); // This is a workaround to a limitation of WinForms.
			try                             // See remarks of the terminal's _DropDownOpening().
			{
				var t = (ActiveMdiChild as Terminal);
				t.toolStripMenuItem_TerminalMenu_File_DropDownOpening(sender, e);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_File_New_Click(object sender, EventArgs e)
		{
			ShowNewTerminalDialog();
		}

		private void toolStripMenuItem_MainMenu_File_Open_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void toolStripMenuItem_MainMenu_File_CloseAll_Click(object sender, EventArgs e)
		{
			this.workspace.CloseAllTerminals();
		}

		private void toolStripMenuItem_MainMenu_File_SaveAll_Click(object sender, EventArgs e)
		{
			this.workspace.SaveAllTerminals();
		}

		#region Controls Event Handlers > Main Menu > File > Workspace
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > File > Workspace
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time workspace status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_File_Workspace_SetMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				bool workspaceIsReady = (this.workspace != null);

				bool workspaceFileIsReadOnly = false;
				if (workspaceIsReady)
					workspaceFileIsReadOnly = this.workspace.SettingsFileIsReadOnly;

				toolStripMenuItem_MainMenu_File_Workspace_Close.Enabled  = workspaceIsReady;
				toolStripMenuItem_MainMenu_File_Workspace_Save.Enabled   = workspaceIsReady && !workspaceFileIsReadOnly;
				toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Enabled = workspaceIsReady;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_File_Workspace_SetMenuItems();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_New_Click(object sender, EventArgs e)
		{
			this.main.CreateNewWorkspace();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_Open_Click(object sender, EventArgs e)
		{
			ShowOpenWorkspaceFileDialog();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_Close_Click(object sender, EventArgs e)
		{
			this.workspace.Close();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_Save_Click(object sender, EventArgs e)
		{
			this.workspace.Save();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_SaveAs_Click(object sender, EventArgs e)
		{
			ShowSaveWorkspaceAsFileDialog();
		}

		#endregion

		private void toolStripMenuItem_MainMenu_File_Preferences_Click(object sender, EventArgs e)
		{
			ShowPreferences();
		}

		private void toolStripMenuItem_MainMenu_File_Exit_Click(object sender, EventArgs e)
		{
			Close();
		}

		#endregion

		#region Controls Event Handlers > Main Menu > Terminal
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Terminal
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_Terminal_SetChildMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				bool childIsReady = (ActiveMdiChild != null);

				toolStripMenuItem_MainMenu_Terminal_AllClear.Enabled   = childIsReady;
				toolStripMenuItem_MainMenu_Terminal_AllRefresh.Enabled = childIsReady;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_Terminal_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_Terminal_SetChildMenuItems();

			this.isSettingControls.Enter(); // This is a workaround to a limitation of WinForms.
			try                             // See remarks of the terminal's _DropDownOpening().
			{
				var t = (ActiveMdiChild as Terminal);
				t.toolStripMenuItem_TerminalMenu_Terminal_DropDownOpening(sender, e);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_Terminal_AllClear_Click(object sender, EventArgs e)
		{
			this.workspace.AllClear();
		}

		private void toolStripMenuItem_MainMenu_Terminal_AllRefresh_Click(object sender, EventArgs e)
		{
			this.workspace.AllRefresh();
		}

		#endregion

		#region Controls Event Handlers > Main Menu > Log
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Log
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_Log_SetChildMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				bool childIsReady = (ActiveMdiChild != null);
				var t = (ActiveMdiChild as Terminal);

				// Attention:
				// Similar code exists in Terminal.toolStripMenuItem_TerminalMenu_Log_SetMenuItems().
				// Changes here may have to be applied there too.

				bool logIsOn = false;
				if (t != null)
					logIsOn = t.LogIsOn;

				bool allLogsAreOn = false;
				if (t != null)
					allLogsAreOn = t.AllLogsAreOn;

				toolStripMenuItem_MainMenu_Log_AllOn.Enabled    = (childIsReady && !allLogsAreOn);
				toolStripMenuItem_MainMenu_Log_AllOff.Enabled   = (childIsReady &&       logIsOn);
				toolStripMenuItem_MainMenu_Log_AllClear.Enabled = (childIsReady &&       logIsOn);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_Log_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_Log_SetChildMenuItems();

			this.isSettingControls.Enter(); // This is a workaround to a limitation of WinForms.
			try                             // See remarks of the terminal's _DropDownOpening().
			{
				var t = (ActiveMdiChild as Terminal);
				t.toolStripMenuItem_TerminalMenu_Log_DropDownOpening(sender, e);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_Log_AllOn_Click(object sender, EventArgs e)
		{
			this.workspace.AllLogOn();
		}

		private void toolStripMenuItem_MainMenu_Log_AllOff_Click(object sender, EventArgs e)
		{
			this.workspace.AllLogOff();
		}

		private void toolStripMenuItem_MainMenu_Log_AllClear_Click(object sender, EventArgs e)
		{
			this.workspace.AllLogClear();
		}

		#endregion

		#region Controls Event Handlers > Main Menu > Window
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Window
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_Window_SetMainMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Checked = ApplicationSettings.LocalUserSettings.MainWindow.AlwaysOnTop;

			#if (WITH_SCRIPTING)
			////toolStripMenuItem_MainMenu_Script_Panel.Checked = ApplicationSettings.RoamingUserSettings.View.ScriptPanelIsVisible;
				toolStripMenuItem_MainMenu_Script_Panel.Checked = this.scriptDialogIsOpen;
			#endif
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
		private void toolStripMenuItem_MainMenu_Window_SetChildMenuItems()
		{
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems(false);
		}

		private void toolStripMenuItem_MainMenu_Window_SetChildMenuItems(bool isDropDownOpening)
		{
			bool childIsReady = (ActiveMdiChild != null);

			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_MainMenu_Window_Automatic.Enabled      = childIsReady;
				toolStripMenuItem_MainMenu_Window_Cascade.Enabled        = childIsReady;
				toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = childIsReady;
				toolStripMenuItem_MainMenu_Window_TileVertical.Enabled   = childIsReady;
				toolStripMenuItem_MainMenu_Window_Minimize.Enabled       = childIsReady;
				toolStripMenuItem_MainMenu_Window_Maximize.Enabled       = childIsReady;
			}
			finally
			{
				this.isSettingControls.Leave();
			}

			// This is a workaround to the following bugs:
			//  > #119 "MDI child list isn't always updated"
			//  > #180 "Menu update of the terminal settings"
			//  > #213 "Wrong indication 'COM 1 - Closed'"
			//
			// Attention, only apply the workaround in case of the event below!
			// Otherwise, ActivateMdiChild(f) -> SetChildControls() will recurse here!
			if (isDropDownOpening)
			{
			#if (FALSE)
				// \fixme:
				// I don't know how to fix bug #31 "MDI window list invisible if no MDI children".
				// The following code doesn't fix it. Probably a .NET bug... Added to limitations.
				if (childIsReady)
				{
					menuStrip_Main.MdiWindowListItem = toolStripMenuItem_MainMenu_Window;
				}
				else
				{
					menuStrip_Main.MdiWindowListItem = null;
					- and/or -
					menuStrip_Main.MdiWindowListItem = toolStripMenuItem_MainMenu_Dummy;
					- and/or -
					toolStripMenuItem_MainMenu_Window.Invalidate();
					- and/or -
					ActivateMdiChild(null);
				}
			#else
				if (childIsReady)
				{
					var f = this.ActiveMdiChild;
					ActivateMdiChild(null);
					ActivateMdiChild(f);
				}
			#endif
			}
		}

		private void toolStripMenuItem_MainMenu_Window_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_Window_SetMainMenuItems();
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems(true);
		}

		private void toolStripMenuItem_MainMenu_Window_AlwaysOnTop_Click(object sender, EventArgs e)
		{
			ApplicationSettings.LocalUserSettings.MainWindow.AlwaysOnTop = !ApplicationSettings.LocalUserSettings.MainWindow.AlwaysOnTop;
			ApplicationSettings.SaveLocalUserSettings();
		}

		private void toolStripMenuItem_MainMenu_Window_Automatic_Click(object sender, EventArgs e)
		{
			SetTerminalLayout(WorkspaceLayout.Automatic);
		}

		private void toolStripMenuItem_MainMenu_Window_Cascade_Click(object sender, EventArgs e)
		{
			SetTerminalLayout(WorkspaceLayout.Cascade);
		}

		private void toolStripMenuItem_MainMenu_Window_TileHorizontal_Click(object sender, EventArgs e)
		{
			SetTerminalLayout(WorkspaceLayout.TileHorizontal);
		}

		private void toolStripMenuItem_MainMenu_Window_TileVertical_Click(object sender, EventArgs e)
		{
			SetTerminalLayout(WorkspaceLayout.TileVertical);
		}

		private void toolStripMenuItem_MainMenu_Window_Minimize_Click(object sender, EventArgs e)
		{
			SetTerminalLayout(WorkspaceLayout.Minimize);
		}

		private void toolStripMenuItem_MainMenu_Window_Maximize_Click(object sender, EventArgs e)
		{
			SetTerminalLayout(WorkspaceLayout.Maximize);
		}

		#endregion

	#if (WITH_SCRIPTING)

		#region Controls Event Handlers > Main Menu > Script
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Script
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_MainMenu_Script_Panel_Click(object sender, EventArgs e)
		{
			ToggleScriptDialog();
		}

		#endregion

	#endif // WITH_SCRIPTING

		#region Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_Contents_Click(object sender, EventArgs e)
		{
			var f = new Help();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

	#if !(WITH_SCRIPTING)

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			var f = new ReleaseNotes();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestSupport_Click(object sender, EventArgs e)
		{
			var f = new FeedbackInstructions(FeedbackType.Support);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestFeature_Click(object sender, EventArgs e)
		{
			var f = new FeedbackInstructions(FeedbackType.Feature);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_SubmitBug_Click(object sender, EventArgs e)
		{
			var f = new FeedbackInstructions(FeedbackType.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_AnyOtherFeedback_Click(object sender, EventArgs e)
		{
			var f = new FeedbackInstructions(FeedbackType.AnyOther);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_Update_Click(object sender, EventArgs e)
		{
			var link = "https://sourceforge.net/p/y-a-terminal/files/";
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(this, link);
		}

		[ModalBehaviorContract(ModalBehavior.Never)]
		private void toolStripMenuItem_MainMenu_Help_Donate_Click(object sender, EventArgs e)
		{
			var link = "https://sourceforge.net/p/y-a-terminal/donate/";
			LinkHelper.TryBrowseUriAndShowErrorIfItFails(this, link);
		}

	#endif // WITH_SCRIPTING

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_About_Click(object sender, EventArgs e)
		{
			var f = new About();
			ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this);
		}

		#endregion

		#endregion

		#region Controls Event Handlers > Toolbar
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Toolbar
		//------------------------------------------------------------------------------------------

		private void toolStripButton_MainTool_SetControls()
		{
			this.isSettingControls.Enter();
			try
			{
				bool childIsReady = (ActiveMdiChild != null);

				bool terminalFileIsReadOnly = false;
				if (childIsReady)
					terminalFileIsReadOnly = ((Terminal)ActiveMdiChild).SettingsFileIsReadOnly;

				bool terminalIsStopped = false;
				if (childIsReady)
					terminalIsStopped = ((Terminal)ActiveMdiChild).IsStopped;

				bool terminalIsStarted = false;
				if (childIsReady)
					terminalIsStarted = ((Terminal)ActiveMdiChild).IsStarted;

				bool radixIsReady = false;
				var radix = Domain.Radix.None;
				if (childIsReady)
				{
					var t = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
					if ((t != null) && (!t.IsInDisposal))
					{
						radixIsReady = !(t.SettingsRoot.Display.SeparateTxRxRadix);
						if (radixIsReady)
							radix = t.SettingsRoot.Display.TxRadix;
					}
				}

				toolStripButton_MainTool_File_Save.Enabled = (childIsReady && !terminalFileIsReadOnly);

				toolStripButton_MainTool_Terminal_Start   .Enabled = (childIsReady && terminalIsStopped);
				toolStripButton_MainTool_Terminal_Stop    .Enabled = (childIsReady && terminalIsStarted);
				toolStripButton_MainTool_Terminal_Settings.Enabled =  childIsReady;

				toolStripButton_MainTool_Radix_String .Enabled = (childIsReady && radixIsReady);
				toolStripButton_MainTool_Radix_Char   .Enabled = (childIsReady && radixIsReady);
				toolStripButton_MainTool_Radix_Bin    .Enabled = (childIsReady && radixIsReady);
				toolStripButton_MainTool_Radix_Oct    .Enabled = (childIsReady && radixIsReady);
				toolStripButton_MainTool_Radix_Dec    .Enabled = (childIsReady && radixIsReady);
				toolStripButton_MainTool_Radix_Hex    .Enabled = (childIsReady && radixIsReady);
				toolStripButton_MainTool_Radix_Unicode.Enabled = (childIsReady && radixIsReady);

				toolStripButton_MainTool_Radix_String .Checked = (radix == Domain.Radix.String);
				toolStripButton_MainTool_Radix_Char   .Checked = (radix == Domain.Radix.Char);
				toolStripButton_MainTool_Radix_Bin    .Checked = (radix == Domain.Radix.Bin);
				toolStripButton_MainTool_Radix_Oct    .Checked = (radix == Domain.Radix.Oct);
				toolStripButton_MainTool_Radix_Dec    .Checked = (radix == Domain.Radix.Dec);
				toolStripButton_MainTool_Radix_Hex    .Checked = (radix == Domain.Radix.Hex);
				toolStripButton_MainTool_Radix_Unicode.Checked = (radix == Domain.Radix.Unicode);

				toolStripButton_MainTool_Terminal_Clear          .Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_Refresh        .Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_CopyToClipboard.Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_SaveToFile     .Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_Print          .Enabled = childIsReady;

				toolStripButton_MainTool_SetFindControls(childIsReady); // See remark of that method.

				bool logIsOn = false;
				if (childIsReady)
					logIsOn = ((Terminal)ActiveMdiChild).LogIsOn;

				bool logFileExists = false;
				if (childIsReady)
					logFileExists = ((Terminal)ActiveMdiChild).LogFileExists;

				toolStripButton_MainTool_Log_Settings     .Enabled =  childIsReady;
				toolStripButton_MainTool_Log_On           .Enabled = (childIsReady && !logIsOn);
				toolStripButton_MainTool_Log_Off          .Enabled = (childIsReady &&  logIsOn);
				toolStripButton_MainTool_Log_OpenFile     .Enabled = (childIsReady &&  logFileExists);
			////toolStripButton_MainTool_Log_OpenDirectory.Enabled shall always be active, default folder can be opened even if child is inactive.

				toolStripButton_MainTool_SetAutoActionControls(  childIsReady); // See remark of that method.
				toolStripButton_MainTool_SetAutoResponseControls(childIsReady); // See remark of that method.

			#if (WITH_SCRIPTING)
			////if (ApplicationSettings.RoamingUserSettings.View.ScriptPanelIsVisible)
				if (this.scriptDialogIsOpen)
				{
					toolStripButton_MainTool_Script_ShowHide.Checked = true;
					toolStripButton_MainTool_Script_ShowHide.Text = "Hide Script Panel";
				}
				else
				{
					toolStripButton_MainTool_Script_ShowHide.Checked = false;
					toolStripButton_MainTool_Script_ShowHide.Text = "Show Script Panel";
				}
			#endif // WITH_SCRIPTING

				toolStripButton_MainTool_Terminal_Format.Enabled = childIsReady;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Separated to prevent flickering of other text/combo controls when editing the search pattern.
		/// </remarks>
		private void toolStripButton_MainTool_SetFindControls(bool childIsReady)
		{
			this.isSettingControls.Enter();
			try
			{
				DebugFindEnter(MethodBase.GetCurrentMethod().Name);
				{
					if (ApplicationSettings.RoamingUserSettings.View.FindIsVisible)
					{
						toolStripButton_MainTool_Find_ShowHide.Checked = true;
						toolStripButton_MainTool_Find_ShowHide.Text = "Hide Find";

						if (!this.mainToolValidationWorkaround_UpdateIsSuspended)
						{
							var activePattern = ApplicationSettings.RoamingUserSettings.Find.ActivePattern;
							var recentPatterns = ApplicationSettings.RoamingUserSettings.Find.RecentPatterns.ToArray();

							ToolStripComboBoxHelper.UpdateItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_Find_Pattern, recentPatterns);
							ToolStripComboBoxHelper.Select(                              toolStripComboBox_MainTool_Find_Pattern, activePattern, activePattern);
							toolStripComboBox_MainTool_Find_Pattern.Visible = true; // Note that pattern shall also be editable when no terminal is open.
						}

						SetFindStateAndControls();

						toolStripButton_MainTool_Find_CaseSensitive.Checked = ApplicationSettings.RoamingUserSettings.Find.Options.CaseSensitive;
						toolStripButton_MainTool_Find_CaseSensitive.Enabled = childIsReady;
						toolStripButton_MainTool_Find_CaseSensitive.Visible = true;
						toolStripButton_MainTool_Find_WholeWord    .Checked = ApplicationSettings.RoamingUserSettings.Find.Options.WholeWord;
						toolStripButton_MainTool_Find_WholeWord    .Enabled = childIsReady;
						toolStripButton_MainTool_Find_WholeWord    .Visible = true;
						toolStripButton_MainTool_Find_EnableRegex  .Checked = ApplicationSettings.RoamingUserSettings.Find.Options.EnableRegex;
						////                          EnableRegex  .Enabled = true (always true).
						toolStripButton_MainTool_Find_EnableRegex  .Visible = true;

						toolStripButton_MainTool_Find_Next    .Enabled = childIsReady;
						toolStripButton_MainTool_Find_Next    .Visible = true;
						toolStripButton_MainTool_Find_Previous.Enabled = childIsReady;
						toolStripButton_MainTool_Find_Previous.Visible = true;
					}
					else
					{
						toolStripButton_MainTool_Find_ShowHide.Checked = false;
						toolStripButton_MainTool_Find_ShowHide.Text = "Show Find";

						ToolStripComboBoxHelper.Deselect                           (toolStripComboBox_MainTool_Find_Pattern);
						ToolStripComboBoxHelper.ClearItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_Find_Pattern);
						toolStripComboBox_MainTool_Find_Pattern.Visible = false;

						toolStripButton_MainTool_Find_CaseSensitive.Visible = false;
						toolStripButton_MainTool_Find_WholeWord    .Visible = false;
						toolStripButton_MainTool_Find_EnableRegex  .Visible = false;

						toolStripButton_MainTool_Find_Next    .Visible = false;
						toolStripButton_MainTool_Find_Previous.Visible = false;
					}
				}
				DebugFindLeave();
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Separated to prevent flickering of other text/combo controls when editing the trigger.
		/// </remarks>
		private void toolStripButton_MainTool_SetAutoActionControls(bool childIsReady)
		{
			this.isSettingControls.Enter();
			try
			{
				bool isActive                 = false;

				AutoTriggerEx[]  triggerItems = AutoTriggerEx.GetFixedItems();
				AutoTriggerEx    trigger      = (AutoTriggerEx)AutoTrigger.None;
				AutoContentState triggerState = AutoContentState.Neutral;

				bool triggerTextIsSupported  = false;
				bool triggerRegexIsSupported = false;

				AutoActionEx[]   actionItems = AutoActionEx.GetItems();
				AutoActionEx     action      = AutoAction.None;
			////AutoContentState actionState = AutoContentState.Neutral is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

				if (childIsReady)
				{
					var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
					if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
					{
						isActive                = activeTerminal.SettingsRoot.AutoAction.IsActive;

						var tis = new List<AutoTriggerEx>(activeTerminal.SettingsRoot.GetValidAutoActionTriggerItems());    // Common and recent patterns/triggers are always shown, though they
						tis.AddRange(AutoTriggerEx.CommonRegexCapturePatterns.Select(x => new AutoTriggerEx(x)).ToArray()); // could be limited depending on the options, but that is incomprehensive.
						tis.AddRange(ApplicationSettings.RoamingUserSettings.AutoAction.RecentExplicitTriggers.ConvertAll(new Converter<RecentItem<string>, AutoTriggerEx>((x) => { return (x.Item); })));
						triggerItems            = tis.ToArray();
						trigger                 = activeTerminal.SettingsRoot.AutoAction.Trigger;
						triggerState            = ((Terminal)ActiveMdiChild).AutoActionTriggerState;

						triggerTextIsSupported  = trigger.TextIsSupported;
						triggerRegexIsSupported = trigger.RegexIsSupported;

					////var ais = new List<AutoActionEx>(activeTerminal.SettingsRoot.GetValidAutoActionItems()); is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
					////ais.AddRange(AutoActionEx.CommonActions.Select(x => new AutoActionEx(x)).ToArray());
					////ais.AddRange(ApplicationSettings.RoamingUserSettings.AutoAction.RecentExplicitActions.ConvertAll(new Converter<RecentItem<string>, AutoActionEx>((x) => { return (x.Item); })));
						actionItems             = activeTerminal.SettingsRoot.GetValidAutoActionItems();
						action                  = activeTerminal.SettingsRoot.AutoAction.Action;
					////actionState             = ((Terminal)ActiveMdiChild).AutoActionActionState is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
					}
				}

				toolStripButton_MainTool_AutoAction_ShowHide.Checked = isActive;

				if (ApplicationSettings.RoamingUserSettings.View.AutoActionIsVisible)
				{
					toolStripButton_MainTool_AutoAction_ShowHide.Text = "Hide Automatic Action";

					// Attention:
					// Similar code exists in the following location:
					//  > View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
					// Changes here may have to be applied there too.

					ToolStripComboBoxHelper.UpdateItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoAction_Trigger, triggerItems);
					ToolStripComboBoxHelper.Select(                              toolStripComboBox_MainTool_AutoAction_Trigger, trigger, trigger);
					toolStripComboBox_MainTool_AutoAction_Trigger.Enabled = childIsReady;
					toolStripComboBox_MainTool_AutoAction_Trigger.Visible = true;

					ToolStripComboBoxHelper.UpdateItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoAction_Action, actionItems);
					ToolStripComboBoxHelper.Select(                              toolStripComboBox_MainTool_AutoAction_Action, action, action);
					toolStripComboBox_MainTool_AutoAction_Action.Enabled = childIsReady;
					toolStripComboBox_MainTool_AutoAction_Action.Visible = true;

					SetAutoActionTriggerStateControls(triggerState);
				////SetAutoActionActionStateControls(actionState) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

					SetAutoActionTriggerOptionControls(childIsReady, triggerTextIsSupported, triggerRegexIsSupported);
				////SetAutoActionActionOptionControls() is not needed (yet) because there are no such options (yet).

					toolStripButton_MainTool_SetAutoActionCount();

					toolStripLabel_MainTool_AutoAction_Count.Enabled = isActive;
					toolStripLabel_MainTool_AutoAction_Count.Visible = true;

					toolStripButton_MainTool_AutoAction_Deactivate.Enabled = (trigger.IsActive || action.IsActive);
					toolStripButton_MainTool_AutoAction_Deactivate.Visible =  true;
				}
				else
				{
					toolStripButton_MainTool_AutoAction_ShowHide.Text = "Show Automatic Action";

					ToolStripComboBoxHelper.Deselect                           (toolStripComboBox_MainTool_AutoAction_Trigger);
					ToolStripComboBoxHelper.ClearItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoAction_Trigger);
					toolStripComboBox_MainTool_AutoAction_Trigger            .Visible = false;

					ToolStripComboBoxHelper.Deselect                           (toolStripComboBox_MainTool_AutoAction_Action);
					ToolStripComboBoxHelper.ClearItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoAction_Action);
					toolStripComboBox_MainTool_AutoAction_Action             .Visible = false;

					toolStripButton_MainTool_AutoAction_Trigger_UseText      .Visible = false;
					toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Visible = false;
					toolStripButton_MainTool_AutoAction_Trigger_WholeWord    .Visible = false;
					toolStripButton_MainTool_AutoAction_Trigger_EnableRegex  .Visible = false;

					toolStripLabel_MainTool_AutoAction_Count                 .Visible = false;
					toolStripButton_MainTool_AutoAction_Deactivate           .Visible = false;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoActionTriggerStateControls(AutoContentState state)
		{
			this.isSettingControls.Enter();
			try
			{
				switch (state)
				{
					case AutoContentState.Neutral:
						toolStripComboBox_MainTool_AutoAction_Trigger.BackColor = SystemColors.Window;
						toolStripComboBox_MainTool_AutoAction_Trigger.ForeColor = SystemColors.WindowText;
						break;

					case AutoContentState.Invalid:
						toolStripComboBox_MainTool_AutoAction_Trigger.BackColor = SystemColors.ControlDark;
						toolStripComboBox_MainTool_AutoAction_Trigger.ForeColor = SystemColors.ControlText;
						break;

					default:
						throw (new ArgumentOutOfRangeException("state", state, MessageHelper.InvalidExecutionPreamble + "'" + state + "' is an automatic content state that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Required to allow/disallow changing options while editing a not yet validated trigger.
		/// The 'TextChanged' event will allow, the 'Leave' event (for explicit triggers) or the
		/// 'SelectedIndexChanged' event (for predefined triggers) will again disallow.
		/// </remarks>
		private void SetAutoActionTriggerOptionControls(bool childIsReady, bool triggerTextIsSupported, bool triggerRegexIsSupported)
		{
			bool triggerCaseSensitive = false;
			bool triggerWholeWord     = false;
			bool triggerUseText       = false;
			bool triggerEnableRegex   = false;

			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
				{
					triggerUseText          = activeTerminal.SettingsRoot.AutoAction.TriggerOptions.UseText;
					triggerCaseSensitive    = activeTerminal.SettingsRoot.AutoAction.TriggerOptions.CaseSensitive;
					triggerWholeWord        = activeTerminal.SettingsRoot.AutoAction.TriggerOptions.WholeWord;
					triggerEnableRegex      = activeTerminal.SettingsRoot.AutoAction.TriggerOptions.EnableRegex;
				}
			}

			this.isSettingControls.Enter();
			try
			{
				toolStripButton_MainTool_AutoAction_Trigger_UseText.Checked = (triggerTextIsSupported && triggerUseText);
				toolStripButton_MainTool_AutoAction_Trigger_UseText.Enabled =  triggerTextIsSupported;
				toolStripButton_MainTool_AutoAction_Trigger_UseText.Visible =  true;

				toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Checked = (triggerTextIsSupported && triggerUseText && triggerCaseSensitive);
				toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Enabled = (triggerTextIsSupported && triggerUseText);
				toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive.Visible =  true;

				toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Checked = (triggerTextIsSupported && triggerUseText && triggerWholeWord);
				toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Enabled = (triggerTextIsSupported && triggerUseText);
				toolStripButton_MainTool_AutoAction_Trigger_WholeWord.Visible =  true;

				toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex);
				toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported);
				toolStripButton_MainTool_AutoAction_Trigger_EnableRegex.Visible =  true;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ResetAutoActionTriggerOptionControls(bool childIsReady)
		{
			bool triggerTextIsSupported  = false;
			bool triggerRegexIsSupported = false;

			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
				{
					var trigger = activeTerminal.SettingsRoot.AutoAction.Trigger;

					triggerTextIsSupported  = trigger.TextIsSupported;
					triggerRegexIsSupported = trigger.RegexIsSupported;
				}
			}

			SetAutoActionTriggerOptionControls(childIsReady, triggerTextIsSupported, triggerRegexIsSupported);
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerUseTextAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoActionTriggerUseText();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerCaseSensitiveAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoActionTriggerCaseSensitive();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerWholeWordAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoActionTriggerWholeWord();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoActionTriggerEnableRegexAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoActionTriggerEnableRegex();
			RevalidateAndRequestAutoActionTrigger();
		}

		/// <remarks>
		/// Separated to prevent flickering of other text/combo controls when editing trigger/response.
		/// </remarks>
		private void toolStripButton_MainTool_SetAutoResponseControls(bool childIsReady)
		{
			this.isSettingControls.Enter();
			try
			{
				bool isActive                  = false;

				AutoTriggerEx[]  triggerItems  = AutoTriggerEx.GetFixedItems();
				AutoTriggerEx    trigger       = (AutoTriggerEx)AutoTrigger.None;
				AutoContentState triggerState  = AutoContentState.Neutral;

				bool triggerTextIsSupported     = false;
				bool triggerRegexIsSupported    = false;

				AutoResponseEx[] responseItems  = AutoResponseEx.GetFixedItems();
				AutoResponseEx   response       = (AutoResponseEx)AutoResponse.None;
				AutoContentState responseState  = AutoContentState.Neutral;

				bool responseReplaceIsSupported = false;

				if (childIsReady)
				{
					var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
					if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
					{
						isActive                   = activeTerminal.SettingsRoot.AutoResponse.IsActive;

						var tis = new List<AutoTriggerEx>(activeTerminal.SettingsRoot.GetValidAutoResponseTriggerItems());  // Common and recent patterns/triggers are always shown, though they
						tis.AddRange(AutoTriggerEx.CommonRegexCapturePatterns.Select(x => new AutoTriggerEx(x)).ToArray()); // could be limited depending on the options, but that is incomprehensive.
						tis.AddRange(ApplicationSettings.RoamingUserSettings.AutoResponse.RecentExplicitTriggers.ConvertAll(new Converter<RecentItem<string>, AutoTriggerEx>((x) => { return (x.Item); })));
						triggerItems               = tis.ToArray();
						trigger                    = activeTerminal.SettingsRoot.AutoResponse.Trigger;
						triggerState               = ((Terminal)ActiveMdiChild).AutoResponseTriggerState;

						triggerTextIsSupported     = trigger.TextIsSupported;
						triggerRegexIsSupported    = trigger.RegexIsSupported;

						var ris = new List<AutoResponseEx>(activeTerminal.SettingsRoot.GetValidAutoResponseItems(Path.GetDirectoryName(activeTerminal.SettingsFilePath)));
						ris.AddRange(AutoResponseEx.CommonRegexReplacementPatterns.Select(x => new AutoResponseEx(x)).ToArray());
						ris.AddRange(ApplicationSettings.RoamingUserSettings.AutoResponse.RecentExplicitResponses.ConvertAll(new Converter<RecentItem<string>, AutoResponseEx>((x) => { return (x.Item); })));
						responseItems              = ris.ToArray();
						response                   = activeTerminal.SettingsRoot.AutoResponse.Response;
						responseState              = ((Terminal)ActiveMdiChild).AutoResponseResponseState;

						responseReplaceIsSupported = response.ReplaceIsSupported;
					}
				}

				toolStripButton_MainTool_AutoResponse_ShowHide.Checked = isActive;

				if (ApplicationSettings.RoamingUserSettings.View.AutoResponseIsVisible)
				{
					toolStripButton_MainTool_AutoResponse_ShowHide.Text = "Hide Automatic Response";

					// Attention:
					// Similar code exists in...
					// ...View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
					// Changes here may have to be applied there too.

					ToolStripComboBoxHelper.UpdateItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoResponse_Trigger, triggerItems);
					ToolStripComboBoxHelper.Select(                              toolStripComboBox_MainTool_AutoResponse_Trigger, trigger, trigger);
					toolStripComboBox_MainTool_AutoResponse_Trigger.Enabled = childIsReady;
					toolStripComboBox_MainTool_AutoResponse_Trigger.Visible = true;

					ToolStripComboBoxHelper.UpdateItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoResponse_Response, responseItems);
					ToolStripComboBoxHelper.Select(                              toolStripComboBox_MainTool_AutoResponse_Response, response, response);
					toolStripComboBox_MainTool_AutoResponse_Response.Enabled = childIsReady;
					toolStripComboBox_MainTool_AutoResponse_Response.Visible = true;

					SetAutoResponseTriggerStateControls(triggerState);
					SetAutoResponseResponseStateControls(responseState);

					SetAutoResponseTriggerOptionControls(childIsReady, triggerTextIsSupported, triggerRegexIsSupported);
					SetAutoResponseResponseOptionControls(childIsReady, triggerTextIsSupported, triggerRegexIsSupported, responseReplaceIsSupported);

					toolStripButton_MainTool_SetAutoResponseCount();

					toolStripLabel_MainTool_AutoResponse_Count.Enabled = isActive;
					toolStripLabel_MainTool_AutoResponse_Count.Visible = true;

					toolStripButton_MainTool_AutoResponse_Deactivate.Enabled = (trigger.IsActive || response.IsActive);
					toolStripButton_MainTool_AutoResponse_Deactivate.Visible =  true;
				}
				else
				{
					toolStripButton_MainTool_AutoResponse_ShowHide.Text = "Show Automatic Response";

					ToolStripComboBoxHelper.Deselect                           (toolStripComboBox_MainTool_AutoResponse_Trigger);
					ToolStripComboBoxHelper.ClearItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoResponse_Trigger);
					toolStripComboBox_MainTool_AutoResponse_Trigger             .Visible = false;

					ToolStripComboBoxHelper.Deselect                           (toolStripComboBox_MainTool_AutoResponse_Response);
					ToolStripComboBoxHelper.ClearItemsKeepingCursorAndSelection(toolStripComboBox_MainTool_AutoResponse_Response);
					toolStripComboBox_MainTool_AutoResponse_Response            .Visible = false;

					toolStripButton_MainTool_AutoResponse_Trigger_UseText       .Visible = false;
					toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive .Visible = false;
					toolStripButton_MainTool_AutoResponse_Trigger_WholeWord     .Visible = false;
					toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex   .Visible = false;

					toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Visible = false;

					toolStripLabel_MainTool_AutoResponse_Count                  .Visible = false;
					toolStripButton_MainTool_AutoResponse_Deactivate            .Visible = false;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoResponseTriggerStateControls(AutoContentState state)
		{
			this.isSettingControls.Enter();
			try
			{
				switch (state)
				{
					case AutoContentState.Neutral:
						toolStripComboBox_MainTool_AutoResponse_Trigger.BackColor = SystemColors.Window;
						toolStripComboBox_MainTool_AutoResponse_Trigger.ForeColor = SystemColors.WindowText;
						break;

					case AutoContentState.Invalid:
						toolStripComboBox_MainTool_AutoResponse_Trigger.BackColor = SystemColors.ControlDark;
						toolStripComboBox_MainTool_AutoResponse_Trigger.ForeColor = SystemColors.ControlText;
						break;

					default:
						throw (new ArgumentOutOfRangeException("state", state, MessageHelper.InvalidExecutionPreamble + "'" + state + "' is an automatic content state that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetAutoResponseResponseStateControls(AutoContentState state)
		{
			this.isSettingControls.Enter();
			try
			{
				switch (state)
				{
					case AutoContentState.Neutral:
						toolStripComboBox_MainTool_AutoResponse_Response.BackColor = SystemColors.Window;
						toolStripComboBox_MainTool_AutoResponse_Response.ForeColor = SystemColors.WindowText;
						break;

					case AutoContentState.Invalid:
						toolStripComboBox_MainTool_AutoResponse_Response.BackColor = SystemColors.ControlDark;
						toolStripComboBox_MainTool_AutoResponse_Response.ForeColor = SystemColors.ControlText;
						break;

					default:
						throw (new ArgumentOutOfRangeException("state", state, MessageHelper.InvalidExecutionPreamble + "'" + state + "' is an automatic content state that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		/// <remarks>
		/// Required to allow/disallow changing options while editing a not yet validated trigger.
		/// The 'TextChanged' event will allow, the 'Leave' event (for explicit triggers) or the
		/// 'SelectedIndexChanged' event (for predefined triggers) will again disallow.
		/// </remarks>
		private void SetAutoResponseTriggerOptionControls(bool childIsReady, bool triggerTextIsSupported, bool triggerRegexIsSupported)
		{
			bool triggerUseText       = false;
			bool triggerCaseSensitive = false;
			bool triggerWholeWord     = false;
			bool triggerEnableRegex   = false;

			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
				{
					triggerUseText       = activeTerminal.SettingsRoot.AutoResponse.TriggerOptions.UseText;
					triggerCaseSensitive = activeTerminal.SettingsRoot.AutoResponse.TriggerOptions.CaseSensitive;
					triggerWholeWord     = activeTerminal.SettingsRoot.AutoResponse.TriggerOptions.WholeWord;
					triggerEnableRegex   = activeTerminal.SettingsRoot.AutoResponse.TriggerOptions.EnableRegex;
				}
			}

			this.isSettingControls.Enter();
			try
			{
				toolStripButton_MainTool_AutoResponse_Trigger_UseText.Checked = (triggerTextIsSupported && triggerUseText);
				toolStripButton_MainTool_AutoResponse_Trigger_UseText.Enabled =  triggerTextIsSupported;
				toolStripButton_MainTool_AutoResponse_Trigger_UseText.Visible =  true;

				toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Checked = (triggerTextIsSupported && triggerUseText && triggerCaseSensitive);
				toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Enabled = (triggerTextIsSupported && triggerUseText);
				toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive.Visible =  true;

				toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Checked = (triggerTextIsSupported && triggerUseText && triggerWholeWord);
				toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Enabled = (triggerTextIsSupported && triggerUseText);
				toolStripButton_MainTool_AutoResponse_Trigger_WholeWord.Visible =  true;

				toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex);
				toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported);
				toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex.Visible =  true;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ResetAutoResponseTriggerOptionControls(bool childIsReady)
		{
			bool triggerTextIsSupported  = false;
			bool triggerRegexIsSupported = false;

			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
				{
					var trigger = activeTerminal.SettingsRoot.AutoResponse.Trigger;

					triggerTextIsSupported  = trigger.TextIsSupported;
					triggerRegexIsSupported = trigger.RegexIsSupported;
				}
			}

			SetAutoResponseTriggerOptionControls(childIsReady, triggerTextIsSupported, triggerRegexIsSupported);
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerUseTextAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoResponseTriggerEnableRegex();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerCaseSensitiveAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoResponseTriggerCaseSensitive();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerWholeWordAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoResponseTriggerWholeWord();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseTriggerEnableRegexAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoResponseTriggerEnableRegex();
			RevalidateAndRequestAutoResponseTrigger();
		}

		/// <remarks>
		/// Required to allow/disallow changing options while editing a not yet validated trigger.
		/// The 'TextChanged' event will allow, the 'Leave' event (for explicit triggers) or the
		/// 'SelectedIndexChanged' event (for predefined triggers) will again disallow.
		/// </remarks>
		private void SetAutoResponseResponseOptionControls(bool childIsReady, bool triggerTextIsSupported, bool triggerRegexIsSupported, bool responseReplaceIsSupported)
		{
			bool triggerUseText        = false;
			bool triggerEnableRegex    = false;
			bool responseEnableReplace = false;

			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
				{
					triggerUseText        = activeTerminal.SettingsRoot.AutoResponse.TriggerOptions.UseText;
					triggerEnableRegex    = activeTerminal.SettingsRoot.AutoResponse.TriggerOptions.EnableRegex;
					responseEnableReplace = activeTerminal.SettingsRoot.AutoResponse.ResponseOptions.EnableReplace;
				}
			}

			this.isSettingControls.Enter();
			try
			{
				toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Checked = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex && responseReplaceIsSupported && responseEnableReplace);
				toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Enabled = (triggerTextIsSupported && triggerUseText && triggerRegexIsSupported && triggerEnableRegex && responseReplaceIsSupported);
				toolStripButton_MainTool_AutoResponse_Response_EnableReplace.Visible =  true;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void ResetAutoResponseResponseOptionControls(bool childIsReady)
		{
			bool triggerTextIsSupported     = false;
			bool triggerRegexIsSupported    = false;
			bool responseReplaceIsSupported = false;

			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
				{
					var trigger  = activeTerminal.SettingsRoot.AutoResponse.Trigger;
					var response = activeTerminal.SettingsRoot.AutoResponse.Response;

					triggerTextIsSupported     = trigger.TextIsSupported;
					triggerRegexIsSupported    = trigger.RegexIsSupported;
					responseReplaceIsSupported = response.ReplaceIsSupported;
				}
			}

			SetAutoResponseResponseOptionControls(childIsReady, triggerTextIsSupported, triggerRegexIsSupported, responseReplaceIsSupported);
		}

		/// <summary></summary>
		protected virtual void RequestToggleAutoResponseResponseEnableReplaceAndRevalidate()
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoResponseResponseEnableReplace();
			RevalidateAndRequestAutoResponseResponse();
		}

		/// <remarks>
		/// Separated to prevent flickering of text/combo controls.
		/// </remarks>
		private void toolStripButton_MainTool_SetAutoActionCount()
		{
			var count = 0;

			var childIsReady = (ActiveMdiChild != null);
			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
					count = activeTerminal.AutoActionCount;
			}

			toolStripLabel_MainTool_AutoAction_Count.Text = string.Format(CultureInfo.CurrentCulture, "({0})", count);
		}

		/// <remarks>
		/// Separated to prevent flickering of text/combo controls.
		/// </remarks>
		private void toolStripButton_MainTool_SetAutoResponseCount()
		{
			int count = 0;

			var childIsReady = (ActiveMdiChild != null);
			if (childIsReady)
			{
				var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((activeTerminal != null) && (!activeTerminal.IsInDisposal))
					count = activeTerminal.AutoResponseCount;
			}

			toolStripLabel_MainTool_AutoResponse_Count.Text = string.Format(CultureInfo.CurrentCulture, "({0})", count);
		}

		private void toolStripButton_MainTool_File_New_Click(object sender, EventArgs e)
		{
			ShowNewTerminalDialog();
		}

		private void toolStripButton_MainTool_File_Open_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void toolStripButton_MainTool_File_Save_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestSaveFile();
		}

		private void toolStripButton_MainTool_File_SaveWorkspace_Click(object sender, EventArgs e)
		{
			this.workspace.Save();
		}

		private void toolStripButton_MainTool_Terminal_Start_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestStartTerminal();
		}

		private void toolStripButton_MainTool_Terminal_Stop_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestStopTerminal();
		}

		private void toolStripButton_MainTool_Terminal_Settings_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestEditTerminalSettings();
		}

		private void toolStripButton_MainTool_Radix_String_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.String);
		}

		private void toolStripButton_MainTool_Radix_Char_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Char);
		}

		private void toolStripButton_MainTool_Radix_Bin_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Bin);
		}

		private void toolStripButton_MainTool_Radix_Oct_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Oct);
		}

		private void toolStripButton_MainTool_Radix_Dec_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Dec);
		}

		private void toolStripButton_MainTool_Radix_Hex_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Hex);
		}

		private void toolStripButton_MainTool_Radix_Unicode_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Unicode);
		}

		private void toolStripButton_MainTool_Terminal_Clear_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestClear();
		}

		private void toolStripButton_MainTool_Terminal_Refresh_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRefresh();
		}

		private void toolStripButton_MainTool_Terminal_CopyToClipboard_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestCopyToClipboard();
		}

		private void toolStripButton_MainTool_Terminal_SaveToFile_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestSaveToFile();
		}

		private void toolStripButton_MainTool_Terminal_Print_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestPrint();
		}

		private void toolStripButton_MainTool_Find_ShowHide_Click(object sender, EventArgs e)
		{
			ApplicationSettings.RoamingUserSettings.View.FindIsVisible = !ApplicationSettings.RoamingUserSettings.View.FindIsVisible;
			ApplicationSettings.SaveRoamingUserSettings();

			if (ApplicationSettings.RoamingUserSettings.View.FindIsVisible)
			{
			////toolStripComboBox_MainTool_Terminal_Find_Pattern.Select();    doesn't work, 'ToolStrip'
				toolStripComboBox_MainTool_Find_Pattern.Focus(); // seems to require calling Focus().
			}
		}

		private void toolStripComboBox_MainTool_Find_Pattern_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ValidateAndFindOnEdit();
		}

		private void toolStripComboBox_MainTool_Find_Pattern_Enter(object sender, EventArgs e)
		{
			DebugFindEnter(MethodBase.GetCurrentMethod().Name);
			SuspendFindShortcutsCtrlFNP();       // Suspend while in find field.
			SuspendEditShortcutsCtrlACVDelete(); // Suspend while in find field.
			EnterFindOnEdit();
			DebugFindLeave();
		}

		private void toolStripComboBox_MainTool_Find_Pattern_Leave(object sender, EventArgs e)
		{
			DebugFindEnter(MethodBase.GetCurrentMethod().Name);
			LeaveFindOnEdit(toolStripComboBox_MainTool_Find_Pattern.Text);
			ResumeEditShortcutsCtrlACVDelete(); // Suspended while in find field.
			ResumeFindShortcutsCtrlFNP();       // Suspended while in find field.
			DebugFindLeave();
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because of 'FindOnEdit()'.
		/// </remarks>
		private void toolStripComboBox_MainTool_Find_Pattern_TextChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				if (toolStripComboBox_MainTool_Find_Pattern.SelectedIndex == ControlEx.InvalidIndex)
				{
					var pattern = toolStripComboBox_MainTool_Find_Pattern.Text;
					if (!string.IsNullOrEmpty(pattern))
					{
						if (!TryValidateFindPattern(pattern))
						{
							SetFindStateAndControls(FindDirection.Undetermined, FindResult.Invalid);
							return; // Skip 'FindOnEdit' in case of invalid (regex) pattern.
						}

						this.mainToolValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							FindOnEdit(pattern); // No need to ValidateAndFindOnEdit(pattern) since just validated above.
						}
						finally
						{
							this.mainToolValidationWorkaround_UpdateIsSuspended = false;
						}
					}
					else
					{
						EmptyFindOnEdit();
					}
				}
			}
		}

		private void toolStripComboBox_MainTool_Find_Pattern_KeyDown(object sender, KeyEventArgs e)
		{
			// \remind (2017-11-22..12-16 / MKY) there are limitations in .NET WinForms (or Win32):
			//
			// [Severe]
			// Setting 'e.Handled' is *not* sufficient, e.g. [Alt+W] would still activate the
			// 'Window' menu. Only setting 'e.SuppressKeyPress' works!
			//
			// [Acceptable]
			// Even if all [Alt] events were suppressed by...
			//    default: e.SuppressKeyPress = true; break;
			// ...the underlines still become visible in the menu bar.
			// Not critical, Visual Studio has exactly the same problem ;-)
			//
			// [Severe]
			// Only shortcuts that are not active elsewhere can be used here.
			// Therefore, the [Ctrl+F/N/P] shortcuts are suspended while in find field.
			//  => Suspend/ResumeCtrlFNPShortcuts() above and in terminals.
			//
			// [Severe]
			// The ToolStripComboBox 'Leave' event is fired as soon as a MsgBox is shown (e.g.
			// "no more found"). As a consequence, the other shortcuts are already active again.
			//  => Not showing a "no more found" message box anymore.

			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				ValidateAndFindNext(); // [Enter] shall always be active.
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.C: ToggleFindCaseSensitive(); e.SuppressKeyPress = true; break;
					case Keys.W: ToggleFindWholeWord();     e.SuppressKeyPress = true; break;
					case Keys.E: ToggleFindEnableRegex();   e.SuppressKeyPress = true; break;

					case Keys.F:               // Additional shortcuts shall be executable under same conditions as normal shortcuts.
					case Keys.N: if (FindNextIsFeasible)     { ValidateAndFindNext();     } e.SuppressKeyPress = true; break;
					case Keys.P: if (FindPreviousIsFeasible) { ValidateAndFindPrevious(); } e.SuppressKeyPress = true; break;

					default: break;
				}
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Control)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.F:               // Additional shortcuts shall be executable under same conditions as normal shortcuts.
					case Keys.N: if (FindNextIsFeasible)     { ValidateAndFindNext();     } e.SuppressKeyPress = true; break;
					case Keys.P: if (FindPreviousIsFeasible) { ValidateAndFindPrevious(); } e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		/// <remarks>
		/// Suppress same keys for symmetricity with 'KeyDown' above.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		private void toolStripComboBox_MainTool_Find_Pattern_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.C: e.SuppressKeyPress = true; break;
					case Keys.W: e.SuppressKeyPress = true; break;
					case Keys.E: e.SuppressKeyPress = true; break;

					case Keys.F:
					case Keys.N: e.SuppressKeyPress = true; break;
					case Keys.P: e.SuppressKeyPress = true; break;

					default: break;
				}
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Control)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.F:
					case Keys.N: e.SuppressKeyPress = true; break;
					case Keys.P: e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		private void SetFindStateAndControls()
		{
			SetFindStateAndControls(this.findDirection, this.findResult);
		}

		private void SetFindStateAndControls(FindDirection direction, FindResult result)
		{
			this.findDirection = direction;
			this.findResult    = result;

			switch (result)
			{
				case FindResult.Reset:
					toolStripComboBox_MainTool_Find_Pattern.BackColor = SystemColors.Window;
					toolStripComboBox_MainTool_Find_Pattern.ForeColor = SystemColors.WindowText;
					toolStripButton_MainTool_Find_Next     .Enabled   = this.findNextIsFeasible     = true;
					toolStripButton_MainTool_Find_Previous .Enabled   = this.findPreviousIsFeasible = true;
					break;

				case FindResult.Empty:
					toolStripComboBox_MainTool_Find_Pattern.BackColor = SystemColors.Window;
					toolStripComboBox_MainTool_Find_Pattern.ForeColor = SystemColors.WindowText;
					toolStripButton_MainTool_Find_Next     .Enabled   = this.findNextIsFeasible     = false;
					toolStripButton_MainTool_Find_Previous .Enabled   = this.findPreviousIsFeasible = false;
					break;

				case FindResult.Found:
					toolStripComboBox_MainTool_Find_Pattern.BackColor = SystemColors.Window;     // Same colors as
					toolStripComboBox_MainTool_Find_Pattern.ForeColor = SystemColors.WindowText; // 'Reset' above.
					toolStripButton_MainTool_Find_Next     .Enabled   = this.findNextIsFeasible     = true;
					toolStripButton_MainTool_Find_Previous .Enabled   = this.findPreviousIsFeasible = true;
					break;

				case FindResult.NotFoundAnymore:
					toolStripComboBox_MainTool_Find_Pattern.BackColor = SystemColors.Highlight;     // Same color as last found line,
					toolStripComboBox_MainTool_Find_Pattern.ForeColor = SystemColors.HighlightText; // which is also 'highlighted'.
					toolStripButton_MainTool_Find_Next     .Enabled   = this.findNextIsFeasible     = (direction != FindDirection.Forward);
					toolStripButton_MainTool_Find_Previous .Enabled   = this.findPreviousIsFeasible = (direction != FindDirection.Backward);
					break;

				case FindResult.NotFoundAtAll:
					toolStripComboBox_MainTool_Find_Pattern.BackColor = SystemColors.Info;
					toolStripComboBox_MainTool_Find_Pattern.ForeColor = SystemColors.InfoText;
					toolStripButton_MainTool_Find_Next     .Enabled   = this.findNextIsFeasible     = false;
					toolStripButton_MainTool_Find_Previous .Enabled   = this.findPreviousIsFeasible = false;
					break;

				case FindResult.Invalid:
					toolStripComboBox_MainTool_Find_Pattern.BackColor = SystemColors.ControlDark;
					toolStripComboBox_MainTool_Find_Pattern.ForeColor = SystemColors.ControlText;
					toolStripButton_MainTool_Find_Next     .Enabled   = this.findNextIsFeasible     = false;
					toolStripButton_MainTool_Find_Previous .Enabled   = this.findPreviousIsFeasible = false;
					break;

				default:
					throw (new ArgumentOutOfRangeException("result", result, MessageHelper.InvalidExecutionPreamble + "'" + result + "' is a find result that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		/// <summary></summary>
		protected virtual void EnterFindOnEdit()
		{
			ValidateAndFindOnEdit();
		}

		/// <summary></summary>
		protected virtual void ValidateAndFindOnEdit()
		{
			var pattern = toolStripComboBox_MainTool_Find_Pattern.Text;
			if (!string.IsNullOrEmpty(pattern))
			{
				if (ValidateFindPattern(pattern))
				{
					FindOnEdit(pattern);
				}
				else
				{
					SetFindStateAndControls(FindDirection.Undetermined, FindResult.Invalid);
				}
			}
			else // Opposed to FindNext/Previous(), an "empty" FindOnEdit() shall result in 'Reset'.
			{
				EmptyFindOnEdit();
			}
		}

		/// <summary></summary>
		protected virtual void FindOnEdit(string pattern)
		{
			if (!string.IsNullOrEmpty(pattern))
			{
				var fd = FindDirection.Undetermined;
				var fr = FindResult.Reset;

				var t = (ActiveMdiChild as Terminal);
				if (t != null)
					fr = t.TryFindOnEdit(pattern, out fd);

				SetFindStateAndControls(fd, fr);
			}
			else
			{
				EmptyFindOnEdit();
			}
		}

		/// <summary></summary>
		protected virtual void EmptyFindOnEdit()
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.EmptyFindOnEdit();

			SetFindStateAndControls(FindDirection.Undetermined, FindResult.Empty);
		}

		/// <summary></summary>
		protected virtual void LeaveFindOnEdit(string pattern)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.LeaveFindOnEdit(pattern);

			SetFindStateAndControls(FindDirection.Undetermined, FindResult.Reset);
		}

		private void toolStripButton_MainTool_Find_Next_Click(object sender, EventArgs e)
		{
			ValidateAndFindNext();
		}

		/// <summary></summary>
		protected virtual void ValidateAndFindNext()
		{
			var pattern = toolStripComboBox_MainTool_Find_Pattern.Text;
			if (!string.IsNullOrEmpty(pattern) && ValidateFindPattern(pattern))
			{
				var fr = FindResult.Reset;

				var t = (ActiveMdiChild as Terminal);
				if (t != null)
					fr = t.TryFindNext(pattern, MessageBoxIsPermissible);

				SetFindStateAndControls(FindDirection.Forward, fr);
			}
			else
			{
				SetFindStateAndControls(FindDirection.Forward, FindResult.Invalid);
			}
		}

		private void toolStripButton_MainTool_Find_Previous_Click(object sender, EventArgs e)
		{
			ValidateAndFindPrevious();
		}

		/// <summary></summary>
		protected virtual void ValidateAndFindPrevious()
		{
			var pattern = toolStripComboBox_MainTool_Find_Pattern.Text;
			if (ValidateFindPattern(pattern))
			{
				var fr = FindResult.Reset;

				var t = (ActiveMdiChild as Terminal);
				if (t != null)
					fr = t.TryFindPrevious(pattern, MessageBoxIsPermissible);

				SetFindStateAndControls(FindDirection.Backward, fr);
			}
			else
			{
				SetFindStateAndControls(FindDirection.Backward, FindResult.Invalid);
			}
		}

		/// <summary></summary>
		protected virtual bool MessageBoxIsPermissible
		{
			get
			{
				if (ApplicationSettings.RoamingUserSettings.View.FindIsVisible)
					return (!this.toolStripComboBox_MainTool_Find_Pattern.Focused);
				else
					return (false);
			}
		}

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		protected virtual bool ValidateFindPattern(string pattern)
		{
			string errorMessage;
			if (TryValidateFindPattern(pattern, out errorMessage))
			{
				return (true);
			}
			else
			{
				if (MessageBoxIsPermissible)
				{
					var caption = new StringBuilder("Invalid Find ");
					if (ApplicationSettings.RoamingUserSettings.Find.Options.EnableRegex)
						caption.Append("Pattern");
					else
						caption.Append("Text");

					MessageBoxEx.Show
					(
						this,
						errorMessage,
						caption.ToString(),
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}

				return (false);
			}
		}

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		protected virtual bool TryValidateFindPattern(string pattern)
		{
			string errorMessage;
			return (TryValidateFindPattern(pattern, out errorMessage));
		}

		/// <remarks>
		/// Using "pattern" instead of "textOrPattern" for simplicity.
		/// </remarks>
		protected virtual bool TryValidateFindPattern(string pattern, out string errorMessage)
		{
			if (ApplicationSettings.RoamingUserSettings.Find.Options.EnableRegex)
			{
				return (RegexEx.TryValidatePattern(pattern, out errorMessage));
			}
			else // Not using regex:
			{
				errorMessage = null;
				return (true);
			}
		}

		private void toolStripButton_MainTool_Find_CaseSensitive_Click(object sender, EventArgs e)
		{
			ToggleFindCaseSensitive();
		}

		/// <summary></summary>
		protected virtual void ToggleFindCaseSensitive()
		{
			var options = ApplicationSettings.RoamingUserSettings.Find.Options;
			options.CaseSensitive = !options.CaseSensitive; // Settings member must be changed to let the changed event be raised!
			ApplicationSettings.RoamingUserSettings.Find.Options = options;
			ApplicationSettings.SaveRoamingUserSettings();

			ValidateAndFindOnEdit();
		}

		private void toolStripButton_MainTool_Find_WholeWord_Click(object sender, EventArgs e)
		{
			ToggleFindWholeWord();
		}

		/// <summary></summary>
		protected virtual void ToggleFindWholeWord()
		{
			var options = ApplicationSettings.RoamingUserSettings.Find.Options;
			options.WholeWord = !options.WholeWord; // Settings member must be changed to let the changed event be raised!
			ApplicationSettings.RoamingUserSettings.Find.Options = options;
			ApplicationSettings.SaveRoamingUserSettings();

			ValidateAndFindOnEdit();
		}

		private void toolStripButton_MainTool_Find_EnableRegex_Click(object sender, EventArgs e)
		{
			ToggleFindEnableRegex();
		}

		/// <summary></summary>
		protected virtual void ToggleFindEnableRegex()
		{
			var options = ApplicationSettings.RoamingUserSettings.Find.Options;
			options.EnableRegex = !options.EnableRegex; // Settings member must be changed to let the changed event be raised!
			ApplicationSettings.RoamingUserSettings.Find.Options = options;
			ApplicationSettings.SaveRoamingUserSettings();

			ValidateAndFindOnEdit();
		}

		private void toolStripButton_MainTool_Log_Settings_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestEditLogSettings();
		}

		private void toolStripButton_MainTool_Log_On_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestSwitchLogOn();
		}

		private void toolStripButton_MainTool_Log_Off_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestSwitchLogOff();
		}

		private void toolStripButton_MainTool_Log_Open_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestOpenLogFile();
		}

		private void toolStripButton_MainTool_Log_OpenDirectory_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestOpenLogDirectory();
			else
				OpenDefaultLogDirectory();
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that all 7 potential exceptions of 'Directory.CreateDirectory()' and 'Path.GetDirectoryName()' are handled.")]
		private void OpenDefaultLogDirectory()
		{
			// Attention:
			// Similar code exists in Model.Terminal.OpenLogDirectory().
			// Changes here may have to be applied there too.

			string rootPath = ApplicationSettings.LocalUserSettings.Paths.LogFiles;

			// Create directory if not existing yet:
			if (!Directory.Exists(Path.GetDirectoryName(rootPath)))
			{
				try
				{
					Directory.CreateDirectory(Path.GetDirectoryName(rootPath));
				}
				catch (Exception exCreate)
				{
					string message = "Unable to create folder." + Environment.NewLine + Environment.NewLine +
					                    "System error message:" + Environment.NewLine + exCreate.Message;

					MessageBoxEx.Show
					(
						this,
						message,
						"Folder Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}

			// Open directory:
			Exception exBrowse;
			if (!DirectoryEx.TryBrowse(rootPath, out exBrowse))
			{
				MessageBoxEx.Show
				(
					this,
					ErrorHelper.ComposeMessage("Unable to open log folder", rootPath, exBrowse),
					"Log Folder Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error
				);
			}
		}

		private void toolStripButton_MainTool_AutoAction_ShowHide_Click(object sender, EventArgs e)
		{
			ApplicationSettings.RoamingUserSettings.View.AutoActionIsVisible = !ApplicationSettings.RoamingUserSettings.View.AutoActionIsVisible;
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void toolStripComboBox_MainTool_AutoAction_Trigger_DropDown(object sender, EventArgs e)
		{
			SetAutoActionChildControls(); // Needed to refresh trigger validation.
		}

		private void toolStripComboBox_MainTool_AutoAction_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_MainTool_AutoAction_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
			{
				if (((Terminal)ActiveMdiChild).RequestAutoActionValidateTrigger(trigger))
				{
					if (trigger.IsExplicit)
						((Terminal)ActiveMdiChild).RequestAutoActionAdjustTriggerOptionsSilently(trigger);

					((Terminal)ActiveMdiChild).ActivateAutoActionTrigger(trigger);
				}
				else
				{
					SetAutoActionChildControls(); // Revert trigger.
				}
			}
		}

		private void toolStripComboBox_MainTool_AutoAction_Trigger_Enter(object sender, EventArgs e)
		{
			SuspendEditShortcutsCtrlACVDelete(); // Suspend while in trigger field.

			this.autoActionTriggerValidationIsOngoing = false;
		}

		/// <remarks>
		/// Note that this 'Leave' event also has a particular behavior:
		/// <list type="bullet">
		/// <item><description>On [Tab] jumping to the first option, the event is invoked immediately.</description></item>
		/// <item><description>Directly clicking an option with the mouse, the event is not invoked!</description></item>
		/// </list>
		/// </remarks>
		private void toolStripComboBox_MainTool_AutoAction_Trigger_Leave(object sender, EventArgs e)
		{
			if (toolStripComboBox_MainTool_AutoAction_Trigger.SelectedIndex != ControlEx.InvalidIndex)
				ResetAutoActionTriggerOptionControls((ActiveMdiChild != null));

			if (!this.autoActionTriggerValidationIsOngoing) // Revalidation may already be ongoing triggered by clicking on option.
				RevalidateAndRequestAutoActionTrigger();

			ResumeEditShortcutsCtrlACVDelete(); // Suspended while in trigger field.
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// Directly using the underlying <see cref="ToolStripComboBox.ComboBox"/>'es event doesn't help either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'es' relates to preceeding tag.")]
		private void toolStripComboBox_MainTool_AutoAction_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_MainTool_AutoAction_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				SetAutoActionTriggerOptionControls((ActiveMdiChild != null), true, true); // Allow changing options while editing a not yet validated trigger!

				var triggerTextOrRegexPattern = toolStripComboBox_MainTool_AutoAction_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					if (!((Terminal)ActiveMdiChild).RequestAutoActionValidateTriggerTextSilently(triggerTextOrRegexPattern))
					{
						((Terminal)ActiveMdiChild).AutoActionTriggerState = AutoContentState.Invalid;
						return; // Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).
				}
			}

			((Terminal)ActiveMdiChild).AutoActionTriggerState = AutoContentState.Neutral;
		}

		/// <remarks>
		/// See remark at 'TextChanged' event handler above.
		/// </remarks>
		private void RevalidateAndRequestAutoActionTrigger()
		{
			var selectedIndex = toolStripComboBox_MainTool_AutoAction_Trigger.SelectedIndex;
			var selectedItem = (toolStripComboBox_MainTool_AutoAction_Trigger.SelectedItem as AutoTriggerEx);
			                  //// Not listed             or                            listed explicit tigger.
			if ((selectedIndex == ControlEx.InvalidIndex) || ((selectedItem != null) && selectedItem.IsExplicit))
			{
				var triggerTextOrRegexPattern = toolStripComboBox_MainTool_AutoAction_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					int invalidTextStart;
					int invalidTextLength;

					this.autoActionTriggerValidationIsOngoing = true;
					var success = ((Terminal)ActiveMdiChild).RequestAutoActionValidateTriggerText(triggerTextOrRegexPattern, out invalidTextStart, out invalidTextLength);
					this.autoActionTriggerValidationIsOngoing = false;

					if (!success)
					{
						SetAutoActionTriggerOptionControls((ActiveMdiChild != null), true, true); // Allow changing options while editing a not yet validated trigger!
						((Terminal)ActiveMdiChild).AutoActionTriggerState = AutoContentState.Invalid;
						toolStripComboBox_MainTool_AutoAction_Trigger.Focus();
						toolStripComboBox_MainTool_AutoAction_Trigger.Select(invalidTextStart, invalidTextLength);
						return;
					}
				}

				((Terminal)ActiveMdiChild).ActivateAutoActionTrigger(triggerTextOrRegexPattern);
			}

			((Terminal)ActiveMdiChild).AutoActionTriggerState = AutoContentState.Neutral;
		}

		private void toolStripComboBox_MainTool_AutoAction_Trigger_KeyDown(object sender, KeyEventArgs e)
		{
			// Note the \remind (2017-11-22..12-16 / MKY) limitations in .NET WinForms (or Win32)
			// described in toolStripComboBox_MainTool_Find_Pattern_KeyDown() further above.

			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				RevalidateAndRequestAutoActionTrigger();
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.T: RequestToggleAutoActionTriggerUseTextAndRevalidate();       e.SuppressKeyPress = true; break;
					case Keys.C: RequestToggleAutoActionTriggerCaseSensitiveAndRevalidate(); e.SuppressKeyPress = true; break;
					case Keys.W: RequestToggleAutoActionTriggerWholeWordAndRevalidate();     e.SuppressKeyPress = true; break;
					case Keys.E: RequestToggleAutoActionTriggerEnableRegexAndRevalidate();   e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		/// <remarks>
		/// Suppress same keys for symmetricity with 'KeyDown' above.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		private void toolStripComboBox_MainTool_AutoAction_Trigger_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.T: e.SuppressKeyPress = true; break;
					case Keys.C: e.SuppressKeyPress = true; break;
					case Keys.W: e.SuppressKeyPress = true; break;
					case Keys.E: e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		private void toolStripButton_MainTool_AutoAction_Trigger_UseText_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerUseTextAndRevalidate();
		}

		private void toolStripButton_MainTool_AutoAction_Trigger_CaseSensitive_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerCaseSensitiveAndRevalidate();
		}

		private void toolStripButton_MainTool_AutoAction_Trigger_WholeWord_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerWholeWordAndRevalidate();
		}

		private void toolStripButton_MainTool_AutoAction_Trigger_EnableRegex_Click(object sender, EventArgs e)
		{
			RequestToggleAutoActionTriggerEnableRegexAndRevalidate();
		}

		private void toolStripComboBox_MainTool_AutoAction_Action_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var action = (toolStripComboBox_MainTool_AutoAction_Action.SelectedItem as AutoActionEx);
			if (action != null)
			{
				if (((Terminal)ActiveMdiChild).RequestAutoActionValidateAction(action))
					((Terminal)ActiveMdiChild).ActivateAutoActionAction(action);
				else
					SetAutoActionChildControls(); // Revert action.
			}
		}

	////private void toolStripComboBox_MainTool_AutoAction_Action_Enter(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private void toolStripComboBox_MainTool_AutoAction_Action_Leave(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private void toolStripComboBox_MainTool_AutoAction_Action_TextChanged(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
	////private void SetAutoActionActionState(AutoContentState state)                                     is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

		private void toolStripLabel_MainTool_AutoAction_Count_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestAutoActionResetCount();
		}

		private void toolStripButton_MainTool_AutoAction_Deactivate_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestAutoActionDeactivate();
		}

		private void toolStripButton_MainTool_AutoResponse_ShowHide_Click(object sender, EventArgs e)
		{
			ApplicationSettings.RoamingUserSettings.View.AutoResponseIsVisible = !ApplicationSettings.RoamingUserSettings.View.AutoResponseIsVisible;
			ApplicationSettings.SaveRoamingUserSettings();
		}

		private void toolStripComboBox_MainTool_AutoResponse_Trigger_DropDown(object sender, EventArgs e)
		{
			SetAutoResponseChildControls(); // Needed to refresh trigger validation.
		}

		private void toolStripComboBox_MainTool_AutoResponse_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_MainTool_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
			{
			////if (((Terminal)ActiveMdiChild).RequestAutoResponseValidateTrigger(trigger)) is not needed (yet).
				{
					if (trigger.IsExplicit)
						((Terminal)ActiveMdiChild).RequestAutoResponseAdjustTriggerOptionsSilently(trigger);

					((Terminal)ActiveMdiChild).ActivateAutoResponseTrigger(trigger);
				}
			////else
			////{
			////	SetAutoResponseChildControls(); // Revert trigger.
			////}
			}
		}

		private void toolStripComboBox_MainTool_AutoResponse_Trigger_Enter(object sender, EventArgs e)
		{
			SuspendEditShortcutsCtrlACVDelete(); // Suspend while in trigger field.

			this.autoResponseTriggerValidationIsOngoing = false;
		}

		/// <remarks>
		/// Note that this 'Leave' event also has a particular behavior:
		/// <list type="bullet">
		/// <item><description>On [Tab] jumping to the first option, the event is invoked immediately.</description></item>
		/// <item><description>Directly clicking an option with the mouse, the event is not invoked!</description></item>
		/// </list>
		/// </remarks>
		private void toolStripComboBox_MainTool_AutoResponse_Trigger_Leave(object sender, EventArgs e)
		{
			if (toolStripComboBox_MainTool_AutoResponse_Trigger.SelectedIndex != ControlEx.InvalidIndex)
				ResetAutoResponseTriggerOptionControls((ActiveMdiChild != null));

			if (!this.autoResponseTriggerValidationIsOngoing) // Revalidation may already be ongoing triggered by clicking on option.
				RevalidateAndRequestAutoResponseTrigger();

			ResumeEditShortcutsCtrlACVDelete(); // Suspended while in trigger field.
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// Directly using the underlying <see cref="ToolStripComboBox.ComboBox"/>'es event doesn't help either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'es' relates to preceeding tag.")]
		private void toolStripComboBox_MainTool_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_MainTool_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				SetAutoResponseTriggerOptionControls((ActiveMdiChild != null), true, true); // Allow changing options while editing a not yet validated trigger!

				var triggerTextOrRegexPattern = toolStripComboBox_MainTool_AutoResponse_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					if (!((Terminal)ActiveMdiChild).RequestAutoResponseValidateTriggerTextSilently(triggerTextOrRegexPattern))
					{
						((Terminal)ActiveMdiChild).AutoResponseTriggerState = AutoContentState.Invalid;
						return; // Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).
				}
			}

			((Terminal)ActiveMdiChild).AutoResponseTriggerState = AutoContentState.Neutral;
		}

		/// <remarks>
		/// See remark at 'TextChanged' event handler above.
		/// </remarks>
		private void RevalidateAndRequestAutoResponseTrigger()
		{
			var selectedIndex = toolStripComboBox_MainTool_AutoResponse_Trigger.SelectedIndex;
			var selectedItem = (toolStripComboBox_MainTool_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
			                  //// Not listed             or                            listed explicit tigger.
			if ((selectedIndex == ControlEx.InvalidIndex) || ((selectedItem != null) && selectedItem.IsExplicit))
			{
				var triggerTextOrRegexPattern = toolStripComboBox_MainTool_AutoResponse_Trigger.Text;
				if (!string.IsNullOrEmpty(triggerTextOrRegexPattern))
				{
					int invalidTextStart;
					int invalidTextLength;

					this.autoResponseTriggerValidationIsOngoing = true;
					var success = ((Terminal)ActiveMdiChild).RequestAutoResponseValidateTriggerText(triggerTextOrRegexPattern, out invalidTextStart, out invalidTextLength);
					this.autoResponseTriggerValidationIsOngoing = false;

					if (!success)
					{
						SetAutoResponseTriggerOptionControls((ActiveMdiChild != null), true, true); // Allow changing options while editing a not yet validated trigger!
						((Terminal)ActiveMdiChild).AutoResponseTriggerState = AutoContentState.Invalid;
						toolStripComboBox_MainTool_AutoResponse_Trigger.Focus();
						toolStripComboBox_MainTool_AutoResponse_Trigger.Select(invalidTextStart, invalidTextLength);
						return;
					}
				}

				((Terminal)ActiveMdiChild).ActivateAutoResponseTrigger(triggerTextOrRegexPattern);
			}

			((Terminal)ActiveMdiChild).AutoResponseTriggerState = AutoContentState.Neutral;
		}

		private void toolStripComboBox_MainTool_AutoResponse_Trigger_KeyDown(object sender, KeyEventArgs e)
		{
			// Note the \remind (2017-11-22..12-16 / MKY) limitations in .NET WinForms (or Win32)
			// described in toolStripComboBox_MainTool_Find_Pattern_KeyDown() further above.

			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				RevalidateAndRequestAutoResponseTrigger();
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.T: RequestToggleAutoResponseTriggerUseTextAndRevalidate();       e.SuppressKeyPress = true; break;
					case Keys.C: RequestToggleAutoResponseTriggerCaseSensitiveAndRevalidate(); e.SuppressKeyPress = true; break;
					case Keys.W: RequestToggleAutoResponseTriggerWholeWordAndRevalidate();     e.SuppressKeyPress = true; break;
					case Keys.E: RequestToggleAutoResponseTriggerEnableRegexAndRevalidate();   e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		/// <remarks>
		/// Suppress same keys for symmetricity with 'KeyDown' above.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		private void toolStripComboBox_MainTool_AutoResponse_Trigger_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.T: e.SuppressKeyPress = true; break;
					case Keys.C: e.SuppressKeyPress = true; break;
					case Keys.W: e.SuppressKeyPress = true; break;
					case Keys.E: e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		private void toolStripButton_MainTool_AutoResponse_Trigger_UseText_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerUseTextAndRevalidate();
		}

		private void toolStripButton_MainTool_AutoResponse_Trigger_CaseSensitive_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerCaseSensitiveAndRevalidate();
		}

		private void toolStripButton_MainTool_AutoResponse_Trigger_WholeWord_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerWholeWordAndRevalidate();
		}

		private void toolStripButton_MainTool_AutoResponse_Trigger_EnableRegex_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseTriggerEnableRegexAndRevalidate();
		}

		private void toolStripComboBox_MainTool_AutoResponse_Response_DropDown(object sender, EventArgs e)
		{
			SetAutoResponseChildControls(); // Needed to refresh response validation.
		}

		private void toolStripComboBox_MainTool_AutoResponse_Response_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var response = (toolStripComboBox_MainTool_AutoResponse_Response.SelectedItem as AutoResponseEx);
			if (response != null)
			{
			////if (((Terminal)ActiveMdiChild).RequestAutoResponseValidateResponse(response)) is not needed (yet).
				{
					if (response.IsExplicit)
						((Terminal)ActiveMdiChild).RequestAutoResponseAdjustResponseOptionsSilently(response);

					((Terminal)ActiveMdiChild).ActivateAutoResponseResponse(response);
				}
			////else
			////{
			////	SetAutoResponseChildControls(); // Revert resopnse.
			////}
			}
		}

		private void toolStripComboBox_MainTool_AutoResponse_Response_Enter(object sender, EventArgs e)
		{
			SuspendEditShortcutsCtrlACVDelete(); // Suspend while in trigger field.

			this.autoResponseResponseValidationIsOngoing = false;
		}

		/// <remarks>
		/// Note that this 'Leave' event also has a particular behavior:
		/// <list type="bullet">
		/// <item><description>On [Tab] jumping to the first option, the event is invoked immediately.</description></item>
		/// <item><description>Directly clicking an option with the mouse, the event is not invoked!</description></item>
		/// </list>
		/// </remarks>
		private void toolStripComboBox_MainTool_AutoResponse_Response_Leave(object sender, EventArgs e)
		{
			if (toolStripComboBox_MainTool_AutoResponse_Response.SelectedIndex != ControlEx.InvalidIndex)
				ResetAutoResponseResponseOptionControls((ActiveMdiChild != null));

			if (!this.autoResponseResponseValidationIsOngoing) // Revalidation may already be ongoing triggered by clicking on option.
				RevalidateAndRequestAutoResponseResponse();

			ResumeEditShortcutsCtrlACVDelete(); // Suspended while in trigger field.
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke
		/// that event way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// Directly using the underlying <see cref="ToolStripComboBox.ComboBox"/>'es event doesn't help either.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'es' relates to preceeding tag.")]
		private void toolStripComboBox_MainTool_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			if (toolStripComboBox_MainTool_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex)
			{
				SetAutoResponseResponseOptionControls((ActiveMdiChild != null), true, true, true); // Allow changing options while editing a not yet validated response!

				var responseText = toolStripComboBox_MainTool_AutoResponse_Response.Text;
				if (!string.IsNullOrEmpty(responseText))
				{
					if (!((Terminal)ActiveMdiChild).RequestAutoResponseValidateResponseTextSilently(responseText))
					{
						((Terminal)ActiveMdiChild).AutoResponseResponseState = AutoContentState.Invalid;
						return; // Likely only invalid temporarily (e.g. incomplete escape,...), thus indicating
					}           // by color and using ValidateTextSilently() (instead of error message on ValidateText()).
				}
			}

			((Terminal)ActiveMdiChild).AutoResponseResponseState = AutoContentState.Neutral;
		}

		/// <remarks>
		/// See remark at 'TextChanged' event handler above.
		/// </remarks>
		private void RevalidateAndRequestAutoResponseResponse()
		{
			var selectedIndex = toolStripComboBox_MainTool_AutoResponse_Response.SelectedIndex;
			var selectedItem = (toolStripComboBox_MainTool_AutoResponse_Response.SelectedItem as AutoTriggerEx);
			                  //// Not listed             or                            listed explicit response.
			if ((selectedIndex == ControlEx.InvalidIndex) || ((selectedItem != null) && selectedItem.IsExplicit))
			{
				var responseText = toolStripComboBox_MainTool_AutoResponse_Response.Text;
				if (!string.IsNullOrEmpty(responseText))
				{
					int invalidTextStart;
					int invalidTextLength;

					this.autoResponseResponseValidationIsOngoing = true;
					var success = ((Terminal)ActiveMdiChild).RequestAutoResponseValidateResponseText(responseText, out invalidTextStart, out invalidTextLength);
					this.autoResponseResponseValidationIsOngoing = false;

					if (!success)
					{
						SetAutoResponseResponseOptionControls((ActiveMdiChild != null), true, true, true); // Allow changing options while editing a not yet validated trigger!
						((Terminal)ActiveMdiChild).AutoResponseResponseState = AutoContentState.Invalid;
						toolStripComboBox_MainTool_AutoResponse_Response.Focus();
						toolStripComboBox_MainTool_AutoResponse_Response.Select(invalidTextStart, invalidTextLength);
						return;
					}
				}

				((Terminal)ActiveMdiChild).ActivateAutoResponseResponse(responseText);
			}

			((Terminal)ActiveMdiChild).AutoResponseResponseState = AutoContentState.Neutral;
		}

		private void toolStripComboBox_MainTool_AutoResponse_Response_KeyDown(object sender, KeyEventArgs e)
		{
			// Note the \remind (2017-11-22..12-16 / MKY) limitations in .NET WinForms (or Win32)
			// described in toolStripComboBox_MainTool_Find_Pattern_KeyDown() further above.

			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				RevalidateAndRequestAutoResponseResponse();
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.R: RequestToggleAutoResponseResponseEnableReplaceAndRevalidate(); e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		/// <remarks>
		/// Suppress same keys for symmetricity with 'KeyDown' above.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		private void toolStripComboBox_MainTool_AutoResponse_Response_KeyUp(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				e.SuppressKeyPress = true;
			}
			else if ((e.KeyData & Keys.Modifiers) == Keys.Alt)
			{
				switch (e.KeyData & Keys.KeyCode)
				{
					case Keys.R: e.SuppressKeyPress = true; break;

					default: break;
				}
			}
		}

		private void toolStripButton_MainTool_AutoResponse_Response_EnableReplace_Click(object sender, EventArgs e)
		{
			RequestToggleAutoResponseResponseEnableReplaceAndRevalidate();
		}

		private void toolStripLabel_MainTool_AutoResponse_Count_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestAutoResponseResetCount();
		}

		private void toolStripButton_MainTool_AutoResponse_Deactivate_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestAutoResponseDeactivate();
		}

	#if (WITH_SCRIPTING)

		private void toolStripButton_MainTool_Script_ShowHide_Click(object sender, EventArgs e)
		{
			ToggleScriptDialog();
		}

	#endif // WITH_SCRIPTING

		private void toolStripButton_MainTool_Terminal_Format_Click(object sender, EventArgs e)
		{
			var t = (ActiveMdiChild as Terminal);
			if (t != null)
				t.RequestEditFormatSettings();
		}

		#endregion

		#region Controls Event Handlers > Main Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Main_Opening(object sender, CancelEventArgs e)
		{
			// Prevent context menu being displayed within the child window:
			if (ActiveMdiChild != null)
			{
				e.Cancel = true;
				return;
			}

			// Update and show recent files:
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();
			try
			{
				toolStripMenuItem_MainContextMenu_File_Recent.Enabled = (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainContextMenu_File_New_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ShowNewTerminalDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_Open_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ShowOpenFileDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_OpenWorkspace_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ShowOpenWorkspaceFileDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_Exit_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			Close();
		}

		#endregion

		#region Controls Event Handlers > File Recent Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > File Recent Context Menu
		//------------------------------------------------------------------------------------------

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of related item and field name.")]
		private List<ToolStripMenuItem> menuItems_recent;

		private void contextMenuStrip_FileRecent_InitializeControls()
		{
			this.menuItems_recent = new List<ToolStripMenuItem>(RecentFileSettings.MaxFilePaths); // Preset the required capacity to improve memory management.
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_1);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_2);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_3);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_4);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_5);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_6);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_7);
			this.menuItems_recent.Add(toolStripMenuItem_FileRecentContextMenu_8);
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_FileRecent_SetRecentMenuItems()
		{
			this.isSettingControls.Enter();
			try
			{
				// Hide all:
				for (int i = 0; i < RecentFileSettings.MaxFilePaths; i++)
				{
					string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1); // 'InvariantCulture' for prefix!
					this.menuItems_recent[i].Text = "&" + prefix;
					this.menuItems_recent[i].Visible = false;
				}

				// Show valid:
				for (int i = 0; i < ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count; i++)
				{
					string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1); // 'InvariantCulture' for prefix!
					if (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i] != null)
					{
						string file = PathEx.Limit(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i].Item, 60);
						this.menuItems_recent[i].Text = "&" + prefix + file;
						this.menuItems_recent[i].Enabled = true;
					}
					else
					{
						this.menuItems_recent[i].Text = "&" + prefix;
						this.menuItems_recent[i].Enabled = false;
					}
					this.menuItems_recent[i].Visible = true;
				}
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void contextMenuStrip_FileRecent_Opening(object sender, CancelEventArgs e)
		{
		////ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll(); No need to validate again, is already done on opening of parent menu.

			contextMenuStrip_FileRecent_SetRecentMenuItems();
		}

		private void toolStripMenuItem_FileRecentContextMenu_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			this.main.OpenRecent(ToolStripMenuItemEx.TagToInt32(sender)); // Attention, 'ToolStripMenuItem' is no 'Control'!
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
				toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Checked = ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
				toolStripMenuItem_StatusContextMenu_ShowTime        .Checked = ApplicationSettings.LocalUserSettings.MainWindow.ShowTime;
				toolStripMenuItem_StatusContextMenu_ShowChrono      .Checked = ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_StatusContextMenu_ShowTerminalInfo_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo = !ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
			ApplicationSettings.SaveLocalUserSettings();
		}

		private void toolStripMenuItem_StatusContextMenu_ShowTime_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ApplicationSettings.LocalUserSettings.MainWindow.ShowTime = !ApplicationSettings.LocalUserSettings.MainWindow.ShowTime;
			ApplicationSettings.SaveLocalUserSettings();
		}

		private void toolStripMenuItem_StatusContextMenu_ShowChrono_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono = !ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
			ApplicationSettings.SaveLocalUserSettings();
		}

		private void toolStripMenuItem_StatusContextMenu_Preferences_Click(object sender, EventArgs e)
		{
		////if (ContextMenuStripShortcutModalFormWorkaround.IsCurrentlyShowingModalForm)
		////	return;    => see bug #460 "Issues with ContextMenuStripShortcutModalFormWorkaround"

			ShowPreferences();
		}

		#endregion

		#region Controls Event Handlers > Time
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Time
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Currently fixed/limited to local time "T" but could be extended to be configurable
		/// (e.g. UTC) if requested.
		/// </remarks>
		private void timer_Time_Tick(object sender, EventArgs e)
		{
			toolStripStatusLabel_MainStatus_Time.Text = DateTime.Now.ToString("T", DateTimeFormatInfo.CurrentInfo);
		}

		#endregion

		#region Controls Event Handlers > Chrono
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Chrono
		//------------------------------------------------------------------------------------------

		private void toolStripStatusLabel_MainStatus_Chrono_Click(object sender, EventArgs e)
		{
			chronometer_Main.StartStop();
		}

		private void toolStripStatusLabel_MainStatus_Chrono_DoubleClick(object sender, EventArgs e)
		{
			chronometer_Main.Stop();
			chronometer_Main.Reset();
		}

		private void chronometer_Main_TimeSpanChanged(object sender, TimeSpanEventArgs e)
		{
			toolStripStatusLabel_MainStatus_Chrono.Text = TimeSpanEx.FormatInvariantThousandthsEnforceMinutes(e.TimeSpan);
		}

		#endregion

		#endregion

		#region Window
		//==========================================================================================
		// Window
		//==========================================================================================

		private void ApplyWindowSettingsAccordingToStartupState()
		{
			// Attention:
			// Almost the same code exists in AutoActionPlot.ApplyWindowSettingsAccordingToStartupState().
			// Changes here likely have to be applied there too.

			if (ApplicationSettings.LocalUserSettingsSuccessfullyLoadedFromFile)
			{
				// Do not Suspend/ResumeLayout() when changing the form itself!

				// Window state:
				WindowState = ApplicationSettings.LocalUserSettings.MainWindow.State;

				// Start position:
				var savedStartPosition = ApplicationSettings.LocalUserSettings.MainWindow.StartPosition;
				var savedLocation      = ApplicationSettings.LocalUserSettings.MainWindow.Location; // Note the issue/limitation described
				var savedSize          = ApplicationSettings.LocalUserSettings.MainWindow.Size;     // in SaveWindowSettings() below.

				var savedBounds = new Rectangle(savedLocation, savedSize);
				var isWithinBounds = ScreenEx.IsWithinAnyBounds(savedBounds);
				if (isWithinBounds) // Restore saved settings if within bounds:
				{
					StartPosition = savedStartPosition;
					Location      = savedLocation;
					Size          = savedSize;
				}
				else // Let the operating system adjust the position if out of bounds:
				{
					StartPosition = FormStartPosition.WindowsDefaultBounds;
				}

				// Note that check must be done regardless of the window state, since the state may
				// be changed by the user at any time after the initial layout.
			}
		}

		/// <summary>
		/// Updates the window settings without saving it to the local user settings (yet).
		/// </summary>
		/// <remarks>
		/// Advantage: Prevents many save operations on resizing the form.
		/// Disadvantage: State gets lost if application crashes.
		/// </remarks>
		private void UpdateWindowSettings(bool setStartPositionToManual)
		{
			// Attention:
			// Almost the same code exists in AutoActionPlot.SaveWindowSettings().
			// Changes here likely have to be applied there too.

			if (setStartPositionToManual)
			{
				ApplicationSettings.LocalUserSettings.MainWindow.StartPosition = FormStartPosition.Manual;
				StartPosition = ApplicationSettings.LocalUserSettings.MainWindow.StartPosition;
			}

			ApplicationSettings.LocalUserSettings.MainWindow.State = WindowState;

			if (WindowState == FormWindowState.Normal)
			{
				if (StartPosition == FormStartPosition.Manual)
					ApplicationSettings.LocalUserSettings.MainWindow.Location = Location;

				ApplicationSettings.LocalUserSettings.MainWindow.Size = Size;

				// Note the following issue/limitation:
				// Windows or WinForm seems to consider the shadow around a form to belong to the form,
				// i.e. a form that is placed at a screen's edge, may tell values outside the screen.
				//
				// Example with two screens [2] [1] (where 1 is the main screen, and both screens are 1920 × 1080)
				// and the main form placed at the upper left corner, spreading across the whole screen. This may
				// result in the following [LocalUserSettings] values:
				//
				//    <Location>
				//      <X>-1924</X>
				//      <Y>2</Y>
				//    </Location>
				//    <Size>
				//      <Width>1926</Width>
				//      <Height>480</Height>
				//    </Size>
				//
				// Location.X and Size.Width are outside the screen's dimensions even though the form is inside!
				// As a consequence, MKY.Windows.Forms.ScreenEx.IsWithinAnyBounds() will wrongly determine that
				// the form doesn't fit a screen and ApplyWindowSettingsAccordingToStartup() will fall back to
				// 'FormStartPosition.WindowsDefaultBounds'.
				//
				// Issue/limitation is considered very acceptable, neither bug filed nor added to release notes.
			}

			// Don't save right now, see remarks of this method as well as 'SaveWindowSettings()' below.
		}

		private static void SaveWindowSettings()
		{
			if (ApplicationSettings.LocalUserSettings.MainWindow.HaveChanged)
				ApplicationSettings.SaveLocalUserSettings();
		}

		#endregion

		#region Preferences
		//==========================================================================================
		// Preferences
		//==========================================================================================

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowPreferences()
		{
			var f = new Preferences(ApplicationSettings.LocalUserSettings);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				ApplicationSettings.LocalUserSettings.MainWindow = f.SettingsResult.MainWindow;
				ApplicationSettings.LocalUserSettings.General    = f.SettingsResult.General;
				ApplicationSettings.SaveLocalUserSettings();
			}
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachUserSettingsEventHandlers()
		{
			if (ApplicationSettings.LocalUserSettings != null)
				ApplicationSettings.LocalUserSettings.Changed += localUserSettingsRoot_Changed;

			if (ApplicationSettings.RoamingUserSettings != null)
				ApplicationSettings.RoamingUserSettings.Changed += roamingUserSettingsRoot_Changed;
		}

		private void DetachUserSettingsEventHandlers()
		{
			if (ApplicationSettings.LocalUserSettings != null)
				ApplicationSettings.LocalUserSettings.Changed -= localUserSettingsRoot_Changed;

			if (ApplicationSettings.RoamingUserSettings != null)
				ApplicationSettings.RoamingUserSettings.Changed -= roamingUserSettingsRoot_Changed;
		}

		private void localUserSettingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do, no need to care about 'ProductVersion' and such.
			}
			else if (ReferenceEquals(e.Inner.Source, ApplicationSettings.LocalUserSettings.MainWindow))
			{
				SetMainControls();
			}
			else if (ReferenceEquals(e.Inner.Source, ApplicationSettings.LocalUserSettings.RecentFiles))
			{
				SetRecentControls();
			}
		}

		private void roamingUserSettingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			if (e.Inner == null)
			{
				// Nothing to do, no need to care about 'ProductVersion' and such.
			}
			else if (ReferenceEquals(e.Inner.Source, ApplicationSettings.RoamingUserSettings.Find))
			{
				SetFindControls();
			}
			else if (ReferenceEquals(e.Inner.Source, ApplicationSettings.RoamingUserSettings.View))
			{
				SetMainControls();
				SetChildControls(); // Child controls must also be updated when Find/AA/AR visibility changes.
			}
		}

		#endregion

		#region Main
		//==========================================================================================
		// Main
		//==========================================================================================

		#region Main > Lifetime
		//------------------------------------------------------------------------------------------
		// Main > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachMainEventHandlers()
		{
			if (this.main != null)
			{
			#if (WITH_SCRIPTING)
				this.main.Started += main_Started;
			#endif
				this.main.WorkspaceOpened += main_WorkspaceOpened;
				this.main.WorkspaceClosed += main_WorkspaceClosed;

				this.main.FixedStatusTextRequest += main_FixedStatusTextRequest;
				this.main.TimedStatusTextRequest += main_TimedStatusTextRequest;
				this.main.MessageInputRequest    += main_MessageInputRequest;
				this.main.CursorRequest          += main_CursorRequest;

				this.main.Exited += main_Exited;
			}
		}

		private void DetachMainEventHandlers()
		{
			if (this.main != null)
			{
			#if (WITH_SCRIPTING)
				this.main.Started -= main_Started;
			#endif
				this.main.WorkspaceOpened -= main_WorkspaceOpened;
				this.main.WorkspaceClosed -= main_WorkspaceClosed;

				this.main.FixedStatusTextRequest -= main_FixedStatusTextRequest;
				this.main.TimedStatusTextRequest -= main_TimedStatusTextRequest;
				this.main.MessageInputRequest    -= main_MessageInputRequest;
				this.main.CursorRequest          -= main_CursorRequest;

				this.main.Exited -= main_Exited;
			}
		}

		#endregion

		#region Main > Event Handlers
		//------------------------------------------------------------------------------------------
		// Main > Event Handlers
		//------------------------------------------------------------------------------------------

	#if (WITH_SCRIPTING)
		private void main_Started(object sender, EventArgs e)
		{
			this.main.ScriptBridge.notifyScriptDialogOpened += new ScriptBridge.ScriptDialogOpenedDelegate(NotifyScriptDialogOpened);
			this.main.ScriptBridge.notifyScriptDialogClosed += new ScriptBridge.ScriptDialogClosedDelegate(NotifyScriptDialogClosed);
			this.main.ScriptBridge.notifyScriptEnded        += new ScriptBridge.ScriptEndedDelegate(       NotitfyScriptEnded);
		}
	#endif

		private void main_WorkspaceOpened(object sender, EventArgs<Model.Workspace> e)
		{
			this.workspace = e.Value;
			AttachWorkspaceEventHandlers();

			SetWorkspaceControls();
		}

		/// <remarks>
		/// Workspace::Closed event is handled at workspace_Closed().
		/// </remarks>
		private void main_WorkspaceClosed(object sender, EventArgs e)
		{
			SetWorkspaceControls();
		}

		private void main_TimedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetTimedStatusText(e.Value);
		}

		private void main_FixedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetFixedStatusText(e.Value);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void main_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			DialogResult dr;
			dr = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
			e.Result = dr;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void main_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			Cursor = e.Value;
		}

		private void main_Exited(object sender, EventArgs<Model.MainResult> e)
		{
			if (this.result == Model.MainResult.Success) // Otherwise, keep the previous result, e.g. 'ApplicationStartError'.
				this.result = e.Value;

		#if (WITH_SCRIPTING)
			this.main.ScriptBridge.notifyScriptDialogOpened -= new ScriptBridge.ScriptDialogOpenedDelegate(NotifyScriptDialogOpened);
			this.main.ScriptBridge.notifyScriptDialogClosed -= new ScriptBridge.ScriptDialogClosedDelegate(NotifyScriptDialogClosed);
			this.main.ScriptBridge.notifyScriptEnded        -= new ScriptBridge.ScriptEndedDelegate(       NotitfyScriptEnded);
		#endif

			DetachMainEventHandlers();
			this.main = null;

			DetachUserSettingsEventHandlers();

			if (this.closingState == ClosingState.None) // Prevent multiple calls to Close().
			{
				this.closingState = ClosingState.IsClosingFromModel;
				Close();
			}
		}

		#endregion

		#region Main > Properties
		//------------------------------------------------------------------------------------------
		// Main > Properties
		//------------------------------------------------------------------------------------------

		/// <summary></summary>
		public Model.Main UnderlyingMain
		{
			get { return (this.main); }
		}

		/// <summary></summary>
		public Model.Workspace UnderlyingWorkspace
		{
			get { return (this.workspace); }
		}

		#endregion

		#region Main > Methods
		//------------------------------------------------------------------------------------------
		// Main > Methods
		//------------------------------------------------------------------------------------------

		private void FixContextMenus()
		{
			var strips = new List<ContextMenuStrip>(3); // Preset the required capacity to improve memory management.
			strips.Add(contextMenuStrip_FileRecent);
			strips.Add(contextMenuStrip_Main);
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
			InitializeRecentControls();
		}

		private void InitializeRecentControls()
		{
			contextMenuStrip_FileRecent_InitializeControls();
		}

		private void SetControls()
		{
			SetMainControls();
			SetChildControls();
			SetRecentControls();
			SetWorkspaceControls();
		}

		private void SetMainControls()
		{
			// Shortcuts associated to menu items are only active when items are visible and enabled!
			toolStripMenuItem_MainMenu_Window_SetMainMenuItems();

			toolStripButton_MainTool_SetControls(); // Contains 'Main' as well as 'Child' dependent controls.

			this.isSettingControls.Enter();
			try
			{
				timer_Time.Enabled = ApplicationSettings.LocalUserSettings.MainWindow.ShowTime;

				toolStripStatusLabel_MainStatus_TerminalInfo.Visible = ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
				toolStripStatusLabel_MainStatus_Time.Visible         = ApplicationSettings.LocalUserSettings.MainWindow.ShowTime;
				toolStripStatusLabel_MainStatus_Chrono.Visible       = ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
			}
			finally
			{
				this.isSettingControls.Leave();
			}

			TopMost = ApplicationSettings.LocalUserSettings.MainWindow.AlwaysOnTop;
		}

		/// <remarks>
		/// Separated to prevent flickering of other text/combo controls when editing the search pattern.
		/// </remarks>
		/// <remarks>
		/// 'Find' belongs to 'Main' even though the child state is required below.
		/// </remarks>
		private void SetFindControls()
		{
			bool childIsReady = (ActiveMdiChild != null);
			toolStripButton_MainTool_SetFindControls(childIsReady); // See remark above.
		}

		private void SetChildControls()
		{
			// Shortcuts associated to menu items are only active when items are visible and enabled!
			toolStripMenuItem_MainMenu_File_SetChildMenuItems();
			toolStripMenuItem_MainMenu_Terminal_SetChildMenuItems();
			toolStripMenuItem_MainMenu_Log_SetChildMenuItems();
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems();

			toolStripButton_MainTool_SetControls(); // Contains 'Main' as well as 'Child' dependent controls.
		}

		/// <remarks>
		/// Separated to prevent flickering of other text/combo controls when editing the trigger.
		/// </remarks>
		private void SetAutoActionChildControls()
		{
			bool childIsReady = (ActiveMdiChild != null);
			toolStripButton_MainTool_SetAutoActionControls(childIsReady);
		}

		private void SetAutoActionCountChildControls()
		{
			toolStripButton_MainTool_SetAutoActionCount();
		}

		/// <remarks>
		/// Separated to prevent flickering of other text/combo controls when editing the response.
		/// </remarks>
		private void SetAutoResponseChildControls()
		{
			bool childIsReady = (ActiveMdiChild != null);
			toolStripButton_MainTool_SetAutoResponseControls(childIsReady);
		}

		private void SetAutoResponseCountChildControls()
		{
			toolStripButton_MainTool_SetAutoResponseCount();
		}

		private void SetRecentControls()
		{
			// Shortcuts associated to menu items are only active when items are visible and enabled!
			toolStripMenuItem_MainMenu_File_SetRecentMenuItems();
			contextMenuStrip_FileRecent_SetRecentMenuItems();
		}

		private void SetWorkspaceControls()
		{
			// Shortcuts associated to menu items are only active when items are visible and enabled!
			toolStripMenuItem_MainMenu_File_Workspace_SetMenuItems();
		}

		private void SuspendEditShortcutsCtrlACVDelete()
		{
			// Could be implemented more cleverly, by iterating over all potential shortcut controls
			// and then handle those that use one of the shortcuts in question. However, that would
			// be an overkill, thus using this straight-forward implementation.

			foreach (var child in MdiChildren)
			{
				var t = (child as Terminal);
				if (t != null)
					t.SuspendEditShortcutsCtrlACVDelete();
			}
		}

		private void ResumeEditShortcutsCtrlACVDelete()
		{
			foreach (var child in MdiChildren)
			{
				var t = (child as Terminal);
				if (t != null)
					t.ResumeEditShortcutsCtrlACVDelete();
			}
		}

		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool toolStripMenuItem_MainMenu_File_New_EnabledToRestore; // = false;

		private void SuspendFindShortcutsCtrlFNP()
		{
			toolStripMenuItem_MainMenu_File_New_EnabledToRestore = toolStripMenuItem_MainMenu_File_New.Enabled;
			toolStripMenuItem_MainMenu_File_New.Enabled = false; // Ctrl+N

			// Could be implemented more cleverly, by iterating over all potential shortcut controls
			// and then handle those that use one of the shortcuts in question. However, that would
			// be an overkill, thus using this straight-forward implementation.

			foreach (var child in MdiChildren)
			{
				var t = (child as Terminal);
				if (t != null)
					t.SuspendFindShortcutsCtrlFNP();
			}
		}

		private void ResumeFindShortcutsCtrlFNP()
		{
			toolStripMenuItem_MainMenu_File_New.Enabled = toolStripMenuItem_MainMenu_File_New_EnabledToRestore;

			foreach (var child in MdiChildren)
			{
				var t = (child as Terminal);
				if (t != null)
					t.ResumeFindShortcutsCtrlFNP();
			}
		}

		#region Main > Methods > New
		//------------------------------------------------------------------------------------------
		// Main > Methods > New
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowNewTerminalDialog()
		{
			ShowNewTerminalDialog(ApplicationSettings.LocalUserSettings.NewTerminal);
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowNewTerminalDialog(Model.Settings.NewTerminalSettings newTerminalSettings)
		{
			SetFixedStatusText("New terminal...");

			var f = new NewTerminal(newTerminalSettings);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				ApplicationSettings.LocalUserSettings.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.SaveLocalUserSettings();

			////ResetStatusText() is not needed, Main.CreateNewTerminalFromSettings() will continue outputting.

				Refresh(); // Ensure that form has been refreshed before continuing.
				var sh = new DocumentSettingsHandler<TerminalSettingsRoot>(f.TerminalSettingsResult);
				this.main.CreateNewTerminalFromSettings(sh);
			}
			else
			{
				ResetStatusText();
			}
		}

		#endregion

		#region Main > Methods > Open File
		//------------------------------------------------------------------------------------------
		// Main > Methods > Open File
		//------------------------------------------------------------------------------------------

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenFileDialog()
		{
			SetFixedStatusText("Select a file...");

			var ofd = new OpenFileDialog();
			ofd.Title       = "Open Terminal or Workspace";
			ofd.Filter      = ExtensionHelper.TerminalOrWorkspaceFilesFilter;
			ofd.FilterIndex = ExtensionHelper.TerminalOrWorkspaceFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.TerminalExtension);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Refresh(); // Ensure that form has been refreshed before continuing.
				this.main.OpenFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		#endregion

		#endregion

		#endregion

		#region Workspace
		//==========================================================================================
		// Workspace
		//==========================================================================================

		#region Workspace > Methods
		//------------------------------------------------------------------------------------------
		// Workspace > Methods
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// This method shows a 'File Open' dialog that only allows workspace files to be selected.
		/// This is for symmetricity with 'Save Workspace' and 'Save Workspace As...'. However, it
		/// is also possible to select a workspace file using the 'normal' 'File Open' method.
		/// </remarks>
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "'Symmetricity' is a correct English term.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenWorkspaceFileDialog()
		{
			SetFixedStatusText("Select a file...");

			var ofd = new OpenFileDialog();
			ofd.Title       = "Open Workspace";
			ofd.Filter      = ExtensionHelper.WorkspaceFilesFilter;
			ofd.FilterIndex = ExtensionHelper.WorkspaceFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.WorkspaceExtension);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Refresh(); // Ensure that form has been refreshed before continuing.
				this.main.OpenFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private DialogResult ShowSaveWorkspaceAsFileDialog()
		{
			SetFixedStatusText("Select a workspace file name...");

			var sfd = new SaveFileDialog();
			sfd.Title       = "Save Workspace As";
			sfd.Filter      = ExtensionHelper.WorkspaceFilesFilter;
			sfd.FilterIndex = ExtensionHelper.WorkspaceFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.WorkspaceExtension);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;

			// Other than for terminals, workspace 'Save As' always suggests '<UserName>.yaw':
			sfd.FileName = Environment.UserName + PathEx.NormalizeExtension(sfd.DefaultExt); // Note that 'DefaultExt' states "the returned string does not include the period".

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUserSettings();

				Refresh(); // Ensure that form has been refreshed before continuing.
				this.workspace.SaveAs(sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}

			return (dr);
		}

		/// <summary>
		/// Sets the terminal layout including forwarding the setting to the workspace.
		/// </summary>
		private void SetTerminalLayout(WorkspaceLayout layout)
		{
			if (this.workspace != null)
			{
				// Only notify the workspace, so it can keep the setting. But layouting itself is done
				// here as the MDI functionality is an integral part of the Windows.Forms environment.
				this.workspace.NotifyLayout(layout);

				LayoutWorkspace(layout);
			}
		}

		private void ResetTerminalLayout()
		{
			SetTerminalLayout(Model.Settings.WorkspaceSettings.LayoutDefault);
		}

		private void ResizeWorkspace()
		{
			// Simply forward the resize request to the MDI layout engine:
			LayoutWorkspace();
		}

		/// <summary>
		/// Performs the layout operation on the workspace, i.e. the terminals.
		/// </summary>
		/// <remarks>
		/// Uses the MDI functionality of the Windows.Forms environment to perform the layout.
		/// </remarks>
		private void LayoutWorkspace()
		{
			if (this.workspace != null)
				LayoutWorkspace(this.workspace.SettingsRoot.Workspace.Layout);
		}

		/// <summary>
		/// Performs the layout operation on the workspace, i.e. the terminals.
		/// This method does not notify the workspace.
		/// </summary>
		/// <remarks>
		/// Uses the MDI functionality of the Windows.Forms environment to perform the layout.
		/// </remarks>
		private void LayoutWorkspace(WorkspaceLayout layout)
		{
			switch (layout)
			{
				case WorkspaceLayout.Automatic:
					int terminalCount = ((this.workspace != null) ? (this.workspace.TerminalCount) : (0));
					if (terminalCount <= 1)
						MaximizeActiveMdiChild();
					else
						LayoutMdi(MdiLayout.TileVertical);

					break;

				case WorkspaceLayout.Cascade:
				case WorkspaceLayout.TileHorizontal:
				case WorkspaceLayout.TileVertical:
					LayoutMdi((WorkspaceLayoutEx)layout);
					break;

				case WorkspaceLayout.Manual:
					NotifyManualLayoutingToMdi();
					break;

				case WorkspaceLayout.Minimize:
					MinimizeActiveMdiChild();
					break;

				case WorkspaceLayout.Maximize:
					MaximizeActiveMdiChild();
					break;

				default:
					throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + layout + "' is a workspace layout that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
			}
		}

		private void NotifyManualLayoutingToMdi()
		{
			foreach (var f in this.MdiChildren)
			{
				var t = (f as Terminal);
				if (t != null)
					t.NotifyWindowStateChanged();
			}
		}

		/// <summary>
		/// Arranges the multiple-document interface (MDI) child forms within the MDI parent form.
		/// </summary>
		/// <param name="value">
		/// One of the <see cref="MdiLayout"/> values that defines the layout of MDI child forms.
		/// </param>
		/// <remarks>
		/// This overridden method remembers that the MDI layout is currently ongoing. This is
		/// required when handling terminal (MDI child) layout/resize events.
		/// </remarks>
		protected new void LayoutMdi(MdiLayout value)
		{
			this.isLayoutingMdi = true;
			NotifyIntegralMdiLayoutingToMdiChildren(true);
			base.LayoutMdi(value);
			NotifyIntegralMdiLayoutingToMdiChildren(false);
			this.isLayoutingMdi = false;
		}

		private void NotifyIntegralMdiLayoutingToMdiChildren(bool isLayouting)
		{
			foreach (var f in this.MdiChildren)
			{
				var t = (f as Terminal);
				if (t != null)
					t.NotifyIntegralMdiLayouting(isLayouting);
			}
		}

		private void MinimizeActiveMdiChild()
		{
			if (ActiveMdiChild != null)
			{
				this.isLayoutingMdi = true;
				ActiveMdiChild.WindowState = FormWindowState.Minimized;
				this.isLayoutingMdi = false;
			}
		}

		private void MaximizeActiveMdiChild()
		{
			if (ActiveMdiChild != null)
			{
				this.isLayoutingMdi = true;
				ActiveMdiChild.WindowState = FormWindowState.Maximized;
				this.isLayoutingMdi = false;
			}
		}

		#endregion

		#region Workspace > Lifetime
		//------------------------------------------------------------------------------------------
		// Workspace > Lifetime
		//------------------------------------------------------------------------------------------

		private void AttachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.TerminalAdded                      += workspace_TerminalAdded;
				this.workspace.TerminalRemoved                    += workspace_TerminalRemoved;

				this.workspace.TimedStatusTextRequest             += workspace_TimedStatusTextRequest;
				this.workspace.FixedStatusTextRequest             += workspace_FixedStatusTextRequest;
				this.workspace.CursorRequest                      += workspace_CursorRequest;
				this.workspace.MessageInputRequest                += workspace_MessageInputRequest;

				this.workspace.SaveAsFileDialogRequest            += workspace_SaveAsFileDialogRequest;
				this.workspace.SaveCommandPageAsFileDialogRequest += workspace_SaveCommandPageAsFileDialogRequest;
				this.workspace.OpenCommandPageFileDialogRequest   += workspace_OpenCommandPageFileDialogRequest;

				this.workspace.Closed                             += workspace_Closed;

				if (this.workspace.SettingsRoot != null)
					this.workspace.SettingsRoot.Changed += workspaceSettingsRoot_Changed;
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.TerminalAdded                      -= workspace_TerminalAdded;
				this.workspace.TerminalRemoved                    -= workspace_TerminalRemoved;

				this.workspace.TimedStatusTextRequest             -= workspace_TimedStatusTextRequest;
				this.workspace.FixedStatusTextRequest             -= workspace_FixedStatusTextRequest;
				this.workspace.CursorRequest                      -= workspace_CursorRequest;
				this.workspace.MessageInputRequest                -= workspace_MessageInputRequest;

				this.workspace.SaveAsFileDialogRequest            -= workspace_SaveAsFileDialogRequest;
				this.workspace.SaveCommandPageAsFileDialogRequest -= workspace_SaveCommandPageAsFileDialogRequest;
				this.workspace.OpenCommandPageFileDialogRequest   -= workspace_OpenCommandPageFileDialogRequest;

				this.workspace.Closed                             -= workspace_Closed;

				if (this.workspace.SettingsRoot != null)
					this.workspace.SettingsRoot.Changed -= workspaceSettingsRoot_Changed;
			}
		}

		#endregion

		#region Workspace > Event Handlers
		//------------------------------------------------------------------------------------------
		// Workspace > Event Handlers
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Terminal is removed in <see cref="terminalMdiChild_FormClosed"/> event handler.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_TerminalAdded(object sender, Model.TerminalEventArgs e)
		{
			// Create terminal form and immediately show it:

			var mdiChild = new Terminal(e.Terminal);
			AttachTerminalEventHandlersAndMdiChildToParent(mdiChild);

			this.isLayoutingMdi = true;
			NotifyIntegralMdiLayoutingToMdiChildren(true);
			mdiChild.Show(); // MDI children must be shown without reference to 'this'.
			NotifyIntegralMdiLayoutingToMdiChildren(false);
			this.isLayoutingMdi = false;

			LayoutWorkspace();
			SetChildControls();
		}

		/// <remarks>
		/// Terminal is removed in <see cref="terminalMdiChild_FormClosed"/> event handler.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_TerminalRemoved(object sender, Model.TerminalEventArgs e)
		{
			// Nothing to do, see remarks above.
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_TimedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetTimedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_FixedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetFixedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		[ModalBehaviorContract(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void workspace_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			Refresh(); // Ensure that form has been refreshed before showing the message box.
			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveWorkspaceAsFileDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_SaveCommandPageAsFileDialogRequest(object sender, Model.FilePathDialogEventArgs e)
		{
			string filePathNew;
			e.Result = CommandPagesSettingsFileLinkHelper.ShowSaveAsFileDialog(this, e.FilePathOld, out filePathNew);
			e.FilePathNew = filePathNew;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_OpenCommandPageFileDialogRequest(object sender, Model.FilePathDialogEventArgs e)
		{
			string filePathNew;
			e.Result = CommandPagesSettingsFileLinkHelper.ShowOpenFileDialog(this, e.FilePathOld, out filePathNew);
			e.FilePathNew = filePathNew;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			Cursor = e.Value;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspace_Closed(object sender, Model.ClosedEventArgs e)
		{
			DetachWorkspaceEventHandlers();
			this.workspace = null;

			SetChildControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the invoking thread onto the main thread.")]
		private void workspaceSettingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			SetWorkspaceControls();
		}

		#endregion

		#endregion

		#region Terminal MDI Child
		//==========================================================================================
		// Terminal MDI Child
		//==========================================================================================

		#region Terminal MDI Child > Methods
		//------------------------------------------------------------------------------------------
		// Terminal MDI Child > Methods
		//------------------------------------------------------------------------------------------

		private void AttachTerminalEventHandlersAndMdiChildToParent(Terminal terminalMdiChild)
		{
			terminalMdiChild.MdiParent = this;

			terminalMdiChild.Changed                          += terminalMdiChild_Changed;
			terminalMdiChild.Saved                            += terminalMdiChild_Saved;

			terminalMdiChild.AutoActionSettingsChanged        += terminalMdiChild_AutoActionSettingsChanged;
			terminalMdiChild.AutoActionTriggerStateChanged    += terminalMdiChild_AutoActionTriggerStateChanged;
		////terminalMdiChild.AutoActionActionStateChanged     += terminalMdiChild_AutoActionActionStateChanged is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
			terminalMdiChild.AutoActionCountChanged           += terminalMdiChild_AutoActionCountChanged;
			terminalMdiChild.AutoResponseSettingsChanged      += terminalMdiChild_AutoResponseSettingsChanged;
			terminalMdiChild.AutoResponseTriggerStateChanged  += terminalMdiChild_AutoResponseTriggerStateChanged;
			terminalMdiChild.AutoResponseResponseStateChanged += terminalMdiChild_AutoResponseResponseStateChanged;
			terminalMdiChild.AutoResponseCountChanged         += terminalMdiChild_AutoResponseCountChanged;

			terminalMdiChild.Resize                           += terminalMdiChild_Resize;
			terminalMdiChild.FormClosed                       += terminalMdiChild_FormClosed;
		}

		private void DetachTerminalEventHandlersAndMdiChildFromParent(Terminal terminalMdiChild)
		{
			terminalMdiChild.Changed                          -= terminalMdiChild_Changed;
			terminalMdiChild.Saved                            -= terminalMdiChild_Saved;

			terminalMdiChild.AutoActionSettingsChanged        -= terminalMdiChild_AutoActionSettingsChanged;
			terminalMdiChild.AutoActionTriggerStateChanged    -= terminalMdiChild_AutoActionTriggerStateChanged;
		////terminalMdiChild.AutoActionActionStateChanged     -= terminalMdiChild_AutoActionActionStateChanged is not needed (yet) because 'DropDownStyle' is 'DropDownList'.
			terminalMdiChild.AutoActionCountChanged           -= terminalMdiChild_AutoActionCountChanged;
			terminalMdiChild.AutoResponseSettingsChanged      -= terminalMdiChild_AutoResponseSettingsChanged;
			terminalMdiChild.AutoResponseTriggerStateChanged  -= terminalMdiChild_AutoResponseTriggerStateChanged;
			terminalMdiChild.AutoResponseResponseStateChanged -= terminalMdiChild_AutoResponseResponseStateChanged;
			terminalMdiChild.AutoResponseCountChanged         -= terminalMdiChild_AutoResponseCountChanged;

			terminalMdiChild.Resize                           -= terminalMdiChild_Resize;
			terminalMdiChild.FormClosed                       -= terminalMdiChild_FormClosed;

			// Do not set terminalMdiChild.MdiParent to null. Doing so results in a detached non-
			// MDI-form which appears for a short moment at the default startup location of windows.
			// Pretty ugly!
		}

		#endregion

		#region Terminal MDI Child > Events
		//------------------------------------------------------------------------------------------
		// Terminal MDI Child > Events
		//------------------------------------------------------------------------------------------

		private void terminalMdiChild_Changed(object sender, EventArgs e)
		{
			SetTimedStatus(Status.ChildChanged);

			SetChildControls();
		}

		private void terminalMdiChild_Saved(object sender, Model.SavedEventArgs e)
		{
			if (!e.IsAutoSave)
				SetTimedStatus(Status.ChildSaved);

			SetChildControls();
		}

		private void terminalMdiChild_AutoActionSettingsChanged(object sender, EventArgs e)
		{
			SetAutoActionChildControls();
		}

		private void terminalMdiChild_AutoActionTriggerStateChanged(object sender, EventArgs e)
		{
			var state = ((Terminal)ActiveMdiChild).AutoActionTriggerState;
			SetAutoActionTriggerStateControls(state);
		}

	////private void terminalMdiChild_AutoActionActionStateChanged(object sender, EventArgs e) is not needed (yet) because 'DropDownStyle' is 'DropDownList'.

		private void terminalMdiChild_AutoActionCountChanged(object sender, EventArgs<int> e)
		{
			SetAutoActionCountChildControls();
		}

		private void terminalMdiChild_AutoResponseSettingsChanged(object sender, EventArgs e)
		{
			SetAutoResponseChildControls();
		}

		private void terminalMdiChild_AutoResponseTriggerStateChanged(object sender, EventArgs e)
		{
			var state = ((Terminal)ActiveMdiChild).AutoResponseTriggerState;
			SetAutoResponseTriggerStateControls(state);
		}

		private void terminalMdiChild_AutoResponseResponseStateChanged(object sender, EventArgs e)
		{
			var state = ((Terminal)ActiveMdiChild).AutoResponseResponseState;
			SetAutoResponseResponseStateControls(state);
		}

		private void terminalMdiChild_AutoResponseCountChanged(object sender, EventArgs<int> e)
		{
			SetAutoResponseCountChildControls();
		}

		private void terminalMdiChild_Resize(object sender, EventArgs e)
		{
			if (!this.isLayoutingMdi)
			{
				var t = (sender as Terminal);
				if (t != null)
				{
					switch (t.WindowState)
					{
						case FormWindowState.Normal:
							SetTerminalLayout(WorkspaceLayout.Manual);
							break;

						case FormWindowState.Minimized:
							SetTerminalLayout(WorkspaceLayout.Minimize);
							break;

						// There is a limitation here:
						//
						// Code execution also gets here if the main form is resized. Thus, if the
						// workspace layout is 'Automatic' and there is only a single terminal in
						// the workspace, i.e. 'Maximized', the workspace layout will be changed to
						// 'Maximized' as well.
						//
						// Note the following check doesn't work, as it eliminates the possibility
						// to intentionally maximize terminal and thus the whole workspace:
						// if (this.workspace.SettingsRoot.Workspace.Layout != WorkspaceLayout.Automatic))
						//     Prevent change from automatic to maximize when just resizing the main form.
						case FormWindowState.Maximized:
							SetTerminalLayout(WorkspaceLayout.Maximize);
							break;

						default:
							throw (new NotSupportedException(MessageHelper.InvalidExecutionPreamble + "'" + t.WindowState.ToString() + "' is an item that is not (yet) supported!" + Environment.NewLine + Environment.NewLine + MessageHelper.SubmitBug));
					}
				}
			}
		}

		private void terminalMdiChild_FormClosed(object sender, FormClosedEventArgs e)
		{
			SetTimedStatus(Status.ChildClosed);

			// Sender MUST be a terminal, otherwise something must have freaked out...
			DetachTerminalEventHandlersAndMdiChildFromParent(sender as Terminal);

			// Update the layout AFTER to ensure that closed MDI child has been disposed of:
			this.invokeLayout = true;

			// There is a limitation here:
			//
			// This 'FormClosed' event is invoked before the MDI child has been disposed of.
			// Thus, layouting would take the just-about-to-be-closed terminal into account.
			// The workaround with 'invokeLayout' unfortunately doesn't work as well, as the
			// close form is still active when the 'MdiChildActivate' event is invoked...
			//
			// Keeping this limitation, shall again be checked after upgrading to .NET 4+.
		}

		#endregion

		#endregion

		#region Find
		//==========================================================================================
		// Find
		//==========================================================================================

		/// <summary>
		/// Requests to activate the find field.
		/// </summary>
		public virtual void RequestFind()
		{
			if (!ApplicationSettings.RoamingUserSettings.View.FindIsVisible)
			{
				ApplicationSettings.RoamingUserSettings.View.FindIsVisible = true;
				ApplicationSettings.SaveRoamingUserSettings();
			}

		////toolStripComboBox_MainTool_Terminal_Find_Pattern.Select()    doesn't work, 'ToolStrip'
			toolStripComboBox_MainTool_Find_Pattern.Focus(); // seems to require Focus().
		}

		/// <summary>
		/// Gets whether the find is ready to search forward.
		/// </summary>
		public virtual bool FindNextIsFeasible
		{
			get
			{
				bool childIsReady = (ActiveMdiChild != null);
				return (childIsReady && this.findNextIsFeasible);
			}
		}

		/// <summary>
		/// Gets whether the find is ready to search backward.
		/// </summary>
		public virtual bool FindPreviousIsFeasible
		{
			get
			{
				bool childIsReady = (ActiveMdiChild != null);
				return (childIsReady && this.findPreviousIsFeasible);
			}
		}

		/// <summary>
		/// Requests find next.
		/// </summary>
		public virtual void RequestFindNext()
		{
			ValidateAndFindNext();
		}

		/// <summary>
		/// Requests find previous.
		/// </summary>
		public virtual void RequestFindPrevious()
		{
			ValidateAndFindPrevious();
		}

		#endregion

	#if (WITH_SCRIPTING)

		#region Scripting
		//==========================================================================================
		// Scripting
		//==========================================================================================

		private void ToggleScriptDialog()
		{
			if (this.scriptDialogIsOpen)
				CloseScriptDialog();
			else
				OpenScriptDialog();
		}

		private void OpenScriptDialog()
		{
		////ApplicationSettings.RoamingUserSettings.View.ScriptPanelIsVisible = true;
		////ApplicationSettings.SaveRoamingUserSettings();

			this.main.ScriptBridge.ProcessCommand("Script:OpenDialog");
		}

		private void CloseScriptDialog()
		{
		////ApplicationSettings.RoamingUserSettings.View.ScriptPanelIsVisible = false;
		////ApplicationSettings.SaveRoamingUserSettings();

			this.main.ScriptBridge.ProcessCommand("Script:CloseDialog");
		}

		private void NotifyScriptDialogOpened()
		{
			this.scriptDialogIsOpen = true;

			toolStripMenuItem_MainMenu_Script_Panel.Checked = true;

			toolStripButton_MainTool_Script_ShowHide.Checked = true;
			toolStripButton_MainTool_Script_ShowHide.Text = "Hide Script Panel";
		}

		private void NotifyScriptDialogClosed()
		{
			this.scriptDialogIsOpen = false;

			toolStripMenuItem_MainMenu_Script_Panel.Checked = false;

			toolStripButton_MainTool_Script_ShowHide.Checked = false;
			toolStripButton_MainTool_Script_ShowHide.Text = "Show Script Panel";
		}

		private void NotitfyScriptEnded(int result)
		{
			this.main.Result = (Model.MainResult)result;
		}

		#endregion

	#endif // WITH_SCRIPTING

		#region Status
		//==========================================================================================
		// Status
		//==========================================================================================

		private enum Status
		{
			ChildActivated,
			ChildChanged,
			ChildSaved,
			ChildClosed,
			Default
		}

		private string GetStatusText(Status status)
		{
			if (ActiveMdiChild != null)
			{
				string childText = "[" + ((Terminal)ActiveMdiChild).Text + "]";
				switch (status)
				{
					case Status.ChildActivated: return (childText + " activated");
					case Status.ChildChanged:   return (childText + " changed");
					case Status.ChildSaved:     return (childText + " saved");
					case Status.ChildClosed:    return (childText + " closed");
				}
			}

			return ("");
		}

		private void SetFixedStatusText(string text)
		{
			timer_Status.Enabled = false;
			toolStripStatusLabel_MainStatus_Status.Text = text;
		}

		private void SetFixedStatus(Status status)
		{
			SetFixedStatusText(GetStatusText(status));
		}

		private void SetTimedStatusText(string text)
		{
			timer_Status.Enabled = false;
			toolStripStatusLabel_MainStatus_Status.Text = text;
			timer_Status.Interval = TimedStatusInterval;
			timer_Status.Enabled = true;
		}

		private void SetTimedStatus(Status status)
		{
			SetTimedStatusText(GetStatusText(status));
		}

		/// <summary>
		/// Resets the status text.
		/// </summary>
		public virtual void ResetStatusText()
		{
			SetFixedStatus(Status.Default);
		}

		/// <remarks>
		/// This 'Windows.Forms.Timer' event handler will be called on the application main thread,
		/// i.e. is single-threaded. No synchronization or prevention of a race condition is needed.
		/// </remarks>
		private void timer_Status_Tick(object sender, EventArgs e)
		{
			timer_Status.Enabled = false;
			ResetStatusText();
		}

		/// <remarks>
		/// Using term 'Info' since the info contains name and IDs.
		/// </remarks>
		private void SetTerminalInfoText(string text)
		{
			toolStripStatusLabel_MainStatus_TerminalInfo.Text = text;
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		/// <remarks>
		/// Name "DebugWriteLine" would show relation to <see cref="Debug.WriteLine(string)"/>.
		/// However, named "Message" for compactness and more clarity that something will happen
		/// with <paramref name="message"/>, and rather than e.g. "Common" for comprehensibility.
		/// </remarks>
		[Conditional("DEBUG")]
		protected virtual void DebugMessage(string message)
		{
			string guid;
			if (this.main != null)
				guid = this.main.Guid.ToString();
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

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_MDI")]
		private void DebugMdi(string message)
		{
			DebugMessage(message);
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_FIND")]
		private void DebugFindEnter(string methodName)
		{
			Debug.WriteLine(methodName);
			Debug.Indent();

			DebugFindState();
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_FIND")]
		private void DebugFindLeave()
		{
			DebugFindState();

			Debug.Unindent();
		}

		/// <remarks>
		/// <c>private</c> because value of <see cref="ConditionalAttribute"/> is limited to file scope.
		/// </remarks>
		[Conditional("DEBUG_FIND")]
		private void DebugFindState()
		{
			Debug.Write    (@"Text   = """         + toolStripComboBox_MainTool_Find_Pattern.Text);
			Debug.Write    (@""" / Cursor @ "      + toolStripComboBox_MainTool_Find_Pattern.SelectionStart);
			Debug.Write    (" / Selection @ "      + toolStripComboBox_MainTool_Find_Pattern.SelectionLength);
			Debug.WriteLine(" / Selected index @ " + toolStripComboBox_MainTool_Find_Pattern.SelectedIndex);
		}

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
