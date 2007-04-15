using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

using HSR.Utilities.Settings;
using HSR.YAT.Settings.Application;
using HSR.YAT.Settings.Document;
using HSR.YAT.Gui.Settings;

namespace HSR.YAT.Gui.Forms
{
	/// <summary>
	/// Main form, provides setup dialogs and hosts terminal forms (MDI forms)
	/// </summary>
	public partial class Main : Form
	{
		//------------------------------------------------------------------------------------------
		// Attributes
		//------------------------------------------------------------------------------------------

		// startup
		private bool _isStartingUp = true;

		// args
		private string[] _commandLineArgs;

		// MDI
		private const string _TerminalText = "Terminal";
		private long _terminalId = 0;
		private const int _PathLength = 80;

		// recent files
		private List<ToolStripMenuItem> _menuItems_recents;

		// status
		private const string _DefaultStatusText = "Ready";
		private const int _TimedStatusInterval = 2000;

		//------------------------------------------------------------------------------------------
		// Constructor
		//------------------------------------------------------------------------------------------

		// Important note to ensure proper z-order of this form:
		// - With visual designer, proceed with the following order
		//     1. Add control ToolStripPanel to the toolbox
		//     2. Place three panels onto the form
		//     3. Place the toolstrip into "toolStripPanel_Top"
		//     4. Dock "toolStripPanel_Left" and "toolStripPanel_Right"
		//       (Dock "toolStripPanel_Bottom")
		//     5. Dock "toolStripPanel_Top"
		//     6. Dock "statusStrip_Main" to bottom
		//     7. Dock "menuStrip_Main" to top
		// - In source code, proceed according to the MenuStrip Class example in the MSDN

		public Main(string[] args, bool applicationSettingsLoaded)
		{
			_commandLineArgs = args;

			InitializeComponent();
			Initialize(applicationSettingsLoaded);
		}

		private void Initialize(bool applicationSettingsLoaded)
		{
			InitializeRecents();

			// form title
			string text = Application.ProductName;
			if (VersionInfo.HasProductNamePostFix)
				text += VersionInfo.ProductNamePostFix;
			Text = text;

			if (applicationSettingsLoaded)
			{
				StartPosition = FormStartPosition.Manual;
				ApplyWindowSettings();
			}
			SetToolControls();
		}

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
					success = OpenTerminalFromFile(_commandLineArgs[0]);
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
				SaveWindowSettings();
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

		private void Main_Closing(object sender, CancelEventArgs e)
		{
			if (!e.Cancel)
				SetFixedStatusText("Closing terminals...");
			else
				ResetStatusText();
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
			toolStripMenuItem_MainMenu_File_CloseAll.Enabled = childReady;
			toolStripMenuItem_MainMenu_File_SaveAll.Enabled = childReady;
			
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
			CloseAllFiles();
		}

		private void toolStripMenuItem_MainMenu_File_SaveAll_Click(object sender, EventArgs e)
		{
			SaveAllFiles();
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
			SaveFile();
		}

		private void toolStripButton_MainTool_Terminal_Open_Click(object sender, EventArgs e)
		{
			OpenTerminal();
		}

