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
using System.Text;
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
using YAT.View.Utilities;

#endregion

#region Module-level FxCop suppressions
//==================================================================================================
// Module-level FxCop suppressions
//==================================================================================================

[module: SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "YAT.View.Forms.Main.#toolTip", Justification = "This is a bug in FxCop 1.36.")]

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
		private bool invokeLayout = false;
		private ClosingState closingState = ClosingState.None;
		private Model.MainResult result = Model.MainResult.Success;

		// Model:
		private Model.Main main;
		private Model.Workspace workspace;

		// Settings:
		private LocalUserSettingsRoot localUserSettingsRoot;

		// Toolstrip-combobox-validation-workaround (too late invocation of 'Validate' event):
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

			Text = this.main.AutoName;

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

							StringBuilder sb = new StringBuilder();
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
							StringBuilder sb = new StringBuilder();
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

					case Model.MainResult.ApplicationRunError:
					{
						if (showErrorModally)
						{
							StringBuilder sb = new StringBuilder();
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
						LayoutTerminals();
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
				ActiveMdiChild.BringToFront(); // Fixes bug #2996684>#152.

				if (this.invokeLayout)
				{
					this.invokeLayout = false;
					LayoutTerminals();
				}

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
				Model.MainResult modelResult = this.main.Exit(out cancel);
				if (cancel)
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

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_File_CloseAll.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_File_SaveAll.Enabled  = childIsReady;

			this.isSettingControls.Leave();
		}

		/// <remarks>
		/// Must be called each time the corresponding context state changes, because shortcuts
		/// associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_File_SetRecentMenuItems()
		{
			ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();

			toolStripMenuItem_MainMenu_File_Recent.Enabled = (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0);

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

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_Log_AllOn.Enabled    = childIsReady;
			toolStripMenuItem_MainMenu_Log_AllOff.Enabled   = childIsReady;
			toolStripMenuItem_MainMenu_Log_AllClear.Enabled = childIsReady;

			this.isSettingControls.Leave();
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
			this.isSettingControls.Enter();

			bool workspaceIsReady = (this.workspace != null);
			toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Enabled = workspaceIsReady;

			bool alwaysOnTop = ((this.workspace != null) ? this.workspace.SettingsRoot.Workspace.AlwaysOnTop : false);
			toolStripMenuItem_MainMenu_Window_AlwaysOnTop.Checked = alwaysOnTop;

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_Window_Automatic.Enabled      = childIsReady;
			toolStripMenuItem_MainMenu_Window_Cascade.Enabled        = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileVertical.Enabled   = childIsReady;
			toolStripMenuItem_MainMenu_Window_Minimize.Enabled       = childIsReady;
			toolStripMenuItem_MainMenu_Window_Maximize.Enabled       = childIsReady;

			this.isSettingControls.Leave();

			// This is a work-around to the following bugs:
			//  > #119 "MDI child list isn't always updated"
			//  > #180 "Menu update of the terminal settings"
			//  > #213 "Wrong indication 'COM 1 - Closed'"
			//
			// Attention, only apply the work-around in case of the event below!
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
			Form f = new Help();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			Form f = new ReleaseNotes();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestSupport_Click(object sender, EventArgs e)
		{
			Form f = new TrackerInstructions(TrackerType.Support);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_RequestFeature_Click(object sender, EventArgs e)
		{
			Form f = new TrackerInstructions(TrackerType.Feature);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_SubmitBug_Click(object sender, EventArgs e)
		{
			Form f = new TrackerInstructions(TrackerType.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void toolStripMenuItem_MainMenu_Help_About_Click(object sender, EventArgs e)
		{
			Form f = new About();
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
				terminalFileIsWritable = ((Terminal)ActiveMdiChild).SettingsFileIsWritable;

			bool terminalIsStopped = false;
			if (childIsReady)
				terminalIsStopped = ((Terminal)ActiveMdiChild).IsStopped;

			bool terminalIsStarted = false;
			if (childIsReady)
				terminalIsStarted = ((Terminal)ActiveMdiChild).IsStarted;

			bool radixIsReady = false;
			Domain.Radix radix = Domain.Radix.None;
			if (childIsReady)
			{
				Model.Terminal terminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((terminal != null) && (!terminal.IsDisposed))
				{
					radixIsReady = !(terminal.SettingsRoot.Display.SeparateTxRxRadix);
					if (radixIsReady)
						radix = terminal.SettingsRoot.Display.TxRadix;
				}
			}

			bool logIsOn = false;
			if (childIsReady)
				logIsOn = ((Terminal)ActiveMdiChild).LogIsOn;

			bool logFileExists = false;
			if (childIsReady)
				logFileExists = ((Terminal)ActiveMdiChild).LogFileExists;

			toolStripButton_MainTool_File_Save.Enabled         = childIsReady && terminalFileIsWritable;
			toolStripButton_MainTool_Terminal_Start.Enabled    = childIsReady && terminalIsStopped;
			toolStripButton_MainTool_Terminal_Stop.Enabled     = childIsReady && terminalIsStarted;
			toolStripButton_MainTool_Terminal_Settings.Enabled = childIsReady;

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

			bool arVisible = false;

			AutoTriggerEx[] arTriggerItems = AutoTriggerEx.GetFixedItems();
			AutoTriggerEx   arTrigger      = AutoTrigger.None;

			AutoResponseEx[] arResponseItems = AutoResponseEx.GetFixedItems();
			AutoResponseEx   arResponse      = AutoResponse.None;

			if (childIsReady)
			{
				Model.Terminal terminal = ((Terminal)ActiveMdiChild).UnderlyingTerminal;
				if ((terminal != null) && (!terminal.IsDisposed))
				{
					arVisible      = terminal.SettingsRoot.AutoResponse.Visible;

					arTriggerItems = terminal.SettingsRoot.ValidAutoResponseTriggerItems;
					arTrigger      = terminal.SettingsRoot.AutoResponse.Trigger;

					arResponseItems = terminal.SettingsRoot.ValidAutoResponseResponseItems;
					arResponse      = terminal.SettingsRoot.AutoResponse.Response;
				}
			}

			toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Enabled = childIsReady;

			if (arVisible)
			{
				toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Text = "Hide AutoResponse command";

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

					if (arTrigger != null)
					{
						if (toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Items.Contains(arTrigger))
						{	// Applies if an item of the combo box is selected.
							toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedItem = arTrigger;
						}
						else
						{	// Applies if an item that is not in the combo box is selected.
							toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedIndex = ControlEx.InvalidIndex;
							toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Text = new Command(arTrigger).SingleLineText;
						}
					}
					else
					{	// Item doesn't exist, use default = first item in the combo box, or none if list is empty.
						toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedIndex = 0;
					}

					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Visible = true;
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Enabled = childIsReady;
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.Clear();
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.AddRange(arResponseItems);

					if (arResponse != null)
					{
						if (toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.Contains(arResponse))
						{	// Applies if an item of the combo box is selected.
							toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedItem = arResponse;
						}
						else
						{	// Applies if an item that is not in the combo box is selected.
							toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedIndex = ControlEx.InvalidIndex;
							toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Text = new Command(arResponse).SingleLineText;
						}
					}
					else
					{	// Item doesn't exist, use default = first item in the combo box, or none if list is empty.
						toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedIndex = 0;
					}
				}
			}
			else
			{
				toolStripButton_MainTool_Terminal_AutoResponse_ShowHide.Text = "Show AutoResponse command";

				toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Visible = false;
				toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Enabled = false;
				toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Items.Clear();

				toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Visible = false;
				toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Enabled = false;
				toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Items.Clear();
			}

			toolStripButton_MainTool_Terminal_Clear.Enabled             = childIsReady;
			toolStripButton_MainTool_Terminal_Refresh.Enabled           = childIsReady;
			toolStripButton_MainTool_Terminal_CopyToClipboard.Enabled   = childIsReady;
			toolStripButton_MainTool_Terminal_SaveToFile.Enabled        = childIsReady;
			toolStripButton_MainTool_Terminal_Print.Enabled             = childIsReady;

			toolStripButton_MainTool_Terminal_Log_Settings.Enabled      = childIsReady;
			toolStripButton_MainTool_Terminal_Log_On.Enabled            = childIsReady && !logIsOn;
			toolStripButton_MainTool_Terminal_Log_Off.Enabled           = childIsReady &&  logIsOn;
			toolStripButton_MainTool_Terminal_Log_OpenFile.Enabled      = childIsReady &&  logFileExists;
			toolStripButton_MainTool_Terminal_Log_OpenDirectory.Enabled = childIsReady &&  logFileExists;

			toolStripButton_MainTool_Terminal_Format.Enabled            = childIsReady;

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

		private void toolStripButton_MainTool_Terminal_AutoResponse_ShowHide_Click(object sender, EventArgs e)
		{
			((Terminal)ActiveMdiChild).RequestToggleAutoResponseVisible();
		}

		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var trigger = (toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedItem as AutoTriggerEx);
				if (trigger != null)
					((Terminal)ActiveMdiChild).RequestAutoResponseTrigger(trigger);
			}
		}

		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger_TextChanged(object sender, EventArgs e)
		{
			if (toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.SelectedIndex == ControlEx.InvalidIndex)
			{
				string triggerText = toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Text;
				int invalidTextStart;
				if (Validation.ValidateText(this, "automatic response trigger", triggerText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.mainToolValidationWorkaround_UpdateIsSuspended = true;
						((Terminal)ActiveMdiChild).RequestAutoResponseTrigger(triggerText);
						this.mainToolValidationWorkaround_UpdateIsSuspended = false;
					}
				}
				else
				{
					toolStripComboBox_MainTool_Terminal_AutoResponse_Trigger.Text.Remove(invalidTextStart);
				}
			}
		}

		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Response_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				var response = (toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedItem as AutoResponseEx);
				if (response != null)
					((Terminal)ActiveMdiChild).RequestAutoResponseResponse(response);
			}
		}

		/// <remarks>
		/// The 'TextChanged' instead of the 'Validating' event is used because tool strip combo boxes invoke that event way too late,
		/// only when the hosting control (i.e. the whole tool bar) is being validated.
		/// </remarks>
		private void toolStripComboBox_MainTool_Terminal_AutoResponse_Response_TextChanged(object sender, EventArgs e)
		{
			if (toolStripComboBox_MainTool_Terminal_AutoResponse_Response.SelectedIndex == ControlEx.InvalidIndex)
			{
				string responseText = toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Text;
				int invalidTextStart;
				if (Validation.ValidateText(this, "automatic response", responseText, out invalidTextStart))
				{
					if (!this.isSettingControls)
					{
						this.mainToolValidationWorkaround_UpdateIsSuspended = true;
						((Terminal)ActiveMdiChild).RequestAutoResponseResponse(responseText);
						this.mainToolValidationWorkaround_UpdateIsSuspended = false;
					}
				}
				else
				{
					toolStripComboBox_MainTool_Terminal_AutoResponse_Response.Text.Remove(invalidTextStart);
				}
			}
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
			toolStripMenuItem_MainContextMenu_File_Recent.Enabled = (ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count > 0);
			this.isSettingControls.Leave();
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
			this.menuItems_recent = new List<ToolStripMenuItem>(Model.Settings.RecentFileSettings.MaxFilePaths); // Preset the required capactiy to improve memory management.
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

			// Hide all.
			for (int i = 0; i < Model.Settings.RecentFileSettings.MaxFilePaths; i++)
			{
				string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1);
				this.menuItems_recent[i].Text = "&" + prefix;
				this.menuItems_recent[i].Visible = false;
			}

			// Show valid.
			for (int i = 0; i < ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths.Count; i++)
			{
				string prefix = string.Format(CultureInfo.InvariantCulture, "{0}: ", i + 1);
				string file = PathEx.LimitPath(ApplicationSettings.LocalUserSettings.RecentFiles.FilePaths[i].Item, 60);
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
			toolStripStatusLabel_MainStatus_Chrono.Text = TimeSpanEx.FormatInvariantTimeSpan(e.TimeSpan, true, true, true);
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
			Preferences f = new Preferences(ApplicationSettings.LocalUserSettings);
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

		private void AttachLocalUserSettingsEventHandlers()
		{
			if (this.localUserSettingsRoot != null)
				this.localUserSettingsRoot.Changed += new EventHandler<SettingsEventArgs>(localUserSettingsRoot_Changed);
		}

		private void DetachLocalUserSettingsEventHandlers()
		{
			if (this.localUserSettingsRoot != null)
				this.localUserSettingsRoot.Changed -= new EventHandler<SettingsEventArgs>(localUserSettingsRoot_Changed);
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
				this.main.WorkspaceOpened += new EventHandler<Model.WorkspaceEventArgs>(main_WorkspaceOpened);
				this.main.WorkspaceClosed += new EventHandler(main_WorkspaceClosed);

				this.main.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(main_FixedStatusTextRequest);
				this.main.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(main_TimedStatusTextRequest);
				this.main.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(main_MessageInputRequest);

				this.main.Exited += new EventHandler<Model.ExitEventArgs>(main_Exited);
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

				this.main.Exited -= new EventHandler<Model.ExitEventArgs>(main_Exited);
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

		private void main_Exited(object sender, Model.ExitEventArgs e)
		{
			this.result = e.Result;

			// Prevent multiple calls to Close():
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

			NewTerminal f = new NewTerminal(newTerminalSettings);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUserSettings.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.Save();

				DocumentSettingsHandler<TerminalSettingsRoot> sh = new DocumentSettingsHandler<TerminalSettingsRoot>(f.TerminalSettingsResult);
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

		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenFileDialog()
		{
			SetFixedStatusText("Select a file...");

			OpenFileDialog ofd = new OpenFileDialog();
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
		[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "What's wrong with 'symmetricity'?")]
		[ModalBehavior(ModalBehavior.Always, Approval = "Always used to intentionally display a modal dialog.")]
		private void ShowOpenWorkspaceFromFileDialog()
		{
			SetFixedStatusText("Select a file...");

			OpenFileDialog ofd = new OpenFileDialog();
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

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save Workspace As";
			sfd.Filter      = ExtensionHelper.WorkspaceFilesFilter;
			sfd.FilterIndex = ExtensionHelper.WorkspaceFilesFilterDefault;
			sfd.DefaultExt  = PathEx.DenormalizeExtension(ExtensionHelper.WorkspaceFile);
			sfd.InitialDirectory = ApplicationSettings.LocalUserSettings.Paths.MainFiles;

			// Other than for terminals, workspace 'Save As' always suggests 'UserName.yaw':
			sfd.FileName = Environment.UserName + PathEx.NormalizeExtension(sfd.DefaultExt);
			// Note that 'DefaultExt' states "The returned string does not include the period."!

			DialogResult dr = sfd.ShowDialog(this);
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

				LayoutTerminals(layout);
			}
		}

		private void ResizeTerminals()
		{
			// Simply forward the resize request to the MDI layout engine:
			LayoutTerminals();
		}

		/// <summary>
		/// Performs the layout operation on the terminals.
		/// </summary>
		/// <remarks>
		/// Uses the MDI functionality Windows.Forms environment to perform the layout.
		/// </remarks>
		private void LayoutTerminals()
		{
			if (this.workspace != null)
				LayoutTerminals(this.workspace.SettingsRoot.Workspace.Layout);
		}

		/// <summary>
		/// Performs the layout operation on the terminals. This method does not notify the workspace.
		/// </summary>
		/// <remarks>
		/// Uses the MDI functionality Windows.Forms environment to perform the layout.
		/// </remarks>
		private void LayoutTerminals(WorkspaceLayout layout)
		{
			switch (layout)
			{
				case WorkspaceLayout.Automatic:
					int terminalCount = ((this.workspace != null) ? this.workspace.TerminalCount : 0);
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
					// Nothing to do. Manual layout is kept as is.
					break;

				case WorkspaceLayout.Minimize:
					MinimizeActiveMdiChild();
					break;

				case WorkspaceLayout.Maximize:
					MaximizeActiveMdiChild();
					break;

				default:
					throw (new NotSupportedException("Program execution should never get here, '" + layout + "' is an invalid workspace layout!"));
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
				this.workspace.TerminalAdded   += new EventHandler<Model.TerminalEventArgs>(workspace_TerminalAdded);
				this.workspace.TerminalRemoved += new EventHandler<Model.TerminalEventArgs>(workspace_TerminalRemoved);

				this.workspace.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(workspace_TimedStatusTextRequest);
				this.workspace.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(workspace_FixedStatusTextRequest);
				this.workspace.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(workspace_MessageInputRequest);

				this.workspace.SaveAsFileDialogRequest += new EventHandler<Model.DialogEventArgs>(workspace_SaveAsFileDialogRequest);
				
				this.workspace.Closed += new EventHandler<Model.ClosedEventArgs>(workspace_Closed);

				if (this.workspace.SettingsRoot != null)
					this.workspace.SettingsRoot.Changed += new EventHandler<SettingsEventArgs>(workspaceSettingsRoot_Changed);
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

				if (this.workspace.SettingsRoot != null)
					this.workspace.SettingsRoot.Changed -= new EventHandler<SettingsEventArgs>(workspaceSettingsRoot_Changed);
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
		private void workspace_TerminalAdded(object sender, Model.TerminalEventArgs e)
		{
			// Create terminal form and immediately show it:

			Terminal mdiChild = new Terminal(e.Terminal);
			AttachTerminalEventHandlersAndMdiChildToParent(mdiChild);

			this.isLayoutingMdi = true;
			mdiChild.Show(); // MDI children must be shown without reference to 'this'.
			this.isLayoutingMdi = false;

			LayoutTerminals();
			SetChildControls();
		}

		/// <remarks>
		/// Terminal is removed in <see cref="terminalMdiChild_FormClosed"/> event handler.
		/// </remarks>
		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_TerminalRemoved(object sender, Model.TerminalEventArgs e)
		{
			// Nothing to do, see remarks above.
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		[CallingContract(IsAlwaysMainThread = true, Rationale = "Synchronized from the underlying thread onto the main thread.")]
		private void workspace_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
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

						case FormWindowState.Maximized:
						default:
							SetTerminalLayout(WorkspaceLayout.Maximize);
							break;
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
				string childText = "[" + ((Terminal)ActiveMdiChild).Text + "]";
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
