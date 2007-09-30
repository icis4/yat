using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MKY.Utilities.Settings;
using MKY.YAT.Settings;
using MKY.YAT.Settings.Application;
using MKY.YAT.Settings.Terminal;
using MKY.YAT.Settings.Workspace;
using MKY.YAT.Gui.Settings;

namespace MKY.YAT.Gui.Forms
{
	/// <summary>
	/// Main form, provides setup dialogs and hosts terminal forms (MDI forms)
	/// </summary>
	public partial class Main : Form
	{
		//------------------------------------------------------------------------------------------
		// Fields
		//------------------------------------------------------------------------------------------

		// startup
		private bool _isStartingUp = true;

		// args
		private string[] _commandLineArgs;

		// MDI
		private const string _TerminalText = "Terminal";
		private int _terminalIdCounter = 0;
		private const int _PathLength = 80;

		// recent files
		private List<ToolStripMenuItem> _menuItems_recents;

		// workspace settings
		private Guid _guid = Guid.NewGuid();
		private DocumentSettingsHandler<WorkspaceSettingsRoot> _workspaceSettingsHandler;
		private WorkspaceSettingsRoot _workspaceSettingsRoot;
		private bool _handlingWorkspaceSettingsIsSuspended = false;

		// status
		private const string _DefaultStatusText = "Ready";
		private const int _TimedStatusInterval = 2000;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

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

		public Main(string[] args, bool applicationSettingsLoaded)
		{
			_commandLineArgs = args;

			InitializeComponent();
			Initialize(applicationSettingsLoaded);
		}

		private void Initialize(bool applicationSettingsLoaded)
		{
			InitializeRecents();

			_workspaceSettingsHandler = new DocumentSettingsHandler<WorkspaceSettingsRoot>();
			_workspaceSettingsHandler.SettingsFilePath = ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath;
			_workspaceSettingsRoot = _workspaceSettingsHandler.Settings;
			AttachWorkspaceSettingsHandlers();

			// form title
			string text = Application.ProductName;
			text += VersionInfo.ProductNamePostFix;
			Text = text;

			if (applicationSettingsLoaded)
			{
				StartPosition = FormStartPosition.Manual;
				ApplyWindowSettings();
			}
			SetToolControls();
		}

		#region Properties
		//******************************************************************************************
		// Properties
		//******************************************************************************************

		public Guid Guid
		{
			get { return (_guid); }
		}

		#endregion

		#region Form Event Handlers
		//******************************************************************************************
		// Form Event Handlers
		//******************************************************************************************

