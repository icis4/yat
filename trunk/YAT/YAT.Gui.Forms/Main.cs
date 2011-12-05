//==================================================================================================
// YAT - Yet Another Terminal.
// Visit YAT at http://sourceforge.net/projects/y-a-terminal.
// Contact YAT by mailto:y-a-terminal@users.sourceforge.net.
// ------------------------------------------------------------------------------------------------
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// YAT 2.0 Beta 4 Candidate 2 Development Version 1.99.29
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// See release notes for product version details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2011 Matthias Kläy.
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
using System.Windows.Forms;

using MKY;
using MKY.Diagnostics;
using MKY.Event;
using MKY.IO;
using MKY.Settings;
using MKY.Time;
using MKY.Windows.Forms;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;

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
			/// <summary>/// Normal operation of the form.</summary>
			None,

			/// <summary>/// Closing has been initiated by a form event.</summary>
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
		private ClosingState closingState = ClosingState.None;
		private Model.MainResult mainResult = Model.MainResult.Success;

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
			InitializeComponent();

			InitializeControls();

			// Link and attach to main model
			this.main = main;
			AttachMainEventHandlers();

			Text = this.main.UserName;

			// Link and attach to terminal settings.
			this.settingsRoot = ApplicationSettings.LocalUser;
			AttachSettingsEventHandlers();

			ApplyWindowSettingsAccordingToStartup();

			SetControls();
		}

		#endregion

		#region Properties
		//==========================================================================================
		// Properties
		//==========================================================================================

		private bool IsClosing
		{
			get { return (this.closingState != ClosingState.None); }
		}

		/// <summary></summary>
		public Model.MainResult MainResult
		{
			get { return (this.mainResult); }
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Rest is done here as soon as form is visible.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		private void Main_Paint(object sender, PaintEventArgs e)
		{
			if (this.isStartingUp)
			{
				this.isStartingUp = false;

				// Start YAT according to the main settings.
				this.mainResult = this.main.Start();

				if (this.mainResult != Model.MainResult.Success)
				{
					if (this.mainResult == Model.MainResult.CommandLineError)
					{
						MessageBox.Show
							(
							this,
							"YAT could not be started because the given command line is invalid." + Environment.NewLine +
							"Use 'YAT.exe /?' for command line help.",
							"Invalid Command Line",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning
							);
					}
					else
					{
						MessageBox.Show
							(
							this,
							"YAT could not be started!",
							"Application Start Error",
							MessageBoxButtons.OK,
							MessageBoxIcon.Error
							);
					}
					Close();
				}
				else
				{
					if (this.workspace.TerminalCount == 0)
					{
						// If workspace is empty, and requested, display new terminal dialog.
						if (this.main.StartArgs.ShowNewTerminalDialog)
							ShowNewTerminalDialog();
					}
					else
					{
						// If workspace contains terminals, and requested, tile the terminal forms accordingly.
						if      (this.main.StartArgs.TileHorizontal)
							LayoutMdi(MdiLayout.TileHorizontal);
						else if (this.main.StartArgs.TileVertical)
							LayoutMdi(MdiLayout.TileVertical);
					}

					// Automatically trigger transmit data if desired.
					if (this.main.StartArgs.PerformActionOnRequestedTerminal)
					{
						SetFixedStatusText("Triggering start action(s)...");
						timer_PerformStartAction.Start();
					}
				}
			}
		}

		private void Main_LocationChanged(object sender, EventArgs e)
		{
			if (!this.isStartingUp)
				SaveWindowSettings(true);
		}

		private void Main_SizeChanged(object sender, EventArgs e)
		{
			if (!this.isStartingUp)
				SaveWindowSettings();
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

				e.Cancel = (this.main.Exit() != Model.MainResult.Success);

				// Revert closing state in case of cancel.
				if (!e.Cancel)
					this.closingState = ClosingState.None;
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
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();

			bool recentsAreReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
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
			ShowOpenTerminalFromFileDialog();
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
			toolStripMenuItem_MainMenu_File_Workspace_New.Enabled = !workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_Close.Enabled = workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_Save.Enabled = workspaceIsReady;
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
			toolStripMenuItem_MainMenu_Window_Cascade.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileVertical.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_ArrangeIcons.Enabled = childIsReady;

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
			LayoutMdi(MdiLayout.Cascade);
		}

		private void toolStripMenuItem_MainMenu_Window_TileHorizontal_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileHorizontal);
		}

		private void toolStripMenuItem_MainMenu_Window_TileVertical_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.TileVertical);
		}

		private void toolStripMenuItem_MainMenu_Window_ArrangeIcons_Click(object sender, EventArgs e)
		{
			LayoutMdi(MdiLayout.ArrangeIcons);
		}

		#endregion

		#region Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > Help
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_MainMenu_Help_Contents_Click(object sender, EventArgs e)
		{
			Gui.Forms.Help f = new Gui.Forms.Help();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			Gui.Forms.ReleaseNotes f = new Gui.Forms.ReleaseNotes();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_RequestSupport_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Support);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_RequestFeature_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Feature);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_SubmitBug_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = ControlEx.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

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
				if (terminal != null)
				{
					radixIsReady = !(terminal.SettingsRoot.Display.SeparateTxRxRadix);
					if (radixIsReady)
						radix = terminal.SettingsRoot.Display.TxRadix;
				}
			}

			toolStripButton_MainTool_File_Save.Enabled      = childIsReady;
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
			ShowOpenTerminalFromFileDialog();
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
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();

			this.isSettingControls.Enter();

			bool recentsAreReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
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
			ShowOpenTerminalFromFileDialog();
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
				string prefix = string.Format(NumberFormatInfo.InvariantInfo, "{0}: ", i + 1);
				this.menuItems_recents[i].Text = "&" + prefix;
				this.menuItems_recents[i].Visible = false;
			}

			// Show valid.
			for (int i = 0; i < ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count; i++)
			{
				string prefix = string.Format(NumberFormatInfo.InvariantInfo, "{0}: ", i + 1);
				string file = PathEx.LimitPath(ApplicationSettings.LocalUser.RecentFiles.FilePaths[i].Item, 60);
				if (ApplicationSettings.LocalUser.RecentFiles.FilePaths[i] != null)
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
		/// a fix, it should be that way by default. However, due to some resaons, they somtimes
		/// appear somewhere at the top-left corner of the screen if this fix isn't done.
		/// </summary>
		/// <remarks>
		/// Is this a .NET bug?
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
			this.main.OpenRecent(int.Parse((string)(((ToolStripMenuItem)sender).Tag), NumberFormatInfo.InvariantInfo));
		}

		#endregion

		#region Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Status Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Status_Opening(object sender, CancelEventArgs e)
		{
			toolStripMenuItem_StatusContextMenu_ShowTerminalInfo.Checked = ApplicationSettings.LocalUser.MainWindow.ShowTerminalInfo;
			toolStripMenuItem_StatusContextMenu_ShowChrono.Checked       = ApplicationSettings.LocalUser.MainWindow.ShowChrono;
		}

		private void toolStripMenuItem_StatusContextMenu_ShowTerminalInfo_Click(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				ApplicationSettings.LocalUser.MainWindow.ShowTerminalInfo = !ApplicationSettings.LocalUser.MainWindow.ShowTerminalInfo;
				ApplicationSettings.Save();
			}
		}

		private void toolStripMenuItem_StatusContextMenu_ShowChrono_Click(object sender, EventArgs e)
		{
			if (!this.isSettingControls)
			{
				ApplicationSettings.LocalUser.MainWindow.ShowChrono = !ApplicationSettings.LocalUser.MainWindow.ShowChrono;
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
			toolStripStatusLabel_MainStatus_Chrono.Text = TimeSpanEx.FormatTimeSpan(e.TimeSpan, true);
		}

		#endregion

		#region Controls Event Handlers > PerformStartAction
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > PerformStartAction
		//------------------------------------------------------------------------------------------

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Intends to really catch all exceptions.")]
		private void timer_PerformStartAction_Tick(object sender, EventArgs e)
		{
			int id = this.main.StartArgs.RequestedDynamicTerminalIndex;
			if (this.workspace.GetTerminalByDynamicIndex(id) != null)
			{
				SetTimedStatusText("Trigger received, pending until terminal has been created...");
				return;
			}
			if (!this.workspace.GetTerminalByDynamicIndex(id).IsConnected)
			{
				SetTimedStatusText("Trigger received, pending until terminal has connected...");
				return;
			}

			// Preconditions fullfilled.
			timer_PerformStartAction.Stop();
			SetTimedStatusText("Trigger received, preparing transmit");

			// Automatically transmit data if desired.
			if (!string.IsNullOrEmpty(this.main.StartArgs.RequestedTransmitFilePath))
			{
				try
				{
					SetFixedStatusText("Automatically transmitting data on terminal " + id);

					string filePath = this.main.StartArgs.RequestedTransmitFilePath;
					this.workspace.Terminals[id].SendFile(new Model.Types.Command("", true, filePath));

					SetFixedStatusText("Automatically closing YAT");
					Close();
				}
				catch (Exception ex)
				{
					DebugEx.WriteException(this.GetType(), ex);
					this.mainResult = Model.MainResult.ApplicationRunError;
				}
			}
			else
			{
				this.mainResult = Model.MainResult.ApplicationRunError;
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
			if (ApplicationSettings.LocalUserSettingsSuccessfullyLoaded)
			{
				SuspendLayout();

				// Retrieve saved settings.
				FormWindowState savedWindowState = ApplicationSettings.LocalUser.MainWindow.WindowState;
				FormStartPosition savedStartPosition = ApplicationSettings.LocalUser.MainWindow.StartPosition;

				Point savedLocation = ApplicationSettings.LocalUser.MainWindow.Location;
				Size savedSize = ApplicationSettings.LocalUser.MainWindow.Size;
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
				ApplicationSettings.LocalUser.MainWindow.StartPosition = FormStartPosition.Manual;
				StartPosition = ApplicationSettings.LocalUser.MainWindow.StartPosition;
			}

			ApplicationSettings.LocalUser.MainWindow.WindowState = WindowState;

			if ((StartPosition == FormStartPosition.Manual) && (WindowState == FormWindowState.Normal))
				ApplicationSettings.LocalUser.MainWindow.Location = Location;

			if (WindowState == FormWindowState.Normal)
				ApplicationSettings.LocalUser.MainWindow.Size = Size;

			ApplicationSettings.Save();
		}

		#endregion

		#region Preferences
		//==========================================================================================
		// Preferences
		//==========================================================================================

		private void ShowPreferences()
		{
			Gui.Forms.Preferences f = new Gui.Forms.Preferences(ApplicationSettings.LocalUser);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.MainWindow = f.SettingsResult.MainWindow;
				ApplicationSettings.LocalUser.General    = f.SettingsResult.General;
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

		private void main_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			DialogResult dr;
			dr = MessageBox.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
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

		#endregion

		#region Workspace
		//==========================================================================================
		// Workspace
		//==========================================================================================

		#region Workspace > Methods
		//------------------------------------------------------------------------------------------
		// Workspace > Methods
		//------------------------------------------------------------------------------------------

		private void ShowOpenWorkspaceFromFileDialog()
		{
			SetFixedStatusText("Opening workspace...");

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open";
			ofd.Filter = ExtensionSettings.WorkspaceFilesFilter;
			ofd.DefaultExt = ExtensionSettings.WorkspaceFile;
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.main.OpenFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		private DialogResult ShowSaveWorkspaceAsFileDialog()
		{
			SetFixedStatusText("Saving workspace as...");

			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save Workspace As";
			sfd.Filter = ExtensionSettings.WorkspaceFilesFilter;
			sfd.DefaultExt = ExtensionSettings.WorkspaceFile;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath;
			sfd.FileName = Environment.UserName + "." + sfd.DefaultExt;

			DialogResult dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.Save();

				this.workspace.SaveAs(sfd.FileName);
			}
			else
			{
				ResetStatusText();
			}
			return (dr);
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

		private void workspace_TerminalAdded(object sender, Model.TerminalEventArgs e)
		{
			// Create terminal form
			Terminal mdiChild = new Terminal(e.Terminal);

			// Link MDI child this MDI parent
			mdiChild.MdiParent = this;

			mdiChild.Changed    += new EventHandler(mdiChild_Changed);
			mdiChild.Saved      += new EventHandler<Model.SavedEventArgs>(mdiChild_Saved);
			mdiChild.FormClosed += new FormClosedEventHandler(mdiChild_FormClosed);

			// Show form
			mdiChild.Show();

			SetChildControls();
		}

		/// <remarks>
		/// Terminal is removed by mdiChild_FormClose event handler.
		/// </remarks>
		private void workspace_TerminalRemoved(object sender, Model.TerminalEventArgs e)
		{
			SetChildControls();
		}

		private void workspace_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		private void workspace_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		private void workspace_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = MessageBox.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
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

		#region Terminal
		//==========================================================================================
		// Terminal
		//==========================================================================================

		private void ShowNewTerminalDialog()
		{
			SetFixedStatusText("New terminal...");

			Gui.Forms.NewTerminal f = new Gui.Forms.NewTerminal(ApplicationSettings.LocalUser.NewTerminal);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.Save();

				DocumentSettingsHandler<TerminalSettingsRoot> sh = new DocumentSettingsHandler<TerminalSettingsRoot>(f.TerminalSettingsResult);
				this.main.CreateNewTerminalFromSettings(sh);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void ShowOpenTerminalFromFileDialog()
		{
			SetFixedStatusText("Opening terminal...");

			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Title = "Open";
			ofd.Filter = ExtensionSettings.TerminalFilesFilter;
			ofd.DefaultExt = ExtensionSettings.TerminalFile;
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.TerminalFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.TerminalFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				this.main.OpenTerminalFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		#endregion

		#region MDI Parent
		//==========================================================================================
		// MDI Parent
		//==========================================================================================

		#region MDI Parent > Properties
		//------------------------------------------------------------------------------------------
		// MDI Parent > Properties
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

		#region MDI Parent > Methods
		//------------------------------------------------------------------------------------------
		// MDI Parent > Methods
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

			toolStripStatusLabel_MainStatus_TerminalInfo.Visible = ApplicationSettings.LocalUser.MainWindow.ShowTerminalInfo;
			toolStripStatusLabel_MainStatus_Chrono.Visible       = ApplicationSettings.LocalUser.MainWindow.ShowChrono;

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

		#endregion

		#endregion

		#region MDI Children
		//==========================================================================================
		// MDI Children
		//==========================================================================================

		#region MDI Children > Events
		//------------------------------------------------------------------------------------------
		// MDI Children > Events
		//------------------------------------------------------------------------------------------

		private void mdiChild_Changed(object sender, EventArgs e)
		{
			SetTimedStatus(Status.ChildChanged);
			SetChildControls();
		}

		private void mdiChild_Saved(object sender, Model.SavedEventArgs e)
		{
			if (!e.IsAutoSave)
				SetTimedStatus(Status.ChildSaved);

			SetChildControls();
		}

		private void mdiChild_FormClosed(object sender, FormClosedEventArgs e)
		{
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
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
