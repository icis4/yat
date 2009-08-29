//==================================================================================================
// $URL$
// $Author$
// $Date$
// $Revision$
// ------------------------------------------------------------------------------------------------
// See SVN change log for revision details.
// ------------------------------------------------------------------------------------------------
// Copyright © 2003-2004 HSR Hochschule für Technik Rapperswil.
// Copyright © 2003-2009 Matthias Kläy.
// All rights reserved.
// ------------------------------------------------------------------------------------------------
// YAT is licensed under the GNU LGPL.
// See http://www.gnu.org/licenses/lgpl.html for license details.
//==================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.IO;
using MKY.Utilities.Settings;
using MKY.Utilities.Windows.Forms;

using YAT.Settings;
using YAT.Settings.Application;
using YAT.Settings.Terminal;
using YAT.Settings.Workspace;

using YAT.Utilities;

namespace YAT.Gui.Forms
{
	/// <summary>
	/// Main form, provides setup dialogs and hosts terminal forms (MDI forms)
	/// </summary>
	public partial class Main : Form
	{
		#region Fields
		//==========================================================================================
		// Fields
		//==========================================================================================

		// startup
		private bool _isStartingUp = true;
		// not needed yet: private bool _isSettingControls = false;
		private bool _isClosingFromForm = false;
		private bool _isClosingFromModel = false;

		// model
		private Model.Main _main;
		private Model.Workspace _workspace;

		// status
		private const string _DefaultStatusText = "Ready";
		private const int _TimedStatusInterval = 2000;

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

		public Main()
		{
			InitializeComponent();
			Initialize(new Model.Main());
		}

		public Main(Model.Main main)
		{
			InitializeComponent();
			Initialize(main);
		}

		private void Initialize(Model.Main main)
		{
			InitializeControls();

			// link and attach to main model
			_main = main;
			AttachMainEventHandlers();

			Text = _main.UserName;

			ApplyWindowSettingsAccordingToStartup();

			SetControls();
		}

		#endregion

		#region Form Event Handlers
		//==========================================================================================
		// Form Event Handlers
		//==========================================================================================

		/// <summary>
		/// Rest is done here as soon as form is visible.
		/// </summary>
		private void Main_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				// start YAT according to main settings
				if (!_main.Start())
				{
					MessageBox.Show
						(
						this,
						"YAT could not be started!",
						"Serious Application Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
						);
					Close();
				}