		/// <summary>
		/// Rest is done here as soon as form is visible.
		/// </summary>
		private void Main_Paint(object sender, PaintEventArgs e)
		{
			if (_isStartingUp)
			{
				_isStartingUp = false;

				bool success = false;

				if ((_commandLineArgs != null) && (_commandLineArgs.Length >= 1))
				{
					success = OpenFromFile(_commandLineArgs[0]);
				}

				if (!success && ApplicationSettings.LocalUser.General.AutoOpenWorkspace)
				{
					if (File.Exists(ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath))
						success = OpenWorkspaceFromFile(ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath);
				}

				if (!success)
				{
					ShowNewTerminalDialog();
				}
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
			SetTimedStatus(Status.ChildActivated);
			SetToolControls();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
			{
				SaveWorkspaceFile(true);
			}
			else
			{
				if (_workspaceSettingsRoot.HaveChanged)
				{
					DialogResult dr = MessageBox.Show
						(
						this,
						"Save workspace?",
						Text,
						MessageBoxButtons.YesNoCancel,
						MessageBoxIcon.Question
						);

					switch (dr)
					{
						case DialogResult.Yes: SaveWorkspaceFile(); break;
						case DialogResult.No:                       break;
						default:               e.Cancel = true;     return;
					}
				}
			}

			if (!e.Cancel)
			{
				SetFixedStatusText("Exiting YAT...");
				SuspendHandlingWorkspaceSettings();
			}
			else
			{
				ResetStatusText();
			}
		}

		#endregion

		#region Controls Event Handlers
		//******************************************************************************************
		// Controls Event Handlers
		//******************************************************************************************

		#region Controls Event Handlers > Main Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu
		//------------------------------------------------------------------------------------------

		#region Controls Event Handlers > Main Menu > File
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Menu > File
		//------------------------------------------------------------------------------------------

		private void toolStripMenuItem_MainMenu_File_DropDownOpening(object sender, EventArgs e)
		{
			bool childReady = (ActiveMdiChild != null);
			bool workspaceReady = _workspaceSettingsHandler.SettingsFileExists;
			toolStripMenuItem_MainMenu_File_CloseAll.Enabled = childReady;
			toolStripMenuItem_MainMenu_File_SaveAll.Enabled = (childReady && workspaceReady);
			
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();
			bool recentsReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
			toolStripMenuItem_MainMenu_File_Recent.Enabled = recentsReady;
		}

		private void toolStripMenuItem_MainMenu_File_New_Click(object sender, EventArgs e)
		{
			ShowNewTerminalDialog();
		}

		private void toolStripMenuItem_MainMenu_File_Open_Click(object sender, EventArgs e)
		{
			ShowOpenTerminalDialog();
		}

		private void toolStripMenuItem_MainMenu_File_CloseAll_Click(object sender, EventArgs e)
		{
			CloseAllTerminals();
		}

		private void toolStripMenuItem_MainMenu_File_OpenWorkspace_Click(object sender, EventArgs e)
		{
			ShowOpenWorkspaceFileDialog();
		}

		private void toolStripMenuItem_MainMenu_File_SaveWorkspace_Click(object sender, EventArgs e)
		{
			SaveWorkspaceFile();
		}

		private void toolStripMenuItem_MainMenu_File_SaveWorkspaceAs_Click(object sender, EventArgs e)
		{
			ShowSaveWorkspaceFileAsDialog();
		}

		private void toolStripMenuItem_MainMenu_File_SaveAll_Click(object sender, EventArgs e)
		{
			SaveAll();
		}

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

		private void toolStripMenuItem_MainMenu_Window_DropDownOpening(object sender, EventArgs e)
		{
			bool childReady = (ActiveMdiChild != null);
			toolStripMenuItem_MainMenu_Window_Cascade.Enabled = childReady;
			toolStripMenuItem_MainMenu_Window_TileHorizontal.Enabled = childReady;
			toolStripMenuItem_MainMenu_Window_TileVertical.Enabled = childReady;
			toolStripMenuItem_MainMenu_Window_ArrangeIcons.Enabled = childReady;
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

		private void toolStripMenuItem_MainMenu_Help_DropDownOpening(object sender, EventArgs e)
		{
			// nothing to do
		}

		private void toolStripMenuItem_MainMenu_Help_Contents_Click(object sender, EventArgs e)
		{
			Gui.Forms.Help f = new Gui.Forms.Help();
			f.Show(this);
		}

		private void toolStripMenuItem_MainMenu_Help_ReleaseNotes_Click(object sender, EventArgs e)
		{
			Gui.Forms.ReleaseNotes f = new Gui.Forms.ReleaseNotes();
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

		private void toolStripButton_MainTool_File_New_Click(object sender, EventArgs e)
		{
			ShowNewTerminalDialog();
		}

		private void toolStripButton_MainTool_File_Open_Click(object sender, EventArgs e)
		{
			ShowOpenTerminalDialog();
		}

		private void toolStripButton_MainTool_File_Save_Click(object sender, EventArgs e)
		{
			SaveTerminal();
		}

		private void toolStripButton_MainTool_Terminal_Open_Click(object sender, EventArgs e)
		{
			OpenTerminalIO();
		}

		private void toolStripButton_MainTool_Terminal_Close_Click(object sender, EventArgs e)
		{
			CloseTerminalIO();
		}

		private void toolStripButton_MainTool_Terminal_Settings_Click(object sender, EventArgs e)
		{
			EditTerminalSettings();
		}

		#endregion

		#region Controls Event Handlers > Main Context Menu
		//------------------------------------------------------------------------------------------
		// Controls Event Handlers > Main Context Menu
		//------------------------------------------------------------------------------------------

		private void contextMenuStrip_Main_Opening(object sender, CancelEventArgs e)
		{
			// prevent context menu being displayed within the child window
			if (ActiveMdiChild != null)
			{
				e.Cancel = true;
				return;
			}

			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ValidateAll();
			bool recentsReady = (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count > 0);
			toolStripMenuItem_MainContextMenu_File_Recent.Enabled = recentsReady;
		}

		private void toolStripMenuItem_MainContextMenu_File_New_Click(object sender, EventArgs e)
		{
			ShowNewTerminalDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_Open_Click(object sender, EventArgs e)
		{
			ShowOpenTerminalDialog();
		}

		private void toolStripMenuItem_MainContextMenu_File_OpenWorkspace_Click(object sender, EventArgs e)
		{
			ShowOpenWorkspaceFileDialog();
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

			SetRecents();
		}

		private void toolStripMenuItem_FileRecentContextMenu_Click(object sender, EventArgs e)
		{
			OpenRecent(int.Parse((string)(((ToolStripMenuItem)sender).Tag)));
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
			chronometer_Main.Reset();
		}

		#endregion

		#endregion

		#region Workspace Settings
		//******************************************************************************************
		// Workspace Settings
		//******************************************************************************************

		private void AttachWorkspaceSettingsHandlers()
		{
			_workspaceSettingsRoot.ClearChanged();
			_workspaceSettingsRoot.Changed += new EventHandler<SettingsEventArgs>(_workspaceSettings_Changed);
		}

		//------------------------------------------------------------------------------------------
		// Workspace Settings Events
		//------------------------------------------------------------------------------------------

		private void _workspaceSettings_Changed(object sender, SettingsEventArgs e)
		{
			if (_handlingWorkspaceSettingsIsSuspended)
				return;

			if (ApplicationSettings.LocalUser.General.AutoSaveWorkspace)
				DoSaveWorkspaceFile(true);
		}

		private void SuspendHandlingWorkspaceSettings()
		{
			_handlingWorkspaceSettingsIsSuspended = true;
		}

		private void ResumeHandlingWorkspaceSettings()
		{
			_handlingWorkspaceSettingsIsSuspended = false;
		}

		#endregion

		#region Menu
		//******************************************************************************************
		// Menu
		//******************************************************************************************

		private void InitializeRecents()
		{
			_menuItems_recents = new List<ToolStripMenuItem>(Settings.RecentFileSettings.MaximumFilePaths);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_1);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_2);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_3);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_4);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_5);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_6);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_7);
			_menuItems_recents.Add(toolStripMenuItem_FileRecentContextMenu_8);
		}

		/// <summary>
		/// Update recent entry.
		/// </summary>
		/// <param name="recentFile">Recent file.</param>
		private void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
			ApplicationSettings.SaveLocalUser();

			SetRecents();
		}

		/// <summary>
		/// Updates the main menu File > Recent > Recents...
		/// </summary>
		private void SetRecents()
		{
			if (ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count == 0)
			{
				toolStripMenuItem_MainMenu_File_Recent.Enabled = false;
				toolStripMenuItem_MainMenu_File_Recent.Enabled = false;
				return;
			}
			toolStripMenuItem_MainMenu_File_Recent.Enabled = true;
			toolStripMenuItem_MainMenu_File_Recent.Enabled = true;

			// hide all
			for (int i = 0; i < Settings.RecentFileSettings.MaximumFilePaths; i++)
			{
				string prefix = string.Format("{0}: ", i + 1);
				_menuItems_recents[i].Text = "&" + prefix;
				_menuItems_recents[i].Visible = false;
			}

			// show valid
			for (int i = 0; i < ApplicationSettings.LocalUser.RecentFiles.FilePaths.Count; i++)
			{
				string prefix = string.Format("{0}: ", i + 1);
				string file = Utilities.IO.XPath.LimitPath(ApplicationSettings.LocalUser.RecentFiles.FilePaths[i].Item, 60);
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
		}

		#endregion

		#region Tool
		//******************************************************************************************
		// Tool
		//******************************************************************************************

		private void SetToolControls()
		{
			bool childReady = (ActiveMdiChild != null);

			bool terminalOpen = false;
			if (childReady)
				terminalOpen = ((Gui.Forms.Terminal)ActiveMdiChild).IsOpen;

			toolStripButton_MainTool_File_Save.Enabled = childReady;
			toolStripButton_MainTool_Terminal_Open.Enabled = childReady && !terminalOpen;
			toolStripButton_MainTool_Terminal_Close.Enabled = childReady && terminalOpen;
			toolStripButton_MainTool_Terminal_Settings.Enabled = childReady;
		}

		#endregion

		#region MDI Parent
		//******************************************************************************************
		// MDI Parent
		//******************************************************************************************

		private void ApplyWindowSettings()
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

			ApplicationSettings.SaveLocalUser();
		}

		private bool OpenFromFile(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			string extension = Path.GetExtension(filePath);
			if      (ExtensionSettings.IsWorkspaceFile(extension))
			{
				SetFixedStatusText("Opening workspace " + fileName + "...");
				return (OpenWorkspaceFromFile(filePath));
			}
			else if (ExtensionSettings.IsTerminalFile(extension))
			{
				SetFixedStatusText("Opening terminal " + fileName + "...");
				return (OpenTerminalFromFile(filePath));
			}
			else
			{
				SetFixedStatusText("Unknown file type!");

				MessageBox.Show
					(
					this,
					"File" + Environment.NewLine + filePath + Environment.NewLine +
					"has unknown type!",
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
					);

				SetTimedStatusText("No file opened!");
				return (false);
			}
		}

		private void SaveAll()
		{
			SaveAllTerminals();
			SaveWorkspaceFile();
		}

		#endregion

		#region MDI Workspace
		//******************************************************************************************
		// MDI Workspace
		//******************************************************************************************

		private void ShowOpenWorkspaceFileDialog()
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
				ApplicationSettings.SaveLocalUser();

				OpenWorkspaceFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		private bool OpenWorkspaceFromFile(string filePath)
		{
			// close all terminal 
			CloseAllTerminals();
			if (MdiChildren.Length > 0)
				return (false);

			SetFixedStatusText("Opening workspace...");
			try
			{
				_workspaceSettingsHandler.SettingsFilePath = filePath;
				_workspaceSettingsHandler.Load();
				_workspaceSettingsRoot = _workspaceSettingsHandler.Settings;

				ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath = filePath;
				ApplicationSettings.SaveLocalUser();

				if (!_workspaceSettingsRoot.AutoSaved)
					SetRecent(filePath);

				SetTimedStatusText("Workspace opened");
			}
			catch (System.Xml.XmlException ex)
			{
				SetFixedStatusText("Error opening workspace!");

				MessageBox.Show
					(
					this,
					"Unable to open file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
					"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
					"File error message: " + ex.InnerException.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
					);

				SetTimedStatusText("No workspace opened!");
				return (false);
			}

			int terminalCount = _workspaceSettingsRoot.TerminalSettings.Count;
			if (terminalCount == 1)
				SetFixedStatusText("Opening workspace terminal...");
			else if (terminalCount > 1)
				SetFixedStatusText("Opening workspace terminals...");

			TerminalSettingsItemCollection clone = new TerminalSettingsItemCollection(_workspaceSettingsRoot.TerminalSettings);
			foreach (TerminalSettingsItem item in clone)
			{
				try
				{
					OpenTerminalFromFile(item.FilePath, item.Guid, item.Window, true);
				}
				catch (System.Xml.XmlException ex)
				{
					SetFixedStatusText("Error opening terminal!");

					DialogResult result = MessageBox.Show
						(
						this,
						"Unable to open file" + Environment.NewLine + filePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message + Environment.NewLine + Environment.NewLine +
						"Continue loading workspace?",
						"File Error",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.Exclamation
						);

					SetTimedStatusText("Terminal not opened!");

					if (result == DialogResult.No)
						break;
				}
			}

			// attach workspace settings after terminals have been opened
			AttachWorkspaceSettingsHandlers();

			if (terminalCount == 1)
				SetTimedStatusText("Workspace terminal opened");
			else if (terminalCount > 1)
				SetTimedStatusText("Workspace terminals opened");
			else
				SetTimedStatusText("Workspace contains no terminal to open");

			return (true);
		}

		private void SaveWorkspaceFile()
		{
			SaveWorkspaceFile(false);
		}

		private void SaveWorkspaceFile(bool autoSave)
		{
			if (autoSave)
			{
				DoSaveWorkspaceFile(true);
			}
			else
			{
				if (_workspaceSettingsHandler.SettingsFilePathIsValid && !_workspaceSettingsHandler.Settings.AutoSaved)
					DoSaveWorkspaceFile(false);
				else
					ShowSaveWorkspaceFileAsDialog();
			}
		}

		private void ShowSaveWorkspaceFileAsDialog()
		{
			SetFixedStatusText("Saving workspace as...");
			SaveFileDialog sfd = new SaveFileDialog();
			sfd.Title = "Save Workspace As";
			sfd.Filter = ExtensionSettings.WorkspaceFilesFilter;
			sfd.DefaultExt = ExtensionSettings.WorkspaceFiles;
			sfd.InitialDirectory = ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath;
			sfd.FileName = Environment.UserName + "." + sfd.DefaultExt;
			if ((sfd.ShowDialog(this) == DialogResult.OK) && (sfd.FileName.Length > 0))
			{
				Refresh();

				ApplicationSettings.LocalUser.Paths.WorkspaceFilesPath = System.IO.Path.GetDirectoryName(sfd.FileName);
				ApplicationSettings.SaveLocalUser();

				string autoSaveFilePathToDelete = "";
				if (_workspaceSettingsRoot.AutoSaved)
					autoSaveFilePathToDelete = _workspaceSettingsHandler.SettingsFilePath;

				_workspaceSettingsHandler.SettingsFilePath = sfd.FileName;
				DoSaveWorkspaceFile(false, autoSaveFilePathToDelete);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void DoSaveWorkspaceFile(bool autoSave)
		{
			DoSaveWorkspaceFile(autoSave, "");
		}

		private void DoSaveWorkspaceFile(bool autoSave, string autoSaveFilePathToDelete)
		{
			if (!autoSave)
				SetFixedStatusText("Saving workspace...");

			try
			{
				_workspaceSettingsRoot.SuspendChangeEvent();

				// set workspace file path before terminals are saved in order
				// to correctly retrieve relative paths
				if (autoSave)
				{
					string autoSaveFilePath = GeneralSettings.AutoSaveRoot + Path.DirectorySeparatorChar + GeneralSettings.AutoSaveWorkspaceFileNamePrefix + Guid.ToString() + ExtensionSettings.WorkspaceFiles;
					if (!_workspaceSettingsHandler.SettingsFilePathIsValid)
						_workspaceSettingsHandler.SettingsFilePath = autoSaveFilePath;
				}

				// save all contained terminals
				foreach (Form f in MdiChildren)
				{
					((Gui.Forms.Terminal)f).RequestAutoSaveFile();
				}

				// update workspace
				foreach (Form f in MdiChildren)
				{
					AddToOrReplaceInWorkspace((Gui.Forms.Terminal)f);
				}

				// save workspace
				_workspaceSettingsHandler.Settings.AutoSaved = autoSave;
				_workspaceSettingsHandler.Save();

				_workspaceSettingsRoot.ClearChanged();
				_workspaceSettingsRoot.ResumeChangeEvent();

				ApplicationSettings.LocalUser.General.CurrentWorkspaceFilePath = _workspaceSettingsHandler.SettingsFilePath;
				ApplicationSettings.SaveLocalUser();

				if (!autoSave)
				{
					SetRecent(_workspaceSettingsHandler.SettingsFilePath);
					SetTimedStatusText("Workspace saved");
				}

				// try to delete existing auto save file
				try
				{
					if (File.Exists(autoSaveFilePathToDelete))
						File.Delete(autoSaveFilePathToDelete);
				}
				catch (Exception)
				{
				}
			}
			catch (System.Xml.XmlException ex)
			{
				if (!autoSave)
				{
					SetFixedStatusText("Error saving workspace!");
					MessageBox.Show
						(
						this,
						"Unable to save file" + Environment.NewLine + _workspaceSettingsHandler.SettingsFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning
						);
					SetTimedStatusText("Workspace not saved!");
				}
			}
		}

		private void ShowPreferences()
		{
			Gui.Forms.Preferences f = new Gui.Forms.Preferences(ApplicationSettings.LocalUser.General);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.General = f.SettingsResult;
				ApplicationSettings.SaveLocalUser();
			}
		}

		private void OpenRecent(int userIndex)
		{
			OpenFromFile(ApplicationSettings.LocalUser.RecentFiles.FilePaths[userIndex - 1].Item);
		}

		private void AddToOrReplaceInWorkspace(Terminal terminal)
		{
			WindowSettings ws = new WindowSettings();
			ws.State = terminal.WindowState;
			ws.Location = terminal.Location;
			ws.Size = terminal.Size;

			TerminalSettingsItem tsi = new TerminalSettingsItem();
			tsi.Guid = terminal.Guid;
			string filePath = terminal.SettingsFilePath;
			if (ApplicationSettings.LocalUser.General.UseRelativePaths)
			{
				Utilities.IO.XPathCompareResult pcr = Utilities.IO.XPath.CompareFilePaths(_workspaceSettingsHandler.SettingsFilePath, terminal.SettingsFilePath);
				if (pcr.AreRelative)
					filePath = pcr.RelativePath;
			}
			tsi.FilePath = filePath;
			tsi.Window = ws;

			_workspaceSettingsRoot.TerminalSettings.AddOrReplaceGuid(tsi);
			_workspaceSettingsRoot.SetChanged();
		}

		private void RemoveFromWorkspace(Terminal terminal)
		{
			_workspaceSettingsRoot.TerminalSettings.RemoveGuid(terminal.Guid);
			_workspaceSettingsRoot.SetChanged();
		}

		#endregion

		#region MDI Children
		//******************************************************************************************
		// MDI Children
		//******************************************************************************************

		private int GetNextTerminalId()
		{
			_terminalIdCounter++;
			return (_terminalIdCounter);
		}

		private void ShowNewTerminalDialog()
		{
			SetFixedStatusText("New terminal...");
			Gui.Forms.NewTerminal f = new Gui.Forms.NewTerminal(ApplicationSettings.LocalUser.NewTerminal);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.NewTerminal = f.NewTerminalSettingsResult;
				ApplicationSettings.SaveLocalUser();

				SetFixedStatusText("Creating new terminal...");

				DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> sh = new DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot>(f.TerminalSettingsResult);
				Gui.Forms.Terminal terminal = new Gui.Forms.Terminal(sh);

				terminal.UserName = _TerminalText + GetNextTerminalId();
				terminal.MdiParent = this;
				terminal.TerminalChanged += new EventHandler(mdi_child_TerminalChanged);
				terminal.TerminalSaved += new EventHandler<TerminalSavedEventArgs>(mdi_child_TerminalSaved);
				terminal.FormClosed += new FormClosedEventHandler(mdi_child_FormClosed);
				terminal.Show();

				AddToOrReplaceInWorkspace(terminal);

				SetTimedStatusText("New terminal created");
			}
			else
			{
				ResetStatusText();
			}
		}

		private void ShowOpenTerminalDialog()
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
				ApplicationSettings.SaveLocalUser();

				OpenTerminalFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		private bool OpenTerminalFromFile(string filePath)
		{
			return (OpenTerminalFromFile(filePath, Guid.Empty, null, false));
		}

		private bool OpenTerminalFromFile(string filePath, Guid guid, WindowSettings windowSettings, bool suppressErrorHandling)
		{
			string absoluteFilePath = filePath;

			SetFixedStatusText("Opening terminal...");
			try
			{
				DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot> sh = new DocumentSettingsHandler<YAT.Settings.Terminal.TerminalSettingsRoot>();

				// combine absolute workspace path with terminal path if that one is relative
				absoluteFilePath = Utilities.IO.XPath.CombineFilePaths(_workspaceSettingsHandler.SettingsFilePath, filePath);
				sh.SettingsFilePath = absoluteFilePath;
				sh.Load();

				// replace window settings with those saved in workspace
				if (windowSettings != null)
					sh.Settings.Window = windowSettings;

				// create terminal
				Gui.Forms.Terminal terminal = new Gui.Forms.Terminal(sh);

				if (guid != Guid.Empty)
					terminal.Guid = guid;

				if (sh.Settings.AutoSaved)
					terminal.UserName = _TerminalText + GetNextTerminalId();
				else
					terminal.UserNameFromFile = absoluteFilePath;

				terminal.MdiParent = this;
				terminal.TerminalChanged += new EventHandler(mdi_child_TerminalChanged);
				terminal.TerminalSaved += new EventHandler<TerminalSavedEventArgs>(mdi_child_TerminalSaved);
				terminal.FormClosed += new FormClosedEventHandler(mdi_child_FormClosed);
				terminal.Show();

				AddToOrReplaceInWorkspace(terminal);
				if (!sh.Settings.AutoSaved)
					SetRecent(filePath);

				SetTimedStatusText("Terminal opened");
				return (true);
			}
			catch (System.Xml.XmlException ex)
			{
				if (!suppressErrorHandling)
				{
					SetFixedStatusText("Error opening terminal!");

					MessageBox.Show
						(
						this,
						"Unable to open file" + Environment.NewLine + absoluteFilePath + Environment.NewLine + Environment.NewLine +
						"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
						"File error message: " + ex.InnerException.Message,
						"File Error",
						MessageBoxButtons.OK,
						MessageBoxIcon.Stop
						);

					SetTimedStatusText("Terminal not opened!");
					return (false);
				}
				else
				{
					throw (ex);
				}
			}
		}

		private void CloseTerminal()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestCloseFile();
		}

		private void CloseAllTerminals()
		{
			foreach (Form f in MdiChildren)
				((Gui.Forms.Terminal)f).RequestCloseFile();
		}

		private void SaveTerminal()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestSaveFile();
		}

		private void SaveAllTerminals()
		{
			foreach (Form f in MdiChildren)
				((Gui.Forms.Terminal)f).RequestSaveFile();
		}

		private void OpenTerminalIO()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestOpenTerminal();
		}

		private void CloseTerminalIO()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestCloseTerminal();
		}

		private void EditTerminalSettings()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestEditTerminalSettings();
		}

		//------------------------------------------------------------------------------------------
		// MDI Children > Events
		//------------------------------------------------------------------------------------------

		private void mdi_child_TerminalChanged(object sender, EventArgs e)
		{
			SetTimedStatus(Status.ChildChanged);
			SetToolControls();
		}

		private void mdi_child_TerminalSaved(object sender, TerminalSavedEventArgs e)
		{
			AddToOrReplaceInWorkspace((Terminal)sender);
			if (!e.AutoSave)
			{
				SetRecent(e.FilePath);
				SetTimedStatus(Status.ChildSaved);
			}
			SetToolControls();
		}

		private void mdi_child_FormClosed(object sender, FormClosedEventArgs e)
		{
			RemoveFromWorkspace((Terminal)sender);
			SetTimedStatus(Status.ChildClosed);
			SetToolControls();
		}

		#endregion

		#region Status
		//******************************************************************************************
		// Status
		//******************************************************************************************

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

		#region Chrono
		//******************************************************************************************
		// Chrono
		//******************************************************************************************

		private void chronometer_Main_Tick(object sender, EventArgs e)
		{
			toolStripStatusLabel_MainStatus_Chrono.Text = chronometer_Main.ToString();
		}

		#endregion
	}
}
