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
// YAT 2.0 Epsilon Version 1.99.90
// ------------------------------------------------------------------------------------------------
// See release notes for product version details.
// See SVN change log for file revision details.
// Author(s): Matthias Klaey
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2017 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// This source code is licensed under the GNU LGPL.
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Contracts;
using MKY.IO;
using MKY.Settings;
using MKY.Windows.Forms;

using YAT.Application.Utilities;
using YAT.Model.Types;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

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
			IsClosingFromModel,
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

		// Model:
		private Model.Main main;
		private Model.Workspace workspace;

		// Settings:
		private LocalUserSettingsRoot localUserSettingsRoot;

		// Toolstrip-combobox-validation-workaround (too late invocation of 'Validate' event):
		[SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "Clear separation of item and postfix.")]
		private bool mainToolValidationWorkaround_UpdateIsSuspended;

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
		public Main()
			: this(new Model.Main())
		{
		}

		/// <summary></summary>
		public Main(Model.Main main)
		{
			DebugMessage("Creating...");

			InitializeComponent();

			InitializeControls();

			// Register this form as the main form:
			NativeMessageHandler.RegisterMainForm(this);

			// Link and attach to main model:
			this.main = main;
			AttachMainEventHandlers();

			Text = this.main.IndicatedName;

			// Link and attach to terminal settings:
			this.localUserSettingsRoot = ApplicationSettings.LocalUserSettings;
			AttachLocalUserSettingsEventHandlers();

			ApplyWindowSettingsAccordingToStartup();

			SetControls();

			DebugMessage("...successfully created.");
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

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
		/// WITHOUT a view, <see cref="YAT.Model.Main.Start"/> is called by either
		/// YAT.Controller.Main.RunFullyFromConsole() or YAT.Controller.Main.RunInvisible().
		/// </remarks>
		[ModalBehavior(ModalBehavior.InCaseOfNonUserError, Approval = "StartArgs are considered to decide on behavior.")]
		private void Main_Shown(object sender, EventArgs e)
		{
			this.isStartingUp = false;

			// Start YAT according to the requested settings. Temporarily notify MDI layouting
			// to prevent that initial layouting overwrites the workspace settings.
			this.isLayoutingMdi = true;
			this.result = this.main.Start();
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
							string executableName = Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);

							var sb = new StringBuilder();
							sb.Append(ApplicationEx.ProductName);
							sb.Append(" could not be started because the given command line is invalid!");
							sb.AppendLine();
							sb.AppendLine();
							sb.Append(@"Use """ + executableName + @" /?"" for command line help.");

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
							sb.Append(ApplicationEx.ProductName);
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
							sb.Append(ApplicationEx.ProductName);
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
						Model.Settings.NewTerminalSettings processed = new Model.Settings.NewTerminalSettings(ApplicationSettings.LocalUserSettings.NewTerminal);
						if (this.main.ProcessCommandLineArgsIntoExistingNewTerminalSettings(processed))
							ShowNewTerminalDialog(processed);
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
			if (!IsStartingUp)
				SaveWindowSettings(true);
		}

		private void Main_SizeChanged(object sender, EventArgs e)
		{
			if (!IsStartingUp)
				SaveWindowSettings();
		}

		private void Main_Resize(object sender, EventArgs e)
		{
			if (!IsStartingUp)
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

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Prevent multiple calls to Exit()/Close():
			if (this.closingState == ClosingState.None)
			{
				this.closingState = ClosingState.IsClosingFromForm;

				// Also notify MDI children about closing:
				foreach (var f in this.MdiChildren)
				{
					var t = (f as Terminal);
					if (t != null)
						t.NotifyClosingFromForm();
				}

				bool cancel;
				this.main.Exit(out cancel); // Only need to handle cancel on form here, no need to
				if (cancel)                 // deal with result, that is handled in main_Exited().
				{
					e.Cancel = true;

					// Revert closing state in case of cancel:
					this.closingState = ClosingState.None;

					// Also revert closing state for MDI children:
					foreach (var f in this.MdiChildren)
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

			DetachLocalUserSettingsEventHandlers();
			this.localUserSettingsRoot = null;
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
			ShowOpenWorkspaceFromFileDialog();
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

		#region Controls Event Handlers > Main Menu > Log
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Log
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_Log_SetMenuItems()
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

				bool logFileExists = false;
				if (t != null)
					logFileExists = t.LogFileExists;

				toolStripMenuItem_MainMenu_Log_AllOn.Enabled    = childIsReady && !allLogsAreOn;
				toolStripMenuItem_MainMenu_Log_AllOff.Enabled   = childIsReady &&       logIsOn;
				toolStripMenuItem_MainMenu_Log_AllClear.Enabled = childIsReady &&      (logIsOn || logFileExists);
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void toolStripMenuItem_MainMenu_Log_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_Log_SetMenuItems();
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
				bool workspaceIsReady = (this.workspace != null);
				toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Enabled = workspaceIsReady;

				bool alwaysOnTop = ((this.workspace != null) ? (this.workspace.SettingsRoot.Workspace.AlwaysOnTop) : (false));
				toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Checked = alwaysOnTop;

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
				if (childIsReady)
				{
					Form f = this.ActiveMdiChild;
					ActivateMdiChild(null);
					ActivateMdiChild(f);
				}
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
			#endif
			}
		}

		private void toolStripMenuItem_MainMenu_Window_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems(true);
		}

		private void toolStripMenuItem_MainMenu_Window_AlwaysOnTop_Click(object sender, EventArgs e)
		{
			ToggleAlwaysOnTop();
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

		#region Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_Contents_Click(object sender, EventArgs e)
		{
			var f = new Help();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			var f = new ReleaseNotes();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestSupport_Click(object sender, EventArgs e)
		{
			var f = new TrackerInstructions(TrackerType.Support);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestFeature_Click(object sender, EventArgs e)
		{
			var f = new TrackerInstructions(TrackerType.Feature);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_SubmitBug_Click(object sender, EventArgs e)
		{
			var f = new TrackerInstructions(TrackerType.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
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
					if ((t != null) && (!t.IsDisposed))
					{
						radixIsReady = !(t.SettingsRoot.Display.SeparateTxRxRadix);
						if (radixIsReady)
							radix = t.SettingsRoot.Display.TxRadix;
					}
				}

				bool logIsOn = false;
				if (childIsReady)
					logIsOn = ((Terminal)ActiveMdiChild).LogIsOn;

				bool logFileExists = false;
				if (childIsReady)
					logFileExists = ((Terminal)ActiveMdiChild).LogFileExists;

				toolStripButton_MainTool_File_Save.Enabled         = childIsReady && !terminalFileIsReadOnly;
				toolStripButton_MainTool_Terminal_Start.Enabled    = childIsReady && terminalIsStopped;
				toolStripButton_MainTool_Terminal_Stop.Enabled     = childIsReady && terminalIsStarted;
				toolStripButton_MainTool_Terminal_Settings.Enabled = childIsReady;

				toolStripButton_MainTool_Terminal_Radix_String.Enabled  = childIsReady && radixIsReady;
				toolStripButton_MainTool_Terminal_Radix_Char.Enabled    = childIsReady && radixIsReady;
				toolStripButton_MainTool_Terminal_Radix_Bin.Enabled     = childIsReady && radixIsReady;
				toolStripButton_MainTool_Terminal_Radix_Oct.Enabled     = childIsReady && radixIsReady;
				toolStripButton_MainTool_Terminal_Radix_Dec.Enabled     = childIsReady && radixIsReady;
				toolStripButton_MainTool_Terminal_Radix_Hex.Enabled     = childIsReady && radixIsReady;
				toolStripButton_MainTool_Terminal_Radix_Unicode.Enabled = childIsReady && radixIsReady;

				toolStripButton_MainTool_Terminal_Radix_String.Checked  = (radix == Domain.Radix.String);
				toolStripButton_MainTool_Terminal_Radix_Char.Checked    = (radix == Domain.Radix.Char);
				toolStripButton_MainTool_Terminal_Radix_Bin.Checked     = (radix == Domain.Radix.Bin);
				toolStripButton_MainTool_Terminal_Radix_Oct.Checked     = (radix == Domain.Radix.Oct);
				toolStripButton_MainTool_Terminal_Radix_Dec.Checked     = (radix == Domain.Radix.Dec);
				toolStripButton_MainTool_Terminal_Radix_Hex.Checked     = (radix == Domain.Radix.Hex);
				toolStripButton_MainTool_Terminal_Radix_Unicode.Checked = (radix == Domain.Radix.Unicode);

				bool arVisible = false;
				bool arIsActive = false;

				AutoTriggerEx[] arTriggerItems = AutoTriggerEx.GetFixedItems();
				AutoTriggerEx   arTrigger      = AutoTrigger.None;

				AutoResponseEx[] arResponseItems = AutoResponseEx.GetFixedItems();
				AutoResponseEx   arResponse      = AutoResponse.None;

				int arCount = 0;

				if (childIsReady)
				{
					// Icon shall be visible if any terminal uses this option.
					//
					// Rationale:
					// Icons shall not move/shift when switching among terminals.
					arVisible = AutoResponseVisibleInAnyTerminal;

					var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
					if ((activeTerminal != null) && (!activeTerminal.IsDisposed))
					{
					////arVisible       = activeTerminal.SettingsRoot.AutoResponse.Visible; => See above.
						arIsActive      = activeTerminal.SettingsRoot.AutoResponse.IsActive;

						arTriggerItems  = activeTerminal.SettingsRoot.GetValidAutoTriggerItems();
						arTrigger       = activeTerminal.SettingsRoot.AutoResponse.Trigger;

						arResponseItems = activeTerminal.SettingsRoot.GetValidAutoResponseItems();
						arResponse      = activeTerminal.SettingsRoot.AutoResponse.Response;

						arCount         = activeTerminal.AutoResponseCount;
					}
				}

				toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Checked = arVisible;

				if (arVisible)
				{
					toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Text = "Hide Automatic Response";

					// Attention:
					// Similar code exists in the following location:
					//  > View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
					// Changes here may have to be applied there too.

					if (!this.mainToolValidationWorkaround_UpdateIsSuspended)
					{
						toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Visible = true;
						toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Enabled = childIsReady;
						toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Items.Clear();
						toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Items.AddRange(arTriggerItems);

						SelectionHelper.Select(toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger, arTrigger, new Command(arTrigger).SingleLineText); // No explicit default radix available (yet).

						toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Visible = true;
						toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Enabled = childIsReady;
						toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.Clear();
						toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.AddRange(arResponseItems);

						SelectionHelper.Select(toolStripComboBox_MainTool_Terminal_AutoResponse_Response, arResponse, new Command(arResponse).SingleLineText); // No explicit default radix available (yet).
					}

					toolStripLabel_MainTool_Terminal_AutoResponse_Count.Visible = true;
					toolStripLabel_MainTool_Terminal_AutoResponse_Count.Enabled = arIsActive;
					toolStripLabel_MainTool_Terminal_AutoResponse_Count.Text = string.Format("({0})", arCount);

					toolStripButton_MainTool_Terminal_AutoResponse_Deactivate.Visible = true;
					toolStripButton_MainTool_Terminal_AutoResponse_Deactivate.Enabled = arIsActive;
				}
				else
				{
					toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Text = "Show Automatic Response";

					toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Visible = false;
					toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Enabled = false;
					toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Items.Clear();

					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Visible = false;
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Enabled = false;
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.Clear();

					toolStripLabel_MainTool_Terminal_AutoResponse_Count.Visible = false;
					toolStripLabel_MainTool_Terminal_AutoResponse_Count.Text = "";

					toolStripButton_MainTool_Terminal_AutoResponse_Deactivate.Visible = false;
					toolStripButton_MainTool_Terminal_AutoResponse_Deactivate.Enabled = false;
				}

				bool aaVisible = false;
				bool aaIsActive = false;

				AutoTriggerEx[] aaTriggerItems = AutoTriggerEx.GetFixedItems();
				AutoTriggerEx   aaTrigger      = AutoTrigger.None;

				AutoActionEx[] aaActionItems = AutoActionEx.GetItems();
				AutoActionEx   aaAction      = AutoAction.None;

				int aaCount = 0;

				if (childIsReady)
				{
					// Icon shall be visible if any terminal uses this option.
					//
					// Rationale:
					// Icons shall not move/shift when switching among terminals.
					aaVisible = AutoActionVisibleInAnyTerminal;

					var activeTerminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
					if ((activeTerminal != null) && (!activeTerminal.IsDisposed))
					{
					////aaVisible      = activeTerminal.SettingsRoot.AutoAction.Visible; => See above.
						aaIsActive     = activeTerminal.SettingsRoot.AutoAction.IsActive;

						aaTriggerItems = activeTerminal.SettingsRoot.GetValidAutoTriggerItems();
						aaTrigger      = activeTerminal.SettingsRoot.AutoAction.Trigger;

						aaActionItems  = activeTerminal.SettingsRoot.GetValidAutoActionItems();
						aaAction       = activeTerminal.SettingsRoot.AutoAction.Action;

						aaCount        = activeTerminal.AutoActionCount;
					}
				}

				toolStripButton_MainTool_Terminal_AutoAction_ShowHide.Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_AutoAction_ShowHide.Checked = aaVisible;

				if (aaVisible)
				{
					toolStripButton_MainTool_Terminal_AutoAction_ShowHide.Text = "Hide Automatic Action";

					// Attention:
					// Similaa code exists in the following location:
					//  > View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
					// Changes here may have to be applied there too.

					if (!this.mainToolValidationWorkaround_UpdateIsSuspended)
					{
						toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Visible = true;
						toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Enabled = childIsReady;
						toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Items.Clear();
						toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Items.AddRange(aaTriggerItems);

						SelectionHelper.Select(toolStripComboBox_MainTool_Terminal_AutoAction_Trigger, aaTrigger, new Command(aaTrigger).SingleLineText); // No explicit default radix available (yet).

						toolStripComboBox_MainTool_Terminal_AutoAction_Action.Visible = true;
						toolStripComboBox_MainTool_Terminal_AutoAction_Action.Enabled = childIsReady;
						toolStripComboBox_MainTool_Terminal_AutoAction_Action.Items.Clear();
						toolStripComboBox_MainTool_Terminal_AutoAction_Action.Items.AddRange(aaActionItems);

						SelectionHelper.Select(toolStripComboBox_MainTool_Terminal_AutoAction_Action, aaAction, new Command(aaAction).SingleLineText); // No explicit default radix available (yet).
					}

					toolStripLabel_MainTool_Terminal_AutoAction_Count.Visible = true;
					toolStripLabel_MainTool_Terminal_AutoAction_Count.Enabled = aaIsActive;
					toolStripLabel_MainTool_Terminal_AutoAction_Count.Text = string.Format("({0})", aaCount);

					toolStripButton_MainTool_Terminal_AutoAction_Deactivate.Visible = true;
					toolStripButton_MainTool_Terminal_AutoAction_Deactivate.Enabled = aaIsActive;
				}
				else
				{
					toolStripButton_MainTool_Terminal_AutoAction_ShowHide.Text = "Show Automatic Action";

					toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Visible = false;
					toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Enabled = false;
					toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Items.Clear();

					toolStripComboBox_MainTool_Terminal_AutoAction_Action.Visible = false;
					toolStripComboBox_MainTool_Terminal_AutoAction_Action.Enabled = false;
					toolStripComboBox_MainTool_Terminal_AutoAction_Action.Items.Clear();

					toolStripLabel_MainTool_Terminal_AutoAction_Count.Visible = false;
					toolStripLabel_MainTool_Terminal_AutoAction_Count.Text = "";

					toolStripButton_MainTool_Terminal_AutoAction_Deactivate.Visible = false;
					toolStripButton_MainTool_Terminal_AutoAction_Deactivate.Enabled = false;
				}

				toolStripButton_MainTool_Terminal_Clear.Enabled             = childIsReady;
				toolStripButton_MainTool_Terminal_Refresh.Enabled           = childIsReady;
				toolStripButton_MainTool_Terminal_CopyToClipboard.Enabled   = childIsReady;
				toolStripButton_MainTool_Terminal_SaveToFile.Enabled        = childIsReady;
				toolStripButton_MainTool_Terminal_Print.Enabled             = childIsReady;

				bool findVisible = this.localUserSettingsRoot.MainWindow.ShowFindField;

				toolStripButton_MainTool_Terminal_Find_ShowHide.Enabled = childIsReady;
				toolStripButton_MainTool_Terminal_Find_ShowHide.Checked = findVisible;

				if (findVisible)
				{
					toolStripButton_MainTool_Terminal_Find_ShowHide.Text = "Hide Find";

					// Attention:
					// Similar code exists in the following location:
					//  > View.Forms.Terminal.toolStripMenuItem_TerminalMenu_Send_SetMenuItems()
					// Changes here may have to be applied there too.

					if (!this.mainToolValidationWorkaround_UpdateIsSuspended)
					{
						toolStripComboBox_MainTool_Terminal_Find_Pattern.Visible = true;
						toolStripComboBox_MainTool_Terminal_Find_Pattern.Enabled = childIsReady;
						toolStripComboBox_MainTool_Terminal_Find_Pattern.Items.Clear();
						toolStripComboBox_MainTool_Terminal_Find_Pattern.Items.AddRange(this.localUserSettingsRoot.MainWindow.RecentFindPatterns.ToArray());

						SelectionHelper.Select(toolStripComboBox_MainTool_Terminal_Find_Pattern, this.localUserSettingsRoot.MainWindow.ActiveFindPattern);
					}

					toolStripButton_MainTool_Terminal_Find_Next    .Visible = true;
					toolStripButton_MainTool_Terminal_Find_Previous.Visible = true;
				}
				else
				{
					toolStripButton_MainTool_Terminal_Find_ShowHide.Text = "Show Find";

					toolStripComboBox_MainTool_Terminal_Find_Pattern.Visible = false;
					toolStripComboBox_MainTool_Terminal_Find_Pattern.Enabled = false;
					toolStripComboBox_MainTool_Terminal_Find_Pattern.Items.Clear();

					toolStripButton_MainTool_Terminal_Find_Next    .Visible = false;
					toolStripButton_MainTool_Terminal_Find_Previous.Visible = false;
				}

				toolStripButton_MainTool_Terminal_Log_Settings.Enabled      = childIsReady;
				toolStripButton_MainTool_Terminal_Log_On.Enabled            = childIsReady && !logIsOn;
				toolStripButton_MainTool_Terminal_Log_Off.Enabled           = childIsReady &&  logIsOn;
				toolStripButton_MainTool_Terminal_Log_OpenFile.Enabled      = childIsReady &&  logFileExists;
				toolStripButton_MainTool_Terminal_Log_OpenDirectory.Enabled = childIsReady;

				toolStripButton_MainTool_Terminal_Format.Enabled            = childIsReady;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
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

		private void toolStripButton_MainTool_Terminal_Radix_String_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.String);
		}

		private void toolStripButton_MainTool_Terminal_Radix_Char_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Char);
		}

		private void toolStripButton_MainTool_Terminal_Radix_Bin_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Bin);
		}

		private void toolStripButton_MainTool_Terminal_Radix_Oct_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Oct);
		}

		private void toolStripButton_MainTool_Terminal_Radix_Dec_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Dec);
		}

		private void toolStripButton_MainTool_Terminal_Radix_Hex_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Hex);
		}

		private void toolStripButton_MainTool_Terminal_Radix_Unicode_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestRadix(Domain.Radix.Unicode);
		}

		private void toolStripButton_MainTool_Terminal_AutoResponse_ShowHide_Click(object sender, EventArgs e)
		{
			// Icon shall be visible if any terminal uses this option.
			//
			// Rationale:
			// Icons shall not move/shift when switching among terminals.
			//
			// As a consequence, changing the option must be applied to all terminals:
			RequestAutoResponseVisibleInAllTerminals(!AutoResponseVisibleInAnyTerminal); // Toggle.
		}

		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
				((Terminal)ActiveMdiChild).RequestAutoResponseTrigger(trigger);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event
		/// way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			// Attention, 'isSettingControls' must only be checked further below!

			if (toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				string triggerText = toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Text;
				int invalidTextStart;
				if (Utilities.ValidationHelper.ValidateText(this, "automatic response trigger", triggerText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.mainToolValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							((Terminal)ActiveMdiChild).RequestAutoResponseTrigger(triggerText);
						}
						finally
						{
							this.mainToolValidationWorkaround_UpdateIsSuspended = false;
						}
					}
				}
				else
				{
					toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Text = triggerText.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Response_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var response = (toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedItem as AutoResponseEx);
			if (response != null)
				((Terminal)ActiveMdiChild).RequestAutoResponseResponse(response);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event
		/// way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			// Attention, 'isSettingControls' must only be checked further below!

			if (toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex)
			{
				string responseText = toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Text;
				int invalidTextStart;
				if (Utilities.ValidationHelper.ValidateText(this, "automatic response", responseText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.mainToolValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							((Terminal)ActiveMdiChild).RequestAutoResponseResponse(responseText);
						}
						finally
						{
							this.mainToolValidationWorkaround_UpdateIsSuspended = false;
						}
					}
				}
				else
				{
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Text = responseText.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripLabel_MainTool_Terminal_AutoResponse_Count_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestAutoResponseResetCount();
		}

		private void toolStripButton_MainTool_Terminal_AutoResponse_Deactivate_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestAutoResponseDeactivate();
		}

		private void toolStripButton_MainTool_Terminal_AutoAction_ShowHide_Click(object sender, EventArgs e)
		{
			// Icon shall be visible if any terminal uses this option.
			//
			// Rationale:
			// Icons shall not move/shift when switching among terminals.
			//
			// As a consequence, changing the option must be applied to all terminals:
			RequestAutoActionVisibleInAllTerminals(!AutoActionVisibleInAnyTerminal); // Toggle.
		}

		private void toolStripComboBox_MainTool_Terminal_AutoAction_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var trigger = (toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.SelectedItem as AutoTriggerEx);
			if (trigger != null)
				((Terminal)ActiveMdiChild).RequestAutoActionTrigger(trigger);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event
		/// way too late, only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_MainTool_Terminal_AutoAction_Trigger_TextChanged(object sender, EventArgs e)
		{
			// Attention, 'isSettingControls' must only be checked further below!

			if (toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				string triggerText = toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Text;
				int invalidTextStart;
				if (Utilities.ValidationHelper.ValidateText(this, "automatic action trigger", triggerText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.mainToolValidationWorkaround_UpdateIsSuspended = true;
						try
						{
							((Terminal)ActiveMdiChild).RequestAutoActionTrigger(triggerText);
						}
						finally
						{
							this.mainToolValidationWorkaround_UpdateIsSuspended = false;
						}
					}
				}
				else
				{
					toolStripComboBox_MainTool_Terminal_AutoAction_Trigger.Text = triggerText.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripComboBox_MainTool_Terminal_AutoAction_Action_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var response = (toolStripComboBox_MainTool_Terminal_AutoAction_Action.SelectedItem as AutoActionEx);
			if (response != null)
				((Terminal)ActiveMdiChild).RequestAutoActionAction(response);
		}

		private void toolStripLabel_MainTool_Terminal_AutoAction_Count_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestAutoActionResetCount();
		}

		private void toolStripButton_MainTool_Terminal_AutoAction_Deactivate_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestAutoActionDeactivate();
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

		private void toolStripButton_MainTool_Terminal_Find_ShowHide_Click(object sender, EventArgs e)
		{
			this.localUserSettingsRoot.MainWindow.ShowFindField = !this.localUserSettingsRoot.MainWindow.ShowFindField;
		}

		private void toolStripComboBox_MainTool_Terminal_Find_Pattern_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.KeyData & Keys.KeyCode) == Keys.Enter)
			{
				var pattern = (toolStripComboBox_MainTool_Terminal_Find_Pattern.SelectedItem as string);
				if (pattern != null)
					((Terminal)ActiveMdiChild).Find(pattern);
			}
		}

		private void toolStripComboBox_MainTool_Terminal_Find_Pattern_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			var pattern = (toolStripComboBox_MainTool_Terminal_Find_Pattern.SelectedItem as string);
			if (pattern != null)
				((Terminal)ActiveMdiChild).Find(pattern);
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_MainTool_Terminal_Find_Pattern_TextChanged(object sender, EventArgs e)
		{
			if (toolStripComboBox_MainTool_Terminal_Find_Pattern.SelectedIndex == ControlEx.InvalidIndex)
			{
				string pattern = toolStripComboBox_MainTool_Terminal_Find_Pattern.Text;
				try
				{
					var regex = new Regex(pattern);
					UnusedLocal.PreventAnalysisWarning(regex);
				}
				catch (ArgumentException ex)
				{
					MessageBoxEx.Show
					(
						this,
						ex.Message,
						"Invalid Regex",
						MessageBoxButtons.OK,
						MessageBoxIcon.Error
					);
				}
			}
		}

		private void toolStripButton_MainTool_Terminal_Find_Next_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).FindNext();
		}

		private void toolStripButton_MainTool_Terminal_Find_Previous_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).FindPrevious();
		}

		private void toolStripButton_MainTool_Terminal_Log_Settings_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestEditLogSettings();
		}

		private void toolStripButton_MainTool_Terminal_Log_On_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestSwitchLogOn();
		}

		private void toolStripButton_MainTool_Terminal_Log_Off_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestSwitchLogOff();
		}

		private void toolStripButton_MainTool_Terminal_Log_Open_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestOpenLogFile();
		}

		private void toolStripButton_MainTool_Terminal_Log_OpenDirectory_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestOpenLogDirectory();
		}

		private void toolStripButton_MainTool_Terminal_Format_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestEditFormatSettings();
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
			ShowNewTerminalDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_Open_Click(object sender, EventArgs e)
		{
			ShowOpenFileDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_OpenWorkspace_Click(object sender, EventArgs e)
		{
			ShowOpenWorkspaceFromFileDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_Exit_Click(object sender, EventArgs e)
		{
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
			this.menuItems_recent = new List<ToolStripMenuItem>(Model.Settings.RecentFileSettings.MaxFilePaths); // Preset the required capacity to improve memory management.
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
				for (int i = 0; i < Model.Settings.RecentFileSettings.MaxFilePaths; i++)
				{
					string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1);
					this.menuItems_recent[i].Text = "&" + prefix;
					this.menuItems_recent[i].Visible = false;
				}

				// Show valid:
				for (int i = 0; i < ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count; i++)
				{
					string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1);
					string file = PathEx.Limit(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i].Item, 60);
					if (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i] != null)
					{
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

		/// <summary>
		/// Makes sure that context menus are at the right position upon first drop down. This is
		/// a fix, it should be that way by default. However, due to some reasons, they sometimes
		/// appear somewhere at the top-left corner of the screen if this fix isn't done.
		/// </summary>
		/// <remarks>
		/// Is this a .NET bug?
		/// 
		/// Saying hello to StyleCop ;-.
		/// </remarks>
		private void contextMenuStrip_FileRecent_Paint(object sender, PaintEventArgs e)
		{
			ContextMenuStrip contextMenuStrip = sender as ContextMenuStrip;
			if (contextMenuStrip != null)
			{
				if (contextMenuStrip.SourceControl == null)
				{
					contextMenuStrip_FileRecent.SuspendLayout();
					contextMenuStrip_FileRecent.Top = toolStripMenuItem_MainMenu_File_Recent.Bounds.Top;
					contextMenuStrip_FileRecent.Left = toolStripMenuItem_MainMenu_File_Recent.Bounds.Left + toolStripMenuItem_MainMenu_File_Recent.Bounds.Width;
					contextMenuStrip_FileRecent.ResumeLayout();
				}
			}
		}

		private void contextMenuStrip_FileRecent_Opening(object sender, CancelEventArgs e)
		{
			// No need to validate again, is already done on opening of parent menu
			// ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();

			contextMenuStrip_FileRecent_SetRecentMenuItems();
		}

		private void toolStripMenuItem_FileRecentContextMenu_Click(object sender, EventArgs e)
		{
			this.main.OpenRecent(ToolStripMenuItemEx.TagToIndex(sender)); // Attention, 'ToolStripMenuItem' is no 'Control'!
		}

		#endregion

		#region Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Status_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Checked = ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
			toolStripMenuItem_StatusContextMenu_ShowChrono.Checked       = ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
		}

		private void toolStripMenuItem_StatusContextMenu_ShowTerminalInfo_Click(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo = !ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
			ApplicationSettings.Save();
		}

		private void toolStripMenuItem_StatusContextMenu_ShowChrono_Click(object sender, EventArgs e)
		{
			if (this.isSettingControls)
				return;

			ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono = !ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
			ApplicationSettings.Save();
		}

		private void toolStripMenuItem_StatusContextMenu_Preferences_Click(object sender, EventArgs e)
		{
			ShowPreferences();
		}

		#endregion

		#region Controls Event Handlers > Status > Chrono
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status > Chrono
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

		#endregion

		#region Controls Event Handlers > Chrono
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Chrono
		//------------------------------------------------------------------------------------------

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

		private void ApplyWindowSettingsAccordingToStartup()
		{
			if (ApplicationSettings.LocalUserSettingsSuccessfullyLoadedFromFile)
			{
				SuspendLayout();

				// Window state:
				WindowState = ApplicationSettings.LocalUserSettings.MainWindow.WindowState;

				// Start position:
				var savedStartPosition = ApplicationSettings.LocalUserSettings.MainWindow.StartPosition;
				var savedLocation      = ApplicationSettings.LocalUserSettings.MainWindow.Location;
				var savedSize          = ApplicationSettings.LocalUserSettings.MainWindow.Size;

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

				ResumeLayout();
			}
		}

		private void SaveWindowSettings()
		{
			SaveWindowSettings(false);
		}

		private void SaveWindowSettings(bool setStartPositionToManual)
		{
			if (setStartPositionToManual)
			{
				ApplicationSettings.LocalUserSettings.MainWindow.StartPosition = FormStartPosition.Manual;
				StartPosition = ApplicationSettings.LocalUserSettings.MainWindow.StartPosition;
			}

			ApplicationSettings.LocalUserSettings.MainWindow.WindowState = WindowState;

			if ((StartPosition == FormStartPosition.Manual) && (WindowState == FormWindowState.Normal))
				ApplicationSettings.LocalUserSettings.MainWindow.Location = Location;

			if (WindowState == FormWindowState.Normal)
				ApplicationSettings.LocalUserSettings.MainWindow.Size = Size;

			ApplicationSettings.Save();
		}

		#endregion

		#region Preferences
		//==========================================================================================
		// Preferences
		//==========================================================================================

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowPreferences()
		{
			var f = new Preferences(ApplicationSettings.LocalUserSettings);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.MainWindow = f.SettingsResult.MainWindow;
				ApplicationSettings.LocalUserSettings.General    = f.SettingsResult.General;
				ApplicationSettings.Save();
			}
		}

		#endregion

		#region Settings
		//==========================================================================================
		// Settings
		//==========================================================================================

		private void AttachLocalUserSettingsEventHandlers()
		{
			if (this.localUserSettingsRoot != null)
				this.localUserSettingsRoot.Changed += localUserSettingsRoot_Changed;
		}

		private void DetachLocalUserSettingsEventHandlers()
		{
			if (this.localUserSettingsRoot != null)
				this.localUserSettingsRoot.Changed -= localUserSettingsRoot_Changed;
		}

		private void localUserSettingsRoot_Changed(object sender, SettingsEventArgs e)
		{
			SetMainControls();
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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void main_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			DialogResult dr;
			dr = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
			e.Result = dr;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void main_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			Cursor = e.Value;
		}

		private void main_Exited(object sender, EventArgs<Model.MainResult> e)
		{
			if (this.result == Model.MainResult.Success) // Otherwise, keep the previous result, e.g. 'ApplicationStartError'.
				this.result = e.Value;

			DetachMainEventHandlers();
			this.main = null;

			DetachLocalUserSettingsEventHandlers();
			this.localUserSettingsRoot = null;

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
			toolStripButton_MainTool_SetControls();

			this.isSettingControls.Enter();
			try
			{
				toolStripStatusLabel_MainStatus_TerminalInfo.Visible = ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
				toolStripStatusLabel_MainStatus_Chrono.Visible       = ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
			}
			finally
			{
				this.isSettingControls.Leave();
			}
		}

		private void SetChildControls()
		{
			toolStripButton_MainTool_SetControls();

			// Shortcuts associated to menu items are only active when items are visible and enabled!
			toolStripMenuItem_MainMenu_File_SetChildMenuItems();
			toolStripMenuItem_MainMenu_Log_SetMenuItems();
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems();
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

			if (this.workspace != null)
				this.TopMost = this.workspace.SettingsRoot.Workspace.AlwaysOnTop;
		}

		#region Main > Methods > New
		//------------------------------------------------------------------------------------------
		// Main > Methods > New
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowNewTerminalDialog()
		{
			ShowNewTerminalDialog(ApplicationSettings.LocalUserSettings.NewTerminal);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowNewTerminalDialog(Model.Settings.NewTerminalSettings newTerminalSettings)
		{
			SetFixedStatusText("New terminal...");

			var f = new NewTerminal(newTerminalSettings);
			if (ContextMenuStripShortcutModalFormWorkaround.InvokeShowDialog(f, this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.Save();

				var sh = new DocumentSettingsHandler<TerminalSettingsRoot>(f.TerminalSettingsResult);
				this.main.CreateNewTerminalFromSettings(sh);
			}
			else
			{
				// Still update to keep changed settings for next new terminal:
				ApplicationSettings.LocalUserSettings.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.Save();

				ResetStatusText();
			}
		}

		#endregion

		#region Main > Methods > Open File
		//------------------------------------------------------------------------------------------
		// Main > Methods > Open File
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenFileDialog()
		{
			SetFixedStatusText("Select a file...");

			var ofd = new OpenFileDialog();
			ofd.Title = "Open Terminal or Workspace";
			ofd.Filter      = ExtensionHelper.TerminalOrWorkspaceFilesFilter;
			ofd.FilterIndex = ExtensionHelper.TerminalOrWorkspaceFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.TerminalFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

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
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenWorkspaceFromFileDialog()
		{
			SetFixedStatusText("Select a file...");

			var ofd = new OpenFileDialog();
			ofd.Title = "Open Workspace";
			ofd.Filter      = ExtensionHelper.WorkspaceFilesFilter;
			ofd.FilterIndex = ExtensionHelper.WorkspaceFilesFilterDefault;
			ofd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.WorkspaceFile);
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (!string.IsNullOrEmpty(ofd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.main.OpenFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private DialogResult ShowSaveWorkspaceAsFileDialog()
		{
			SetFixedStatusText("Select a workspace file name...");

			var sfd = new SaveFileDialog();
			sfd.Title = "Save Workspace As";
			sfd.Filter      = ExtensionHelper.WorkspaceFilesFilter;
			sfd.FilterIndex = ExtensionHelper.WorkspaceFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.WorkspaceFile);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;

			// Other than for terminals, workspace 'Save As' always suggests 'UserName.yaw':
			sfd.FileName = Environment.UserName + PathEx.NormalizeExtension(sfd.DefaultExt); // Note that 'DefaultExt' states "The returned string does not include the period."!

			var dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (!string.IsNullOrEmpty(sfd.FileName)))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.MainFiles = Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.Save();

				this.workspace.SaveAs(sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}

			return (dr);
		}

		private void ToggleAlwaysOnTop()
		{
			if (this.workspace != null)
				this.workspace.SettingsRoot.Workspace.AlwaysOnTop = !this.workspace.SettingsRoot.Workspace.AlwaysOnTop;
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

		private bool AutoResponseVisibleInAnyTerminal
		{
			get
			{
				foreach (var anyTerminal in MdiChildren)
				{
					if (((Terminal)anyTerminal).UnderlyingTerminal.SettingsRoot.AutoResponse.Visible)
						return (true);
				}

				return (false);
			}
		}

		private void RequestAutoResponseVisibleInAllTerminals(bool visible)
		{
			foreach (var anyTerminal in MdiChildren)
			{
				((Terminal)anyTerminal).RequestAutoResponseVisible(visible);
			}
		}

		private bool AutoActionVisibleInAnyTerminal
		{
			get
			{
				foreach (var anyTerminal in MdiChildren)
				{
					if (((Terminal)anyTerminal).UnderlyingTerminal.SettingsRoot.AutoAction.Visible)
						return (true);
				}

				return (false);
			}
		}

		private void RequestAutoActionVisibleInAllTerminals(bool visible)
		{
			foreach (var anyTerminal in MdiChildren)
			{
				((Terminal)anyTerminal).RequestAutoActionVisible(visible);
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
				this.workspace.TerminalAdded   += workspace_TerminalAdded;
				this.workspace.TerminalRemoved += workspace_TerminalRemoved;

				this.workspace.TimedStatusTextRequest += workspace_TimedStatusTextRequest;
				this.workspace.FixedStatusTextRequest += workspace_FixedStatusTextRequest;
				this.workspace.MessageInputRequest    += workspace_MessageInputRequest;

				this.workspace.SaveAsFileDialogRequest += workspace_SaveAsFileDialogRequest;
				this.workspace.CursorRequest           += workspace_CursorRequest;
				
				this.workspace.Closed += workspace_Closed;

				if (this.workspace.SettingsRoot != null)
					this.workspace.SettingsRoot.Changed += workspaceSettingsRoot_Changed;
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.TerminalAdded   -= workspace_TerminalAdded;
				this.workspace.TerminalRemoved -= workspace_TerminalRemoved;

				this.workspace.TimedStatusTextRequest -= workspace_TimedStatusTextRequest;
				this.workspace.FixedStatusTextRequest -= workspace_FixedStatusTextRequest;
				this.workspace.MessageInputRequest    -= workspace_MessageInputRequest;

				this.workspace.SaveAsFileDialogRequest -= workspace_SaveAsFileDialogRequest;
				this.workspace.CursorRequest           -= workspace_CursorRequest;

				this.workspace.Closed -= workspace_Closed;

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
		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_TerminalAdded(object sender, EventArgs<Model.Terminal> e)
		{
			// Create terminal form and immediately show it:

			var mdiChild = new Terminal(e.Value);
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
		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_TerminalRemoved(object sender, EventArgs<Model.Terminal> e)
		{
			// Nothing to do, see remarks above.
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_TimedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetTimedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_FixedStatusTextRequest(object sender, EventArgs<string> e)
		{
			SetFixedStatusText(e.Value);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void workspace_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveWorkspaceAsFileDialog();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_CursorRequest(object sender, EventArgs<Cursor> e)
		{
			Cursor = e.Value;
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_Closed(object sender, Model.ClosedEventArgs e)
		{
			DetachWorkspaceEventHandlers();
			this.workspace = null;

			SetChildControls();
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
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

			terminalMdiChild.Changed    += terminalMdiChild_Changed;
			terminalMdiChild.Saved      += terminalMdiChild_Saved;

			terminalMdiChild.AutoResponseCountChanged += terminalMdiChild_AutoResponseCountChanged;
			terminalMdiChild.AutoActionCountChanged   += terminalMdiChild_AutoActionCountChanged;

			terminalMdiChild.Resize     += terminalMdiChild_Resize;
			terminalMdiChild.FormClosed += terminalMdiChild_FormClosed;
		}

		private void DetachTerminalEventHandlersAndMdiChildFromParent(Terminal terminalMdiChild)
		{
			terminalMdiChild.Changed    -= terminalMdiChild_Changed;
			terminalMdiChild.Saved      -= terminalMdiChild_Saved;

			terminalMdiChild.AutoResponseCountChanged -= terminalMdiChild_AutoResponseCountChanged;
			terminalMdiChild.AutoActionCountChanged   -= terminalMdiChild_AutoActionCountChanged;

			terminalMdiChild.Resize     -= terminalMdiChild_Resize;
			terminalMdiChild.FormClosed -= terminalMdiChild_FormClosed;

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
		////SetTimedStatus(Status.ChildChanged) is no longer used to limit information.

			SetChildControls();
		}

		private void terminalMdiChild_Saved(object sender, Model.SavedEventArgs e)
		{
			if (!e.IsAutoSave)
				SetTimedStatus(Status.ChildSaved);

			SetChildControls();
		}

		private void terminalMdiChild_AutoResponseCountChanged(object sender, EventArgs<int> e)
		{
			SetChildControls();
		}

		private void terminalMdiChild_AutoActionCountChanged(object sender, EventArgs<int> e)
		{
			SetChildControls();
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

			// Update the layout AFTER to ensure that closed MDI child has been disposed:
			this.invokeLayout = true;
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
			if (!this.localUserSettingsRoot.MainWindow.ShowFindField)
				this.localUserSettingsRoot.MainWindow.ShowFindField = true;

			toolStripComboBox_MainTool_Terminal_Find_Pattern.Select();
		}

		/// <summary>
		/// Gets whether the find is ready.
		/// </summary>
		public virtual bool FindIsReady
		{
			get
			{
				if (this.localUserSettingsRoot.MainWindow.ShowFindField)
					return (!string.IsNullOrEmpty(this.localUserSettingsRoot.MainWindow.ActiveFindPattern));
				else
					return (false);
			}
		}

		#endregion

		#region Status
		//==========================================================================================
		// Status
		//==========================================================================================

		private enum Status
		{
			ChildActivated,
		////ChildActive is no longer used to limit information.
		////ChildChanged is no longer used to limit information. Used to display "childText + " changed"" but that results in such messages each time a command is sent (due to the 'IsReadyToSend' changes). In order to get this again, 'IsReadyToSend' changes would have to be separated from the 'IOChanged' event.
			ChildSaved,
			ChildClosed,
			Default,
		}

		private string GetStatusText(Status status)
		{
			if (ActiveMdiChild != null)
			{
				string childText = "[" + ((Terminal)ActiveMdiChild).Text + "]";
				switch (status)
				{
					case Status.ChildActivated: return (childText + " activated");
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

		private void ResetStatusText()
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
		/// Using term 'Info' since the info contains name and indices.
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

		[Conditional("DEBUG")]
		private void DebugMessage(string message)
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