				// if workspace is empty, display new terminal dialog
				if (_workspace.TerminalCount == 0)
					ShowNewTerminalDialog();
			}
		}

		private void Main_LocationChanged(object sender, EventArgs e)
		{
			if (!_isStartingUp)
				SaveWindowSettings(true);
		}

		private void Main_SizeChanged(object sender, EventArgs e)
		{
			if (!_isStartingUp)
				SaveWindowSettings();
		}

		private void Main_MdiChildActivate(object sender, EventArgs e)
		{
			if (ActiveMdiChild != null)
			{
				_workspace.ActivateTerminal(((Terminal)ActiveMdiChild).UnderlyingTerminal);
				SetTimedStatus(Status.ChildActivated);
			}
			SetChildControls();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			// prevent multiple calls to Close()
			if (!_isClosingFromModel)
			{
				_isClosingFromForm = true;
				e.Cancel = (!_main.Exit());
			}
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
			// not needed yet: _isSettingControls = true;

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_File_CloseAll.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_File_SaveAll.Enabled = childIsReady;

			// not needed yet: _isSettingControls = false;
		}

		/// <remarks>
		/// Must be called each time recent status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void toolStripMenuItem_MainMenu_File_SetRecentMenuItems()
		{
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();

			// not needed yet: _isSettingControls = true;

			bool recentsAreReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
			toolStripMenuItem_MainMenu_File_Recent.Enabled = recentsAreReady;

			// not needed yet: _isSettingControls = false;
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
			_workspace.CloseAllTerminals();
		}

		private void toolStripMenuItem_MainMenu_File_SaveAll_Click(object sender, EventArgs e)
		{
			_workspace.SaveAllTerminals();
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
			// not needed yet: _isSettingControls = true;

			bool workspaceIsReady = (_workspace != null);
			toolStripMenuItem_MainMenu_File_Workspace_New.Enabled = !workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_Close.Enabled = workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_Save.Enabled = workspaceIsReady;
			toolStripMenuItem_MainMenu_File_Workspace_SaveAs.Enabled = workspaceIsReady;

			// not needed yet: _isSettingControls = false;
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_DropDownOpening(object sender, EventArgs e)
		{
			toolStripMenuItem_MainMenu_File_Workspace_SetMenuItems();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_New_Click(object sender, EventArgs e)
		{
			_main.CreateNewWorkspace();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_Open_Click(object sender, EventArgs e)
		{
			ShowOpenWorkspaceFromFileDialog();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_Close_Click(object sender, EventArgs e)
		{
			_workspace.Close();
		}

		private void toolStripMenuItem_MainMenu_File_Workspace_Save_Click(object sender, EventArgs e)
		{
			_workspace.Save();
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
			// not needed yet: _isSettingControls = true;

			bool childIsReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_Window_Cascade.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_TileVertical.Enabled = childIsReady;
			toolStripMenuItem_MainMenu_Window_ArrangeIcons.Enabled = childIsReady;

			// not needed yet: _isSettingControls = false;

#if (FALSE)
			// \fixme
			// I don't know how to fix bug item #1808494 "MDI window list invisible if no MDI children".
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
			f.Location = XForm.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			Gui.Forms.ReleaseNotes f = new Gui.Forms.ReleaseNotes();
			f.StartPosition = FormStartPosition.Manual;
			f.Location = XForm.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_RequestSupport_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Support);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = XForm.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_RequestFeature_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Feature);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = XForm.CalculateManualCenterParentLocation(this, f);
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_SubmitBug_Click(object sender, EventArgs e)
		{
			Gui.Forms.TrackerInstructions f = new Gui.Forms.TrackerInstructions(Gui.Forms.TrackerInstructions.Tracker.Bug);
			f.StartPosition = FormStartPosition.Manual;
			f.Location = XForm.CalculateManualCenterParentLocation(this, f);
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
			// not needed yet: _isSettingControls = true;

			bool childIsReady = (ActiveMdiChild != null);

			bool terminalIsStarted = false;
			if (childIsReady)
				terminalIsStarted = ((Gui.Forms.Terminal)ActiveMdiChild).IsStarted;

			toolStripButton_MainTool_File_Save.Enabled                = childIsReady;
			toolStripButton_MainTool_Terminal_Start.Enabled           = childIsReady && !terminalIsStarted;
			toolStripButton_MainTool_Terminal_Stop.Enabled            = childIsReady && terminalIsStarted;
			toolStripButton_MainTool_Terminal_Clear.Enabled           = childIsReady;
			toolStripButton_MainTool_Terminal_SaveToFile.Enabled      = childIsReady;
			toolStripButton_MainTool_Terminal_CopyToClipboard.Enabled = childIsReady;
			toolStripButton_MainTool_Terminal_Print.Enabled           = childIsReady;
			toolStripButton_MainTool_Terminal_Settings.Enabled        = childIsReady;

			// not needed yet: _isSettingControls = false;
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

			// not needed yet: _isSettingControls = true;

			bool recentsAreReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
			toolStripMenuItem_MainContextMenu_File_Recent.Enabled = recentsAreReady;

			// not needed yet: _isSettingControls = false;
		}

		private void contextMenuStrip_Main_Opening(object sender, CancelEventArgs e)
		{
			// prevent context menu being displayed within the child window
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

		private List<ToolStripMenuItem> _menuItems_recents;

		private void contextMenuStrip_FileRecent_InitializeControls()
		{
			_menuItems_recents = new List<ToolStripMenuItem>(Model.Settings.RecentFileSettings.MaxFilePaths);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_1);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_2);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_3);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_4);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_5);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_6);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_7);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_8);
		}

		/// <remarks>
		/// Must be called each time recent status changes.
		/// Reason: Shortcuts associated to menu items are only active when items are visible and enabled.
		/// </remarks>
		private void contextMenuStrip_FileRecent_SetRecentMenuItems()
		{
			// not needed yet: _isSettingControls = true;

			// hide all
			for (int i = 0; i < Model.Settings.RecentFileSettings.MaxFilePaths; i++)
			{
				string prefix = string.Format("{0}: ", i + 1);
				_menuItems_recents[i].Text = "&" + prefix;
				_menuItems_recents[i].Visible = false;
			}

			// show valid
			for (int i = 0; i < ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count; i++)
			{
				string prefix = string.Format("{0}: ", i + 1);
				string file = XPath.LimitPath(ApplicationSettings.LocalUser.RecentFiles.FilePaths[i].Item, 60);
				if (ApplicationSettings.LocalUser.RecentFiles.FilePaths[i] != null)
				{
					_menuItems_recents[i].Text = "&" + prefix + file;
					_menuItems_recents[i].Enabled = true;
				}
				else
				{
					_menuItems_recents[i].Text = "&" + prefix;
					_menuItems_recents[i].Enabled = false;
				}
				_menuItems_recents[i].Visible = true;
			}

			// not needed yet: _isSettingControls = false;
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
			// no need to validate again, is already done on opening of parent menu
			// ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();

			contextMenuStrip_FileRecent_SetRecentMenuItems();
		}

		private void toolStripMenuItem_FileRecentContextMenu_Click(object sender, EventArgs e)
		{
			_main.OpenRecent(int.Parse((string)(((ToolStripMenuItem)sender).Tag)));
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

		private void chronometer_Main_TimeSpanChanged(object sender, EventArgs e)
		{
			toolStripStatusLabel_MainStatus_Chrono.Text = chronometer_Main.ToString();
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

				StartPosition = ApplicationSettings.LocalUser.MainWindow.StartPosition;
				WindowState = ApplicationSettings.LocalUser.MainWindow.WindowState;

				if ((StartPosition == FormStartPosition.Manual) && (WindowState == FormWindowState.Normal))
					Location = ApplicationSettings.LocalUser.MainWindow.Location;

				if (WindowState == FormWindowState.Normal)
					Size = ApplicationSettings.LocalUser.MainWindow.Size;

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
			Gui.Forms.Preferences f = new Gui.Forms.Preferences(ApplicationSettings.LocalUser.General);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.General = f.SettingsResult;
				ApplicationSettings.Save();
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
			_main.WorkspaceOpened += new EventHandler<Model.WorkspaceEventArgs>(_main_WorkspaceOpened);
			_main.WorkspaceClosed += new EventHandler(_main_WorkspaceClosed);

			_main.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(_main_FixedStatusTextRequest);
			_main.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(_main_TimedStatusTextRequest);
			_main.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(_main_MessageInputRequest);

			_main.Exited += new EventHandler(_main_Exited);
		}

		private void DetachMainEventHandlers()
		{
			_main.WorkspaceOpened -= new EventHandler<Model.WorkspaceEventArgs>(_main_WorkspaceOpened);
			_main.WorkspaceClosed -= new EventHandler(_main_WorkspaceClosed);

			_main.FixedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(_main_FixedStatusTextRequest);
			_main.TimedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(_main_TimedStatusTextRequest);
			_main.MessageInputRequest    -= new EventHandler<Model.MessageInputEventArgs>(_main_MessageInputRequest);

			_main.Exited -= new EventHandler(_main_Exited);
		}

		#endregion

		#region Main > Event Handlers
		//------------------------------------------------------------------------------------------
		// Main > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _main_WorkspaceOpened(object sender, Model.WorkspaceEventArgs e)
		{
			_workspace = e.Workspace;
			AttachWorkspaceEventHandlers();

			SetWorkspaceControls();
		}

		/// <remarks>
		/// Workspace::Closed event is handled at _workspace_Closed().
		/// </remarks>
		private void _main_WorkspaceClosed(object sender, EventArgs e)
		{
			SetWorkspaceControls();
		}

		private void _main_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		private void _main_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		private void _main_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			DialogResult dr;
			dr = MessageBox.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
			e.Result = dr;
		}

		private void _main_Exited(object sender, EventArgs e)
		{
			// prevent multiple calls to Close()
			if (!_isClosingFromForm)
			{
				_isClosingFromModel = true;
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
			ofd.DefaultExt = ExtensionSettings.WorkspaceFiles;
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName != ""))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				_main.OpenWorkspaceFromFile(ofd.FileName);
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
			sfd.DefaultExt = ExtensionSettings.WorkspaceFiles;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath;
			sfd.FileName = Environment.UserName + "." + sfd.DefaultExt;

			DialogResult dr = sfd.ShowDialog(this);
			if ((dr == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.Save();

				_workspace.SaveAs(sfd.FileName);
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
			_workspace.TerminalAdded   += new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalAdded);
			_workspace.TerminalRemoved += new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalRemoved);

			_workspace.TimedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(_workspace_TimedStatusTextRequest);
			_workspace.FixedStatusTextRequest += new EventHandler<Model.StatusTextEventArgs>(_workspace_FixedStatusTextRequest);
			_workspace.MessageInputRequest    += new EventHandler<Model.MessageInputEventArgs>(_workspace_MessageInputRequest);

			_workspace.SaveAsFileDialogRequest += new EventHandler<Model.DialogEventArgs>(_workspace_SaveAsFileDialogRequest);
			
			_workspace.Closed += new EventHandler<Model.ClosedEventArgs>(_workspace_Closed);
		}

		private void DetachWorkspaceEventHandlers()
		{
			_workspace.TerminalAdded   -= new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalAdded);
			_workspace.TerminalRemoved -= new EventHandler<Model.TerminalEventArgs>(_workspace_TerminalRemoved);

			_workspace.TimedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(_workspace_TimedStatusTextRequest);
			_workspace.FixedStatusTextRequest -= new EventHandler<Model.StatusTextEventArgs>(_workspace_FixedStatusTextRequest);
			_workspace.MessageInputRequest    -= new EventHandler<Model.MessageInputEventArgs>(_workspace_MessageInputRequest);

			_workspace.SaveAsFileDialogRequest -= new EventHandler<Model.DialogEventArgs>(_workspace_SaveAsFileDialogRequest);

			_workspace.Closed -= new EventHandler<Model.ClosedEventArgs>(_workspace_Closed);
		}

		#endregion

		#region Workspace > Event Handlers
		//------------------------------------------------------------------------------------------
		// Workspace > Event Handlers
		//------------------------------------------------------------------------------------------

		private void _workspace_TerminalAdded(object sender, Model.TerminalEventArgs e)
		{
			// create terminal form
			Terminal mdiChild = new Terminal(e.Terminal);

			// link MDI child this MDI parent
			mdiChild.MdiParent = this;

			mdiChild.Changed    += new EventHandler(mdiChild_Changed);
			mdiChild.Saved      += new EventHandler<Model.SavedEventArgs>(mdiChild_Saved);
			mdiChild.FormClosed += new FormClosedEventHandler(mdiChild_FormClosed);

			// show form
			mdiChild.Show();

			SetChildControls();
		}

		/// <remarks>
		/// Terminal is removed by mdiChild_FormClose event handler.
		/// </remarks>
		private void _workspace_TerminalRemoved(object sender, Model.TerminalEventArgs e)
		{
			SetChildControls();
		}

		private void _workspace_TimedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetTimedStatusText(e.Text);
		}

		private void _workspace_FixedStatusTextRequest(object sender, Model.StatusTextEventArgs e)
		{
			SetFixedStatusText(e.Text);
		}

		private void _workspace_MessageInputRequest(object sender, Model.MessageInputEventArgs e)
		{
			e.Result = MessageBox.Show(this, e.Text, e.Caption, e.Buttons, e.Icon, e.DefaultButton);
		}

		private void _workspace_SaveAsFileDialogRequest(object sender, Model.DialogEventArgs e)
		{
			e.Result = ShowSaveWorkspaceAsFileDialog();
		}

		private void _workspace_Closed(object sender, Model.ClosedEventArgs e)
		{
			DetachWorkspaceEventHandlers();
			_workspace = null;

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

				// check whether workspace is ready, otherwise empty workspace needs to be creaeted first
				if (_workspace != null)
					_workspace.CreateNewTerminal(sh);
				else
					_main.CreateNewWorkspaceAndTerminal(sh);
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
			ofd.DefaultExt = ExtensionSettings.TerminalFiles;
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Paths.TerminalFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName != ""))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.TerminalFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
				ApplicationSettings.Save();

				// check whether workspace is ready, otherwise empty workspace needs to be creaeted first
				if (_workspace != null)
					_workspace.OpenTerminalFromFile(ofd.FileName);
				else
					_main.OpenFromFile(ofd.FileName);
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
			SetChildControls();
			SetRecentControls();
			SetWorkspaceControls();
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
					//case Status.ChildActive:  return (childText + " active");
					case Status.ChildActive:    return (""); // display nothing to keep information lower
					case Status.ChildChanged:   return (childText + " changed");
					case Status.ChildSaved:     return (childText + " saved");
					case Status.ChildClosed:    return (childText + " closed");
				}
			}
			return (_DefaultStatusText);
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
			timer_Status.Interval = _TimedStatusInterval;
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

		#endregion
	}
}

//==================================================================================================
// End of
// $URL$
//==================================================================================================
