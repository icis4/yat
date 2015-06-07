//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal/.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Gamma 2 Development Version 1.99.34
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2015 Matthias Kläy.
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
using System.Threading;
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.IO;
using MKY.Settings;
using MKY.Time;
using MKY.Windows.Forms;

using YAT.Model.Types;
using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Utilities;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "YAT.Gui.Forms.Main.#toolTip", Justification = "This is a bug in FxCop 1.36.")]

#endregion

namespace YAT.Gui.Forms
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

		// Status
		private const string DefaultStatusText = "Ready";
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
		private ClosingState closingState = ClosingState.None;
		private Model.Main.Result result = Model.Main.Result.Success;

		// Model:
		private Model.Main main;
		private Model.Workspace workspace;

		// Settings:
		private LocalUserSettingsRoot settingsRoot;

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
			WriteDebugMessageLine("Creating...");

			InitializeComponent();

			InitializeControls();

			// Register this form as the main form:
			NativeMessageHandler.RegisterMainForm(this);

			// Link and attach to main model:
			this.main = main;
			AttachMainEventHandlers();

			Text = this.main.AutoName;

			// Link and attach to terminal settings:
			this.settingsRoot = ApplicationSettings.LocalUserSettings;
			AttachSettingsEventHandlers();

			ApplyWindowSettingsAccordingToStartup();

			SetControls();

			WriteDebugMessageLine("...successfully created.");
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
		public Model.Main.Result Result
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
		/// event can depend on a properly drawn form, even when a modal dialog (e.g. a message box)
		/// is shown. This is due to the fact that the 'Paint' event will happen right after this
		/// 'Shown' event and will somehow be processed asynchronously.
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

			if (this.result != Model.Main.Result.Success)
			{
				bool showErrorModally = this.main.StartArgs.KeepOpenOnError;
				bool keepOpenOnError  = this.main.StartArgs.KeepOpenOnError;

				switch (this.result)
				{
					case Model.Main.Result.CommandLineError:
					{
						if (showErrorModally)
						{
							string executableName = Path.GetFileName(Application.ExecutablePath);
							string message =
								ApplicationInfo.ProductName + " could not be started because the given command line is invalid." + Environment.NewLine +
								@"Use """ + executableName + @" /?"" for command line help.";

							MessageBoxEx.Show
							(
								this,
								message,
								"Invalid Command Line",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning
							);
						}
						break;
					}

					case Model.Main.Result.ApplicationStartError:
					{
						if (showErrorModally)
						{
							MessageBoxEx.Show
							(
								this,
								ApplicationInfo.ProductName + " could not successfully be started with the given settings in the current environment!",
								"Start Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning
							);
						}
						break;
					}

					case Model.Main.Result.ApplicationRunError:
					{
						if (showErrorModally)
						{
							MessageBoxEx.Show
							(
								this,
								ApplicationInfo.ProductName + " could not successfully execute the requested operation!",
								"Execution Warning",
								MessageBoxButtons.OK,
								MessageBoxIcon.Warning
							);
						}
						break;
					}

					// Do nothing in the following cases:
					case Model.Main.Result.ApplicationExitError:
					case Model.Main.Result.UnhandledException:
					default:
					{
						break;
					}
				}

				if (!keepOpenOnError)
					Close();
			}
			else // In case of Model.Main.Result.Success.
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
						LayoutTerminals(WorkspaceLayout.TileHorizontal);
					else if (this.main.StartArgs.TileVertical)
						LayoutTerminals(WorkspaceLayout.TileVertical);
					else
						LayoutTerminals(this.workspace.SettingsRoot.Workspace.Layout);
				}

				// Automatically trigger start operation if desired:
				if (this.main.StartArgs.PerformOperationOnRequestedTerminal)
				{
					SetFixedStatusText("Triggering start operation...");
					timer_PerformStartOperation.Start();
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
				ResizeTerminals();
		}

		private void Main_MdiChildActivate(object sender, EventArgs e)
		{
			if (ActiveMdiChild != null)
			{
				this.workspace.ActivateTerminal(((Terminal)ActiveMdiChild).UnderlyingTerminal);

				// Activate the MDI child, to ensure that shortcuts effect the correct terminal.
				// Fixes bug #2996684.
				ActiveMdiChild.BringToFront();

				SetTimedStatus(Status.ChildActivated);
				SetTerminalText(this.workspace.ActiveTerminalStatusText);
			}
			else
			{
				SetTerminalText("");
			}

			SetChildControls();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Prevent multiple calls to Exit()/Close().
			if (this.closingState == ClosingState.None)
			{
				this.closingState = ClosingState.IsClosingFromForm;

				bool cancel;
				Model.Main.Result modelResult = this.main.Exit(out cancel);
				if (cancel)
				{
					e.Cancel = true;

					// Revert closing state in case of cancel:
					this.closingState = ClosingState.None;
					
					// Also revert closing state for MDI children:
					foreach (Form f in this.MdiChildren)
					{
						Terminal t = f as Terminal;
						if (t != null)
							t.RevertClosingState();
					}
				}
				else
				{
					this.result = modelResult;
				}
			}
		}

		private void Main_FormClosed(object sender, FormClosedEventArgs e)
		{
			DetachWorkspaceEventHandlers();
			this.workspace = null;

			DetachMainEventHandlers();
			this.main = null;

			DetachSettingsEventHandlers();
			this.settingsRoot = null;
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
		/// Must be called each time MDI child status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_File_SetChildMenuItems()
		{
			this.isSettingControls.Enter();

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_File_CloseAll.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_File_SaveAll.Enabled = childIsReady;

			this.isSettingControls.Leave();
		}

		/// <remarks>
		/// Must be called each time recent status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_File_SetRecentMenuItems()
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();

			bool recentsAreReady = (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0);
			toolStripMenuItem_MainMenu_File_Recent.Enabled = recentsAreReady;

			this.isSettingControls.Leave();
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

			bool workspaceIsReady = (this.workspace != null);

			bool workspaceFileIsWritable = false;
			if (workspaceIsReady)
				workspaceFileIsWritable = this.workspace.SettingsFileIsWritable;

			toolStripMenuItem_MainMenu_File_Workspace_New.Enabled    = !workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_Close.Enabled  = workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_Save.Enabled   = workspaceIsReady && workspaceFileIsWritable;
			toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Enabled = workspaceIsReady;

			this.isSettingControls.Leave();
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

		#region Controls Event Handlers > Main Menu > Window
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Window
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time MDI child status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_Window_SetChildMenuItems()
		{
			this.isSettingControls.Enter();

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_Window_Cascade.Enabled        = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileVertical.Enabled   = childIsReady;

			this.isSettingControls.Leave();

#if (FALSE)
			// \fixme:
			// I don't know how to fix bug #1808494 "MDI window list invisible if no MDI children".
			// The following code doesn't fix it. Could it even be a .NET bug?
			if (childIsReady)
				menuStrip_Main.MdiWindowListItem = toolStripMenuItem_MainMenu_Window;
			else
				menuStrip_Main.MdiWindowListItem = null;
#endif
		}

		private void toolStripMenuItem_MainMenu_Window_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems();
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
		
		#endregion

		#region Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_Contents_Click(object sender, EventArgs e)
		{
			Gui.Forms.Help f = new Gui.Forms.Help();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			Gui.Forms.ReleaseNotes f = new Gui.Forms.ReleaseNotes();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestFeature_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerType.Feature);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_SubmitBug_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerType.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_About_Click(object sender, EventArgs e)
		{
			Gui.Forms.About f = new Gui.Forms.About();
			f.ShowDialog(this);
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

			bool childIsReady = (ActiveMdiChild != null);

			bool terminalFileIsWritable = false;
			if (childIsReady)
				terminalFileIsWritable = ((Gui.Forms.Terminal)ActiveMdiChild).SettingsFileIsWritable;

			bool terminalIsStopped = false;
			if (childIsReady)
				terminalIsStopped = ((Gui.Forms.Terminal)ActiveMdiChild).IsStopped;

			bool terminalIsStarted = false;
			if (childIsReady)
				terminalIsStarted = ((Gui.Forms.Terminal)ActiveMdiChild).IsStarted;

			bool radixIsReady = false;
			Domain.Radix radix = Domain.Radix.None;
			if (childIsReady)
			{
				Model.Terminal terminal = ((Gui.Forms.Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((terminal != null) && (!terminal.IsDisposed))
				{
					radixIsReady = !(terminal.SettingsRoot.Display.SeparateTxRxRadix);
					if (radixIsReady)
						radix = terminal.SettingsRoot.Display.TxRadix;
				}
			}

			toolStripButton_MainTool_File_Save.Enabled      = childIsReady && terminalFileIsWritable;
			toolStripButton_MainTool_Terminal_Start.Enabled = childIsReady && terminalIsStopped;
			toolStripButton_MainTool_Terminal_Stop.Enabled  = childIsReady && terminalIsStarted;

			toolStripButton_MainTool_Terminal_Radix_String.Enabled = childIsReady && radixIsReady;
			toolStripButton_MainTool_Terminal_Radix_Char.Enabled   = childIsReady && radixIsReady;
			toolStripButton_MainTool_Terminal_Radix_Bin.Enabled    = childIsReady && radixIsReady;
			toolStripButton_MainTool_Terminal_Radix_Oct.Enabled    = childIsReady && radixIsReady;
			toolStripButton_MainTool_Terminal_Radix_Dec.Enabled    = childIsReady && radixIsReady;
			toolStripButton_MainTool_Terminal_Radix_Hex.Enabled    = childIsReady && radixIsReady;

			toolStripButton_MainTool_Terminal_Radix_String.Checked = (radix == Domain.Radix.String);
			toolStripButton_MainTool_Terminal_Radix_Char.Checked   = (radix == Domain.Radix.Char);
			toolStripButton_MainTool_Terminal_Radix_Bin.Checked    = (radix == Domain.Radix.Bin);
			toolStripButton_MainTool_Terminal_Radix_Oct.Checked    = (radix == Domain.Radix.Oct);
			toolStripButton_MainTool_Terminal_Radix_Dec.Checked    = (radix == Domain.Radix.Dec);
			toolStripButton_MainTool_Terminal_Radix_Hex.Checked    = (radix == Domain.Radix.Hex);

			toolStripButton_MainTool_Terminal_Clear.Enabled           = childIsReady;
			toolStripButton_MainTool_Terminal_SaveToFile.Enabled      = childIsReady;
			toolStripButton_MainTool_Terminal_CopyToClipboard.Enabled = childIsReady;
			toolStripButton_MainTool_Terminal_Print.Enabled           = childIsReady;
			toolStripButton_MainTool_Terminal_Settings.Enabled        = childIsReady;

			this.isSettingControls.Leave();
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

		private void toolStripButton_MainTool_Terminal_Start_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestStartTerminal();
		}

		private void toolStripButton_MainTool_Terminal_Stop_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestStopTerminal();
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

		private void toolStripButton_MainTool_Terminal_Clear_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestClear();
		}

		private void toolStripButton_MainTool_Terminal_SaveToFile_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestSaveToFile();
		}

		private void toolStripButton_MainTool_Terminal_CopyToClipboard_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestCopyToClipboard();
		}

		private void toolStripButton_MainTool_Terminal_Print_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestPrint();
		}

		private void toolStripButton_MainTool_Terminal_Settings_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestEditTerminalSettings();
		}

		#endregion

		#region Controls Event Handlers > Main Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Context Menu
		//------------------------------------------------------------------------------------------

		/// <remarks>
		/// Must be called each time recent status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_Main_SetRecentMenuItems()
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();

			bool recentsAreReady = (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0);
			toolStripMenuItem_MainContextMenu_File_Recent.Enabled = recentsAreReady;

			this.isSettingControls.Leave();
		}

		private void contextMenuStrip_Main_Opening(object sender, CancelEventArgs e)
		{
			// Prevent context menu being displayed within the child window.
			if (ActiveMdiChild != null)
			{
				e.Cancel = true;
				return;
			}

			contextMenuStrip_Main_SetRecentMenuItems();
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
		private List<ToolStripMenuItem> menuItems_recents;

		private void contextMenuStrip_FileRecent_InitializeControls()
		{
			this.menuItems_recents = new List<ToolStripMenuItem>(Model.Settings.RecentFileSettings.MaxFilePaths);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_1);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_2);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_3);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_4);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_5);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_6);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_7);
			this.menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_8);
		}

		/// <remarks>
		/// Must be called each time recent status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_FileRecent_SetRecentMenuItems()
		{
			this.isSettingControls.Enter();

			// Hide all.
			for (int i = 0; i < Model.Settings.RecentFileSettings.MaxFilePaths; i++)
			{
				string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1);
				this.menuItems_recents[i].Text = "&" + prefix;
				this.menuItems_recents[i].Visible = false;
			}

			// Show valid.
			for (int i = 0; i < ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count; i++)
			{
				string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1);
				string file = PathEx.LimitPath(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i].Item, 60);
				if (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i] != null)
				{
					this.menuItems_recents[i].Text = "&" + prefix + file;
					this.menuItems_recents[i].Enabled = true;
				}
				else
				{
					this.menuItems_recents[i].Text = "&" + prefix;
					this.menuItems_recents[i].Enabled = false;
				}
				this.menuItems_recents[i].Visible = true;
			}

			this.isSettingControls.Leave();
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
			this.main.OpenRecent(int.Parse((string)(((ToolStripMenuItem)sender).Tag), CultureInfo.InvariantCulture));
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
			if (!this.isSettingControls)
			{
				ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo = !ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
				ApplicationSettings.Save();
			}
		}

		private void toolStripMenuItem_StatusContextMenu_ShowChrono_Click(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono = !ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;
				ApplicationSettings.Save();
			}
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
			toolStripStatusLabel_MainStatus_Chrono.Text = TimeSpanEx.FormatInvariantTimeSpan(e.TimeSpan, true, true);
		}

		#endregion

		#region Controls Event Handlers > PerformStart/ExitOperation
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > PerformStart/ExitOperation
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void timer_PerformStartOperation_Tick(object sender, EventArgs e)
		{
			int id = this.main.StartArgs.RequestedDynamicTerminalIndex;
			Model.Terminal terminal = this.workspace.GetTerminalByDynamicIndex(id);
			if (terminal == null)
			{
				SetTimedStatusText("Trigger received, pending until terminal has been created...");
				return; // Pend!
			}
			if (!terminal.IsStarted)
			{
				SetTimedStatusText("Trigger received, pending until terminal has been started...");
				return; // Pend!
			}
			if (!terminal.IsReadyToSend)
			{
				SetTimedStatusText("Trigger received, pending until terminal is ready to transmit...");
				return; // Pend!
			}									// Using term 'Transmission' to indicate potential
												// 'intelligence' to send + receive/verify the data.
			// Preconditions fullfilled.
			timer_PerformStartOperation.Stop();
			SetTimedStatusText("Trigger received, preparing automatic transmission...");

			// Automatically transmit file if desired.
			string filePath = this.main.StartArgs.RequestedTransmitFilePath;
			if (!string.IsNullOrEmpty(filePath))
			{
				try
				{
					SetFixedStatusText("Automatically transmitting data on terminal " + id);
					terminal.SendFile(new Model.Types.Command("", true, filePath));
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
					this.result = Model.Main.Result.ApplicationRunError;
				}

				if ((!this.main.StartArgs.KeepOpen) &&
					(!(this.main.StartArgs.KeepOpenOnError && (this.result != Model.Main.Result.Success))))
				{
					timer_PerformExitOperation.Start();
				}
			}
			else
			{
				this.result = Model.Main.Result.ApplicationRunError;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Ensure that operation succeeds in any case.")]
		private void timer_PerformExitOperation_Tick(object sender, EventArgs e)
		{
			int id = this.main.StartArgs.RequestedDynamicTerminalIndex;
			Model.Terminal terminal = this.workspace.GetTerminalByDynamicIndex(id);
			if ((terminal != null) && (terminal.IsBusy))
			{
				return; // Pend!
			}

			if ((!this.main.StartArgs.KeepOpen) &&
				(!(this.main.StartArgs.KeepOpenOnError && (this.result != Model.Main.Result.Success))))
			{
				try
				{
					SetFixedStatusText("Automatically closing " + ApplicationInfo.ProductName);
					Close();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(GetType(), ex);
					this.result = Model.Main.Result.ApplicationExitError;
				}
			}
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

				// Retrieve saved settings.
				FormWindowState savedWindowState = ApplicationSettings.LocalUserSettings.MainWindow.WindowState;
				FormStartPosition savedStartPosition = ApplicationSettings.LocalUserSettings.MainWindow.StartPosition;

				Point savedLocation = ApplicationSettings.LocalUserSettings.MainWindow.Location;
				Size savedSize = ApplicationSettings.LocalUserSettings.MainWindow.Size;
				Rectangle savedBounds = new Rectangle(savedLocation, savedSize);

				bool contains = false;
				foreach (Screen screen in Screen.AllScreens)
				{
					// Retrieve current bounds to ensure that main form is displayed within the visible bounds.
					if (screen.Bounds.Contains(savedBounds))
					{
						contains = true;
						break;
					}
				}

				// Keep settings if within bounds. Adjust start position if out of bounds.
				// Must be adjusted regardless of the window state since the state may be changed by the user.
				if (contains)
				{
					// Set saved settings.
					WindowState = savedWindowState;
					StartPosition = savedStartPosition;
					Location = savedLocation;
					Size = savedSize;
				}
				else
				{
					// Let the operating system adjust the bounds.
					WindowState = savedWindowState;
					StartPosition = FormStartPosition.WindowsDefaultBounds;
				}

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
			Gui.Forms.Preferences f = new Gui.Forms.Preferences(ApplicationSettings.LocalUserSettings);
			if (f.ShowDialog(this) == DialogResult.OK)
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
			SetMainControls();
		}

		#endregion

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
				this.main.WorkspaceOpened += new EventHandler<Model.WorkspaceEventArgs>(main_WorkspaceOpened);
				this.main.WorkspaceClosed += new EventHandler(main_WorkspaceClosed);

				this.main.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(main_FixedStatusTextRequest);
				this.main.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(main_TimedStatusTextRequest);
				this.main.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(main_MessageInputRequest);

				this.main.Exited += new EventHandler(main_Exited);
			}
		}

		private void DetachMainEventHandlers()
		{
			if (this.main != null)
			{
				this.main.WorkspaceOpened -= new EventHandler<Model.WorkspaceEventArgs>(main_WorkspaceOpened);
				this.main.WorkspaceClosed -= new EventHandler(main_WorkspaceClosed);

				this.main.FixedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(main_FixedStatusTextRequest);
				this.main.TimedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(main_TimedStatusTextRequest);
				this.main.MessageInputRequest    -= new EventHandler<Model.MessageInputEventArgs>(main_MessageInputRequest);

				this.main.Exited -= new EventHandler(main_Exited);
			}
		}

		#endregion

		#region Main > Event Handlers
		//------------------------------------------------------------------------------------------
		// Main > Event Handlers
		//------------------------------------------------------------------------------------------

		private void main_WorkspaceOpened(object sender, Model.WorkspaceEventArgs e)
		{
			this.workspace = e.Workspace;
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

		private void main_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		private void main_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void main_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			DialogResult dr;
			dr = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
			e.Result = dr;
		}

		private void main_Exited(object sender, EventArgs e)
		{
			// Prevent multiple calls to Close().
			if (this.closingState == ClosingState.None)
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
			this.isSettingControls.Enter();

			toolStripStatusLabel_MainStatus_TerminalInfo.Visible = ApplicationSettings.LocalUserSettings.MainWindow.ShowTerminalInfo;
			toolStripStatusLabel_MainStatus_Chrono.Visible       = ApplicationSettings.LocalUserSettings.MainWindow.ShowChrono;

			this.isSettingControls.Leave();
		}

		private void SetChildControls()
		{
			toolStripButton_MainTool_SetControls();

			toolStripMenuItem_MainMenu_File_SetChildMenuItems();
			toolStripMenuItem_MainMenu_Window_SetChildMenuItems();
		}

		private void SetRecentControls()
		{
			toolStripMenuItem_MainMenu_File_SetRecentMenuItems();

			contextMenuStrip_Main_SetRecentMenuItems();
			contextMenuStrip_FileRecent_SetRecentMenuItems();
		}

		private void SetWorkspaceControls()
		{
			toolStripMenuItem_MainMenu_File_Workspace_SetMenuItems();
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

			Gui.Forms.NewTerminal f = new Gui.Forms.NewTerminal(newTerminalSettings);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.Save();

				DocumentSettingsHandler<TerminalSettingsRoot> settingsHandler = new DocumentSettingsHandler<TerminalSettingsRoot>(f.TerminalSettingsResult);
				this.main.CreateNewTerminalFromSettings(settingsHandler);
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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenFileDialog()
		{
			SetFixedStatusText("Select a file...");

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open Terminal or Workspace";
			ofd.Filter      = ExtensionSettings.TerminalOrWorkspaceFilesFilter;
			ofd.FilterIndex = ExtensionSettings.TerminalOrWorkspaceFilesFilterDefault;
			ofd.DefaultExt  = ExtensionSettings.TerminalFile;
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.TerminalFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				// \remind (2012-09-19 / mky @ Dalian ;-)
				// As soon as "PathSettings.TerminalFilesPath and .WorkspaceFilesPath should be merged"
				// has been implemented, the following if-elif-else can be merged back as well.
				if (ExtensionSettings.IsTerminalFile(ofd.FileName))
				{
					ApplicationSettings.LocalUserSettings.Paths.TerminalFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
					ApplicationSettings.Save();

					this.main.OpenFromFile(ofd.FileName);
				}
				else if (ExtensionSettings.IsWorkspaceFile(ofd.FileName))
				{
					ApplicationSettings.LocalUserSettings.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
					ApplicationSettings.Save();

					this.main.OpenFromFile(ofd.FileName);
				}
				else
				{
					throw (new InvalidOperationException("Program execution should never get here, please report this bug!"));
				}
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with 'symmetricity'?")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenWorkspaceFromFileDialog()
		{
			SetFixedStatusText("Select a file...");

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open Workspace";
			ofd.Filter      = ExtensionSettings.WorkspaceFilesFilter;
			ofd.FilterIndex = ExtensionSettings.WorkspaceFilesFilterDefault;
			ofd.DefaultExt  = ExtensionSettings.WorkspaceFile;
			ofd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.WorkspaceFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
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

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save Workspace As";
			sfd.Filter      = ExtensionSettings.WorkspaceFilesFilter;
			sfd.FilterIndex = ExtensionSettings.WorkspaceFilesFilterDefault;
			sfd.DefaultExt  = ExtensionSettings.WorkspaceFile;
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.WorkspaceFilesPath;

			// Other than for terminal files, the workspace 'Save As' always suggests 'UserName.yaw'.
			sfd.FileName = Environment.UserName + "." + sfd.DefaultExt;

			DialogResult dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.Save();

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

				LayoutTerminals(layout);
			}
		}

		private void ResizeTerminals()
		{
			// Simply forward the resize request to the MDI layout engine:
			if (this.workspace != null)
				LayoutTerminals(this.workspace.SettingsRoot.Workspace.Layout);
		}

		/// <summary>
		/// Performs the layout operation on the terminals.
		/// </summary>
		/// <remarks>
		/// Uses the MDI functionality Windows.Forms environment to perform the layout.
		/// </remarks>
		private void LayoutTerminals(WorkspaceLayout layout)
		{
			switch (layout)
			{
				case WorkspaceLayout.Manual:
					// Nothing to do. Manual layout is kept as is.
					break;

				case WorkspaceLayout.Cascade:
				case WorkspaceLayout.TileHorizontal:
				case WorkspaceLayout.TileVertical:
					LayoutMdi((WorkspaceLayoutEx)layout);
					break;

				default:
					throw (new InvalidOperationException("Program execution should never get here, " + layout + " is an invalid workspace layout!"));
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
			base.LayoutMdi(value);
			this.isLayoutingMdi = false;
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
				this.workspace.TerminalAdded   += new EventHandler<Model.TerminalEventArgs>(workspace_TerminalAdded);
				this.workspace.TerminalRemoved += new EventHandler<Model.TerminalEventArgs>(workspace_TerminalRemoved);

				this.workspace.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(workspace_TimedStatusTextRequest);
				this.workspace.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(workspace_FixedStatusTextRequest);
				this.workspace.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(workspace_MessageInputRequest);

				this.workspace.SaveAsFileDialogRequest += new EventHandler<Model.DialogEventArgs>(workspace_SaveAsFileDialogRequest);
				
				this.workspace.Closed += new EventHandler<Model.ClosedEventArgs>(workspace_Closed);
			}
		}

		private void DetachWorkspaceEventHandlers()
		{
			if (this.workspace != null)
			{
				this.workspace.TerminalAdded   -= new EventHandler<Model.TerminalEventArgs>(workspace_TerminalAdded);
				this.workspace.TerminalRemoved -= new EventHandler<Model.TerminalEventArgs>(workspace_TerminalRemoved);

				this.workspace.TimedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(workspace_TimedStatusTextRequest);
				this.workspace.FixedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(workspace_FixedStatusTextRequest);
				this.workspace.MessageInputRequest    -= new EventHandler<Model.MessageInputEventArgs>(workspace_MessageInputRequest);

				this.workspace.SaveAsFileDialogRequest -= new EventHandler<Model.DialogEventArgs>(workspace_SaveAsFileDialogRequest);

				this.workspace.Closed -= new EventHandler<Model.ClosedEventArgs>(workspace_Closed);
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
		private void workspace_TerminalAdded(object sender, Model.TerminalEventArgs e)
		{
			// Create terminal form and immediately show it.
			Terminal mdiChild = new Terminal(e.Terminal);
			AttachTerminalEventHandlersAndMdiChildToParent(mdiChild);
			mdiChild.Show();
			SetChildControls();
		}

		/// <remarks>
		/// Terminal is removed in <see cref="terminalMdiChild_FormClosed"/> event handler.
		/// </remarks>
		private void workspace_TerminalRemoved(object sender, Model.TerminalEventArgs e)
		{
			// Nothing to do, see remarks above.
		}

		private void workspace_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		private void workspace_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void workspace_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = MessageBoxEx.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		private void workspace_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveWorkspaceAsFileDialog();
		}

		private void workspace_Closed(object sender, Model.ClosedEventArgs e)
		{
			DetachWorkspaceEventHandlers();
			this.workspace = null;

			SetChildControls();
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

			terminalMdiChild.Changed    += new EventHandler(terminalMdiChild_Changed);
			terminalMdiChild.Saved      += new EventHandler<Model.SavedEventArgs>(terminalMdiChild_Saved);
			terminalMdiChild.Resize     += new EventHandler(terminalMdiChild_Resize);
			terminalMdiChild.FormClosed += new FormClosedEventHandler(terminalMdiChild_FormClosed);
		}

		private void DetachTerminalEventHandlersAndMdiChildFromParent(Terminal terminalMdiChild)
		{
			terminalMdiChild.Changed    -= new EventHandler(terminalMdiChild_Changed);
			terminalMdiChild.Saved      -= new EventHandler<Model.SavedEventArgs>(terminalMdiChild_Saved);
			terminalMdiChild.Resize     -= new EventHandler(terminalMdiChild_Resize);
			terminalMdiChild.FormClosed -= new FormClosedEventHandler(terminalMdiChild_FormClosed);

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

		private void terminalMdiChild_Resize(object sender, EventArgs e)
		{
			if (!isLayoutingMdi)
				SetTerminalLayout(WorkspaceLayout.Manual);
		}

		private void terminalMdiChild_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Sender MUST be a terminal, otherwise something must have freaked out...
			DetachTerminalEventHandlersAndMdiChildFromParent(sender as Terminal);

			SetTimedStatus(Status.ChildClosed);
			SetChildControls();
		}

		#endregion

		#endregion

		#region Status
		//==========================================================================================
		// Status
		//==========================================================================================

		private enum Status
		{
			ChildActivated,
			ChildActive,
			ChildChanged,
			ChildSaved,
			ChildClosed,
			Default,
		}

		private string GetStatusText(Status status)
		{
			if (ActiveMdiChild != null)
			{
				string childText = "[" + ((Gui.Forms.Terminal)ActiveMdiChild).Text + "]";
				switch (status)
				{
					case Status.ChildActivated: return (childText + " activated");
					case Status.ChildActive:    return (""); // Display nothing to limit information.
					case Status.ChildChanged:   return (childText + " changed");
					case Status.ChildSaved:     return (childText + " saved");
					case Status.ChildClosed:    return (childText + " closed");
				}
			}
			return (DefaultStatusText);
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
			if (ActiveMdiChild != null)
				SetFixedStatus(Status.ChildActive);
			else
				SetFixedStatus(Status.Default);
		}

		private void timer_Status_Tick(object sender, EventArgs e)
		{
			timer_Status.Enabled = false;
			ResetStatusText();
		}

		private void SetTerminalText(string text)
		{
			toolStripStatusLabel_MainStatus_TerminalInfo.Text = text;
		}

		#endregion

		#region Debug
		//==========================================================================================
		// Debug
		//==========================================================================================

		[Conditional("DEBUG")]
		private void WriteDebugMessageLine(string message)
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