		private void toolStripButton_MainTool_Terminal_Close_Click(object sender, EventArgs e)
		{
			CloseTerminal();
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
		public void SetRecent(string recentFile)
		{
			ApplicationSettings.LocalUser.RecentFiles.FilePaths.ReplaceOrInsertAtBeginAndRemoveMostRecentIfNecessary(recentFile);
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

		private void SaveRecents()
		{
			if (ApplicationSettings.LocalUser.RecentFiles.HaveChanged)
				ApplicationSettings.Save();
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
			WindowState = ApplicationSettings.LocalUser.MainWindow.WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				Location = ApplicationSettings.LocalUser.MainWindow.Location;
				Size     = ApplicationSettings.LocalUser.MainWindow.Size;
			}
			ResumeLayout();
		}

		private void SaveWindowSettings()
		{
			ApplicationSettings.LocalUser.MainWindow.WindowState = WindowState;
			if (WindowState == FormWindowState.Normal)
			{
				ApplicationSettings.LocalUser.MainWindow.Location = Location;
				ApplicationSettings.LocalUser.MainWindow.Size     = Size;
			}
			if (ApplicationSettings.LocalUser.MainWindow.HaveChanged)
			{
				ApplicationSettings.Save();
			}
		}

		#endregion

		#region MDI Children
		//******************************************************************************************
		// MDI Children
		//******************************************************************************************

		private void ShowNewTerminalDialog()
		{
			SetFixedStatusText("New terminal...");
			Gui.Forms.NewTerminal f = new Gui.Forms.NewTerminal(ApplicationSettings.LocalUser.NewTerminal);
			if (f.ShowDialog(this) == DialogResult.OK)
			{
				Refresh();

				ApplicationSettings.LocalUser.NewTerminal = f.NewTerminalSettingsResult;
				if (ApplicationSettings.LocalUser.NewTerminal.HaveChanged)
					ApplicationSettings.Save();

				SetFixedStatusText("Creating new terminal...");

				DocumentSettingsHandler<DocumentSettings> sh = new DocumentSettingsHandler<DocumentSettings>(f.DocumentSettingsResult);
				Gui.Forms.Terminal terminal = new Gui.Forms.Terminal(sh);
				_terminalId++;
				terminal.Id = _TerminalText + _terminalId;
				terminal.MdiParent = this;
				terminal.TerminalChanged += new EventHandler(mdi_child_TerminalChanged);
				terminal.TerminalSaved += new EventHandler<TerminalSavedEventArgs>(mdi_child_TerminalSaved);
				terminal.Show();

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
			ofd.Filter = ApplicationSettings.Extensions.TerminalFilesFilter;
			ofd.DefaultExt = ApplicationSettings.Extensions.TerminalFilesDefault;
			ofd.InitialDirectory = ApplicationSettings.LocalUser.Path.SettingsFilesPath;
			if ((ofd.ShowDialog(this) == DialogResult.OK) && (ofd.FileName != string.Empty))
			{
				Refresh();

				ApplicationSettings.LocalUser.Path.SettingsFilesPath = System.IO.Path.GetDirectoryName(ofd.FileName);
				OpenTerminalFromFile(ofd.FileName);
			}
			else
			{
				ResetStatusText();
			}
		}

		private void OpenRecent(int userIndex)
		{
			SetFixedStatusText("Opening recent terminal...");
			OpenTerminalFromFile(ApplicationSettings.LocalUser.RecentFiles.FilePaths[userIndex - 1].Item);
		}

		private bool OpenTerminalFromFile(string file)
		{
			try
			{
				DocumentSettingsHandler<DocumentSettings> sh = new DocumentSettingsHandler<DocumentSettings>();
				sh.SettingsFilePath = file;
				sh.Load();
				Gui.Forms.Terminal terminal = new Gui.Forms.Terminal(sh);
				terminal.IdFromFile = file;
				terminal.MdiParent = this;
				terminal.TerminalChanged += new EventHandler(mdi_child_TerminalChanged);
				terminal.TerminalSaved += new EventHandler<TerminalSavedEventArgs>(mdi_child_TerminalSaved);
				terminal.Show();
				SetRecent(file);

				SetTimedStatusText("Terminal opened");
				return (true);
			}
			catch (System.Xml.XmlException ex)
			{
				SetFixedStatusText("Error opening terminal!");

				MessageBox.Show
					(
					this,
					"Unable to open file" + Environment.NewLine + file + Environment.NewLine + Environment.NewLine +
					"XML error message: " + ex.Message + Environment.NewLine + Environment.NewLine +
					"File error message: " + ex.InnerException.Message,
					"File Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Stop
					);

				SetTimedStatusText("No terminal opened!");
				return (false);
			}
		}

		private void CloseFile()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestCloseFile();
		}

		private void CloseAllFiles()
		{
			foreach (Form f in MdiChildren)
				((Gui.Forms.Terminal)f).RequestCloseFile();
		}

		private void SaveFile()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestSaveFile();
		}

		private void SaveAllFiles()
		{
			foreach (Form f in MdiChildren)
				((Gui.Forms.Terminal)f).RequestSaveFile();
		}

		private void OpenTerminal()
		{
			((Gui.Forms.Terminal)ActiveMdiChild).RequestOpenTerminal();
		}

		private void CloseTerminal()
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
			SetRecent(e.FilePath);
			SetTimedStatus(Status.ChildSaved);
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
